using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RevitAi.Abstractions.Authentication;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class MainViewModel : ObservableObject
{
	private readonly IAuthManager _authManager;

	private readonly IWindowManager _windowManager;

	[ObservableProperty]
	private string _userEmail = string.Empty;

	[ObservableProperty]
	private string _licenseType = "试用";

	[ObservableProperty]
	private bool _isOnlineMode;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? showSettingsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? showFeaturePanelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? showModelingToolsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? showRoadBridgeToolsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? showAIChatCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string UserEmail
	{
		get
		{
			return _userEmail;
		}
		[MemberNotNull("_userEmail")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_userEmail, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.UserEmail);
				_userEmail = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.UserEmail);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string LicenseType
	{
		get
		{
			return _licenseType;
		}
		[MemberNotNull("_licenseType")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_licenseType, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LicenseType);
				_licenseType = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LicenseType);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsOnlineMode
	{
		get
		{
			return _isOnlineMode;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isOnlineMode, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsOnlineMode);
				_isOnlineMode = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsOnlineMode);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ShowSettingsCommand => showSettingsCommand ?? (showSettingsCommand = new RelayCommand(ShowSettings));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ShowFeaturePanelCommand => showFeaturePanelCommand ?? (showFeaturePanelCommand = new RelayCommand(ShowFeaturePanel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ShowModelingToolsCommand => showModelingToolsCommand ?? (showModelingToolsCommand = new RelayCommand(ShowModelingTools));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ShowRoadBridgeToolsCommand => showRoadBridgeToolsCommand ?? (showRoadBridgeToolsCommand = new RelayCommand(ShowRoadBridgeTools));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ShowAIChatCommand => showAIChatCommand ?? (showAIChatCommand = new RelayCommand(ShowAIChat));

	public MainViewModel(IAuthManager authManager, IWindowManager windowManager)
	{
		_authManager = authManager;
		_windowManager = windowManager;
		UpdateUserInfo();
	}

	[RelayCommand]
	private void ShowSettings()
	{
		_windowManager.ShowSettingsWindow();
	}

	[RelayCommand]
	private void ShowFeaturePanel()
	{
		_windowManager.ShowFeaturePanelWindow();
	}

	[RelayCommand]
	private void ShowModelingTools()
	{
		_windowManager.ShowFeaturePanelWindow();
	}

	[RelayCommand]
	private void ShowRoadBridgeTools()
	{
		_windowManager.ShowFeaturePanelWindow();
	}

	[RelayCommand]
	private void ShowAIChat()
	{
		_windowManager.ShowAIChatPanelWindow();
	}

	private void UpdateUserInfo()
	{
		IUserIdentity currentUser = _authManager.CurrentUser;
		if (currentUser != null)
		{
			UserEmail = currentUser.Email;
		}
		IsOnlineMode = !_authManager.IsOfflineMode;
	}
}
