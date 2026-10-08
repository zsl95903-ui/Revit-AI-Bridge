using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 4 第三刀实现：元素操作（复制/旋转/镜像/阵列/拆分/删除/连接/剪切）。
/// </summary>
public static class ElementOpsImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("copy_element", new CopyElementImplementation());
        ToolImplementationRegistry.Register("rotate_element", new RotateElementImplementation());
        ToolImplementationRegistry.Register("mirror_element", new MirrorElementImplementation());
        ToolImplementationRegistry.Register("array_element", new ArrayElementImplementation());
        ToolImplementationRegistry.Register("split_element", new SplitElementImplementation());
        ToolImplementationRegistry.Register("delete_elements", new DeleteElementsImplementation());
        ToolImplementationRegistry.Register("join_geometry", new JoinGeometryImplementation());
        ToolImplementationRegistry.Register("cut_geometry", new CutGeometryImplementation());
    }
}

internal static class ElementOpsArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    /// <summary>从 elementIds/elementId/cacheId 解析元素 id。</summary>
    public static List<int> ResolveIds(AIToolContext context, params string[] keys)
    {
        var all = keys.Length > 0 ? keys : new[] { "elementIds", "element_ids", "elementId", "element_id" };
        var ids = WriteArgs.Ids(context, all);

        if (ids.Count == 0)
        {
            var cacheKeys = new[] { "cacheId", "cache_id" };
            foreach (var cacheKey in cacheKeys)
            {
                var cacheId = WriteArgs.Text(context, cacheKey);
                if (string.IsNullOrWhiteSpace(cacheId))
                {
                    continue;
                }

                if (context.GetCachedData<List<Dictionary<string, object?>>>(cacheId) is { } cached)
                {
                    ids.AddRange(ToolArgumentReader.ExtractIds(cached));
                }
                else if (context.GetCachedData<List<int>>(cacheId) is { } cachedIds)
                {
                    ids.AddRange(cachedIds);
                }

                break;
            }
        }

        return ids.Distinct().ToList();
    }

    public static AIToolResult Report(string what, TransformResult result, int? extraId = null)
    {
        var payload = new
        {
            requested = result.Requested,
            succeeded = result.Succeeded,
            resultIds = result.ResultIds,
            elementId = extraId,
            failures = result.Failures,
        };

        return result.Succeeded > 0
            ? AIToolResult.Ok($"✅ {what} 完成 {result.Succeeded}/{result.Requested}", payload)
            : AIToolResult.Fail($"{what} 未成功：{string.Join("；", result.Failures)}", payload);
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CopyElementTool.cs（元素修改）
internal sealed class CopyElementImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementOpsService ?? throw new InvalidOperationException("无法获取 ElementOpsService");
        var document = ElementOpsArgs.RequireDocument(adapter, context);

        var ids = ElementOpsArgs.ResolveIds(context);
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId、elementIds 或 cacheId。"));
        }

        var result = service.CopyElements(
            document,
            ids,
            WriteArgs.Number(context, 0, "x", "dx"),
            WriteArgs.Number(context, 0, "y", "dy"),
            WriteArgs.Number(context, 0, "z", "dz"));

        return Task.FromResult(ElementOpsArgs.Report($"复制元素（偏移 x={WriteArgs.Number(context, 0, "x")} y={WriteArgs.Number(context, 0, "y")} z={WriteArgs.Number(context, 0, "z")} mm）", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\RotateElementTool.cs（元素修改）
internal sealed class RotateElementImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementOpsService ?? throw new InvalidOperationException("无法获取 ElementOpsService");
        var document = ElementOpsArgs.RequireDocument(adapter, context);

        var ids = ElementOpsArgs.ResolveIds(context);
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId、elementIds 或 cacheId。"));
        }

        var angle = WriteArgs.Number(context, double.NaN, "angleDegrees", "angle_degrees", "angle");
        if (double.IsNaN(angle))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 angleDegrees。"));
        }

        var result = service.RotateElements(
            document,
            ids,
            WriteArgs.Number(context, 0, "axisX", "axis_x"),
            WriteArgs.Number(context, 0, "axisY", "axis_y"),
            WriteArgs.Number(context, 1, "axisZ", "axis_z"),
            angle,
            WriteArgs.Number(context, 0, "originX", "origin_x"),
            WriteArgs.Number(context, 0, "originY", "origin_y"),
            WriteArgs.Number(context, 0, "originZ", "origin_z"));

        return Task.FromResult(ElementOpsArgs.Report($"旋转元素（{angle}°）", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\MirrorElementTool.cs（元素修改）
internal sealed class MirrorElementImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementOpsService ?? throw new InvalidOperationException("无法获取 ElementOpsService");
        var document = ElementOpsArgs.RequireDocument(adapter, context);

        var ids = ElementOpsArgs.ResolveIds(context);
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId、elementIds 或 cacheId。"));
        }

        var mirrorType = WriteArgs.Text(context, "mirrorType", "mirror_type") ?? "line";
        var result = service.MirrorElements(
            document,
            ids,
            mirrorType,
            WriteArgs.Number(context, 0, "lineStartX", "line_start_x"),
            WriteArgs.Number(context, 0, "lineStartY", "line_start_y"),
            WriteArgs.Number(context, 0, "lineEndX", "line_end_x"),
            WriteArgs.Number(context, 0, "lineEndY", "line_end_y"),
            WriteArgs.Number(context, 0, "pointX", "point_x"),
            WriteArgs.Number(context, 0, "pointY", "point_y"));

        return Task.FromResult(ElementOpsArgs.Report($"镜像元素（{mirrorType}）", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ArrayElementTool.cs（元素修改）
internal sealed class ArrayElementImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementOpsService ?? throw new InvalidOperationException("无法获取 ElementOpsService");
        var document = ElementOpsArgs.RequireDocument(adapter, context);

        var elementId = WriteArgs.OptionalInt(context, "element_id", "elementId");
        if (elementId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 element_id。"));
        }

        var count = (int)WriteArgs.Number(context, 0, "count");
        if (count < 2)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 count（至少 2）。"));
        }

        var arrayType = WriteArgs.Text(context, "array_type", "arrayType") ?? "linear";
        var result = service.ArrayElements(
            document,
            elementId.Value,
            arrayType,
            count,
            WriteArgs.Number(context, 0, "move_x", "moveX"),
            WriteArgs.Number(context, 0, "move_y", "moveY"),
            WriteArgs.Number(context, 0, "move_z", "moveZ"),
            WriteArgs.Number(context, 0, "origin_x", "originX"),
            WriteArgs.Number(context, 0, "origin_y", "originY"),
            WriteArgs.Number(context, 0, "origin_z", "originZ"));

        return Task.FromResult(ElementOpsArgs.Report($"阵列元素（{arrayType} × {count}）", result, elementId));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SplitElementTool.cs（元素修改）
internal sealed class SplitElementImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementOpsService ?? throw new InvalidOperationException("无法获取 ElementOpsService");
        var document = ElementOpsArgs.RequireDocument(adapter, context);

        // 注意：厂商 schema 里缓存键叫 elementCacheId。
        var ids = ElementOpsArgs.ResolveIds(context, "elementIds", "element_ids", "elementId", "element_id");
        if (ids.Count == 0)
        {
            var cacheId = WriteArgs.Text(context, "elementCacheId", "element_cache_id");
            if (!string.IsNullOrWhiteSpace(cacheId) &&
                context.GetCachedData<List<Dictionary<string, object?>>>(cacheId) is { } cached)
            {
                ids.AddRange(ToolArgumentReader.ExtractIds(cached));
            }
        }

        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId/elementIds 或 elementCacheId。"));
        }

        var splitMode = WriteArgs.Text(context, "splitMode", "split_mode");
        if (string.IsNullOrWhiteSpace(splitMode))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 splitMode（point / points / length / parameter）。"));
        }

        double? length = context.Parameters.ContainsKey("length") ? WriteArgs.Number(context, 0, "length") : null;
        var points = ShapeArgs.ReadPoints(new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase), "points");
        var parameters = new List<double>();
        if (context.Parameters.TryGetValue("parameters", out var raw) && raw is List<object?> list)
        {
            parameters.AddRange(list.Where(item => item is not null).Select(ToolArgumentReader.ToDouble));
        }

        var splits = 0;
        var failures = new List<string>();
        foreach (var id in ids)
        {
            try
            {
                splits += service.SplitElement(document, id, splitMode!, length, points, parameters);
            }
            catch (Exception ex)
            {
                failures.Add($"{id}: {ex.GetBaseException().Message}");
            }
        }

        return Task.FromResult(splits > 0
            ? AIToolResult.Ok(
                $"✅ 已拆分 {splits} 处（{string.Join('/', ids)}，模式 {splitMode}）",
                new { elementIds = ids, splitMode, splits, autoCreateFittings = WriteArgs.Flag(context, false, "autoCreateFittings", "auto_create_fittings"), failures })
            : AIToolResult.Fail(
                $"拆分失败：{string.Join("；", failures)}",
                new { elementIds = ids, splitMode, splits, failures }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\DeleteElementsTool.cs（元素修改）
internal sealed class DeleteElementsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementOpsService ?? throw new InvalidOperationException("无法获取 ElementOpsService");
        var document = ElementOpsArgs.RequireDocument(adapter, context);

        var ids = ElementOpsArgs.ResolveIds(context);
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId、elementIds 或 cacheId。"));
        }

        var deleted = service.DeleteElements(document, ids);
        return Task.FromResult(deleted > 0
            ? AIToolResult.Ok($"✅ 已删除 {deleted} 个元素（请求 {ids.Count} 个）", new { requested = ids.Count, deleted, elementIds = ids })
            : AIToolResult.Fail("没有元素被删除（可能元素不存在或不可删除）。", new { requested = ids.Count, deleted = 0 }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\JoinGeometryTool.cs（几何操作）
internal sealed class JoinGeometryImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementOpsService ?? throw new InvalidOperationException("无法获取 ElementOpsService");
        var document = ElementOpsArgs.RequireDocument(adapter, context);

        var first = ElementOpsArgs.ResolveIds(context, "element1Ids", "element1_ids", "element1Id", "element1_id");
        var second = ElementOpsArgs.ResolveIds(context, "element2Ids", "element2_ids", "element2Id", "element2_id");
        var operation = WriteArgs.Text(context, "operation") ?? "join";

        var affected = service.JoinGeometry(document, first, second, operation);
        return Task.FromResult(affected > 0
            ? AIToolResult.Ok($"✅ 已处理 {affected} 对连接的组合（{operation}）", new { operation, affected, first, second })
            : AIToolResult.Fail($"没有组合被处理（{operation}）；请确认两组元素都存在且可连接。", new { operation, affected = 0 }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CutGeometryTool.cs（几何操作）
internal sealed class CutGeometryImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementOpsService ?? throw new InvalidOperationException("无法获取 ElementOpsService");
        var document = ElementOpsArgs.RequireDocument(adapter, context);

        var toCut = ElementOpsArgs.ResolveIds(context, "elementToCutIds", "element_to_cut_ids", "elementToCutId", "element_to_cut_id");
        var cutting = ElementOpsArgs.ResolveIds(context, "cuttingElementIds", "cutting_element_ids", "cuttingElementId", "cutting_element_id");
        var operation = WriteArgs.Text(context, "operation") ?? "cut";

        var affected = service.CutGeometry(document, toCut, cutting, operation);
        return Task.FromResult(affected > 0
            ? AIToolResult.Ok($"✅ 已处理 {affected} 对剪切（{operation}）", new { operation, affected, toCut, cutting })
            : AIToolResult.Fail($"没有剪切被处理（{operation}）；请确认元素满足剪切条件（实心/空心族实例）。", new { operation, affected = 0 }));
    }
}
