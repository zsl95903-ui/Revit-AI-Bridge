using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RevitAi.Abstractions.Product;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class SettingsViewModel : ObservableObject
{
	private readonly IThemeService _themeService;

	private readonly IUserPreferencesService _preferencesService;

	private readonly IDialogService _dialogService;

	private readonly IWindowManager _windowManager;

	[ObservableProperty]
	private UserPreferences _preferences;

	[ObservableProperty]
	private string _versionInfo = ProductInfo.GetFullVersionInfo();

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? saveSettingsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? toggleThemeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<AppTheme>? applyThemeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? resetToDefaultsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? showAboutCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public UserPreferences Preferences
	{
		get
		{
			return _preferences;
		}
		[MemberNotNull("_preferences")]
		set
		{
			if (!EqualityComparer<UserPreferences>.Default.Equals(_preferences, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Preferences);
				_preferences = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Preferences);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string VersionInfo
	{
		get
		{
			return _versionInfo;
		}
		[MemberNotNull("_versionInfo")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_versionInfo, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.VersionInfo);
				_versionInfo = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.VersionInfo);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SaveSettingsCommand => saveSettingsCommand ?? (saveSettingsCommand = new RelayCommand(SaveSettings));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ToggleThemeCommand => toggleThemeCommand ?? (toggleThemeCommand = new RelayCommand(ToggleTheme));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<AppTheme> ApplyThemeCommand => applyThemeCommand ?? (applyThemeCommand = new RelayCommand<AppTheme>(ApplyTheme));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ResetToDefaultsCommand => resetToDefaultsCommand ?? (resetToDefaultsCommand = new RelayCommand(ResetToDefaults));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ShowAboutCommand => showAboutCommand ?? (showAboutCommand = new RelayCommand(ShowAbout));

	public SettingsViewModel(IThemeService themeService, IUserPreferencesService preferencesService, IDialogService dialogService, IWindowManager windowManager)
	{
		_themeService = themeService;
		_preferencesService = preferencesService;
		_dialogService = dialogService;
		_windowManager = windowManager;
		_preferences = _preferencesService.CurrentPreferences;
		_themeService.ThemeChanged += OnThemeChanged;
	}

	[RelayCommand]
	private void SaveSettings()
	{
		try
		{
			_preferencesService.SavePreferences(Preferences);
			_dialogService.ShowInfo("设置已保存", "保存成功");
		}
		catch (Exception ex)
		{
			_dialogService.ShowError("保存设置失败：" + ex.Message, "保存失败");
		}
	}

	[RelayCommand]
	private void Cancel()
	{
		_windowManager.CloseSettingsWindow();
	}

	[RelayCommand]
	private void ToggleTheme()
	{
		_themeService.ToggleTheme();
	}

	[RelayCommand]
	private void ApplyTheme(AppTheme theme)
	{
		_themeService.SetTheme(theme);
		Preferences.Theme = theme;
	}

	[RelayCommand]
	private void ResetToDefaults()
	{
		if (_dialogService.ShowConfirm("确定要重置所有设置为默认值吗？", "确认重置"))
		{
			Preferences = new UserPreferences();
			_dialogService.ShowInfo("设置已重置为默认值", "重置完成");
		}
	}

	[RelayCommand]
	private void ShowAbout()
	{
		_dialogService.ShowInfo($"RevitAi - 建筑智能化工具集\n\n版本：{ProductInfo.Version}\n{ProductInfo.Copyright}\n\n" + "本软件采用 MaterialDesign 设计风格\n支持 Revit 2018-2026+\n\n更多信息请访问项目主页", "关于 RevitAi");
	}

	private void OnThemeChanged(object? sender, AppTheme newTheme)
	{
		Preferences.Theme = newTheme;
	}
}
