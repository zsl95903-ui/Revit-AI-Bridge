using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Common;

namespace RevitAi.Abstractions.Services;

public interface IModelPlacementService
{
	Result<ModelPlacementResult> PlaceModelAtStation(object document, ModelPlacementConfig config);

	Result<List<ModelPlacementResult>> PlaceModelsAlongRoad(object document, Guid projectId, double startStationKm, double endStationKm, double interval, string familyName, string familyTypeName, double lateralOffset = 0.0);

	Result<List<ModelPlacementResult>> PlaceMultipleModels(object document, List<ModelPlacementConfig> configs);

	Result<ModelPlacementPreview> PreviewPlacement(Guid projectId, double stationKm, double lateralOffset = 0.0);
}
