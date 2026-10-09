using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using ns6;

namespace RevitAi.Revit.Infrastructure;

public class CADImportSelectionFilter : ISelectionFilter
{
	private readonly string string_0;

	public CADImportSelectionFilter(string objectTypeStr)
	{
		string_0 = objectTypeStr;
	}

	public bool AllowElement(Element elem)
	{
		return elem is ImportInstance;
	}

	public bool AllowReference(Reference reference, XYZ position)
	{
		if (string_0 == "Element")
		{
			return true;
		}
		return true;
	}
}
