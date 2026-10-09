using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IModificationService
{
	IEnumerable<object> ArrayElement(object document, object element, int numberOfMembers, double moveX = 0.0, double moveY = 0.0, double moveZ = 0.0);

	IEnumerable<object> ArrayElementLinear(object document, object element, int numberOfMembers, double moveX, double moveY, double moveZ);

	IEnumerable<object> ArrayElementRadial(object document, object element, int numberOfMembers, double axisX, double axisY, double axisZ, double originX, double originY, double originZ);

	object? MirrorElement(object document, object element, double planeNormalX, double planeNormalY, double planeNormalZ, double planeOriginX, double planeOriginY, double planeOriginZ);

	object? MirrorElementByLine(object document, object element, object line);

	object? MirrorElementByPoint(object document, object element, object origin, object normal);

	bool TrimExtendElement(object document, object element, string operation, object? targetElement = null);

	SplitResult SplitElementAdvanced(object document, object element, string splitMode, IEnumerable<(double X, double Y, double Z)>? splitPoints = null, double splitLength = 0.0, IEnumerable<double>? splitParameters = null, bool autoCreateFittings = true);

	bool JoinElements(object document, object element1, object element2, string joinType = "Join");

	object? GroupElements(object document, IEnumerable<int> elementIds);

	int MoveElements(object document, IEnumerable<int> elementIds, double moveX, double moveY, double moveZ);

	int RotateElements(object document, IEnumerable<int> elementIds, double axisX, double axisY, double axisZ, double angleDegrees, double originX = 0.0, double originY = 0.0, double originZ = 0.0);

	MirrorResult MirrorElements(object document, IEnumerable<int> elementIds, double planeOriginX, double planeOriginY, double planeOriginZ, double planeNormalX, double planeNormalY, double planeNormalZ, bool mirrorCopies = false);

	IEnumerable<object> CopyElements(object document, IEnumerable<int> elementIds, double copyX, double copyY, double copyZ);

	int DeleteElements(object document, IEnumerable<int> elementIds);

	int ChangeElementTypes(object document, IEnumerable<int> elementIds, int newTypeId);

	object? GroupElements(object document, IEnumerable<int> elementIds, string? groupName = null);

	IEnumerable<int> UngroupElements(object document, int groupId);

	IEnumerable<object> DivideRoom(object document, object room, object startPoint, object endPoint, object? transaction = null);

	object? MergeRooms(object document, IEnumerable<object> rooms, object? transaction = null);

	int PurgeUnusedFamilies(object document);

	int PurgeUnusedMaterials(object document);

	int PurgeUnusedMaterialAssets(object document);

	int PurgeUnusedViews(object document);

	int PurgeUnusedViewTemplates(object document);

	int PurgeUnusedLinePatterns(object document);

	int PurgeUnusedFillPatterns(object document);

	int PurgeUnusedAnnotationStyles(object document);

	int PurgeUnusedModelGroups(object document);

	int PurgeUnusedDetailGroups(object document);

	int PurgeUnusedViewFilters(object document);

	bool CutGeometry(object document, object elementToCut, object cuttingElement);

	bool UncutGeometry(object document, object elementToUncut, object cuttingElement);

	bool JoinGeometry(object document, object element1, object element2);

	bool UnjoinGeometry(object document, object element1, object element2);

	bool SwitchJoinOrder(object document, object element1, object element2);

	bool AreElementsJoined(object element1, object element2);

	string? GetJoinOrder(object element1, object element2);

	bool IsCuttingElementInJoin(object element1, object element2);
}
