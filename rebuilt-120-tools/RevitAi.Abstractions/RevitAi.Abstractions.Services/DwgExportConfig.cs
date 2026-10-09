namespace RevitAi.Abstractions.Services;

public class DwgExportConfig
{
	public string FilePath { get; set; } = string.Empty;

	public string? ExportSettingName { get; set; }

	public bool ExportAs3D { get; set; }

	public bool SharedLevels { get; set; } = true;

	public string? VersionDescription { get; set; }

	public bool ExportAsDXF { get; set; }
}
