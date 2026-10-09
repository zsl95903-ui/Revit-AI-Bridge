using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;

namespace RevitAi.UI.Services;

internal sealed class RoadModelingUIService : IRoadModelingUIService
{
	private readonly ILogger _logger;

	private readonly IFamilyDownloadService _familyDownloadService;

	public RoadModelingUIService(IFamilyDownloadService familyDownloadService)
	{
		_logger = ServiceProvider.GetLogger();
		_familyDownloadService = familyDownloadService;
	}

	public async Task<RoadModelingUIResult> CreateRoadModelAsync(RoadProject project, bool splitAtIntegerStations = true, int integerStationInterval = 20)
	{
		_ = 1;
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader()?.RevitAdapter;
			if (revitAdapter == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "Revit 适配器未初始化"
				};
			}
			if (revitAdapter.GetActiveDocument() == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "未打开 Revit 文档"
				};
			}
			object externalEventObj = revitAdapter.GetRoadModelingExternalEvent();
			if (externalEventObj == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "无法创建道路建模 ExternalEvent"
				};
			}
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name == "RevitAi.Revit");
			if (assembly == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "未找到 RevitAi.Revit 程序集"
				};
			}
			Type requestType = assembly.GetType("RevitAi.Revit.RoadModeling.RoadModelingExternalEventRequest");
			Type handlerType = assembly.GetType("RevitAi.Revit.RoadModeling.RoadModelingExternalEventHandler");
			if (requestType == null || handlerType == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "未找到道路建模 ExternalEvent 类型"
				};
			}
			Type tcsType = typeof(TaskCompletionSource<>).MakeGenericType(typeof(RoadModelingResult));
			object tcs = Activator.CreateInstance(tcsType);
			object request = Activator.CreateInstance(requestType);
			requestType.GetProperty("Project")?.SetValue(request, project);
			requestType.GetProperty("SplitAtIntegerStations")?.SetValue(request, splitAtIntegerStations);
			requestType.GetProperty("IntegerStationInterval")?.SetValue(request, integerStationInterval);
			requestType.GetProperty("CompletionSource")?.SetValue(request, tcs);
			string targetFamilyName = "AST_R_路基路面_3";
			FamilyDownloadResult familyDownloadResult = await _familyDownloadService.DownloadFamilyFileAsync(targetFamilyName);
			if (!familyDownloadResult.IsSuccess || string.IsNullOrEmpty(familyDownloadResult.FilePath))
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = (familyDownloadResult.Error ?? ("无法获取族文件: " + targetFamilyName))
				};
			}
			requestType.GetProperty("FamilyFilePath")?.SetValue(request, familyDownloadResult.FilePath);
			requestType.GetProperty("FamilyName")?.SetValue(request, targetFamilyName);
			handlerType.GetProperty("CurrentRequest", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, request);
			MethodInfo method = externalEventObj.GetType().GetMethod("Raise");
			if (method == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "未找到 ExternalEvent.Raise 方法"
				};
			}
			method.Invoke(externalEventObj, null);
			if (!(tcsType.GetProperty("Task")?.GetValue(tcs) is Task<RoadModelingResult> task))
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "无法获取任务"
				};
			}
			RoadModelingResult roadModelingResult = await task;
			return new RoadModelingUIResult
			{
				IsSuccess = true,
				CreatedInstanceCount = roadModelingResult.CreatedInstanceCount,
				CreatedTypeCount = roadModelingResult.CreatedTypeCount,
				CreatedMaterialCount = roadModelingResult.CreatedMaterialCount,
				Message = roadModelingResult.Message,
				LayerInstanceCounts = roadModelingResult.LayerInstanceCounts
			};
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingUIService] 创建道路模型失败", ex);
			return new RoadModelingUIResult
			{
				IsSuccess = false,
				Error = ex.Message
			};
		}
	}

	public async Task<RoadModelingUIResult> CreateAncillaryStructureAsync(RoadProject project, bool splitAtIntegerStations = true, int integerStationInterval = 20)
	{
		_ = 1;
		try
		{
			IRevitAdapter revitAdapter = ServiceProvider.GetModuleLoader()?.RevitAdapter;
			if (revitAdapter == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "Revit 适配器未初始化"
				};
			}
			if (revitAdapter.GetActiveDocument() == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "未打开 Revit 文档"
				};
			}
			object externalEventObj = revitAdapter.GetRoadModelingExternalEvent();
			if (externalEventObj == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "无法创建道路建模 ExternalEvent"
				};
			}
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name == "RevitAi.Revit");
			if (assembly == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "未找到 RevitAi.Revit 程序集"
				};
			}
			Type requestType = assembly.GetType("RevitAi.Revit.RoadModeling.RoadModelingExternalEventRequest");
			Type handlerType = assembly.GetType("RevitAi.Revit.RoadModeling.RoadModelingExternalEventHandler");
			if (requestType == null || handlerType == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "未找到道路建模 ExternalEvent 类型"
				};
			}
			Type tcsType = typeof(TaskCompletionSource<>).MakeGenericType(typeof(RoadModelingResult));
			object tcs = Activator.CreateInstance(tcsType);
			object request = Activator.CreateInstance(requestType);
			requestType.GetProperty("Project")?.SetValue(request, project);
			requestType.GetProperty("SplitAtIntegerStations")?.SetValue(request, splitAtIntegerStations);
			requestType.GetProperty("IntegerStationInterval")?.SetValue(request, integerStationInterval);
			requestType.GetProperty("CompletionSource")?.SetValue(request, tcs);
			requestType.GetProperty("IsAncillaryStructure")?.SetValue(request, true);
			HashSet<string> hashSet = new HashSet<string>(from s in project.AncillaryStructuresConfiguration?.Structures ?? new List<AncillaryStructureData>()
				where s.IsEnabled
				select GetFamilyNameByStructureType(s.StructureType));
			Dictionary<string, string> downloadedFamilyPaths = new Dictionary<string, string>();
			List<(string FamilyName, string Error)> failedFamilies = new List<(string, string)>();
			foreach (string familyName in hashSet)
			{
				FamilyDownloadResult familyDownloadResult = await _familyDownloadService.DownloadFamilyFileAsync(familyName);
				if (familyDownloadResult.IsSuccess && !string.IsNullOrEmpty(familyDownloadResult.FilePath))
				{
					downloadedFamilyPaths[familyName] = familyDownloadResult.FilePath;
				}
				else
				{
					failedFamilies.Add((familyName, familyDownloadResult.Error ?? "未知原因"));
				}
			}
			if (downloadedFamilyPaths.Count == 0 && failedFamilies.Count > 0)
			{
				string text = string.Join("\n", failedFamilies.Select<(string, string), string>(((string FamilyName, string Error) f) => "• " + f.FamilyName + ": " + f.Error));
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "无法下载任何附属结构族文件：\n\n" + text
				};
			}
			requestType.GetProperty("DownloadedFamilyPaths")?.SetValue(request, downloadedFamilyPaths);
			handlerType.GetProperty("CurrentRequest", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, request);
			MethodInfo method = externalEventObj.GetType().GetMethod("Raise");
			if (method == null)
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "未找到 ExternalEvent.Raise 方法"
				};
			}
			method.Invoke(externalEventObj, null);
			if (!(tcsType.GetProperty("Task")?.GetValue(tcs) is Task<RoadModelingResult> task))
			{
				return new RoadModelingUIResult
				{
					IsSuccess = false,
					Error = "无法获取任务"
				};
			}
			RoadModelingResult roadModelingResult = await task;
			return new RoadModelingUIResult
			{
				IsSuccess = true,
				CreatedInstanceCount = roadModelingResult.CreatedInstanceCount,
				CreatedTypeCount = roadModelingResult.CreatedTypeCount,
				CreatedMaterialCount = roadModelingResult.CreatedMaterialCount,
				Message = roadModelingResult.Message,
				LayerInstanceCounts = roadModelingResult.LayerInstanceCounts
			};
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingUIService] 创建附属结构失败", ex);
			return new RoadModelingUIResult
			{
				IsSuccess = false,
				Error = ex.Message
			};
		}
	}

	private static string GetFamilyNameByStructureType(AncillaryStructureType structureType)
	{
		return structureType switch
		{
			AncillaryStructureType.Trapezoid => "AST_R_直角梯形_3", 
			AncillaryStructureType.Rectangle => "AST_R_矩形_3", 
			AncillaryStructureType.RectangleHollow => "AST_R_矩形空心_3", 
			AncillaryStructureType.SingleSlopeSurface => "AST_R_单坡面层_3", 
			AncillaryStructureType.DoubleSlopeSurface => "AST_R_双坡面层_3", 
			AncillaryStructureType.RoundedCurb => "AST_R_圆角路沿石_3", 
			_ => "AST_R_矩形_3", 
		};
	}
}
