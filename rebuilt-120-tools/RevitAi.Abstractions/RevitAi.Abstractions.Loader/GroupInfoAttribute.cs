using System;

namespace RevitAi.Abstractions.Loader;

[AttributeUsage(AttributeTargets.Field)]
public sealed class GroupInfoAttribute : Attribute
{
	public string DisplayName { get; }

	public string ColorHex { get; }

	public string Identifier { get; }

	public GroupInfoAttribute(string displayName, string colorHex, string identifier)
	{
		DisplayName = displayName;
		ColorHex = colorHex;
		Identifier = identifier;
	}
}
