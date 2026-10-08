using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次「查询与交互」实现：元素查询/过滤、房间边界、轴网交点、偏移点、明细表字段与数据、
/// 碰撞检查、删除链接、项目单位、类别可见性。
/// </summary>
public static class QueryInteractionImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("element_query", new ElementQueryImplementation());
        ToolImplementationRegistry.Register("filter_elements", new FilterElementsImplementation());
        ToolImplementationRegistry.Register("get_room_boundaries", new GetRoomBoundariesImplementation());
        ToolImplementationRegistry.Register("get_grid_intersection", new GetGridIntersectionImplementation());
        ToolImplementationRegistry.Register("get_offset_point", new GetOffsetPointImplementation());
        ToolImplementationRegistry.Register("get_schedule_fields", new GetScheduleFieldsImplementation());
        ToolImplementationRegistry.Register("read_schedule_data", new ReadScheduleDataImplementation());
        ToolImplementationRegistry.Register("check_collision", new CheckCollisionImplementation());
        ToolImplementationRegistry.Register("delete_link", new DeleteLinkImplementation());
        ToolImplementationRegistry.Register("set_project_units", new SetProjectUnitsImplementation());
        ToolImplementationRegistry.Register("set_category_visibility", new SetCategoryVisibilityImplementation());
    }
}

