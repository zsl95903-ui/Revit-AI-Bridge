using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.Core.AI;
using RevitAi.Core.AI.Models;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class CodeMarketViewModel : ObservableObject
{
	private readonly ICodeMarketService _codeMarketService;

	private readonly IAuthManager _authManager;

	private readonly IDialogService _dialogService;

	private readonly IWindowManager _windowManager;

	private readonly Dispatcher _dispatcher;

	[ObservableProperty]
	private string _currentTab = "hot";

	[ObservableProperty]
	private ObservableCollection<CodeSnippetMarketItemViewModel> _snippets = new ObservableCollection<CodeSnippetMarketItemViewModel>();

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private bool _hasMore;

	[ObservableProperty]
	private int _totalCount;

	[ObservableProperty]
	private string _searchKeyword = string.Empty;

	[ObservableProperty]
	private string _selectedCategory = "全部";

	[ObservableProperty]
	private int _currentPage;

	private const int PageSize = 20;

	private HashSet<string> _favoritedSnippetIds = new HashSet<string>();

	private static readonly Dictionary<string, string> CategoryMapping = new Dictionary<string, string>
	{
		{ "全部", "all" },
		{ "建模工具", "modeling" },
		{ "标注工具", "annotation" },
		{ "分析工具", "analysis" },
		{ "视图工具", "view" },
		{ "文档工具", "documentation" },
		{ "实用工具", "utility" },
		{ "其他", "other" }
	};

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string>? switchTabCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loadSnippetsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loadMoreCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? searchCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string>? viewSnippetDetailCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string>? downloadSnippetCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string>? toggleFavoriteCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? uploadSnippetCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<string>? deleteSnippetCommand;

	public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>(new string[8] { "全部", "建模工具", "标注工具", "分析工具", "视图工具", "文档工具", "实用工具", "其他" });

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string CurrentTab
	{
		get
		{
			return _currentTab;
		}
		[MemberNotNull("_currentTab")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_currentTab, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CurrentTab);
				_currentTab = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CurrentTab);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<CodeSnippetMarketItemViewModel> Snippets
	{
		get
		{
			return _snippets;
		}
		[MemberNotNull("_snippets")]
		set
		{
			if (!EqualityComparer<ObservableCollection<CodeSnippetMarketItemViewModel>>.Default.Equals(_snippets, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Snippets);
				_snippets = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Snippets);
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
	public bool HasMore
	{
		get
		{
			return _hasMore;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_hasMore, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.HasMore);
				_hasMore = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.HasMore);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int TotalCount
	{
		get
		{
			return _totalCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_totalCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.TotalCount);
				_totalCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.TotalCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SearchKeyword
	{
		get
		{
			return _searchKeyword;
		}
		[MemberNotNull("_searchKeyword")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_searchKeyword, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SearchKeyword);
				_searchKeyword = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SearchKeyword);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedCategory
	{
		get
		{
			return _selectedCategory;
		}
		[MemberNotNull("_selectedCategory")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedCategory, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedCategory);
				_selectedCategory = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedCategory);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int CurrentPage
	{
		get
		{
			return _currentPage;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_currentPage, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CurrentPage);
				_currentPage = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CurrentPage);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string> SwitchTabCommand => switchTabCommand ?? (switchTabCommand = new AsyncRelayCommand<string>(SwitchTabAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoadSnippetsCommand => loadSnippetsCommand ?? (loadSnippetsCommand = new AsyncRelayCommand(LoadSnippetsAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoadMoreCommand => loadMoreCommand ?? (loadMoreCommand = new AsyncRelayCommand(LoadMoreAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SearchCommand => searchCommand ?? (searchCommand = new AsyncRelayCommand(SearchAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string> ViewSnippetDetailCommand => viewSnippetDetailCommand ?? (viewSnippetDetailCommand = new AsyncRelayCommand<string>(ViewSnippetDetailAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string> DownloadSnippetCommand => downloadSnippetCommand ?? (downloadSnippetCommand = new AsyncRelayCommand<string>(DownloadSnippetAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string> ToggleFavoriteCommand => toggleFavoriteCommand ?? (toggleFavoriteCommand = new AsyncRelayCommand<string>(ToggleFavoriteAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand UploadSnippetCommand => uploadSnippetCommand ?? (uploadSnippetCommand = new AsyncRelayCommand(UploadSnippetAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<string> DeleteSnippetCommand => deleteSnippetCommand ?? (deleteSnippetCommand = new AsyncRelayCommand<string>(DeleteSnippetAsync));

	public event EventHandler? SnippetDownloaded;

	private void DispatchToUIThread(Action action)
	{
		if (_dispatcher.CheckAccess())
		{
			action();
		}
		else
		{
			_dispatcher.Invoke(action);
		}
	}

	private async Task DispatchToUIThreadAsync(Func<Task> action)
	{
		if (_dispatcher.CheckAccess())
		{
			await action();
		}
		else
		{
			await _dispatcher.InvokeAsync<Task>(action);
		}
	}

	public CodeMarketViewModel(ICodeMarketService codeMarketService, IAuthManager authManager, IDialogService dialogService, IWindowManager windowManager)
	{
		_codeMarketService = codeMarketService ?? throw new ArgumentNullException("codeMarketService");
		_authManager = authManager ?? throw new ArgumentNullException("authManager");
		_dialogService = dialogService ?? throw new ArgumentNullException("dialogService");
		_windowManager = windowManager ?? throw new ArgumentNullException("windowManager");
		_dispatcher = Dispatcher.CurrentDispatcher;
	}

	[RelayCommand]
	private async Task SwitchTabAsync(string tab)
	{
		CurrentTab = tab;
		CurrentPage = 0;
		await LoadSnippetsAsync();
	}

	[RelayCommand]
	public async Task LoadSnippetsAsync()
	{
		IsLoading = true;
		try
		{
			string searchKeyword = SearchKeyword;
			List<string> list = new List<string>();
			if (!string.IsNullOrWhiteSpace(SearchKeyword))
			{
				MatchCollection matchCollection = Regex.Matches(SearchKeyword, "#(\\S+)");
				if (matchCollection.Count > 0)
				{
					list = (from Match m in matchCollection
						select m.Groups[1].Value).ToList();
					searchKeyword = Regex.Replace(SearchKeyword, "#\\S+", "").Trim();
				}
			}
			CodeMarketSearchOptions codeMarketSearchOptions = new CodeMarketSearchOptions();
			codeMarketSearchOptions.SearchKeyword = searchKeyword;
			codeMarketSearchOptions.Tags = ((list.Count > 0) ? list : null);
			CodeMarketSearchOptions codeMarketSearchOptions2 = codeMarketSearchOptions;
			string currentTab = CurrentTab;
			CodeMarketSortOrder sortOrder = ((!(currentTab == "hot") && currentTab == "latest") ? CodeMarketSortOrder.Latest : CodeMarketSortOrder.Hot);
			codeMarketSearchOptions2.SortOrder = sortOrder;
			codeMarketSearchOptions.Limit = 20;
			codeMarketSearchOptions.Offset = CurrentPage * 20;
			CodeMarketSearchOptions codeMarketSearchOptions3 = codeMarketSearchOptions;
			if (SelectedCategory != "全部" && CategoryMapping.TryGetValue(SelectedCategory, out string value))
			{
				codeMarketSearchOptions3.Categories = new List<string> { value };
			}
			if (CurrentTab == "favorites")
			{
				CodeMarketResult<List<CodeSnippetMarketItem>> codeMarketResult = await _codeMarketService.GetMyFavoritesAsync(codeMarketSearchOptions3.Limit, codeMarketSearchOptions3.Offset);
				if (codeMarketResult.Success)
				{
					IEnumerable<CodeSnippetMarketItemViewModel> items = codeMarketResult.Data?.Select((CodeSnippetMarketItem s) => new CodeSnippetMarketItemViewModel(s, isFavorited: true)) ?? Enumerable.Empty<CodeSnippetMarketItemViewModel>();
					await DispatchToUIThreadAsync(async delegate
					{
						if (CurrentPage == 0)
						{
							Snippets.Clear();
						}
						foreach (CodeSnippetMarketItemViewModel item in items)
						{
							Snippets.Add(item);
						}
					});
					HasMore = items.Count() == 20;
					TotalCount = Snippets.Count;
				}
				else
				{
					_dialogService.ShowError("加载收藏失败", codeMarketResult.Error ?? "未知错误");
				}
				return;
			}
			if (CurrentTab == "my_published")
			{
				CodeMarketResult<List<CodeSnippetMarketItem>> codeMarketResult2 = await _codeMarketService.GetMyPublishedSnippetsAsync();
				if (codeMarketResult2.Success)
				{
					IEnumerable<CodeSnippetMarketItemViewModel> items2 = codeMarketResult2.Data?.Where((CodeSnippetMarketItem s) => s.Status != "deleted").Select((CodeSnippetMarketItem s) => new CodeSnippetMarketItemViewModel(s, isFavorited: false, isOwned: true)) ?? Enumerable.Empty<CodeSnippetMarketItemViewModel>();
					await DispatchToUIThreadAsync(async delegate
					{
						Snippets.Clear();
						foreach (CodeSnippetMarketItemViewModel item2 in items2)
						{
							Snippets.Add(item2);
						}
					});
					HasMore = false;
					TotalCount = Snippets.Count;
				}
				else
				{
					_dialogService.ShowError("加载发布失败", codeMarketResult2.Error ?? "未知错误");
				}
				return;
			}
			CodeMarketResult<CodeMarketSearchResult> codeMarketResult3 = await _codeMarketService.BrowseMarketAsync(codeMarketSearchOptions3);
			if (codeMarketResult3.Success)
			{
				CodeMarketSearchResult searchResult = codeMarketResult3.Data;
				if (searchResult == null)
				{
					return;
				}
				List<CodeSnippetMarketItemViewModel> items3 = searchResult.Snippets.Select((CodeSnippetMarketItem s) => new CodeSnippetMarketItemViewModel(s)).ToList();
				await DispatchToUIThreadAsync(async delegate
				{
					if (CurrentPage == 0)
					{
						Snippets.Clear();
					}
					foreach (CodeSnippetMarketItemViewModel item3 in items3)
					{
						Snippets.Add(item3);
					}
				});
				HasMore = searchResult.HasMore;
				TotalCount = searchResult.Total;
				await CheckFavoritedStatusAsync(items3);
			}
			else
			{
				_dialogService.ShowError("加载失败", codeMarketResult3.Error ?? "未知错误");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeMarketViewModel] 加载片段失败: " + ex.Message, ex);
			_dialogService.ShowError("加载失败", ex.Message);
		}
		finally
		{
			IsLoading = false;
		}
	}

	[RelayCommand]
	public async Task LoadMoreAsync()
	{
		if (!IsLoading && HasMore)
		{
			CurrentPage++;
			await LoadSnippetsAsync();
		}
	}

	[RelayCommand]
	public async Task SearchAsync()
	{
		CurrentPage = 0;
		await LoadSnippetsAsync();
	}

	[RelayCommand]
	public async Task ViewSnippetDetailAsync(string snippetId)
	{
		try
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				new SnippetDetailDialog(snippetId).ShowDialog();
			});
			await Task.CompletedTask;
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeMarketViewModel] 打开详情对话框失败: " + ex.Message, ex);
			_dialogService.ShowError("打开详情失败", ex.Message);
		}
	}

	[RelayCommand]
	public async Task DownloadSnippetAsync(string snippetId)
	{
		_ = 1;
		try
		{
			CodeMarketResult<CodeSnippetDownloadResult> codeMarketResult = await _codeMarketService.DownloadSnippetAsync(snippetId);
			if (codeMarketResult.Success)
			{
				CodeSnippetDownloadResult data = codeMarketResult.Data;
				if (data != null && data.Snippet != null)
				{
					CodeSnippet snippet = new CodeSnippet
					{
						Name = data.Snippet.Name,
						Description = data.Snippet.Description,
						Code = (data.Snippet.CodeContent ?? string.Empty),
						Tags = (data.Snippet.Tags ?? new List<string>())
					};
					new CodeSnippetStorageService().AddSnippet(snippet);
					string title = (data.IsFree ? "下载成功！已保存到本地代码片段库" : $"购买成功！花费 {data.PricePaid:G0} 电量\n已保存到本地代码片段库");
					_dialogService.ShowInfo("成功", title);
					SnippetDownloaded?.Invoke(this, EventArgs.Empty);
					await LoadSnippetsAsync();
				}
			}
			else
			{
				_dialogService.ShowError("下载失败", codeMarketResult.Error ?? "未知错误");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeMarketViewModel] 下载片段失败: " + ex.Message, ex);
			_dialogService.ShowError("下载失败", ex.Message);
		}
	}

	[RelayCommand]
	public async Task ToggleFavoriteAsync(string snippetId)
	{
		try
		{
			CodeMarketResult<bool> codeMarketResult = await _codeMarketService.ToggleFavoriteAsync(snippetId);
			if (codeMarketResult.Success)
			{
				bool data = codeMarketResult.Data;
				CodeSnippetMarketItemViewModel snippet = Snippets.FirstOrDefault((CodeSnippetMarketItemViewModel s) => s.Id == snippetId);
				if (snippet != null)
				{
					snippet.IsFavorited = data;
					if (data)
					{
						snippet.FavoriteCount++;
					}
					else
					{
						snippet.FavoriteCount--;
					}
				}
				if (CurrentTab == "favorites" && !data && snippet != null)
				{
					DispatchToUIThread(delegate
					{
						Snippets.Remove(snippet);
					});
				}
			}
			else
			{
				_dialogService.ShowError("收藏操作失败", codeMarketResult.Error ?? "未知错误");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeMarketViewModel] 收藏操作失败: " + ex.Message, ex);
			_dialogService.ShowError("收藏操作失败", ex.Message);
		}
	}

	[RelayCommand]
	public async Task UploadSnippetAsync()
	{
		try
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				if (new UploadSnippetDialog().ShowDialog() == true)
				{
					Task.Run(async delegate
					{
						await Task.Delay(500);
						await LoadSnippetsAsync();
					});
				}
			});
			await Task.CompletedTask;
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeMarketViewModel] 打开上传对话框失败: " + ex.Message, ex);
			_dialogService.ShowError("打开上传对话框失败", ex.Message);
		}
	}

	[RelayCommand]
	public async Task DeleteSnippetAsync(string snippetId)
	{
		try
		{
			CodeSnippetMarketItemViewModel snippet = Snippets.FirstOrDefault((CodeSnippetMarketItemViewModel s) => s.Id == snippetId);
			if (snippet == null)
			{
				return;
			}
			string title = "确定要下架代码片段「" + snippet.Name + "」吗？\n\n下架后其他用户将无法看到和下载此片段。";
			if (!_dialogService.ShowConfirm("确认下架", title))
			{
				return;
			}
			CodeMarketResult<bool> codeMarketResult = await _codeMarketService.DeleteSnippetAsync(snippetId);
			if (codeMarketResult.Success)
			{
				_dialogService.ShowInfo("成功", "片段已下架");
				if (snippet != null)
				{
					DispatchToUIThread(delegate
					{
						Snippets.Remove(snippet);
					});
				}
			}
			else
			{
				_dialogService.ShowError("下架失败", codeMarketResult.Error ?? "未知错误");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeMarketViewModel] 下架片段失败: " + ex.Message, ex);
			_dialogService.ShowError("操作失败", ex.Message);
		}
	}

	private async Task CheckFavoritedStatusAsync(List<CodeSnippetMarketItemViewModel> items)
	{
		try
		{
			List<string> snippetIds = items.Select((CodeSnippetMarketItemViewModel i) => i.Id).ToList();
			CodeMarketResult<HashSet<string>> codeMarketResult = await _codeMarketService.CheckFavoritedStatusAsync(snippetIds);
			if (!codeMarketResult.Success || codeMarketResult.Data == null)
			{
				return;
			}
			foreach (CodeSnippetMarketItemViewModel item in items)
			{
				item.IsFavorited = codeMarketResult.Data.Contains(item.Id);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[CodeMarketViewModel] 检查收藏状态失败: " + ex.Message);
		}
	}
}
