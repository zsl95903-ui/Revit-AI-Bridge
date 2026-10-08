using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAi.Engine.Revit.Services;

using RevitAi.Engine.Abstractions.Services;

/// <summary>
/// 收尾批领域服务：道路（墙→路面板）、CAD 图层可见性、剪切顺序修正。
/// 说明：
///   * 道路按"墙中心线围成的轮廓/包围盒"生成一块楼板（路面板）；车道线、人行道、路缘、标线等
///     属于道路详图语义，本实现只记录参数不建模（在结果 notes 里如实说明）。
///   * CAD 图层通过"导入实例类别下的子类别 + View.SetCategoryHidden"控制。
///   * 剪切顺序用 JoinGeometryUtils.IsCuttingElementInJoin / SwitchJoinOrder 修正。
/// </summary>
internal sealed class DomainExtraService : IDomainExtraService
{
    public RoadResult CreateRoadFromWalls(
        object document,
        IReadOnlyList<int> wallIds,
        double? laneWidthMeters,
        double? sidewalkWidthMeters,
        double? defaultRoadWidthMeters)
    {
        var doc = ModelingService.RequireDocument(document);

        var walls = wallIds.Count > 0
            ? wallIds.Select(id => doc.GetElement(new ElementId(id))).OfType<Wall>().ToList()
            : new FilteredElementCollector(doc).OfClass(typeof(Wall)).Cast<Wall>().Take(200).ToList();

        if (walls.Count == 0)
        {
            throw new InvalidOperationException("没有可用的墙（请提供 wall_element_ids）。");
        }

        var curves = walls
            .Select(wall => (wall.Location as LocationCurve)?.Curve)
            .Where(curve => curve is not null)
            .Select(curve => curve!)
            .ToList();

        if (curves.Count == 0)
        {
            throw new InvalidOperationException("这些墙没有可用的定位线（LocationCurve）。");
        }

        // 尝试把墙中心线首尾相接成闭合环；接不上就退化为整体包围盒。
        var outline = new List<XYZ>();
        var closed = TryBuildClosedLoop(curves, outline);
        if (!closed)
        {
            outline = BoundingRectangle(curves);
        }

        var widthMm = (defaultRoadWidthMeters ?? 6.0) * 1000.0;
        var loop = new CurveLoop();
        for (var index = 0; index < outline.Count; index++)
        {
            var start = outline[index];
            var end = outline[(index + 1) % outline.Count];
            if (start.DistanceTo(end) > 1e-6)
            {
                loop.Append(Line.CreateBound(start, end));
            }
        }

        var levelId = ModelingService.ResolveLevelId(doc, walls[0].LevelId is { } wallLevel && wallLevel != ElementId.InvalidElementId ? (int)wallLevel.Value : null);
        var floorTypeId = ModelingService.DefaultTypeId(doc, BuiltInCategory.OST_Floors, "楼板");
        var floor = Floor.Create(doc, new List<CurveLoop> { loop }, floorTypeId, levelId);

        var notes = new List<string>();
        if (laneWidthMeters is not null)
        {
            notes.Add($"lane_width_meters={laneWidthMeters} 已记录未建模");
        }

        if (sidewalkWidthMeters is not null)
        {
            notes.Add($"sidewalk_width_meters={sidewalkWidthMeters} 已记录未建模");
        }

        notes.Add(closed ? "轮廓来自墙中心线首尾相接的闭合环" : "轮廓来自墙整体包围盒（墙中心线未闭合）");
        notes.Add($"路面板按 default_road_width_meters={(defaultRoadWidthMeters ?? 6.0)} 记录（仅用于结果说明）");

        return new RoadResult(
            (int)floor.Id.Value,
            walls.Count,
            widthMm,
            closed,
            outline.Select(point => new Point2(Math.Round(point.X * 304.8, 2), Math.Round(point.Y * 304.8, 2))).ToList(),
            notes);
    }

