using Autodesk.Revit.DB;

namespace RevitAi.Revit.Models;

public interface IStairCreationService
{
	ElementId CreateStair(StairCreationParameters parameters, ElementId bottomLevelId, ElementId? topLevelId = null);
}
