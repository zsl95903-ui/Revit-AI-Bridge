namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 收尾批：道路（墙→路面板）、CAD 图层管理、剪切顺序修正、UI 选择/缩放/激活视图。
// ---------------------------------------------------------------------------------------------

public sealed record RoadResult(
    int FloorId,
    int WallCount,
    double WidthMm,
    bool ClosedLoop,
    IReadOnlyList<Point2> Outline,
    IReadOnlyList<string> Notes);

public sealed record CadLayerInfo(string Name, int CategoryId, bool Visible);

public sealed record CutOrderResult(int Checked, int Fixed, bool DryRun, IReadOnlyList<string> Details);

public sealed record SelectionResult(int Selected, int Cleared, bool Zoomed, string Message);

public interface IDomainExtraService
{
    RoadResult CreateRoadFromWalls(
        object document,
        IReadOnlyList<int> wallIds,
        double? laneWidthMeters,
        double? sidewalkWidthMeters,
        double? defaultRoadWidthMeters);

    IReadOnlyList<CadLayerInfo> ManageCadLayers(object document, int viewId, int linkId, string operation, string? layerName);

    CutOrderResult FixCuttingOrder(
        object document,
        IReadOnlyList<int> cuttingIds,
        IReadOnlyList<int> toCutIds,
        bool dryRun);
}

public interface IViewInteractionService
{
    /// <summary>uiDocument 为 UIDocument；服务内部负责类型校验。</summary>
    SelectionResult SelectElements(object uiDocument, IReadOnlyList<int> elementIds, bool append, bool zoomToFit);

    SelectionResult ClearSelection(object uiDocument);

    SelectionResult ZoomToElements(object uiDocument, IReadOnlyList<int> elementIds);

    SelectionResult ActivateView(object uiDocument, int viewId);
}
