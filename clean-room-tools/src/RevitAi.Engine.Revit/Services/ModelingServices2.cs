using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// MEP 建模服务：管道 / 风管 / 桥架直段。
/// API 事实（反射核对 27.3）：
///   Pipe.Create(doc, systemTypeId, pipeTypeId, levelId, XYZ start, XYZ end)；
///   Duct.Create(doc, systemTypeId, ductTypeId, levelId, XYZ start, XYZ end)；
///   CableTray.Create(doc, cableTrayTypeId, XYZ start, XYZ end, levelId)。
/// 类型未提供时取项目里第一个可用类型；空样板里没有 MEP 类型时给出可读错误。
/// </summary>
internal sealed class MepModelingService : IMepModelingService
{
    public CreateManyResult CreatePipes(object document, IReadOnlyList<MePSpec> items)
    {
        var doc = ModelingService.RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var item in items)
        {
            try
            {
                var systemTypeId = item.SystemTypeId is not null
                    ? new ElementId(item.SystemTypeId.Value)
                    : FirstTypeId<PipingSystemType>(doc, "管道系统类型");

                var pipeTypeId = item.TypeId is not null
                    ? new ElementId(item.TypeId.Value)
                    : FirstTypeId<PipeType>(doc, "管道类型");

                var levelId = ModelingService.ResolveLevelId(doc, item.LevelId);
                var (start, end) = Segment(item);
                var pipe = Pipe.Create(doc, systemTypeId, pipeTypeId, levelId, start, end);

                if (item.Diameter is > 0)
                {
                    var diameterParameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
                    if (diameterParameter is { IsReadOnly: false })
                    {
                        diameterParameter.Set(ModelingService.Mm(item.Diameter.Value));
                    }
                }

                createdIds.Add((int)pipe.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(items.Count, createdIds.Count, createdIds, failures);
    }

    public CreateManyResult CreateDucts(object document, IReadOnlyList<MePSpec> items)
    {
        var doc = ModelingService.RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var item in items)
        {
            try
            {
                var systemTypeId = item.SystemTypeId is not null
                    ? new ElementId(item.SystemTypeId.Value)
                    : FirstTypeId<MechanicalSystemType>(doc, "风管系统类型");

                var ductTypeId = item.TypeId is not null
                    ? new ElementId(item.TypeId.Value)
                    : FirstTypeId<DuctType>(doc, "风管类型");

                var levelId = ModelingService.ResolveLevelId(doc, item.LevelId);
                var (start, end) = Segment(item);
                var duct = Duct.Create(doc, systemTypeId, ductTypeId, levelId, start, end);

                if (item.Width is > 0)
                {
                    var widthParameter = duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM);
                    if (widthParameter is { IsReadOnly: false })
                    {
                        widthParameter.Set(ModelingService.Mm(item.Width.Value));
                    }
                }

                if (item.Height is > 0)
                {
                    var heightParameter = duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM);
                    if (heightParameter is { IsReadOnly: false })
                    {
                        heightParameter.Set(ModelingService.Mm(item.Height.Value));
                    }
                }

                createdIds.Add((int)duct.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(items.Count, createdIds.Count, createdIds, failures);
    }

    public CreateManyResult CreateCableTrays(object document, IReadOnlyList<MePSpec> items)
    {
        var doc = ModelingService.RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var item in items)
        {
            try
            {
                var trayTypeId = item.TypeId is not null
                    ? new ElementId(item.TypeId.Value)
                    : FirstTypeId<CableTrayType>(doc, "桥架类型");

                var levelId = ModelingService.ResolveLevelId(doc, item.LevelId);
                var (start, end) = Segment(item);
                var tray = CableTray.Create(doc, trayTypeId, start, end, levelId);

                if (item.Width is > 0)
                {
                    var widthParameter = tray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);
                    if (widthParameter is { IsReadOnly: false })
                    {
                        widthParameter.Set(ModelingService.Mm(item.Width.Value));
                    }
                }

                createdIds.Add((int)tray.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(items.Count, createdIds.Count, createdIds, failures);
    }

    private static (XYZ Start, XYZ End) Segment(MePSpec item)
    {
        var start = new XYZ(ModelingService.Mm(item.StartX), ModelingService.Mm(item.StartY), ModelingService.Mm(item.StartZ));
        var end = new XYZ(ModelingService.Mm(item.EndX), ModelingService.Mm(item.EndY), ModelingService.Mm(item.EndZ));
        if (start.DistanceTo(end) < 1e-6)
        {
            throw new InvalidOperationException("起点与终点重合。");
        }

        return (start, end);
    }

    internal static ElementId FirstTypeId<T>(Document document, string what)
        where T : ElementType
    {
        var type = new FilteredElementCollector(document)
            .OfClass(typeof(T))
            .Cast<T>()
            .FirstOrDefault();

        return type?.Id ?? throw new InvalidOperationException($"项目里没有可用的{what}，请先加载/创建或显式提供类型 id。");
    }
}

/// <summary>
/// 形体建模服务：楼板（闭合轮廓）、足迹屋顶、拉伸屋顶、按元素标注尺寸、3D 视图。
/// API 事实：Floor.Create(doc, IList&lt;CurveLoop&gt;, floorTypeId, levelId)；
/// doc.Create.NewFootPrintRoof(CurveArray, Level, RoofType, out ModelCurveArray)；
/// doc.Create.NewExtrusionRoof(CurveArray, ReferencePlane, Level, RoofType, start, end)；
/// doc.Create.NewDimension(View, Line, ReferenceArray)；View3D.CreateIsometric(doc, viewFamilyTypeId)。
/// </summary>
internal sealed class ShapeModelingService : IShapeModelingService
{
    public CreateManyResult CreateFloors(object document, IReadOnlyList<ProfileSpec> floors)
    {
        var doc = ModelingService.RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var floor in floors)
        {
            try
            {
                var levelId = ModelingService.ResolveLevelId(doc, floor.LevelId);
                var floorTypeId = floor.TypeId is not null
                    ? new ElementId(floor.TypeId.Value)
                    : ModelingService.DefaultTypeId(doc, BuiltInCategory.OST_Floors, "楼板");

                var loop = BuildLoop(floor.Points);
                var created = Floor.Create(doc, new List<CurveLoop> { loop }, floorTypeId, levelId);
                createdIds.Add((int)created.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(floors.Count, createdIds.Count, createdIds, failures);
    }

    public CreateManyResult CreateFootprintRoofs(object document, IReadOnlyList<ProfileSpec> roofs)
    {
        var doc = ModelingService.RequireDocument(document);
        var createdIds = new List<int>();
        var failures = new List<string>();

        foreach (var roof in roofs)
        {
            try
            {
                var level = ModelingService.ResolveLevel(doc, roof.LevelId);
                var roofTypeId = roof.TypeId is not null
                    ? new ElementId(roof.TypeId.Value)
                    : ModelingService.DefaultTypeId(doc, BuiltInCategory.OST_Roofs, "屋顶");

                var roofType = doc.GetElement(roofTypeId) as RoofType
                               ?? throw new InvalidOperationException($"{roofTypeId.Value} 不是屋顶类型。");

                var curveArray = new CurveArray();
                foreach (var curve in BuildLoop(roof.Points))
                {
                    curveArray.Append(curve);
                }

                var created = doc.Create.NewFootPrintRoof(curveArray, level, roofType, out _);
                createdIds.Add((int)created.Id.Value);
            }
            catch (Exception ex)
            {
                failures.Add(ex.GetBaseException().Message);
            }
        }

        return new CreateManyResult(roofs.Count, createdIds.Count, createdIds, failures);
    }

    public int CreateExtrusionRoof(
        object document,
        IReadOnlyList<Point2> profile,
        int levelId,
        int? roofTypeId,
        double extrusionStartMm,
        double extrusionEndMm)
    {
        var doc = ModelingService.RequireDocument(document);
        var level = ModelingService.ResolveLevel(doc, levelId);
        var typeId = roofTypeId is not null
            ? new ElementId(roofTypeId.Value)
            : ModelingService.DefaultTypeId(doc, BuiltInCategory.OST_Roofs, "屋顶");

        var roofType = doc.GetElement(typeId) as RoofType
                       ?? throw new InvalidOperationException($"{typeId.Value} 不是屋顶类型。");

        if (profile.Count < 3)
        {
            throw new InvalidOperationException("拉伸屋顶的轮廓至少需要 3 个点。");
        }

        // 轮廓画在 XZ 立面内（X=水平、Z=高度），沿 Y 方向拉伸。
        // 说明：厂商 schema 只给二维点，这里固定按"XZ 立面 + Y 向拉伸"解释。
        var curveArray = new CurveArray();
        var bottom = level.Elevation;
        for (var i = 0; i < profile.Count; i++)
        {
            var current = profile[i];
            var next = profile[(i + 1) % profile.Count];
            var start = new XYZ(ModelingService.Mm(current.X), 0, bottom + ModelingService.Mm(current.Y));
            var end = new XYZ(ModelingService.Mm(next.X), 0, bottom + ModelingService.Mm(next.Y));
            if (start.DistanceTo(end) > 1e-6)
            {
                curveArray.Append(Line.CreateBound(start, end));
            }
        }

        var plane = doc.Create.NewReferencePlane(
            new XYZ(0, extrusionStartMm == 0 ? 0 : 0, bottom),
            XYZ.BasisX,
            XYZ.BasisZ,
            doc.ActiveView);

        var roof = doc.Create.NewExtrusionRoof(
            curveArray,
            plane,
            level,
            roofType,
            ModelingService.Mm(extrusionStartMm),
            ModelingService.Mm(extrusionEndMm));

        return (int)roof.Id.Value;
    }

    public int CreateDimensionByElements(object document, int viewId, IReadOnlyList<int> elementIds, double offsetXMm, double offsetYMm)
    {
        var doc = ModelingService.RequireDocument(document);
        var view = doc.GetElement(new ElementId(viewId)) as View
                   ?? throw new InvalidOperationException($"未找到视图 {viewId}。");

        if (elementIds.Count < 2)
        {
            throw new InvalidOperationException("按元素标注尺寸至少需要 2 个元素（才能确定标注跨度）。");
        }

        var references = new ReferenceArray();
        var points = new List<XYZ>();

        foreach (var elementId in elementIds)
        {
            var element = doc.GetElement(new ElementId(elementId))
                          ?? throw new InvalidOperationException($"未找到元素 {elementId}。");

            references.Append(ResolveReference(element));
            if (element.get_BoundingBox(view) is { } box)
            {
                points.Add(box.Min);
                points.Add(box.Max);
            }
        }

        if (points.Count == 0)
        {
            throw new InvalidOperationException("元素在当前视图里没有包围盒，无法放置尺寸标注。");
        }

        var minX = points.Min(point => point.X);
        var maxX = points.Max(point => point.X);
        var minY = points.Min(point => point.Y);
        var line = Line.CreateBound(
            new XYZ(minX + ModelingService.Mm(offsetXMm), minY + ModelingService.Mm(offsetYMm), 0),
            new XYZ(maxX + ModelingService.Mm(offsetXMm), minY + ModelingService.Mm(offsetYMm), 0));

        var dimension = doc.Create.NewDimension(view, line, references);
        return (int)dimension.Id.Value;
    }

    public int Create3DView(object document, string? viewName, IReadOnlyList<int> elementIds)
    {
        var doc = ModelingService.RequireDocument(document);

        var viewFamilyTypeId = new FilteredElementCollector(doc)
            .OfClass(typeof(ViewFamilyType))
            .Cast<ViewFamilyType>()
            .FirstOrDefault(type => type.ViewFamily == ViewFamily.ThreeDimensional)?.Id
            ?? throw new InvalidOperationException("项目里没有可用的三维视图类型。");

        var view = View3D.CreateIsometric(doc, viewFamilyTypeId);

        if (!string.IsNullOrWhiteSpace(viewName))
        {
            try
            {
                view.Name = viewName!;
            }
            catch (Exception)
            {
                // 名称冲突时保留默认名。
            }
        }

        if (elementIds.Count > 0)
        {
            try
            {
                var boxes = elementIds
                    .Select(id => doc.GetElement(new ElementId(id))?.get_BoundingBox(null))
                    .Where(box => box is not null)
                    .Select(box => box!)
                    .ToList();

                if (boxes.Count > 0)
                {
                    var min = new XYZ(boxes.Min(box => box.Min.X), boxes.Min(box => box.Min.Y), boxes.Min(box => box.Min.Z));
                    var max = new XYZ(boxes.Max(box => box.Max.X), boxes.Max(box => box.Max.Y), boxes.Max(box => box.Max.Z));
                    var sectionBox = new BoundingBoxXYZ { Min = min, Max = max };
                    view.SetSectionBox(sectionBox);
                }
            }
            catch (Exception)
            {
                // 剖面框设置失败不影响视图创建。
            }
        }

        return (int)view.Id.Value;
    }

    private static CurveLoop BuildLoop(IReadOnlyList<Point2> points)
    {
        if (points.Count < 3)
        {
            throw new InvalidOperationException("闭合轮廓至少需要 3 个点。");
        }

        var curves = new List<Curve>();
        for (var i = 0; i < points.Count; i++)
        {
            var current = points[i];
            var next = points[(i + 1) % points.Count];
            var start = new XYZ(ModelingService.Mm(current.X), ModelingService.Mm(current.Y), 0);
            var end = new XYZ(ModelingService.Mm(next.X), ModelingService.Mm(next.Y), 0);
            if (start.DistanceTo(end) < 1e-6)
            {
                continue;
            }

            curves.Add(Line.CreateBound(start, end));
        }

        if (curves.Count < 3)
        {
            throw new InvalidOperationException("闭合轮廓至少需要 3 条有效边（有重合点被跳过）。");
        }

        return CurveLoop.Create(curves);
    }

    private static Reference ResolveReference(Element element)
    {
        try
        {
            var options = new Options { ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine };
            if (element.get_Geometry(options) is { } geometry)
            {
                foreach (var item in geometry)
                {
                    switch (item)
                    {
                        case Solid solid when solid.Faces.Size > 0:
                            return solid.Faces.get_Item(0).Reference;
                        case GeometryInstance instance:
                            foreach (var nested in instance.GetInstanceGeometry())
                            {
                                if (nested is Solid nestedSolid && nestedSolid.Faces.Size > 0)
                                {
                                    return nestedSolid.Faces.get_Item(0).Reference;
                                }
                            }

                            break;
                    }
                }
            }
        }
        catch (Exception)
        {
            // 退化到元素参照。
        }

        return new Reference(element);
    }
}
