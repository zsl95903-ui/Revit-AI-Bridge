using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

public class CreateAncillaryStructureViewModel : ObservableObject
{
	private readonly IRoadProjectManager _projectManager;

	private readonly IRoadModelingUIService _roadModelingService;

	private readonly ILogger _logger;

	[ObservableProperty]
	private ObservableCollection<RoadProject> _roadProjects = new ObservableCollection<RoadProject>();

	[ObservableProperty]
	private RoadProject? _selectedRoadProject;

	[ObservableProperty]
	private ObservableCollection<AncillaryStructureDisplayModel> _ancillaryStructures = new ObservableCollection<AncillaryStructureDisplayModel>();

	[ObservableProperty]
	private AncillaryStructureDisplayModel? _selectedAncillaryStructure;

	[ObservableProperty]
	private string _statusMessage = "就绪";

	[ObservableProperty]
	private bool _isCreating;

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
	private AncillaryStructureDisplayModel? _selectedStructureForLayout;

	[ObservableProperty]
	private AncillaryStructureLayoutSegment? _selectedLayoutSegment;

	[ObservableProperty]
	private AncillaryStructureLayoutSegment? _tempLayoutSegment;

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
	private RelayCommand? addAncillaryStructureCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? removeSelectedAncillaryStructureCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? moveAncillaryStructureUpCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? moveAncillaryStructureDownCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? createAncillaryStructureCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? saveDataToRoadCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addLayoutSegmentCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? removeSelectedLayoutSegmentCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? applyLayoutSegmentCommand;

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
	public RoadProject? SelectedRoadProject
	{
		get
		{
			return _selectedRoadProject;
		}
		set
		{
			if (!EqualityComparer<RoadProject>.Default.Equals(_selectedRoadProject, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedRoadProject);
				_selectedRoadProject = value;
				OnSelectedRoadProjectChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedRoadProject);
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
	public ObservableCollection<LayerPreviewData> AncillaryStructurePreviewData
	{
		get
		{
			return _ancillaryStructurePreviewData;
		}
		[MemberNotNull("_ancillaryStructurePreviewData")]
		set
		{
			if (!EqualityComparer<ObservableCollection<LayerPreviewData>>.Default.Equals(_ancillaryStructurePreviewData, value))
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
	public AncillaryStructureDisplayModel? SelectedStructureForLayout
	{
		get
		{
			return _selectedStructureForLayout;
		}
		set
		{
			if (!EqualityComparer<AncillaryStructureDisplayModel>.Default.Equals(_selectedStructureForLayout, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedStructureForLayout);
				_selectedStructureForLayout = value;
				OnSelectedStructureForLayoutChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedStructureForLayout);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public AncillaryStructureLayoutSegment? SelectedLayoutSegment
	{
		get
		{
			return _selectedLayoutSegment;
		}
		set
		{
			if (!EqualityComparer<AncillaryStructureLayoutSegment>.Default.Equals(_selectedLayoutSegment, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedLayoutSegment);
				_selectedLayoutSegment = value;
				OnSelectedLayoutSegmentChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedLayoutSegment);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public AncillaryStructureLayoutSegment? TempLayoutSegment
	{
		get
		{
			return _tempLayoutSegment;
		}
		set
		{
			if (!EqualityComparer<AncillaryStructureLayoutSegment>.Default.Equals(_tempLayoutSegment, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.TempLayoutSegment);
				_tempLayoutSegment = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.TempLayoutSegment);
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
	public IAsyncRelayCommand CreateAncillaryStructureCommand => createAncillaryStructureCommand ?? (createAncillaryStructureCommand = new AsyncRelayCommand(CreateAncillaryStructureAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SaveDataToRoadCommand => saveDataToRoadCommand ?? (saveDataToRoadCommand = new RelayCommand(SaveDataToRoad));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddLayoutSegmentCommand => addLayoutSegmentCommand ?? (addLayoutSegmentCommand = new RelayCommand(AddLayoutSegment));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RemoveSelectedLayoutSegmentCommand => removeSelectedLayoutSegmentCommand ?? (removeSelectedLayoutSegmentCommand = new RelayCommand(RemoveSelectedLayoutSegment));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ApplyLayoutSegmentCommand => applyLayoutSegmentCommand ?? (applyLayoutSegmentCommand = new RelayCommand(ApplyLayoutSegment));

	public CreateAncillaryStructureViewModel()
	{
		IServiceProvider services = UIBootstrapper.Services;
		_projectManager = services.GetRequiredService<IRoadProjectManager>();
		_roadModelingService = services.GetRequiredService<IRoadModelingUIService>();
		_logger = ServiceProvider.GetLogger();
		PlanLayoutPreviewData = new PlanLayoutPreviewData();
		PlanPreviewViewState = new PlanPreviewViewState();
		TempLayoutSegment = AncillaryStructureLayoutSegment.CreateEmpty();
		AncillaryStructures.CollectionChanged += delegate(object? s, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (AncillaryStructureDisplayModel newItem in e.NewItems)
				{
					AncillaryStructureLayoutSegment item = AncillaryStructureLayoutSegment.CreateEmpty();
					newItem.LayoutSegments.Add(item);
				}
			}
			if (e.OldItems != null)
			{
				foreach (AncillaryStructureDisplayModel oldItem in e.OldItems)
				{
					oldItem.LayoutSegments.Clear();
				}
			}
		};
		LoadRoadProjects();
	}

	public void UpdatePreviewSize(double width, double height)
	{
		PreviewWidth = width;
		PreviewHeight = height;
		UpdateAncillaryStructurePreview();
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
			_logger.Error("[CreateAncillaryStructureViewModel] 加载路线列表失败", ex);
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
			_logger.Error("[CreateAncillaryStructureViewModel] 跳转到道路项目管理失败", ex);
			StatusMessage = "跳转到道路项目管理失败";
		}
	}

	[RelayCommand]
	private void AddAncillaryStructure()
	{
		AncillaryStructureDisplayModel ancillaryStructureDisplayModel = AncillaryStructureDisplayModel.CreateEmpty(AncillaryStructures.Count);
		ancillaryStructureDisplayModel.ParameterChanged += OnAncillaryStructureParameterChanged;
		AncillaryStructures.Add(ancillaryStructureDisplayModel);
		UpdateAllAncillaryStructureColors();
		SelectedAncillaryStructure = ancillaryStructureDisplayModel;
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
			UpdateAncillaryStructurePreview();
		}
	}

	private void OnAncillaryStructureParameterChanged()
	{
		UpdateAncillaryStructurePreview();
	}

	[RelayCommand]
	private async Task CreateAncillaryStructureAsync()
	{
		if (SelectedRoadProject == null)
		{
			MessageBox.Show("请先选择路线", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		if (AncillaryStructures.Count == 0)
		{
			MessageBox.Show("请至少添加一个附属结构", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		try
		{
			AncillaryStructuresConfiguration ancillaryStructuresConfiguration = new AncillaryStructuresConfiguration
			{
				Structures = AncillaryStructures.Select(delegate(AncillaryStructureDisplayModel s)
				{
					AncillaryStructureData ancillaryStructureData = AncillaryStructureData.FromAncillaryStructure(s.ToStructure());
					ancillaryStructureData.LayoutSegments = s.LayoutSegments.Select((AncillaryStructureLayoutSegment seg) => new AncillaryStructureLayoutSegmentData
					{
						SegmentId = seg.SegmentId,
						StartStationKm = seg.StartStationKm,
						EndStationKm = seg.EndStationKm,
						ModelLength = seg.ModelLength
					}).ToList();
					return ancillaryStructureData;
				}).ToList()
			};
			SelectedRoadProject.AncillaryStructuresConfiguration = ancillaryStructuresConfiguration;
			_projectManager.UpdateProject(SelectedRoadProject);
			_logger.Info("[CreateAncillaryStructureViewModel] 已自动保存设置到路线: " + SelectedRoadProject.Name);
			StatusMessage = "已保存设置，正在创建附属结构...";
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateAncillaryStructureViewModel] 自动保存设置失败", ex);
			StatusMessage = "保存设置失败: " + ex.Message;
			if (MessageBox.Show("自动保存设置失败: " + ex.Message + "\n是否继续创建模型？", "保存失败", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
			{
				return;
			}
		}
		IsCreating = true;
		StatusMessage = "正在创建附属结构...";
		RoadModelingUIResult result = null;
		try
		{
			result = await _roadModelingService.CreateAncillaryStructureAsync(SelectedRoadProject, splitAtIntegerStations: false);
		}
		catch (Exception ex2)
		{
			_logger.Error("[CreateAncillaryStructureViewModel] 创建附属结构失败", ex2);
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
			StatusMessage = "附属结构创建成功: " + result.Message;
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				CreateAncillaryStructureWindow createAncillaryStructureWindow = Application.Current?.Windows.OfType<CreateAncillaryStructureWindow>().FirstOrDefault();
				if (createAncillaryStructureWindow != null)
				{
					createAncillaryStructureWindow.Topmost = true;
					createAncillaryStructureWindow.Activate();
				}
				MessageBox.Show($"附属结构创建成功！\n\n创建实例数量: {result.CreatedInstanceCount}\n创建族类型数量: {result.CreatedTypeCount}\n创建材质数量: {result.CreatedMaterialCount}\n\n按结构统计:\n{string.Join("\n", result.LayerInstanceCounts.Select<KeyValuePair<string, int>, string>((KeyValuePair<string, int> kvp) => $"  {kvp.Key}: {kvp.Value} 个"))}", "成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				if (createAncillaryStructureWindow != null)
				{
					createAncillaryStructureWindow.Topmost = false;
				}
			}, (DispatcherPriority)4, Array.Empty<object>());
			return;
		}
		StatusMessage = "创建失败: " + result.Error;
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			CreateAncillaryStructureWindow createAncillaryStructureWindow = Application.Current?.Windows.OfType<CreateAncillaryStructureWindow>().FirstOrDefault();
			if (createAncillaryStructureWindow != null)
			{
				createAncillaryStructureWindow.Topmost = true;
				createAncillaryStructureWindow.Activate();
			}
			MessageBox.Show("创建失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			if (createAncillaryStructureWindow != null)
			{
				createAncillaryStructureWindow.Topmost = false;
			}
		}, (DispatcherPriority)4, Array.Empty<object>());
	}

	[RelayCommand]
	private void Close()
	{
		Application.Current?.Windows.OfType<CreateAncillaryStructureWindow>().FirstOrDefault()?.Close();
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
			AncillaryStructuresConfiguration ancillaryStructuresConfiguration = new AncillaryStructuresConfiguration
			{
				Structures = AncillaryStructures.Select(delegate(AncillaryStructureDisplayModel s)
				{
					AncillaryStructureData ancillaryStructureData = AncillaryStructureData.FromAncillaryStructure(s.ToStructure());
					ancillaryStructureData.LayoutSegments = s.LayoutSegments.Select((AncillaryStructureLayoutSegment seg) => new AncillaryStructureLayoutSegmentData
					{
						SegmentId = seg.SegmentId,
						StartStationKm = seg.StartStationKm,
						EndStationKm = seg.EndStationKm,
						ModelLength = seg.ModelLength
					}).ToList();
					return ancillaryStructureData;
				}).ToList()
			};
			SelectedRoadProject.AncillaryStructuresConfiguration = ancillaryStructuresConfiguration;
			_projectManager.UpdateProject(SelectedRoadProject);
			StatusMessage = "已保存结构数据到路线: " + SelectedRoadProject.Name;
			MessageBox.Show("已保存到路线: " + SelectedRoadProject.Name, "保存成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateAncillaryStructureViewModel] 保存数据失败", ex);
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
			if (SelectedRoadProject.AncillaryStructuresConfiguration != null && SelectedRoadProject.AncillaryStructuresConfiguration.Structures.Count > 0)
			{
				AncillaryStructuresConfiguration ancillaryStructuresConfiguration = SelectedRoadProject.AncillaryStructuresConfiguration;
				AncillaryStructures.Clear();
				foreach (AncillaryStructureData item in ancillaryStructuresConfiguration.Structures.OrderBy((AncillaryStructureData s) => s.Order))
				{
					AncillaryStructureDisplayModel ancillaryStructureDisplayModel = AncillaryStructureDisplayModel.FromStructure(item.ToAncillaryStructure());
					ancillaryStructureDisplayModel.ParameterChanged += OnAncillaryStructureParameterChanged;
					AncillaryStructures.Add(ancillaryStructureDisplayModel);
				}
				_logger.Info($"[CreateAncillaryStructureViewModel] 已加载附属结构配置: {ancillaryStructuresConfiguration.Structures.Count} 个结构");
			}
			else
			{
				AncillaryStructures.Clear();
				_logger.Info("[CreateAncillaryStructureViewModel] 路线无附属结构配置，已清空附属结构列表");
			}
			UpdateAncillaryStructurePreview();
			UpdatePlanPreview();
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateAncillaryStructureViewModel] 加载路线数据失败", ex);
		}
	}

	public void UpdateAncillaryStructurePreview()
	{
		try
		{
			AncillaryStructurePreviewData.Clear();
			foreach (LayerPreviewData item in GenerateCrossSectionPreview())
			{
				AncillaryStructurePreviewData.Add(item);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateAncillaryStructureViewModel] 更新附属结构预览失败", ex);
		}
	}

	private List<LayerPreviewData> GenerateCrossSectionPreview()
	{
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		List<LayerPreviewData> list = new List<LayerPreviewData>();
		double previewWidth = PreviewWidth;
		double previewHeight = PreviewHeight;
		int num = 20;
		double num2 = PreviewRoadWidth * 1000.0;
		List<(double, double, double, double)> list2 = new List<(double, double, double, double)>();
		double num3 = num2;
		double num4 = 0.0;
		double num5 = num2;
		List<SubgradeLayer> list3 = new List<SubgradeLayer>();
		if (SelectedRoadProject?.RoadStructureConfiguration?.Layers != null)
		{
			foreach (SubgradeLayerData item20 in SelectedRoadProject.RoadStructureConfiguration.Layers.OrderBy((SubgradeLayerData l) => l.Order))
			{
				list3.Add(item20.ToSubgradeLayer());
			}
		}
		if (list3.Count == 0)
		{
			list3.Add(new SubgradeLayer
			{
				Name = "路面",
				WidthIncrement = 0.0,
				IncrementType = WidthIncrementType.BothSides,
				SlopeRatio = 1.0,
				Height = 100.0,
				Order = 0
			});
		}
		foreach (SubgradeLayer item21 in list3.OrderBy((SubgradeLayer l) => l.Order))
		{
			double num6 = num5 + item21.WidthIncrement;
			double num7 = num6 + 2.0 * item21.Height * item21.SlopeRatio;
			list2.Add((num6, num7, num4, num4 + item21.Height));
			if (num7 > num3)
			{
				num3 = num7;
			}
			num4 += item21.Height;
			num5 = num7;
		}
		List<(AncillaryStructureDisplayModel, double, double, double, double, double, double)> list4 = new List<(AncillaryStructureDisplayModel, double, double, double, double, double, double)>();
		double num8 = num2 / 2.0;
		foreach (AncillaryStructureDisplayModel item22 in from s in AncillaryStructures
			where s.IsEnabled
			orderby s.Order
			select s)
		{
			double num9 = 0.0;
			if (item22.StructureType == AncillaryStructureType.Trapezoid && item22.SlopeRatio.HasValue && item22.SlopeRatio.Value > 0.0)
			{
				num9 = item22.Height * item22.SlopeRatio.Value;
			}
			switch (item22.PositionType)
			{
			case AncillaryPositionType.Left:
			{
				double num22 = 0.0 - num8 - item22.HorizontalOffset - item22.Width;
				double num23 = 0.0 - num8 - item22.HorizontalOffset;
				double num24 = 0.0 - item22.ElevationDifference;
				double item7 = num24 + item22.Height;
				double num25 = num22 - num9;
				double item8 = num23;
				list4.Add((item22, num22, num23, num25, item8, num24, item7));
				if (Math.Abs(num25) > num3 / 2.0)
				{
					num3 = Math.Abs(num25) * 2.0;
				}
				break;
			}
			case AncillaryPositionType.Right:
			{
				double num18 = num8 + item22.HorizontalOffset;
				double num19 = num8 + item22.HorizontalOffset + item22.Width;
				double num20 = 0.0 - item22.ElevationDifference;
				double item5 = num20 + item22.Height;
				double item6 = num18;
				double num21 = num19 + num9;
				list4.Add((item22, num18, num19, item6, num21, num20, item5));
				if (Math.Abs(num21) > num3 / 2.0)
				{
					num3 = Math.Abs(num21) * 2.0;
				}
				break;
			}
			case AncillaryPositionType.Both:
			{
				double num10 = 0.0 - num8 - item22.HorizontalOffset - item22.Width;
				double num11 = 0.0 - num8 - item22.HorizontalOffset;
				double num12 = 0.0 - item22.ElevationDifference;
				double item = num12 + item22.Height;
				double num13 = num10 - num9;
				double item2 = num11;
				list4.Add((item22, num10, num11, num13, item2, num12, item));
				if (Math.Abs(num13) > num3 / 2.0)
				{
					num3 = Math.Abs(num13) * 2.0;
				}
				double num14 = num8 + item22.HorizontalOffset;
				double num15 = num8 + item22.HorizontalOffset + item22.Width;
				double num16 = 0.0 - item22.ElevationDifference;
				double item3 = num16 + item22.Height;
				double item4 = num14;
				double num17 = num15 + num9;
				list4.Add((item22, num14, num15, item4, num17, num16, item3));
				if (Math.Abs(num17) > num3 / 2.0)
				{
					num3 = Math.Abs(num17) * 2.0;
				}
				break;
			}
			}
		}
		double num26 = 0.0;
		double num27 = num4;
		if (list4.Count > 0)
		{
			num26 = list4.Min<(AncillaryStructureDisplayModel, double, double, double, double, double, double)>(((AncillaryStructureDisplayModel structure, double topLeftX, double topRightX, double bottomLeftX, double bottomRightX, double topY, double bottomY) r) => r.topY);
			num27 = Math.Max(num4, list4.Max<(AncillaryStructureDisplayModel, double, double, double, double, double, double)>(((AncillaryStructureDisplayModel structure, double topLeftX, double topRightX, double bottomLeftX, double bottomRightX, double topY, double bottomY) r) => r.bottomY));
		}
		double num28 = num27 - num26;
		if (num3 <= 0.0 || num28 <= 0.0)
		{
			return list;
		}
		double num29 = previewWidth - (double)(2 * num);
		double num30 = previewHeight - (double)(2 * num);
		double val = num29 / num3;
		double val2 = num30 / num28;
		double num31 = Math.Min(val, val2);
		double num32 = previewWidth / 2.0;
		double num33 = (double)num - num26 * num31;
		int num34 = 0;
		foreach (SubgradeLayer item23 in list3.OrderBy((SubgradeLayer l) => l.Order))
		{
			(double, double, double, double) tuple = list2[num34];
			double item9 = tuple.Item1;
			double item10 = tuple.Item2;
			double item11 = tuple.Item3;
			double item12 = tuple.Item4;
			num34++;
			PointCollection points = new PointCollection
			{
				new Point(num32 - item9 / 2.0 * num31, num33 + item11 * num31),
				new Point(num32 + item9 / 2.0 * num31, num33 + item11 * num31),
				new Point(num32 + item10 / 2.0 * num31, num33 + item12 * num31),
				new Point(num32 - item10 / 2.0 * num31, num33 + item12 * num31)
			};
			list.Add(new LayerPreviewData
			{
				Points = points,
				FillColor = "#CCCCCC",
				Name = item23.Name,
				StrokeThickness = 0.8
			});
		}
		foreach (var item24 in list4)
		{
			AncillaryStructureDisplayModel item13 = item24.Item1;
			double item14 = item24.Item2;
			double item15 = item24.Item3;
			double item16 = item24.Item4;
			double item17 = item24.Item5;
			double item18 = item24.Item6;
			double item19 = item24.Item7;
			PointCollection points2 = new PointCollection
			{
				new Point(num32 + item14 * num31, num33 + item18 * num31),
				new Point(num32 + item15 * num31, num33 + item18 * num31),
				new Point(num32 + item17 * num31, num33 + item19 * num31),
				new Point(num32 + item16 * num31, num33 + item19 * num31)
			};
			list.Add(new LayerPreviewData
			{
				Points = points2,
				FillColor = item13.Color,
				Name = item13.Name,
				StrokeThickness = 0.8
			});
		}
		return list;
	}

	private void UpdatePlanPreview()
	{
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (PlanLayoutPreviewData == null)
			{
				PlanLayoutPreviewData = new PlanLayoutPreviewData();
			}
			PlanLayoutPreviewData.CenterlinePoints = null;
			PlanLayoutPreviewData.StationMarkers.Clear();
			PlanLayoutPreviewData.CrossSectionLines.Clear();
			PlanLayoutPreviewData.AncillaryStructureLayoutLines.Clear();
			PlanLayoutPreviewData.LeftBoundaryPoints = null;
			PlanLayoutPreviewData.RightBoundaryPoints = null;
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
			List<(double, double, double)> list;
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
			foreach (var item in list2)
			{
				double num16 = item.Item1 * num13 + num14;
				double num17 = planPreviewHeight - (item.Item2 * num13 + num15);
				pointCollection.Add(new Point(num16, num17));
			}
			PlanLayoutPreviewData.CenterlinePoints = pointCollection;
			PlanLayoutPreviewData.Bounds = new Rect(0.0, 0.0, planPreviewWidth, planPreviewHeight);
			double num18 = list[list.Count - 1].Item3 - list[0].Item3;
			PlanLayoutPreviewData.TotalLength = num18 * 1000.0;
			PlanLayoutPreviewData.StartStationDisplay = FormatStationKm(list[0].Item3);
			PlanLayoutPreviewData.EndStationDisplay = FormatStationKm(list[list.Count - 1].Item3);
			(double, double, double) tuple = list[0];
			(double, double, double) tuple2 = list[list.Count - 1];
			double num19 = tuple.Item1 * num13 + num14;
			double num20 = planPreviewHeight - (tuple.Item2 * num13 + num15);
			double num21 = tuple2.Item1 * num13 + num14;
			double num22 = planPreviewHeight - (tuple2.Item2 * num13 + num15);
			PlanLayoutPreviewData.StationMarkers.Add(new StationMarker
			{
				Position = new Point(num19, num20),
				DisplayText = FormatStationKm(tuple.Item3),
				StationKm = tuple.Item3,
				Color = "#00CC00",
				IsSelected = false,
				IsSpecial = true
			});
			PlanLayoutPreviewData.StationMarkers.Add(new StationMarker
			{
				Position = new Point(num21, num22),
				DisplayText = FormatStationKm(tuple2.Item3),
				StationKm = tuple2.Item3,
				Color = "#00CC00",
				IsSelected = false,
				IsSpecial = true
			});
			DrawRoadOutlineAndCrossSections(list, num13, num14, num15, planPreviewWidth, planPreviewHeight);
			DrawAncillaryStructureLayoutLines(list, num13, num14, num15, planPreviewWidth, planPreviewHeight);
		}
		catch (Exception ex)
		{
			_logger.Error("[CreateAncillaryStructureViewModel] 更新平面预览失败", ex);
		}
	}

	private void DrawRoadOutlineAndCrossSections(List<(double X, double Y, double StationKm)> planarPoints, double scale, double offsetX, double offsetY, double canvasW, double canvasH)
	{
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
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
			List<double> list = new List<double>();
			if (SelectedRoadProject?.StationParametersConfiguration?.Parameters != null && SelectedRoadProject.StationParametersConfiguration.Parameters.Count > 0)
			{
				list = SelectedRoadProject.StationParametersConfiguration.Parameters.Select((StationParameterData p) => p.StationKm).ToList();
			}
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
				double num4 = CalculateTangentAngle(planarPoints, tuple.Item3) + Math.PI / 2.0 - crossSectionOffsetAngleAtStation * Math.PI / 180.0;
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
			_logger.Error("[CreateAncillaryStructureViewModel] 绘制道路轮廓线和横断面线失败", ex);
		}
	}

	private void DrawAncillaryStructureLayoutLines(List<(double X, double Y, double StationKm)> planarPoints, double scale, double offsetX, double offsetY, double canvasW, double canvasH)
	{
		if (PlanLayoutPreviewData == null)
		{
			return;
		}
		foreach (AncillaryStructureDisplayModel item in AncillaryStructures.Where((AncillaryStructureDisplayModel s) => s.IsEnabled))
		{
			double offsetMeters = item.HorizontalOffset / 1000.0;
			foreach (AncillaryStructureLayoutSegment layoutSegment in item.LayoutSegments)
			{
				double startStationKm = layoutSegment.StartStationKm;
				double endStationKm = layoutSegment.EndStationKm;
				if (!(endStationKm <= startStationKm))
				{
					double modelLength = layoutSegment.ModelLength;
					switch (item.PositionType)
					{
					case AncillaryPositionType.Left:
						AddLayoutLineForSide(startStationKm, endStationKm, planarPoints, scale, offsetX, offsetY, canvasH, -1, offsetMeters, item, modelLength);
						break;
					case AncillaryPositionType.Right:
						AddLayoutLineForSide(startStationKm, endStationKm, planarPoints, scale, offsetX, offsetY, canvasH, 1, offsetMeters, item, modelLength);
						break;
					case AncillaryPositionType.Both:
						AddLayoutLineForSide(startStationKm, endStationKm, planarPoints, scale, offsetX, offsetY, canvasH, -1, offsetMeters, item, modelLength);
						AddLayoutLineForSide(startStationKm, endStationKm, planarPoints, scale, offsetX, offsetY, canvasH, 1, offsetMeters, item, modelLength);
						break;
					}
				}
			}
		}
	}

	private void AddLayoutLineForSide(double startStationKm, double endStationKm, List<(double X, double Y, double StationKm)> planarPoints, double scale, double offsetX, double offsetY, double canvasH, int sideSign, double offsetMeters, AncillaryStructureDisplayModel structure, double modelLengthMeters)
	{
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		if (PlanLayoutPreviewData == null || endStationKm <= startStationKm)
		{
			return;
		}
		if (modelLengthMeters <= 0.0)
		{
			modelLengthMeters = 5.0;
		}
		double num = endStationKm - startStationKm;
		double num2 = modelLengthMeters / 1000.0;
		if ((int)Math.Ceiling(num / num2) > 10000)
		{
			num2 = num / 10000.0;
			_logger.Warning($"[AddLayoutLineForSide] 布置范围过大 ({num * 1000.0:F1}m)，自动调整采样间隔为 {num2 * 1000.0:F1}m");
		}
		List<Point> list = new List<Point>();
		double num3 = startStationKm;
		int num4 = 0;
		List<double> list2 = new List<double>();
		while (num3 <= endStationKm + 1E-09 && num4 < 20000)
		{
			list2.Add(num3);
			num3 += num2;
			num4++;
			if (num4 > 20000)
			{
				_logger.Error($"[AddLayoutLineForSide] 采样次数超过安全限制 ({20000})，已终止循环。请检查布置范围和模型长度设置。");
				break;
			}
		}
		foreach (double item in list2)
		{
			(double, double, double)? tuple = FindClosestPointBinarySearch(planarPoints, item);
			if (tuple.HasValue)
			{
				double num5 = tuple.Value.Item1 * scale + offsetX;
				double num6 = canvasH - (tuple.Value.Item2 * scale + offsetY);
				double roadWidthAtStation = GetRoadWidthAtStation(tuple.Value.Item3, PreviewRoadWidth);
				double crossSectionOffsetAngleAtStation = GetCrossSectionOffsetAngleAtStation(tuple.Value.Item3, 0.0);
				double num7 = CalculateTangentAngle(planarPoints, tuple.Value.Item3) + Math.PI / 2.0 - crossSectionOffsetAngleAtStation * Math.PI / 180.0;
				double num8 = roadWidthAtStation / 2.0;
				double d = Math.Abs(crossSectionOffsetAngleAtStation * Math.PI / 180.0);
				double num9 = Math.Max(0.1, Math.Cos(d));
				double num10 = num8 / num9;
				double num11 = offsetMeters / num9;
				double num12 = num10 + num11;
				double num13 = num5 + (double)sideSign * num12 * Math.Cos(num7) * scale;
				double num14 = num6 + (double)sideSign * num12 * Math.Sin(num7) * scale;
				list.Add(new Point(num13, num14));
			}
		}
		if (list.Count >= 2)
		{
			for (int i = 0; i < list.Count - 1; i++)
			{
				PlanLayoutPreviewData.AncillaryStructureLayoutLines.Add(new AncillaryStructureLayoutLine
				{
					StartPoint = list[i],
					EndPoint = list[i + 1],
					Color = structure.Color,
					StrokeDashArray = new DoubleCollection(),
					StructureName = structure.Name,
					StartStationDisplay = FormatStationKm(startStationKm),
					EndStationDisplay = FormatStationKm(endStationKm)
				});
			}
		}
	}

	private static (double X, double Y, double StationKm)? FindClosestPointBinarySearch(List<(double X, double Y, double StationKm)> planarPoints, double targetStationKm)
	{
		if (planarPoints == null || planarPoints.Count == 0)
		{
			return null;
		}
		if (targetStationKm <= planarPoints[0].StationKm)
		{
			return planarPoints[0];
		}
		if (targetStationKm >= planarPoints[planarPoints.Count - 1].StationKm)
		{
			return planarPoints[planarPoints.Count - 1];
		}
		int num = 0;
		int num2 = planarPoints.Count - 1;
		while (num2 - num > 1)
		{
			int num3 = (num + num2) / 2;
			if (planarPoints[num3].StationKm < targetStationKm)
			{
				num = num3;
			}
			else
			{
				num2 = num3;
			}
		}
		double num4 = Math.Abs(planarPoints[num].StationKm - targetStationKm);
		double num5 = Math.Abs(planarPoints[num2].StationKm - targetStationKm);
		return (num4 < num5) ? planarPoints[num] : planarPoints[num2];
	}

	private double GetRoadWidthAtStation(double stationKm, double defaultWidth)
	{
		if (SelectedRoadProject?.StationParametersConfiguration?.Parameters == null || SelectedRoadProject.StationParametersConfiguration.Parameters.Count == 0)
		{
			return defaultWidth;
		}
		List<StationParameterData> list = SelectedRoadProject.StationParametersConfiguration.Parameters.OrderBy((StationParameterData p) => p.StationKm).ToList();
		StationParameterData stationParameterData = null;
		StationParameterData stationParameterData2 = null;
		foreach (StationParameterData item in list)
		{
			if (item.StationKm <= stationKm)
			{
				stationParameterData = item;
			}
			if (item.StationKm >= stationKm && stationParameterData2 == null)
			{
				stationParameterData2 = item;
			}
		}
		if (stationParameterData == null && stationParameterData2 == null)
		{
			return defaultWidth;
		}
		if (stationParameterData == null)
		{
			return stationParameterData2.RoadWidth ?? defaultWidth;
		}
		if (stationParameterData2 == null)
		{
			return stationParameterData.RoadWidth ?? defaultWidth;
		}
		double num = stationParameterData.RoadWidth ?? defaultWidth;
		double num2 = stationParameterData2.RoadWidth ?? defaultWidth;
		if (Math.Abs(stationParameterData2.StationKm - stationParameterData.StationKm) < 0.0001)
		{
			return num;
		}
		double num3 = (stationKm - stationParameterData.StationKm) / (stationParameterData2.StationKm - stationParameterData.StationKm);
		return num + (num2 - num) * num3;
	}

	private double GetCrossSectionOffsetAngleAtStation(double stationKm, double defaultAngle)
	{
		if (SelectedRoadProject?.StationParametersConfiguration?.Parameters == null || SelectedRoadProject.StationParametersConfiguration.Parameters.Count == 0)
		{
			return defaultAngle;
		}
		List<StationParameterData> list = SelectedRoadProject.StationParametersConfiguration.Parameters.OrderBy((StationParameterData p) => p.StationKm).ToList();
		StationParameterData stationParameterData = null;
		StationParameterData stationParameterData2 = null;
		foreach (StationParameterData item in list)
		{
			if (item.StationKm <= stationKm)
			{
				stationParameterData = item;
			}
			if (item.StationKm >= stationKm && stationParameterData2 == null)
			{
				stationParameterData2 = item;
			}
		}
		if (stationParameterData == null && stationParameterData2 == null)
		{
			return defaultAngle;
		}
		if (stationParameterData == null)
		{
			return stationParameterData2.CrossSectionOffsetAngle ?? defaultAngle;
		}
		if (stationParameterData2 == null)
		{
			return stationParameterData.CrossSectionOffsetAngle ?? defaultAngle;
		}
		double num = stationParameterData.CrossSectionOffsetAngle ?? defaultAngle;
		double num2 = stationParameterData2.CrossSectionOffsetAngle ?? defaultAngle;
		if (Math.Abs(stationParameterData2.StationKm - stationParameterData.StationKm) < 0.0001)
		{
			return num;
		}
		double num3 = (stationKm - stationParameterData.StationKm) / (stationParameterData2.StationKm - stationParameterData.StationKm);
		return num + (num2 - num) * num3;
	}

	private double CalculateTangentAngle(List<(double X, double Y, double StationKm)> planarPoints, double stationKm)
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
				(double, double, double) tuple2 = planarPoints[num + 1];
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
			return $"K{num}+{num2:F3}";
		}
		return $"K{num}+{(int)Math.Round(num2):D3}";
	}

	[RelayCommand]
	private void AddLayoutSegment()
	{
		if (SelectedStructureForLayout == null)
		{
			StatusMessage = "请先选择一个附属结构";
			return;
		}
		if (SelectedRoadProject == null)
		{
			StatusMessage = "请先选择路线";
			return;
		}
		AncillaryStructureLayoutSegment ancillaryStructureLayoutSegment = ((TempLayoutSegment == null) ? AncillaryStructureLayoutSegment.CreateEmpty() : new AncillaryStructureLayoutSegment
		{
			SegmentId = Guid.NewGuid(),
			StartStationKm = TempLayoutSegment.StartStationKm,
			EndStationKm = TempLayoutSegment.EndStationKm,
			ModelLength = TempLayoutSegment.ModelLength
		});
		SelectedStructureForLayout.LayoutSegments.Add(ancillaryStructureLayoutSegment);
		SelectedLayoutSegment = ancillaryStructureLayoutSegment;
		StatusMessage = $"已为 '{SelectedStructureForLayout.Name}' 添加布置段 ({ancillaryStructureLayoutSegment.StartStationDisplay} ~ {ancillaryStructureLayoutSegment.EndStationDisplay})";
		UpdatePlanPreview();
	}

	[RelayCommand]
	private void RemoveSelectedLayoutSegment()
	{
		if (SelectedLayoutSegment == null || SelectedStructureForLayout == null)
		{
			StatusMessage = "请先选择要删除的布置段";
			return;
		}
		if (SelectedStructureForLayout.LayoutSegments.Count <= 1)
		{
			StatusMessage = "每个结构至少需要保留一个布置段";
			return;
		}
		SelectedStructureForLayout.LayoutSegments.Remove(SelectedLayoutSegment);
		SelectedLayoutSegment = null;
		TempLayoutSegment = null;
		StatusMessage = "已删除 '" + SelectedStructureForLayout.Name + "' 的布置段";
		UpdatePlanPreview();
	}

	[RelayCommand]
	private void ApplyLayoutSegment()
	{
		if (SelectedLayoutSegment == null || TempLayoutSegment == null)
		{
			StatusMessage = "请先选择一个布置段";
			return;
		}
		if (TempLayoutSegment.EndStationKm <= TempLayoutSegment.StartStationKm)
		{
			MessageBox.Show("终止桩号必须大于起始桩号", "参数错误", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			StatusMessage = "终止桩号必须大于起始桩号";
			return;
		}
		if (TempLayoutSegment.ModelLength <= 0.0)
		{
			MessageBox.Show("模型长度必须大于0", "参数错误", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			StatusMessage = "模型长度必须大于0";
			return;
		}
		double num = (TempLayoutSegment.EndStationKm - TempLayoutSegment.StartStationKm) * 1000.0;
		int num2 = (int)Math.Ceiling(num / TempLayoutSegment.ModelLength);
		if (num2 <= 0)
		{
			MessageBox.Show($"当前参数计算的预计数量为 {num2}，请检查起始桩号、终止桩号和模型长度设置", "参数错误", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			StatusMessage = $"预计数量为 {num2}，无法应用修改";
		}
		else if (num2 > 10000 && MessageBox.Show($"当前设置将生成约 {num2} 个模型，这可能需要较长时间。\n\n建议：\n- 增大模型长度（当前 {TempLayoutSegment.ModelLength:F1}m）\n- 减小布置范围（当前 {num:F1}m）\n\n是否继续？", "采样点数量过多警告", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
		{
			StatusMessage = "已取消应用修改";
		}
		else
		{
			SelectedLayoutSegment.StartStationKm = TempLayoutSegment.StartStationKm;
			SelectedLayoutSegment.EndStationKm = TempLayoutSegment.EndStationKm;
			SelectedLayoutSegment.ModelLength = TempLayoutSegment.ModelLength;
			string value = SelectedStructureForLayout?.Name ?? "未知结构";
			StatusMessage = $"已应用修改：{value} ({SelectedLayoutSegment.StartStationDisplay} ~ {SelectedLayoutSegment.EndStationDisplay}，预计 {num2} 个)";
			UpdatePlanPreview();
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedRoadProjectChanged(RoadProject? value)
	{
		if (value != null && value.Centerline3DPoints != null && value.Centerline3DPoints.Count > 0)
		{
			double stationKm = value.Centerline3DPoints[0].StationKm;
			double stationKm2 = value.Centerline3DPoints[value.Centerline3DPoints.Count - 1].StationKm;
			StatusMessage = $"已选择路线: {value.Name}，桩号范围: K{stationKm * 1000.0:F0}~K{stationKm2 * 1000.0:F0}";
			LoadDataFromRoad();
			UpdatePlanPreview();
		}
		else if (value != null)
		{
			StatusMessage = "已选择路线: " + value.Name + "（未生成三维曲线）";
			LoadDataFromRoad();
		}
		else
		{
			AncillaryStructures.Clear();
			UpdateAncillaryStructurePreview();
			UpdatePlanPreview();
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedStructureForLayoutChanged(AncillaryStructureDisplayModel? value)
	{
		if (value == null)
		{
			SelectedLayoutSegment = null;
			TempLayoutSegment = null;
			return;
		}
		if (value.LayoutSegments.Count == 0)
		{
			AncillaryStructureLayoutSegment item = AncillaryStructureLayoutSegment.CreateEmpty();
			value.LayoutSegments.Add(item);
		}
		SelectedLayoutSegment = value.LayoutSegments.FirstOrDefault();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedLayoutSegmentChanged(AncillaryStructureLayoutSegment? value)
	{
		if (value == null)
		{
			if (TempLayoutSegment == null)
			{
				TempLayoutSegment = AncillaryStructureLayoutSegment.CreateEmpty();
			}
		}
		else
		{
			TempLayoutSegment = value.Clone();
		}
	}
}
