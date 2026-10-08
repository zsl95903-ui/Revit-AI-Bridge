using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 进阶查询服务：元素查询/过滤、房间边界、轴网交点、明细表字段与数据、碰撞检查、删除链接。
/// API 事实（反射核对 27.3）：
///   Room.GetBoundarySegments(SpatialElementBoundaryOptions)；
///   ElementIntersectsElementFilter(Element[, bool])；
///   ViewSchedule.GetTableData().GetSectionData(SectionType) + ViewSchedule.GetCellText；
///   Curve.Intersect 在新版返回 CurveIntersectResult，故轴线交点按解析法算（仅支持直线轴网）。
/// </summary>
internal sealed class AdvancedQueryService : IAdvancedQueryService
{
    public IReadOnlyList<ElementSummary> QueryElements(
        object document,
        string operation,
        IReadOnlyList<int> elementIds,
        string? categoryName,
        int? typeId,
        string? typeName,
        int limit)
    {
        var doc = ModelingService.RequireDocument(document);
        var safeLimit = limit <= 0 ? 200 : limit;
        var normalized = (operation ?? "all").Trim().ToLowerInvariant();

        var collected = new List<Element>();

        switch (normalized)
        {
            case "byid":
            case "id":
            case "ids":
                collected.AddRange(elementIds.Select(id => doc.GetElement(new ElementId(id))).Where(element => element is not null).Select(element => element!));
                break;

            case "bycategory":
            case "category":
                foreach (var element in new FilteredElementCollector(doc).WhereElementIsNotElementType())
                {
                    if (string.Equals(element.Category?.Name, categoryName, StringComparison.OrdinalIgnoreCase))
                    {
                        collected.Add(element);
                        if (collected.Count >= safeLimit)
                        {
                            break;
                        }
                    }
                }

                break;

            case "bytype":
            case "type":
            {
                var targetTypeId = typeId;
                if (targetTypeId is null && !string.IsNullOrWhiteSpace(typeName))
                {
                    targetTypeId = new FilteredElementCollector(doc)
                        .WhereElementIsElementType()
                        .FirstOrDefault(element => string.Equals(element.Name, typeName, StringComparison.OrdinalIgnoreCase))
                        ?.Id is { } foundId
                            ? (int)foundId.Value
                            : null;
                }

                if (targetTypeId is null)
                {
                    throw new InvalidOperationException("operation=byType 需要提供 typeId 或 typeName。");
                }

                foreach (var element in new FilteredElementCollector(doc).WhereElementIsNotElementType())
                {
                    var elementTypeId = element.GetTypeId();
                    if (elementTypeId is not null && (int)elementTypeId.Value == targetTypeId.Value)
                    {
                        collected.Add(element);
                        if (collected.Count >= safeLimit)
                        {
                            break;
                        }
                    }
                }

                break;
            }

            default:
                collected.AddRange(new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .Take(safeLimit));
                break;
        }

        return collected.Take(safeLimit).Select(Summarize).ToList();
    }

