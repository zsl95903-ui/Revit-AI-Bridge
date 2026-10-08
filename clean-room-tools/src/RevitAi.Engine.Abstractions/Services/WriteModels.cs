namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 2（视图与标注类）契约：视图写入、视图/图纸/文字创建、过滤器写入。
// 这批都是写操作：工具声明 RequiresTransaction + RequiresModification，
// 由宿主在 Revit 主线程开事务执行；服务实现内部不再自行开事务。
// 参数名与厂商 schema 对齐（view_id/detail_level/display_style/action/element_ids/
// viewId/duplicateDetail/level_id/view_name/min_point/max_point/title/number/position/
// filterName/visible/color 等），实现里同时接受常见别名。
// ---------------------------------------------------------------------------------------------

public sealed record TemporaryViewControlResult(int ViewId, string Action, int AffectedCount, bool IsInTemporaryMode);

public interface IViewWriteService
{
    ViewSettingsInfo? SetDetailLevel(object document, int? viewId, string detailLevel);

    ViewSettingsInfo? SetDisplayStyle(object document, int? viewId, string displayStyle);

    void SetBackgroundColor(object document, int red, int green, int blue);

    TemporaryViewControlResult ApplyTemporaryViewControl(
        object document,
        int? viewId,
        string action,
        IReadOnlyList<int> elementIds);

    int DuplicateView(object document, int? viewId, string option, string? newName);
}

public interface IViewCreationService
{
    int CreateFloorPlan(object document, string levelName, string? viewName);

    int CreateSection(
        object document,
        string? viewName,
        double minX,
        double minY,
        double maxX,
        double maxY,
        double minZ,
        double maxZ);

    int CreateSheet(object document, string? sheetNumber, string? sheetName);

    int CreateTextNote(object document, string text, double xMm, double yMm, int? viewId);
}

/// <summary>过滤器写入：action = apply / remove / set_visibility / set_color。</summary>
public interface IFilterService
{
    /// <summary>把过滤器应用到某视图并设置颜色/可见性；返回过滤器元素 id。</summary>
    int ApplyFilterToView(
        object document,
        int? viewId,
        string filterName,
        string action,
        int red,
        int green,
        int blue,
        bool visible);

    bool DeleteFilter(object document, string filterName);
}
