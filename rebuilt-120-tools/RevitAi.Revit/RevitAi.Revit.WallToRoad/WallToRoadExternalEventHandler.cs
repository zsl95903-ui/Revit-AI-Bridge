using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit;
using RevitAi.Abstractions.Revit.WallToRoad;
using RevitAi.Revit.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.WallToRoad;

public sealed class WallToRoadExternalEventHandler : IExternalEventHandler
{
	public string GetName()
	{
		return "从墙生成道路网络";
	}

	public void Execute(UIApplication app)
	{
		ILogger logger = ServiceProvider.GetLogger();
		WallToRoadRequest andClearRequest = WallToRoadRequestManager.GetAndClearRequest();
		if (andClearRequest == null)
		{
			logger.Error("[WallToRoadExternalEventHandler] 请求为空", (Exception)null);
			return;
		}
		try
		{
			object document = andClearRequest.Document;
			object obj = ((document is Document) ? document : null);
			if (obj == null)
			{
				UIDocument activeUIDocument = app.ActiveUIDocument;
				obj = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			}
			Document val = (Document)obj;
			if (val == null)
			{
				logger.Error("[WallToRoadExternalEventHandler] 无法获取文档", (Exception)null);
				andClearRequest.OnCompleted?.Invoke(new InvalidOperationException("无法获取文档"), arg2: false);
				return;
			}
			WallToRoadService wallToRoadService = new WallToRoadService(app, val);
			if (wallToRoadService == null)
			{
				logger.Error("[WallToRoadExternalEventHandler] 无法创建 WallToRoadService", (Exception)null);
				andClearRequest.OnCompleted?.Invoke(new InvalidOperationException("无法创建 WallToRoadService"), arg2: false);
				return;
			}
			MethodInfo method = wallToRoadService.GetType().GetMethod("CreateRoadFromWalls");
			if (method == null)
			{
				logger.Error("[WallToRoadExternalEventHandler] 未找到 CreateRoadFromWalls 方法", (Exception)null);
				andClearRequest.OnCompleted?.Invoke(new MissingMethodException("未找到创建方法"), arg2: false);
				return;
			}
			object obj2 = method.Invoke(wallToRoadService, new object[2] { val, andClearRequest });
			if (obj2 == null)
			{
				logger.Error("[WallToRoadExternalEventHandler] 创建道路失败", (Exception)null);
				andClearRequest.OnCompleted?.Invoke(new Exception("创建道路失败"), arg2: false);
				return;
			}
			PropertyInfo property = obj2.GetType().GetProperty("IsSuccess");
			if (!(property == null))
			{
				object value = property.GetValue(obj2);
				if (value is bool && (bool)value)
				{
					if (obj2.GetType().GetProperty("Value")?.GetValue(obj2) is List<int> { Count: >0 } list)
					{
						andClearRequest.CreatedFloorIds = list;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[WallToRoadExternalEventHandler] 道路创建成功，创建了 ");
						defaultInterpolatedStringHandler.AppendFormatted(list.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个楼板");
						logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					andClearRequest.OnCompleted?.Invoke(null, arg2: true);
					logger.Info("[WallToRoadExternalEventHandler] 操作完成");
					return;
				}
			}
			PropertyInfo property2 = obj2.GetType().GetProperty("Error");
			object obj3;
			if ((object)property2 == null)
			{
				obj3 = null;
			}
			else
			{
				object? value2 = property2.GetValue(obj2);
				if (value2 == null)
				{
					obj3 = null;
				}
				else
				{
					obj3 = value2.ToString();
					if (obj3 != null)
					{
						goto IL_0205;
					}
				}
			}
			obj3 = "未知错误";
			goto IL_0205;
			IL_0205:
			string text = (string)obj3;
			logger.Error("[WallToRoadExternalEventHandler] 创建道路失败: " + text, (Exception)null);
			andClearRequest.OnCompleted?.Invoke(new Exception(text), arg2: false);
		}
		catch (Exception ex)
		{
			logger.Error("[WallToRoadExternalEventHandler] 执行失败", ex);
			andClearRequest.OnCompleted?.Invoke(ex, arg2: false);
		}
	}
}
