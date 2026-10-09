using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitAi.Revit.Services;

public class RouteChain
{
	public List<Curve> Curves { get; set; } = new List<Curve>();

	public double Width { get; set; }

	public List<double> CurveWidths { get; set; } = new List<double>();

	public List<XYZ> GetPoints()
	{
		List<XYZ> list = new List<XYZ>();
		if (Curves.Count == 0)
		{
			return list;
		}
		list.Add(Curves[0].GetEndPoint(0));
		foreach (Curve curf in Curves)
		{
			list.Add(curf.GetEndPoint(1));
		}
		return list;
	}
}
