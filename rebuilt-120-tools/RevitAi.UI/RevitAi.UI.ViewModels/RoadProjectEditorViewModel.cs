using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using RevitAi.Abstractions.Adapters;
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

public class RoadProjectEditorViewModel : ObservableObject
{
	private readonly IRoadProjectManager _projectManager;

	private readonly IRoadProjectService? _projectService;

	private readonly IRoadCenterlineDataService _dataService;

	private readonly ILogger _logger;

	private readonly IFamilyDownloadService _familyDownloadService;

	private RoadProject? _originalProject;

	private bool _isNewProject;

	private bool _isSaved;

	[ObservableProperty]
	private RoadProject? _currentProject;

	[ObservableProperty]
	private string _projectName = string.Empty;

	[ObservableProperty]
	private double _originOffsetX;

	[ObservableProperty]
	private double _originOffsetY;

	[ObservableProperty]
	private double _originOffsetZ;

	[ObservableProperty]
	private double _calculationInterval = 0.5;

	[ObservableProperty]
	private double _annotationInterval = 100.0;

	[ObservableProperty]
	private bool _createStationAnnotations = true;

	[ObservableProperty]
	private RoadCenterlineDataSource _selectedDataSource;

	[ObservableProperty]
	private bool _hasStationElevationData;

	[ObservableProperty]
	private bool _hasCurveData;

	[ObservableProperty]
	private bool _isGenerated;

	[ObservableProperty]
	private string _statusMessage = "就绪";

	[ObservableProperty]
	private bool _isLoading;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? inputStationElevationCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? inputHorizontalVerticalCurveCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? drawCenterlineCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? saveCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	public bool IsProcessing => IsLoading;

	public bool CanDrawCenterline
	{
		get
		{
			if (!HasStationElevationData)
			{
				return HasCurveData;
			}
			return true;
		}
	}

	public bool IsCheckedStationElevation
	{
		get
		{
			return SelectedDataSource == RoadCenterlineDataSource.StationElevation;
		}
		set
		{
			if (value)
			{
				SelectedDataSource = RoadCenterlineDataSource.StationElevation;
			}
			OnPropertyChanged("IsCheckedStationElevation");
		}
	}

	public bool IsCheckedCurve
	{
		get
		{
			return SelectedDataSource == RoadCenterlineDataSource.HorizontalVerticalCurve;
		}
		set
		{
			if (value)
			{
				SelectedDataSource = RoadCenterlineDataSource.HorizontalVerticalCurve;
			}
			OnPropertyChanged("IsCheckedCurve");
		}
	}

