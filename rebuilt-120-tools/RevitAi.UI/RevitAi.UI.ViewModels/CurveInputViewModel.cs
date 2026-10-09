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

using ServiceProvider = RevitAi.Abstractions.Loader.ServiceProvider;

namespace RevitAi.UI.ViewModels;

public class CurveInputViewModel : ObservableObject
{
	private readonly object? _document;

	private readonly IRoadCenterlineDataService? _dataService;

	private Window? _window;

	[ObservableProperty]
	private double _originOffsetX;

	[ObservableProperty]
	private double _originOffsetY;

	[ObservableProperty]
	private double _originOffsetZ;

	[ObservableProperty]
	private string _statusMessage = "准备就绪 - 输入平纵曲线数据后点击完成";

	[ObservableProperty]
	private bool _isProcessing;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addSampleDataCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? importHorizontalCurveCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? importVerticalCurveCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? clearHorizontalDataCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? clearVerticalDataCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? deleteSelectedHorizontalRowsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? deleteSelectedVerticalRowsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addHorizontalRowCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? swapHorizontalXYCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addVerticalRowCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? completeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	public ObservableCollection<HorizontalCurvePointViewModel> HorizontalCurvePoints { get; } = new ObservableCollection<HorizontalCurvePointViewModel>();

	public ObservableCollection<VerticalCurvePointViewModel> VerticalCurvePoints { get; } = new ObservableCollection<VerticalCurvePointViewModel>();

	public IList? SelectedHorizontalItems { get; set; }

	public IList? SelectedVerticalItems { get; set; }

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
	public IRelayCommand ImportHorizontalCurveCommand => importHorizontalCurveCommand ?? (importHorizontalCurveCommand = new RelayCommand(ImportHorizontalCurve));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ImportVerticalCurveCommand => importVerticalCurveCommand ?? (importVerticalCurveCommand = new RelayCommand(ImportVerticalCurve));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ClearHorizontalDataCommand => clearHorizontalDataCommand ?? (clearHorizontalDataCommand = new RelayCommand(ClearHorizontalData));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ClearVerticalDataCommand => clearVerticalDataCommand ?? (clearVerticalDataCommand = new RelayCommand(ClearVerticalData));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DeleteSelectedHorizontalRowsCommand => deleteSelectedHorizontalRowsCommand ?? (deleteSelectedHorizontalRowsCommand = new RelayCommand(DeleteSelectedHorizontalRows));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DeleteSelectedVerticalRowsCommand => deleteSelectedVerticalRowsCommand ?? (deleteSelectedVerticalRowsCommand = new RelayCommand(DeleteSelectedVerticalRows));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddHorizontalRowCommand => addHorizontalRowCommand ?? (addHorizontalRowCommand = new RelayCommand(AddHorizontalRow));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SwapHorizontalXYCommand => swapHorizontalXYCommand ?? (swapHorizontalXYCommand = new RelayCommand(SwapHorizontalXY));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddVerticalRowCommand => addVerticalRowCommand ?? (addVerticalRowCommand = new RelayCommand(AddVerticalRow));

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

