using System.Collections.Generic;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Revit.WallToRoad;

namespace RevitAi.Abstractions.Services;

public interface IWallToRoadService
{
	Result<List<int>> CreateRoadFromWalls(object document, WallToRoadRequest request);

	Result<List<int>> CreateRoadFromWallsWithTransaction(object document, WallToRoadRequest request, object externalTransaction);
}
