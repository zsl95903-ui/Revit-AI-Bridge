using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class StationParameter
{
	public double StationKm { get; set; }

	public double? RoadWidth { get; set; }

	public double? CrossSectionOffsetAngle { get; set; }

	public string? Note { get; set; }

	public string StationDisplay => GetStationDisplay();

	public string GetStationDisplay()
	{
		int num = (int)Math.Floor(StationKm);
		double num2 = (StationKm - (double)num) * 1000.0;
		if (!(Math.Abs(Math.Round(num2, 3) - Math.Round(num2)) < 0.001))
		{
			return $"K{num}+{num2:F3}";
		}
		return $"K{num}+{(int)Math.Round(num2):D3}";
	}

	public override string ToString()
	{
		List<string> list = new List<string> { GetStationDisplay() };
		if (RoadWidth.HasValue)
		{
			list.Add($"宽度:{RoadWidth.Value:F2}m");
		}
		if (CrossSectionOffsetAngle.HasValue)
		{
			list.Add($"偏转角:{CrossSectionOffsetAngle.Value:F1}°");
		}
		return string.Join(", ", list);
	}
}
