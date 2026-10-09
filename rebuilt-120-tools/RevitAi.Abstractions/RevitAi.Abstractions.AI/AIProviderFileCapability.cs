using System.Collections.Generic;

namespace RevitAi.Abstractions.AI;

public sealed class AIProviderFileCapability
{
	public Dictionary<FileAttachmentType, FileTypeCapability> SupportedTypes { get; set; } = new Dictionary<FileAttachmentType, FileTypeCapability>();

	public long DefaultMaxFileSize { get; set; } = 20971520L;

	public int MaxFileCount { get; set; } = 5;

	public bool SupportsMultimodal { get; set; } = true;

	public ContentFormatType ContentFormat { get; set; }

	public bool IsSupported(FileAttachmentType fileType)
	{
		if (!SupportedTypes.TryGetValue(fileType, out FileTypeCapability value))
		{
			return false;
		}
		return value.CapabilityType != FileCapabilityType.None;
	}

	public FileTypeCapability GetCapability(FileAttachmentType fileType)
	{
		if (SupportedTypes.TryGetValue(fileType, out FileTypeCapability value))
		{
			return value;
		}
		return new FileTypeCapability
		{
			CapabilityType = FileCapabilityType.None
		};
	}

	public long GetMaxFileSize(FileAttachmentType fileType)
	{
		if (SupportedTypes.TryGetValue(fileType, out FileTypeCapability value) && value.MaxFileSize > 0)
		{
			return value.MaxFileSize;
		}
		return DefaultMaxFileSize;
	}
}
