using System.Collections.Generic;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.Abstractions.Services;

public interface IInfrastructureService
{
	Result<List<RoadCenterlinePoint3D>> GenerateUnifiedSamplePoints(HorizontalCurveTable horizontalTable, VerticalCurveTable verticalTable, List<double> stationsKm);

	Result<RoadCenterlinePoint3D> GeneratePointAtStation(HorizontalCurveTable horizontalTable, VerticalCurveTable verticalTable, double stationKm);
}
