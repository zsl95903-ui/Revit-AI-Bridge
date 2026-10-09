namespace RevitAi.Abstractions.Services;

public class GridCurveInfo
{
	public string CurveType { get; set; } = "Line";

	public double StartX { get; set; }

	public double StartY { get; set; }

	public double StartZ { get; set; }

	public double EndX { get; set; }

	public double EndY { get; set; }

	public double EndZ { get; set; }

	public double DirectionX { get; set; }

	public double DirectionY { get; set; }

	public double DirectionZ { get; set; }

	public double Length { get; set; }
}
