using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Core.Authentication.Models;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class DeviceLimitViewModel : ObservableObject
{
	private readonly IAuthManager _authManager;

	private readonly IDialogService _dialogService;

	private readonly Window _window;

	private readonly string _currentDeviceId;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private ObservableCollection<DeviceLimitItemViewModel> _userDevices;

	[ObservableProperty]
	private bool _hasNoDevices;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string?>? unbindDeviceCommand;

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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<DeviceLimitItemViewModel> UserDevices
	{
		get
		{
			return _userDevices;
		}
		[MemberNotNull("_userDevices")]
		set
		{
			if (!EqualityComparer<ObservableCollection<DeviceLimitItemViewModel>>.Default.Equals(_userDevices, value))
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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string?> UnbindDeviceCommand => unbindDeviceCommand ?? (unbindDeviceCommand = new AsyncRelayCommand<string>(UnbindDeviceAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	public DeviceLimitViewModel(IAuthManager authManager, IDialogService dialogService, Window window)
	{
		_authManager = authManager;
		_dialogService = dialogService;
		_window = window;
		_isLoading = false;
		_hasNoDevices = true;
		_userDevices = new ObservableCollection<DeviceLimitItemViewModel>();
		_currentDeviceId = _authManager.CurrentDevice?.DeviceId ?? string.Empty;
		LoadDevicesAsync();
	}

	[RelayCommand]
	private async Task UnbindDeviceAsync(string? deviceId)
	{
		if (string.IsNullOrEmpty(deviceId))
		{
			await _dialogService.ShowErrorAsync("设备 ID 不能为空");
		}
		else
		{
			if (!(await _dialogService.ShowConfirmAsync("确定要解绑设备 " + deviceId.Substring(0, Math.Min(8, deviceId.Length)) + "... 吗？", "确认解绑")))
			{
				return;
			}
			IsLoading = true;
			try
			{
				Result result = await _authManager.UnbindDeviceAsync(deviceId);
				if (!result.IsSuccess)
				{
					await _dialogService.ShowErrorAsync("设备解绑失败：" + result.Error);
					return;
				}
				await LoadDevicesAsync();
				await Task.Delay(500);
				await _authManager.RefreshCurrentDeviceAsync();
				var (flag, text) = await _authManager.CanBindCurrentDeviceAsync();
				if (flag)
				{
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					string text2 = (text.Contains("DEVICE_LIMIT_REACHED") ? text.Replace("DEVICE_LIMIT_REACHED|", "") : text);
					await _dialogService.ShowWarningAsync("设备解绑成功，但仍无法绑定当前设备：" + text2);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowErrorAsync("设备解绑异常：" + ex.Message);
			}
			finally
			{
				IsLoading = false;
			}
		}
	}

	[RelayCommand]
	private void Cancel()
	{
		_window.DialogResult = false;
		_window.Close();
	}

	private async Task LoadDevicesAsync()
	{
		if (_authManager.CurrentUser == null)
		{
			await _dialogService.ShowErrorAsync("用户未登录");
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
			foreach (IDeviceInfo item in result.Value)
			{
				DateTime? dateTime = (item as DeviceInfo)?.LastSeenAt;
				bool isCurrentDevice = item.DeviceId == _currentDeviceId;
				UserDevices.Add(new DeviceLimitItemViewModel
				{
					DeviceId = item.DeviceId,
					DeviceName = item.DeviceName,
					OsInfo = (item.OsVersion ?? "未知"),
					LastActiveAt = (dateTime ?? DateTime.MinValue),
					LastActiveString = FormatLastActive(dateTime),
					IsCurrentDevice = isCurrentDevice
				});
			}
			HasNoDevices = UserDevices.Count == 0;
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

	private string FormatLastActive(DateTime? lastActive)
	{
		if (!lastActive.HasValue || lastActive.Value == DateTime.MinValue)
		{
			return "未知";
		}
		TimeSpan timeSpan = DateTime.UtcNow - lastActive.Value;
		if (timeSpan.TotalMinutes < 1.0)
		{
			return "刚刚";
		}
		if (timeSpan.TotalMinutes < 60.0)
		{
			return $"{(int)timeSpan.TotalMinutes} 分钟前";
		}
		if (timeSpan.TotalHours < 24.0)
		{
			return $"{(int)timeSpan.TotalHours} 小时前";
		}
		if (timeSpan.TotalDays < 7.0)
		{
			return $"{(int)timeSpan.TotalDays} 天前";
		}
		return lastActive.Value.ToString("yyyy-MM-dd");
	}
}
