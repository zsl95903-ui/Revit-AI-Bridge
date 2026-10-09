using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RevitAi.UI.Models;

public class AIConfigStorage
{
	[JsonPropertyName("version")]
	public int Version { get; set; } = 1;

	[JsonPropertyName("configs")]
	public List<AIProviderConfig> Configs { get; set; } = new List<AIProviderConfig>();
}
