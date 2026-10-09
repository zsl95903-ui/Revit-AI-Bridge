using System;
using System.Collections.Generic;
using System.Linq;
using RevitAi.Abstractions.Logging;

namespace RevitAi.Abstractions.Loader;

public static class CommandRegistry
{
	private static readonly Dictionary<string, RegisteredCommand> _commands = new Dictionary<string, RegisteredCommand>();

	private static readonly object _lock = new object();

	public static void Register(string name, Type commandType, CommandAttribute attribute)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Command name cannot be null or empty", "name");
		}
		if (commandType == null)
		{
			throw new ArgumentNullException("commandType");
		}
		if (attribute == null)
		{
			throw new ArgumentNullException("attribute");
		}
		lock (_lock)
		{
			if (_commands.ContainsKey(name))
			{
				Logger.Warning("命令 '" + name + "' 已存在，将被覆盖");
			}
			_commands[name] = new RegisteredCommand
			{
				Name = name,
				CommandType = commandType,
				Attribute = attribute
			};
		}
	}

	public static RegisteredCommand? GetCommand(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		lock (_lock)
		{
			_commands.TryGetValue(name, out RegisteredCommand value);
			return value;
		}
	}

	public static IReadOnlyList<RegisteredCommand> GetAllCommands()
	{
		lock (_lock)
		{
			return _commands.Values.ToList();
		}
	}

	public static bool IsRegistered(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		lock (_lock)
		{
			return _commands.ContainsKey(name);
		}
	}

	public static void Clear()
	{
		lock (_lock)
		{
			_commands.Clear();
			Logger.Info("命令注册表已清空");
		}
	}
}
