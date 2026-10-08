using RevitAi.Engine.Abstractions;

namespace RevitAi.Engine.Core;

/// <summary>
/// 调用处理器：按 [AITool] 的三个布尔决定"直接执行"还是"封送回 Revit 主线程"。
///
/// 与厂商 AIToolInvocationHandler 的路由规则一致：
///   * RequiresModification == false 且 RequiresActiveDocument == false → 当前线程直接执行（纯只读/网络类）；
///   * 其余（含"只读但需要文档"，因为 RequiresActiveDocument 默认 true）→ 交给 IRevitToolHost 回主线程。
/// 事务是否开启由宿主依据 RequiresTransaction 决定。
/// </summary>
public sealed class AIToolInvocationHandler
{
    private readonly IAIToolRegistry _registry;
    private readonly IRevitToolHost _host;
    private readonly IAIToolDataCache? _dataCache;

    public AIToolInvocationHandler(IAIToolRegistry registry, IRevitToolHost host, IAIToolDataCache? dataCache = null)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _dataCache = dataCache;
    }

    public async Task<AIToolResult> HandleToolCallAsync(
        ToolCallInfo toolCall,
        AIToolContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(toolCall);
        ArgumentNullException.ThrowIfNull(context);

        var tool = _registry.GetTool(toolCall.ToolName)
            ?? throw new InvalidOperationException($"未注册的工具：{toolCall.ToolName}");

        var attribute = _registry.GetAttribute(toolCall.ToolName)
            ?? throw new InvalidOperationException($"工具 {toolCall.ToolName} 缺少 [AITool] 元数据。");

        context.Parameters = toolCall.Parameters;
        context.DataCache ??= _dataCache;
        context.RevitAdapter ??= (_registry as AIToolRegistry)?.Adapter;

        if (toolCall.Parameters.TryGetValue("sessionId", out var sessionId) && sessionId is not null)
        {
            context.SessionId = sessionId.ToString();
        }

        var needsRevitThread = attribute.RequiresModification || attribute.RequiresActiveDocument;

        if (!needsRevitThread)
        {
            try
            {
                return await tool.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return AIToolResult.Fail($"执行工具 {toolCall.ToolName} 时发生异常：{GetMessage(ex)}");
            }
        }

        if (!_host.IsAvailable)
        {
            return AIToolResult.Fail($"工具 {toolCall.ToolName} 需要 Revit 主线程，但宿主不可用。");
        }

        return await _host
            .ExecuteAsync(context, attribute.RequiresTransaction, ctx => tool.ExecuteAsync(ctx, cancellationToken))
            .ConfigureAwait(false);
    }

    private static string GetMessage(Exception ex)
        => ex.GetBaseException().Message;
}
