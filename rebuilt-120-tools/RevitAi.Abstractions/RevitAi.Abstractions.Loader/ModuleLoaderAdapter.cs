using RevitAi.Abstractions.Adapters;

namespace RevitAi.Abstractions.Loader;

public sealed class ModuleLoaderAdapter : IModuleLoader
{
	public IRevitAdapter? RevitAdapter { get; set; }

	public void ShowAboutWindow()
	{
		ModuleLoader.ShowAboutWindow();
	}

	public void ShowLoginWindow()
	{
		ModuleLoader.ShowLoginWindow();
	}

	public void ShowAIChatWindow()
	{
		ModuleLoader.ShowAIChatWindow();
	}

	public void ShowFeatureStoreWindow()
	{
		ModuleLoader.ShowFeatureStoreWindow();
	}

	public void ShowBatchLinkModelsWindow()
	{
		ModuleLoader.ShowBatchLinkModelsWindow();
	}

	public void ShowManageModelLinksWindow()
	{
		ModuleLoader.ShowManageModelLinksWindow();
	}

	public void ShowRoadProjectManagementWindow()
	{
		ModuleLoader.ShowRoadProjectManagementWindow();
	}

	public void CloseAllWindows()
	{
		ModuleLoader.CloseAllWindows();
	}

	public void ShutdownUIModule()
	{
		ModuleLoader.ShutdownAsync().GetAwaiter().GetResult();
	}

	public void ShowRoadModelPlacementWindow()
	{
		ModuleLoader.ShowRoadModelPlacementWindow();
	}

	public void ShowMunicipalPipelineNetworkWindow()
	{
		ModuleLoader.ShowMunicipalPipelineNetworkWindow();
	}

	public void ShowCreateBridgeComponentWindow()
	{
		ModuleLoader.ShowCreateBridgeComponentWindow();
	}

	public void ShowDefinePileWindow()
	{
		ModuleLoader.ShowDefinePileWindow();
	}

	public void ShowDefineFoundationWindow()
	{
		ModuleLoader.ShowDefineFoundationWindow();
	}

	public void ShowDefinePierWindow()
	{
		ModuleLoader.ShowDefinePierWindow();
	}

	public void ShowDefineBeamWindow()
	{
		ModuleLoader.ShowDefineBeamWindow();
	}

	public void ShowDefineBearingWindow()
	{
		ModuleLoader.ShowDefineBearingWindow();
	}

	public void ShowDefineBridgeTypeWindow()
	{
		ModuleLoader.ShowDefineBridgeTypeWindow();
	}

	public void ShowTopographyFromFloorWindow()
	{
		ModuleLoader.ShowTopographyFromFloorWindow();
	}

	public void ShowExportFamilyMetadataWindow()
	{
		ModuleLoader.ShowExportFamilyMetadataWindow();
	}

	public void ShowWallToRoadWindow()
	{
		ModuleLoader.ShowWallToRoadWindow();
	}

	public void ShowSubgradeModelWindow()
	{
		ModuleLoader.ShowSubgradeModelWindow();
	}

	public void ShowAncillaryStructureWindow()
	{
		ModuleLoader.ShowAncillaryStructureWindow();
	}

	public void ShowPaymentDialog(string message, string commandText)
	{
		ModuleLoader.ShowPaymentDialog(message, commandText);
	}

	public void ShowRoadSurfaceRefinementWindow()
	{
		ModuleLoader.ShowRoadSurfaceRefinementWindow();
	}

	public void ShowInsulationWindow()
	{
		ModuleLoader.ShowInsulationWindow();
	}
}
