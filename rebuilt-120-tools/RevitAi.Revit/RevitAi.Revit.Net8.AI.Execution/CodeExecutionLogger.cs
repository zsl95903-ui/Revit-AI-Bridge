using RevitAi.Abstractions.Logging;

namespace RevitAi.Revit.Net8.AI.Execution;

public sealed class CodeExecutionLogger
{
	public void Info(string message)
	{
		Logger.Info(message);
	}

	public void Warning(string message)
	{
		Logger.Warning(message);
	}

	public void Error(string message)
	{
		Logger.Error(message);
	}
}
