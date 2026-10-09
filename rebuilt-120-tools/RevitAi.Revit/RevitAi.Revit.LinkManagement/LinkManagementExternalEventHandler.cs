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

public sealed class LinkManagementExternalEventHandler : IExternalEventHandler
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static LinkManagementExternalEventRequest? linkManagementExternalEventRequest_0;

	public static LinkManagementExternalEventRequest? CurrentRequest
	{
		[CompilerGenerated]
		get
		{
			return linkManagementExternalEventRequest_0;
		}
		[CompilerGenerated]
		set
		{
			linkManagementExternalEventRequest_0 = value;
		}
	}

	public string GetName()
	{
		return "Link Management Operations";
	}

	public void Execute(UIApplication app)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected I4, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		ILogger logger = ServiceProvider.GetLogger();
		LinkManagementExternalEventRequest currentRequest = CurrentRequest;
		if (currentRequest == null)
		{
			logger.Error("[LinkManagementExternalEventHandler] 请求为空", (Exception)null);
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
					LinkService linkService_ = new LinkService(app);
					LinkManagementOperation operation = currentRequest.Operation;
					LinkManagementOperation val2 = operation;
					switch ((int)val2)
					{
					case 0:
						method_0(val, linkService_, currentRequest, logger);
						return;
					case 1:
						method_1(val, linkService_, currentRequest, logger);
						return;
					case 2:
						method_2(val, linkService_, currentRequest, logger);
						return;
					case 3:
						method_3(val, linkService_, currentRequest, logger);
						return;
					case 4:
						method_4(val, linkService_, currentRequest, logger);
						return;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[LinkManagementExternalEventHandler] 未知的操作类型: ");
					defaultInterpolatedStringHandler.AppendFormatted<LinkManagementOperation>(currentRequest.Operation);
					logger.Error(defaultInterpolatedStringHandler.ToStringAndClear(), (Exception)null);
					Action<Exception, bool> onCompleted = currentRequest.OnCompleted;
					if (onCompleted != null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(9, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("未知的操作类型: ");
						defaultInterpolatedStringHandler2.AppendFormatted<LinkManagementOperation>(currentRequest.Operation);
						onCompleted(new InvalidOperationException(defaultInterpolatedStringHandler2.ToStringAndClear()), arg2: false);
					}
					return;
				}
			}
			logger.Error("[LinkManagementExternalEventHandler] 无法获取文档", (Exception)null);
			currentRequest.OnCompleted?.Invoke(new InvalidOperationException("无法获取文档"), arg2: false);
		}
		catch (Exception ex)
		{
			logger.Error("[LinkManagementExternalEventHandler] 执行失败", ex);
			currentRequest.ResultMessage = "操作失败: " + ex.Message;
			currentRequest.OnCompleted?.Invoke(ex, arg2: false);
		}
		finally
		{
			CurrentRequest = null;
		}
	}

	private void method_0(Document document_0, LinkService linkService_0, LinkManagementExternalEventRequest linkManagementExternalEventRequest_1, ILogger ilogger_0)
	{
		if (linkManagementExternalEventRequest_1.LinkInstance == null)
		{
			ilogger_0.Error("[LinkManagementExternalEventHandler] Delete: LinkInstance 为空", (Exception)null);
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(new ArgumentException("LinkInstance 为空"), arg2: false);
			return;
		}
		ilogger_0.Info("[LinkManagementExternalEventHandler] 开始删除链接");
		if (linkService_0.DeleteLink(document_0, linkManagementExternalEventRequest_1.LinkInstance))
		{
			linkManagementExternalEventRequest_1.ResultMessage = "链接删除成功";
			linkManagementExternalEventRequest_1.SuccessCount = 1;
			ilogger_0.Info("[LinkManagementExternalEventHandler] 链接删除成功");
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(null, arg2: true);
		}
		else
		{
			linkManagementExternalEventRequest_1.ResultMessage = "链接删除失败";
			linkManagementExternalEventRequest_1.FailureCount = 1;
			ilogger_0.Error("[LinkManagementExternalEventHandler] 链接删除失败", (Exception)null);
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(new Exception("链接删除失败"), arg2: false);
		}
	}

	private void method_1(Document document_0, LinkService linkService_0, LinkManagementExternalEventRequest linkManagementExternalEventRequest_1, ILogger ilogger_0)
	{
		if (!linkManagementExternalEventRequest_1.LinkId.HasValue)
		{
			ilogger_0.Error("[LinkManagementExternalEventHandler] Reload: LinkId 为空", (Exception)null);
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(new ArgumentException("LinkId 为空"), arg2: false);
			return;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[LinkManagementExternalEventHandler] 开始重新载入链接 ID: ");
		defaultInterpolatedStringHandler.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
		ilogger_0.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		if (linkService_0.ReloadLink(document_0, linkManagementExternalEventRequest_1.LinkId.Value))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("链接 ID ");
			defaultInterpolatedStringHandler2.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
			defaultInterpolatedStringHandler2.AppendLiteral(" 重新载入成功");
			linkManagementExternalEventRequest_1.ResultMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
			linkManagementExternalEventRequest_1.SuccessCount = 1;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(50, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("[LinkManagementExternalEventHandler] 链接 ID ");
			defaultInterpolatedStringHandler3.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
			defaultInterpolatedStringHandler3.AppendLiteral(" 重新载入成功");
			ilogger_0.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(null, arg2: true);
		}
		else
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler4.AppendLiteral("链接 ID ");
			defaultInterpolatedStringHandler4.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
			defaultInterpolatedStringHandler4.AppendLiteral(" 重新载入失败");
			linkManagementExternalEventRequest_1.ResultMessage = defaultInterpolatedStringHandler4.ToStringAndClear();
			linkManagementExternalEventRequest_1.FailureCount = 1;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(50, 1);
			defaultInterpolatedStringHandler5.AppendLiteral("[LinkManagementExternalEventHandler] 链接 ID ");
			defaultInterpolatedStringHandler5.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
			defaultInterpolatedStringHandler5.AppendLiteral(" 重新载入失败");
			ilogger_0.Error(defaultInterpolatedStringHandler5.ToStringAndClear(), (Exception)null);
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(new Exception("链接重新载入失败"), arg2: false);
		}
	}

	private void method_2(Document document_0, LinkService linkService_0, LinkManagementExternalEventRequest linkManagementExternalEventRequest_1, ILogger ilogger_0)
	{
		if (!linkManagementExternalEventRequest_1.LinkId.HasValue)
		{
			ilogger_0.Error("[LinkManagementExternalEventHandler] Unload: LinkId 为空", (Exception)null);
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(new ArgumentException("LinkId 为空"), arg2: false);
			return;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[LinkManagementExternalEventHandler] 开始卸载链接 ID: ");
		defaultInterpolatedStringHandler.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
		ilogger_0.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		if (linkService_0.UnloadLink(document_0, linkManagementExternalEventRequest_1.LinkId.Value))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(11, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("链接 ID ");
			defaultInterpolatedStringHandler2.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
			defaultInterpolatedStringHandler2.AppendLiteral(" 卸载成功");
			linkManagementExternalEventRequest_1.ResultMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
			linkManagementExternalEventRequest_1.SuccessCount = 1;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(48, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("[LinkManagementExternalEventHandler] 链接 ID ");
			defaultInterpolatedStringHandler3.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
			defaultInterpolatedStringHandler3.AppendLiteral(" 卸载成功");
			ilogger_0.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(null, arg2: true);
		}
		else
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(11, 1);
			defaultInterpolatedStringHandler4.AppendLiteral("链接 ID ");
			defaultInterpolatedStringHandler4.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
			defaultInterpolatedStringHandler4.AppendLiteral(" 卸载失败");
			linkManagementExternalEventRequest_1.ResultMessage = defaultInterpolatedStringHandler4.ToStringAndClear();
			linkManagementExternalEventRequest_1.FailureCount = 1;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(48, 1);
			defaultInterpolatedStringHandler5.AppendLiteral("[LinkManagementExternalEventHandler] 链接 ID ");
			defaultInterpolatedStringHandler5.AppendFormatted(linkManagementExternalEventRequest_1.LinkId);
			defaultInterpolatedStringHandler5.AppendLiteral(" 卸载失败");
			ilogger_0.Error(defaultInterpolatedStringHandler5.ToStringAndClear(), (Exception)null);
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(new Exception("链接卸载失败"), arg2: false);
		}
	}

	private void method_3(Document document_0, LinkService linkService_0, LinkManagementExternalEventRequest linkManagementExternalEventRequest_1, ILogger ilogger_0)
	{
		if (linkManagementExternalEventRequest_1.LinkIds == null || !linkManagementExternalEventRequest_1.LinkIds.Any())
		{
			ilogger_0.Error("[LinkManagementExternalEventHandler] ReloadAll: LinkIds 为空", (Exception)null);
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(new ArgumentException("LinkIds 为空"), arg2: false);
			return;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[LinkManagementExternalEventHandler] 开始批量重新载入 ");
		defaultInterpolatedStringHandler.AppendFormatted(linkManagementExternalEventRequest_1.LinkIds.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个链接");
		ilogger_0.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		int num = 0;
		int num2 = 0;
		foreach (int linkId in linkManagementExternalEventRequest_1.LinkIds)
		{
			try
			{
				if (linkService_0.ReloadLink(document_0, linkId))
				{
					num++;
					continue;
				}
				num2++;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(50, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[LinkManagementExternalEventHandler] 链接 ID ");
				defaultInterpolatedStringHandler2.AppendFormatted(linkId);
				defaultInterpolatedStringHandler2.AppendLiteral(" 重新载入失败");
				ilogger_0.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			catch (Exception ex)
			{
				num2++;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(52, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("[LinkManagementExternalEventHandler] 链接 ID ");
				defaultInterpolatedStringHandler3.AppendFormatted(linkId);
				defaultInterpolatedStringHandler3.AppendLiteral(" 重新载入异常: ");
				defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
				ilogger_0.Error(defaultInterpolatedStringHandler3.ToStringAndClear(), (Exception)null);
			}
		}
		linkManagementExternalEventRequest_1.SuccessCount = num;
		linkManagementExternalEventRequest_1.FailureCount = num2;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(20, 2);
		defaultInterpolatedStringHandler4.AppendLiteral("批量重新载入完成：成功 ");
		defaultInterpolatedStringHandler4.AppendFormatted(num);
		defaultInterpolatedStringHandler4.AppendLiteral(" 个，失败 ");
		defaultInterpolatedStringHandler4.AppendFormatted(num2);
		defaultInterpolatedStringHandler4.AppendLiteral(" 个");
		linkManagementExternalEventRequest_1.ResultMessage = defaultInterpolatedStringHandler4.ToStringAndClear();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(57, 2);
		defaultInterpolatedStringHandler5.AppendLiteral("[LinkManagementExternalEventHandler] 批量重新载入完成：成功 ");
		defaultInterpolatedStringHandler5.AppendFormatted(num);
		defaultInterpolatedStringHandler5.AppendLiteral(" 个，失败 ");
		defaultInterpolatedStringHandler5.AppendFormatted(num2);
		defaultInterpolatedStringHandler5.AppendLiteral(" 个");
		ilogger_0.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
		linkManagementExternalEventRequest_1.OnCompleted?.Invoke(null, num > 0);
	}

	private void method_4(Document document_0, LinkService linkService_0, LinkManagementExternalEventRequest linkManagementExternalEventRequest_1, ILogger ilogger_0)
	{
		if (linkManagementExternalEventRequest_1.LinkIds == null || !linkManagementExternalEventRequest_1.LinkIds.Any())
		{
			ilogger_0.Error("[LinkManagementExternalEventHandler] UnloadAll: LinkIds 为空", (Exception)null);
			linkManagementExternalEventRequest_1.OnCompleted?.Invoke(new ArgumentException("LinkIds 为空"), arg2: false);
			return;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[LinkManagementExternalEventHandler] 开始批量卸载 ");
		defaultInterpolatedStringHandler.AppendFormatted(linkManagementExternalEventRequest_1.LinkIds.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个链接");
		ilogger_0.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		int num = 0;
		int num2 = 0;
		foreach (int linkId in linkManagementExternalEventRequest_1.LinkIds)
		{
			try
			{
				if (linkService_0.UnloadLink(document_0, linkId))
				{
					num++;
					continue;
				}
				num2++;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(48, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[LinkManagementExternalEventHandler] 链接 ID ");
				defaultInterpolatedStringHandler2.AppendFormatted(linkId);
				defaultInterpolatedStringHandler2.AppendLiteral(" 卸载失败");
				ilogger_0.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			catch (Exception ex)
			{
				num2++;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(50, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("[LinkManagementExternalEventHandler] 链接 ID ");
				defaultInterpolatedStringHandler3.AppendFormatted(linkId);
				defaultInterpolatedStringHandler3.AppendLiteral(" 卸载异常: ");
				defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
				ilogger_0.Error(defaultInterpolatedStringHandler3.ToStringAndClear(), (Exception)null);
			}
		}
		linkManagementExternalEventRequest_1.SuccessCount = num;
		linkManagementExternalEventRequest_1.FailureCount = num2;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(18, 2);
		defaultInterpolatedStringHandler4.AppendLiteral("批量卸载完成：成功 ");
		defaultInterpolatedStringHandler4.AppendFormatted(num);
		defaultInterpolatedStringHandler4.AppendLiteral(" 个，失败 ");
		defaultInterpolatedStringHandler4.AppendFormatted(num2);
		defaultInterpolatedStringHandler4.AppendLiteral(" 个");
		linkManagementExternalEventRequest_1.ResultMessage = defaultInterpolatedStringHandler4.ToStringAndClear();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(55, 2);
		defaultInterpolatedStringHandler5.AppendLiteral("[LinkManagementExternalEventHandler] 批量卸载完成：成功 ");
		defaultInterpolatedStringHandler5.AppendFormatted(num);
		defaultInterpolatedStringHandler5.AppendLiteral(" 个，失败 ");
		defaultInterpolatedStringHandler5.AppendFormatted(num2);
		defaultInterpolatedStringHandler5.AppendLiteral(" 个");
		ilogger_0.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
		linkManagementExternalEventRequest_1.OnCompleted?.Invoke(null, num > 0);
	}
}
