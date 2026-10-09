using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class LoginViewModel : ObservableObject
{
	private readonly IAuthManager _authManager;

	private readonly IDialogService _dialogService;

	private readonly IWindowManager _windowManager;

	[ObservableProperty]
	private string _email = string.Empty;

	[ObservableProperty]
	private string _password = string.Empty;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private string _licenseStatus = "加载中...";

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loginCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? continueAsTrialCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? signUpCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? forgotPasswordCommand;

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
	public string Password
	{
		get
		{
			return _password;
		}
		[MemberNotNull("_password")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_password, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Password);
				_password = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Password);
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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoginCommand => loginCommand ?? (loginCommand = new AsyncRelayCommand(LoginAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ContinueAsTrialCommand => continueAsTrialCommand ?? (continueAsTrialCommand = new RelayCommand(ContinueAsTrial));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SignUpCommand => signUpCommand ?? (signUpCommand = new RelayCommand(SignUp));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ForgotPasswordCommand => forgotPasswordCommand ?? (forgotPasswordCommand = new RelayCommand(ForgotPassword));

	public LoginViewModel(IAuthManager authManager, IDialogService dialogService, IWindowManager windowManager)
	{
		_authManager = authManager;
		_dialogService = dialogService;
		_windowManager = windowManager;
		UpdateLicenseStatus();
	}

	[RelayCommand]
	private async Task LoginAsync()
	{
		if (string.IsNullOrWhiteSpace(Email))
		{
			await _dialogService.ShowErrorAsync("请输入邮箱地址");
			return;
		}
		if (!string.IsNullOrWhiteSpace(Password))
		{
			IsLoading = true;
			try
			{
				try
				{
					Result<IUserIdentity> result = await _authManager.SignInAsync(Email, Password);
					if (!result.IsSuccess)
					{
						await _dialogService.ShowErrorAsync("登录失败：" + result.Error);
						return;
					}
					await _dialogService.ShowInfoAsync("登录成功！");
					_windowManager.CloseLoginWindow();
					_windowManager.ShowMainWindow();
				}
				catch (Exception ex)
				{
					await _dialogService.ShowErrorAsync("登录异常：" + ex.Message);
				}
				return;
			}
			finally
			{
				IsLoading = false;
			}
		}
		await _dialogService.ShowErrorAsync("请输入密码");
	}

	[RelayCommand]
	private void Cancel()
	{
		_windowManager.CloseLoginWindow();
	}

	[RelayCommand]
	private void ContinueAsTrial()
	{
		_dialogService.ShowInfo("试用模式暂未开放，请先注册账户");
	}

	[RelayCommand]
	private void SignUp()
	{
		try
		{
			new SignUpWindow().ShowDialog();
		}
		catch (Exception ex)
		{
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
			_dialogService.ShowError("打开重置密码窗口失败：" + ex.Message);
		}
	}

	private async void UpdateLicenseStatus()
	{
		_ = 1;
		try
		{
			Result<bool> result = await _authManager.HasValidLicenseAsync();
			if (result.IsSuccess && result.Value)
			{
				LicenseStatus = "您有有效的许可证：" + (await _authManager.GetLicenseTypeAsync()).Value;
				return;
			}
			AuthorizationState authorizationState = _authManager.GetAuthorizationState();
			if (authorizationState.TrialUsage.Count > 0)
			{
				int value = authorizationState.TrialUsage.Values.Sum();
				int count = authorizationState.TrialUsage.Count;
				LicenseStatus = $"试用模式 - 可用功能：{count} 个，总剩余次数：{value}";
			}
			else
			{
				LicenseStatus = "试用模式";
			}
		}
		catch (Exception)
		{
			LicenseStatus = "试用模式";
		}
	}
}
