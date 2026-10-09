using System.Collections.Generic;

namespace RevitAi.Abstractions.Models;

public sealed class WallFloorAlignmentResult
{
	public int SuccessCount { get; }

	public int FailedCount { get; }

	public IReadOnlyList<string> Errors { get; }

	public bool HasErrors => Errors.Count > 0;

	public int TotalCount => SuccessCount + FailedCount;

	public WallFloorAlignmentResult(int successCount, int failedCount, List<string> errors)
	{
		SuccessCount = successCount;
		FailedCount = failedCount;
		Errors = errors;
	}
}
