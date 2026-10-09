namespace RevitAi.Abstractions.Services;

public sealed class HorizontalCurveReadConfig
{
	public int DataStartRow { get; set; } = 2;

	public int IPNumberColumn { get; set; } = 1;

	public int StationColumn { get; set; } = 2;

	public int XCoordinateColumn { get; set; } = 3;

	public int YCoordinateColumn { get; set; } = 4;

	public int RadiusColumn { get; set; } = 5;

	public int? FirstTransitionLengthColumn { get; set; } = 6;

	public int? SecondTransitionLengthColumn { get; set; } = 7;
}
