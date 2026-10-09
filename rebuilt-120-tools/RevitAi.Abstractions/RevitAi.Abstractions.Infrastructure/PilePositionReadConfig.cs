namespace RevitAi.Abstractions.Infrastructure;

public sealed class PilePositionReadConfig
{
	public int DataStartRow { get; set; } = 2;

	public int PierNumberColumn { get; set; } = 1;

	public int PileNumberColumn { get; set; } = 2;

	public int XCoordinateColumn { get; set; } = 3;

	public int YCoordinateColumn { get; set; } = 4;

	public int? PileLengthColumn { get; set; } = 5;
}
