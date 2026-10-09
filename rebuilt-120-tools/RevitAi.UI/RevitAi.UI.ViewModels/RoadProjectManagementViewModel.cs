using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using ServiceProvider = RevitAi.Abstractions.Loader.ServiceProvider;

namespace RevitAi.UI.ViewModels;

public class RoadProjectManagementViewModel : ObservableObject
{
	private readonly IRoadProjectManager _projectManager;

	private readonly ILogger _logger;

	[ObservableProperty]
	private ObservableCollection<RoadProject> _projects = new ObservableCollection<RoadProject>();

	[ObservableProperty]
	private RoadProject? _selectedProject;

	[ObservableProperty]
	private RoadProject? _activeProject;

	[ObservableProperty]
	private string _statusMessage = "就绪";

	[ObservableProperty]
	private bool _isLoading;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? newProjectCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<RoadProject?>? editProjectCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<RoadProject?>? deleteProjectCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<RoadProject?>? setActiveProjectCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<RoadProject?>? exportProjectCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? importProjectCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? exportAllProjectsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? refreshProjectsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<RoadProject> Projects
	{
		get
		{
			return _projects;
		}
		[MemberNotNull("_projects")]
		set
		{
			if (!EqualityComparer<ObservableCollection<RoadProject>>.Default.Equals(_projects, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Projects);
				_projects = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Projects);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public RoadProject? SelectedProject
	{
		get
		{
			return _selectedProject;
		}
		set
		{
			if (!EqualityComparer<RoadProject>.Default.Equals(_selectedProject, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedProject);
				_selectedProject = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedProject);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public RoadProject? ActiveProject
	{
		get
		{
			return _activeProject;
		}
		set
		{
			if (!EqualityComparer<RoadProject>.Default.Equals(_activeProject, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ActiveProject);
				_activeProject = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ActiveProject);
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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand NewProjectCommand => newProjectCommand ?? (newProjectCommand = new RelayCommand(NewProject));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<RoadProject?> EditProjectCommand => editProjectCommand ?? (editProjectCommand = new RelayCommand<RoadProject>(EditProject));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<RoadProject?> DeleteProjectCommand => deleteProjectCommand ?? (deleteProjectCommand = new AsyncRelayCommand<RoadProject>(DeleteProjectAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<RoadProject?> SetActiveProjectCommand => setActiveProjectCommand ?? (setActiveProjectCommand = new RelayCommand<RoadProject>(SetActiveProject));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<RoadProject?> ExportProjectCommand => exportProjectCommand ?? (exportProjectCommand = new AsyncRelayCommand<RoadProject>(ExportProjectAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ImportProjectCommand => importProjectCommand ?? (importProjectCommand = new AsyncRelayCommand(ImportProjectAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ExportAllProjectsCommand => exportAllProjectsCommand ?? (exportAllProjectsCommand = new AsyncRelayCommand(ExportAllProjectsAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand RefreshProjectsCommand => refreshProjectsCommand ?? (refreshProjectsCommand = new RelayCommand(RefreshProjects));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public RoadProjectManagementViewModel()
	{
		IServiceProvider services = UIBootstrapper.Services;
		_projectManager = services.GetRequiredService<IRoadProjectManager>();
		_logger = ServiceProvider.GetLogger();
		_projectManager.ProjectChanged += OnProjectChanged;
		RefreshProjects();
		RoadProject activeProject = _projectManager.GetActiveProject();
		if (activeProject != null)
		{
			ActiveProject = activeProject;
		}
	}

	[RelayCommand]
	private void NewProject()
	{
		try
		{
			RoadProjectEditorViewModel viewModel = UIBootstrapper.Services.GetRequiredService<RoadProjectEditorViewModel>();
			viewModel.InitializeForNewProject();
			RoadProjectEditorWindow roadProjectEditorWindow = new RoadProjectEditorWindow();
			roadProjectEditorWindow.DataContext = viewModel;
			roadProjectEditorWindow.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsActive);
			roadProjectEditorWindow.Closed += delegate
			{
				if (viewModel.IsSaved)
				{
					RefreshProjects();
				}
			};
			roadProjectEditorWindow.Show();
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectManagementViewModel] 新建项目失败", ex);
			MessageBox.Show("新建项目失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void EditProject(RoadProject? project)
	{
		try
		{
			if (project == null)
			{
				MessageBox.Show("请先选择一个项目", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			RoadProjectEditorViewModel viewModel = UIBootstrapper.Services.GetRequiredService<RoadProjectEditorViewModel>();
			viewModel.InitializeForEdit(project);
			RoadProjectEditorWindow roadProjectEditorWindow = new RoadProjectEditorWindow();
			roadProjectEditorWindow.DataContext = viewModel;
			roadProjectEditorWindow.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsActive);
			roadProjectEditorWindow.Closed += delegate
			{
				if (viewModel.IsSaved)
				{
					RefreshProjects();
				}
			};
			roadProjectEditorWindow.Show();
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectManagementViewModel] 编辑项目失败", ex);
			MessageBox.Show("编辑项目失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private async Task DeleteProjectAsync(RoadProject? project)
	{
		try
		{
			if (project == null)
			{
				MessageBox.Show("请先选择一个项目", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			else if (MessageBox.Show("确定要删除项目 '" + project.Name + "' 吗？\n\n此操作不可撤销。", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				IsLoading = true;
				StatusMessage = "正在删除项目...";
				Result<bool> result = await Task.Run(() => _projectManager.DeleteProject(project.Id));
				if (result.IsFailure)
				{
					MessageBox.Show("删除项目失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
				else
				{
					StatusMessage = "项目 '" + project.Name + "' 已删除";
					_logger.Info("[RoadProjectManagementViewModel] 已删除项目: " + project.Name);
				}
				RefreshProjects();
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectManagementViewModel] 删除项目失败", ex);
			MessageBox.Show("删除项目失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private void SetActiveProject(RoadProject? project)
	{
		try
		{
			if (project == null)
			{
				MessageBox.Show("请先选择一个项目", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			Result<bool> result = _projectManager.SetActiveProject(project.Id);
			if (result.IsFailure)
			{
				MessageBox.Show("设置活动项目失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			ActiveProject = project;
			StatusMessage = "活动项目: " + project.Name;
			_logger.Info("[RoadProjectManagementViewModel] 已设置活动项目: " + project.Name);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectManagementViewModel] 设置活动项目失败", ex);
			MessageBox.Show("设置活动项目失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private async Task ExportProjectAsync(RoadProject? project)
	{
		try
		{
			if (project == null)
			{
				MessageBox.Show("请先选择一个项目", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			SaveFileDialog saveDialog = new SaveFileDialog
			{
				Filter = "道路项目文件 (*.roadproject)|*.roadproject|所有文件 (*.*)|*.*",
				DefaultExt = "roadproject",
				FileName = project.Name + ".roadproject"
			};
			if (saveDialog.ShowDialog() == true)
			{
				IsLoading = true;
				StatusMessage = "正在导出项目...";
				Result<bool> result = await _projectManager.ExportProjectAsync(project.Id, saveDialog.FileName);
				if (result.IsFailure)
				{
					MessageBox.Show("导出项目失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
					return;
				}
				StatusMessage = "项目已导出到: " + saveDialog.FileName;
				MessageBox.Show("项目已成功导出到:\n" + saveDialog.FileName, "导出成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectManagementViewModel] 导出项目失败", ex);
			MessageBox.Show("导出项目失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private async Task ImportProjectAsync()
	{
		_ = 1;
		try
		{
			OpenFileDialog openDialog = new OpenFileDialog
			{
				Filter = "道路项目文件 (*.roadproject;*.roadprojects)|*.roadproject;*.roadprojects|所有文件 (*.*)|*.*",
				Title = "选择要导入的项目文件"
			};
			if (openDialog.ShowDialog() != true)
			{
				return;
			}
			IsLoading = true;
			StatusMessage = "正在导入项目...";
			Result<List<RoadProject>> result = await _projectManager.ImportMultipleProjectsAsync(openDialog.FileName);
			if (result.IsSuccess && result.Value != null)
			{
				List<RoadProject> value = result.Value;
				StatusMessage = $"已导入 {value.Count} 个项目";
				MessageBox.Show($"已成功导入 {value.Count} 个项目", "导入成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				RefreshProjects();
			}
			else
			{
				Result<RoadProject> result2 = await _projectManager.ImportProjectAsync(openDialog.FileName);
				if (result2.IsFailure)
				{
					MessageBox.Show("导入项目失败: " + result2.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
					return;
				}
				StatusMessage = "项目 '" + result2.Value.Name + "' 已导入";
				MessageBox.Show("项目 '" + result2.Value.Name + "' 已成功导入", "导入成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				RefreshProjects();
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectManagementViewModel] 导入项目失败", ex);
			MessageBox.Show("导入项目失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private async Task ExportAllProjectsAsync()
	{
		try
		{
			IReadOnlyList<RoadProject> allProjects = _projectManager.GetAllProjects();
			if (allProjects.Count == 0)
			{
				MessageBox.Show("没有可导出的项目", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				return;
			}
			SaveFileDialog saveDialog = new SaveFileDialog
			{
				Filter = "道路项目集合 (*.roadprojects)|*.roadprojects|所有文件 (*.*)|*.*",
				DefaultExt = "roadprojects",
				FileName = $"道路项目集合_{DateTime.Now:yyyyMMdd_HHmmss}.roadprojects"
			};
			if (saveDialog.ShowDialog() == true)
			{
				IsLoading = true;
				StatusMessage = "正在导出所有项目...";
				Result<bool> result = await _projectManager.ExportAllProjectsAsync(saveDialog.FileName);
				if (result.IsFailure)
				{
					MessageBox.Show("导出失败: " + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
					return;
				}
				StatusMessage = $"已导出 {allProjects.Count} 个项目";
				MessageBox.Show($"已成功导出 {allProjects.Count} 个项目到:\n{saveDialog.FileName}", "导出成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectManagementViewModel] 导出全部项目失败", ex);
			MessageBox.Show("导出失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	private void RefreshProjects()
	{
		try
		{
			Projects.Clear();
			foreach (RoadProject allProject in _projectManager.GetAllProjects())
			{
				Projects.Add(allProject);
			}
			StatusMessage = $"已加载 {Projects.Count} 个项目";
			_logger.Info($"[RoadProjectManagementViewModel] 已刷新项目列表，共 {Projects.Count} 个项目");
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadProjectManagementViewModel] 刷新项目列表失败", ex);
			StatusMessage = "刷新失败";
		}
	}

	[RelayCommand]
	private void Close()
	{
		_projectManager.ProjectChanged -= OnProjectChanged;
		Application.Current?.Windows.OfType<RoadProjectManagementWindow>().FirstOrDefault()?.Close();
	}

	private void OnProjectChanged(object? sender, RoadProjectChangedEventArgs e)
	{
		Application current = Application.Current;
		if (current == null)
		{
			return;
		}
		((DispatcherObject)current).Dispatcher.Invoke((Action)delegate
		{
			RefreshProjects();
			if (e.ChangeType == RoadProjectChangeType.ActiveChanged && e.Project != null)
			{
				ActiveProject = e.Project;
			}
		});
	}
}
