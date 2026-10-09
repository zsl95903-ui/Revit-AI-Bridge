namespace RevitAi.UI.Models;

public sealed class UserPreferences
{
	public AppTheme Theme { get; set; }

	public bool AutoLogin { get; set; }

	public bool RememberWindowPosition { get; set; } = true;

	public bool ShowWelcomeScreen { get; set; } = true;

	public bool EnableAnimations { get; set; } = true;

	public string Language { get; set; } = "zh-CN";

	public string LogLevel { get; set; } = "Info";

	public string ChatFontFamily { get; set; } = "Segoe UI, Microsoft YaHei UI";

	public int ChatFontSize { get; set; } = 14;
}
