using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Behaviors;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class FamilyLibraryViewModel : ObservableObject
{
	private readonly IFamilyLibraryService _familyLibraryService;

	private readonly IAuthManager _authManager;

	private readonly FamilyDetailCacheManager _detailCacheManager;

	private readonly FamilyLibraryCacheManager _cacheManager;

	private readonly string _cachePath;

	private bool _isInitialized;

	private bool _isInitializing;

	private Timer? _quotaRefreshTimer;

	[ObservableProperty]
	private ObservableCollection<FamilyLibraryItem> _families = new ObservableCollection<FamilyLibraryItem>();

	[ObservableProperty]
	private int _thumbnailRefreshToken;

	[ObservableProperty]
	private ObservableCollection<FamilyCategory> _categories = new ObservableCollection<FamilyCategory>();

	[ObservableProperty]
	private ObservableCollection<FamilyCategoryNode> _categoryTree = new ObservableCollection<FamilyCategoryNode>();

	[ObservableProperty]
	private ObservableCollection<string> _revitCategories = new ObservableCollection<string>();

	[ObservableProperty]
	private FamilyCategory? _selectedCategory;

	[ObservableProperty]
	private FamilyCategoryNode? _selectedCategoryNode;

	[ObservableProperty]
	private string _searchText = string.Empty;

	[ObservableProperty]
	private string? _selectedRevitCategory;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private bool _isLoadingMore;

	[ObservableProperty]
	private DownloadQuotaInfo? _quotaInfo;

	[ObservableProperty]
	private FamilyLibraryItem? _selectedFamily;

	[ObservableProperty]
	private FamilyLibraryDetail? _selectedFamilyDetail;

	[ObservableProperty]
	private string _statusMessage = "就绪";

	[ObservableProperty]
	private bool _isLoadingQuota;

	private int _currentPage = 1;

	private const int PageSize = 50;

	private bool _hasMorePages = true;

	private bool _isLoadingFamilies;

	[ObservableProperty]
	private string _selectedFamilyType = string.Empty;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loadDataCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loadCategoriesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loadRevitCategoriesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loadFamiliesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loadQuotaCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? searchCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? loadMoreCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<FamilyLibraryItem?>? insertFamilyCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<FamilyLibraryItem?>? placeFamilyCommand;

	public string AuthorizationStatusText => GetAuthorizationStatusText();

	public string AuthorizationStatusColor => GetAuthorizationStatusColor();

	public FamilyDetailCacheManager DetailCacheManager => _detailCacheManager;

	public ICommand ScrollPreviewCommand => new RelayCommand<ScrollPreviewEventArgs>(OnScrollPreview);

	public string AuthorizationStatusTooltip => GetAuthorizationStatusTooltip();

	public List<ParameterGroup>? FilteredParameters
	{
		get
		{
			if (SelectedFamilyDetail?.TypeGroups == null || SelectedFamilyDetail.TypeGroups.Count == 0)
			{
				return null;
			}
			string currentType = _selectedFamilyType;
			FamilyTypeParameterGroup familyTypeParameterGroup = SelectedFamilyDetail.TypeGroups.FirstOrDefault((FamilyTypeParameterGroup g) => g.TypeName == currentType) ?? SelectedFamilyDetail.TypeGroups.FirstOrDefault();
			if (familyTypeParameterGroup == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(currentType) && SelectedFamilyDetail.TypeGroups.Count > 0)
			{
				SelectedFamilyType = SelectedFamilyDetail.TypeGroups[0].TypeName;
			}
			return (from p in familyTypeParameterGroup.Parameters.Where((FamilyParameter p) => !string.Equals(p.DefaultValue, "N/A", StringComparison.OrdinalIgnoreCase) && !string.Equals(p.DefaultValue, "N\\A", StringComparison.OrdinalIgnoreCase) && !string.Equals(p.ParameterName, "类型图像", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(p.DefaultValue)).ToList()
				group p by p.ParameterValueType ?? "其他" into g
				orderby g.Key
				select new ParameterGroup
				{
					GroupName = g.Key,
					Parameters = g.OrderBy((FamilyParameter p) => p.ParameterName).ToList()
				}).ToList();
		}
	}

	public List<string> FamilyTypes
	{
		get
		{
			if (SelectedFamilyDetail?.TypeGroups == null)
			{
				return new List<string>();
			}
			return (from g in SelectedFamilyDetail.TypeGroups
				select g.TypeName into t
				orderby t
				select t).ToList();
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<FamilyLibraryItem> Families
	{
		get
		{
			return _families;
		}
		[MemberNotNull("_families")]
		set
		{
			if (!EqualityComparer<ObservableCollection<FamilyLibraryItem>>.Default.Equals(_families, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Families);
				_families = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Families);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int ThumbnailRefreshToken
	{
		get
		{
			return _thumbnailRefreshToken;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_thumbnailRefreshToken, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ThumbnailRefreshToken);
				_thumbnailRefreshToken = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ThumbnailRefreshToken);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<FamilyCategory> Categories
	{
		get
		{
			return _categories;
		}
		[MemberNotNull("_categories")]
		set
		{
			if (!EqualityComparer<ObservableCollection<FamilyCategory>>.Default.Equals(_categories, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Categories);
				_categories = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Categories);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<FamilyCategoryNode> CategoryTree
	{
		get
		{
			return _categoryTree;
		}
		[MemberNotNull("_categoryTree")]
		set
		{
			if (!EqualityComparer<ObservableCollection<FamilyCategoryNode>>.Default.Equals(_categoryTree, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CategoryTree);
				_categoryTree = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CategoryTree);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<string> RevitCategories
	{
		get
		{
			return _revitCategories;
		}
		[MemberNotNull("_revitCategories")]
		set
		{
			if (!EqualityComparer<ObservableCollection<string>>.Default.Equals(_revitCategories, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.RevitCategories);
				_revitCategories = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.RevitCategories);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public FamilyCategory? SelectedCategory
	{
		get
		{
			return _selectedCategory;
		}
		set
		{
			if (!EqualityComparer<FamilyCategory>.Default.Equals(_selectedCategory, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedCategory);
				_selectedCategory = value;
				OnSelectedCategoryChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedCategory);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public FamilyCategoryNode? SelectedCategoryNode
	{
		get
		{
			return _selectedCategoryNode;
		}
		set
		{
			if (!EqualityComparer<FamilyCategoryNode>.Default.Equals(_selectedCategoryNode, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedCategoryNode);
				_selectedCategoryNode = value;
				OnSelectedCategoryNodeChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedCategoryNode);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SearchText
	{
		get
		{
			return _searchText;
		}
		[MemberNotNull("_searchText")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_searchText, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SearchText);
				_searchText = value;
				OnSearchTextChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SearchText);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? SelectedRevitCategory
	{
		get
		{
			return _selectedRevitCategory;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedRevitCategory, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedRevitCategory);
				_selectedRevitCategory = value;
				OnSelectedRevitCategoryChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedRevitCategory);
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
	public bool IsLoadingMore
	{
		get
		{
			return _isLoadingMore;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLoadingMore, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsLoadingMore);
				_isLoadingMore = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsLoadingMore);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public DownloadQuotaInfo? QuotaInfo
	{
		get
		{
			return _quotaInfo;
		}
		set
		{
			if (!EqualityComparer<DownloadQuotaInfo>.Default.Equals(_quotaInfo, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.QuotaInfo);
				_quotaInfo = value;
				OnQuotaInfoChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.QuotaInfo);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public FamilyLibraryItem? SelectedFamily
	{
		get
		{
			return _selectedFamily;
		}
		set
		{
			if (!EqualityComparer<FamilyLibraryItem>.Default.Equals(_selectedFamily, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedFamily);
				_selectedFamily = value;
				OnSelectedFamilyChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedFamily);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public FamilyLibraryDetail? SelectedFamilyDetail
	{
		get
		{
			return _selectedFamilyDetail;
		}
		set
		{
			if (!EqualityComparer<FamilyLibraryDetail>.Default.Equals(_selectedFamilyDetail, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedFamilyDetail);
				_selectedFamilyDetail = value;
				OnSelectedFamilyDetailChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedFamilyDetail);
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
	public bool IsLoadingQuota
	{
		get
		{
			return _isLoadingQuota;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLoadingQuota, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsLoadingQuota);
				_isLoadingQuota = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsLoadingQuota);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedFamilyType
	{
		get
		{
			return _selectedFamilyType;
		}
		[MemberNotNull("_selectedFamilyType")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedFamilyType, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedFamilyType);
				_selectedFamilyType = value;
				OnSelectedFamilyTypeChanged(value);
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedFamilyType);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoadDataCommand => loadDataCommand ?? (loadDataCommand = new AsyncRelayCommand(LoadDataAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoadCategoriesCommand => loadCategoriesCommand ?? (loadCategoriesCommand = new AsyncRelayCommand(LoadCategoriesAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoadRevitCategoriesCommand => loadRevitCategoriesCommand ?? (loadRevitCategoriesCommand = new AsyncRelayCommand(LoadRevitCategoriesAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoadFamiliesCommand => loadFamiliesCommand ?? (loadFamiliesCommand = new AsyncRelayCommand(LoadFamiliesAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoadQuotaCommand => loadQuotaCommand ?? (loadQuotaCommand = new AsyncRelayCommand(LoadQuotaAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SearchCommand => searchCommand ?? (searchCommand = new AsyncRelayCommand(SearchAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand LoadMoreCommand => loadMoreCommand ?? (loadMoreCommand = new AsyncRelayCommand(LoadMoreAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<FamilyLibraryItem?> InsertFamilyCommand => insertFamilyCommand ?? (insertFamilyCommand = new AsyncRelayCommand<FamilyLibraryItem>(InsertFamilyAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<FamilyLibraryItem?> PlaceFamilyCommand => placeFamilyCommand ?? (placeFamilyCommand = new AsyncRelayCommand<FamilyLibraryItem>(PlaceFamilyAsync));

	public void RefreshFamiliesCollection()
	{
		OnPropertyChanged("Families");
	}

	public FamilyLibraryViewModel(IFamilyLibraryService familyLibraryService, IAuthManager authManager)
	{
		_familyLibraryService = familyLibraryService ?? throw new ArgumentNullException("familyLibraryService");
		_authManager = authManager ?? throw new ArgumentNullException("authManager");
		_detailCacheManager = new FamilyDetailCacheManager(_familyLibraryService);
		_cacheManager = new FamilyLibraryCacheManager();
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "FamilyLibrary");
		_cachePath = Path.Combine(path, "FamilyFiles");
		Directory.CreateDirectory(_cachePath);
		Categories.Add(new FamilyCategory
		{
			Id = Guid.Empty,
			Name = "全部",
			DisplayOrder = 0
		});
		_isInitialized = false;
	}

	public async Task InitializeAsync()
	{
		if (_isInitialized)
		{
			return;
		}
		_isInitialized = true;
		_isInitializing = true;
		try
		{
			SelectedCategory = Categories[0];
			await LoadCachedDataSync();
			RefreshDataInBackground();
			StartQuotaRefreshTimer();
			_authManager.AuthorizationStateChanged += OnAuthorizationStateChanged;
			Task.Run(async delegate
			{
				await Task.Delay(TimeSpan.FromMinutes(5L));
				_cacheManager.CleanExpiredCache();
			});
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibrary] InitializeAsync 异常", ex);
			throw;
		}
		finally
		{
			_isInitializing = false;
		}
		await LoadFamiliesAsync();
	}

	private Task LoadCachedDataSync()
	{
		return Task.CompletedTask;
	}

	private async Task RefreshDataInBackground()
	{
		_ = 2;
		try
		{
			await Task.Delay(500);
			List<FamilyCategory> categories;
			List<FamilyLibraryItem> families;
			DownloadQuotaInfo quota;
			List<RevitCategoryItem> revitCategories;
			(categories, families, quota, revitCategories) = await FetchAllDataAsync();
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
			{
				try
				{
					if (categories != null)
					{
						FamilyCategory item = Categories[0];
						Categories.Clear();
						Categories.Add(item);
						BuildCategoryTree(categories);
						foreach (FamilyCategory item2 in categories.OrderBy((FamilyCategory c) => c.Name))
						{
							Categories.Add(item2);
						}
					}
					if (quota != null)
					{
						QuotaInfo = quota;
					}
					if (revitCategories != null && revitCategories.Count > 0)
					{
						RevitCategories.Clear();
						foreach (RevitCategoryItem item3 in revitCategories)
						{
							RevitCategories.Add(item3.RevitCategoryName);
						}
					}
					else if (families != null && families.Count > 0)
					{
						List<string> list = (from c in (from f in families
								where !string.IsNullOrEmpty(f.RevitCategoryName)
								select f.RevitCategoryName).Distinct()
							orderby c
							select c).ToList();
						RevitCategories.Clear();
						foreach (string item4 in list)
						{
							RevitCategories.Add(item4);
						}
					}
					StatusMessage = ((families != null) ? $"已加载 {families.Count} 个族" : "加载完成");
				}
				catch (Exception ex2)
				{
					Logger.Error("[FamilyLibrary] 更新界面失败", ex2);
				}
			}, (DispatcherPriority)7);
			if (families == null || families.Count <= 0)
			{
				return;
			}
			Task.Run(async delegate
			{
				try
				{
					Guid[] familyIds = (from f in families.Take(50)
						select f.Id).ToArray();
					_detailCacheManager.PrefetchDetailsBatch(familyIds);
					await Task.WhenAll((from f in families.Take(20)
						where !string.IsNullOrEmpty(f.ThumbnailUrl)
						select _cacheManager.LoadThumbnailAsync(f.Id, f.ThumbnailUrl)).ToArray());
				}
				catch (Exception ex2)
				{
					Logger.Warning("[FamilyLibrary] 预加载失败: " + ex2.Message);
				}
			});
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibrary] 后台刷新失败", ex);
		}
	}

	private async Task<(List<FamilyCategory>? categories, List<FamilyLibraryItem>? families, DownloadQuotaInfo? quota, List<RevitCategoryItem>? revitCategories)> FetchAllDataAsync()
	{
		try
		{
			Task<Result<List<FamilyCategory>>> categoriesTask = _familyLibraryService.GetCategoriesAsync();
			Task<Result<List<FamilyLibraryItem>>> familiesTask = _familyLibraryService.GetFamiliesAsync(null, null, null, null, null, null, 1, 100);
			Task<Result<List<RevitCategoryItem>>> revitCategoriesTask = _familyLibraryService.GetRevitCategoriesAsync();
			InlineArray3<Task> buffer = default(InlineArray3<Task>);
			buffer[0] = categoriesTask;
			buffer[1] = familiesTask;
			buffer[2] = revitCategoriesTask;
			await Task.WhenAll(buffer);
			List<FamilyCategory> item = (categoriesTask.Result.IsSuccess ? categoriesTask.Result.Value : null);
			List<FamilyLibraryItem> list = (familiesTask.Result.IsSuccess ? familiesTask.Result.Value : null);
			List<RevitCategoryItem> item2 = (revitCategoriesTask.Result.IsSuccess ? revitCategoriesTask.Result.Value : null);
			AuthorizationState authorizationState = _authManager.GetAuthorizationState();
			Result<DownloadQuotaInfo> result = _familyLibraryService.CalculateDownloadQuotaLocally(authorizationState);
			DownloadQuotaInfo item3 = (result.IsSuccess ? result.Value : null);
			if (list != null)
			{
				_cacheManager.SaveFamiliesCache(list, null, string.Empty);
			}
			return (categories: item, families: list, quota: item3, revitCategories: item2);
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyLibrary] 获取数据失败: " + ex.Message);
			return (categories: null, families: null, quota: null, revitCategories: null);
		}
	}

	private async Task PrefetchAllDetailsAsync(List<FamilyLibraryItem> families)
	{
		try
		{
			foreach (FamilyLibraryItem family in families)
			{
				try
				{
					await _detailCacheManager.GetDetailsAsync(family.Id);
				}
				catch
				{
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyLibrary] 预加载详情失败: " + ex.Message);
		}
	}

	private async Task LoadNetworkDataAsync()
	{
		_ = 1;
		try
		{
			InlineArray3<Task> buffer = default(InlineArray3<Task>);
			buffer[0] = LoadCategoriesAsync();
			buffer[1] = LoadRevitCategoriesAsync();
			buffer[2] = LoadQuotaAsync();
			await Task.WhenAll(buffer);
			await LoadFamiliesAsync();
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyLibrary] 网络加载失败: " + ex.Message);
		}
	}

	[RelayCommand]
	private async Task LoadDataAsync()
	{
		InlineArray3<Task> buffer = default(InlineArray3<Task>);
		buffer[0] = LoadCategoriesAsync();
		buffer[1] = LoadRevitCategoriesAsync();
		buffer[2] = LoadQuotaAsync();
		await Task.WhenAll(buffer);
		await LoadFamiliesAsync();
	}

	[RelayCommand]
	private async Task LoadCategoriesAsync()
	{
		try
		{
			Result<List<FamilyCategory>> result = await _familyLibraryService.GetCategoriesAsync();
			if (!result.IsSuccess || result.Value == null)
			{
				return;
			}
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
			{
				FamilyCategory item = Categories[0];
				Categories.Clear();
				Categories.Add(item);
				BuildCategoryTree(result.Value);
				foreach (FamilyCategory item2 in result.Value.OrderBy((FamilyCategory c) => c.Name))
				{
					Categories.Add(item2);
				}
			});
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibrary] 加载分类失败", ex);
		}
	}

	private void BuildCategoryTree(List<FamilyCategory> categories)
	{
		CategoryTree.Clear();
		FamilyCategoryNode item = new FamilyCategoryNode(new FamilyCategory
		{
			Id = Guid.Empty,
			Name = "全部",
			DisplayOrder = -1
		});
		CategoryTree.Add(item);
		Dictionary<Guid, FamilyCategory> categoryMap = categories.ToDictionary((FamilyCategory c) => c.Id);
		foreach (FamilyCategory item3 in (from c in categories
			where !c.ParentId.HasValue
			orderby c.DisplayOrder, c.Name
			select c).ToList())
		{
			FamilyCategoryNode item2 = CreateCategoryNode(item3, categories, categoryMap);
			CategoryTree.Add(item2);
		}
	}

	private FamilyCategoryNode CreateCategoryNode(FamilyCategory category, List<FamilyCategory> allCategories, Dictionary<Guid, FamilyCategory> categoryMap)
	{
		FamilyCategoryNode familyCategoryNode = new FamilyCategoryNode(category);
		foreach (FamilyCategory item2 in (from c in allCategories
			where c.ParentId == category.Id
			orderby c.DisplayOrder, c.Name
			select c).ToList())
		{
			FamilyCategoryNode item = CreateCategoryNode(item2, allCategories, categoryMap);
			familyCategoryNode.SubCategories.Add(item);
		}
		return familyCategoryNode;
	}

	[RelayCommand]
	private async Task LoadRevitCategoriesAsync()
	{
		try
		{
			Result<List<RevitCategoryItem>> result = await _familyLibraryService.GetRevitCategoriesAsync();
			if (!result.IsSuccess || result.Value == null)
			{
				return;
			}
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
			{
				RevitCategories.Clear();
				foreach (RevitCategoryItem item in result.Value)
				{
					RevitCategories.Add(item.RevitCategoryName);
				}
			});
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyLibrary] 加载 Revit 类别异常: " + ex.Message);
		}
	}

	[RelayCommand]
	private async Task LoadFamiliesAsync()
	{
		if (_isLoadingFamilies)
		{
			return;
		}
		_isLoadingFamilies = true;
		StatusMessage = "加载中...";
		try
		{
			Guid? categoryId = ((SelectedCategory?.Id == Guid.Empty) ? ((Guid?)null) : SelectedCategory?.Id);
			string text = (string.IsNullOrWhiteSpace(SearchText) ? null : SearchText);
			string text2 = SelectedRevitCategory;
			if (!string.IsNullOrEmpty(text) && text.Contains("revit_category:"))
			{
				string[] array = text.Split(new string[1] { "revit_category:" }, StringSplitOptions.None);
				if (array.Length > 1)
				{
					text2 = array[1].Trim();
					text = null;
				}
			}
			IFamilyLibraryService familyLibraryService = _familyLibraryService;
			string searchText = text;
			string revitCategoryName = text2;
			int currentPage = _currentPage;
			Result<List<FamilyLibraryItem>> result = await familyLibraryService.GetFamiliesAsync(categoryId, searchText, null, null, revitCategoryName, null, currentPage);
			if (!result.IsSuccess || result.Value == null)
			{
				StatusMessage = "加载失败: " + result.Error;
				return;
			}
			List<FamilyLibraryItem> families = result.Value;
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
			{
				if (_currentPage == 1)
				{
					Families.Clear();
				}
				foreach (FamilyLibraryItem item in families)
				{
					Families.Add(item);
				}
			});
			_hasMorePages = families.Count == 50;
			if (_hasMorePages)
			{
				StatusMessage = $"已加载 {Families.Count} 个族（第{_currentPage}页，返回{families.Count}个）";
			}
			else
			{
				StatusMessage = $"已加载 {Families.Count} 个族";
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibrary] 加载族列表失败", ex);
			StatusMessage = "加载失败: " + ex.Message;
		}
		finally
		{
			_isLoadingFamilies = false;
		}
	}

	public void OnFamilyItemVisible(Guid familyId)
	{
		_detailCacheManager.PrefetchDetails(familyId);
	}

	public void PrefetchFamilyDetails(params Guid[] familyIds)
	{
		_detailCacheManager.PrefetchDetailsBatch(familyIds);
	}

	public async Task<FamilyLibraryDetail?> GetFamilyDetailsAsync(Guid familyId)
	{
		return await _detailCacheManager.GetDetailsAsync(familyId);
	}

	private void OnScrollPreview(ScrollPreviewEventArgs? args)
	{
		if (args != null)
		{
			Guid[] array = (from e in args.GetVisibleChildren().OfType<FrameworkElement>()
				where e.Tag is Guid
				select (Guid)e.Tag).ToArray();
			if (array.Length != 0)
			{
				_detailCacheManager.PrefetchDetailsBatch(array);
			}
		}
	}

	[RelayCommand]
	private async Task LoadQuotaAsync()
	{
		try
		{
			IsLoadingQuota = true;
			AuthorizationState authorizationState = _authManager.GetAuthorizationState();
			Result<DownloadQuotaInfo> result = await _familyLibraryService.CalculateDownloadQuotaLocallyAsync(authorizationState);
			if (result.IsSuccess && result.Value != null)
			{
				QuotaInfo = result.Value;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyLibrary] 获取配额失败: " + ex.Message);
		}
		finally
		{
			IsLoadingQuota = false;
		}
	}

	[RelayCommand]
	private async Task SearchAsync()
	{
		_currentPage = 1;
		_hasMorePages = true;
		await LoadFamiliesAsync();
	}

	[RelayCommand]
	private async Task LoadMoreAsync()
	{
		if (_hasMorePages && !_isLoadingFamilies)
		{
			_currentPage++;
			await LoadFamiliesAsync();
		}
	}

	[RelayCommand]
	private async Task InsertFamilyAsync(FamilyLibraryItem? family)
	{
		if (family == null)
		{
			return;
		}
		try
		{
			IsLoading = true;
			StatusMessage = "准备载入 " + family.DisplayName + "...";
			string cachedFile = Path.Combine(_cachePath, $"{family.Id}.rfa");
			bool flag = File.Exists(cachedFile);
			bool needCountQuota = true;
			if (!family.IsFree)
			{
				if (_familyLibraryService.IsFamilyDownloadedToday(family.Id))
				{
					needCountQuota = false;
				}
				else if (QuotaInfo != null && QuotaInfo.Remaining <= 0)
				{
					StatusMessage = "下载配额已用完";
					if (QuotaInfo.DailyLimit == 0)
					{
						MessageBox.Show("试用用户无法载入族文件。\n\n请购买许可证后使用族库功能。", "载入限制", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						return;
					}
					MessageBox.Show($"24小时下载次数已达到限制（{QuotaInfo.DownloadedToday}/{QuotaInfo.DailyLimit}）\n\n增加授权时长可提升24小时下载配额。\n配额计算：剩余天数÷10×3，最小3次。", "载入限制", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
			}
			if (flag)
			{
				StatusMessage = "使用本地缓存...";
				await LoadFamilyIntoRevitAsync(cachedFile, family.DisplayName);
				if (needCountQuota)
				{
					await _familyLibraryService.RecordDownloadLocallyAsync(family.Id);
					await LoadQuotaAsync();
				}
				StatusMessage = "已载入 " + family.DisplayName;
				return;
			}
			Result<FamilyDownloadInfo> result = await _familyLibraryService.RequestDownloadAsync(family.Id);
			if (!result.IsSuccess || result.Value == null)
			{
				StatusMessage = "下载失败: " + result.Error;
				MessageBox.Show("下载请求失败: " + result.Error, "下载失败", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			FamilyDownloadInfo value = result.Value;
			if (!value.CanDownload)
			{
				StatusMessage = value.Message ?? "无法下载";
				MessageBox.Show(value.Message ?? "无法下载此族", "下载限制", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			StatusMessage = "下载中...";
			string text = await DownloadFileAsync(value.FileUrl, cachedFile);
			if (File.Exists(text))
			{
				await LoadFamilyIntoRevitAsync(text, family.DisplayName);
				if (needCountQuota)
				{
					await _familyLibraryService.RecordDownloadLocallyAsync(family.Id);
					await LoadQuotaAsync();
				}
				StatusMessage = "已加载 " + family.DisplayName;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibrary] 插入族失败", ex);
			StatusMessage = "插入失败: " + ex.Message;
			MessageBox.Show("插入族失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			IsLoading = false;
		}
	}

	private async Task<string> DownloadFileAsync(string url, string destinationPath)
	{
		using HttpClient client = new HttpClient();
		client.Timeout = TimeSpan.FromMinutes(5L);
		HttpResponseMessage obj = await client.GetAsync(url);
		obj.EnsureSuccessStatusCode();
		string tempPath = destinationPath + ".tmp";
		using (Stream stream = await obj.Content.ReadAsStreamAsync())
		{
			using FileStream fileStream = File.Create(tempPath);
			await stream.CopyToAsync(fileStream);
		}
		if (File.Exists(destinationPath))
		{
			File.Delete(destinationPath);
		}
		File.Move(tempPath, destinationPath);
		Logger.Info("[FamilyLibrary] 文件下载完成: " + destinationPath);
		return destinationPath;
	}

	private async Task LoadFamilyIntoRevitAsync(string filePath, string displayName)
	{
		try
		{
			object obj = null;
			Type type = null;
			Type type2 = null;
			try
			{
				Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name?.StartsWith("RevitAi.Revit") ?? false);
				if (assembly != null)
				{
					Type type3 = assembly.GetType("RevitAi.Revit.UI.RevitAdapterManager");
					if (type3 == null)
					{
						type3 = assembly.GetTypes().FirstOrDefault((Type t) => t.Name == "RevitAdapterManager" && t.IsClass && t.IsAbstract && t.IsSealed);
					}
					if (type3 != null)
					{
						object obj2 = type3.GetProperty("CurrentAdapter")?.GetValue(null);
						if (obj2 != null)
						{
							obj = obj2.GetType().GetMethod("GetFamilyLoadExternalEvent")?.Invoke(obj2, null);
							type = assembly.GetType("RevitAi.Revit.Revit.FamilyLoadRequestManager");
							if (type == null)
							{
								type = assembly.GetTypes().FirstOrDefault((Type t) => t.Name == "FamilyLoadRequestManager");
							}
							type2 = assembly.GetType("RevitAi.Revit.Revit.FamilyLoadRequest");
							if (type2 == null)
							{
								type2 = assembly.GetTypes().FirstOrDefault((Type t) => t.Name == "FamilyLoadRequest");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[FamilyLibrary] 通过反射获取族加载 ExternalEvent 失败: " + ex.Message);
			}
			if (obj == null || type == null || type2 == null)
			{
				((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
				{
					MessageBox.Show("无法连接到 Revit，请确保 Revit 正在运行。", "加载失败", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				});
				return;
			}
			object obj3 = Activator.CreateInstance(type2);
			type2.GetProperty("FilePath")?.SetValue(obj3, filePath);
			type2.GetProperty("FamilyName")?.SetValue(obj3, displayName);
			object obj4 = type2.GetProperty("TaskSource")?.GetValue(obj3);
			if (obj4 == null)
			{
				Logger.Warning("[FamilyLibrary] 无法获取 TaskCompletionSource");
				return;
			}
			type.GetMethod("SetRequest")?.Invoke(null, new object[1] { obj3 });
			obj.GetType().GetMethod("Raise")?.Invoke(obj, null);
			if (!(obj4.GetType().GetProperty("Task")?.GetValue(obj4) is Task<object> task))
			{
				return;
			}
			object obj5 = await task;
			PropertyInfo propertyInfo = obj5?.GetType().GetProperty("Success");
			PropertyInfo propertyInfo2 = obj5?.GetType().GetProperty("Error");
			if (!(propertyInfo != null))
			{
				return;
			}
			if ((bool?)propertyInfo.GetValue(obj5) == true)
			{
				StatusMessage = "已加载 " + displayName;
				return;
			}
			string error = propertyInfo2?.GetValue(obj5) as string;
			Logger.Warning("[FamilyLibrary] 族加载失败: " + displayName + " - " + error);
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				MessageBox.Show("族加载失败: " + error, "加载失败", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			});
		}
		catch (Exception ex2)
		{
			Exception ex3 = ex2;
			Exception ex4 = ex3;
			Logger.Error("[FamilyLibrary] 加载族到 Revit 失败", ex4);
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				MessageBox.Show("加载族时发生错误: " + ex4.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			});
		}
	}

	[RelayCommand]
	private async Task PlaceFamilyAsync(FamilyLibraryItem? family)
	{
		if (family == null)
		{
			return;
		}
		try
		{
			IsLoading = true;
			StatusMessage = "准备载入并放置 " + family.DisplayName + "...";
			string cachedFile = Path.Combine(_cachePath, $"{family.Id}.rfa");
			bool hasLocalCache = File.Exists(cachedFile);
			bool needCountQuota = true;
			if (!family.IsFree)
			{
				if (_familyLibraryService.IsFamilyDownloadedToday(family.Id))
				{
					needCountQuota = false;
				}
				else if (QuotaInfo != null && QuotaInfo.Remaining <= 0)
				{
					StatusMessage = "下载配额已用完";
					if (QuotaInfo.DailyLimit == 0)
					{
						MessageBox.Show("试用用户无法载入族文件。\n\n请购买许可证后使用族库功能。", "载入限制", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						return;
					}
					MessageBox.Show($"24小时下载次数已达到限制（{QuotaInfo.DownloadedToday}/{QuotaInfo.DailyLimit}）\n\n增加授权时长可提升24小时下载配额。\n配额计算：剩余天数÷10×3，最小3次。", "载入限制", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
			}
			string filePath = cachedFile;
			if (!hasLocalCache)
			{
				Result<FamilyDownloadInfo> result = await _familyLibraryService.RequestDownloadAsync(family.Id);
				if (!result.IsSuccess || result.Value == null)
				{
					StatusMessage = "下载失败: " + result.Error;
					MessageBox.Show("下载请求失败: " + result.Error, "下载失败", MessageBoxButton.OK, MessageBoxImage.Hand);
					return;
				}
				FamilyDownloadInfo value = result.Value;
				if (!value.CanDownload)
				{
					StatusMessage = value.Message ?? "无法下载";
					MessageBox.Show(value.Message ?? "无法下载此族", "下载限制", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
				StatusMessage = "下载中...";
				filePath = await DownloadFileAsync(value.FileUrl, cachedFile);
			}
			await LoadAndPlaceFamilyIntoRevitAsync(filePath, family.DisplayName);
			if (needCountQuota && !hasLocalCache)
			{
				await _familyLibraryService.RecordDownloadLocallyAsync(family.Id);
				await LoadQuotaAsync();
			}
			StatusMessage = "已加载 " + family.DisplayName + "，请在模型中点击放置";
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				foreach (Window window in Application.Current.Windows)
				{
					if (window is FamilyLibraryWindow)
					{
						window.WindowState = WindowState.Minimized;
						break;
					}
				}
			});
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibrary] 放置族失败", ex);
			StatusMessage = "放置失败: " + ex.Message;
			MessageBox.Show("放置族失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			IsLoading = false;
		}
	}

	private async Task LoadAndPlaceFamilyIntoRevitAsync(string filePath, string displayName)
	{
		try
		{
			object obj = null;
			Type type = null;
			Type type2 = null;
			try
			{
				Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name?.StartsWith("RevitAi.Revit") ?? false);
				if (assembly != null)
				{
					Type type3 = assembly.GetType("RevitAi.Revit.UI.RevitAdapterManager");
					if (type3 == null)
					{
						type3 = assembly.GetTypes().FirstOrDefault((Type t) => t.Name == "RevitAdapterManager" && t.IsClass && t.IsAbstract && t.IsSealed);
					}
					if (type3 != null)
					{
						object obj2 = type3.GetProperty("CurrentAdapter")?.GetValue(null);
						if (obj2 != null)
						{
							obj = obj2.GetType().GetMethod("GetFamilyPlacementExternalEvent")?.Invoke(obj2, null);
							type = assembly.GetType("RevitAi.Revit.Revit.FamilyPlacementRequestManager");
							if (type == null)
							{
								type = assembly.GetTypes().FirstOrDefault((Type t) => t.Name == "FamilyPlacementRequestManager");
							}
							type2 = assembly.GetType("RevitAi.Revit.Revit.FamilyPlacementRequest");
							if (type2 == null)
							{
								type2 = assembly.GetTypes().FirstOrDefault((Type t) => t.Name == "FamilyPlacementRequest");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[FamilyLibrary] 通过反射获取族放置 ExternalEvent 失败: " + ex.Message);
			}
			if (obj == null || type == null || type2 == null)
			{
				((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
				{
					MessageBox.Show("无法连接到 Revit，请确保 Revit 正在运行。", "加载失败", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				});
				return;
			}
			object obj3 = Activator.CreateInstance(type2);
			type2.GetProperty("FilePath")?.SetValue(obj3, filePath);
			type2.GetProperty("FamilyName")?.SetValue(obj3, displayName);
			object obj4 = type2.GetProperty("TaskSource")?.GetValue(obj3);
			if (obj4 == null)
			{
				Logger.Warning("[FamilyLibrary] 无法获取 TaskCompletionSource");
				return;
			}
			type.GetMethod("SetRequest")?.Invoke(null, new object[1] { obj3 });
			obj.GetType().GetMethod("Raise")?.Invoke(obj, null);
			if (!(obj4.GetType().GetProperty("Task")?.GetValue(obj4) is Task<object> task))
			{
				return;
			}
			object obj5 = await task;
			PropertyInfo propertyInfo = obj5?.GetType().GetProperty("Success");
			PropertyInfo propertyInfo2 = obj5?.GetType().GetProperty("Error");
			if (!(propertyInfo != null))
			{
				return;
			}
			if ((bool?)propertyInfo.GetValue(obj5) == true)
			{
				StatusMessage = "已激活 " + displayName + "，请点击模型放置";
				return;
			}
			string error = propertyInfo2?.GetValue(obj5) as string;
			Logger.Warning("[FamilyLibrary] 族放置失败: " + displayName + " - " + error);
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				MessageBox.Show("族放置失败: " + error, "放置失败", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			});
		}
		catch (Exception ex2)
		{
			Exception ex3 = ex2;
			Exception ex4 = ex3;
			Logger.Error("[FamilyLibrary] 加载并放置族到 Revit 失败", ex4);
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				MessageBox.Show("加载族时发生错误: " + ex4.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			});
		}
	}

	public string GetLocalThumbnailPath(Guid familyId)
	{
		return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "FamilyLibrary", "Thumbnails"), $"{familyId}.jpg");
	}

	public bool IsThumbnailCached(Guid familyId)
	{
		return File.Exists(GetLocalThumbnailPath(familyId));
	}

	public Visibility GetThumbnailVisibility(Guid familyId, string? remoteUrl)
	{
		if (IsThumbnailCached(familyId))
		{
			return Visibility.Collapsed;
		}
		string.IsNullOrEmpty(remoteUrl);
		return Visibility.Visible;
	}

	public BitmapImage? GetThumbnail(Guid familyId, string? remoteUrl)
	{
		try
		{
			string localPath = GetLocalThumbnailPath(familyId);
			if (File.Exists(localPath))
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.UriSource = new Uri(localPath);
				bitmapImage.EndInit();
				((Freezable)bitmapImage).Freeze();
				return bitmapImage;
			}
			if (!string.IsNullOrEmpty(remoteUrl))
			{
				Task.Run(async delegate
				{
					_ = 2;
					try
					{
						using HttpClient client = new HttpClient();
						client.Timeout = TimeSpan.FromSeconds(10L);
						HttpResponseMessage httpResponseMessage = await client.GetAsync(remoteUrl);
						if (httpResponseMessage.IsSuccessStatusCode)
						{
							byte[] bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();
							File.WriteAllBytes(localPath, bytes);
							await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
							{
								OnPropertyChanged("Families");
							});
						}
					}
					catch
					{
					}
				});
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string FormatFileSize(long? bytes)
	{
		if (!bytes.HasValue)
		{
			return "-";
		}
		long value = bytes.Value;
		if (value >= 1024)
		{
			if (value < 1048576)
			{
				return $"{bytes.Value / 1024:F1} KB";
			}
			return $"{bytes.Value / 1048576:F1} MB";
		}
		return $"{bytes.Value} B";
	}

	private string GetAuthorizationStatusText()
	{
		if (QuotaInfo == null)
		{
			return "加载中...";
		}
		if (QuotaInfo.DaysRemaining <= 0)
		{
			return "试用/到期";
		}
		if (QuotaInfo.DaysRemaining >= 29200)
		{
			return "永久授权";
		}
		if (QuotaInfo.DaysRemaining > 365)
		{
			return $"剩余 {QuotaInfo.DaysRemaining / 365} 年";
		}
		if (QuotaInfo.DaysRemaining > 30)
		{
			return $"剩余 {QuotaInfo.DaysRemaining / 30} 个月";
		}
		if (QuotaInfo.DaysRemaining > 0)
		{
			return $"剩余 {QuotaInfo.DaysRemaining} 天";
		}
		return "试用/到期";
	}

	private string GetAuthorizationStatusColor()
	{
		if (QuotaInfo == null)
		{
			return "#888888";
		}
		if (QuotaInfo.DaysRemaining <= 0)
		{
			return "#F44336";
		}
		if (QuotaInfo.DaysRemaining <= 7)
		{
			return "#FF9800";
		}
		if (QuotaInfo.DaysRemaining <= 30)
		{
			return "#FFC107";
		}
		return "#4CAF50";
	}

	private string GetAuthorizationStatusTooltip()
	{
		if (QuotaInfo == null)
		{
			return "正在加载授权信息...";
		}
		if (QuotaInfo.DaysRemaining <= 0)
		{
			return "试用用户：无法下载族文件\n\n请购买许可证后使用族库功能。";
		}
		if (QuotaInfo.DaysRemaining < 3650)
		{
			int daysRemaining = QuotaInfo.DaysRemaining;
			string text = ((daysRemaining <= 7) ? $"即将到期（剩余 {QuotaInfo.DaysRemaining} 天）" : ((daysRemaining > 30) ? $"授权有效期充足（剩余 {QuotaInfo.DaysRemaining} 天）" : $"授权有效期不足一个月（剩余 {QuotaInfo.DaysRemaining} 天）"));
			string value = text;
			return $"{value}\n\n每日下载限额：{QuotaInfo.DailyLimit} 次\n计算公式：剩余天数 ÷ 10 × 3（最小 3 次）";
		}
		return $"永久授权\n\n每日下载限额：{QuotaInfo.DailyLimit} 次";
	}

	private void StartQuotaRefreshTimer()
	{
		_quotaRefreshTimer = new Timer(async delegate
		{
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
			{
				try
				{
					await LoadQuotaAsync();
				}
				catch (Exception ex)
				{
					Logger.Warning("[FamilyLibrary] 自动刷新配额失败: " + ex.Message);
				}
			});
		}, null, TimeSpan.FromSeconds(30L), TimeSpan.FromSeconds(30L));
	}

	public void StopQuotaRefreshTimer()
	{
		_quotaRefreshTimer?.Dispose();
		_quotaRefreshTimer = null;
	}

	public void Cleanup()
	{
		StopQuotaRefreshTimer();
		_authManager.AuthorizationStateChanged -= OnAuthorizationStateChanged;
	}

	private void OnAuthorizationStateChanged(object? sender, AuthorizationStateChangedEventArgs e)
	{
		Logger.Info("[FamilyLibrary] 收到授权状态变化事件，刷新配额信息");
		((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
		{
			await LoadQuotaAsync();
		});
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedCategoryChanged(FamilyCategory? value)
	{
		if (!_isInitializing && value != null)
		{
			_isInitializing = true;
			SelectedRevitCategory = null;
			_isInitializing = false;
			_currentPage = 1;
			_hasMorePages = true;
			LoadFamiliesAsync();
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedCategoryNodeChanged(FamilyCategoryNode? value)
	{
		if (value != null && value.Category != null)
		{
			SelectedCategory = value.Category;
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSearchTextChanged(string value)
	{
		if (_isInitializing)
		{
			return;
		}
		Task.Run(async delegate
		{
			await Task.Delay(500);
			if (SearchText == value)
			{
				_currentPage = 1;
				_hasMorePages = true;
				await LoadFamiliesAsync();
			}
		});
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedRevitCategoryChanged(string? value)
	{
		if (!_isInitializing && !string.IsNullOrEmpty(value))
		{
			_isInitializing = true;
			SelectedCategory = Categories.FirstOrDefault((FamilyCategory c) => c.Id == Guid.Empty);
			_isInitializing = false;
			_currentPage = 1;
			_hasMorePages = true;
			LoadFamiliesAsync();
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnQuotaInfoChanged(DownloadQuotaInfo? value)
	{
		try
		{
			OnPropertyChanged("AuthorizationStatusText");
			OnPropertyChanged("AuthorizationStatusColor");
			OnPropertyChanged("AuthorizationStatusTooltip");
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibrary] OnQuotaInfoChanged 异常", ex);
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedFamilyChanged(FamilyLibraryItem? value)
	{
		if (value == null)
		{
			SelectedFamilyDetail = null;
			return;
		}
		Task.Run(async delegate
		{
			_ = 1;
			try
			{
				FamilyLibraryDetail details = await _detailCacheManager.GetDetailsAsync(value.Id);
				if (details != null)
				{
					await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
					{
						SelectedFamilyDetail = details;
					});
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[FamilyLibrary] 加载族详情异常: " + value.DisplayName, ex);
			}
		});
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedFamilyDetailChanged(FamilyLibraryDetail? value)
	{
		_selectedFamilyType = string.Empty;
		OnPropertyChanged("FilteredParameters");
		OnPropertyChanged("FamilyTypes");
		OnPropertyChanged("SelectedFamilyType");
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	private void OnSelectedFamilyTypeChanged(string value)
	{
		OnPropertyChanged("FilteredParameters");
	}
}
