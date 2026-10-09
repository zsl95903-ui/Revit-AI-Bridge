using System.Collections.Generic;

namespace RevitAi.Abstractions.Models;

public class InsulationOperationResult
{
	public bool Success { get; set; }

	public int Added { get; set; }

	public int Modified { get; set; }

	public int Deleted { get; set; }

	public int Skipped { get; set; }

	public List<string> Errors { get; set; } = new List<string>();

	public string? SystemTypeName { get; set; }

	public InsulationTargetType TargetType { get; set; }
}
