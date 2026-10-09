using System;

namespace RevitAi.Abstractions.Services;

public sealed class ModelPlacementConfig
{
	public Guid ProjectId { get; set; }

	public double StationKm { get; set; }

	public double LateralOffset { get; set; }

	public double VerticalOffset { get; set; }

	public double RotationAngle { get; set; }

	public string? FamilyTypeName { get; set; }

	public string? FamilyName { get; set; }
}
