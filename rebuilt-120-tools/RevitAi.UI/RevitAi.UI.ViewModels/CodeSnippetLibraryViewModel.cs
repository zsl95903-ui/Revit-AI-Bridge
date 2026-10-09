using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.AI;
using RevitAi.Core.AI.Models;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace RevitAi.UI.ViewModels;

public class CodeSnippetLibraryViewModel : ObservableObject, IDisposable
{
	private readonly CodeSnippetStorageService _storage;

	private readonly FileSystemWatcher _fileWatcher;

	private string _searchKeyword = string.Empty;

	private string _selectedTag = string.Empty;

	private bool _isRefreshing;

	[ObservableProperty]
	private ObservableCollection<CodeSnippetItemViewModel> _snippets = new ObservableCollection<CodeSnippetItemViewModel>();

	[ObservableProperty]
	private CodeSnippetItemViewModel? _selectedSnippet;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private ObservableCollection<string> _allTags = new ObservableCollection<string>();

	[ObservableProperty]
	private bool _hasSnippets;

	[ObservableProperty]
	private bool _isCompactView;

	[ObservableProperty]
	private bool _isSortMode;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? searchCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string>? filterByTagCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<CodeSnippetItemViewModel?>? executeSnippetCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<CodeSnippetItemViewModel?>? deleteSnippetCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<CodeSnippetItemViewModel?>? exportSnippetCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? exportAllSnippetsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<CodeSnippetItemViewModel?>? uploadSnippetToMarketCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? importSnippetsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? refreshCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? toggleViewModeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? toggleSortModeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<CodeSnippetItemViewModel?>? moveSnippetUpCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<CodeSnippetItemViewModel?>? moveSnippetDownCommand;

