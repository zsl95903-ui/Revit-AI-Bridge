using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Core;
using RevitAi.Engine.Revit;

namespace RevitAi.Engine.Addin;

/// <summary>
/// 引擎自检命令。流程刻意走"后台线程 → 调用处理器 → Revit ExternalEvent → 工具在 Revit 主线程执行"，
/// 因为 IExternalCommand 本身就在 Revit 主线程上，若同步等待 ExternalEvent 会死锁。
/// 自检内容：
///   1) 反射发现工具并导出 tools.json（下发模型用的 tools 数组）；
///   2) get_all_levels（只读；经宿主回主线程执行但不建事务）；
///   3) upsert_level dry_run（只读预演，不修改模型）；
///   4) create_level（写入；经宿主开事务提交）；
///   5) 结果写 %LOCALAPPDATA%\RevitAiEngine\selftest.json，并用 TaskDialog 在主线程汇报。
/// </summary>
[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public sealed class SelfTestCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            var uiApplication = commandData.Application;
            var adapter = new RevitAdapter(uiApplication);
            var document = adapter.GetActiveDocument();
            if (document is null)
            {
                TaskDialog.Show("Revit AI Engine", "没有活动文档，请先打开一个模型再运行自检。");
                return Result.Cancelled;
            }

            var cache = new SessionAIToolDataCache();
            var units = new UnitService();
            var registry = AIToolRegistry.Instance;
            registry.SetRevitAdapter(adapter);
            registry.SetDataCache(cache);
            registry.SetUnitService(units);

            // 工具集：RevitToolSet 里由生成器产出的 121 个工具类（厂商 120 + 自研扩展），
            // 先把已重写的实现注册进实现表，其余工具会走回退引擎或返回"尚未实现"。
            RevitAi.Engine.Revit.Implementations.RevitToolSetImplementations.RegisterAll();
            RevitToolSet.Implementations.ReadOnlyImplementations.RegisterAll();
            RevitToolSet.Implementations.ReadOnlyImplementations2.RegisterAll();
            RevitToolSet.Implementations.ReadOnlyImplementations3.RegisterAll();
            RevitToolSet.Implementations.WriteImplementations.RegisterAll();
            RevitToolSet.Implementations.WriteImplementations2.RegisterAll();
            RevitToolSet.Implementations.WriteImplementations3.RegisterAll();
            RevitToolSet.Implementations.ParameterImplementations.RegisterAll();
            RevitToolSet.Implementations.MaterialImplementations.RegisterAll();
            RevitToolSet.Implementations.FamilyElementImplementations.RegisterAll();
            RevitToolSet.Implementations.ProjectParameterImplementations.RegisterAll();
            RevitToolSet.Implementations.ModelingImplementations.RegisterAll();
            RevitToolSet.Implementations.ModelingImplementations2.RegisterAll();
            RevitToolSet.Implementations.ElementOpsImplementations.RegisterAll();
            RevitToolSet.Implementations.QueryInteractionImplementations.RegisterAll();
            RevitToolSet.Implementations.DomainImplementations.RegisterAll();
            RevitToolSet.Implementations.DomainImplementations2.RegisterAll();
            RevitToolSet.Implementations.DomainImplementations3.RegisterAll();
            RevitToolSet.Implementations.CodeToolImplementations.RegisterAll();
            registry.SetAdapterAssembly(typeof(RevitToolSet.GeneratedToolBase).Assembly);
            registry.EnsureToolsDiscovered();

            var host = new RevitToolHost();
            var handler = new AIToolInvocationHandler(registry, host, cache);

            var outputDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RevitAiEngine");
            Directory.CreateDirectory(outputDirectory);
            File.WriteAllText(Path.Combine(outputDirectory, "tools.json"), registry.GetToolsDefinitionForAI(), Encoding.UTF8);

            // Fire-and-forget：命令立即返回，避免阻塞 Revit 主线程导致 ExternalEvent 无法被处理。
            _ = Task.Run(async () =>
            {
                var report = new StringBuilder();
                try
                {
                    var toolNames = string.Join(", ", registry.GetAllTools().Select(tool => tool.Name));
                    report.AppendLine($"发现工具 {registry.GetAllTools().Count} 个：{toolNames}");
                    report.AppendLine();

                    var context = new AIToolContext
                    {
                        RevitAdapter = adapter,
                        Document = document,
                        DataCache = cache,
                        UnitService = units,
                    };

                    var levels = await handler.HandleToolCallAsync(
                        new ToolCallInfo("get_all_levels", ToolParameterParser.Parse("{}")), context);
                    report.AppendLine($"[get_all_levels] success={levels.Success} message={levels.Message}");

                    var dryRun = await handler.HandleToolCallAsync(
                        new ToolCallInfo("upsert_level", ToolParameterParser.Parse("""{"name":"引擎自检标高","elevation":12.345,"dry_run":true}""")), context);
                    report.AppendLine($"[upsert_level dry_run] success={dryRun.Success} message={dryRun.Message}");

                    var create = await handler.HandleToolCallAsync(
                        new ToolCallInfo("create_level", ToolParameterParser.Parse("""{"elevation":12.345,"levelName":"引擎自检标高"}""")), context);
                    report.AppendLine($"[create_level] success={create.Success} message={create.Message}");

                    var json = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        toolCount = registry.GetAllTools().Count,
                        get_all_levels = new { levels.Success, levels.Message },
                        upsert_level_dry_run = new { dryRun.Success, dryRun.Message },
                        create_level = new { create.Success, create.Message },
                        report = report.ToString(),
                    }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(Path.Combine(outputDirectory, "selftest.json"), json, Encoding.UTF8);

                    // 汇报必须回到 Revit 主线程，这里再借一次 ExternalEvent。
                    await host.ExecuteAsync(new AIToolContext(), false, _ =>
                    {
                        TaskDialog.Show("Revit AI Engine 自检", report.ToString());
                        return Task.FromResult(AIToolResult.Ok("reported"));
                    });
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Path.Combine(outputDirectory, "selftest-error.log"), ex + Environment.NewLine, Encoding.UTF8);
                }
            });

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.ToString();
            return Result.Failed;
        }
    }
}

internal static class EngineLog
{
    private static readonly object Sync = new();

    public static void Write(string text)
    {
        try
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RevitAiEngine");
            Directory.CreateDirectory(directory);
            lock (Sync)
            {
                File.AppendAllText(
                    Path.Combine(directory, "engine.log"),
                    $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}] {text}{Environment.NewLine}",
                    Encoding.UTF8);
            }
        }
        catch
        {
            // 日志失败不能影响插件运行。
        }
    }
}