	public bool IsSaved => _isSaved;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public RoadProject? CurrentProject
	{
		get
		{
			return _currentProject;
		}
		set
		{
			if (!EqualityComparer<RoadProject>.Default.Equals(_currentProject, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CurrentProject);
				_currentProject = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CurrentProject);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string ProjectName
	{
		get
		{
			return _projectName;
		}
		[MemberNotNull("_projectName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_projectName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ProjectName);
				_projectName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ProjectName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double OriginOffsetX
	{
		get
		{
			return _originOffsetX;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_originOffsetX, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.OriginOffsetX);
				_originOffsetX = value;
				OnOriginOffsetXChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.OriginOffsetX);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double OriginOffsetY
	{
		get
		{
			return _originOffsetY;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_originOffsetY, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.OriginOffsetY);
				_originOffsetY = value;
				OnOriginOffsetYChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.OriginOffsetY);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double OriginOffsetZ
	{
		get
		{
			return _originOffsetZ;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_originOffsetZ, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.OriginOffsetZ);
				_originOffsetZ = value;
				OnOriginOffsetZChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.OriginOffsetZ);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double CalculationInterval
	{
		get
		{
			return _calculationInterval;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_calculationInterval, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CalculationInterval);
				_calculationInterval = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CalculationInterval);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double AnnotationInterval
	{
		get
		{
			return _annotationInterval;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(_annotationInterval, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AnnotationInterval);
				_annotationInterval = value;
				OnAnnotationIntervalChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AnnotationInterval);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool CreateStationAnnotations
	{
		get
		{
			return _createStationAnnotations;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_createStationAnnotations, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CreateStationAnnotations);
				_createStationAnnotations = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CreateStationAnnotations);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public RoadCenterlineDataSource SelectedDataSource
	{
		get
		{
			return _selectedDataSource;
		}
		set
		{
			if (!EqualityComparer<RoadCenterlineDataSource>.Default.Equals(_selectedDataSource, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedDataSource);
				_selectedDataSource = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedDataSource);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasStationElevationData
	{
		get
		{
			return _hasStationElevationData;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasStationElevationData, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasStationElevationData);
				_hasStationElevationData = value;
				OnHasStationElevationDataChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasStationElevationData);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasCurveData
	{
		get
		{
			return _hasCurveData;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasCurveData, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasCurveData);
				_hasCurveData = value;
				OnHasCurveDataChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasCurveData);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsGenerated
	{
		get
		{
			return _isGenerated;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isGenerated, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsGenerated);
				_isGenerated = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsGenerated);
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
	public IRelayCommand InputStationElevationCommand => inputStationElevationCommand ?? (inputStationElevationCommand = new RelayCommand(InputStationElevation));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand InputHorizontalVerticalCurveCommand => inputHorizontalVerticalCurveCommand ?? (inputHorizontalVerticalCurveCommand = new RelayCommand(InputHorizontalVerticalCurve));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand DrawCenterlineCommand => drawCenterlineCommand ?? (drawCenterlineCommand = new AsyncRelayCommand(DrawCenterlineAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SaveCommand => saveCommand ?? (saveCommand = new RelayCommand(Save));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public RoadProjectEditorViewModel()
	{
		IServiceProvider services = UIBootstrapper.Services;
		_projectManager = services.GetRequiredService<IRoadProjectManager>();
		_dataService = services.GetRequiredService<IRoadCenterlineDataService>();
		_familyDownloadService = services.GetRequiredService<IFamilyDownloadService>();
		_logger = ServiceProvider.GetLogger();
		IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader()?.RevitAdapter;
		IInfrastructureService infrastructureService = revitAdapter?.InfrastructureService;
		if (infrastructureService == null && revitAdapter != null)
		{
			_logger.Info("[RoadProjectEditorViewModel] InfrastructureService 为 null，尝试主动创建");
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name == "RevitAi.Revit");
			if (assembly != null)
			{
				Type type = assembly.GetType("RevitAi.Revit.Services.InfrastructureService");
				if (type != null)
				{
					infrastructureService = Activator.CreateInstance(type) as IInfrastructureService;
					_logger.Info("[RoadProjectEditorViewModel] ✅ InfrastructureService 创建成功");
				}
				else
				{
					_logger.Error("[RoadProjectEditorViewModel] 未找到 InfrastructureService 类型");
				}
			}
		}
		if (infrastructureService != null)
		{
			Assembly assembly2 = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name == "RevitAi.Revit");
			if (assembly2 != null)
			{
				Type type2 = assembly2.GetType("RevitAi.Revit.Services.RoadProjectService");
				if (type2 != null)
				{
					_projectService = (IRoadProjectService)Activator.CreateInstance(type2, infrastructureService);
					if (_projectService == null)
					{
						_logger.Error("[RoadProjectEditorViewModel] 创建 RoadProjectService 失败");
						throw new InvalidOperationException("无法创建 RoadProjectService");
					}
					return;
				}
				_logger.Error("[RoadProjectEditorViewModel] 未找到 RoadProjectService 类型");
				throw new InvalidOperationException("未找到 RoadProjectService 类型");
			}
			_logger.Error("[RoadProjectEditorViewModel] 未找到 RevitAi.Revit 程序集");
			throw new InvalidOperationException("未找到 RevitAi.Revit 程序集");
		}
		_logger.Error("[RoadProjectEditorViewModel] InfrastructureService 为 null，且无法创建");
		throw new InvalidOperationException("无法获取或创建 InfrastructureService");
	}

	private void SyncOriginOffsetToService()
	{
		if (_dataService != null)
		{
			_dataService.SetGlobalOriginOffset(OriginOffsetX, OriginOffsetY, OriginOffsetZ);
		}
	}

	public void InitializeForNewProject()
	{
		_isNewProject = true;
		_isSaved = false;
		_originalProject = null;
		string text = "新建道路项目";
		string name = text;
		int num = 2;
		while (_projectManager.GetProjectByName(name) != null)
		{
			name = $"{text}_{num}";
			num++;
		}
		CurrentProject = new RoadProject
		{
			Name = name,
			Description = "",
			GlobalOriginOffset = (X: 0.0, Y: 0.0, Z: 0.0),
			CalculationInterval = 0.5,
			AnnotationInterval = 20.0,
			DataSource = RoadCenterlineDataSource.None
		};
		ProjectName = CurrentProject.Name;
		OriginOffsetX = CurrentProject.GlobalOriginOffset.X;
		OriginOffsetY = CurrentProject.GlobalOriginOffset.Y;
		OriginOffsetZ = CurrentProject.GlobalOriginOffset.Z;
		CalculationInterval = CurrentProject.CalculationInterval;
		AnnotationInterval = CurrentProject.AnnotationInterval;
		SelectedDataSource = CurrentProject.DataSource;
		HasStationElevationData = false;
		HasCurveData = false;
		IsGenerated = false;
		CreateStationAnnotations = true;
		StatusMessage = "新建项目";
	}

	public void InitializeForEdit(RoadProject project)
	{
		_isNewProject = false;
		_isSaved = false;
		_originalProject = project;
		CurrentProject = project;
		ProjectName = project.Name;
		OriginOffsetX = project.GlobalOriginOffset.X;
		OriginOffsetY = project.GlobalOriginOffset.Y;
		OriginOffsetZ = project.GlobalOriginOffset.Z;
		CalculationInterval = project.CalculationInterval;
		AnnotationInterval = project.AnnotationInterval;
		SelectedDataSource = project.DataSource;
		HasStationElevationData = project.StationElevationData != null && project.StationElevationData.Count > 0;
		HasCurveData = project.HorizontalCurveTable != null && project.VerticalCurveTable != null;
		IsGenerated = project.IsGenerated;
		CreateStationAnnotations = true;
		LoadProjectDataToService(project);
		StatusMessage = "编辑项目: " + project.Name;
	}

	private void LoadProjectDataToService(RoadProject project)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			_dataService.Clear();
			if (project.DataSource == RoadCenterlineDataSource.StationElevation && project.StationElevationData != null && project.StationElevationData.Count > 0)
			{
				Point originOffset = default(Point);
				originOffset = new Point(project.GlobalOriginOffset.X, project.GlobalOriginOffset.Y);
				_dataService.SetStationElevationData(project.StationElevationData, originOffset);
				_dataService.SetGlobalOriginOffset(project.GlobalOriginOffset.X, project.GlobalOriginOffset.Y, project.GlobalOriginOffset.Z);
				_dataService.SetAnnotationInterval(project.AnnotationInterval);
				_logger.Info($"[RoadProjectEditorViewModel] 已加载桩号高程表数据 ({project.StationElevationData.Count} 个点)");
			}
			else if (project.DataSource == RoadCenterlineDataSource.HorizontalVerticalCurve && project.HorizontalCurveTable != null && project.VerticalCurveTable != null)
			{
				_dataService.SetCurveData(project.HorizontalCurveTable, project.VerticalCurveTable);
				_dataService.SetGlobalOriginOffset(project.GlobalOriginOffset.X, project.GlobalOriginOffset.Y, project.GlobalOriginOffset.Z);
				_dataService.SetAnnotationInterval(project.AnnotationInterval);
				_logger.Info("[RoadProjectEditorViewModel] 已加载平纵曲线表数据");
			}
			_dataService.SetGlobalOriginOffset(project.GlobalOriginOffset.X, project.GlobalOriginOffset.Y, project.GlobalOriginOffset.Z);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectEditorViewModel] 加载项目数据到服务失败", ex);
		}
	}

	[RelayCommand]
	private void InputStationElevation()
	{
		try
		{
			StationElevationInputViewModel requiredService = UIBootstrapper.Services.GetRequiredService<StationElevationInputViewModel>();
			StationElevationInputWindow stationElevationInputWindow = new StationElevationInputWindow();
			stationElevationInputWindow.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsActive);
			stationElevationInputWindow.DataContext = requiredService;
			stationElevationInputWindow.ShowDialog();
			if (_dataService.GetStationElevationData() != null)
			{
				SelectedDataSource = RoadCenterlineDataSource.StationElevation;
				HasStationElevationData = true;
				HasCurveData = false;
				(double, double, double) globalOriginOffset = _dataService.GlobalOriginOffset;
				OriginOffsetX = globalOriginOffset.Item1;
				OriginOffsetY = globalOriginOffset.Item2;
				OriginOffsetZ = globalOriginOffset.Item3;
				StatusMessage = $"已输入桩号高程表数据，原点偏移: X={globalOriginOffset.Item1:F2}, Y={globalOriginOffset.Item2:F2}, Z={globalOriginOffset.Item3:F2}";
				if (CurrentProject != null)
				{
					CurrentProject.DataSource = RoadCenterlineDataSource.StationElevation;
					CurrentProject.StationElevationData = _dataService.GetStationElevationData();
					CurrentProject.GlobalOriginOffset = globalOriginOffset;
				}
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectEditorViewModel] 输入桩号高程表失败", ex);
			MessageBox.Show("输入失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void InputHorizontalVerticalCurve()
	{
		try
		{
			CurveInputViewModel requiredService = UIBootstrapper.Services.GetRequiredService<CurveInputViewModel>();
			CurveInputWindow curveInputWindow = new CurveInputWindow();
			curveInputWindow.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsActive);
			curveInputWindow.DataContext = requiredService;
			curveInputWindow.ShowDialog();
			HorizontalCurveTable horizontalCurveTable = _dataService.GetHorizontalCurveTable();
			VerticalCurveTable verticalCurveTable = _dataService.GetVerticalCurveTable();
			if (horizontalCurveTable != null && verticalCurveTable != null)
			{
				SelectedDataSource = RoadCenterlineDataSource.HorizontalVerticalCurve;
				HasCurveData = true;
				HasStationElevationData = false;
				(double, double, double) globalOriginOffset = _dataService.GlobalOriginOffset;
				OriginOffsetX = globalOriginOffset.Item1;
				OriginOffsetY = globalOriginOffset.Item2;
				OriginOffsetZ = globalOriginOffset.Item3;
				StatusMessage = $"已输入平纵曲线表数据，原点偏移: X={globalOriginOffset.Item1:F2}, Y={globalOriginOffset.Item2:F2}, Z={globalOriginOffset.Item3:F2}";
				if (CurrentProject != null)
				{
					CurrentProject.DataSource = RoadCenterlineDataSource.HorizontalVerticalCurve;
					CurrentProject.HorizontalCurveTable = horizontalCurveTable;
					CurrentProject.VerticalCurveTable = verticalCurveTable;
					CurrentProject.GlobalOriginOffset = globalOriginOffset;
				}
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectEditorViewModel] 输入平纵曲线表失败", ex);
			MessageBox.Show("输入失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private async Task DrawCenterlineAsync()
	{
		_ = 1;
		try
		{
			if (CurrentProject == null)
			{
				StatusMessage = "项目未初始化";
				return;
			}
			if (!CurrentProject.Validate())
			{
				StatusMessage = "项目数据验证失败，请先输入数据";
				return;
			}
			if (_projectService == null)
			{
				StatusMessage = "错误：项目服务未初始化";
				return;
			}
			IsLoading = true;
			StatusMessage = "正在计算曲线...";
			List<RoadCenterlinePoint3D> points3D;
			if (CurrentProject.DataSource == RoadCenterlineDataSource.StationElevation)
			{
				if (CurrentProject.StationElevationData == null || CurrentProject.StationElevationData.Count == 0)
				{
					IsLoading = false;
					StatusMessage = "桩号高程表数据为空";
					return;
				}
				_ = CurrentProject.GlobalOriginOffset;
				_ = CurrentProject.GlobalOriginOffset;
				_ = CurrentProject.GlobalOriginOffset;
				points3D = new List<RoadCenterlinePoint3D>(CurrentProject.StationElevationData.Count);
				foreach (RoadCenterlinePoint3D stationElevationDatum in CurrentProject.StationElevationData)
				{
					points3D.Add(new RoadCenterlinePoint3D
					{
						StationKm = stationElevationDatum.StationKm,
						X = stationElevationDatum.X,
						Y = stationElevationDatum.Y,
						Z = stationElevationDatum.Z,
						Azimuth = stationElevationDatum.Azimuth,
						Curvature = stationElevationDatum.Curvature,
						PointType = stationElevationDatum.PointType
					});
				}
				_logger.Info($"[RoadProjectEditorViewModel] 桩号高程表流程：生成 {points3D.Count} 个点");
			}
			else
			{
				if (CurrentProject.DataSource != RoadCenterlineDataSource.HorizontalVerticalCurve)
				{
					IsLoading = false;
					StatusMessage = $"不支持的数据源类型: {CurrentProject.DataSource}";
					return;
				}
				if (CurrentProject.HorizontalCurveTable == null || CurrentProject.VerticalCurveTable == null)
				{
					IsLoading = false;
					StatusMessage = "平纵曲线表数据为空";
					return;
				}
				Result<List<RoadCenterlinePoint3D>> result = _projectService.Calculate3DCenterline(CurrentProject);
				if (result.IsFailure)
				{
					IsLoading = false;
					StatusMessage = "计算三维中心线失败: " + result.Error;
					return;
				}
				points3D = result.Value;
				_logger.Info($"[RoadProjectEditorViewModel] ✅ 平纵曲线表流程：生成 {points3D.Count} 个点（统一采样，消除双重插值误差）");
			}
			CurrentProject.Centerline3DPoints = points3D;
			IsGenerated = true;
			try
			{
				Save();
			}
			catch (Exception ex)
			{
				_logger.Warning("[RoadProjectEditorViewModel] 自动保存失败，继续绘制: " + ex.Message);
				StatusMessage = "自动保存失败，继续绘制: " + ex.Message;
			}
			if (!string.IsNullOrWhiteSpace(ProjectName))
			{
				StatusMessage = $"计算成功，生成 {points3D.Count} 个点，正在绘制 Revit 曲线...";
			}
			List<RoadCenterlinePoint3D> value = points3D.Select((RoadCenterlinePoint3D p) => new RoadCenterlinePoint3D
			{
				StationKm = p.StationKm,
				X = p.X - CurrentProject.GlobalOriginOffset.X,
				Y = p.Y - CurrentProject.GlobalOriginOffset.Y,
				Z = p.Z - CurrentProject.GlobalOriginOffset.Z,
				Azimuth = p.Azimuth,
				Curvature = p.Curvature,
				PointType = p.PointType
			}).ToList();
			try
			{
				IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader()?.RevitAdapter;
				if (revitAdapter == null)
				{
					IsLoading = false;
					StatusMessage = "无法获取 Revit 适配器";
					return;
				}
				MethodInfo method = revitAdapter.GetType().GetMethod("GetRoadCenterlineExternalEvent");
				if (method == null)
				{
					IsLoading = false;
					StatusMessage = "无法获取 ExternalEvent";
					return;
				}
				object externalEvent = method.Invoke(revitAdapter, null);
				if (externalEvent == null)
				{
					IsLoading = false;
					StatusMessage = "ExternalEvent 未创建";
					return;
				}
				Type type = Type.GetType("RevitAi.Revit.RoadCenterline.RoadCenterlineExternalEventRequest, RevitAi.Revit");
				if (type == null)
				{
					IsLoading = false;
					StatusMessage = "找不到事件请求类型";
					return;
				}
				object request = Activator.CreateInstance(type);
				if (request == null)
				{
					IsLoading = false;
					StatusMessage = "无法创建事件请求实例";
					return;
				}
				PropertyInfo property = request.GetType().GetProperty("Points3D");
				PropertyInfo property2 = request.GetType().GetProperty("RoadProject");
				PropertyInfo property3 = request.GetType().GetProperty("RoadProjectName");
				PropertyInfo property4 = request.GetType().GetProperty("Document");
				PropertyInfo createAnnotationsProp = request.GetType().GetProperty("CreateAnnotations");
				PropertyInfo annotationIntervalProp = request.GetType().GetProperty("AnnotationIntervalFeet");
				if (property != null)
				{
					property.SetValue(request, value);
				}
				if (property2 != null)
				{
					property2.SetValue(request, CurrentProject);
				}
				if (property3 != null && CurrentProject != null)
				{
					property3.SetValue(request, CurrentProject.Name);
				}
				if (property4 != null)
				{
					try
					{
						object activeDocument = revitAdapter.GetActiveDocument();
						property4.SetValue(request, activeDocument);
					}
					catch
					{
						property4.SetValue(request, null);
					}
				}
				if (createAnnotationsProp != null)
				{
					createAnnotationsProp.SetValue(request, CreateStationAnnotations);
				}
				string annotationFamilyPath = null;
				if (CreateStationAnnotations)
				{
					if (await CheckFamilyExistsInProjectAsync("AST_R_桩号标注"))
					{
						annotationFamilyPath = string.Empty;
					}
					else
					{
						FamilyDownloadResult familyDownloadResult = await _familyDownloadService.DownloadFamilyFileAsync("AST_R_桩号标注");
						if (familyDownloadResult.IsSuccess && !string.IsNullOrEmpty(familyDownloadResult.FilePath))
						{
							annotationFamilyPath = familyDownloadResult.FilePath;
							_logger.Info("[RoadProjectEditorViewModel] 桩号标注族已下载: " + annotationFamilyPath);
						}
						else
						{
							_logger.Warning("[RoadProjectEditorViewModel] ⚠\ufe0f 桩号标注族下载失败: " + (familyDownloadResult.Error ?? "未知原因"));
							createAnnotationsProp?.SetValue(request, false);
						}
					}
				}
				PropertyInfo property5 = request.GetType().GetProperty("AnnotationFamilyPath");
				if (property5 != null)
				{
					property5.SetValue(request, annotationFamilyPath);
				}
				if (annotationIntervalProp != null)
				{
					annotationIntervalProp.SetValue(request, AnnotationInterval / 0.3048);
				}
				Action<Exception, bool> value2 = delegate(Exception? ex4, bool success)
				{
					IsLoading = false;
					if (ex4 != null)
					{
						_logger.Error("[RoadProjectEditorViewModel] 绘制 Revit 曲线失败", ex4);
						StatusMessage = "绘制失败: " + ex4.Message;
					}
					else if (success)
					{
						StatusMessage = $"绘制完成！生成 {points3D.Count} 个三维点" + (CreateStationAnnotations ? $"，已创建桩号标注（间距{AnnotationInterval}米）" : "");
						_logger.Info($"[RoadProjectEditorViewModel] 曲线绘制成功，生成 {points3D.Count} 个三维点");
					}
					else
					{
						StatusMessage = "绘制 Revit 曲线失败";
					}
				};
				request.GetType().GetProperty("OnCompleted")?.SetValue(request, value2);
				Type type2 = Type.GetType("RevitAi.Revit.RoadCenterline.RoadCenterlineExternalEventHandler, RevitAi.Revit");
				if (type2 != null)
				{
					type2.GetProperty("CurrentRequest")?.SetValue(null, request);
				}
				externalEvent.GetType().GetMethod("Raise")?.Invoke(externalEvent, null);
				StatusMessage = $"已提交绘制请求，正在生成 {points3D.Count} 个三维点...";
			}
			catch (Exception ex2)
			{
				IsLoading = false;
				_logger.Error("[RoadProjectEditorViewModel] 绘制 Revit 曲线失败", ex2);
				StatusMessage = "绘制失败: " + ex2.Message;
			}
		}
		catch (Exception ex3)
		{
			IsLoading = false;
			_logger.Error("[RoadProjectEditorViewModel] 绘制曲线失败", ex3);
			StatusMessage = "绘制失败: " + ex3.Message;
		}
	}

	[RelayCommand]
	private void Save()
	{
		try
		{
			if (CurrentProject == null)
			{
				MessageBox.Show("项目未初始化", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			if (string.IsNullOrWhiteSpace(ProjectName))
			{
				MessageBox.Show("项目名称不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			CurrentProject.Name = ProjectName;
			CurrentProject.Description = "";
			CurrentProject.GlobalOriginOffset = (X: OriginOffsetX, Y: OriginOffsetY, Z: OriginOffsetZ);
			CurrentProject.CalculationInterval = CalculationInterval;
			CurrentProject.AnnotationInterval = AnnotationInterval;
			CurrentProject.DataSource = SelectedDataSource;
			CurrentProject.UpdatedAt = DateTime.Now;
			Result<bool> result;
			if (_isNewProject)
			{
				RoadProject projectByName = _projectManager.GetProjectByName(ProjectName);
				if (projectByName != null)
				{
					if (MessageBox.Show("项目名称 '" + ProjectName + "' 已存在。\n\n是否要覆盖现有项目的数据和参数？", "确认覆盖", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
					{
						StatusMessage = "保存已取消";
						return;
					}
					CurrentProject.Id = projectByName.Id;
					CurrentProject.CreatedAt = projectByName.CreatedAt;
					result = _projectManager.UpdateProject(CurrentProject);
				}
				else
				{
					Result<RoadProject> result2 = _projectManager.AddProject(ProjectName, "");
					if (result2.IsSuccess && result2.Value != null)
					{
						RoadProject value = result2.Value;
						value.DataSource = CurrentProject.DataSource;
						value.StationElevationData = CurrentProject.StationElevationData;
						value.HorizontalCurveTable = CurrentProject.HorizontalCurveTable;
						value.VerticalCurveTable = CurrentProject.VerticalCurveTable;
						value.GlobalOriginOffset = CurrentProject.GlobalOriginOffset;
						value.CalculationInterval = CurrentProject.CalculationInterval;
						value.AnnotationInterval = CurrentProject.AnnotationInterval;
						value.Centerline3DPoints = CurrentProject.Centerline3DPoints;
						result = _projectManager.UpdateProject(value);
						CurrentProject = value;
					}
					else
					{
						result = Result<bool>.Failure(result2.Error ?? "添加项目失败");
					}
				}
			}
			else
			{
				result = _projectManager.UpdateProject(CurrentProject);
			}
			if (result.IsFailure)
			{
				MessageBox.Show("保存失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			_isSaved = true;
			StatusMessage = "项目 '" + ProjectName + "' 已保存";
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectEditorViewModel] 保存项目失败", ex);
			MessageBox.Show("保存失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void Close()
	{
		Application.Current?.Windows.OfType<RoadProjectEditorWindow>().FirstOrDefault()?.Close();
	}

	private async Task<bool> CheckFamilyExistsInProjectAsync(string familyName)
	{
		_ = 1;
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader()?.RevitAdapter;
			if (revitAdapter == null)
			{
				_logger.Warning("[RoadProjectEditorViewModel] 无法获取 Revit 适配器");
				return false;
			}
			object familyCheckExternalEvent = revitAdapter.GetFamilyCheckExternalEvent();
			if (familyCheckExternalEvent == null)
			{
				_logger.Warning("[RoadProjectEditorViewModel] 无法获取族检查 ExternalEvent");
				return false;
			}
			Type type = Type.GetType("RevitAi.Revit.Revit.FamilyCheckRequest, RevitAi.Revit");
			if (type == null)
			{
				_logger.Warning("[RoadProjectEditorViewModel] 无法找到 FamilyCheckRequest 类型");
				return false;
			}
			object obj = Activator.CreateInstance(type);
			PropertyInfo property = type.GetProperty("FamilyName");
			PropertyInfo? property2 = type.GetProperty("TaskSource");
			if (property != null)
			{
				property.SetValue(obj, familyName);
			}
			object obj2 = property2?.GetValue(obj);
			if (obj2 == null)
			{
				_logger.Warning("[RoadProjectEditorViewModel] 无法获取 TaskSource");
				return false;
			}
			Type type2 = Type.GetType("RevitAi.Revit.Revit.FamilyCheckRequestManager, RevitAi.Revit");
			if (type2 == null)
			{
				_logger.Warning("[RoadProjectEditorViewModel] 无法找到 FamilyCheckRequestManager 类型");
				return false;
			}
			MethodInfo method = type2.GetMethod("SetRequest");
			if (method == null)
			{
				_logger.Warning("[RoadProjectEditorViewModel] 无法找到 SetRequest 方法");
				return false;
			}
			method.Invoke(null, new object[1] { obj });
			familyCheckExternalEvent.GetType().GetMethod("Raise")?.Invoke(familyCheckExternalEvent, null);
			if (!(obj2.GetType().GetProperty("Task")?.GetValue(obj2) is Task<bool> task))
			{
				_logger.Warning("[RoadProjectEditorViewModel] 无法获取 Task");
				return false;
			}
			if (await Task.WhenAny(task, Task.Delay(5000)) != task)
			{
				_logger.Warning("[RoadProjectEditorViewModel] 族检查超时");
				return false;
			}
			return await task;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectEditorViewModel] 族检查失败", ex);
			return false;
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnOriginOffsetXChanged(double value)
	{
		SyncOriginOffsetToService();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnOriginOffsetYChanged(double value)
	{
		SyncOriginOffsetToService();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnOriginOffsetZChanged(double value)
	{
		SyncOriginOffsetToService();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnAnnotationIntervalChanged(double value)
	{
		if (_dataService != null)
		{
			_dataService.SetAnnotationInterval(value);
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnHasStationElevationDataChanged(bool value)
	{
		OnPropertyChanged("CanDrawCenterline");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnHasCurveDataChanged(bool value)
	{
		OnPropertyChanged("CanDrawCenterline");
	}
}
