namespace RevitAi.Abstractions.Services;

public sealed class ModelPlacementPreview
{
	public double StationKm { get; set; }

	public (double X, double Y, double Z) Position { get; set; }

	public double Azimuth { get; set; }

	public string FormattedStation { get; set; } = string.Empty;
}
