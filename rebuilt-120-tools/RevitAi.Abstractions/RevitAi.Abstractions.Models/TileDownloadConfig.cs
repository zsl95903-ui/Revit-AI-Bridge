namespace RevitAi.Abstractions.Models;

public class TileDownloadConfig
{
	public int MinDelayMs { get; set; } = 100;

	public int MaxDelayMs { get; set; } = 300;

	public int MaxConcurrentRequests { get; set; } = 2;

	public int MaxRetries { get; set; } = 5;

	public int RequestTimeoutSeconds { get; set; } = 30;
}
