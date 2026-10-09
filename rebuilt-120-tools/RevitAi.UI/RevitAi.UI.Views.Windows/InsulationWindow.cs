using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;

namespace RevitAi.UI.Views.Windows;

public partial class InsulationWindow : Window, IComponentConnector, IStyleConnector
{
	private readonly object? _revitAdapter;

	private InsulationFilterType _currentFilter;

	private List<SystemInsulationInfo> _systemInfos = new List<SystemInsulationInfo>();

	private List<InsulationTypeInfoLite> _insulationTypes = new List<InsulationTypeInfoLite>();

	private List<SizeRangeInfo> _sizeRanges = new List<SizeRangeInfo>();

	private SystemInsulationInfo? _currentSizeBasedSystem;

	private List<SelectedElementInfo> _selectedElements = new List<SelectedElementInfo>();

	public InsulationWindow()
	{
		InitializeComponent();
		_revitAdapter = ServiceProvider.GetModuleLoader()?.RevitAdapter;
		base.Loaded += delegate
		{
			((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				UpdateStatus("正在加载数据，请稍候...");
				RefreshAllData();
			}, (DispatcherPriority)3, Array.Empty<object>());
		};
	}

	private object? GetActiveDocument()
	{
		try
		{
			if (_revitAdapter == null)
			{
				return null;
			}
			return _revitAdapter.GetType().GetMethod("GetActiveDocument")?.Invoke(_revitAdapter, null);
		}
		catch (Exception ex)
		{
			Logger.Error("[InsulationWindow] 获取活动文档失败: " + ex.Message);
			return null;
		}
	}

	private void TriggerInsulation(InsulationRequest request)
	{
		try
		{
			if (_revitAdapter == null)
			{
				UpdateStatus("无法获取 Revit 适配器");
				return;
			}
			request.Document = GetActiveDocument();
			MethodInfo method = _revitAdapter.GetType().GetMethod("TriggerInsulation");
			if (method == null)
			{
				UpdateStatus("未找到 TriggerInsulation 方法");
				return;
			}
			method.Invoke(_revitAdapter, new object[1] { request });
		}
		catch (Exception ex)
		{
			Logger.Error("[InsulationWindow] 触发保温操作失败: " + ex.Message);
			UpdateStatus("触发失败: " + ex.Message);
		}
	}

