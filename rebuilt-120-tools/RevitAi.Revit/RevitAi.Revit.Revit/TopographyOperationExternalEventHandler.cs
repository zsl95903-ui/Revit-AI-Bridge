using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ns6;

namespace RevitAi.Revit.Revit;

public sealed class TopographyOperationExternalEventHandler : IExternalEventHandler
{
	public string GetName()
	{
		return "Topography Operation";
	}

	public void Execute(UIApplication app)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		CreateTopographyRequest andClearCreateTopographyRequest = TopographyOperationRequestManager.GetAndClearCreateTopographyRequest();
		if (andClearCreateTopographyRequest != null)
		{
			method_0(app, andClearCreateTopographyRequest);
			return;
		}
		TopographyOperationRequest andClearRequest = TopographyOperationRequestManager.GetAndClearRequest();
		if (andClearRequest == null)
		{
			method_4("[TopographyOperationExternalEventHandler] 没有待处理的请求");
			return;
		}
		try
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[TopographyOperationExternalEventHandler] 开始处理请求，模式: ");
			defaultInterpolatedStringHandler.AppendFormatted<OperationMode>(andClearRequest.Mode);
			method_3(defaultInterpolatedStringHandler.ToStringAndClear());
			UIDocument activeUIDocument = app.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				method_5("[TopographyOperationExternalEventHandler] 没有活动文档");
				return;
			}
			List<object> list = method_1(activeUIDocument, andClearRequest);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(50, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[TopographyOperationExternalEventHandler] 选择了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个元素");
			method_3(defaultInterpolatedStringHandler2.ToStringAndClear());
			andClearRequest.OnCompleted?.Invoke(list);
		}
		catch (OperationCanceledException)
		{
			method_3("[TopographyOperationExternalEventHandler] 用户取消了选择");
		}
		catch (Exception ex2)
		{
			method_5("[TopographyOperationExternalEventHandler] 执行失败: " + ex2.Message);
		}
	}

	private void method_0(UIApplication uiapplication_0, CreateTopographyRequest createTopographyRequest_0)
	{
		try
		{
			method_3("[TopographyOperationExternalEventHandler] 开始处理创建地形请求");
			UIDocument activeUIDocument = uiapplication_0.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				method_5("[TopographyOperationExternalEventHandler] 没有活动文档");
				createTopographyRequest_0.OnCompleted?.Invoke((0, new List<string> { "没有活动文档" }, new List<object>()));
				return;
			}
			Document document = activeUIDocument.Document;
			Type type = Type.GetType("RevitAi.Abstractions.Loader.ServiceProvider, RevitAi.Abstractions");
			if (type == null)
			{
				method_5("[TopographyOperationExternalEventHandler] 无法获取 ServiceProvider 类型");
				createTopographyRequest_0.OnCompleted?.Invoke((0, new List<string> { "无法获取 ServiceProvider" }, new List<object>()));
				return;
			}
			MethodInfo method = type.GetMethod("GetModuleLoader", BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				method_5("[TopographyOperationExternalEventHandler] 无法获取 GetModuleLoader 方法");
				createTopographyRequest_0.OnCompleted?.Invoke((0, new List<string> { "无法获取 GetModuleLoader 方法" }, new List<object>()));
				return;
			}
			object obj = method.Invoke(null, null);
			if (obj == null)
			{
				method_5("[TopographyOperationExternalEventHandler] ModuleLoader 为 null");
				createTopographyRequest_0.OnCompleted?.Invoke((0, new List<string> { "ModuleLoader 为 null" }, new List<object>()));
				return;
			}
			PropertyInfo property = obj.GetType().GetProperty("RevitAdapter");
			if (property == null)
			{
				method_5("[TopographyOperationExternalEventHandler] 无法获取 RevitAdapter 属性");
				createTopographyRequest_0.OnCompleted?.Invoke((0, new List<string> { "无法获取 RevitAdapter" }, new List<object>()));
				return;
			}
			object value = property.GetValue(obj);
			if (value == null)
			{
				method_5("[TopographyOperationExternalEventHandler] RevitAdapter 为 null");
				createTopographyRequest_0.OnCompleted?.Invoke((0, new List<string> { "RevitAdapter 为 null" }, new List<object>()));
				return;
			}
			PropertyInfo property2 = value.GetType().GetProperty("TopographyService");
			if (property2 == null)
			{
				method_5("[TopographyOperationExternalEventHandler] 无法获取 TopographyService 属性");
				createTopographyRequest_0.OnCompleted?.Invoke((0, new List<string> { "无法获取 TopographyService" }, new List<object>()));
				return;
			}
			object? value2 = property2.GetValue(value);
			ITopographyService val = (ITopographyService)((value2 is ITopographyService) ? value2 : null);
			if (val == null)
			{
				method_5("[TopographyOperationExternalEventHandler] TopographyService 为 null");
				createTopographyRequest_0.OnCompleted?.Invoke((0, new List<string> { "TopographyService 为 null" }, new List<object>()));
				return;
			}
			(int, List<string>, List<object>) obj2 = val.CreateTopographiesFromFloorProfiles((object)document, createTopographyRequest_0.FloorElements, createTopographyRequest_0.TopographyElement, createTopographyRequest_0.ProgressReporter);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[TopographyOperationExternalEventHandler] 创建地形完成，成功: ");
			defaultInterpolatedStringHandler.AppendFormatted(obj2.Item1);
			method_3(defaultInterpolatedStringHandler.ToStringAndClear());
			createTopographyRequest_0.OnProgress?.Invoke(obj2.Item1, createTopographyRequest_0.FloorElements.Count, "处理完成");
			createTopographyRequest_0.OnCompleted?.Invoke(obj2);
		}
		catch (Exception ex)
		{
			method_5("[TopographyOperationExternalEventHandler] 创建地形失败: " + ex.Message);
			createTopographyRequest_0.OnCompleted?.Invoke((0, new List<string> { ex.Message }, new List<object>()));
		}
	}

	private List<object> method_1(UIDocument uidocument_0, TopographyOperationRequest topographyOperationRequest_0)
	{
		Selection selection = uidocument_0.Selection;
		Document document = uidocument_0.Document;
		ISelectionFilter val = method_2(topographyOperationRequest_0);
		List<object> list = new List<object>();
		if (topographyOperationRequest_0.IsSingleSelection)
		{
			Reference val2 = selection.PickObject((ObjectType)1, val, topographyOperationRequest_0.Prompt);
			Element element = document.GetElement(val2);
			if (element != null)
			{
				list.Add(element);
			}
		}
		else
		{
			IList<Reference> list2 = selection.PickObjects((ObjectType)1, val, topographyOperationRequest_0.Prompt);
			foreach (Reference item in list2)
			{
				Element element2 = document.GetElement(item);
				if (element2 != null)
				{
					list.Add(element2);
				}
			}
		}
		return list;
	}

	private ISelectionFilter method_2(TopographyOperationRequest topographyOperationRequest_0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (ISelectionFilter)(object)new TopographySelectionFilter(topographyOperationRequest_0.Mode);
	}

	private void method_3(string string_0)
	{
		try
		{
			Logger.Info("[TopographyOperationExternalEventHandler] " + string_0);
		}
		catch
		{
		}
	}

	private void method_4(string string_0)
	{
		try
		{
			Logger.Warning("[TopographyOperationExternalEventHandler] " + string_0);
		}
		catch
		{
		}
	}

	private void method_5(string string_0)
	{
		try
		{
			Logger.Error("[TopographyOperationExternalEventHandler] " + string_0);
		}
		catch
		{
		}
	}
}
