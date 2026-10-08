namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 1 第二批只读 DTO / 服务契约（族、房间、标记、元素几何、项目单位、视图查询、类型列表）。
// 与 ReadModels.cs 保持同一约定：DTO 不依赖 Revit，Revit 侧只做投影。
// ---------------------------------------------------------------------------------------------

public sealed record ElementSummaryInfo(int Id, string Name, string Category, string TypeName);

public sealed record ElementGeometryInfo(
    int ElementId,
    string Category,
    string TypeName,
    string LocationKind,
    double LocationX,
    double LocationY,
    double LocationZ,
    bool HasBoundingBox,
    double MinX,
    double MinY,
    double MinZ,
    double MaxX,
    double MaxY,
    double MaxZ,
    int SolidCount,
    int CurveCount,
    string Unit);

public sealed record ProjectUnitsInfo(
    string LengthUnit,
    string LengthSymbol,
    string AreaUnit,
    string VolumeUnit,
    string AngleUnit,
    string InternalLengthUnit);

public sealed record ViewSettingsInfo(
    int ViewId,
    string Name,
    string ViewType,
    int Scale,
    string DetailLevel,
    string DisplayStyle,
    bool CropActive,
    bool CropBoxVisible);

public sealed record ViewFilterInfo(int FilterId, string Name, bool Enabled, bool Visible);

public sealed record CategoryVisibilityInfo(int ViewId, string Category, bool Visible);

/// <summary>族与族类型查询（批次 1 第二批）。</summary>
public interface IFamilyQueryService
{
    IReadOnlyList<FamilyInfo> GetAllFamilies(object document);

    IReadOnlyList<FamilyTypeInfo> GetFamilyTypes(object document, string? familyName, int limit);
}

// IRoomService / ITagService 已在 ReadModels.cs 中声明，这里不再重复。

/// <summary>元素几何/摘要查询。</summary>
public interface IElementQueryService
{
    IReadOnlyList<ElementSummaryInfo> GetAllElements(object document, string? categoryName, int limit);

    ElementGeometryInfo? GetElementGeometry(object document, int elementId);
}

/// <summary>项目单位查询。</summary>
public interface IProjectUnitService
{
    ProjectUnitsInfo GetProjectUnits(object document);
}

/// <summary>视图只读查询（设置、过滤器、类别可见性）。</summary>
public interface IViewQueryService
{
    ViewSettingsInfo? GetViewSettings(object document, int? viewId);

    IReadOnlyList<ViewFilterInfo> GetViewFilters(object document, int? viewId);

    CategoryVisibilityInfo? GetCategoryVisibility(object document, int? viewId, string categoryName);
}

/// <summary>按类别取类型列表（管/风管/桥架/屋顶等）。</summary>
public interface ITypeService
{
    /// <param name="categoryKey">pipe / duct / cableTray / roof</param>
    IReadOnlyList<FamilyTypeInfo> GetTypes(object document, string categoryKey, int limit);
}
