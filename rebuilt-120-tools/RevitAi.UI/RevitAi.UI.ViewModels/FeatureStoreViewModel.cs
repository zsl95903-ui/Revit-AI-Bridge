using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class FeatureStoreViewModel : ObservableObject
{
	private List<FeatureItem> _allFeatures = new List<FeatureItem>();

	private static readonly Dictionary<string, int> FeatureGroupOrderMap = new Dictionary<string, int>(StringComparer.Ordinal)
	{
		{ "AI智能设计", 1 },
		{ "房间与建筑方案", 2 },
		{ "标高轴网与基准", 3 },
		{ "墙梁柱板结构", 4 },
		{ "机电管线综合", 5 },
		{ "尺寸标注与注释", 6 },
		{ "视图图纸与出图", 7 },
		{ "族类型参数管理", 8 },
		{ "批量拆分与合并", 9 },
		{ "表格数据互通", 10 },
		{ "工具辅助系统", 11 },
		{ "路桥市政与基建", 12 }
	};

	[ObservableProperty]
	private List<FeatureItem> filteredFeatures = new List<FeatureItem>();

	[ObservableProperty]
	private string searchText;

	[ObservableProperty]
	private string selectedCategory;

	[ObservableProperty]
	private string selectAllButtonText = "全选";

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<FeatureItem?>? toggleFeatureCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectAllCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? saveAndRestartCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? resetToDefaultCommand;

	public List<FeatureItem> AllFeatures
	{
		get
		{
			return _allFeatures;
		}
		set
		{
			SetProperty(ref _allFeatures, value, "AllFeatures");
		}
	}

	public List<string> Categories
	{
		get
		{
			List<string> list = new List<string> { "全部" };
			foreach (FeatureGroup value in Enum.GetValues(typeof(FeatureGroup)))
			{
				if (value != FeatureGroup.CoreFeatures)
				{
					list.Add(value.GetDisplayName());
				}
			}
			return list;
		}
	}

	public int EnabledCount => AllFeatures.Count((FeatureItem f) => f.IsEnabled);

	public int TotalCount => AllFeatures.Count;

	public string ConfigFilePath => "user-ribbon-config.json";

	public Action? OnRequestClose { get; set; }

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public List<FeatureItem> FilteredFeatures
	{
		get
		{
			return filteredFeatures;
		}
		[MemberNotNull("filteredFeatures")]
		set
		{
			if (!EqualityComparer<List<FeatureItem>>.Default.Equals(filteredFeatures, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.FilteredFeatures);
				filteredFeatures = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.FilteredFeatures);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SearchText
	{
		get
		{
			return searchText;
		}
		[MemberNotNull("searchText")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(searchText, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SearchText);
				searchText = value;
				OnSearchTextChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SearchText);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedCategory
	{
		get
		{
			return selectedCategory;
		}
		[MemberNotNull("selectedCategory")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(selectedCategory, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedCategory);
				selectedCategory = value;
				OnSelectedCategoryChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedCategory);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectAllButtonText
	{
		get
		{
			return selectAllButtonText;
		}
		[MemberNotNull("selectAllButtonText")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(selectAllButtonText, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectAllButtonText);
				selectAllButtonText = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectAllButtonText);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<FeatureItem?> ToggleFeatureCommand => toggleFeatureCommand ?? (toggleFeatureCommand = new RelayCommand<FeatureItem>(ToggleFeature));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectAllCommand => selectAllCommand ?? (selectAllCommand = new RelayCommand(SelectAll));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SaveAndRestartCommand => saveAndRestartCommand ?? (saveAndRestartCommand = new RelayCommand(SaveAndRestart));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ResetToDefaultCommand => resetToDefaultCommand ?? (resetToDefaultCommand = new RelayCommand(ResetToDefault));

	public FeatureStoreViewModel()
	{
		searchText = string.Empty;
		selectedCategory = "全部";
		LoadFeatures();
		UpdateSelectAllButtonText();
	}

	[RelayCommand]
	private void ToggleFeature(FeatureItem? feature)
	{
		if (feature != null)
		{
			feature.IsEnabled = !feature.IsEnabled;
			FilterFeatures();
			UpdateSelectAllButtonText();
			UpdateCommandVisibility(feature);
		}
	}

	[RelayCommand]
	private void SelectAll()
	{
		List<FeatureItem> visibleFeaturesForSelection = GetVisibleFeaturesForSelection();
		if (!visibleFeaturesForSelection.Any())
		{
			return;
		}
		bool flag = !visibleFeaturesForSelection.All((FeatureItem f) => f.IsEnabled);
		foreach (FeatureItem item in visibleFeaturesForSelection)
		{
			if (item.IsEnabled != flag)
			{
				item.IsEnabled = flag;
				UpdateCommandVisibility(item);
			}
		}
		FilterFeatures();
		UpdateSelectAllButtonText();
	}

	private void UpdateCommandVisibility(FeatureItem feature)
	{
		try
		{
			Type type = Type.GetType("RevitAi.Main.RibbonVisibilityManager, RevitAi.Main");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("SetCommandVisibilityAndSave");
				if (method != null)
				{
					method.Invoke(null, new object[2] { feature.CommandName, feature.IsEnabled });
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("更新命令可见性失败: " + feature.CommandName, ex);
		}
	}

	[RelayCommand]
	private void SaveAndRestart()
	{
		try
		{
			Type type = Type.GetType("RevitAi.Main.RibbonConfigManager, RevitAi.Main");
			if (type != null)
			{
				type.GetMethod("SaveConfig")?.Invoke(null, null);
				CloseWindow();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("保存配置失败", ex);
			MessageBox.Show("保存配置失败: " + ex.Message, "应用仓库", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void Cancel()
	{
		CloseWindow();
	}

	[RelayCommand]
	private void ResetToDefault()
	{
		if (MessageBox.Show("确定要重置为默认配置吗？\n\n这将启用所有命令并恢复默认设置。", "应用仓库", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
		{
			return;
		}
		try
		{
			LoadFeatures();
			Type type = Type.GetType("RevitAi.Main.RealTimeRibbonManager, RevitAi.Main");
			if (type != null)
			{
				type.GetMethod("NotifyRibbonChanged")?.Invoke(null, null);
			}
			MessageBox.Show("已重置为默认配置！\n\n所有命令已启用。", "应用仓库", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (Exception ex)
		{
			Logger.Error("重置配置失败", ex);
			MessageBox.Show("重置配置失败: " + ex.Message, "应用仓库", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void LoadFeatures()
	{
		LoadFeaturesFromRegistry();
	}

	private void LoadFeaturesFromRegistry()
	{
		try
		{
			IReadOnlyList<RegisteredCommand> allCommands = CommandRegistry.GetAllCommands();
			List<FeatureItem> list = new List<FeatureItem>();
			MethodInfo methodInfo = Type.GetType("RevitAi.Main.RibbonVisibilityManager, RevitAi.Main")?.GetMethod("GetCommandVisibility");
			foreach (RegisteredCommand item in allCommands)
			{
				if (!item.Name.Equals("FeatureStoreCommand", StringComparison.OrdinalIgnoreCase) && !item.Attribute.IsBasicFeature)
				{
					bool isEnabled = true;
					if (methodInfo != null && methodInfo.Invoke(null, new object[1] { item.Name }) is bool flag)
					{
						isEnabled = flag;
					}
					string displayName = item.Attribute.Text.Replace("\n", "");
					list.Add(new FeatureItem
					{
						CommandName = item.Name,
						DisplayName = displayName,
						Description = item.Attribute.Description,
						Category = item.Attribute.Group.GetDisplayName(),
						SubCategory = item.Attribute.SubCategory,
						Order = item.Attribute.Order,
						IsEnabled = isEnabled,
						ColorHex = item.Attribute.Group.GetColorHex(),
						IconPath = LoadIconFilePath(item.Name)
					});
				}
			}
			AllFeatures = list;
			FilterFeatures();
			Logger.Info($"已从 CommandRegistry 加载 {AllFeatures.Count} 个功能项");
		}
		catch (Exception ex)
		{
			Logger.Error("从 CommandRegistry 加载功能失败", ex);
			AllFeatures = new List<FeatureItem>();
			FilterFeatures();
		}
	}

	private void FilterFeatures()
	{
		IEnumerable<FeatureItem> source = AllFeatures.AsEnumerable();
		if (SelectedCategory != "全部")
		{
			source = source.Where((FeatureItem f) => f.Category == SelectedCategory);
		}
		if (!string.IsNullOrWhiteSpace(SearchText))
		{
			string searchLower = SearchText.ToLower();
			source = source.Where((FeatureItem f) => f.DisplayName.ToLower().Contains(searchLower) || f.Description.ToLower().Contains(searchLower) || f.CommandName.ToLower().Contains(searchLower));
		}
		FilteredFeatures = source.OrderBy((FeatureItem f) => GetFeatureGroupOrder(f.Category)).ThenBy<FeatureItem, string>((FeatureItem f) => f.SubCategory, StringComparer.Ordinal).ThenBy((FeatureItem f) => f.Order)
			.ThenBy<FeatureItem, string>((FeatureItem f) => f.DisplayName, StringComparer.Ordinal)
			.ToList();
		OnPropertyChanged("EnabledCount");
		OnPropertyChanged("TotalCount");
	}

	private static int GetFeatureGroupOrder(string categoryName)
	{
		if (!FeatureGroupOrderMap.TryGetValue(categoryName, out var value))
		{
			return 999;
		}
		return value;
	}

	private List<FeatureItem> GetVisibleFeaturesForSelection()
	{
		IEnumerable<FeatureItem> source = AllFeatures.AsEnumerable();
		if (SelectedCategory != "全部")
		{
			source = source.Where((FeatureItem f) => f.Category == SelectedCategory);
		}
		if (!string.IsNullOrWhiteSpace(SearchText))
		{
			string searchLower = SearchText.ToLower();
			source = source.Where((FeatureItem f) => f.DisplayName.ToLower().Contains(searchLower) || f.Description.ToLower().Contains(searchLower) || f.CommandName.ToLower().Contains(searchLower));
		}
		return source.ToList();
	}

	private void UpdateSelectAllButtonText()
	{
		List<FeatureItem> visibleFeaturesForSelection = GetVisibleFeaturesForSelection();
		if (!visibleFeaturesForSelection.Any())
		{
			SelectAllButtonText = "全选";
			return;
		}
		bool flag = visibleFeaturesForSelection.All((FeatureItem f) => f.IsEnabled);
		SelectAllButtonText = (flag ? "取消全选" : "全选");
	}

	private void CloseWindow()
	{
		OnRequestClose?.Invoke();
	}

	private static string? LoadIconFilePath(string iconName)
	{
		try
		{
			string location = Assembly.GetExecutingAssembly().Location;
			if (string.IsNullOrEmpty(location))
			{
				return null;
			}
			string directoryName = Path.GetDirectoryName(location);
			if (string.IsNullOrEmpty(directoryName))
			{
				return null;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
			for (int i = 0; i <= 2; i++)
			{
				string text = Path.Combine(directoryInfo.FullName, "Resources", "Icons", iconName + ".png");
				if (File.Exists(text))
				{
					return text;
				}
				if (directoryInfo.Parent == null)
				{
					break;
				}
				directoryInfo = directoryInfo.Parent;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Warning("查找图标文件失败: " + iconName + " - " + ex.Message);
			return null;
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSearchTextChanged(string value)
	{
		FilterFeatures();
		UpdateSelectAllButtonText();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedCategoryChanged(string value)
	{
		FilterFeatures();
		UpdateSelectAllButtonText();
	}
}
