using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class DeviceManagementViewModel : ObservableObject
{
	private readonly IAuthManager _authManager;

	private readonly IDialogService _dialogService;

	private readonly Window _window;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private ObservableCollection<DeviceInfoViewModel> _userDevices = new ObservableCollection<DeviceInfoViewModel>();

	[ObservableProperty]
	private bool _hasNoDevices;

	[ObservableProperty]
	private int _deviceCount;

	[ObservableProperty]
	private int _maxDevices;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string?>? unbindDeviceCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string?> UnbindDeviceCommand => unbindDeviceCommand ?? (unbindDeviceCommand = new AsyncRelayCommand<string>(UnbindDeviceAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public DeviceManagementViewModel(IAuthManager authManager, IDialogService dialogService, Window window)
	{
		_authManager = authManager;
		_dialogService = dialogService;
		_window = window;
		_isLoading = false;
		_hasNoDevices = true;
		_deviceCount = 0;
		_maxDevices = 3;
		UserDevices = new ObservableCollection<DeviceInfoViewModel>();
		LoadDevicesAsync();
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
		string message = (isCurrentDevice ? "确定要解绑当前设备吗？\n\n解绑后，当前设备将无法使用授权功能。" : "确定要解绑此设备吗？\n\n解绑后该设备将无法使用授权功能。");
		if (!(await _dialogService.ShowConfirmAsync(message, "确认解绑")))
		{
			return;
		}
		IsLoading = true;
		try
		{
			Result result = await _authManager.UnbindDeviceAsync(deviceId);
			if (result.IsSuccess)
			{
				await _dialogService.ShowInfoAsync("设备解绑成功");
				if (isCurrentDevice)
				{
					await _authManager.SignOutAsync();
					_window.DialogResult = false;
					_window.Close();
				}
				else
				{
					await LoadDevicesAsync();
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
		_window.DialogResult = true;
		_window.Close();
	}

	private async Task LoadDevicesAsync()
	{
		if (_authManager.CurrentUser == null)
		{
			await _dialogService.ShowErrorAsync("请先登录");
			_window.DialogResult = false;
			_window.Close();
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
}
