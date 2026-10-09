using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IMaterialService
{
	IEnumerable<object> GetAllMaterials(object document);

	IEnumerable<object> GetMaterials(object document);

	object? GetMaterialByName(object document, string materialName);

	IEnumerable<object> GetElementMaterials(object element);

	bool SetElementMaterial(object element, int materialId, object document);

	object? CreateMaterial(object document, string materialName);

	object? DuplicateMaterial(object document, int sourceMaterialId, string newMaterialName);

	(string? Name, float Red, float Green, float Blue, float Alpha)? GetMaterialProperties(object material);

	string? GetMaterialClass(object material);

	(byte R, byte G, byte B)? GetMaterialColor(object material);

	bool SetMaterialColor(object material, byte red, byte green, byte blue);

	bool SetMaterialTransparency(object material, int transparency);

	int? GetMaterialTransparency(object material);

	string? GetAppearanceName(object material);

	bool SetElementMaterial(object element, int materialId, object document, MaterialSetStrategy strategy = MaterialSetStrategy.Auto, string? parameterName = null, int layerIndex = 0);

	bool SetElementMaterialByName(object element, string materialName, object document, MaterialSetStrategy strategy = MaterialSetStrategy.Auto, string? parameterName = null, int layerIndex = 0);

	(int successCount, int failedCount) SetElementMaterialsBatch(IEnumerable<object> elements, int materialId, object document, MaterialSetStrategy strategy = MaterialSetStrategy.Auto, string? parameterName = null, int layerIndex = 0);

	(int successCount, int failedCount) SetMaterialsByCategoryMap(object document, IDictionary<string, string> categoryMaterialMap, bool elementTypeFilter = true);

	bool DeleteMaterial(object document, string materialName, bool checkUsage = true);

	bool RenameMaterial(object document, string oldMaterialName, string newMaterialName);

	bool IsMaterialInUse(object document, int materialId);

	IEnumerable<object> GetElementsUsingMaterial(object document, int materialId);
}
