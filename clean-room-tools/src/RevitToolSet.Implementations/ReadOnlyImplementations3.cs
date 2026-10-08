using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 1 第三批（收尾）：阶段、工作集归属、链接与链接元素、屋顶信息、过滤器可用参数。
/// 完成后批次 1 共 31 个只读工具。
/// </summary>
public static class ReadOnlyImplementations3
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("get_element_phases", new GetElementPhasesImplementation());
        ToolImplementationRegistry.Register("get_element_workset", new GetElementWorksetImplementation());
        ToolImplementationRegistry.Register("get_links", new GetLinksImplementation());
        ToolImplementationRegistry.Register("get_link_elements", new GetLinkElementsImplementation());
        ToolImplementationRegistry.Register("get_roof_info", new GetRoofInfoImplementation());
        ToolImplementationRegistry.Register("get_available_filter_parameters", new GetAvailableFilterParametersImplementation());
    }
}

internal static class ReadOnlySupport3
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetElementPhasesTool.cs（材料管理）
internal sealed class GetElementPhasesImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.PhaseService ?? throw new InvalidOperationException("无法获取 PhaseService");
        var document = ReadOnlySupport3.RequireDocument(adapter, context);
        var elementId = context.GetParameter("elementId", 0);
        if (elementId <= 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId。"));
        }

        var phase = service.GetElementPhase(document, elementId);
        if (phase is null)
        {
            return Task.FromResult(AIToolResult.Fail($"未找到元素 {elementId} 或无法读取其阶段信息。"));
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 元素 {elementId} 创建阶段：{(string.IsNullOrEmpty(phase.CreatedPhaseName) ? "（无）" : phase.CreatedPhaseName)}",
            new
            {
                elementId,
                createdPhaseId = phase.CreatedPhaseId,
                createdPhaseName = phase.CreatedPhaseName,
                demolishedPhaseId = phase.DemolishedPhaseId,
                demolishedPhaseName = phase.DemolishedPhaseName,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetElementWorksetTool.cs（工作集管理）
internal sealed class GetElementWorksetImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementWorksetService ?? throw new InvalidOperationException("无法获取 ElementWorksetService");
        var document = ReadOnlySupport3.RequireDocument(adapter, context);
        var elementId = context.GetParameter("elementId", 0);
        if (elementId <= 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId。"));
        }

        var workset = service.GetElementWorkset(document, elementId);
        if (workset is null)
        {
            return Task.FromResult(AIToolResult.Fail($"未找到元素 {elementId}。"));
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 元素 {elementId} 属于工作集：{(string.IsNullOrEmpty(workset.WorksetName) ? "（无/非工作共享模型）" : workset.WorksetName)}",
            new { elementId, worksetId = workset.WorksetId, worksetName = workset.WorksetName }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetLinksTool.cs（链接管理）
internal sealed class GetLinksImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.LinkService ?? throw new InvalidOperationException("无法获取 LinkService");
        var document = ReadOnlySupport3.RequireDocument(adapter, context);
        var links = service.GetLinks(document);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {links.Count} 个链接",
            new
            {
                links = links.Select(link => new
                {
                    linkInstanceId = link.LinkInstanceId,
                    name = link.LinkName,
                    documentPath = link.DocumentPath,
                    isLoaded = link.IsLoaded,
                    linkTypeName = link.LinkTypeName,
                }).ToArray(),
                count = links.Count,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetLinkElementsTool.cs
internal sealed class GetLinkElementsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.LinkService ?? throw new InvalidOperationException("无法获取 LinkService");
        var document = ReadOnlySupport3.RequireDocument(adapter, context);

        var linkInstanceId = context.GetParameter("linkInstanceId", 0);
        if (linkInstanceId <= 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 linkInstanceId（可先用 get_links 获取）。"));
        }

        var limit = context.GetParameter("limit", 200);
        var elements = service.GetLinkElements(document, linkInstanceId, limit);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 链接 {linkInstanceId} 中读取 {elements.Count} 个元素",
            new
            {
                linkInstanceId,
                elements = elements.Select(element => new
                {
                    id = element.Id,
                    name = element.Name,
                    category = element.Category,
                    typeName = element.TypeName,
                }).ToArray(),
                count = elements.Count,
                limit,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetRoofInfoTool.cs（视图/建模查询）
internal sealed class GetRoofInfoImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.RoofService ?? throw new InvalidOperationException("无法获取 RoofService");
        var document = ReadOnlySupport3.RequireDocument(adapter, context);
        var elementId = context.GetParameter("elementId", 0);
        if (elementId <= 0)
        {
            elementId = context.GetParameter("roofId", 0);
        }
        if (elementId <= 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId（屋顶元素 ID）。"));
        }

        var roof = service.GetRoofInfo(document, elementId);
        if (roof is null)
        {
            return Task.FromResult(AIToolResult.Fail($"元素 {elementId} 不是屋顶或未找到。"));
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 屋顶 {elementId}：{roof.RoofTypeName}，面积 {roof.AreaSquareMeters} m²",
            new
            {
                roofId = roof.RoofId,
                roofTypeName = roof.RoofTypeName,
                levelName = roof.LevelName,
                area = roof.AreaSquareMeters,
                unit = "square_meters",
                isFootprintRoof = roof.IsFootprintRoof,
                isExtrusionRoof = roof.IsExtrusionRoof,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAvailableFilterParametersTool.cs（视图高级操作）
internal sealed class GetAvailableFilterParametersImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.FilterParameterService ?? throw new InvalidOperationException("无法获取 FilterParameterService");
        var document = ReadOnlySupport3.RequireDocument(adapter, context);

        var categoryName = context.GetParameter<string?>("categoryName", null);
        var parameters = service.GetAvailableFilterParameters(document, categoryName);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 可用过滤器参数 {parameters.Count} 个" + (string.IsNullOrWhiteSpace(categoryName) ? string.Empty : $"（类别：{categoryName}）"),
            new
            {
                categoryName,
                parameters = parameters.Select(parameter => new
                {
                    parameterId = parameter.ParameterId,
                    name = parameter.Name,
                }).ToArray(),
                count = parameters.Count,
            }));
    }
}
