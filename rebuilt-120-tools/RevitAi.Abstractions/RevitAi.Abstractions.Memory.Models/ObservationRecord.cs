using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Memory.Models;

public class ObservationRecord
{
	public long Id { get; set; }

	public string SessionId { get; set; } = string.Empty;

	public ObservationType Type { get; set; }

	public string Content { get; set; } = string.Empty;

	public DateTime Timestamp { get; set; }

	public List<string> RelatedFiles { get; set; } = new List<string>();

	public List<string> RelatedComponents { get; set; } = new List<string>();

	public int Importance { get; set; } = 5;
}
