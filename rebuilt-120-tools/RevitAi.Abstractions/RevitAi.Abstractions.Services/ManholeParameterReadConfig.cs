namespace RevitAi.Abstractions.Services;

public sealed class ManholeParameterReadConfig
{
	public int DataStartRow { get; set; } = 2;

	public int ManholeIdColumn { get; set; } = 1;

	public int GroundElevationColumn { get; set; } = 2;

	public int PipeBottomElevationColumn { get; set; } = 3;

	public int WellDepthColumn { get; set; } = 4;
}
