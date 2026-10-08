namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 1 第三批只读 DTO / 服务契约：阶段、工作集归属、链接、屋顶、过滤器可用参数。
// ---------------------------------------------------------------------------------------------

public sealed record ElementPhaseInfo(
    int ElementId,
    int CreatedPhaseId,
    string CreatedPhaseName,
    int DemolishedPhaseId,
    string DemolishedPhaseName);

public sealed record ElementWorksetInfo(int ElementId, int WorksetId, string WorksetName);

public sealed record LinkInfo(
    int LinkInstanceId,
    string LinkName,
    string DocumentPath,
    bool IsLoaded,
    string LinkTypeName);

public sealed record RoofInfo(
    int RoofId,
    string RoofTypeName,
    string LevelName,
    double AreaSquareMeters,
    bool IsFootprintRoof,
    bool IsExtrusionRoof);

public sealed record FilterParameterInfo(int ParameterId, string Name);

/// <summary>元素阶段（创建/拆除阶段）查询。</summary>
public interface IPhaseService
{
    ElementPhaseInfo? GetElementPhase(object document, int elementId);
}

/// <summary>元素所属工作集查询。</summary>
public interface IElementWorksetService
{
    ElementWorksetInfo? GetElementWorkset(object document, int elementId);
}

/// <summary>Revit 链接查询。</summary>
public interface ILinkService
{
    IReadOnlyList<LinkInfo> GetLinks(object document);

    IReadOnlyList<ElementSummaryInfo> GetLinkElements(object document, int linkInstanceId, int limit);
}

/// <summary>屋顶信息查询。</summary>
public interface IRoofService
{
    RoofInfo? GetRoofInfo(object document, int elementId);
}

/// <summary>过滤器可用参数查询。</summary>
public interface IFilterParameterService
{
    IReadOnlyList<FilterParameterInfo> GetAvailableFilterParameters(object document, string? categoryName);
}
