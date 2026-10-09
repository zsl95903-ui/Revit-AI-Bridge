using Autodesk.Revit.DB;

namespace RevitAi.Revit.Services;

public class CurveWithWidth
{
	public Curve Curve { get; set; }

	public double Width { get; set; }

	public bool NeedReverse { get; set; }
}
