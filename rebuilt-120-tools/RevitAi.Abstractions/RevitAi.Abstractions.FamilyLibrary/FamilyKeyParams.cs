namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyKeyParams
{
	public int TypeParamCount { get; set; }

	public int InstanceParamCount { get; set; }

	public FamilyParamSummary[]? CommonTypeParams { get; set; }
}
