namespace RevitAi.Abstractions.Infrastructure;

public sealed class BridgeComponentConfigurationDto
{
	public int Number { get; set; }

	public string Station { get; set; } = "0+000";

	public string? BridgeTypeId { get; set; }

	public string? BridgeTypeName { get; set; }

	public string? BridgeParameters { get; set; }

	public string? FoundationTypeId { get; set; }

	public string? FoundationTypeName { get; set; }

	public string? FoundationParameters { get; set; }

	public string? PierTypeId { get; set; }

	public string? PierTypeName { get; set; }

	public string? PierParameters { get; set; }

	public string? BeamTypeId { get; set; }

	public string? BeamTypeName { get; set; }

	public string? BeamParameters { get; set; }

	public string? BearingTypeId { get; set; }

	public string? BearingTypeName { get; set; }

	public string? BearingParameters { get; set; }
}
