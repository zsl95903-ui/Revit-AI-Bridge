using System.Reflection;
using Autodesk.Revit.UI;
using RevitAi.Engine.Core;

namespace RevitAi.Engine.Addin;

/// <summary>
/// 插件入口：注册功能区面板与"引擎自检"命令。
/// 与厂商 AS.Tools.Loader / RevitAi.Addin 的职责相同（IExternalApplication + Ribbon），
/// 但不含任何 UI 框架依赖，便于先验证引擎本身。
/// </summary>
public sealed class EngineApplication : IExternalApplication
{
    private const string PanelName = "Revit AI Engine";

    public Result OnStartup(UIControlledApplication application)
    {
        AIToolRegistry.Log = message => EngineLog.Write(message);

        try
        {
            var panel = application.CreateRibbonPanel(Tab.AddIns, PanelName);
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var button = new PushButtonData(
                "RevitAi.Engine.SelfTest",
                "引擎自检",
                assemblyPath,
                typeof(SelfTestCommand).FullName)
            {
                ToolTip = "发现引擎工具、跑一次只读与写入自检，并导出 tools.json。",
                LongDescription = "Revit AI Engine PoC：验证注册表发现、ExternalEvent 封送、事务边界与工具契约。",
            };

            panel.AddItem(button);
            EngineLog.Write("EngineApplication 启动完成。");
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            EngineLog.Write("EngineApplication 启动失败：" + ex);
            return Result.Failed;
        }
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        EngineLog.Write("EngineApplication 关闭。");
        return Result.Succeeded;
    }
}
