using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 批次 1 只读服务：把 Revit API 对象投影成不依赖 Revit 的 DTO。
/// 只在适配器内部使用；工具实现完全面向 DTO，因此可以在无 Revit 的测试里跑。
/// </summary>
internal sealed class GridService : IGridService
{
    public IReadOnlyList<GridInfo> GetAllGrids(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<GridInfo>();
        }

        return new FilteredElementCollector(doc)
            .OfClass(typeof(Grid))
            .Cast<Grid>()
            .Select(grid => new GridInfo((int)grid.Id.Value, grid.Name))
            .ToArray();
    }
}

internal sealed class RevitViewService : IViewService
{
    public IReadOnlyList<ViewInfo> GetAllViews(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<ViewInfo>();
        }

        return new FilteredElementCollector(doc)
            .OfClass(typeof(View))
            .Cast<View>()
            .Select(ToInfo)
            .ToArray();
    }

    public ViewInfo? GetActiveView(object document)
        => document is Document doc && doc.ActiveView is { } view ? ToInfo(view) : null;

    private static ViewInfo ToInfo(View view)
        => new((int)view.Id.Value, view.Name, view.ViewType.ToString(), view.IsTemplate);
}

internal sealed class CategoryService : ICategoryService
{
    public IReadOnlyList<CategoryInfo> GetAllCategories(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<CategoryInfo>();
        }

        var result = new List<CategoryInfo>();
        foreach (Category category in doc.Settings.Categories)
        {
            try
            {
                result.Add(new CategoryInfo(
                    category.Name,
                    (int)category.Id.Value,
                    category.CategoryType.ToString()));
            }
            catch (Exception)
            {
                // 个别类别（如无 Id 的内置类别）取不到 Id 时跳过，不影响整体返回。
            }
        }

        return result;
    }
}

internal sealed class WorksetService : IWorksetService
{
    public IReadOnlyList<WorksetInfo> GetAllWorksets(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<WorksetInfo>();
        }

        try
        {
            return new FilteredWorksetCollector(doc)
                .OfKind(WorksetKind.UserWorkset)
                .Select(workset => new WorksetInfo(
                    workset.Id.IntegerValue,
                    workset.Name,
                    workset.IsOpen,
                    workset.IsEditable,
                    workset.Kind.ToString()))
                .ToArray();
        }
        catch (Exception)
        {
            // 非工作共享模型没有工作集表。
            return Array.Empty<WorksetInfo>();
        }
    }
}