internal static class QueryArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    public static List<int> ResolveIds(AIToolContext context, params string[] keys)
    {
        var all = keys.Length > 0 ? keys : new[] { "elementIds", "element_ids", "elementId", "element_id" };
        var ids = WriteArgs.Ids(context, all);

        if (ids.Count == 0)
        {
            var cacheId = WriteArgs.Text(context, "cacheId", "cache_id");
            if (!string.IsNullOrWhiteSpace(cacheId))
            {
                if (context.GetCachedData<List<Dictionary<string, object?>>>(cacheId) is { } cached)
                {
                    ids.AddRange(ToolArgumentReader.ExtractIds(cached));
                }
                else if (context.GetCachedData<List<int>>(cacheId) is { } cachedIds)
                {
                    ids.AddRange(cachedIds);
                }
            }
        }

        return ids.Distinct().ToList();
    }

    public static object Summarize(ElementSummary element) => new
    {
        elementId = element.ElementId,
        name = element.Name,
        category = element.CategoryName,
        typeId = element.TypeId,
        typeName = element.TypeName,
        levelId = element.LevelId,
        levelName = element.LevelName,
    };
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ElementQueryTool.cs（元素查询）
internal sealed class ElementQueryImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.AdvancedQueryService ?? throw new InvalidOperationException("无法获取 AdvancedQueryService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "all").Trim();
        var elements = service.QueryElements(
            document,
            operation,
            QueryArgs.ResolveIds(context),
            WriteArgs.Text(context, "categoryName", "category_name", "category"),
            WriteArgs.OptionalInt(context, "typeId", "type_id"),
            WriteArgs.Text(context, "typeName", "type_name"),
            (int)WriteArgs.Number(context, 200, "limit"));

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 查询到 {elements.Count} 个元素（operation={operation}）",
            new
            {
                operation,
                count = elements.Count,
                elements = elements.Select(QueryArgs.Summarize).ToArray(),
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\FilterElementsTool.cs（元素查询）
internal sealed class FilterElementsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.AdvancedQueryService ?? throw new InvalidOperationException("无法获取 AdvancedQueryService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var elements = service.FilterElements(
            document,
            QueryArgs.ResolveIds(context),
            WriteArgs.Text(context, "categoryName", "category_name", "category"),
            WriteArgs.Text(context, "typeName", "type_name"),
            WriteArgs.Text(context, "levelName", "level_name"),
            (int)WriteArgs.Number(context, 200, "limit"));

        var customFilter = WriteArgs.Text(context, "customFilter", "custom_filter");

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 过滤后剩 {elements.Count} 个元素",
            new
            {
                count = elements.Count,
                elements = elements.Select(QueryArgs.Summarize).ToArray(),
                customFilter,
                note = string.IsNullOrWhiteSpace(customFilter) ? null : "customFilter 为厂商侧表达式，本实现只回显不解析（安全考虑）。",
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetRoomBoundariesTool.cs（空间分析）
internal sealed class GetRoomBoundariesImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.AdvancedQueryService ?? throw new InvalidOperationException("无法获取 AdvancedQueryService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var roomId = WriteArgs.OptionalInt(context, "roomId", "room_id");
        if (roomId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 roomId。"));
        }

        var loops = service.GetRoomBoundaries(document, roomId.Value);
        var total = Math.Round(loops.Sum(loop => loop.LengthMm), 2);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 房间 {roomId} 共 {loops.Count} 条边界环，总周长 {total} mm",
            new
            {
                roomId = roomId.Value,
                loopCount = loops.Count,
                totalLengthMM = total,
                loops = loops.Select(loop => new
                {
                    index = loop.Index,
                    isOuter = loop.IsOuter,
                    lengthMM = loop.LengthMm,
                    pointCount = loop.Points.Count,
                    points = loop.Points.Select(point => new { x = point.X, y = point.Y }).ToArray(),
                }).ToArray(),
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetGridIntersectionTool.cs（空间分析）
internal sealed class GetGridIntersectionImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.AdvancedQueryService ?? throw new InvalidOperationException("无法获取 AdvancedQueryService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var grid1 = WriteArgs.OptionalInt(context, "grid1_id", "grid1Id");
        var grid2 = WriteArgs.OptionalInt(context, "grid2_id", "grid2Id");
        if (grid1 is null || grid2 is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 grid1_id 与 grid2_id。"));
        }

        var result = service.GetGridIntersection(document, grid1.Value, grid2.Value);
        return Task.FromResult(result.Exists
            ? AIToolResult.Ok($"✅ 交点 ({result.X}, {result.Y}) mm", new { exists = true, x = result.X, y = result.Y, z = result.Z, unit = "millimeters" })
            : AIToolResult.Fail($"没有唯一交点：{result.Message}", new { exists = false }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetOffsetPointTool.cs（几何计算）
internal sealed class GetOffsetPointImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var basePoint = WriteArgs.Point(context, (0, 0, 0), "base_point", "basePoint", "point");
        var distance = WriteArgs.Number(context, double.NaN, "distance");
        if (double.IsNaN(distance))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 distance（毫米）。"));
        }

        var angle = context.Parameters.ContainsKey("angle")
            ? (double?)WriteArgs.Number(context, 0, "angle")
            : null;

        var directionText = WriteArgs.Text(context, "direction") ?? "east";
        var (dx, dy) = ResolveDirection(directionText, angle);
        if (dx == 0 && dy == 0 && angle is null)
        {
            return Task.FromResult(AIToolResult.Fail(
                "direction 只支持 north/south/east/west/ne/nw/se/sw（或提供 angle 角度）。"));
        }

        var x = basePoint.X + distance * dx;
        var y = basePoint.Y + distance * dy;

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 偏移点 ({Math.Round(x, 2)}, {Math.Round(y, 2)}, {basePoint.Z}) mm",
            new
            {
                x = Math.Round(x, 2),
                y = Math.Round(y, 2),
                z = basePoint.Z,
                unit = "millimeters",
                basePoint = new { x = basePoint.X, y = basePoint.Y, z = basePoint.Z },
                direction = directionText,
                angle,
                distance,
            }));
    }

    private static (double Dx, double Dy) ResolveDirection(string direction, double? angle)
    {
        if (angle is not null)
        {
            var radians = angle.Value * Math.PI / 180.0;
            return (Math.Cos(radians), Math.Sin(radians));
        }

        return direction.Trim().ToLowerInvariant() switch
        {
            "north" or "n" or "北" => (0, 1),
            "south" or "s" or "南" => (0, -1),
            "east" or "e" or "东" => (1, 0),
            "west" or "w" or "西" => (-1, 0),
            "ne" or "东北" => (Math.Sqrt(0.5), Math.Sqrt(0.5)),
            "nw" or "西北" => (-Math.Sqrt(0.5), Math.Sqrt(0.5)),
            "se" or "东南" => (Math.Sqrt(0.5), -Math.Sqrt(0.5)),
            "sw" or "西南" => (-Math.Sqrt(0.5), -Math.Sqrt(0.5)),
            _ => (0, 0),
        };
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetScheduleFieldsTool.cs（明细表）
internal sealed class GetScheduleFieldsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.AdvancedQueryService ?? throw new InvalidOperationException("无法获取 AdvancedQueryService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var scheduleId = WriteArgs.OptionalInt(context, "scheduleId", "schedule_id");
        var scheduleName = WriteArgs.Text(context, "scheduleName", "schedule_name");
        if (scheduleId is null && string.IsNullOrWhiteSpace(scheduleName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 scheduleId 或 scheduleName。"));
        }

        var fields = service.GetScheduleFields(document, scheduleId, scheduleName);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 明细表共 {fields.Count} 个字段",
            new
            {
                count = fields.Count,
                fields = fields.Select(field => new
                {
                    index = field.Index,
                    name = field.Name,
                    fieldType = field.FieldType,
                    isHidden = field.IsHidden,
                    heading = field.Heading,
                }).ToArray(),
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ReadScheduleDataTool.cs（明细表）
internal sealed class ReadScheduleDataImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.AdvancedQueryService ?? throw new InvalidOperationException("无法获取 AdvancedQueryService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var scheduleId = WriteArgs.OptionalInt(context, "scheduleId", "schedule_id");
        var scheduleName = WriteArgs.Text(context, "scheduleName", "schedule_name");
        if (scheduleId is null && string.IsNullOrWhiteSpace(scheduleName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 scheduleId 或 scheduleName。"));
        }

        var data = service.ReadScheduleData(
            document,
            scheduleId,
            scheduleName,
            (int)WriteArgs.Number(context, 0, "startRow", "start_row"),
            (int)WriteArgs.Number(context, 200, "maxRows", "max_rows"),
            WriteArgs.Flag(context, true, "includeHeader", "include_header"));

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 明细表 {data.ScheduleName} 共 {data.TotalRows} 行，本次返回 {data.ReturnedRows} 行",
            new
            {
                scheduleName = data.ScheduleName,
                totalRows = data.TotalRows,
                returnedRows = data.ReturnedRows,
                headers = data.Headers,
                rows = data.Rows,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CheckCollisionTool.cs（碰撞检查）
internal sealed class CheckCollisionImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.AdvancedQueryService ?? throw new InvalidOperationException("无法获取 AdvancedQueryService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var sourceSpecified = context.Parameters.ContainsKey("source") || context.Parameters.ContainsKey("elementIds") || context.Parameters.ContainsKey("cacheId");
        if (!sourceSpecified)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 source（源元素集合：id 数组或 cacheId）。"));
        }

        var sources = WriteArgs.Ids(context, "source", "sourceIds", "elementIds");
        var targets = WriteArgs.Ids(context, "target", "targetIds");
        var sourceLink = WriteArgs.OptionalInt(context, "sourceLinkInstanceId", "source_link_instance_id");
        var targetLink = WriteArgs.OptionalInt(context, "targetLinkInstanceId", "target_link_instance_id");

        var pairs = service.CheckCollision(document, sources, targets);
        var collisions = pairs.Where(pair => pair.Intersects).ToList();

        return Task.FromResult(AIToolResult.Ok(
            collisions.Count == 0
                ? $"✅ 未发现碰撞（检查 {pairs.Count} 组）"
                : $"⚠️ 发现 {collisions.Count} 处碰撞",
            new
            {
                sourceCount = sources.Count,
                targetCount = targets.Count,
                collisionCount = collisions.Count,
                collisions = collisions.Select(pair => new { sourceId = pair.SourceId, targetId = pair.TargetId }).ToArray(),
                sourceLinkInstanceId = sourceLink,
                targetLinkInstanceId = targetLink,
                note = "链接模型内的元素暂不参与相交判定（厂商的 Link 版本需要链接文档上下文）。",
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\DeleteLinkTool.cs（链接管理）
internal sealed class DeleteLinkImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.AdvancedQueryService ?? throw new InvalidOperationException("无法获取 AdvancedQueryService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var linkId = WriteArgs.OptionalInt(context, "linkId", "link_id");
        if (linkId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 linkId。"));
        }

        var deleted = service.DeleteLink(document, linkId.Value);
        return Task.FromResult(deleted
            ? AIToolResult.Ok($"✅ 已删除链接 {linkId}", new { linkId = linkId.Value, deleted = true })
            : AIToolResult.Fail($"链接 {linkId} 未被删除。", new { linkId = linkId.Value, deleted = false }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SetProjectUnitsTool.cs（项目设置）
internal sealed class SetProjectUnitsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ProjectSettingsService ?? throw new InvalidOperationException("无法获取 ProjectSettingsService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var length = WriteArgs.Text(context, "length");
        var area = WriteArgs.Text(context, "area");
        var volume = WriteArgs.Text(context, "volume");
        var angle = WriteArgs.Text(context, "angle");
        var slope = WriteArgs.Text(context, "slope");

        if (length is null && area is null && volume is null && angle is null && slope is null)
        {
            return Task.FromResult(AIToolResult.Fail(
                "必须至少提供 length/area/volume/angle/slope 之一（如 length=mm、area=m2、angle=deg）。"));
        }

        var changed = service.SetProjectUnits(document, length, area, volume, angle, slope);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已更新 {changed} 项项目单位",
            new { changed, length, area, volume, angle, slope }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SetCategoryVisibilityTool.cs（视图管理）
internal sealed class SetCategoryVisibilityImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ProjectSettingsService ?? throw new InvalidOperationException("无法获取 ProjectSettingsService");
        var document = QueryArgs.RequireDocument(adapter, context);

        var viewId = WriteArgs.OptionalInt(context, "view_id", "viewId");
        var categoryName = WriteArgs.Text(context, "category_name", "categoryName", "category");
        if (viewId is null || string.IsNullOrWhiteSpace(categoryName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 view_id 与 category_name。"));
        }

        var visible = WriteArgs.Flag(context, true, "visible");
        var changed = service.SetCategoryVisibility(document, viewId.Value, categoryName!, visible);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 视图 {viewId} 的类别「{categoryName}」已{(visible ? "显示" : "隐藏")}",
            new { viewId = viewId.Value, categoryName, visible, changed }));
    }
}
