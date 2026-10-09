using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public class DwgBatchExportResult
{
	public string ExportDirectory { get; set; } = string.Empty;

	public int TotalViews { get; set; }

	public int SuccessCount { get; set; }

	public int FailCount { get; set; }

	public List<DwgExportResult> Results { get; set; } = new List<DwgExportResult>();
}
