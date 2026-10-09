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

public class ResetPasswordViewModel : ObservableObject
{
	private readonly IAuthManager _authManager;

	private readonly IDialogService _dialogService;

	private readonly Window _window;

	[ObservableProperty]
	private bool _isLoading;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? sendResetEmailCommand;

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
	public IAsyncRelayCommand SendResetEmailCommand => sendResetEmailCommand ?? (sendResetEmailCommand = new AsyncRelayCommand(SendResetEmailAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	public ResetPasswordViewModel(IAuthManager authManager, IDialogService dialogService, Window window)
	{
		_authManager = authManager;
		_dialogService = dialogService;
		_window = window;
	}

	[RelayCommand]
	private async Task SendResetEmailAsync()
	{
		if (!(_window is ResetPasswordWindow resetPasswordWindow))
		{
			return;
		}
		string email = resetPasswordWindow.GetEmail();
		if (string.IsNullOrWhiteSpace(email))
		{
			await _dialogService.ShowErrorAsync("请输入邮箱地址");
			return;
		}
		if (email.Contains("@") && email.Contains("."))
		{
			IsLoading = true;
			try
			{
				try
				{
					Result result = await _authManager.RequestPasswordResetAsync(email);
					if (!result.IsSuccess)
					{
						await _dialogService.ShowErrorAsync("发送失败：" + result.Error);
						return;
					}
					await _dialogService.ShowInfoAsync("重置邮件已发送！\n\n请检查您的邮箱，点击邮件中的链接重置密码。");
					_window.DialogResult = true;
					_window.Close();
				}
				catch (Exception ex)
				{
					await _dialogService.ShowErrorAsync("发送异常：" + ex.Message);
				}
				return;
			}
			finally
			{
				IsLoading = false;
			}
		}
		await _dialogService.ShowErrorAsync("请输入有效的邮箱地址");
	}

	[RelayCommand]
	private void Cancel()
	{
		_window.DialogResult = false;
		_window.Close();
	}
}
