using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 1：只读查询类工具的自研实现。
/// 这些实现只依赖引擎抽象（IRevitAdapter + 只读 DTO），不引用 RevitAPI，
/// 因此可以在无 Revit 的冒烟测试里用假适配器完整跑一遍。
/// 工具名与厂商 120 项严格一致；每个类都标注反编译对照位置。
/// </summary>
public static class ReadOnlyImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("get_all_grids", new GetAllGridsImplementation());
        ToolImplementationRegistry.Register("get_all_views", new GetAllViewsImplementation());
        ToolImplementationRegistry.Register("get_active_view", new GetActiveViewImplementation());
        ToolImplementationRegistry.Register("get_all_categories", new GetAllCategoriesImplementation());
        ToolImplementationRegistry.Register("get_all_worksets", new GetAllWorksetsImplementation());
        ToolImplementationRegistry.Register("get_cache_data", new GetCacheDataImplementation());
    }
}

internal static class ReadOnlySupport
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAllGridsTool.cs（轴网查询）
internal sealed class GetAllGridsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var gridService = adapter.GridService ?? throw new InvalidOperationException("无法获取 GridService");
        var document = ReadOnlySupport.RequireDocument(adapter, context);
        var grids = gridService.GetAllGrids(document);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {grids.Count} 个轴网",
            new
            {
                grids = grids.Select(grid => new { id = grid.Id, name = grid.Name }).ToArray(),
                count = grids.Count,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAllViewsTool.cs（视图查询）
internal sealed class GetAllViewsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var viewService = adapter.ViewService ?? throw new InvalidOperationException("无法获取 ViewService");
        var document = ReadOnlySupport.RequireDocument(adapter, context);
        var views = viewService.GetAllViews(document);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {views.Count} 个视图",
            new
            {
                views = views.Select(view => new { id = view.Id, name = view.Name, viewType = view.ViewType, isTemplate = view.IsTemplate }).ToArray(),
                count = views.Count,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetActiveViewTool.cs
internal sealed class GetActiveViewImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var viewService = adapter.ViewService ?? throw new InvalidOperationException("无法获取 ViewService");
        var document = ReadOnlySupport.RequireDocument(adapter, context);
        var view = viewService.GetActiveView(document);

        if (view is null)
        {
            return Task.FromResult(AIToolResult.Fail("当前没有活动视图。"));
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 活动视图：{view.Name}",
            new
            {
                id = view.Id,
                name = view.Name,
                viewType = view.ViewType,
                isTemplate = view.IsTemplate,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAllCategoriesTool.cs
internal sealed class GetAllCategoriesImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var categoryService = adapter.CategoryService ?? throw new InvalidOperationException("无法获取 CategoryService");
        var document = ReadOnlySupport.RequireDocument(adapter, context);
        var categories = categoryService.GetAllCategories(document);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {categories.Count} 个类别",
            new { categories = categories.Select(category => category.Name).ToArray(), count = categories.Count }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAllWorksetsTool.cs（工作集管理）
internal sealed class GetAllWorksetsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var worksetService = adapter.WorksetService ?? throw new InvalidOperationException("无法获取 WorksetService");
        var document = ReadOnlySupport.RequireDocument(adapter, context);
        var worksets = worksetService.GetAllWorksets(document);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {worksets.Count} 个工作集",
            new
            {
                worksets = worksets.Select(workset => new
                {
                    id = workset.Id,
                    name = workset.Name,
                    isOpen = workset.IsOpen,
                    isEditable = workset.IsEditable,
                    kind = workset.Kind,
                }).ToArray(),
                count = worksets.Count,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetCacheDataTool.cs（12 KB）
// 语义：按 cache_id 取回之前工具缓存的数据，可截断返回；不触碰 Revit，可离线验证。
internal sealed class GetCacheDataImplementation : IToolImplementation
{
    private const int DefaultLimit = 200;

    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var cacheId = context.GetParameter<string?>("cacheId", null)
                      ?? context.GetParameter<string?>("cache_id", null);

        if (string.IsNullOrWhiteSpace(cacheId))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 cacheId（缓存 ID），可从上一个查询工具的返回结果中获取。"));
        }

        var validation = context.ValidateCache(cacheId);
        if (!validation.IsValid)
        {
            return Task.FromResult(AIToolResult.Fail($"缓存 ID '{cacheId}' 无效：{validation.Reason ?? "不存在或已过期"}"));
        }

        var offset = Math.Max(0, context.GetParameter("offset", 0));
        var limit = context.GetParameter("limit", DefaultLimit);
        if (limit <= 0)
        {
            limit = DefaultLimit;
        }

        var items = context.GetCachedData<List<object?>>(cacheId) ?? new List<object?>();
        var page = items.Skip(offset).Take(limit).ToArray();

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 从缓存 {cacheId} 读取 {page.Length} 条（共 {items.Count} 条）",
            new
            {
                cache_id = cacheId,
                total_items = items.Count,
                returned_items = page.Length,
                offset,
                items = page,
                cache_info = new
                {
                    total_items = items.Count,
                    returned_items = page.Length,
                    remaining_items = Math.Max(0, items.Count - offset - page.Length),
                },
            }));
    }
}
