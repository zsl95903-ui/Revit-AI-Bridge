using System.Threading.Tasks;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.Services;

public interface IRoadModelingUIService
{
	Task<RoadModelingUIResult> CreateRoadModelAsync(RoadProject project, bool splitAtIntegerStations = true, int integerStationInterval = 20);

	Task<RoadModelingUIResult> CreateAncillaryStructureAsync(RoadProject project, bool splitAtIntegerStations = true, int integerStationInterval = 20);
}