	private void RefreshAllData()
	{
		InsulationRequest request = new InsulationRequest
		{
			RequestType = InsulationRequestType.LoadAllSystemData,
			OnCompleted = delegate(InsulationRequestResult result)
			{
				((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
				{
					if (result.Error != null)
					{
						UpdateStatus("加载失败: " + result.Error);
					}
					else
					{
						_insulationTypes = result.InsulationTypes ?? new List<InsulationTypeInfoLite>();
						_systemInfos = result.Systems ?? new List<SystemInsulationInfo>();
						foreach (SystemInsulationInfo systemInfo in _systemInfos)
						{
							systemInfo.PropertyChanged += delegate(object? s, PropertyChangedEventArgs e)
							{
								if (e.PropertyName == "IsSelected")
								{
									UpdateSelectedCount();
								}
							};
						}
						InitializeSizeBasedSystemComboBox();
						UpdateDisplayedSystems();
						UpdateStatistics();
						UpdateStatus($"已加载 {_systemInfos.Count} 个系统类型（水管 + 风管）");
					}
				}, Array.Empty<object>());
			}
		};
		TriggerInsulation(request);
	}

	private void InitializeSizeBasedSystemComboBox()
	{
		SizeBasedSystemComboBox.ItemsSource = _systemInfos.Where((SystemInsulationInfo s) => s.ElementType == SystemElementType.Pipe).ToList();
	}

	private void UpdateDisplayedSystems()
	{
		SystemItemsControl.ItemsSource = null;
		SystemItemsControl.ItemsSource = GetDisplayedSystems().ToList();
	}

	private IEnumerable<SystemInsulationInfo> GetDisplayedSystems()
	{
		if (_currentFilter == InsulationFilterType.Pipe)
		{
			return _systemInfos.Where((SystemInsulationInfo s) => s.ElementType == SystemElementType.Pipe);
		}
		if (_currentFilter == InsulationFilterType.Duct)
		{
			return _systemInfos.Where((SystemInsulationInfo s) => s.ElementType == SystemElementType.Duct);
		}
		return _systemInfos;
	}

	private void UpdateStatistics()
	{
		List<SystemInsulationInfo> list = GetDisplayedSystems().ToList();
		SystemCountTextBlock.Text = list.Count.ToString();
		TotalElementsTextBlock.Text = list.Sum((SystemInsulationInfo s) => s.ElementCount).ToString();
		InsulatedElementsTextBlock.Text = list.Sum((SystemInsulationInfo s) => s.InsulatedCount).ToString();
		UpdateSelectedCount();
	}

	private void UpdateSelectedCount()
	{
		List<SystemInsulationInfo> list = _systemInfos.Where((SystemInsulationInfo s) => s.IsSelected).ToList();
		int value = list.Sum((SystemInsulationInfo s) => s.UninsulatedCount);
		SelectedCountTextBlock.Text = ((list.Count == 0) ? "未选择任何系统" : $"已选择 {list.Count} 个系统，可添加 {value} 个保温");
	}

	private void OnFilterTypeChanged(object sender, RoutedEventArgs e)
	{
		if (base.IsLoaded)
		{
			if (AllRadio.IsChecked == true)
			{
				_currentFilter = InsulationFilterType.All;
			}
			else if (PipeRadio.IsChecked == true)
			{
				_currentFilter = InsulationFilterType.Pipe;
			}
			else if (DuctRadio.IsChecked == true)
			{
				_currentFilter = InsulationFilterType.Duct;
			}
			UpdateDisplayedSystems();
			UpdateStatistics();
		}
	}

	private void OnIsolateSystem(object sender, RoutedEventArgs e)
	{
		try
		{
			if (!(sender is Button { Tag: var tag }))
			{
				return;
			}
			SystemInsulationInfo systemInfo = tag as SystemInsulationInfo;
			if (systemInfo == null)
			{
				return;
			}
			InsulationRequest request = new InsulationRequest
			{
				RequestType = InsulationRequestType.IsolateSystem,
				TargetSystem = systemInfo,
				OnCompleted = delegate(InsulationRequestResult result)
				{
					((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						if (result.Error != null)
						{
							MessageBox.Show(result.Error, "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						}
						else
						{
							UpdateStatus($"已隔离系统：{systemInfo.SystemTypeName}（{result.Count} 个元素，含保温层），点击「恢复显示」可取消");
						}
					}, Array.Empty<object>());
				}
			};
			TriggerInsulation(request);
		}
		catch (Exception ex)
		{
			MessageBox.Show("隔离系统失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void OnSelectUninsulated(object sender, RoutedEventArgs e)
	{
		try
		{
			if (!(sender is Button { Tag: var tag }))
			{
				return;
			}
			SystemInsulationInfo systemInfo = tag as SystemInsulationInfo;
			if (systemInfo == null)
			{
				return;
			}
			InsulationRequest request = new InsulationRequest
			{
				RequestType = InsulationRequestType.SelectUninsulated,
				TargetSystem = systemInfo,
				OnCompleted = delegate(InsulationRequestResult result)
				{
					((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						if (result.Error != null)
						{
							MessageBox.Show(result.Error, "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						}
						else
						{
							UpdateStatus((result.Count > 0) ? $"已选择 {result.Count} 个未保温元素（{systemInfo.SystemTypeName}）" : ("系统中没有未保温元素（" + systemInfo.SystemTypeName + "）"));
						}
					}, Array.Empty<object>());
				}
			};
			TriggerInsulation(request);
		}
		catch (Exception ex)
		{
			MessageBox.Show("选择未保温元素失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void OnRestoreDisplay(object sender, RoutedEventArgs e)
	{
		try
		{
			InsulationRequest request = new InsulationRequest
			{
				RequestType = InsulationRequestType.RestoreDisplay,
				OnCompleted = delegate(InsulationRequestResult result)
				{
					((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						UpdateStatus(result.Error ?? result.Message);
					}, Array.Empty<object>());
				}
			};
			TriggerInsulation(request);
		}
		catch (Exception ex)
		{
			MessageBox.Show("恢复显示失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void OnExecuteSystemAdd(object sender, RoutedEventArgs e)
	{
		try
		{
			List<SystemInsulationInfo> list = _systemInfos.Where((SystemInsulationInfo s) => s.IsSelected).ToList();
			if (list.Count == 0)
			{
				MessageBox.Show("请先选择要添加保温的系统类型", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			foreach (SystemInsulationInfo item in list)
			{
				if (!double.TryParse(item.ThicknessInput, out var result) || result <= 0.0)
				{
					MessageBox.Show("系统 '" + item.SystemTypeName + "' 的保温厚度无效，请输入有效的数值", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
				if (string.IsNullOrWhiteSpace(item.MaterialInput))
				{
					MessageBox.Show("系统 '" + item.SystemTypeName + "' 的保温材质名称为空，请输入材质名称", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
			}
			string text = "将为以下 " + list.Count + " 个系统批量添加保温：\n\n";
			List<SystemInsulationInfo> list2 = new List<SystemInsulationInfo>();
			foreach (SystemInsulationInfo system in list)
			{
				string value = ((system.ElementType == SystemElementType.Pipe) ? "水管" : "风管");
				InsulationTypeInfoLite? insulationTypeInfoLite = _insulationTypes.FirstOrDefault((InsulationTypeInfoLite t) => t.Name.Equals(system.MaterialInput, StringComparison.OrdinalIgnoreCase) || t.Name.Contains(system.MaterialInput));
				string value2 = ((insulationTypeInfoLite != null) ? "" : " (将创建新类型)");
				text += $"• [{value}] {system.SystemTypeName}: {system.ThicknessInput}mm {system.MaterialInput}{value2}\n";
				if (insulationTypeInfoLite == null)
				{
					list2.Add(system);
				}
			}
			if (list2.Any())
			{
				text += $"\n将自动创建 {list2.Count} 个新的保温类型";
			}
			int value3 = list.Sum((SystemInsulationInfo s) => s.UninsulatedCount);
			text += $"\n预计添加约 {value3} 个保温";
			if (MessageBox.Show(text + "\n\n是否继续？", "确认批量操作", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
			{
				return;
			}
			UpdateStatus("正在执行保温操作...");
			Mouse.OverrideCursor = Cursors.Wait;
			InsulationRequest request = new InsulationRequest
			{
				RequestType = InsulationRequestType.SystemAdd,
				SelectedSystems = list,
				OnCompleted = delegate(InsulationRequestResult insulationRequestResult)
				{
					((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						Mouse.OverrideCursor = null;
						if (insulationRequestResult.Error != null)
						{
							MessageBox.Show("执行失败: " + insulationRequestResult.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
							UpdateStatus("操作失败");
						}
						else
						{
							ShowOperationResult(insulationRequestResult.Summary);
							RefreshAllData();
							UpdateStatus("操作完成");
						}
					}, Array.Empty<object>());
				}
			};
			TriggerInsulation(request);
		}
		catch (Exception ex)
		{
			Mouse.OverrideCursor = null;
			MessageBox.Show("执行失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void ShowOperationResult(InsulationOperationSummary? summary)
	{
		if (summary != null)
		{
			string text = "操作完成：";
			if (summary.Added > 0)
			{
				text += $"\n新增：{summary.Added} 个";
			}
			if (summary.Modified > 0)
			{
				text += $"\n修改：{summary.Modified} 个";
			}
			if (summary.Skipped > 0)
			{
				text += $"\n跳过：{summary.Skipped} 个";
			}
			if (summary.Errors.Count > 0)
			{
				text += $"\n\n未添加：{summary.Errors.Count} 个（部分模型类型不支持添加保温，已自动跳过）";
			}
			MessageBox.Show(text, "操作完成", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
	}

	private void OnSelectElements(object sender, RoutedEventArgs e)
	{
		try
		{
			InsulationRequest request = new InsulationRequest
			{
				RequestType = InsulationRequestType.PickInsulationElements,
				Prompt = "请选择要添加保温的管道或风管",
				OnCompleted = delegate(InsulationRequestResult result)
				{
					((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						if (result.Error != null)
						{
							MessageBox.Show("选择元素失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
						}
						else
						{
							_selectedElements.Clear();
							_selectedElements.AddRange(result.PickedElements ?? new List<SelectedElementInfo>());
							UpdateSelectedElementsDisplay();
						}
					}, Array.Empty<object>());
				}
			};
			TriggerInsulation(request);
		}
		catch (Exception ex)
		{
			MessageBox.Show("选择元素失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void OnIsolateSelected(object sender, RoutedEventArgs e)
	{
		try
		{
			List<int> selectedIds = _selectedElements.Select((SelectedElementInfo s) => s.Id).ToList();
			if (!selectedIds.Any())
			{
				MessageBox.Show("请先点击「选择元素」选择要隔离的管道或风管", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			InsulationRequest request = new InsulationRequest
			{
				RequestType = InsulationRequestType.IsolateElements,
				ElementIds = selectedIds,
				OnCompleted = delegate(InsulationRequestResult result)
				{
					((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						if (result.Error != null)
						{
							MessageBox.Show("当前视图不支持自动隔离（" + result.Error + "）。", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						}
						else
						{
							UpdateStatus($"已在当前视图隔离选中的 {selectedIds.Count} 个元素（含保温层）");
						}
					}, Array.Empty<object>());
				}
			};
			TriggerInsulation(request);
		}
		catch (Exception ex)
		{
			MessageBox.Show("隔离选中元素失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void OnClearSelection(object sender, RoutedEventArgs e)
	{
		_selectedElements.Clear();
		UpdateSelectedElementsDisplay();
	}

	private void OnRemoveSelectedElement(object sender, RoutedEventArgs e)
	{
		if (sender is Button { Tag: SelectedElementInfo tag })
		{
			_selectedElements.Remove(tag);
			UpdateSelectedElementsDisplay();
		}
	}

	private void UpdateSelectedElementsDisplay()
	{
		if (_selectedElements.Any())
		{
			NoSelectionTextBlock.Visibility = Visibility.Collapsed;
			SelectedElementsDataGrid.Visibility = Visibility.Visible;
			SelectedElementsDataGrid.ItemsSource = null;
			SelectedElementsDataGrid.ItemsSource = _selectedElements;
			SelectedElementsCountTextBlock.Text = $"已选择 {_selectedElements.Count} 个元素";
		}
		else
		{
			NoSelectionTextBlock.Visibility = Visibility.Visible;
			SelectedElementsDataGrid.Visibility = Visibility.Collapsed;
			SelectedElementsCountTextBlock.Text = "已选择 0 个元素";
		}
	}

	private void OnExecuteManual(object sender, RoutedEventArgs e)
	{
		if (!_selectedElements.Any())
		{
			MessageBox.Show("请先选择要添加保温的元素", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
		if (!double.TryParse(ManualThicknessTextBox.Text, out var result) || result <= 0.0)
		{
			MessageBox.Show("请输入有效的保温厚度", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
		string text = ManualMaterialTextBox.Text.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			MessageBox.Show("请输入保温材质名称", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}
		else
		{
			if (MessageBox.Show($"将为 {_selectedElements.Count} 个选定元素添加保温：\n保温厚度：{result}mm\n保温材质：{text}\n\n是否继续？", "确认操作", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
			{
				return;
			}
			Mouse.OverrideCursor = Cursors.Wait;
			InsulationRequest request = new InsulationRequest
			{
				RequestType = InsulationRequestType.ManualAdd,
				ManualElements = new List<SelectedElementInfo>(_selectedElements),
				ManualThicknessMM = result,
				ManualMaterial = text,
				OnCompleted = delegate(InsulationRequestResult insulationRequestResult)
				{
					((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						Mouse.OverrideCursor = null;
						if (insulationRequestResult.Error != null)
						{
							MessageBox.Show("执行失败: " + insulationRequestResult.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
						}
						else
						{
							ShowOperationResult(insulationRequestResult.Summary);
							RefreshAllData();
						}
					}, Array.Empty<object>());
				}
			};
			TriggerInsulation(request);
		}
	}

	private void OnSizeBasedSystemChanged(object sender, SelectionChangedEventArgs e)
	{
		if (SizeBasedSystemComboBox.SelectedItem is SystemInsulationInfo systemInsulationInfo)
		{
			_currentSizeBasedSystem = systemInsulationInfo;
			LoadSizeBasedData(systemInsulationInfo);
		}
	}

	private void LoadSizeBasedData(SystemInsulationInfo systemInfo)
	{
		InsulationRequest request = new InsulationRequest
		{
			RequestType = InsulationRequestType.LoadSizeRanges,
			TargetSystem = systemInfo,
			OnCompleted = delegate(InsulationRequestResult result)
			{
				((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
				{
					if (result.Error != null)
					{
						MessageBox.Show("加载尺寸分档数据失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
					}
					else
					{
						_sizeRanges = result.SizeRanges ?? new List<SizeRangeInfo>();
						SizeBasedDataGrid.ItemsSource = null;
						SizeBasedDataGrid.ItemsSource = _sizeRanges;
					}
				}, Array.Empty<object>());
			}
		};
		TriggerInsulation(request);
	}

	private void OnExecuteSizeBased(object sender, RoutedEventArgs e)
	{
		if (_currentSizeBasedSystem == null)
		{
			MessageBox.Show("请先选择系统类型", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
		List<SizeRangeInfo> list = _sizeRanges.Where((SizeRangeInfo r) => !string.IsNullOrWhiteSpace(r.Material) && double.TryParse(r.Thickness, out var result) && result > 0.0).ToList();
		if (!list.Any())
		{
			MessageBox.Show("没有可应用的尺寸分档，请先为分档填写有效的保温厚度和材质", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
		int num = _sizeRanges.Count - list.Count;
		string text = "将为系统「" + _currentSizeBasedSystem.SystemTypeName + "」按尺寸分档添加保温：\n\n";
		foreach (SizeRangeInfo item in list)
		{
			text += $"• {item.SizeRange}: {item.Thickness}mm {item.Material} ({item.ElementCount}个元素)\n";
		}
		if (num > 0)
		{
			text += $"\n（已自动跳过 {num} 个未填写保温参数的分档）\n";
		}
		if (MessageBox.Show(text + "\n\n是否继续？", "确认操作", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
		{
			return;
		}
		UpdateStatus("正在执行按尺寸分档保温操作...");
		Mouse.OverrideCursor = Cursors.Wait;
		InsulationRequest request = new InsulationRequest
		{
			RequestType = InsulationRequestType.SizeBasedAdd,
			TargetSystem = _currentSizeBasedSystem,
			SizeRanges = list,
			OnCompleted = delegate(InsulationRequestResult result)
			{
				((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
				{
					Mouse.OverrideCursor = null;
					if (result.Error != null)
					{
						MessageBox.Show("执行失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
						UpdateStatus("操作失败");
					}
					else
					{
						ShowOperationResult(result.Summary);
						RefreshAllData();
						UpdateStatus("操作完成");
					}
				}, Array.Empty<object>());
			}
		};
		TriggerInsulation(request);
	}

	private void UpdateStatus(string message)
	{
		if (StatusTextBlock != null)
		{
			StatusTextBlock.Text = message;
		}
	}

	private void OnClose(object sender, RoutedEventArgs e)
	{
		Close();
	}
}
