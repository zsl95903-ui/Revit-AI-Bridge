using System;

namespace RevitAi.Abstractions.Logging;

public sealed class LoggerAdapter : ILogger
{
	public void Debug(string message)
	{
		Logger.Debug(message);
	}

	public void Info(string message)
	{
		Logger.Info(message);
	}

	public void Warning(string message)
	{
		Logger.Warning(message);
	}

	public void Error(string message, Exception? ex = null)
	{
		if (ex != null)
		{
			Logger.Error(message, ex);
		}
		else
		{
			Logger.Error(message);
		}
	}
}
