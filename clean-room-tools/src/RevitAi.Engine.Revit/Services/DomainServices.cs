using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 地形服务：按点建地形、把楼板轮廓点并入已有地形。
/// API 事实：TopographySurface.Create(doc, IList&lt;XYZ&gt;)；TopographySurface.AddPoints(IList&lt;XYZ&gt;)。
/// </summary>
internal sealed class TopographyService : ITopographyService
{
    public int CreateTopography(object document, IReadOnlyList<Point3> points)
    {
        var doc = ModelingService.RequireDocument(document);
        if (points.Count < 3)
        {
            throw new InvalidOperationException("创建地形至少需要 3 个点。");
        }

        var xyz = points
            .Select(point => new XYZ(ModelingService.Mm(point.X), ModelingService.Mm(point.Y), ModelingService.Mm(point.Z)))
            .ToList();

        if (xyz.Distinct().Count() < 3)
        {
            throw new InvalidOperationException("地形点必须互不重合（至少 3 个不同点）。");
        }

        var surface = TopographySurface.Create(doc, xyz);
        return (int)surface.Id.Value;
    }

    public TopographyInfo AddPointsFromFloors(object document, int topographyId, IReadOnlyList<int> floorIds)
    {
        var doc = ModelingService.RequireDocument(document);
        var surface = doc.GetElement(new ElementId(topographyId)) as TopographySurface
                      ?? throw new InvalidOperationException($"元素 {topographyId} 不是地形表面。");

        if (floorIds.Count == 0)
        {
            throw new InvalidOperationException("必须提供 floor_element_ids/floor_element_id。");
        }

        var points = new List<XYZ>();
        foreach (var floorId in floorIds)
        {
            if (doc.GetElement(new ElementId(floorId)) is not Floor floor)
            {
                continue;
            }

            points.AddRange(FloorOutlinePoints(doc, floor));
        }

        if (points.Count == 0)
        {
            throw new InvalidOperationException("没有从楼板取到任何轮廓点。");
        }

        var added = 0;
        foreach (var point in points)
        {
            try
            {
                surface.AddPoints(new List<XYZ> { point });
                added++;
            }
            catch (Exception)
            {
                // 落在既有边界外/重复的点会被拒绝，跳过。
            }
        }

        var all = surface.GetPoints();
        var min = all.Count > 0 ? all.Min(item => item.Z) : 0;
        var max = all.Count > 0 ? all.Max(item => item.Z) : 0;

        return new TopographyInfo(
            (int)surface.Id.Value,
            all.Count,
            added,
            Math.Round(min * 304.8, 2),
            Math.Round(max * 304.8, 2));
    }

    private static List<XYZ> FloorOutlinePoints(Document document, Floor floor)
    {
        // 优先用楼板的几何边界（顶面轮廓），取不到时退化到包围盒四角。
        // 注意：C# 不允许在 try 块里 yield，所以先收集成列表再返回。
        var points = new List<XYZ>();
        try
        {
            var options = new Options { ComputeReferences = false, DetailLevel = ViewDetailLevel.Medium };
            if (floor.get_Geometry(options) is { } geometry)
            {
                foreach (var item in geometry)
                {
                    if (item is not Solid solid)
                    {
                        continue;
                    }

                    foreach (Face face in solid.Faces)
                    {
                        if (face is not PlanarFace planar)
                        {
                            continue;
                        }

                        var loop = planar.GetEdgesAsCurveLoops().FirstOrDefault();
                        if (loop is null)
                        {
                            continue;
                        }

                        foreach (var curve in loop)
                        {
                            points.AddRange(curve.Tessellate());
                        }

                        return points;
                    }
                }
            }
        }
        catch (Exception)
        {
            // 退化路径。
        }

        if (points.Count > 0)
        {
            return points;
        }

        if (floor.get_BoundingBox(null) is { } box)
        {
            points.Add(box.Min);
            points.Add(new XYZ(box.Max.X, box.Min.Y, box.Min.Z));
            points.Add(box.Max);
            points.Add(new XYZ(box.Min.X, box.Max.Y, box.Min.Z));
        }

        return points;
    }
}

