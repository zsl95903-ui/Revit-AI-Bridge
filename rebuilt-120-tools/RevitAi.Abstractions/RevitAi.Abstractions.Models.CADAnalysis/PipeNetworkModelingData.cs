using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class PipeNetworkModelingData
{
	public List<EnrichedManholeData> Manholes { get; set; } = new List<EnrichedManholeData>();

	public List<EnrichedPipeData> Pipes { get; set; } = new List<EnrichedPipeData>();

	public PipeNetworkAnalysisResult? AnalysisResult { get; set; }

	public string? ParameterDataSource { get; set; }

	public int GetCompleteManholesCount()
	{
		return Manholes.Count((EnrichedManholeData m) => m.HasCompleteData);
	}

	public int GetCompletePipesCount()
	{
		return Pipes.Count((EnrichedPipeData p) => p.HasCompleteConnections);
	}

	public string GetSummary()
	{
		return $"管井总数: {Manholes.Count} (完整数据: {GetCompleteManholesCount()})\n管道总数: {Pipes.Count} (完整连接: {GetCompletePipesCount()})";
	}
}
