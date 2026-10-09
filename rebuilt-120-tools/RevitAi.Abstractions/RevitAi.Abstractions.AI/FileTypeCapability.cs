namespace RevitAi.Abstractions.AI;

public sealed class FileTypeCapability
{
	public FileCapabilityType CapabilityType { get; set; }

	public long MaxFileSize { get; set; }

	public bool IsContentItem { get; set; } = true;

	public string? ApiContentType { get; set; }
}
