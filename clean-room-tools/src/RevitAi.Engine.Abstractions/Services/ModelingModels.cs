namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 4 第一刀：通用建模原语（轴网 / 房间 / 墙 / 柱 / 梁 / 族实例 / 门窗）。
// 坐标一律对外用毫米，Revit 内部英尺由服务换算。
// ---------------------------------------------------------------------------------------------

public sealed record CreateManyResult(int Requested, int Created, IReadOnlyList<int> CreatedIds, IReadOnlyList<string> Failures);

public sealed record GridSpec(string? Name, double StartX, double StartY, double EndX, double EndY);

public sealed record WallSpec(double StartX, double StartY, double EndX, double EndY, int? LevelId, double HeightMm, int? WallTypeId);

public sealed record ColumnSpec(double X, double Y, double Z, int? LevelId, int? TypeId);

public sealed record BeamSpec(double StartX, double StartY, double EndX, double EndY, int? LevelId, int? TypeId);

public sealed record InstanceSpec(int TypeId, double X, double Y, double Z, int? LevelId);

/// <summary>宿主实例（门/窗）：hostId 是墙，typeId 是族类型。</summary>
public sealed record HostedSpec(int HostId, int TypeId, double X, double Y, double Z, int? LevelId, bool Flip);

public interface IModelingService
{
    CreateManyResult CreateGrids(object document, IReadOnlyList<GridSpec> grids);

    int CreateRoom(object document, int levelId, double xMm, double yMm, string? roomName, string? roomNumber);

    CreateManyResult CreateWalls(object document, IReadOnlyList<WallSpec> walls);

    CreateManyResult CreateColumns(object document, IReadOnlyList<ColumnSpec> columns);

    CreateManyResult CreateBeams(object document, IReadOnlyList<BeamSpec> beams);

    CreateManyResult CreateFamilyInstances(object document, IReadOnlyList<InstanceSpec> instances);

    CreateManyResult CreateHostedInstances(object document, IReadOnlyList<HostedSpec> hosted);
}
