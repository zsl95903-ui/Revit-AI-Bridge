using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAi.Revit.UI.Availabilities;

public class ExportFamilyMetadataCommandAvailability : IExternalCommandAvailability
{
	public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
	{
		return true;
	}
}
