using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace RevitAi.UI.ViewModels;

public class EditBridgeComponentsViewModel : ObservableObject
{
	private readonly RoadProject _roadProject;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private UserDefinedBridgeComponent? _defaultPileType;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addConfigurationCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<BridgeComponentConfigurationItem?>? removeConfigurationCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addPilePositionCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<PilePositionItem?>? removePilePositionCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? saveCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? importPilePositionsExcelCommand;

	public string RoadName => _roadProject.Name;

	public ObservableCollection<BridgeComponentConfigurationItem> ComponentConfigurations { get; } = new ObservableCollection<BridgeComponentConfigurationItem>();

	public ObservableCollection<PilePositionItem> PilePositions { get; } = new ObservableCollection<PilePositionItem>();

	public ObservableCollection<UserDefinedBridgeComponent> PileTypes { get; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> BridgeTypes { get; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> FoundationTypes { get; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> PierTypes { get; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> BeamTypes { get; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public ObservableCollection<UserDefinedBridgeComponent> BearingTypes { get; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public Action? RequestClose { get; set; }

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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public UserDefinedBridgeComponent? DefaultPileType
	{
		get
		{
			return _defaultPileType;
		}
		set
		{
			if (!EqualityComparer<UserDefinedBridgeComponent>.Default.Equals(_defaultPileType, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DefaultPileType);
				_defaultPileType = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DefaultPileType);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddConfigurationCommand => addConfigurationCommand ?? (addConfigurationCommand = new RelayCommand(AddConfiguration));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<BridgeComponentConfigurationItem?> RemoveConfigurationCommand => removeConfigurationCommand ?? (removeConfigurationCommand = new RelayCommand<BridgeComponentConfigurationItem>(RemoveConfiguration));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddPilePositionCommand => addPilePositionCommand ?? (addPilePositionCommand = new RelayCommand(AddPilePosition));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<PilePositionItem?> RemovePilePositionCommand => removePilePositionCommand ?? (removePilePositionCommand = new RelayCommand<PilePositionItem>(RemovePilePosition));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SaveCommand => saveCommand ?? (saveCommand = new RelayCommand(Save));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ImportPilePositionsExcelCommand => importPilePositionsExcelCommand ?? (importPilePositionsExcelCommand = new RelayCommand(ImportPilePositionsExcel));

	public EditBridgeComponentsViewModel(RoadProject roadProject)
	{
		_roadProject = roadProject ?? throw new ArgumentNullException("roadProject");
		LoadComponentTypes();
		LoadExistingConfigurations();
	}

	private void LoadComponentTypes()
	{
		try
		{
			BridgeComponentDefinitionStore instance = BridgeComponentDefinitionStore.Instance;
			List<UserDefinedBridgeComponent> components = instance.GetComponents("桩部件");
			PileTypes.Clear();
			foreach (UserDefinedBridgeComponent item in components)
			{
				PileTypes.Add(item);
			}
			List<UserDefinedBridgeComponent> components2 = instance.GetComponents("桥型");
			BridgeTypes.Clear();
			foreach (UserDefinedBridgeComponent item2 in components2)
			{
				BridgeTypes.Add(item2);
			}
			List<UserDefinedBridgeComponent> components3 = instance.GetComponents("基础部件");
			FoundationTypes.Clear();
			foreach (UserDefinedBridgeComponent item3 in components3)
			{
				FoundationTypes.Add(item3);
			}
			List<UserDefinedBridgeComponent> components4 = instance.GetComponents("墩柱");
			PierTypes.Clear();
			foreach (UserDefinedBridgeComponent item4 in components4)
			{
				PierTypes.Add(item4);
			}
			List<UserDefinedBridgeComponent> components5 = instance.GetComponents("盖梁");
			BeamTypes.Clear();
			foreach (UserDefinedBridgeComponent item5 in components5)
			{
				BeamTypes.Add(item5);
			}
			List<UserDefinedBridgeComponent> components6 = instance.GetComponents("支座");
			BearingTypes.Clear();
			foreach (UserDefinedBridgeComponent item6 in components6)
			{
				BearingTypes.Add(item6);
			}
			Logger.Info($"[{RoadName}] 已加载部件类型选项：桩 {PileTypes.Count} 个，桥型 {BridgeTypes.Count} 个，基础 {FoundationTypes.Count} 个，墩柱 {PierTypes.Count} 个，盖梁 {BeamTypes.Count} 个，支座 {BearingTypes.Count} 个");
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 加载部件类型选项失败", ex);
		}
	}

	private void LoadExistingConfigurations()
	{
		try
		{
			ComponentConfigurations.Clear();
			foreach (BridgeComponentConfigurationItem item in _roadProject.LoadConfigurationsFromProject(BridgeTypes.ToList(), FoundationTypes.ToList(), PierTypes.ToList(), BeamTypes.ToList(), BearingTypes.ToList()))
			{
				ComponentConfigurations.Add(item);
			}
			Logger.Info($"[{RoadName}] 已加载 {ComponentConfigurations.Count} 个部件配置");
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 加载已有配置失败", ex);
		}
		LoadExistingPilePositions();
	}

	private void LoadExistingPilePositions()
	{
		try
		{
			PilePositions.Clear();
			foreach (PilePositionItem item in _roadProject.LoadPilePositionsFromProject(PileTypes.ToList()))
			{
				PilePositions.Add(item);
			}
			Logger.Info($"[{RoadName}] 已加载 {PilePositions.Count} 个桩定位配置");
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 加载桩定位配置失败", ex);
		}
	}

	[RelayCommand]
	private void AddConfiguration()
	{
		try
		{
			int num = ComponentConfigurations.Count + 1;
			BridgeComponentConfigurationItem item = new BridgeComponentConfigurationItem
			{
				Number = num,
				Station = "0+000"
			};
			ComponentConfigurations.Add(item);
			Logger.Info($"[{RoadName}] 添加配置项: 编号 {num}");
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 添加配置项失败", ex);
			MessageBox.Show("添加失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void RemoveConfiguration(BridgeComponentConfigurationItem? item)
	{
		try
		{
			if (item != null && MessageBox.Show($"确定要删除编号 [{item.Number}] 的配置吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				ComponentConfigurations.Remove(item);
				ReNumberItems();
				Logger.Info($"[{RoadName}] 删除配置项: 编号 {item.Number}");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 删除配置项失败", ex);
			MessageBox.Show("删除失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void AddPilePosition()
	{
		try
		{
			int num = PilePositions.Count + 1;
			PilePositionItem item = new PilePositionItem
			{
				Number = num,
				PileType = DefaultPileType
			};
			PilePositions.Add(item);
			Logger.Info($"[{RoadName}] 添加桩定位项: 编号 {num}");
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 添加桩定位项失败", ex);
			MessageBox.Show("添加失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void RemovePilePosition(PilePositionItem? item)
	{
		try
		{
			if (item != null && MessageBox.Show($"确定要删除编号 [{item.Number}] 的桩定位配置吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				PilePositions.Remove(item);
				ReNumberPilePositions();
				Logger.Info($"[{RoadName}] 删除桩定位项: 编号 {item.Number}");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 删除桩定位项失败", ex);
			MessageBox.Show("删除失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void Save()
	{
		try
		{
			_roadProject.SaveConfigurationsToProject(ComponentConfigurations.ToList());
			_roadProject.SavePilePositionsToProject(PilePositions.ToList());
			_roadProject.UpdatedAt = DateTime.Now;
			Logger.Info($"[{RoadName}] 保存 {ComponentConfigurations.Count} 个部件配置和 {PilePositions.Count} 个桩定位配置到 RoadProject（内存）");
			RequestClose?.Invoke();
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 保存配置失败", ex);
			MessageBox.Show("保存失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void Cancel()
	{
		try
		{
			RequestClose?.Invoke();
		}
		catch (Exception ex)
		{
			Logger.Error("取消操作失败: " + RoadName, ex);
		}
	}

	private void ReNumberItems()
	{
		for (int i = 0; i < ComponentConfigurations.Count; i++)
		{
			ComponentConfigurations[i].Number = i + 1;
		}
	}

	private void ReNumberPilePositions()
	{
		for (int i = 0; i < PilePositions.Count; i++)
		{
			PilePositions[i].Number = i + 1;
		}
	}

	[RelayCommand]
	private void ImportPilePositionsExcel()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择桩定位表 Excel 文件",
				Filter = "Excel 文件 (*.xlsx;*.xls;*.xlsm)|*.xlsx;*.xls;*.xlsm|所有文件 (*.*)|*.*",
				RestoreDirectory = true
			};
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			string fileName = openFileDialog.FileName;
			Logger.Info("[" + RoadName + "] 选择了桩定位 Excel 文件: " + fileName);
			List<string> excelSheetNames = GetExcelSheetNames(fileName);
			if (excelSheetNames == null || excelSheetNames.Count == 0)
			{
				MessageBox.Show("文件中没有找到工作表", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			string sheetName = excelSheetNames[0];
			if (excelSheetNames.Count > 1)
			{
				string text = ShowSheetSelectionDialog(excelSheetNames, fileName);
				if (text == null)
				{
					return;
				}
				sheetName = text;
			}
			ImportPilePositionsFromExcel(fileName, sheetName);
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 导入桩定位 Excel 失败", ex);
			MessageBox.Show("导入失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private List<string>? GetExcelSheetNames(string filePath)
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				Logger.Error("[" + RoadName + "] 获取Excel工作表名称失败: RevitAdapter 为 null");
				MessageBox.Show("无法访问 Excel 数据服务", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return null;
			}
			IExcelDataService excelDataService = revitAdapter.GetExcelDataService();
			if (excelDataService == null)
			{
				Logger.Error("[" + RoadName + "] 获取Excel工作表名称失败: GetExcelDataService 返回 null");
				MessageBox.Show("无法访问 Excel 数据服务", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return null;
			}
			Result<List<string>> sheetNames = excelDataService.GetSheetNames(filePath);
			if (sheetNames.IsFailure || sheetNames.Value == null)
			{
				Logger.Error("[" + RoadName + "] 获取Excel工作表名称失败: " + sheetNames.Error);
				MessageBox.Show("读取工作表失败: " + sheetNames.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return null;
			}
			List<string> value = sheetNames.Value;
			Logger.Info($"[{RoadName}] Excel文件包含 {value.Count} 个工作表: {string.Join(", ", value)}");
			return value;
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 获取Excel工作表名称失败", ex);
			MessageBox.Show("读取工作表失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			return null;
		}
	}

	private string? ShowSheetSelectionDialog(List<string> sheetNames, string filePath)
	{
		try
		{
			string fileName = Path.GetFileName(filePath);
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
			Logger.Error("[" + RoadName + "] 显示工作表选择对话框失败", ex);
			return sheetNames.FirstOrDefault();
		}
	}

	private void ImportPilePositionsFromExcel(string filePath, string sheetName)
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				MessageBox.Show("Revit适配器不可用", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				Logger.Error("[" + RoadName + "] 导入桩定位Excel失败: RevitAdapter 为 null");
				return;
			}
			IExcelDataService excelDataService = revitAdapter.GetExcelDataService();
			if (excelDataService == null)
			{
				MessageBox.Show("Excel数据服务不可用", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				Logger.Error("[" + RoadName + "] 导入桩定位Excel失败: GetExcelDataService 返回 null");
				return;
			}
			Result<PilePositionExcelData> result = excelDataService.ReadPilePositionTable(filePath, sheetName);
			if (!result.IsSuccess || result.Value == null)
			{
				MessageBox.Show("读取Excel失败: " + (result.Error ?? "未知错误"), "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			PilePositionExcelData value = result.Value;
			int num = 0;
			foreach (PilePositionExcelItem position in value.Positions)
			{
				PilePositionItem item = new PilePositionItem
				{
					Number = PilePositions.Count + 1,
					PierNumber = position.PierNumber,
					PileNumber = position.PileNumber,
					PileType = DefaultPileType,
					XCoordinate = position.XCoordinate.ToString("F3"),
					YCoordinate = position.YCoordinate.ToString("F3"),
					PileLength = (position.PileLength.HasValue ? position.PileLength.Value.ToString("F2") : "")
				};
				PilePositions.Add(item);
				num++;
			}
			string text = $"成功从 \"{sheetName}\" 导入 {num} 个桩位数据";
			if (value.Positions.Any((PilePositionExcelItem p) => !p.PileLength.HasValue))
			{
				text += "\n\n提示：部分桩位缺少桩长信息，请检查并补充";
			}
			MessageBox.Show(text, "导入成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			Logger.Info($"[{RoadName}] 导入桩定位Excel成功: {sheetName}, {num} 个桩位");
		}
		catch (Exception ex)
		{
			Logger.Error("[" + RoadName + "] 从Excel导入桩定位数据失败", ex);
			MessageBox.Show("导入失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}
}
