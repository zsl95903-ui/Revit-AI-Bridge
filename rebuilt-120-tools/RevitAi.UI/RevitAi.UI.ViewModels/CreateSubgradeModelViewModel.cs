using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

using ServiceProvider = RevitAi.Abstractions.Loader.ServiceProvider;

namespace RevitAi.UI.ViewModels;

public class CreateSubgradeModelViewModel : ObservableObject
{
	private readonly IRoadProjectManager _projectManager;

	private readonly IRoadModelingUIService _roadModelingService;

	private readonly ILogger _logger;

	[ObservableProperty]
	private ObservableCollection<RoadProject> _roadProjects = new ObservableCollection<RoadProject>();

	private RoadProject? _selectedRoadProject;

	[ObservableProperty]
	private ObservableCollection<SubgradeLayerDisplayModel> _subgradeLayers = new ObservableCollection<SubgradeLayerDisplayModel>();

	[ObservableProperty]
	private SubgradeLayerDisplayModel? _selectedLayer;

	[ObservableProperty]
	private ObservableCollection<AncillaryStructureDisplayModel> _ancillaryStructures = new ObservableCollection<AncillaryStructureDisplayModel>();

	[ObservableProperty]
	private AncillaryStructureDisplayModel? _selectedAncillaryStructure;

	[ObservableProperty]
	private bool _splitAtIntegerStations = true;

	[ObservableProperty]
	private int _integerStationInterval = 20;

	[ObservableProperty]
	private string _statusMessage = "就绪";

	[ObservableProperty]
	private bool _isCreating;

	[ObservableProperty]
	private ObservableCollection<LayerPreviewData> _layerPreviewData = new ObservableCollection<LayerPreviewData>();

	[ObservableProperty]
	private ObservableCollection<LayerPreviewData> _ancillaryStructurePreviewData = new ObservableCollection<LayerPreviewData>();

	[ObservableProperty]
	private double _previewWidth = 280.0;

	[ObservableProperty]
	private double _previewHeight = 350.0;

	[ObservableProperty]
	private double _previewRoadWidth = 7.0;

	[ObservableProperty]
	private PlanLayoutPreviewData? _planLayoutPreviewData;

	[ObservableProperty]
	private ObservableCollection<StationParameterDisplayModel> _stationParameters = new ObservableCollection<StationParameterDisplayModel>();

	[ObservableProperty]
	private StationParameterDisplayModel? _selectedStationParameter;

	[ObservableProperty]
	private StationParameterDisplayModel _tempStationParameter = new StationParameterDisplayModel();

	[ObservableProperty]
	private double _bulkRoadWidth = 7.0;

	private bool _isSyncingSelectedToTemp;

	[ObservableProperty]
	private PlanPreviewViewState _planPreviewViewState = new PlanPreviewViewState();

	[ObservableProperty]
	private double _planPreviewWidth = 280.0;

	[ObservableProperty]
	private double _planPreviewHeight = 350.0;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? loadRoadProjectsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? navigateToRoadProjectManagementCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addLayerCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? removeSelectedLayerCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? moveLayerUpCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? moveLayerDownCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? copyLayersToOtherRoadsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? refreshPreviewCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addAncillaryStructureCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? removeSelectedAncillaryStructureCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? moveAncillaryStructureUpCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? moveAncillaryStructureDownCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? createSubgradeModelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? saveDataToRoadCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addStationParameterCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? removeSelectedStationParameterCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? refreshPlanPreviewCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? autoGenerateStationParametersCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? clearStationParametersCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? setAllRoadWidthsCommand;

