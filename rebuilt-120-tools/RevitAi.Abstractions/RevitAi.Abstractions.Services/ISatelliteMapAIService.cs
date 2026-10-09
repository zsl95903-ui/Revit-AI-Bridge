using RevitAi.Abstractions.Models;

namespace RevitAi.Abstractions.Services;

public interface ISatelliteMapAIService
{
	SatelliteMapImportResult ImportSatelliteMap(SatelliteMapImportRequest request);

	SatelliteMapImportResult ImportSatelliteMapWithTransaction(SatelliteMapImportRequest request, object transaction);
}
