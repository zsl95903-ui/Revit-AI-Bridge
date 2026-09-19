using Autodesk.Revit.UI;

namespace RevitAiBatch;

public sealed class RevitAiBatchApplication : IExternalApplication
{
    private BridgeServer? _server;
    private ToolDispatcher? _dispatcher;

    public Result OnStartup(UIControlledApplication application)
    {
        try
        {
            _dispatcher = new ToolDispatcher();
            _server = new BridgeServer(_dispatcher);
            RevitAiBatchApplicationRuntime.Server = _server;
            CodexBatchHost.Register(_dispatcher);
            _server.Start();

            const string tabName = "Codex Revit";
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch
            {
                // Tab already exists.
            }

            var panel = application.CreateRibbonPanel(tabName, "Batch Bridge");
            var button = new PushButtonData(
                "RevitAiBatchStatus",
                "Bridge\nStatus",
                typeof(RevitAiBatchApplication).Assembly.Location,
                typeof(ShowBridgeStatusCommand).FullName);
            button.ToolTip = "Show the Codex Revit Batch bridge status.";
            panel.AddItem(button);

            BridgeLog.Write("RevitAI Codex Batch add-in started.");
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            BridgeLog.Write($"Startup failed: {ex}");
            return Result.Failed;
        }
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        try
        {
            if (_server is not null)
            {
                _server.DisposeAsync().AsTask().GetAwaiter().GetResult();
                _server = null;
                RevitAiBatchApplicationRuntime.Server = null;
            }

            if (_dispatcher is not null)
            {
                CodexBatchHost.Unregister(_dispatcher);
                _dispatcher = null;
            }
            BridgeLog.Write("RevitAI Codex Batch add-in stopped.");
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            BridgeLog.Write($"Shutdown failed: {ex}");
            return Result.Failed;
        }
    }
}

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public sealed class ShowBridgeStatusCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        Autodesk.Revit.DB.ElementSet elements)
    {
        var server = RevitAiBatchApplicationRuntime.Server;
        TaskDialog.Show(
            "Codex Revit Batch",
            server is null
                ? "Bridge is not running."
                : $"Bridge is running.\n\nPipe: {server.PipeName}\nPending: {server.Invoker.PendingCount}\n\nCodex drawing is handled through apply_drawing_plan. The original Revit AI console remains available on the Revit AI tab.");
        return Result.Succeeded;
    }
}

internal static class RevitAiBatchApplicationRuntime
{
    public static BridgeServer? Server { get; set; }
}
