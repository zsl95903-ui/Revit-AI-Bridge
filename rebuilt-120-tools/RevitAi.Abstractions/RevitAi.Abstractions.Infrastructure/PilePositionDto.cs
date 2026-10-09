namespace RevitAi.Abstractions.Infrastructure;

public sealed class PilePositionDto
{
	public int Number { get; set; }

	public string? PierNumber { get; set; }

	public string? PileNumber { get; set; }

	public string? PileTypeId { get; set; }

	public string? PileTypeName { get; set; }

	public string? XCoordinate { get; set; }

	public string? YCoordinate { get; set; }

	public string? PileLength { get; set; }
}
