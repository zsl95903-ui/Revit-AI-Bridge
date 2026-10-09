using System;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class VerticalCurvePVI
{
	public string? PointNumber { get; set; }

	public double Station { get; set; }

	public double Elevation { get; set; }

	public double Radius { get; set; }

	public VerticalCurveType CurveType { get; set; }

	public double? FrontGradient { get; set; }

	public double? BackGradient { get; set; }

	public double? CurveLength { get; set; }

	public string GetFormattedStation()
	{
		int num = (int)Station;
		double num2 = (Station - (double)num) * 1000.0;
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

	public override string ToString()
	{
		return $"{PointNumber ?? "PVI"} {GetFormattedStation()} H={Elevation:F2} R={Radius:F2}";
	}
}
