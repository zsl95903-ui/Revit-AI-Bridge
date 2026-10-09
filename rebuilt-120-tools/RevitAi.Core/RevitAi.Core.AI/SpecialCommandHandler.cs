using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ns7;

namespace RevitAi.Core.AI;

public static class SpecialCommandHandler
{
	[CompilerGenerated]
	public sealed class Class134
	{
		public string string_0;

		internal bool method_0(string string_1)
		{
			if (!string_0.Equals(string_1, StringComparison.OrdinalIgnoreCase))
			{
				return string_0.StartsWith(string_1 + " ", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}
	}

	private static readonly string[] string_0 = new string[3]
	{
		"REQUEST_TOOLS",
		"REQUEST_SKILL_LIST",
		"REQUEST_SKILL_DETAIL"
	};

	public static bool IsSpecialCommand(string? content)
	{
		if (string.IsNullOrWhiteSpace(content))
		{
			return false;
		}
		string string_0 = content.Trim();
		return SpecialCommandHandler.string_0.Any((string text) => string_0.Equals(text, StringComparison.OrdinalIgnoreCase) || string_0.StartsWith(text + " ", StringComparison.OrdinalIgnoreCase));
	}

	public static (string command, string? argument) ParseSpecialCommand(string content)
	{
		if (string.IsNullOrWhiteSpace(content))
		{
			return (command: "", argument: null);
		}
		string text = content.Trim();
		string[] array = string_0;
		int num = 0;
		string text2;
		while (true)
		{
			if (num < array.Length)
			{
				text2 = array[num];
				if (!text.Equals(text2, StringComparison.OrdinalIgnoreCase))
				{
					if (text.StartsWith(text2 + " ", StringComparison.OrdinalIgnoreCase))
					{
						break;
					}
					num++;
					continue;
				}
				return (command: text2, argument: null);
			}
			return (command: "", argument: null);
		}
		string item = text.Substring(text2.Length + 1).Trim();
		return (command: text2, argument: item);
	}

	public static bool ShouldHideFromUser(string content)
	{
		return IsSpecialCommand(content);
	}

	public static string[] GetSupportedCommands()
	{
		return (string[])string_0.Clone();
	}
}
