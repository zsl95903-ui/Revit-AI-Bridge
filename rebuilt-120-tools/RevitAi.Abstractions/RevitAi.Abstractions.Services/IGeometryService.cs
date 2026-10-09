using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IGeometryService
{
	double? GetWallTopLevel(object wall);

	double? GetFloorBottomLevel(object floor);

	bool IsWallUnderFloor(object wall, object floor);

	bool ModifyWallTopLevel(object document, object wall, double newLevel);

	double? GetDistanceBetweenElements(object element1, object element2);

	double GetDistanceBetweenPoints(object point1, object point2);

	((double MinX, double MinY, double MinZ)? Min, (double MaxX, double MaxY, double MaxZ)? Max)? GetBoundingBox(object element);

	object? GetElementGeometry(object element, object document);

	IList<IList<IDictionary<string, double>>>? GetRoomBoundaries(object document, object room);
}
