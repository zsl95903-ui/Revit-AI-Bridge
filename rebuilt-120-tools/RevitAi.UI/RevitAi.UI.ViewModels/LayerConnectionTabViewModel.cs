using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace RevitAi.UI.ViewModels;

public class LayerConnectionTabViewModel : ObservableObject
{
	public class CADLayerItem : ObservableObject
	{
		private string _name;

		private bool _isManholeSelected;

		private bool _isManholeAnnotationSelected;

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

		public CADLayerItem(string name)
		{
			_name = name;
		}
	}

	private readonly ILogger _logger;

	[ObservableProperty]
	private string? _selectedLayerName;

	[ObservableProperty]
	private bool _hasSelectedLayer;

	[ObservableProperty]
	private string? _connectionTablePath;

	[ObservableProperty]
	private string? _connectionTableName;

	[ObservableProperty]
	private bool _hasConnectionTable;

	[ObservableProperty]
	private string _pipeSystemName = "排水管";

	[ObservableProperty]
	private string? _selectionSummary = "未选择";

	[ObservableProperty]
	private bool _hasLayerInfo;

	[ObservableProperty]
	private string? _detectedLayers = "";

	[ObservableProperty]
	private int _selectedManholeCount;

	[ObservableProperty]
	private int _selectedManholeAnnotationCount;

	[ObservableProperty]
	private SelectionMode _currentSelectionMode;

	private HashSet<string> _manholeLayerNames = new HashSet<string>();

	private HashSet<string> _manholeAnnotationLayerNames = new HashSet<string>();

	[ObservableProperty]
	private List<CADLayerItem> _availableLayers = new List<CADLayerItem>();