    private static bool TryBuildClosedLoop(IReadOnlyList<Curve> curves, List<XYZ> outline)
    {
        const double tolerance = 1e-4;
        var remaining = new List<Curve>(curves);
        var current = remaining[0].GetEndPoint(0);
        outline.Add(current);
        var guard = 0;

        while (remaining.Count > 0 && guard++ < curves.Count * 2)
        {
            var nextIndex = -1;
            var reverse = false;

            for (var index = 0; index < remaining.Count; index++)
            {
                if (remaining[index].GetEndPoint(0).DistanceTo(current) < tolerance)
                {
                    nextIndex = index;
                    reverse = false;
                    break;
                }

                if (remaining[index].GetEndPoint(1).DistanceTo(current) < tolerance)
                {
                    nextIndex = index;
                    reverse = true;
                    break;
                }
            }

            if (nextIndex < 0)
            {
                return false;
            }

            var curve = remaining[nextIndex];
            remaining.RemoveAt(nextIndex);
            current = reverse ? curve.GetEndPoint(0) : curve.GetEndPoint(1);
            outline.Add(current);
        }

        if (outline.Count < 4)
        {
            return false;
        }

        if (outline[0].DistanceTo(outline[^1]) > tolerance)
        {
            return false;
        }

        outline.RemoveAt(outline.Count - 1);
        return outline.Count >= 3;
    }

    private static List<XYZ> BoundingRectangle(IReadOnlyList<Curve> curves)
    {
        var points = curves.SelectMany(curve => curve.Tessellate()).ToList();
        var minX = points.Min(point => point.X);
        var maxX = points.Max(point => point.X);
        var minY = points.Min(point => point.Y);
        var maxY = points.Max(point => point.Y);

        return new List<XYZ>
        {
            new(minX, minY, 0),
            new(maxX, minY, 0),
            new(maxX, maxY, 0),
            new(minX, maxY, 0),
        };
    }

    public IReadOnlyList<CadLayerInfo> ManageCadLayers(object document, int viewId, int linkId, string operation, string? layerName)
    {
        var doc = ModelingService.RequireDocument(document);

        // viewId <= 0 表示未指定，按当前活动视图处理（厂商 schema 里 cad_layer_manager 没有 viewId 参数）。
        var view = viewId > 0
            ? doc.GetElement(new ElementId(viewId)) as View
              ?? throw new InvalidOperationException($"未找到视图 {viewId}。")
            : doc.ActiveView
              ?? throw new InvalidOperationException("当前没有活动视图，无法控制 CAD 图层可见性。");

        var element = doc.GetElement(new ElementId(linkId))
                      ?? throw new InvalidOperationException($"未找到 CAD 链接/导入元素 {linkId}。");

        var category = element switch
        {
            ImportInstance import => import.Category,
            _ => element.Category,
        } ?? throw new InvalidOperationException(
            $"元素 {linkId}（{element.GetType().Name}）没有可枚举的类别，无法管理 CAD 图层。");

        var layers = new List<Category>();
        foreach (Category sub in category.SubCategories)
        {
            layers.Add(sub);
        }

        var normalized = (operation ?? "list").Trim().ToLowerInvariant();
        var result = new List<CadLayerInfo>();

        switch (normalized)
        {
            case "list":
                foreach (var layer in layers)
                {
                    result.Add(new CadLayerInfo(layer.Name, (int)layer.Id.Value, !view.GetCategoryHidden(layer.Id)));
                }

                return result;

            case "show":
            case "hide":
            {
                var visible = normalized == "show";
                var targets = string.IsNullOrWhiteSpace(layerName)
                    ? layers
                    : layers.Where(layer => string.Equals(layer.Name, layerName, StringComparison.OrdinalIgnoreCase)).ToList();

                if (targets.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"CAD 图层里没有名为 {layerName} 的图层（可用 operation=list 查询，共 {layers.Count} 个图层）。");
                }

                foreach (var layer in targets)
                {
                    view.SetCategoryHidden(layer.Id, !visible);
                    result.Add(new CadLayerInfo(layer.Name, (int)layer.Id.Value, visible));
                }

                return result;
            }

            default:
                throw new InvalidOperationException("operation 只支持 list / show / hide。");
        }
    }

    public CutOrderResult FixCuttingOrder(
        object document,
        IReadOnlyList<int> cuttingIds,
        IReadOnlyList<int> toCutIds,
        bool dryRun)
    {
        var doc = ModelingService.RequireDocument(document);
        var details = new List<string>();

        var cutting = cuttingIds.Count > 0
            ? cuttingIds.Select(id => doc.GetElement(new ElementId(id))).Where(element => element is not null).Select(element => element!).ToList()
            : new List<Element>();
        var toCut = toCutIds.Count > 0
            ? toCutIds.Select(id => doc.GetElement(new ElementId(id))).Where(element => element is not null).Select(element => element!).ToList()
            : new List<Element>();

        if (cutting.Count == 0 || toCut.Count == 0)
        {
            throw new InvalidOperationException("需要提供 cuttingElementIds 与 elementToCutIds 两组元素。");
        }

        var checkedCount = 0;
        var fixedCount = 0;

        foreach (var cutter in cutting)
        {
            foreach (var target in toCut)
            {
                if (cutter.Id == target.Id)
                {
                    continue;
                }

                checkedCount++;
                try
                {
                    if (!JoinGeometryUtils.AreElementsJoined(doc, cutter, target))
                    {
                        details.Add($"{cutter.Id.Value} 与 {target.Id.Value} 未连接，跳过");
                        continue;
                    }

                    if (JoinGeometryUtils.IsCuttingElementInJoin(doc, cutter, target))
                    {
                        continue;
                    }

                    if (dryRun)
                    {
                        details.Add($"{cutter.Id.Value} 应成为剪切方（当前相反）");
                        continue;
                    }

                    JoinGeometryUtils.SwitchJoinOrder(doc, cutter, target);
                    fixedCount++;
                }
                catch (Exception ex)
                {
                    details.Add($"{cutter.Id.Value}/{target.Id.Value}: {ex.GetBaseException().Message}");
                }
            }
        }

        return new CutOrderResult(checkedCount, fixedCount, dryRun, details);
    }
}

