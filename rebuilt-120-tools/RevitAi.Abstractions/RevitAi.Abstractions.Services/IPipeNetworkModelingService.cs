using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Models.CADAnalysis;

namespace RevitAi.Abstractions.Services;

public interface IPipeNetworkModelingService
{
	Result<Dictionary<string, ManholeParameterData>> ReadManholeParameterTable(string filePath, string sheetName, ManholeParameterReadConfig? config = null);

	PipeNetworkModelingData MergeData(PipeNetworkAnalysisResult analysisResult, Dictionary<string, ManholeParameterData> parameterData, PipeNetworkModelingOptions? options = null);

	Task<PipeNetworkModelingResult> CreatePipeNetworkModelAsync(object document, PipeNetworkModelingData modelingData, PipeNetworkModelingOptions? options = null, Action<ModelingProgress>? progressCallback = null);
}
