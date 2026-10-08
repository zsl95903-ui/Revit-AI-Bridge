using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 批次 2 视图写入服务。宿主已在 Revit 主线程开启事务，这里直接改模型。
/// 每个方法都做输入校验并抛带中文说明的异常，由宿主回滚并回灌给模型。
/// 说明：Revit API 的 DisplayStyle 没有 HiddenLine，隐藏线对应 HLR；
/// Color 构造函数需要 byte 分量。
/// </summary>
internal sealed class ViewWriteService : IViewWriteService
{
    private const double MillimetersPerFoot = 304.8;

    public ViewSettingsInfo? SetDetailLevel(object document, int? viewId, string detailLevel)
    {
        var (_, view) = RequireView(document, viewId);
        var level = ParseEnum<ViewDetailLevel>(detailLevel,
            new Dictionary<string, ViewDetailLevel>(StringComparer.OrdinalIgnoreCase)
            {
                ["coarse"] = ViewDetailLevel.Coarse,
                ["粗略"] = ViewDetailLevel.Coarse,
                ["medium"] = ViewDetailLevel.Medium,
                ["中等"] = ViewDetailLevel.Medium,
                ["fine"] = ViewDetailLevel.Fine,
                ["精细"] = ViewDetailLevel.Fine,
            },
            "详细程度只支持 Coarse/Medium/Fine（粗略/中等/精细）。");

        view.DetailLevel = level;
        return Describe(view);
    }

    public ViewSettingsInfo? SetDisplayStyle(object document, int? viewId, string displayStyle)
    {
        var (_, view) = RequireView(document, viewId);
        var style = ParseEnum<DisplayStyle>(displayStyle,
            new Dictionary<string, DisplayStyle>(StringComparer.OrdinalIgnoreCase)
            {
                ["wireframe"] = DisplayStyle.Wireframe,
                ["线框"] = DisplayStyle.Wireframe,
                ["hiddenline"] = DisplayStyle.HLR,
                ["隐藏线"] = DisplayStyle.HLR,
                ["hlr"] = DisplayStyle.HLR,
                ["shaded"] = DisplayStyle.Shading,
                ["着色"] = DisplayStyle.Shading,
                ["shadedwithedges"] = DisplayStyle.ShadingWithEdges,
                ["着色并显示边"] = DisplayStyle.ShadingWithEdges,
                ["flatcolors"] = DisplayStyle.FlatColors,
                ["平面颜色"] = DisplayStyle.FlatColors,
                ["realistic"] = DisplayStyle.Realistic,
                ["真实"] = DisplayStyle.Realistic,
                ["rendered"] = DisplayStyle.Rendering,
                ["渲染"] = DisplayStyle.Rendering,
            },
            "显示样式只支持 Wireframe/HLR/Shading/ShadingWithEdges/FlatColors/Realistic/Rendering。");

        view.DisplayStyle = style;
        return Describe(view);
    }

