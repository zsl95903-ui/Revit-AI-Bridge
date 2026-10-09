using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit;
using RevitAi.Abstractions.Services;
using RevitAi.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace RevitAi.UI.Views.Windows;

public partial class ExportFamilyMetadataWindow : Window, IComponentConnector
{
	private readonly ILogger _logger;

	private readonly IRevitAdapter _revitAdapter;

	private readonly ObservableCollection<FamilyDisplayItem> _families = new ObservableCollection<FamilyDisplayItem>();

	private string? _lastExportDirectory;

	public ObservableCollection<FamilyDisplayItem> Families => _families;

	public Visibility ProgressVisibility { get; set; } = Visibility.Collapsed;

	public string ProgressText { get; set; } = "";

	public double ProgressValue { get; set; }

	public ExportFamilyMetadataWindow()
	{
		InitializeComponent();
		base.DataContext = this;
		IServiceProvider services = UIBootstrapper.Services;
		if (services == null)
		{
			throw new InvalidOperationException("无法获取服务容器");
		}
		_logger = services.GetRequiredService<ILogger>();
		_revitAdapter = services.GetRequiredService<IRevitAdapter>();
		base.Loaded += ExportFamilyMetadataWindow_Loaded;
	}

	private void ExportFamilyMetadataWindow_Loaded(object sender, RoutedEventArgs e)
	{
		LoadFamilies();
		UpdateSelectedCount();
		_families.CollectionChanged += delegate
		{
			UpdateSelectedCount();
		};
		ChkHideProjectFamilies.Checked += delegate
		{
			LoadFamilies();
		};
		ChkHideProjectFamilies.Unchecked += delegate
		{
			LoadFamilies();
		};
	}

