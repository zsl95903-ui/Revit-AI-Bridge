using System;

namespace RevitAi.Abstractions.Update.Models;

public sealed class UpdateHistoryItem
{
	public string Version { get; set; } = string.Empty;

	public DateTime UpdateDate { get; set; }

	public string UpdateFrom { get; set; } = string.Empty;

	public string UpdateMethod { get; set; } = string.Empty;
}
