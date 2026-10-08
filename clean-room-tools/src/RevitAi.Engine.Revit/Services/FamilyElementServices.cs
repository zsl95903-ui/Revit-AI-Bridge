using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 族类型服务：复制类型、批量换类型。
/// API 校正记录（反射核对 27.3）：ElementType.Duplicate(string) 返回 ElementType；
/// Element.ChangeTypeId(ElementId) 返回新的类型 id。
/// </summary>
internal sealed class FamilyTypeService : IFamilyTypeService
{
    public int DuplicateType(object document, int sourceTypeId, string newTypeName)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        if (string.IsNullOrWhiteSpace(newTypeName))
        {
            throw new InvalidOperationException("必须提供 newTypeName。");
        }

        var source = doc.GetElement(new ElementId(sourceTypeId)) as ElementType
                     ?? throw new InvalidOperationException($"元素 {sourceTypeId} 不是可复制的类型（ElementType）。");

        var duplicated = source.Duplicate(newTypeName);
        return (int)duplicated.Id.Value;
    }

    public int ChangeElementTypes(object document, IReadOnlyList<int> elementIds, int newTypeId)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var target = doc.GetElement(new ElementId(newTypeId)) as ElementType
                     ?? throw new InvalidOperationException($"目标类型 {newTypeId} 不存在或不是 ElementType。");

        var changed = 0;
        foreach (var elementId in elementIds)
        {
            var element = doc.GetElement(new ElementId(elementId));
            if (element is null)
            {
                continue;
            }

            try
            {
                element.ChangeTypeId(target.Id);
                changed++;
            }
            catch (Exception)
            {
                // 类型不兼容时跳过该元素。
            }
        }

        return changed;
    }
}

/// <summary>
/// 元素管理服务：锁定（Pinned）/解锁、分组/解组/列组、清理未使用。
/// 说明：Revit API 没有独立的"锁定"概念（UI 的锁定 = 元素 Pinned），因此 lock/unlock 映射到 Pinned。
/// Revit 27.3 没有 Document.PurgeUnused，清理未使用由本服务自行判定后逐个删除。
/// </summary>
internal sealed class ElementAdminService : IElementAdminService
{
    public IReadOnlyList<ElementLockInfo> GetLockState(object document, IReadOnlyList<int> elementIds)
    {
        if (document is not Document doc)
        {
            return Array.Empty<ElementLockInfo>();
        }

        var result = new List<ElementLockInfo>();
        foreach (var elementId in elementIds)
        {
            if (doc.GetElement(new ElementId(elementId)) is not { } element)
            {
                continue;
            }

            result.Add(new ElementLockInfo(
                elementId,
                element.Name ?? string.Empty,
                element.Category?.Name ?? string.Empty,
                SafePinned(element)));
        }

        return result;
    }

    public int SetPinned(object document, IReadOnlyList<int> elementIds, bool pinned)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var changed = 0;
        foreach (var elementId in elementIds)
        {
            if (doc.GetElement(new ElementId(elementId)) is not { } element)
            {
                continue;
            }

            try
            {
                element.Pinned = pinned;
                changed++;
            }
            catch (Exception)
            {
                // 元素不支持 Pinned 时跳过。
            }
        }

