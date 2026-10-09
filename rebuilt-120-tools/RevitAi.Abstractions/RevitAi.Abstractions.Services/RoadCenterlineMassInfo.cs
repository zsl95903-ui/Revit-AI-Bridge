namespace RevitAi.Abstractions.Services;

public sealed class RoadCenterlineMassInfo
{
	public object? MassInstance { get; set; }

	public object? Curve3D { get; set; }

	public double TotalHorizontalLengthMeters { get; set; }

	public string? TempFamilyPath { get; set; }
}
