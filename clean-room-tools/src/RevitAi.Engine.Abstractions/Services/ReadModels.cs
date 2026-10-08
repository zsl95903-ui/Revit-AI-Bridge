namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 只读查询用的轻量 DTO。放在 Abstractions（不依赖 Revit）里，好处：
//   * 工具实现可以在无 Revit 的工程里编写并被冒烟测试覆盖；
//   * Revit 侧只负责把 API 对象投影成 DTO，边界清晰。
// ---------------------------------------------------------------------------------------------

public sealed record GridInfo(int Id, string Name);

public sealed record ViewInfo(int Id, string Name, string ViewType, bool IsTemplate);

public sealed record CategoryInfo(string Name, int Id, string CategoryType);

public sealed record WorksetInfo(int Id, string Name, bool IsOpen, bool IsEditable, string Kind);

public sealed record FamilyInfo(int FamilyId, string FamilyName, string Category, int TypeCount);

public sealed record FamilyTypeInfo(int TypeId, string FamilyName, string TypeName, string Category);

public sealed record RoomInfo(int RoomId, string Number, string Name, double AreaSquareMeters, int LevelId, string LevelName);

public sealed record TagInfo(int TagId, string Text, int? TaggedElementId, string Category);

public sealed record ElementLocationInfo(int ElementId, double X, double Y, double Z, string Unit, string LocationKind);

/// <summary>轴网（datum）查询。</summary>
public interface IGridService
{
    IReadOnlyList<GridInfo> GetAllGrids(object document);
}

/// <summary>视图查询。</summary>
public interface IViewService
{
    IReadOnlyList<ViewInfo> GetAllViews(object document);

    ViewInfo? GetActiveView(object document);
}

/// <summary>类别查询。</summary>
public interface ICategoryService
{
    IReadOnlyList<CategoryInfo> GetAllCategories(object document);
}

/// <summary>工作集查询。</summary>
public interface IWorksetService
{
    IReadOnlyList<WorksetInfo> GetAllWorksets(object document);
}

/// <summary>族与族类型查询。</summary>
public interface IFamilyService
{
    IReadOnlyList<FamilyInfo> GetAllFamilies(object document);

    IReadOnlyList<FamilyTypeInfo> GetFamilyTypes(object document, string? familyName, string? categoryName, int limit);
}

/// <summary>房间查询。</summary>
public interface IRoomService
{
    IReadOnlyList<RoomInfo> GetAllRooms(object document);
}

/// <summary>标注（标记）查询。</summary>
public interface ITagService
{
    IReadOnlyList<TagInfo> GetAllTags(object document);
}

/// <summary>元素定位（位置点 / 包围盒中心）。</summary>
public interface IElementLocationService
{
    ElementLocationInfo? GetElementLocation(object document, int elementId);
}
