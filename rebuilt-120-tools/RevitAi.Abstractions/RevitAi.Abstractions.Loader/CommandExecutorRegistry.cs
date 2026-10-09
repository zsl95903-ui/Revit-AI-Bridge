using System;
using RevitAi.Abstractions.Commands;

namespace RevitAi.Abstractions.Loader;

public static class CommandExecutorRegistry
{
	private static ICommandExecutor? _executor;

	public static void RegisterExecutor(ICommandExecutor executor)
	{
		_executor = executor ?? throw new ArgumentNullException("executor");
	}

	public static ICommandExecutor? GetExecutor()
	{
		return _executor;
	}

	public static void Clear()
	{
		_executor = null;
	}
}
