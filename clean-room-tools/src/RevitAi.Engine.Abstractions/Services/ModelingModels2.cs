namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 4 第二刀：MEP 管线/风管/桥架 + 楼板轮廓/屋顶/尺寸/3D 视图。
// ---------------------------------------------------------------------------------------------

/// <summary>二维点（毫米）。</summary>
public sealed record Point2(double X, double Y);

/// <summary>MEP 直段规格（管线/风管/桥架共用）。</summary>
public sealed record MePSpec(
    double StartX,
    double StartY,
    double StartZ,
    double EndX,
    double EndY,
    double EndZ,
    int? LevelId,
    int? TypeId,
    int? SystemTypeId,
    double? Diameter,
    double? Width,
    double? Height);

/// <summary>按闭合轮廓创建的构件（楼板 / 足迹屋顶）。</summary>
public sealed record ProfileSpec(IReadOnlyList<Point2> Points, int? LevelId, int? TypeId);

public interface IMepModelingService
{
    CreateManyResult CreatePipes(object document, IReadOnlyList<MePSpec> items);

    CreateManyResult CreateDucts(object document, IReadOnlyList<MePSpec> items);

    CreateManyResult CreateCableTrays(object document, IReadOnlyList<MePSpec> items);
}

public interface IShapeModelingService
{
    CreateManyResult CreateFloors(object document, IReadOnlyList<ProfileSpec> floors);

    CreateManyResult CreateFootprintRoofs(object document, IReadOnlyList<ProfileSpec> roofs);

    int CreateExtrusionRoof(
        object document,
        IReadOnlyList<Point2> profile,
        int levelId,
        int? roofTypeId,
        double extrusionStartMm,
        double extrusionEndMm);

    int CreateDimensionByElements(
        object document,
        int viewId,
        IReadOnlyList<int> elementIds,
        double offsetXMm,
        double offsetYMm);

    int Create3DView(object document, string? viewName, IReadOnlyList<int> elementIds);
}
