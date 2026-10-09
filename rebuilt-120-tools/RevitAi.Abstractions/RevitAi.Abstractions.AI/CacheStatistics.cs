using System;

namespace RevitAi.Abstractions.AI;

public class CacheStatistics
{
	public string CacheId { get; set; } = string.Empty;

	public string SessionId { get; set; } = string.Empty;

	public string Key { get; set; } = string.Empty;

	public DateTime CreatedAt { get; set; }

	public DateTime LastAccessedAt { get; set; }

	public int AccessCount { get; set; }

	public long EstimatedSizeBytes { get; set; }

	public string DataType { get; set; } = string.Empty;
}
