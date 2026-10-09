namespace RevitAi.Abstractions.Services;

public sealed class ViewExportResult
{
	public bool IsSuccess { get; set; }

	public string? FilePath { get; set; }

	public string? ViewName { get; set; }

	public string? Error { get; set; }
}
