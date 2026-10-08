using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 2（视图与标注类）工具实现。全部是写操作：靠宿主的 ExternalEvent + 事务执行。
/// 参数名按厂商 schema 读取，同时兼容常见别名。
/// </summary>
public static class WriteImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("set_view_detail_level", new SetViewDetailLevelImplementation());
        ToolImplementationRegistry.Register("set_view_display_style", new SetViewDisplayStyleImplementation());
        ToolImplementationRegistry.Register("set_background_color", new SetBackgroundColorImplementation());
        ToolImplementationRegistry.Register("temporary_view_control", new TemporaryViewControlImplementation());
        ToolImplementationRegistry.Register("duplicate_view", new DuplicateViewImplementation());
        ToolImplementationRegistry.Register("create_floor_plan", new CreateFloorPlanImplementation());
        ToolImplementationRegistry.Register("create_section_view", new CreateSectionViewImplementation());
        ToolImplementationRegistry.Register("create_sheet", new CreateSheetImplementation());
        ToolImplementationRegistry.Register("create_text_note", new CreateTextNoteImplementation());
        ToolImplementationRegistry.Register("set_view_filter", new SetViewFilterImplementation());
        ToolImplementationRegistry.Register("delete_view_filter", new DeleteViewFilterImplementation());
    }
}

