namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class ManholeParameterData
{
	public string ManholeId { get; set; } = string.Empty;

	public double GroundElevationM { get; set; }

	public double PipeBottomElevationM { get; set; }

	public double WellDepthM { get; set; }

	public string? Source { get; set; }

	public bool IsValid()
	{
		return !string.IsNullOrWhiteSpace(ManholeId);
	}

	public bool HasValidElevation()
	{
		if (GroundElevationM == 0.0)
		{
			return PipeBottomElevationM != 0.0;
		}
		return true;
	}

	public bool HasValidWellDepth()
	{
		return WellDepthM > 0.0;
	}
}