	public RoadProject? SelectedRoadProject
	{
		get
		{
			return _selectedRoadProject;
		}
		set
		{
			_selectedRoadProject = value;
			OnPropertyChanged("SelectedRoadProject");
			HandleRoadProjectSelection(value);
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<RoadProject> RoadProjects
	{
		get
		{
			return _roadProjects;
		}
		[MemberNotNull("_roadProjects")]
		set
		{
			if (!EqualityComparer<ObservableCollection<RoadProject>>.Default.Equals(_roadProjects, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.RoadProjects);
				_roadProjects = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.RoadProjects);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<SubgradeLayerDisplayModel> SubgradeLayers
	{
		get
		{
			return _subgradeLayers;
		}
		[MemberNotNull("_subgradeLayers")]
		set
		{
			if (!EqualityComparer<ObservableCollection<SubgradeLayerDisplayModel>>.Default.Equals(_subgradeLayers, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SubgradeLayers);
				_subgradeLayers = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SubgradeLayers);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public SubgradeLayerDisplayModel? SelectedLayer
	{
		get
		{
			return _selectedLayer;
		}
		set
		{
			if (!EqualityComparer<SubgradeLayerDisplayModel>.Default.Equals(_selectedLayer, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedLayer);
				_selectedLayer = value;
				OnSelectedLayerChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedLayer);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<AncillaryStructureDisplayModel> AncillaryStructures
	{
		get
		{
			return _ancillaryStructures;
		}
		[MemberNotNull("_ancillaryStructures")]
		set
		{
			if (!EqualityComparer<ObservableCollection<AncillaryStructureDisplayModel>>.Default.Equals(_ancillaryStructures, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AncillaryStructures);
				_ancillaryStructures = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AncillaryStructures);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public AncillaryStructureDisplayModel? SelectedAncillaryStructure
	{
		get
		{
			return _selectedAncillaryStructure;
		}
		set
		{
			if (!EqualityComparer<AncillaryStructureDisplayModel>.Default.Equals(_selectedAncillaryStructure, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedAncillaryStructure);
				_selectedAncillaryStructure = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedAncillaryStructure);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool SplitAtIntegerStations
	{
		get
		{
			return _splitAtIntegerStations;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_splitAtIntegerStations, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SplitAtIntegerStations);
				_splitAtIntegerStations = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SplitAtIntegerStations);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int IntegerStationInterval
	{
		get
		{
			return _integerStationInterval;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_integerStationInterval, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IntegerStationInterval);
				_integerStationInterval = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IntegerStationInterval);
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
	public bool IsCreating
	{
		get
		{
			return _isCreating;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isCreating, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsCreating);
				_isCreating = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsCreating);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<LayerPreviewData> LayerPreviewData
	{
		get
		{
			return _layerPreviewData;
		}
		[MemberNotNull("_layerPreviewData")]
		set
		{
			if (!EqualityComparer<ObservableCollection<RevitAi.UI.ViewModels.LayerPreviewData>>.Default.Equals(_layerPreviewData, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LayerPreviewData);
				_layerPreviewData = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LayerPreviewData);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<LayerPreviewData> AncillaryStructurePreviewData
	{
		get
		{
			return _ancillaryStructurePreviewData;
		}
		[MemberNotNull("_ancillaryStructurePreviewData")]
		set
		{
			if (!EqualityComparer<ObservableCollection<RevitAi.UI.ViewModels.LayerPreviewData>>.Default.Equals(_ancillaryStructurePreviewData, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AncillaryStructurePreviewData);
				_ancillaryStructurePreviewData = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AncillaryStructurePreviewData);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double PreviewWidth
	{
		get
		{
			return _previewWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_previewWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PreviewWidth);
				_previewWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PreviewWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double PreviewHeight
	{
		get
		{
			return _previewHeight;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_previewHeight, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PreviewHeight);
				_previewHeight = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PreviewHeight);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double PreviewRoadWidth
	{
		get
		{
			return _previewRoadWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_previewRoadWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PreviewRoadWidth);
				_previewRoadWidth = value;
				OnPreviewRoadWidthChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PreviewRoadWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public PlanLayoutPreviewData? PlanLayoutPreviewData
	{
		get
		{
			return _planLayoutPreviewData;
		}
		set
		{
			if (!EqualityComparer<RevitAi.UI.ViewModels.PlanLayoutPreviewData>.Default.Equals(_planLayoutPreviewData, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PlanLayoutPreviewData);
				_planLayoutPreviewData = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PlanLayoutPreviewData);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<StationParameterDisplayModel> StationParameters
	{
		get
		{
			return _stationParameters;
		}
		[MemberNotNull("_stationParameters")]
		set
		{
			if (!EqualityComparer<ObservableCollection<StationParameterDisplayModel>>.Default.Equals(_stationParameters, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StationParameters);
				_stationParameters = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StationParameters);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public StationParameterDisplayModel? SelectedStationParameter
	{
		get
		{
			return _selectedStationParameter;
		}
		set
		{
			if (!EqualityComparer<StationParameterDisplayModel>.Default.Equals(_selectedStationParameter, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedStationParameter);
				_selectedStationParameter = value;
				OnSelectedStationParameterChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedStationParameter);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public StationParameterDisplayModel TempStationParameter
	{
		get
		{
			return _tempStationParameter;
		}
		[MemberNotNull("_tempStationParameter")]
		set
		{
			if (!EqualityComparer<StationParameterDisplayModel>.Default.Equals(_tempStationParameter, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.TempStationParameter);
				_tempStationParameter = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.TempStationParameter);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double BulkRoadWidth
	{
		get
		{
			return _bulkRoadWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_bulkRoadWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.BulkRoadWidth);
				_bulkRoadWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.BulkRoadWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public PlanPreviewViewState PlanPreviewViewState
	{
		get
		{
			return _planPreviewViewState;
		}
		[MemberNotNull("_planPreviewViewState")]
		set
		{
			if (!EqualityComparer<RevitAi.UI.ViewModels.PlanPreviewViewState>.Default.Equals(_planPreviewViewState, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PlanPreviewViewState);
				_planPreviewViewState = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PlanPreviewViewState);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double PlanPreviewWidth
	{
		get
		{
			return _planPreviewWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_planPreviewWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PlanPreviewWidth);
				_planPreviewWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PlanPreviewWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double PlanPreviewHeight
	{
		get
		{
			return _planPreviewHeight;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_planPreviewHeight, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PlanPreviewHeight);
				_planPreviewHeight = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PlanPreviewHeight);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand LoadRoadProjectsCommand => loadRoadProjectsCommand ?? (loadRoadProjectsCommand = new RelayCommand(LoadRoadProjects));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand NavigateToRoadProjectManagementCommand => navigateToRoadProjectManagementCommand ?? (navigateToRoadProjectManagementCommand = new RelayCommand(NavigateToRoadProjectManagement));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddLayerCommand => addLayerCommand ?? (addLayerCommand = new RelayCommand(AddLayer));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RemoveSelectedLayerCommand => removeSelectedLayerCommand ?? (removeSelectedLayerCommand = new RelayCommand(RemoveSelectedLayer));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand MoveLayerUpCommand => moveLayerUpCommand ?? (moveLayerUpCommand = new RelayCommand(MoveLayerUp));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand MoveLayerDownCommand => moveLayerDownCommand ?? (moveLayerDownCommand = new RelayCommand(MoveLayerDown));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CopyLayersToOtherRoadsCommand => copyLayersToOtherRoadsCommand ?? (copyLayersToOtherRoadsCommand = new RelayCommand(CopyLayersToOtherRoads));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RefreshPreviewCommand => refreshPreviewCommand ?? (refreshPreviewCommand = new RelayCommand(RefreshPreview));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddAncillaryStructureCommand => addAncillaryStructureCommand ?? (addAncillaryStructureCommand = new RelayCommand(AddAncillaryStructure));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RemoveSelectedAncillaryStructureCommand => removeSelectedAncillaryStructureCommand ?? (removeSelectedAncillaryStructureCommand = new RelayCommand(RemoveSelectedAncillaryStructure));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand MoveAncillaryStructureUpCommand => moveAncillaryStructureUpCommand ?? (moveAncillaryStructureUpCommand = new RelayCommand(MoveAncillaryStructureUp));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand MoveAncillaryStructureDownCommand => moveAncillaryStructureDownCommand ?? (moveAncillaryStructureDownCommand = new RelayCommand(MoveAncillaryStructureDown));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand CreateSubgradeModelCommand => createSubgradeModelCommand ?? (createSubgradeModelCommand = new AsyncRelayCommand(CreateSubgradeModelAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SaveDataToRoadCommand => saveDataToRoadCommand ?? (saveDataToRoadCommand = new RelayCommand(SaveDataToRoad));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddStationParameterCommand => addStationParameterCommand ?? (addStationParameterCommand = new RelayCommand(AddStationParameter));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RemoveSelectedStationParameterCommand => removeSelectedStationParameterCommand ?? (removeSelectedStationParameterCommand = new RelayCommand(RemoveSelectedStationParameter));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RefreshPlanPreviewCommand => refreshPlanPreviewCommand ?? (refreshPlanPreviewCommand = new RelayCommand(RefreshPlanPreview));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AutoGenerateStationParametersCommand => autoGenerateStationParametersCommand ?? (autoGenerateStationParametersCommand = new RelayCommand(AutoGenerateStationParameters));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ClearStationParametersCommand => clearStationParametersCommand ?? (clearStationParametersCommand = new RelayCommand(ClearStationParameters));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SetAllRoadWidthsCommand => setAllRoadWidthsCommand ?? (setAllRoadWidthsCommand = new RelayCommand(SetAllRoadWidths));

	public CreateSubgradeModelViewModel()
	{
		IServiceProvider services = UIBootstrapper.Services;
		_projectManager = services.GetRequiredService<IRoadProjectManager>();
		_roadModelingService = services.GetRequiredService<IRoadModelingUIService>();
		_logger = ServiceProvider.GetLogger();
		PlanLayoutPreviewData = new PlanLayoutPreviewData();
		PlanPreviewViewState = new PlanPreviewViewState();
		TempStationParameter = StationParameterDisplayModel.CreateEmpty(0.0, PreviewRoadWidth);
		PlanLayoutPreviewData = new PlanLayoutPreviewData();
		PlanPreviewViewState = new PlanPreviewViewState();
		TempStationParameter = StationParameterDisplayModel.CreateEmpty(0.0, PreviewRoadWidth);
		TempStationParameter.PropertyChanged += delegate(object? sender, PropertyChangedEventArgs args)
		{
			if (!_isSyncingSelectedToTemp && SelectedStationParameter != null)
			{
				StationParameterDisplayModel tempStationParameter = TempStationParameter;
				StationParameterDisplayModel selectedStationParameter = SelectedStationParameter;
				switch (args.PropertyName)
				{
				case "StationKm":
				case "StationKmInteger":
				case "StationMeter":
					if (!selectedStationParameter.IsLocked)
					{
						selectedStationParameter.StationKm = tempStationParameter.StationKm;
					}
					break;
				case "RoadWidth":
					selectedStationParameter.RoadWidth = tempStationParameter.RoadWidth;
					break;
				case "CrossSectionOffsetAngle":
					selectedStationParameter.CrossSectionOffsetAngle = tempStationParameter.CrossSectionOffsetAngle;
					break;
				}
			}
		};
		StationParameters.CollectionChanged += delegate(object? s, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (StationParameterDisplayModel newItem in e.NewItems)
				{
					if (newItem != null)
					{
						((INotifyPropertyChanged)newItem).PropertyChanged += delegate(object? sender, PropertyChangedEventArgs args)
						{
							if (args.PropertyName == "StationKm" || args.PropertyName == "StationKmInteger" || args.PropertyName == "StationMeter" || args.PropertyName == "RoadWidth" || args.PropertyName == "CrossSectionOffsetAngle")
							{
								UpdatePlanPreview();
							}
						};
					}
				}
			}
			UpdatePlanPreview();
		};
		LoadRoadProjects();
		InitializeDefaultLayersIfNeeded();
		UpdatePreview();
		UpdateAncillaryStructurePreview();
	}

	private void InitializeDefaultLayersIfNeeded()
	{
		if (SubgradeLayers.Count <= 0 && (SelectedRoadProject?.RoadStructureConfiguration == null || SelectedRoadProject.RoadStructureConfiguration.Layers.Count <= 0))
		{
			SubgradeLayerDisplayModel subgradeLayerDisplayModel = new SubgradeLayerDisplayModel
			{
				Name = "面层",
				WidthIncrement = 0.0,
				IncrementType = WidthIncrementType.BothSides,
				SlopeRatio = 1.0,
				Height = 100.0,
				Order = 0,
				Color = "#FF9800"
			};
			subgradeLayerDisplayModel.ParameterChanged += OnLayerParameterChanged;
			subgradeLayerDisplayModel.NameValidationFailed += OnLayerNameValidationFailed;
			SubgradeLayers.Add(subgradeLayerDisplayModel);
			SubgradeLayerDisplayModel subgradeLayerDisplayModel2 = new SubgradeLayerDisplayModel
			{
				Name = "基层",
				WidthIncrement = 0.0,
				IncrementType = WidthIncrementType.BothSides,
				SlopeRatio = 1.0,
				Height = 200.0,
				Order = 1,
				Color = "#4CAF50"
			};
			subgradeLayerDisplayModel2.ParameterChanged += OnLayerParameterChanged;
			subgradeLayerDisplayModel2.NameValidationFailed += OnLayerNameValidationFailed;
			SubgradeLayers.Add(subgradeLayerDisplayModel2);
			_logger.Info("[CreateSubgradeModelViewModel] 已初始化默认路基层（面层、基层）");
		}
	}

	private void HandleRoadProjectSelection(RoadProject? value)
	{
		if (value != null)
		{
			var (num, num2) = GetRoadStationRange();
			if (num2 > num)
			{
				StatusMessage = $"已选择路线: {value.Name}，桩号范围: K{num * 1000.0:F0}~K{num2 * 1000.0:F0}";
			}
			else
			{
				StatusMessage = "已选择路线: " + value.Name + "（无有效数据源）";
			}
			LoadDataFromRoad();
			UpdatePlanPreview();
		}
		else
		{
			SubgradeLayers.Clear();
			AncillaryStructures.Clear();
			StationParameters.Clear();
			UpdatePreview();
			UpdateAncillaryStructurePreview();
			UpdatePlanPreview();
			InitializeDefaultLayersIfNeeded();
		}
	}

	public void UpdatePreviewSize(double width, double height)
	{
		PreviewWidth = width;
		PreviewHeight = height;
		UpdatePreview();
	}

	public void UpdatePlanPreviewSize(double width, double height)
	{
		PlanPreviewWidth = width;
		PlanPreviewHeight = height;
		UpdatePlanPreview();
	}

	[RelayCommand]
	private void LoadRoadProjects()
	{
		try
		{
			IReadOnlyList<RoadProject> allProjects = _projectManager.GetAllProjects();
			RoadProjects.Clear();
			foreach (RoadProject item in allProjects)
			{
				RoadProjects.Add(item);
			}
			RoadProject activeProject = _projectManager.GetActiveProject();
			if (activeProject != null)
			{
				SelectedRoadProject = RoadProjects.FirstOrDefault((RoadProject p) => p.Id == activeProject.Id);
			}
			StatusMessage = $"已加载 {RoadProjects.Count} 条路线";
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 加载路线列表失败", ex);
			StatusMessage = "加载路线失败";
		}
	}

	[RelayCommand]
	private void NavigateToRoadProjectManagement()
	{
		try
		{
			UIBootstrapper.Services.GetRequiredService<IWindowManager>().ShowRoadProjectManagementWindow();
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 跳转到道路项目管理失败", ex);
			StatusMessage = "跳转到道路项目管理失败";
		}
	}

	[RelayCommand]
	private void AddLayer()
	{
		SubgradeLayerDisplayModel subgradeLayerDisplayModel = new SubgradeLayerDisplayModel
		{
			Name = $"新层{SubgradeLayers.Count + 1}",
			WidthIncrement = 0.0,
			IncrementType = WidthIncrementType.BothSides,
			SlopeRatio = 1.0,
			Height = 200.0,
			Order = SubgradeLayers.Count,
			Color = GetNextColor(SubgradeLayers.Count)
		};
		subgradeLayerDisplayModel.ParameterChanged += OnLayerParameterChanged;
		subgradeLayerDisplayModel.NameValidationFailed += OnLayerNameValidationFailed;
		SubgradeLayers.Add(subgradeLayerDisplayModel);
		SelectedLayer = subgradeLayerDisplayModel;
		UpdatePreview();
		StatusMessage = "已添加 " + subgradeLayerDisplayModel.Name;
	}

	[RelayCommand]
	private void RemoveSelectedLayer()
	{
		if (SelectedLayer == null)
		{
			StatusMessage = "请先选择要删除的层";
			return;
		}
		string name = SelectedLayer.Name;
		SelectedLayer.ParameterChanged -= OnLayerParameterChanged;
		SelectedLayer.NameValidationFailed -= OnLayerNameValidationFailed;
		SubgradeLayers.Remove(SelectedLayer);
		for (int i = 0; i < SubgradeLayers.Count; i++)
		{
			SubgradeLayers[i].Order = i;
		}
		SelectedLayer = null;
		UpdatePreview();
		StatusMessage = "已删除 " + name;
	}

	[RelayCommand]
	private void MoveLayerUp()
	{
		if (SelectedLayer == null || SelectedLayer.Order == 0)
		{
			return;
		}
		int num = SubgradeLayers.IndexOf(SelectedLayer);
		if (num > 0)
		{
			SubgradeLayers.Move(num, num - 1);
			for (int i = 0; i < SubgradeLayers.Count; i++)
			{
				SubgradeLayers[i].Order = i;
			}
			UpdatePreview();
		}
	}

	[RelayCommand]
	private void MoveLayerDown()
	{
		if (SelectedLayer == null || SelectedLayer.Order >= SubgradeLayers.Count - 1)
		{
			return;
		}
		int num = SubgradeLayers.IndexOf(SelectedLayer);
		if (num >= 0 && num < SubgradeLayers.Count - 1)
		{
			SubgradeLayers.Move(num, num + 1);
			for (int i = 0; i < SubgradeLayers.Count; i++)
			{
				SubgradeLayers[i].Order = i;
			}
			UpdatePreview();
		}
	}

	[RelayCommand]
	private void CopyLayersToOtherRoads()
	{
		if (SelectedRoadProject == null)
		{
			StatusMessage = "请先选择当前路线";
			return;
		}
		if (SubgradeLayers.Count == 0)
		{
			StatusMessage = "当前路基层定义为空，无需复制";
			return;
		}
		List<RoadProject> list = RoadProjects.Where((RoadProject p) => p.Id != SelectedRoadProject.Id).ToList();
		if (list.Count == 0)
		{
			MessageBox.Show("没有其他路线可供复制", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		List<RoadProject> list2 = ShowCopyLayersDialog(list);
		if (list2 == null || list2.Count == 0)
		{
			return;
		}
		try
		{
			int num = 0;
			foreach (RoadProject item in list2)
			{
				RoadStructureConfiguration roadStructureConfiguration = new RoadStructureConfiguration
				{
					SplitAtIntegerStations = SplitAtIntegerStations,
					IntegerStationInterval = IntegerStationInterval,
					Layers = SubgradeLayers.Select((SubgradeLayerDisplayModel l) => SubgradeLayerData.FromSubgradeLayer(l.ToSubgradeLayer())).ToList()
				};
				item.RoadStructureConfiguration = roadStructureConfiguration;
				_projectManager.UpdateProject(item);
				num++;
			}
			StatusMessage = $"已复制 {SubgradeLayers.Count} 个路基层定义到 {num} 条路线";
			MessageBox.Show($"成功将 {SubgradeLayers.Count} 个路基层定义复制到 {num} 条路线", "复制成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 复制路基层定义失败", ex);
			StatusMessage = "复制失败: " + ex.Message;
			MessageBox.Show("复制失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private List<RoadProject>? ShowCopyLayersDialog(List<RoadProject> otherProjects)
	{
		CopySubgradeLayersDialog copySubgradeLayersDialog = new CopySubgradeLayersDialog(otherProjects);
		if (copySubgradeLayersDialog.ShowDialog() == true)
		{
			return copySubgradeLayersDialog.SelectedProjects;
		}
		return null;
	}

	[RelayCommand]
	private void RefreshPreview()
	{
		UpdatePreview();
		StatusMessage = "断面预览已刷新";
	}

	[RelayCommand]
	private void AddAncillaryStructure()
	{
		AncillaryStructureDisplayModel ancillaryStructureDisplayModel = AncillaryStructureDisplayModel.CreateEmpty(AncillaryStructures.Count);
		ancillaryStructureDisplayModel.ParameterChanged += OnAncillaryStructureParameterChanged;
		AncillaryStructures.Add(ancillaryStructureDisplayModel);
		UpdateAllAncillaryStructureColors();
		SelectedAncillaryStructure = ancillaryStructureDisplayModel;
		UpdatePreview();
		UpdateAncillaryStructurePreview();
		StatusMessage = "已添加 " + ancillaryStructureDisplayModel.Name;
	}

	private void UpdateAllAncillaryStructureColors()
	{
		for (int i = 0; i < AncillaryStructures.Count; i++)
		{
			AncillaryStructures[i].Color = GetRainbowColorForIndex(i);
		}
	}

	private static string GetRainbowColorForIndex(int index)
	{
		string[] array = new string[7] { "#FF9800", "#4CAF50", "#2196F3", "#9C27B0", "#F44336", "#00BCD4", "#795548" };
		return array[index % array.Length];
	}

	[RelayCommand]
	private void RemoveSelectedAncillaryStructure()
	{
		if (SelectedAncillaryStructure == null)
		{
			StatusMessage = "请先选择要删除的附属结构";
			return;
		}
		string name = SelectedAncillaryStructure.Name;
		SelectedAncillaryStructure.ParameterChanged -= OnAncillaryStructureParameterChanged;
		AncillaryStructures.Remove(SelectedAncillaryStructure);
		for (int i = 0; i < AncillaryStructures.Count; i++)
		{
			AncillaryStructures[i].Order = i;
		}
		UpdateAllAncillaryStructureColors();
		SelectedAncillaryStructure = null;
		UpdatePreview();
		UpdateAncillaryStructurePreview();
		StatusMessage = "已删除 " + name;
	}

	[RelayCommand]
	private void MoveAncillaryStructureUp()
	{
		if (SelectedAncillaryStructure == null || SelectedAncillaryStructure.Order == 0)
		{
			return;
		}
		int num = AncillaryStructures.IndexOf(SelectedAncillaryStructure);
		if (num > 0)
		{
			AncillaryStructures.Move(num, num - 1);
			for (int i = 0; i < AncillaryStructures.Count; i++)
			{
				AncillaryStructures[i].Order = i;
			}
			UpdateAllAncillaryStructureColors();
			UpdatePreview();
			UpdateAncillaryStructurePreview();
		}
	}

	[RelayCommand]
	private void MoveAncillaryStructureDown()
	{
		if (SelectedAncillaryStructure == null || SelectedAncillaryStructure.Order >= AncillaryStructures.Count - 1)
		{
			return;
		}
		int num = AncillaryStructures.IndexOf(SelectedAncillaryStructure);
		if (num >= 0 && num < AncillaryStructures.Count - 1)
		{
			AncillaryStructures.Move(num, num + 1);
			for (int i = 0; i < AncillaryStructures.Count; i++)
			{
				AncillaryStructures[i].Order = i;
			}
			UpdateAllAncillaryStructureColors();
			UpdatePreview();
			UpdateAncillaryStructurePreview();
		}
	}

	private void OnAncillaryStructureParameterChanged()
	{
		UpdatePreview();
		UpdateAncillaryStructurePreview();
	}

	[RelayCommand]
	private async Task CreateSubgradeModelAsync()
	{
		if (SelectedRoadProject == null)
		{
			MessageBox.Show("请先选择路线", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		if (SubgradeLayers.Count == 0)
		{
			MessageBox.Show("请至少添加一个路基层", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		try
		{
			RoadStructureConfiguration roadStructureConfiguration = new RoadStructureConfiguration
			{
				SplitAtIntegerStations = SplitAtIntegerStations,
				IntegerStationInterval = IntegerStationInterval,
				Layers = SubgradeLayers.Select((SubgradeLayerDisplayModel l) => SubgradeLayerData.FromSubgradeLayer(l.ToSubgradeLayer())).ToList()
			};
			SelectedRoadProject.RoadStructureConfiguration = roadStructureConfiguration;
			AncillaryStructuresConfiguration ancillaryStructuresConfiguration = new AncillaryStructuresConfiguration
			{
				Structures = AncillaryStructures.Select((AncillaryStructureDisplayModel s) => AncillaryStructureData.FromAncillaryStructure(s.ToStructure())).ToList()
			};
			SelectedRoadProject.AncillaryStructuresConfiguration = ancillaryStructuresConfiguration;
			StationParametersConfiguration stationParametersConfiguration = new StationParametersConfiguration
			{
				Parameters = StationParameters.Select((StationParameterDisplayModel p) => new StationParameterData
				{
					StationKm = p.StationKm,
					RoadWidth = p.RoadWidth,
					CrossSectionOffsetAngle = p.CrossSectionOffsetAngle,
					Note = p.Note
				}).ToList(),
				DefaultRoadWidthM = PreviewRoadWidth
			};
			SelectedRoadProject.StationParametersConfiguration = stationParametersConfiguration;
			_projectManager.UpdateProject(SelectedRoadProject);
			_logger.Info("[CreateSubgradeModelViewModel] 已自动保存设置到路线: " + SelectedRoadProject.Name);
			StatusMessage = "已保存设置，正在创建路基模型...";
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 自动保存设置失败", ex);
			StatusMessage = "保存设置失败: " + ex.Message;
			if (MessageBox.Show("自动保存设置失败: " + ex.Message + "\n是否继续创建模型？", "保存失败", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
			{
				return;
			}
		}
		IsCreating = true;
		StatusMessage = "正在创建路基模型...";
		RoadModelingUIResult result = null;
		try
		{
			result = await _roadModelingService.CreateRoadModelAsync(SelectedRoadProject, SplitAtIntegerStations, IntegerStationInterval);
		}
		catch (Exception ex2)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 创建路基模型失败", ex2);
			StatusMessage = "创建失败: " + ex2.Message;
			result = new RoadModelingUIResult
			{
				IsSuccess = false,
				Error = ex2.Message
			};
		}
		finally
		{
			IsCreating = false;
		}
		if (result.IsSuccess)
		{
			StatusMessage = "路基模型创建成功: " + result.Message;
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				CreateSubgradeModelWindow createSubgradeModelWindow = Application.Current?.Windows.OfType<CreateSubgradeModelWindow>().FirstOrDefault();
				if (createSubgradeModelWindow != null)
				{
					createSubgradeModelWindow.Topmost = true;
					createSubgradeModelWindow.Activate();
				}
				MessageBox.Show($"路基模型创建成功！\n\n创建实例数量: {result.CreatedInstanceCount}\n创建族类型数量: {result.CreatedTypeCount}\n创建材质数量: {result.CreatedMaterialCount}\n\n按层统计:\n{string.Join("\n", result.LayerInstanceCounts.Select<KeyValuePair<string, int>, string>((KeyValuePair<string, int> kvp) => $"  {kvp.Key}: {kvp.Value} 个"))}", "成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				if (createSubgradeModelWindow != null)
				{
					createSubgradeModelWindow.Topmost = false;
				}
			}, (DispatcherPriority)4, Array.Empty<object>());
			return;
		}
		StatusMessage = "创建失败: " + result.Error;
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			CreateSubgradeModelWindow createSubgradeModelWindow = Application.Current?.Windows.OfType<CreateSubgradeModelWindow>().FirstOrDefault();
			if (createSubgradeModelWindow != null)
			{
				createSubgradeModelWindow.Topmost = true;
				createSubgradeModelWindow.Activate();
			}
			MessageBox.Show("创建失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			if (createSubgradeModelWindow != null)
			{
				createSubgradeModelWindow.Topmost = false;
			}
		}, (DispatcherPriority)4, Array.Empty<object>());
	}

	[RelayCommand]
	private void Close()
	{
		Application.Current?.Windows.OfType<CreateSubgradeModelWindow>().FirstOrDefault()?.Close();
	}

	[RelayCommand]
	private void SaveDataToRoad()
	{
		if (SelectedRoadProject == null)
		{
			MessageBox.Show("请先选择路线", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		try
		{
			RoadStructureConfiguration roadStructureConfiguration = new RoadStructureConfiguration
			{
				SplitAtIntegerStations = SplitAtIntegerStations,
				IntegerStationInterval = IntegerStationInterval,
				Layers = SubgradeLayers.Select((SubgradeLayerDisplayModel l) => SubgradeLayerData.FromSubgradeLayer(l.ToSubgradeLayer())).ToList()
			};
			SelectedRoadProject.RoadStructureConfiguration = roadStructureConfiguration;
			AncillaryStructuresConfiguration ancillaryStructuresConfiguration = new AncillaryStructuresConfiguration
			{
				Structures = AncillaryStructures.Select((AncillaryStructureDisplayModel s) => AncillaryStructureData.FromAncillaryStructure(s.ToStructure())).ToList()
			};
			SelectedRoadProject.AncillaryStructuresConfiguration = ancillaryStructuresConfiguration;
			StationParametersConfiguration stationParametersConfiguration = new StationParametersConfiguration
			{
				Parameters = StationParameters.Select((StationParameterDisplayModel p) => new StationParameterData
				{
					StationKm = p.StationKm,
					RoadWidth = p.RoadWidth,
					CrossSectionOffsetAngle = p.CrossSectionOffsetAngle,
					Note = p.Note
				}).ToList(),
				DefaultRoadWidthM = PreviewRoadWidth
			};
			SelectedRoadProject.StationParametersConfiguration = stationParametersConfiguration;
			_projectManager.UpdateProject(SelectedRoadProject);
			StatusMessage = "已保存结构数据到路线: " + SelectedRoadProject.Name;
			MessageBox.Show("已保存到路线: " + SelectedRoadProject.Name, "保存成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 保存数据失败", ex);
			StatusMessage = "保存失败: " + ex.Message;
			MessageBox.Show("保存失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void LoadDataFromRoad()
	{
		if (SelectedRoadProject == null)
		{
			return;
		}
		try
		{
			bool flag = false;
			if (SelectedRoadProject.RoadStructureConfiguration != null && SelectedRoadProject.RoadStructureConfiguration.Layers.Count > 0)
			{
				RoadStructureConfiguration roadStructureConfiguration = SelectedRoadProject.RoadStructureConfiguration;
				SplitAtIntegerStations = roadStructureConfiguration.SplitAtIntegerStations;
				IntegerStationInterval = roadStructureConfiguration.IntegerStationInterval;
				SubgradeLayers.Clear();
				foreach (SubgradeLayerData item2 in roadStructureConfiguration.Layers.OrderBy((SubgradeLayerData l) => l.Order))
				{
					SubgradeLayerDisplayModel subgradeLayerDisplayModel = new SubgradeLayerDisplayModel
					{
						Name = item2.Name,
						WidthIncrement = item2.WidthIncrement,
						IncrementType = item2.IncrementType,
						SlopeRatio = item2.SlopeRatio,
						Height = item2.Height,
						Order = item2.Order,
						Color = item2.Color
					};
					subgradeLayerDisplayModel.ParameterChanged += OnLayerParameterChanged;
					subgradeLayerDisplayModel.NameValidationFailed += OnLayerNameValidationFailed;
					SubgradeLayers.Add(subgradeLayerDisplayModel);
				}
				flag = true;
				_logger.Info($"[CreateSubgradeModelViewModel] 已加载路面结构配置: {roadStructureConfiguration.Layers.Count} 层");
			}
			else
			{
				SubgradeLayers.Clear();
				InitializeDefaultLayersIfNeeded();
				_logger.Info("[CreateSubgradeModelViewModel] 项目无路面结构配置，已清空并初始化默认层");
			}
			if (SelectedRoadProject.AncillaryStructuresConfiguration != null)
			{
				AncillaryStructuresConfiguration ancillaryStructuresConfiguration = SelectedRoadProject.AncillaryStructuresConfiguration;
				AncillaryStructures.Clear();
				foreach (AncillaryStructureData item3 in ancillaryStructuresConfiguration.Structures.OrderBy((AncillaryStructureData s) => s.Order))
				{
					AncillaryStructureDisplayModel ancillaryStructureDisplayModel = AncillaryStructureDisplayModel.FromStructure(item3.ToAncillaryStructure());
					ancillaryStructureDisplayModel.ParameterChanged += OnAncillaryStructureParameterChanged;
					AncillaryStructures.Add(ancillaryStructureDisplayModel);
				}
				flag = true;
				_logger.Info($"[CreateSubgradeModelViewModel] 已加载附属结构配置: {ancillaryStructuresConfiguration.Structures.Count} 个结构");
			}
			if (SelectedRoadProject.StationParametersConfiguration != null)
			{
				StationParametersConfiguration stationParametersConfiguration = SelectedRoadProject.StationParametersConfiguration;
				StationParameters.Clear();
				foreach (StationParameterData item4 in stationParametersConfiguration.Parameters.OrderBy((StationParameterData p) => p.StationKm))
				{
					StationParameterDisplayModel item = StationParameterDisplayModel.FromParameter(item4.ToStationParameter());
					StationParameters.Add(item);
				}
				EnsureStartEndStations();
				flag = true;
				_logger.Info($"[CreateSubgradeModelViewModel] 已加载桩号参数配置: {stationParametersConfiguration.Parameters.Count} 个桩号");
			}
			else
			{
				StationParameters.Clear();
				_logger.Info("[CreateSubgradeModelViewModel] 路线无桩号参数配置，已清空桩号参数列表");
				var (num, num2) = GetRoadStationRange();
				if (num2 > num)
				{
					StationParameterDisplayModel stationParameterDisplayModel = StationParameterDisplayModel.CreateEmpty(num, PreviewRoadWidth);
					stationParameterDisplayModel.IsLocked = true;
					StationParameters.Add(stationParameterDisplayModel);
					StationParameterDisplayModel stationParameterDisplayModel2 = StationParameterDisplayModel.CreateEmpty(num2, PreviewRoadWidth);
					stationParameterDisplayModel2.IsLocked = true;
					StationParameters.Add(stationParameterDisplayModel2);
					_logger.Info("[CreateSubgradeModelViewModel] 已添加新路线起终点: " + stationParameterDisplayModel.StationDisplay + " ~ " + stationParameterDisplayModel2.StationDisplay);
				}
			}
			UpdatePreview();
			UpdateAncillaryStructurePreview();
			UpdatePlanPreview();
			if (flag)
			{
				StatusMessage = "已从路线 '" + SelectedRoadProject.Name + "' 加载结构数据";
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 加载路线数据失败", ex);
		}
	}

	[RelayCommand]
	private void AddStationParameter()
	{
		if (SelectedRoadProject == null)
		{
			StatusMessage = "请先选择路线";
			return;
		}
		(double start, double end) roadStationRange = GetRoadStationRange();
		double item = roadStationRange.start;
		double item2 = roadStationRange.end;
		double stationKm = TempStationParameter.StationKm;
		if (stationKm <= item || stationKm >= item2)
		{
			StatusMessage = $"桩号需在起终点之间 (K{item * 1000.0:F0}~K{item2 * 1000.0:F0})";
			return;
		}
		StationParameterDisplayModel stationParameterDisplayModel = new StationParameterDisplayModel
		{
			Parameter = new StationParameter
			{
				StationKm = stationKm,
				RoadWidth = TempStationParameter.RoadWidth,
				CrossSectionOffsetAngle = TempStationParameter.CrossSectionOffsetAngle,
				Note = TempStationParameter.Note
			},
			StationKm = stationKm,
			RoadWidth = TempStationParameter.RoadWidth,
			CrossSectionOffsetAngle = TempStationParameter.CrossSectionOffsetAngle
		};
		StationParameters.Add(stationParameterDisplayModel);
		List<StationParameterDisplayModel> list = StationParameters.OrderBy((StationParameterDisplayModel p) => p.StationKm).ToList();
		StationParameters.Clear();
		foreach (StationParameterDisplayModel item3 in list)
		{
			StationParameters.Add(item3);
		}
		UpdatePlanPreview();
		StatusMessage = "已添加桩号参数: " + stationParameterDisplayModel.StationDisplay;
		TempStationParameter.StationKm = 0.0;
		TempStationParameter.RoadWidth = PreviewRoadWidth;
		TempStationParameter.CrossSectionOffsetAngle = 0.0;
		TempStationParameter.Note = null;
	}

	[RelayCommand]
	private void RemoveSelectedStationParameter()
	{
		if (SelectedStationParameter == null)
		{
			StatusMessage = "请先选择要删除的桩号参数";
			return;
		}
		if (SelectedStationParameter.IsLocked)
		{
			StatusMessage = "起终点桩号不可删除";
			return;
		}
		string stationDisplay = SelectedStationParameter.StationDisplay;
		StationParameters.Remove(SelectedStationParameter);
		SelectedStationParameter = null;
		UpdatePlanPreview();
		StatusMessage = "已删除桩号参数: " + stationDisplay;
	}

	[RelayCommand]
	private void RefreshPlanPreview()
	{
		UpdatePlanPreview();
		StatusMessage = "平面预览已刷新";
	}

	[RelayCommand]
	private void AutoGenerateStationParameters()
	{
		if (SelectedRoadProject == null)
		{
			StatusMessage = "请先选择路线";
			return;
		}
		double num = 0.0;
		double num2 = 0.0;
		bool flag = false;
		if (SelectedRoadProject.HorizontalCurveTable != null)
		{
			List<HorizontalCurveIP> sortedPoints = SelectedRoadProject.HorizontalCurveTable.GetSortedPoints();
			if (sortedPoints.Count > 1)
			{
				num = sortedPoints[0].Station;
				num2 = sortedPoints[sortedPoints.Count - 1].Station;
				flag = true;
			}
		}
		if (!flag && SelectedRoadProject.StationElevationData != null && SelectedRoadProject.StationElevationData.Count > 0)
		{
			List<RoadCenterlinePoint3D> list = SelectedRoadProject.StationElevationData.OrderBy((RoadCenterlinePoint3D p) => p.StationKm).ToList();
			num = list[0].StationKm;
			num2 = list[list.Count - 1].StationKm;
			flag = true;
		}
		if (!flag)
		{
			StatusMessage = "路线数据无效，无法自动生成桩号参数（需要平纵曲线表或桩号高程表）";
			return;
		}
		int integerStationInterval = IntegerStationInterval;
		StationParameters.Clear();
		double num3 = num * 1000.0;
		double num4 = num2 * 1000.0;
		for (double num5 = Math.Ceiling(num3 / (double)integerStationInterval) * (double)integerStationInterval; num5 <= num4 + 0.001; num5 += (double)integerStationInterval)
		{
			if (num5 > num4)
			{
				num5 = num4;
			}
			StationParameterDisplayModel item = StationParameterDisplayModel.CreateEmpty(num5 / 1000.0, PreviewRoadWidth);
			StationParameters.Add(item);
			if (num5 >= num4)
			{
				break;
			}
		}
		EnsureStartEndStations();
		UpdatePlanPreview();
		StatusMessage = $"已自动生成 {StationParameters.Count} 个桩号参数（起终点已锁定）";
	}

	[RelayCommand]
	private void ClearStationParameters()
	{
		List<StationParameterDisplayModel> list = StationParameters.Where((StationParameterDisplayModel p) => p.IsLocked).ToList();
		if (StationParameters.Count == list.Count)
		{
			StatusMessage = "没有可删除的桩号参数";
			return;
		}
		foreach (StationParameterDisplayModel item in StationParameters.Where((StationParameterDisplayModel p) => !p.IsLocked).ToList())
		{
			StationParameters.Remove(item);
		}
		SelectedStationParameter = null;
		UpdatePlanPreview();
		StatusMessage = "已清空桩号参数列表（保留起终点）";
	}

	[RelayCommand]
	private void SetAllRoadWidths()
	{
		if (StationParameters.Count == 0)
		{
			StatusMessage = "桩号参数列表为空";
			return;
		}
		double bulkRoadWidth = BulkRoadWidth;
		if (bulkRoadWidth <= 0.0)
		{
			StatusMessage = "道路宽度必须大于 0";
			return;
		}
		int num = 0;
		foreach (StationParameterDisplayModel stationParameter in StationParameters)
		{
			if (stationParameter.RoadWidth != bulkRoadWidth)
			{
				stationParameter.RoadWidth = bulkRoadWidth;
				num++;
			}
		}
		UpdatePlanPreview();
		StatusMessage = $"已设置 {StationParameters.Count} 个桩号的道路宽度为 {bulkRoadWidth:F2}m";
	}

	private void EnsureStartEndStations()
	{
		if (SelectedRoadProject == null)
		{
			return;
		}
		var (startStation, endStation) = GetRoadStationRange();
		if (!(endStation <= startStation))
		{
			StationParameterDisplayModel stationParameterDisplayModel = StationParameters.FirstOrDefault((StationParameterDisplayModel p) => Math.Abs(p.StationKm - startStation) < 0.001);
			if (stationParameterDisplayModel == null)
			{
				stationParameterDisplayModel = StationParameterDisplayModel.CreateEmpty(startStation, PreviewRoadWidth);
				stationParameterDisplayModel.IsLocked = true;
				StationParameters.Insert(0, stationParameterDisplayModel);
				_logger.Info("[CreateSubgradeModelViewModel] 已添加起点桩号: " + stationParameterDisplayModel.StationDisplay);
			}
			else
			{
				stationParameterDisplayModel.IsLocked = true;
			}
			StationParameterDisplayModel stationParameterDisplayModel2 = StationParameters.FirstOrDefault((StationParameterDisplayModel p) => Math.Abs(p.StationKm - endStation) < 0.001);
			if (stationParameterDisplayModel2 == null)
			{
				stationParameterDisplayModel2 = StationParameterDisplayModel.CreateEmpty(endStation, PreviewRoadWidth);
				stationParameterDisplayModel2.IsLocked = true;
				StationParameters.Add(stationParameterDisplayModel2);
				_logger.Info("[CreateSubgradeModelViewModel] 已添加终点桩号: " + stationParameterDisplayModel2.StationDisplay);
			}
			else
			{
				stationParameterDisplayModel2.IsLocked = true;
			}
		}
	}

	private (double start, double end) GetRoadStationRange()
	{
		if (SelectedRoadProject == null)
		{
			return (start: 0.0, end: 0.0);
		}
		if (SelectedRoadProject.HorizontalCurveTable != null)
		{
			List<HorizontalCurveIP> sortedPoints = SelectedRoadProject.HorizontalCurveTable.GetSortedPoints();
			if (sortedPoints.Count > 1)
			{
				return (start: sortedPoints[0].Station, end: sortedPoints[sortedPoints.Count - 1].Station);
			}
		}
		if (SelectedRoadProject.StationElevationData != null && SelectedRoadProject.StationElevationData.Count > 0)
		{
			List<RoadCenterlinePoint3D> list = SelectedRoadProject.StationElevationData.OrderBy((RoadCenterlinePoint3D p) => p.StationKm).ToList();
			return (start: list[0].StationKm, end: list[list.Count - 1].StationKm);
		}
		return (start: 0.0, end: 0.0);
	}

	private void UpdatePreview()
	{
		try
		{
			LayerPreviewData.Clear();
			foreach (LayerPreviewData item in GenerateCrossSectionPreview(includeAncillaryStructures: false))
			{
				LayerPreviewData.Add(item);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 更新断面预览失败", ex);
		}
	}

	public void UpdateAncillaryStructurePreview()
	{
		try
		{
			AncillaryStructurePreviewData.Clear();
			foreach (LayerPreviewData item in GenerateCrossSectionPreview(includeAncillaryStructures: true))
			{
				AncillaryStructurePreviewData.Add(item);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 更新附属结构预览失败", ex);
		}
	}

	private List<LayerPreviewData> GenerateCrossSectionPreview(bool includeAncillaryStructures)
	{
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		List<LayerPreviewData> list = new List<LayerPreviewData>();
		double previewWidth = PreviewWidth;
		double previewHeight = PreviewHeight;
		int num = 20;
		double num2 = GetDefaultRoadWidth() * 1000.0;
		List<(double, double, double, double)> list2 = new List<(double, double, double, double)>();
		double num3 = num2;
		double num4 = 0.0;
		double baseWidth = num2;
		foreach (SubgradeLayerDisplayModel item18 in SubgradeLayers.OrderBy((SubgradeLayerDisplayModel l) => l.Order))
		{
			double topWidth = item18.GetTopWidth(baseWidth);
			double bottomWidth = item18.GetBottomWidth(baseWidth);
			list2.Add((topWidth, bottomWidth, num4, num4 + item18.Height));
			if (bottomWidth > num3)
			{
				num3 = bottomWidth;
			}
			num4 += item18.Height;
			baseWidth = bottomWidth;
		}
		List<(AncillaryStructureDisplayModel, double, double, double, double)> list3 = new List<(AncillaryStructureDisplayModel, double, double, double, double)>();
		double num5 = num2 / 2.0;
		if (includeAncillaryStructures)
		{
			foreach (AncillaryStructureDisplayModel item19 in from s in AncillaryStructures
				where s.IsEnabled
				orderby s.Order
				select s)
			{
				switch (item19.PositionType)
				{
				case AncillaryPositionType.Left:
				{
					double num12 = 0.0 - num5 - item19.HorizontalOffset - item19.Width;
					double item7 = 0.0 - num5 - item19.HorizontalOffset;
					double num13 = 0.0 - item19.ElevationDifference;
					double item8 = num13 + item19.Height;
					list3.Add((item19, num12, item7, num13, item8));
					if (Math.Abs(num12) > num3 / 2.0)
					{
						num3 = Math.Abs(num12) * 2.0;
					}
					break;
				}
				case AncillaryPositionType.Right:
				{
					double item5 = num5 + item19.HorizontalOffset;
					double num10 = num5 + item19.HorizontalOffset + item19.Width;
					double num11 = 0.0 - item19.ElevationDifference;
					double item6 = num11 + item19.Height;
					list3.Add((item19, item5, num10, num11, item6));
					if (Math.Abs(num10) > num3 / 2.0)
					{
						num3 = Math.Abs(num10) * 2.0;
					}
					break;
				}
				case AncillaryPositionType.Both:
				{
					double num6 = 0.0 - num5 - item19.HorizontalOffset - item19.Width;
					double item = 0.0 - num5 - item19.HorizontalOffset;
					double num7 = 0.0 - item19.ElevationDifference;
					double item2 = num7 + item19.Height;
					list3.Add((item19, num6, item, num7, item2));
					double item3 = num5 + item19.HorizontalOffset;
					double num8 = num5 + item19.HorizontalOffset + item19.Width;
					double num9 = 0.0 - item19.ElevationDifference;
					double item4 = num9 + item19.Height;
					list3.Add((item19, item3, num8, num9, item4));
					if (Math.Abs(num6) > num3 / 2.0)
					{
						num3 = Math.Abs(num6) * 2.0;
					}
					if (Math.Abs(num8) > num3 / 2.0)
					{
						num3 = Math.Abs(num8) * 2.0;
					}
					break;
				}
				}
			}
		}
		double num14 = 0.0;
		double num15 = num4;
		if (includeAncillaryStructures && list3.Count > 0)
		{
			num14 = list3.Min<(AncillaryStructureDisplayModel, double, double, double, double)>(((AncillaryStructureDisplayModel structure, double leftX, double rightX, double topY, double bottomY) r) => r.topY);
			num15 = Math.Max(num4, list3.Max<(AncillaryStructureDisplayModel, double, double, double, double)>(((AncillaryStructureDisplayModel structure, double leftX, double rightX, double topY, double bottomY) r) => r.bottomY));
		}
		double num16 = num15 - num14;
		if (num3 <= 0.0 || num16 <= 0.0)
		{
			return list;
		}
		double num17 = previewWidth - (double)(2 * num);
		double num18 = previewHeight - (double)(2 * num);
		double val = num17 / num3;
		double val2 = num18 / num16;
		double num19 = Math.Min(val, val2);
		double num20 = previewWidth / 2.0;
		double num21 = (double)num - num14 * num19;
		int num22 = 0;
		foreach (SubgradeLayerDisplayModel item20 in SubgradeLayers.OrderBy((SubgradeLayerDisplayModel l) => l.Order))
		{
			(double, double, double, double) tuple = list2[num22];
			double item9 = tuple.Item1;
			double item10 = tuple.Item2;
			double item11 = tuple.Item3;
			double item12 = tuple.Item4;
			num22++;
			PointCollection points = new PointCollection
			{
				new Point(num20 - item9 / 2.0 * num19, num21 + item11 * num19),
				new Point(num20 + item9 / 2.0 * num19, num21 + item11 * num19),
				new Point(num20 + item10 / 2.0 * num19, num21 + item12 * num19),
				new Point(num20 - item10 / 2.0 * num19, num21 + item12 * num19)
			};
			list.Add(new LayerPreviewData
			{
				Points = points,
				FillColor = (includeAncillaryStructures ? "#CCCCCC" : item20.Color),
				Name = item20.Name,
				StrokeThickness = 0.8
			});
		}
		if (includeAncillaryStructures)
		{
			foreach (var item21 in list3)
			{
				AncillaryStructureDisplayModel item13 = item21.Item1;
				double item14 = item21.Item2;
				double item15 = item21.Item3;
				double item16 = item21.Item4;
				double item17 = item21.Item5;
				PointCollection points2 = new PointCollection
				{
					new Point(num20 + item14 * num19, num21 + item16 * num19),
					new Point(num20 + item15 * num19, num21 + item16 * num19),
					new Point(num20 + item15 * num19, num21 + item17 * num19),
					new Point(num20 + item14 * num19, num21 + item17 * num19)
				};
				list.Add(new LayerPreviewData
				{
					Points = points2,
					FillColor = item13.Color,
					Name = item13.Name,
					StrokeThickness = 0.8
				});
			}
		}
		return list;
	}

	private double GetDefaultRoadWidth()
	{
		return PreviewRoadWidth;
	}

	private void OnLayerParameterChanged()
	{
		UpdatePreview();
		UpdateAncillaryStructurePreview();
	}

	private void OnLayerNameValidationFailed(string invalidName)
	{
		int num = invalidName.IndexOfAny(SubgradeLayerDisplayModel.ForbiddenCharacters);
		char invalidChar = ((num >= 0) ? invalidName[num] : ' ');
		_logger.Warning($"[层名称验证] 层名称包含禁止字符: '{invalidChar}'");
		StatusMessage = $"错误: 层名称不能包含字符 '{invalidChar}'，请修正后重新输入";
		Application current = Application.Current;
		if (current != null)
		{
			((DispatcherObject)current).Dispatcher.Invoke((Action)delegate
			{
				MessageBox.Show($"层名称 '{invalidName}' 包含禁止字符 '{invalidChar}'\n\nRevit 类型名称不能包含以下字符:\n\\ : {{ }} [ ] | ; < > ? ` ~\n\n请修改层名称。", "名称验证失败", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			});
		}
	}

	private void UpdatePlanPreview()
	{
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (PlanLayoutPreviewData == null)
			{
				PlanLayoutPreviewData = new PlanLayoutPreviewData();
			}
			PlanLayoutPreviewData.CenterlinePoints = null;
			PlanLayoutPreviewData.LeftBoundaryPoints = null;
			PlanLayoutPreviewData.RightBoundaryPoints = null;
			PlanLayoutPreviewData.StationMarkers.Clear();
			PlanLayoutPreviewData.CrossSectionLines.Clear();
			PlanLayoutPreviewData.Bounds = null;
			PlanLayoutPreviewData.TotalLength = null;
			PlanLayoutPreviewData.StartStationDisplay = null;
			PlanLayoutPreviewData.EndStationDisplay = null;
			if (SelectedRoadProject == null)
			{
				return;
			}
			double planPreviewWidth = PlanPreviewWidth;
			double planPreviewHeight = PlanPreviewHeight;
			if (planPreviewWidth <= 0.0 || planPreviewHeight <= 0.0)
			{
				return;
			}
			List<(double X, double Y, double StationKm)> list;
			switch (SelectedRoadProject.DataSource)
			{
			default:
				return;
			case RoadCenterlineDataSource.StationElevation:
				if (SelectedRoadProject.StationElevationData == null || SelectedRoadProject.StationElevationData.Count == 0)
				{
					return;
				}
				list = SelectedRoadProject.StationElevationData.Select((RoadCenterlinePoint3D p) => (X: p.X, Y: p.Y, StationKm: p.StationKm)).ToList();
				break;
			case RoadCenterlineDataSource.HorizontalVerticalCurve:
				if (SelectedRoadProject.HorizontalCurveTable == null || !SelectedRoadProject.HorizontalCurveTable.Validate())
				{
					return;
				}
				list = CalculatePlanarPointsFromHorizontalCurve(SelectedRoadProject.HorizontalCurveTable, 0.001);
				break;
			}
			if (list.Count == 0)
			{
				return;
			}
			IList<(double, double, double)> list2;
			if (list.Count <= 500)
			{
				list2 = list;
			}
			else
			{
				double num = (double)(list.Count - 1) / 499.0;
				list2 = new List<(double, double, double)>(500);
				for (int num2 = 0; num2 < 500; num2++)
				{
					int num3 = (int)Math.Round((double)num2 * num);
					if (num3 >= list.Count)
					{
						num3 = list.Count - 1;
					}
					list2.Add(list[num3]);
				}
			}
			List<double> source = list.Select<(double, double, double), double>(((double X, double Y, double StationKm) p) => p.X).ToList();
			List<double> source2 = list.Select<(double, double, double), double>(((double X, double Y, double StationKm) p) => p.Y).ToList();
			double num4 = source.Min();
			double num5 = source.Max();
			double num6 = source2.Min();
			double num7 = source2.Max();
			int num8 = 40;
			double num9 = planPreviewWidth - (double)(2 * num8);
			double num10 = planPreviewHeight - (double)(2 * num8);
			double num11 = num5 - num4;
			double num12 = num7 - num6;
			if (num11 < 0.001)
			{
				num11 = 100.0;
			}
			if (num12 < 0.001)
			{
				num12 = 100.0;
			}
			double val = num9 / num11;
			double val2 = num10 / num12;
			double num13 = Math.Min(val, val2);
			double num14 = (double)num8 + (num9 - num11 * num13) / 2.0 - num4 * num13;
			double num15 = (double)num8 + (num10 - num12 * num13) / 2.0 - num6 * num13;
			PointCollection pointCollection = new PointCollection();
			foreach (var item2 in list2)
			{
				double num16 = item2.Item1 * num13 + num14;
				double num17 = planPreviewHeight - (item2.Item2 * num13 + num15);
				pointCollection.Add(new Point(num16, num17));
			}
			PlanLayoutPreviewData.CenterlinePoints = pointCollection;
			PlanLayoutPreviewData.Bounds = new Rect(0.0, 0.0, planPreviewWidth, planPreviewHeight);
			double num18 = list[list.Count - 1].Item3 - list[0].Item3;
			PlanLayoutPreviewData.TotalLength = num18 * 1000.0;
			PlanLayoutPreviewData.StartStationDisplay = FormatStationKm(list[0].Item3);
			PlanLayoutPreviewData.EndStationDisplay = FormatStationKm(list[list.Count - 1].Item3);
			Dictionary<double, (double, double)> dictionary = new Dictionary<double, (double, double)>();
			foreach (var item3 in list)
			{
				double key = Math.Round(item3.Item3, 3);
				if (!dictionary.ContainsKey(key))
				{
					dictionary[key] = (item3.Item1, item3.Item2);
				}
			}
			_ = list[0];
			_ = list[list.Count - 1];
			foreach (StationParameterDisplayModel stationParam in StationParameters.Where((StationParameterDisplayModel p) => p.IsLocked).ToList())
			{
				double key2 = Math.Round(stationParam.StationKm, 3);
				(double, double) tuple = (0.0, 0.0);
				bool flag = false;
				if (dictionary.TryGetValue(key2, out var value))
				{
					tuple = value;
					flag = true;
				}
				else
				{
					double tolerance = 0.002;
					var tuple2 = (from p in list
						where Math.Abs(p.StationKm - stationParam.StationKm) < tolerance
						orderby Math.Abs(p.StationKm - stationParam.StationKm)
						select p).FirstOrDefault();
					(double, double, double) tuple3 = tuple2;
					if (tuple3.Item1 != 0.0 || tuple3.Item2 != 0.0 || tuple3.Item3 != 0.0)
					{
						tuple = (tuple2.Item1, tuple2.Item2);
						flag = true;
					}
				}
				if (flag)
				{
					double num19 = tuple.Item1 * num13 + num14;
					double num20 = planPreviewHeight - (tuple.Item2 * num13 + num15);
					double num21 = 2.5 * num13;
					double num22 = CalculateTangentAngleAtStation(list, stationParam.StationKm) + Math.PI / 4.0;
					double num23 = num21 * Math.Cos(num22);
					double num24 = num21 * Math.Sin(num22);
					double num25 = num19 + num23;
					double num26 = num20 - num24;
					string color = "#00CC00";
					bool isSpecial = true;
					StationMarker item = new StationMarker
					{
						Position = new Point(num25, num26),
						DisplayText = stationParam.StationDisplay,
						StationKm = stationParam.StationKm,
						Color = color,
						IsSelected = (stationParam == SelectedStationParameter),
						IsSpecial = isSpecial
					};
					PlanLayoutPreviewData.StationMarkers.Add(item);
				}
			}
			GenerateCrossSectionLines(list, num13, num14, num15, planPreviewHeight, planPreviewWidth);
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 更新平面预览失败", ex);
		}
	}

	private void GenerateCrossSectionLines(List<(double X, double Y, double StationKm)> planarPoints, double scale, double offsetX, double offsetY, double canvasH, double canvasW)
	{
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (PlanLayoutPreviewData == null)
			{
				return;
			}
			PlanLayoutPreviewData.CrossSectionLines.Clear();
			PlanLayoutPreviewData.LeftBoundaryPoints = new PointCollection();
			PlanLayoutPreviewData.RightBoundaryPoints = new PointCollection();
			if (planarPoints.Count == 0)
			{
				return;
			}
			double previewRoadWidth = PreviewRoadWidth;
			double defaultAngle = 0.0;
			List<double> list = StationParameters?.Select((StationParameterDisplayModel p) => p.StationKm).ToList() ?? new List<double>();
			if (list.Count == 0)
			{
				return;
			}
			List<Point> list2 = new List<Point>();
			List<Point> list3 = new List<Point>();
			foreach (double stationKm in list)
			{
				double roadWidthAtStation = GetRoadWidthAtStation(stationKm, previewRoadWidth);
				double crossSectionOffsetAngleAtStation = GetCrossSectionOffsetAngleAtStation(stationKm, defaultAngle);
				int num = planarPoints.FindIndex(((double X, double Y, double StationKm) p) => Math.Abs(p.StationKm - stationKm) < 0.0001);
				if (num < 0)
				{
					num = planarPoints.FindIndex(((double X, double Y, double StationKm) p) => p.StationKm > stationKm);
					if (num < 0)
					{
						num = planarPoints.Count - 1;
					}
				}
				(double, double, double) tuple = planarPoints[num];
				double num2 = tuple.Item1 * scale + offsetX;
				double num3 = canvasH - (tuple.Item2 * scale + offsetY);
				double num4 = CalculateTangentAngleAtStation(planarPoints, stationKm) + Math.PI / 2.0 - crossSectionOffsetAngleAtStation * Math.PI / 180.0;
				double num5 = roadWidthAtStation / 2.0;
				double d = Math.Abs(crossSectionOffsetAngleAtStation * Math.PI / 180.0);
				double num6 = Math.Max(0.1, Math.Cos(d));
				double num7 = num5 / num6;
				double num8 = num2 - num7 * Math.Cos(num4) * scale;
				double num9 = num3 + num7 * Math.Sin(num4) * scale;
				double num10 = num2 + num7 * Math.Cos(num4) * scale;
				double num11 = num3 - num7 * Math.Sin(num4) * scale;
				PlanLayoutPreviewData.CrossSectionLines.Add(new CrossSectionLine
				{
					CenterPoint = new Point(num2, num3),
					LeftPoint = new Point(num8, num9),
					RightPoint = new Point(num10, num11),
					StationDisplay = FormatStationKm(stationKm),
					RoadWidth = roadWidthAtStation,
					OffsetAngle = crossSectionOffsetAngleAtStation,
					StationKm = stationKm
				});
				list2.Add(new Point(num8, num9));
				list3.Add(new Point(num10, num11));
			}
			PlanLayoutPreviewData.LeftBoundaryPoints = new PointCollection(list2);
			PlanLayoutPreviewData.RightBoundaryPoints = new PointCollection(list3);
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateSubgradeModelViewModel] 生成横断面线失败", ex);
		}
	}

	private double CalculateTangentAngleAtStation(List<(double X, double Y, double StationKm)> planarPoints, double stationKm)
	{
		if (planarPoints.Count < 2)
		{
			return 0.0;
		}
		int num = planarPoints.FindIndex(((double X, double Y, double StationKm) p) => Math.Abs(p.StationKm - stationKm) < 0.0001);
		if (num >= 0)
		{
			if (num < planarPoints.Count - 1 && num > 0)
			{
				(double, double, double) tuple = planarPoints[num - 1];
				var tuple2 = planarPoints[num + 1];
				return Math.Atan2(tuple2.Item2 - tuple.Item2, tuple2.Item1 - tuple.Item1);
			}
			if (num < planarPoints.Count - 1)
			{
				(double, double, double) tuple3 = planarPoints[num];
				int num2;
				for (num2 = num + 1; num2 < planarPoints.Count - 1; num2++)
				{
					(double, double, double) tuple4 = planarPoints[num2];
					if (Math.Sqrt(Math.Pow(tuple4.Item1 - tuple3.Item1, 2.0) + Math.Pow(tuple4.Item2 - tuple3.Item2, 2.0)) >= 5.0)
					{
						break;
					}
				}
				if (num2 < planarPoints.Count)
				{
					(double, double, double) tuple5 = planarPoints[num2];
					return Math.Atan2(tuple5.Item2 - tuple3.Item2, tuple5.Item1 - tuple3.Item1);
				}
				(double, double, double) tuple6 = planarPoints[num + 1];
				return Math.Atan2(tuple6.Item2 - tuple3.Item2, tuple6.Item1 - tuple3.Item1);
			}
			if (num > 0)
			{
				(double, double, double) tuple7 = planarPoints[num];
				int num3;
				for (num3 = num - 1; num3 > 0; num3--)
				{
					(double, double, double) tuple8 = planarPoints[num3];
					if (Math.Sqrt(Math.Pow(tuple8.Item1 - tuple7.Item1, 2.0) + Math.Pow(tuple8.Item2 - tuple7.Item2, 2.0)) >= 5.0)
					{
						break;
					}
				}
				if (num3 >= 0)
				{
					(double, double, double) tuple9 = planarPoints[num3];
					return Math.Atan2(tuple7.Item2 - tuple9.Item2, tuple7.Item1 - tuple9.Item1);
				}
				(double, double, double) tuple10 = planarPoints[num - 1];
				return Math.Atan2(tuple7.Item2 - tuple10.Item2, tuple7.Item1 - tuple10.Item1);
			}
		}
		for (int num4 = 0; num4 < planarPoints.Count - 1; num4++)
		{
			if (planarPoints[num4].StationKm <= stationKm && planarPoints[num4 + 1].StationKm >= stationKm)
			{
				(double, double, double) tuple11 = planarPoints[num4];
				(double, double, double) tuple12 = planarPoints[num4 + 1];
				return Math.Atan2(tuple12.Item2 - tuple11.Item2, tuple12.Item1 - tuple11.Item1);
			}
		}
		return 0.0;
	}

	private double GetRoadWidthAtStation(double stationKm, double defaultWidth)
	{
		if (StationParameters.Count == 0)
		{
			return defaultWidth;
		}
		List<StationParameterDisplayModel> list = StationParameters.OrderBy((StationParameterDisplayModel p) => p.StationKm).ToList();
		StationParameterDisplayModel stationParameterDisplayModel = null;
		StationParameterDisplayModel stationParameterDisplayModel2 = null;
		foreach (StationParameterDisplayModel item in list)
		{
			if (item.StationKm <= stationKm)
			{
				stationParameterDisplayModel = item;
			}
			if (item.StationKm >= stationKm && stationParameterDisplayModel2 == null)
			{
				stationParameterDisplayModel2 = item;
			}
		}
		if (stationParameterDisplayModel == null && stationParameterDisplayModel2 == null)
		{
			return defaultWidth;
		}
		if (stationParameterDisplayModel == null)
		{
			return stationParameterDisplayModel2.RoadWidth ?? defaultWidth;
		}
		if (stationParameterDisplayModel2 == null)
		{
			return stationParameterDisplayModel.RoadWidth ?? defaultWidth;
		}
		double num = stationParameterDisplayModel.RoadWidth ?? defaultWidth;
		double num2 = stationParameterDisplayModel2.RoadWidth ?? defaultWidth;
		if (Math.Abs(stationParameterDisplayModel2.StationKm - stationParameterDisplayModel.StationKm) < 0.0001)
		{
			return num;
		}
		double num3 = (stationKm - stationParameterDisplayModel.StationKm) / (stationParameterDisplayModel2.StationKm - stationParameterDisplayModel.StationKm);
		return num + (num2 - num) * num3;
	}

	private double GetCrossSectionOffsetAngleAtStation(double stationKm, double defaultAngle)
	{
		if (StationParameters.Count == 0)
		{
			return defaultAngle;
		}
		List<StationParameterDisplayModel> list = StationParameters.OrderBy((StationParameterDisplayModel p) => p.StationKm).ToList();
		StationParameterDisplayModel stationParameterDisplayModel = null;
		StationParameterDisplayModel stationParameterDisplayModel2 = null;
		foreach (StationParameterDisplayModel item in list)
		{
			if (item.StationKm <= stationKm)
			{
				stationParameterDisplayModel = item;
			}
			if (item.StationKm >= stationKm && stationParameterDisplayModel2 == null)
			{
				stationParameterDisplayModel2 = item;
			}
		}
		if (stationParameterDisplayModel == null && stationParameterDisplayModel2 == null)
		{
			return defaultAngle;
		}
		if (stationParameterDisplayModel == null)
		{
			return stationParameterDisplayModel2.CrossSectionOffsetAngle ?? defaultAngle;
		}
		if (stationParameterDisplayModel2 == null)
		{
			return stationParameterDisplayModel.CrossSectionOffsetAngle ?? defaultAngle;
		}
		double num = stationParameterDisplayModel.CrossSectionOffsetAngle ?? defaultAngle;
		double num2 = stationParameterDisplayModel2.CrossSectionOffsetAngle ?? defaultAngle;
		if (Math.Abs(stationParameterDisplayModel2.StationKm - stationParameterDisplayModel.StationKm) < 0.0001)
		{
			return num;
		}
		double num3 = (stationKm - stationParameterDisplayModel.StationKm) / (stationParameterDisplayModel2.StationKm - stationParameterDisplayModel.StationKm);
		return num + (num2 - num) * num3;
	}

	private List<(double X, double Y, double StationKm)> CalculatePlanarPointsFromHorizontalCurve(HorizontalCurveTable horizontalTable, double interval)
	{
		List<(double, double, double)> list = new List<(double, double, double)>();
		List<HorizontalCurveIP> sortedPoints = horizontalTable.GetSortedPoints();
		if (sortedPoints.Count < 2)
		{
			return list;
		}
		double[] array = new double[sortedPoints.Count];
		for (int i = 0; i < sortedPoints.Count - 1; i++)
		{
			array[i] = Math.Atan2(sortedPoints[i + 1].CoordinateY - sortedPoints[i].CoordinateY, sortedPoints[i + 1].CoordinateX - sortedPoints[i].CoordinateX);
			while (array[i] < 0.0)
			{
				array[i] += Math.PI * 2.0;
			}
			while (array[i] >= Math.PI * 2.0)
			{
				array[i] -= Math.PI * 2.0;
			}
		}
		array[^1] = array[^2];
		double num = sortedPoints[0].Station;
		double num2 = sortedPoints[0].CoordinateX;
		double num3 = sortedPoints[0].CoordinateY;
		list.Add((num2, num3, num));
		for (int j = 1; j < sortedPoints.Count - 1; j++)
		{
			HorizontalCurveIP horizontalCurveIP = sortedPoints[j];
			double num4 = array[j - 1];
			double num5 = array[j];
			double num6;
			for (num6 = num5 - num4; num6 > Math.PI; num6 -= Math.PI * 2.0)
			{
			}
			for (; num6 < -Math.PI; num6 += Math.PI * 2.0)
			{
			}
			int num7 = ((num6 >= 0.0) ? 1 : (-1));
			double num8 = Math.Abs(num6);
			double radius = horizontalCurveIP.Radius;
			double valueOrDefault = horizontalCurveIP.FirstTransitionLength.GetValueOrDefault();
			double valueOrDefault2 = horizontalCurveIP.SecondTransitionLength.GetValueOrDefault();
			if (radius <= 1.0 || num8 < 1E-06)
			{
				double num9 = Math.Sqrt(Math.Pow(horizontalCurveIP.CoordinateX - num2, 2.0) + Math.Pow(horizontalCurveIP.CoordinateY - num3, 2.0));
				double num10 = num;
				double num11 = num10 + num9 / 1000.0;
				for (double num12 = (double)((int)(num10 / interval) + 1) * interval; num12 < num11; num12 += interval)
				{
					double num13 = (num12 - num10) / (num11 - num10);
					double item = num2 + (horizontalCurveIP.CoordinateX - num2) * num13;
					double item2 = num3 + (horizontalCurveIP.CoordinateY - num3) * num13;
					list.Add((item, item2, num12));
				}
				list.Add((horizontalCurveIP.CoordinateX, horizontalCurveIP.CoordinateY, num11));
				num = num11;
				num2 = horizontalCurveIP.CoordinateX;
				num3 = horizontalCurveIP.CoordinateY;
				continue;
			}
			double num14 = valueOrDefault / 2.0 / radius;
			double num15 = valueOrDefault2 / 2.0 / radius;
			double num16 = ((valueOrDefault > 0.0) ? (valueOrDefault / 2.0 - Math.Pow(valueOrDefault, 3.0) / (240.0 * radius * radius)) : 0.0);
			double num17 = ((valueOrDefault > 0.0) ? (Math.Pow(valueOrDefault, 2.0) / (24.0 * radius)) : 0.0);
			double num18 = ((valueOrDefault2 > 0.0) ? (valueOrDefault2 / 2.0 - Math.Pow(valueOrDefault2, 3.0) / (240.0 * radius * radius)) : 0.0);
			double num19 = ((valueOrDefault2 > 0.0) ? (Math.Pow(valueOrDefault2, 2.0) / (24.0 * radius)) : 0.0);
			double num20 = Math.Tan(num8 / 2.0);
			double num21 = (radius + num17) * num20 + num16;
			double num22 = (radius + num19) * num20 + num18;
			double num23 = horizontalCurveIP.CoordinateX - num21 * Math.Cos(num4);
			double num24 = horizontalCurveIP.CoordinateY - num21 * Math.Sin(num4);
			double num25 = horizontalCurveIP.CoordinateX + num22 * Math.Cos(num5);
			double num26 = horizontalCurveIP.CoordinateY + num22 * Math.Sin(num5);
			double num27 = Math.Sqrt(Math.Pow(num23 - num2, 2.0) + Math.Pow(num24 - num3, 2.0));
			double num28 = num;
			double num29 = num28 + num27 / 1000.0;
			for (double num12 = (double)((int)(num28 / interval) + 1) * interval; num12 < num29; num12 += interval)
			{
				double num30 = (num12 - num28) / (num29 - num28);
				double item3 = num2 + (num23 - num2) * num30;
				double item4 = num3 + (num24 - num3) * num30;
				list.Add((item3, item4, num12));
			}
			list.Add((num23, num24, num29));
			num = num29;
			num2 = num23;
			num3 = num24;
			if (valueOrDefault > 1E-06)
			{
				double num31 = num;
				double num32 = num31 + valueOrDefault / 1000.0;
				for (double num12 = (double)((int)(num31 / interval) + 1) * interval; num12 < num32; num12 += interval)
				{
					double num33 = (num12 - num31) * 1000.0;
					double num34 = num33 - Math.Pow(num33, 5.0) / (40.0 * radius * radius * valueOrDefault * valueOrDefault);
					double num35 = Math.Pow(num33, 3.0) / (6.0 * radius * valueOrDefault);
					double item5 = num23 + num34 * Math.Cos(num4) - (double)num7 * num35 * Math.Sin(num4);
					double item6 = num24 + num34 * Math.Sin(num4) + (double)num7 * num35 * Math.Cos(num4);
					list.Add((item5, item6, num12));
				}
				double num36 = valueOrDefault - Math.Pow(valueOrDefault, 5.0) / (40.0 * radius * radius * valueOrDefault * valueOrDefault);
				double num37 = Math.Pow(valueOrDefault, 3.0) / (6.0 * radius * valueOrDefault);
				double num38 = num23 + num36 * Math.Cos(num4) - (double)num7 * num37 * Math.Sin(num4);
				double num39 = num24 + num36 * Math.Sin(num4) + (double)num7 * num37 * Math.Cos(num4);
				list.Add((num38, num39, num32));
				num = num32;
				num2 = num38;
				num3 = num39;
			}
			double num40 = num8 - num14 - num15;
			if (num40 > 1E-06)
			{
				double num41 = num4 + (double)num7 * num14;
				double num44;
				double num45;
				if (valueOrDefault > 1E-06)
				{
					double num42 = valueOrDefault - Math.Pow(valueOrDefault, 5.0) / (40.0 * radius * radius * valueOrDefault * valueOrDefault);
					double num43 = Math.Pow(valueOrDefault, 3.0) / (6.0 * radius * valueOrDefault);
					num44 = num23 + num42 * Math.Cos(num4) - (double)num7 * num43 * Math.Sin(num4);
					num45 = num24 + num42 * Math.Sin(num4) + (double)num7 * num43 * Math.Cos(num4);
				}
				else
				{
					num44 = num23;
					num45 = num24;
				}
				double num46 = radius * num40;
				double num47 = num;
				double num48 = num47 + num46 / 1000.0;
				double num49 = num44 + radius * Math.Cos(num41 + (double)num7 * Math.PI / 2.0);
				double num50 = num45 + radius * Math.Sin(num41 + (double)num7 * Math.PI / 2.0);
				for (double num12 = (double)((int)(num47 / interval) + 1) * interval; num12 < num48; num12 += interval)
				{
					double num51 = (num12 - num47) * 1000.0;
					double num52 = num41 + (double)num7 * (num51 / radius);
					double item7 = num49 + radius * Math.Cos(num52 - (double)num7 * Math.PI / 2.0);
					double item8 = num50 + radius * Math.Sin(num52 - (double)num7 * Math.PI / 2.0);
					list.Add((item7, item8, num12));
				}
				double num53 = num41 + (double)num7 * num40;
				double num54 = num49 + radius * Math.Cos(num53 - (double)num7 * Math.PI / 2.0);
				double num55 = num50 + radius * Math.Sin(num53 - (double)num7 * Math.PI / 2.0);
				list.Add((num54, num55, num48));
				num = num48;
				num2 = num54;
				num3 = num55;
			}
			if (valueOrDefault2 > 1E-06)
			{
				double num56 = num;
				double num57 = num56 + valueOrDefault2 / 1000.0;
				for (double num12 = (double)((int)(num56 / interval) + 1) * interval; num12 < num57; num12 += interval)
				{
					double num58 = (num12 - num56) * 1000.0;
					double num59 = valueOrDefault2 - num58;
					double num60 = num59 - Math.Pow(num59, 5.0) / (40.0 * radius * radius * valueOrDefault2 * valueOrDefault2);
					double num61 = Math.Pow(num59, 3.0) / (6.0 * radius * valueOrDefault2);
					double item9 = num25 - num60 * Math.Cos(num5) - (double)num7 * num61 * Math.Sin(num5);
					double item10 = num26 - num60 * Math.Sin(num5) + (double)num7 * num61 * Math.Cos(num5);
					list.Add((item9, item10, num12));
				}
				list.Add((num25, num26, num57));
				num = num57;
			}
			num2 = num25;
			num3 = num26;
		}
		if (sortedPoints.Count >= 2)
		{
			HorizontalCurveIP horizontalCurveIP2 = sortedPoints[sortedPoints.Count - 1];
			double num62 = Math.Sqrt(Math.Pow(horizontalCurveIP2.CoordinateX - num2, 2.0) + Math.Pow(horizontalCurveIP2.CoordinateY - num3, 2.0));
			double num63 = num;
			double num64 = num63 + num62 / 1000.0;
			for (double num12 = (double)((int)(num63 / interval) + 1) * interval; num12 < num64; num12 += interval)
			{
				double num65 = (num12 - num63) / (num64 - num63);
				double item11 = num2 + (horizontalCurveIP2.CoordinateX - num2) * num65;
				double item12 = num3 + (horizontalCurveIP2.CoordinateY - num3) * num65;
				list.Add((item11, item12, num12));
			}
			list.Add((horizontalCurveIP2.CoordinateX, horizontalCurveIP2.CoordinateY, num64));
		}
		return list;
	}

	private static string FormatStationKm(double stationKm)
	{
		int num = (int)Math.Floor(stationKm);
		double num2 = (stationKm - (double)num) * 1000.0;
		if (!(Math.Abs(num2 - Math.Round(num2)) < 0.001))
		{
			return $"K{num}+{num2:000.000}";
		}
		return $"K{num}+{(int)Math.Round(num2):D3}";
	}

	private static string GetNextColor(int index)
	{
		string[] array = new string[7] { "#FF9800", "#4CAF50", "#2196F3", "#9C27B0", "#F44336", "#00BCD4", "#795548" };
		return array[index % array.Length];
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedLayerChanged(SubgradeLayerDisplayModel? value)
	{
		UpdatePreview();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnPreviewRoadWidthChanged(double value)
	{
		UpdatePreview();
		UpdateAncillaryStructurePreview();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedStationParameterChanged(StationParameterDisplayModel? value)
	{
		if (PlanLayoutPreviewData?.CrossSectionLines != null)
		{
			foreach (CrossSectionLine crossSectionLine in PlanLayoutPreviewData.CrossSectionLines)
			{
				crossSectionLine.IsSelected = value != null && Math.Abs(crossSectionLine.StationKm - value.StationKm) < 0.001;
			}
		}
		if (value != null)
		{
			_isSyncingSelectedToTemp = true;
			TempStationParameter.StationKm = value.StationKm;
			TempStationParameter.StationKmInteger = value.StationKmInteger;
			TempStationParameter.StationMeter = value.StationMeter;
			TempStationParameter.RoadWidth = value.RoadWidth;
			TempStationParameter.CrossSectionOffsetAngle = value.CrossSectionOffsetAngle;
			_isSyncingSelectedToTemp = false;
		}
	}
}
