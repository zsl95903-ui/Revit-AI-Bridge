using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 2 剩余实现：过滤器创建、标记、明细表创建与 CSV 导出、视图导出（图片/DWG）。
/// 导出类工具按厂商契约是"免事务"（tx=False），但仍需回主线程（RequiresActiveDocument=true）。
/// </summary>
public static class WriteImplementations2
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("create_view_filter", new CreateViewFilterImplementation());
        ToolImplementationRegistry.Register("create_tag", new CreateTagImplementation());
        ToolImplementationRegistry.Register("create_schedule", new CreateScheduleImplementation());
        ToolImplementationRegistry.Register("export_view_images", new ExportViewImagesImplementation());
        ToolImplementationRegistry.Register("export_views_as_dwg", new ExportViewsAsDwgImplementation());
        ToolImplementationRegistry.Register("export_schedule_to_csv", new ExportScheduleToCsvImplementation());
    }
}

internal static class ExportArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    /// <summary>viewIds 优先；其次 cacheId；都没有则返回空列表（由服务回退到活动视图）。</summary>
    public static List<int> ResolveViewIds(AIToolContext context)
    {
        var ids = WriteArgs.Ids(context, "viewIds", "view_ids");
        if (ids.Count > 0)
        {
            return ids;
        }

        var cacheId = WriteArgs.Text(context, "cacheId", "cache_id");
        if (string.IsNullOrWhiteSpace(cacheId))
        {
            return ids;
        }

        var cached = context.GetCachedData<List<Dictionary<string, object?>>>(cacheId);
        if (cached is not null)
        {
            ids.AddRange(ToolArgumentReader.ExtractIds(cached));
        }
        else
        {
            var cachedIds = context.GetCachedData<List<int>>(cacheId);
            if (cachedIds is not null)
            {
                ids.AddRange(cachedIds);
            }
        }

        return ids;
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateViewFilterTool.cs（视图高级操作）
internal sealed class CreateViewFilterImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.FilterCreationService ?? throw new InvalidOperationException("无法获取 FilterCreationService");
        var document = ExportArgs.RequireDocument(adapter, context);

        var filterName = WriteArgs.Text(context, "filterName", "filter_name")
                         ?? throw new InvalidOperationException("必须提供 filterName。");

        var categoryNames = ToolArgumentReader.ReadStrings(
            new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase), "categoryNames");
        var categoryName = categoryNames.FirstOrDefault() ?? WriteArgs.Text(context, "categoryName", "category");
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 categoryNames（至少一个类别）。"));
        }

        var (parameterName, value, rule) = ReadRule(context);
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return Task.FromResult(AIToolResult.Fail("rules 里必须提供 parameterName（或直接用 parameterName/value 简写）。"));
        }

        var filterId = service.CreateFilter(document, filterName, categoryName!, parameterName!, value, rule);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建过滤器 {filterId}：{filterName}（类别 {categoryName}，参数 {parameterName}，规则 {rule ?? "equals"}）",
            new { filterId, filterName, categoryName, parameterName, value, rule = rule ?? "equals" }));
    }

    private static (string? ParameterName, string? Value, string? Rule) ReadRule(AIToolContext context)
    {
        // 简写：parameterName + value + rule
        var shorthandName = WriteArgs.Text(context, "parameterName", "parameter_name");
        if (!string.IsNullOrWhiteSpace(shorthandName))
        {
            return (shorthandName, WriteArgs.Text(context, "value"), WriteArgs.Text(context, "rule", "ruleType"));
        }

        // 标准：rules: [{ parameterName, ruleType, value }]
        if (context.Parameters.TryGetValue("rules", out var rules) && rules is List<object?> list && list.Count > 0)
        {
            if (list[0] is Dictionary<string, object?> rule)
            {
                string? Get(params string[] keys)
                {
                    foreach (var key in keys)
                    {
                        if (rule.TryGetValue(key, out var v) && v is not null && !string.IsNullOrWhiteSpace(v.ToString()))
                        {
                            return v.ToString();
                        }
                    }

                    return null;
                }

                return (Get("parameterName", "parameter", "parameter_name"), Get("value"), Get("ruleType", "rule", "comparison"));
            }
        }

        return (null, null, null);
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateTagTool.cs（注释与标记）
internal sealed class CreateTagImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.TagCreationService ?? throw new InvalidOperationException("无法获取 TagCreationService");
        var document = ExportArgs.RequireDocument(adapter, context);

        var elementId = WriteArgs.OptionalInt(context, "elementId", "element_id");
        if (elementId is null || elementId <= 0)
        {
            var batchMode = WriteArgs.Flag(context, false, "batchMode", "batch_mode");
            return Task.FromResult(AIToolResult.Fail(batchMode
                ? "本实现暂不支持 batchMode（按类别批量标注）；请提供 elementId 逐个标注。"
                : "必须提供 elementId。"));
        }

        var position = WriteArgs.Point(context, (0, 0, 0), "position", "location");
        var tagId = service.CreateTag(
            document,
            elementId.Value,
            WriteArgs.OptionalInt(context, "tagTypeId", "tag_type_id"),
            WriteArgs.OptionalInt(context, "viewId", "view_id"),
            position.X,
            position.Y,
            WriteArgs.Text(context, "orientation"),
            WriteArgs.Flag(context, false, "addLeader", "add_leader"));

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已为元素 {elementId} 创建标记 {tagId}",
            new { tagId, elementId, position = new { x = position.X, y = position.Y, unit = "millimeters" } }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateScheduleTool.cs（视图查询）
internal sealed class CreateScheduleImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ScheduleService ?? throw new InvalidOperationException("无法获取 ScheduleService");
        var document = ExportArgs.RequireDocument(adapter, context);

        var category = WriteArgs.Text(context, "category", "categoryName")
                       ?? throw new InvalidOperationException("必须提供 category（类别名）。");
        var name = WriteArgs.Text(context, "scheduleName", "name");

        var fields = ToolArgumentReader.ReadStrings(
            new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase), "fields");

        if (fields.Count == 0 && context.Parameters.ContainsKey("fieldUniqueIds"))
        {
            return Task.FromResult(AIToolResult.Fail(
                "本实现用字段名创建明细表字段；请改用 fields:[...]（fieldUniqueIds 需要额外的参数唯一 ID 解析，暂未支持）。"));
        }

        var scheduleId = service.CreateSchedule(document, category, name, fields);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建明细表 {scheduleId}" + (fields.Count > 0 ? $"（{fields.Count} 个字段）" : string.Empty),
            new { scheduleId, category, name, fields, fieldCount = fields.Count }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ExportViewImagesTool.cs（视图导出）
