namespace RevitAi.Abstractions.Models;

public class SizeRangeInfo
{
	public string SizeRange { get; set; } = "";

	public double MinDiameter { get; set; }

	public double MaxDiameter { get; set; }

	public int ElementCount { get; set; }

	public int InsulatedCount { get; set; }

	public string Thickness { get; set; } = "0";

	public string Material { get; set; } = "";
}
