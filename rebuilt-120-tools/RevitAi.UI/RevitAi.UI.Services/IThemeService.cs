using System;
using RevitAi.UI.Models;

namespace RevitAi.UI.Services;

public interface IThemeService
{
	AppTheme CurrentTheme { get; }

	event EventHandler<AppTheme>? ThemeChanged;

	void SetTheme(AppTheme theme);

	void SetLightTheme();

	void SetDarkTheme();

	void ToggleTheme();
}
