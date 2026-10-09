namespace RevitAi.Abstractions.Revit;

public class FamilyExportItem
{
	public object Family { get; set; }

	public string FamilyName { get; set; } = string.Empty;

	public bool IsExternalFamily { get; set; }

	public string? FilePath { get; set; }
}
