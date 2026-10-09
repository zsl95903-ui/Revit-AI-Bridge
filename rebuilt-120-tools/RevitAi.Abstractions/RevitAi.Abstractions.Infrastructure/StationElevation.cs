using System;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class StationElevation
{
	public string Prefix { get; set; } = "K";

	public int StationInteger { get; set; }

	public double StationDecimal { get; set; }

	public double FullStationKm => (double)StationInteger + StationDecimal / 1000.0;

	public double Elevation { get; set; }

	public double? Gradient { get; set; }

	public string GetFormattedStation()
	{
		int stationInteger = StationInteger;
		double stationDecimal = StationDecimal;
		string value = ((stationDecimal >= 0.0) ? "+" : "-");
		double num = Math.Abs(stationDecimal);
		int num2 = (int)num;
		double num3 = num - (double)num2;
		if (num3 < 0.001)
		{
			return $"{Prefix}{stationInteger}{value}{num2:D3}";
		}
		return $"{Prefix}{stationInteger}{value}{num2:D3}.{num3:F3}".TrimEnd('0').TrimEnd('.');
	}

	public override string ToString()
	{
		return $"{GetFormattedStation()} H={Elevation:F2}";
	}
}
