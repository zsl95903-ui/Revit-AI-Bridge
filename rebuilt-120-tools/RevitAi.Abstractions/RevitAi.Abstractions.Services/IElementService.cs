using System.Collections.Generic;
using RevitAi.Abstractions.Collision;
using RevitAi.Abstractions.Models;

namespace RevitAi.Abstractions.Services;

public interface IElementService
{
	object? GetElementById(object document, int elementId);

	object? GetElementByUniqueId(object document, string uniqueId);

	IEnumerable<object> GetAllElements(object document);

	IEnumerable<object> GetElementsByCategory(object document, string categoryName);

	IEnumerable<object> GetElementsByType(object document, string typeName);

	bool DeleteElement(object document, object element);

	object? CopyElement(object document, object element);

	bool MoveElement(object document, object element, double x, double y, double z);

	bool RotateElement(object document, object element, double axisX, double axisY, double axisZ, double angleDegrees);

	(double X, double Y, double Z)? GetElementLocation(object element);

	GridCurveInfo? GetGridCurveInfo(object element);

	string? GetElementCategory(object element);

	string? GetElementName(object element);

	int? GetElementId(object element);

	object? CreateStraightWall(object document, double startX, double startY, double endX, double endY, int levelId, double height, int? wallTypeId = null);

	object? CreateDoorInWall(object document, int wallId, double positionX, double positionY, int? doorTypeId = null, bool flip = false);

	object? CreateWindowInWall(object document, int wallId, double positionX, double positionY, int? windowTypeId = null, double heightOffset = 4.0);

	object? CreateColumn(object document, double positionX, double positionY, int levelId, double height = 0.0, int? columnTypeId = null);

	object? CreateBeam(object document, double startX, double startY, double endX, double endY, int levelId, int? beamTypeId = null);

	object? CreatePipe(object document, double startX, double startY, double startZ, double endX, double endY, double endZ, int levelId, int pipeTypeId, int? systemTypeId = null, double diameterMM = 100.0);

	IEnumerable<object> GetDuctTypes(object document);

	IEnumerable<object> GetDuctSystemTypes(object document);

	object? CreateDuct(object document, double startX, double startY, double startZ, double endX, double endY, double endZ, int levelId, int ductTypeId, int? systemTypeId = null, double widthMM = 300.0, double heightMM = 300.0, double diameterMM = 0.0);

	object? CreateFloorByProfile(object document, IEnumerable<(double X, double Y)> points, int levelId, int? floorTypeId = null);

	IEnumerable<object> GetPipeTypes(object document);

	IEnumerable<object> GetPipingSystemTypes(object document);

	IEnumerable<object> GetCableTrayTypes(object document);

	object? CreateCableTray(object document, double startX, double startY, double startZ, double endX, double endY, double endZ, int levelId, int cableTrayTypeId, double widthMM = 300.0, double heightMM = 150.0);

	int DeleteElements(object document, IEnumerable<int> elementIds);

	int CountElementsByCategory(object document, string categoryName);

	IEnumerable<(string Name, object? Value, string Type)> GetAllParameters(object element);

	object? GetParameterValue(object element, string parameterName);

	bool SetParameterValue(object element, string parameterName, object value, object document);

	string? GetParameterFormula(object element, string parameterName);

	string? GetElementTypeName(object element);

	bool IsElementType(object element);

	int? GetElementTypeId(object element);

	double? GetElementArea(object element);

	double? GetElementVolume(object element);

	(string? CreatedPhase, string? DemolishedPhase)? GetElementPhases(object element);

	IEnumerable<object> GetWorksets(object document, bool userCreatedOnly = false);

	object? GetWorksetByName(object document, string worksetName);

	int GetWorksetId(object workset);

	string? GetWorksetName(object workset);

	bool IsWorksetVisible(object workset);

	bool IsWorksetOpen(object workset);

	bool IsDefaultWorkset(object workset);

	object? GetElementWorkset(object element);

	bool SetElementWorkset(object element, object workset, object document);

	object? CreateWorkset(object document, string worksetName);

	int? GetMaterialIdByName(object document, string materialName);

	int? DuplicateView(object document, object view, string duplicateOption);

	void SetViewLevel(object view, int levelId);

	void SetViewName(object view, string name);

	object? GetElementGeometry(object document, object element);

	double GetElementArea(object document, object element);

	string? GetCadElementLayerName(object element);

	int ColorElements(object document, IEnumerable<int> elementIds, (byte Red, byte Green, byte Blue)? lineColorRGB = null, (byte Red, byte Green, byte Blue)? fillPatternRGB = null, (byte Red, byte Green, byte Blue)? cutFillColorRGB = null);

