using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class MunicipalPipelineNetworkViewModel : ObservableObject
{
	private readonly ILogger _logger;

	[ObservableProperty]
	private LayerConnectionTabViewModel _layerConnectionTab;

	[ObservableProperty]
	private LayerParameterTabViewModel _layerParameterTab;

	[ObservableProperty]
	private string _statusMessage = "请选择一种生成方式";

	[ObservableProperty]
	private bool _isLoading;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public LayerConnectionTabViewModel LayerConnectionTab
	{
		get
		{
			return _layerConnectionTab;
		}
		[MemberNotNull("_layerConnectionTab")]
		set
		{
			if (!EqualityComparer<LayerConnectionTabViewModel>.Default.Equals(_layerConnectionTab, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LayerConnectionTab);
				_layerConnectionTab = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LayerConnectionTab);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public LayerParameterTabViewModel LayerParameterTab
	{
		get
		{
			return _layerParameterTab;
		}
		[MemberNotNull("_layerParameterTab")]
		set
		{
			if (!EqualityComparer<LayerParameterTabViewModel>.Default.Equals(_layerParameterTab, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LayerParameterTab);
				_layerParameterTab = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LayerParameterTab);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string StatusMessage
	{
		get
		{
			return _statusMessage;
		}
		[MemberNotNull("_statusMessage")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_statusMessage, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StatusMessage);
				_statusMessage = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StatusMessage);
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

	public MunicipalPipelineNetworkViewModel()
	{
		_logger = ServiceProvider.GetLogger();
		LayerConnectionTab = new LayerConnectionTabViewModel();
		LayerParameterTab = new LayerParameterTabViewModel();
	}
}
