using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 2 收尾实现：点标注创建、明细表编辑、Excel 导出。
/// export_excel 直接调用 Core 里的 XlsxWriterLite（零第三方依赖），因此不需要服务。
/// </summary>
public static class WriteImplementations3
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("create_spot_dimension", new CreateSpotDimensionImplementation());
        ToolImplementationRegistry.Register("edit_schedule", new EditScheduleImplementation());
        ToolImplementationRegistry.Register("export_excel", new ExportExcelImplementation());
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateSpotDimensionTool.cs（注释与标记）
internal sealed class CreateSpotDimensionImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.DimensionCreationService ?? throw new InvalidOperationException("无法获取 DimensionCreationService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var kind = WriteArgs.Text(context, "operator", "kind") ?? "elevation";
        var elementId = WriteArgs.OptionalInt(context, "elementId", "element_id");
        if (elementId is null || elementId <= 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId。"));
        }

        var point = WriteArgs.Point(context, (0, 0, 0), "point", "position");
        var dimensionId = service.CreateSpotDimension(
            document,
            kind,
            elementId.Value,
            WriteArgs.OptionalInt(context, "viewId", "view_id"),
            point.X,
            point.Y,
            WriteArgs.OptionalInt(context, "spotDimensionTypeId", "spot_dimension_type_id"),
            WriteArgs.Flag(context, false, "hasLeader", "has_leader"));

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建点标注 {dimensionId}（{kind}）",
            new { dimensionId, kind, elementId = elementId.Value, position = new { x = point.X, y = point.Y, unit = "millimeters" } }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\EditScheduleTool.cs（视图高级操作）
internal sealed class EditScheduleImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ScheduleEditService ?? throw new InvalidOperationException("无法获取 ScheduleEditService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var scheduleId = WriteArgs.OptionalInt(context, "scheduleId", "schedule_id");
        var scheduleName = WriteArgs.Text(context, "scheduleName", "schedule_name");
        if (scheduleId is null && string.IsNullOrWhiteSpace(scheduleName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 scheduleId 或 scheduleName。"));
        }

        var parameters = new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase);
        var addFieldParameterIds = ToolArgumentReader.ReadIntegers(parameters, "addFieldParameterIds", "add_field_parameter_ids");
        var removeFieldParameterIds = ToolArgumentReader.ReadIntegers(parameters, "removeFieldParameterIds", "remove_field_parameter_ids");
        var replaceFields = WriteArgs.Flag(context, false, "replaceFields", "replace_fields");
        var sortByParameterId = WriteArgs.OptionalInt(context, "sortByParameterId", "sort_by_parameter_id");
        var groupByParameterId = WriteArgs.OptionalInt(context, "groupByParameterId", "group_by_parameter_id");

        var id = service.EditSchedule(
            document,
            scheduleId,
            scheduleName,
            addFieldParameterIds,
            removeFieldParameterIds,
            replaceFields,
            sortByParameterId,
            WriteArgs.Text(context, "sortOrder", "sort_order"),
            groupByParameterId);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 明细表 {id} 已更新（新增 {addFieldParameterIds.Count} 字段，移除 {removeFieldParameterIds.Count} 字段，replaceFields={replaceFields}）",
            new
            {
                scheduleId = id,
                addedFields = addFieldParameterIds.Count,
                removedFields = removeFieldParameterIds.Count,
                replaceFields,
                sortByParameterId,
                sortOrder = WriteArgs.Text(context, "sortOrder", "sort_order"),
                groupByParameterId,
                note = "排序/分组本期只记录不落地（见 ScheduleEditService 注释）。",
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ExportExcelTool.cs（数据导出）
internal sealed class ExportExcelImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var dataSource = WriteArgs.Text(context, "dataSource", "data_source");
        if (string.IsNullOrWhiteSpace(dataSource))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 dataSource（data / cache / schedule）。"));
        }

        var rows = ResolveRows(context, out var resolutionNote);
        if (rows.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail(
                $"没有可导出的数据（dataSource={dataSource}）。请在 data 里提供二维数组，或用 cacheId 指向已有缓存。{resolutionNote}"));
        }

        var sheetName = WriteArgs.Text(context, "sheetName", "sheet_name") ?? "Sheet1";
        var filePath = WriteArgs.Text(context, "filePath", "file_path");
        if (string.IsNullOrWhiteSpace(filePath))
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RevitAiEngine",
                "exports");
            Directory.CreateDirectory(directory);
            filePath = Path.Combine(directory, Sanitize(sheetName) + ".xlsx");
        }

        XlsxWriterLite.Write(filePath!, sheetName, rows);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已导出 {rows.Count} 行到 {filePath}",
            new { outputPath = filePath, rows = rows.Count, sheetName, dataSource }));
    }

    private static List<IReadOnlyList<string>> ResolveRows(AIToolContext context, out string note)
    {
        note = string.Empty;

        // 1) 显式 data：二维数组 或 对象数组（对象取键作为表头）
        if (context.Parameters.TryGetValue("data", out var data) && data is List<object?> list && list.Count > 0)
        {
            if (list[0] is Dictionary<string, object?> first)
            {
                var headers = first.Keys.ToList();
                var rows = new List<IReadOnlyList<string>> { headers };
                foreach (var item in list.OfType<Dictionary<string, object?>>())
                {
                    rows.Add(headers.Select(key => item.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty).ToList());
                }

                return rows;
            }

            if (list[0] is List<object?>)
            {
                return list
                    .OfType<List<object?>>()
                    .Select(row => (IReadOnlyList<string>)row.Select(cell => cell?.ToString() ?? string.Empty).ToList())
                    .ToList();
            }
        }

        // 2) cacheId：缓存里的字典数组，键作为表头
        var cacheId = WriteArgs.Text(context, "cacheId", "cache_id");
        if (!string.IsNullOrWhiteSpace(cacheId))
        {
            var cached = context.GetCachedData<List<Dictionary<string, object?>>>(cacheId);
            if (cached is not null && cached.Count > 0)
            {
                var headers = cached[0].Keys.ToList();
                var rows = new List<IReadOnlyList<string>> { headers };
                rows.AddRange(cached.Select(item =>
                    (IReadOnlyList<string>)headers.Select(key => item.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty).ToList()));
                return rows;
            }

            note = $"（cacheId={cacheId} 未取到字典数组缓存）";
        }

        return new List<IReadOnlyList<string>>();
    }

    private static string Sanitize(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var text = new string(name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
        return string.IsNullOrWhiteSpace(text) ? "export" : text;
    }
}
