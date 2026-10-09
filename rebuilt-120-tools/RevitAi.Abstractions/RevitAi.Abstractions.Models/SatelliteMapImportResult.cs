namespace RevitAi.Abstractions.Models;

public class SatelliteMapImportResult
{
	public bool IsSuccess { get; set; }

	public string? ErrorMessage { get; set; }

	public int CreatedFloorElementId { get; set; }

	public int CreatedMaterialElementId { get; set; }

	public GeocodingResult? UsedLocation { get; set; }

	public double ImageWidthFeet { get; set; }

	public double ImageHeightFeet { get; set; }

	public int UsedZoomLevel { get; set; }

	public string UsedMapSource { get; set; } = string.Empty;
}
