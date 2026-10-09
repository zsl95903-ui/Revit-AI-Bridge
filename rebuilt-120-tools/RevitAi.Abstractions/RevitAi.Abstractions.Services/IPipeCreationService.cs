using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Models.CADAnalysis;

namespace RevitAi.Abstractions.Services;

public interface IPipeCreationService
{
	object? CreatePipe(object document, EnrichedPipeData pipeData, PipeNetworkModelingOptions options);

	PipeCreationResult CreatePipes(object document, List<EnrichedPipeData> pipes, PipeNetworkModelingOptions options, Action<int, int>? progressCallback = null);
}
