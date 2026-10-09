using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class StationElevationTable
{
	public List<StationElevation> Elevations { get; set; } = new List<StationElevation>();

	public string? Source { get; set; }

	public List<StationElevation> GetSortedElevations()
	{
		return Elevations.OrderBy((StationElevation e) => e.FullStationKm).ToList();
	}

	public double? InterpolateElevationAtStation(double stationKm)
	{
		List<StationElevation> sortedElevations = GetSortedElevations();
		if (sortedElevations.Count == 0)
		{
			return null;
		}
		StationElevation stationElevation = sortedElevations.FirstOrDefault((StationElevation e) => Math.Abs(e.FullStationKm - stationKm) < 0.0001);
		if (stationElevation != null)
		{
			return stationElevation.Elevation;
		}
		StationElevation stationElevation2 = sortedElevations.LastOrDefault((StationElevation e) => e.FullStationKm < stationKm);
		StationElevation stationElevation3 = sortedElevations.FirstOrDefault((StationElevation e) => e.FullStationKm > stationKm);
		if (stationElevation2 == null || stationElevation3 == null)
		{
			return null;
		}
		double num = (stationKm - stationElevation2.FullStationKm) / (stationElevation3.FullStationKm - stationElevation2.FullStationKm);
		return stationElevation2.Elevation + num * (stationElevation3.Elevation - stationElevation2.Elevation);
	}
}
