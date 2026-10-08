using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 批次 1 第三批只读服务：阶段 / 工作集归属 / 链接 / 屋顶 / 过滤器可用参数。
/// </summary>
internal sealed class PhaseService : IPhaseService
{
    public ElementPhaseInfo? GetElementPhase(object document, int elementId)
    {
        if (document is not Document doc || doc.GetElement(new ElementId(elementId)) is not { } element)
        {
            return null;
        }

        try
        {
            var createdId = element.CreatedPhaseId ?? ElementId.InvalidElementId;
            var demolishedId = element.DemolishedPhaseId ?? ElementId.InvalidElementId;

            return new ElementPhaseInfo(
                elementId,
                ToInt(createdId),
                NameOf(doc, createdId),
                ToInt(demolishedId),
                NameOf(doc, demolishedId));
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static int ToInt(ElementId id) => id == ElementId.InvalidElementId ? 0 : (int)id.Value;

    private static string NameOf(Document document, ElementId id)
        => id == ElementId.InvalidElementId ? string.Empty : document.GetElement(id)?.Name ?? string.Empty;
}

internal sealed class ElementWorksetService : IElementWorksetService
{
    public ElementWorksetInfo? GetElementWorkset(object document, int elementId)
    {
        if (document is not Document doc || doc.GetElement(new ElementId(elementId)) is not { } element)
        {
            return null;
        }

        try
        {
            var worksetId = element.WorksetId;
            if (worksetId is null || worksetId == WorksetId.InvalidWorksetId)
            {
                return new ElementWorksetInfo(elementId, 0, string.Empty);
            }

            var workset = doc.GetWorksetTable().GetWorkset(worksetId);
            return new ElementWorksetInfo(elementId, worksetId.IntegerValue, workset?.Name ?? string.Empty);
        }
        catch (Exception)
        {
            // 非工作共享模型没有工作集表。
            return new ElementWorksetInfo(elementId, 0, string.Empty);
        }
    }
}

internal sealed class LinkService : ILinkService
{
    public IReadOnlyList<LinkInfo> GetLinks(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<LinkInfo>();
        }

        var result = new List<LinkInfo>();
        foreach (var instance in new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>())
        {
            try
            {
                var linkDocument = instance.GetLinkDocument();
                result.Add(new LinkInfo(
                    (int)instance.Id.Value,
                    instance.Name,
                    linkDocument?.PathName ?? string.Empty,
                    linkDocument is not null,
                    doc.GetElement(instance.GetTypeId())?.Name ?? string.Empty));
            }
            catch (Exception)
            {
                // 链接文件缺失/卸载时仍尽量返回条目。
                result.Add(new LinkInfo((int)instance.Id.Value, instance.Name, string.Empty, false, string.Empty));
            }
        }

        return result;
    }

    public IReadOnlyList<ElementSummaryInfo> GetLinkElements(object document, int linkInstanceId, int limit)
    {
        if (document is not Document doc || doc.GetElement(new ElementId(linkInstanceId)) is not RevitLinkInstance instance)
        {
            return Array.Empty<ElementSummaryInfo>();
        }

        var linkDocument = instance.GetLinkDocument();
        if (linkDocument is null)
        {
            return Array.Empty<ElementSummaryInfo>();
        }

        return new FilteredElementCollector(linkDocument)
            .WhereElementIsNotElementType()
            .Where(element => element.Category is not null)
            .Take(limit <= 0 ? 200 : limit)
            .Select(element => new ElementSummaryInfo(
                (int)element.Id.Value,
                element.Category?.Name is { Length: > 0 } category ? $"{category}" : string.Empty,
                element.Category?.Name ?? string.Empty,
                linkDocument.GetElement(element.GetTypeId())?.Name ?? string.Empty))
            .ToArray();
    }
}

internal sealed class RoofService : IRoofService
{
    private const double SquareFeetToSquareMeters = 0.09290304;

    public RoofInfo? GetRoofInfo(object document, int elementId)
    {
        if (document is not Document doc || doc.GetElement(new ElementId(elementId)) is not RoofBase roof)
        {
            return null;
        }

        try
        {
            var area = roof.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED)?.AsDouble() ?? 0d;
            var levelId = roof.get_Parameter(BuiltInParameter.ROOF_BASE_LEVEL_PARAM)?.AsElementId() ?? ElementId.InvalidElementId;

            return new RoofInfo(
                elementId,
                doc.GetElement(roof.GetTypeId())?.Name ?? string.Empty,
                levelId == ElementId.InvalidElementId ? string.Empty : doc.GetElement(levelId)?.Name ?? string.Empty,
                Math.Round(area * SquareFeetToSquareMeters, 3),
                roof is FootPrintRoof,
                roof is ExtrusionRoof);
        }
        catch (Exception)
        {
            return null;
        }
    }
}

internal sealed class FilterParameterService : IFilterParameterService
{
    public IReadOnlyList<FilterParameterInfo> GetAvailableFilterParameters(object document, string? categoryName)
    {
        if (document is not Document doc)
        {
            return Array.Empty<FilterParameterInfo>();
        }

        try
        {
            var categories = ResolveCategories(doc, categoryName);
            var parameterIds = categories.Count == 0
                ? ParameterFilterUtilities.GetFilterableParametersInCommon(doc, ParameterFilterUtilities.GetAllFilterableCategories())
                : ParameterFilterUtilities.GetFilterableParametersInCommon(doc, categories);

            return parameterIds
                .Select(id => new FilterParameterInfo((int)id.Value, doc.GetElement(id)?.Name ?? string.Empty))
                .Where(info => !string.IsNullOrWhiteSpace(info.Name))
                .ToArray();
        }
        catch (Exception)
        {
            return Array.Empty<FilterParameterInfo>();
        }
    }

    private static ICollection<ElementId> ResolveCategories(Document document, string? categoryName)
    {
        var result = new List<ElementId>();
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return result;
        }

        foreach (Category category in document.Settings.Categories)
        {
            if (string.Equals(category.Name, categoryName, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(category.Id);
            }
        }

        return result;
    }
}
