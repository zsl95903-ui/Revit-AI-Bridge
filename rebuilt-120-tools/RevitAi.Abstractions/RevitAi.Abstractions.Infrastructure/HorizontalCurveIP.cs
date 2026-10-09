using System;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class HorizontalCurveIP
{
	public string? IPNumber { get; set; }

	public double Station { get; set; }

	public double CoordinateX { get; set; }

	public double CoordinateY { get; set; }

	public double DeflectionAngle { get; set; }

	public double Radius { get; set; }

	public double? TransitionParameterA { get; set; }

	public double? FirstTransitionLength { get; set; }

	public double? SecondTransitionLength { get; set; }

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
		string value = (FirstTransitionLength.HasValue ? $", Ls1={FirstTransitionLength.Value:F2}" : "");
		string value2 = (SecondTransitionLength.HasValue ? $", Ls2={SecondTransitionLength.Value:F2}" : "");
		return $"{IPNumber ?? "JD"} {GetFormattedStation()} R={Radius:F2}{value}{value2}";
	}
}
