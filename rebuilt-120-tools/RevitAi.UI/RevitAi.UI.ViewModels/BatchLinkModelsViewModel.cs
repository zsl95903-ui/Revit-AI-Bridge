using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.LinkManagement;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace RevitAi.UI.ViewModels;

public class BatchLinkModelsViewModel : ObservableObject
{
	private readonly ILinkService? _linkService;

	private readonly object? _document;

	private readonly IRevitAdapter? _revitAdapter;

	private readonly object? _batchLinkModelsExternalEvent;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? selectFilesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<ModelFileInfo?>? removeFileCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? clearFilesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? linkModelsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelCommand;

	public ObservableCollection<ModelFileInfo> SelectedFiles { get; } = new ObservableCollection<ModelFileInfo>();

	public int FileCount => SelectedFiles.Count;

	public Action? OnRequestClose { get; set; }

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SelectFilesCommand => selectFilesCommand ?? (selectFilesCommand = new RelayCommand(SelectFiles));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<ModelFileInfo?> RemoveFileCommand => removeFileCommand ?? (removeFileCommand = new RelayCommand<ModelFileInfo>(RemoveFile));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ClearFilesCommand => clearFilesCommand ?? (clearFilesCommand = new RelayCommand(ClearFiles));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand LinkModelsCommand => linkModelsCommand ?? (linkModelsCommand = new RelayCommand(LinkModels));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	public BatchLinkModelsViewModel()
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
					MethodInfo method = revitAdapter.GetType().GetMethod("GetBatchLinkModelsExternalEvent");
					if (method != null)
					{
						_batchLinkModelsExternalEvent = method.Invoke(revitAdapter, null);
						if (_batchLinkModelsExternalEvent != null)
						{
							Logger.Info("✅ BatchLinkModelsViewModel 成功获取 BatchLinkModels ExternalEvent");
						}
						else
						{
							Logger.Warning("⚠\ufe0f GetBatchLinkModelsExternalEvent 返回 null");
						}
					}
					else
					{
						Logger.Warning("⚠\ufe0f 未找到 GetBatchLinkModelsExternalEvent 方法");
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("⚠\ufe0f 获取 BatchLinkModels ExternalEvent 失败: " + ex.Message);
				}
			}
			if (_linkService == null || _document == null)
			{
				Logger.Warning("批量链接模型: 无法获取 Revit 服务或文档");
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("初始化批量链接模型失败", ex2);
		}
	}

	[RelayCommand]
	private void SelectFiles()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择 Revit 模型文件",
				Filter = "Revit 模型文件 (*.rvt)|*.rvt|所有文件 (*.*)|*.*",
				Multiselect = true,
				RestoreDirectory = true
			};
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			int num = 0;
			string[] fileNames = openFileDialog.FileNames;
			foreach (string fileName in fileNames)
			{
				if (!SelectedFiles.Any((ModelFileInfo f) => f.FilePath == fileName))
				{
					SelectedFiles.Add(new ModelFileInfo
					{
						FileName = Path.GetFileName(fileName),
						FilePath = fileName
					});
					num++;
				}
			}
			OnPropertyChanged("FileCount");
			if (num > 0)
			{
				Logger.Info($"已添加 {num} 个文件到列表");
			}
			else
			{
				MessageBox.Show("所选文件已存在于列表中", "批量链接模型", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("选择文件失败", ex);
			MessageBox.Show("选择文件失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void RemoveFile(ModelFileInfo? file)
	{
		if (file == null)
		{
			return;
		}
		try
		{
			SelectedFiles.Remove(file);
			OnPropertyChanged("FileCount");
			Logger.Info("已移除文件: " + file.FileName);
		}
		catch (Exception ex)
		{
			Logger.Error("移除文件失败", ex);
		}
	}

	[RelayCommand]
	private void ClearFiles()
	{
		try
		{
			int count = SelectedFiles.Count;
			SelectedFiles.Clear();
			OnPropertyChanged("FileCount");
			Logger.Info($"已清空文件列表，移除 {count} 个文件");
		}
		catch (Exception ex)
		{
			Logger.Error("清空文件列表失败", ex);
		}
	}

	[RelayCommand]
	private void LinkModels()
	{
		try
		{
			if (SelectedFiles.Count == 0)
			{
				MessageBox.Show("请先选择要链接的模型文件", "批量链接模型", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if (_batchLinkModelsExternalEvent == null)
			{
				MessageBox.Show("无法获取批量链接模型 ExternalEvent", "批量链接模型", MessageBoxButton.OK, MessageBoxImage.Hand);
				Logger.Error("LinkModels: ExternalEvent 为 null");
				return;
			}
			if (_document == null)
			{
				MessageBox.Show("无法获取 Revit 文档，请确保在 Revit 中运行此命令", "批量链接模型", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			Logger.Info($"开始链接 {SelectedFiles.Count} 个模型文件");
			List<string> filePaths = SelectedFiles.Select((ModelFileInfo f) => f.FilePath).ToList();
			BatchLinkModelsExternalEventRequest value = new BatchLinkModelsExternalEventRequest
			{
				Document = _document,
				FilePaths = filePaths,
				OnCompleted = delegate(Exception? ex3, int successCount)
				{
					((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
					{
						if (ex3 != null)
						{
							MessageBox.Show("链接失败: " + ex3.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
							Logger.Error("链接模型失败", ex3);
						}
						else if (successCount == SelectedFiles.Count)
						{
							MessageBox.Show($"成功链接 {successCount} / {SelectedFiles.Count} 个模型文件", "批量链接模型", MessageBoxButton.OK, MessageBoxImage.Asterisk);
							Logger.Info($"成功链接 {successCount} 个模型文件");
							SelectedFiles.Clear();
							OnPropertyChanged("FileCount");
							CloseWindow();
						}
						else if (successCount > 0)
						{
							MessageBox.Show($"部分链接成功：{successCount} / {SelectedFiles.Count} 个模型文件\n\n请检查失败的文件路径和权限后重试", "批量链接模型", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							Logger.Warning($"部分链接成功：{successCount} / {SelectedFiles.Count}");
						}
						else
						{
							MessageBox.Show("链接失败，请检查文件路径和权限", "批量链接模型", MessageBoxButton.OK, MessageBoxImage.Hand);
							Logger.Error("链接模型失败");
						}
					});
				}
			};
			try
			{
				Assembly assembly = _revitAdapter?.GetType().Assembly;
				Type type = null;
				Logger.Info("RevitAdapter Assembly: " + assembly?.GetName().FullName);
				if (assembly != null)
				{
					Type[] types = assembly.GetTypes();
					Logger.Info($"Assembly contains {types.Length} types");
					type = assembly.GetTypes().FirstOrDefault((Type t) => t.Name == "BatchLinkModelsExternalEventHandler");
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
							type = assembly2.GetType("RevitAi.Revit.LinkManagement.BatchLinkModelsExternalEventHandler");
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
					(_batchLinkModelsExternalEvent?.GetType().GetMethod("Raise"))?.Invoke(_batchLinkModelsExternalEvent, null);
					Logger.Info("成功触发 ExternalEvent 批量链接操作");
				}
				else
				{
					Logger.Error("无法找到 BatchLinkModelsExternalEventHandler 类型");
					MessageBox.Show("无法找到批量链接模型处理器，请确保 Revit 适配层已正确加载", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("触发 ExternalEvent 异常: " + ex.Message + "\n" + ex.StackTrace);
				MessageBox.Show("触发批量链接操作失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("链接模型失败", ex2);
			MessageBox.Show("链接模型失败: " + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void Cancel()
	{
		try
		{
			if (SelectedFiles.Count <= 0 || MessageBox.Show($"确定要关闭窗口吗？当前已选择 {SelectedFiles.Count} 个文件，这些选择将丢失。", "批量链接模型", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
			{
				CloseWindow();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("取消操作失败", ex);
		}
	}

	private void CloseWindow()
	{
		OnRequestClose?.Invoke();
	}
}
