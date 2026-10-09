using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class AncillaryStructureData
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

	public List<AncillaryStructureLayoutSegmentData> LayoutSegments { get; set; } = new List<AncillaryStructureLayoutSegmentData>();

	public AncillaryStructure ToAncillaryStructure()
	{
		return new AncillaryStructure
		{
			Id = Id,
			Name = Name,
			StructureType = StructureType,
			PositionType = PositionType,
			HorizontalOffset = HorizontalOffset,
			ElevationDifference = ElevationDifference,
			Width = Width,
			Height = Height,
			SlopeRatio = SlopeRatio,
			Color = Color,
			IsEnabled = IsEnabled,
			Order = Order
		};
	}

	public static AncillaryStructureData FromAncillaryStructure(AncillaryStructure structure)
	{
		return new AncillaryStructureData
		{
			Id = structure.Id,
			Name = structure.Name,
			StructureType = structure.StructureType,
			PositionType = structure.PositionType,
			HorizontalOffset = structure.HorizontalOffset,
			ElevationDifference = structure.ElevationDifference,
			Width = structure.Width,
			Height = structure.Height,
			SlopeRatio = structure.SlopeRatio,
			Color = structure.Color,
			IsEnabled = structure.IsEnabled,
			Order = structure.Order
		};
	}
}
