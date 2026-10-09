using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class RoadProject
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string Name { get; set; } = string.Empty;

	public string? Description { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.Now;

	public DateTime UpdatedAt { get; set; } = DateTime.Now;

	public (double X, double Y, double Z) GlobalOriginOffset { get; set; } = (X: 0.0, Y: 0.0, Z: 0.0);

	public RoadCenterlineDataSource DataSource { get; set; }

	public List<RoadCenterlinePoint3D>? StationElevationData { get; set; }

	public HorizontalCurveTable? HorizontalCurveTable { get; set; }

	public VerticalCurveTable? VerticalCurveTable { get; set; }

	public double CalculationInterval { get; set; } = 0.5;

	public double AnnotationInterval { get; set; } = 20.0;

	public List<RoadCenterlinePoint3D>? Centerline3DPoints { get; set; }

	public BridgeComponentConfigurations? BridgeComponentConfigurations { get; set; }

	public PilePositionConfigurations? PilePositionConfigurations { get; set; }

	public RoadStructureConfiguration? RoadStructureConfiguration { get; set; }

	public AncillaryStructuresConfiguration? AncillaryStructuresConfiguration { get; set; }

	public StationParametersConfiguration? StationParametersConfiguration { get; set; }

	public bool IsGenerated
	{
		get
		{
			if (Centerline3DPoints != null)
			{
				return Centerline3DPoints.Count > 0;
			}
			return false;
		}
	}

	public string DataSourceDescription => GetDataSourceDescription();

	public string GetDataSourceDescription()
	{
		return DataSource switch
		{
			RoadCenterlineDataSource.None => "无数据源", 
			RoadCenterlineDataSource.StationElevation => $"桩号高程表 ({StationElevationData?.Count ?? 0} 个点)", 
			RoadCenterlineDataSource.HorizontalVerticalCurve => $"平纵曲线表 (交点: {HorizontalCurveTable?.IntersectionPoints.Count ?? 0}, 变坡点: {VerticalCurveTable?.Points.Count ?? 0})", 
			_ => "未知数据源", 
		};
	}

	public double? GetLength()
	{
		if (Centerline3DPoints == null || Centerline3DPoints.Count == 0)
		{
			return null;
		}
		RoadCenterlinePoint3D roadCenterlinePoint3D = Centerline3DPoints[0];
		return Centerline3DPoints[Centerline3DPoints.Count - 1].StationKm - roadCenterlinePoint3D.StationKm;
	}

	public void ClearCache()
	{
		Centerline3DPoints = null;
	}

	public bool Validate()
	{
		return DataSource switch
		{
			RoadCenterlineDataSource.StationElevation => StationElevationData != null && StationElevationData.Count > 0, 
			RoadCenterlineDataSource.HorizontalVerticalCurve => HorizontalCurveTable != null && HorizontalCurveTable.Validate() && VerticalCurveTable != null && VerticalCurveTable.Points.Count > 0, 
			_ => false, 
		};
	}

	public double? GetElevationAtStation(double stationKm)
	{
		if (Centerline3DPoints == null || Centerline3DPoints.Count == 0)
		{
			return null;
		}
		RoadCenterlinePoint3D roadCenterlinePoint3D = Centerline3DPoints.FirstOrDefault((RoadCenterlinePoint3D p) => Math.Abs(p.StationKm - stationKm) < 0.0001);
		if (roadCenterlinePoint3D != null)
		{
			return roadCenterlinePoint3D.Z;
		}
		RoadCenterlinePoint3D roadCenterlinePoint3D2 = Centerline3DPoints.LastOrDefault((RoadCenterlinePoint3D p) => p.StationKm < stationKm);
		RoadCenterlinePoint3D roadCenterlinePoint3D3 = Centerline3DPoints.FirstOrDefault((RoadCenterlinePoint3D p) => p.StationKm > stationKm);
		if (roadCenterlinePoint3D2 == null || roadCenterlinePoint3D3 == null)
		{
			return null;
		}
		double num = (stationKm - roadCenterlinePoint3D2.StationKm) / (roadCenterlinePoint3D3.StationKm - roadCenterlinePoint3D2.StationKm);
		return roadCenterlinePoint3D2.Z + num * (roadCenterlinePoint3D3.Z - roadCenterlinePoint3D2.Z);
	}

	public RoadCenterlinePoint3D? GetPointAtStation(double stationKm)
	{
		if (Centerline3DPoints == null || Centerline3DPoints.Count == 0)
		{
			return null;
		}
		RoadCenterlinePoint3D roadCenterlinePoint3D = Centerline3DPoints.FirstOrDefault((RoadCenterlinePoint3D p) => Math.Abs(p.StationKm - stationKm) < 0.0001);
		if (roadCenterlinePoint3D != null)
		{
			return roadCenterlinePoint3D;
		}
		RoadCenterlinePoint3D roadCenterlinePoint3D2 = Centerline3DPoints.LastOrDefault((RoadCenterlinePoint3D p) => p.StationKm < stationKm);
		RoadCenterlinePoint3D roadCenterlinePoint3D3 = Centerline3DPoints.FirstOrDefault((RoadCenterlinePoint3D p) => p.StationKm > stationKm);
		if (roadCenterlinePoint3D2 == null || roadCenterlinePoint3D3 == null)
		{
			return null;
		}
		double num = (stationKm - roadCenterlinePoint3D2.StationKm) / (roadCenterlinePoint3D3.StationKm - roadCenterlinePoint3D2.StationKm);
		double x = roadCenterlinePoint3D2.X + num * (roadCenterlinePoint3D3.X - roadCenterlinePoint3D2.X);
		double y = roadCenterlinePoint3D2.Y + num * (roadCenterlinePoint3D3.Y - roadCenterlinePoint3D2.Y);
		double x2 = roadCenterlinePoint3D3.X - roadCenterlinePoint3D2.X;
		double num2;
		for (num2 = Math.Atan2(roadCenterlinePoint3D3.Y - roadCenterlinePoint3D2.Y, x2); num2 < 0.0; num2 += Math.PI * 2.0)
		{
		}
		while (num2 >= Math.PI * 2.0)
		{
			num2 -= Math.PI * 2.0;
		}
		return new RoadCenterlinePoint3D
		{
			StationKm = stationKm,
			X = x,
			Y = y,
			Z = roadCenterlinePoint3D2.Z + num * (roadCenterlinePoint3D3.Z - roadCenterlinePoint3D2.Z),
			Azimuth = num2,
			Curvature = roadCenterlinePoint3D2.Curvature + num * (roadCenterlinePoint3D3.Curvature - roadCenterlinePoint3D2.Curvature),
			PointType = RoadCenterlinePointType.Interpolated
		};
	}

	public (double X, double Y)? GetHorizontalCoordinateAtStation(double stationKm)
	{
		if (Centerline3DPoints == null || Centerline3DPoints.Count == 0)
		{
			return null;
		}
		RoadCenterlinePoint3D roadCenterlinePoint3D = Centerline3DPoints.FirstOrDefault((RoadCenterlinePoint3D p) => Math.Abs(p.StationKm - stationKm) < 0.0001);
		if (roadCenterlinePoint3D != null)
		{
			return (roadCenterlinePoint3D.X, roadCenterlinePoint3D.Y);
		}
		RoadCenterlinePoint3D roadCenterlinePoint3D2 = Centerline3DPoints.LastOrDefault((RoadCenterlinePoint3D p) => p.StationKm < stationKm);
		RoadCenterlinePoint3D roadCenterlinePoint3D3 = Centerline3DPoints.FirstOrDefault((RoadCenterlinePoint3D p) => p.StationKm > stationKm);
		if (roadCenterlinePoint3D2 == null || roadCenterlinePoint3D3 == null)
		{
			return null;
		}
		double num = (stationKm - roadCenterlinePoint3D2.StationKm) / (roadCenterlinePoint3D3.StationKm - roadCenterlinePoint3D2.StationKm);
		double item = roadCenterlinePoint3D2.X + num * (roadCenterlinePoint3D3.X - roadCenterlinePoint3D2.X);
		double item2 = roadCenterlinePoint3D2.Y + num * (roadCenterlinePoint3D3.Y - roadCenterlinePoint3D2.Y);
		return (item, item2);
	}

	public double? GetVerticalElevationAtStation(double stationKm)
	{
		return GetElevationAtStation(stationKm);
	}

	public override string ToString()
	{
		return Name + " (" + GetDataSourceDescription() + ")";
	}
}
