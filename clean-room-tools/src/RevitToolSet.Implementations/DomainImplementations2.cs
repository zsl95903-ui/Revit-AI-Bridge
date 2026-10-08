using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 收尾批实现：道路（墙→路面板）、CAD 图层管理、剪切顺序修正、UI 选择/缩放/激活视图。
/// </summary>
public static class DomainImplementations2
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("create_road_from_walls", new CreateRoadFromWallsImplementation());
        ToolImplementationRegistry.Register("cad_layer_manager", new CadLayerManagerImplementation());
        ToolImplementationRegistry.Register("fix_cutting_order", new FixCuttingOrderImplementation());
        ToolImplementationRegistry.Register("select_elements", new SelectElementsImplementation());
        ToolImplementationRegistry.Register("clear_selection", new ClearSelectionImplementation());
        ToolImplementationRegistry.Register("zoom_to_elements", new ZoomToElementsImplementation());
        ToolImplementationRegistry.Register("activate_view", new ActivateViewImplementation());
    }
}

internal static class UiArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    public static object RequireUiDocument(IRevitAdapter adapter)
        => adapter.GetActiveUiDocument()
           ?? throw new InvalidOperationException("当前没有可用的 UIDocument（请在 Revit 界面会话里执行该工具）。");

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
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateRoadFromWallsTool.cs（道路）
internal sealed class CreateRoadFromWallsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.DomainExtraService ?? throw new InvalidOperationException("无法获取 DomainExtraService");
        var document = UiArgs.RequireDocument(adapter, context);

        var wallIds = UiArgs.ResolveIds(context, "wall_element_ids", "wallElementIds", "wallIds");
        if (wallIds.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 wall_element_ids。"));
        }

        var result = service.CreateRoadFromWalls(
            document,
            wallIds,
            OptionalDouble(context, "lane_width_meters", "laneWidthMeters"),
            OptionalDouble(context, "sidewalk_width_meters", "sidewalkWidthMeters"),
            OptionalDouble(context, "default_road_width_meters", "defaultRoadWidthMeters"));

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已按 {result.WallCount} 面墙生成路面板 {result.FloorId}（轮廓 {result.Outline.Count} 点，{(result.ClosedLoop ? "闭合环" : "包围盒")}）",
            new
            {
                floorId = result.FloorId,
                wallCount = result.WallCount,
                closedLoop = result.ClosedLoop,
                outline = result.Outline.Select(point => new { x = point.X, y = point.Y }).ToArray(),
                unit = "millimeters",
                notes = result.Notes,
            }));
    }

    private static double? OptionalDouble(AIToolContext context, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (context.Parameters.ContainsKey(key))
            {
                return WriteArgs.Number(context, 0, key);
            }
        }

        return null;
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CadLayerManagerTool.cs（CAD）
internal sealed class CadLayerManagerImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.DomainExtraService ?? throw new InvalidOperationException("无法获取 DomainExtraService");
        var document = UiArgs.RequireDocument(adapter, context);

        var operation = WriteArgs.Text(context, "operation");
        var linkId = WriteArgs.OptionalInt(context, "linkId", "link_id");
        if (string.IsNullOrWhiteSpace(operation) || linkId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 operation 与 linkId。"));
        }

        var viewId = WriteArgs.OptionalInt(context, "viewId", "view_id", "view_id") ?? 0;
        var layers = service.ManageCadLayers(document, viewId, linkId.Value, operation!, WriteArgs.Text(context, "layerName", "layer_name"));

        return Task.FromResult(AIToolResult.Ok(
            $"✅ CAD 图层操作 {operation}：涉及 {layers.Count} 个图层",
            new
            {
                operation,
                linkId = linkId.Value,
                count = layers.Count,
                layers = layers.Select(layer => new { name = layer.Name, categoryId = layer.CategoryId, visible = layer.Visible }).ToArray(),
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\FixCuttingOrderTool.cs（几何操作）
internal sealed class FixCuttingOrderImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.DomainExtraService ?? throw new InvalidOperationException("无法获取 DomainExtraService");
        var document = UiArgs.RequireDocument(adapter, context);

        var cuttingIds = UiArgs.ResolveIds(context, "cuttingElementIds", "cutting_element_ids");
        var toCutIds = UiArgs.ResolveIds(context, "elementToCutIds", "element_to_cut_ids");
        var dryRun = WriteArgs.Flag(context, false, "dryRun", "dry_run");
        var includeDetails = WriteArgs.Flag(context, false, "includeDetails", "include_details");

        var result = service.FixCuttingOrder(document, cuttingIds, toCutIds, dryRun);

        return Task.FromResult(AIToolResult.Ok(
            dryRun
                ? $"✅ 已检查 {result.Checked} 对连接（dryRun），其中 {result.Details.Count} 对需要修正"
                : $"✅ 已检查 {result.Checked} 对连接，修正 {result.Fixed} 对剪切顺序",
            new
            {
                checkedPairs = result.Checked,
                fixedPairs = result.Fixed,
                dryRun,
                details = includeDetails || dryRun ? result.Details : null,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SelectElementsTool.cs（UI 交互）
internal sealed class SelectElementsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewInteractionService ?? throw new InvalidOperationException("无法获取 ViewInteractionService");
        var uiDocument = UiArgs.RequireUiDocument(adapter);

        var ids = UiArgs.ResolveIds(context);
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementIds 或 cacheId。"));
        }

        var result = service.SelectElements(
            uiDocument,
            ids,
            WriteArgs.Flag(context, false, "append"),
            WriteArgs.Flag(context, false, "zoomToFit", "zoom_to_fit"));

        return Task.FromResult(AIToolResult.Ok(
            $"✅ {result.Message}",
            new { selected = result.Selected, zoomed = result.Zoomed, requested = ids.Count }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ClearSelectionTool.cs（UI 交互）
internal sealed class ClearSelectionImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewInteractionService ?? throw new InvalidOperationException("无法获取 ViewInteractionService");
        var uiDocument = UiArgs.RequireUiDocument(adapter);

        var result = service.ClearSelection(uiDocument);
        return Task.FromResult(AIToolResult.Ok($"✅ {result.Message}", new { cleared = result.Cleared }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ZoomToElementsTool.cs（UI 交互）
internal sealed class ZoomToElementsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewInteractionService ?? throw new InvalidOperationException("无法获取 ViewInteractionService");
        var uiDocument = UiArgs.RequireUiDocument(adapter);

        var ids = UiArgs.ResolveIds(context);
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementIds 或 cacheId。"));
        }

        var result = service.ZoomToElements(uiDocument, ids);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ {result.Message}",
            new { zoomed = result.Zoomed, requested = ids.Count, fitFactor = context.Parameters.ContainsKey("fitFactor") ? WriteArgs.Number(context, 1, "fitFactor") : (double?)null }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ActivateViewTool.cs（UI 交互）
internal sealed class ActivateViewImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewInteractionService ?? throw new InvalidOperationException("无法获取 ViewInteractionService");
        var uiDocument = UiArgs.RequireUiDocument(adapter);

        var viewId = WriteArgs.OptionalInt(context, "viewId", "view_id");
        if (viewId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 viewId。"));
        }

        var result = service.ActivateView(uiDocument, viewId.Value);
        return Task.FromResult(AIToolResult.Ok($"✅ {result.Message}", new { viewId = viewId.Value, activated = true }));
    }
}
