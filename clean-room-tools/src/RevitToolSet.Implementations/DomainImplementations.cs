using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 5 第一批实现：地形、保温、MEP 系统。
/// </summary>
public static class DomainImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("create_topography_surface", new CreateTopographySurfaceImplementation());
        ToolImplementationRegistry.Register("create_topography_from_floors", new CreateTopographyFromFloorsImplementation());
        ToolImplementationRegistry.Register("insulation_manager", new InsulationManagerImplementation());
        ToolImplementationRegistry.Register("insulation_query", new InsulationQueryImplementation());
        ToolImplementationRegistry.Register("mep_system_manager", new MepSystemManagerImplementation());
        ToolImplementationRegistry.Register("mep_system_query", new MepSystemQueryImplementation());
    }
}

internal static class DomainArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    public static List<int> ResolveIds(AIToolContext context, params string[] keys)
    {
        var all = keys.Length > 0 ? keys : new[] { "element_ids", "elementIds", "element_id", "elementId" };
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

    /// <summary>支持 points: [{x,y,z}] 与 points: [[x,y,z]]。</summary>
    public static List<Point3> ReadPoints3(AIToolContext context, string key)
    {
        var points = new List<Point3>();
        if (!context.Parameters.TryGetValue(key, out var value) || value is not List<object?> list)
        {
            return points;
        }

        foreach (var entry in list)
        {
            switch (entry)
            {
                case Dictionary<string, object?> map:
                    points.Add(new Point3(
                        ModelingArgs.Number(map, "x"),
                        ModelingArgs.Number(map, "y"),
                        ModelingArgs.Number(map, "z")));
                    break;
                case List<object?> triple when triple.Count >= 3:
                    points.Add(new Point3(
                        ToolArgumentReader.ToDouble(triple[0]),
                        ToolArgumentReader.ToDouble(triple[1]),
                        ToolArgumentReader.ToDouble(triple[2])));
                    break;
            }
        }

        return points;
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateTopographySurfaceTool.cs（地形）
internal sealed class CreateTopographySurfaceImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.TopographyService ?? throw new InvalidOperationException("无法获取 TopographyService");
        var document = DomainArgs.RequireDocument(adapter, context);

        var points = DomainArgs.ReadPoints3(context, "points");
        if (points.Count < 3)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 points（至少 3 个点，格式 [{x,y,z}] 或 [[x,y,z]]，单位毫米）。"));
        }

        var topographyId = service.CreateTopography(document, points);
        var minZ = points.Min(point => point.Z);
        var maxZ = points.Max(point => point.Z);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建地形 {topographyId}（{points.Count} 个点，高程 {minZ}–{maxZ} mm）",
            new { topographyId, pointCount = points.Count, minElevationMM = minZ, maxElevationMM = maxZ }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateTopographyFromFloorsTool.cs（地形）
internal sealed class CreateTopographyFromFloorsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.TopographyService ?? throw new InvalidOperationException("无法获取 TopographyService");
        var document = DomainArgs.RequireDocument(adapter, context);

        var topographyId = WriteArgs.OptionalInt(context, "topography_element_id", "topographyElementId");
        if (topographyId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 topography_element_id（目标地形元素）。"));
        }

        var floorIds = DomainArgs.ResolveIds(context, "floor_element_ids", "floorElementIds", "floor_element_id", "floorElementId");
        if (floorIds.Count == 0)
        {
            var cacheId = WriteArgs.Text(context, "floor_cache_id", "floorCacheId");
            if (!string.IsNullOrWhiteSpace(cacheId) &&
                context.GetCachedData<List<Dictionary<string, object?>>>(cacheId) is { } cached)
            {
                floorIds.AddRange(ToolArgumentReader.ExtractIds(cached));
            }
        }

        if (floorIds.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 floor_element_id/floor_element_ids 或 floor_cache_id。"));
        }

        var confirmed = WriteArgs.Flag(context, false, "confirmed");
        if (!confirmed)
        {
            return Task.FromResult(AIToolResult.Fail(
                $"该操作会向地形 {topographyId} 追加 {floorIds.Count} 个楼板的轮廓点（可能改变既有地形形状）；"
                + "请在确认后带上 confirmed=true 重新调用。"));
        }

        var info = service.AddPointsFromFloors(document, topographyId.Value, floorIds);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 地形 {info.ElementId} 现有 {info.PointCount} 个点（本次新增 {info.AddedPoints} 个），高程 {info.MinElevationMm}–{info.MaxElevationMm} mm",
            new
            {
                topographyId = info.ElementId,
                pointCount = info.PointCount,
                addedPoints = info.AddedPoints,
                minElevationMM = info.MinElevationMm,
                maxElevationMM = info.MaxElevationMm,
                floorElementIds = floorIds,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\InsulationManagerTool.cs（保温）
internal sealed class InsulationManagerImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.InsulationService ?? throw new InvalidOperationException("无法获取 InsulationService");
        var document = DomainArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "add").Trim().ToLowerInvariant();
        if (operation is not ("add" or "apply" or "update" or "replace" or "remove" or "delete"))
        {
            return Task.FromResult(AIToolResult.Fail("operation 只支持 add / update / remove。"));
        }

        var ids = DomainArgs.ResolveIds(context);
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 element_ids 或 cacheId。"));
        }

        double? thickness = context.Parameters.ContainsKey("thickness") ? WriteArgs.Number(context, 0, "thickness") : null;
        var material = WriteArgs.Text(context, "material", "materialName");
        var overrideExisting = WriteArgs.Flag(context, false, "override_existing", "overrideExisting")
                               || operation is "update" or "replace";

        var affected = service.ManageInsulation(document, operation, ids, thickness, material, overrideExisting);

        return Task.FromResult(affected > 0
            ? AIToolResult.Ok(
                $"✅ 已对 {affected} 个元素（其类型）执行保温 {operation}"
                + (thickness is not null ? $"，厚度 {thickness} mm" : string.Empty),
                new
                {
                    operation,
                    affected,
                    requested = ids.Count,
                    thicknessMM = thickness,
                    material,
                    overrideExisting,
                    systemTypeId = WriteArgs.OptionalInt(context, "system_type_id", "systemTypeId"),
                })
            : AIToolResult.Fail(
                $"没有元素被修改（{operation}）：请确认元素带有复合层结构（墙/楼板/屋顶/天花板），"
                + "且 add 时目标类型尚未有保温层（已有保温层需 override_existing=true）。",
                new { operation, affected = 0, requested = ids.Count }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\InsulationQueryTool.cs（保温）
internal sealed class InsulationQueryImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.InsulationService ?? throw new InvalidOperationException("无法获取 InsulationService");
        var document = DomainArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "list").Trim().ToLowerInvariant();
        var onlyUninsulated = WriteArgs.Flag(context, false, "only_uninsulated", "onlyUninsulated")
                              || operation == "uninsulated";

        var infos = service.QueryInsulation(
            document,
            DomainArgs.ResolveIds(context),
            onlyUninsulated,
            (int)WriteArgs.Number(context, 200, "limit"));

        var insulated = infos.Count(info => info.HasInsulation);
        var totalThickness = Math.Round(infos.Where(info => info.HasInsulation).Sum(info => info.ThicknessMm), 2);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 保温查询：{infos.Count} 条层记录，其中保温层 {insulated} 条，总厚度 {totalThickness} mm",
            new
            {
                operation,
                target = WriteArgs.Text(context, "target"),
                onlyUninsulated,
                recordCount = infos.Count,
                insulatedCount = insulated,
                totalThicknessMM = totalThickness,
                records = infos.Select(info => new
                {
                    elementId = info.ElementId,
                    elementName = info.ElementName,
                    typeName = info.TypeName,
                    hasInsulation = info.HasInsulation,
                    thicknessMM = info.ThicknessMm,
                    materialName = info.MaterialName,
                    layerIndex = info.LayerIndex,
                }).ToArray(),
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\MepSystemManagerTool.cs（管网）
internal sealed class MepSystemManagerImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.MepSystemService ?? throw new InvalidOperationException("无法获取 MepSystemService");
        var document = DomainArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "create").Trim().ToLowerInvariant();
        var systemTypeId = WriteArgs.OptionalInt(context, "system_type_id", "systemTypeId", "source_system_type_id");
        var newName = WriteArgs.Text(context, "new_system_name", "new_name", "name");
        var force = WriteArgs.Flag(context, false, "force");

        switch (operation)
        {
            case "create":
            {
                if (systemTypeId is null)
                {
                    return Task.FromResult(AIToolResult.Fail("create 需要 system_type_id。"));
                }

                var system = service.CreateSystem(document, systemTypeId.Value, newName);
                return Task.FromResult(system is null
                    ? AIToolResult.Fail("创建系统失败。")
                    : AIToolResult.Ok(
                        $"✅ 已创建系统 {system.SystemId}（{system.Name}，类型 {system.SystemTypeName}）",
                        new { systemId = system.SystemId, name = system.Name, systemTypeName = system.SystemTypeName, kind = system.Kind }));
            }

            case "rename":
            {
                if (systemTypeId is null)
                {
                    return Task.FromResult(AIToolResult.Fail("rename 需要 system_type_id 作为目标系统 id（或改用 mep_system_query 查回 id）。"));
                }

                if (string.IsNullOrWhiteSpace(newName))
                {
                    return Task.FromResult(AIToolResult.Fail("rename 需要 new_name。"));
                }

                var changed = service.RenameSystem(document, systemTypeId.Value, newName!);
                return Task.FromResult(AIToolResult.Ok($"✅ 系统 {systemTypeId} 已重命名为 {newName}", new { systemId = systemTypeId.Value, newName, changed }));
            }

            case "delete":
            case "remove":
            {
                if (systemTypeId is null)
                {
                    return Task.FromResult(AIToolResult.Fail("delete 需要 system_type_id 作为目标系统 id。"));
                }

                var deleted = service.DeleteSystem(document, systemTypeId.Value, force);
                return Task.FromResult(deleted
                    ? AIToolResult.Ok($"✅ 已删除系统 {systemTypeId}", new { systemId = systemTypeId.Value, deleted = true })
                    : AIToolResult.Fail($"系统 {systemTypeId} 未被删除。"));
            }

            default:
                return Task.FromResult(AIToolResult.Fail("operation 只支持 create / rename / delete。"));
        }
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\MepSystemQueryTool.cs（管网）
internal sealed class MepSystemQueryImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.MepSystemService ?? throw new InvalidOperationException("无法获取 MepSystemService");
        var document = DomainArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "list").Trim().ToLowerInvariant();
        var systemTypeId = WriteArgs.OptionalInt(context, "system_type_id", "systemTypeId");
        var onlyUninsulated = WriteArgs.Flag(context, false, "only_uninsulated", "onlyUninsulated");
        var includeSizes = WriteArgs.Flag(context, true, "include_size_distribution", "includeSizeDistribution");
        var includeBreakdown = WriteArgs.Flag(context, false, "include_category_breakdown", "includeCategoryBreakdown");
        var includeFittings = WriteArgs.Flag(context, false, "include_fittings", "includeFittings");

        var systems = service.QuerySystems(
            document,
            operation,
            systemTypeId,
            onlyUninsulated,
            (int)WriteArgs.Number(context, 200, "limit"));

        var memberTotal = systems.Sum(system => system.MemberCount);
        var uninsulatedTotal = systems.Sum(system => system.UninsulatedMemberCount);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 共 {systems.Count} 个 MEP 系统，成员 {memberTotal} 个，未保温 {uninsulatedTotal} 个",
            new
            {
                operation,
                target = WriteArgs.Text(context, "target"),
                systemCount = systems.Count,
                memberTotal,
                uninsulatedTotal,
                includeFittings,
                includeCategoryBreakdown = includeBreakdown,
                systems = systems.Select(system => new
                {
                    systemId = system.SystemId,
                    name = system.Name,
                    systemTypeName = system.SystemTypeName,
                    kind = system.Kind,
                    memberCount = system.MemberCount,
                    uninsulatedMemberCount = system.UninsulatedMemberCount,
                    memberIds = WriteArgs.Flag(context, false, "include_member_ids", "includeMemberIds") ? system.MemberIds : null,
                    sizeDistribution = includeSizes ? system.SizeDistribution : null,
                }).ToArray(),
                note = "include_fittings/include_category_breakdown 目前只回显开关；成员明细与管径分布已给出。",
            }));
    }
}
