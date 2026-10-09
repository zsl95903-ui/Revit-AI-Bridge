using System;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class RoadCenterlinePoint3D
{
	public double StationKm { get; set; }

	public double X { get; set; }

	public double Y { get; set; }

	public double Z { get; set; }

	public double Azimuth { get; set; }

	public double Curvature { get; set; }

	public RoadCenterlinePointType PointType { get; set; }

	public string GetFormattedStation()
	{
		int num = (int)StationKm;
		double num2 = (StationKm - (double)num) * 1000.0;
		string value = ((num2 >= 0.0) ? "+" : "-");
		double num3 = Math.Abs(num2);
		int num4 = (int)num3;
		double num5 = num3 - (double)num4;
		if (num5 < 0.001)
		{
			return $"K{num}{value}{num4:D3}";
		}
		return $"K{num}{value}{num4:D3}.{num5:F3}".TrimEnd('0').TrimEnd('.');
	}

	public (double X, double Y, double Z) ToFeet()
	{
		return (X: X / 0.3048, Y: Y / 0.3048, Z: Z / 0.3048);
	}

	public override string ToString()
	{
		return $"{GetFormattedStation()} ({X:F2}, {Y:F2}, {Z:F2})";
	}
}
