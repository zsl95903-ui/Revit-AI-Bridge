using System;

namespace RevitAi.Abstractions.Models;

public class GeoReferencePoint
{
	public double CenterLat { get; set; }

	public double CenterLon { get; set; }

	public double RevitOriginX { get; set; }

	public double RevitOriginY { get; set; }

	public double RevitOriginZ { get; set; }

	public DateTime Timestamp { get; set; }

	public GeoReferencePoint(double lat, double lon, double revitX, double revitY, double revitZ)
	{
		CenterLat = lat;
		CenterLon = lon;
		RevitOriginX = revitX;
		RevitOriginY = revitY;
		RevitOriginZ = revitZ;
		Timestamp = DateTime.Now;
	}

	public string GetDisplayText()
	{
		return $"({CenterLat:F6}, {CenterLon:F6}) → Revit({RevitOriginX:F2}, {RevitOriginY:F2}, {RevitOriginZ:F2})";
	}
}