	private void LoadFamilies()
	{
		try
		{
			IFamilyMetadataService familyMetadataService = UIBootstrapper.Services?.GetRequiredService<IFamilyMetadataService>();
			if (familyMetadataService == null)
			{
				_logger.Warning("[ExportFamilyMetadataWindow] 无法获取族元数据服务");
				System.Windows.MessageBox.Show("无法获取族元数据服务，请确保在 Revit 命令上下文中调用", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			bool valueOrDefault = ChkHideProjectFamilies.IsChecked == true;
			List<FamilyDisplayItem> list = _families.Where((FamilyDisplayItem f) => f.IsExternalFamily).ToList();
			_families.Clear();
			if (!valueOrDefault)
			{
				object activeDocument = _revitAdapter.GetActiveDocument();
				if (activeDocument != null)
				{
					foreach (FamilyInfo allFamily in familyMetadataService.GetAllFamilies(activeDocument))
					{
						FamilyDisplayItem familyDisplayItem = new FamilyDisplayItem
						{
							Family = allFamily.Family,
							DisplayName = allFamily.DisplayName,
							FamilyName = allFamily.FamilyName,
							CategoryName = allFamily.CategoryName,
							IsSelected = false,
							IsExternalFamily = false
						};
						familyDisplayItem.SelectionChanged += delegate
						{
							UpdateSelectedCount();
						};
						_families.Add(familyDisplayItem);
					}
				}
			}
			foreach (FamilyDisplayItem item in list)
			{
				_families.Add(item);
			}
			_logger.Info($"[ExportFamilyMetadataWindow] 加载了 {_families.Count} 个族");
			UpdateSelectedCount();
		}
		catch (Exception ex)
		{
			_logger.Error("[ExportFamilyMetadataWindow] 加载族列表失败", ex);
			System.Windows.MessageBox.Show("加载族列表失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void BtnAddFamilies_Click(object sender, RoutedEventArgs e)
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
		{
			Title = "选择族文件",
			Filter = "族文件 (*.rfa)|*.rfa|所有文件 (*.*)|*.*",
			Multiselect = true,
			RestoreDirectory = true
		};
		if (openFileDialog.ShowDialog() != true)
		{
			return;
		}
		string[] fileNames = openFileDialog.FileNames;
		foreach (string text in fileNames)
		{
			try
			{
				string familyName = Path.GetFileNameWithoutExtension(text);
				if (!_families.Any((FamilyDisplayItem f) => f.FamilyName == familyName && f.IsExternalFamily))
				{
					FamilyDisplayItem familyDisplayItem = new FamilyDisplayItem
					{
						Family = text,
						DisplayName = familyName + " (外部文件)",
						FamilyName = familyName,
						CategoryName = "外部",
						IsSelected = true,
						IsExternalFamily = true,
						FilePath = text
					};
					familyDisplayItem.SelectionChanged += delegate
					{
						UpdateSelectedCount();
					};
					_families.Add(familyDisplayItem);
				}
			}
			catch (Exception ex)
			{
				_logger.Warning("[ExportFamilyMetadataWindow] 添加族文件失败: " + text + " - " + ex.Message);
			}
		}
		UpdateSelectedCount();
	}

	private void UpdateSelectedCount()
	{
		int value = _families.Count((FamilyDisplayItem f) => f.IsSelected);
		TxtSelectedCount.Text = $"已选择: {value} 个族";
	}

	private void ChkSelectAll_Checked(object sender, RoutedEventArgs e)
	{
		foreach (FamilyDisplayItem family in _families)
		{
			family.IsSelected = true;
		}
		UpdateSelectedCount();
	}

	private void ChkSelectAll_Unchecked(object sender, RoutedEventArgs e)
	{
		foreach (FamilyDisplayItem family in _families)
		{
			family.IsSelected = false;
		}
		UpdateSelectedCount();
	}

	private async void BtnExport_Click(object sender, RoutedEventArgs e)
	{
		List<FamilyDisplayItem> list = _families.Where((FamilyDisplayItem f) => f.IsSelected).ToList();
		if (list.Count == 0)
		{
			System.Windows.MessageBox.Show("请选择要导出的族", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		Microsoft.Win32.OpenFolderDialog dialog = new Microsoft.Win32.OpenFolderDialog
		{
			Title = "选择导出目录"
		};
		string initialExportDirectory = GetInitialExportDirectory(list);
		if (!string.IsNullOrEmpty(initialExportDirectory))
		{
			dialog.InitialDirectory = initialExportDirectory;
		}
		if (dialog.ShowDialog() == true)
		{
			string selectedPath = dialog.FolderName;
			if (string.IsNullOrEmpty(selectedPath))
			{
				System.Windows.MessageBox.Show("请选择有效的导出目录", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			_lastExportDirectory = selectedPath;
			await ExportFamiliesAsync(list, selectedPath);
		}
	}

	private string? GetInitialExportDirectory(List<FamilyDisplayItem> selectedFamilies)
	{
		if (!string.IsNullOrEmpty(_lastExportDirectory))
		{
			return _lastExportDirectory;
		}
		FamilyDisplayItem familyDisplayItem = selectedFamilies.FirstOrDefault();
		if (familyDisplayItem != null)
		{
			if (familyDisplayItem.IsExternalFamily && !string.IsNullOrEmpty(familyDisplayItem.FilePath))
			{
				return Path.GetDirectoryName(familyDisplayItem.FilePath);
			}
			object activeDocument = _revitAdapter.GetActiveDocument();
			if (activeDocument != null)
			{
				try
				{
					string text = activeDocument.GetType().GetProperty("Path")?.GetValue(activeDocument) as string;
					if (!string.IsNullOrEmpty(text))
					{
						return Path.GetDirectoryName(text);
					}
				}
				catch
				{
				}
			}
		}
		return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
	}

	private async Task ExportFamiliesAsync(List<FamilyDisplayItem> families, string exportDirectory)
	{
		bool valueOrDefault = ChkSaveFamilyFile.IsChecked == true;
		bool valueOrDefault2 = ChkExportThumbnail.IsChecked == true;
		bool valueOrDefault3 = ChkExportMetadata.IsChecked == true;
		bool valueOrDefault4 = ChkOverwrite.IsChecked == true;
		if (!valueOrDefault && !valueOrDefault2 && !valueOrDefault3)
		{
			System.Windows.MessageBox.Show("请至少选择一种导出内容（族文件/缩略图/元数据）", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		List<FamilyExportItem> families2 = families.Select((FamilyDisplayItem f) => new FamilyExportItem
		{
			Family = f.Family,
			FamilyName = f.FamilyName,
			IsExternalFamily = f.IsExternalFamily,
			FilePath = f.FilePath
		}).ToList();
		TaskCompletionSource<ExportFamilyMetadataResult> tcs = new TaskCompletionSource<ExportFamilyMetadataResult>();
		ExportFamilyMetadataRequest exportFamilyMetadataRequest = new ExportFamilyMetadataRequest
		{
			Families = families2,
			ExportDirectory = exportDirectory,
			SaveFamilyFile = valueOrDefault,
			ExportThumbnail = valueOrDefault2,
			ExportMetadata = valueOrDefault3,
			Overwrite = valueOrDefault4,
			OnProgress = delegate(int current, int total, string message)
			{
				((DispatcherObject)this).Dispatcher.Invoke((Action)delegate
				{
					ProgressValue = (double)current / (double)total * 100.0;
					ProgressText = message;
				});
			},
			OnCompleted = delegate(ExportFamilyMetadataResult result)
			{
				tcs.TrySetResult(result);
			}
		};
		ProgressVisibility = Visibility.Visible;
		ProgressText = "正在准备导出...";
		ProgressValue = 0.0;
		try
		{
			MethodInfo method = _revitAdapter.GetType().GetMethod("TriggerExportFamilyMetadata");
			if (method == null)
			{
				_logger.Error("[ExportFamilyMetadataWindow] 无法找到 TriggerExportFamilyMetadata 方法");
				System.Windows.MessageBox.Show("无法触发导出操作：未找到触发方法", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			_logger.Info("[ExportFamilyMetadataWindow] 准备触发导出操作...");
			object obj = method.Invoke(_revitAdapter, new object[1] { exportFamilyMetadataRequest });
			_logger.Info($"[ExportFamilyMetadataWindow] 触发结果类型: {obj?.GetType().Name}, 值: {obj}");
			if (!(obj is bool) || !(bool)obj)
			{
				_logger.Error($"[ExportFamilyMetadataWindow] 触发导出失败，结果: {obj}");
				System.Windows.MessageBox.Show($"触发导出操作失败，结果: {obj}", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			_logger.Info("[ExportFamilyMetadataWindow] 导出操作已触发，等待完成...");
			ProgressText = "导出操作已触发，正在处理...";
			ExportFamilyMetadataResult exportFamilyMetadataResult = await tcs.Task;
			ProgressValue = 100.0;
			string text = $"导出完成！\n\n成功: {exportFamilyMetadataResult.SuccessCount}\n失败: {exportFamilyMetadataResult.FailCount}";
			if (exportFamilyMetadataResult.SkippedCount > 0)
			{
				text += $"\n跳过: {exportFamilyMetadataResult.SkippedCount}";
			}
			ProgressText = text;
			System.Windows.MessageBox.Show(text, "完成", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (Exception ex)
		{
			_logger.Error("[ExportFamilyMetadataWindow] 导出过程失败", ex);
			System.Windows.MessageBox.Show("导出失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			await Task.Delay(2000);
			ProgressVisibility = Visibility.Collapsed;
		}
	}

	private void BtnClose_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}
}
