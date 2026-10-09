using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class HorizontalCurveTable
{
	public List<HorizontalCurveIP> IntersectionPoints { get; set; } = new List<HorizontalCurveIP>();

	public string? Source { get; set; }

	public (double X, double Y)? StartPoint { get; set; }

	public double? StartAzimuth { get; set; }

	public List<HorizontalCurveIP> GetSortedPoints()
	{
		return IntersectionPoints.OrderBy((HorizontalCurveIP ip) => ip.Station).ToList();
	}

	public bool Validate()
	{
		if (IntersectionPoints.Count == 0)
		{
			return false;
		}
		if (IntersectionPoints.Count < 2)
		{
			return false;
		}
		foreach (HorizontalCurveIP intersectionPoint in IntersectionPoints)
		{
			if (double.IsNaN(intersectionPoint.CoordinateX) || double.IsNaN(intersectionPoint.CoordinateY))
			{
				return false;
			}
			if (intersectionPoint.Radius < 0.0)
			{
				return false;
			}
		}
		return true;
	}
}
