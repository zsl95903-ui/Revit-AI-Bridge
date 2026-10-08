using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 3 第二刀实现：材料查询/管理、复合层结构读写。
/// </summary>
public static class MaterialImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("material_query", new MaterialQueryImplementation());
        ToolImplementationRegistry.Register("material_manager", new MaterialManagerImplementation());
        ToolImplementationRegistry.Register("get_compound_structure_layers", new GetCompoundStructureLayersImplementation());
        ToolImplementationRegistry.Register("add_compound_structure_layer", new AddCompoundStructureLayerImplementation());
        ToolImplementationRegistry.Register("delete_compound_structure_layer", new DeleteCompoundStructureLayerImplementation());
        ToolImplementationRegistry.Register("modify_compound_structure_layer", new ModifyCompoundStructureLayerImplementation());
    }
}

internal static class MaterialArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    public static string? HexFromRgb(AIToolContext context)
    {
        if (!context.Parameters.ContainsKey("red") && !context.Parameters.ContainsKey("green") && !context.Parameters.ContainsKey("blue"))
        {
            return WriteArgs.Text(context, "hexColor", "hex_color", "color");
        }

        var red = Math.Clamp((int)WriteArgs.Number(context, 0, "red"), 0, 255);
        var green = Math.Clamp((int)WriteArgs.Number(context, 0, "green"), 0, 255);
        var blue = Math.Clamp((int)WriteArgs.Number(context, 0, "blue"), 0, 255);
        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    public static int? OptionalInt(AIToolContext context, params string[] keys) => WriteArgs.OptionalInt(context, keys);
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\MaterialQueryTool.cs（材料管理）
internal sealed class MaterialQueryImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.MaterialService ?? throw new InvalidOperationException("无法获取 MaterialService");
        var document = MaterialArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "list").Trim().ToLowerInvariant();
        var filter = WriteArgs.Text(context, "filter", "materialName", "filterText");
        var limit = (int)WriteArgs.Number(context, 200, "limit");

        switch (operation)
        {
            case "get":
            {
                var material = service.Get(document, MaterialArgs.OptionalInt(context, "materialId", "material_id"), filter);
                return Task.FromResult(material is null
                    ? AIToolResult.Fail("未找到指定材料。")
                    : AIToolResult.Ok($"✅ 材料 {material.Name}", new
                    {
                        materialId = material.MaterialId,
                        name = material.Name,
                        @class = material.Class,
                        color = material.ColorHex,
                        transparency = material.Transparency,
                        isPaint = material.IsPaint,
                    }));
            }

            default:
            {
                var materials = service.Query(document, filter, limit);
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 查询到 {materials.Count} 个材料" + (string.IsNullOrWhiteSpace(filter) ? string.Empty : $"（过滤：{filter}）"),
                    new
                    {
                        materials = materials.Select(material => new
                        {
                            materialId = material.MaterialId,
                            name = material.Name,
                            @class = material.Class,
                            color = material.ColorHex,
                            transparency = material.Transparency,
                        }).ToArray(),
                        count = materials.Count,
                    }));
            }
        }
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\MaterialManagerTool.cs（材料管理）
internal sealed class MaterialManagerImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.MaterialService ?? throw new InvalidOperationException("无法获取 MaterialService");
        var document = MaterialArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "create").Trim().ToLowerInvariant();
        var name = WriteArgs.Text(context, "materialName", "material_name", "name");
        var materialId = MaterialArgs.OptionalInt(context, "materialId", "material_id");
        var colorHex = MaterialArgs.HexFromRgb(context);
        var transparency = MaterialArgs.OptionalInt(context, "transparency");
        var force = WriteArgs.Flag(context, false, "force");

        switch (operation)
        {
            case "create":
            case "ensure":
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return Task.FromResult(AIToolResult.Fail("create 需要 materialName。"));
                }

                var material = service.Create(document, name!, colorHex, transparency, force);
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 材料 {material.Name} 已就绪（id={material.MaterialId}，颜色 {material.ColorHex}，透明度 {material.Transparency}）",
                    new { materialId = material.MaterialId, name = material.Name, color = material.ColorHex, transparency = material.Transparency }));
            }

            case "update":
            case "set":
            {
                var material = service.Update(document, materialId, name, colorHex, transparency);
                return Task.FromResult(material is null
                    ? AIToolResult.Fail("未找到要更新的材料（请提供 materialId 或 materialName）。")
                    : AIToolResult.Ok(
                        $"✅ 材料 {material.Name} 已更新（{material.ColorHex}，透明度 {material.Transparency}）",
                        new { materialId = material.MaterialId, name = material.Name, color = material.ColorHex, transparency = material.Transparency }));
            }

            case "delete":
            {
                var deleted = service.Delete(document, materialId, name);
                return Task.FromResult(deleted
                    ? AIToolResult.Ok($"✅ 已删除材料 {name ?? materialId?.ToString()}", new { deleted = true })
                    : AIToolResult.Fail("未找到要删除的材料。"));
            }

            default:
                return Task.FromResult(AIToolResult.Fail("operation 只支持 create / update / delete。"));
        }
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetCompoundStructureLayersTool.cs（复合层结构）
internal sealed class GetCompoundStructureLayersImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.CompoundStructureService ?? throw new InvalidOperationException("无法获取 CompoundStructureService");
        var document = MaterialArgs.RequireDocument(adapter, context);

        var typeId = MaterialArgs.OptionalInt(context, "elementTypeId", "element_type_id");
        var typeName = WriteArgs.Text(context, "elementTypeName", "element_type_name", "typeName");
        if (typeId is null && string.IsNullOrWhiteSpace(typeName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementTypeId 或 elementTypeName。"));
        }

        var layers = service.GetLayers(document, typeId, typeName);
        var totalMm = Math.Round(layers.Sum(layer => layer.WidthMm), 2);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 共 {layers.Count} 层，总厚度 {totalMm} mm",
            new
            {
                layers = layers.Select(layer => new
                {
                    index = layer.Index,
                    function = layer.Function,
                    thicknessMM = layer.WidthMm,
                    materialId = layer.MaterialId,
                    materialName = layer.MaterialName,
                    isStructural = layer.IsStructuralMaterial,
                }).ToArray(),
                count = layers.Count,
                totalThicknessMM = totalMm,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\AddCompoundStructureLayerTool.cs
internal sealed class AddCompoundStructureLayerImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.CompoundStructureService ?? throw new InvalidOperationException("无法获取 CompoundStructureService");
        var document = MaterialArgs.RequireDocument(adapter, context);

        var typeId = MaterialArgs.OptionalInt(context, "elementTypeId", "element_type_id");
        var typeName = WriteArgs.Text(context, "elementTypeName", "element_type_name", "typeName");
        if (typeId is null && string.IsNullOrWhiteSpace(typeName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementTypeId 或 elementTypeName。"));
        }

        var thickness = WriteArgs.Number(context, 0, "thicknessMM", "thickness_mm", "thickness");
        var function = WriteArgs.Text(context, "function") ?? "Finish1";
        var materialId = MaterialArgs.OptionalInt(context, "materialId", "material_id");
        var materialName = WriteArgs.Text(context, "materialName", "material_name");
        var insertIndex = MaterialArgs.OptionalInt(context, "insertIndex", "insert_index");

        var layerCount = service.AddLayer(document, typeId, typeName, thickness, function, materialId, materialName, insertIndex);
        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已插入复合层（{function}，{thickness} mm），当前共 {layerCount} 层",
            new { layerCount, function, thicknessMM = thickness, insertIndex }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\DeleteCompoundStructureLayerTool.cs
internal sealed class DeleteCompoundStructureLayerImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.CompoundStructureService ?? throw new InvalidOperationException("无法获取 CompoundStructureService");
        var document = MaterialArgs.RequireDocument(adapter, context);

        var typeId = MaterialArgs.OptionalInt(context, "elementTypeId", "element_type_id");
        var typeName = WriteArgs.Text(context, "elementTypeName", "element_type_name", "typeName");
        var layerIndex = MaterialArgs.OptionalInt(context, "layerIndex", "layer_index");
        if (layerIndex is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 layerIndex。"));
        }

        var deleted = service.DeleteLayer(document, typeId, typeName, layerIndex.Value);
        return Task.FromResult(deleted
            ? AIToolResult.Ok($"✅ 已删除第 {layerIndex} 层", new { layerIndex, deleted = true })
            : AIToolResult.Fail($"未能删除第 {layerIndex} 层。"));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ModifyCompoundStructureLayerTool.cs
internal sealed class ModifyCompoundStructureLayerImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.CompoundStructureService ?? throw new InvalidOperationException("无法获取 CompoundStructureService");
        var document = MaterialArgs.RequireDocument(adapter, context);

        var typeId = MaterialArgs.OptionalInt(context, "elementTypeId", "element_type_id");
        var typeName = WriteArgs.Text(context, "elementTypeName", "element_type_name", "typeName");
        var layerIndex = MaterialArgs.OptionalInt(context, "layerIndex", "layer_index");
        if (layerIndex is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 layerIndex。"));
        }

        double? thickness = null;
        if (context.Parameters.ContainsKey("thicknessMM") || context.Parameters.ContainsKey("thickness"))
        {
            thickness = WriteArgs.Number(context, 0, "thicknessMM", "thickness_mm", "thickness");
        }

        var function = WriteArgs.Text(context, "function");
        var materialId = MaterialArgs.OptionalInt(context, "materialId", "material_id");
        var materialName = WriteArgs.Text(context, "materialName", "material_name");

        var modified = service.ModifyLayer(document, typeId, typeName, layerIndex.Value, thickness, function, materialId, materialName);
        return Task.FromResult(modified
            ? AIToolResult.Ok(
                $"✅ 已修改第 {layerIndex} 层" + (thickness is not null ? $"（厚度 {thickness} mm）" : string.Empty),
                new { layerIndex, thicknessMM = thickness, function, materialId, materialName })
            : AIToolResult.Fail($"未能修改第 {layerIndex} 层。"));
    }
}
