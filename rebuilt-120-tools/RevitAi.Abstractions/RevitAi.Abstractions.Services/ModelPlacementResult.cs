namespace RevitAi.Abstractions.Services;

public sealed class ModelPlacementResult
{
	public bool IsSuccess { get; set; }

	public string? Error { get; set; }

	public object? ElementId { get; set; }

	public double StationKm { get; set; }

	public (double X, double Y, double Z) Position { get; set; }
}
