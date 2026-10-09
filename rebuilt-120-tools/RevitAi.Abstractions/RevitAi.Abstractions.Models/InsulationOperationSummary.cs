using System.Collections.Generic;

namespace RevitAi.Abstractions.Models;

public class InsulationOperationSummary
{
	public int Added { get; set; }

	public int Modified { get; set; }

	public int Skipped { get; set; }

	public List<string> Errors { get; set; } = new List<string>();

	public static InsulationOperationSummary Failed(string message)
	{
		return new InsulationOperationSummary
		{
			Errors = new List<string> { message }
		};
	}
}
