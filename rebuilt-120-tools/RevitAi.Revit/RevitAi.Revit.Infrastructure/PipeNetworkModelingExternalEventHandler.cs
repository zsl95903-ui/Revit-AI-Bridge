using System;
using System.Threading.Tasks;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using RevitAi.Revit.Revit.PipeNetwork;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Infrastructure;

public sealed class PipeNetworkModelingExternalEventHandler : IExternalEventHandler
{
	public string GetName()
	{
		return "Pipe Network Modeling";
	}

	public void Execute(UIApplication app)
	{
		ILogger logger = ServiceProvider.GetLogger();
		PipeNetworkModelingRequest currentRequest = PipeNetworkModelingRequest.CurrentRequest;
		if (currentRequest == null)
		{
			logger.Warning("[PipeNetworkModelingExternalEventHandler] 没有待执行的请求");
			return;
		}
		PipeNetworkModelingResult val2;
		try
		{
			object obj = currentRequest.Document;
			if (obj == null)
			{
				UIDocument activeUIDocument = app.ActiveUIDocument;
				obj = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			}
			Document val = (Document)obj;
			if (val == null)
			{
				logger.Error("[PipeNetworkModelingExternalEventHandler] 无法获取文档", (Exception)null);
				val2 = PipeNetworkModelingResult.CreateFailure("无法获取文档");
				currentRequest.OnCompleted?.Invoke(val2);
				return;
			}
			logger.Info("[PipeNetworkModelingExternalEventHandler] 开始创建管网模型");
			IModuleLoader moduleLoader = ServiceProvider.GetModuleLoader();
			if (moduleLoader == null)
			{
				logger.Error("[PipeNetworkModelingExternalEventHandler] 无法获取 ModuleLoader", (Exception)null);
				val2 = PipeNetworkModelingResult.CreateFailure("无法获取 ModuleLoader");
				currentRequest.OnCompleted?.Invoke(val2);
				return;
			}
			IRevitAdapter revitAdapter = moduleLoader.RevitAdapter;
			if (revitAdapter == null)
			{
				logger.Error("[PipeNetworkModelingExternalEventHandler] 无法获取 RevitAdapter", (Exception)null);
				val2 = PipeNetworkModelingResult.CreateFailure("无法获取 RevitAdapter");
				currentRequest.OnCompleted?.Invoke(val2);
				return;
			}
			IPipeNetworkModelingService pipeNetworkModelingService = revitAdapter.GetPipeNetworkModelingService();
			if (pipeNetworkModelingService == null)
			{
				logger.Error("[PipeNetworkModelingExternalEventHandler] 无法获取建模服务", (Exception)null);
				val2 = PipeNetworkModelingResult.CreateFailure("无法获取建模服务");
				currentRequest.OnCompleted?.Invoke(val2);
				return;
			}
			Task<PipeNetworkModelingResult> task = pipeNetworkModelingService.CreatePipeNetworkModelAsync((object)val, currentRequest.ModelingData, currentRequest.Options, (Action<ModelingProgress>)delegate
			{
			});
			task.Wait();
			val2 = task.Result;
			logger.Info("[PipeNetworkModelingExternalEventHandler] 建模完成: " + (val2.Success ? "成功" : "失败"));
		}
		catch (Exception ex)
		{
			logger.Error("[PipeNetworkModelingExternalEventHandler] 执行失败", ex);
			val2 = PipeNetworkModelingResult.CreateFailure("建模失败: " + ex.Message);
		}
		finally
		{
			PipeNetworkModelingRequest.CurrentRequest = null;
		}
		currentRequest.OnCompleted?.Invoke(val2);
	}
}