	int ResetElementColors(object document, IEnumerable<int> elementIds);

	int ResetAllElementColors(object document);

	bool EnsureSharedParameterFile(object application);

	IEnumerable<object> GetAllSharedParameters(object document);

	object? GetSharedParameter(object document, string parameterName);

	object? CreateSharedParameter(object document, string parameterName, string parameterType, string groupName, IEnumerable<string> categoryNames, bool instanceParameter = true);

	bool DeleteSharedParameter(object document, string parameterName);

	int RemoveSharedParameterBindings(object document, string parameterName, IEnumerable<string> categoryNames);

	int GetSharedParameterUsageCount(object document, string parameterName);

	IEnumerable<object> GetAllGlobalParameters(object document);

	object? GetGlobalParameter(object document, string parameterName);

	object? CreateGlobalParameter(object document, string parameterName, string parameterType, string? formula = null, object? value = null);

	bool SetGlobalParameterValue(object document, string parameterName, object value);

	bool DeleteGlobalParameter(object document, string parameterName);

	IEnumerable<object> GetAllProjectParameters(object document);

	object? GetProjectParameter(object document, string parameterName);

	bool DeleteProjectParameter(object document, string parameterName);

	object? CreateProjectParameterBinding(object document, string sharedParameterName, string[] categoryNames, string bindingType, string? parameterGroup = null);

	object? UpdateProjectParameterBinding(object document, string parameterName, string[] categoriesToAdd, string[] categoriesToRemove);

	IEnumerable<object> GetAllParameterGroups(object application);

	IEnumerable<CompoundLayerInfo>? GetCompoundStructureLayers(object elementType);

	double? GetCompoundStructureTotalThickness(object elementType);

	bool ModifyCompoundStructureLayer(object elementType, int layerIndex, object document, double? thicknessMM = null, int? materialId = null, string? function = null);

	int AddCompoundStructureLayer(object elementType, object document, double thicknessMM, string function, int? materialId = null, int? insertIndex = null);

	bool DeleteCompoundStructureLayer(object elementType, int layerIndex, object document);

	bool LockElement(object document, int elementId);

	bool UnlockElement(object document, int elementId);

	IEnumerable<object> GetRoofTypes(object document);

	object? CreateFootPrintRoof(object document, IEnumerable<(double X, double Y)> points, int levelId, int? roofTypeId = null);

	object? CreateExtrusionRoof(object document, IEnumerable<(double X, double Y)> points, int levelId, int? roofTypeId = null, double extrusionStart = 0.0, double extrusionEnd = 30.0);

	bool SetRoofEdgeSlope(object document, object roofElement, int edgeIndex, bool definesSlope, double slopeAngle);

	IEnumerable<(int EdgeIndex, bool DefinesSlope, double SlopeAngle)>? GetFootPrintRoofEdges(object roofElement);

	bool CheckCollision(object document, int sourceElementId, int targetElementId);

	IEnumerable<CollisionResultItem> CheckCollisions(object document, IEnumerable<(int SourceId, int TargetId)> elementPairs);

	IEnumerable<CollisionResultItem> CheckCollisionsBatch(object document, IEnumerable<int> sourceIds, IEnumerable<int> targetIds);

	IEnumerable<InsulationTypeInfo> GetPipeInsulationTypes(object document);

	IEnumerable<InsulationTypeInfo> GetDuctInsulationTypes(object document);

	IEnumerable<InsulationSystemInfo> GetPipeInsulationSystemInfos(object document, int? systemTypeId = null);

	IEnumerable<InsulationSystemInfo> GetDuctInsulationSystemInfos(object document, int? systemTypeId = null);

	InsulationOperationResult? AddPipeInsulation(object document, int? systemTypeId, int insulationTypeId, double thicknessMM, IList<int>? elementIds = null, bool overrideExisting = true);

	InsulationOperationResult? AddDuctInsulation(object document, int? systemTypeId, int insulationTypeId, double thicknessMM, IList<int>? elementIds = null, bool overrideExisting = true);

	InsulationOperationResult? RemovePipeInsulation(object document, int? systemTypeId = null, IList<int>? elementIds = null);

	InsulationOperationResult? RemoveDuctInsulation(object document, int? systemTypeId = null, IList<int>? elementIds = null);

	InsulationOperationResult? ModifyPipeInsulation(object document, int? systemTypeId = null, int? newInsulationTypeId = null, double? newThicknessMM = null, IList<int>? elementIds = null);

	InsulationOperationResult? ModifyDuctInsulation(object document, int? systemTypeId = null, int? newInsulationTypeId = null, double? newThicknessMM = null, IList<int>? elementIds = null);
}
