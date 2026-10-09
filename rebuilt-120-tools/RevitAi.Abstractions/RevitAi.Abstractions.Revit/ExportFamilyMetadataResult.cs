using System.Collections.Generic;

namespace RevitAi.Abstractions.Revit;

public class ExportFamilyMetadataResult
{
	public int SuccessCount { get; set; }

	public int FailCount { get; set; }

	public int SkippedCount { get; set; }

	public List<string> Errors { get; set; } = new List<string>();
}
