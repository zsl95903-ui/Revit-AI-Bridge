using System.Diagnostics;
using System.IO;
using RevitAi.Abstractions.Logging;

namespace RevitAi.Abstractions.Loader;

public static class RevitVersionDetector
{
	public static RevitVersionInfo GetCurrentVersion()
	{
		Process currentProcess = Process.GetCurrentProcess();
		string text = currentProcess.ProcessName.ToUpperInvariant();
		ProcessModule mainModule = currentProcess.MainModule;
		if (text.StartsWith("REVIT") && text.Length > 5 && int.TryParse(text.Substring(5), out var result))
		{
			Logger.Info($"从进程名成功解析版本: {result}");
			return CreateVersionInfo(result);
		}
		if (mainModule != null && !string.IsNullOrEmpty(mainModule.FileName))
		{
			string directoryName = Path.GetDirectoryName(mainModule.FileName);
			if (directoryName != null)
			{
				string[] array = Path.GetFileName(directoryName).Split(' ', '_');
				for (int i = 0; i < array.Length; i++)
				{
					if (int.TryParse(array[i], out var result2) && result2 >= 2010 && result2 <= 2100)
					{
						Logger.Info($"从目录名成功解析版本: {result2}");
						return CreateVersionInfo(result2);
					}
				}
			}
		}
		Logger.Warning("无法自动检测 Revit 版本，使用默认版本 2020");
		return CreateVersionInfo(2020);
	}

	private static RevitVersionInfo CreateVersionInfo(int versionYear)
	{
		RevitRuntimeEnvironment runtimeEnvironment;
		string runtimeDirectoryName;
		string frameworkVersion;
		if (versionYear >= 2025)
		{
			runtimeEnvironment = RevitRuntimeEnvironment.Next;
			runtimeDirectoryName = "R_Next";
			frameworkVersion = "net8.0-windows";
		}
		else if (versionYear == 2024)
		{
			runtimeEnvironment = RevitRuntimeEnvironment.Modern;
			runtimeDirectoryName = "R2024";
			frameworkVersion = "net48";
		}
		else if (versionYear >= 2021)
		{
			runtimeEnvironment = RevitRuntimeEnvironment.Modern;
			runtimeDirectoryName = "R_Modern";
			frameworkVersion = "net48";
		}
		else
		{
			runtimeEnvironment = RevitRuntimeEnvironment.Legacy;
			runtimeDirectoryName = "R_Legacy";
			frameworkVersion = "net48";
		}
		string assemblyName = "RevitAi.Revit";
		return new RevitVersionInfo(versionYear, frameworkVersion, assemblyName, $"Revit {versionYear}", runtimeEnvironment, runtimeDirectoryName);
	}
}
