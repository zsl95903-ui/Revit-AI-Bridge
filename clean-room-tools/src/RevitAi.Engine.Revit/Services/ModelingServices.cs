using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 建模原语服务：轴网 / 房间 / 墙 / 柱 / 梁 / 族实例 / 门窗。
/// 约定：外部坐标一律毫米；类型未提供时按类别取项目里的第一个可用族类型，
/// 取不到时抛出可读错误（空样板常见：项目里没有结构柱/门族）。
/// API 事实（反射核对 27.3）：
///   Grid.Create(doc, Line)；Wall.Create(doc, Curve, wallTypeId, levelId, height, offset, flip, structural)；
///   doc.Create.NewRoom(Level, UV)；doc.Create.NewFamilyInstance(Curve|XYZ, FamilySymbol, Level, StructuralType)。
/// </summary>
internal sealed class ModelingService : IModelingService
{
    private const double MillimetersPerFoot = 304.8;

    public CreateManyResult CreateGrids(object document, IReadOnlyList<GridSpec> grids)
    {
        var doc = RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var grid in grids)
        {
            try
            {
                var start = new XYZ(Mm(grid.StartX), Mm(grid.StartY), 0);
                var end = new XYZ(Mm(grid.EndX), Mm(grid.EndY), 0);
                if (start.DistanceTo(end) < 1e-6)
                {
                    failures.Add("轴网起点与终点重合");
                    continue;
                }

                var line = Line.CreateBound(start, end);
                var element = Grid.Create(doc, line);

                if (!string.IsNullOrWhiteSpace(grid.Name))
                {
                    try
                    {
                        element.Name = grid.Name!;
                    }
                    catch (Exception)
                    {
                        // 名称冲突时保留默认名。
                    }
                }

                createdIds.Add((int)element.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(grids.Count, createdIds.Count, createdIds, failures);
    }

    public int CreateRoom(object document, int levelId, double xMm, double yMm, string? roomName, string? roomNumber)
    {
        var doc = RequireDocument(document);
        var level = doc.GetElement(new ElementId(levelId)) as Level
                    ?? throw new InvalidOperationException($"未找到标高 {levelId}。");

        var room = doc.Create.NewRoom(level, new UV(Mm(xMm), Mm(yMm)));

        if (!string.IsNullOrWhiteSpace(roomName))
        {
            try
            {
                room.Name = roomName!;
            }
            catch (Exception)
            {
                // 名称非法/重复时保留默认名。
            }
        }

        if (!string.IsNullOrWhiteSpace(roomNumber))
        {
            var numberParameter = room.get_Parameter(BuiltInParameter.ROOM_NUMBER);
            if (numberParameter is { IsReadOnly: false })
            {
                numberParameter.Set(roomNumber!);
            }
        }

        return (int)room.Id.Value;
    }

    public CreateManyResult CreateWalls(object document, IReadOnlyList<WallSpec> walls)
    {
        var doc = RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var wall in walls)
        {
            try
            {
                var levelId = ResolveLevelId(doc, wall.LevelId);
                var wallTypeId = wall.WallTypeId is not null
                    ? new ElementId(wall.WallTypeId.Value)
                    : DefaultTypeId(doc, BuiltInCategory.OST_Walls, "墙");

                var start = new XYZ(Mm(wall.StartX), Mm(wall.StartY), 0);
                var end = new XYZ(Mm(wall.EndX), Mm(wall.EndY), 0);
                if (start.DistanceTo(end) < 1e-6)
                {
                    failures.Add("墙起点与终点重合");
                    continue;
                }

                var height = wall.HeightMm > 0 ? Mm(wall.HeightMm) : Mm(3000);
                var element = Wall.Create(doc, Line.CreateBound(start, end), wallTypeId, levelId, height, 0, false, false);
                createdIds.Add((int)element.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(walls.Count, createdIds.Count, createdIds, failures);
    }

    public CreateManyResult CreateColumns(object document, IReadOnlyList<ColumnSpec> columns)
    {
        var doc = RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var column in columns)
        {
            try
            {
                var level = ResolveLevel(doc, column.LevelId);
                var typeId = column.TypeId is not null
                    ? new ElementId(column.TypeId.Value)
                    : DefaultTypeId(doc, BuiltInCategory.OST_StructuralColumns, "结构柱");

                var symbol = RequireSymbol(doc, typeId, "结构柱");
                var point = new XYZ(Mm(column.X), Mm(column.Y), column.Z != 0 ? Mm(column.Z) : 0);
                var instance = doc.Create.NewFamilyInstance(point, symbol, level, StructuralType.Column);
                createdIds.Add((int)instance.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(columns.Count, createdIds.Count, createdIds, failures);
    }

    public CreateManyResult CreateBeams(object document, IReadOnlyList<BeamSpec> beams)
    {
        var doc = RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var beam in beams)
        {
            try
            {
                var level = ResolveLevel(doc, beam.LevelId);
                var typeId = beam.TypeId is not null
                    ? new ElementId(beam.TypeId.Value)
                    : DefaultTypeId(doc, BuiltInCategory.OST_StructuralFraming, "结构框架");

                var symbol = RequireSymbol(doc, typeId, "梁/结构框架");
                var start = new XYZ(Mm(beam.StartX), Mm(beam.StartY), 0);
                var end = new XYZ(Mm(beam.EndX), Mm(beam.EndY), 0);
                if (start.DistanceTo(end) < 1e-6)
                {
                    failures.Add("梁起点与终点重合");
                    continue;
                }

                var instance = doc.Create.NewFamilyInstance(Line.CreateBound(start, end), symbol, level, StructuralType.Beam);
                createdIds.Add((int)instance.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(beams.Count, createdIds.Count, createdIds, failures);
    }

    public CreateManyResult CreateFamilyInstances(object document, IReadOnlyList<InstanceSpec> instances)
    {
        var doc = RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var instance in instances)
        {
            try
            {
                var symbol = RequireSymbol(doc, new ElementId(instance.TypeId), "族类型");
                var point = new XYZ(Mm(instance.X), Mm(instance.Y), Mm(instance.Z));
                var level = instance.LevelId is not null ? ResolveLevel(doc, instance.LevelId) : null;

                var created = level is not null
                    ? doc.Create.NewFamilyInstance(point, symbol, level, StructuralType.NonStructural)
                    : doc.Create.NewFamilyInstance(point, symbol, doc.ActiveView);

                createdIds.Add((int)created.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(instances.Count, createdIds.Count, createdIds, failures);
    }

    public CreateManyResult CreateHostedInstances(object document, IReadOnlyList<HostedSpec> hosted)
    {
        var doc = RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var item in hosted)
        {
            try
            {
                var host = doc.GetElement(new ElementId(item.HostId))
                           ?? throw new InvalidOperationException($"宿主元素 {item.HostId} 不存在。");

                var symbol = RequireSymbol(doc, new ElementId(item.TypeId), "门窗族类型");
                var level = ResolveLevel(doc, item.LevelId);
                var point = new XYZ(Mm(item.X), Mm(item.Y), item.Z != 0 ? Mm(item.Z) : Mm(1000));

                var instance = doc.Create.NewFamilyInstance(point, symbol, host, level, StructuralType.NonStructural);
                if (item.Flip)
                {
                    try
                    {
                        instance.flipFacing();
                    }
                    catch (Exception)
                    {
                        // 该族不支持翻转。
                    }
                }

                createdIds.Add((int)instance.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(hosted.Count, createdIds.Count, createdIds, failures);
    }

    internal static Document RequireDocument(object document)
        => document as Document ?? throw new InvalidOperationException("文档对象无效。");

    internal static double Mm(double millimeters) => millimeters / MillimetersPerFoot;

    internal static ElementId ResolveLevelId(Document document, int? levelId)
    {
        if (levelId is not null)
        {
            return new ElementId(levelId.Value);
        }

        var level = new FilteredElementCollector(document)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .OrderBy(item => item.Elevation)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("项目中没有任何标高，无法定位构件所在层。");

        return level.Id;
    }

    internal static Level ResolveLevel(Document document, int? levelId)
    {
        var id = ResolveLevelId(document, levelId);
        return document.GetElement(id) as Level
               ?? throw new InvalidOperationException($"标高 {id.Value} 无效。");
    }

    internal static FamilySymbol RequireSymbol(Document document, ElementId typeId, string what)
    {
        if (document.GetElement(typeId) is not FamilySymbol symbol)
        {
            throw new InvalidOperationException($"{(int)typeId.Value} 不是可用的{what}族类型。");
        }

        if (!symbol.IsActive)
        {
            symbol.Activate();
            document.Regenerate();
        }

        return symbol;
    }

    internal static ElementId DefaultTypeId(Document document, BuiltInCategory category, string what)
    {
        var symbol = new FilteredElementCollector(document)
            .OfCategory(category)
            .WhereElementIsElementType()
            .OfType<FamilySymbol>()
            .FirstOrDefault();

        return symbol?.Id
               ?? throw new InvalidOperationException($"项目里没有可用的{what}族类型，请先加载{what}族或显式提供类型 id。");
    }
}
