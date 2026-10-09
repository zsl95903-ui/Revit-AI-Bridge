using System;
using System.CodeDom.Compiler;
using System.Collections;
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
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using OfficeOpenXml;

using ServiceProvider = RevitAi.Abstractions.Loader.ServiceProvider;

namespace RevitAi.UI.ViewModels;

public class StationElevationInputViewModel : ObservableObject
{
	private readonly object? _document;

	private Window? _window;

	[ObservableProperty]
	private double _originOffsetX;

	[ObservableProperty]
	private double _originOffsetY;

	[ObservableProperty]
	private double _originOffsetZ;

	[ObservableProperty]
	private string _statusMessage = "准备就绪 - 输入桩号坐标数据后点击完成";

	[ObservableProperty]
	private bool _isProcessing;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addSampleDataCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? importExcelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? clearDataCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addRowCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? swapXYCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? deleteSelectedRowsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? completeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	public ObservableCollection<StationPointViewModel> StationPoints { get; } = new ObservableCollection<StationPointViewModel>();

	public IList? SelectedItems { get; set; }

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
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.OriginOffsetZ);
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
	public bool IsProcessing
	{
		get
		{
			return _isProcessing;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isProcessing, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsProcessing);
				_isProcessing = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsProcessing);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddSampleDataCommand => addSampleDataCommand ?? (addSampleDataCommand = new RelayCommand(AddSampleData));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ImportExcelCommand => importExcelCommand ?? (importExcelCommand = new RelayCommand(ImportExcel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ClearDataCommand => clearDataCommand ?? (clearDataCommand = new RelayCommand(ClearData));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddRowCommand => addRowCommand ?? (addRowCommand = new RelayCommand(AddRow));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SwapXYCommand => swapXYCommand ?? (swapXYCommand = new RelayCommand(SwapXY));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DeleteSelectedRowsCommand => deleteSelectedRowsCommand ?? (deleteSelectedRowsCommand = new RelayCommand(DeleteSelectedRows));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CompleteCommand => completeCommand ?? (completeCommand = new RelayCommand(Complete));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public void SetWindow(Window window)
	{
		_window = window;
	}

	public StationElevationInputViewModel()
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter != null)
			{
				_document = revitAdapter.GetActiveDocument();
			}
			if (_document == null)
			{
				StatusMessage = "警告: 无法获取 Revit 文档";
				Logger.Warning("桩号位置高程表输入: 无法获取 Revit 文档");
			}
			LoadExistingData();
		}
		catch (Exception ex)
		{
			Logger.Error("初始化桩号位置高程表输入失败", ex);
			StatusMessage = "初始化失败: " + ex.Message;
		}
	}

	private void LoadExistingData()
	{
		try
		{
			IRoadCenterlineDataService service = UIBootstrapper.Services.GetService<IRoadCenterlineDataService>();
			if (service == null || service.StationElevationData == null)
			{
				return;
			}
			(OriginOffsetX, OriginOffsetY, OriginOffsetZ) = service.GlobalOriginOffset;
			foreach (RoadCenterlinePoint3D stationElevationDatum in service.StationElevationData)
			{
				StationPoints.Add(new StationPointViewModel
				{
					Station = stationElevationDatum.StationKm,
					X = stationElevationDatum.X,
					Y = stationElevationDatum.Y,
					Z = stationElevationDatum.Z
				});
			}
			StatusMessage = $"已加载 {StationPoints.Count} 个桩号点的数据";
			Logger.Info($"桩号位置高程表: 加载了 {StationPoints.Count} 个已有数据点");
		}
		catch (Exception ex)
		{
			Logger.Error("加载已有数据失败", ex);
		}
	}

	[RelayCommand]
	private void AddSampleData()
	{
		try
		{
			StationPoints.Clear();
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.0,
				X = 0.0,
				Y = 0.0,
				Z = 0.0
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.01,
				X = 9.984,
				Y = -0.551,
				Z = 0.046
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.02,
				X = 19.969,
				Y = -1.103,
				Z = 0.092
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.03,
				X = 29.954,
				Y = -1.654,
				Z = 0.137
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.04,
				X = 39.939,
				Y = -2.205,
				Z = 0.182
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.05,
				X = 49.924,
				Y = -2.756,
				Z = 0.215
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.06,
				X = 59.908,
				Y = -3.308,
				Z = 0.232
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.07,
				X = 69.893,
				Y = -3.859,
				Z = 0.231
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.08,
				X = 79.878,
				Y = -4.41,
				Z = 0.214
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.09,
				X = 89.863,
				Y = -4.961,
				Z = 0.185
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.1,
				X = 99.848,
				Y = -5.512,
				Z = 0.155
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.11,
				X = 109.832,
				Y = -6.064,
				Z = 0.125
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.110395,
				X = 110.227,
				Y = -6.085,
				Z = 0.123
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.12,
				X = 119.782,
				Y = -7.033,
				Z = 0.095
			});
			StationPoints.Add(new StationPointViewModel
			{
				Station = 0.128198,
				X = 127.846,
				Y = -8.498,
				Z = 0.07
			});
			StatusMessage = $"已添加示例数据（{StationPoints.Count} 行）";
			Logger.Info($"桩号位置高程表: 添加了示例数据（{StationPoints.Count} 个点）");
		}
		catch (Exception ex)
		{
			Logger.Error("添加示例数据失败", ex);
			StatusMessage = "添加失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void ImportExcel()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择逐桩坐标高程表 Excel 文件",
				Filter = "Excel 文件 (*.xlsx;*.xls;*.xlsm)|*.xlsx;*.xls;*.xlsm|所有文件 (*.*)|*.*",
				RestoreDirectory = true
			};
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			string fileName = openFileDialog.FileName;
			Logger.Info("选择了 Excel 文件: " + fileName);
			List<string> excelSheetNames = GetExcelSheetNames(fileName);
			if (excelSheetNames == null || excelSheetNames.Count == 0)
			{
				StatusMessage = "文件中没有找到工作表";
				return;
			}
			string sheetName = excelSheetNames[0];
			if (excelSheetNames.Count > 1)
			{
				string text = ShowSheetSelectionDialog(excelSheetNames, fileName);
				if (text == null)
				{
					StatusMessage = "已取消导入";
					return;
				}
				sheetName = text;
			}
			ImportFromExcel(fileName, sheetName);
		}
		catch (Exception ex)
		{
			Logger.Error("导入 Excel 失败", ex);
			StatusMessage = "导入失败: " + ex.Message;
		}
	}

	private List<string>? GetExcelSheetNames(string filePath)
	{
		try
		{
			using FileStream newStream = File.OpenRead(filePath);
			using ExcelPackage excelPackage = new ExcelPackage(newStream);
			List<string> list = excelPackage.Workbook.Worksheets.Select((ExcelWorksheet ws) => ws.Name).ToList();
			Logger.Info($"Excel文件包含 {list.Count} 个工作表: {string.Join(", ", list)}");
			return list;
		}
		catch (Exception ex)
		{
			Logger.Error("获取Excel工作表名称失败", ex);
			StatusMessage = "读取工作表失败: " + ex.Message;
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
			Logger.Error("显示工作表选择对话框失败", ex);
			return sheetNames.FirstOrDefault();
		}
	}

	private void ImportFromExcel(string filePath, string sheetName)
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				StatusMessage = "Revit适配器不可用";
				Logger.Error("导入Excel失败: RevitAdapter 为 null");
				return;
			}
			IExcelDataService excelDataService = revitAdapter.GetExcelDataService();
			if (excelDataService == null)
			{
				StatusMessage = "Excel数据服务不可用";
				Logger.Error("导入Excel失败: GetExcelDataService 返回 null");
				return;
			}
			Result<StationCoordinateTable> result = excelDataService.ReadStationCoordinateTable(filePath, sheetName);
			if (!result.IsSuccess || result.Value == null)
			{
				StatusMessage = "读取Excel失败: " + (result.Error ?? "未知错误");
				return;
			}
			StationCoordinateTable? value = result.Value;
			StationPoints.Clear();
			foreach (StationCoordinate coordinate in value.Coordinates)
			{
				StationPoints.Add(new StationPointViewModel
				{
					Station = coordinate.FullStationKm,
					X = coordinate.CoordinateX,
					Y = coordinate.CoordinateY,
					Z = coordinate.Elevation.GetValueOrDefault()
				});
			}
			StatusMessage = $"成功从 \"{sheetName}\" 导入 {StationPoints.Count} 个桩号点";
			Logger.Info($"导入Excel成功: {sheetName}, {StationPoints.Count} 个点");
		}
		catch (Exception ex)
		{
			Logger.Error("从Excel导入数据失败", ex);
			StatusMessage = "导入失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void ClearData()
	{
		try
		{
			StationPoints.Clear();
			StatusMessage = "已清空所有数据";
			Logger.Info("已清空桩号点数据");
		}
		catch (Exception ex)
		{
			Logger.Error("清空数据失败", ex);
			StatusMessage = "清空失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void AddRow()
	{
		try
		{
			double num = (StationPoints.Any() ? StationPoints.Max((StationPointViewModel p) => p.Station) : 0.0);
			StationPoints.Add(new StationPointViewModel
			{
				Station = num + 0.1,
				X = 0.0,
				Y = 0.0,
				Z = 10.0
			});
			StatusMessage = $"已添加第 {StationPoints.Count} 行数据";
		}
		catch (Exception ex)
		{
			Logger.Error("添加行失败", ex);
			StatusMessage = "添加失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void SwapXY()
	{
		try
		{
			if (StationPoints.Count == 0)
			{
				StatusMessage = "没有数据可切换";
				return;
			}
			foreach (StationPointViewModel stationPoint in StationPoints)
			{
				double x = stationPoint.X;
				stationPoint.X = stationPoint.Y;
				stationPoint.Y = x;
			}
			StatusMessage = $"已切换 {StationPoints.Count} 个点的 X/Y 坐标";
			Logger.Info($"桩号位置高程表: 已切换 {StationPoints.Count} 个点的 X/Y 坐标");
		}
		catch (Exception ex)
		{
			Logger.Error("切换X/Y坐标失败", ex);
			StatusMessage = "切换失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void DeleteSelectedRows()
	{
		try
		{
			if (SelectedItems == null || SelectedItems.Count == 0)
			{
				StatusMessage = "请先选择要删除的行";
				return;
			}
			List<StationPointViewModel> list = SelectedItems.Cast<StationPointViewModel>().ToList();
			if (list.Count == 0)
			{
				StatusMessage = "请先选择要删除的行";
				return;
			}
			foreach (StationPointViewModel item in list)
			{
				StationPoints.Remove(item);
			}
			StatusMessage = $"已删除 {list.Count} 行数据";
			Logger.Info($"删除了 {list.Count} 行桩号点数据");
		}
		catch (Exception ex)
		{
			Logger.Error("删除行失败", ex);
			StatusMessage = "删除失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void Complete()
	{
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (StationPoints.Count < 2)
			{
				StatusMessage = "请至少输入两个桩号点";
				return;
			}
			foreach (StationPointViewModel stationPoint in StationPoints)
			{
				if (double.IsNaN(stationPoint.X) || double.IsNaN(stationPoint.Y) || double.IsNaN(stationPoint.Z))
				{
					StatusMessage = $"桩号 {stationPoint.Station:F3} 的坐标数据不完整";
					return;
				}
			}
			IRoadCenterlineDataService service = UIBootstrapper.Services.GetService<IRoadCenterlineDataService>();
			if (service != null)
			{
				StationPointViewModel stationPointViewModel = StationPoints.OrderBy((StationPointViewModel p) => p.Station).FirstOrDefault();
				if (stationPointViewModel != null)
				{
					bool num = Math.Abs(stationPointViewModel.X) > 0.001 || Math.Abs(stationPointViewModel.Y) > 0.001;
					bool flag = Math.Abs(OriginOffsetX) < 0.001 && Math.Abs(OriginOffsetY) < 0.001 && Math.Abs(OriginOffsetZ) < 0.001;
					if (num & flag)
					{
						OriginOffsetX = stationPointViewModel.X;
						OriginOffsetY = stationPointViewModel.Y;
						Logger.Info($"桩号位置高程表: 自动设置原点偏移为第一行坐标: X={OriginOffsetX:F2}, Y={OriginOffsetY:F2}, Z={OriginOffsetZ:F2}");
						StatusMessage = $"已自动设置原点偏移: X={OriginOffsetX:F2}, Y={OriginOffsetY:F2}";
					}
				}
				List<RoadCenterlinePoint3D> list = (from sp in StationPoints
					orderby sp.Station
					select new RoadCenterlinePoint3D
					{
						StationKm = sp.Station,
						X = sp.X,
						Y = sp.Y,
						Z = sp.Z,
						Azimuth = 0.0,
						Curvature = 0.0,
						PointType = RoadCenterlinePointType.Interpolated
					}).ToList();
				service.SetGlobalOriginOffset(OriginOffsetX, OriginOffsetY, OriginOffsetZ);
				service.SetStationElevationData(list, new Point(OriginOffsetX, OriginOffsetY));
				StatusMessage = $"数据已保存 ({StationPoints.Count} 个桩号点)";
				Logger.Info($"桩号位置高程表: 保存了 {list.Count} 个桩号点（原始坐标），全局偏移量: X={OriginOffsetX}, Y={OriginOffsetY}, Z={OriginOffsetZ}");
			}
			Close();
		}
		catch (Exception ex)
		{
			Logger.Error("完成失败", ex);
			StatusMessage = "完成失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void Close()
	{
		_window?.Close();
	}
}
