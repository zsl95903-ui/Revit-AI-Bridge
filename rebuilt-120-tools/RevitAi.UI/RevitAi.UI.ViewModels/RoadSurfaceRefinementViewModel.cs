using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class RoadSurfaceRefinementViewModel : ObservableObject
{
	private readonly ILogger _logger;

	[ObservableProperty]
	private bool _step2Completed;

	[ObservableProperty]
	private bool _step3Completed;

	[ObservableProperty]
	private string _operationMessage = "准备就绪，请按需执行各步骤";

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private bool _hasSelectedFloor;

	[ObservableProperty]
	private string _selectedFloorName = "未选择";

	[ObservableProperty]
	private string _selectedVoidFamilyName = "未选择";

	[ObservableProperty]
	private string _selectedCategoryCountText = "0个";

	[ObservableProperty]
	private string _selectedCategoryInstanceCountText = "0个实例";

	[ObservableProperty]
	private bool _cutAllInstances;

	private object? _selectedFloor;

	private object? _selectedVoidInstance;

	private List<object> _generatedVoidInstances = new List<object>();

	private List<object> _selectedCategoryInstances = new List<object>();

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? selectFloorCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? executeStep2Command;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? selectVoidFamilyCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? selectCategoriesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? executeStep3Command;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? executeCloseWindowCommand;

	public ObservableCollection<string> SelectedCategories { get; } = new ObservableCollection<string>();

	public bool CanExecuteCut
	{
		get
		{
			if (_selectedVoidInstance != null)
			{
				return SelectedCategories.Count > 0;
			}
			return false;
		}
	}

	public bool AllStepsCompleted
	{
		get
		{
			if (Step2Completed)
			{
				return Step3Completed;
			}
			return false;
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool Step2Completed
	{
		get
		{
			return _step2Completed;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_step2Completed, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Step2Completed);
				_step2Completed = value;
				OnStep2CompletedChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Step2Completed);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool Step3Completed
	{
		get
		{
			return _step3Completed;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_step3Completed, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Step3Completed);
				_step3Completed = value;
				OnStep3CompletedChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Step3Completed);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string OperationMessage
	{
		get
		{
			return _operationMessage;
		}
		[MemberNotNull("_operationMessage")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_operationMessage, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.OperationMessage);
				_operationMessage = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.OperationMessage);
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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasSelectedFloor
	{
		get
		{
			return _hasSelectedFloor;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasSelectedFloor, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasSelectedFloor);
				_hasSelectedFloor = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasSelectedFloor);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedFloorName
	{
		get
		{
			return _selectedFloorName;
		}
		[MemberNotNull("_selectedFloorName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedFloorName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedFloorName);
				_selectedFloorName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedFloorName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedVoidFamilyName
	{
		get
		{
			return _selectedVoidFamilyName;
		}
		[MemberNotNull("_selectedVoidFamilyName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedVoidFamilyName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedVoidFamilyName);
				_selectedVoidFamilyName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedVoidFamilyName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedCategoryCountText
	{
		get
		{
			return _selectedCategoryCountText;
		}
		[MemberNotNull("_selectedCategoryCountText")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedCategoryCountText, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedCategoryCountText);
				_selectedCategoryCountText = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedCategoryCountText);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedCategoryInstanceCountText
	{
		get
		{
			return _selectedCategoryInstanceCountText;
		}
		[MemberNotNull("_selectedCategoryInstanceCountText")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedCategoryInstanceCountText, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedCategoryInstanceCountText);
				_selectedCategoryInstanceCountText = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedCategoryInstanceCountText);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool CutAllInstances
	{
		get
		{
			return _cutAllInstances;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_cutAllInstances, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CutAllInstances);
				_cutAllInstances = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CutAllInstances);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SelectFloorCommand => selectFloorCommand ?? (selectFloorCommand = new AsyncRelayCommand(SelectFloorAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ExecuteStep2Command => executeStep2Command ?? (executeStep2Command = new AsyncRelayCommand(ExecuteStep2Async));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SelectVoidFamilyCommand => selectVoidFamilyCommand ?? (selectVoidFamilyCommand = new AsyncRelayCommand(SelectVoidFamilyAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SelectCategoriesCommand => selectCategoriesCommand ?? (selectCategoriesCommand = new AsyncRelayCommand(SelectCategoriesAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ExecuteStep3Command => executeStep3Command ?? (executeStep3Command = new AsyncRelayCommand(ExecuteStep3Async));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ExecuteCloseWindowCommand => executeCloseWindowCommand ?? (executeCloseWindowCommand = new RelayCommand(ExecuteCloseWindow));

	public RoadSurfaceRefinementViewModel()
	{
		_logger = ServiceProvider.GetLogger();
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

	private string? GetElementCategoryName(object? element)
	{
		if (element == null)
		{
			return null;
		}
		try
		{
			PropertyInfo property = element.GetType().GetProperty("Category");
			if (property != null)
			{
				object value = property.GetValue(element);
				if (value != null)
				{
					PropertyInfo property2 = value.GetType().GetProperty("Name");
					if (property2 != null)
					{
						return property2.GetValue(value)?.ToString();
					}
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private long GetElementIdLong(object? element)
	{
		if (element == null)
		{
			return -1L;
		}
		try
		{
			PropertyInfo property = element.GetType().GetProperty("Id");
			if (property != null)
			{
				object value = property.GetValue(element);
				Type type = value?.GetType();
				if (type != null && type.Name == "ElementId")
				{
					PropertyInfo property2 = type.GetProperty("Value");
					if (property2 != null && property2.GetValue(value) is long result)
					{
						return result;
					}
					PropertyInfo property3 = type.GetProperty("IntegerValue");
					if (property3 != null && property3.GetValue(value) is int num)
					{
						return num;
					}
					if (value is long result2)
					{
						return result2;
					}
					if (value is int num2)
					{
						return num2;
					}
				}
				if (value is long result3)
				{
					return result3;
				}
				if (value is int num3)
				{
					return num3;
				}
			}
		}
		catch
		{
		}
		return -1L;
	}

	[RelayCommand]
	private async Task SelectFloorAsync()
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				OperationMessage = "无法获取 Revit 适配器";
				return;
			}
			IsLoading = true;
			OperationMessage = "请在 Revit 中选择一个楼板...";
			_logger.Info("[精修路基路面] 开始选择楼板");
			TaskCompletionSource<List<object>> tcs = new TaskCompletionSource<List<object>>();
			RoadSurfaceRefinementRequestManager.SetSelectionRequest(new RoadSurfaceSelectionRequest
			{
				Mode = RoadSurfaceSelectionMode.SelectFloor,
				Prompt = "选择楼板",
				IsSingleSelection = true,
				OnCompleted = delegate(List<object> result)
				{
					tcs.TrySetResult(result);
				}
			});
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerRoadSurfaceSelection");
			if (method == null)
			{
				OperationMessage = "无法触发楼板选择";
				return;
			}
			method.Invoke(revitAdapter, new object[0]);
			List<object> list = await tcs.Task;
			if (list.Count > 0)
			{
				_selectedFloor = list[0];
				HasSelectedFloor = true;
				string elementName = GetElementName(_selectedFloor);
				SelectedFloorName = elementName ?? "未知楼板";
				OperationMessage = "已选择楼板: " + SelectedFloorName + "，请点击生成空心按钮";
				_logger.Info("[精修路基路面] 楼板选择完成: " + SelectedFloorName);
			}
			else
			{
				OperationMessage = "已取消选择楼板";
			}
		}
		catch (Exception ex)
		{
			OperationMessage = "选择楼板失败: " + ex.Message;
			_logger.Error("[精修路基路面] 选择楼板失败", ex);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private async Task ExecuteStep2Async()
	{
		try
		{
			if (_selectedFloor == null)
			{
				OperationMessage = "请先选择楼板";
				return;
			}
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				OperationMessage = "无法获取 Revit 适配器";
				return;
			}
			IsLoading = true;
			OperationMessage = "正在执行步骤2：生成空心...";
			_logger.Info("[精修路基路面] 开始执行步骤2：生成空心");
			TaskCompletionSource<(bool Success, string Message, List<object>? CreatedVoids)> tcs = new TaskCompletionSource<(bool, string, List<object>)>();
			RoadSurfaceVoidGenerationRequest roadSurfaceVoidGenerationRequest = new RoadSurfaceVoidGenerationRequest
			{
				FloorElement = _selectedFloor,
				OnCompleted = delegate((bool Success, string Message, List<object>? CreatedVoids) result)
				{
					tcs.TrySetResult(result);
				}
			};
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerRoadSurfaceVoidGeneration");
			if (method == null)
			{
				OperationMessage = "无法触发生成空心操作";
				return;
			}
			method.Invoke(revitAdapter, new object[1] { roadSurfaceVoidGenerationRequest });
			(bool, string, List<object>) tuple = await tcs.Task;
			if (tuple.Item1)
			{
				_generatedVoidInstances = tuple.Item3 ?? new List<object>();
				Step2Completed = true;
				OperationMessage = $"步骤2完成：已生成 {_generatedVoidInstances.Count} 个空心族实例";
				_logger.Info($"[精修路基路面] 步骤2完成，生成 {_generatedVoidInstances.Count} 个空心");
			}
			else
			{
				OperationMessage = "步骤2执行失败: " + tuple.Item2;
			}
		}
		catch (Exception ex)
		{
			OperationMessage = "步骤2执行失败: " + ex.Message;
			_logger.Error("[精修路基路面] 步骤2执行失败", ex);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private async Task SelectVoidFamilyAsync()
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				OperationMessage = "无法获取 Revit 适配器";
				return;
			}
			IsLoading = true;
			OperationMessage = "请在 Revit 中选择一个空心族实例...";
			_logger.Info("[精修路基路面] 开始选择空心族");
			TaskCompletionSource<List<object>> tcs = new TaskCompletionSource<List<object>>();
			RoadSurfaceRefinementRequestManager.SetSelectionRequest(new RoadSurfaceSelectionRequest
			{
				Mode = RoadSurfaceSelectionMode.SelectVoidFamily,
				Prompt = "选择空心族实例",
				IsSingleSelection = true,
				OnCompleted = delegate(List<object> result)
				{
					tcs.TrySetResult(result);
				}
			});
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerRoadSurfaceSelection");
			if (method == null)
			{
				OperationMessage = "无法触发空心族选择";
				return;
			}
			method.Invoke(revitAdapter, new object[0]);
			List<object> list = await tcs.Task;
			if (list.Count > 0)
			{
				_selectedVoidInstance = list[0];
				string elementName = GetElementName(_selectedVoidInstance);
				SelectedVoidFamilyName = elementName ?? "未知空心族";
				SelectedCategories.Clear();
				SelectedCategoryCountText = string.Empty;
				SelectedCategoryInstanceCountText = string.Empty;
				OperationMessage = "已选择空心族: " + SelectedVoidFamilyName + "，请继续选择族类型";
				OnPropertyChanged("CanExecuteCut");
				_logger.Info("[精修路基路面] 空心族选择完成: " + SelectedVoidFamilyName);
			}
			else
			{
				OperationMessage = "已取消选择空心族";
				OnPropertyChanged("CanExecuteCut");
			}
		}
		catch (Exception ex)
		{
			OperationMessage = "选择空心族失败: " + ex.Message;
			_logger.Error("[精修路基路面] 选择空心族失败", ex);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private async Task SelectCategoriesAsync()
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				OperationMessage = "无法获取 Revit 适配器";
				return;
			}
			IsLoading = true;
			OperationMessage = "请选择要剪切的族类型...";
			_logger.Info("[精修路基路面] 开始选择族类型");
			TaskCompletionSource<List<object>> tcs = new TaskCompletionSource<List<object>>();
			RoadSurfaceRefinementRequestManager.SetSelectionRequest(new RoadSurfaceSelectionRequest
			{
				Mode = RoadSurfaceSelectionMode.SelectCategories,
				Prompt = "选择族类型",
				IsSingleSelection = false,
				OnCompleted = delegate(List<object> result)
				{
					tcs.TrySetResult(result);
				}
			});
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerRoadSurfaceSelection");
			if (method == null)
			{
				OperationMessage = "无法触发生族类型选择";
				return;
			}
			method.Invoke(revitAdapter, new object[0]);
			List<object> list = await tcs.Task;
			if (list.Count > 0)
			{
				_selectedCategoryInstances = list;
				SelectedCategories.Clear();
				HashSet<string> hashSet = new HashSet<string>();
				foreach (object item in list)
				{
					string elementCategoryName = GetElementCategoryName(item);
					if (!string.IsNullOrEmpty(elementCategoryName))
					{
						hashSet.Add(elementCategoryName);
					}
				}
				foreach (string item2 in hashSet)
				{
					SelectedCategories.Add(item2);
				}
				SelectedCategoryCountText = $"{SelectedCategories.Count}个";
				SelectedCategoryInstanceCountText = $"{list.Count}个实例";
				SelectedVoidFamilyName = string.Empty;
				OperationMessage = $"已选择 {SelectedCategories.Count} 个族类型（{list.Count} 个实例），请点击执行剪切按钮";
				OnPropertyChanged("CanExecuteCut");
				_logger.Info($"[精修路基路面] 族类型选择完成，共 {SelectedCategories.Count} 个类别，{list.Count} 个实例");
			}
			else
			{
				OperationMessage = "已取消选择族类型";
			}
		}
		catch (Exception ex)
		{
			OperationMessage = "选择族类型失败: " + ex.Message;
			_logger.Error("[精修路基路面] 选择族类型失败", ex);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private async Task ExecuteStep3Async()
	{
		try
		{
			if (_selectedVoidInstance == null)
			{
				OperationMessage = "请先选择空心族";
				return;
			}
			if (SelectedCategories.Count == 0)
			{
				OperationMessage = "请先选择族类型";
				return;
			}
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader().RevitAdapter;
			if (revitAdapter == null)
			{
				OperationMessage = "无法获取 Revit 适配器";
				return;
			}
			IsLoading = true;
			OperationMessage = "正在执行步骤3：批量剪切...";
			_logger.Info("[精修路基路面] 开始执行步骤3：批量剪切");
			TaskCompletionSource<(int SuccessCount, List<string> Errors)> tcs = new TaskCompletionSource<(int, List<string>)>();
			RoadSurfaceRefinementRequestManager.SetCutRequest(new RoadSurfaceCutRequest
			{
				VoidInstances = new List<object> { _selectedVoidInstance },
				CategoryNames = SelectedCategories.ToList(),
				CutAllInstances = CutAllInstances,
				SelectedCategoryInstances = _selectedCategoryInstances,
				OnCompleted = delegate((int SuccessCount, List<string> Errors) result)
				{
					tcs.TrySetResult(result);
				}
			});
			MethodInfo method = revitAdapter.GetType().GetMethod("TriggerRoadSurfaceCut");
			if (method == null)
			{
				OperationMessage = "无法触发剪切操作";
				return;
			}
			method.Invoke(revitAdapter, new object[0]);
			(int, List<string>) tuple = await tcs.Task;
			Step3Completed = true;
			OperationMessage = $"步骤3完成：成功剪切 {tuple.Item1} 个元素";
			if (tuple.Item2.Count > 0)
			{
				OperationMessage += $"，{tuple.Item2.Count} 个失败";
			}
			_logger.Info($"[精修路基路面] 步骤3完成，成功 {tuple.Item1}，失败 {tuple.Item2.Count}");
		}
		catch (Exception ex)
		{
			OperationMessage = "步骤3执行失败: " + ex.Message;
			_logger.Error("[精修路基路面] 步骤3执行失败", ex);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private void ExecuteCloseWindow()
	{
		_logger.Info("[精修路基路面] 用户关闭窗口");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStep2CompletedChanged(bool value)
	{
		OnPropertyChanged("AllStepsCompleted");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnStep3CompletedChanged(bool value)
	{
		OnPropertyChanged("AllStepsCompleted");
	}
}
