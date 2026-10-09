using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Product;
using RevitAi.Core.AI;
using RevitAi.Core.Feedback;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class AboutViewModel : ObservableObject
{
	private static readonly SolidColorBrush RedBrush;

	private static readonly SolidColorBrush OrangeBrush;

	private static readonly SolidColorBrush AmberBrush;

	private static readonly SolidColorBrush GreenBrush;

	private readonly IAuthManager _authManager;

	private readonly IDialogService _dialogService;

	private readonly Window? _window;

	private readonly IUserPreferencesService? _preferencesService;

	private readonly LocalAIConfigService _aiConfigService;

	private readonly TokenUsageService? _tokenUsageService;

	private readonly FeedbackService? _feedbackService;

	[ObservableProperty]
	private string _email = string.Empty;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private bool _isLoggedIn;

	[ObservableProperty]
	private string _deviceId;

	[ObservableProperty]
	private string _deviceStatus;

	[ObservableProperty]
	private string _licenseStatus;

	[ObservableProperty]
	private string _licenseType;

	[ObservableProperty]
	private string _licenseExpiry;

	[ObservableProperty]
	private string _userEmail = string.Empty;

	[ObservableProperty]
	private bool _showLicenseDetails;

	[ObservableProperty]
	private string _statusIcon;

	[ObservableProperty]
	private Brush _statusBrush;

	[ObservableProperty]
	private ObservableCollection<DeviceInfoViewModel> _userDevices = new ObservableCollection<DeviceInfoViewModel>();

	[ObservableProperty]
	private bool _hasNoDevices;

	[ObservableProperty]
	private int _deviceCount;

	[ObservableProperty]
	private int _maxDevices = 2;

	[ObservableProperty]
	private bool _isCurrentDeviceBound;

	[ObservableProperty]
	private ObservableCollection<AIProviderViewModel> _aiProviders = new ObservableCollection<AIProviderViewModel>();

	[ObservableProperty]
	private bool _hasNoAIConfigs = true;

	[ObservableProperty]
	private string? _authErrorMessage;

	[ObservableProperty]
	private decimal _currentBalance;

	[ObservableProperty]
	private decimal _totalPurchased;

	[ObservableProperty]
	private decimal _totalConsumed;

	[ObservableProperty]
	private bool _isLoadingCredits;

	[ObservableProperty]
	private int _unreadCount;

	[ObservableProperty]
	private bool _hasUnreadFeedback;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loginCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? logoutCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? signUpCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? forgotPasswordCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? copyDeviceIdCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? refreshDeviceStatusCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? refreshAuthorizationCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? openLogFolderCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? openChangelogCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? refreshDevicesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? bindCurrentDeviceCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string?>? unbindDeviceCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? showPurchaseLicenseCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? purchaseCreditsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addAIProviderCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<string?>? editAIProviderCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string?>? deleteAIProviderCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? showFeedbackCommand;

	public bool IsLoggedOut => !IsLoggedIn;

	public bool ShowCurrentDeviceUnboundWarning
	{
		get
		{
			if (IsLoggedIn && !IsCurrentDeviceBound)
			{
				return DeviceCount >= MaxDevices;
			}
			return false;
		}
	}

	public string CreditsDisplayText => CurrentBalance.ToString("F2");

	public string ProductVersionInfo => ProductInfo.GetFullVersionInfo();

	public string ProductVersion => "版本 " + ProductInfo.Version;

	public string ProductCopyright => ProductInfo.Copyright;

	public string TotalPurchasedDisplayText => $"{TotalPurchased:F2} 电量";

	public string TotalConsumedDisplayText => $"{TotalConsumed:F2} 电量";

	public string CreditsStatusText
	{
		get
		{
			if (CurrentBalance <= 0m)
			{
				return "⚠\ufe0f 电量已耗尽，请充值";
			}
			if (CurrentBalance < 1m)
			{
				return "⚠\ufe0f 电量不足，建议充值";
			}
			if (CurrentBalance < 10m)
			{
				return "ℹ\ufe0f 电量偏低";
			}
			return "✅ 电量充足";
		}
	}

	public Brush CreditsStatusBrush
	{
		get
		{
			if (CurrentBalance <= 0m)
			{
				return RedBrush;
			}
			if (CurrentBalance < 1m)
			{
				return OrangeBrush;
			}
			if (CurrentBalance < 10m)
			{
				return AmberBrush;
			}
			return GreenBrush;
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Email
	{
		get
		{
			return _email;
		}
		[MemberNotNull("_email")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_email, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Email);
				_email = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Email);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsLoading
	{
		get
		{
			return _isLoading;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLoading, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsLoading);
				_isLoading = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsLoading);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsLoggedIn
	{
		get
		{
			return _isLoggedIn;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLoggedIn, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsLoggedIn);
				_isLoggedIn = value;
				OnIsLoggedInChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsLoggedIn);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string DeviceId
	{
		get
		{
			return _deviceId;
		}
		[MemberNotNull("_deviceId")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_deviceId, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DeviceId);
				_deviceId = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DeviceId);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string DeviceStatus
	{
		get
		{
			return _deviceStatus;
		}
		[MemberNotNull("_deviceStatus")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_deviceStatus, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DeviceStatus);
				_deviceStatus = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DeviceStatus);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string LicenseStatus
	{
		get
		{
			return _licenseStatus;
		}
		[MemberNotNull("_licenseStatus")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_licenseStatus, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LicenseStatus);
				_licenseStatus = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LicenseStatus);
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
	public string LicenseExpiry
	{
		get
		{
			return _licenseExpiry;
		}
		[MemberNotNull("_licenseExpiry")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_licenseExpiry, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LicenseExpiry);
				_licenseExpiry = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LicenseExpiry);
			}
		}
	}

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
	public bool ShowLicenseDetails
	{
		get
		{
			return _showLicenseDetails;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_showLicenseDetails, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ShowLicenseDetails);
				_showLicenseDetails = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ShowLicenseDetails);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string StatusIcon
	{
		get
		{
			return _statusIcon;
		}
		[MemberNotNull("_statusIcon")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_statusIcon, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StatusIcon);
				_statusIcon = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StatusIcon);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public Brush StatusBrush
	{
		get
		{
			return _statusBrush;
		}
		[MemberNotNull("_statusBrush")]
		set
		{
			if (!EqualityComparer<Brush>.Default.Equals(_statusBrush, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StatusBrush);
				_statusBrush = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StatusBrush);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<DeviceInfoViewModel> UserDevices
	{
		get
		{
			return _userDevices;
		}
		[MemberNotNull("_userDevices")]
		set
		{
			if (!EqualityComparer<ObservableCollection<DeviceInfoViewModel>>.Default.Equals(_userDevices, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.UserDevices);
				_userDevices = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.UserDevices);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasNoDevices
	{
		get
		{
			return _hasNoDevices;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasNoDevices, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasNoDevices);
				_hasNoDevices = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasNoDevices);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int DeviceCount
	{
		get
		{
			return _deviceCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_deviceCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DeviceCount);
				_deviceCount = value;
				OnDeviceCountChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DeviceCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int MaxDevices
	{
		get
		{
			return _maxDevices;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_maxDevices, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.MaxDevices);
				_maxDevices = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.MaxDevices);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsCurrentDeviceBound
	{
		get
		{
			return _isCurrentDeviceBound;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isCurrentDeviceBound, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsCurrentDeviceBound);
				_isCurrentDeviceBound = value;
				OnIsCurrentDeviceBoundChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsCurrentDeviceBound);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<AIProviderViewModel> AiProviders
	{
		get
		{
			return _aiProviders;
		}
		[MemberNotNull("_aiProviders")]
		set
		{
			if (!EqualityComparer<ObservableCollection<AIProviderViewModel>>.Default.Equals(_aiProviders, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AiProviders);
				_aiProviders = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AiProviders);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasNoAIConfigs
	{
		get
		{
			return _hasNoAIConfigs;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasNoAIConfigs, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasNoAIConfigs);
				_hasNoAIConfigs = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasNoAIConfigs);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? AuthErrorMessage
	{
		get
		{
			return _authErrorMessage;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_authErrorMessage, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AuthErrorMessage);
				_authErrorMessage = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AuthErrorMessage);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public decimal CurrentBalance
	{
		get
		{
			return _currentBalance;
		}
		set
		{
			if (!EqualityComparer<decimal>.Default.Equals(_currentBalance, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CurrentBalance);
				_currentBalance = value;
				OnCurrentBalanceChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CurrentBalance);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public decimal TotalPurchased
	{
		get
		{
			return _totalPurchased;
		}
		set
		{
			if (!EqualityComparer<decimal>.Default.Equals(_totalPurchased, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.TotalPurchased);
				_totalPurchased = value;
				OnTotalPurchasedChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.TotalPurchased);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public decimal TotalConsumed
	{
		get
		{
			return _totalConsumed;
		}
		set
		{
			if (!EqualityComparer<decimal>.Default.Equals(_totalConsumed, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.TotalConsumed);
				_totalConsumed = value;
				OnTotalConsumedChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.TotalConsumed);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsLoadingCredits
	{
		get
		{
			return _isLoadingCredits;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLoadingCredits, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsLoadingCredits);
				_isLoadingCredits = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsLoadingCredits);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int UnreadCount
	{
		get
		{
			return _unreadCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_unreadCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.UnreadCount);
				_unreadCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.UnreadCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasUnreadFeedback
	{
		get
		{
			return _hasUnreadFeedback;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasUnreadFeedback, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasUnreadFeedback);
				_hasUnreadFeedback = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasUnreadFeedback);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoginCommand => loginCommand ?? (loginCommand = new AsyncRelayCommand(LoginAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LogoutCommand => logoutCommand ?? (logoutCommand = new AsyncRelayCommand(LogoutAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SignUpCommand => signUpCommand ?? (signUpCommand = new RelayCommand(SignUp));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ForgotPasswordCommand => forgotPasswordCommand ?? (forgotPasswordCommand = new RelayCommand(ForgotPassword));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand CopyDeviceIdCommand => copyDeviceIdCommand ?? (copyDeviceIdCommand = new AsyncRelayCommand(CopyDeviceIdAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand RefreshDeviceStatusCommand => refreshDeviceStatusCommand ?? (refreshDeviceStatusCommand = new AsyncRelayCommand(RefreshDeviceStatusAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand RefreshAuthorizationCommand => refreshAuthorizationCommand ?? (refreshAuthorizationCommand = new AsyncRelayCommand(RefreshAuthorizationAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand OpenLogFolderCommand => openLogFolderCommand ?? (openLogFolderCommand = new RelayCommand(OpenLogFolder));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand OpenChangelogCommand => openChangelogCommand ?? (openChangelogCommand = new RelayCommand(OpenChangelog));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand RefreshDevicesCommand => refreshDevicesCommand ?? (refreshDevicesCommand = new AsyncRelayCommand(RefreshDevicesAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand BindCurrentDeviceCommand => bindCurrentDeviceCommand ?? (bindCurrentDeviceCommand = new AsyncRelayCommand(BindCurrentDeviceAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string?> UnbindDeviceCommand => unbindDeviceCommand ?? (unbindDeviceCommand = new AsyncRelayCommand<string>(UnbindDeviceAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ShowPurchaseLicenseCommand => showPurchaseLicenseCommand ?? (showPurchaseLicenseCommand = new RelayCommand(ShowPurchaseLicense));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand PurchaseCreditsCommand => purchaseCreditsCommand ?? (purchaseCreditsCommand = new AsyncRelayCommand(PurchaseCreditsAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddAIProviderCommand => addAIProviderCommand ?? (addAIProviderCommand = new RelayCommand(AddAIProvider));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<string?> EditAIProviderCommand => editAIProviderCommand ?? (editAIProviderCommand = new RelayCommand<string>(EditAIProvider));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string?> DeleteAIProviderCommand => deleteAIProviderCommand ?? (deleteAIProviderCommand = new AsyncRelayCommand<string>(DeleteAIProviderAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ShowFeedbackCommand => showFeedbackCommand ?? (showFeedbackCommand = new RelayCommand(ShowFeedback));

	static AboutViewModel()
	{
		RedBrush = new SolidColorBrush(Color.FromRgb(244, 67, 54));
		OrangeBrush = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 152, 0));
		AmberBrush = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 193, 7));
		GreenBrush = new SolidColorBrush(Color.FromRgb(76, 175, 80));
		((Freezable)RedBrush).Freeze();
		((Freezable)OrangeBrush).Freeze();
		((Freezable)AmberBrush).Freeze();
		((Freezable)GreenBrush).Freeze();
	}

	public AboutViewModel(IAuthManager authManager, IDialogService dialogService, TokenUsageService? tokenUsageService = null, Window? window = null)
	{
		_authManager = authManager;
		_dialogService = dialogService;
		_window = window;
		_aiConfigService = new LocalAIConfigService();
		_tokenUsageService = tokenUsageService;
		try
		{
			_feedbackService = UIBootstrapper.TryGetService<FeedbackService>();
		}
		catch
		{
			_feedbackService = null;
		}
		try
		{
			if (Application.Current != null)
			{
				PropertyInfo property = ((object)Application.Current).GetType().GetProperty("Services");
				if (property != null)
				{
					object value = property.GetValue(Application.Current);
					if (value != null)
					{
						MethodInfo method = value.GetType().GetMethod("GetService", new Type[1] { typeof(Type) });
						if (method != null)
						{
							object obj2 = method.Invoke(value, new object[1] { typeof(IUserPreferencesService) });
							_preferencesService = obj2 as IUserPreferencesService;
						}
					}
				}
			}
		}
		catch
		{
			_preferencesService = null;
		}
		_isLoggedIn = false;
		_deviceId = "获取中...";
		_deviceStatus = "未知";
		_licenseStatus = "检查中...";
		_licenseType = "无";
		_licenseExpiry = "无";
		_statusIcon = "HelpCircle";
		_statusBrush = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 152, 0));
		_currentBalance = 0m;
		_totalPurchased = 0m;
		_totalConsumed = 0m;
		_isLoadingCredits = false;
		LoadAIConfigs();
	}

	public async Task InitializeDataAsync()
	{
		_ = 2;
		try
		{
			try
			{
				await LoadStatusAsync();
			}
			catch (Exception ex)
			{
				Logger.Error("[AboutViewModel] LoadStatusAsync 失败", ex);
				DeviceId = "未知";
				DeviceStatus = "状态获取失败";
				LicenseStatus = "未知";
				StatusIcon = "AlertCircle";
				StatusBrush = RedBrush;
			}
			try
			{
				if (_tokenUsageService != null)
				{
					await RefreshCreditsFromTokenServiceAsync(forceRefresh: true);
				}
				else
				{
					LoadCreditsFromCache();
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("[AboutViewModel] 加载电量信息失败，降级到缓存", ex2);
				try
				{
					LoadCreditsFromCache();
				}
				catch
				{
					CurrentBalance = 0m;
				}
			}
			try
			{
				await CheckUnreadFeedbackAsync();
			}
			catch (Exception ex3)
			{
				Logger.Error("[AboutViewModel] 检查未读反馈失败", ex3);
			}
		}
		catch (Exception ex4)
		{
			Logger.Error("[AboutViewModel] InitializeDataAsync 未处理的异常", ex4);
		}
	}

	[RelayCommand]
	private async Task LoginAsync()
	{
		AuthErrorMessage = null;
		string text = null;
		if (_window is AboutWindow aboutWindow)
		{
			text = aboutWindow.GetPassword();
		}
		if (string.IsNullOrWhiteSpace(Email))
		{
			AuthErrorMessage = "请输入邮箱地址";
			return;
		}
		if (string.IsNullOrWhiteSpace(text))
		{
			AuthErrorMessage = "请输入密码";
			return;
		}
		IsLoading = true;
		try
		{
			Result<IUserIdentity> result = await _authManager.SignInAsync(Email, text);
			if (result.IsSuccess)
			{
				UserEmail = Email;
				Email = string.Empty;
				if (_window is AboutWindow aboutWindow2)
				{
					(aboutWindow2.FindName("PasswordBox") as PasswordBox)?.Clear();
				}
				try
				{
					var (flag, message) = await _authManager.CanBindCurrentDeviceAsync();
					if (!flag)
					{
						if (!message.Contains("DEVICE_LIMIT_REACHED"))
						{
							await _authManager.SignOutAsync();
							UserEmail = string.Empty;
							Email = string.Empty;
							await _dialogService.ShowWarningAsync("设备绑定失败：" + message + "\n\n由于当前设备无法绑定，已自动退出登录。\n请稍后重试。");
							await LoadStatusAsync();
							return;
						}
						DeviceLimitWindow deviceLimitWindow = new DeviceLimitWindow
						{
							Owner = _window
						};
						DeviceLimitViewModel dataContext = new DeviceLimitViewModel(_authManager, _dialogService, deviceLimitWindow);
						deviceLimitWindow.DataContext = dataContext;
						if (deviceLimitWindow.ShowDialog() != true)
						{
							await _authManager.SignOutAsync();
							UserEmail = string.Empty;
							Email = string.Empty;
							await LoadStatusAsync();
							return;
						}
						Result bindResult = await _authManager.BindCurrentDeviceAsync();
						if (!bindResult.IsSuccess)
						{
							await _authManager.SignOutAsync();
							UserEmail = string.Empty;
							Email = string.Empty;
							await _dialogService.ShowWarningAsync("设备绑定失败：" + bindResult.Error + "\n\n由于当前设备无法绑定，已自动退出登录。\n请稍后重试。");
							await LoadStatusAsync();
							return;
						}
					}
					else
					{
						Result bindResult = await _authManager.BindCurrentDeviceAsync();
						if (!bindResult.IsSuccess)
						{
							await _authManager.SignOutAsync();
							UserEmail = string.Empty;
							Email = string.Empty;
							await _dialogService.ShowWarningAsync("设备绑定失败：" + bindResult.Error + "\n\n由于当前设备无法绑定，已自动退出登录。\n请稍后重试。");
							await LoadStatusAsync();
							return;
						}
					}
				}
				catch (Exception ex)
				{
					await _authManager.SignOutAsync();
					UserEmail = string.Empty;
					Email = string.Empty;
					await _dialogService.ShowWarningAsync("设备绑定异常：" + ex.Message + "\n\n由于当前设备无法绑定，已自动退出登录。\n请稍后重试。");
					await LoadStatusAsync();
					return;
				}
				await Task.Delay(500);
				await LoadStatusAsync();
				await RefreshDevicesAsync();
				if (!IsLoggedIn && _authManager.CurrentUser != null)
				{
					IsLoggedIn = true;
					ShowLicenseDetails = true;
					StatusIcon = "CheckCircle";
					StatusBrush = GreenBrush;
					LicenseStatus = "已授权";
				}
			}
			else
			{
				AuthErrorMessage = "登录失败：" + result.Error;
			}
		}
		catch (Exception ex2)
		{
			AuthErrorMessage = "登录异常：" + ex2.Message;
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private async Task LogoutAsync()
	{
		try
		{
			Result result = await _authManager.SignOutAsync();
			if (!result.IsSuccess)
			{
				await _dialogService.ShowErrorAsync("退出失败：" + result.Error);
				return;
			}
			UserEmail = string.Empty;
			await _authManager.RefreshAuthorizationDataAsync();
			await LoadStatusAsync();
			if (_tokenUsageService != null)
			{
				await RefreshCreditsFromTokenServiceAsync(forceRefresh: true);
			}
			else
			{
				LoadCreditsFromCache();
			}
		}
		catch (Exception ex)
		{
			await _dialogService.ShowErrorAsync("退出异常：" + ex.Message);
		}
	}

	[RelayCommand]
	private void SignUp()
	{
		try
		{
			SignUpWindow signUpWindow = new SignUpWindow();
			signUpWindow.Topmost = true;
			SignUpViewModel dataContext = new SignUpViewModel(_authManager, _dialogService, signUpWindow);
			signUpWindow.DataContext = dataContext;
			if (signUpWindow.ShowDialog() == true)
			{
				LoadStatusAsync();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] SignUp 命令异常", ex);
			_dialogService.ShowError("打开注册窗口失败：" + ex.Message);
		}
	}

	[RelayCommand]
	private void ForgotPassword()
	{
		try
		{
			ResetPasswordWindow resetPasswordWindow = new ResetPasswordWindow();
			if (!string.IsNullOrWhiteSpace(Email) && resetPasswordWindow.EmailTextBox != null)
			{
				resetPasswordWindow.EmailTextBox.Text = Email;
			}
			resetPasswordWindow.ShowDialog();
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] ForgotPassword 命令异常", ex);
			_dialogService.ShowError("打开重置密码窗口失败：" + ex.Message);
		}
	}

	[RelayCommand]
	private async Task CopyDeviceIdAsync()
	{
		try
		{
			if (string.IsNullOrWhiteSpace(DeviceId) || DeviceId == "获取中...")
			{
				await _dialogService.ShowErrorAsync("设备ID尚未准备好");
				return;
			}
			bool success = false;
			for (int i = 0; i < 10; i++)
			{
				try
				{
					Clipboard.SetText(DeviceId);
					success = true;
				}
				catch (COMException) when (i < 9)
				{
					await Task.Delay(50);
					continue;
				}
				break;
			}
			if (success)
			{
				await _dialogService.ShowInfoAsync("设备ID已复制到剪贴板");
				return;
			}
			Logger.Error("[AboutViewModel] 复制设备ID失败：剪贴板持续被占用");
			await _dialogService.ShowErrorAsync("复制失败：剪贴板被其他程序占用，请稍后重试");
		}
		catch (Exception ex2)
		{
			Logger.Error("[AboutViewModel] 复制设备ID失败", ex2);
			await _dialogService.ShowErrorAsync("复制失败：" + ex2.Message);
		}
	}

	[RelayCommand]
	private async Task RefreshDeviceStatusAsync()
	{
		await LoadStatusAsync();
		await _dialogService.ShowInfoAsync("设备状态已刷新");
	}

	[RelayCommand]
	private async Task RefreshAuthorizationAsync()
	{
		Logger.Info("[AboutViewModel] \ud83d\udd04 手动刷新授权数据（许可证、试用记录、电量）");
		try
		{
			IsLoading = true;
			IsLoadingCredits = true;
			if (_tokenUsageService != null)
			{
				Logger.Info("[AboutViewModel] 清除 TokenUsageService 电量缓存");
				_tokenUsageService.ClearCreditsCache();
			}
			Result result = await _authManager.RefreshAuthorizationDataAsync();
			if (result.IsSuccess)
			{
				await LoadStatusAsync();
				if (_tokenUsageService != null)
				{
					await RefreshCreditsFromTokenServiceAsync(forceRefresh: true);
				}
				else
				{
					LoadCreditsFromCache();
				}
				Logger.Info("[AboutViewModel] ✅ 授权数据刷新完成");
				await _dialogService.ShowInfoAsync("刷新成功");
			}
			else
			{
				Logger.Error("[AboutViewModel] ❌ 刷新授权数据失败: " + result.Error);
				await _dialogService.ShowErrorAsync("刷新失败：" + result.Error);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] 刷新授权数据异常", ex);
			await _dialogService.ShowErrorAsync("刷新异常：" + ex.Message);
		}
		finally
		{
			IsLoading = false;
			IsLoadingCredits = false;
		}
	}

	[RelayCommand]
	private void OpenLogFolder()
	{
		try
		{
			string logDirectory = Logger.LogDirectory;
			if (Directory.Exists(logDirectory))
			{
				Process.Start("explorer.exe", logDirectory);
			}
			else
			{
				_dialogService.ShowError("日志文件夹不存在");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] 打开日志文件夹失败", ex);
			_dialogService.ShowError("打开失败：" + ex.Message);
		}
	}

	[RelayCommand]
	private void OpenChangelog()
	{
		try
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "https://astools.tech/changelog.html",
				UseShellExecute = true
			});
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] 打开更新日志失败", ex);
			_dialogService.ShowError("打开失败：" + ex.Message);
		}
	}

	[RelayCommand]
	private async Task RefreshDevicesAsync()
	{
		if (_authManager.CurrentUser == null)
		{
			await _dialogService.ShowErrorAsync("请先登录");
			return;
		}
		IsLoading = true;
		try
		{
			Result<List<IDeviceInfo>> result = await _authManager.GetUserDevicesAsync();
			if (!result.IsSuccess || result.Value == null)
			{
				await _dialogService.ShowErrorAsync("加载设备列表失败：" + result.Error);
				return;
			}
			UserDevices.Clear();
			string text = _authManager.CurrentDevice?.DeviceId ?? string.Empty;
			foreach (IDeviceInfo item in result.Value)
			{
				UserDevices.Add(new DeviceInfoViewModel
				{
					DeviceId = item.DeviceId,
					DeviceName = item.DeviceName,
					OsInfo = (item.OsVersion ?? "未知"),
					LastActiveAt = DateTime.Now,
					IsCurrentDevice = (item.DeviceId == text)
				});
			}
			HasNoDevices = UserDevices.Count == 0;
			DeviceCount = UserDevices.Count;
		}
		catch (Exception ex)
		{
			await _dialogService.ShowErrorAsync("加载设备列表异常：" + ex.Message);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private async Task BindCurrentDeviceAsync()
	{
		if (_authManager.CurrentUser == null)
		{
			await _dialogService.ShowErrorAsync("请先登录");
			return;
		}
		if (_authManager.CurrentDevice != null)
		{
			IsLoading = true;
			try
			{
				var (flag, text) = await _authManager.CanBindCurrentDeviceAsync();
				if (!flag)
				{
					if (text.Contains("DEVICE_LIMIT_REACHED"))
					{
						string text2 = text.Replace("DEVICE_LIMIT_REACHED|", "");
						await _dialogService.ShowWarningAsync(text2 + "\n\n请在下方设备列表中选择一台设备解绑后，再点击\"刷新设备列表\"按钮。");
					}
					else
					{
						await _dialogService.ShowErrorAsync("无法绑定设备：" + text);
					}
					return;
				}
				Result result = await _authManager.BindCurrentDeviceAsync();
				if (result.IsSuccess)
				{
					await _dialogService.ShowInfoAsync("设备绑定成功");
					await RefreshDevicesAsync();
					await LoadStatusAsync();
				}
				else
				{
					await _dialogService.ShowErrorAsync("设备绑定失败：" + result.Error);
				}
				return;
			}
			finally
			{
				IsLoading = false;
			}
		}
		await _dialogService.ShowErrorAsync("设备信息不可用");
	}

	[RelayCommand]
	private async Task UnbindDeviceAsync(string? deviceId)
	{
		if (string.IsNullOrEmpty(deviceId))
		{
			await _dialogService.ShowErrorAsync("设备 ID 不能为空");
			return;
		}
		bool isCurrentDevice = _authManager.CurrentDevice != null && _authManager.CurrentDevice.DeviceId == deviceId;
		IsLoading = true;
		try
		{
			Result result = await _authManager.UnbindDeviceAsync(deviceId);
			if (result.IsSuccess)
			{
				if (isCurrentDevice)
				{
					await _authManager.SignOutAsync();
					UserEmail = string.Empty;
				}
				else
				{
					await RefreshDevicesAsync();
					await LoadStatusAsync();
				}
			}
			else
			{
				await _dialogService.ShowErrorAsync("设备解绑失败：" + result.Error);
			}
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private void Close()
	{
		_window?.Close();
	}

	[RelayCommand]
	private void ShowPurchaseLicense()
	{
		try
		{
			PurchaseLicenseWindow purchaseLicenseWindow = new PurchaseLicenseWindow();
			IPaymentService paymentService = UIBootstrapper.TryGetService<IPaymentService>();
			IPaymentCompletionService paymentCompletionService = UIBootstrapper.TryGetService<IPaymentCompletionService>();
			PurchaseLicenseViewModel dataContext = new PurchaseLicenseViewModel(_dialogService, purchaseLicenseWindow, paymentService, paymentCompletionService);
			purchaseLicenseWindow.DataContext = dataContext;
			if (purchaseLicenseWindow.ShowDialog() == true)
			{
				LoadStatusAsync();
			}
		}
		catch (Exception ex)
		{
			_dialogService.ShowError("打开购买授权窗口失败：" + ex.Message);
		}
	}

	[RelayCommand]
	private async Task PurchaseCreditsAsync()
	{
		try
		{
			PurchaseCreditsWindow purchaseCreditsWindow = new PurchaseCreditsWindow();
			PurchaseCreditsViewModel dataContext = new PurchaseCreditsViewModel(_dialogService, purchaseCreditsWindow);
			purchaseCreditsWindow.DataContext = dataContext;
			if (purchaseCreditsWindow.ShowDialog() == true)
			{
				await RefreshAuthorizationAsync();
			}
		}
		catch (Exception ex)
		{
			await _dialogService.ShowErrorAsync("打开购买电量窗口失败：" + ex.Message);
		}
	}

	[RelayCommand]
	private void AddAIProvider()
	{
		try
		{
			AIProviderEditWindow aIProviderEditWindow = new AIProviderEditWindow();
			if (aIProviderEditWindow.ShowDialog() == true && aIProviderEditWindow.ResultConfig != null)
			{
				AIProviderViewModel item = new AIProviderViewModel(aIProviderEditWindow.ResultConfig);
				AiProviders.Add(item);
				SaveAIConfigs();
				HasNoAIConfigs = AiProviders.Count == 0;
			}
		}
		catch (Exception ex)
		{
			_dialogService.ShowError("添加AI提供商失败：" + ex.Message);
		}
	}

	[RelayCommand]
	private void EditAIProvider(string? configId)
	{
		if (string.IsNullOrEmpty(configId))
		{
			_dialogService.ShowError("配置ID不能为空");
			return;
		}
		try
		{
			AIProviderViewModel aIProviderViewModel = AiProviders.FirstOrDefault((AIProviderViewModel p) => p.Id == configId);
			if (aIProviderViewModel == null)
			{
				_dialogService.ShowError("未找到指定的配置");
				return;
			}
			AIProviderConfig config = aIProviderViewModel.Config;
			AIProviderEditWindow aIProviderEditWindow = new AIProviderEditWindow(new AIProviderConfig
			{
				Id = config.Id,
				Provider = config.Provider,
				ModelName = config.ModelName,
				DisplayName = config.DisplayName,
				ProviderEndpoint = config.ProviderEndpoint,
				EncryptedApiKey = string.Empty,
				MaxTokens = config.MaxTokens,
				Temperature = config.Temperature,
				Priority = config.Priority,
				IsDefault = config.IsDefault
			});
			if (aIProviderEditWindow.ShowDialog() == true && aIProviderEditWindow.ResultConfig != null)
			{
				AIProviderConfig resultConfig = aIProviderEditWindow.ResultConfig;
				aIProviderViewModel.Config = resultConfig;
				SaveAIConfigs();
			}
		}
		catch (Exception ex)
		{
			_dialogService.ShowError("编辑AI提供商失败：" + ex.Message);
		}
	}

	[RelayCommand]
	private async Task DeleteAIProviderAsync(string? configId)
	{
		if (string.IsNullOrEmpty(configId))
		{
			await _dialogService.ShowErrorAsync("配置ID不能为空");
		}
		else
		{
			if (!(await _dialogService.ShowConfirmAsync("确定要删除此AI提供商配置吗？", "确认删除")))
			{
				return;
			}
			try
			{
				AIProviderViewModel aIProviderViewModel = AiProviders.FirstOrDefault((AIProviderViewModel p) => p.Id == configId);
				if (aIProviderViewModel != null)
				{
					AiProviders.Remove(aIProviderViewModel);
					SaveAIConfigs();
					HasNoAIConfigs = AiProviders.Count == 0;
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowErrorAsync("删除AI提供商失败：" + ex.Message);
			}
		}
	}

	private void LoadAIConfigs()
	{
		try
		{
			ObservableCollection<AIProviderConfig> observableCollection = _aiConfigService.LoadConfigs();
			AiProviders.Clear();
			if (observableCollection != null)
			{
				foreach (AIProviderConfig item in observableCollection)
				{
					try
					{
						AiProviders.Add(new AIProviderViewModel(item));
					}
					catch (Exception ex)
					{
						Logger.Error("[AboutViewModel] 添加AI配置失败: " + item?.DisplayName, ex);
					}
				}
			}
			HasNoAIConfigs = AiProviders.Count == 0;
		}
		catch (Exception ex2)
		{
			Logger.Error("[AboutViewModel] 加载AI配置失败", ex2);
			AiProviders.Clear();
			HasNoAIConfigs = true;
		}
	}

	private void SaveAIConfigs()
	{
		try
		{
			ObservableCollection<AIProviderConfig> configs = new ObservableCollection<AIProviderConfig>(AiProviders.Select((AIProviderViewModel p) => p.Config));
			_aiConfigService.SaveConfigs(configs);
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] 保存AI配置失败", ex);
			throw;
		}
	}

	private async Task LoadStatusAsync()
	{
		try
		{
			IUserIdentity currentUser = _authManager.CurrentUser;
			IsLoggedIn = currentUser != null;
			IDeviceInfo currentDevice = _authManager.CurrentDevice;
			if (currentDevice != null)
			{
				DeviceId = currentDevice.DeviceId ?? "未知";
				try
				{
					IsCurrentDeviceBound = currentDevice.IsBound;
					DeviceStatus = (currentDevice.IsBound ? "已绑定" : "未绑定");
				}
				catch (Exception ex)
				{
					Logger.Error("[AboutViewModel] 设置设备状态失败: " + ex.Message);
					DeviceStatus = "未知";
					IsCurrentDeviceBound = false;
				}
			}
			else
			{
				DeviceId = "未知";
				DeviceStatus = "未知";
				IsCurrentDeviceBound = false;
			}
			AuthorizationState authorizationState = _authManager.GetAuthorizationState();
			if (authorizationState.HasValidLicense)
			{
				Logger.Info("[AboutViewModel] 有有效许可证（从缓存）");
				ShowLicenseDetails = true;
				StatusIcon = "CheckCircle";
				StatusBrush = GreenBrush;
				LicenseStatus = "已授权";
				LicenseType = authorizationState.LicenseType ?? "未知";
				if (authorizationState.ExpiryDate.HasValue && authorizationState.ExpiryDate.Value.Year >= 2100)
				{
					LicenseExpiry = "永久";
				}
				else
				{
					LicenseExpiry = authorizationState.ExpiryDate?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "永久";
				}
				if (currentUser != null)
				{
					UserEmail = currentUser.Email;
				}
			}
			else
			{
				Logger.Info("[AboutViewModel] 无有效许可证（试用模式）");
				ShowLicenseDetails = false;
				StatusIcon = "AlertCircle";
				StatusBrush = OrangeBrush;
				LicenseStatus = "试用模式";
				LicenseType = "试用版";
				LicenseExpiry = "无限制";
				Dictionary<string, int> cachedTrialUsage = _authManager.GetCachedTrialUsage();
				if (cachedTrialUsage.Count > 0)
				{
					string text = cachedTrialUsage.Keys.FirstOrDefault();
					if (text != null)
					{
						DeviceStatus = $"试用次数剩余: {cachedTrialUsage[text]}";
					}
				}
				else
				{
					DeviceStatus = "未绑定";
				}
			}
			List<IDeviceInfo> cachedUserDevices = _authManager.GetCachedUserDevices();
			if (cachedUserDevices != null)
			{
				DeviceCount = cachedUserDevices.Count;
				Logger.Info($"[AboutViewModel] 用户设备数量: {DeviceCount}");
				UserDevices.Clear();
				string text2 = _authManager.CurrentDevice?.DeviceId ?? string.Empty;
				foreach (IDeviceInfo item in cachedUserDevices)
				{
					UserDevices.Add(new DeviceInfoViewModel
					{
						DeviceId = (item.DeviceId ?? "未知"),
						DeviceName = (item.DeviceName ?? "未命名设备"),
						OsInfo = (item.OsVersion ?? "未知"),
						LastActiveAt = DateTime.Now,
						IsCurrentDevice = (item.DeviceId == text2)
					});
				}
				HasNoDevices = UserDevices.Count == 0;
			}
			else
			{
				DeviceCount = 0;
				UserDevices.Clear();
				HasNoDevices = true;
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("[AboutViewModel] LoadStatusAsync 异常", ex2);
			DeviceId = "未知";
			DeviceStatus = "未知";
			LicenseStatus = "未知";
			StatusIcon = "AlertCircle";
			StatusBrush = RedBrush;
		}
		finally
		{
			await Task.CompletedTask;
		}
	}

	public async Task RefreshCreditsFromTokenServiceAsync(bool forceRefresh = false)
	{
		if (_tokenUsageService == null)
		{
			Logger.Warning("[AboutViewModel] TokenUsageService 不可用，无法刷新电量信息");
			return;
		}
		TokenUsageService? tokenUsageService = _tokenUsageService;
		bool forceRefresh2 = forceRefresh;
		CombinedCreditsInfo combinedCreditsInfo = await tokenUsageService.GetUserCreditsAsync(null, null, forceRefresh2);
		if (combinedCreditsInfo != null)
		{
			CurrentBalance = combinedCreditsInfo.TotalBalance;
			TotalPurchased = combinedCreditsInfo.TotalPurchased;
			TotalConsumed = combinedCreditsInfo.TotalConsumed;
			string value = (forceRefresh ? "数据库（强制刷新）" : "缓存");
			Logger.Info($"[AboutViewModel] ✅ 从{value}加载电量信息: 余额={combinedCreditsInfo.TotalBalance:F2}, 购买={combinedCreditsInfo.TotalPurchased:F2}, 消耗={combinedCreditsInfo.TotalConsumed:F2}");
		}
	}

	private void LoadCreditsFromCache()
	{
		try
		{
			CombinedCreditsInfo cachedCreditsInfo = _authManager.GetCachedCreditsInfo();
			CurrentBalance = cachedCreditsInfo.TotalBalance;
			TotalPurchased = cachedCreditsInfo.TotalPurchased;
			TotalConsumed = cachedCreditsInfo.TotalConsumed;
			Logger.Info($"[AboutViewModel] ✅ 从缓存加载完整电量信息: 余额={cachedCreditsInfo.TotalBalance:F2}, 购买={cachedCreditsInfo.TotalPurchased:F2}, 消耗={cachedCreditsInfo.TotalConsumed:F2}");
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] ❌ 从缓存加载电量信息失败: " + ex.Message, ex);
			CurrentBalance = 0m;
			TotalPurchased = 0m;
			TotalConsumed = 0m;
		}
	}

	private async Task LoadCreditsAsync()
	{
		if (_tokenUsageService == null)
		{
			Logger.Warning("[AboutViewModel] TokenUsageService 不可用，无法加载电量信息");
			return;
		}
		try
		{
			IsLoadingCredits = true;
			CombinedCreditsInfo combinedCreditsInfo = await _tokenUsageService.GetUserCreditsAsync();
			if (combinedCreditsInfo != null)
			{
				CurrentBalance = combinedCreditsInfo.TotalBalance;
				TotalPurchased = combinedCreditsInfo.TotalPurchased;
				TotalConsumed = combinedCreditsInfo.TotalConsumed;
				Logger.Info($"[AboutViewModel] ✅ 电量信息加载成功: 余额={combinedCreditsInfo.TotalBalance:F2}, 购买={combinedCreditsInfo.TotalPurchased:F2}, 消耗={combinedCreditsInfo.TotalConsumed:F2}");
			}
			else
			{
				Logger.Warning("[AboutViewModel] ⚠\ufe0f 电量信息查询返回 null");
				CurrentBalance = 0m;
				TotalPurchased = 0m;
				TotalConsumed = 0m;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] ❌ 加载电量信息失败: " + ex.Message, ex);
			CurrentBalance = 0m;
			TotalPurchased = 0m;
			TotalConsumed = 0m;
		}
		finally
		{
			IsLoadingCredits = false;
		}
	}

	private async Task CheckUnreadFeedbackAsync()
	{
		if (_feedbackService == null)
		{
			return;
		}
		try
		{
			Result<int> result = await _feedbackService.GetUnreadReplyCountAsync();
			if (result.IsSuccess)
			{
				UnreadCount = result.Value;
				HasUnreadFeedback = UnreadCount > 0;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] 检查未读反馈异常", ex);
		}
	}

	[RelayCommand]
	private void ShowFeedback()
	{
		try
		{
			if (_feedbackService == null)
			{
				_dialogService.ShowError("反馈服务不可用");
				return;
			}
			FeedbackListWindow feedbackListWindow = new FeedbackListWindow();
			FeedbackListViewModel dataContext = new FeedbackListViewModel(_feedbackService, feedbackListWindow);
			feedbackListWindow.DataContext = dataContext;
			feedbackListWindow.ShowDialog();
			CheckUnreadFeedbackAsync();
		}
		catch (Exception ex)
		{
			Logger.Error("[AboutViewModel] 打开反馈窗口异常", ex);
			_dialogService.ShowError("打开反馈窗口失败：" + ex.Message);
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnIsLoggedInChanged(bool value)
	{
		OnPropertyChanged("IsLoggedOut");
		OnPropertyChanged("ShowCurrentDeviceUnboundWarning");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnDeviceCountChanged(int value)
	{
		OnPropertyChanged("ShowCurrentDeviceUnboundWarning");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnIsCurrentDeviceBoundChanged(bool value)
	{
		OnPropertyChanged("ShowCurrentDeviceUnboundWarning");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnCurrentBalanceChanged(decimal value)
	{
		OnPropertyChanged("CreditsDisplayText");
		OnPropertyChanged("CreditsStatusText");
		OnPropertyChanged("CreditsStatusBrush");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnTotalPurchasedChanged(decimal value)
	{
		OnPropertyChanged("TotalPurchasedDisplayText");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnTotalConsumedChanged(decimal value)
	{
		OnPropertyChanged("TotalConsumedDisplayText");
	}
}
