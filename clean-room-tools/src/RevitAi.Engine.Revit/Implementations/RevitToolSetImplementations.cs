using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;
using RevitAi.Engine.Revit.Tools;
using RevitToolSet;

namespace RevitAi.Engine.Revit.Implementations;

/// <summary>
/// RevitToolSet 的自研实现注册入口。工具类（120 个）由生成器产出，这里提供"真实现"。
///
/// 命名空间/命名约定：新工具集统一用 <b>RevitToolSet</b> 前缀（替代原 AS.Tools / RevitAI.Vendor
/// 的命名），工具名（function name）保持与厂商 120 项完全一致，以便 1:1 替换。
/// 每个实现类都标注了对应的反编译对照位置。
/// </summary>
public static class RevitToolSetImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("get_all_levels", new GetAllLevelsImplementation());
        ToolImplementationRegistry.Register("create_level", new CreateLevelImplementation());
        ToolImplementationRegistry.Register("move_elements", new MoveElementsImplementation());
        ToolImplementationRegistry.Register("upsert_level", new UpsertLevelImplementation());
    }
}

// ---------------------------------------------------------------------------------------------
// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAllLevelsTool.cs:80-123
// 语义：逐标高输出 id / name / elevation（英尺→米，3 位小数），消息 "✅ 成功获取 N 个标高"
// ---------------------------------------------------------------------------------------------
internal sealed class GetAllLevelsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var levelService = adapter.LevelService ?? throw new InvalidOperationException("无法获取 LevelService");
        var document = ToolContextAccess.Document(context, adapter);
        var units = ToolContextAccess.Units(context);

        var levels = new List<object>();
        foreach (var level in levelService.GetAllLevels(document))
        {
            levels.Add(new Dictionary<string, object?>
            {
                ["id"] = levelService.GetLevelId(level),
                ["name"] = levelService.GetLevelName(level),
                ["elevation"] = units.Round(units.FeetToMeters(levelService.GetLevelElevationFeet(level)), 3),
            });
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {levels.Count} 个标高",
            new { levels, unit = "meters", count = levels.Count }));
    }
}

// ---------------------------------------------------------------------------------------------
// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateLevelTool.cs（ICreationService + ILevelService）
// 自研对照：RevitAiBatch\ToolDispatcher.cs:553-589 CreateLevel（本实现补齐了数组入参）
// ---------------------------------------------------------------------------------------------
internal sealed class CreateLevelImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var levelService = adapter.LevelService ?? throw new InvalidOperationException("无法获取 LevelService");
        var document = ToolContextAccess.Document(context, adapter);
        var units = ToolContextAccess.Units(context);

        var parameters = new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase);
        var elevations = ToolArgumentReader.ReadNumbers(parameters, "elevation");
        if (elevations.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elevation（米）。"));
        }

        var names = ToolArgumentReader.ReadStrings(parameters, "levelName");
        if (names.Count > 0 && names.Count != elevations.Count)
        {
            return Task.FromResult(AIToolResult.Fail("levelName 数组长度必须与 elevation 数组一致。"));
        }

        var toleranceFeet = units.MmToFeet(1.0);
        var results = new List<object>();

        for (var i = 0; i < elevations.Count; i++)
        {
            var meters = elevations[i];
            var name = names.Count > i ? names[i] : $"Level {meters:0.###}";
            var feet = units.MetersToFeet(meters);

            var existing = levelService.FindLevel(document, name, feet, toleranceFeet);
            if (existing is not null)
            {
                results.Add(new Dictionary<string, object?>
                {
                    ["id"] = levelService.GetLevelId(existing),
                    ["name"] = levelService.GetLevelName(existing),
                    ["elevation"] = units.Round(units.FeetToMeters(levelService.GetLevelElevationFeet(existing)), 6),
                    ["existing"] = true,
                });
                continue;
            }

            var created = levelService.CreateLevel(document, name, feet, buildingStory: true);
            results.Add(new Dictionary<string, object?>
            {
                ["id"] = levelService.GetLevelId(created),
                ["name"] = levelService.GetLevelName(created),
                ["elevation"] = units.Round(units.FeetToMeters(levelService.GetLevelElevationFeet(created)), 6),
                ["existing"] = false,
            });
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功创建/复用 {results.Count} 个标高",
            new { levels = results, unit = "meters", count = results.Count }));
    }
}

