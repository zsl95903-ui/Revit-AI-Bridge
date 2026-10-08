using System.Reflection;
using System.Text;
using System.Text.Json;
using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Core;

/// <summary>
/// 工具注册表：单例 + 反射发现 + 按类型 switch 的服务注入。
///
/// 架构完全对齐厂商 AIToolRegistry：
///   * 扫描程序集，取 class / 非抽象 / 实现 IAITool 且带 [AITool] 的类型；
///   * 实例化时逐个构造参数按适配器属性做类型 switch（不是 DI 容器）；
///   * GetToolsDefinitionForAI() 生成下发模型的 tools 数组，工具清单不进 system prompt。
/// 参考：反编译重建源码 Core\AS\Tools\Core\AI\AIToolRegistry.cs。
/// </summary>
public sealed class AIToolRegistry : IAIToolRegistry
{
    private static readonly Lazy<AIToolRegistry> LazyInstance = new(() => new AIToolRegistry());

    private readonly Dictionary<string, IAITool> _tools = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, AIToolAttribute> _attributes = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _sync = new();

    private IRevitAdapter? _adapter;
    private IAIToolDataCache? _dataCache;
    private IUnitConverter? _unitService;
    private Assembly? _adapterAssembly;

    public static AIToolRegistry Instance => LazyInstance.Value;

    /// <summary>当前适配器（供调用处理器注入到上下文）。</summary>
    public IRevitAdapter? Adapter => _adapter;

    /// <summary>诊断日志出口（默认丢弃）。</summary>
    public static Action<string>? Log { get; set; }

    public void SetRevitAdapter(IRevitAdapter adapter) => _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));

    public void SetDataCache(IAIToolDataCache cache) => _dataCache = cache ?? throw new ArgumentNullException(nameof(cache));

    public void SetUnitService(IUnitConverter unitService) => _unitService = unitService ?? throw new ArgumentNullException(nameof(unitService));

    public void SetAdapterAssembly(Assembly assembly) => _adapterAssembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

    /// <summary>首次调用时按需发现（与厂商 EnsureToolsDiscovered 行为一致）。</summary>
    public void EnsureToolsDiscovered()
    {
        if (_adapterAssembly is null)
        {
            throw new InvalidOperationException("尚未设置工具程序集（SetAdapterAssembly）。");
        }

        DiscoverAndRegisterToolsFromAssembly(_adapterAssembly);
    }

    public void DiscoverAndRegisterToolsFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract || !typeof(IAITool).IsAssignableFrom(type))
            {
                continue;
            }

            var attribute = type.GetCustomAttribute<AIToolAttribute>();
            if (attribute is null)
            {
                Log?.Invoke($"[AIToolRegistry] 类型 {type.FullName} 实现了 IAITool 但未标记 AIToolAttribute，已跳过。");
                continue;
            }

            var instance = CreateToolInstance(type);
            if (instance is null)
            {
                Log?.Invoke($"[AIToolRegistry] 无法实例化工具 {type.FullName}（构造参数中有无法解析的服务）。");
                continue;
            }

            RegisterTool(instance);
        }
    }

    public void RegisterTool(IAITool tool)
    {
        ArgumentNullException.ThrowIfNull(tool);

        var attribute = tool.GetType().GetCustomAttribute<AIToolAttribute>()
            ?? throw new InvalidOperationException($"工具 {tool.GetType().Name} 缺少 [AITool] 特性。");

        lock (_sync)
        {
            _tools[attribute.Name] = tool;
            _attributes[attribute.Name] = attribute;
        }
    }

    public bool UnregisterTool(string toolName)
    {
        lock (_sync)
        {
            _attributes.Remove(toolName);
            return _tools.Remove(toolName);
        }
    }

    public IAITool? GetTool(string toolName)
    {
        lock (_sync)
        {
            return _tools.TryGetValue(toolName, out var tool) ? tool : null;
        }
    }

    public AIToolAttribute? GetAttribute(string toolName)
    {
        lock (_sync)
        {
            return _attributes.TryGetValue(toolName, out var attribute) ? attribute : null;
        }
    }

    public IReadOnlyCollection<IAITool> GetAllTools()
    {
        lock (_sync)
        {
            return _tools.Values.ToArray();
        }
    }

    public bool ContainsTool(string toolName)
    {
        lock (_sync)
        {
            return _tools.ContainsKey(toolName);
        }
    }

    public string GetToolsDefinitionForAI()
    {
        KeyValuePair<string, IAITool>[] snapshot;
        lock (_sync)
        {
            snapshot = _tools.ToArray();
        }

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartArray();
            foreach (var (name, tool) in snapshot)
            {
                writer.WriteStartObject();
                writer.WriteString("type", "function");
                writer.WritePropertyName("function");
                writer.WriteStartObject();
                writer.WriteString("name", name);
                writer.WriteString("description", tool.Description);
                writer.WritePropertyName("parameters");
                using var schema = JsonDocument.Parse(NormalizeSchema(tool.ParametersSchema));
                schema.RootElement.WriteTo(writer);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static string NormalizeSchema(string? schema)
        => string.IsNullOrWhiteSpace(schema) ? """{"type":"object","properties":{}}""" : schema;

    private IAITool? CreateToolInstance(Type type)
    {
        foreach (var constructor in type.GetConstructors().OrderBy(c => c.GetParameters().Length))
        {
            var parameters = constructor.GetParameters();
            var arguments = new object?[parameters.Length];
            var resolvable = true;

            for (var i = 0; i < parameters.Length; i++)
            {
                arguments[i] = ResolveService(parameters[i].ParameterType);
                if (arguments[i] is null && !parameters[i].HasDefaultValue)
                {
                    resolvable = false;
                    break;
                }
            }

            if (!resolvable)
            {
                continue;
            }

            try
            {
                return (IAITool?)constructor.Invoke(arguments);
            }
            catch (Exception ex)
            {
                Log?.Invoke($"[AIToolRegistry] 构造 {type.FullName} 失败：{ex.GetBaseException().Message}");
            }
        }

        return null;
    }

    /// <summary>按类型解析服务——厂商版是对 IRevitAdapter 的 21 个属性做 switch，这里保留同样机制。</summary>
    private object? ResolveService(Type serviceType)
    {
        if (serviceType == typeof(IRevitAdapter))
        {
            return _adapter;
        }

        if (serviceType == typeof(IAIToolDataCache))
        {
            return _dataCache;
        }

        if (serviceType == typeof(IUnitConverter))
        {
            return _unitService;
        }

        var adapter = _adapter;
        if (adapter is null)
        {
            return null;
        }

        if (serviceType == typeof(ILevelService))
        {
            return adapter.LevelService;
        }

        if (serviceType == typeof(IElementService))
        {
            return adapter.ElementService;
        }

        if (serviceType == typeof(IModificationService))
        {
            return adapter.ModificationService;
        }

        return null;
    }
}
