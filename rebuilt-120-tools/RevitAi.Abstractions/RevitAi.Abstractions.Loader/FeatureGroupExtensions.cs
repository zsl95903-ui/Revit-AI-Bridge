using System.Linq;

namespace RevitAi.Abstractions.Loader;

public static class FeatureGroupExtensions
{
	public static string GetDisplayName(this FeatureGroup group)
	{
		return (group.GetType().GetField(group.ToString())?.GetCustomAttributes(typeof(GroupInfoAttribute), inherit: false)?.FirstOrDefault() as GroupInfoAttribute)?.DisplayName ?? group.ToString();
	}

	public static string GetColorHex(this FeatureGroup group)
	{
		return (group.GetType().GetField(group.ToString())?.GetCustomAttributes(typeof(GroupInfoAttribute), inherit: false)?.FirstOrDefault() as GroupInfoAttribute)?.ColorHex ?? "#607D8B";
	}

	public static string GetIdentifier(this FeatureGroup group)
	{
		return (group.GetType().GetField(group.ToString())?.GetCustomAttributes(typeof(GroupInfoAttribute), inherit: false)?.FirstOrDefault() as GroupInfoAttribute)?.Identifier ?? group.ToString().ToLowerInvariant();
	}
}