	public string SearchKeyword
	{
		get
		{
			return _searchKeyword;
		}
		set
		{
			if (SetProperty(ref _searchKeyword, value, "SearchKeyword"))
			{
				Task.Run(async delegate
				{
					await SearchAsync();
				});
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<CodeSnippetItemViewModel> Snippets
	{
		get
		{
			return _snippets;
		}
		[MemberNotNull("_snippets")]
		set
		{
			if (!EqualityComparer<ObservableCollection<CodeSnippetItemViewModel>>.Default.Equals(_snippets, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Snippets);
				_snippets = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Snippets);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public CodeSnippetItemViewModel? SelectedSnippet
	{
		get
		{
			return _selectedSnippet;
		}
		set
		{
			if (!EqualityComparer<CodeSnippetItemViewModel>.Default.Equals(_selectedSnippet, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedSnippet);
				_selectedSnippet = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedSnippet);
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
	public ObservableCollection<string> AllTags
	{
		get
		{
			return _allTags;
		}
		[MemberNotNull("_allTags")]
		set
		{
			if (!EqualityComparer<ObservableCollection<string>>.Default.Equals(_allTags, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AllTags);
				_allTags = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AllTags);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool HasSnippets
	{
		get
		{
			return _hasSnippets;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasSnippets, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasSnippets);
				_hasSnippets = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasSnippets);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsCompactView
	{
		get
		{
			return _isCompactView;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isCompactView, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsCompactView);
				_isCompactView = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsCompactView);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsSortMode
	{
		get
		{
			return _isSortMode;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isSortMode, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsSortMode);
				_isSortMode = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsSortMode);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SearchCommand => searchCommand ?? (searchCommand = new AsyncRelayCommand(SearchAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string> FilterByTagCommand => filterByTagCommand ?? (filterByTagCommand = new AsyncRelayCommand<string>(FilterByTagAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<CodeSnippetItemViewModel?> ExecuteSnippetCommand => executeSnippetCommand ?? (executeSnippetCommand = new AsyncRelayCommand<CodeSnippetItemViewModel>(ExecuteSnippetAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<CodeSnippetItemViewModel?> DeleteSnippetCommand => deleteSnippetCommand ?? (deleteSnippetCommand = new AsyncRelayCommand<CodeSnippetItemViewModel>(DeleteSnippetAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<CodeSnippetItemViewModel?> ExportSnippetCommand => exportSnippetCommand ?? (exportSnippetCommand = new RelayCommand<CodeSnippetItemViewModel>(ExportSnippet));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ExportAllSnippetsCommand => exportAllSnippetsCommand ?? (exportAllSnippetsCommand = new RelayCommand(ExportAllSnippets));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<CodeSnippetItemViewModel?> UploadSnippetToMarketCommand => uploadSnippetToMarketCommand ?? (uploadSnippetToMarketCommand = new AsyncRelayCommand<CodeSnippetItemViewModel>(UploadSnippetToMarketAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ImportSnippetsCommand => importSnippetsCommand ?? (importSnippetsCommand = new AsyncRelayCommand(ImportSnippetsAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand RefreshCommand => refreshCommand ?? (refreshCommand = new AsyncRelayCommand(RefreshAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ToggleViewModeCommand => toggleViewModeCommand ?? (toggleViewModeCommand = new RelayCommand(ToggleViewMode));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ToggleSortModeCommand => toggleSortModeCommand ?? (toggleSortModeCommand = new RelayCommand(ToggleSortMode));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<CodeSnippetItemViewModel?> MoveSnippetUpCommand => moveSnippetUpCommand ?? (moveSnippetUpCommand = new AsyncRelayCommand<CodeSnippetItemViewModel>(MoveSnippetUpAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<CodeSnippetItemViewModel?> MoveSnippetDownCommand => moveSnippetDownCommand ?? (moveSnippetDownCommand = new AsyncRelayCommand<CodeSnippetItemViewModel>(MoveSnippetDownAsync));

	public event EventHandler<ExecuteSnippetEventArgs>? ExecuteSnippetRequested;

	public CodeSnippetLibraryViewModel()
	{
		_storage = new CodeSnippetStorageService();
		string directoryName = Path.GetDirectoryName(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "AI", "SavedCodeSnippets.json"));
		if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName))
		{
			_fileWatcher = new FileSystemWatcher(directoryName)
			{
				Filter = "SavedCodeSnippets.json",
				NotifyFilter = (NotifyFilters.Size | NotifyFilters.LastWrite)
			};
			_fileWatcher.Changed += async delegate(object s, FileSystemEventArgs e)
			{
				await OnFileChangedAsync(e);
			};
			_fileWatcher.EnableRaisingEvents = true;
		}
		else
		{
			_fileWatcher = null;
		}
		LoadSnippetsAsync();
	}

	private async Task OnFileChangedAsync(FileSystemEventArgs e)
	{
		if (_isRefreshing)
		{
			return;
		}
		_isRefreshing = true;
		try
		{
			await Task.Delay(300);
			await RefreshAsync();
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetLibrary] 文件变化监听失败: " + ex.Message, ex);
		}
		finally
		{
			_isRefreshing = false;
		}
	}

	public void Dispose()
	{
		_fileWatcher?.Dispose();
	}

	public async Task LoadSnippetsAsync()
	{
		await Task.Run(delegate
		{
			_storage.Reload();
			List<CodeSnippet> snippets = _storage.GetAllSnippets(sortByOrder: true);
			List<string> tags = _storage.GetAllTags();
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				Snippets.Clear();
				foreach (CodeSnippet item in snippets)
				{
					Snippets.Add(new CodeSnippetItemViewModel(item));
				}
				AllTags.Clear();
				foreach (string item2 in tags)
				{
					AllTags.Add(item2);
				}
				HasSnippets = Snippets.Count > 0;
			});
		});
	}

	[RelayCommand]
	private async Task SearchAsync()
	{
		await Task.Run(delegate
		{
			List<CodeSnippet> results = (string.IsNullOrWhiteSpace(_searchKeyword) ? _storage.GetAllSnippets(sortByOrder: true) : _storage.SearchSnippets(_searchKeyword));
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				Snippets.Clear();
				foreach (CodeSnippet item in results)
				{
					Snippets.Add(new CodeSnippetItemViewModel(item));
				}
				HasSnippets = Snippets.Count > 0;
			});
		});
	}

	[RelayCommand]
	private async Task FilterByTagAsync(string tag)
	{
		await Task.Run(delegate
		{
			List<CodeSnippet> results = (string.IsNullOrWhiteSpace(tag) ? _storage.GetAllSnippets(sortByOrder: true) : _storage.FilterByTag(tag));
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				Snippets.Clear();
				foreach (CodeSnippet item in results)
				{
					Snippets.Add(new CodeSnippetItemViewModel(item));
				}
				_selectedTag = tag;
				HasSnippets = Snippets.Count > 0;
			});
		});
	}

	[RelayCommand]
	private async Task ExecuteSnippetAsync(CodeSnippetItemViewModel? snippet)
	{
		if (snippet == null)
		{
			return;
		}
		try
		{
			_storage.RecordUsage(snippet.Id);
			ExecuteSnippetRequested?.Invoke(this, new ExecuteSnippetEventArgs(snippet.Code));
			Logger.Info("[CodeSnippetLibrary] 执行片段: " + snippet.Name);
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetLibrary] 执行片段失败: " + ex.Message, ex);
		}
	}

	[RelayCommand]
	private async Task DeleteSnippetAsync(CodeSnippetItemViewModel? snippet)
	{
		if (snippet == null)
		{
			return;
		}
		try
		{
			_storage.DeleteSnippet(snippet.Id);
			await LoadSnippetsAsync();
			Logger.Info("[CodeSnippetLibrary] 删除片段: " + snippet.Name);
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetLibrary] 删除片段失败: " + ex.Message, ex);
		}
	}

	[RelayCommand]
	private void ExportSnippet(CodeSnippetItemViewModel? snippet)
	{
		if (snippet == null)
		{
			return;
		}
		try
		{
			string contents = _storage.ExportSnippet(snippet.Id);
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
				DefaultExt = "json",
				FileName = snippet.Name.Replace(":", "_").Replace("/", "_") + ".json"
			};
			if (saveFileDialog.ShowDialog() == true)
			{
				File.WriteAllText(saveFileDialog.FileName, contents);
				Logger.Info("[CodeSnippetLibrary] 导出片段: " + snippet.Name);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetLibrary] 导出片段失败: " + ex.Message, ex);
		}
	}

	[RelayCommand]
	private void ExportAllSnippets()
	{
		try
		{
			string contents = _storage.ExportAllSnippets();
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
				DefaultExt = "json",
				FileName = $"代码片段库_{DateTime.Now:yyyyMMdd_HHmmss}.json"
			};
			if (saveFileDialog.ShowDialog() == true)
			{
				File.WriteAllText(saveFileDialog.FileName, contents);
				Logger.Info("[CodeSnippetLibrary] 导出所有片段");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetLibrary] 导出所有片段失败: " + ex.Message, ex);
		}
	}

	[RelayCommand]
	private async Task UploadSnippetToMarketAsync(CodeSnippetItemViewModel? snippet)
	{
		if (snippet == null)
		{
			return;
		}
		try
		{
			CodeSnippetStorageService codeSnippetStorageService = new CodeSnippetStorageService();
			CodeSnippet codeSnippet = codeSnippetStorageService.GetSnippet(snippet.Id);
			if (codeSnippet == null)
			{
				Logger.Error("[CodeSnippetLibrary] 未找到片段: " + snippet.Id);
				return;
			}
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				_ = new UploadSnippetDialog(codeSnippet).ShowDialog() == true;
			});
			await Task.CompletedTask;
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetLibrary] 上传片段失败: " + ex.Message, ex);
		}
	}

	[RelayCommand]
	private async Task ImportSnippetsAsync()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
				Multiselect = false
			};
			if (openFileDialog.ShowDialog() == true)
			{
				string json = File.ReadAllText(openFileDialog.FileName);
				_storage.ImportSnippets(json);
				await LoadSnippetsAsync();
				Logger.Info("[CodeSnippetLibrary] 导入片段成功");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetLibrary] 导入片段失败: " + ex.Message, ex);
		}
	}

	[RelayCommand]
	public async Task RefreshAsync()
	{
		await LoadSnippetsAsync();
	}

	[RelayCommand]
	private void ToggleViewMode()
	{
		IsCompactView = !IsCompactView;
	}

	[RelayCommand]
	private void ToggleSortMode()
	{
		IsSortMode = !IsSortMode;
	}

	[RelayCommand]
	private async Task MoveSnippetUpAsync(CodeSnippetItemViewModel? snippet)
	{
		if (snippet == null)
		{
			return;
		}
		try
		{
			_storage.MoveSnippetUp(snippet.Id, IsSortMode);
			await LoadSnippetsAsync();
			Logger.Info("[CodeSnippetLibrary] 上移片段: " + snippet.Name);
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetLibrary] 上移片段失败: " + ex.Message, ex);
		}
	}

	[RelayCommand]
	private async Task MoveSnippetDownAsync(CodeSnippetItemViewModel? snippet)
	{
		if (snippet == null)
		{
			return;
		}
		try
		{
			_storage.MoveSnippetDown(snippet.Id, IsSortMode);
			await LoadSnippetsAsync();
			Logger.Info("[CodeSnippetLibrary] 下移片段: " + snippet.Name);
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetLibrary] 下移片段失败: " + ex.Message, ex);
		}
	}
}
