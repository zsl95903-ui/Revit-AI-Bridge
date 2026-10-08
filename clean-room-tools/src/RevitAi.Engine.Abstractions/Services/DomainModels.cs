namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 5 领域重工程（第一批）：地形、保温、MEP 系统。
// ---------------------------------------------------------------------------------------------

public sealed record Point3(double X, double Y, double Z);

public sealed record TopographyInfo(
    int ElementId,
    int PointCount,
    int AddedPoints,
    double MinElevationMm,
    double MaxElevationMm);

public sealed record InsulationInfo(
    int ElementId,
    string ElementName,
    string TypeName,
    bool HasInsulation,
    double ThicknessMm,
    string MaterialName,
    int LayerIndex);

public sealed record MepSystemInfo(
    int SystemId,
    string Name,
    string SystemTypeName,
    string Kind,
    int MemberCount,
    IReadOnlyList<int> MemberIds,
    int UninsulatedMemberCount,
    IReadOnlyDictionary<string, int> SizeDistribution);

public interface ITopographyService
{
    int CreateTopography(object document, IReadOnlyList<Point3> points);

    TopographyInfo AddPointsFromFloors(object document, int topographyId, IReadOnlyList<int> floorIds);
}

public interface IInsulationService
{
    IReadOnlyList<InsulationInfo> QueryInsulation(
        object document,
        IReadOnlyList<int> elementIds,
        bool onlyUninsulated,
        int limit);

    int ManageInsulation(
        object document,
        string operation,
        IReadOnlyList<int> elementIds,
        double? thicknessMm,
        string? materialName,
        bool overrideExisting);
}

public interface IMepSystemService
{
    IReadOnlyList<MepSystemInfo> QuerySystems(object document, string operation, int? systemTypeId, bool onlyUninsulated, int limit);

    MepSystemInfo? CreateSystem(object document, int systemTypeId, string? newName);

    int RenameSystem(object document, int systemId, string newName);

    bool DeleteSystem(object document, int systemId, bool force);
}
