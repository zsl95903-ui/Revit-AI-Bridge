namespace RevitAi.Abstractions.Infrastructure;

public sealed class SubgradeLayerData
{
	public string Name { get; set; } = string.Empty;

	public double WidthIncrement { get; set; }

	public WidthIncrementType IncrementType { get; set; }

	public double SlopeRatio { get; set; }

	public double Height { get; set; }

	public int Order { get; set; }

	public string Color { get; set; } = "#4CAF50";

	public SubgradeLayer ToSubgradeLayer()
	{
		return new SubgradeLayer
		{
			Name = Name,
			WidthIncrement = WidthIncrement,
			IncrementType = IncrementType,
			SlopeRatio = SlopeRatio,
			Height = Height,
			Order = Order,
			Color = Color
		};
	}

	public static SubgradeLayerData FromSubgradeLayer(SubgradeLayer layer)
	{
		return new SubgradeLayerData
		{
			Name = layer.Name,
			WidthIncrement = layer.WidthIncrement,
			IncrementType = layer.IncrementType,
			SlopeRatio = layer.SlopeRatio,
			Height = layer.Height,
			Order = layer.Order,
			Color = layer.Color
		};
	}

	public double GetTopWidth(double baseWidth)
	{
		double num = ((IncrementType == WidthIncrementType.SingleSide) ? (WidthIncrement * 2.0) : WidthIncrement);
		return baseWidth + num;
	}
}
