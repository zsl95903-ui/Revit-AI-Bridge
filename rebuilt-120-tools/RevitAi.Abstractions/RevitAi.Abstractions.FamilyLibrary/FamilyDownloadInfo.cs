using System;

namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyDownloadInfo
{
	public bool Success { get; set; }

	public bool CanDownload { get; set; }

	public bool IsFree { get; set; }

	public Guid? FamilyId { get; set; }

	public string DisplayName { get; set; } = string.Empty;

	public string FileUrl { get; set; } = string.Empty;

	public long? FileSize { get; set; }

	public string? ThumbnailUrl { get; set; }

	public DownloadQuotaInfo? QuotaInfo { get; set; }

	public string? Error { get; set; }

	public string? Message { get; set; }
}
