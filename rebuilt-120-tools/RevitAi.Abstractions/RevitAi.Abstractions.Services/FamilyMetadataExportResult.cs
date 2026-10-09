namespace RevitAi.Abstractions.Services;

public class FamilyMetadataExportResult
{
	public bool IsSuccess { get; set; }

	public string? ErrorMessage { get; set; }

	public string? ThumbnailPath { get; set; }

	public string? MetadataPath { get; set; }
}
