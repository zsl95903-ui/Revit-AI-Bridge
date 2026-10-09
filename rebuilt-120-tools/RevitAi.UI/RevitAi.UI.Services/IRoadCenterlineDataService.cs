using System.Collections.Generic;
using System.Windows;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.Services;

public interface IRoadCenterlineDataService
{
	RoadCenterlineDataSource DataSource { get; set; }

	List<RoadCenterlinePoint3D>? StationElevationData { get; }

	Point? StationElevationOriginOffset { get; }

	(double X, double Y, double Z) GlobalOriginOffset { get; }

	HorizontalCurveTable? HorizontalCurveTable { get; }

	VerticalCurveTable? VerticalCurveTable { get; }

	double AnnotationInterval { get; }

	void SetGlobalOriginOffset(double x, double y, double z);

	void SetStationElevationData(List<RoadCenterlinePoint3D> points, Point originOffset);

	void SetCurveData(HorizontalCurveTable horizontalTable, VerticalCurveTable verticalTable);

	void SetAnnotationInterval(double intervalMeters);

	void Clear();

	RoadCenterlineDataSource GetDataSource();

	bool HasData();

	List<RoadCenterlinePoint3D>? GetStationElevationData();

	HorizontalCurveTable? GetHorizontalCurveTable();

	VerticalCurveTable? GetVerticalCurveTable();
}
