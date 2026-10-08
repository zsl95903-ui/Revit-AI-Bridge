using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 批次 1 第二批只读服务：族 / 房间 / 标记 / 元素几何 / 项目单位 / 视图查询 / 类型列表。
/// 一律 try/catch 兜底，单个元素异常不影响整体返回。
/// 说明：RevitAPI 27.3 里 Family 用 GetFamilySymbolIds()；Room 在 Autodesk.Revit.DB.Architecture；
/// View 未公开读取"过滤器启用状态"，Enabled 固定为 true（表示该过滤器已挂在视图上）。
/// </summary>
internal sealed class FamilyQueryService : IFamilyQueryService
{
    public IReadOnlyList<FamilyInfo> GetAllFamilies(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<FamilyInfo>();
        }

        return new FilteredElementCollector(doc)
            .OfClass(typeof(Family))
            .Cast<Family>()
            .Select(family => new FamilyInfo(
                (int)family.Id.Value,
                family.Name,
                family.FamilyCategory?.Name ?? string.Empty,
                CountTypes(family)))
            .ToArray();
    }

    public IReadOnlyList<FamilyTypeInfo> GetFamilyTypes(object document, string? familyName, int limit)
    {
        if (document is not Document doc)
        {
            return Array.Empty<FamilyTypeInfo>();
        }

        var query = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilySymbol))
            .Cast<FamilySymbol>();

        if (!string.IsNullOrWhiteSpace(familyName))
        {
            query = query.Where(symbol =>
                string.Equals(symbol.FamilyName, familyName, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .Take(limit <= 0 ? int.MaxValue : limit)
            .Select(ToInfo)
            .ToArray();
    }

    internal static FamilyTypeInfo ToInfo(FamilySymbol symbol)
        => new((int)symbol.Id.Value, symbol.FamilyName, symbol.Name, symbol.Category?.Name ?? string.Empty);

    private static int CountTypes(Family family)
    {
        try
        {
            return family.GetFamilySymbolIds().Count;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}

internal sealed class RoomService : IRoomService
{
    private const double SquareFeetToSquareMeters = 0.09290304;

    public IReadOnlyList<RoomInfo> GetAllRooms(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<RoomInfo>();
        }

        return new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .Cast<Room>()
            .Select(room => new RoomInfo(
                (int)room.Id.Value,
                room.Number ?? string.Empty,
                room.Name ?? string.Empty,
                Math.Round(room.Area * SquareFeetToSquareMeters, 3),
                room.Level is null ? 0 : (int)room.Level.Id.Value,
                room.Level?.Name ?? string.Empty))
            .ToArray();
    }
}

internal sealed class TagService : ITagService
{
    public IReadOnlyList<TagInfo> GetAllTags(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<TagInfo>();
        }

        var result = new List<TagInfo>();
        foreach (var tag in new FilteredElementCollector(doc).OfClass(typeof(IndependentTag)).Cast<IndependentTag>())
        {
            try
            {
                result.Add(new TagInfo(
                    (int)tag.Id.Value,
                    tag.TagText ?? string.Empty,
                    TryGetTaggedElementId(tag),
                    tag.Category?.Name ?? string.Empty));
            }
            catch (Exception)
            {
                // 个别标记取不到文本/宿主时跳过。
            }
        }

        return result;
    }

    private static int? TryGetTaggedElementId(IndependentTag tag)
    {
        try
        {
            foreach (var reference in tag.GetTaggedElementIds())
            {
                return (int)reference.HostElementId.Value;
            }
        }
        catch (Exception)
        {
            // 特殊标记没有 GetTaggedElementIds，忽略。
        }

        return null;
    }
}

internal sealed class ElementQueryService : IElementQueryService
{
    private const double MillimetersPerFoot = 304.8;

    public IReadOnlyList<ElementSummaryInfo> GetAllElements(object document, string? categoryName, int limit)
    {
        if (document is not Document doc)
        {
            return Array.Empty<ElementSummaryInfo>();
        }

        var query = new FilteredElementCollector(doc)
            .WhereElementIsNotElementType()
            .Where(element => element.Category is not null);

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(element =>
                string.Equals(element.Category!.Name, categoryName, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .Take(limit <= 0 ? 500 : limit)
            .Select(element => new ElementSummaryInfo(
                (int)element.Id.Value,
                SafeName(element),
                element.Category?.Name ?? string.Empty,
                doc.GetElement(element.GetTypeId())?.Name ?? string.Empty))
            .ToArray();
    }

    public ElementGeometryInfo? GetElementGeometry(object document, int elementId)
    {
        if (document is not Document doc)
        {
            return null;
        }

        var element = doc.GetElement(new ElementId(elementId));
        if (element is null)
        {
            return null;
        }

        var locationKind = "None";
        var x = 0d;
        var y = 0d;
        var z = 0d;

        switch (element.Location)
        {
            case LocationPoint point:
                locationKind = "Point";
                x = point.Point.X;
                y = point.Point.Y;
                z = point.Point.Z;
                break;
            case LocationCurve curve:
                locationKind = "Curve";
                var midpoint = curve.Curve.Evaluate(0.5, true);
                x = midpoint.X;
                y = midpoint.Y;
                z = midpoint.Z;
                break;
        }

        var hasBox = false;
        double minX = 0, minY = 0, minZ = 0, maxX = 0, maxY = 0, maxZ = 0;
        if (element.get_BoundingBox(null) is { } box)
        {
            hasBox = true;
            minX = box.Min.X * MillimetersPerFoot;
            minY = box.Min.Y * MillimetersPerFoot;
            minZ = box.Min.Z * MillimetersPerFoot;
            maxX = box.Max.X * MillimetersPerFoot;
            maxY = box.Max.Y * MillimetersPerFoot;
            maxZ = box.Max.Z * MillimetersPerFoot;
        }

        var (solids, curves) = CountGeometry(element);

        return new ElementGeometryInfo(
            elementId,
            element.Category?.Name ?? string.Empty,
            doc.GetElement(element.GetTypeId())?.Name ?? string.Empty,
            locationKind,
            Math.Round(x * MillimetersPerFoot, 2),
            Math.Round(y * MillimetersPerFoot, 2),
            Math.Round(z * MillimetersPerFoot, 2),
            hasBox,
            Math.Round(minX, 2),
            Math.Round(minY, 2),
            Math.Round(minZ, 2),
            Math.Round(maxX, 2),
            Math.Round(maxY, 2),
            Math.Round(maxZ, 2),
            solids,
            curves,
            "millimeters");
    }

    private static (int Solids, int Curves) CountGeometry(Element element)
    {
        try
        {
            var options = new Options
            {
                DetailLevel = ViewDetailLevel.Fine,
                ComputeReferences = false,
                IncludeNonVisibleObjects = false,
            };

            var solids = 0;
            var curves = 0;
            Collect(element.get_Geometry(options), ref solids, ref curves);
            return (solids, curves);
        }
        catch (Exception)
        {
            return (0, 0);
        }
    }

    private static void Collect(GeometryElement? geometry, ref int solids, ref int curves)
    {
        if (geometry is null)
        {
            return;
        }

        foreach (var item in geometry)
        {
            switch (item)
            {
                case Solid solid when solid.Faces.Size > 0 || solid.Edges.Size > 0:
                    solids++;
                    break;
                case Curve:
                    curves++;
                    break;
                case GeometryInstance instance:
                    Collect(instance.GetInstanceGeometry(), ref solids, ref curves);
                    break;
            }
        }
    }

    private static string SafeName(Element element)
    {
        try
        {
            return element.Name ?? string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}

internal sealed class ProjectUnitService : IProjectUnitService
{
    public ProjectUnitsInfo GetProjectUnits(object document)
    {
        if (document is not Document doc)
        {
            return new ProjectUnitsInfo("unknown", string.Empty, "unknown", "unknown", "unknown", "decimal feet");
        }

        var units = doc.GetUnits();
        return new ProjectUnitsInfo(
            Describe(units, SpecTypeId.Length),
            Symbol(units, SpecTypeId.Length),
            Describe(units, SpecTypeId.Area),
            Describe(units, SpecTypeId.Volume),
            Describe(units, SpecTypeId.Angle),
            "decimal feet");
    }

    private static string Describe(Units units, ForgeTypeId specTypeId)
    {
        try
        {
            return units.GetFormatOptions(specTypeId).GetUnitTypeId()?.TypeId ?? "unknown";
        }
        catch (Exception)
        {
            return "unknown";
        }
    }

    private static string Symbol(Units units, ForgeTypeId specTypeId)
    {
        try
        {
            var unitTypeId = units.GetFormatOptions(specTypeId).GetUnitTypeId();
            return LabelUtils.GetLabelForUnit(unitTypeId) ?? string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}

internal sealed class ViewQueryService : IViewQueryService
{
    public ViewSettingsInfo? GetViewSettings(object document, int? viewId)
    {
        if (ResolveView(document, viewId) is not { } view)
        {
            return null;
        }

        return new ViewSettingsInfo(
            (int)view.Id.Value,
            view.Name,
            view.ViewType.ToString(),
            view.Scale,
            view.DetailLevel.ToString(),
            view.DisplayStyle.ToString(),
            view.CropBoxActive,
            view.CropBoxVisible);
    }

    public IReadOnlyList<ViewFilterInfo> GetViewFilters(object document, int? viewId)
    {
        if (document is not Document doc || ResolveView(document, viewId) is not { } view)
        {
            return Array.Empty<ViewFilterInfo>();
        }

        var result = new List<ViewFilterInfo>();
        foreach (var filterId in view.GetFilters())
        {
            try
            {
                result.Add(new ViewFilterInfo(
                    (int)filterId.Value,
                    doc.GetElement(filterId)?.Name ?? string.Empty,
                    true,
                    view.GetFilterVisibility(filterId)));
            }
            catch (Exception)
            {
                // 过滤器被删除等异常情况下跳过。
            }
        }

        return result;
    }

    public CategoryVisibilityInfo? GetCategoryVisibility(object document, int? viewId, string categoryName)
    {
        if (document is not Document doc || ResolveView(document, viewId) is not { } view)
        {
            return null;
        }

        foreach (Category category in doc.Settings.Categories)
        {
            if (!string.Equals(category.Name, categoryName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            try
            {
                return new CategoryVisibilityInfo((int)view.Id.Value, category.Name, !view.GetCategoryHidden(category.Id));
            }
            catch (Exception)
            {
                return null;
            }
        }

        return null;
    }

    private static View? ResolveView(object document, int? viewId)
    {
        if (document is not Document doc)
        {
            return null;
        }

        return viewId is null ? doc.ActiveView : doc.GetElement(new ElementId(viewId.Value)) as View;
    }
}

internal sealed class TypeService : ITypeService
{
    public IReadOnlyList<FamilyTypeInfo> GetTypes(object document, string categoryKey, int limit)
    {
        if (document is not Document doc || !TryMapCategory(categoryKey, out var builtInCategory))
        {
            return Array.Empty<FamilyTypeInfo>();
        }

        return new FilteredElementCollector(doc)
            .OfCategory(builtInCategory)
            .WhereElementIsElementType()
            .OfType<FamilySymbol>()
            .Take(limit <= 0 ? 200 : limit)
            .Select(FamilyQueryService.ToInfo)
            .ToArray();
    }

    private static bool TryMapCategory(string categoryKey, out BuiltInCategory category)
    {
        switch (categoryKey?.Trim().ToLowerInvariant())
        {
            case "pipe":
                category = BuiltInCategory.OST_PipeCurves;
                return true;
            case "duct":
                category = BuiltInCategory.OST_DuctCurves;
                return true;
            case "cabletray":
                category = BuiltInCategory.OST_CableTray;
                return true;
            case "roof":
                category = BuiltInCategory.OST_Roofs;
                return true;
            default:
                category = BuiltInCategory.INVALID;
                return false;
        }
    }
}
