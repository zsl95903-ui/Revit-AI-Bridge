namespace RevitAi.Abstractions.Services;

public sealed class ViewExportConfig
{
	public string FilePath { get; set; } = string.Empty;

	public int Width { get; set; } = 1920;

	public int Height { get; set; } = 1080;

	public string ExportRange { get; set; } = "Viewport";

	public double? RegionLeft { get; set; }

	public double? RegionTop { get; set; }

	public double? RegionRight { get; set; }

	public double? RegionBottom { get; set; }

	public int Quality { get; set; } = 100;

	public bool ExportOnlyVisible { get; set; } = true;

	public bool IncludeBackground { get; set; } = true;
}
