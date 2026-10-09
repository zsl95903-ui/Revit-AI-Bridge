using System;

namespace RevitAi.Abstractions.Update.Models;

public sealed class RemoteVersionManifest
{
	public string SchemaVersion { get; set; } = "1.0";

	public DateTime LastUpdated { get; set; }

	public LatestVersionInfo Latest { get; set; }
}
