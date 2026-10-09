using System.Collections.Generic;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class AncillaryStructuresConfiguration
{
	public List<AncillaryStructureData> Structures { get; set; } = new List<AncillaryStructureData>();
}
