using RevitAi.Abstractions.Adapters;

namespace RevitAi.Abstractions.Loader;

public interface IModuleLoader
{
	IRevitAdapter? RevitAdapter { get; set; }

	void ShowAboutWindow();

	void ShowLoginWindow();

	void ShowAIChatWindow();

	void ShowFeatureStoreWindow();

	void ShowBatchLinkModelsWindow();

	void ShowManageModelLinksWindow();

	void ShowRoadProjectManagementWindow();

	void CloseAllWindows();

	void ShutdownUIModule();

	void ShowRoadModelPlacementWindow();

	void ShowMunicipalPipelineNetworkWindow();

	void ShowCreateBridgeComponentWindow();

	void ShowDefinePileWindow();

	void ShowDefineFoundationWindow();

	void ShowDefinePierWindow();

	void ShowDefineBeamWindow();

	void ShowDefineBearingWindow();

	void ShowDefineBridgeTypeWindow();

	void ShowTopographyFromFloorWindow();

	void ShowExportFamilyMetadataWindow();

	void ShowWallToRoadWindow();

	void ShowSubgradeModelWindow();

	void ShowAncillaryStructureWindow();

	void ShowRoadSurfaceRefinementWindow();

	void ShowInsulationWindow();
}
