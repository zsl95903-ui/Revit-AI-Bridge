using System.Text.Json;
using Autodesk.Revit.UI;

namespace RevitAiBatch;

public sealed record CodexBatchToolDescriptor(
    string Name,
    string Description,
    bool Mutating,
    string InputSchemaJson);

public static class CodexBatchHost
{
    private static readonly object Sync = new();
    private static ToolDispatcher? _dispatcher;

    public static bool IsReady
    {
        get
        {
            lock (Sync)
            {
                return _dispatcher is not null;
            }
        }
    }

    internal static void Register(ToolDispatcher dispatcher)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        lock (Sync)
        {
            _dispatcher = dispatcher;
        }
    }

    internal static void Unregister(ToolDispatcher dispatcher)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        lock (Sync)
        {
            if (ReferenceEquals(_dispatcher, dispatcher))
            {
                _dispatcher = null;
            }
        }
    }

    public static IReadOnlyList<CodexBatchToolDescriptor> GetTools()
    {
        lock (Sync)
        {
            return _dispatcher?.DescribeTools() ?? [];
        }
    }

    public static bool IsMutating(string tool)
    {
        lock (Sync)
        {
            return _dispatcher?.IsMutating(tool) ?? false;
        }
    }

    public static object? Execute(
        UIApplication application,
        string tool,
        JsonElement arguments)
    {
        ArgumentNullException.ThrowIfNull(application);
        if (string.IsNullOrWhiteSpace(tool))
        {
            throw new ArgumentException("A Codex batch tool name is required.", nameof(tool));
        }

        ToolDispatcher dispatcher;
        lock (Sync)
        {
            dispatcher = _dispatcher
                ?? throw new InvalidOperationException("The Codex batch dispatcher is not running.");
        }

        var command = new PendingCommand
        {
            Tool = tool,
            Arguments = arguments.ValueKind == JsonValueKind.Undefined
                ? Json.EmptyObject()
                : arguments.Clone(),
            Completion = new TaskCompletionSource<object?>(
                TaskCreationOptions.RunContinuationsAsynchronously),
        };
        return dispatcher.ExecuteOnRevitThread(application, command);
    }
}
