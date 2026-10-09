using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Services;
using RevitAi.Abstractions.Update;
using RevitAi.Abstractions.Update.Models;
using RevitAi.Core;
using RevitAi.Core.AI;
using RevitAi.Core.Authentication;
using RevitAi.Core.FamilyLibrary;
using RevitAi.Core.Feedback;
using RevitAi.Core.Infrastructure;
using RevitAi.Core.Security;
using RevitAi.Core.Services;
using RevitAi.UI.Models;
using RevitAi.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ServiceProvider = RevitAi.Abstractions.Loader.ServiceProvider;

namespace RevitAi.UI.Services;

public sealed class UIBootstrapper
{
	private sealed class NoOpUpdateService : IUpdateService
	{
		public Task<UpdateInfo?> CheckForUpdatesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult<UpdateInfo>(null);
		}

		public Task<string> DownloadUpdateAsync(UpdateInfo updateInfo, Action<int>? progressCallback = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult(string.Empty);
		}

		public bool VerifyUpdatePackage(string filePath, string expectedChecksum)
		{
			return false;
		}

		public Task PrepareUpdateAsync(string packagePath, UpdateInfo updateInfo, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.CompletedTask;
		}

		public bool HasPendingUpdate()
		{
			return false;
		}

		public UpdateInfo? GetPendingUpdateInfo()
		{
			return null;
		}

		public string GetCurrentVersion()
		{
			return "0.0.0";
		}

