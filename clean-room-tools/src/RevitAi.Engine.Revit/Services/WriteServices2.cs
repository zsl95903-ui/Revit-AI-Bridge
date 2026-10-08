using System.Globalization;
using System.Text;
using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 批次 2 剩余服务：过滤器创建、标记创建、明细表创建/导出、视图导出。
/// 导出目录默认落在 %LOCALAPPDATA%\RevitAiEngine\exports，避免污染项目目录。
/// API 校正记录：ImageExportOptions 没有 ViewId，改用 SetViewsAndSheets；
/// FilteredElementCollector 按类别过滤用 OfCategoryId(ElementId)；
/// ParameterFilterRuleFactory 的数值比较规则用 int 重载（double 重载在 27.3 不存在）。
/// </summary>
internal static class ExportPaths
{
    public static string DefaultDirectory()
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RevitAiEngine",
            "exports");
        Directory.CreateDirectory(directory);
        return directory;
    }

    public static string Sanitize(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(name.Length);
        foreach (var ch in name)
        {
            builder.Append(invalid.Contains(ch) ? '_' : ch);
        }

        return builder.Length == 0 ? "export" : builder.ToString();
    }
}

internal sealed class FilterCreationService : IFilterCreationService
{
    public int CreateFilter(object document, string filterName, string categoryName, string parameterName, string? value, string? rule)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        if (string.IsNullOrWhiteSpace(filterName) || string.IsNullOrWhiteSpace(categoryName) || string.IsNullOrWhiteSpace(parameterName))
        {
            throw new InvalidOperationException("必须提供 filterName、categoryName 与 parameterName。");
        }

        var categoryIds = new List<ElementId>();
        foreach (Category category in doc.Settings.Categories)
        {
            if (string.Equals(category.Name, categoryName, StringComparison.OrdinalIgnoreCase))
            {
                categoryIds.Add(category.Id);
            }
        }

        if (categoryIds.Count == 0)
        {
            throw new InvalidOperationException($"未找到类别 {categoryName}。");
        }

        var parameterId = ParameterFilterUtilities
            .GetFilterableParametersInCommon(doc, categoryIds)
            .FirstOrDefault(id => string.Equals(doc.GetElement(id)?.Name, parameterName, StringComparison.OrdinalIgnoreCase));

        if (parameterId is null)
        {
            throw new InvalidOperationException($"类别 {categoryName} 上没有可过滤的参数 {parameterName}。");
        }

        var filterRule = BuildRule(parameterId, value, rule);
        var elementFilter = new ElementParameterFilter(filterRule);
        var filter = ParameterFilterElement.Create(doc, filterName, categoryIds, elementFilter);
        return (int)filter.Id.Value;
    }

    private static FilterRule BuildRule(ElementId parameterId, string? value, string? rule)
    {
        var text = value ?? string.Empty;
        var normalized = (rule ?? "equals").Trim().ToLowerInvariant();

        switch (normalized)
        {
            case "contains":
                return ParameterFilterRuleFactory.CreateContainsRule(parameterId, text);
            case "greater":
                return ParameterFilterRuleFactory.CreateGreaterRule(parameterId, ToInteger(text));
            case "less":
                return ParameterFilterRuleFactory.CreateLessRule(parameterId, ToInteger(text));
            default:
                return TryEquals(parameterId, text);
        }
    }

    private static int ToInteger(string text)
    {
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
        {
            return integer;
        }

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
        {
            return (int)number;
        }

        throw new InvalidOperationException($"大于/小于规则需要数值参数值，收到 '{text}'。");
    }

    private static FilterRule TryEquals(ElementId parameterId, string text)
    {
        try
        {
            return ParameterFilterRuleFactory.CreateEqualsRule(parameterId, text);
        }
        catch (Exception)
        {
            // 非文本参数：退化为整数比较。
        }

        return ParameterFilterRuleFactory.CreateEqualsRule(parameterId, ToInteger(text));
    }
}

internal sealed class TagCreationService : ITagCreationService
{
    public int CreateTag(object document, int elementId, int? tagTypeId, int? viewId, double xMm, double yMm, string? orientation, bool addLeader)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var element = doc.GetElement(new ElementId(elementId))
            ?? throw new InvalidOperationException($"未找到元素 {elementId}。");

        var view = viewId is null
            ? doc.ActiveView
            : doc.GetElement(new ElementId(viewId.Value)) as View
            ?? throw new InvalidOperationException($"未找到视图 {viewId}。");

        var typeId = tagTypeId is not null
            ? new ElementId(tagTypeId.Value)
            : element.Category is null
                ? ElementId.InvalidElementId
                : doc.GetDefaultFamilyTypeId(element.Category.Id);

        if (typeId is null || typeId == ElementId.InvalidElementId)
        {
            throw new InvalidOperationException($"元素 {elementId} 的类别没有默认标记类型，请显式提供 tagTypeId。");
        }

        var tagOrientation = string.Equals(orientation, "vertical", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(orientation, "垂直", StringComparison.Ordinal)
            ? TagOrientation.Vertical
            : TagOrientation.Horizontal;

        var point = new XYZ(xMm / 304.8, yMm / 304.8, 0);
        var tag = IndependentTag.Create(doc, typeId, view.Id, new Reference(element), addLeader, tagOrientation, point);
        return (int)tag.Id.Value;
    }
}

