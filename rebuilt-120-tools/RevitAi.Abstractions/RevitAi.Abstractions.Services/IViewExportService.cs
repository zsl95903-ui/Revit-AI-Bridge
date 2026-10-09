using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IViewExportService
{
	ViewExportResult ExportView(object document, object view, ViewExportConfig config);

	BatchExportResult ExportViews(object document, IEnumerable<object> views, string directory, ViewExportConfig? config = null);

	BatchExportResult ExportViewsByIds(object document, IEnumerable<int> viewIds, string directory, ViewExportConfig? config = null);

	string? GetDefaultFileName(object view, string format = "png");

	string? SelectFolder(string? initialDirectory = null);
}
