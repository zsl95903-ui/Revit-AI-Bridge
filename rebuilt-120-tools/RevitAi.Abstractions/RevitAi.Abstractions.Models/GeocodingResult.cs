namespace RevitAi.Abstractions.Models;

public class GeocodingResult
{
	public double Latitude { get; set; }

	public double Longitude { get; set; }

	public string DisplayName { get; set; } = string.Empty;

	public string OriginalQuery { get; set; } = string.Empty;
}
