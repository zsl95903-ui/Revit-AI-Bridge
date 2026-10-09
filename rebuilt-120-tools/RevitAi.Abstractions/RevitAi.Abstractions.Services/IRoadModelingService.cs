using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.Abstractions.Services;

public interface IRoadModelingService
{
	Task<Result<RoadModelingResult>> CreateRoadModelAsync(object document, RoadProject project, bool splitAtIntegerStations = true, int integerStationInterval = 20);

	Task<Result<RoadModelingResult>> CreateAncillaryStructureAsync(object document, RoadProject project);
}
