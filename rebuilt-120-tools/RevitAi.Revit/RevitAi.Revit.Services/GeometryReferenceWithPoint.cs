using Autodesk.Revit.DB;

namespace RevitAi.Revit.Services;

internal class GeometryReferenceWithPoint
{
	public XYZ Point { get; set; } = XYZ.Zero;

	public Reference? Reference { get; set; }
}
