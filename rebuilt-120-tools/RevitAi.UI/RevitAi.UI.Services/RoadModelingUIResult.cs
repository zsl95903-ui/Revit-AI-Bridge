using System.Collections.Generic;

namespace RevitAi.UI.Services;

public class RoadModelingUIResult
{
	public bool IsSuccess { get; set; }

	public int CreatedInstanceCount { get; set; }

	public int CreatedTypeCount { get; set; }

	public int CreatedMaterialCount { get; set; }

	public string? Error { get; set; }

	public string? Message { get; set; }

	public Dictionary<string, int> LayerInstanceCounts { get; set; } = new Dictionary<string, int>();
}