	public CurveInputViewModel()
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
				Logger.Warning("平纵曲线表输入: 无法获取 Revit 文档");
			}
			_dataService = UIBootstrapper.Services.GetService<IRoadCenterlineDataService>();
			LoadExistingData();
		}
		catch (Exception ex)
		{
			Logger.Error("初始化平纵曲线表输入失败", ex);
			StatusMessage = "初始化失败: " + ex.Message;
		}
	}

	private void LoadExistingData()
	{
		try
		{
			if (_dataService == null)
			{
				return;
			}
			(OriginOffsetX, OriginOffsetY, OriginOffsetZ) = _dataService.GlobalOriginOffset;
			if (_dataService.HorizontalCurveTable?.IntersectionPoints != null)
			{
				foreach (HorizontalCurveIP intersectionPoint in _dataService.HorizontalCurveTable.IntersectionPoints)
				{
					HorizontalCurvePoints.Add(new HorizontalCurvePointViewModel
					{
						IpNumber = (intersectionPoint.IPNumber ?? ""),
						Station = intersectionPoint.Station,
						X = intersectionPoint.CoordinateX,
						Y = intersectionPoint.CoordinateY,
						Radius = intersectionPoint.Radius,
						Ls1 = intersectionPoint.FirstTransitionLength.GetValueOrDefault(),
						Ls2 = intersectionPoint.SecondTransitionLength.GetValueOrDefault()
					});
				}
			}
			if (_dataService.VerticalCurveTable?.Points != null)
			{
				foreach (VerticalCurvePVI point in _dataService.VerticalCurveTable.Points)
				{
					double radius = point.Radius;
					radius = ((point.CurveType != VerticalCurveType.Convex) ? Math.Abs(radius) : (0.0 - Math.Abs(radius)));
					VerticalCurvePoints.Add(new VerticalCurvePointViewModel
					{
						PointNumber = (point.PointNumber ?? ""),
						Station = point.Station,
						Elevation = point.Elevation,
						Radius = radius,
						CurveType = ((point.CurveType == VerticalCurveType.Convex) ? "凸" : "凹")
					});
				}
			}
			if (HorizontalCurvePoints.Count > 0 || VerticalCurvePoints.Count > 0)
			{
				StatusMessage = $"已加载 {HorizontalCurvePoints.Count} 个平曲线点，{VerticalCurvePoints.Count} 个纵曲线点";
				Logger.Info($"平纵曲线表: 加载了 {HorizontalCurvePoints.Count} 个平曲线点，{VerticalCurvePoints.Count} 个纵曲线点");
			}
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
			HorizontalCurvePoints.Clear();
			VerticalCurvePoints.Clear();
			HorizontalCurvePoints.Add(new HorizontalCurvePointViewModel
			{
				IpNumber = "QD",
				Station = 0.0,
				X = 453577.614,
				Y = 4357724.399,
				Radius = 0.0,
				Ls1 = 0.0,
				Ls2 = 0.0
			});
			HorizontalCurvePoints.Add(new HorizontalCurvePointViewModel
			{
				IpNumber = "JD1",
				Station = 0.150851,
				X = 453723.771,
				Y = 4357687.062,
				Radius = 1200.0,
				Ls1 = 0.0,
				Ls2 = 0.0
			});
			HorizontalCurvePoints.Add(new HorizontalCurvePointViewModel
			{
				IpNumber = "JD2",
				Station = 0.954555,
				X = 454529.05,
				Y = 4357687.062,
				Radius = 700.0,
				Ls1 = 50.0,
				Ls2 = 50.0
			});
			HorizontalCurvePoints.Add(new HorizontalCurvePointViewModel
			{
				IpNumber = "JD3",
				Station = 1.527653,
				X = 455092.285,
				Y = 4357795.205,
				Radius = 1000.0,
				Ls1 = 0.0,
				Ls2 = 0.0
			});
			HorizontalCurvePoints.Add(new HorizontalCurvePointViewModel
			{
				IpNumber = "ZD",
				Station = 1.961117,
				X = 455495.273,
				Y = 4357562.54,
				Radius = 0.0,
				Ls1 = 0.0,
				Ls2 = 0.0
			});
			VerticalCurvePoints.Add(new VerticalCurvePointViewModel
			{
				PointNumber = "BP1",
				Station = 0.0,
				Elevation = 13.536,
				Radius = 0.0
			});
			VerticalCurvePoints.Add(new VerticalCurvePointViewModel
			{
				PointNumber = "BP2",
				Station = 0.178237,
				Elevation = 13.0,
				Radius = 20000.0
			});
			VerticalCurvePoints.Add(new VerticalCurvePointViewModel
			{
				PointNumber = "BP3",
				Station = 0.39511,
				Elevation = 13.651,
				Radius = -20000.0
			});
			VerticalCurvePoints.Add(new VerticalCurvePointViewModel
			{
				PointNumber = "BP4",
				Station = 0.563119,
				Elevation = 13.144,
				Radius = 7100.0
			});
			VerticalCurvePoints.Add(new VerticalCurvePointViewModel
			{
				PointNumber = "BP5",
				Station = 0.715337,
				Elevation = 15.29,
				Radius = -4500.0
			});
			VerticalCurvePoints.Add(new VerticalCurvePointViewModel
			{
				PointNumber = "BP6",
				Station = 0.873119,
				Elevation = 13.248,
				Radius = 7600.0
			});
			VerticalCurvePoints.Add(new VerticalCurvePointViewModel
			{
				PointNumber = "BP7",
				Station = 1.486137,
				Elevation = 15.089,
				Radius = -19900.0
			});
			VerticalCurvePoints.Add(new VerticalCurvePointViewModel
			{
				PointNumber = "BP8",
				Station = 1.970769,
				Elevation = 13.619,
				Radius = 0.0
			});
			StatusMessage = $"已添加示例数据（{HorizontalCurvePoints.Count} 个平曲线点，{VerticalCurvePoints.Count} 个纵曲线点）";
			Logger.Info("平纵曲线表: 添加了示例数据");
		}
		catch (Exception ex)
		{
			Logger.Error("添加示例数据失败", ex);
			StatusMessage = "添加失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void ImportHorizontalCurve()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择平曲线表 Excel 文件",
				Filter = "Excel 文件 (*.xlsx;*.xls;*.xlsm)|*.xlsx;*.xls;*.xlsm|所有文件 (*.*)|*.*",
				RestoreDirectory = true
			};
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			string fileName = openFileDialog.FileName;
			Logger.Info("选择了平曲线 Excel 文件: " + fileName);
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
			ImportHorizontalCurveFromExcel(fileName, sheetName);
		}
		catch (Exception ex)
		{
			Logger.Error("导入平曲线 Excel 失败", ex);
			StatusMessage = "导入失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void ImportVerticalCurve()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择纵曲线表 Excel 文件",
				Filter = "Excel 文件 (*.xlsx;*.xls;*.xlsm)|*.xlsx;*.xls;*.xlsm|所有文件 (*.*)|*.*",
				RestoreDirectory = true
			};
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			string fileName = openFileDialog.FileName;
			Logger.Info("选择了纵曲线 Excel 文件: " + fileName);
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
			ImportVerticalCurveFromExcel(fileName, sheetName);
		}
		catch (Exception ex)
		{
			Logger.Error("导入纵曲线 Excel 失败", ex);
			StatusMessage = "导入失败: " + ex.Message;
		}
	}

	private List<string>? GetExcelSheetNames(string filePath)
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				Logger.Error("获取Excel工作表名称失败: RevitAdapter 为 null");
				StatusMessage = "无法访问 Excel 数据服务";
				return null;
			}
			IExcelDataService excelDataService = revitAdapter.GetExcelDataService();
			if (excelDataService == null)
			{
				Logger.Error("获取Excel工作表名称失败: GetExcelDataService 返回 null");
				StatusMessage = "无法访问 Excel 数据服务";
				return null;
			}
			Result<List<string>> sheetNames = excelDataService.GetSheetNames(filePath);
			if (sheetNames.IsFailure || sheetNames.Value == null)
			{
				Logger.Error("获取Excel工作表名称失败: " + sheetNames.Error);
				StatusMessage = "读取工作表失败: " + sheetNames.Error;
				return null;
			}
			List<string> value = sheetNames.Value;
			Logger.Info($"Excel文件包含 {value.Count} 个工作表: {string.Join(", ", value)}");
			return value;
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

	private void ImportHorizontalCurveFromExcel(string filePath, string sheetName)
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				StatusMessage = "Revit适配器不可用";
				Logger.Error("导入平曲线Excel失败: RevitAdapter 为 null");
				return;
			}
			IExcelDataService excelDataService = revitAdapter.GetExcelDataService();
			if (excelDataService == null)
			{
				StatusMessage = "Excel数据服务不可用";
				Logger.Error("导入平曲线Excel失败: GetExcelDataService 返回 null");
				return;
			}
			Result<HorizontalCurveTable> result = excelDataService.ReadHorizontalCurveTable(filePath, sheetName);
			if (!result.IsSuccess || result.Value == null)
			{
				StatusMessage = "读取Excel失败: " + (result.Error ?? "未知错误");
				return;
			}
			HorizontalCurveTable? value = result.Value;
			HorizontalCurvePoints.Clear();
			foreach (HorizontalCurveIP intersectionPoint in value.IntersectionPoints)
			{
				HorizontalCurvePoints.Add(new HorizontalCurvePointViewModel
				{
					IpNumber = (intersectionPoint.IPNumber ?? ""),
					Station = intersectionPoint.Station,
					X = intersectionPoint.CoordinateX,
					Y = intersectionPoint.CoordinateY,
					Radius = intersectionPoint.Radius,
					Ls1 = intersectionPoint.FirstTransitionLength.GetValueOrDefault(),
					Ls2 = intersectionPoint.SecondTransitionLength.GetValueOrDefault()
				});
			}
			StatusMessage = $"成功从 \"{sheetName}\" 导入 {HorizontalCurvePoints.Count} 个平曲线交点";
			Logger.Info($"导入平曲线Excel成功: {sheetName}, {HorizontalCurvePoints.Count} 个交点");
		}
		catch (Exception ex)
		{
			Logger.Error("从Excel导入平曲线数据失败", ex);
			StatusMessage = "导入失败: " + ex.Message;
		}
	}

	private void ImportVerticalCurveFromExcel(string filePath, string sheetName)
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				StatusMessage = "Revit适配器不可用";
				Logger.Error("导入纵曲线Excel失败: RevitAdapter 为 null");
				return;
			}
			IExcelDataService excelDataService = revitAdapter.GetExcelDataService();
			if (excelDataService == null)
			{
				StatusMessage = "Excel数据服务不可用";
				Logger.Error("导入纵曲线Excel失败: GetExcelDataService 返回 null");
				return;
			}
			Result<VerticalCurveTable> result = excelDataService.ReadVerticalCurveTable(filePath, sheetName);
			if (!result.IsSuccess || result.Value == null)
			{
				StatusMessage = "读取Excel失败: " + (result.Error ?? "未知错误");
				return;
			}
			VerticalCurveTable? value = result.Value;
			VerticalCurvePoints.Clear();
			foreach (VerticalCurvePVI point in value.Points)
			{
				VerticalCurvePoints.Add(new VerticalCurvePointViewModel
				{
					PointNumber = (point.PointNumber ?? ""),
					Station = point.Station,
					Elevation = point.Elevation,
					Radius = point.Radius
				});
			}
			StatusMessage = $"成功从 \"{sheetName}\" 导入 {VerticalCurvePoints.Count} 个纵曲线变坡点";
			Logger.Info($"导入纵曲线Excel成功: {sheetName}, {VerticalCurvePoints.Count} 个变坡点");
		}
		catch (Exception ex)
		{
			Logger.Error("从Excel导入纵曲线数据失败", ex);
			StatusMessage = "导入失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void ClearHorizontalData()
	{
		try
		{
			HorizontalCurvePoints.Clear();
			StatusMessage = "已清空平曲线数据";
			Logger.Info("平纵曲线表: 已清空平曲线数据");
		}
		catch (Exception ex)
		{
			Logger.Error("清空平曲线数据失败", ex);
			StatusMessage = "清空失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void ClearVerticalData()
	{
		try
		{
			VerticalCurvePoints.Clear();
			StatusMessage = "已清空纵曲线数据";
			Logger.Info("平纵曲线表: 已清空纵曲线数据");
		}
		catch (Exception ex)
		{
			Logger.Error("清空纵曲线数据失败", ex);
			StatusMessage = "清空失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void DeleteSelectedHorizontalRows()
	{
		try
		{
			if (SelectedHorizontalItems == null || SelectedHorizontalItems.Count == 0)
			{
				StatusMessage = "请先选择要删除的平曲线行";
				return;
			}
			List<HorizontalCurvePointViewModel> list = SelectedHorizontalItems.Cast<HorizontalCurvePointViewModel>().ToList();
			if (list.Count == 0)
			{
				StatusMessage = "请先选择要删除的平曲线行";
				return;
			}
			foreach (HorizontalCurvePointViewModel item in list)
			{
				HorizontalCurvePoints.Remove(item);
			}
			StatusMessage = $"已删除 {list.Count} 个平曲线交点";
			Logger.Info($"删除了 {list.Count} 个平曲线交点");
		}
		catch (Exception ex)
		{
			Logger.Error("删除平曲线行失败", ex);
			StatusMessage = "删除失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void DeleteSelectedVerticalRows()
	{
		try
		{
			if (SelectedVerticalItems == null || SelectedVerticalItems.Count == 0)
			{
				StatusMessage = "请先选择要删除的纵曲线行";
				return;
			}
			List<VerticalCurvePointViewModel> list = SelectedVerticalItems.Cast<VerticalCurvePointViewModel>().ToList();
			if (list.Count == 0)
			{
				StatusMessage = "请先选择要删除的纵曲线行";
				return;
			}
			foreach (VerticalCurvePointViewModel item in list)
			{
				VerticalCurvePoints.Remove(item);
			}
			StatusMessage = $"已删除 {list.Count} 个纵曲线变坡点";
			Logger.Info($"删除了 {list.Count} 个纵曲线变坡点");
		}
		catch (Exception ex)
		{
			Logger.Error("删除纵曲线行失败", ex);
			StatusMessage = "删除失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void AddHorizontalRow()
	{
		try
		{
			double num = (HorizontalCurvePoints.Any() ? HorizontalCurvePoints.Max((HorizontalCurvePointViewModel p) => p.Station) : 0.0);
			HorizontalCurvePoints.Add(new HorizontalCurvePointViewModel
			{
				IpNumber = $"JD{HorizontalCurvePoints.Count + 1}",
				Station = num + 0.1,
				X = 0.0,
				Y = 0.0,
				Radius = 0.0,
				Ls1 = 0.0,
				Ls2 = 0.0
			});
			StatusMessage = $"已添加第 {HorizontalCurvePoints.Count} 个平曲线交点";
		}
		catch (Exception ex)
		{
			Logger.Error("添加平曲线行失败", ex);
			StatusMessage = "添加失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void SwapHorizontalXY()
	{
		try
		{
			if (HorizontalCurvePoints.Count == 0)
			{
				StatusMessage = "没有平曲线数据可切换";
				return;
			}
			foreach (HorizontalCurvePointViewModel horizontalCurvePoint in HorizontalCurvePoints)
			{
				double x = horizontalCurvePoint.X;
				horizontalCurvePoint.X = horizontalCurvePoint.Y;
				horizontalCurvePoint.Y = x;
			}
			StatusMessage = $"已切换 {HorizontalCurvePoints.Count} 个平曲线交点的 X/Y 坐标";
			Logger.Info($"平纵曲线表: 已切换 {HorizontalCurvePoints.Count} 个平曲线交点的 X/Y 坐标");
		}
		catch (Exception ex)
		{
			Logger.Error("切换平曲线X/Y坐标失败", ex);
			StatusMessage = "切换失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void AddVerticalRow()
	{
		try
		{
			double num = (VerticalCurvePoints.Any() ? VerticalCurvePoints.Max((VerticalCurvePointViewModel p) => p.Station) : 0.0);
			VerticalCurvePoints.Add(new VerticalCurvePointViewModel
			{
				PointNumber = $"PVI{VerticalCurvePoints.Count + 1}",
				Station = num + 0.1,
				Elevation = 10.0,
				Radius = -25000.0
			});
			StatusMessage = $"已添加第 {VerticalCurvePoints.Count} 个纵曲线变坡点";
		}
		catch (Exception ex)
		{
			Logger.Error("添加纵曲线行失败", ex);
			StatusMessage = "添加失败: " + ex.Message;
		}
	}

	[RelayCommand]
	private void Complete()
	{
		try
		{
			if (HorizontalCurvePoints.Count < 2)
			{
				StatusMessage = "请至少输入两个平曲线交点";
				return;
			}
			foreach (HorizontalCurvePointViewModel horizontalCurvePoint in HorizontalCurvePoints)
			{
				if (double.IsNaN(horizontalCurvePoint.X) || double.IsNaN(horizontalCurvePoint.Y) || double.IsNaN(horizontalCurvePoint.Radius))
				{
					StatusMessage = "平曲线交点 " + horizontalCurvePoint.IpNumber + " 的数据不完整";
					return;
				}
			}
			if (_dataService == null)
			{
				StatusMessage = "数据服务不可用";
				return;
			}
			List<HorizontalCurvePointViewModel> list = HorizontalCurvePoints.OrderBy((HorizontalCurvePointViewModel p) => p.Station).ToList();
			List<HorizontalCurveIP> list2 = new List<HorizontalCurveIP>();
			for (int num = 0; num < list.Count; num++)
			{
				HorizontalCurvePointViewModel horizontalCurvePointViewModel = list[num];
				double num2 = 0.0;
				if (num > 0 && num < list.Count - 1)
				{
					HorizontalCurvePointViewModel horizontalCurvePointViewModel2 = list[num - 1];
					HorizontalCurvePointViewModel horizontalCurvePointViewModel3 = list[num + 1];
					double num3 = Math.Atan2(x: horizontalCurvePointViewModel.X - horizontalCurvePointViewModel2.X, y: horizontalCurvePointViewModel.Y - horizontalCurvePointViewModel2.Y) * 180.0 / Math.PI;
					for (num2 = Math.Atan2(x: horizontalCurvePointViewModel3.X - horizontalCurvePointViewModel.X, y: horizontalCurvePointViewModel3.Y - horizontalCurvePointViewModel.Y) * 180.0 / Math.PI - num3; num2 > 180.0; num2 -= 360.0)
					{
					}
					for (; num2 < -180.0; num2 += 360.0)
					{
					}
				}
				list2.Add(new HorizontalCurveIP
				{
					IPNumber = horizontalCurvePointViewModel.IpNumber,
					Station = horizontalCurvePointViewModel.Station,
					CoordinateX = horizontalCurvePointViewModel.X,
					CoordinateY = horizontalCurvePointViewModel.Y,
					DeflectionAngle = num2,
					Radius = horizontalCurvePointViewModel.Radius,
					FirstTransitionLength = ((horizontalCurvePointViewModel.Ls1 > 0.0) ? new double?(horizontalCurvePointViewModel.Ls1) : ((double?)null)),
					SecondTransitionLength = ((horizontalCurvePointViewModel.Ls2 > 0.0) ? new double?(horizontalCurvePointViewModel.Ls2) : ((double?)null))
				});
			}
			HorizontalCurveTable horizontalTable = new HorizontalCurveTable
			{
				IntersectionPoints = list2
			};
			VerticalCurveTable verticalTable = null;
			if (VerticalCurvePoints.Count > 0)
			{
				List<VerticalCurvePointViewModel> list3 = VerticalCurvePoints.OrderBy((VerticalCurvePointViewModel p) => p.Station).ToList();
				List<VerticalCurvePVI> list4 = new List<VerticalCurvePVI>();
				for (int num4 = 0; num4 < list3.Count; num4++)
				{
					VerticalCurvePointViewModel verticalCurvePointViewModel = list3[num4];
					double? frontGradient = null;
					if (num4 > 0)
					{
						VerticalCurvePointViewModel verticalCurvePointViewModel2 = list3[num4 - 1];
						double num5 = (verticalCurvePointViewModel.Station - verticalCurvePointViewModel2.Station) * 1000.0;
						double num6 = verticalCurvePointViewModel.Elevation - verticalCurvePointViewModel2.Elevation;
						if (num5 > 1E-06)
						{
							frontGradient = num6 / num5 * 100.0;
						}
					}
					double? backGradient = null;
					if (num4 < list3.Count - 1)
					{
						VerticalCurvePointViewModel verticalCurvePointViewModel3 = list3[num4 + 1];
						double num7 = (verticalCurvePointViewModel3.Station - verticalCurvePointViewModel.Station) * 1000.0;
						double num8 = verticalCurvePointViewModel3.Elevation - verticalCurvePointViewModel.Elevation;
						if (num7 > 1E-06)
						{
							backGradient = num8 / num7 * 100.0;
						}
					}
					double num9 = Math.Abs(verticalCurvePointViewModel.Radius);
					double? curveLength = null;
					VerticalCurveType curveType = VerticalCurveType.Concave;
					if (frontGradient.HasValue && backGradient.HasValue && num9 > 1E-06)
					{
						double num10 = backGradient.Value - frontGradient.Value;
						curveType = ((!(num10 < 0.0)) ? VerticalCurveType.Concave : VerticalCurveType.Convex);
						double value = num10 / 100.0;
						double num11 = num9 * Math.Abs(value) / 2.0;
						curveLength = 2.0 * num11;
					}
					list4.Add(new VerticalCurvePVI
					{
						PointNumber = verticalCurvePointViewModel.PointNumber,
						Station = verticalCurvePointViewModel.Station,
						Elevation = verticalCurvePointViewModel.Elevation,
						Radius = num9,
						CurveType = curveType,
						FrontGradient = frontGradient,
						BackGradient = backGradient,
						CurveLength = curveLength
					});
				}
				verticalTable = new VerticalCurveTable
				{
					Points = list4
				};
			}
			HorizontalCurvePointViewModel horizontalCurvePointViewModel4 = list.FirstOrDefault();
			if (horizontalCurvePointViewModel4 != null)
			{
				bool num12 = Math.Abs(horizontalCurvePointViewModel4.X) > 0.001 || Math.Abs(horizontalCurvePointViewModel4.Y) > 0.001;
				bool flag = Math.Abs(OriginOffsetX) < 0.001 && Math.Abs(OriginOffsetY) < 0.001 && Math.Abs(OriginOffsetZ) < 0.001;
				if (num12 & flag)
				{
					OriginOffsetX = horizontalCurvePointViewModel4.X;
					OriginOffsetY = horizontalCurvePointViewModel4.Y;
					Logger.Info($"平纵曲线表: 自动设置原点偏移为第一行坐标: X={OriginOffsetX:F2}, Y={OriginOffsetY:F2}, Z={OriginOffsetZ:F2}");
					StatusMessage = $"已自动设置原点偏移: X={OriginOffsetX:F2}, Y={OriginOffsetY:F2}";
				}
			}
			_dataService.SetGlobalOriginOffset(OriginOffsetX, OriginOffsetY, OriginOffsetZ);
			_dataService.SetCurveData(horizontalTable, verticalTable);
			StatusMessage = $"数据已保存（{HorizontalCurvePoints.Count} 个平曲线点，{VerticalCurvePoints.Count} 个纵曲线点）";
			Logger.Info($"平纵曲线表: 保存了 {HorizontalCurvePoints.Count} 个平曲线点，{VerticalCurvePoints.Count} 个纵曲线点，全局偏移量: X={OriginOffsetX}, Y={OriginOffsetY}, Z={OriginOffsetZ}");
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
