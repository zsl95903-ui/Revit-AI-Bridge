using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAi.Revit.UI.Availabilities;

public class RoadProjectManagementCommandAvailability : IExternalCommandAvailability
{
	public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
	{
		return true;
	}
}
