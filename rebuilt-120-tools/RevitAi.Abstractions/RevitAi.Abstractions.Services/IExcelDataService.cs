using System.Collections.Generic;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Models.CADAnalysis;

namespace RevitAi.Abstractions.Services;

public interface IExcelDataService
{
	Result<List<string>> GetSheetNames(string filePath);

	Result<StationCoordinateTable> ReadStationCoordinateTable(string filePath, string sheetName, StationCoordinateReadConfig? config = null);

	Result<HorizontalCurveTable> ReadHorizontalCurveTable(string filePath, string sheetName, HorizontalCurveReadConfig? config = null);

	Result<StationElevationTable> ReadStationElevationTable(string filePath, string sheetName, StationElevationReadConfig? config = null);

	Result<VerticalCurveTable> ReadVerticalCurveTable(string filePath, string sheetName, VerticalCurveReadConfig? config = null);

	Result<Dictionary<string, ManholeParameterData>> ReadManholeParameterTable(string filePath, string sheetName, ManholeParameterReadConfig? config = null);

	Result<PilePositionExcelData> ReadPilePositionTable(string filePath, string sheetName, PilePositionReadConfig? config = null);

	Result<string> ExportDataToExcel(string? filePath, string sheetName, List<List<string>> data, bool autoFormat = true);
}
