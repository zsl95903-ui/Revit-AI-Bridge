namespace RevitAi.Abstractions.Models;

public class GoogleMapResult
{
	public string LocalImagePath { get; set; } = string.Empty;

	public int ImageWidth { get; set; }

	public int ImageHeight { get; set; }

	public double WidthInFeet { get; set; }

	public double HeightInFeet { get; set; }

	public BoundingBox GeoBounds { get; set; }

	public double CenterLat { get; set; }

	public double CenterLon { get; set; }
}
