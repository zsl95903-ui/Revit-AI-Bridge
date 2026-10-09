using System.Collections.Generic;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class RoadStructureConfiguration
{
	public List<SubgradeLayerData> Layers { get; set; } = new List<SubgradeLayerData>();

	public bool SplitAtIntegerStations { get; set; } = true;

	public int IntegerStationInterval { get; set; } = 20;
}
