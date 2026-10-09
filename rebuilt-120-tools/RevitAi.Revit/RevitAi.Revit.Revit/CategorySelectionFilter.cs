using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RevitAi.Revit.Revit;

public class CategorySelectionFilter : ISelectionFilter
{
	private readonly BuiltInCategory builtInCategory_0;

	public CategorySelectionFilter(BuiltInCategory category)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		builtInCategory_0 = category;
	}

	public bool AllowElement(Element elem)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return elem.Category != null && elem.Category.Id.Value == (int)builtInCategory_0;
	}

	public bool AllowReference(Reference reference, XYZ position)
	{
		return false;
	}
}
