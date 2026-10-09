using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.LinkManagement;

public sealed class BatchLinkModelsExternalEventRequest
{
	public object? Document { get; set; }

	public List<string>? FilePaths { get; set; }

	public Action<Exception?, int>? OnCompleted { get; set; }

	public string? ResultMessage { get; set; }

	public int SuccessCount { get; set; }

	public int FailureCount { get; set; }
}
