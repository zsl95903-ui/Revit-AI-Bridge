using System;

namespace RevitAi.UI.Services;

public class FamilyCacheStats
{
	public int ThumbnailCount { get; set; }

	public long ThumbnailSize { get; set; }

	public int FamilyFileCount { get; set; }

	public long FamilyFileSize { get; set; }

	public int MetadataCount { get; set; }

	public DateTime LastCleanup { get; set; }
}
