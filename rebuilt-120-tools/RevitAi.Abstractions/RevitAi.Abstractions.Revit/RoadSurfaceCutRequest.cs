using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Services;

namespace RevitAi.Abstractions.Revit;

public class RoadSurfaceCutRequest
{
	public List<object> VoidInstances { get; set; } = new List<object>();

	public List<string> CategoryNames { get; set; } = new List<string>();

	public List<object> SelectedCategoryInstances { get; set; } = new List<object>();

	public Action<(int SuccessCount, List<string> Errors)>? OnCompleted { get; set; }

	public bool CutAllInstances { get; set; }

	public IProgressReporter? ProgressReporter { get; set; }
}