		public Task<string?> DownloadTextAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult<string>(null);
		}

		public Task<LatestVersionInfo?> GetVersionInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult<LatestVersionInfo>(null);
		}
	}

	private static IHost? _host;

	private static IServiceProvider? _services;

	private static Application? _wpfApplication;

	public static IServiceProvider Services => _services ?? throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");

	public static async Task InitializeAsync(IAuthManager? authManager = null, INativeCryptoService? cryptoService = null, ISupabaseClient? supabaseClient = null)
	{
		_host = new HostBuilder().ConfigureServices(delegate(IServiceCollection services)
		{
			if (authManager != null)
			{
				services.AddSingleton(authManager);
			}
			if (cryptoService != null)
			{
				services.AddSingleton(cryptoService);
			}
			if (supabaseClient != null)
			{
				services.AddSingleton(supabaseClient);
			}
			if (cryptoService != null && supabaseClient != null)
			{
				ApiKeyManager implementationInstance = new ApiKeyManager(cryptoService, supabaseClient);
				services.AddSingleton(implementationInstance);
			}
			ConfigureServices(services, authManager, cryptoService, supabaseClient);
		}).ConfigureLogging(delegate(ILoggingBuilder logging)
		{
			logging.ClearProviders();
			logging.AddConsole();
		}).Build();
		await _host.StartAsync();
		_services = _host.Services;
		RegisterServicesToServiceProvider();
		try
		{
			LoadResources();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] 加载资源字典失败", ex);
		}
		if (authManager == null)
		{
			await _services.GetRequiredService<IAuthManager>().InitializeAsync();
		}
	}

	public static void ShowLoginWindow()
	{
		if (_services == null)
		{
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowLoginWindow();
	}

	public static void ShowMainWindow()
	{
		if (_services == null)
		{
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowMainWindow();
	}

	public static void ShowAboutWindow()
	{
		if (_services == null)
		{
			Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowAboutWindow();
	}

	public static void ShowAIChatPanelWindow()
	{
		if (_services == null)
		{
			Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowAIChatPanelWindow();
	}

	public static void ShowFeatureStoreWindow()
	{
		if (_services == null)
		{
			Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowFeatureStoreWindow();
	}

	public static void ShowBatchLinkModelsWindow()
	{
		if (_services == null)
		{
			Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowBatchLinkModelsWindow();
	}

	public static void ShowManageModelLinksWindow()
	{
		if (_services == null)
		{
			Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowManageModelLinksWindow();
	}

	public static void ShowRoadProjectManagementWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowRoadProjectManagementWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowRoadProjectManagementWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowMunicipalPipelineNetworkWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowMunicipalPipelineNetworkWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowMunicipalPipelineNetworkWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowCreateBridgeComponentWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowCreateBridgeComponentWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowCreateBridgeComponentWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowFamilyLibraryWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowFamilyLibraryWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowFamilyLibraryWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowDefinePileWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowDefinePileWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowDefinePileWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowDefineFoundationWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowDefineFoundationWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowDefineFoundationWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowDefinePierWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowDefinePierWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowDefinePierWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowDefineBeamWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowDefineBeamWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowDefineBeamWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowDefineBearingWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowDefineBearingWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowDefineBearingWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowDefineBridgeTypeWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowDefineBridgeTypeWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowDefineBridgeTypeWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowTopographyFromFloorWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowTopographyFromFloorWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowTopographyFromFloorWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowExportFamilyMetadataWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowExportFamilyMetadataWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowExportFamilyMetadataWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowMapSelectionWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowMapSelectionWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowMapSelectionWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowMapSelectionWindowWithCallback(Action<RevitAi.Abstractions.Models.MapSelectionCompletedEventArgs> callback, double? initialLat = null, double? initialLon = null)
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowMapSelectionWindowWithCallback(callback, initialLat, initialLon);
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowMapSelectionWindowWithCallback 执行失败", ex);
			throw;
		}
	}

	public static void CloseMapSelectionWindow()
	{
		try
		{
			if (_services != null)
			{
				EnsureApplicationAvailable();
				IWindowManager requiredService = _services.GetRequiredService<IWindowManager>();
				requiredService.GetType().GetMethod("CloseMapSelectionWindow")?.Invoke(requiredService, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[UIBootstrapper] CloseMapSelectionWindow 失败: " + ex.Message);
		}
	}

	public static void ShowWallToRoadWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowWallToRoadWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowWallToRoadWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowSubgradeModelWindow()
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowSubgradeModelWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] ShowSubgradeModelWindow 执行失败", ex);
			throw;
		}
	}

	public static void ShowMainWindowBasedOnAuth()
	{
		if (_services == null)
		{
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		IAuthManager requiredService = _services.GetRequiredService<IAuthManager>();
		IWindowManager requiredService2 = _services.GetRequiredService<IWindowManager>();
		if (requiredService.CurrentUser != null)
		{
			requiredService2.ShowMainWindow();
		}
		else
		{
			requiredService2.ShowLoginWindow();
		}
	}

	public static void CloseAllWindows()
	{
		if (_services != null)
		{
			IWindowManager requiredService = _services.GetRequiredService<IWindowManager>();
			requiredService.CloseLoginWindow();
			requiredService.CloseMainWindow();
			requiredService.CloseAboutWindow();
			requiredService.CloseSettingsWindow();
		}
	}

	public static async Task ShutdownAsync()
	{
		CloseAllWindows();
		if (_host == null)
		{
			return;
		}
		try
		{
			using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(1L));
			await _host.StopAsync(cts.Token);
		}
		catch (Exception ex)
		{
			Logger.Warning("[UIBootstrapper] 停止 Host 时出现异常: " + ex.Message);
		}
		try
		{
			_host.Dispose();
		}
		catch (Exception ex2)
		{
			Logger.Warning("[UIBootstrapper] 释放 Host 时出现异常: " + ex2.Message);
		}
		_host = null;
		_services = null;
	}

	private static void ConfigureServices(IServiceCollection services, IAuthManager? authManager, INativeCryptoService? cryptoService, ISupabaseClient? supabaseClient)
	{
		services.AddSingleton<RevitAi.Abstractions.Logging.ILogger, LoggerAdapter>();
		services.AddSingleton<IDialogService, DialogService>();
		services.AddSingleton<INavigationService, NavigationService>();
		services.AddSingleton<IWindowManager, WindowManager>();
		services.AddSingleton<IThemeService, ThemeService>();
		services.AddSingleton<IUserPreferencesService, UserPreferencesService>();
		services.AddSingleton<IRoadCenterlineDataService, RoadCenterlineDataService>();
		services.AddSingleton<IRoadProjectManager, RoadProjectManager>();
		services.AddSingleton<IRoadModelingUIService, RoadModelingUIService>();
		services.AddSingleton<IFamilyDownloadService, FamilyDownloadService>();
		if (supabaseClient != null && authManager != null)
		{
			services.AddSingleton((IApiKeyService)new ApiKeyService(supabaseClient.HttpClient, supabaseClient.BaseUrl, supabaseClient.HttpClient.DefaultRequestHeaders.GetValues("apikey").FirstOrDefault() ?? "", authManager));
		}
		services.AddSingleton((IServiceProvider sp) => CoreServicesFactory.CreateCADFileService());
		services.AddSingleton((IServiceProvider sp) => CoreServicesFactory.CreateCADAnalyzerService(sp.GetRequiredService<ICADFileService>()));
		ZPayConfig zPayConfig = new ZPayConfig();
		if (!string.IsNullOrEmpty(zPayConfig.MerchantId) && !string.IsNullOrEmpty(zPayConfig.MerchantKey))
		{
			services.AddSingleton((IPaymentService)new ZPayService(zPayConfig));
		}
		else
		{
			Logger.Warning("[UIBootstrapper] ⚠\ufe0f ZPay配置不可用，支付功能将被禁用");
		}
		if (authManager != null && supabaseClient != null)
		{
			SupabaseHttpClientProxy httpProxy = new SupabaseHttpClientProxy(supabaseClient.HttpClient);
			services.AddSingleton((ISupabaseClientProxy)httpProxy);
			services.AddSingleton((Func<IServiceProvider, IPaymentCompletionService>)((IServiceProvider sp) => new PaymentCompletionService(httpProxy, authManager)));
			services.AddSingleton((IServiceProvider sp) => new TokenUsageService(supabaseClient, authManager));
			services.AddSingleton((IServiceProvider sp) => new FeedbackService(authManager, supabaseClient));
			services.AddSingleton((Func<IServiceProvider, ICodeMarketService>)((IServiceProvider sp) => new CodeMarketService(supabaseClient, authManager, sp.GetRequiredService<IDeviceService>())));
			services.AddSingleton((Func<IServiceProvider, ISkillManager>)delegate(IServiceProvider sp)
			{
				IUpdateService service = sp.GetService<IUpdateService>();
				AIToolRegistry instance = AIToolRegistry.Instance;
				if (service == null)
				{
					Logger.Warning("[UIBootstrapper] ⚠\ufe0f IUpdateService 未注册，SkillManager 将无法从 COS 更新");
				}
				SkillManager skillManager = new SkillManager(service ?? new NoOpUpdateService(), instance);
				Task.Run(async delegate
				{
					try
					{
						await skillManager.InitializeAsync();
					}
					catch (Exception ex3)
					{
						Logger.Error("[UIBootstrapper] 初始化 SkillManager 失败: " + ex3.Message, ex3);
					}
				});
				return skillManager;
			});
		}
		if (cryptoService == null)
		{
			try
			{
				Type cryptoServiceType = Type.GetType("RevitAi.Core.Security.NativeCryptoService, RevitAi.Core");
				Type type = Type.GetType("RevitAi.Abstractions.Security.INativeCryptoService, RevitAi.Abstractions");
				if (cryptoServiceType != null && type != null)
				{
					services.AddSingleton(type, (IServiceProvider sp) => Activator.CreateInstance(cryptoServiceType) ?? throw new InvalidOperationException("无法创建 NativeCryptoService 实例"));
				}
				else
				{
					Logger.Warning("[UIBootstrapper] ⚠\ufe0f 未找到 NativeCryptoService 类型，跳过注册");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[UIBootstrapper] 注册 NativeCryptoService 失败: " + ex.Message);
			}
		}
		try
		{
			services.AddSingleton((Func<IServiceProvider, IAIToolDataCache>)((IServiceProvider sp) => new SessionAIToolDataCache()));
		}
		catch (Exception ex2)
		{
			Logger.Error("[UIBootstrapper] 注册 AI 工具数据缓存服务失败: " + ex2.Message);
		}
		services.AddSingleton<IDeviceService, DeviceService>();
		services.AddTransient<LoginViewModel>();
		services.AddTransient<MainViewModel>();
		services.AddTransient<AboutViewModel>();
		services.AddTransient<SettingsViewModel>();
		services.AddTransient<FeaturePanelViewModel>();
		services.AddTransient<AIChatPanelViewModel>();
		services.AddTransient<FeatureStoreViewModel>();
		services.AddTransient<BatchLinkModelsViewModel>();
		services.AddTransient<ManageModelLinksViewModel>();
		services.AddTransient<PurchaseLicenseViewModel>();
		services.AddTransient<RoadProjectManagementViewModel>();
		services.AddTransient<RoadProjectEditorViewModel>();
		services.AddTransient<StationElevationInputViewModel>();
		services.AddTransient<CurveInputViewModel>();
		services.AddTransient<CodeMarketViewModel>();
		services.AddTransient<RoadModelPlacementViewModel>();
		services.AddTransient<MunicipalPipelineNetworkViewModel>();
		services.AddTransient<CreateBridgeComponentViewModel>();
		services.AddTransient<DefinePileViewModel>();
		services.AddTransient<DefineFoundationViewModel>();
		services.AddTransient<DefinePierViewModel>();
		services.AddTransient<DefineBeamViewModel>();
		services.AddTransient<DefineBearingViewModel>();
		services.AddTransient<DefineBridgeTypeViewModel>();
		services.AddTransient<TopographyFromFloorViewModel>();
		services.AddTransient<EditBridgeComponentsViewModel>();
		services.AddTransient<FamilyLibraryViewModel>();
		services.AddTransient<WallToRoadViewModel>();
		services.AddTransient<CreateSubgradeModelViewModel>();
		services.AddTransient<CreateAncillaryStructureViewModel>();
		services.AddTransient<RoadSurfaceRefinementViewModel>();
		if (supabaseClient != null && authManager != null)
		{
			services.AddSingleton((Func<IServiceProvider, IFamilyLibraryService>)((IServiceProvider sp) => new FamilyLibraryService(supabaseClient, authManager)));
		}
		services.AddSingleton(delegate
		{
			try
			{
				IModuleLoader moduleLoader = ServiceProvider.GetModuleLoader();
				if (moduleLoader?.RevitAdapter == null)
				{
					throw new InvalidOperationException("IRevitAdapter 未初始化");
				}
				return moduleLoader.RevitAdapter;
			}
			catch (Exception ex3)
			{
				Logger.Error("[UIBootstrapper] 获取 IRevitAdapter 失败: " + ex3.Message);
				throw;
			}
		});
		services.AddSingleton(delegate(IServiceProvider sp)
		{
			try
			{
				IRevitAdapter requiredService = sp.GetRequiredService<IRevitAdapter>();
				if (requiredService.FamilyMetadataService == null)
				{
					throw new InvalidOperationException("IFamilyMetadataService 未初始化，请确保在命令上下文中调用");
				}
				return requiredService.FamilyMetadataService;
			}
			catch (Exception ex3)
			{
				Logger.Error("[UIBootstrapper] 获取 IFamilyMetadataService 失败: " + ex3.Message);
				throw;
			}
		});
	}

	private static void LoadResources()
	{
		if (Application.Current == null)
		{
			_wpfApplication = new Application
			{
				ShutdownMode = ShutdownMode.OnExplicitShutdown
			};
		}
		else if (_wpfApplication == null)
		{
			_wpfApplication = Application.Current;
			_wpfApplication.ShutdownMode = ShutdownMode.OnExplicitShutdown;
		}
		Application.ResourceAssembly = typeof(UIBootstrapper).Assembly;
	}

	private static void EnsureApplicationAvailable()
	{
		if (_wpfApplication == null || Application.Current == null)
		{
			Logger.Warning("[UIBootstrapper] ⚠\ufe0f Application 不可用，重新创建");
			LoadResources();
			return;
		}
		try
		{
			_ = Application.Current.Windows;
		}
		catch (InvalidOperationException)
		{
			Logger.Warning("[UIBootstrapper] ⚠\ufe0f Application 已关闭，重新创建");
			_wpfApplication = null;
			LoadResources();
		}
	}

	public static T GetService<T>() where T : notnull
	{
		if (_services == null)
		{
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		return _services.GetRequiredService<T>();
	}

	public static T? TryGetService<T>()
	{
		if (_services == null)
		{
			return default(T);
		}
		return _services.GetService<T>();
	}

	private static void RegisterServicesToServiceProvider()
	{
		_ = _services;
	}

	public static void ShowPaymentDialog(string message, string commandText)
	{
		try
		{
			if (_services == null)
			{
				Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
				return;
			}
			EnsureApplicationAvailable();
			_services.GetRequiredService<IWindowManager>().ShowPaymentDialog(message, commandText);
		}
		catch (Exception ex)
		{
			Logger.Error("[UIBootstrapper] 显示授权对话框失败", ex);
		}
	}

	public static void ShowRoadModelPlacementWindow()
	{
		if (_services == null)
		{
			Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowRoadModelPlacementWindow();
	}

	public static void ShowAncillaryStructureWindow()
	{
		if (_services == null)
		{
			Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowAncillaryStructureWindow();
	}

	public static void ShowRoadSurfaceRefinementWindow()
	{
		if (_services == null)
		{
			Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowRoadSurfaceRefinementWindow();
	}

	public static void ShowInsulationWindow()
	{
		if (_services == null)
		{
			Logger.Error("[UIBootstrapper] 错误: _services 为 null，UIBootstrapper 未初始化");
			throw new InvalidOperationException("UIBootstrapper 未初始化，请先调用 InitializeAsync()");
		}
		EnsureApplicationAvailable();
		_services.GetRequiredService<IWindowManager>().ShowInsulationWindow();
	}
}
