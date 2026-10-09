using System.Collections.Generic;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class StationParametersConfiguration
{
	public List<StationParameterData> Parameters { get; set; } = new List<StationParameterData>();

	public double DefaultRoadWidthM { get; set; } = 7.0;
}
