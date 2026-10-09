using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class CADElementSelectionRequest
{
	public SelectionMode Mode { get; set; }

	public string Prompt { get; set; } = "请选择图元";

	public SelectionObjectType ObjectType { get; set; }

	public bool AllowMultiple { get; set; }

	public object? Document { get; set; }

	public Action<bool, List<int>, List<string>>? OnCompleted { get; set; }
}
