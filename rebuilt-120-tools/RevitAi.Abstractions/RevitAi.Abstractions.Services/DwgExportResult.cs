namespace RevitAi.Abstractions.Services;

public class DwgExportResult
{
	public bool IsSuccess { get; set; }

	public string? FilePath { get; set; }

	public string? ViewName { get; set; }

	public string? Error { get; set; }
}
