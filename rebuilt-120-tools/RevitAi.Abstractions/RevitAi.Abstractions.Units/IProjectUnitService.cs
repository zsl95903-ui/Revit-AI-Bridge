using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitAi.Abstractions.Units;

public interface IProjectUnitService
{
	Dictionary<UnitType, ProjectUnitInfo> GetProjectUnits(object document);

	ProjectUnitInfo? GetProjectUnit(object document, UnitType unitType);

	Task<bool> SetProjectUnitAsync(object document, UnitType unitType, object displayUnitType);

	Task<bool> SetProjectUnitsAsync(object document, Dictionary<UnitType, object> unitSettings);

	Task<bool> SetMetricUnitsAsync(object document);

	Task<bool> SetImperialUnitsAsync(object document);

	void CacheProjectUnits(object document);

	void ClearCache(object document);

	void ClearAllCache();
}
