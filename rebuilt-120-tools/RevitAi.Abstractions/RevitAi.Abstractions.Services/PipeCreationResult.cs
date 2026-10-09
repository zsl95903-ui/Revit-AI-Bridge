using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public sealed class PipeCreationResult
{
	public int SuccessCount { get; set; }

	public int FailureCount { get; set; }

	public List<string> Errors { get; set; } = new List<string>();

	public List<string> Warnings { get; set; } = new List<string>();

	public List<int> CreatedPipeIds { get; set; } = new List<int>();
}
