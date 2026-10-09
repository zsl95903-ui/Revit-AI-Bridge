using System;

namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyDownloadHistoryItem
{
	public Guid Id { get; set; }

	public Guid FamilyId { get; set; }

	public string? FamilyName { get; set; }

	public string? ThumbnailUrl { get; set; }

	public DateTime DownloadDate { get; set; }

	public DateTime DownloadedAt { get; set; }
}
