using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Logging;

namespace RevitAi.Abstractions.Loader;

public static class ModuleLoader
{
	private static Assembly? _uiAssembly;

	private static object? _uiBootstrapperInstance;

	private const string UI_ASSEMBLY_NAME = "RevitAi.UI";

	private const string UI_BOOTSTRAPPER_TYPE_NAME = "RevitAi.UI.Services.UIBootstrapper";

	public static bool IsUIModuleLoaded
	{
		get
		{
			if (_uiAssembly != null)
			{
				return _uiBootstrapperInstance != null;
			}
			return false;
		}
	}

	public static bool InitializeUIModule(object? authManager, object? cryptoService = null, object? supabaseClient = null)
	{
		try
		{
			if (_uiAssembly == null)
			{
				_uiAssembly = LoadAssembly("RevitAi.UI");
				if (_uiAssembly == null)
				{
					Logger.Error("无法加载 UI 程序集");
					return false;
				}
			}
			Type type = _uiAssembly.GetType("RevitAi.UI.Services.UIBootstrapper");
			if (type == null)
			{
				Logger.Error("无法找到类型: RevitAi.UI.Services.UIBootstrapper");
				return false;
			}
			MethodInfo methodInfo = null;
			object[] parameters = Array.Empty<object>();
			if (cryptoService != null && supabaseClient != null)
			{
				Type typeFromHandle = typeof(IAuthManager);
				Type type2 = Type.GetType("RevitAi.Core.Security.INativeCryptoService, RevitAi.Core");
				Type type3 = Type.GetType("RevitAi.Core.Authentication.ISupabaseClient, RevitAi.Core");
				if (type2 != null && type3 != null)
				{
					methodInfo = type.GetMethod("InitializeAsync", new Type[3] { typeFromHandle, type2, type3 });
					if (methodInfo != null)
					{
						parameters = new object[3] { authManager, cryptoService, supabaseClient };
					}
				}
			}
			if (methodInfo == null)
			{
				methodInfo = type.GetMethod("InitializeAsync", new Type[1] { typeof(IAuthManager) });
				if (methodInfo != null)
				{
					parameters = new object[1] { authManager };
				}
			}
			if (methodInfo == null)
			{
				Logger.Error("无法找到 InitializeAsync 方法");
				return false;
			}
			if (!(methodInfo.Invoke(null, parameters) is Task task))
			{
				Logger.Error("InitializeAsync 方法返回值不是 Task 类型");
				return false;
			}
			task.GetAwaiter().GetResult();
			_uiBootstrapperInstance = type;
			return true;
		}
		catch (TargetInvocationException ex)
		{
			Logger.Error("UI 模块初始化失败 - 内部异常", ex.InnerException ?? ex);
			return false;
		}
		catch (Exception ex2)
		{
			Logger.Error("UI 模块初始化失败", ex2);
			return false;
		}
	}

