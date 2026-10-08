namespace RevitAi.Engine.Abstractions;

/// <summary>一次工具调用的入参（对应厂商 ToolCallInfo）。</summary>
public sealed class ToolCallInfo
{
    public ToolCallInfo(string toolName, IDictionary<string, object?>? parameters = null, string? callId = null)
    {
        ToolName = toolName ?? throw new ArgumentNullException(nameof(toolName));
        Parameters = parameters ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        CallId = callId;
    }

    public string ToolName { get; }

    public IDictionary<string, object?> Parameters { get; }

    public string? CallId { get; }
}

/// <summary>
/// 工具注册表。实现见 RevitAi.Engine.Core.AIToolRegistry（反射发现 + 属性式服务注入 + tools JSON 生成）。
/// </summary>
public interface IAIToolRegistry
{
    void RegisterTool(IAITool tool);

    IAITool? GetTool(string toolName);

    AIToolAttribute? GetAttribute(string toolName);

    IReadOnlyCollection<IAITool> GetAllTools();

    bool ContainsTool(string toolName);

    /// <summary>生成下发给模型的 tools 数组 JSON（type=function 形式）。</summary>
    string GetToolsDefinitionForAI();
}

/// <summary>
/// Revit 宿主抽象：把"需要回主线程执行"的调用封送给 Revit（实现见 RevitAi.Engine.Revit.RevitToolHost，
/// 用 ExternalEvent + IExternalEventHandler + Transaction 实现）。
/// Core 只依赖本接口，不引用任何 Revit 类型——与厂商"Core 用反射 Raise"的思路一致。
/// </summary>
public interface IRevitToolHost
{
    bool IsAvailable { get; }

    Task<AIToolResult> ExecuteAsync(
        AIToolContext context,
        bool requiresTransaction,
        Func<AIToolContext, Task<AIToolResult>> action);
}

/// <summary>单位换算契约（mm / ft / m 三种口径互转）。</summary>
public interface IUnitConverter
{
    /// <summary>Revit 内部长度单位为英尺；1 英尺 = 304.8 毫米。</summary>
    double MillimetersPerFoot { get; }

    double MmToFeet(double millimeters);

    double FeetToMm(double feet);

    double MetersToFeet(double meters);

    double FeetToMeters(double feet);

    double Round(double value, int digits);
}
