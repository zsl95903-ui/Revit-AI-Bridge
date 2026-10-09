using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public sealed class FloorTopographyResult
{
	public int SuccessCount { get; set; }

	public List<string> Errors { get; set; } = new List<string>();

	public List<object> CreatedTopographies { get; set; } = new List<object>();

	public List<int> CreatedSubRegionIds { get; set; } = new List<int>();
}
