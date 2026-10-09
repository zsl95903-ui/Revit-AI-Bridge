using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using RevitAi.UI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class AIProviderViewModel : ObservableObject
{
	[ObservableProperty]
	private AIProviderConfig _config;

	[ObservableProperty]
	private bool _isSelected;

	public string Id => Config?.Id ?? string.Empty;

	public string Provider => Config?.Provider ?? string.Empty;

	public string ModelName => Config?.ModelName ?? string.Empty;

	public string DisplayName => Config?.DisplayName ?? string.Empty;

	public string ProviderDisplayName => Config?.ProviderDisplayName ?? "未知";

	public bool IsDefault => Config?.IsDefault ?? false;

	public bool IsUserConfig => Config?.IsUserConfig ?? true;

	public bool SupportsVision => Config?.SupportsVision ?? false;

	public string SourceLabel => Config?.SourceLabel ?? "未知";

	public string SourceColor => Config?.SourceColor ?? "#FF000000";

	public Brush SourceBrush
	{
		get
		{
			try
			{
				return Config?.SourceBrush ?? Brushes.Black;
			}
			catch
			{
				return Brushes.Black;
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public AIProviderConfig Config
	{
		get
		{
			return _config;
		}
		[MemberNotNull("_config")]
		set
		{
			if (!EqualityComparer<AIProviderConfig>.Default.Equals(_config, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Config);
				_config = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Config);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isSelected, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsSelected);
				_isSelected = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsSelected);
			}
		}
	}

	public AIProviderViewModel(AIProviderConfig? config)
	{
		_config = config ?? new AIProviderConfig
		{
			Id = Guid.NewGuid().ToString(),
			Provider = "unknown",
			ModelName = "Unknown",
			DisplayName = "未知配置",
			IsUserConfig = true
		};
		_isSelected = false;
	}
}
