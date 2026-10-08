namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 3 第二刀：材料查询/管理、复合层结构（墙类型/楼板类型等）读写。
// ---------------------------------------------------------------------------------------------

public sealed record MaterialInfo(
    int MaterialId,
    string Name,
    string Class,
    string ColorHex,
    int Transparency,
    bool IsPaint);

public sealed record CompoundLayerInfo(
    int Index,
    string Function,
    double WidthMm,
    int MaterialId,
    string MaterialName,
    bool IsCore,
    bool IsStructuralMaterial);

public interface IMaterialService
{
    IReadOnlyList<MaterialInfo> Query(object document, string? nameFilter, int limit);

    MaterialInfo? Get(object document, int? materialId, string? name);

    MaterialInfo Create(object document, string name, string? colorHex, int? transparency, bool force);

    MaterialInfo? Update(object document, int? materialId, string? name, string? colorHex, int? transparency);

    bool Delete(object document, int? materialId, string? name);
}

public interface ICompoundStructureService
{
    IReadOnlyList<CompoundLayerInfo> GetLayers(object document, int? typeId, string? typeName);

    int AddLayer(
        object document,
        int? typeId,
        string? typeName,
        double thicknessMm,
        string function,
        int? materialId,
        string? materialName,
        int? insertIndex);

    bool DeleteLayer(object document, int? typeId, string? typeName, int layerIndex);

    bool ModifyLayer(
        object document,
        int? typeId,
        string? typeName,
        int layerIndex,
        double? thicknessMm,
        string? function,
        int? materialId,
        string? materialName);
}
