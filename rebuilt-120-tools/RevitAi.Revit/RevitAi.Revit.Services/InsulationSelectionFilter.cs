using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;

namespace RevitAi.Revit.Services;

internal sealed class InsulationSelectionFilter : ISelectionFilter
{
	public bool AllowElement(Element elem)
	{
		if (elem is Pipe || elem is Duct)
		{
			return true;
		}
		if (elem.Category == null)
		{
			return false;
		}
		int num = (int)elem.Category.Id.Value;
		return num == -2008049 || num == -2008055 || num == -2008010 || num == -2008016;
	}

	public bool AllowReference(Reference reference, XYZ position)
	{
		return false;
	}
}
