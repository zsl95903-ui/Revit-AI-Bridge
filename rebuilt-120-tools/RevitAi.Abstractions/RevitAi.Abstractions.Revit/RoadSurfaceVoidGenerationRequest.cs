using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Services;

namespace RevitAi.Abstractions.Revit;

public class RoadSurfaceVoidGenerationRequest
{
	public object FloorElement { get; set; }

	public Action<(bool Success, string Message, List<object>? CreatedVoids)>? OnCompleted { get; set; }

	public IProgressReporter? ProgressReporter { get; set; }
}