    public IReadOnlyList<ElementSummary> FilterElements(
        object document,
        IReadOnlyList<int> elementIds,
        string? categoryName,
        string? typeName,
        string? levelName,
        int limit)
    {
        var doc = ModelingService.RequireDocument(document);
        var safeLimit = limit <= 0 ? 200 : limit;

        var source = elementIds.Count > 0
            ? elementIds.Select(id => doc.GetElement(new ElementId(id))).Where(element => element is not null).Select(element => element!)
            : new FilteredElementCollector(doc).WhereElementIsNotElementType();

        var result = new List<ElementSummary>();
        foreach (var element in source)
        {
            if (result.Count >= safeLimit)
            {
                break;
            }

            if (!string.IsNullOrWhiteSpace(categoryName) &&
                !string.Equals(element.Category?.Name, categoryName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(typeName))
            {
                var elementType = element.GetTypeId() is { } typeId && typeId != ElementId.InvalidElementId
                    ? doc.GetElement(typeId)
                    : null;
                if (!string.Equals(elementType?.Name, typeName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }

            if (!string.IsNullOrWhiteSpace(levelName))
            {
                var level = element.LevelId is { } levelId && levelId != ElementId.InvalidElementId
                    ? doc.GetElement(levelId)
                    : null;
                if (!string.Equals(level?.Name, levelName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }

            result.Add(Summarize(element));
        }

        return result;
    }

    private static ElementSummary Summarize(Element element)
    {
        var doc = element.Document;
        var typeId = element.GetTypeId();
        var typeElement = typeId is not null && typeId != ElementId.InvalidElementId ? doc.GetElement(typeId) : null;
        var levelElement = element.LevelId is { } levelId && levelId != ElementId.InvalidElementId ? doc.GetElement(levelId) : null;

        return new ElementSummary(
            (int)element.Id.Value,
            element.Name ?? string.Empty,
            element.Category?.Name ?? string.Empty,
            typeId is null ? -1 : (int)typeId.Value,
            typeElement?.Name ?? string.Empty,
            element.LevelId is null ? -1 : (int)element.LevelId.Value,
            levelElement?.Name ?? string.Empty);
    }

    public IReadOnlyList<BoundaryLoopInfo> GetRoomBoundaries(object document, int roomId)
    {
        var doc = ModelingService.RequireDocument(document);
        var room = doc.GetElement(new ElementId(roomId)) as Room
                   ?? throw new InvalidOperationException($"元素 {roomId} 不是房间（Room）。");

        var options = new SpatialElementBoundaryOptions();
        var loops = room.GetBoundarySegments(options);
        if (loops is null)
        {
            return Array.Empty<BoundaryLoopInfo>();
        }

        var result = new List<BoundaryLoopInfo>();
        for (var index = 0; index < loops.Count; index++)
        {
            var segments = loops[index];
            var points = new List<Point2>();
            var length = 0.0;

            foreach (var segment in segments)
            {
                var curve = segment.GetCurve();
                length += curve.Length;
                foreach (var point in curve.Tessellate())
                {
                    points.Add(new Point2(Math.Round(point.X * 304.8, 2), Math.Round(point.Y * 304.8, 2)));
                }
            }

            result.Add(new BoundaryLoopInfo(index, index == 0, Math.Round(length * 304.8, 2), points));
        }

        return result;
    }

    public GridIntersectionInfo GetGridIntersection(object document, int grid1Id, int grid2Id)
    {
        var doc = ModelingService.RequireDocument(document);

        var first = doc.GetElement(new ElementId(grid1Id)) as Grid
                    ?? throw new InvalidOperationException($"元素 {grid1Id} 不是轴网。");
        var second = doc.GetElement(new ElementId(grid2Id)) as Grid
                     ?? throw new InvalidOperationException($"元素 {grid2Id} 不是轴网。");

        if ((first.Location as LocationCurve)?.Curve is not Line line1 ||
            (second.Location as LocationCurve)?.Curve is not Line line2)
        {
            return new GridIntersectionInfo(false, 0, 0, 0, "仅支持直线轴网求交点（弧形轴网请用曲线求交接口）。");
        }

        var p1 = line1.GetEndPoint(0);
        var p2 = line1.GetEndPoint(1);
        var p3 = line2.GetEndPoint(0);
        var p4 = line2.GetEndPoint(1);

        var d1 = new Point2Double(p2.X - p1.X, p2.Y - p1.Y);
        var d2 = new Point2Double(p4.X - p3.X, p4.Y - p3.Y);
        var denominator = d1.X * d2.Y - d1.Y * d2.X;

        if (Math.Abs(denominator) < 1e-12)
        {
            return new GridIntersectionInfo(false, 0, 0, 0, "两条轴网平行或重合，没有唯一交点。");
        }

        var dx = p3.X - p1.X;
        var dy = p3.Y - p1.Y;
        var t = (dx * d2.Y - dy * d2.X) / denominator;
        var x = p1.X + t * d1.X;
        var y = p1.Y + t * d1.Y;

        return new GridIntersectionInfo(
            true,
            Math.Round(x * 304.8, 2),
            Math.Round(y * 304.8, 2),
            0,
            $"轴网 {first.Name} × {second.Name}");
    }

    private readonly record struct Point2Double(double X, double Y);

    public IReadOnlyList<ScheduleFieldInfo> GetScheduleFields(object document, int? scheduleId, string? scheduleName)
    {
        var schedule = RequireSchedule(document, scheduleId, scheduleName);
        var definition = schedule.Definition;
        var result = new List<ScheduleFieldInfo>();

        for (var index = 0; index < definition.GetFieldCount(); index++)
        {
            var field = definition.GetField(index);
            string heading;
            try
            {
                heading = field.ColumnHeading ?? string.Empty;
            }
            catch (Exception)
            {
                heading = string.Empty;
            }

            result.Add(new ScheduleFieldInfo(index, field.GetName(), field.FieldType.ToString(), field.IsHidden, heading));
        }

        return result;
    }

    public ScheduleDataInfo ReadScheduleData(
        object document,
        int? scheduleId,
        string? scheduleName,
        int startRow,
        int maxRows,
        bool includeHeader)
    {
        var schedule = RequireSchedule(document, scheduleId, scheduleName);
        var tableData = schedule.GetTableData();
        var header = tableData.GetSectionData(SectionType.Header);
        var body = tableData.GetSectionData(SectionType.Body);

        var headers = new List<string>();
        if (includeHeader)
        {
            for (var column = 0; column < header.NumberOfColumns; column++)
            {
                headers.Add(SafeCell(schedule, SectionType.Header, 0, column));
            }
        }

        var rows = new List<IReadOnlyList<string>>();
        var safeStart = Math.Max(0, startRow);
        var safeMax = maxRows <= 0 ? 200 : maxRows;
        var end = Math.Min(body.NumberOfRows, safeStart + safeMax);

        for (var row = safeStart; row < end; row++)
        {
            var cells = new List<string>();
            for (var column = 0; column < body.NumberOfColumns; column++)
            {
                cells.Add(SafeCell(schedule, SectionType.Body, row, column));
            }

            rows.Add(cells);
        }

        return new ScheduleDataInfo(schedule.Name, headers, rows, body.NumberOfRows, rows.Count);
    }

    public IReadOnlyList<CollisionPairInfo> CheckCollision(object document, IReadOnlyList<int> sourceIds, IReadOnlyList<int> targetIds)
    {
        var doc = ModelingService.RequireDocument(document);
        var result = new List<CollisionPairInfo>();

        var sources = sourceIds.Count > 0
            ? sourceIds.Select(id => doc.GetElement(new ElementId(id))).Where(element => element is not null).Select(element => element!).ToList()
            : new FilteredElementCollector(doc).WhereElementIsNotElementType().Take(50).ToList();

        var targets = targetIds.Count > 0
            ? targetIds.Select(id => doc.GetElement(new ElementId(id))).Where(element => element is not null).Select(element => element!).ToList()
            : new FilteredElementCollector(doc).WhereElementIsNotElementType().Take(200).ToList();

        var targetIdSet = targets.Select(element => (int)element.Id.Value).ToHashSet();

        foreach (var source in sources.Take(50))
        {
            try
            {
                var filter = new ElementIntersectsElementFilter(source);
                var hits = new FilteredElementCollector(doc)
                    .WherePasses(filter)
                    .WhereElementIsNotElementType()
                    .Select(element => (int)element.Id.Value)
                    .Where(id => targetIdSet.Contains(id))
                    .ToList();

                if (hits.Count == 0)
                {
                    result.Add(new CollisionPairInfo((int)source.Id.Value, -1, false));
                }
                else
                {
                    result.AddRange(hits.Select(hit => new CollisionPairInfo((int)source.Id.Value, hit, true)));
                }
            }
            catch (Exception)
            {
                // 不支持相交过滤的元素跳过。
            }
        }

        return result;
    }

    public bool DeleteLink(object document, int linkId)
    {
        var doc = ModelingService.RequireDocument(document);
        var element = doc.GetElement(new ElementId(linkId))
                      ?? throw new InvalidOperationException($"未找到链接元素 {linkId}。");

        if (element is not RevitLinkInstance && element is not RevitLinkType)
        {
            throw new InvalidOperationException($"元素 {linkId}（{element.GetType().Name}）不是 Revit 链接，"
                + "本工具只删除链接（避免误删模型元素）；删除模型元素请用 delete_elements。");
        }

        var deleted = doc.Delete(element.Id);
        return deleted.Count > 0;
    }

    private static ViewSchedule RequireSchedule(object? document, int? scheduleId, string? scheduleName)
    {
        var doc = ModelingService.RequireDocument(document!);

        var schedule = (scheduleId is not null ? doc.GetElement(new ElementId(scheduleId.Value)) as ViewSchedule : null)
                       ?? new FilteredElementCollector(doc)
                           .OfClass(typeof(ViewSchedule))
                           .Cast<ViewSchedule>()
                           .FirstOrDefault(item => string.Equals(item.Name, scheduleName, StringComparison.OrdinalIgnoreCase));

        return schedule ?? throw new InvalidOperationException(
            scheduleId is not null ? $"未找到明细表 {scheduleId}。" : $"未找到明细表 {scheduleName}。");
    }

    private static string SafeCell(ViewSchedule schedule, SectionType section, int row, int column)
    {
        try
        {
            return schedule.GetCellText(section, row, column) ?? string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}

/// <summary>
/// 项目设置服务：项目单位（长度/面积/体积/角度/坡度）与视图类别可见性。
/// API 事实：doc.GetUnits()/doc.SetUnits(Units)；Units.SetFormatOptions(ForgeTypeId, FormatOptions)；
/// FormatOptions(ForgeTypeId unitTypeId)；View.SetCategoryHidden(ElementId, bool)。
/// </summary>
internal sealed class ProjectSettingsService : IProjectSettingsService
{
    public int SetProjectUnits(object document, string? length, string? area, string? volume, string? angle, string? slope)
    {
        var doc = ModelingService.RequireDocument(document);
        var units = doc.GetUnits();
        var changed = 0;

        void Apply(ForgeTypeId specTypeId, ForgeTypeId? unitTypeId)
        {
            if (unitTypeId is null)
            {
                return;
            }

            units.SetFormatOptions(specTypeId, new FormatOptions(unitTypeId));
            changed++;
        }

        Apply(SpecTypeId.Length, ResolveUnitTypeId(length, LengthUnits));
        Apply(SpecTypeId.Area, ResolveUnitTypeId(area, AreaUnits));
        Apply(SpecTypeId.Volume, ResolveUnitTypeId(volume, VolumeUnits));
        Apply(SpecTypeId.Angle, ResolveUnitTypeId(angle, AngleUnits));
        Apply(SpecTypeId.Slope, ResolveUnitTypeId(slope, SlopeUnits));

        if (changed == 0)
        {
            throw new InvalidOperationException(
                "没有可应用的单位设置；请至少提供 length/area/volume/angle/slope 之一。"
                + "支持：length=mm|cm|m|ft|inch，area=m2|ft2，volume=m3|ft3，angle=deg|rad，slope=deg|percent|ratio。");
        }

        doc.SetUnits(units);
        return changed;
    }

    public int SetCategoryVisibility(object document, int viewId, string categoryName, bool visible)
    {
        var doc = ModelingService.RequireDocument(document);
        var view = doc.GetElement(new ElementId(viewId)) as View
                   ?? throw new InvalidOperationException($"未找到视图 {viewId}。");

        if (view.IsTemplate)
        {
            throw new InvalidOperationException($"视图 {view.Name} 是视图样板，不能直接改类别可见性。");
        }

        Category? category = null;
        foreach (Category candidate in doc.Settings.Categories)
        {
            if (string.Equals(candidate.Name, categoryName, StringComparison.OrdinalIgnoreCase))
            {
                category = candidate;
                break;
            }
        }

        if (category is null)
        {
            throw new InvalidOperationException($"未找到类别 {categoryName}。");
        }

        view.SetCategoryHidden(category.Id, !visible);
        return 1;
    }

    private static readonly Dictionary<string, ForgeTypeId> LengthUnits = new(StringComparer.OrdinalIgnoreCase)
    {
        ["mm"] = UnitTypeId.Millimeters,
        ["毫米"] = UnitTypeId.Millimeters,
        ["cm"] = UnitTypeId.Centimeters,
        ["m"] = UnitTypeId.Meters,
        ["米"] = UnitTypeId.Meters,
        ["ft"] = UnitTypeId.Feet,
        ["feet"] = UnitTypeId.Feet,
        ["inch"] = UnitTypeId.Inches,
        ["in"] = UnitTypeId.Inches,
    };

    private static readonly Dictionary<string, ForgeTypeId> AreaUnits = new(StringComparer.OrdinalIgnoreCase)
    {
        ["m2"] = UnitTypeId.SquareMeters,
        ["平方米"] = UnitTypeId.SquareMeters,
        ["ft2"] = UnitTypeId.SquareFeet,
        ["mm2"] = UnitTypeId.SquareMillimeters,
    };

    private static readonly Dictionary<string, ForgeTypeId> VolumeUnits = new(StringComparer.OrdinalIgnoreCase)
    {
        ["m3"] = UnitTypeId.CubicMeters,
        ["立方米"] = UnitTypeId.CubicMeters,
        ["ft3"] = UnitTypeId.CubicFeet,
    };

    private static readonly Dictionary<string, ForgeTypeId> AngleUnits = new(StringComparer.OrdinalIgnoreCase)
    {
        ["deg"] = UnitTypeId.Degrees,
        ["degree"] = UnitTypeId.Degrees,
        ["度"] = UnitTypeId.Degrees,
        ["rad"] = UnitTypeId.Radians,
        ["弧度"] = UnitTypeId.Radians,
    };

    private static readonly Dictionary<string, ForgeTypeId> SlopeUnits = new(StringComparer.OrdinalIgnoreCase)
    {
        ["deg"] = UnitTypeId.Degrees,
        ["degree"] = UnitTypeId.Degrees,
        ["slopedegrees"] = UnitTypeId.SlopeDegrees,
        // API 校正：UnitTypeId 没有 Percent/Ratio，实际是 Percentage / RatioTo1。
        ["percent"] = UnitTypeId.Percentage,
        ["百分比"] = UnitTypeId.Percentage,
        ["ratio"] = UnitTypeId.RatioTo1,
        ["比值"] = UnitTypeId.RatioTo1,
    };

    private static ForgeTypeId? ResolveUnitTypeId(string? text, IDictionary<string, ForgeTypeId> map)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return map.TryGetValue(text.Trim(), out var unitTypeId) ? unitTypeId : null;
    }
}
