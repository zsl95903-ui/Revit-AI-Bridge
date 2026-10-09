using System;
using System.Runtime.CompilerServices;

namespace RevitAi.Abstractions.Models;

public class BoundingBox
{
	public double minLon { get; set; }

	public double minLat { get; set; }

	public double maxLon { get; set; }

	public double maxLat { get; set; }

	public double Width => maxLon - minLon;

	public double Height => maxLat - minLat;

	public double CenterLon => (minLon + maxLon) / 2.0;

	public double CenterLat => (minLat + maxLat) / 2.0;

	public string GetDisplayText()
	{
		InlineArray4<object> buffer = default(InlineArray4<object>);
		buffer[0] = minLon;
		buffer[1] = minLat;
		buffer[2] = maxLon;
		buffer[3] = maxLat;
		return string.Format("[{0:F6}, {1:F6}] 至 [{2:F6}, {3:F6}]", (ReadOnlySpan<object?>)buffer);
	}

	public bool IsValid()
	{
		if (minLon >= -180.0 && maxLon <= 180.0 && minLat >= -90.0 && maxLat <= 90.0 && minLon < maxLon)
		{
			return minLat < maxLat;
		}
		return false;
	}
}
