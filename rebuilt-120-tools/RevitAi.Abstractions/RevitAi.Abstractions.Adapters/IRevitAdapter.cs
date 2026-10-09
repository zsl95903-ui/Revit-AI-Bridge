using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Services;
using RevitAi.Abstractions.UI;
using RevitAi.Abstractions.Units;

namespace RevitAi.Abstractions.Adapters;

public interface IRevitAdapter
{
	RevitVersion Version { get; }

	IDocumentService? DocumentService { get; }

	IElementService? ElementService { get; }

	IParameterService? ParameterService { get; }

	ISelectionService? SelectionService { get; }

	IGeometryService? GeometryService { get; }

	IApplicationService? ApplicationService { get; }

	IViewService? ViewService { get; }

	IViewExportService? ViewExportService { get; }

	IDwgExportService? DwgExportService { get; }

	ILevelService? LevelService { get; }

	IAnnotationService? AnnotationService { get; }

	IModificationService? ModificationService { get; }

	IMaterialService? MaterialService { get; }

	IAnalysisService? AnalysisService { get; }

	ILinkService? LinkService { get; }

	IFamilyService? FamilyService { get; }

	IFamilyMetadataService? FamilyMetadataService { get; }

	IPhaseService? PhaseService { get; }

	ICreationService? CreationService { get; }

	IInfrastructureService? InfrastructureService { get; }

	ICADGeometryService? CADGeometryService { get; }

	ICADFileService? CADFileService { get; }

	IAuthManager? AuthManager { get; set; }

	IInsulationService? InsulationService { get; }

	bool InitializeForUI(object application);

	bool InitializeForCommand(object commandData);

	IUIApplication CreateUIApplication(object application);

	bool Shutdown();

	IFamilyLoadService? GetFamilyLoadService();

	IExcelDataService? GetExcelDataService();

	IFileAttachmentService? GetFileAttachmentService();

	IPipeNetworkModelingService? GetPipeNetworkModelingService();

	IPipeCreationService? GetPipeCreationService();

	object? GetPipeNetworkModelingExternalEvent();

	object? GetFamilyLoadExternalEvent();

	object? GetFamilyCheckExternalEvent();

	object? GetRoadModelingExternalEvent();

	IWallToRoadService? GetWallToRoadService();

	ITopographyService? GetTopographyService();

	object? GetStairCreationService();

	object? GetStairTypeManagerService();

	Dictionary<UnitType, ProjectUnitInfo> GetProjectUnits(object document);

	Task<bool> SetProjectUnitAsync(object document, UnitType unitType, object displayUnitType);

	object? GetActiveDocument();

	void LogDebug(string message);

	void LogInfo(string message);

	void LogWarning(string message);

	void LogError(string message, Exception? ex = null);
}
