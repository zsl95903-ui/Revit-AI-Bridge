using System;

namespace RevitAi.Abstractions.Update.Models;

public sealed class UpdateInfo
{
	public string Version { get; set; } = string.Empty;

	public string? CurrentVersion { get; set; }

	public string DownloadUrl { get; set; } = string.Empty;

	public string Checksum { get; set; } = string.Empty;

	public long PackageSize { get; set; }

	public bool IsForceUpdate { get; set; }

	public string ReleaseNotes { get; set; } = string.Empty;

	public DateTime ReleaseDate { get; set; }
}
