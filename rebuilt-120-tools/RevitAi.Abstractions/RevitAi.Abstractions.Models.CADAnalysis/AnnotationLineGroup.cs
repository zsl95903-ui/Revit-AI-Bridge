using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Services;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class AnnotationLineGroup
{
	public string Id { get; set; } = Guid.NewGuid().ToString();

	public List<CADLineInfo> Lines { get; set; } = new List<CADLineInfo>();

	public List<CADTextInfo> AssociatedTexts { get; set; } = new List<CADTextInfo>();

	public double PrimaryDirectionAngle { get; set; }

	public double TotalLength { get; set; }

	public (double X, double Y, double Z) StartPoint { get; set; }

	public (double X, double Y, double Z) EndPoint { get; set; }

	public bool HasAssociatedTexts => AssociatedTexts.Count > 0;

	public (double X, double Y, double Z) GetFirstEndpoint()
	{
		return StartPoint;
	}

	public (double X, double Y, double Z) GetLastEndpoint()
	{
		return EndPoint;
	}

	public List<(double X, double Y, double Z)> GetAllEndpoints()
	{
		List<(double, double, double)> list = new List<(double, double, double)>();
		if (Lines.Count == 0)
		{
			return list;
		}
		list.Add((Lines[0].StartX, Lines[0].StartY, Lines[0].StartZ));
		foreach (CADLineInfo line in Lines)
		{
			list.Add((line.EndX, line.EndY, line.EndZ));
		}
		return list;
	}

	public string GetMergedTextContent()
	{
		if (AssociatedTexts.Count == 0)
		{
			return string.Empty;
		}
		if (AssociatedTexts.Count == 1)
		{
			return AssociatedTexts[0].Content;
		}
		return string.Join("; ", AssociatedTexts.ConvertAll((CADTextInfo t) => t.Content));
	}
}
