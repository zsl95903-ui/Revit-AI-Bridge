using System.Collections.Concurrent;
using System.Text.Json;
using Autodesk.Revit.UI;

namespace ReVitAI.Bridge;

internal sealed class ExternalEventInvoker : IExternalEventHandler
{
    private readonly ToolDispatcher _dispatcher;
    private readonly ConcurrentQueue<PendingCommand> _queue = new();
    private readonly ExternalEvent _externalEvent;

    public ExternalEventInvoker(ToolDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _externalEvent = ExternalEvent.Create(this);
    }

    public int PendingCount => _queue.Count;

    public Task<object?> InvokeAsync(string tool, JsonElement arguments)
    {
        var completion = new TaskCompletionSource<object?>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        _queue.Enqueue(new PendingCommand
        {
            Tool = tool,
            Arguments = arguments.Clone(),
            Completion = completion,
        });

        var request = _externalEvent.Raise();
        if (request is ExternalEventRequest.Denied or ExternalEventRequest.TimedOut)
        {
            while (_queue.TryDequeue(out var pending))
            {
                pending.Completion.TrySetException(
                    new InvalidOperationException($"ExternalEvent raise failed: {request}."));
            }
        }

        return completion.Task;
    }

    public void Execute(UIApplication app)
    {
        while (_queue.TryDequeue(out var pending))
        {
            try
            {
                var result = _dispatcher.ExecuteOnRevitThread(app, pending);
                pending.Completion.TrySetResult(result);
            }
            catch (Exception ex)
            {
                pending.Completion.TrySetException(ex);
            }
        }
    }

    public string GetName() => "ReVitAI Bridge External Event";
}
