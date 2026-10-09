using System.Collections.Generic;

namespace RevitAi.UI.Models;

public sealed class ToolCallInfo
{
	public string ToolName { get; set; } = string.Empty;

	public IDictionary<string, object>? Parameters { get; set; }

	public string? CallId { get; set; }

	public ToolCallStatus Status { get; set; }

	public string? Result { get; set; }

	public string? Error { get; set; }
}
