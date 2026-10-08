using System.Collections.Concurrent;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAi.Engine.Abstractions;

namespace RevitAi.Engine.Revit;

/// <summary>
/// Revit 宿主：把需要回主线程的工具调用封送给 Revit —— ExternalEvent + IExternalEventHandler + Transaction。
///
/// 与厂商实现的对应关系：
///   * 厂商 Core 侧 RevitExternalEventHandler 用"反射 Raise"把请求交给 Revit（因为 Core 不引用 Revit）；
///     本 PoC 的 Core 只依赖 IRevitToolHost 接口，反射这一步由本项目直接调用 Revit API 完成。
///   * 事务边界、Regenerate、按 AIToolResult.Success 决定 Commit / Rollback 与厂商一致；
///     FailureHandlingOptions 预处理器用于压掉"连接但不相交 / 实例原点 / 无法连接"这类警告弹窗。
///   参考：反编译重建源码 Revit\AS\Tools\Revit\AI\AIToolExternalEventHandler.cs、Core\AS\Tools\Core\AI\RevitExternalEventHandler.cs。
/// </summary>
public sealed class RevitToolHost : IRevitToolHost, IExternalEventHandler
{
    private const string DefaultTransactionName = "AI Tool Execution";

    private sealed record Request(
        Func<AIToolContext, Task<AIToolResult>> Action,
        AIToolContext Context,
        TaskCompletionSource<AIToolResult> Completion,
        bool RequiresTransaction);

    private readonly ConcurrentQueue<Request> _queue = new();
    private readonly ExternalEvent _externalEvent;

    public RevitToolHost()
    {
        _externalEvent = ExternalEvent.Create(this);
    }

    public bool IsAvailable => true;

    public ExternalEvent ExternalEvent => _externalEvent;

    public Task<AIToolResult> ExecuteAsync(
        AIToolContext context,
        bool requiresTransaction,
        Func<AIToolContext, Task<AIToolResult>> action)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(action);

        var completion = new TaskCompletionSource<AIToolResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        _queue.Enqueue(new Request(action, context, completion, requiresTransaction));

        var status = _externalEvent.Raise();
        if (status is not (ExternalEventRequest.Accepted or ExternalEventRequest.Pending))
        {
            completion.TrySetResult(AIToolResult.Fail($"无法触发 Revit ExternalEvent：{status}"));
        }

        return completion.Task;
    }

    public void Execute(UIApplication app)
    {
        while (_queue.TryDequeue(out var request))
        {
            ExecuteRequest(app, request);
        }
    }

    public string GetName() => "RevitAi.Engine.ToolHost";

    private static void ExecuteRequest(UIApplication app, Request request)
    {
        var document = app.ActiveUIDocument?.Document ?? request.Context.Document as Document;
        if (document is null)
        {
            request.Completion.TrySetResult(AIToolResult.Fail("没有活动文档。"));
            return;
        }

        request.Context.Document = document;
        Transaction? transaction = null;

        try
        {
            if (request.RequiresTransaction)
            {
                transaction = new Transaction(document, DefaultTransactionName);
                var options = transaction.GetFailureHandlingOptions();
                options.SetFailuresPreprocessor(new WarningSuppressor());
                transaction.SetFailureHandlingOptions(options);
                transaction.Start();
                request.Context.Transaction = transaction;
            }

            var result = request.Action(request.Context).GetAwaiter().GetResult();

            if (transaction is not null)
            {
                if (result.Success)
                {
                    document.Regenerate();
                    var status = transaction.Commit();
                    if (status != TransactionStatus.Committed)
                    {
                        result = AIToolResult.Fail($"事务未能提交（{status}）：{result.Message}");
                    }
                }
                else
                {
                    transaction.RollBack();
                }
            }

            request.Completion.TrySetResult(result);
        }
        catch (Exception ex)
        {
            TryRollback(transaction);
            request.Completion.TrySetResult(AIToolResult.Fail($"执行工具时发生异常：{ex.GetBaseException().Message}"));
        }
        finally
        {
            request.Context.Transaction = null;
        }
    }

    private static void TryRollback(Transaction? transaction)
    {
        try
        {
            if (transaction is not null && transaction.GetStatus() == TransactionStatus.Started)
            {
                transaction.RollBack();
            }
        }
        catch
        {
            // 回滚失败不能覆盖原始异常。
        }
    }

    /// <summary>压掉事务内的警告，避免 Revit 弹窗打断 Agent 循环（厂商在同一位置做同样的事）。</summary>
    private sealed class WarningSuppressor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            foreach (var failure in failuresAccessor.GetFailureMessages())
            {
                if (failure.GetSeverity() == FailureSeverity.Warning)
                {
                    failuresAccessor.DeleteWarning(failure);
                }
            }

            return FailureProcessingResult.Continue;
        }
    }
}