	public static void ShowAboutWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示关于窗口");
				return;
			}
			Type type = _uiBootstrapperInstance as Type;
			if (type == null)
			{
				Logger.Error("UIBootstrapper 类型无效");
				return;
			}
			MethodInfo method = type.GetMethod("ShowAboutWindow");
			if (method == null)
			{
				Logger.Error("无法找到 ShowAboutWindow 方法");
			}
			else
			{
				method?.Invoke(null, null);
			}
		}
		catch (TargetInvocationException ex)
		{
			Logger.Error("显示关于窗口失败 - 内部异常", ex.InnerException ?? ex);
			Logger.Error("显示关于窗口失败 - 包装异常", ex);
		}
		catch (Exception ex2)
		{
			Logger.Error("显示关于窗口失败", ex2);
		}
	}

	public static void ShowLoginWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示登录窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowLoginWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示登录窗口失败", ex);
		}
	}

	public static void ShowFeatureStoreWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示应用仓库窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowFeatureStoreWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示应用仓库窗口失败", ex);
		}
	}

	public static void ShowBatchLinkModelsWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示批量链接模型窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowBatchLinkModelsWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示批量链接模型窗口失败", ex);
		}
	}

	public static void ShowMapSelectionWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示地图选择窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowMapSelectionWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示地图选择窗口失败", ex);
		}
	}

	public static void ShowManageModelLinksWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示管理链接模型窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowManageModelLinksWindow"))?.Invoke(null, null);
			}
		}
		catch (TargetInvocationException ex)
		{
			Logger.Error("显示管理链接模型窗口失败 - TargetInvocationException", ex);
			if (ex.InnerException != null)
			{
				Logger.Error("内部异常详情", ex.InnerException);
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("显示管理链接模型窗口失败", ex2);
		}
	}

	public static void ShowRoadProjectManagementWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示道路项目管理窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowRoadProjectManagementWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示道路项目管理窗口失败", ex);
		}
	}

	public static void ShowAIChatWindow()
	{
		ShowDockablePaneAIChat();
	}

	private static Assembly? LoadAdapterAssembly()
	{
		try
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (directoryName == null)
			{
				return null;
			}
			string text = Path.Combine(directoryName, "RevitAi.Revit.dll");
			if (File.Exists(text))
			{
				return Assembly.LoadFrom(text);
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static void CloseAllWindows()
	{
		try
		{
			if (_uiBootstrapperInstance != null)
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("CloseAllWindows"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("关闭窗口失败", ex);
		}
	}

	public static void ShowDockablePaneAIChat()
	{
		try
		{
			Assembly assembly = LoadAdapterAssembly();
			if (assembly == null)
			{
				Logger.Error("[ModuleLoader] ❌ 无法加载 Revit 适配程序集");
				return;
			}
			Type type = assembly.GetType("RevitAi.Revit.UI.AIDockablePaneHelper");
			if (type == null)
			{
				Logger.Error("[ModuleLoader] ❌ 未找到 AIDockablePaneHelper 类型");
				return;
			}
			MethodInfo method = type.GetMethod("ShowPane", BindingFlags.Static | BindingFlags.Public, null, Type.EmptyTypes, null);
			if (method == null)
			{
				Logger.Error("[ModuleLoader] ❌ 未找到无参数的 ShowPane 方法");
				MethodInfo[] array = (from m in type.GetMethods()
					where m.Name == "ShowPane"
					select m).ToArray();
				Logger.Error($"[ModuleLoader] 找到 {array.Length} 个 ShowPane 方法:");
				MethodInfo[] array2 = array;
				foreach (MethodInfo methodInfo in array2)
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					string value = string.Join(", ", parameters.Select((ParameterInfo p) => p.ParameterType.Name));
					Logger.Error($"  - {methodInfo.Name}({value})");
				}
			}
			else
			{
				method.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[ModuleLoader] 显示 DockablePane 失败", ex);
		}
	}

	public static void HideDockablePaneAIChat()
	{
		try
		{
			Assembly assembly = LoadAdapterAssembly();
			if (!(assembly == null))
			{
				Type type = assembly.GetType("RevitAi.Revit.UI.AIDockablePaneHelper");
				if (!(type == null))
				{
					type.GetMethod("HidePane")?.Invoke(null, null);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("隐藏 DockablePane 失败", ex);
		}
	}

	public static async Task ShutdownAsync()
	{
		try
		{
			if (_uiBootstrapperInstance != null)
			{
				MethodInfo methodInfo = (_uiBootstrapperInstance as Type)?.GetMethod("ShutdownAsync");
				if (methodInfo != null && methodInfo.Invoke(null, null) is Task task)
				{
					await task;
				}
				_uiBootstrapperInstance = null;
				_uiAssembly = null;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("关闭 UI 模块失败", ex);
		}
	}

	public static void ShowPaymentDialog(string message, string commandText)
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示授权对话框");
				return;
			}
			((_uiBootstrapperInstance as Type)?.GetMethod("ShowPaymentDialog"))?.Invoke(null, new object[2] { message, commandText });
		}
		catch (Exception ex)
		{
			Logger.Error("显示授权对话框失败", ex);
		}
	}

	public static void ShowRoadModelPlacementWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示道路模型放置窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowRoadModelPlacementWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示道路模型放置窗口失败", ex);
		}
	}

	public static void ShowMunicipalPipelineNetworkWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示市政管网窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowMunicipalPipelineNetworkWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示市政管网窗口失败", ex);
		}
	}

	public static void ShowCreateBridgeComponentWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示创建桥梁部件窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowCreateBridgeComponentWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示创建桥梁部件窗口失败", ex);
		}
	}

	public static void ShowDefinePileWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示定义桩部件窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowDefinePileWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示定义桩部件窗口失败", ex);
		}
	}

	public static void ShowDefineFoundationWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示定义基础部件窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowDefineFoundationWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示定义基础部件窗口失败", ex);
		}
	}

	public static void ShowDefinePierWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示定义墩柱窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowDefinePierWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示定义墩柱窗口失败", ex);
		}
	}

	public static void ShowDefineBeamWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示定义盖梁窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowDefineBeamWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示定义盖梁窗口失败", ex);
		}
	}

	public static void ShowDefineBearingWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示定义支座窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowDefineBearingWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示定义支座窗口失败", ex);
		}
	}

	public static void ShowDefineBridgeTypeWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示定义桥型窗口");
			}
			else
			{
				((_uiBootstrapperInstance as Type)?.GetMethod("ShowDefineBridgeTypeWindow"))?.Invoke(null, null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("显示定义桥型窗口失败", ex);
		}
	}

	public static void ShowTopographyFromFloorWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示楼板裁剪地形窗口");
				return;
			}
			((_uiBootstrapperInstance as Type)?.GetMethod("ShowTopographyFromFloorWindow"))?.Invoke(null, null);
			Logger.Info("已调用 UIBootstrapper.ShowTopographyFromFloorWindow()");
		}
		catch (Exception ex)
		{
			Logger.Error("显示楼板裁剪地形窗口失败", ex);
		}
	}

	private static void EnsureUIModuleLoaded()
	{
		if (!IsUIModuleLoaded)
		{
			Logger.Warning("UI 模块未加载，尝试延迟初始化...");
		}
	}

	private static Assembly? LoadAssembly(string assemblyName)
	{
		try
		{
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name == assemblyName);
			if (assembly != null)
			{
				Logger.Debug("程序集已在内存中: " + assemblyName);
				return assembly;
			}
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (string.IsNullOrEmpty(directoryName))
			{
				Logger.Error("无法获取插件目录");
				return null;
			}
			string text = Path.Combine(directoryName, assemblyName + ".dll");
			if (!File.Exists(text))
			{
				Logger.Error("程序集文件不存在: " + text);
				return null;
			}
			return Assembly.LoadFrom(text);
		}
		catch (Exception ex)
		{
			Logger.Error("加载程序集失败: " + assemblyName, ex);
			return null;
		}
	}

	public static void ShowExportFamilyMetadataWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示导出族元数据窗口");
				return;
			}
			((_uiBootstrapperInstance as Type)?.GetMethod("ShowExportFamilyMetadataWindow"))?.Invoke(null, null);
			Logger.Info("[ModuleLoader] 已打开导出族元数据窗口");
		}
		catch (Exception ex)
		{
			Logger.Error("显示导出族元数据窗口失败", ex);
		}
	}

	public static void ShowWallToRoadWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示从墙生成道路网络窗口");
				return;
			}
			((_uiBootstrapperInstance as Type)?.GetMethod("ShowWallToRoadWindow"))?.Invoke(null, null);
			Logger.Info("[ModuleLoader] 已打开从墙生成道路网络窗口");
		}
		catch (Exception ex)
		{
			Logger.Error("显示从墙生成道路网络窗口失败", ex);
		}
	}

	public static void ShowSubgradeModelWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("UIBootstrapper 未初始化，无法显示道路路基建模窗口");
				return;
			}
			((_uiBootstrapperInstance as Type)?.GetMethod("ShowSubgradeModelWindow"))?.Invoke(null, null);
			Logger.Info("[ModuleLoader] 已打开道路路基建模窗口");
		}
		catch (Exception ex)
		{
			Logger.Error("显示道路路基建模窗口失败", ex);
		}
	}

	public static void ShowAncillaryStructureWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("[ModuleLoader] UI Bootstrapper 未初始化");
				return;
			}
			((_uiBootstrapperInstance as Type)?.GetMethod("ShowAncillaryStructureWindow"))?.Invoke(null, null);
			Logger.Info("[ModuleLoader] 已打开创建道路附属结构窗口");
		}
		catch (Exception ex)
		{
			Logger.Error("显示创建道路附属结构窗口失败", ex);
		}
	}

	public static void ShowRoadSurfaceRefinementWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("[ModuleLoader] UI Bootstrapper 未初始化");
				return;
			}
			((_uiBootstrapperInstance as Type)?.GetMethod("ShowRoadSurfaceRefinementWindow"))?.Invoke(null, null);
			Logger.Info("[ModuleLoader] 已打开精修路基路面窗口");
		}
		catch (Exception ex)
		{
			Logger.Error("显示精修路基路面窗口失败", ex);
		}
	}

	public static void ShowInsulationWindow()
	{
		try
		{
			EnsureUIModuleLoaded();
			if (_uiBootstrapperInstance == null)
			{
				Logger.Error("[ModuleLoader] UI Bootstrapper 未初始化");
				return;
			}
			((_uiBootstrapperInstance as Type)?.GetMethod("ShowInsulationWindow"))?.Invoke(null, null);
			Logger.Info("[ModuleLoader] 已打开保温工具窗口");
		}
		catch (Exception ex)
		{
			Logger.Error("显示保温工具窗口失败", ex);
		}
	}
}
