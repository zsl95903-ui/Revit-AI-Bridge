using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface ICADFileService
{
	CADFileInfo? GetCADFilePath(object importInstance, object document);

	CADFileData? ParseCADFile(string filePath, double conversionFactorToMM);

	CADFileData? ParseImportInstance(object importInstance, object document);

	CADFileData? FilterByLayer(object importInstance, object document, string layerName);

	List<CADFileData> ParseMultipleImportInstances(IEnumerable<object> importInstances, object document);

	bool IsDwgFile(string filePath);

	string? ConvertDwgToDxf(string dwgFilePath, string? outputFolder = null);

	DwgConverterStatus GetConverterStatus();

	List<string> GetLayersFromCADFile(string cadFilePath);
}
