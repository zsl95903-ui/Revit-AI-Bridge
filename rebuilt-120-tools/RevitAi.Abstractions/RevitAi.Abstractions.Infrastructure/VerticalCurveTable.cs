using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class VerticalCurveTable
{
	public List<VerticalCurvePVI> Points { get; set; } = new List<VerticalCurvePVI>();

	public string? Source { get; set; }

	public List<VerticalCurvePVI> GetSortedPoints()
	{
		return Points.OrderBy((VerticalCurvePVI p) => p.Station).ToList();
	}

	public double? CalculateElevationAtStation(double stationKm)
	{
		List<VerticalCurvePVI> sortedPoints = GetSortedPoints();
		if (sortedPoints.Count == 0)
		{
			return null;
		}
		for (int i = 0; i < sortedPoints.Count - 1; i++)
		{
			VerticalCurvePVI verticalCurvePVI = sortedPoints[i];
			VerticalCurvePVI verticalCurvePVI2 = sortedPoints[i + 1];
			if (!(stationKm >= verticalCurvePVI.Station) || !(stationKm <= verticalCurvePVI2.Station))
			{
				continue;
			}
			bool flag = verticalCurvePVI.CurveLength.HasValue && verticalCurvePVI.CurveLength.Value > 0.0;
			double valueOrDefault = verticalCurvePVI.CurveLength.GetValueOrDefault();
			double num = (flag ? (valueOrDefault / 2.0 / 1000.0) : 0.0);
			double num2 = verticalCurvePVI.Station - num;
			double num3 = verticalCurvePVI.Station + num;
			if ((stationKm >= num2 && stationKm <= num3) & flag)
			{
				double valueOrDefault2 = verticalCurvePVI.FrontGradient.GetValueOrDefault();
				verticalCurvePVI.BackGradient.GetValueOrDefault();
				double num4 = verticalCurvePVI.Radius / 1000.0;
				double num5 = verticalCurvePVI.Elevation + valueOrDefault2 * (stationKm - verticalCurvePVI.Station) / 100.0;
				double num6 = Math.Pow(stationKm - verticalCurvePVI.Station, 2.0) / (2.0 * num4) * 1000.0;
				if (verticalCurvePVI.CurveType == VerticalCurveType.Concave)
				{
					return num5 + num6;
				}
				return num5 - num6;
			}
			double valueOrDefault3 = verticalCurvePVI.BackGradient.GetValueOrDefault();
			return verticalCurvePVI.Elevation + valueOrDefault3 * (stationKm - verticalCurvePVI.Station) / 100.0;
		}
		return null;
	}
}
