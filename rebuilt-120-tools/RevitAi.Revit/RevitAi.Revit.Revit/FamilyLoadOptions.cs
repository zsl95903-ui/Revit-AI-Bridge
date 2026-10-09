using Autodesk.Revit.DB;

namespace RevitAi.Revit.Revit;

public class FamilyLoadOptions : IFamilyLoadOptions
{
	public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
	{
		overwriteParameterValues = true;
		return true;
	}

	public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
	{
		source = (FamilySource)0;
		overwriteParameterValues = true;
		return true;
	}
}
