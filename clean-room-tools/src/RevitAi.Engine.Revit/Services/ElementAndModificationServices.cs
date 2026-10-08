using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

internal sealed class ElementService : IElementService
{
    public bool Exists(object document, int elementId)
        => document is Document doc && doc.GetElement(new ElementId((long)elementId)) is not null;
}

/// <summary>
/// 修改服务实现。位移单位：英尺（工具层已把模型的毫米换算过来）。
/// 与自研外壳 RevitAiBatch\ToolDispatcher.cs 的 MoveElements 使用同一 API：ElementTransformUtils.MoveElements。
/// </summary>
internal sealed class ModificationService : IModificationService
{
    public int MoveElements(object document, IReadOnlyList<int> elementIds, double dxFeet, double dyFeet, double dzFeet)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        if (elementIds.Count == 0)
        {
            return 0;
        }

        var ids = elementIds.Select(id => new ElementId((long)id)).ToArray();
        ElementTransformUtils.MoveElements(doc, ids, new XYZ(dxFeet, dyFeet, dzFeet));
        return ids.Length;
    }
}
