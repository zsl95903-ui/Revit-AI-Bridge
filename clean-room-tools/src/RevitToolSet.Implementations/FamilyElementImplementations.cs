using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 3 第三刀实现：族类型复制/换类型、元素锁定与分组、清理未使用、元素着色。
/// </summary>
public static class FamilyElementImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("duplicate_family_type", new DuplicateFamilyTypeImplementation());
        ToolImplementationRegistry.Register("change_element_types", new ChangeElementTypesImplementation());
        ToolImplementationRegistry.Register("element_lock_manager", new ElementLockManagerImplementation());
        ToolImplementationRegistry.Register("element_group_manager", new ElementGroupManagerImplementation());
        ToolImplementationRegistry.Register("purge_unused", new PurgeUnusedImplementation());
        ToolImplementationRegistry.Register("set_element_color", new SetElementColorImplementation());
    }
}

internal static class FamilyElementArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    public static List<int> ResolveElementIds(AIToolContext context)
    {
        var ids = WriteArgs.Ids(context, "elementIds", "element_ids", "elementId", "element_id");
        if (ids.Count > 0)
        {
            return ids;
        }

        var cacheId = WriteArgs.Text(context, "cacheId", "cache_id");
        if (string.IsNullOrWhiteSpace(cacheId))
        {
            return ids;
        }

        if (context.GetCachedData<List<Dictionary<string, object?>>>(cacheId) is { } cached)
        {
            ids.AddRange(ToolArgumentReader.ExtractIds(cached));
        }
        else if (context.GetCachedData<List<int>>(cacheId) is { } cachedIds)
        {
            ids.AddRange(cachedIds);
        }

        return ids;
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\DuplicateFamilyTypeTool.cs（族管理）
internal sealed class DuplicateFamilyTypeImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.FamilyTypeService ?? throw new InvalidOperationException("无法获取 FamilyTypeService");
        var document = FamilyElementArgs.RequireDocument(adapter, context);

        var sourceTypeId = WriteArgs.OptionalInt(context, "sourceTypeId", "source_type_id", "typeId");
        var newTypeName = WriteArgs.Text(context, "newTypeName", "new_type_name", "name");
        if (sourceTypeId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 sourceTypeId。"));
        }

        if (string.IsNullOrWhiteSpace(newTypeName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 newTypeName。"));
        }

        var newTypeId = service.DuplicateType(document, sourceTypeId.Value, newTypeName!);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已复制族类型 {sourceTypeId} → 新类型 {newTypeId}（{newTypeName}）",
            new { sourceTypeId = sourceTypeId.Value, newTypeId, newTypeName }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ChangeElementTypesTool.cs（元素修改）
internal sealed class ChangeElementTypesImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.FamilyTypeService ?? throw new InvalidOperationException("无法获取 FamilyTypeService");
        var document = FamilyElementArgs.RequireDocument(adapter, context);

        var newTypeId = WriteArgs.OptionalInt(context, "newTypeId", "new_type_id", "typeId");
        if (newTypeId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 newTypeId。"));
        }

        var ids = FamilyElementArgs.ResolveElementIds(context);
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId、elementIds 或 cacheId。"));
        }

        var changed = service.ChangeElementTypes(document, ids, newTypeId.Value);
        var message = changed == ids.Count
            ? $"✅ 已把 {changed} 个元素换成类型 {newTypeId}"
            : $"⚠️ 已把 {changed}/{ids.Count} 个元素换成类型 {newTypeId}";

        return Task.FromResult(changed > 0
            ? AIToolResult.Ok(message, new { requested = ids.Count, changed, newTypeId = newTypeId.Value })
            : AIToolResult.Fail($"{message}（可能类型不兼容或元素不存在）"));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ElementLockManagerTool.cs（元素操作）
internal sealed class ElementLockManagerImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementAdminService ?? throw new InvalidOperationException("无法获取 ElementAdminService");
        var document = FamilyElementArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "get").Trim().ToLowerInvariant();
        var ids = FamilyElementArgs.ResolveElementIds(context);

        switch (operation)
        {
            case "get":
            case "query":
            case "list":
            {
                if (ids.Count == 0)
                {
                    return Task.FromResult(AIToolResult.Fail("get 需要 elementId、elementIds 或 cacheId。"));
                }

                var states = service.GetLockState(document, ids);
                var lockedCount = states.Count(state => state.IsPinned);
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 查询 {states.Count} 个元素，其中已锁定 {lockedCount} 个",
                    new
                    {
                        elements = states.Select(state => new
                        {
                            elementId = state.ElementId,
                            name = state.Name,
                            category = state.CategoryName,
                            isLocked = state.IsPinned,
                        }).ToArray(),
                        lockedCount,
                    }));
            }

            case "lock":
            case "unlock":
            {
                if (ids.Count == 0)
                {
                    return Task.FromResult(AIToolResult.Fail($"{operation} 需要 elementId、elementIds 或 cacheId。"));
                }

                var pinned = operation == "lock";
                var changed = service.SetPinned(document, ids, pinned);
                return Task.FromResult(changed > 0
                    ? AIToolResult.Ok(
                        $"✅ 已{(pinned ? "锁定" : "解锁")} {changed} 个元素（请求 {ids.Count} 个）",
                        new { requested = ids.Count, changed, pinned })
                    : AIToolResult.Fail($"未能{(pinned ? "锁定" : "解锁")}任何元素（元素可能不存在）。"));
            }

            default:
                return Task.FromResult(AIToolResult.Fail("operation 只支持 get / lock / unlock。"));
        }
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ElementGroupManagerTool.cs（元素操作）
internal sealed class ElementGroupManagerImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementAdminService ?? throw new InvalidOperationException("无法获取 ElementAdminService");
        var document = FamilyElementArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "create").Trim().ToLowerInvariant();

        switch (operation)
        {
            case "create":
            case "group":
            {
                var ids = FamilyElementArgs.ResolveElementIds(context);
                if (ids.Count == 0)
                {
                    return Task.FromResult(AIToolResult.Fail("create 需要 elementIds。"));
                }

                var group = service.CreateGroup(document, ids, WriteArgs.Text(context, "groupName", "group_name"));
                return Task.FromResult(group is null
                    ? AIToolResult.Fail("建组失败。")
                    : AIToolResult.Ok(
                        $"✅ 已建组 {group.GroupId}（{group.Name}），包含 {group.MemberCount} 个元素",
                        new { groupId = group.GroupId, name = group.Name, memberCount = group.MemberCount, memberIds = group.MemberIds }));
            }

            case "ungroup":
            case "delete":
            {
                var groupIds = WriteArgs.Ids(context, "groupId", "groupIds", "group_id");
                if (groupIds.Count == 0)
                {
                    return Task.FromResult(AIToolResult.Fail("ungroup 需要 groupId。"));
                }

                var ungrouped = service.Ungroup(document, groupIds);
                return Task.FromResult(ungrouped > 0
                    ? AIToolResult.Ok($"✅ 已解组 {ungrouped} 个组", new { requested = groupIds.Count, ungrouped })
                    : AIToolResult.Fail("未能解组任何组（组不存在或不可解组）。"));
            }

            case "list":
            case "get":
            {
                var groups = service.ListGroups(document, (int)WriteArgs.Number(context, 100, "limit"));
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 共 {groups.Count} 个组",
                    new
                    {
                        groups = groups.Select(group => new
                        {
                            groupId = group.GroupId,
                            name = group.Name,
                            memberCount = group.MemberCount,
                        }).ToArray(),
                        count = groups.Count,
                    }));
            }

            default:
                return Task.FromResult(AIToolResult.Fail("operation 只支持 create / ungroup / list。"));
        }
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\PurgeUnusedTool.cs（文档维护）
internal sealed class PurgeUnusedImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementAdminService ?? throw new InvalidOperationException("无法获取 ElementAdminService");
        var document = FamilyElementArgs.RequireDocument(adapter, context);

        var kinds = ToolArgumentReader.ReadStrings(
            new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase), "types");

        var result = service.PurgeUnused(document, kinds);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已清理 {result.DeletedCount}/{result.ScannedCount} 个未使用元素（类别：{string.Join('/', result.Kinds)}）",
            new
            {
                deletedCount = result.DeletedCount,
                scannedCount = result.ScannedCount,
                kinds = result.Kinds,
                failures = result.Failures,
                note = "Revit 27.3 无 Document.PurgeUnused，本实现按'实例引用关系'判定后逐个删除；被删不掉的项记入 failures。",
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SetElementColorTool.cs（元素修改）
internal sealed class SetElementColorImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementColorService ?? throw new InvalidOperationException("无法获取 ElementColorService");
        var document = FamilyElementArgs.RequireDocument(adapter, context);

        var action = WriteArgs.Text(context, "action") ?? "set";
        var viewId = WriteArgs.OptionalInt(context, "viewId", "view_id");

        // 形式 1：elements: [8001, 8002]
        var ids = WriteArgs.Ids(context, "elements", "elementIds");
        var globalHex = ColorHexFrom(context, "color") ?? WriteArgs.Text(context, "hexColor", "hex_color");
        var globalTransparency = context.Parameters.ContainsKey("transparency")
            ? (int?)WriteArgs.Number(context, 0, "transparency")
            : null;

        // 形式 2：elements: [{ elementId, color:{red,green,blue}, transparency }]
        var perElement = new List<(int ElementId, string? Hex, int? Transparency)>();
        if (context.Parameters.TryGetValue("elements", out var raw) && raw is List<object?> list)
        {
            foreach (var item in list.OfType<Dictionary<string, object?>>())
            {
                var id = ToolArgumentReader.ToDouble(item.TryGetValue("elementId", out var value) ? value : null);
                if (id <= 0)
                {
                    continue;
                }

                var hex = ReadHexFromMap(item, "color", "hexColor");
                var transparency = item.TryGetValue("transparency", out var t) && t is not null
                    ? (int?)ToolArgumentReader.ToDouble(t)
                    : null;
                perElement.Add(((int)id, hex, transparency));
            }
        }

        if (ids.Count == 0 && perElement.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elements（元素 id 数组，或带颜色的对象数组）。"));
        }

        var applied = 0;
        var failures = new List<string>();

        if (perElement.Count > 0)
        {
            foreach (var group in perElement.GroupBy(item => (item.Hex, item.Transparency)))
            {
                var result = service.SetElementColor(
                    document,
                    viewId,
                    group.Select(item => item.ElementId).ToArray(),
                    action,
                    group.Key.Hex ?? globalHex,
                    group.Key.Transparency ?? globalTransparency);
                applied += result.Applied;
                failures.AddRange(result.Failures);
            }
        }
        else
        {
            var result = service.SetElementColor(document, viewId, ids, action, globalHex, globalTransparency);
            applied = result.Applied;
            failures.AddRange(result.Failures);
        }

        var requested = perElement.Count > 0 ? perElement.Count : ids.Count;
        var message = applied == requested
            ? $"✅ 已给 {applied} 个元素设置图形覆盖（{action}）"
            : $"⚠️ 已给 {applied}/{requested} 个元素设置图形覆盖（{action}）";

        return Task.FromResult(applied > 0
            ? AIToolResult.Ok(message, new { requested, applied, action, color = globalHex, transparency = globalTransparency, failures })
            : AIToolResult.Fail($"{message}。失败原因：{string.Join("；", failures)}"));
    }

    private static string? ColorHexFrom(AIToolContext context, string key)
    {
        if (!context.Parameters.TryGetValue(key, out var value) || value is not Dictionary<string, object?> map)
        {
            return value?.ToString();
        }

        return ReadHexFromMap(map, "hexColor", "hex");
    }

    private static string? ReadHexFromMap(Dictionary<string, object?> map, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (map.TryGetValue(key, out var value) && value is not null)
            {
                var text = value.ToString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }
        }

        object? red = null;
        object? green = null;
        object? blue = null;
        map.TryGetValue("red", out red);
        map.TryGetValue("green", out green);
        map.TryGetValue("blue", out blue);

        if (red is not null || green is not null || blue is not null)
        {
            var r = Math.Clamp((int)ToolArgumentReader.ToDouble(red ?? 0), 0, 255);
            var g = Math.Clamp((int)ToolArgumentReader.ToDouble(green ?? 0), 0, 255);
            var b = Math.Clamp((int)ToolArgumentReader.ToDouble(blue ?? 0), 0, 255);
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        return null;
    }
}
