namespace RevitAi.Abstractions.Services;

public sealed class CADLayerInfo
{
	public string Name { get; set; } = string.Empty;

	public int? ColorIndex { get; set; }

	public bool IsVisible { get; set; } = true;

	public bool IsLocked { get; set; }

	public string? LineTypeName { get; set; }

	public double? LineWeight { get; set; }
}
