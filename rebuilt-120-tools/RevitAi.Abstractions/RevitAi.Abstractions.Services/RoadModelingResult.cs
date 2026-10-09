using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public class RoadModelingResult
{
	public bool IsSuccess { get; set; }

	public int CreatedInstanceCount { get; set; }

	public int CreatedTypeCount { get; set; }

	public int CreatedMaterialCount { get; set; }

	public string? Error { get; set; }

	public string? Message { get; set; }

	public List<string> CreatedInstanceIds { get; set; } = new List<string>();

	public Dictionary<string, int> LayerInstanceCounts { get; set; } = new Dictionary<string, int>();
}
