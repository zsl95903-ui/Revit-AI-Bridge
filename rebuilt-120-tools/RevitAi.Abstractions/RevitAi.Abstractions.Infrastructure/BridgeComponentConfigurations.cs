using System.Collections.Generic;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class BridgeComponentConfigurations
{
	public List<BridgeComponentConfigurationDto> Configurations { get; set; } = new List<BridgeComponentConfigurationDto>();

	public int Count => Configurations.Count;

	public bool IsEmpty => Configurations.Count == 0;
}
