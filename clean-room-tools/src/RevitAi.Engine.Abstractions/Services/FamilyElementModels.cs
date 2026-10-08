namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 3 第三刀：族类型复制/换类型、元素锁定与分组、清理未使用、元素着色。
// ---------------------------------------------------------------------------------------------

public sealed record ElementLockInfo(int ElementId, string Name, string CategoryName, bool IsPinned);

public sealed record GroupInfo(int GroupId, string Name, int MemberCount, IReadOnlyList<int> MemberIds);

public sealed record PurgeResult(int DeletedCount, int ScannedCount, IReadOnlyList<string> Kinds, IReadOnlyList<string> Failures);

public sealed record ElementColorResult(int Requested, int Applied, IReadOnlyList<string> Failures);

public interface IFamilyTypeService
{
    /// <summary>复制族类型（ElementType.Duplicate）并返回新类型 id。</summary>
    int DuplicateType(object document, int sourceTypeId, string newTypeName);

    /// <summary>批量把元素换成新类型（Element.ChangeTypeId），返回成功个数。</summary>
    int ChangeElementTypes(object document, IReadOnlyList<int> elementIds, int newTypeId);
}

public interface IElementAdminService
{
    IReadOnlyList<ElementLockInfo> GetLockState(object document, IReadOnlyList<int> elementIds);

    int SetPinned(object document, IReadOnlyList<int> elementIds, bool pinned);

    GroupInfo? CreateGroup(object document, IReadOnlyList<int> elementIds, string? groupName);

    IReadOnlyList<GroupInfo> ListGroups(object document, int limit);

    int Ungroup(object document, IReadOnlyList<int> groupIds);

    /// <summary>清理未使用的类型/材料/标高（Revit 27.3 没有 Document.PurgeUnused，这里自行判定并删除）。</summary>
    PurgeResult PurgeUnused(object document, IReadOnlyList<string> kinds);
}

public interface IElementColorService
{
    /// <summary>按视图覆盖元素图形（action = set / reset）。</summary>
    ElementColorResult SetElementColor(
        object document,
        int? viewId,
        IReadOnlyList<int> elementIds,
        string action,
        string? colorHex,
        int? transparency);
}
