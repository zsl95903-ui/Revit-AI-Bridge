using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;

namespace RevitAi.Engine.Core;

/// <summary>
/// 单个工具的"自研实现"契约。放在 Core（不依赖 Revit）里，
/// 这样实现工程可以是不带 Revit 引用的 net10.0 库，冒烟测试也能直接跑真实现。
/// </summary>
public interface IToolImplementation
{
    Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// 工具名 → 实现的注册表。名称与厂商工具名严格一致（见 tool-index.json），便于 1:1 替换。
/// 生成出来的工具类（RevitToolSet.Tools.*）执行时先查这里。
/// </summary>
public static class ToolImplementationRegistry
{
    private static readonly Dictionary<string, IToolImplementation> Implementations = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object Sync = new();

    public static int Count
    {
        get
        {
            lock (Sync)
            {
                return Implementations.Count;
            }
        }
    }

    public static void Register(string toolName, IToolImplementation implementation)
    {
        if (string.IsNullOrWhiteSpace(toolName))
        {
            throw new ArgumentException("工具名不能为空。", nameof(toolName));
        }

        ArgumentNullException.ThrowIfNull(implementation);

        lock (Sync)
        {
            Implementations[toolName] = implementation;
        }
    }

    public static bool TryGet(string toolName, out IToolImplementation implementation)
    {
        lock (Sync)
        {
            return Implementations.TryGetValue(toolName, out implementation!);
        }
    }

    public static IReadOnlyCollection<string> ImplementedNames
    {
        get
        {
            lock (Sync)
            {
                return Implementations.Keys.OrderBy(name => name, StringComparer.Ordinal).ToArray();
            }
        }
    }

    public static void Clear()
    {
        lock (Sync)
        {
            Implementations.Clear();
        }
    }
}
