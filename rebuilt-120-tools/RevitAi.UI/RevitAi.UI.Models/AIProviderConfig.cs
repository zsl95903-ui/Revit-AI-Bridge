using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.Models;

public class AIProviderConfig : ObservableObject
{
	[ObservableProperty]
	private string _id = Guid.NewGuid().ToString();

	[ObservableProperty]
	private string _provider = "claude";

	[ObservableProperty]
	private string _modelName = string.Empty;

	[ObservableProperty]
	private string _displayName = string.Empty;

	[ObservableProperty]
	private string _providerEndpoint = string.Empty;

	[ObservableProperty]
	private string _encryptedApiKey = string.Empty;

	[ObservableProperty]
	private int _maxTokens = 4096;

	[ObservableProperty]
	private double _temperature = 1.0;

	[ObservableProperty]
	private int _priority;

	[ObservableProperty]
	private bool _isDefault;

	[ObservableProperty]
	private bool _isUserConfig = true;

	[ObservableProperty]
	private bool _supportsVision;

	[JsonIgnore]
	private Brush? _cachedSourceBrush;

	public string ProviderDisplayName => Provider switch
	{
		"deepseek" => "DeepSeek", 
		"openai" => "OpenAI", 
		"ollama" => "Ollama (本地)", 
		"vllm" => "vLLM (本地)", 
		"custom" => "自定义", 
		_ => "未知", 
	};

	public string SourceLabel
	{
		get
		{
			if (!IsUserConfig)
			{
				return "系统提供";
			}
			return "用户配置";
		}
	}

	public string SourceColor
	{
		get
		{
			if (!IsUserConfig)
			{
				return "#FF2196F3";
			}
			return "#FF4CAF50";
		}
	}

	[JsonIgnore]
	public Brush SourceBrush
	{
		get
		{
			try
			{
				if (_cachedSourceBrush == null)
				{
					Color color = (IsUserConfig ? Color.FromRgb(76, 175, 50) : Color.FromRgb(33, 150, 243));
					_cachedSourceBrush = new SolidColorBrush(color);
					((Freezable)_cachedSourceBrush).Freeze();
				}
				return _cachedSourceBrush;
			}
			catch
			{
				return Brushes.Black;
			}
		}
	}

	public bool ShowAdvanced
	{
		get
		{
			if (MaxTokens == 4096)
			{
				return Temperature != 0.7;
			}
			return true;
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Id
	{
		get
		{
			return _id;
		}
		[MemberNotNull("_id")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_id, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Id);
				_id = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Id);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Provider
	{
		get
		{
			return _provider;
		}
		[MemberNotNull("_provider")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_provider, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Provider);
				_provider = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Provider);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string ModelName
	{
		get
		{
			return _modelName;
		}
		[MemberNotNull("_modelName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_modelName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ModelName);
				_modelName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ModelName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string DisplayName
	{
		get
		{
			return _displayName;
		}
		[MemberNotNull("_displayName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_displayName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DisplayName);
				_displayName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DisplayName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string ProviderEndpoint
	{
		get
		{
			return _providerEndpoint;
		}
		[MemberNotNull("_providerEndpoint")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_providerEndpoint, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ProviderEndpoint);
				_providerEndpoint = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ProviderEndpoint);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string EncryptedApiKey
	{
		get
		{
			return _encryptedApiKey;
		}
		[MemberNotNull("_encryptedApiKey")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_encryptedApiKey, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.EncryptedApiKey);
				_encryptedApiKey = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.EncryptedApiKey);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int MaxTokens
	{
		get
		{
			return _maxTokens;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_maxTokens, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.MaxTokens);
				_maxTokens = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.MaxTokens);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double Temperature
	{
		get
		{
			return _temperature;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_temperature, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Temperature);
				_temperature = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Temperature);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int Priority
	{
		get
		{
			return _priority;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_priority, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Priority);
				_priority = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Priority);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsDefault
	{
		get
		{
			return _isDefault;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isDefault, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsDefault);
				_isDefault = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsDefault);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsUserConfig
	{
		get
		{
			return _isUserConfig;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isUserConfig, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsUserConfig);
				_isUserConfig = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsUserConfig);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool SupportsVision
	{
		get
		{
			return _supportsVision;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_supportsVision, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SupportsVision);
				_supportsVision = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SupportsVision);
			}
		}
	}
}
