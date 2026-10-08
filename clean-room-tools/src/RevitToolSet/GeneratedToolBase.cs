using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet;

/// <summary>
/// 121 个自研工具的生成类基类。
///
/// <para>
/// 开源版本只执行 <see cref="ToolImplementationRegistry"/> 中的自研实现；
/// 不包含厂商 DLL、反编译代码、厂商回退桥或厂商优先策略。
/// </para>
///
/// <para>
/// 工具类由 <c>tools/ToolSetGenerator</c> 根据
/// <c>contracts/tool-contracts.json</c> 生成到构建临时目录。
/// </para>
/// </summary>
public abstract class GeneratedToolBase : IAITool
{
    protected GeneratedToolBase(IRevitAdapter adapter)
        => Adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));

    protected IRevitAdapter Adapter { get; }

    public abstract string Name { get; }

    public abstract string Category { get; }

    public abstract string Description { get; }

    public abstract string ParametersSchema { get; }

    public virtual Task<AIToolResult> ExecuteAsync(
        AIToolContext context,
        CancellationToken cancellationToken = default)
    {
        if (ToolImplementationRegistry.TryGet(Name, out var implementation))
        {
            return implementation.ExecuteAsync(Adapter, context, cancellationToken);
        }

        return Task.FromResult(AIToolResult.Fail(
            $"工具 {Name} 尚未在 RevitToolSet 中实现（当前已实现 {ToolImplementationRegistry.Count} 个）。"));
    }
}
