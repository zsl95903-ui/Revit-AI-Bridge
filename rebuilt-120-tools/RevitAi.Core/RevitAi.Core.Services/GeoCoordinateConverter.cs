using System;

namespace RevitAi.Core.Services;

public static class GeoCoordinateConverter
{
	private const double double_0 = 6378137.0;

	public static double LatDeltaToMeters(double latDelta)
	{
		return latDelta * Math.PI / 180.0 * 6378137.0;
	}

	public static double LonDeltaToMeters(double lat, double lonDelta)
	{
		double d = lat * Math.PI / 180.0;
		return lonDelta * Math.PI / 180.0 * 6378137.0 * Math.Cos(d);
	}

	public static double MetersToLatDelta(double meters)
	{
		return meters / 6378137.0 * 180.0 / Math.PI;
	}

	public static double MetersToLonDelta(double lat, double meters)
	{
		double d = lat * Math.PI / 180.0;
		return meters / (6378137.0 * Math.Cos(d)) * 180.0 / Math.PI;
	}
}
