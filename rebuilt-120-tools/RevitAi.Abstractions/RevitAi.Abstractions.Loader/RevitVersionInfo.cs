using System;

namespace RevitAi.Abstractions.Loader;

public sealed class RevitVersionInfo
{
	public int VersionYear { get; }

	public string FrameworkVersion { get; }

	public string AssemblyName { get; }

	public string DisplayName { get; }

	public RevitRuntimeEnvironment RuntimeEnvironment { get; }

	public string RuntimeDirectoryName { get; }

	public RevitVersionInfo(int versionYear, string frameworkVersion, string assemblyName, string displayName, RevitRuntimeEnvironment runtimeEnvironment, string runtimeDirectoryName)
	{
		VersionYear = versionYear;
		FrameworkVersion = frameworkVersion ?? throw new ArgumentNullException("frameworkVersion");
		AssemblyName = assemblyName ?? throw new ArgumentNullException("assemblyName");
		DisplayName = displayName ?? throw new ArgumentNullException("displayName");
		RuntimeEnvironment = runtimeEnvironment;
		RuntimeDirectoryName = runtimeDirectoryName ?? throw new ArgumentNullException("runtimeDirectoryName");
	}
}
