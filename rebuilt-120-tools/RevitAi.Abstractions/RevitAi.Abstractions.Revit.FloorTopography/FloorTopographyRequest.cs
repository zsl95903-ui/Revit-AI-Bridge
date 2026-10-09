using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Revit.FloorTopography;

public sealed class FloorTopographyRequest
{
	public List<int> FloorElementIds { get; set; } = new List<int>();

	public int TopographyElementId { get; set; }

	public Action<Exception?, bool>? OnCompleted { get; set; }

	public List<int> CreatedSubRegionIds { get; set; } = new List<int>();

	public int SuccessCount { get; set; }

	public List<string> Errors { get; set; } = new List<string>();

	public object? Document { get; set; }
}
