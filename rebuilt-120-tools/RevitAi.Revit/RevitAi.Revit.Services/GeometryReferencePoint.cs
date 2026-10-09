using Autodesk.Revit.DB;

namespace RevitAi.Revit.Services;

internal class GeometryReferencePoint
{
	public XYZ? Position { get; set; }

	public Reference? Reference { get; set; }

	public double ProjectionOnDimensionLine { get; set; }

	public ElementId? ElementId { get; set; }
}
