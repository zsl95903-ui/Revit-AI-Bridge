namespace RevitAi.Abstractions.Loader;

public sealed class FeatureItem
{
	public string CommandName { get; set; } = string.Empty;

	public string DisplayName { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string Category { get; set; } = "未分类";

	public bool IsEnabled { get; set; }

	public string? IconPath { get; set; }

	public string ColorHex { get; set; } = "#2196F3";

	public string SubCategory { get; set; } = "其他";

	public int Order { get; set; } = 1000;
}
