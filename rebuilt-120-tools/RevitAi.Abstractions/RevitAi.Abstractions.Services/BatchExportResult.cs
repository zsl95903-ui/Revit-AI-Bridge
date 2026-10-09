using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public sealed class BatchExportResult
{
	public int TotalViews { get; set; }

	public int SuccessCount { get; set; }

	public int FailureCount { get; set; }

	public string? ExportDirectory { get; set; }

	public List<ViewExportResult> Results { get; set; } = new List<ViewExportResult>();
}
