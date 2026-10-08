using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitAi.Engine.Revit.Tools;

/// <summary>工具公共取件：适配器 / 文档 / 单位换算。</summary>
internal static class ToolContextAccess
{
    public static IRevitAdapter Adapter(AIToolContext context)
        => context.RevitAdapter as IRevitAdapter
           ?? throw new InvalidOperationException("RevitAdapter 不可用。");

    public static object Document(AIToolContext context, IRevitAdapter adapter)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空。");

    public static IUnitConverter Units(AIToolContext context)
        => context.UnitService ?? new UnitService();
}
