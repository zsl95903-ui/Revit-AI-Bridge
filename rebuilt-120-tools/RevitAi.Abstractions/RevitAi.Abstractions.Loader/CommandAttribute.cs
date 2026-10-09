using System;

namespace RevitAi.Abstractions.Loader;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class CommandAttribute : Attribute
{
	public string Name { get; }

	public string Text { get; }

	public string Description { get; }

	public string VisibilityMode { get; }

	public bool IsBasicFeature { get; }

	public FeatureGroup Group { get; }

	public string SubCategory { get; set; } = "其他";

	public int Order { get; set; } = 1000;

	public CommandAttribute(string name, string text, string description = "", string visibilityMode = "AlwaysVisible", bool isBasicFeature = false, FeatureGroup group = FeatureGroup.ToolAssistant, string subCategory = "其他", int order = 1000)
	{
		Name = name ?? throw new ArgumentNullException("name");
		Text = text ?? throw new ArgumentNullException("text");
		Description = description;
		VisibilityMode = visibilityMode;
		IsBasicFeature = isBasicFeature;
		Group = group;
		SubCategory = subCategory;
		Order = order;
	}
}
