using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 4 第二刀实现：MEP 直段（管道/风管/桥架）+ 楼板/屋顶/尺寸/3D 视图。
/// </summary>
public static class ModelingImplementations2
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("create_pipe", new CreatePipeImplementation());
        ToolImplementationRegistry.Register("create_duct", new CreateDuctImplementation());
        ToolImplementationRegistry.Register("create_cable_tray", new CreateCableTrayImplementation());
        ToolImplementationRegistry.Register("create_floor_by_profile", new CreateFloorByProfileImplementation());
        ToolImplementationRegistry.Register("create_footprint_roof", new CreateFootprintRoofImplementation());
        ToolImplementationRegistry.Register("create_extrusion_roof", new CreateExtrusionRoofImplementation());
        ToolImplementationRegistry.Register("create_dimension_by_elements", new CreateDimensionByElementsImplementation());
        ToolImplementationRegistry.Register("create_3d_view", new Create3DViewImplementation());
    }
}

internal static class ShapeArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    /// <summary>支持 points: [{x,y}] 与 points: [[x,y]] 两种写法。</summary>
    public static List<Point2> ReadPoints(Dictionary<string, object?> item, string key)
    {
        var points = new List<Point2>();
        if (!item.TryGetValue(key, out var value) || value is not List<object?> list)
        {
            return points;
        }

        foreach (var entry in list)
        {
            switch (entry)
            {
                case Dictionary<string, object?> map:
                    points.Add(new Point2(
                        ModelingArgs.Number(map, "x"),
                        ModelingArgs.Number(map, "y")));
                    break;
                case List<object?> pair when pair.Count >= 2:
                    points.Add(new Point2(
                        ToolArgumentReader.ToDouble(pair[0]),
                        ToolArgumentReader.ToDouble(pair[1])));
                    break;
            }
        }

        return points;
    }

    public static MePSpec ToSpec(Dictionary<string, object?> item)
        => new(
            ModelingArgs.Number(item, "start_x", ModelingArgs.Number(item, "startX")),
            ModelingArgs.Number(item, "start_y", ModelingArgs.Number(item, "startY")),
            ModelingArgs.Number(item, "start_z", ModelingArgs.Number(item, "startZ")),
            ModelingArgs.Number(item, "end_x", ModelingArgs.Number(item, "endX")),
            ModelingArgs.Number(item, "end_y", ModelingArgs.Number(item, "endY")),
            ModelingArgs.Number(item, "end_z", ModelingArgs.Number(item, "endZ")),
            ModelingArgs.OptionalInt(item, "level_id", "levelId"),
            ModelingArgs.OptionalInt(item, "pipe_type_id", "duct_type_id", "cable_tray_type_id", "type_id", "typeId"),
            ModelingArgs.OptionalInt(item, "system_type_id", "systemTypeId"),
            OptionalDouble(item, "diameter"),
            OptionalDouble(item, "width"),
            OptionalDouble(item, "height"));

    private static double? OptionalDouble(Dictionary<string, object?> item, string key)
        => item.TryGetValue(key, out var value) && value is not null ? ToolArgumentReader.ToDouble(value) : null;
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreatePipeTool.cs（MEP 建模）
internal sealed class CreatePipeImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.MepModelingService ?? throw new InvalidOperationException("无法获取 MepModelingService");
        var document = ShapeArgs.RequireDocument(adapter, context);

        var specs = ModelingArgs.Combine(context, "pipes").Select(ShapeArgs.ToSpec).ToList();
        var result = service.CreatePipes(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("管道", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateDuctTool.cs（MEP 建模）
internal sealed class CreateDuctImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.MepModelingService ?? throw new InvalidOperationException("无法获取 MepModelingService");
        var document = ShapeArgs.RequireDocument(adapter, context);

        var specs = ModelingArgs.Combine(context, "ducts").Select(ShapeArgs.ToSpec).ToList();
        var result = service.CreateDucts(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("风管", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateCableTrayTool.cs（MEP 建模）
internal sealed class CreateCableTrayImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.MepModelingService ?? throw new InvalidOperationException("无法获取 MepModelingService");
        var document = ShapeArgs.RequireDocument(adapter, context);

        var specs = ModelingArgs.Combine(context, "cable_trays").Select(ShapeArgs.ToSpec).ToList();
        var result = service.CreateCableTrays(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("桥架", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateFloorByProfileTool.cs（元素创建）
internal sealed class CreateFloorByProfileImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ShapeModelingService ?? throw new InvalidOperationException("无法获取 ShapeModelingService");
        var document = ShapeArgs.RequireDocument(adapter, context);

        var items = ModelingArgs.Combine(context, "floors");
        var specs = new List<ProfileSpec>();
        foreach (var item in items)
        {
            var points = ShapeArgs.ReadPoints(item, "points");
            if (points.Count < 3)
            {
                return Task.FromResult(AIToolResult.Fail("每个楼板都需要 points（至少 3 个点，格式 [{x,y}] 或 [[x,y]]）。"));
            }

            specs.Add(new ProfileSpec(
                points,
                ModelingArgs.OptionalInt(item, "level_id", "levelId"),
                ModelingArgs.OptionalInt(item, "floor_type_id", "floorTypeId")));
        }

        var result = service.CreateFloors(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("楼板", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateFootprintRoofTool.cs（元素创建）
internal sealed class CreateFootprintRoofImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ShapeModelingService ?? throw new InvalidOperationException("无法获取 ShapeModelingService");
        var document = ShapeArgs.RequireDocument(adapter, context);

        var item = new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase);
        var points = ShapeArgs.ReadPoints(item, "points");
        if (points.Count < 3)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 points（至少 3 个点）。"));
        }

        var levelId = ModelingArgs.OptionalInt(item, "level_id", "levelId");
        if (levelId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 level_id。"));
        }

        var spec = new ProfileSpec(points, levelId, ModelingArgs.OptionalInt(item, "roof_type_id", "roofTypeId"));
        var result = service.CreateFootprintRoofs(document, new List<ProfileSpec> { spec });
        return Task.FromResult(CreateGridImplementation.Result("足迹屋顶", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateExtrusionRoofTool.cs（元素创建）
internal sealed class CreateExtrusionRoofImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ShapeModelingService ?? throw new InvalidOperationException("无法获取 ShapeModelingService");
        var document = ShapeArgs.RequireDocument(adapter, context);

        var item = new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase);
        var points = ShapeArgs.ReadPoints(item, "points");
        if (points.Count < 3)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 points（至少 3 个点，位于 XZ 立面）。"));
        }

        var levelId = ModelingArgs.OptionalInt(item, "level_id", "levelId");
        if (levelId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 level_id。"));
        }

        var start = ModelingArgs.Number(item, "extrusion_start", 0);
        var end = ModelingArgs.Number(item, "extrusion_end", 6000);
        var roofId = service.CreateExtrusionRoof(
            document,
            points,
            levelId.Value,
            ModelingArgs.OptionalInt(item, "roof_type_id", "roofTypeId"),
            start,
            end);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建拉伸屋顶 {roofId}（沿 Y 拉伸 {start}→{end} mm）",
            new { roofId, levelId = levelId.Value, extrusionStart = start, extrusionEnd = end, profilePoints = points.Count }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateDimensionByElementsTool.cs（注释与标记）
internal sealed class CreateDimensionByElementsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ShapeModelingService ?? throw new InvalidOperationException("无法获取 ShapeModelingService");
        var document = ShapeArgs.RequireDocument(adapter, context);

        var viewId = WriteArgs.OptionalInt(context, "viewId", "view_id");
        if (viewId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 viewId。"));
        }

        var elementIds = WriteArgs.Ids(context, "elementIds", "element_ids");
        if (elementIds.Count < 2)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供至少 2 个 elementIds。"));
        }

        var offsetX = 0d;
        var offsetY = 0d;
        if (context.Parameters.TryGetValue("position", out var position) && position is Dictionary<string, object?> map)
        {
            offsetX = ModelingArgs.Number(map, "x");
            offsetY = ModelingArgs.Number(map, "y");
        }

        var autoOffset = ModelingArgs.Number(
            new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase), "autoOffsetDistance", 0);
        if (autoOffset != 0)
        {
            offsetY += autoOffset;
        }

        var dimensionId = service.CreateDimensionByElements(document, viewId.Value, elementIds, offsetX, offsetY);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建尺寸标注 {dimensionId}（{elementIds.Count} 个元素）",
            new { dimensionId, viewId = viewId.Value, elementIds, offsetXmm = offsetX, offsetYmm = offsetY }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\Create3DViewTool.cs（视图创建）
internal sealed class Create3DViewImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ShapeModelingService ?? throw new InvalidOperationException("无法获取 ShapeModelingService");
        var document = ShapeArgs.RequireDocument(adapter, context);

        var elementIds = WriteArgs.Ids(context, "elementIds", "element_ids", "elementId");
        if (elementIds.Count == 0)
        {
            var cacheId = WriteArgs.Text(context, "cacheId", "cache_id");
            if (!string.IsNullOrWhiteSpace(cacheId) &&
                context.GetCachedData<List<Dictionary<string, object?>>>(cacheId) is { } cached)
            {
                elementIds.AddRange(ToolArgumentReader.ExtractIds(cached));
            }
        }

        var viewName = WriteArgs.Text(context, "viewName", "view_name", "name");
        var viewId = service.Create3DView(document, viewName, elementIds);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建三维视图 {viewId}" + (elementIds.Count > 0 ? $"（剖面框聚焦 {elementIds.Count} 个元素）" : string.Empty),
            new { viewId, viewName, focusedElements = elementIds.Count }));
    }
}
