using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Services;

namespace RevitAi.Abstractions.Revit;

public class CreateTopographyRequest
{
	public object Document { get; set; }

	public List<object> FloorElements { get; set; }

	public object TopographyElement { get; set; }

	public Action<(int SuccessCount, List<string> Errors, List<object> CreatedTopographies)>? OnCompleted { get; set; }

	public Action<int, int, string?>? OnProgress { get; set; }

	public IProgressReporter? ProgressReporter { get; set; }
}
