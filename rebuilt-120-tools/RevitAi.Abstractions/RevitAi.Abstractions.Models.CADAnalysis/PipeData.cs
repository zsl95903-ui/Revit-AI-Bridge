using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class PipeData
{
	public string Id { get; set; } = Guid.NewGuid().ToString();

	public double StartX { get; set; }

	public double StartY { get; set; }

	public double StartZ { get; set; }

	public double EndX { get; set; }

	public double EndY { get; set; }

	public double EndZ { get; set; }

	public string LayerName { get; set; } = string.Empty;

	public string LineType { get; set; } = string.Empty;

	public double Length { get; set; }

	public string? Annotation { get; set; }

	public string? AnnotationType { get; set; }

	public double? DiameterMM { get; set; }

	public List<PipeAnnotation> Annotations { get; set; } = new List<PipeAnnotation>();

	public string? StartManholeId { get; set; }

	public string? EndManholeId { get; set; }

	public bool HasAnnotation
	{
		get
		{
			if (Annotations.Count <= 0)
			{
				return !string.IsNullOrEmpty(Annotation);
			}
			return true;
		}
	}

	public bool HasDiameter
	{
		get
		{
			if (DiameterMM.HasValue)
			{
				return DiameterMM.Value > 0.0;
			}
			return false;
		}
	}

	public bool IsConnectedToManholes
	{
		get
		{
			if (StartManholeId == null)
			{
				return EndManholeId != null;
			}
			return true;
		}
	}

	public static double? ExtractDiameterFromAnnotation(string? annotation)
	{
		if (string.IsNullOrWhiteSpace(annotation))
		{
			return null;
		}
		string[] array = new string[4] { "(?:DN|dn|Dn)\\s*(\\d+(?:\\.\\d+)?)", "[Dd]\\s*(\\d+(?:\\.\\d+)?)", "[φΦ]\\s*(\\d+(?:\\.\\d+)?)", "直径\\s*[:：]?\\s*(\\d+(?:\\.\\d+)?)" };
		foreach (string pattern in array)
		{
			Match match = Regex.Match(annotation, pattern, RegexOptions.IgnoreCase);
			if (match.Success && double.TryParse(match.Groups[1].Value, out var result))
			{
				return result;
			}
		}
		return null;
	}

	public void ExtractDiameterFromAnnotations()
	{
		foreach (PipeAnnotation annotation in Annotations)
		{
			double? num = ExtractDiameterFromAnnotation(annotation.Content);
			if (num.HasValue)
			{
				DiameterMM = num.Value;
				return;
			}
		}
		if (!string.IsNullOrEmpty(Annotation))
		{
			DiameterMM = ExtractDiameterFromAnnotation(Annotation);
		}
	}

	public string GetAllAnnotationsText()
	{
		if (Annotations.Count > 0)
		{
			return string.Join("; ", Annotations.ConvertAll((PipeAnnotation a) => a.Content));
		}
		return Annotation ?? "";
	}

	public (double X, double Y, double Z) GetStartPoint()
	{
		return (X: StartX, Y: StartY, Z: StartZ);
	}

	public (double X, double Y, double Z) GetEndPoint()
	{
		return (X: EndX, Y: EndY, Z: EndZ);
	}
}
