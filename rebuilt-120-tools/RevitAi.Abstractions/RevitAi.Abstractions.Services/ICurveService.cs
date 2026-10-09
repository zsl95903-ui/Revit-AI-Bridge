using System.Collections.Generic;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.Abstractions.Services;

public interface ICurveService
{
	Result<object> CreateLine(object document, double x1, double y1, double z1, double x2, double y2, double z2);

	Result<object> CreateArc(object document, double centerX, double centerY, double centerZ, double radius, double startAngle, double endAngle);

	Result<object> CreateSplineCurve(object document, List<(double X, double Y, double Z)> points);

	Result<List<object>> CreateRoadCenterlineCurves(object document, List<RoadCenterlinePoint3D> points3D, bool convertToFeet = true);

	Result<RoadCenterlineMassInfo> CreateRoadCenterlineInMass(object document, List<RoadCenterlinePoint3D> points3D, string? massTemplateName = null, string? roadProjectName = null);

	Result<int> PlaceStationAnnotations(object document, object curve, List<RoadCenterlinePoint3D> points3D, string familyPath, double stationInterval = 100.0, string? symbolName = null, RoadProject? roadProject = null);
}
