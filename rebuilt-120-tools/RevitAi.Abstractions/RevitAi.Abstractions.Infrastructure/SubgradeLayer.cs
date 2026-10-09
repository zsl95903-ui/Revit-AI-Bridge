namespace RevitAi.Abstractions.Infrastructure;

public sealed class SubgradeLayer
{
	public string Name { get; set; } = "路基层";

	public double WidthIncrement { get; set; }

	public WidthIncrementType IncrementType { get; set; } = WidthIncrementType.BothSides;

	public double SlopeRatio { get; set; } = 1.5;

	public double Height { get; set; } = 300.0;

	public int Order { get; set; }

	public string Color { get; set; } = "#4CAF50";

	public double GetTopWidth(double baseWidth)
	{
		double num = ((IncrementType == WidthIncrementType.SingleSide) ? (WidthIncrement * 2.0) : WidthIncrement);
		return baseWidth + num;
	}

	public double GetBottomWidth(double baseWidth)
	{
		return GetTopWidth(baseWidth) + 2.0 * Height * SlopeRatio;
	}

	public override string ToString()
	{
		string value = ((IncrementType == WidthIncrementType.BothSides) ? "两侧" : "单侧");
		return $"{Name} (增量:{WidthIncrement}mm/{value}, 坡度:1:{SlopeRatio}, 高:{Height}mm)";
	}
}
