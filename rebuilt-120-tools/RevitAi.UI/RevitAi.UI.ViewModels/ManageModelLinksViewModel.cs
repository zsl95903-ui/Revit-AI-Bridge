using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.LinkManagement;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace RevitAi.UI.ViewModels;

public class ManageModelLinksViewModel : ObservableObject
{
	private readonly ILinkService? _linkService;

	private readonly object? _document;

	private readonly IRevitAdapter? _revitAdapter;

	private readonly object? _linkManagementExternalEvent;

	[ObservableProperty]
	private ModelLinkInfo? selectedLink;

	private bool _selectAll;

	private static int linkInstanceId = 1;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? reloadAllCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? reloadSelectedCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? unloadSelectedCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? deleteSelectedCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<ModelLinkInfo?>? selectFilePathCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? saveChangesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? refreshCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	public ObservableCollection<ModelLinkInfo> ModelLinks { get; } = new ObservableCollection<ModelLinkInfo>();

	public int LinkCount => ModelLinks.Count;

	public IList<ModelLinkInfo> SelectedLinks => ModelLinks.Where((ModelLinkInfo x) => x.IsSelected).ToList();

	public bool SelectAll
	{
		get
		{
			return _selectAll;
		}
		set
		{
			if (!SetProperty(ref _selectAll, value, "SelectAll"))
			{
				return;
			}
			foreach (ModelLinkInfo modelLink in ModelLinks)
			{
				modelLink.IsSelected = value;
			}
		}
	}

	public int SelectedCount => ModelLinks.Count((ModelLinkInfo x) => x.IsSelected);

	public Action? OnRequestClose { get; set; }

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ModelLinkInfo? SelectedLink
	{
		get
		{
			return selectedLink;
		}
		set
		{
			if (!EqualityComparer<ModelLinkInfo>.Default.Equals(selectedLink, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedLink);
				selectedLink = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedLink);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ReloadAllCommand => reloadAllCommand ?? (reloadAllCommand = new RelayCommand(ReloadAll));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ReloadSelectedCommand => reloadSelectedCommand ?? (reloadSelectedCommand = new RelayCommand(ReloadSelected));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand UnloadSelectedCommand => unloadSelectedCommand ?? (unloadSelectedCommand = new RelayCommand(UnloadSelected));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand DeleteSelectedCommand => deleteSelectedCommand ?? (deleteSelectedCommand = new RelayCommand(DeleteSelected));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<ModelLinkInfo?> SelectFilePathCommand => selectFilePathCommand ?? (selectFilePathCommand = new RelayCommand<ModelLinkInfo>(SelectFilePath));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SaveChangesCommand => saveChangesCommand ?? (saveChangesCommand = new RelayCommand(SaveChanges));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RefreshCommand => refreshCommand ?? (refreshCommand = new RelayCommand(Refresh));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public ManageModelLinksViewModel()
	{
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader()?.RevitAdapter;
			if (revitAdapter != null)
			{
				_revitAdapter = revitAdapter;
				_linkService = revitAdapter.LinkService;
				_document = revitAdapter.GetActiveDocument();
				try
				{
					MethodInfo method = revitAdapter.GetType().GetMethod("GetLinkManagementExternalEvent");
					if (method != null)
					{
						_linkManagementExternalEvent = method.Invoke(revitAdapter, null);
						if (_linkManagementExternalEvent == null)
						{
							Logger.Warning("GetLinkManagementExternalEvent 返回 null");
						}
					}
					else
					{
						Logger.Warning("未找到 GetLinkManagementExternalEvent 方法");
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("获取 LinkManagement ExternalEvent 失败: " + ex.Message);
				}
			}
			if (_linkService == null || _document == null)
			{
				Logger.Warning($"管理链接模型: LinkService={_linkService != null}, Document={_document != null}");
				return;
			}
			Task.Run(delegate
			{
				try
				{
					LoadLinkModelsFromRevit();
				}
				catch (Exception ex3)
				{
					Logger.Error("异步加载链接模型失败", ex3);
				}
			});
		}
		catch (Exception ex2)
		{
			Logger.Error("初始化管理链接模型失败", ex2);
		}
	}

	private void LoadLinkModelsFromRevit()
	{
		try
		{
			if (_linkService == null || _document == null)
			{
				Logger.Error("无法加载链接模型：服务或文档不可用");
				return;
			}
			IEnumerable<object> allLinkInstances = _linkService.GetAllLinkInstances(_document);
			if (allLinkInstances == null)
			{
				Logger.Warning("未找到任何链接实例");
				return;
			}
			List<ModelLinkInfo> collectedLinks = new List<ModelLinkInfo>();
			foreach (object item in allLinkInstances)
			{
				try
				{
					(double, double, double)? linkPosition = _linkService.GetLinkPosition(item);
					double? linkRotation = _linkService.GetLinkRotation(item);
					object linkTypeFromInstance = _linkService.GetLinkTypeFromInstance(item);
					if (linkTypeFromInstance == null)
					{
						Logger.Warning("无法从链接实例获取链接类型");
						continue;
					}
					(string, string, bool, int?)? linkInfo = _linkService.GetLinkInfo(linkTypeFromInstance);
					if (!linkInfo.HasValue)
					{
						Logger.Warning("无法获取链接信息");
						continue;
					}
					ModelLinkInfo modelLinkInfo = new ModelLinkInfo();
					modelLinkInfo.InitializeAllValues(linkInstanceId++.ToString(), linkInfo.Value.Item2 ?? "未命名", linkInfo.Value.Item1 ?? "", linkInfo.Value.Item3 ? "已载入" : "已卸载", linkInfo.Value.Item3, linkInfo.Value.Item3, Math.Round(linkPosition?.Item1 ?? 0.0, 3), Math.Round(linkPosition?.Item2 ?? 0.0, 3), Math.Round(linkPosition?.Item3 ?? 0.0, 3), Math.Round(linkRotation.GetValueOrDefault(), 2), item, linkTypeFromInstance, linkInfo.Value.Item4.GetValueOrDefault());
					collectedLinks.Add(modelLinkInfo);
				}
				catch (Exception ex)
				{
					Logger.Error("加载链接实例失败: " + ex.Message);
				}
			}
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				ModelLinks.Clear();
				foreach (ModelLinkInfo item2 in collectedLinks)
				{
					item2.PropertyChanged += delegate(object? s, PropertyChangedEventArgs e)
					{
						if (e.PropertyName == "IsSelected")
						{
							OnPropertyChanged("SelectedCount");
						}
					};
					ModelLinks.Add(item2);
				}
				OnPropertyChanged("LinkCount");
				Logger.Info($"已加载 {ModelLinks.Count} 个链接模型");
			});
		}
		catch (Exception ex2)
		{
			Logger.Error("加载链接模型失败", ex2);
		}
	}

