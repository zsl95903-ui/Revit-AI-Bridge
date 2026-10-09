using System.Collections.Generic;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Revit.FloorTopography;

namespace RevitAi.Abstractions.Services;

public interface ITopographyService
{
	object? CreateTopographyFromFloorProfile(object document, object floorElement, object topographyElement);

	(int SuccessCount, List<string> Errors, List<object> CreatedTopographies) CreateTopographiesFromFloorProfiles(object document, List<object> floorElements, object topographyElement);

	(int SuccessCount, List<string> Errors, List<object> CreatedTopographies) CreateTopographiesFromFloorProfiles(object document, List<object> floorElements, object topographyElement, IProgressReporter? progressReporter);

	Result<FloorTopographyResult> CreateTopographiesFromFloorProfiles(object document, FloorTopographyRequest request);

	Result<FloorTopographyResult> CreateTopographiesFromFloorProfilesWithTransaction(object document, FloorTopographyRequest request, object externalTransaction);

	object? CreateTopographySurface(object document, IList<object> points);

	int AddPointsToTopography(object document, object topographyElement, IList<object> newPoints);

	int ModifyTopographyPoints(object document, object topographyElement, IList<object> targetPoints, IList<object> newPoints);

	int DeleteTopographyPoints(object document, object topographyElement, IList<object> pointsToDelete);
}
