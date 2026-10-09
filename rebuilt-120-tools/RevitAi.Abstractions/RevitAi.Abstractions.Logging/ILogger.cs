using System;

namespace RevitAi.Abstractions.Logging;

public interface ILogger
{
	void Debug(string message);

	void Info(string message);

	void Warning(string message);

	void Error(string message, Exception? ex = null);
}