internal sealed class ExportViewImagesImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ExportService ?? throw new InvalidOperationException("无法获取 ExportService");
        var document = ExportArgs.RequireDocument(adapter, context);

        var dataSource = WriteArgs.Text(context, "dataSource", "data_source");
        if (string.IsNullOrWhiteSpace(dataSource))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 dataSource（active / all / ids / cache）。"));
        }

        var viewIds = ExportArgs.ResolveViewIds(context);
        var directory = WriteArgs.Text(context, "exportDirectory", "export_directory", "directory");
        var pixelSize = (int)WriteArgs.Number(context, 2048, "width", "pixelSize");

        var result = service.ExportViewImages(document, viewIds, directory, pixelSize);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已导出 {result.ItemCount} 张视图图片 → {result.OutputPath}",
            new { outputPath = result.OutputPath, count = result.ItemCount, width = pixelSize, imageFormat = WriteArgs.Text(context, "imageFormat") ?? "png" }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ExportViewsAsDwgTool.cs（视图导出）
internal sealed class ExportViewsAsDwgImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ExportService ?? throw new InvalidOperationException("无法获取 ExportService");
        var document = ExportArgs.RequireDocument(adapter, context);

        var dataSource = WriteArgs.Text(context, "dataSource", "data_source");
        if (string.IsNullOrWhiteSpace(dataSource))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 dataSource（active / all / ids / cache）。"));
        }

        var viewIds = ExportArgs.ResolveViewIds(context);
        var directory = WriteArgs.Text(context, "exportDirectory", "export_directory", "directory");
        var settingName = WriteArgs.Text(context, "exportSettingName", "export_setting_name");

        var result = service.ExportViewsAsDwg(document, viewIds, directory, settingName);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已导出 {result.ItemCount} 个视图为 DWG → {result.OutputPath}",
            new
            {
                outputPath = result.OutputPath,
                count = result.ItemCount,
                exportSettingName = settingName ?? "ACAD2018",
                exportAsDxf = WriteArgs.Flag(context, false, "exportAsDXF", "export_as_dxf"),
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ExportScheduleToCsvTool.cs（数据导出）
internal sealed class ExportScheduleToCsvImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ScheduleService ?? throw new InvalidOperationException("无法获取 ScheduleService");
        var document = ExportArgs.RequireDocument(adapter, context);

        var scheduleId = WriteArgs.OptionalInt(context, "scheduleId", "schedule_id");
        var scheduleName = WriteArgs.Text(context, "scheduleName", "schedule_name");
        if (scheduleId is null && string.IsNullOrWhiteSpace(scheduleName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 scheduleId 或 scheduleName。"));
        }

        var filePath = WriteArgs.Text(context, "filePath", "file_path");
        var result = service.ExportScheduleToCsv(document, scheduleId, scheduleName, filePath);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 明细表已导出 {result.ItemCount} 行 → {result.OutputPath}",
            new { outputPath = result.OutputPath, rows = result.ItemCount, includeHeader = WriteArgs.Flag(context, true, "includeHeader") }));
    }
}
