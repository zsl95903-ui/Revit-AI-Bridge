using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.Revit.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.RoadCenterline;

public sealed class RoadCenterlineExternalEventHandler : IExternalEventHandler
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static RoadCenterlineExternalEventRequest? roadCenterlineExternalEventRequest_0;

	public static RoadCenterlineExternalEventRequest? CurrentRequest
	{
		[CompilerGenerated]
		get
		{
			return roadCenterlineExternalEventRequest_0;
		}
		[CompilerGenerated]
		set
		{
			roadCenterlineExternalEventRequest_0 = value;
		}
	}

	public string GetName()
	{
		return "Road Centerline Creation";
	}

	public void Execute(UIApplication app)
	{
		ILogger logger = ServiceProvider.GetLogger();
		RoadCenterlineExternalEventRequest currentRequest = CurrentRequest;
		if (currentRequest == null)
		{
			logger.Error("[RoadCenterlineExternalEventHandler] 请求为空", (Exception)null);
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
			Document val = (Document)obj;
			if (val == null)
			{
				logger.Error("[RoadCenterlineExternalEventHandler] 无法获取文档", (Exception)null);
				currentRequest.OnCompleted?.Invoke(new InvalidOperationException("无法获取文档"), arg2: false);
				return;
			}
			IInfrastructureService service = ServiceProvider.GetService<IInfrastructureService>();
			if (service == null)
			{
				logger.Error("[RoadCenterlineExternalEventHandler] 无法获取 InfrastructureService", (Exception)null);
				currentRequest.OnCompleted?.Invoke(new InvalidOperationException("无法获取 InfrastructureService"), arg2: false);
				return;
			}
			CurveService curveService = new CurveService(app, service);
			if (curveService == null)
			{
				logger.Error("[RoadCenterlineExternalEventHandler] 无法创建 CurveService", (Exception)null);
				currentRequest.OnCompleted?.Invoke(new InvalidOperationException("无法创建 CurveService"), arg2: false);
				return;
			}
			MethodInfo method = curveService.GetType().GetMethod("CreateRoadCenterlineInMass");
			if (method == null)
			{
				logger.Error("[RoadCenterlineExternalEventHandler] 未找到 CreateRoadCenterlineInMass 方法", (Exception)null);
				currentRequest.OnCompleted?.Invoke(new MissingMethodException("未找到创建曲线方法"), arg2: false);
				return;
			}
			object obj2 = method.Invoke(curveService, new object[4] { val, currentRequest.Points3D, null, currentRequest.RoadProjectName });
			if (obj2 == null)
			{
				logger.Error("[RoadCenterlineExternalEventHandler] 创建曲线失败", (Exception)null);
				currentRequest.OnCompleted?.Invoke(new Exception("创建曲线失败"), arg2: false);
				return;
			}
			PropertyInfo property = obj2.GetType().GetProperty("IsSuccess");
			if (!(property == null))
			{
				object value = property.GetValue(obj2);
				if (value is bool && (bool)value)
				{
					object obj3 = obj2.GetType().GetProperty("Value")?.GetValue(obj2);
					if (obj3 != null)
					{
						object? obj4 = obj3.GetType().GetProperty("CurveElementId")?.GetValue(obj3);
						currentRequest.CreatedCurveId = (ElementId?)((obj4 is ElementId) ? obj4 : null);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[RoadCenterlineExternalEventHandler] 曲线创建成功，ElementId: ");
						defaultInterpolatedStringHandler.AppendFormatted<ElementId>(currentRequest.CreatedCurveId);
						logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						if (currentRequest.CreateAnnotations && currentRequest.AnnotationFamilyPath != null)
						{
							MethodInfo method2 = curveService.GetType().GetMethod("PlaceStationAnnotations");
							if (method2 != null)
							{
								object obj5 = obj3.GetType().GetProperty("Curve3D")?.GetValue(obj3);
								object obj6 = method2.Invoke(curveService, new object[7] { val, obj5, currentRequest.Points3D, currentRequest.AnnotationFamilyPath, currentRequest.AnnotationIntervalFeet, null, currentRequest.RoadProject });
								if (obj6 != null)
								{
									PropertyInfo property2 = obj6.GetType().GetProperty("IsSuccess");
									object obj7 = property2?.GetValue(obj6);
									bool flag = default(bool);
									int num;
									if (property2 != null)
									{
										if (obj7 is bool)
										{
											flag = (bool)obj7;
											num = 1;
										}
										else
										{
											num = 0;
										}
									}
									else
									{
										num = 0;
									}
									if (((uint)num & (flag ? 1u : 0u)) != 0)
									{
										logger.Info("[RoadCenterlineExternalEventHandler] 桩号标注创建成功");
									}
									else
									{
										PropertyInfo property3 = obj6.GetType().GetProperty("Error");
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(47, 1);
										defaultInterpolatedStringHandler2.AppendLiteral("[RoadCenterlineExternalEventHandler] 桩号标注创建失败: ");
										defaultInterpolatedStringHandler2.AppendFormatted<object>(property3?.GetValue(obj6));
										logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
									}
								}
							}
						}
					}
					currentRequest.OnCompleted?.Invoke(null, arg2: true);
					logger.Info("[RoadCenterlineExternalEventHandler] 操作完成");
					return;
				}
			}
			PropertyInfo property4 = obj2.GetType().GetProperty("Error");
			object obj8;
			if ((object)property4 == null)
			{
				obj8 = null;
			}
			else
			{
				object? value2 = property4.GetValue(obj2);
				if (value2 == null)
				{
					obj8 = null;
				}
				else
				{
					obj8 = value2.ToString();
					if (obj8 != null)
					{
						goto IL_0255;
					}
				}
			}
			obj8 = "未知错误";
			goto IL_0255;
			IL_0255:
			string text = (string)obj8;
			logger.Error("[RoadCenterlineExternalEventHandler] 创建曲线失败: " + text, (Exception)null);
			currentRequest.OnCompleted?.Invoke(new Exception(text), arg2: false);
		}
		catch (Exception ex)
		{
			logger.Error("[RoadCenterlineExternalEventHandler] 执行失败", ex);
			currentRequest.OnCompleted?.Invoke(ex, arg2: false);
		}
		finally
		{
			CurrentRequest = null;
		}
	}
}