internal static class WriteArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    public static string? Text(AIToolContext context, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (context.Parameters.TryGetValue(key, out var value) && value is not null)
            {
                var text = value.ToString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }
        }

        return null;
    }

    public static int? OptionalInt(AIToolContext context, params string[] keys)
    {
        // 注意：必须按"键是否存在"判断，不能按值是否为 0——layerIndex/insertIndex 等参数合法值就是 0。
        foreach (var key in keys)
        {
            if (context.Parameters.TryGetValue(key, out var value) && value is not null)
            {
                return (int)ToolArgumentReader.ToDouble(value);
            }
        }

        return null;
    }

    public static double Number(AIToolContext context, double fallback, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (context.Parameters.TryGetValue(key, out var value) && value is not null)
            {
                return ToolArgumentReader.ToDouble(value);
            }
        }

        return fallback;
    }

    public static bool Flag(AIToolContext context, bool fallback, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (context.Parameters.TryGetValue(key, out var value) && value is not null)
            {
                if (value is bool flag)
                {
                    return flag;
                }

                return string.Equals(value.ToString(), "true", StringComparison.OrdinalIgnoreCase);
            }
        }

        return fallback;
    }

    public static List<int> Ids(AIToolContext context, params string[] keys)
    {
        var result = new List<int>();
        foreach (var key in keys)
        {
            if (context.Parameters.TryGetValue(key, out var value) && value is not null)
            {
                result.AddRange(ToolArgumentReader.ReadIntegers(
                    new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase), key));
            }
        }

        return result.Distinct().ToList();
    }

    public static (double X, double Y, double Z) Point(AIToolContext context, (double X, double Y, double Z) fallback, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (context.Parameters.TryGetValue(key, out var value) && value is Dictionary<string, object?> map)
            {
                return (
                    map.TryGetValue("x", out var x) ? ToolArgumentReader.ToDouble(x) : fallback.X,
                    map.TryGetValue("y", out var y) ? ToolArgumentReader.ToDouble(y) : fallback.Y,
                    map.TryGetValue("z", out var z) ? ToolArgumentReader.ToDouble(z) : fallback.Z);
            }
        }

        return fallback;
    }

    public static (int R, int G, int B) Color(AIToolContext context, (int R, int G, int B) fallback, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (context.Parameters.TryGetValue(key, out var value) && value is Dictionary<string, object?> map)
            {
                return (
                    map.TryGetValue("red", out var r) ? (int)ToolArgumentReader.ToDouble(r) : fallback.R,
                    map.TryGetValue("green", out var g) ? (int)ToolArgumentReader.ToDouble(g) : fallback.G,
                    map.TryGetValue("blue", out var b) ? (int)ToolArgumentReader.ToDouble(b) : fallback.B);
            }
        }

        return fallback;
    }

    public static (int R, int G, int B) ParseHex(string? hex, (int R, int G, int B) fallback)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return fallback;
        }

        var text = hex.Trim().TrimStart('#');
        if (text.Length != 6 || !int.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out var value))
        {
            return fallback;
        }

        return ((value >> 16) & 0xFF, (value >> 8) & 0xFF, value & 0xFF);
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SetViewDetailLevelTool.cs（视图管理）
internal sealed class SetViewDetailLevelImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewWriteService ?? throw new InvalidOperationException("无法获取 ViewWriteService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var detailLevel = WriteArgs.Text(context, "detail_level", "detailLevel")
                          ?? throw new InvalidOperationException("必须提供 detail_level（粗略/中等/精细）。");

        var settings = service.SetDetailLevel(document, WriteArgs.OptionalInt(context, "view_id", "viewId"), detailLevel);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 视图 {settings?.Name} 详细程度已设为 {settings?.DetailLevel}",
            new { viewId = settings?.ViewId, detailLevel = settings?.DetailLevel }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SetViewDisplayStyleTool.cs（视图管理）
internal sealed class SetViewDisplayStyleImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewWriteService ?? throw new InvalidOperationException("无法获取 ViewWriteService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var displayStyle = WriteArgs.Text(context, "display_style", "displayStyle")
                           ?? throw new InvalidOperationException("必须提供 display_style（线框/隐藏线/着色/真实 等）。");

        var settings = service.SetDisplayStyle(document, WriteArgs.OptionalInt(context, "view_id", "viewId"), displayStyle);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 视图 {settings?.Name} 视觉样式已设为 {settings?.DisplayStyle}",
            new { viewId = settings?.ViewId, displayStyle = settings?.DisplayStyle }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SetBackgroundColorTool.cs（视图管理）
internal sealed class SetBackgroundColorImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewWriteService ?? throw new InvalidOperationException("无法获取 ViewWriteService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var toggle = WriteArgs.Flag(context, false, "toggle");
        var (red, green, blue) = toggle
            ? (255, 255, 255) // 说明：厂商 toggle 是黑/白切换；这里简化为切到白色（读当前背景色需要额外接口）
            : WriteArgs.ParseHex(
                WriteArgs.Text(context, "hexColor", "hex_color"),
                (
                    (int)WriteArgs.Number(context, 255, "red"),
                    (int)WriteArgs.Number(context, 255, "green"),
                    (int)WriteArgs.Number(context, 255, "blue")));

        service.SetBackgroundColor(document, red, green, blue);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 背景色已设为 RGB({red}, {green}, {blue})",
            new { red, green, blue, toggled = toggle }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\TemporaryViewControlTool.cs（视图管理）
internal sealed class TemporaryViewControlImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewWriteService ?? throw new InvalidOperationException("无法获取 ViewWriteService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var action = WriteArgs.Text(context, "action")
                     ?? throw new InvalidOperationException("必须提供 action（ISOLATE_ELEMENTS/HIDE_ELEMENTS/RESET/QUERY_STATUS）。");

        var scope = WriteArgs.Text(context, "target_scope", "targetScope") ?? "SPECIFIC_IDS";
        if (scope.Equals("CATEGORY_NAMES", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AIToolResult.Fail(
                "本实现暂不支持按类别临时隔离（CATEGORY_NAMES）；请改用 SPECIFIC_IDS / CACHE_ID。"));
        }

        var ids = WriteArgs.Ids(context, "element_ids", "elementIds", "ids");
        var cacheId = WriteArgs.Text(context, "cacheId", "cache_id");
        if (ids.Count == 0 && !string.IsNullOrWhiteSpace(cacheId))
        {
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
        }

        var result = service.ApplyTemporaryViewControl(
            document, WriteArgs.OptionalInt(context, "view_id", "viewId"), action, ids);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 临时视图控制 {result.Action}：影响 {result.AffectedCount} 个元素，临时模式={(result.IsInTemporaryMode ? "开启" : "关闭")}",
            new
            {
                viewId = result.ViewId,
                action = result.Action,
                affectedCount = result.AffectedCount,
                isInTemporaryMode = result.IsInTemporaryMode,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\DuplicateViewTool.cs（视图高级操作）
internal sealed class DuplicateViewImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewWriteService ?? throw new InvalidOperationException("无法获取 ViewWriteService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var viewId = WriteArgs.OptionalInt(context, "viewId", "view_id")
                     ?? throw new InvalidOperationException("必须提供 viewId。");

        var option = WriteArgs.Text(context, "duplicateDetail", "option", "duplicateOption") ?? "duplicate";
        var newName = WriteArgs.Text(context, "newViewName", "newName");

        var newViewId = service.DuplicateView(document, viewId, option, newName);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已复制视图 {viewId} → 新视图 {newViewId}（{option}）",
            new { sourceViewId = viewId, newViewId, option, newViewName = newName }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateFloorPlanTool.cs（元素创建）
internal sealed class CreateFloorPlanImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewCreationService ?? throw new InvalidOperationException("无法获取 ViewCreationService");
        var levelService = adapter.LevelService ?? throw new InvalidOperationException("无法获取 LevelService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var levelName = WriteArgs.Text(context, "levelName", "level_name");
        var levelId = WriteArgs.OptionalInt(context, "level_id", "levelId");
        if (string.IsNullOrWhiteSpace(levelName) && levelId is not null)
        {
            levelName = levelService
                .GetAllLevels(document)
                .FirstOrDefault(level => levelService.GetLevelId(level) == levelId) is { } level
                ? levelService.GetLevelName(level)
                : null;
        }

        if (string.IsNullOrWhiteSpace(levelName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 level_id 或 levelName，且对应的标高需要存在。"));
        }

        var viewId = service.CreateFloorPlan(document, levelName!, WriteArgs.Text(context, "view_name", "viewName"));
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建楼层平面视图 {viewId}（标高 {levelName}）",
            new { viewId, levelName }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateSectionViewTool.cs（视图创建）
internal sealed class CreateSectionViewImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewCreationService ?? throw new InvalidOperationException("无法获取 ViewCreationService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var viewName = WriteArgs.Text(context, "view_name", "viewName")
                       ?? throw new InvalidOperationException("必须提供 view_name。");

        var min = WriteArgs.Point(context, (0, 0, 0), "min_point", "minPoint");
        var max = WriteArgs.Point(context, (10000, 10000, 4000), "max_point", "maxPoint");

        var viewId = service.CreateSection(document, viewName, min.X, min.Y, max.X, max.Y, min.Z, max.Z);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建剖面视图 {viewId}（{viewName}）",
            new
            {
                viewId,
                viewName,
                direction = WriteArgs.Text(context, "direction") ?? string.Empty,
                minPoint = new { x = min.X, y = min.Y, z = min.Z, unit = "millimeters" },
                maxPoint = new { x = max.X, y = max.Y, z = max.Z, unit = "millimeters" },
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateSheetTool.cs（元素创建）
internal sealed class CreateSheetImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewCreationService ?? throw new InvalidOperationException("无法获取 ViewCreationService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var sheetName = WriteArgs.Text(context, "title", "sheetName", "name");
        var sheetNumber = WriteArgs.Text(context, "number", "sheetNumber");
        var viewId = service.CreateSheet(document, sheetNumber, sheetName);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建图纸 {viewId}" + (string.IsNullOrWhiteSpace(sheetNumber) ? string.Empty : $"（图号 {sheetNumber}）"),
            new
            {
                sheetId = viewId,
                sheetNumber,
                sheetName,
                addView = WriteArgs.Text(context, "add_view", "addView"),
                note = "add_view/view_position 暂未实现（放置视图port需要 Viewport.Create）。",
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateTextNoteTool.cs（注释与标记）
internal sealed class CreateTextNoteImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewCreationService ?? throw new InvalidOperationException("无法获取 ViewCreationService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var text = WriteArgs.Text(context, "text")
                   ?? throw new InvalidOperationException("必须提供 text（文字内容）。");
        var position = WriteArgs.Point(context, (0, 0, 0), "position", "location");

        var noteId = service.CreateTextNote(document, text, position.X, position.Y, WriteArgs.OptionalInt(context, "viewId", "view_id"));
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建文字注释 {noteId}",
            new { noteId, text, position = new { x = position.X, y = position.Y, unit = "millimeters" } }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SetViewFilterTool.cs（视图高级操作）
internal sealed class SetViewFilterImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.FilterService ?? throw new InvalidOperationException("无法获取 FilterService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var filterName = WriteArgs.Text(context, "filterName", "filter_name")
                         ?? throw new InvalidOperationException("必须提供 filterName。");
        var action = WriteArgs.Text(context, "action") ?? "apply";
        var visible = WriteArgs.Flag(context, true, "visible");
        var (red, green, blue) = WriteArgs.Color(context, (255, 0, 0), "color", "patternColor");

        var filterId = service.ApplyFilterToView(
            document, WriteArgs.OptionalInt(context, "viewId", "view_id"), filterName, action, red, green, blue, visible);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 过滤器 {filterName} 已执行 {action}（视图 {WriteArgs.OptionalInt(context, "viewId", "view_id")?.ToString() ?? "当前"}）",
            new { filterId, filterName, action, visible, color = new { red, green, blue } }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\DeleteViewFilterTool.cs（视图高级操作）
internal sealed class DeleteViewFilterImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.FilterService ?? throw new InvalidOperationException("无法获取 FilterService");
        var document = WriteArgs.RequireDocument(adapter, context);

        var filterName = WriteArgs.Text(context, "filterName", "filter_name")
                         ?? throw new InvalidOperationException("必须提供 filterName。");

        var deleted = service.DeleteFilter(document, filterName);
        return Task.FromResult(deleted
            ? AIToolResult.Ok($"✅ 已删除过滤器 {filterName}", new { filterName, deleted = true })
            : AIToolResult.Fail($"未找到名为 {filterName} 的过滤器（可能已被删除）。"));
    }
}
