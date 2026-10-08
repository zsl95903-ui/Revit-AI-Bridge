using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 材料服务：查询 / 创建 / 更新（颜色 &amp; 透明度）/ 删除。
/// 颜色用 #RRGGBB 文本交换，透明度 0–100（0 = 不透明）。
/// API 校正记录（用反射核对 Revit 27.3 真实签名）：
///   * Material.Create(Document, string) 返回 ElementId，需要再 GetElement 取 Material；
///   * Material 有 Transparency/MaterialClass/Color/Shininess/Smoothness，没有 IsPaintMaterial()。
/// </summary>
internal sealed class MaterialService : IMaterialService
{
    public IReadOnlyList<MaterialInfo> Query(object document, string? nameFilter, int limit)
    {
        if (document is not Document doc)
        {
            return Array.Empty<MaterialInfo>();
        }

        var query = new FilteredElementCollector(doc).OfClass(typeof(Material)).Cast<Material>();
        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(material =>
                material.Name.Contains(nameFilter!, StringComparison.OrdinalIgnoreCase));
        }

        return query.Take(limit <= 0 ? 500 : limit).Select(Describe).ToArray();
    }

    public MaterialInfo? Get(object document, int? materialId, string? name)
        => Find(document, materialId, name) is { } material ? Describe(material) : null;

    public MaterialInfo Create(object document, string name, string? colorHex, int? transparency, bool force)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("必须提供 materialName。");
        }

        var existing = Find(doc, null, name);
        if (existing is not null)
        {
            if (!force)
            {
                throw new InvalidOperationException($"材料 {name} 已存在（materialId={existing.Id.Value}）；force=true 可复用并更新。");
            }

            ApplyColorAndTransparency(existing, colorHex, transparency);
            return Describe(existing);
        }

        var createdId = Material.Create(doc, name);
        var material = doc.GetElement(createdId) as Material
                       ?? throw new InvalidOperationException($"材料 {name} 创建后无法获取。");

        ApplyColorAndTransparency(material, colorHex, transparency);
        return Describe(material);
    }

    public MaterialInfo? Update(object document, int? materialId, string? name, string? colorHex, int? transparency)
    {
        if (document is not Document doc)
        {
            return null;
        }

        var material = Find(doc, materialId, name);
        if (material is null)
        {
            return null;
        }

        ApplyColorAndTransparency(material, colorHex, transparency);
        return Describe(material);
    }

    public bool Delete(object document, int? materialId, string? name)
    {
        if (document is not Document doc)
        {
            return false;
        }

        var material = Find(doc, materialId, name);
        if (material is null)
        {
            return false;
        }

        doc.Delete(material.Id);
        return true;
    }

    internal static Material? Find(object? document, int? materialId, string? name)
    {
        if (document is not Document doc)
        {
            return null;
        }

        if (materialId is not null && doc.GetElement(new ElementId(materialId.Value)) is Material byId)
        {
            return byId;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return new FilteredElementCollector(doc)
            .OfClass(typeof(Material))
            .Cast<Material>()
            .FirstOrDefault(material => string.Equals(material.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    private static void ApplyColorAndTransparency(Material material, string? colorHex, int? transparency)
    {
        if (TryParseHex(colorHex, out var color))
        {
            material.Color = color;
        }

        if (transparency is not null)
        {
            material.Transparency = Math.Clamp(transparency.Value, 0, 100);
        }
    }

    internal static bool TryParseHex(string? hex, out Color color)
    {
        color = new Color(0, 0, 0);
        if (string.IsNullOrWhiteSpace(hex))
        {
            return false;
        }

        var text = hex.Trim().TrimStart('#');
        if (text.Length != 6 || !int.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out var value))
        {
            return false;
        }

        color = new Color((byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF));
        return true;
    }

    internal static MaterialInfo Describe(Material material)
    {
        var hex = string.Empty;
        try
        {
            var color = material.Color;
            if (color is not null && color.IsValid)
            {
                hex = $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
            }
        }
        catch (Exception)
        {
            // 无颜色信息。
        }

        var transparency = 0;
        try
        {
            transparency = material.Transparency;
        }
        catch (Exception)
        {
            // 忽略。
        }

        return new MaterialInfo(
            (int)material.Id.Value,
            material.Name,
            material.MaterialClass ?? string.Empty,
            hex,
            transparency,
            false);
    }
}

/// <summary>
/// 复合层结构服务：读取墙/楼板等类型的分层，并支持插入/删除/修改层。
/// 说明：CompoundStructure 只存在于 HostObjAttributes（WallType/FloorType/CeilingType…）；
/// 每次修改后都要 SetCompoundStructure 回写。厚度对外用毫米，内部英尺。
/// API 校正记录：27.3 没有 InsertLayer/SetMaterialId，改用 SetLayers（整体替换）与 SetLayer（单层替换）。
/// </summary>
internal sealed class CompoundStructureService : ICompoundStructureService
{
    private const double MillimetersPerFoot = 304.8;

    public IReadOnlyList<CompoundLayerInfo> GetLayers(object document, int? typeId, string? typeName)
    {
        var (_, structure) = RequireStructure(document, typeId, typeName);
        return structure.GetLayers().Select((layer, index) => Describe(document, layer, index)).ToArray();
    }

    public int AddLayer(
        object document,
        int? typeId,
        string? typeName,
        double thicknessMm,
        string function,
        int? materialId,
        string? materialName,
        int? insertIndex)
    {
        var (type, structure) = RequireStructure(document, typeId, typeName);
        if (thicknessMm <= 0)
        {
            throw new InvalidOperationException("thicknessMM 必须大于 0。");
        }

        var material = ResolveMaterial(document, materialId, materialName);
        var layers = structure.GetLayers().ToList();
        var index = Math.Clamp(insertIndex ?? layers.Count, 0, layers.Count);

        layers.Insert(index, new CompoundStructureLayer(
            thicknessMm / MillimetersPerFoot,
            ParseFunction(function),
            material?.Id ?? ElementId.InvalidElementId));

        structure.SetLayers(layers);
        type.SetCompoundStructure(structure);
        return structure.GetLayers().Count;
    }

    public bool DeleteLayer(object document, int? typeId, string? typeName, int layerIndex)
    {
        var (type, structure) = RequireStructure(document, typeId, typeName);
        var layers = structure.GetLayers().ToList();
        if (layerIndex < 0 || layerIndex >= layers.Count)
        {
            throw new InvalidOperationException($"layerIndex {layerIndex} 越界（共 {layers.Count} 层）。");
        }

        if (layers.Count <= 1)
        {
            throw new InvalidOperationException("复合层至少要保留一层，无法删除最后一层。");
        }

        layers.RemoveAt(layerIndex);
        structure.SetLayers(layers);
        type.SetCompoundStructure(structure);
        return true;
    }

    public bool ModifyLayer(
        object document,
        int? typeId,
        string? typeName,
        int layerIndex,
        double? thicknessMm,
        string? function,
        int? materialId,
        string? materialName)
    {
        var (type, structure) = RequireStructure(document, typeId, typeName);
        var layers = structure.GetLayers().ToList();
        if (layerIndex < 0 || layerIndex >= layers.Count)
        {
            throw new InvalidOperationException($"layerIndex {layerIndex} 越界（共 {layers.Count} 层）。");
        }

        if (thicknessMm is not null && thicknessMm <= 0)
        {
            throw new InvalidOperationException("thicknessMM 必须大于 0。");
        }

        var material = ResolveMaterial(document, materialId, materialName);
        var layer = layers[layerIndex];

        if (thicknessMm is not null)
        {
            layer.Width = thicknessMm.Value / MillimetersPerFoot;
        }

        if (!string.IsNullOrWhiteSpace(function))
        {
            layer.Function = ParseFunction(function!);
        }

        if (material is not null)
        {
            layer.MaterialId = material.Id;
        }

        structure.SetLayer(layerIndex, layer);
        type.SetCompoundStructure(structure);
        return true;
    }

    private static (HostObjAttributes Type, CompoundStructure Structure) RequireStructure(object document, int? typeId, string? typeName)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        HostObjAttributes? type = null;
        if (typeId is not null)
        {
            type = doc.GetElement(new ElementId(typeId.Value)) as HostObjAttributes;
        }

        if (type is null && !string.IsNullOrWhiteSpace(typeName))
        {
            type = new FilteredElementCollector(doc)
                .OfClass(typeof(HostObjAttributes))
                .Cast<HostObjAttributes>()
                .FirstOrDefault(item => string.Equals(item.Name, typeName, StringComparison.OrdinalIgnoreCase));
        }

        if (type is null)
        {
            throw new InvalidOperationException(
                typeId is not null ? $"元素 {typeId} 不是带复合层的类型（墙/楼板/天花板等）。" : $"未找到类型 {typeName}。");
        }

        var structure = type.GetCompoundStructure();
        if (structure is null)
        {
            throw new InvalidOperationException($"类型 {type.Name} 没有复合层结构。");
        }

        return (type, structure);
    }

    internal static Material? ResolveMaterial(object document, int? materialId, string? materialName)
    {
        if (materialId is null && string.IsNullOrWhiteSpace(materialName))
        {
            return null;
        }

        var material = MaterialService.Find(document, materialId, materialName);
        return material ?? throw new InvalidOperationException(
            materialId is not null ? $"未找到材料 {materialId}。" : $"未找到材料 {materialName}。");
    }

    private static CompoundLayerInfo Describe(object? document, CompoundStructureLayer layer, int index)
    {
        var materialName = string.Empty;
        if (layer.MaterialId != ElementId.InvalidElementId && document is Document doc)
        {
            materialName = doc.GetElement(layer.MaterialId)?.Name ?? string.Empty;
        }

        return new CompoundLayerInfo(
            index,
            layer.Function.ToString(),
            Math.Round(layer.Width * MillimetersPerFoot, 2),
            (int)layer.MaterialId.Value,
            materialName,
            layer.Function == MaterialFunctionAssignment.Structure,
            layer.Function is MaterialFunctionAssignment.Structure or MaterialFunctionAssignment.Substrate);
    }

    private static MaterialFunctionAssignment ParseFunction(string function)
    {
        var text = (function ?? string.Empty).Trim().ToLowerInvariant();
        return text switch
        {
            "structure" or "结构" => MaterialFunctionAssignment.Structure,
            "substrate" or "基层" => MaterialFunctionAssignment.Substrate,
            "insulation" or "保温" => MaterialFunctionAssignment.Insulation,
            "finish" or "面层" or "饰面" => MaterialFunctionAssignment.Finish1,
            "finish1" => MaterialFunctionAssignment.Finish1,
            "finish2" or "面层2" => MaterialFunctionAssignment.Finish2,
            "membrane" or "防水" or "膜" => MaterialFunctionAssignment.Membrane,
            "structuraldeck" or "结构板" => MaterialFunctionAssignment.StructuralDeck,
            _ => MaterialFunctionAssignment.Finish1,
        };
    }
}
