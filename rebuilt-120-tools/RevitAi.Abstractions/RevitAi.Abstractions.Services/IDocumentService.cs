using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IDocumentService
{
	object? GetActiveDocument();

	IEnumerable<object> GetAllDocuments();

	object? CreateNewDocument(string? templateFile = null);

	object? OpenDocument(string filePath);

	bool CloseDocument(object document, bool save = true);

	bool SaveDocument(object document, string? filePath = null);

	string? GetDocumentPath(object document);

	string? GetDocumentName(object document);

	bool IsDocumentModified(object document);

	object? CreateSheet(object document, int titleBlockTypeId, string sheetName, string sheetNumber);

	object? CreateSheet(object document, int? titleBlockTypeId, string sheetName, string sheetNumber);

	object? AddViewToSheet(object document, object sheet, object view, object? position);

	object? PlaceViewOnSheet(object document, object sheet, object view, object position);

	bool ExportToPDF(object document, string filePath, IEnumerable<int>? viewIds = null);
}
