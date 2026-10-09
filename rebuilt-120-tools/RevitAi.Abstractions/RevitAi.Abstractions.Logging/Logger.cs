using System;
using System.IO;

namespace RevitAi.Abstractions.Logging;

public static class Logger
{
	private static readonly string _logDirectory;

	private static readonly object _lock;

	private static DateTime _currentLogFileDate;

	public static string LogDirectory => _logDirectory;

	static Logger()
	{
		_logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "Logs");
		_lock = new object();
		_currentLogFileDate = DateTime.MinValue;
		EnsureLogDirectory();
	}

	public static void Debug(string message)
	{
		Log(LogLevel.Debug, message);
	}

	public static void Info(string message)
	{
		Log(LogLevel.Info, message);
	}

	public static void Warning(string message)
	{
		Log(LogLevel.Warning, message);
	}

	public static void Warning(string message, Exception ex)
	{
		Log(LogLevel.Warning, $"{message}: {ex.GetType().Name} - {ex.Message}");
	}

	public static void Error(string message)
	{
		Log(LogLevel.Error, message);
	}

	public static void Error(string message, Exception ex)
	{
		Log(LogLevel.Error, $"{message}: {ex.GetType().Name} - {ex.Message}");
	}

	private static void Log(LogLevel level, string message)
	{
		try
		{
			lock (_lock)
			{
				EnsureLogDirectory();
				DateTime now = DateTime.Now;
				if (now.Date != _currentLogFileDate)
				{
					_currentLogFileDate = now.Date;
					CleanupOldLogFiles();
				}
				string value = now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				string text = $"[{value}] [{level}] [RevitAi] {message}";
				File.AppendAllText(Path.Combine(LogDirectory, $"astools_{now:yyyyMMdd}.log"), text + Environment.NewLine);
			}
		}
		catch
		{
		}
	}

	private static void EnsureLogDirectory()
	{
		if (!Directory.Exists(_logDirectory))
		{
			Directory.CreateDirectory(LogDirectory);
		}
	}

	private static void CleanupOldLogFiles()
	{
		try
		{
			if (!Directory.Exists(_logDirectory))
			{
				return;
			}
			string[] files = Directory.GetFiles(LogDirectory, "astools_*.log");
			DateTime dateTime = DateTime.Now.AddDays(-7.0);
			string[] array = files;
			foreach (string text in array)
			{
				if (new FileInfo(text).LastWriteTime < dateTime)
				{
					try
					{
						File.Delete(text);
					}
					catch
					{
					}
				}
			}
		}
		catch
		{
		}
	}

	public static void LogEnvironmentInfo(string context)
	{
		Info("========================================");
		Info("环境信息 (" + context + "):");
		Info("  机器名称: " + Environment.MachineName);
		Info("  用户名: " + Environment.UserName);
		Info($"  操作系统: {Environment.OSVersion}");
		Info($"  处理器数: {Environment.ProcessorCount}");
		Info($"  CLR 版本: {Environment.Version}");
		Info("  运行时: " + (Environment.Is64BitProcess ? "x64" : "x86"));
		Info("========================================");
	}
}
