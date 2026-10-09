using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Models;

public class InsulationRequest
{
	public InsulationRequestType RequestType { get; set; }

	public object? Document { get; set; }

	public List<SystemInsulationInfo>? SelectedSystems { get; set; }

	public SystemInsulationInfo? TargetSystem { get; set; }

	public List<SizeRangeInfo>? SizeRanges { get; set; }

	public List<SelectedElementInfo>? ManualElements { get; set; }

	public double ManualThicknessMM { get; set; }

	public string ManualMaterial { get; set; } = "";

	public List<int>? ElementIds { get; set; }

	public string Prompt { get; set; } = "";

	public Action<InsulationRequestResult>? OnCompleted { get; set; }
}
