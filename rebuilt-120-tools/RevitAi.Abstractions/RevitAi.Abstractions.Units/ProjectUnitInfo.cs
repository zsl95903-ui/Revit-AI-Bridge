namespace RevitAi.Abstractions.Units;

public sealed class ProjectUnitInfo
{
	public UnitType UnitType { get; set; }

	public string DisplayUnitSymbol { get; set; } = string.Empty;

	public object? DisplayUnitType { get; set; }

	public bool IsMetric { get; set; }

	public double FactorToMillimeters { get; set; } = 1.0;

	public double FactorToSquareMeters { get; set; } = 1.0;

	public double FactorToCubicMeters { get; set; } = 1.0;

	public int Precision { get; set; } = 2;

	public string FormatString { get; set; } = "0.00";

	public string DisplayName { get; set; } = string.Empty;
}
