using System;
using System.Windows;
using RevitAi.UI.Models;

namespace RevitAi.UI.Services;

public sealed class ThemeService : IThemeService
{
	private AppTheme _currentTheme;

	public AppTheme CurrentTheme => _currentTheme;

	public event EventHandler<AppTheme>? ThemeChanged;

	public void SetTheme(AppTheme theme)
	{
		if (_currentTheme != theme)
		{
			_currentTheme = theme;
			ApplyTheme(theme);
			ThemeChanged?.Invoke(this, theme);
		}
	}

	public void SetLightTheme()
	{
		SetTheme(AppTheme.Light);
	}

	public void SetDarkTheme()
	{
		SetTheme(AppTheme.Dark);
	}

	public void ToggleTheme()
	{
		SetTheme((_currentTheme == AppTheme.Light) ? AppTheme.Dark : AppTheme.Light);
	}

	private void ApplyTheme(AppTheme theme)
	{
		if (Application.Current == null)
		{
			return;
		}
		try
		{
			ResourceDictionary resources = Application.Current.Resources;
			ResourceDictionary item = new ResourceDictionary
			{
				Source = new Uri($"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.{theme}.xaml", UriKind.Absolute)
			};
			for (int num = resources.MergedDictionaries.Count - 1; num >= 0; num--)
			{
				ResourceDictionary resourceDictionary = resources.MergedDictionaries[num];
				if (resourceDictionary.Source != null && resourceDictionary.Source.OriginalString.Contains("MaterialDesignTheme."))
				{
					resources.MergedDictionaries.RemoveAt(num);
				}
			}
			resources.MergedDictionaries.Add(item);
		}
		catch (Exception)
		{
		}
	}
}
