using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit;
using RevitAi.Abstractions.Services;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class TopographyFromFloorViewModel : ObservableObject, IProgressReporter
{
	private class DefaultDialogService : IDialogService
	{
		public void ShowInfo(string message, string title = "信息")
		{
			MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}

		public Task ShowInfoAsync(string message, string title = "信息")
		{
			return Task.Run(delegate
			{
				ShowInfo(message, title);
			});
		}

		public void ShowWarning(string message, string title = "警告")
		{
			MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		public Task ShowWarningAsync(string message, string title = "警告")
		{
			return Task.Run(delegate
			{
				ShowWarning(message, title);
			});
		}

		public void ShowError(string message, string title = "错误")
		{
			MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Hand);
		}

		public Task ShowErrorAsync(string message, string title = "错误")
		{
			return Task.Run(delegate
			{
				ShowError(message, title);
			});
		}

		public bool ShowConfirm(string message, string title = "确认")
		{
			return MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
		}

		public Task<bool> ShowConfirmAsync(string message, string title = "确认")
		{
			return Task.Run(() => ShowConfirm(message, title));
		}
	}

	private readonly ILogger _logger;

	private readonly IDialogService _dialogService;

	private const int ConfirmationThreshold = 100;

	[ObservableProperty]
	private int selectedFloorCount;

	[ObservableProperty]
	private string? selectedTopographyName;

	[ObservableProperty]
	private object? selectedTopography;

	[ObservableProperty]
	private string statusMessage = "请选择楼板和地形，然后点击创建地形";

	[ObservableProperty]
	private bool isProcessing;

	[ObservableProperty]
	private int currentProgress;

	[ObservableProperty]
	private int totalProgress;

	[ObservableProperty]
	private string progressText = "";

	private List<object> _selectedFloors = new List<object>();

	private Dispatcher? _dispatcher;

	private bool _isVersionUnsupported;

	[ObservableProperty]
	private bool isCreateEnabled;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? selectFloorsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? selectTopographyCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? createTopographyCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	public bool IsVersionUnsupported => _isVersionUnsupported;

	public bool IsCancellationRequested => false;

	public bool CanCreate
	{
		get
		{
			if (!IsProcessing)
			{
				return IsCreateEnabled;
			}
			return false;
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int SelectedFloorCount
	{
		get
		{
			return selectedFloorCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(selectedFloorCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedFloorCount);
				selectedFloorCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedFloorCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? SelectedTopographyName
	{
		get
		{
			return selectedTopographyName;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(selectedTopographyName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedTopographyName);
				selectedTopographyName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedTopographyName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public object? SelectedTopography
	{
		get
		{
			return selectedTopography;
		}
		set
		{
			if (!EqualityComparer<object>.Default.Equals(selectedTopography, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedTopography);
				selectedTopography = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedTopography);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string StatusMessage
	{
		get
		{
			return statusMessage;
		}
		[MemberNotNull("statusMessage")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(statusMessage, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StatusMessage);
				statusMessage = value;
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
			return isProcessing;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(isProcessing, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsProcessing);
				isProcessing = value;
				OnIsProcessingChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsProcessing);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int CurrentProgress
	{
		get
		{
			return currentProgress;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(currentProgress, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CurrentProgress);
				currentProgress = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CurrentProgress);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int TotalProgress
	{
		get
		{
			return totalProgress;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(totalProgress, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.TotalProgress);
				totalProgress = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.TotalProgress);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string ProgressText
	{
		get
		{
			return progressText;
		}
		[MemberNotNull("progressText")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(progressText, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ProgressText);
				progressText = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ProgressText);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsCreateEnabled
	{
		get
		{
			return isCreateEnabled;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(isCreateEnabled, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsCreateEnabled);
				isCreateEnabled = value;
				OnIsCreateEnabledChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsCreateEnabled);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SelectFloorsCommand => selectFloorsCommand ?? (selectFloorsCommand = new AsyncRelayCommand(SelectFloors));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SelectTopographyCommand => selectTopographyCommand ?? (selectTopographyCommand = new AsyncRelayCommand(SelectTopography));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand CreateTopographyCommand => createTopographyCommand ?? (createTopographyCommand = new AsyncRelayCommand(CreateTopography));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public TopographyFromFloorViewModel()
	{
		_logger = ServiceProvider.GetLogger();
		_dialogService = GetDialogService();
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter != null && revitAdapter.Version.VersionYear >= 2025)
			{
				_isVersionUnsupported = true;
				StatusMessage = "⚠\ufe0f 暂不支持 Revit 2025 及以上版本的地形切分功能（地形 API 已变更）";
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[TopographyFromFloorViewModel] 版本检查失败: " + ex.Message);
		}
	}

	public void SetDispatcher(Dispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}

	public void Report(int current, int total, string message)
	{
		if (_dispatcher != null)
		{
			_dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				CurrentProgress = current;
				TotalProgress = total;
				ProgressText = message;
			}, Array.Empty<object>());
		}
	}

	public void ReportCategory(string category, int current, int total)
	{
		if (_dispatcher != null)
		{
			_dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				int value = ((total > 0) ? (current * 100 / total) : 0);
				ProgressText = $"正在{category} {current}/{total} ({value}%)";
			}, Array.Empty<object>());
		}
	}

	private ITopographyService GetTopographyService()
	{
		IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
		if (revitAdapter == null)
		{
			throw new InvalidOperationException("无法获取 Revit 适配器");
		}
		PropertyInfo? property = revitAdapter.GetType().GetProperty("TopographyService");
		if (property == null)
		{
			throw new InvalidOperationException("Revit 适配器没有 TopographyService 属性");
		}
		return (ITopographyService)(property.GetValue(revitAdapter) ?? throw new InvalidOperationException("TopographyService 为 null"));
	}

	private IDialogService GetDialogService()
	{
		try
		{
			IDialogService dialogService = UIBootstrapper.TryGetService<IDialogService>();
			if (dialogService != null)
			{
				return dialogService;
			}
			_logger.Warning("[TopographyFromFloorViewModel] 无法从 UIBootstrapper 获取对话框服务，使用默认实现");
			return new DefaultDialogService();
		}
		catch (Exception ex)
		{
			_logger.Warning("[TopographyFromFloorViewModel] 获取对话框服务失败: " + ex.Message);
			return new DefaultDialogService();
		}
	}

	[RelayCommand]
	private async Task SelectFloors()
	{
		if (_isVersionUnsupported)
		{
			StatusMessage = "⚠\ufe0f 暂不支持 Revit 2025 及以上版本";
			return;
		}
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				StatusMessage = "无法获取 Revit 适配器";
				return;
			}
			IsProcessing = true;
			StatusMessage = "请在 Revit 中选择一个或多个楼板...";
			TaskCompletionSource<List<object>> tcs = new TaskCompletionSource<List<object>>();
			TopographyOperationRequest topographyOperationRequest = new TopographyOperationRequest
			{
				Mode = OperationMode.SelectFloors,
				Prompt = "选择楼板",
				OnCompleted = delegate(List<object> result)
				{
					tcs.TrySetResult(result);
				}
			};
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerTopographyOperation");
			if (method == null)
			{
				StatusMessage = "无法触发楼板选择";
				return;
			}
			method.Invoke(revitAdapter, new object[1] { topographyOperationRequest });
			_selectedFloors = await tcs.Task;
			SelectedFloorCount = _selectedFloors.Count;
			StatusMessage = $"已选择 {SelectedFloorCount} 个楼板";
			UpdateCreateButtonState();
		}
		catch (Exception ex)
		{
			_logger.Error("[TopographyFromFloorViewModel] 选择楼板失败", ex);
			StatusMessage = "选择楼板失败: " + ex.Message;
		}
		finally
		{
			IsProcessing = false;
		}
	}

	[RelayCommand]
	private async Task SelectTopography()
	{
		if (_isVersionUnsupported)
		{
			StatusMessage = "⚠\ufe0f 暂不支持 Revit 2025 及以上版本";
			return;
		}
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				StatusMessage = "无法获取 Revit 适配器";
				return;
			}
			IsProcessing = true;
			StatusMessage = "请在 Revit 中选择一个地形...";
			TaskCompletionSource<List<object>> tcs = new TaskCompletionSource<List<object>>();
			TopographyOperationRequest topographyOperationRequest = new TopographyOperationRequest
			{
				Mode = OperationMode.SelectTopography,
				Prompt = "选择地形",
				IsSingleSelection = true,
				OnCompleted = delegate(List<object> result)
				{
					tcs.TrySetResult(result);
				}
			};
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerTopographyOperation");
			if (method == null)
			{
				StatusMessage = "无法触发生形选择";
				return;
			}
			method.Invoke(revitAdapter, new object[1] { topographyOperationRequest });
			List<object> list = await tcs.Task;
			if (list.Count > 0)
			{
				SelectedTopography = list[0];
				SelectedTopographyName = GetElementName(SelectedTopography);
				StatusMessage = "已选择地形: " + SelectedTopographyName;
			}
			UpdateCreateButtonState();
		}
		catch (Exception ex)
		{
			_logger.Error("[TopographyFromFloorViewModel] 选择地形失败", ex);
			StatusMessage = "选择地形失败: " + ex.Message;
		}
		finally
		{
			IsProcessing = false;
		}
	}

	[RelayCommand]
	private async Task CreateTopography()
	{
		if (_isVersionUnsupported)
		{
			StatusMessage = "⚠\ufe0f 暂不支持 Revit 2025 及以上版本";
			return;
		}
		try
		{
			if (_selectedFloors.Count == 0)
			{
				StatusMessage = "请先选择楼板";
				return;
			}
			if (SelectedTopography == null)
			{
				StatusMessage = "请先选择地形";
				return;
			}
			if (_selectedFloors.Count > 100)
			{
				string message = $"检测到您选择了 {_selectedFloors.Count} 个楼板，超过 {100} 个阈值。\n\n处理大量楼板可能需要较长时间，请确认是否继续？";
				if (!(await _dialogService.ShowConfirmAsync(message, "确认处理大量楼板")))
				{
					StatusMessage = "已取消操作";
					return;
				}
			}
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				StatusMessage = "无法获取 Revit 适配器";
				return;
			}
			IsProcessing = true;
			StatusMessage = "正在创建地形...";
			CurrentProgress = 0;
			TotalProgress = _selectedFloors.Count;
			ProgressText = $"正在处理 {_selectedFloors.Count} 个楼板...";
			TaskCompletionSource<(int SuccessCount, List<string> Errors, List<object> CreatedTopographies)> tcs = new TaskCompletionSource<(int, List<string>, List<object>)>();
			CreateTopographyRequest createTopographyRequest = new CreateTopographyRequest
			{
				FloorElements = _selectedFloors,
				TopographyElement = SelectedTopography,
				ProgressReporter = this,
				OnCompleted = delegate((int SuccessCount, List<string> Errors, List<object> CreatedTopographies) result)
				{
					tcs.TrySetResult(result);
				}
			};
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerCreateTopography");
			if (method == null)
			{
				StatusMessage = "无法触发创建地形操作";
				IsProcessing = false;
				return;
			}
			method.Invoke(revitAdapter, new object[1] { createTopographyRequest });
			(int, List<string>, List<object>) obj = await tcs.Task;
			int item = obj.Item1;
			List<string> item2 = obj.Item2;
			ProgressText = "完成！";
			if (item > 0)
			{
				StatusMessage = $"✓ 成功创建 {item} 个地形子面域";
				_logger.Info($"[TopographyFromFloorViewModel] 成功创建 {item} 个地形");
			}
			else
			{
				StatusMessage = "✗ 创建失败: " + string.Join("; ", item2);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[TopographyFromFloorViewModel] 创建地形失败", ex);
			StatusMessage = "✗ 创建地形失败: " + ex.Message;
			ProgressText = "处理失败";
		}
		finally
		{
			IsProcessing = false;
		}
	}

	[RelayCommand]
	private void Close()
	{
	}

	private string? GetElementName(object? element)
	{
		if (element == null)
		{
			return null;
		}
		try
		{
			PropertyInfo property = element.GetType().GetProperty("Name");
			if (property != null)
			{
				return property.GetValue(element)?.ToString();
			}
			PropertyInfo property2 = element.GetType().GetProperty("Id");
			if (property2 != null)
			{
				object value = property2.GetValue(element);
				return $"ID: {value}";
			}
		}
		catch
		{
		}
		return "未知元素";
	}

	private void UpdateCreateButtonState()
	{
		if (_isVersionUnsupported)
		{
			IsCreateEnabled = false;
			OnPropertyChanged("CanCreate");
		}
		else
		{
			IsCreateEnabled = SelectedFloorCount > 0 && SelectedTopography != null;
			OnPropertyChanged("CanCreate");
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnIsProcessingChanged(bool value)
	{
		OnPropertyChanged("CanCreate");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnIsCreateEnabledChanged(bool value)
	{
		OnPropertyChanged("CanCreate");
	}
}
