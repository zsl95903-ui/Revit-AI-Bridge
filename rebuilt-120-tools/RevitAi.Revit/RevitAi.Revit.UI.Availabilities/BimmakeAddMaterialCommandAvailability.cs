using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAi.Revit.UI.Availabilities;

public class BimmakeAddMaterialCommandAvailability : IExternalCommandAvailability
{
	public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
	{
		return true;
	}
}
