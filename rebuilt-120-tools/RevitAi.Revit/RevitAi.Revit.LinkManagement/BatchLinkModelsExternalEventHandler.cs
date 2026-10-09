using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.LinkManagement;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Revit.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.LinkManagement;

public sealed class BatchLinkModelsExternalEventHandler : IExternalEventHandler
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static BatchLinkModelsExternalEventRequest? batchLinkModelsExternalEventRequest_0;

	public static BatchLinkModelsExternalEventRequest? CurrentRequest
	{
		[CompilerGenerated]
		get
		{
			return batchLinkModelsExternalEventRequest_0;
		}
		[CompilerGenerated]
		set
		{
			batchLinkModelsExternalEventRequest_0 = value;
		}
	}

	public string GetName()
	{
		return "Batch Link Models Operations";
	}

	public void Execute(UIApplication app)
	{
		ILogger logger = ServiceProvider.GetLogger();
		BatchLinkModelsExternalEventRequest currentRequest = CurrentRequest;
		if (currentRequest == null)
		{
			logger.Error("[BatchLinkModelsExternalEventHandler] 请求为空", (Exception)null);
			return;
		}
		try
		{
			object obj = currentRequest.Document;
			if (obj == null)
			{
				UIDocument activeUIDocument = app.ActiveUIDocument;
				obj = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			}
			object obj2 = obj;
			if (obj2 != null)
			{
				Document val = (Document)((obj2 is Document) ? obj2 : null);
				if (val != null)
				{
					if (currentRequest.FilePaths == null || !currentRequest.FilePaths.Any())
					{
						logger.Error("[BatchLinkModelsExternalEventHandler] 文件路径列表为空", (Exception)null);
						currentRequest.OnCompleted?.Invoke(new ArgumentException("文件路径列表为空"), 0);
						return;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[BatchLinkModelsExternalEventHandler] 开始批量链接 ");
					defaultInterpolatedStringHandler.AppendFormatted(currentRequest.FilePaths.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个模型文件");
					logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					LinkService linkService = new LinkService(app);
					int num = (currentRequest.SuccessCount = linkService.LoadLinks(val, currentRequest.FilePaths));
					currentRequest.FailureCount = currentRequest.FilePaths.Count - num;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("批量链接完成：成功 ");
					defaultInterpolatedStringHandler2.AppendFormatted(num);
					defaultInterpolatedStringHandler2.AppendLiteral(" 个，失败 ");
					defaultInterpolatedStringHandler2.AppendFormatted(currentRequest.FailureCount);
					defaultInterpolatedStringHandler2.AppendLiteral(" 个");
					currentRequest.ResultMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(56, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("[BatchLinkModelsExternalEventHandler] 批量链接完成：成功 ");
					defaultInterpolatedStringHandler3.AppendFormatted(num);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个，失败 ");
					defaultInterpolatedStringHandler3.AppendFormatted(currentRequest.FailureCount);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个");
					logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
					currentRequest.OnCompleted?.Invoke(null, num);
					return;
				}
			}
			logger.Error("[BatchLinkModelsExternalEventHandler] 无法获取文档", (Exception)null);
			currentRequest.OnCompleted?.Invoke(new InvalidOperationException("无法获取文档"), 0);
		}
		catch (Exception ex)
		{
			logger.Error("[BatchLinkModelsExternalEventHandler] 执行失败", ex);
			currentRequest.ResultMessage = "操作失败: " + ex.Message;
			currentRequest.OnCompleted?.Invoke(ex, 0);
		}
		finally
		{
			CurrentRequest = null;
		}
	}
}
