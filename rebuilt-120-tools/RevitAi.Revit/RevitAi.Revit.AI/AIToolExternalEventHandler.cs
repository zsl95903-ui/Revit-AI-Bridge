using System;
using System.Collections.Generic;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Revit.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.AI;

public sealed class AIToolExternalEventHandler : IExternalEventHandler
{
	private class Class339 : IFailuresPreprocessor
	{
		FailureProcessingResult IFailuresPreprocessor.PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
			foreach (FailureMessageAccessor item in failureMessages)
			{
				string descriptionText = item.GetDescriptionText();
				if (descriptionText.Contains("高亮显示的图元已被连接但未相交") || descriptionText.Contains("Highlighted elements are joined but do not intersect") || descriptionText.Contains("Elements are joined but do not intersect") || descriptionText.Contains("图元已被连接") || descriptionText.Contains("joined but do not intersect"))
				{
					failuresAccessor.DeleteWarning(item);
					Logger.Info("[GeometryWarningSuppressor] 已抑制警告: " + descriptionText);
				}
				else if ((descriptionText.Contains("实例原点") && descriptionText.Contains("主体")) || descriptionText.Contains("实例原点没有位于主体面上") || descriptionText.Contains("实例将丢失与主体的关联") || (descriptionText.Contains("Instance origin") && descriptionText.Contains("host")) || descriptionText.Contains("Instance loses its association to host"))
				{
					failuresAccessor.DeleteWarning(item);
					Logger.Info("[GeometryWarningSuppressor] 已抑制警告: " + descriptionText);
				}
				else if (descriptionText.Contains("无法连接元素") || descriptionText.Contains("Elements cannot be joined") || descriptionText.Contains("连接但不相交") || descriptionText.Contains("Cannot join elements"))
				{
					failuresAccessor.DeleteWarning(item);
					Logger.Info("[GeometryWarningSuppressor] 已抑制警告: " + descriptionText);
				}
			}
			return (FailureProcessingResult)0;
		}
	}

	public string GetName()
	{
		return "AI Tool Execution";
	}

	public void Execute(UIApplication app)
	{
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		Transaction val = null;
		try
		{
			IRevitExternalEventRequest andClearRequest = AIRequestManager.GetAndClearRequest();
			if (andClearRequest == null)
			{
				Logger.Warning("[AIToolExternalEventHandler] 没有待执行的请求");
				return;
			}
			if (andClearRequest.Context.Document == null && app.ActiveUIDocument != null)
			{
				andClearRequest.Context.Document = app.ActiveUIDocument.Document;
			}
			if (andClearRequest.Context.Document == null)
			{
				Logger.Error("[AIToolExternalEventHandler] Document 为空，无法执行");
				andClearRequest.TaskSource.SetResult(AIToolResult.Fail("Document 为空"));
				return;
			}
			object document = andClearRequest.Context.Document;
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				Logger.Error("[AIToolExternalEventHandler] Document 类型错误");
				andClearRequest.TaskSource.SetResult(AIToolResult.Fail("Document 类型错误"));
				return;
			}
			if (andClearRequest.RequiresTransaction)
			{
				val = new Transaction(val2, "AI Tool Execution");
				val.Start();
				FailureHandlingOptions failureHandlingOptions = val.GetFailureHandlingOptions();
				failureHandlingOptions.SetFailuresPreprocessor((IFailuresPreprocessor)(object)new Class339());
				andClearRequest.Context.Transaction = val;
				andClearRequest.Context.TransactionName = "AI Tool Execution";
				Logger.Info("[AIToolExternalEventHandler] 已启动事务");
			}
			AIToolResult val3;
			try
			{
				val3 = andClearRequest.Action(andClearRequest.Context).GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				Logger.Error("[AIToolExternalEventHandler] 工具执行失败", ex);
				val3 = AIToolResult.Fail("执行失败: " + ex.Message);
			}
			if (val3.Success)
			{
				if (val != null)
				{
					try
					{
						val2.Regenerate();
					}
					catch (Exception ex2)
					{
						Logger.Warning("[AIToolExternalEventHandler] 刷新视图失败: " + ex2.Message);
					}
					val.Commit();
				}
				smethod_0(val2, app);
				andClearRequest.TaskSource.SetResult(val3);
			}
			else
			{
				if (val != null)
				{
					Logger.Warning("[AIToolExternalEventHandler] 工具执行失败，回滚事务");
					val.RollBack();
				}
				else
				{
					Logger.Warning("[AIToolExternalEventHandler] 工具执行失败，无事务需要回滚");
				}
				andClearRequest.TaskSource.SetResult(val3);
			}
		}
		catch (Exception ex3)
		{
			Logger.Error("[AIToolExternalEventHandler] Execute 异常", ex3);
			SatelliteMapAIService.DeferredPaintInfo = null;
			try
			{
				if (val != null && val.HasStarted())
				{
					val.RollBack();
				}
			}
			catch (Exception ex4)
			{
				Logger.Error("[AIToolExternalEventHandler] 回滚事务失败", ex4);
			}
			try
			{
				IRevitExternalEventRequest andClearRequest2 = AIRequestManager.GetAndClearRequest();
				if (andClearRequest2 != null && !andClearRequest2.TaskSource.Task.IsCompleted)
				{
					andClearRequest2.TaskSource.SetResult(AIToolResult.Fail("执行异常: " + ex3.Message));
				}
			}
			catch (Exception ex5)
			{
				Logger.Error("[AIToolExternalEventHandler] 设置异常结果失败", ex5);
			}
		}
	}

	private static void smethod_0(Document document_0, UIApplication uiapplication_0)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		(int, int)? deferredPaintInfo = SatelliteMapAIService.DeferredPaintInfo;
		if (!deferredPaintInfo.HasValue)
		{
			return;
		}
		SatelliteMapAIService.DeferredPaintInfo = null;
		try
		{
			ElementId val = new ElementId((long)deferredPaintInfo.Value.Item1);
			ElementId materialId = new ElementId((long)deferredPaintInfo.Value.Item2);
			Element element = document_0.GetElement(val);
			Floor val2 = (Floor)(object)((element is Floor) ? element : null);
			if (val2 == null)
			{
				Logger.Warning("[AIToolExternalEventHandler] 延迟Paint: 无法获取楼板元素");
				return;
			}
			Transaction val3 = new Transaction(document_0, "应用卫星图材质");
			try
			{
				val3.Start();
				using (RevitSatelliteMapImporter revitSatelliteMapImporter = new RevitSatelliteMapImporter())
				{
					revitSatelliteMapImporter.ApplyDeferredPaint(document_0, val2, materialId);
				}
				val3.Commit();
				Logger.Info("[AIToolExternalEventHandler] 延迟Paint 执行成功");
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			smethod_1(uiapplication_0);
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIToolExternalEventHandler] 延迟Paint 失败: " + ex.Message);
		}
	}

	private static void smethod_1(UIApplication uiapplication_0)
	{
		try
		{
			UIDocument activeUIDocument = uiapplication_0.ActiveUIDocument;
			View val = ((activeUIDocument != null) ? activeUIDocument.ActiveView : null);
			if (val != null)
			{
				Parameter val2 = ((Element)val).get_Parameter((BuiltInParameter)(-1005165L));
				if (val2 != null && !((APIObject)val2).IsReadOnly)
				{
					val2.Set(6);
					Logger.Info("[AIToolExternalEventHandler] 已设置视图样式为'真实'");
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIToolExternalEventHandler] 设置视图样式失败: " + ex.Message);
		}
	}
}
