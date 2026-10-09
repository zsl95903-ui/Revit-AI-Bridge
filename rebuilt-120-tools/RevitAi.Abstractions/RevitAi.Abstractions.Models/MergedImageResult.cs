namespace RevitAi.Abstractions.Models;

public class MergedImageResult
{
	public string ImagePath { get; set; } = string.Empty;

	public int ImageWidth { get; set; }

	public int ImageHeight { get; set; }

	public RevitSize RevitSize { get; set; }

	public BoundingBox GeoBounds { get; set; }
}