/// <summary>
/// 保温服务：在墙/楼板/屋顶等类型的复合层里增删"保温层"（MaterialFunctionAssignment.Insulation）。
/// API 事实：CompoundStructure.GetLayers/SetLayers；HostObjAttributes.SetCompoundStructure。
/// </summary>
internal sealed class InsulationService : IInsulationService
{
    private const double MillimetersPerFoot = 304.8;

    public IReadOnlyList<InsulationInfo> QueryInsulation(
        object document,
        IReadOnlyList<int> elementIds,
        bool onlyUninsulated,
        int limit)
    {
        var doc = ModelingService.RequireDocument(document);
        var safeLimit = limit <= 0 ? 200 : limit;
        var result = new List<InsulationInfo>();

        var hosts = elementIds.Count > 0
            ? elementIds
                .Select(id => doc.GetElement(new ElementId(id)))
                .Where(element => element is not null)
                .Select(element =>
                {
                    var typeId = element!.GetTypeId();
                    return typeId is not null && typeId != ElementId.InvalidElementId
                        ? doc.GetElement(typeId) as HostObjAttributes ?? element as HostObjAttributes
                        : element as HostObjAttributes;
                })
                .Where(host => host is not null)
                .Select(host => host!)
                .Distinct()
            : new FilteredElementCollector(doc).OfClass(typeof(HostObjAttributes)).Cast<HostObjAttributes>();

        foreach (var host in hosts)
        {
            if (result.Count >= safeLimit)
            {
                break;
            }

            var structure = host.GetCompoundStructure();
            if (structure is null)
            {
                continue;
            }

            var layers = structure.GetLayers();
            for (var index = 0; index < layers.Count; index++)
            {
                var layer = layers[index];
                var isInsulation = layer.Function == MaterialFunctionAssignment.Insulation;
                var materialName = layer.MaterialId == ElementId.InvalidElementId
                    ? string.Empty
                    : doc.GetElement(layer.MaterialId)?.Name ?? string.Empty;

                if (isInsulation || !onlyUninsulated)
                {
                    result.Add(new InsulationInfo(
                        (int)host.Id.Value,
                        host.Name,
                        host.Name,
                        isInsulation,
                        Math.Round(layer.Width * MillimetersPerFoot, 2),
                        materialName,
                        index));
                }
            }
        }

        return result;
    }

    public int ManageInsulation(
        object document,
        string operation,
        IReadOnlyList<int> elementIds,
        double? thicknessMm,
        string? materialName,
        bool overrideExisting)
    {
        var doc = ModelingService.RequireDocument(document);
        if (elementIds.Count == 0)
        {
            throw new InvalidOperationException("必须提供 element_ids/cacheId。");
        }

        var normalized = (operation ?? "add").Trim().ToLowerInvariant();
        var affected = 0;

        foreach (var elementId in elementIds)
        {
            var element = doc.GetElement(new ElementId(elementId));
            if (element is null)
            {
                continue;
            }

            var typeId = element.GetTypeId();
            var host = (typeId is not null && typeId != ElementId.InvalidElementId ? doc.GetElement(typeId) : null) as HostObjAttributes
                       ?? element as HostObjAttributes;
            if (host is null)
            {
                continue;
            }

            var structure = host.GetCompoundStructure();
            if (structure is null)
            {
                continue;
            }

            var layers = structure.GetLayers().ToList();
            var insulationIndexes = layers
                .Select((layer, index) => (layer, index))
                .Where(item => item.layer.Function == MaterialFunctionAssignment.Insulation)
                .Select(item => item.index)
                .ToList();

            if (normalized is "remove" or "delete")
            {
                foreach (var index in insulationIndexes.OrderByDescending(index => index))
                {
                    if (layers.Count <= 1)
                    {
                        break;
                    }

                    layers.RemoveAt(index);
                }

                structure.SetLayers(layers);
                host.SetCompoundStructure(structure);
                affected++;
                continue;
            }

            var material = CompoundStructureService.ResolveMaterial(doc, null, materialName);
            var thickness = thicknessMm is > 0 ? thicknessMm.Value / MillimetersPerFoot : 25.0 / MillimetersPerFoot;

            if (insulationIndexes.Count > 0 && !overrideExisting)
            {
                continue;
            }

            if (insulationIndexes.Count > 0)
            {
                var index = insulationIndexes[0];
                var layer = layers[index];
                layer.Width = thickness;
                if (material is not null)
                {
                    layer.MaterialId = material.Id;
                }

                layers[index] = layer;
            }
            else
            {
                layers.Insert(0, new CompoundStructureLayer(
                    thickness,
                    MaterialFunctionAssignment.Insulation,
                    material?.Id ?? ElementId.InvalidElementId));
            }

            structure.SetLayers(layers);
            host.SetCompoundStructure(structure);
            affected++;
        }

        return affected;
    }
}

