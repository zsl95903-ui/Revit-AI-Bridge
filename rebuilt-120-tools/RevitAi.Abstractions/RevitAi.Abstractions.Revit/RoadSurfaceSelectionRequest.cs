using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Revit;

public class RoadSurfaceSelectionRequest
{
	public RoadSurfaceSelectionMode Mode { get; set; }

	public string Prompt { get; set; } = string.Empty;

	public Action<List<object>>? OnCompleted { get; set; }

	public int? FilterCategoryId { get; set; }

	public bool IsSingleSelection { get; set; }
}
