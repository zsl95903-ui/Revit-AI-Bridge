using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IDwgExportService
{
	DwgExportResult ExportView(object document, object view, DwgExportConfig config);

	DwgBatchExportResult ExportViews(object document, IEnumerable<object> views, string directory, DwgExportConfig? config = null);

	DwgBatchExportResult ExportViewsByIds(object document, IEnumerable<int> viewIds, string directory, DwgExportConfig? config = null);

	string? GetDefaultFileName(object view, string format = "dwg");

	string? SelectFolder(string? initialDirectory = null);
}
