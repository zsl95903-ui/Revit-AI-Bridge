namespace RevitAi.Abstractions.Services;

public sealed class StationElevationReadConfig
{
	public int DataStartRow { get; set; } = 2;

	public int StationColumn { get; set; } = 1;

	public int ElevationColumn { get; set; } = 2;

	public int? GradientColumn { get; set; } = 3;
}
