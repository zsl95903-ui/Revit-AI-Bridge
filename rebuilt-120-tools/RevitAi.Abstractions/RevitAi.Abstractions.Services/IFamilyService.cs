using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IFamilyService
{
	IEnumerable<object> GetAllFamilies(object document);

	IEnumerable<object> GetFamilyTypes(object document, string familyName);

	IEnumerable<object> GetFamilyTypes(object document, int familyId);

	object? LoadFamily(object document, string familyFilePath);

	object? LoadFamily(object document, string familyFilePath, string? familyName);

	object? CreateFamilyInstance(object document, int familySymbolId, double positionX, double positionY, double positionZ, int? levelId = null);

	object? GetElementFamily(object element);

	object? GetElementFamilySymbol(object element);

	bool ChangeFamilySymbol(object element, int newSymbolId, object document);

	object? GetFamilyOfTypeId(object document, int typeId);

	IEnumerable<int>? GetFamilySymbolIds(object family);

	object? DuplicateFamilyType(object document, int sourceTypeId, string newTypeName);

	object? DuplicateFamilyType(object document, int sourceTypeId, string newTypeName, IDictionary<string, object>? parameterValues);

	IEnumerable<object> GetAllSystemElementTypes(object document);

	IEnumerable<object> GetTypesByCategory(object document, string categoryName);

	bool FamilyExists(object document, string familyName);
}
