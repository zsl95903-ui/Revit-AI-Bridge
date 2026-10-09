using System.Collections.Generic;

namespace RevitAi.Abstractions.Models;

public class InsulationRequestResult
{
	public string? Error { get; set; }

	public List<SystemInsulationInfo>? Systems { get; set; }

	public List<InsulationTypeInfoLite>? InsulationTypes { get; set; }

	public List<SizeRangeInfo>? SizeRanges { get; set; }

	public List<SelectedElementInfo>? PickedElements { get; set; }

	public InsulationOperationSummary? Summary { get; set; }

	public int Count { get; set; }

	public string Message { get; set; } = "";
}
