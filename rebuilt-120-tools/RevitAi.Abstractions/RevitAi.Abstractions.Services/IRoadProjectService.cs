using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.Abstractions.Services;

public interface IRoadProjectService
{
	Result<List<RoadCenterlinePoint3D>> Calculate3DCenterline(RoadProject project);

	Result<RoadCenterlinePoint3D> GetPointAtStation(RoadProject project, double stationKm);

	Result<List<RoadCenterlinePoint3D>> GetPointsInRange(RoadProject project, double startKm, double endKm);

	Task<Dictionary<Guid, Result<List<RoadCenterlinePoint3D>>>> CalculateMultipleProjectsAsync(IEnumerable<RoadProject> projects);
}
