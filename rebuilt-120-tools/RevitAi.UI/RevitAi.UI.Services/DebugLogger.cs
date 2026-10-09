using System;
using System.IO;

namespace RevitAi.UI.Services;

public static class DebugLogger
{
	private static readonly string LogFilePath;

	private static readonly object _lock;

	static DebugLogger()
	{
		LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "Debug", $"AIChat_{DateTime.Now:yyyyMMdd_HHmmss}.log");
		_lock = new object();
		string directoryName = Path.GetDirectoryName(LogFilePath);
		if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		WriteLog("========================================");
		WriteLog("RevitAi AI Chat Debug Log");
		WriteLog("Log File: " + LogFilePath);
		WriteLog($"Start Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
		WriteLog("========================================\n");
	}

	public static void Log(string message)
	{
		string text = DateTime.Now.ToString("HH:mm:ss.fff");
		WriteLog("[" + text + "] " + message);
	}

	private static void WriteLog(string message)
	{
		lock (_lock)
		{
			try
			{
				File.AppendAllText(LogFilePath, message + Environment.NewLine);
			}
			catch
			{
			}
		}
	}

	public static string GetLogFilePath()
	{
		return LogFilePath;
	}
}