internal sealed class ScheduleService : IScheduleService
{
    public int CreateSchedule(object document, string categoryName, string? name, IReadOnlyList<string> fields)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var category = doc.Settings.Categories
            .Cast<Category>()
            .FirstOrDefault(item => string.Equals(item.Name, categoryName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"未找到类别 {categoryName}。");

        var schedule = ViewSchedule.CreateSchedule(doc, category.Id);
        if (!string.IsNullOrWhiteSpace(name))
        {
            try
            {
                schedule.Name = name!;
            }
            catch (Exception)
            {
                // 名称冲突时保留默认名。
            }
        }

        foreach (var field in fields)
        {
            var parameterId = ResolveParameterId(doc, category, field);
            if (parameterId is null)
            {
                continue;
            }

            try
            {
                schedule.Definition.AddField(ScheduleFieldType.Instance, parameterId);
            }
            catch (Exception)
            {
                // 该字段不适用于此类别时跳过。
            }
        }

        return (int)schedule.Id.Value;
    }

    public ExportResult ExportScheduleToCsv(object document, int? scheduleId, string? scheduleName, string? outputPath)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var schedule = (scheduleId is not null
                ? doc.GetElement(new ElementId(scheduleId.Value)) as ViewSchedule
                : null)
            ?? new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .FirstOrDefault(item => string.Equals(item.Name, scheduleName, StringComparison.OrdinalIgnoreCase));

        if (schedule is null)
        {
            throw new InvalidOperationException(
                scheduleId is not null ? $"未找到明细表 {scheduleId}。" : $"未找到明细表 {scheduleName}。");
        }

        var tableData = schedule.GetTableData();
        var body = tableData.GetSectionData(SectionType.Body);
        var header = tableData.GetSectionData(SectionType.Header);
        var builder = new StringBuilder();

        AppendSection(builder, header, schedule, true);
        AppendSection(builder, body, schedule, false);

        var path = string.IsNullOrWhiteSpace(outputPath)
            ? Path.Combine(ExportPaths.DefaultDirectory(), ExportPaths.Sanitize(schedule.Name) + ".csv")
            : outputPath!;

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, builder.ToString(), new UTF8Encoding(true));
        return new ExportResult(path, body.NumberOfRows, "rows");
    }

    private static void AppendSection(StringBuilder builder, TableSectionData section, ViewSchedule schedule, bool isHeader)
    {
        if (section.NumberOfRows == 0)
        {
            return;
        }

        for (var row = 0; row < section.NumberOfRows; row++)
        {
            var cells = new List<string>();
            for (var column = 0; column < section.NumberOfColumns; column++)
            {
                string text;
                try
                {
                    text = schedule.GetCellText(isHeader ? SectionType.Header : SectionType.Body, row, column) ?? string.Empty;
                }
                catch (Exception)
                {
                    text = string.Empty;
                }

                cells.Add("\"" + text.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"");
            }

            builder.AppendLine(string.Join(',', cells));
        }
    }

    private static ElementId? ResolveParameterId(Document document, Category category, string parameterName)
    {
        var sample = new FilteredElementCollector(document)
            .OfCategoryId(category.Id)
            .WhereElementIsNotElementType()
            .FirstElement();

        if (sample is not null)
        {
            foreach (Parameter parameter in sample.GetOrderedParameters())
            {
                if (string.Equals(parameter.Definition?.Name, parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    return parameter.Id;
                }
            }
        }

        return null;
    }
}

internal sealed class ExportService : IExportService
{
    public ExportResult ExportViewImages(object document, IReadOnlyList<int> viewIds, string? outputDirectory, int pixelSize)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var directory = string.IsNullOrWhiteSpace(outputDirectory) ? ExportPaths.DefaultDirectory() : outputDirectory!;
        Directory.CreateDirectory(directory);

        var views = ResolveViews(doc, viewIds);
        if (views.Count == 0)
        {
            throw new InvalidOperationException("没有可导出的视图（请提供 viewIds 或打开一个视图）。");
        }

        var size = pixelSize <= 0 ? 2048 : pixelSize;
        var exported = 0;

        foreach (var view in views)
        {
            var path = Path.Combine(directory, ExportPaths.Sanitize(view.Name));
            try
            {
                var options = new ImageExportOptions
                {
                    FilePath = path,
                    PixelSize = size,
                    ZoomType = ZoomFitType.FitToPage,
                    HLRandWFViewsFileType = ImageFileType.PNG,
                    ImageResolution = ImageResolution.DPI_150,
                };
                options.SetViewsAndSheets(new List<ElementId> { view.Id });

                doc.ExportImage(options);
                exported++;
            }
            catch (Exception)
            {
                // 单个视图导出失败不影响其它视图。
            }
        }

        return new ExportResult(directory, exported, "images");
    }

    public ExportResult ExportViewsAsDwg(object document, IReadOnlyList<int> viewIds, string? outputDirectory, string? exportSettingName)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var directory = string.IsNullOrWhiteSpace(outputDirectory) ? ExportPaths.DefaultDirectory() : outputDirectory!;
        Directory.CreateDirectory(directory);

        var views = ResolveViews(doc, viewIds);
        if (views.Count == 0)
        {
            throw new InvalidOperationException("没有可导出的视图（请提供 viewIds 或打开一个视图）。");
        }

        var settingName = string.IsNullOrWhiteSpace(exportSettingName) ? "ACAD2018" : exportSettingName!;
        var options = DWGExportOptions.GetPredefinedOptions(doc, settingName) ?? new DWGExportOptions();
        doc.Export(directory, ExportPaths.Sanitize("RevitToolSet"), views.Select(view => view.Id).ToArray(), options);

        return new ExportResult(directory, views.Count, "views");
    }

    private static List<View> ResolveViews(Document document, IReadOnlyList<int> viewIds)
    {
        if (viewIds.Count > 0)
        {
            return viewIds
                .Select(id => document.GetElement(new ElementId(id)) as View)
                .Where(view => view is not null)
                .Select(view => view!)
                .ToList();
        }

        return document.ActiveView is { } active ? new List<View> { active } : new List<View>();
    }
}