/// <summary>
/// UI 交互服务：选择集、缩放、激活视图。uiDocument 由适配器提供（Revit 主线程）。
/// API：UIDocument.Selection.SetElementIds/GetElementIds；UIDocument.ShowElements；
/// UIDocument.ActiveView 赋值 + RefreshActiveView。
/// </summary>
internal sealed class ViewInteractionService : IViewInteractionService
{
    public SelectionResult SelectElements(object uiDocument, IReadOnlyList<int> elementIds, bool append, bool zoomToFit)
    {
        var uidoc = RequireUiDocument(uiDocument);
        if (elementIds.Count == 0)
        {
            throw new InvalidOperationException("必须提供 elementIds 或 cacheId。");
        }

        var selection = new List<ElementId>();
        if (append)
        {
            selection.AddRange(uidoc.Selection.GetElementIds());
        }

        foreach (var id in elementIds)
        {
            var elementId = new ElementId(id);
            if (!selection.Contains(elementId))
            {
                selection.Add(elementId);
            }
        }

        uidoc.Selection.SetElementIds(selection);

        var zoomed = false;
        if (zoomToFit)
        {
            uidoc.ShowElements(selection);
            zoomed = true;
        }

        return new SelectionResult(
            selection.Count,
            0,
            zoomed,
            $"已选择 {selection.Count} 个元素" + (zoomed ? "，并缩放到该选择集" : string.Empty));
    }

    public SelectionResult ClearSelection(object uiDocument)
    {
        var uidoc = RequireUiDocument(uiDocument);
        var previous = uidoc.Selection.GetElementIds();
        uidoc.Selection.SetElementIds(new List<ElementId>());
        return new SelectionResult(0, previous.Count, false, $"已清空选择（原有 {previous.Count} 个）");
    }

    public SelectionResult ZoomToElements(object uiDocument, IReadOnlyList<int> elementIds)
    {
        var uidoc = RequireUiDocument(uiDocument);
        if (elementIds.Count == 0)
        {
            throw new InvalidOperationException("必须提供 elementIds 或 cacheId。");
        }

        var ids = elementIds.Select(id => new ElementId(id)).ToList();
        uidoc.ShowElements(ids);
        return new SelectionResult(ids.Count, 0, true, $"已缩放到 {ids.Count} 个元素");
    }

    public SelectionResult ActivateView(object uiDocument, int viewId)
    {
        var uidoc = RequireUiDocument(uiDocument);
        var document = uidoc.Document;
        var view = document.GetElement(new ElementId(viewId)) as View
                   ?? throw new InvalidOperationException($"未找到视图 {viewId}。");

        if (view.IsTemplate)
        {
            throw new InvalidOperationException($"视图 {view.Name} 是视图样板，不能激活。");
        }

        uidoc.ActiveView = view;
        uidoc.RefreshActiveView();
        return new SelectionResult(0, 0, false, $"已激活视图 {view.Name}（{view.ViewType}）");
    }

    private static UIDocument RequireUiDocument(object uiDocument)
        => uiDocument as UIDocument
           ?? throw new InvalidOperationException(
               "当前没有可用的 UIDocument（该工具需要在 Revit 界面会话里执行，且必须有打开的项目）。");
}
