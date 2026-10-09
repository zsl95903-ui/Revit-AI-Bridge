using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class ManholeData
{
	public string Id { get; set; } = Guid.NewGuid().ToString();

	public string BlockName { get; set; } = string.Empty;

	public double X { get; set; }

	public double Y { get; set; }

	public double Z { get; set; }

	public string LayerName { get; set; } = string.Empty;

	public string? Annotation { get; set; }

	public string? AnnotationType { get; set; }

	public List<ManholeAnnotation> Annotations { get; set; } = new List<ManholeAnnotation>();

	public List<string> ConnectedPipeIds { get; set; } = new List<string>();

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

	public string? FindAnnotationByName(string name, bool exactMatch = false)
	{
		foreach (ManholeAnnotation annotation in Annotations)
		{
			if (exactMatch)
			{
				if (annotation.Content == name)
				{
					return annotation.Content;
				}
			}
			else if (annotation.Content.Contains(name))
			{
				return annotation.Content;
			}
		}
		if (!string.IsNullOrEmpty(Annotation))
		{
			if (exactMatch)
			{
				if (Annotation == name)
				{
					return Annotation;
				}
			}
			else if (Annotation.Contains(name))
			{
				return Annotation;
			}
		}
		return null;
	}

	public string? FindAnnotationByPattern(string pattern)
	{
		try
		{
			Regex regex = new Regex(pattern);
			foreach (ManholeAnnotation annotation in Annotations)
			{
				if (regex.IsMatch(annotation.Content))
				{
					return annotation.Content;
				}
			}
			if (!string.IsNullOrEmpty(Annotation) && regex.IsMatch(Annotation))
			{
				return Annotation;
			}
		}
		catch
		{
		}
		return null;
	}

	public string GetAllAnnotationsText()
	{
		if (Annotations.Count > 0)
		{
			return string.Join("; ", Annotations.ConvertAll((ManholeAnnotation a) => a.Content));
		}
		return Annotation ?? "";
	}

	public (double X, double Y, double Z) GetPosition()
	{
		return (X: X, Y: Y, Z: Z);
	}
}
