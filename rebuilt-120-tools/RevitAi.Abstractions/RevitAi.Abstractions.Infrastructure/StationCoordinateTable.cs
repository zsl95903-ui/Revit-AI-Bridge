using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class StationCoordinateTable
{
	public List<StationCoordinate> Coordinates { get; set; } = new List<StationCoordinate>();

	public string? Source { get; set; }

	public List<StationCoordinate> GetSortedCoordinates()
	{
		return Coordinates.OrderBy((StationCoordinate c) => c.FullStationKm).ToList();
	}

	public List<StationCoordinate> GetCoordinatesInRange(double startKm, double endKm)
	{
		return (from c in Coordinates
			where c.FullStationKm >= startKm && c.FullStationKm <= endKm
			orderby c.FullStationKm
			select c).ToList();
	}

	public (double X, double Y)? InterpolateAtStation(double stationKm)
	{
		List<StationCoordinate> sortedCoordinates = GetSortedCoordinates();
		if (sortedCoordinates.Count == 0)
		{
			return null;
		}
		StationCoordinate stationCoordinate = sortedCoordinates.FirstOrDefault((StationCoordinate c) => Math.Abs(c.FullStationKm - stationKm) < 0.0001);
		if (stationCoordinate != null)
		{
			return (stationCoordinate.CoordinateX, stationCoordinate.CoordinateY);
		}
		StationCoordinate stationCoordinate2 = sortedCoordinates.LastOrDefault((StationCoordinate c) => c.FullStationKm < stationKm);
		StationCoordinate stationCoordinate3 = sortedCoordinates.FirstOrDefault((StationCoordinate c) => c.FullStationKm > stationKm);
		if (stationCoordinate2 == null || stationCoordinate3 == null)
		{
			return null;
		}
		double num = (stationKm - stationCoordinate2.FullStationKm) / (stationCoordinate3.FullStationKm - stationCoordinate2.FullStationKm);
		double item = stationCoordinate2.CoordinateX + num * (stationCoordinate3.CoordinateX - stationCoordinate2.CoordinateX);
		double item2 = stationCoordinate2.CoordinateY + num * (stationCoordinate3.CoordinateY - stationCoordinate2.CoordinateY);
		return (item, item2);
	}
}