        return changed;
    }

    public GroupInfo? CreateGroup(object document, IReadOnlyList<int> elementIds, string? groupName)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        if (elementIds.Count == 0)
        {
            throw new InvalidOperationException("必须提供 elementIds（至少一个元素）。");
        }

        var ids = elementIds.Select(id => new ElementId(id)).ToList();
        var group = doc.Create.NewGroup(ids);

        if (!string.IsNullOrWhiteSpace(groupName))
        {
            try
            {
                group.Name = groupName!;
            }
            catch (Exception)
            {
                // 组名重复时保留默认名。
            }
        }

        return Describe(group);
    }

    public IReadOnlyList<GroupInfo> ListGroups(object document, int limit)
    {
        if (document is not Document doc)
        {
            return Array.Empty<GroupInfo>();
        }

        return new FilteredElementCollector(doc)
            .OfClass(typeof(Group))
            .Cast<Group>()
            .Take(limit <= 0 ? 200 : limit)
            .Select(Describe)
            .ToArray();
    }

    public int Ungroup(object document, IReadOnlyList<int> groupIds)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var ungrouped = 0;
        foreach (var groupId in groupIds)
        {
            if (doc.GetElement(new ElementId(groupId)) is not Group group)
            {
                continue;
            }

            try
            {
                group.UngroupMembers();
                ungrouped++;
            }
            catch (Exception)
            {
                // 组已解组/不可解组时跳过。
            }
        }

        return ungrouped;
    }

    public PurgeResult PurgeUnused(object document, IReadOnlyList<string> kinds)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var normalized = kinds.Count > 0
            ? kinds.Select(kind => kind.Trim().ToLowerInvariant()).Where(kind => kind.Length > 0).Distinct().ToList()
            : new List<string> { "types", "materials" };

        var usedTypeIds = new HashSet<int>();
        var usedMaterialIds = new HashSet<int>();
        var inferredLevelIds = new HashSet<int>();

        foreach (var element in new FilteredElementCollector(doc).WhereElementIsNotElementType())
        {
            var typeId = element.GetTypeId();
            if (typeId is not null && typeId != ElementId.InvalidElementId)
            {
                usedTypeIds.Add((int)typeId.Value);
            }

            if (element.LevelId is { } levelId && levelId != ElementId.InvalidElementId)
            {
                inferredLevelIds.Add((int)levelId.Value);
            }

            foreach (Parameter parameter in element.GetOrderedParameters())
            {
                if (parameter.StorageType != StorageType.ElementId)
                {
                    continue;
                }

                var referencedId = parameter.AsElementId();
                if (referencedId is null || referencedId == ElementId.InvalidElementId)
                {
                    continue;
                }

                if (doc.GetElement(referencedId) is Material)
                {
                    usedMaterialIds.Add((int)referencedId.Value);
                }
            }
        }

        var candidates = new List<ElementId>();

        if (normalized.Any(kind => kind is "types" or "families" or "familytypes" or "symbols"))
        {
            foreach (var type in new FilteredElementCollector(doc).WhereElementIsElementType())
            {
                // 只清理由实例使用关系判定的族类型；跳过系统族样板等特殊类型由 try/catch 兜底。
                if (type is ElementType && !usedTypeIds.Contains((int)type.Id.Value))
                {
                    candidates.Add(type.Id);
                }
            }
        }

        if (normalized.Any(kind => kind is "materials" or "material"))
        {
            candidates.AddRange(new FilteredElementCollector(doc)
                .OfClass(typeof(Material))
                .Where(material => !usedMaterialIds.Contains((int)material.Id.Value))
                .Select(material => material.Id));
        }

        if (normalized.Any(kind => kind is "levels" or "level"))
        {
            candidates.AddRange(new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Where(level => !inferredLevelIds.Contains((int)level.Id.Value))
                .Select(level => level.Id));
        }

        var distinctCandidates = candidates.Distinct().ToList();
        var deleted = 0;
        var failures = new List<string>();

        foreach (var candidate in distinctCandidates)
        {
            try
            {
                doc.Delete(candidate);
                deleted++;
            }
            catch (Exception ex)
            {
                if (failures.Count < 5)
                {
                    failures.Add($"{(int)candidate.Value}: {ex.GetBaseException().Message}");
                }
            }
        }

        return new PurgeResult(deleted, distinctCandidates.Count, normalized, failures);
    }

    private static GroupInfo Describe(Group group)
    {
        IReadOnlyList<int> memberIds;
        try
        {
            memberIds = group.GetMemberIds().Select(id => (int)id.Value).ToArray();
        }
        catch (Exception)
        {
            memberIds = Array.Empty<int>();
        }

        return new GroupInfo(
            (int)group.Id.Value,
            group.Name ?? string.Empty,
            memberIds.Count,
            memberIds);
    }

    private static bool SafePinned(Element element)
    {
        try
        {
            return element.Pinned;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

/// <summary>
/// 元素着色服务：在指定视图上给元素加图形覆盖（projection/cut 线颜色 + 表面透明度），
/// action=reset 时用空覆盖清除。
/// </summary>
internal sealed class ElementColorService : IElementColorService
{
    public ElementColorResult SetElementColor(
        object document,
        int? viewId,
        IReadOnlyList<int> elementIds,
        string action,
        string? colorHex,
        int? transparency)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var (_, view) = ViewWriteService.RequireView(document, viewId);
        var normalized = (action ?? "set").Trim().ToLowerInvariant();

        var overrides = new OverrideGraphicSettings();
        if (normalized is not ("reset" or "clear" or "none"))
        {
            if (!MaterialService.TryParseHex(colorHex, out var color))
            {
                throw new InvalidOperationException("action=set 时必须提供合法颜色（#RRGGBB 或 red/green/blue）。");
            }

            overrides.SetProjectionLineColor(color);
            overrides.SetCutLineColor(color);
            if (transparency is not null)
            {
                overrides.SetSurfaceTransparency(Math.Clamp(transparency.Value, 0, 100));
            }
        }

        var applied = 0;
        var failures = new List<string>();

        foreach (var elementId in elementIds)
        {
            var id = new ElementId(elementId);
            if (doc.GetElement(id) is null)
            {
                failures.Add($"元素 {elementId} 不存在");
                continue;
            }

            try
            {
                view.SetElementOverrides(id, overrides);
                applied++;
            }
            catch (Exception ex)
            {
                failures.Add($"{elementId}: {ex.GetBaseException().Message}");
            }
        }

        return new ElementColorResult(elementIds.Count, applied, failures);
    }
}
