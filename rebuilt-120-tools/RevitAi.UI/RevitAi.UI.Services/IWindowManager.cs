using System;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Models;

namespace RevitAi.UI.Services;

public interface IWindowManager
{
	void ShowLoginWindow();

	void CloseLoginWindow();

	void ShowMainWindow();

	void CloseMainWindow();

	void ShowAboutWindow();

	void CloseAboutWindow();

	void ShowSettingsWindow();

	void CloseSettingsWindow();

	void ShowFeaturePanelWindow();

	void CloseFeaturePanelWindow();

	void ShowAIChatPanelWindow();

	void CloseAIChatPanelWindow();

	void SetAIChatTopMost(bool topMost);

	void SwitchToDockableMode();

	void SwitchToWindowMode();

	void ToggleAIChatWindow();

	bool IsAIChatWindowOpen();

	void HideAIChatWindow();

	void ShowFeatureStoreWindow();

	void CloseFeatureStoreWindow();

	void ShowBatchLinkModelsWindow();

	void CloseBatchLinkModelsWindow();

	void ShowManageModelLinksWindow();

	void CloseManageModelLinksWindow();

	void ShowRoadProjectManagementWindow();

	void CloseRoadProjectManagementWindow();

	void ShowStationElevationInputWindow(double offsetX, double offsetY, double offsetZ);

	void CloseStationElevationInputWindow();

	void ShowCurveInputWindow(double offsetX, double offsetY, double offsetZ);

	void CloseCurveInputWindow();

	void ShowPaymentDialog(string message, string commandText);

	void ShowRoadModelPlacementWindow();

	void CloseRoadModelPlacementWindow();

	void ShowMunicipalPipelineNetworkWindow();

	void CloseMunicipalPipelineNetworkWindow();

	void ShowCreateBridgeComponentWindow();

	void CloseCreateBridgeComponentWindow();

	void ShowDefinePileWindow();

	void CloseDefinePileWindow();

	void ShowDefineFoundationWindow();

	void CloseDefineFoundationWindow();

	void ShowDefinePierWindow();

	void CloseDefinePierWindow();

	void ShowDefineBeamWindow();

	void CloseDefineBeamWindow();

	void ShowDefineBearingWindow();

	void CloseDefineBearingWindow();

	void ShowDefineBridgeTypeWindow();

	void CloseDefineBridgeTypeWindow();

	void ShowEditBridgeComponentsWindow(RoadProject roadProject);

	void ShowFamilyLibraryWindow();

	void CloseFamilyLibraryWindow();

	void ShowTopographyFromFloorWindow();

	void CloseTopographyFromFloorWindow();

	void ShowExportFamilyMetadataWindow();

	void ShowMapSelectionWindow();

	void ShowMapSelectionWindowWithCallback(Action<MapSelectionCompletedEventArgs> callback, double? initialLat = null, double? initialLon = null);

	void ShowWallToRoadWindow();

	void ShowSubgradeModelWindow();

	void CloseSubgradeModelWindow();

	void ShowAncillaryStructureWindow();

	void CloseAncillaryStructureWindow();

	void ShowRoadSurfaceRefinementWindow();

	void CloseRoadSurfaceRefinementWindow();

	void ShowInsulationWindow();

	void CloseInsulationWindow();
}