// ---------------------------------------------------------------------------------------------
// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\MoveTool.cs（IModificationService + IElementService）
// 自研对照：RevitAiBatch\ToolDispatcher.cs:1666-1693 MoveElements（本实现补齐 cacheId 与未找到清单）
// ---------------------------------------------------------------------------------------------
internal sealed class MoveElementsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var modificationService = adapter.ModificationService ?? throw new InvalidOperationException("无法获取 ModificationService");
        var elementService = adapter.ElementService ?? throw new InvalidOperationException("无法获取 ElementService");
        var document = ToolContextAccess.Document(context, adapter);
        var units = ToolContextAccess.Units(context);

        var parameters = new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase);
        var ids = ToolArgumentReader.ReadIntegers(parameters, "elementIds", "elementId");

        var cacheId = context.GetParameter<string?>("cacheId", null);
        if (!string.IsNullOrWhiteSpace(cacheId))
        {
            var cachedItems = context.GetCachedData<List<Dictionary<string, object?>>>(cacheId);
            if (cachedItems is not null)
            {
                ids.AddRange(ToolArgumentReader.ExtractIds(cachedItems));
            }
            else
            {
                var cachedIds = context.GetCachedData<List<int>>(cacheId);
                if (cachedIds is not null)
                {
                    ids.AddRange(cachedIds);
                }
                else
                {
                    return Task.FromResult(AIToolResult.Fail($"缓存 ID '{cacheId}' 无效或已过期，请重新查询元素"));
                }
            }
        }

        ids = ids.Distinct().ToList();
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("元素 ID 列表不能为空。请提供 elementId（单个）、elementIds（数组）或 cacheId 参数"));
        }

        var x = context.GetParameter("x", 0.0);
        var y = context.GetParameter("y", 0.0);
        var z = context.GetParameter("z", 0.0);

        var missing = ids.Where(id => !elementService.Exists(document, id)).ToList();
        var movable = ids.Where(id => !missing.Contains(id)).ToList();

        var moved = movable.Count == 0
            ? 0
            : modificationService.MoveElements(document, movable, units.MmToFeet(x), units.MmToFeet(y), units.MmToFeet(z));

        var suffix = missing.Count > 0 ? $"，{missing.Count} 个元素未找到" : string.Empty;
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功移动 {moved} 个元素 ({x:F0}, {y:F0}, {z:F0}) 毫米{suffix}",
            new
            {
                requested = ids.Count,
                moved,
                notFoundCount = missing.Count,
                notFound = missing.Count > 0 ? missing : null,
                delta = new { x, y, z, unit = "millimeters" },
            }));
    }
}

// ---------------------------------------------------------------------------------------------
// 自研扩展（厂商 120 项无同名）：schema 取自 RevitAiBatch\LegacySchemaCatalog.cs 的 LevelSchema
// 自研对照：RevitAiBatch\ToolDispatcher.cs:591-654 UpsertLevel
// ---------------------------------------------------------------------------------------------
internal sealed class UpsertLevelImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var levelService = adapter.LevelService ?? throw new InvalidOperationException("无法获取 LevelService");
        var document = ToolContextAccess.Document(context, adapter);
        var units = ToolContextAccess.Units(context);

        var parameters = new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase);
        var name = context.GetParameter<string?>("name", null);
        if (string.IsNullOrWhiteSpace(name))
        {
            name = ToolArgumentReader.ReadStrings(parameters, "levelName").FirstOrDefault();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 name（标高名称）。"));
        }

        var elevationMeters = ToolArgumentReader.ReadNumbers(parameters, "elevation").FirstOrDefault();
        var elevationFeet = units.MetersToFeet(elevationMeters);
        var toleranceFeet = units.MmToFeet(context.GetParameter("toleranceMm", 5.0));
        var updateExisting = context.GetParameter("updateExisting", true);
        var buildingStory = context.GetParameter("buildingStory", true);
        var dryRun = context.GetParameter("dry_run", false);

        var existing = levelService.FindLevel(document, name!, elevationFeet, toleranceFeet);
        if (existing is null)
        {
            if (dryRun)
            {
                return Task.FromResult(AIToolResult.Ok(
                    $"🧪 预演：将创建标高 {name}（{elevationMeters:0.###} m）",
                    new { action = "create", id = (int?)null, name, elevation = units.Round(elevationMeters, 6), existing = false, elevationUpdated = false, dryRun = true }));
            }

            var created = levelService.CreateLevel(document, name!, elevationFeet, buildingStory);
            return Task.FromResult(AIToolResult.Ok(
                $"✅ 已创建标高 {levelService.GetLevelName(created)}",
                new
                {
                    action = "create",
                    id = (int?)levelService.GetLevelId(created),
                    name = levelService.GetLevelName(created),
                    elevation = units.Round(units.FeetToMeters(levelService.GetLevelElevationFeet(created)), 6),
                    existing = false,
                    elevationUpdated = false,
                    dryRun = false,
                }));
        }

        var oldElevation = units.FeetToMeters(levelService.GetLevelElevationFeet(existing));
        var shouldMove = updateExisting && Math.Abs(levelService.GetLevelElevationFeet(existing) - elevationFeet) > toleranceFeet;

        if (dryRun)
        {
            return Task.FromResult(AIToolResult.Ok(
                $"🧪 预演：命中已有标高 {levelService.GetLevelName(existing)}（{oldElevation:0.###} m）→ {(shouldMove ? "将更新标高" : "无需更新")}",
                new
                {
                    action = "update",
                    id = (int?)levelService.GetLevelId(existing),
                    name = levelService.GetLevelName(existing),
                    oldElevation = units.Round(oldElevation, 6),
                    elevation = units.Round(elevationMeters, 6),
                    existing = true,
                    elevationUpdated = false,
                    dryRun = true,
                }));
        }

        if (shouldMove)
        {
            levelService.SetLevelElevation(existing, elevationFeet);
        }

        if (!string.Equals(levelService.GetLevelName(existing), name, StringComparison.OrdinalIgnoreCase))
        {
            levelService.SetLevelName(existing, name!);
        }

        levelService.SetBuildingStory(existing, buildingStory);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已更新标高 {levelService.GetLevelName(existing)}" + (shouldMove ? "（标高已调整）" : string.Empty),
            new
            {
                action = "update",
                id = (int?)levelService.GetLevelId(existing),
                name = levelService.GetLevelName(existing),
                oldElevation = units.Round(oldElevation, 6),
                elevation = units.Round(units.FeetToMeters(levelService.GetLevelElevationFeet(existing)), 6),
                existing = true,
                elevationUpdated = shouldMove,
                dryRun = false,
            }));
    }
}
