using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface ILevelService
{
	IEnumerable<object> GetAllLevels(object document);

	string? GetLevelName(object level);

	double? GetLevelElevation(object level);

	int? GetLevelId(object level);

	object? GetLevelByName(object document, string name);
}