	[ObservableProperty]
	private bool _hasAvailableLayers;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? exportSampleTableCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectManholesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectManholeAnnotationsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? importConnectionTableCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? generateNetworkCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? SelectedLayerName
	{
		get
		{
			return _selectedLayerName;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedLayerName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedLayerName);
				_selectedLayerName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedLayerName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasSelectedLayer
	{
		get
		{
			return _hasSelectedLayer;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasSelectedLayer, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasSelectedLayer);
				_hasSelectedLayer = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasSelectedLayer);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? ConnectionTablePath
	{
		get
		{
			return _connectionTablePath;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_connectionTablePath, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ConnectionTablePath);
				_connectionTablePath = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ConnectionTablePath);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? ConnectionTableName
	{
		get
		{
			return _connectionTableName;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_connectionTableName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ConnectionTableName);
				_connectionTableName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ConnectionTableName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasConnectionTable
	{
		get
		{
			return _hasConnectionTable;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasConnectionTable, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasConnectionTable);
				_hasConnectionTable = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasConnectionTable);
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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ExportSampleTableCommand => exportSampleTableCommand ?? (exportSampleTableCommand = new RelayCommand(ExportSampleTable));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectManholesCommand => selectManholesCommand ?? (selectManholesCommand = new RelayCommand(SelectManholes));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectManholeAnnotationsCommand => selectManholeAnnotationsCommand ?? (selectManholeAnnotationsCommand = new RelayCommand(SelectManholeAnnotations));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ImportConnectionTableCommand => importConnectionTableCommand ?? (importConnectionTableCommand = new RelayCommand(ImportConnectionTable));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand GenerateNetworkCommand => generateNetworkCommand ?? (generateNetworkCommand = new RelayCommand(GenerateNetwork));

	public LayerConnectionTabViewModel()
	{
		_logger = ServiceProvider.GetLogger();
	}

	[RelayCommand]
	private void ExportSampleTable()
	{
		try
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Excel文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*",
				FileName = "管道连接表样例.xlsx",
				Title = "导出管道连接表样例"
			};
			if (saveFileDialog.ShowDialog() != true)
			{
				return;
			}
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (directoryName == null)
			{
				_logger.Error("[LayerConnectionTab] 无法获取程序集目录");
				MessageBox.Show("无法获取程序集目录", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			string[] obj = new string[3] { "Resources\\Templates\\管道连接表样例.xlsx", null, null };
			InlineArray6<string> buffer = default(InlineArray6<string>);
			buffer[0] = directoryName;
			buffer[1] = "..";
			buffer[2] = "..";
			buffer[3] = "Resources";
			buffer[4] = "Templates";
			buffer[5] = "管道连接表样例.xlsx";
			obj[1] = Path.Combine(buffer);
			InlineArray5<string> buffer2 = default(InlineArray5<string>);
			buffer2[0] = directoryName;
			buffer2[1] = "..";
			buffer2[2] = "Resources";
			buffer2[3] = "Templates";
			buffer2[4] = "管道连接表样例.xlsx";
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
				_logger.Error("[LayerConnectionTab] 未找到样例文件: 管道连接表样例.xlsx");
				MessageBox.Show("未找到样例文件：管道连接表样例.xlsx\n\n请确保 Resources/Templates/ 文件夹中存在此文件", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			else
			{
				File.Copy(text, saveFileDialog.FileName, overwrite: true);
				MessageBox.Show("样例表格已导出到：\n" + saveFileDialog.FileName + "\n\n请打开文件，根据实际需求修改数据后导入。", "导出成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				_logger.Info("[LayerConnectionTab] 导出管道连接表样例成功: " + saveFileDialog.FileName);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[LayerConnectionTab] 导出样例失败", ex);
			MessageBox.Show("导出样例失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
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

	private void RequestCADElementSelection(SelectionMode mode, string prompt)
	{
		try
		{
			CurrentSelectionMode = mode;
			_logger.Info($"[LayerConnectionTab] 开始选择 {mode} 图元");
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
						_logger.Info($"[LayerConnectionTab] {mode} 选择成功，检测到 {layerNames.Count} 个图层: {string.Join(", ", layerNames)}");
						AddLayersToSelection(mode, layerNames);
					}
					else
					{
						_logger.Info($"[LayerConnectionTab] {mode} 选择已取消");
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
			_logger.Info($"[LayerConnectionTab] 已触发 {mode} 图元选择");
		}
		catch (Exception ex)
		{
			_logger.Error($"[LayerConnectionTab] 选择 {mode} 时出错: {ex.Message}", ex);
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
		if (mode == SelectionMode.ManholeAnnotations)
		{
			return true;
		}
		return false;
	}

	private bool TriggerStaticCADElementSelection(CADElementSelectionRequest request)
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				_logger.Error("[LayerConnectionTab] 无法获取 RevitAdapter");
				return false;
			}
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerCADElementSelection");
			if (method == null)
			{
				_logger.Error("[LayerConnectionTab] 未找到 TriggerCADElementSelection 方法");
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
			_logger.Error("[LayerConnectionTab] 触发 CAD 选择失败: " + ex.Message, ex);
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
				}
			}
		}
		UpdateSelectedLayers();
		HasAvailableLayers = AvailableLayers.Count > 0;
	}

	private void UpdateSelectedLayers()
	{
		_manholeLayerNames.Clear();
		_manholeAnnotationLayerNames.Clear();
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
		}
		UpdateSelectionSummary();
		UpdateLayerInfoDisplay();
	}

	private void UpdateSelectionSummary()
	{
		List<string> list = new List<string>();
		SelectedManholeCount = _manholeLayerNames.Count;
		SelectedManholeAnnotationCount = _manholeAnnotationLayerNames.Count;
		if (SelectedManholeCount > 0)
		{
			list.Add($"管井图层: {SelectedManholeCount} 个");
		}
		if (SelectedManholeAnnotationCount > 0)
		{
			list.Add($"管井标注: {SelectedManholeAnnotationCount} 个");
		}
		if (list.Count == 0)
		{
			SelectionSummary = "未选择";
		}
		else
		{
			SelectionSummary = string.Join(", ", list);
		}
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
	private void ImportConnectionTable()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择管道连接表",
				Filter = "Excel文件 (*.xlsx;*.xls)|*.xlsx;*.xls|CSV文件 (*.csv)|*.csv|所有文件 (*.*)|*.*",
				Multiselect = false
			};
			if (openFileDialog.ShowDialog() == true)
			{
				ConnectionTablePath = openFileDialog.FileName;
				ConnectionTableName = Path.GetFileName(openFileDialog.FileName);
				HasConnectionTable = true;
				_logger.Info("已导入管道连接表: " + ConnectionTableName);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("导入管道连接表失败", ex);
			MessageBox.Show("导入管道连接表失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void GenerateNetwork()
	{
		try
		{
			if (_manholeLayerNames.Count == 0)
			{
				MessageBox.Show("请先选择管井图层", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			if (!HasConnectionTable || string.IsNullOrEmpty(ConnectionTablePath))
			{
				MessageBox.Show("请先导入管道连接表", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			MessageBox.Show($"管网生成功能待实现\n\n管井图层: {string.Join(", ", _manholeLayerNames)}\n管井标注图层: {string.Join(", ", _manholeAnnotationLayerNames)}\n连接表: {ConnectionTableName}\n管道系统名称: {PipeSystemName}", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			_logger.Info("[LayerConnectionTab] 生成管网命令被执行（功能待实现）");
		}
		catch (Exception ex)
		{
			_logger.Error("生成管网失败", ex);
			MessageBox.Show("生成管网失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}
}
