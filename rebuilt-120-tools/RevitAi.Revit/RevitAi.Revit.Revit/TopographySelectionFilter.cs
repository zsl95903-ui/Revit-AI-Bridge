using RevitAi.Abstractions.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RevitAi.Revit.Revit;

public class TopographySelectionFilter : ISelectionFilter
{
	private readonly OperationMode operationMode_0;

	public TopographySelectionFilter(OperationMode mode)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		operationMode_0 = mode;
	}

	public bool AllowElement(Element elem)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		OperationMode val = operationMode_0;
		OperationMode val2 = val;
		if ((int)val2 != 0)
		{
			if ((int)val2 != 1)
			{
				return false;
			}
			return elem is Toposolid;
		}
		return elem is Floor;
	}

	public bool AllowReference(Reference reference, XYZ position)
	{
		return false;
	}
}
