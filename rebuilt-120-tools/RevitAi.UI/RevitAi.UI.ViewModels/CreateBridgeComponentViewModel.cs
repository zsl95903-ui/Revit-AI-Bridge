using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.Infrastructure;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using ServiceProvider = RevitAi.Abstractions.Loader.ServiceProvider;

namespace RevitAi.UI.ViewModels;

public class CreateBridgeComponentViewModel : ObservableObject
{
	private readonly IRoadProjectManager? _roadProjectManager;

	private readonly IRevitAdapter? _revitAdapter;

	[ObservableProperty]
	private RoadProjectItem? _selectedRoadProject;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private string _statusMessage = "就绪";

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? definePileCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? defineFoundationCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? definePierCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? defineBeamCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? defineBearingCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? defineBridgeTypeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? importAllDefinitionsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? exportAllDefinitionsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<RoadProjectItem?>? editComponentsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<RoadProjectItem?>? exportComponentsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<RoadProjectItem?>? deleteComponentsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? exportAllRouteConfigurationsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? importAllRouteConfigurationsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? refreshCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	public ObservableCollection<RoadProjectItem> RoadProjects { get; } = new ObservableCollection<RoadProjectItem>();

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public RoadProjectItem? SelectedRoadProject
	{
		get
		{
			return _selectedRoadProject;
		}
		set
		{
			if (!EqualityComparer<RoadProjectItem>.Default.Equals(_selectedRoadProject, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedRoadProject);
				_selectedRoadProject = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedRoadProject);
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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DefinePileCommand => definePileCommand ?? (definePileCommand = new RelayCommand(DefinePile));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DefineFoundationCommand => defineFoundationCommand ?? (defineFoundationCommand = new RelayCommand(DefineFoundation));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DefinePierCommand => definePierCommand ?? (definePierCommand = new RelayCommand(DefinePier));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DefineBeamCommand => defineBeamCommand ?? (defineBeamCommand = new RelayCommand(DefineBeam));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DefineBearingCommand => defineBearingCommand ?? (defineBearingCommand = new RelayCommand(DefineBearing));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DefineBridgeTypeCommand => defineBridgeTypeCommand ?? (defineBridgeTypeCommand = new RelayCommand(DefineBridgeType));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ImportAllDefinitionsCommand => importAllDefinitionsCommand ?? (importAllDefinitionsCommand = new RelayCommand(ImportAllDefinitions));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ExportAllDefinitionsCommand => exportAllDefinitionsCommand ?? (exportAllDefinitionsCommand = new RelayCommand(ExportAllDefinitions));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<RoadProjectItem?> EditComponentsCommand => editComponentsCommand ?? (editComponentsCommand = new RelayCommand<RoadProjectItem>(EditComponents));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<RoadProjectItem?> ExportComponentsCommand => exportComponentsCommand ?? (exportComponentsCommand = new AsyncRelayCommand<RoadProjectItem>(ExportComponents));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<RoadProjectItem?> DeleteComponentsCommand => deleteComponentsCommand ?? (deleteComponentsCommand = new RelayCommand<RoadProjectItem>(DeleteComponents));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ExportAllRouteConfigurationsCommand => exportAllRouteConfigurationsCommand ?? (exportAllRouteConfigurationsCommand = new AsyncRelayCommand(ExportAllRouteConfigurations));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ImportAllRouteConfigurationsCommand => importAllRouteConfigurationsCommand ?? (importAllRouteConfigurationsCommand = new AsyncRelayCommand(ImportAllRouteConfigurations));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RefreshCommand => refreshCommand ?? (refreshCommand = new RelayCommand(Refresh));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public CreateBridgeComponentViewModel()
	{
		try
		{
			IServiceProvider services = UIBootstrapper.Services;
			_roadProjectManager = services.GetService<IRoadProjectManager>();
			_revitAdapter = ServiceProvider.GetModuleLoader()?.RevitAdapter;
			LoadRoadProjects();
			if (_roadProjectManager == null)
			{
				Logger.Warning("创建桥梁部件: 无法获取道路项目管理器");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("初始化创建桥梁部件失败", ex);
		}
	}

	private void LoadRoadProjects()
	{
		try
		{
			if (_roadProjectManager == null)
			{
				Logger.Warning("道路项目管理器不可用");
				return;
			}
			RoadProjects.Clear();
			foreach (RoadProject allProject in _roadProjectManager.GetAllProjects())
			{
				RoadProjects.Add(new RoadProjectItem(allProject));
			}
			StatusMessage = $"已加载 {RoadProjects.Count} 条路线";
			Logger.Info($"成功加载 {RoadProjects.Count} 条道路路线");
		}
		catch (Exception ex)
		{
			Logger.Error("加载道路项目列表失败", ex);
			StatusMessage = "加载路线失败";
		}
	}

	[RelayCommand]
	private void DefinePile()
	{
		try
		{
			Logger.Info("打开定义桩部件窗口");
			UIBootstrapper.Services.GetRequiredService<IWindowManager>().ShowDefinePileWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("打开定义桩部件窗口失败", ex);
			MessageBox.Show("打开窗口失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void DefineFoundation()
	{
		try
		{
			Logger.Info("打开定义基础部件窗口");
			UIBootstrapper.Services.GetRequiredService<IWindowManager>().ShowDefineFoundationWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("打开定义基础部件窗口失败", ex);
			MessageBox.Show("打开窗口失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void DefinePier()
	{
		try
		{
			Logger.Info("打开定义墩柱窗口");
			UIBootstrapper.Services.GetRequiredService<IWindowManager>().ShowDefinePierWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("打开定义墩柱窗口失败", ex);
			MessageBox.Show("打开窗口失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void DefineBeam()
	{
		try
		{
			Logger.Info("打开定义盖梁窗口");
			UIBootstrapper.Services.GetRequiredService<IWindowManager>().ShowDefineBeamWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("打开定义盖梁窗口失败", ex);
			MessageBox.Show("打开窗口失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void DefineBearing()
	{
		try
		{
			Logger.Info("打开定义支座窗口");
			UIBootstrapper.Services.GetRequiredService<IWindowManager>().ShowDefineBearingWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("打开定义支座窗口失败", ex);
			MessageBox.Show("打开窗口失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void DefineBridgeType()
	{
		try
		{
			Logger.Info("打开定义桥型窗口");
			UIBootstrapper.Services.GetRequiredService<IWindowManager>().ShowDefineBridgeTypeWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("打开定义桥型窗口失败", ex);
			MessageBox.Show("打开窗口失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void ImportAllDefinitions()
	{
		try
		{
			Logger.Info("导入所有桥梁部件定义");
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "导入桥梁部件定义",
				Filter = "JSON文件|*.json|所有文件|*.*"
			};
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			string json = File.ReadAllText(openFileDialog.FileName);
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};
			using JsonDocument jsonDocument = JsonDocument.Parse(json);
			JsonElement rootElement = jsonDocument.RootElement;
			if (rootElement.TryGetProperty("version", out var value))
			{
				string text = value.GetString();
				Logger.Info("导入文件版本: " + text);
			}
			if (rootElement.TryGetProperty("components", out var value2))
			{
				int num = 0;
				if (value2.TryGetProperty("piles", out var value3) && value3.ValueKind == JsonValueKind.Array)
				{
					List<UserDefinedBridgeComponent> list = JsonSerializer.Deserialize<List<UserDefinedBridgeComponent>>(value3.GetRawText(), options);
					if (list != null && list.Count > 0)
					{
						BridgeComponentDefinitionStore.Instance.SetComponents("桩部件", list);
						num += list.Count;
						Logger.Info($"导入桩部件: {list.Count} 个");
					}
				}
				if (value2.TryGetProperty("foundations", out var value4) && value4.ValueKind == JsonValueKind.Array)
				{
					List<UserDefinedBridgeComponent> list2 = JsonSerializer.Deserialize<List<UserDefinedBridgeComponent>>(value4.GetRawText(), options);
					if (list2 != null && list2.Count > 0)
					{
						BridgeComponentDefinitionStore.Instance.SetComponents("基础部件", list2);
						num += list2.Count;
						Logger.Info($"导入基础部件: {list2.Count} 个");
					}
				}
				if (value2.TryGetProperty("piers", out var value5) && value5.ValueKind == JsonValueKind.Array)
				{
					List<UserDefinedBridgeComponent> list3 = JsonSerializer.Deserialize<List<UserDefinedBridgeComponent>>(value5.GetRawText(), options);
					if (list3 != null && list3.Count > 0)
					{
						BridgeComponentDefinitionStore.Instance.SetComponents("墩柱", list3);
						num += list3.Count;
						Logger.Info($"导入墩柱: {list3.Count} 个");
					}
				}
				if (value2.TryGetProperty("beams", out var value6) && value6.ValueKind == JsonValueKind.Array)
				{
					List<UserDefinedBridgeComponent> list4 = JsonSerializer.Deserialize<List<UserDefinedBridgeComponent>>(value6.GetRawText(), options);
					if (list4 != null && list4.Count > 0)
					{
						BridgeComponentDefinitionStore.Instance.SetComponents("盖梁", list4);
						num += list4.Count;
						Logger.Info($"导入盖梁: {list4.Count} 个");
					}
				}
				if (value2.TryGetProperty("bearings", out var value7) && value7.ValueKind == JsonValueKind.Array)
				{
					List<UserDefinedBridgeComponent> list5 = JsonSerializer.Deserialize<List<UserDefinedBridgeComponent>>(value7.GetRawText(), options);
					if (list5 != null && list5.Count > 0)
					{
						BridgeComponentDefinitionStore.Instance.SetComponents("支座", list5);
						num += list5.Count;
						Logger.Info($"导入支座: {list5.Count} 个");
					}
				}
				if (value2.TryGetProperty("bridgeTypes", out var value8) && value8.ValueKind == JsonValueKind.Array)
				{
					List<UserDefinedBridgeComponent> list6 = JsonSerializer.Deserialize<List<UserDefinedBridgeComponent>>(value8.GetRawText(), options);
					if (list6 != null && list6.Count > 0)
					{
						BridgeComponentDefinitionStore.Instance.SetComponents("桥型", list6);
						num += list6.Count;
						Logger.Info($"导入桥型: {list6.Count} 个");
					}
				}
				if (num > 0)
				{
					MessageBox.Show($"导入成功！\n\n文件位置：{openFileDialog.FileName}\n\n总计导入 {num} 个部件定义\n\n提示：导入的部件已保存到内存中，您可以打开各个定义窗口查看。", "导入成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
					Logger.Info($"桥梁部件定义导入成功: {openFileDialog.FileName}，总计 {num} 个部件");
				}
				else
				{
					MessageBox.Show("文件中没有找到任何部件定义。", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			}
			else
			{
				MessageBox.Show("文件格式不正确，缺少 components 数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
		catch (JsonException ex)
		{
			Logger.Error("导入文件格式错误", ex);
			MessageBox.Show("文件格式错误：" + ex.Message + "\n\n请确保选择的是有效的桥梁部件定义文件。", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		catch (Exception ex2)
		{
			Logger.Error("导入所有定义失败", ex2);
			MessageBox.Show("导入失败: " + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void ExportAllDefinitions()
	{
		try
		{
			Logger.Info("导出所有桥梁部件定义");
			BridgeComponentDefinitions allDefinitions = BridgeComponentDefinitionStore.Instance.GetAllDefinitions();
			int totalCount = allDefinitions.TotalCount;
			if (totalCount == 0)
			{
				MessageBox.Show("当前还没有定义任何部件，请先在各个定义窗口中添加部件。", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Title = "导出桥梁部件定义",
				Filter = "JSON文件|*.json|所有文件|*.*",
				FileName = $"桥梁部件定义_{DateTime.Now:yyyyMMdd_HHmmss}.json"
			};
			if (saveFileDialog.ShowDialog() == true)
			{
				JsonSerializerOptions options = new JsonSerializerOptions
				{
					WriteIndented = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				};
				string contents = JsonSerializer.Serialize(new
				{
					ExportTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
					Version = "1.0",
					Summary = new
					{
						TotalComponents = totalCount,
						PilesCount = allDefinitions.Piles.Count,
						FoundationsCount = allDefinitions.Foundations.Count,
						PiersCount = allDefinitions.Piers.Count,
						BeamsCount = allDefinitions.Beams.Count,
						BearingsCount = allDefinitions.Bearings.Count,
						BridgeTypesCount = allDefinitions.BridgeTypes.Count
					},
					Components = new
					{
						Piles = allDefinitions.Piles.ToList(),
						Foundations = allDefinitions.Foundations.ToList(),
						Piers = allDefinitions.Piers.ToList(),
						Beams = allDefinitions.Beams.ToList(),
						Bearings = allDefinitions.Bearings.ToList(),
						BridgeTypes = allDefinitions.BridgeTypes.ToList()
					}
				}, options);
				File.WriteAllText(saveFileDialog.FileName, contents);
				MessageBox.Show($"导出成功！\n\n文件位置：{saveFileDialog.FileName}\n\n统计信息：\n- 桩部件：{allDefinitions.Piles.Count} 个\n- 基础部件：{allDefinitions.Foundations.Count} 个\n- 墩柱：{allDefinitions.Piers.Count} 个\n- 盖梁：{allDefinitions.Beams.Count} 个\n- 支座：{allDefinitions.Bearings.Count} 个\n- 桥型：{allDefinitions.BridgeTypes.Count} 个\n- 总计：{totalCount} 个", "导出成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				Logger.Info($"桥梁部件定义已导出到: {saveFileDialog.FileName}，总计 {totalCount} 个部件");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("导出所有定义失败", ex);
			MessageBox.Show("导出失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void EditComponents(RoadProjectItem? item)
	{
		try
		{
			if (item == null)
			{
				MessageBox.Show("请先选择一条路线", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			Logger.Info("编辑路线 [" + item.Name + "] 的桥梁部件配置");
			UIBootstrapper.Services.GetRequiredService<IWindowManager>().ShowEditBridgeComponentsWindow(item.Project);
		}
		catch (Exception ex)
		{
			Logger.Error("编辑部件配置失败", ex);
			MessageBox.Show("打开编辑窗口失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private async Task ExportComponents(RoadProjectItem? item)
	{
		try
		{
			if (item == null)
			{
				MessageBox.Show("请先选择一条路线", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			Logger.Info("导出路线 [" + item.Name + "] 的桥梁部件配置");
			if (item.Project == null)
			{
				MessageBox.Show("路线项目数据为空，无法导出。", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			if (item.Project.BridgeComponentConfigurations == null || item.Project.BridgeComponentConfigurations.IsEmpty)
			{
				MessageBox.Show("该路线还没有配置任何桥梁部件。\n\n请先点击\"编辑部件\"按钮进行配置。", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Title = "导出路线部件配置",
				Filter = "路线配置文件 (*.routeconfig)|*.routeconfig|所有文件|*.*",
				FileName = $"{item.Name}_部件配置_{DateTime.Now:yyyyMMdd_HHmmss}.routeconfig"
			};
			if (saveFileDialog.ShowDialog() != true)
			{
				return;
			}
			IsLoading = true;
			Guid guid = item.Project?.Id ?? Guid.Empty;
			if (guid == Guid.Empty)
			{
				MessageBox.Show("项目 ID 无效", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsLoading = false;
				return;
			}
			if (_roadProjectManager == null)
			{
				MessageBox.Show("道路项目管理器未初始化", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				IsLoading = false;
				return;
			}
			Result<bool> result = await _roadProjectManager.ExportProjectAsync(guid, saveFileDialog.FileName);
			IsLoading = false;
			if (result.IsSuccess)
			{
				int valueOrDefault = (item.Project?.BridgeComponentConfigurations?.Count).GetValueOrDefault();
				MessageBox.Show($"导出成功！\n\n文件位置：{saveFileDialog.FileName}\n\n路线名称：{item.Name}\n配置数量：{valueOrDefault} 个桩位", "导出成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				Logger.Info($"路线 [{item.Name}] 部件配置已导出到: {saveFileDialog.FileName}，共 {valueOrDefault} 个桩位");
			}
			else
			{
				MessageBox.Show("导出失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("导出路线部件配置失败", ex);
			MessageBox.Show("导出失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			IsLoading = false;
		}
	}

	[RelayCommand]
	private void DeleteComponents(RoadProjectItem? item)
	{
		try
		{
			if (item == null)
			{
				MessageBox.Show("请先选择一条路线", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			else if (MessageBox.Show("确定要删除路线 [" + item.Name + "] 的桥梁部件配置吗？\n\n此操作不可撤销。", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Logger.Info("删除路线 [" + item.Name + "] 的桥梁部件配置");
				MessageBox.Show("删除路线 [" + item.Name + "] 的桥梁部件配置功能待实现", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("删除部件配置失败", ex);
			MessageBox.Show("删除失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private async Task ExportAllRouteConfigurations()
	{
		try
		{
			Logger.Info("导出所有路线的桥梁部件配置");
			if (RoadProjects.Count == 0)
			{
				MessageBox.Show("当前还没有任何路线项目。", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			List<RoadProject> allProjects = RoadProjects.Select((RoadProjectItem rp) => rp.Project).ToList();
			int configCount = allProjects.Count((RoadProject p) => p.BridgeComponentConfigurations != null && p.BridgeComponentConfigurations.Count > 0);
			if (configCount == 0)
			{
				MessageBox.Show("当前所有路线都还没有配置桥梁部件。", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Title = "导出所有路线配置",
				Filter = "路线配置文件 (*.routeconfig)|*.routeconfig|所有文件|*.*",
				FileName = $"所有路线配置_{DateTime.Now:yyyyMMdd_HHmmss}.routeconfig"
			};
			if (saveFileDialog.ShowDialog() != true)
			{
				return;
			}
			IsLoading = true;
			StatusMessage = "正在导出...";
			Result<bool> result = await RoadProjectRepository.ExportAllProjectsAsync(allProjects, saveFileDialog.FileName);
			IsLoading = false;
			if (result.IsSuccess)
			{
				int value = allProjects.Sum((RoadProject p) => p.BridgeComponentConfigurations?.Count ?? 0);
				string messageBoxText = $"导出成功！\n\n文件位置：{saveFileDialog.FileName}\n\n已导出 {RoadProjects.Count} 条路线\n有配置的路线：{configCount} 条\n总计桩位配置：{value} 个";
				StatusMessage = $"导出成功：{RoadProjects.Count} 条路线";
				MessageBox.Show(messageBoxText, "导出成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				Logger.Info("所有路线配置已导出到: " + saveFileDialog.FileName);
			}
			else
			{
				StatusMessage = "导出失败";
				MessageBox.Show("导出失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("导出所有路线配置失败", ex);
			MessageBox.Show("导出失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			IsLoading = false;
			StatusMessage = "导出失败";
		}
	}

	[RelayCommand]
	private async Task ImportAllRouteConfigurations()
	{
		try
		{
			Logger.Info("导入所有路线的桥梁部件配置");
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "导入路线配置",
				Filter = "路线配置文件 (*.routeconfig)|*.routeconfig|所有文件|*.*"
			};
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			IsLoading = true;
			string json = await Task.Run(() => File.ReadAllText(openFileDialog.FileName));
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};
			List<RoadProject> list;
			try
			{
				list = JsonSerializer.Deserialize<List<RoadProject>>(json, options);
			}
			catch
			{
				RoadProject roadProject = JsonSerializer.Deserialize<RoadProject>(json, options);
				list = ((roadProject == null) ? null : new List<RoadProject> { roadProject });
			}
			IsLoading = false;
			if (list != null && list.Count > 0)
			{
				int num = 0;
				int num2 = 0;
				foreach (RoadProject item in list)
				{
					RoadProject roadProject2 = _roadProjectManager?.GetProjectByName(item.Name);
					if (roadProject2 != null)
					{
						roadProject2.BridgeComponentConfigurations = item.BridgeComponentConfigurations;
						roadProject2.UpdatedAt = DateTime.Now;
						num++;
						if (item.BridgeComponentConfigurations != null)
						{
							num2 += item.BridgeComponentConfigurations.Count;
						}
						continue;
					}
					if (_roadProjectManager == null)
					{
						MessageBox.Show("道路项目管理器未初始化", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
						continue;
					}
					Result<RoadProject> result = _roadProjectManager.AddProject(item.Name, item.Description);
					if (result.IsSuccess && result.Value != null)
					{
						result.Value.BridgeComponentConfigurations = item.BridgeComponentConfigurations;
						num++;
						if (item.BridgeComponentConfigurations != null)
						{
							num2 += item.BridgeComponentConfigurations.Count;
						}
					}
				}
				LoadRoadProjects();
				MessageBox.Show($"导入成功！\n\n文件位置：{openFileDialog.FileName}\n\n已导入 {num} 条路线的配置\n总计 {num2} 个桩位部件", "导入成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				Logger.Info($"路线配置导入成功: {openFileDialog.FileName}，导入 {num} 条路线");
			}
			else
			{
				MessageBox.Show("文件格式不正确，无法解析路线配置。", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("导入所有路线配置失败", ex);
			MessageBox.Show("导入失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			IsLoading = false;
		}
	}

	[RelayCommand]
	private void Refresh()
	{
		LoadRoadProjects();
	}

	[RelayCommand]
	private void Close()
	{
	}
}
