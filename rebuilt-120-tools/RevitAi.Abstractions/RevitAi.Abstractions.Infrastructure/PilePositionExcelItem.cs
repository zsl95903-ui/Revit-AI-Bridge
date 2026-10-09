namespace RevitAi.Abstractions.Infrastructure;

public sealed class PilePositionExcelItem
{
	public string PierNumber { get; set; } = string.Empty;

	public string PileNumber { get; set; } = string.Empty;

	public double XCoordinate { get; set; }

	public double YCoordinate { get; set; }

	public double? PileLength { get; set; }
}