    public void SetBackgroundColor(object document, int red, int green, int blue)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        doc.Application.BackgroundColor = new Color(Clamp(red), Clamp(green), Clamp(blue));
    }

    public TemporaryViewControlResult ApplyTemporaryViewControl(
        object document,
        int? viewId,
        string action,
        IReadOnlyList<int> elementIds)
    {
        var (_, view) = RequireView(document, viewId);
        var ids = elementIds.Select(id => new ElementId(id)).ToArray();
        var normalized = (action ?? string.Empty).Trim().ToUpperInvariant();

        switch (normalized)
        {
            case "ISOLATE_ELEMENTS":
                RequireIds(ids, "ISOLATE_ELEMENTS");
                view.IsolateElementsTemporary(ids);
                break;
            case "HIDE_ELEMENTS":
                RequireIds(ids, "HIDE_ELEMENTS");
                view.HideElementsTemporary(ids);
                break;
            case "RESET":
                view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                break;
            case "QUERY_STATUS":
                break;
            default:
                throw new InvalidOperationException(
                    "action 只支持 ISOLATE_ELEMENTS / HIDE_ELEMENTS / RESET / QUERY_STATUS。");
        }

        var inMode = view.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
        return new TemporaryViewControlResult((int)view.Id.Value, normalized, ids.Length, inMode);
    }

    public int DuplicateView(object document, int? viewId, string option, string? newName)
    {
        var (doc, view) = RequireView(document, viewId);
        var duplicateOption = ParseEnum<ViewDuplicateOption>(option,
            new Dictionary<string, ViewDuplicateOption>(StringComparer.OrdinalIgnoreCase)
            {
                ["duplicate"] = ViewDuplicateOption.Duplicate,
                ["withdetailing"] = ViewDuplicateOption.WithDetailing,
                ["asdependent"] = ViewDuplicateOption.AsDependent,
                ["复制"] = ViewDuplicateOption.Duplicate,
                ["带详图"] = ViewDuplicateOption.WithDetailing,
            },
            "duplicateDetail 只支持 Duplicate / WithDetailing / AsDependent。");

        var newViewId = view.Duplicate(duplicateOption);
        if (!string.IsNullOrWhiteSpace(newName) && doc.GetElement(newViewId) is View duplicated)
        {
            try
            {
                duplicated.Name = newName!;
            }
            catch (Exception)
            {
                // 名称冲突时保留默认名。
            }
        }

        return (int)newViewId.Value;
    }

    internal static (Document Document, View View) RequireView(object document, int? viewId)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var view = viewId is null
            ? doc.ActiveView
            : doc.GetElement(new ElementId(viewId.Value)) as View;

        return view is null
            ? throw new InvalidOperationException(viewId is null ? "当前没有活动视图。" : $"未找到视图 {viewId}。")
            : (doc, view);
    }

    internal static ViewSettingsInfo Describe(View view) => new(
        (int)view.Id.Value,
        view.Name,
        view.ViewType.ToString(),
        view.Scale,
        view.DetailLevel.ToString(),
        view.DisplayStyle.ToString(),
        view.CropBoxActive,
        view.CropBoxVisible);

    internal static double MmToFeet(double millimeters) => millimeters / MillimetersPerFoot;

    private static void RequireIds(ElementId[] ids, string action)
    {
        if (ids.Length == 0)
        {
            throw new InvalidOperationException($"{action} 需要提供 elementIds。");
        }
    }

    private static byte Clamp(int value) => (byte)Math.Clamp(value, 0, 255);

    private static T ParseEnum<T>(string? value, IDictionary<string, T> map, string errorMessage)
        where T : struct
    {
        if (!string.IsNullOrWhiteSpace(value) && map.TryGetValue(value.Trim(), out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException(errorMessage);
    }
}

internal sealed class ViewCreationService : IViewCreationService
{
    public int CreateFloorPlan(object document, string levelName, string? viewName)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var level = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .FirstOrDefault(item => string.Equals(item.Name, levelName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"未找到标高 {levelName}。");

        var viewFamilyTypeId = FindViewFamilyType(doc, ViewFamily.FloorPlan);
        var view = ViewPlan.Create(doc, viewFamilyTypeId, level.Id);
        if (!string.IsNullOrWhiteSpace(viewName))
        {
            TryRename(view, viewName!);
        }

        return (int)view.Id.Value;
    }

    public int CreateSection(
        object document,
        string? viewName,
        double minX,
        double minY,
        double maxX,
        double maxY,
        double minZ,
        double maxZ)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var box = new BoundingBoxXYZ
        {
            Min = new XYZ(ViewWriteService.MmToFeet(minX), ViewWriteService.MmToFeet(minY), ViewWriteService.MmToFeet(minZ)),
            Max = new XYZ(ViewWriteService.MmToFeet(maxX), ViewWriteService.MmToFeet(maxY), ViewWriteService.MmToFeet(maxZ)),
        };

        var viewFamilyTypeId = FindViewFamilyType(doc, ViewFamily.Section);
        var section = ViewSection.CreateSection(doc, viewFamilyTypeId, box);
        if (!string.IsNullOrWhiteSpace(viewName))
        {
            TryRename(section, viewName!);
        }

        return (int)section.Id.Value;
    }

    public int CreateSheet(object document, string? sheetNumber, string? sheetName)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var titleBlockTypeId = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_TitleBlocks)
            .WhereElementIsElementType()
            .FirstElementId();

        if (titleBlockTypeId is null || titleBlockTypeId == ElementId.InvalidElementId)
        {
            throw new InvalidOperationException("当前项目没有可用的标题栏族，无法创建图纸。");
        }

        var sheet = ViewSheet.Create(doc, titleBlockTypeId);
        if (!string.IsNullOrWhiteSpace(sheetNumber))
        {
            try
            {
                sheet.SheetNumber = sheetNumber!;
            }
            catch (Exception)
            {
                // 图号冲突时保留自动编号。
            }
        }

        if (!string.IsNullOrWhiteSpace(sheetName))
        {
            TryRename(sheet, sheetName!);
        }

        return (int)sheet.Id.Value;
    }

    public int CreateTextNote(object document, string text, double xMm, double yMm, int? viewId)
    {
        var (doc, view) = ViewWriteService.RequireView(document, viewId);
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("必须提供 text（文字内容）。");
        }

        var typeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
        var position = new XYZ(ViewWriteService.MmToFeet(xMm), ViewWriteService.MmToFeet(yMm), 0);

        var note = TextNote.Create(doc, view.Id, position, text, typeId);
        return (int)note.Id.Value;
    }

    private static ElementId FindViewFamilyType(Document document, ViewFamily family)
    {
        var type = new FilteredElementCollector(document)
            .OfClass(typeof(ViewFamilyType))
            .Cast<ViewFamilyType>()
            .FirstOrDefault(item => item.ViewFamily == family);

        return type?.Id ?? throw new InvalidOperationException($"当前项目没有可用的 {family} 视图类型。");
    }

    private static void TryRename(Element element, string name)
    {
        try
        {
            element.Name = name;
        }
        catch (Exception)
        {
            // 名称冲突/非法名称时保留默认名。
        }
    }
}

