namespace RevitAi.Abstractions.Adapters;

public sealed class RevitVersion
{
	public int VersionYear { get; }

	public string VersionString { get; }

	public string BuildNumber { get; }

	public string DisplayVersion { get; }

	public RevitVersion(int versionYear, string versionString, string buildNumber, string displayVersion)
	{
		VersionYear = versionYear;
		VersionString = versionString ?? string.Empty;
		BuildNumber = buildNumber ?? string.Empty;
		DisplayVersion = displayVersion ?? string.Empty;
	}
}
