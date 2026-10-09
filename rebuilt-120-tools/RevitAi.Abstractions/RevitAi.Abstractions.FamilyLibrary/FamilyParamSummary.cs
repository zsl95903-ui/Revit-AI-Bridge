namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyParamSummary
{
	public string Name { get; set; } = string.Empty;

	public string ValueType { get; set; } = string.Empty;

	public string? DefaultValue { get; set; }
}
