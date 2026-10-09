using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IAnalysisService
{
	double? GetElementArea(object element);

	double? GetElementArea(object document, object element, string areaType = "surface");

	double? GetElementVolume(object element);

	double? GetElementVolume(object document, object element);

	double? GetElementLength(object element);

	(bool HasCollision, IEnumerable<(double X, double Y, double Z)> CollisionPoints) CheckCollision(object document, object element1, object element2);

	object? GetElementGeometry(object element);

	double? GetDistanceBetweenElements(object element1, object element2);

	((double MinX, double MinY, double MinZ)? Min, (double MaxX, double MaxY, double MaxZ)? Max)? GetBoundingBox(object element);

	IEnumerable<(int ElementId1, int ElementId2, IEnumerable<(double X, double Y, double Z)> CollisionPoints)> CheckCollisions(object document, IEnumerable<int> elementIds);

	IEnumerable<(int DocumentIndex1, int ElementId1, int DocumentIndex2, int ElementId2, IEnumerable<(double X, double Y, double Z)> CollisionPoints)> CheckCollisionsCrossDocument(IList<object> documents, IDictionary<int, IEnumerable<int>> elementIdsByDocument);

	IEnumerable<(int GroupId1, int ElementId1, int GroupId2, int ElementId2, IEnumerable<(double X, double Y, double Z)> CollisionPoints)> CheckCollisionsWithGroups(object document, IDictionary<int, IEnumerable<int>> elementsByGroup);

	IDictionary<int, double> CalculateQuantities(object document, IEnumerable<int> elementIds, string quantityType);
}
