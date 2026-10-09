using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.LinkManagement;

public sealed class LinkManagementExternalEventRequest
{
	public LinkManagementOperation Operation { get; set; }

	public object? Document { get; set; }

	public object? LinkInstance { get; set; }

	public int? LinkId { get; set; }

	public List<object>? LinkInstances { get; set; }

	public List<int>? LinkIds { get; set; }

	public Action<Exception?, bool>? OnCompleted { get; set; }

	public string? ResultMessage { get; set; }

	public int SuccessCount { get; set; }

	public int FailureCount { get; set; }
}
