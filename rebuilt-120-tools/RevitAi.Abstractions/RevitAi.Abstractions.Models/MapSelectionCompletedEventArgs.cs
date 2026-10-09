namespace RevitAi.Abstractions.Models;

public class MapSelectionCompletedEventArgs
{
	public BoundingBox BoundingBox { get; set; }

	public string MapSource { get; set; } = string.Empty;

	public string? TiandituToken { get; set; }

	public string? GoogleMapsApiKey { get; set; }
}
