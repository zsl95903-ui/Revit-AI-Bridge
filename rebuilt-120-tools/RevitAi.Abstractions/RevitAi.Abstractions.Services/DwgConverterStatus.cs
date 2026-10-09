using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public sealed class DwgConverterStatus
{
	public bool HasOdaConverter { get; set; }

	public string? OdaConverterPath { get; set; }

	public bool HasAutoCAD { get; set; }

	public List<string> SupportedMethods { get; set; } = new List<string>();

	public string? RecommendedMethod { get; set; }

	public string? Message { get; set; }
}