internal sealed class FilterService : IFilterService
{
    public int ApplyFilterToView(
        object document,
        int? viewId,
        string filterName,
        string action,
        int red,
        int green,
        int blue,
        bool visible)
    {
        var (doc, view) = ViewWriteService.RequireView(document, viewId);
        if (string.IsNullOrWhiteSpace(filterName))
        {
            throw new InvalidOperationException("必须提供 filterName（过滤器名称）。");
        }

        var filter = new FilteredElementCollector(doc)
            .OfClass(typeof(ParameterFilterElement))
            .Cast<ParameterFilterElement>()
            .FirstOrDefault(item => string.Equals(item.Name, filterName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"未找到名为 {filterName} 的过滤器（可先用 create_view_filter 创建）。");

        var normalized = string.IsNullOrWhiteSpace(action) ? "apply" : action.Trim().ToLowerInvariant();

        switch (normalized)
        {
            case "remove":
                if (view.GetFilters().Contains(filter.Id))
                {
                    view.RemoveFilter(filter.Id);
                }

                return (int)filter.Id.Value;

            case "set_visibility":
                if (!view.GetFilters().Contains(filter.Id))
                {
                    view.AddFilter(filter.Id);
                }

                view.SetFilterVisibility(filter.Id, visible);
                return (int)filter.Id.Value;

            case "set_color":
            case "apply":
                if (!view.GetFilters().Contains(filter.Id))
                {
                    view.AddFilter(filter.Id);
                }

                var overrides = new OverrideGraphicSettings();
                var color = new Color(Clamp(red), Clamp(green), Clamp(blue));
                overrides.SetProjectionLineColor(color);
                overrides.SetCutLineColor(color);
                view.SetFilterOverrides(filter.Id, overrides);
                view.SetFilterVisibility(filter.Id, visible);
                return (int)filter.Id.Value;

            default:
                throw new InvalidOperationException("action 只支持 apply / remove / set_visibility / set_color。");
        }
    }

    public bool DeleteFilter(object document, string filterName)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        if (string.IsNullOrWhiteSpace(filterName))
        {
            throw new InvalidOperationException("必须提供 filterName（过滤器名称）。");
        }

        var filter = new FilteredElementCollector(doc)
            .OfClass(typeof(ParameterFilterElement))
            .Cast<ParameterFilterElement>()
            .FirstOrDefault(item => string.Equals(item.Name, filterName, StringComparison.OrdinalIgnoreCase));

        if (filter is null)
        {
            return false;
        }

        doc.Delete(filter.Id);
        return true;
    }

    private static byte Clamp(int value) => (byte)Math.Clamp(value, 0, 255);
}
