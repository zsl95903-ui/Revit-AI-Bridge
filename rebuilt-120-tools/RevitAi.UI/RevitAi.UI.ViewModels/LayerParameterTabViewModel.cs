using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace RevitAi.UI.ViewModels;

public class LayerParameterTabViewModel : ObservableObject
{
	public class CADLayerItem : ObservableObject
	{
		private string _name;

		private bool _isManholeSelected;

		private bool _isManholeAnnotationSelected;

		private bool _isPipeSelected;

		private bool _isPipeAnnotationSelected;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				SetProperty(ref _name, value, "Name");
			}
		}

		public bool IsManholeSelected
		{
			get
			{
				return _isManholeSelected;
			}
			set
			{
				SetProperty(ref _isManholeSelected, value, "IsManholeSelected");
			}
		}

		public bool IsManholeAnnotationSelected
		{
			get
			{
				return _isManholeAnnotationSelected;
			}
			set
			{
				SetProperty(ref _isManholeAnnotationSelected, value, "IsManholeAnnotationSelected");
			}
		}

		public bool IsPipeSelected
		{
			get
			{
				return _isPipeSelected;
			}
			set
			{
				SetProperty(ref _isPipeSelected, value, "IsPipeSelected");
			}
		}

		public bool IsPipeAnnotationSelected
		{
			get
			{
				return _isPipeAnnotationSelected;
			}
			set
			{
				SetProperty(ref _isPipeAnnotationSelected, value, "IsPipeAnnotationSelected");
			}
		}

		public CADLayerItem(string name)
		{
			_name = name;
		}
	}

	private readonly ILogger _logger;

	private ICADFileService? _cadFileService;

	private IFamilyDownloadService? _familyDownloadService;

	[ObservableProperty]
	private string? _cadFilePath;

	[ObservableProperty]
	private string? _cadFileName;

	[ObservableProperty]
	private bool _hasCadFile;

	[ObservableProperty]
	private SelectionMode _currentSelectionMode;

	[ObservableProperty]
	private string? _selectionSummary = "未选择";

	[ObservableProperty]
	private string? _detectedLayers = "";

	[ObservableProperty]
	private bool _hasLayerInfo;

	[ObservableProperty]
	private int _selectedManholeCount;

	[ObservableProperty]
	private int _selectedManholeAnnotationCount;

	[ObservableProperty]
	private int _selectedPipeCount;

	[ObservableProperty]
	private int _selectedPipeAnnotationCount;

	private HashSet<string> _manholeLayerNames = new HashSet<string>();

	private HashSet<string> _manholeAnnotationLayerNames = new HashSet<string>();

	private HashSet<string> _pipeLayerNames = new HashSet<string>();

	private HashSet<string> _pipeAnnotationLayerNames = new HashSet<string>();

	[ObservableProperty]
	private List<CADLayerItem> _availableLayers = new List<CADLayerItem>();

	[ObservableProperty]
	private bool _hasAvailableLayers;

	[ObservableProperty]
	private bool _isLoadingLayers;

	[ObservableProperty]
	private bool _isAnalyzing;

	[ObservableProperty]
	private string? _analysisProgress;

	[ObservableProperty]
	private int _analysisProgressPercentage;

	[ObservableProperty]
	private bool _isGenerating;

	[ObservableProperty]
	private string? _generationProgress;

	[ObservableProperty]
	private int _generationProgressPercentage;

	[ObservableProperty]
	private string? _generationResult;

	private PipeNetworkAnalysisResult? _analysisResult;

	[ObservableProperty]
	private string? _parameterTablePath;

	[ObservableProperty]
	private string? _parameterTableName;

	[ObservableProperty]
	private string? _parameterSheetName;

	[ObservableProperty]
	private bool _hasParameterTable;

	[ObservableProperty]
	private string _pipeSystemName = "排水管";

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? exportSampleTableCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? selectCadFileCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? updateSelectedLayersCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectManholesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectManholeAnnotationsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectPipesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectPipeAnnotationsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? importParameterTableCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? generateNetworkCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? analyzeCADCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? CadFilePath
	{
		get
		{
			return _cadFilePath;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_cadFilePath, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CadFilePath);
				_cadFilePath = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CadFilePath);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? CadFileName
	{
		get
		{
			return _cadFileName;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_cadFileName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CadFileName);
				_cadFileName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CadFileName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasCadFile
	{
		get
		{
			return _hasCadFile;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasCadFile, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasCadFile);
				_hasCadFile = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasCadFile);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public SelectionMode CurrentSelectionMode
	{
		get
		{
			return _currentSelectionMode;
		}
		set
		{
			if (!EqualityComparer<SelectionMode>.Default.Equals(_currentSelectionMode, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CurrentSelectionMode);
				_currentSelectionMode = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CurrentSelectionMode);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? SelectionSummary
	{
		get
		{
			return _selectionSummary;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectionSummary, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectionSummary);
				_selectionSummary = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectionSummary);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? DetectedLayers
	{
		get
		{
			return _detectedLayers;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_detectedLayers, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DetectedLayers);
				_detectedLayers = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DetectedLayers);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasLayerInfo
	{
		get
		{
			return _hasLayerInfo;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasLayerInfo, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasLayerInfo);
				_hasLayerInfo = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasLayerInfo);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int SelectedManholeCount
	{
		get
		{
			return _selectedManholeCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_selectedManholeCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedManholeCount);
				_selectedManholeCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedManholeCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int SelectedManholeAnnotationCount
	{
		get
		{
			return _selectedManholeAnnotationCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_selectedManholeAnnotationCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedManholeAnnotationCount);
				_selectedManholeAnnotationCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedManholeAnnotationCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int SelectedPipeCount
	{
		get
		{
			return _selectedPipeCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_selectedPipeCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedPipeCount);
				_selectedPipeCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedPipeCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int SelectedPipeAnnotationCount
	{
		get
		{
			return _selectedPipeAnnotationCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_selectedPipeAnnotationCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedPipeAnnotationCount);
				_selectedPipeAnnotationCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedPipeAnnotationCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public List<CADLayerItem> AvailableLayers
	{
		get
		{
			return _availableLayers;
		}
		[MemberNotNull("_availableLayers")]
		set
		{
			if (!EqualityComparer<List<CADLayerItem>>.Default.Equals(_availableLayers, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AvailableLayers);
				_availableLayers = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AvailableLayers);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasAvailableLayers
	{
		get
		{
			return _hasAvailableLayers;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasAvailableLayers, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasAvailableLayers);
				_hasAvailableLayers = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasAvailableLayers);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsLoadingLayers
	{
		get
		{
			return _isLoadingLayers;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLoadingLayers, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsLoadingLayers);
				_isLoadingLayers = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsLoadingLayers);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsAnalyzing
	{
		get
		{
			return _isAnalyzing;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isAnalyzing, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsAnalyzing);
				_isAnalyzing = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsAnalyzing);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? AnalysisProgress
	{
		get
		{
			return _analysisProgress;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_analysisProgress, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AnalysisProgress);
				_analysisProgress = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AnalysisProgress);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int AnalysisProgressPercentage
	{
		get
		{
			return _analysisProgressPercentage;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_analysisProgressPercentage, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AnalysisProgressPercentage);
				_analysisProgressPercentage = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AnalysisProgressPercentage);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsGenerating
	{
		get
		{
			return _isGenerating;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isGenerating, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsGenerating);
				_isGenerating = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsGenerating);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? GenerationProgress
	{
		get
		{
			return _generationProgress;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_generationProgress, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.GenerationProgress);
				_generationProgress = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.GenerationProgress);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int GenerationProgressPercentage
	{
		get
		{
			return _generationProgressPercentage;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_generationProgressPercentage, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.GenerationProgressPercentage);
				_generationProgressPercentage = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.GenerationProgressPercentage);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? GenerationResult
	{
		get
		{
			return _generationResult;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_generationResult, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.GenerationResult);
				_generationResult = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.GenerationResult);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? ParameterTablePath
	{
		get
		{
			return _parameterTablePath;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_parameterTablePath, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ParameterTablePath);
				_parameterTablePath = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ParameterTablePath);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? ParameterTableName
	{
		get
		{
			return _parameterTableName;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_parameterTableName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ParameterTableName);
				_parameterTableName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ParameterTableName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? ParameterSheetName
	{
		get
		{
			return _parameterSheetName;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_parameterSheetName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ParameterSheetName);
				_parameterSheetName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ParameterSheetName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasParameterTable
	{
		get
		{
			return _hasParameterTable;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasParameterTable, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasParameterTable);
				_hasParameterTable = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasParameterTable);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string PipeSystemName
	{
		get
		{
			return _pipeSystemName;
		}
		[MemberNotNull("_pipeSystemName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_pipeSystemName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PipeSystemName);
				_pipeSystemName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PipeSystemName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ExportSampleTableCommand => exportSampleTableCommand ?? (exportSampleTableCommand = new RelayCommand(ExportSampleTable));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SelectCadFileCommand => selectCadFileCommand ?? (selectCadFileCommand = new AsyncRelayCommand(SelectCadFileAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand UpdateSelectedLayersCommand => updateSelectedLayersCommand ?? (updateSelectedLayersCommand = new RelayCommand(UpdateSelectedLayers));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectManholesCommand => selectManholesCommand ?? (selectManholesCommand = new RelayCommand(SelectManholes));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectManholeAnnotationsCommand => selectManholeAnnotationsCommand ?? (selectManholeAnnotationsCommand = new RelayCommand(SelectManholeAnnotations));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectPipesCommand => selectPipesCommand ?? (selectPipesCommand = new RelayCommand(SelectPipes));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectPipeAnnotationsCommand => selectPipeAnnotationsCommand ?? (selectPipeAnnotationsCommand = new RelayCommand(SelectPipeAnnotations));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ImportParameterTableCommand => importParameterTableCommand ?? (importParameterTableCommand = new RelayCommand(ImportParameterTable));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand GenerateNetworkCommand => generateNetworkCommand ?? (generateNetworkCommand = new AsyncRelayCommand(GenerateNetwork));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand AnalyzeCADCommand => analyzeCADCommand ?? (analyzeCADCommand = new AsyncRelayCommand(AnalyzeCAD));

	public LayerParameterTabViewModel()
	{
		_logger = ServiceProvider.GetLogger();
		_cadFileService = TryGetService<ICADFileService>();
		_familyDownloadService = TryGetService<IFamilyDownloadService>();
	}

	private T? TryGetService<T>() where T : class
	{
		try
		{
			return UIBootstrapper.TryGetService<T>();
		}
		catch
		{
			return null;
		}
	}

	[RelayCommand]
	private void ExportSampleTable()
	{
		try
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Excel文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*",
				FileName = "管井参数表样例.xlsx",
				Title = "导出管井参数表样例"
			};
			if (saveFileDialog.ShowDialog() != true)
			{
				return;
			}
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (directoryName == null)
			{
				_logger.Error("[LayerParameterTab] 无法获取程序集目录");
				MessageBox.Show("无法获取程序集目录", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			string[] obj = new string[3] { "Resources\\Templates\\管井参数表样例.xlsx", null, null };
			InlineArray6<string> buffer = default(InlineArray6<string>);
			buffer[0] = directoryName;
			buffer[1] = "..";
			buffer[2] = "..";
			buffer[3] = "Resources";
			buffer[4] = "Templates";
			buffer[5] = "管井参数表样例.xlsx";
			obj[1] = Path.Combine(buffer);
			InlineArray5<string> buffer2 = default(InlineArray5<string>);
			buffer2[0] = directoryName;
			buffer2[1] = "..";
			buffer2[2] = "Resources";
			buffer2[3] = "Templates";
			buffer2[4] = "管井参数表样例.xlsx";
			obj[2] = Path.Combine(buffer2);
			string text = null;
			string[] array = obj;
			foreach (string path in array)
			{
				try
				{
					string fullPath = Path.GetFullPath(path);
					if (File.Exists(fullPath))
					{
						text = fullPath;
						break;
					}
				}
				catch
				{
				}
			}
			if (text == null)
			{
				_logger.Error("[LayerParameterTab] 未找到样例文件: 管井参数表样例.xlsx");
				MessageBox.Show("未找到样例文件：管井参数表样例.xlsx\n\n请确保 Resources/Templates/ 文件夹中存在此文件", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			else
			{
				File.Copy(text, saveFileDialog.FileName, overwrite: true);
				MessageBox.Show("样例表格已导出到：\n" + saveFileDialog.FileName + "\n\n请打开文件，根据实际需求修改数据后导入。", "导出成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				_logger.Info("[LayerParameterTab] 导出管井参数表样例成功: " + saveFileDialog.FileName);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[LayerParameterTab] 导出样例失败", ex);
			MessageBox.Show("导出样例失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private async Task SelectCadFileAsync()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择CAD文件",
				Filter = "CAD文件 (*.dwg;*.dxf)|*.dwg;*.dxf|所有文件 (*.*)|*.*",
				Multiselect = false
			};
			if (openFileDialog.ShowDialog() == true)
			{
				CadFilePath = openFileDialog.FileName;
				CadFileName = Path.GetFileName(openFileDialog.FileName);
				HasCadFile = true;
				_logger.Info("已选择CAD文件: " + CadFileName);
				_manholeLayerNames.Clear();
				_manholeAnnotationLayerNames.Clear();
				_pipeLayerNames.Clear();
				_pipeAnnotationLayerNames.Clear();
				AvailableLayers.Clear();
				HasAvailableLayers = false;
				UpdateSelectionSummary();
				await LoadLayersFromFileAsync();
			}
		}
		catch (Exception ex)
		{
			_logger.Error("选择CAD文件失败", ex);
			MessageBox.Show("选择CAD文件失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private async Task LoadLayersFromFileAsync()
	{
		if (string.IsNullOrEmpty(CadFilePath) || _cadFileService == null)
		{
			MessageBox.Show("请先选择CAD文件", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		IsLoadingLayers = true;
		try
		{
			List<string> list = await Task.Run(() => _cadFileService.GetLayersFromCADFile(CadFilePath));
			if (list.Count > 0)
			{
				AvailableLayers = (from name in list
					select new CADLayerItem(name) into l
					orderby l.Name
					select l).ToList();
				HasAvailableLayers = true;
				_logger.Info($"[LayerParameterTab] 成功加载 {list.Count} 个图层");
			}
			else
			{
				AvailableLayers.Clear();
				HasAvailableLayers = false;
				MessageBox.Show("未能从CAD文件中读取图层信息", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[LayerParameterTab] 加载图层失败: " + ex.Message, ex);
			MessageBox.Show("加载图层失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			IsLoadingLayers = false;
		}
	}

	[RelayCommand]
	private void UpdateSelectedLayers()
	{
		_manholeLayerNames.Clear();
		_manholeAnnotationLayerNames.Clear();
		_pipeLayerNames.Clear();
		_pipeAnnotationLayerNames.Clear();
		foreach (CADLayerItem availableLayer in AvailableLayers)
		{
			if (availableLayer.IsManholeSelected)
			{
				_manholeLayerNames.Add(availableLayer.Name);
			}
			if (availableLayer.IsManholeAnnotationSelected)
			{
				_manholeAnnotationLayerNames.Add(availableLayer.Name);
			}
			if (availableLayer.IsPipeSelected)
			{
				_pipeLayerNames.Add(availableLayer.Name);
			}
			if (availableLayer.IsPipeAnnotationSelected)
			{
				_pipeAnnotationLayerNames.Add(availableLayer.Name);
			}
		}
		UpdateSelectionSummary();
	}

	[RelayCommand]
	private void SelectManholes()
	{
		RequestCADElementSelection(SelectionMode.Manholes, "请选择管井图元（单击选择，选择后自动完成）");
	}

	[RelayCommand]
	private void SelectManholeAnnotations()
	{
		RequestCADElementSelection(SelectionMode.ManholeAnnotations, "请选择管井标注图元（可多选，完成后点击左上角完成按钮）");
	}

	[RelayCommand]
	private void SelectPipes()
	{
		RequestCADElementSelection(SelectionMode.Pipes, "请选择管道图元（单击选择，选择后自动完成）");
	}

	[RelayCommand]
	private void SelectPipeAnnotations()
	{
		RequestCADElementSelection(SelectionMode.PipeAnnotations, "请选择管道标注图元（可多选，完成后点击左上角完成按钮）");
	}

	private void RequestCADElementSelection(SelectionMode mode, string prompt)
	{
		try
		{
			CurrentSelectionMode = mode;
			_logger.Info($"[LayerParameterTab] 开始选择 {mode} 图元");
			Type typeFromHandle = typeof(CADElementSelectionRequest);
			if (!(Activator.CreateInstance(typeFromHandle) is CADElementSelectionRequest cADElementSelectionRequest))
			{
				MessageBox.Show("无法创建选择请求。", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			PropertyInfo property = typeFromHandle.GetProperty("Mode");
			PropertyInfo property2 = typeFromHandle.GetProperty("Prompt");
			PropertyInfo property3 = typeFromHandle.GetProperty("ObjectType");
			PropertyInfo property4 = typeFromHandle.GetProperty("AllowMultiple");
			PropertyInfo? property5 = typeFromHandle.GetProperty("OnCompleted");
			property?.SetValue(cADElementSelectionRequest, mode);
			property2?.SetValue(cADElementSelectionRequest, prompt);
			SelectionObjectType objectTypeForMode = GetObjectTypeForMode(mode);
			property3?.SetValue(cADElementSelectionRequest, objectTypeForMode);
			bool allowMultiple = GetAllowMultiple(mode);
			property4?.SetValue(cADElementSelectionRequest, allowMultiple);
			Action<bool, List<int>, List<string>> value = delegate(bool success, List<int> elementIds, List<string> layerNames)
			{
				((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
				{
					CurrentSelectionMode = SelectionMode.None;
					if (success && layerNames.Count > 0)
					{
						_logger.Info($"[LayerParameterTab] {mode} 选择成功，检测到 {layerNames.Count} 个图层: {string.Join(", ", layerNames)}");
						AddLayersToSelection(mode, layerNames);
					}
					else
					{
						_logger.Info($"[LayerParameterTab] {mode} 选择已取消");
					}
				});
			};
			property5?.SetValue(cADElementSelectionRequest, value);
			if (!TriggerStaticCADElementSelection(cADElementSelectionRequest))
			{
				CurrentSelectionMode = SelectionMode.None;
				MessageBox.Show("CAD 元素选择功能未初始化。\n\n请尝试重新打开此窗口。", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			_logger.Info($"[LayerParameterTab] 已触发 {mode} 图元选择");
		}
		catch (Exception ex)
		{
			_logger.Error($"[LayerParameterTab] 选择 {mode} 时出错: {ex.Message}", ex);
			CurrentSelectionMode = SelectionMode.None;
			MessageBox.Show("选择图元时出错: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private SelectionObjectType GetObjectTypeForMode(SelectionMode mode)
	{
		return SelectionObjectType.PointOnElement;
	}

	private bool GetAllowMultiple(SelectionMode mode)
	{
		return mode switch
		{
			SelectionMode.ManholeAnnotations => true, 
			SelectionMode.PipeAnnotations => true, 
			_ => false, 
		};
	}

	private bool TriggerStaticCADElementSelection(CADElementSelectionRequest request)
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				_logger.Error("[LayerParameterTab] 无法获取 RevitAdapter");
				return false;
			}
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerCADElementSelection");
			if (method == null)
			{
				_logger.Error("[LayerParameterTab] 未找到 TriggerCADElementSelection 方法");
				return false;
			}
			if (method.Invoke(revitAdapter, new object[1] { request }) is bool result)
			{
				return result;
			}
			return false;
		}
		catch (Exception ex)
		{
			_logger.Error("[LayerParameterTab] 触发 CAD 选择失败: " + ex.Message, ex);
			return false;
		}
	}

	private void AddLayersToSelection(SelectionMode mode, List<string> layerNames)
	{
		switch (mode)
		{
		case SelectionMode.Manholes:
			foreach (CADLayerItem availableLayer in AvailableLayers)
			{
				availableLayer.IsManholeSelected = false;
			}
			_manholeLayerNames.Clear();
			break;
		case SelectionMode.ManholeAnnotations:
			foreach (CADLayerItem availableLayer2 in AvailableLayers)
			{
				availableLayer2.IsManholeAnnotationSelected = false;
			}
			_manholeAnnotationLayerNames.Clear();
			break;
		case SelectionMode.Pipes:
			foreach (CADLayerItem availableLayer3 in AvailableLayers)
			{
				availableLayer3.IsPipeSelected = false;
			}
			_pipeLayerNames.Clear();
			break;
		case SelectionMode.PipeAnnotations:
			foreach (CADLayerItem availableLayer4 in AvailableLayers)
			{
				availableLayer4.IsPipeAnnotationSelected = false;
			}
			_pipeAnnotationLayerNames.Clear();
			break;
		}
		foreach (string layerName in layerNames)
		{
			CADLayerItem cADLayerItem = AvailableLayers.FirstOrDefault((CADLayerItem l) => l.Name == layerName);
			if (cADLayerItem == null)
			{
				CADLayerItem cADLayerItem2 = new CADLayerItem(layerName);
				switch (mode)
				{
				case SelectionMode.Manholes:
					cADLayerItem2.IsManholeSelected = true;
					break;
				case SelectionMode.ManholeAnnotations:
					cADLayerItem2.IsManholeAnnotationSelected = true;
					break;
				case SelectionMode.Pipes:
					cADLayerItem2.IsPipeSelected = true;
					break;
				case SelectionMode.PipeAnnotations:
					cADLayerItem2.IsPipeAnnotationSelected = true;
					break;
				}
				AvailableLayers.Add(cADLayerItem2);
			}
			else
			{
				switch (mode)
				{
				case SelectionMode.Manholes:
					cADLayerItem.IsManholeSelected = true;
					break;
				case SelectionMode.ManholeAnnotations:
					cADLayerItem.IsManholeAnnotationSelected = true;
					break;
				case SelectionMode.Pipes:
					cADLayerItem.IsPipeSelected = true;
					break;
				case SelectionMode.PipeAnnotations:
					cADLayerItem.IsPipeAnnotationSelected = true;
					break;
				}
			}
		}
		UpdateSelectedLayers();
		HasAvailableLayers = AvailableLayers.Count > 0;
	}

	private void UpdateSelectionSummary()
	{
		List<string> list = new List<string>();
		SelectedManholeCount = _manholeLayerNames.Count;
		SelectedManholeAnnotationCount = _manholeAnnotationLayerNames.Count;
		SelectedPipeCount = _pipeLayerNames.Count;
		SelectedPipeAnnotationCount = _pipeAnnotationLayerNames.Count;
		if (SelectedManholeCount > 0)
		{
			list.Add($"管井图层: {SelectedManholeCount} 个");
		}
		if (SelectedManholeAnnotationCount > 0)
		{
			list.Add($"管井标注图层: {SelectedManholeAnnotationCount} 个");
		}
		if (SelectedPipeCount > 0)
		{
			list.Add($"管道图层: {SelectedPipeCount} 个");
		}
		if (SelectedPipeAnnotationCount > 0)
		{
			list.Add($"管道标注图层: {SelectedPipeAnnotationCount} 个");
		}
		if (list.Count == 0)
		{
			SelectionSummary = "未选择";
		}
		else
		{
			SelectionSummary = string.Join(", ", list);
		}
		if (SelectedManholeCount > 0 || SelectedManholeAnnotationCount > 0 || SelectedPipeCount > 0)
		{
			_ = 1;
		}
		else
			_ = SelectedPipeAnnotationCount > 0;
		UpdateLayerInfoDisplay();
	}

	private void UpdateLayerInfoDisplay()
	{
		List<string> list = new List<string>();
		if (_manholeLayerNames.Count > 0)
		{
			list.Add("管井图层: " + string.Join(", ", _manholeLayerNames));
		}
		if (_manholeAnnotationLayerNames.Count > 0)
		{
			list.Add("管井标注图层: " + string.Join(", ", _manholeAnnotationLayerNames));
		}
		if (_pipeLayerNames.Count > 0)
		{
			list.Add("管道图层: " + string.Join(", ", _pipeLayerNames));
		}
		if (_pipeAnnotationLayerNames.Count > 0)
		{
			list.Add("管道标注图层: " + string.Join(", ", _pipeAnnotationLayerNames));
		}
		if (list.Count == 0)
		{
			DetectedLayers = "";
			HasLayerInfo = false;
		}
		else
		{
			DetectedLayers = string.Join("\n", list);
			HasLayerInfo = true;
		}
	}

	[RelayCommand]
	private void ImportParameterTable()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择管井参数表",
				Filter = "Excel文件 (*.xlsx;*.xls)|*.xlsx;*.xls|CSV文件 (*.csv)|*.csv|所有文件 (*.*)|*.*",
				Multiselect = false
			};
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			string fileName = openFileDialog.FileName;
			string fileName2 = Path.GetFileName(fileName);
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				_logger.Error("[LayerParameterTab] 导入管井参数表失败: RevitAdapter 为 null");
				MessageBox.Show("无法访问 Excel 数据服务（RevitAdapter 未初始化）", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			IExcelDataService excelDataService = revitAdapter.GetExcelDataService();
			if (excelDataService == null)
			{
				_logger.Error("[LayerParameterTab] 导入管井参数表失败: GetExcelDataService 返回 null");
				MessageBox.Show("无法访问 Excel 数据服务", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			Result<List<string>> sheetNames = excelDataService.GetSheetNames(fileName);
			if (sheetNames.IsFailure || sheetNames.Value == null || sheetNames.Value.Count == 0)
			{
				MessageBox.Show("无法获取工作表列表: " + sheetNames.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			List<string> value = sheetNames.Value;
			if (value.Count == 1)
			{
				ParameterTablePath = fileName;
				ParameterTableName = fileName2;
				ParameterSheetName = value[0];
				HasParameterTable = true;
				_logger.Info("已导入管井参数表: " + ParameterTableName + ", 工作表: " + ParameterSheetName);
				return;
			}
			string text = ShowSheetSelectionDialog(value, fileName2);
			if (text == null)
			{
				_logger.Info("用户取消了工作表选择");
				return;
			}
			ParameterTablePath = fileName;
			ParameterTableName = fileName2;
			ParameterSheetName = text;
			HasParameterTable = true;
			_logger.Info("已导入管井参数表: " + ParameterTableName + ", 工作表: " + ParameterSheetName);
		}
		catch (Exception ex)
		{
			_logger.Error("导入管井参数表失败", ex);
			MessageBox.Show("导入管井参数表失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private string? ShowSheetSelectionDialog(List<string> sheetNames, string fileName)
	{
		try
		{
			SheetSelectionDialog sheetSelectionDialog = new SheetSelectionDialog(sheetNames, fileName);
			Window window = Application.Current.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsLoaded && w.IsVisible);
			if (window != null)
			{
				sheetSelectionDialog.Owner = window;
			}
			if (sheetSelectionDialog.ShowDialog() == true)
			{
				return sheetSelectionDialog.SelectedSheet;
			}
			return null;
		}
		catch (Exception ex)
		{
			_logger.Error("显示工作表选择对话框失败", ex);
			return sheetNames.FirstOrDefault();
		}
	}

	[RelayCommand]
	private async Task GenerateNetwork()
	{
		try
		{
			if (_manholeLayerNames.Count <= 0 && _manholeAnnotationLayerNames.Count <= 0 && _pipeLayerNames.Count <= 0 && _pipeAnnotationLayerNames.Count <= 0)
			{
				MessageBox.Show("请先选择至少一类图层（管井、管井标注、管道或管道标注）", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			else if (!HasParameterTable || string.IsNullOrEmpty(ParameterTablePath))
			{
				MessageBox.Show("请先导入管井参数表", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			else
			{
				await GenerateNetworkInternal();
			}
		}
		catch (Exception ex)
		{
			_logger.Error("生成管网失败", ex);
			MessageBox.Show("生成管网失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private async Task GenerateNetworkInternal()
	{
		_ = 2;
		try
		{
			if (string.IsNullOrEmpty(ParameterTablePath))
			{
				MessageBox.Show("缺少管井参数表路径", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			string parameterTablePath = ParameterTablePath;
			IsGenerating = true;
			GenerationProgress = "准备建模...";
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				MessageBox.Show("无法获取 Revit 适配器", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsGenerating = false;
				return;
			}
			object document = revitAdapter.GetActiveDocument();
			if (document == null)
			{
				MessageBox.Show("无法获取当前 Revit 文档", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsGenerating = false;
				return;
			}
			GenerationProgress = "从 Revit 文档提取 CAD 数据...";
			_logger.Info("[生成管网] 开始从 Revit 文档提取 CAD 数据");
			PipeNetworkAnalysisResult pipeNetworkAnalysisResult = await ExtractCADDataFromRevitDocument(document, revitAdapter);
			if (pipeNetworkAnalysisResult == null || !pipeNetworkAnalysisResult.IsSuccess)
			{
				MessageBox.Show("从 Revit 文档提取 CAD 数据失败", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsGenerating = false;
				return;
			}
			_logger.Info("[生成管网] CAD 数据提取成功:\n" + pipeNetworkAnalysisResult.GetSummary());
			GenerationProgress = "读取管井参数表...";
			_logger.Info("[生成管网] 开始读取管井参数表: " + parameterTablePath);
			IPipeNetworkModelingService pipeNetworkModelingService = revitAdapter.GetPipeNetworkModelingService();
			if (pipeNetworkModelingService == null)
			{
				MessageBox.Show("无法获取管网建模服务", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsGenerating = false;
				return;
			}
			if (string.IsNullOrEmpty(ParameterSheetName))
			{
				MessageBox.Show("未选择工作表，请重新导入管井参数表", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsGenerating = false;
				return;
			}
			Result<Dictionary<string, ManholeParameterData>> result = pipeNetworkModelingService.ReadManholeParameterTable(parameterTablePath, ParameterSheetName);
			if (result.IsFailure || result.Value == null)
			{
				MessageBox.Show("读取管井参数表失败（工作表: " + ParameterSheetName + "）: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsGenerating = false;
				return;
			}
			Dictionary<string, ManholeParameterData> value = result.Value;
			_logger.Info($"[生成管网] 成功读取 {value.Count} 个管井参数");
			GenerationProgress = "合并 CAD 分析结果和参数表数据...";
			_logger.Info("[生成管网] 开始合并数据");
			PipeNetworkModelingData modelingData = pipeNetworkModelingService.MergeData(pipeNetworkAnalysisResult, value);
			_logger.Info("[生成管网] 数据合并完成: " + modelingData.GetSummary());
			GenerationProgress = "准备检查井族...";
			_logger.Info("[生成管网] 检查项目族和准备检查井族文件");
			string familyName = "AST_R_检查井";
			bool num = await CheckFamilyExistsInProjectAsync(familyName);
			_ = string.Empty;
			string text;
			if (num)
			{
				_logger.Info("[生成管网] 项目中已存在族 [" + familyName + "]，跳过下载，直接使用项目中的族");
				GenerationProgress = "使用项目中的检查井族";
				text = string.Empty;
			}
			else
			{
				if (_familyDownloadService == null)
				{
					MessageBox.Show("族下载服务未初始化，无法继续创建管网模型。", "服务错误", MessageBoxButton.OK, MessageBoxImage.Hand);
					IsGenerating = false;
					return;
				}
				GenerationProgress = "下载检查井族文件...";
				_logger.Info("[生成管网] 项目中不存在族，开始从在线族库下载");
				FamilyDownloadResult familyDownloadResult = await _familyDownloadService.DownloadFamilyFileAsync(familyName);
				if (!familyDownloadResult.IsSuccess || string.IsNullOrEmpty(familyDownloadResult.FilePath))
				{
					_logger.Warning("[生成管网] ⚠\ufe0f 检查井族下载失败: " + (familyDownloadResult.Error ?? "未知原因"));
					MessageBox.Show("检查井族下载失败: " + (familyDownloadResult.Error ?? "未知原因") + "\n\n无法继续创建管网模型。", "族下载失败", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					IsGenerating = false;
					return;
				}
				text = familyDownloadResult.FilePath;
				_logger.Info("[生成管网] 检查井族已下载: " + text);
			}
			PipeNetworkModelingOptions pipeNetworkModelingOptions = new PipeNetworkModelingOptions
			{
				ManholeFamilyFilePath = text
			};
			pipeNetworkModelingOptions.PipeSystemName = PipeSystemName;
			_logger.Info("[生成管网] 管道系统名称: " + pipeNetworkModelingOptions.PipeSystemName);
			GenerationProgress = "准备创建 Revit 模型...";
			_logger.Info("[生成管网] 准备通过 ExternalEvent 创建模型");
			object pipeNetworkModelingExternalEvent = revitAdapter.GetPipeNetworkModelingExternalEvent();
			if (pipeNetworkModelingExternalEvent == null)
			{
				_logger.Error("[生成管网] 无法获取管网建模 ExternalEvent");
				MessageBox.Show("无法获取建模服务", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsGenerating = false;
				return;
			}
			Type type = Type.GetType("RevitAi.Revit.Revit.PipeNetwork.PipeNetworkModelingRequest, RevitAi.Revit");
			if (type == null)
			{
				_logger.Error("[生成管网] 未找到 PipeNetworkModelingRequest 类型");
				MessageBox.Show("未找到建模请求类型", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsGenerating = false;
				return;
			}
			dynamic val = Activator.CreateInstance(type);
			if (val == null)
			{
				_logger.Error("[生成管网] 无法创建建模请求");
				MessageBox.Show("无法创建建模请求", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsGenerating = false;
				return;
			}
			val.ModelingData = modelingData;
			val.Document = document;
			val.Options = pipeNetworkModelingOptions;
			Action<PipeNetworkModelingResult> action = delegate(PipeNetworkModelingResult pipeNetworkModelingResult)
			{
				((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
				{
					IsGenerating = false;
					if (pipeNetworkModelingResult.Success)
					{
						GenerationProgress = "建模完成";
						_logger.Info($"[生成管网] 建模成功: {pipeNetworkModelingResult.CreatedManholesCount} 个管井, {pipeNetworkModelingResult.CreatedPipesCount} 个管道");
					}
					else
					{
						GenerationProgress = "建模失败";
						_logger.Error("[生成管网] 建模失败: " + pipeNetworkModelingResult.ErrorMessage);
					}
					ShowModelingResult(pipeNetworkModelingResult);
				});
			};
			type.GetProperty("OnCompleted")?.SetValue(val, action);
			type.GetProperty("CurrentRequest")?.SetValue(null, val);
			GenerationProgress = "正在创建模型...";
			(pipeNetworkModelingExternalEvent?.GetType().GetMethod("Raise"))?.Invoke(pipeNetworkModelingExternalEvent, null);
			_logger.Info("[生成管网] 已触发建模 ExternalEvent");
		}
		catch (Exception ex)
		{
			IsGenerating = false;
			_logger.Error("生成管网失败", ex);
			MessageBox.Show("生成管网失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private Task<PipeNetworkAnalysisResult?> ExtractCADDataFromRevitDocument(object document, IRevitAdapter revitAdapter)
	{
		try
		{
			DateTime now = DateTime.Now;
			ICADGeometryService cADGeometryService = revitAdapter.CADGeometryService;
			if (cADGeometryService == null)
			{
				_logger.Error("[生成管网] 无法获取 CAD 几何服务");
				return Task.FromResult<PipeNetworkAnalysisResult>(null);
			}
			List<object> list = cADGeometryService.GetCADImportInstances(document).ToList();
			if (list.Count == 0)
			{
				_logger.Warning("[生成管网] 未找到 CAD 导入实例");
				return Task.FromResult<PipeNetworkAnalysisResult>(null);
			}
			_logger.Info($"[生成管网] 找到 {list.Count} 个 CAD 导入实例");
			List<object> list2 = new List<object>();
			foreach (object item in list)
			{
				IEnumerable<object> geometryObjects = cADGeometryService.GetGeometryObjects(item);
				list2.AddRange(geometryObjects);
			}
			_logger.Info($"[生成管网] 提取到 {list2.Count} 个几何对象");
			PipeNetworkAnalysisResult pipeNetworkAnalysisResult = new PipeNetworkAnalysisResult
			{
				AnalysisTime = now,
				Options = new CADAnalysisOptions
				{
					ManholeLayerNames = _manholeLayerNames.ToList(),
					PipeLayerNames = _pipeLayerNames.ToList(),
					ManholeAnnotationLayerNames = _manholeAnnotationLayerNames.ToList(),
					PipeAnnotationLayerNames = _pipeAnnotationLayerNames.ToList()
				}
			};
			bool flag = false;
			CADFileData cADFileData = null;
			try
			{
				ICADFileService cADFileService = revitAdapter.CADFileService;
				if (cADFileService != null && list.Count > 0)
				{
					_logger.Info("[生成管网] 尝试使用 ACadSharp 解析 CAD 文件...");
					object importInstance = list.First();
					cADFileData = cADFileService.ParseImportInstance(importInstance, document);
					if (cADFileData != null)
					{
						_logger.Info($"[生成管网] ✅ ACadSharp 解析成功: {cADFileData.Blocks.Count} 个图块, {cADFileData.Lines.Count} 条线条, {cADFileData.Texts.Count} 个文字");
						flag = true;
					}
					else
					{
						_logger.Warning("[生成管网] ⚠\ufe0f ACadSharp 解析失败，回退到几何提取方法");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Warning("[生成管网] ⚠\ufe0f ACadSharp 解析出错: " + ex.Message + "，回退到几何提取方法");
			}
			_logger.Info("[生成管网] 步骤1: 提取管井数据...");
			List<ManholeData> list3;
			if (flag && cADFileData != null)
			{
				list3 = ExtractManholesFromACadSharpData(cADFileData, _manholeLayerNames);
			}
			else
			{
				_logger.Info("[生成管网] 使用几何对象提取方法...");
				list3 = ExtractManholesFromGeometry(list2, _manholeLayerNames, cADGeometryService, document);
			}
			pipeNetworkAnalysisResult.Manholes.AddRange(list3);
			_logger.Info($"[生成管网] 提取到 {list3.Count} 个管井");
			_logger.Info("[生成管网] 步骤2: 提取管道数据...");
			List<PipeData> list4;
			if (flag && cADFileData != null)
			{
				list4 = ExtractPipesFromACadSharpData(cADFileData, _pipeLayerNames);
			}
			else
			{
				_logger.Info("[生成管网] 使用几何对象提取方法...");
				list4 = ExtractPipesFromGeometry(list2, _pipeLayerNames, cADGeometryService, document);
			}
			pipeNetworkAnalysisResult.Pipes.AddRange(list4);
			_logger.Info($"[生成管网] 提取到 {list4.Count} 个管道");
			_logger.Info("[生成管网] 步骤3: 处理管井标注...");
			List<AnnotationLineGroup> list5 = ProcessAnnotationLayers(list2, _manholeAnnotationLayerNames, cADGeometryService, pipeNetworkAnalysisResult.ManholeAnnotationLineGroups, document, list, cADFileData);
			pipeNetworkAnalysisResult.ManholeAnnotationLineGroups.AddRange(list5);
			AssociateAnnotationsToManholes(pipeNetworkAnalysisResult.ManholeAnnotationLineGroups, pipeNetworkAnalysisResult.Manholes, pipeNetworkAnalysisResult.Options);
			_logger.Info($"[生成管网] 处理了 {list5.Count} 个管井标注线条组");
			_logger.Info("[生成管网] 步骤4: 处理管道标注...");
			List<AnnotationLineGroup> list6 = ProcessAnnotationLayers(list2, _pipeAnnotationLayerNames, cADGeometryService, pipeNetworkAnalysisResult.PipeAnnotationLineGroups, document, list, cADFileData);
			pipeNetworkAnalysisResult.PipeAnnotationLineGroups.AddRange(list6);
			AssociateAnnotationsToPipes(pipeNetworkAnalysisResult.PipeAnnotationLineGroups, pipeNetworkAnalysisResult.Pipes, pipeNetworkAnalysisResult.Options);
			_logger.Info($"[生成管网] 处理了 {list6.Count} 个管道标注线条组");
			_logger.Info("[生成管网] 步骤5: 连接管道与管井...");
			ConnectPipesToManholes(pipeNetworkAnalysisResult.Pipes, pipeNetworkAnalysisResult.Manholes, pipeNetworkAnalysisResult.Options);
			DateTime now2 = DateTime.Now;
			pipeNetworkAnalysisResult.Statistics = new AnalysisStatistics
			{
				AnalysisStartTime = now,
				AnalysisEndTime = now2,
				AnalysisDurationMs = (long)(now2 - now).TotalMilliseconds,
				IsSuccess = true,
				TotalManholes = pipeNetworkAnalysisResult.Manholes.Count,
				AnnotatedManholes = pipeNetworkAnalysisResult.Manholes.Count((ManholeData m) => m.HasAnnotation),
				TotalPipes = pipeNetworkAnalysisResult.Pipes.Count,
				AnnotatedPipes = pipeNetworkAnalysisResult.Pipes.Count((PipeData p) => p.HasAnnotation),
				ConnectedPipes = pipeNetworkAnalysisResult.Pipes.Count((PipeData p) => p.IsConnectedToManholes),
				TotalPipeLength = pipeNetworkAnalysisResult.Pipes.Sum((PipeData p) => p.Length)
			};
			_logger.Info("[生成管网] 分析完成:");
			_logger.Info($"  - 管井: {pipeNetworkAnalysisResult.Statistics.TotalManholes} (标注: {pipeNetworkAnalysisResult.Statistics.AnnotatedManholes})");
			_logger.Info($"  - 管道: {pipeNetworkAnalysisResult.Statistics.TotalPipes} (标注: {pipeNetworkAnalysisResult.Statistics.AnnotatedPipes})");
			_logger.Info($"  - 连接: {pipeNetworkAnalysisResult.Statistics.ConnectedPipes}");
			_logger.Info($"  - 耗时: {pipeNetworkAnalysisResult.Statistics.AnalysisDurationMs} ms");
			return Task.FromResult(pipeNetworkAnalysisResult);
		}
		catch (Exception ex2)
		{
			_logger.Error("[生成管网] 从 Revit 文档提取 CAD 数据失败", ex2);
			return Task.FromResult<PipeNetworkAnalysisResult>(null);
		}
	}

	private List<ManholeData> ExtractManholesFromGeometry(List<object> allGeometryObjects, ICollection<string> manholeLayerNames, ICADGeometryService cadGeometryService, object document)
	{
		List<ManholeData> list = new List<ManholeData>();
		foreach (string layerName in manholeLayerNames)
		{
			foreach (object item4 in allGeometryObjects.Where((object geom) => cadGeometryService.GetGeometryObjectLayer(geom, document) == layerName).ToList())
			{
				if (cadGeometryService.GetGeometryObjectType(item4) == "Block")
				{
					var (text, obj) = cadGeometryService.GetBlockInfo(item4);
					if (obj != null)
					{
						(double X, double Y, double Z) tuple2 = ExtractPositionFromObject(obj);
						double item = tuple2.X;
						double item2 = tuple2.Y;
						ManholeData item3 = new ManholeData
						{
							BlockName = (text ?? "Unknown"),
							X = item,
							Y = item2,
							Z = 0.0,
							LayerName = layerName
						};
						list.Add(item3);
					}
				}
			}
		}
		return list;
	}

	private List<PipeData> ExtractPipesFromGeometry(List<object> allGeometryObjects, ICollection<string> pipeLayerNames, ICADGeometryService cadGeometryService, object document)
	{
		List<PipeData> list = new List<PipeData>();
		foreach (string layerName in pipeLayerNames)
		{
			foreach (object item6 in allGeometryObjects.Where((object geom) => cadGeometryService.GetGeometryObjectLayer(geom, document) == layerName).ToList())
			{
				string geometryObjectType = cadGeometryService.GetGeometryObjectType(item6);
				if (geometryObjectType == "Curve" || geometryObjectType == "Line")
				{
					var (obj, obj2) = cadGeometryService.GetCurveEndpoints(item6);
					if (obj != null && obj2 != null)
					{
						(double X, double Y, double Z) tuple2 = ExtractPositionFromObject(obj);
						double item = tuple2.X;
						double item2 = tuple2.Y;
						(double X, double Y, double Z) tuple3 = ExtractPositionFromObject(obj2);
						double item3 = tuple3.X;
						double item4 = tuple3.Y;
						double num = item3 - item;
						double num2 = item4 - item2;
						double length = Math.Sqrt(num * num + num2 * num2);
						PipeData item5 = new PipeData
						{
							StartX = item,
							StartY = item2,
							StartZ = 0.0,
							EndX = item3,
							EndY = item4,
							EndZ = 0.0,
							Length = length,
							LayerName = layerName,
							LineType = (geometryObjectType ?? "Curve")
						};
						list.Add(item5);
					}
				}
			}
		}
		_logger.Info($"[提取管道] 从 {pipeLayerNames.Count} 个图层中提取了 {list.Count} 个管道");
		return list;
	}

	private List<AnnotationLineGroup> ProcessAnnotationLayers(List<object> allGeometryObjects, ICollection<string> annotationLayerNames, ICADGeometryService cadGeometryService, List<AnnotationLineGroup> targetList, object document, IEnumerable<object> importInstanceList, CADFileData? cadData = null)
	{
		List<AnnotationLineGroup> list = new List<AnnotationLineGroup>();
		List<CADTextInfo> list2 = new List<CADTextInfo>();
		if (cadData != null && cadData.Texts.Count > 0)
		{
			list2.AddRange(cadData.Texts);
			_logger.Info($"[ProcessAnnotationLayers] 使用传入的 ACadSharp 数据: {list2.Count} 个文字");
		}
		else
		{
			try
			{
				IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
				if (revitAdapter?.CADFileService != null)
				{
					foreach (object importInstance in importInstanceList)
					{
						try
						{
							CADFileData cADFileData = revitAdapter.CADFileService.ParseImportInstance(importInstance, document);
							if (cADFileData?.Texts != null)
							{
								list2.AddRange(cADFileData.Texts);
							}
						}
						catch (Exception ex)
						{
							_logger.Error("[ProcessAnnotationLayers] ACadSharp 解析 ImportInstance 失败: " + ex.Message);
						}
					}
				}
				else
				{
					_logger.Warning("[ProcessAnnotationLayers] CADFileService 不可用，无法使用 ACadSharp 提取文字");
				}
			}
			catch (Exception ex2)
			{
				_logger.Error("[ProcessAnnotationLayers] 获取 CADFileService 失败: " + ex2.Message);
			}
			_logger.Info($"[ProcessAnnotationLayers] ACadSharp 提取到 {list2.Count} 个文字");
		}
		foreach (string layerName in annotationLayerNames)
		{
			List<CADLineInfo> list3 = new List<CADLineInfo>();
			List<CADTextInfo> list4 = new List<CADTextInfo>();
			if (cadData != null && cadData.Lines.Count > 0)
			{
				List<CADLineInfo> linesByLayer = cadData.GetLinesByLayer(layerName);
				list3.AddRange(linesByLayer);
				_logger.Debug($"[ProcessAnnotationLayers] 从 ACadSharp 获取图层 '{layerName}' 的 {list3.Count} 条线条");
			}
			else
			{
				_logger.Info("[ProcessAnnotationLayers] ACadSharp 数据不可用，从几何对象提取线条: " + layerName);
				foreach (object item in allGeometryObjects.Where((object geom) => cadGeometryService.GetGeometryObjectLayer(geom, document) == layerName).ToList())
				{
					string geometryObjectType = cadGeometryService.GetGeometryObjectType(item);
					if (geometryObjectType == "Curve" || geometryObjectType == "Line")
					{
						var (obj, obj2) = cadGeometryService.GetCurveEndpoints(item);
						if (obj != null && obj2 != null)
						{
							var (startX, startY, startZ) = ExtractPositionFromObject(obj);
							var (endX, endY, endZ) = ExtractPositionFromObject(obj2);
							list3.Add(new CADLineInfo
							{
								StartX = startX,
								StartY = startY,
								StartZ = startZ,
								EndX = endX,
								EndY = endY,
								EndZ = endZ,
								LayerName = layerName,
								GeometryObject = item
							});
						}
					}
				}
			}
			List<CADTextInfo> collection = list2.Where((CADTextInfo t) => t.LayerName == layerName).ToList();
			list4.AddRange(collection);
			_logger.Info($"[ProcessAnnotationLayers] 图层 '{layerName}': {list3.Count} 条线条, {list4.Count} 个文字");
			List<AnnotationLineGroup> list5 = ConnectLineGroups(list3, 10.0);
			HashSet<string> associatedTextIds = new HashSet<string>();
			foreach (AnnotationLineGroup item2 in list5)
			{
				foreach (CADTextInfo item3 in list4)
				{
					if (associatedTextIds.Contains(item3.Id))
					{
						continue;
					}
					CADLineInfo cADLineInfo = null;
					double num = double.MaxValue;
					foreach (CADLineInfo line in item2.Lines)
					{
						double num2 = item3.CalculatePerpendicularDistanceToLine(line.StartX, line.StartY, line.StartZ, line.EndX, line.EndY, line.EndZ);
						if (!(num2 > 500.0))
						{
							double num3 = item3.Rotation.GetValueOrDefault() * 180.0 / Math.PI;
							double num4 = Math.Atan2(line.EndY - line.StartY, line.EndX - line.StartX) * 180.0 / Math.PI;
							double num5;
							for (num5 = Math.Abs(num3 - num4); num5 > 180.0; num5 -= 180.0)
							{
							}
							if (num5 < 0.0)
							{
								num5 = 0.0 - num5;
							}
							if (!(num5 > 30.0) && num2 < num)
							{
								num = num2;
								cADLineInfo = line;
							}
						}
					}
					if (cADLineInfo != null)
					{
						item2.AssociatedTexts.Add(item3);
						associatedTextIds.Add(item3.Id);
					}
				}
			}
			list.AddRange(list5);
			List<CADTextInfo> list6 = list4.Where((CADTextInfo t) => !associatedTextIds.Contains(t.Id)).ToList();
			if (list6.Count <= 0)
			{
				continue;
			}
			_logger.Info($"[ProcessAnnotationLayers] 图层 '{layerName}': 发现 {list6.Count} 个单独标注（无关联线条）");
			foreach (CADTextInfo item4 in list6)
			{
				AnnotationLineGroup annotationLineGroup = new AnnotationLineGroup();
				annotationLineGroup.AssociatedTexts.Add(item4);
				list.Add(annotationLineGroup);
			}
		}
		return list;
	}

	private List<AnnotationLineGroup> ConnectLineGroups(List<CADLineInfo> lines, double tolerance)
	{
		List<AnnotationLineGroup> list = new List<AnnotationLineGroup>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (CADLineInfo line in lines)
		{
			if (hashSet.Contains(line.Id))
			{
				continue;
			}
			AnnotationLineGroup annotationLineGroup = new AnnotationLineGroup();
			List<CADLineInfo> list2 = new List<CADLineInfo> { line };
			hashSet.Add(line.Id);
			Queue<CADLineInfo> queue = new Queue<CADLineInfo>();
			queue.Enqueue(line);
			while (queue.Count > 0)
			{
				CADLineInfo cADLineInfo = queue.Dequeue();
				foreach (CADLineInfo line2 in lines)
				{
					if (!hashSet.Contains(line2.Id) && cADLineInfo.IsConnectedTo(line2, tolerance))
					{
						hashSet.Add(line2.Id);
						list2.Add(line2);
						queue.Enqueue(line2);
					}
				}
			}
			if (list2.Count > 0)
			{
				list2 = SortLinesByConnection(list2);
				annotationLineGroup.Lines.AddRange(list2);
				annotationLineGroup.TotalLength = list2.Sum((CADLineInfo l) => l.Length);
				CADLineInfo cADLineInfo2 = list2.OrderByDescending((CADLineInfo l) => l.Length).First();
				annotationLineGroup.PrimaryDirectionAngle = cADLineInfo2.DirectionAngle;
				annotationLineGroup.StartPoint = list2[0].GetStartPoint();
				annotationLineGroup.EndPoint = list2[list2.Count - 1].GetEndPoint();
				list.Add(annotationLineGroup);
			}
		}
		return list;
	}

	private List<CADLineInfo> SortLinesByConnection(List<CADLineInfo> lines)
	{
		if (lines.Count == 0)
		{
			return lines;
		}
		List<CADLineInfo> list = new List<CADLineInfo>();
		List<CADLineInfo> list2 = new List<CADLineInfo>(lines);
		double num = 10.0;
		list.Add(list2[0]);
		list2.RemoveAt(0);
		while (list2.Count > 0)
		{
			(double, double, double) endPoint = list[list.Count - 1].GetEndPoint();
			bool flag = false;
			for (int i = 0; i < list2.Count; i++)
			{
				CADLineInfo cADLineInfo = list2[i];
				(double, double, double) startPoint = cADLineInfo.GetStartPoint();
				(double, double, double) endPoint2 = cADLineInfo.GetEndPoint();
				double num2 = Math.Sqrt(Math.Pow(endPoint.Item1 - startPoint.Item1, 2.0) + Math.Pow(endPoint.Item2 - startPoint.Item2, 2.0));
				double num3 = Math.Sqrt(Math.Pow(endPoint.Item1 - endPoint2.Item1, 2.0) + Math.Pow(endPoint.Item2 - endPoint2.Item2, 2.0));
				if (num2 <= num || num3 <= num)
				{
					if (num3 <= num && num2 > num)
					{
						SwapLineEndpoints(cADLineInfo);
					}
					list.Add(cADLineInfo);
					list2.RemoveAt(i);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(list2[0]);
				list2.RemoveAt(0);
			}
		}
		return list;
	}

	private void SwapLineEndpoints(CADLineInfo line)
	{
		double startX = line.StartX;
		double startY = line.StartY;
		double startZ = line.StartZ;
		line.StartX = line.EndX;
		line.StartY = line.EndY;
		line.StartZ = line.EndZ;
		line.EndX = startX;
		line.EndY = startY;
		line.EndZ = startZ;
	}

	private void AssociateAnnotationsToManholes(List<AnnotationLineGroup> lineGroups, List<ManholeData> manholes, CADAnalysisOptions options)
	{
		_logger.Info("[AssociateAnnotationsToManholes] 开始处理管井标注关联...");
		List<CADTextInfo> list = new List<CADTextInfo>();
		List<AnnotationLineGroup> list2 = new List<AnnotationLineGroup>();
		foreach (AnnotationLineGroup lineGroup in lineGroups)
		{
			if (lineGroup.HasAssociatedTexts)
			{
				list.AddRange(lineGroup.AssociatedTexts);
				if (lineGroup.Lines.Count > 0)
				{
					list2.Add(lineGroup);
				}
			}
		}
		_logger.Info($"[AssociateAnnotationsToManholes] 共有 {list.Count} 个文字标注，{list2.Count} 个有文字的线条组");
		foreach (CADTextInfo item2 in list)
		{
			ManholeData manholeData = null;
			double? num = null;
			string text = null;
			string lineGroupId = null;
			AnnotationLineGroup annotationLineGroup = FindNearestParallelLine(item2, list2, options);
			if (annotationLineGroup != null)
			{
				manholeData = FindManholeNearLineEndpoints(annotationLineGroup, manholes, options);
				lineGroupId = annotationLineGroup.Id;
				text = "LineGroup";
				if (manholeData != null)
				{
					num = item2.DistanceFromCenterToPoint(manholeData.X, manholeData.Y);
				}
			}
			else
			{
				manholeData = FindNearestManhole(item2, manholes);
				text = "TextOnly";
				if (manholeData != null)
				{
					num = item2.DistanceFromCenterToPoint(manholeData.X, manholeData.Y);
				}
			}
			if (manholeData != null)
			{
				ManholeAnnotation item = new ManholeAnnotation
				{
					Content = item2.Content,
					Type = (text ?? "TextOnly"),
					LineGroupId = lineGroupId,
					TextId = item2.Id,
					Distance = num.GetValueOrDefault()
				};
				manholeData.Annotations.Add(item);
				if (manholeData.Annotations.Count == 1)
				{
					manholeData.Annotation = item2.Content;
					manholeData.AnnotationType = text;
				}
			}
		}
		_logger.Info("[AssociateAnnotationsToManholes] 标注关联完成");
	}

	private AnnotationLineGroup? FindNearestParallelLine(CADTextInfo text, List<AnnotationLineGroup> lineGroups, CADAnalysisOptions options)
	{
		AnnotationLineGroup result = null;
		double num = double.MaxValue;
		foreach (AnnotationLineGroup lineGroup in lineGroups)
		{
			double x = (lineGroup.StartPoint.X + lineGroup.EndPoint.X) / 2.0;
			double y = (lineGroup.StartPoint.Y + lineGroup.EndPoint.Y) / 2.0;
			double num2 = text.DistanceFromCenterToPoint(x, y);
			if (!(num2 > 500.0))
			{
				double num3 = text.Rotation.GetValueOrDefault() * 180.0 / Math.PI;
				double num4 = lineGroup.PrimaryDirectionAngle * 180.0 / Math.PI;
				double num5;
				for (num5 = Math.Abs(num3 - num4); num5 > 180.0; num5 -= 180.0)
				{
				}
				if (num5 < 0.0)
				{
					num5 = 0.0 - num5;
				}
				if (!(num5 > 15.0) && num2 < num)
				{
					num = num2;
					result = lineGroup;
				}
			}
		}
		return result;
	}

	private ManholeData? FindManholeNearLineEndpoints(AnnotationLineGroup lineGroup, List<ManholeData> manholes, CADAnalysisOptions options)
	{
		(double X, double Y, double Z) startPoint = lineGroup.GetFirstEndpoint();
		(double X, double Y, double Z) endPoint = lineGroup.GetLastEndpoint();
		List<ManholeData> list = manholes.Where((ManholeData m) => Math.Sqrt(Math.Pow(m.X - startPoint.X, 2.0) + Math.Pow(m.Y - startPoint.Y, 2.0)) <= options.LineEndToManholeMaxDistanceMM).ToList();
		if (list.Count > 0)
		{
			return list.OrderBy((ManholeData m) => Math.Sqrt(Math.Pow(m.X - startPoint.X, 2.0) + Math.Pow(m.Y - startPoint.Y, 2.0))).FirstOrDefault();
		}
		List<ManholeData> list2 = manholes.Where((ManholeData m) => Math.Sqrt(Math.Pow(m.X - endPoint.X, 2.0) + Math.Pow(m.Y - endPoint.Y, 2.0)) <= options.LineEndToManholeMaxDistanceMM).ToList();
		if (list2.Count > 0)
		{
			return list2.OrderBy((ManholeData m) => Math.Sqrt(Math.Pow(m.X - endPoint.X, 2.0) + Math.Pow(m.Y - endPoint.Y, 2.0))).FirstOrDefault();
		}
		return null;
	}

	private ManholeData? FindNearestManhole(CADTextInfo text, List<ManholeData> manholes)
	{
		return manholes.OrderBy((ManholeData m) => text.DistanceFromCenterToPoint(m.X, m.Y)).FirstOrDefault();
	}

	private void AssociateAnnotationsToPipes(List<AnnotationLineGroup> lineGroups, List<PipeData> pipes, CADAnalysisOptions options)
	{
		_logger.Info("[AssociateAnnotationsToPipes] 开始处理管道标注关联...");
		List<CADTextInfo> list = new List<CADTextInfo>();
		List<AnnotationLineGroup> list2 = new List<AnnotationLineGroup>();
		foreach (AnnotationLineGroup lineGroup in lineGroups)
		{
			if (lineGroup.HasAssociatedTexts)
			{
				list.AddRange(lineGroup.AssociatedTexts);
				if (lineGroup.Lines.Count > 0)
				{
					list2.Add(lineGroup);
				}
			}
		}
		_logger.Info($"[AssociateAnnotationsToPipes] 共有 {list.Count} 个文字标注，{list2.Count} 个有文字的线条组");
		foreach (CADTextInfo item2 in list)
		{
			PipeData pipeData = null;
			double? num = null;
			string text = null;
			string lineGroupId = null;
			AnnotationLineGroup annotationLineGroup = FindNearestParallelLine(item2, list2, options);
			if (annotationLineGroup != null)
			{
				pipeData = FindParallelPipeNearLine(annotationLineGroup, pipes, options);
				lineGroupId = annotationLineGroup.Id;
				text = "LineGroup";
				if (pipeData != null)
				{
					double x = (pipeData.StartX + pipeData.EndX) / 2.0;
					double y = (pipeData.StartY + pipeData.EndY) / 2.0;
					num = item2.DistanceFromCenterToPoint(x, y);
				}
			}
			else
			{
				pipeData = FindNearestPipe(item2, pipes);
				text = "TextOnly";
				if (pipeData != null)
				{
					double x2 = (pipeData.StartX + pipeData.EndX) / 2.0;
					double y2 = (pipeData.StartY + pipeData.EndY) / 2.0;
					num = item2.DistanceFromCenterToPoint(x2, y2);
				}
			}
			if (pipeData != null)
			{
				PipeAnnotation item = new PipeAnnotation
				{
					Content = item2.Content,
					Type = (text ?? "TextOnly"),
					LineGroupId = lineGroupId,
					TextId = item2.Id,
					Distance = num.GetValueOrDefault()
				};
				pipeData.Annotations.Add(item);
				if (pipeData.Annotations.Count == 1)
				{
					pipeData.Annotation = item2.Content;
					pipeData.AnnotationType = text;
				}
			}
		}
		_logger.Info("[AssociateAnnotationsToPipes] 开始提取管道直径...");
		int num2 = 0;
		foreach (PipeData pipe in pipes)
		{
			if (pipe.HasAnnotation)
			{
				pipe.ExtractDiameterFromAnnotations();
				if (pipe.HasDiameter)
				{
					num2++;
				}
			}
		}
		_logger.Info($"[AssociateAnnotationsToPipes] 直径提取完成: {num2}/{pipes.Count} 条管道有直径信息");
	}

	private PipeData? FindParallelPipeNearLine(AnnotationLineGroup lineGroup, List<PipeData> pipes, CADAnalysisOptions options)
	{
		List<PipeData> list = pipes.Where(delegate(PipeData p)
		{
			double x = p.EndX - p.StartX;
			double num;
			for (num = Math.Abs(Math.Atan2(p.EndY - p.StartY, x) - lineGroup.PrimaryDirectionAngle); num > Math.PI; num -= Math.PI)
			{
			}
			bool num2 = num <= options.DirectionAngleToleranceDegrees * Math.PI / 180.0 || num >= Math.PI - options.DirectionAngleToleranceDegrees * Math.PI / 180.0;
			double num3 = (p.StartX + p.EndX) / 2.0;
			double num4 = (p.StartY + p.EndY) / 2.0;
			double num5 = (lineGroup.StartPoint.X + lineGroup.EndPoint.X) / 2.0;
			double num6 = (lineGroup.StartPoint.Y + lineGroup.EndPoint.Y) / 2.0;
			double num7 = Math.Sqrt(Math.Pow(num3 - num5, 2.0) + Math.Pow(num4 - num6, 2.0));
			return num2 && num7 <= options.TextToLineMaxDistanceMM;
		}).ToList();
		if (list.Count > 0)
		{
			CADTextInfo text = lineGroup.AssociatedTexts.FirstOrDefault();
			if (text != null)
			{
				return list.OrderBy(delegate(PipeData p)
				{
					double x = (p.StartX + p.EndX) / 2.0;
					double y = (p.StartY + p.EndY) / 2.0;
					return text.DistanceFromCenterToPoint(x, y);
				}).FirstOrDefault();
			}
		}
		return null;
	}

	private PipeData? FindNearestPipe(CADTextInfo text, List<PipeData> pipes)
	{
		return pipes.OrderBy(delegate(PipeData p)
		{
			double x = (p.StartX + p.EndX) / 2.0;
			double y = (p.StartY + p.EndY) / 2.0;
			return text.DistanceFromCenterToPoint(x, y);
		}).FirstOrDefault();
	}

	private void ConnectPipesToManholes(List<PipeData> pipes, List<ManholeData> manholes, CADAnalysisOptions options)
	{
		foreach (PipeData pipe in pipes)
		{
			ManholeData manholeData = (from m in manholes
				where Math.Sqrt(Math.Pow(pipe.StartX - m.X, 2.0) + Math.Pow(pipe.StartY - m.Y, 2.0)) <= options.PipeToManholeConnectionToleranceMM
				orderby Math.Sqrt(Math.Pow(pipe.StartX - m.X, 2.0) + Math.Pow(pipe.StartY - m.Y, 2.0))
				select m).FirstOrDefault();
			if (manholeData != null)
			{
				pipe.StartManholeId = manholeData.Id;
				manholeData.ConnectedPipeIds.Add(pipe.Id);
			}
			ManholeData manholeData2 = (from m in manholes
				where Math.Sqrt(Math.Pow(pipe.EndX - m.X, 2.0) + Math.Pow(pipe.EndY - m.Y, 2.0)) <= options.PipeToManholeConnectionToleranceMM
				orderby Math.Sqrt(Math.Pow(pipe.EndX - m.X, 2.0) + Math.Pow(pipe.EndY - m.Y, 2.0))
				select m).FirstOrDefault();
			if (manholeData2 != null)
			{
				pipe.EndManholeId = manholeData2.Id;
				manholeData2.ConnectedPipeIds.Add(pipe.Id);
			}
		}
	}

	private List<ManholeData> ExtractManholesFromACadSharpData(CADFileData cadData, ICollection<string> manholeLayerNames)
	{
		List<ManholeData> list = new List<ManholeData>();
		foreach (string manholeLayerName in manholeLayerNames)
		{
			foreach (CADBlockInfo item2 in cadData.GetBlocksByLayer(manholeLayerName))
			{
				ManholeData item = new ManholeData
				{
					BlockName = (item2.Name ?? "Unknown"),
					X = item2.X,
					Y = item2.Y,
					Z = 0.0,
					LayerName = item2.LayerName
				};
				list.Add(item);
			}
		}
		return list;
	}

	private List<PipeData> ExtractPipesFromACadSharpData(CADFileData cadData, ICollection<string> pipeLayerNames)
	{
		List<PipeData> list = new List<PipeData>();
		foreach (string pipeLayerName in pipeLayerNames)
		{
			foreach (CADLineInfo item2 in cadData.GetLinesByLayer(pipeLayerName))
			{
				double num = item2.EndX - item2.StartX;
				double num2 = item2.EndY - item2.StartY;
				double length = Math.Sqrt(num * num + num2 * num2);
				PipeData item = new PipeData
				{
					StartX = item2.StartX,
					StartY = item2.StartY,
					StartZ = 0.0,
					EndX = item2.EndX,
					EndY = item2.EndY,
					EndZ = 0.0,
					Length = length,
					LayerName = item2.LayerName,
					LineType = item2.LineType
				};
				list.Add(item);
			}
		}
		_logger.Info($"[提取管道] ACadSharp 从 {pipeLayerNames.Count} 个图层中提取了 {list.Count} 个管道");
		return list;
	}

	private (double X, double Y, double Z) ExtractPositionFromObject(object position)
	{
		try
		{
			Type type = position.GetType();
			PropertyInfo propertyInfo = type.GetProperty("X") ?? type.GetProperty("x");
			PropertyInfo propertyInfo2 = type.GetProperty("Y") ?? type.GetProperty("y");
			PropertyInfo propertyInfo3 = type.GetProperty("Z") ?? type.GetProperty("z");
			double item = ((propertyInfo != null) ? ((double)(Convert.ChangeType(propertyInfo.GetValue(position), typeof(double)) ?? ((object)0.0))) : 0.0);
			double item2 = ((propertyInfo2 != null) ? ((double)(Convert.ChangeType(propertyInfo2.GetValue(position), typeof(double)) ?? ((object)0.0))) : 0.0);
			double item3 = ((propertyInfo3 != null) ? ((double)(Convert.ChangeType(propertyInfo3.GetValue(position), typeof(double)) ?? ((object)0.0))) : 0.0);
			return (X: item, Y: item2, Z: item3);
		}
		catch (Exception ex)
		{
			_logger.Warning("[提取位置] 无法从对象提取坐标: " + ex.Message);
			return (X: 0.0, Y: 0.0, Z: 0.0);
		}
	}

	private void ShowModelingResult(PipeNetworkModelingResult result)
	{
		if (result.Success)
		{
			string message = $"建模成功！\n\n创建管井: {result.CreatedManholesCount} 个\n创建管道: {result.CreatedPipesCount} 个\n耗时: {result.ElapsedMilliseconds} ms\n";
			if (result.Warnings.Count > 0)
			{
				message += $"\n警告 ({result.Warnings.Count} 条):\n";
				result.Warnings.Take(5).ToList().ForEach(delegate(string w)
				{
					message = message + "  - " + w + "\n";
				});
				if (result.Warnings.Count > 5)
				{
					message += $"  ... 还有 {result.Warnings.Count - 5} 条警告\n";
				}
			}
			if (result.MissingDataReport.Count > 0)
			{
				message += $"\n缺失数据 ({result.MissingDataReport.Count} 条):\n";
				result.MissingDataReport.Take(5).ToList().ForEach(delegate(string m)
				{
					message = message + "  - " + m + "\n";
				});
				if (result.MissingDataReport.Count > 5)
				{
					message += $"  ... 还有 {result.MissingDataReport.Count - 5} 条\n";
				}
			}
			GenerationResult = message;
			MessageBox.Show(message, "建模成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			_logger.Info("[生成管网] 建模成功: " + message);
			return;
		}
		string message2 = "建模失败: " + result.ErrorMessage + "\n\n";
		if (result.Errors.Count > 0)
		{
			message2 += "错误详情:\n";
			result.Errors.Take(5).ToList().ForEach(delegate(string e)
			{
				message2 = message2 + "  - " + e + "\n";
			});
		}
		GenerationResult = message2;
		MessageBox.Show(message2, "建模失败", MessageBoxButton.OK, MessageBoxImage.Hand);
		_logger.Error("[生成管网] 建模失败: " + message2);
	}

	[RelayCommand]
	private async Task AnalyzeCAD()
	{
		await AnalyzeCADInternal();
	}

	private async Task AnalyzeCADInternal()
	{
		try
		{
			if (string.IsNullOrEmpty(CadFilePath) || !HasCadFile)
			{
				MessageBox.Show("请先选择 CAD 文件", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			string cadFilePath = CadFilePath;
			if (_manholeLayerNames.Count <= 0 && _manholeAnnotationLayerNames.Count <= 0 && _pipeLayerNames.Count <= 0 && _pipeAnnotationLayerNames.Count <= 0)
			{
				MessageBox.Show("请先在图层列表中至少选择一类图层（管井、管井标注、管道或管道标注）\n\n选择方式：在图层名称后的复选框中勾选相应类型", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			ICADAnalyzerService iCADAnalyzerService = UIBootstrapper.TryGetService<ICADAnalyzerService>();
			if (iCADAnalyzerService == null)
			{
				MessageBox.Show("无法获取 CAD 分析服务", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				_logger.Error("[LayerParameterTab] 无法获取 ICADAnalyzerService");
				return;
			}
			CADAnalysisOptions options = new CADAnalysisOptions
			{
				ManholeLayerNames = _manholeLayerNames.ToList(),
				PipeLayerNames = _pipeLayerNames.ToList(),
				ManholeAnnotationLayerNames = _manholeAnnotationLayerNames.ToList(),
				PipeAnnotationLayerNames = _pipeAnnotationLayerNames.ToList()
			};
			IsAnalyzing = true;
			AnalysisProgress = "开始分析...";
			AnalysisProgressPercentage = 0;
			_logger.Info("[LayerParameterTab] 开始分析 CAD 文件: " + cadFilePath);
			PipeNetworkAnalysisResult pipeNetworkAnalysisResult = (_analysisResult = await iCADAnalyzerService.AnalyzeCADFileAsync(cadFilePath, options, delegate(AnalysisProgress progress)
			{
				((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
				{
					AnalysisProgress = progress.Stage + ": " + progress.Message;
					AnalysisProgressPercentage = progress.Percentage;
					if (progress.HasError)
					{
						_logger.Error("[LayerParameterTab] 分析错误: " + progress.ErrorMessage);
					}
					else
					{
						_logger.Info($"[LayerParameterTab] {progress.Stage} ({progress.Percentage}%): {progress.Message}");
					}
				});
			}));
			IsAnalyzing = false;
			AnalysisProgressPercentage = 100;
			if (pipeNetworkAnalysisResult == null || !pipeNetworkAnalysisResult.IsSuccess)
			{
				string text = pipeNetworkAnalysisResult?.Statistics.ErrorMessage ?? "未知错误";
				AnalysisProgress = "分析失败: " + text;
				MessageBox.Show("分析 CAD 失败:\n" + text, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				_logger.Error("[LayerParameterTab] 分析失败: " + text);
				return;
			}
			AnalysisProgress = "分析完成";
			_logger.Info("[LayerParameterTab] 分析成功:\n" + pipeNetworkAnalysisResult.GetSummary());
			MessageBox.Show($"CAD 分析完成！\n\n管井: {pipeNetworkAnalysisResult.Statistics.TotalManholes} 个\n  - 已标注: {pipeNetworkAnalysisResult.Statistics.AnnotatedManholes} 个 ({pipeNetworkAnalysisResult.Statistics.ManholeAnnotationCoverage:F1}%)\n  - 未标注: {pipeNetworkAnalysisResult.Statistics.UnannotatedManholes} 个\n\n管道: {pipeNetworkAnalysisResult.Statistics.TotalPipes} 个\n  - 已标注: {pipeNetworkAnalysisResult.Statistics.AnnotatedPipes} 个 ({pipeNetworkAnalysisResult.Statistics.PipeAnnotationCoverage:F1}%)\n  - 未标注: {pipeNetworkAnalysisResult.Statistics.UnannotatedPipes} 个\n  - 已连接: {pipeNetworkAnalysisResult.Statistics.ConnectedPipes} 个 ({pipeNetworkAnalysisResult.Statistics.PipeConnectionRate:F1}%)\n\n总长度: {pipeNetworkAnalysisResult.Statistics.TotalPipeLength / 1000.0:F2} 米\n耗时: {(double)pipeNetworkAnalysisResult.Statistics.AnalysisDurationMs / 1000.0:F2} 秒", "分析完成", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (Exception ex)
		{
			IsAnalyzing = false;
			AnalysisProgress = "分析失败";
			AnalysisProgressPercentage = 0;
			_analysisResult = null;
			_logger.Error("[LayerParameterTab] 分析 CAD 失败", ex);
			MessageBox.Show("分析 CAD 失败:\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private async Task<bool> CheckFamilyExistsInProjectAsync(string familyName)
	{
		_ = 1;
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader()?.RevitAdapter;
			if (revitAdapter == null)
			{
				_logger.Warning("[LayerParameterTab] 无法获取 Revit 适配器");
				return false;
			}
			object familyCheckExternalEvent = revitAdapter.GetFamilyCheckExternalEvent();
			if (familyCheckExternalEvent == null)
			{
				_logger.Warning("[LayerParameterTab] 无法获取族检查 ExternalEvent");
				return false;
			}
			new TaskCompletionSource<bool>();
			Type type = Type.GetType("RevitAi.Revit.Revit.FamilyCheckRequest, RevitAi.Revit");
			if (type == null)
			{
				_logger.Warning("[LayerParameterTab] 无法找到 FamilyCheckRequest 类型");
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
				_logger.Warning("[LayerParameterTab] 无法获取 TaskSource");
				return false;
			}
			Type type2 = Type.GetType("RevitAi.Revit.Revit.FamilyCheckRequestManager, RevitAi.Revit");
			if (type2 == null)
			{
				_logger.Warning("[LayerParameterTab] 无法找到 FamilyCheckRequestManager 类型");
				return false;
			}
			type2.GetMethod("SetRequest")?.Invoke(null, new object[1] { obj });
			(familyCheckExternalEvent?.GetType().GetMethod("Raise"))?.Invoke(familyCheckExternalEvent, null);
			if (!(obj2.GetType().GetProperty("Task")?.GetValue(obj2) is Task<bool> task))
			{
				_logger.Warning("[LayerParameterTab] 无法获取 Task");
				return false;
			}
			if (await Task.WhenAny(task, Task.Delay(5000)) != task)
			{
				_logger.Warning("[LayerParameterTab] 族检查超时");
				return false;
			}
			bool flag = await task;
			_logger.Info($"[LayerParameterTab] 项目族检查完成: {familyName}, 存在={flag}");
			return flag;
		}
		catch (Exception ex)
		{
			_logger.Error("[LayerParameterTab] 检查项目族失败", ex);
			return false;
		}
	}
}
