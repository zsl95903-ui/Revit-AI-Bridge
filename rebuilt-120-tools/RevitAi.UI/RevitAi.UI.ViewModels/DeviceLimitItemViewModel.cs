using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class DeviceLimitItemViewModel : ObservableObject
{
	[ObservableProperty]
	private string _deviceId = string.Empty;

	[ObservableProperty]
	private string _deviceName = string.Empty;

	[ObservableProperty]
	private string _osInfo = string.Empty;

	[ObservableProperty]
	private DateTime _lastActiveAt;

	[ObservableProperty]
	private string _lastActiveString = string.Empty;

	[ObservableProperty]
	private bool _isCurrentDevice;

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
	public string DeviceName
	{
		get
		{
			return _deviceName;
		}
		[MemberNotNull("_deviceName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_deviceName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DeviceName);
				_deviceName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DeviceName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string OsInfo
	{
		get
		{
			return _osInfo;
		}
		[MemberNotNull("_osInfo")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_osInfo, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.OsInfo);
				_osInfo = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.OsInfo);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public DateTime LastActiveAt
	{
		get
		{
			return _lastActiveAt;
		}
		set
		{
			if (!EqualityComparer<DateTime>.Default.Equals(_lastActiveAt, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LastActiveAt);
				_lastActiveAt = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LastActiveAt);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string LastActiveString
	{
		get
		{
			return _lastActiveString;
		}
		[MemberNotNull("_lastActiveString")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_lastActiveString, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LastActiveString);
				_lastActiveString = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LastActiveString);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsCurrentDevice
	{
		get
		{
			return _isCurrentDevice;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isCurrentDevice, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsCurrentDevice);
				_isCurrentDevice = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsCurrentDevice);
			}
		}
	}
}
