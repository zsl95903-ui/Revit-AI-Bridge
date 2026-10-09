using System;

namespace RevitAi.Abstractions.Memory.Models;

public class SessionRecord
{
	public long Id { get; set; }

	public string SessionId { get; set; } = string.Empty;

	public string Title { get; set; } = string.Empty;

	public DateTime StartTime { get; set; }

	public DateTime? EndTime { get; set; }

	public string Summary { get; set; } = string.Empty;
}