/// <summary>
/// MEP 系统服务：查询（管道/风管系统 + 成员 + 管径分布）与创建/改名/删除。
/// API 事实：PipingSystem.Create(doc, systemTypeId[, name])；MechanicalSystem.Create(...)；
/// MEPSystem.Elements(ElementSet)/Name；PipingSystem.SystemType。
/// API 校正：27.3 没有 RBS_ADDITIONAL_INSULATION_THICKNESS 内置参数，
/// 因此"是否保温"改为检查成员依赖元素里是否存在 Insulation 类元件。
/// </summary>
internal sealed class MepSystemService : IMepSystemService
{
    public IReadOnlyList<MepSystemInfo> QuerySystems(object document, string operation, int? systemTypeId, bool onlyUninsulated, int limit)
    {
        var doc = ModelingService.RequireDocument(document);
        var safeLimit = limit <= 0 ? 200 : limit;
        var systems = new List<MepSystemInfo>();

        foreach (var system in new FilteredElementCollector(doc).OfClass(typeof(PipingSystem)).Cast<PipingSystem>())
        {
            systems.Add(Describe(doc, system, "piping", "PipingSystem"));
            if (systems.Count >= safeLimit)
            {
                break;
            }
        }

        if (systems.Count < safeLimit)
        {
            foreach (var system in new FilteredElementCollector(doc).OfClass(typeof(MechanicalSystem)).Cast<MechanicalSystem>())
            {
                systems.Add(Describe(doc, system, "mechanical", "MechanicalSystem"));
                if (systems.Count >= safeLimit)
                {
                    break;
                }
            }
        }

        var filtered = systems.AsEnumerable();
        if (systemTypeId is not null)
        {
            filtered = filtered.Where(system => system.SystemTypeName.Contains(systemTypeId.Value.ToString(), StringComparison.Ordinal));
        }

        if (onlyUninsulated)
        {
            filtered = filtered.Where(system => system.UninsulatedMemberCount > 0);
        }

        return filtered.ToList();
    }

    private static MepSystemInfo Describe(Document document, MEPSystem system, string kind, string fallbackTypeName)
    {
        var memberIds = new List<int>();
        var uninsulated = 0;
        var sizes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        try
        {
            foreach (Element member in system.Elements)
            {
                memberIds.Add((int)member.Id.Value);

                if (!HasInsulationElement(document, member))
                {
                    uninsulated++;
                }

                var diameter = member.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM)
                               ?? member.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM);
                if (diameter is not null)
                {
                    var text = Math.Round(diameter.AsDouble() * 304.8, 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    sizes[text] = sizes.TryGetValue(text, out var count) ? count + 1 : 1;
                }
            }
        }
        catch (Exception)
        {
            // 成员不可枚举时留空。
        }

