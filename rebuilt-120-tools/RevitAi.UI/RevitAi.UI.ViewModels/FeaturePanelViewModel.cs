using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class FeaturePanelViewModel : ObservableObject
{
	private readonly IWindowManager _windowManager;

	private readonly IDialogService _dialogService;

	[ObservableProperty]
	private string _selectedFeature = "modeling";

	[ObservableProperty]
	private bool _isModelingPanelVisible = true;

	[ObservableProperty]
	private bool _isRoadBridgePanelVisible;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectModelingCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectRoadBridgeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closePanelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? createBeamCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? createColumnCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? createWallCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? createFloorCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? batchCreateElementsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? createRoadCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? createBridgeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? createTunnelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? roadAnnotationCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedFeature
	{
		get
		{
			return _selectedFeature;
		}
		[MemberNotNull("_selectedFeature")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedFeature, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedFeature);
				_selectedFeature = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedFeature);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsModelingPanelVisible
	{
		get
		{
			return _isModelingPanelVisible;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isModelingPanelVisible, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsModelingPanelVisible);
				_isModelingPanelVisible = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsModelingPanelVisible);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsRoadBridgePanelVisible
	{
		get
		{
			return _isRoadBridgePanelVisible;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isRoadBridgePanelVisible, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsRoadBridgePanelVisible);
				_isRoadBridgePanelVisible = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsRoadBridgePanelVisible);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectModelingCommand => selectModelingCommand ?? (selectModelingCommand = new RelayCommand(SelectModeling));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectRoadBridgeCommand => selectRoadBridgeCommand ?? (selectRoadBridgeCommand = new RelayCommand(SelectRoadBridge));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ClosePanelCommand => closePanelCommand ?? (closePanelCommand = new RelayCommand(ClosePanel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CreateBeamCommand => createBeamCommand ?? (createBeamCommand = new RelayCommand(CreateBeam));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CreateColumnCommand => createColumnCommand ?? (createColumnCommand = new RelayCommand(CreateColumn));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CreateWallCommand => createWallCommand ?? (createWallCommand = new RelayCommand(CreateWall));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CreateFloorCommand => createFloorCommand ?? (createFloorCommand = new RelayCommand(CreateFloor));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand BatchCreateElementsCommand => batchCreateElementsCommand ?? (batchCreateElementsCommand = new RelayCommand(BatchCreateElements));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CreateRoadCommand => createRoadCommand ?? (createRoadCommand = new RelayCommand(CreateRoad));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CreateBridgeCommand => createBridgeCommand ?? (createBridgeCommand = new RelayCommand(CreateBridge));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CreateTunnelCommand => createTunnelCommand ?? (createTunnelCommand = new RelayCommand(CreateTunnel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RoadAnnotationCommand => roadAnnotationCommand ?? (roadAnnotationCommand = new RelayCommand(RoadAnnotation));

	public FeaturePanelViewModel(IWindowManager windowManager, IDialogService dialogService)
	{
		_windowManager = windowManager;
		_dialogService = dialogService;
	}

	[RelayCommand]
	private void SelectModeling()
	{
		SelectedFeature = "modeling";
		IsModelingPanelVisible = true;
		IsRoadBridgePanelVisible = false;
	}

	[RelayCommand]
	private void SelectRoadBridge()
	{
		SelectedFeature = "roadbridge";
		IsModelingPanelVisible = false;
		IsRoadBridgePanelVisible = true;
	}

	[RelayCommand]
	private void ClosePanel()
	{
		_windowManager.CloseFeaturePanelWindow();
	}

	[RelayCommand]
	private void CreateBeam()
	{
		_dialogService.ShowInfo("创建梁功能即将推出", "功能开发中");
	}

	[RelayCommand]
	private void CreateColumn()
	{
		_dialogService.ShowInfo("创建柱功能即将推出", "功能开发中");
	}

	[RelayCommand]
	private void CreateWall()
	{
		_dialogService.ShowInfo("创建墙功能即将推出", "功能开发中");
	}

	[RelayCommand]
	private void CreateFloor()
	{
		_dialogService.ShowInfo("创建楼板功能即将推出", "功能开发中");
	}

	[RelayCommand]
	private void BatchCreateElements()
	{
		_dialogService.ShowInfo("批量创建构件功能即将推出", "功能开发中");
	}

	[RelayCommand]
	private void CreateRoad()
	{
		_dialogService.ShowInfo("创建道路功能即将推出", "功能开发中");
	}

	[RelayCommand]
	private void CreateBridge()
	{
		_dialogService.ShowInfo("创建桥梁功能即将推出", "功能开发中");
	}

	[RelayCommand]
	private void CreateTunnel()
	{
		_dialogService.ShowInfo("创建隧道功能即将推出", "功能开发中");
	}

	[RelayCommand]
	private void RoadAnnotation()
	{
		_dialogService.ShowInfo("道路标注功能即将推出", "功能开发中");
	}
}
