using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Services;

public class SplitResult
{
	public bool Success { get; set; }

	public IEnumerable<int> NewElementIds { get; set; } = Enumerable.Empty<int>();

	public int SegmentCount { get; set; }

	public IEnumerable<int> CreatedFittingIds { get; set; } = Enumerable.Empty<int>();

	public string? Error { get; set; }

	public string? SkippedReason { get; set; }
}
