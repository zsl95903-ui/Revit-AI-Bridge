using System.Collections.Generic;
using System.Windows;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.Services;

public class RoadCenterlineDataService : IRoadCenterlineDataService
{
	public RoadCenterlineDataSource DataSource { get; set; }

	public List<RoadCenterlinePoint3D>? StationElevationData { get; private set; }

	public Point? StationElevationOriginOffset { get; private set; }

	public (double X, double Y, double Z) GlobalOriginOffset { get; private set; } = (X: 0.0, Y: 0.0, Z: 0.0);

	public HorizontalCurveTable? HorizontalCurveTable { get; private set; }

	public VerticalCurveTable? VerticalCurveTable { get; private set; }

	public double AnnotationInterval { get; private set; } = 20.0;

	public void SetGlobalOriginOffset(double x, double y, double z)
	{
		GlobalOriginOffset = (X: x, Y: y, Z: z);
	}

	public void SetAnnotationInterval(double intervalMeters)
	{
		AnnotationInterval = intervalMeters;
	}

	public void SetStationElevationData(List<RoadCenterlinePoint3D> points, Point originOffset)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		StationElevationData = points;
		StationElevationOriginOffset = originOffset;
		DataSource = RoadCenterlineDataSource.StationElevation;
	}

	public void SetCurveData(HorizontalCurveTable horizontalTable, VerticalCurveTable verticalTable)
	{
		HorizontalCurveTable = horizontalTable;
		VerticalCurveTable = verticalTable;
		DataSource = RoadCenterlineDataSource.HorizontalVerticalCurve;
	}

	public void Clear()
	{
		StationElevationData = null;
		StationElevationOriginOffset = null;
		HorizontalCurveTable = null;
		VerticalCurveTable = null;
		DataSource = RoadCenterlineDataSource.None;
		GlobalOriginOffset = (X: 0.0, Y: 0.0, Z: 0.0);
		AnnotationInterval = 20.0;
	}

	public RoadCenterlineDataSource GetDataSource()
	{
		return DataSource;
	}

	public bool HasData()
	{
		return DataSource switch
		{
			RoadCenterlineDataSource.StationElevation => StationElevationData != null && StationElevationData.Count > 0, 
			RoadCenterlineDataSource.HorizontalVerticalCurve => HorizontalCurveTable != null && HorizontalCurveTable.IntersectionPoints != null && HorizontalCurveTable.IntersectionPoints.Count > 0, 
			_ => false, 
		};
	}

	public List<RoadCenterlinePoint3D>? GetStationElevationData()
	{
		return StationElevationData;
	}

	public HorizontalCurveTable? GetHorizontalCurveTable()
	{
		return HorizontalCurveTable;
	}

	public VerticalCurveTable? GetVerticalCurveTable()
	{
		return VerticalCurveTable;
	}
}
