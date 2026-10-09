namespace RevitAi.Abstractions.Services;

public sealed class CADBlockInfo
{
	public string Name { get; set; } = string.Empty;

	public double X { get; set; }

	public double Y { get; set; }

	public double Z { get; set; }

	public string LayerName { get; set; } = string.Empty;

	public double? ScaleX { get; set; }

	public double? ScaleY { get; set; }

	public double? ScaleZ { get; set; }

	public double? Rotation { get; set; }
}
