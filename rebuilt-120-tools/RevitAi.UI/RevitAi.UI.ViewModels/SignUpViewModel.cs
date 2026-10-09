using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class SignUpViewModel : ObservableObject
{
	private readonly IAuthManager _authManager;

	private readonly IDialogService _dialogService;

	private readonly Window _window;

	[ObservableProperty]
	private bool _isLoading;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? signUpCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelCommand;

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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SignUpCommand => signUpCommand ?? (signUpCommand = new AsyncRelayCommand(SignUpAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	public SignUpViewModel(IAuthManager authManager, IDialogService dialogService, Window window)
	{
		_authManager = authManager;
		_dialogService = dialogService;
		_window = window;
	}

	[RelayCommand]
	private async Task SignUpAsync()
	{
		if (!(_window is SignUpWindow signUpWindow))
		{
			return;
		}
		string email = signUpWindow.GetEmail();
		string password = signUpWindow.GetPassword();
		string confirmPassword = signUpWindow.GetConfirmPassword();
		if (string.IsNullOrWhiteSpace(email))
		{
			await _dialogService.ShowErrorAsync("请输入邮箱地址");
			return;
		}
		if (string.IsNullOrWhiteSpace(password))
		{
			await _dialogService.ShowErrorAsync("请输入密码");
			return;
		}
		if (string.IsNullOrWhiteSpace(confirmPassword))
		{
			await _dialogService.ShowErrorAsync("请确认密码");
			return;
		}
		if (password != confirmPassword)
		{
			await _dialogService.ShowErrorAsync("两次输入的密码不一致");
			return;
		}
		if (password.Length >= 6)
		{
			IsLoading = true;
			try
			{
				try
				{
					Result<IUserIdentity> result = await _authManager.SignUpAsync(email, password);
					if (!result.IsSuccess)
					{
						await _dialogService.ShowErrorAsync("注册失败：" + result.Error);
						return;
					}
					if (!(await _authManager.SignInAsync(email, password)).IsSuccess)
					{
						await _dialogService.ShowInfoAsync("注册成功！请登录您的账户。");
					}
					else
					{
						try
						{
							_ = (await _authManager.InitializeUserCreditsAsync()).IsSuccess;
						}
						catch (Exception)
						{
						}
						try
						{
							_ = (await _authManager.BindCurrentDeviceAsync()).IsSuccess;
						}
						catch (Exception)
						{
						}
						await _dialogService.ShowInfoAsync("注册成功！已自动登录并绑定当前设备。");
					}
					_window.DialogResult = true;
					_window.Close();
				}
				catch (Exception ex3)
				{
					await _dialogService.ShowErrorAsync("注册异常：" + ex3.Message);
				}
				return;
			}
			finally
			{
				IsLoading = false;
			}
		}
		await _dialogService.ShowErrorAsync("密码长度至少为 6 个字符");
	}

	[RelayCommand]
	private void Cancel()
	{
		_window.DialogResult = false;
		_window.Close();
	}
}
