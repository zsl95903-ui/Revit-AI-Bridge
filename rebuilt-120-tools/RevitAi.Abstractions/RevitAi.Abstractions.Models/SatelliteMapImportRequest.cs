namespace RevitAi.Abstractions.Models;

public class SatelliteMapImportRequest
{
	public string? LocationName { get; set; }

	public double? Latitude { get; set; }

	public double? Longitude { get; set; }

	public string MapSource { get; set; } = "tianditu";

	public int ZoomLevel { get; set; } = 18;

	public double RadiusKm { get; set; } = 0.5;

	public object? Document { get; set; }

	public bool IsValid()
	{
		bool num = !string.IsNullOrWhiteSpace(LocationName);
		bool flag = Latitude.HasValue && Longitude.HasValue;
		if (!num && !flag)
		{
			return false;
		}
		if (ZoomLevel < 1 || ZoomLevel > 20)
		{
			return false;
		}
		if (RadiusKm < 0.1 || RadiusKm > 50.0)
		{
			return false;
		}
		if (MapSource != "tianditu" && MapSource != "google")
		{
			return false;
		}
		return true;
	}

	public (double lat, double lon)? GetCenterCoordinates()
	{
		if (Latitude.HasValue && Longitude.HasValue)
		{
			return (Latitude.Value, Longitude.Value);
		}
		return null;
	}
}
