using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

using ServiceProvider = RevitAi.Abstractions.Loader.ServiceProvider;

namespace RevitAi.UI.ViewModels;

public class RoadModelPlacementViewModel : ObservableObject
{
	private readonly IRoadProjectManager _projectManager;

	private readonly IModelPlacementService _placementService;

	private readonly ILogger _logger;

	[ObservableProperty]
	private ObservableCollection<RoadProject> _projects = new ObservableCollection<RoadProject>();

	[ObservableProperty]
	private RoadProject? _selectedProject;

	[ObservableProperty]
	private PlacementScenario _selectedScenario;

	[ObservableProperty]
	private double _stationKm;

	[ObservableProperty]
	private double _startStationKm;

	[ObservableProperty]
	private double _endStationKm;

	[ObservableProperty]
	private double _placementInterval = 50.0;

	[ObservableProperty]
	private string _customStationList = string.Empty;

	[ObservableProperty]
	private double _lateralOffset;

	[ObservableProperty]
	private double _verticalOffset;

	[ObservableProperty]
	private double _rotationAngle;

	[ObservableProperty]
	private string _familyName = string.Empty;

	[ObservableProperty]
	private string _familyTypeName = string.Empty;

	[ObservableProperty]
	private string? _previewResult;

	[ObservableProperty]
	private string _statusMessage = "就绪";