        var systemTypeName = fallbackTypeName;
        try
        {
            // 注意：PipingSystem.SystemType / MechanicalSystem.SystemType 返回的是**枚举**
            // （PipeSystemType / DuctSystemType），不是元素类型；这里取枚举名做展示。
            switch (system)
            {
                case PipingSystem piping:
                    systemTypeName = piping.SystemType.ToString();
                    break;
                case MechanicalSystem mechanical:
                    systemTypeName = mechanical.SystemType.ToString();
                    break;
            }
        }
        catch (Exception)
        {
            // 忽略。
        }

        return new MepSystemInfo(
            (int)system.Id.Value,
            system.Name ?? string.Empty,
            systemTypeName,
            kind,
            memberIds.Count,
            memberIds,
            uninsulated,
            sizes);
    }

    private static bool HasInsulationElement(Document document, Element member)
    {
        try
        {
            return member.GetDependentElements(null)
                .Select(id => document.GetElement(id))
                .Any(element => element is not null &&
                                element.GetType().Name.Contains("Insulation", StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception)
        {
            return false;
        }
    }

    public MepSystemInfo? CreateSystem(object document, int systemTypeId, string? newName)
    {
        var doc = ModelingService.RequireDocument(document);
        var typeId = new ElementId(systemTypeId);
        var type = doc.GetElement(typeId)
                   ?? throw new InvalidOperationException($"未找到系统类型 {systemTypeId}。");

        // 元素类型名在不同版本里可能是 PipingSystemType / MechanicalSystemType，
        // 而 PipeSystemType/DuctSystemType 是枚举；这里按"先试管道再试风管"的兜底链创建，
        // 两条都失败时把 Revit 的原始错误一并抛出，便于定位。
        var typeName = type.GetType().Name;
        var errors = new List<string>();
        MEPSystem? system = null;

        try
        {
            system = string.IsNullOrWhiteSpace(newName)
                ? PipingSystem.Create(doc, typeId)
                : PipingSystem.Create(doc, typeId, newName!);
        }
        catch (Exception ex)
        {
            errors.Add($"PipingSystem: {ex.GetBaseException().Message}");
        }

        if (system is null)
        {
            try
            {
                system = string.IsNullOrWhiteSpace(newName)
                    ? MechanicalSystem.Create(doc, typeId)
                    : MechanicalSystem.Create(doc, typeId, newName!);
            }
            catch (Exception ex)
            {
                errors.Add($"MechanicalSystem: {ex.GetBaseException().Message}");
            }
        }

        if (system is null)
        {
            throw new InvalidOperationException(
                $"元素 {systemTypeId}（{typeName}）不能作为管道/风管系统类型创建系统：{string.Join("；", errors)}");
        }

        var kind = system is PipingSystem ? "piping" : "mechanical";
        return Describe(doc, system, kind, kind == "piping" ? "PipingSystem" : "MechanicalSystem");
    }

    public int RenameSystem(object document, int systemId, string newName)
    {
        var doc = ModelingService.RequireDocument(document);
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new InvalidOperationException("必须提供 new_name/new_system_name。");
        }

        if (doc.GetElement(new ElementId(systemId)) is not MEPSystem system)
        {
            throw new InvalidOperationException($"元素 {systemId} 不是 MEP 系统。");
        }

        system.Name = newName;
        return 1;
    }

    public bool DeleteSystem(object document, int systemId, bool force)
    {
        var doc = ModelingService.RequireDocument(document);
        if (doc.GetElement(new ElementId(systemId)) is not MEPSystem system)
        {
            throw new InvalidOperationException($"元素 {systemId} 不是 MEP 系统。");
        }

        if (system.Elements.Size > 0 && !force)
        {
            throw new InvalidOperationException(
                $"系统 {system.Name} 仍有 {system.Elements.Size} 个成员，删除前请先清空成员或用 force=true 强制删除。");
        }

        var deleted = doc.Delete(system.Id);
        return deleted.Count > 0;
    }
}
