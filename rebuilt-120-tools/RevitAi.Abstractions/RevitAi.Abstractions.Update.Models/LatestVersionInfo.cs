using System;

namespace RevitAi.Abstractions.Update.Models;

public sealed class LatestVersionInfo
{
	public string Version { get; set; } = string.Empty;

	public DateTime ReleaseDate { get; set; }

	public bool IsForceUpdate { get; set; }

	public string DownloadUrl { get; set; } = string.Empty;

	public long PackageSize { get; set; }

	public string Checksum { get; set; } = string.Empty;

	public string ReleaseNotes { get; set; } = string.Empty;

	public string? SkillsVersion { get; set; }

	public string? SkillsUrl { get; set; }

	public string? SkillsChecksum { get; set; }
}
