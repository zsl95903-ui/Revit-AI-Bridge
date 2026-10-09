using System;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class StationCoordinate
{
	public string Prefix { get; set; } = "K";

	public int StationInteger { get; set; }

	public double StationDecimal { get; set; }

	public double FullStationKm => (double)StationInteger + StationDecimal / 1000.0;

	public double CoordinateX { get; set; }

	public double CoordinateY { get; set; }

	public double? Elevation { get; set; }

	public double? Azimuth { get; set; }

	public string GetFormattedStation()
	{
		_ = FullStationKm;
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

	public static bool TryParse(string stationString, out StationCoordinate? coordinate)
	{
		coordinate = null;
		if (string.IsNullOrWhiteSpace(stationString))
		{
			return false;
		}
		try
		{
			string text = stationString.Trim();
			string prefix = "K";
			int num = text.LastIndexOf('+');
			bool flag = false;
			if (num < 0)
			{
				num = text.LastIndexOf('-');
				if (num <= 0)
				{
					double num2 = double.Parse(text);
					int num3 = (int)num2;
					double stationDecimal = (num2 - (double)num3) * 1000.0;
					coordinate = new StationCoordinate
					{
						Prefix = prefix,
						StationInteger = num3,
						StationDecimal = stationDecimal
					};
					return true;
				}
				flag = true;
			}
			if (num <= 0)
			{
				return false;
			}
			string text2 = text.Substring(0, num);
			int num4 = text2.LastIndexOf('K');
			if (num4 < 0)
			{
				num4 = text2.LastIndexOf('k');
			}
			if (num4 >= 0)
			{
				prefix = text2.Substring(0, num4 + 1).ToUpper();
				int stationInteger = int.Parse(text2.Substring(num4 + 1).Trim());
				double num5 = double.Parse(text.Substring(num + 1).Trim());
				if (flag)
				{
					num5 = 0.0 - num5;
				}
				coordinate = new StationCoordinate
				{
					Prefix = prefix,
					StationInteger = stationInteger,
					StationDecimal = num5
				};
				return true;
			}
			int stationInteger2 = int.Parse(text2.Trim());
			double num6 = double.Parse(text.Substring(num + 1).Trim());
			if (flag)
			{
				num6 = 0.0 - num6;
			}
			coordinate = new StationCoordinate
			{
				Prefix = prefix,
				StationInteger = stationInteger2,
				StationDecimal = num6
			};
			return true;
		}
		catch
		{
			return false;
		}
	}

	public override string ToString()
	{
		return $"{GetFormattedStation()} ({CoordinateX:F2}, {CoordinateY:F2})";
	}
}
