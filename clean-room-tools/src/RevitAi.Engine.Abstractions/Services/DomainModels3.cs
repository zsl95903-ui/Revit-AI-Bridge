namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 收尾批：楼梯（含楼梯类型）、屋顶坡度、卫星图导入。
// 说明：这三个领域在不同 Revit 版本里 API 签名差异较大（StairsEditScope 的归属程序集、
// FootPrintRoof 的坡度接口、ImageType/ImageTypeOptions 的构造），因此 Revit 侧采用
// "版本自适应调用"（按名称+参数类型匹配成员，失败时给出可读原因），而不是硬编码签名。
// ---------------------------------------------------------------------------------------------

public sealed record StairResult(int StairId, int RunId, int RunCount, double TotalHeightMm, string ApiPath);

public sealed record StairTypeResult(int TypeId, string TypeName, double? RunThicknessMm, double? LandingThicknessMm, IReadOnlyList<string> Notes);

public sealed record RoofSlopeResult(int RoofId, int ChangedEdges, bool ApplyToAllEdges, double? SlopeAngle, IReadOnlyList<string> Notes);

public sealed record SatelliteImportResult(int? ElementId, string Status, string Message, IReadOnlyList<string> Notes);

public interface IStairService
{
    StairResult CreateStair(
        object document,
        double totalHeightMm,
        int runCount,
        double? treadDepthMm,
        double? runWidthMm,
        double? wellWidthMm,
        double? landingWidthMm,
        double insertPointX,
        double insertPointY);

    StairTypeResult CreateStairType(
        object document,
        string newStairTypeName,
        string? baseStairTypeName,
        double? runThicknessMm,
        double? landingThicknessMm);
}

public interface IRoofEditService
{
    RoofSlopeResult ModifyRoofSlope(
        object document,
        int roofId,
        int? edgeIndex,
        bool definesSlope,
        double? slopeAngleDegrees,
        bool applyToAllEdges);
}

public interface ISatelliteImportService
{
    /// <summary>把本地栅格图（如果提供）作为图像导入到活动视图，并记录地理参考参数。</summary>
    SatelliteImportResult Import(
        object document,
        string? imagePath,
        string? locationName,
        double? latitude,
        double? longitude,
        string? mapSource,
        int? zoomLevel,
        double? radiusKm);
}