	[RelayCommand]
	private void ReloadAll()
	{
		try
		{
			if (ModelLinks.Count == 0)
			{
				MessageBox.Show("当前没有链接模型", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			else if (_linkService == null || _document == null)
			{
				MessageBox.Show("无法获取 Revit 服务或文档", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			else
			{
				if (MessageBox.Show($"确定要重新载入所有 {ModelLinks.Count} 个链接模型吗？\n\n此操作可能需要一些时间。", "管理链接模型", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
				{
					return;
				}
				int num = 0;
				int num2 = 0;
				foreach (ModelLinkInfo item in ModelLinks.ToList())
				{
					if (item.LinkTypeId <= 0)
					{
						continue;
					}
					try
					{
						if (_linkService.ReloadLink(_document, item.LinkTypeId))
						{
							num++;
							item.IsLoaded = true;
							item.Status = "已载入";
						}
						else
						{
							num2++;
						}
					}
					catch (Exception ex)
					{
						Logger.Error("重新载入链接失败 (" + item.Name + "): " + ex.Message);
						num2++;
					}
				}
				MessageBox.Show($"重新载入完成\n成功: {num} 个\n失败: {num2} 个", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				Logger.Info($"重新载入完成: 成功 {num}, 失败 {num2}");
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("重新载入全部失败", ex2);
			MessageBox.Show("重新载入失败: " + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void ReloadSelected()
	{
		try
		{
			List<ModelLinkInfo> list = SelectedLinks.ToList();
			if (list.Count == 0)
			{
				MessageBox.Show("请先勾选要重新载入的链接模型", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if (_linkService == null || _document == null)
			{
				MessageBox.Show("无法获取 Revit 服务或文档", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			int num = 0;
			int num2 = 0;
			foreach (ModelLinkInfo item in list)
			{
				if (item.LinkTypeId > 0)
				{
					try
					{
						Logger.Info($"正在重新载入链接: {item.Name} (LinkTypeId: {item.LinkTypeId})");
						if (_linkService.ReloadLink(_document, item.LinkTypeId))
						{
							num++;
							item.IsLoaded = true;
							item.Status = "已载入";
							Logger.Info("重新载入成功: " + item.Name);
						}
						else
						{
							num2++;
							Logger.Warning($"重新载入失败（服务返回 false）: {item.Name} (LinkTypeId: {item.LinkTypeId})");
						}
					}
					catch (Exception ex)
					{
						num2++;
						Logger.Error($"重新载入链接异常 ({item.Name}, LinkTypeId: {item.LinkTypeId}): {ex.Message}\n{ex.StackTrace}");
					}
				}
				else
				{
					num2++;
					Logger.Warning($"跳过重新载入（LinkTypeId 无效）: {item.Name} (LinkTypeId: {item.LinkTypeId})");
				}
			}
			MessageBox.Show($"重新载入完成\n成功: {num} 个\n失败: {num2} 个", "管理链接模型", MessageBoxButton.OK, (num > 0) ? MessageBoxImage.Asterisk : MessageBoxImage.Exclamation);
			Logger.Info($"重新载入完成: 成功 {num}, 失败 {num2}");
		}
		catch (Exception ex2)
		{
			Logger.Error("重新载入选中失败", ex2);
			MessageBox.Show("重新载入失败: " + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void UnloadSelected()
	{
		try
		{
			List<ModelLinkInfo> list = SelectedLinks.ToList();
			if (list.Count == 0)
			{
				MessageBox.Show("请先勾选要卸载的链接模型", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if (_linkService == null || _document == null)
			{
				MessageBox.Show("无法获取 Revit 服务或文档", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			List<ModelLinkInfo> list2 = list.Where((ModelLinkInfo x) => x.IsLoaded).ToList();
			if (list2.Count == 0)
			{
				MessageBox.Show("所选链接模型都已经是卸载状态", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			else
			{
				if (MessageBox.Show($"确定要卸载 {list2.Count} 个链接模型吗？\n\n卸载后可以随时重新载入。", "管理链接模型", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
				{
					return;
				}
				int num = 0;
				int num2 = 0;
				foreach (ModelLinkInfo item in list2)
				{
					if (item.LinkTypeId > 0)
					{
						try
						{
							Logger.Info("卸载链接模型: " + item.Name);
							if (_linkService.UnloadLink(_document, item.LinkTypeId))
							{
								num++;
								item.IsLoaded = false;
								item.Status = "已卸载";
							}
							else
							{
								num2++;
							}
						}
						catch (Exception ex)
						{
							Logger.Error("卸载链接失败 (" + item.Name + "): " + ex.Message);
							num2++;
						}
					}
					else
					{
						num2++;
					}
				}
				MessageBox.Show($"卸载完成\n成功: {num} 个\n失败: {num2} 个", "管理链接模型", MessageBoxButton.OK, (num > 0) ? MessageBoxImage.Asterisk : MessageBoxImage.Exclamation);
				Logger.Info($"卸载完成: 成功 {num}, 失败 {num2}");
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("卸载选中失败", ex2);
			MessageBox.Show("卸载失败: " + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void DeleteSelected()
	{
		try
		{
			List<ModelLinkInfo> list = SelectedLinks.ToList();
			if (list.Count == 0)
			{
				MessageBox.Show("请先勾选要删除的链接模型", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			else if (_linkManagementExternalEvent == null)
			{
				MessageBox.Show("无法获取链接管理 ExternalEvent", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Hand);
				Logger.Error("DeleteSelected: ExternalEvent 为 null");
			}
			else
			{
				if (MessageBox.Show($"确定要从项目中删除选中的 {list.Count} 个链接模型吗？\n\n此操作不可撤销！", "管理链接模型", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
				{
					return;
				}
				{
					foreach (ModelLinkInfo link in list.ToList())
					{
						if (link.LinkInstance != null)
						{
							try
							{
								Logger.Info($"正在删除链接: {link.Name} (LinkTypeId: {link.LinkTypeId})");
								LinkManagementExternalEventRequest value = new LinkManagementExternalEventRequest
								{
									Operation = LinkManagementOperation.Delete,
									Document = _document,
									LinkInstance = link.LinkInstance,
									OnCompleted = delegate(Exception? ex3, bool success)
									{
										((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
										{
											if (success)
											{
												ModelLinks.Remove(link);
												OnPropertyChanged("LinkCount");
												Logger.Info("删除成功: " + link.Name);
											}
											else
											{
												Logger.Error("删除失败: " + link.Name + ", " + ex3?.Message);
											}
										});
									}
								};
								Assembly assembly = _revitAdapter?.GetType().Assembly;
								Type type = null;
								Logger.Info("RevitAdapter Assembly: " + assembly?.GetName().FullName);
								if (assembly != null)
								{
									Type[] types = assembly.GetTypes();
									Logger.Info($"Assembly contains {types.Length} types");
									type = assembly.GetTypes().FirstOrDefault((Type t) => t.Name == "LinkManagementExternalEventHandler");
									Logger.Info($"Handler found in RevitAdapter assembly: {type != null}");
								}
								if (type == null)
								{
									Logger.Info("Searching in all loaded assemblies...");
									Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
									foreach (Assembly assembly2 in assemblies)
									{
										try
										{
											type = assembly2.GetType("RevitAi.Revit.LinkManagement.LinkManagementExternalEventHandler");
											if (type != null)
											{
												Logger.Info("Found handler in assembly: " + assembly2.GetName().FullName);
												break;
											}
										}
										catch
										{
										}
									}
								}
								if (type != null)
								{
									type.GetProperty("CurrentRequest", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, value);
									(_linkManagementExternalEvent?.GetType().GetMethod("Raise"))?.Invoke(_linkManagementExternalEvent, null);
									Logger.Info("成功触发 ExternalEvent 删除操作");
								}
								else
								{
									Logger.Error("无法找到 LinkManagementExternalEventHandler 类型");
									MessageBox.Show("无法找到链接管理处理器，请确保 Revit 适配层已正确加载", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
								}
							}
							catch (Exception ex)
							{
								Logger.Error($"删除链接异常 ({link.Name}, LinkTypeId: {link.LinkTypeId}): {ex.Message}\n{ex.StackTrace}");
							}
						}
						else
						{
							Logger.Warning("跳过删除（LinkInstance 为 null）: " + link.Name);
						}
					}
					return;
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("删除选中失败", ex2);
			MessageBox.Show("删除失败: " + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void SelectFilePath(ModelLinkInfo? linkInfo)
	{
		try
		{
			if (linkInfo == null)
			{
				MessageBox.Show("无法选择文件：链接信息为空", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择链接文件",
				Filter = "Revit 文件 (*.rvt)|*.rvt|所有文件 (*.*)|*.*",
				RestoreDirectory = true,
				CheckFileExists = true,
				CheckPathExists = true
			};
			if (!string.IsNullOrEmpty(linkInfo.FilePath))
			{
				try
				{
					string directoryName = Path.GetDirectoryName(linkInfo.FilePath);
					if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName))
					{
						openFileDialog.InitialDirectory = directoryName;
					}
					openFileDialog.FileName = Path.GetFileName(linkInfo.FilePath);
				}
				catch
				{
				}
			}
			if (openFileDialog.ShowDialog() == true)
			{
				string fileName = openFileDialog.FileName;
				if (MessageBox.Show("确定要将链接路径更改为：\n\n" + fileName + "\n\n点击「是」保存更改，点击「否」取消。", "确认更改链接路径", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					linkInfo.FilePath = fileName;
					Logger.Info("已更改链接路径: " + linkInfo.Name + " -> " + fileName);
					MessageBox.Show($"链接路径已更新\n\n名称: {linkInfo.Name}\n新路径: {fileName}\n\n提示：请点击「保存修改」按钮以应用更改。", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("选择文件路径失败", ex);
			MessageBox.Show("选择文件失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void SaveChanges()
	{
		try
		{
			if (_linkService == null || _document == null)
			{
				MessageBox.Show("无法获取 Revit 服务或文档", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			List<ModelLinkInfo> list = ModelLinks.Where((ModelLinkInfo l) => l.HasUnsavedChanges).ToList();
			if (list.Count == 0)
			{
				MessageBox.Show("没有需要保存的修改", "管理链接模型", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			foreach (ModelLinkInfo item in list.ToList())
			{
				if (item.LinkInstance == null)
				{
					continue;
				}
				try
				{
					if (!item.IsPathChanged)
					{
						goto IL_0128;
					}
					Logger.Info("替换链接路径: " + item.Name + " -> " + item.FilePath);
					if (_linkService.ReplaceLinkPath(_document, item.LinkInstance, item.FilePath))
					{
						num9++;
						item.OriginalFilePath = item.FilePath;
						goto IL_0128;
					}
					num10++;
					goto end_IL_00bb;
					IL_0128:
					if (item.IsLoadedDesired != item.IsLoaded)
					{
						if (item.IsLoadedDesired)
						{
							if (_linkService.ReloadLink(_document, item.LinkTypeId))
							{
								num5++;
								item.IsLoaded = true;
								item.Status = "已载入";
							}
							else
							{
								num6++;
							}
						}
						else if (_linkService.UnloadLink(_document, item.LinkTypeId))
						{
							num7++;
							item.IsLoaded = false;
							item.Status = "已卸载";
						}
						else
						{
							num8++;
						}
					}
					if (item.IsLoaded && item.IsPositionChanged)
					{
						if (item.PositionSuccessfullyUpdated = _linkService.UpdateLinkPosition(_document, item.LinkInstance, item.PositionX, item.PositionY, item.PositionZ))
						{
							num++;
						}
						else
						{
							num2++;
						}
					}
					else
					{
						item.PositionSuccessfullyUpdated = true;
					}
					if (item.IsLoaded && item.IsRotationChanged)
					{
						if (item.RotationSuccessfullyUpdated = _linkService.UpdateLinkRotation(_document, item.LinkInstance, item.Rotation))
						{
							num3++;
						}
						else
						{
							num4++;
						}
					}
					else
					{
						item.RotationSuccessfullyUpdated = true;
					}
					end_IL_00bb:;
				}
				catch (Exception ex)
				{
					Logger.Error("保存链接失败 (" + item.Name + "): " + ex.Message);
					num2++;
					num4++;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("保存完成");
			StringBuilder stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler;
			if (num9 > 0 || num10 > 0)
			{
				stringBuilder.AppendLine();
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder3 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(16, 2, stringBuilder2);
				handler.AppendLiteral("路径替换：成功 ");
				handler.AppendFormatted(num9);
				handler.AppendLiteral(" 个，失败 ");
				handler.AppendFormatted(num10);
				handler.AppendLiteral(" 个");
				stringBuilder3.AppendLine(ref handler);
			}
			if (num5 > 0 || num6 > 0 || num7 > 0 || num8 > 0)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("载入状态：");
				if (num5 > 0)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder4 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
					handler.AppendLiteral("  载入成功 ");
					handler.AppendFormatted(num5);
					handler.AppendLiteral(" 个");
					stringBuilder4.AppendLine(ref handler);
				}
				if (num6 > 0)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder5 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
					handler.AppendLiteral("  载入失败 ");
					handler.AppendFormatted(num6);
					handler.AppendLiteral(" 个");
					stringBuilder5.AppendLine(ref handler);
				}
				if (num7 > 0)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder6 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
					handler.AppendLiteral("  卸载成功 ");
					handler.AppendFormatted(num7);
					handler.AppendLiteral(" 个");
					stringBuilder6.AppendLine(ref handler);
				}
				if (num8 > 0)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder7 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
					handler.AppendLiteral("  卸载失败 ");
					handler.AppendFormatted(num8);
					handler.AppendLiteral(" 个");
					stringBuilder7.AppendLine(ref handler);
				}
			}
			stringBuilder.AppendLine();
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder8 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(16, 2, stringBuilder2);
			handler.AppendLiteral("位置更新：成功 ");
			handler.AppendFormatted(num);
			handler.AppendLiteral(" 个，失败 ");
			handler.AppendFormatted(num2);
			handler.AppendLiteral(" 个");
			stringBuilder8.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder9 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(16, 2, stringBuilder2);
			handler.AppendLiteral("旋转更新：成功 ");
			handler.AppendFormatted(num3);
			handler.AppendLiteral(" 个，失败 ");
			handler.AppendFormatted(num4);
			handler.AppendLiteral(" 个");
			stringBuilder9.AppendLine(ref handler);
			MessageBox.Show(stringBuilder.ToString(), "管理链接模型", MessageBoxButton.OK, (num > 0 || num3 > 0 || num5 > 0 || num7 > 0 || num9 > 0) ? MessageBoxImage.Asterisk : MessageBoxImage.Exclamation);
			Logger.Info($"保存完成: 路径成功 {num9}, 失败 {num10}; 载入成功 {num5}, 卸载成功 {num7}; 位置成功 {num}, 失败 {num2}; 旋转成功 {num3}, 失败 {num4}");
			foreach (ModelLinkInfo item2 in list)
			{
				bool flag3 = true;
				if (item2.IsPathChanged)
				{
					flag3 = false;
				}
				if (item2.IsLoadedDesired != item2.IsLoaded)
				{
					flag3 = false;
				}
				if (!item2.PositionSuccessfullyUpdated)
				{
					flag3 = false;
				}
				if (!item2.RotationSuccessfullyUpdated)
				{
					flag3 = false;
				}
				if (flag3)
				{
					item2.MarkAsSaved();
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("保存修改失败", ex2);
			MessageBox.Show("保存失败: " + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void Refresh()
	{
		try
		{
			LoadLinkModelsFromRevit();
			Logger.Info("已刷新链接列表");
		}
		catch (Exception ex)
		{
			Logger.Error("刷新失败", ex);
		}
	}

	[RelayCommand]
	private void Close()
	{
		OnRequestClose?.Invoke();
	}
}
