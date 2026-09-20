using Autodesk.Revit.UI;

namespace ReVitAI.Bridge;

public sealed class ReVitAIBridgeApplication : IExternalApplication
{
    private readonly bool _createRibbon;
    private BridgeServer? _server;
    private ToolDispatcher? _dispatcher;

    public ReVitAIBridgeApplication(bool createRibbon = true)
    {
        _createRibbon = createRibbon;
    }

    public Result OnStartup(UIControlledApplication application)
    {
        try
        {
            _dispatcher = new ToolDispatcher();
            _server = new BridgeServer(_dispatcher);
            ReVitAIBridgeApplicationRuntime.Server = _server;
            ReVitAIBatchHost.Register(_dispatcher);
            _server.Start();

            if (_createRibbon)
            {
                const string tabName = "ReVitAI";
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
                    "ReVitAIBridgeStatus",
                    "Bridge\nStatus",
                    typeof(ReVitAIBridgeApplication).Assembly.Location,
                    typeof(ShowBridgeStatusCommand).FullName);
                button.ToolTip = "Show the ReVitAI Bridge status.";
                panel.AddItem(button);
            }

            BridgeLog.Write(
                _createRibbon
                    ? "ReVitAI Bridge add-in started."
                    : "ReVitAI Bridge add-in started headless.");
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
                ReVitAIBridgeApplicationRuntime.Server = null;
            }

            if (_dispatcher is not null)
            {
                ReVitAIBatchHost.Unregister(_dispatcher);
                _dispatcher = null;
            }
            BridgeLog.Write("ReVitAI Bridge add-in stopped.");
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
        var server = ReVitAIBridgeApplicationRuntime.Server;
        TaskDialog.Show(
            "ReVitAI Bridge",
            server is null
                ? "Bridge is not running."
                : $"Bridge is running.\n\nPipe: {server.PipeName}\nPending: {server.Invoker.PendingCount}\n\nStructured drawing plans are handled through apply_drawing_plan.");
        return Result.Succeeded;
    }
}

internal static class ReVitAIBridgeApplicationRuntime
{
    public static BridgeServer? Server { get; set; }
}