	[ObservableProperty]
	private bool _isLoading;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? previewPlacementCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? executePlacementCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? refreshProjectsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	public object? CurrentDocument { get; set; }

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<RoadProject> Projects
	{
		get
		{
			return _projects;
		}
		[MemberNotNull("_projects")]
		set
		{
			if (!EqualityComparer<ObservableCollection<RoadProject>>.Default.Equals(_projects, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Projects);
				_projects = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Projects);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public RoadProject? SelectedProject
	{
		get
		{
			return _selectedProject;
		}
		set
		{
			if (!EqualityComparer<RoadProject>.Default.Equals(_selectedProject, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedProject);
				_selectedProject = value;
				OnSelectedProjectChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedProject);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public PlacementScenario SelectedScenario
	{
		get
		{
			return _selectedScenario;
		}
		set
		{
			if (!EqualityComparer<PlacementScenario>.Default.Equals(_selectedScenario, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedScenario);
				_selectedScenario = value;
				OnSelectedScenarioChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedScenario);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double StationKm
	{
		get
		{
			return _stationKm;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_stationKm, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StationKm);
				_stationKm = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StationKm);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double StartStationKm
	{
		get
		{
			return _startStationKm;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_startStationKm, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StartStationKm);
				_startStationKm = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StartStationKm);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double EndStationKm
	{
		get
		{
			return _endStationKm;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_endStationKm, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.EndStationKm);
				_endStationKm = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.EndStationKm);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double PlacementInterval
	{
		get
		{
			return _placementInterval;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_placementInterval, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PlacementInterval);
				_placementInterval = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PlacementInterval);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string CustomStationList
	{
		get
		{
			return _customStationList;
		}
		[MemberNotNull("_customStationList")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_customStationList, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CustomStationList);
				_customStationList = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CustomStationList);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double LateralOffset
	{
		get
		{
			return _lateralOffset;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_lateralOffset, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LateralOffset);
				_lateralOffset = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LateralOffset);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double VerticalOffset
	{
		get
		{
			return _verticalOffset;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_verticalOffset, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.VerticalOffset);
				_verticalOffset = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.VerticalOffset);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double RotationAngle
	{
		get
		{
			return _rotationAngle;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_rotationAngle, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.RotationAngle);
				_rotationAngle = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.RotationAngle);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string FamilyName
	{
		get
		{
			return _familyName;
		}
		[MemberNotNull("_familyName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_familyName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.FamilyName);
				_familyName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.FamilyName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string FamilyTypeName
	{
		get
		{
			return _familyTypeName;
		}
		[MemberNotNull("_familyTypeName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_familyTypeName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.FamilyTypeName);
				_familyTypeName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.FamilyTypeName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? PreviewResult
	{
		get
		{
			return _previewResult;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_previewResult, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PreviewResult);
				_previewResult = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PreviewResult);
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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand PreviewPlacementCommand => previewPlacementCommand ?? (previewPlacementCommand = new RelayCommand(PreviewPlacement));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ExecutePlacementCommand => executePlacementCommand ?? (executePlacementCommand = new RelayCommand(ExecutePlacement));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RefreshProjectsCommand => refreshProjectsCommand ?? (refreshProjectsCommand = new RelayCommand(RefreshProjects));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public RoadModelPlacementViewModel()
	{
		IServiceProvider services = UIBootstrapper.Services;
		_projectManager = services.GetRequiredService<IRoadProjectManager>();
		_placementService = services.GetRequiredService<IModelPlacementService>();
		_logger = ServiceProvider.GetLogger();
		LoadProjects();
	}

	private void LoadProjects()
	{
		try
		{
			Projects.Clear();
			foreach (RoadProject allProject in _projectManager.GetAllProjects())
			{
				Projects.Add(allProject);
			}
			RoadProject activeProject = _projectManager.GetActiveProject();
			if (activeProject != null)
			{
				SelectedProject = Projects.FirstOrDefault((RoadProject p) => p.Id == activeProject.Id);
			}
			StatusMessage = $"已加载 {Projects.Count} 个项目";
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelPlacementViewModel] 加载项目列表失败", ex);
			StatusMessage = "加载项目列表失败";
		}
	}

	[RelayCommand]
	private void PreviewPlacement()
	{
		try
		{
			if (SelectedProject == null)
			{
				MessageBox.Show("请先选择一个项目", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			else if (SelectedScenario == PlacementScenario.Single)
			{
				PreviewSinglePlacement();
			}
			else
			{
				MessageBox.Show("预览功能仅支持单个放置模式", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelPlacementViewModel] 预览失败", ex);
			MessageBox.Show("预览失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void PreviewSinglePlacement()
	{
		if (SelectedProject != null)
		{
			Result<ModelPlacementPreview> result = _placementService.PreviewPlacement(SelectedProject.Id, StationKm, LateralOffset);
			if (result.IsFailure)
			{
				PreviewResult = "预览失败: " + result.Error;
				return;
			}
			ModelPlacementPreview value = result.Value;
			PreviewResult = $"桩号: {value.FormattedStation}\n坐标: ({value.Position.X:F2}, {value.Position.Y:F2}, {value.Position.Z:F2}) 米\n方位角: {value.Azimuth:F2}°";
		}
	}

	[RelayCommand]
	private void ExecutePlacement()
	{
		try
		{
			if (CurrentDocument == null)
			{
				MessageBox.Show("未获取到 Revit 文档，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			if (SelectedProject == null)
			{
				MessageBox.Show("请先选择一个项目", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			if (string.IsNullOrEmpty(FamilyName) || string.IsNullOrEmpty(FamilyTypeName))
			{
				MessageBox.Show("请输入族名称和族类型名称", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			IsLoading = true;
			switch (SelectedScenario)
			{
			case PlacementScenario.Single:
				ExecuteSinglePlacement();
				break;
			case PlacementScenario.Interval:
				ExecuteIntervalPlacement();
				break;
			case PlacementScenario.CustomList:
				MessageBox.Show("自定义列表放置功能待实现", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				break;
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelPlacementViewModel] 执行放置失败", ex);
			MessageBox.Show("放置失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			IsLoading = false;
		}
	}

	private void ExecuteSinglePlacement()
	{
		if (SelectedProject != null && CurrentDocument != null)
		{
			ModelPlacementConfig config = new ModelPlacementConfig
			{
				ProjectId = SelectedProject.Id,
				StationKm = StationKm,
				LateralOffset = LateralOffset,
				VerticalOffset = VerticalOffset,
				RotationAngle = RotationAngle,
				FamilyName = FamilyName,
				FamilyTypeName = FamilyTypeName
			};
			Result<ModelPlacementResult> result = _placementService.PlaceModelAtStation(CurrentDocument, config);
			if (result.IsFailure)
			{
				MessageBox.Show("放置失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				StatusMessage = "放置失败";
				return;
			}
			StatusMessage = $"放置成功，桩号: {result.Value.StationKm:F3}";
			MessageBox.Show($"模型放置成功！\n\n桩号: {result.Value.StationKm:F3}", "放置成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
	}

	private void ExecuteIntervalPlacement()
	{
		if (SelectedProject == null || CurrentDocument == null)
		{
			return;
		}
		Result<List<ModelPlacementResult>> result = _placementService.PlaceModelsAlongRoad(CurrentDocument, SelectedProject.Id, StartStationKm, EndStationKm, PlacementInterval, FamilyName, FamilyTypeName, LateralOffset);
		if (result.IsFailure)
		{
			MessageBox.Show("放置失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			StatusMessage = "放置失败";
			return;
		}
		int value = result.Value.Count((ModelPlacementResult r) => r.IsSuccess);
		StatusMessage = $"批量放置完成，成功 {value}/{result.Value.Count} 个";
		MessageBox.Show($"批量放置完成！\n\n成功: {value}/{result.Value.Count} 个", "放置成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	[RelayCommand]
	private void RefreshProjects()
	{
		LoadProjects();
	}

	[RelayCommand]
	private void Close()
	{
		Application.Current?.Windows.OfType<RoadModelPlacementWindow>().FirstOrDefault()?.Close();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedProjectChanged(RoadProject? value)
	{
		if (value != null && value.Centerline3DPoints != null && value.Centerline3DPoints.Count > 0)
		{
			double stationKm = value.Centerline3DPoints[0].StationKm;
			double stationKm2 = value.Centerline3DPoints[value.Centerline3DPoints.Count - 1].StationKm;
			StartStationKm = stationKm;
			EndStationKm = stationKm2;
			StationKm = stationKm;
			StatusMessage = $"已选择项目: {value.Name}，桩号范围: {stationKm:F3} ~ {stationKm2:F3}";
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedScenarioChanged(PlacementScenario value)
	{
		PreviewResult = null;
		switch (value)
		{
		case PlacementScenario.Single:
			StatusMessage = "单个放置模式";
			break;
		case PlacementScenario.Interval:
			StatusMessage = "等间距放置模式";
			break;
		case PlacementScenario.CustomList:
			StatusMessage = "自定义列表放置模式";
			break;
		}
	}
}
