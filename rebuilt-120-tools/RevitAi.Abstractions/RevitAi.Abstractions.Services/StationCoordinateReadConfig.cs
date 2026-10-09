namespace RevitAi.Abstractions.Services;

public sealed class StationCoordinateReadConfig
{
	public int DataStartRow { get; set; } = 2;

	public int StationColumn { get; set; } = 1;

	public int XCoordinateColumn { get; set; } = 2;

	public int YCoordinateColumn { get; set; } = 3;

	public int? ElevationColumn { get; set; } = 4;

	public int? AzimuthColumn { get; set; } = 5;
}
