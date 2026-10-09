using System;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class AncillaryStructure
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string Name { get; set; } = string.Empty;

	public AncillaryStructureType StructureType { get; set; }

	public AncillaryPositionType PositionType { get; set; }

	public double HorizontalOffset { get; set; }

	public double ElevationDifference { get; set; }

	public double Width { get; set; }

	public double Height { get; set; }

	public double? SlopeRatio { get; set; }

	public string Color { get; set; } = "#808080";

	public bool IsEnabled { get; set; } = true;

	public int Order { get; set; }

	public override string ToString()
	{
		string value = PositionType switch
		{
			AncillaryPositionType.Left => "左", 
			AncillaryPositionType.Right => "右", 
			AncillaryPositionType.Both => "两侧", 
			_ => "", 
		};
		string value2 = StructureType switch
		{
			AncillaryStructureType.Trapezoid => "梯形", 
			AncillaryStructureType.Rectangle => "矩形", 
			AncillaryStructureType.RectangleHollow => "矩形空心", 
			AncillaryStructureType.SingleSlopeSurface => "单坡面层", 
			AncillaryStructureType.DoubleSlopeSurface => "双坡面层", 
			AncillaryStructureType.RoundedCurb => "圆角路沿石", 
			_ => "其他", 
		};
		return $"{Name} ({value}{value2}, 宽:{Width}mm, 偏移距路面边缘:{HorizontalOffset}mm)";
	}
}
