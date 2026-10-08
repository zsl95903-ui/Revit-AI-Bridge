namespace RevitAi.Engine.Abstractions;

/// <summary>
/// AI 工具契约。与厂商 IAITool 一致：Name/Description/Category/ParametersSchema + ExecuteAsync。
/// </summary>
public interface IAITool
{
    string Name { get; }

    string Description { get; }

    string Category { get; }

    /// <summary>JSON Schema 文本，直接进入下发给模型的 tools 数组。</summary>
    string ParametersSchema { get; }

    Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default);
}
