using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Update.Models;

public sealed class LocalVersionInfo
{
	public string CurrentVersion { get; set; } = string.Empty;

	public DateTime InstallDate { get; set; }

	public DateTime? LastUpdateCheck { get; set; }

	public List<UpdateHistoryItem> UpdateHistory { get; set; } = new List<UpdateHistoryItem>();
}
