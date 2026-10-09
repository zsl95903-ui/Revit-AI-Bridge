using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Revit.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ns6;

using ArgumentException = System.ArgumentException;
using OperationCanceledException = System.OperationCanceledException;
namespace RevitAi.Revit.Infrastructure;

public sealed class CADElementSelectionExternalEventHandler : IExternalEventHandler
{
	public string GetName()
	{
		return "CAD Element Selection";
	}

	public void Execute(UIApplication app)
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		ILogger logger = ServiceProvider.GetLogger();
		CADElementSelectionRequest andClearRequest = CADRequestManager.GetAndClearRequest();
		if (andClearRequest == null)
		{
			logger.Warning("[CADElementSelectionExternalEventHandler] 没有待执行的请求");
			return;
		}
		try
		{
			UIDocument activeUIDocument = app.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				logger.Error("[CADElementSelectionExternalEventHandler] 无法获取活动文档", (Exception)null);
				andClearRequest.OnCompleted?.Invoke(arg1: false, new List<int>(), new List<string>());
				return;
			}
			Document document = activeUIDocument.Document;
			IList<Reference> list = (andClearRequest.AllowMultiple ? smethod_1(activeUIDocument, andClearRequest.Prompt, ((object)andClearRequest.ObjectType/*cast due to constrained. prefix*/).ToString(), logger) : smethod_0(activeUIDocument, andClearRequest.Prompt, ((object)andClearRequest.ObjectType/*cast due to constrained. prefix*/).ToString(), logger));
			if (list == null || list.Count == 0)
			{
				andClearRequest.OnCompleted?.Invoke(arg1: false, new List<int>(), new List<string>());
				return;
			}
			HashSet<string> source = smethod_2(list, document, app, ((object)andClearRequest.ObjectType/*cast due to constrained. prefix*/).ToString(), logger);
			andClearRequest.OnCompleted?.Invoke(arg1: true, smethod_6(list), source.ToList());
		}
		catch (OperationCanceledException)
		{
			andClearRequest.OnCompleted?.Invoke(arg1: false, new List<int>(), new List<string>());
		}
		catch (Exception ex2)
		{
			logger.Error("[CADElementSelectionExternalEventHandler] 执行失败", ex2);
			andClearRequest.OnCompleted?.Invoke(arg1: false, new List<int>(), new List<string>());
		}
	}

	private static IList<Reference> smethod_0(UIDocument uidocument_0, string string_0, string string_1, ILogger ilogger_0)
	{
		Assembly assembly = ((object)uidocument_0.Selection).GetType().Assembly;
		Type type = assembly.GetType("Autodesk.Revit.UI.Selection.ObjectType");
		if (type == null)
		{
			ilogger_0.Error("[CADElementSelectionExternalEventHandler] 无法找到 ObjectType 枚举类型", (Exception)null);
			return new List<Reference>(0);
		}
		object obj;
		try
		{
			obj = Enum.Parse(type, string_1);
		}
		catch (ArgumentException)
		{
			ilogger_0.Warning("[CADElementSelectionExternalEventHandler] ObjectType." + string_1 + " 不存在，回退到 ObjectType.Element");
			obj = Enum.Parse(type, "Element");
		}
		CADImportSelectionFilter cADImportSelectionFilter = new CADImportSelectionFilter(string_1);
		MethodInfo method = ((object)uidocument_0.Selection).GetType().GetMethod("PickObject", new Type[3]
		{
			type,
			typeof(ISelectionFilter),
			typeof(string)
		});
		if (method != null)
		{
			try
			{
				object? obj2 = method.Invoke(uidocument_0.Selection, new object[3] { obj, cADImportSelectionFilter, string_0 });
				Reference val = (Reference)((obj2 is Reference) ? obj2 : null);
				if (val != null)
				{
					return new List<Reference> { val };
				}
				return new List<Reference>(0);
			}
			catch (TargetInvocationException ex2) when (ex2.InnerException is OperationCanceledException)
			{
				throw;
			}
		}
		ilogger_0.Warning("[CADElementSelectionExternalEventHandler] 未找到带 ISelectionFilter 的 PickObject 方法，使用不带过滤器的方法");
		method = ((object)uidocument_0.Selection).GetType().GetMethod("PickObject", new Type[2]
		{
			type,
			typeof(string)
		});
		if (method == null)
		{
			ilogger_0.Error("[CADElementSelectionExternalEventHandler] 无法找到 PickObject 方法", (Exception)null);
			return new List<Reference>(0);
		}
		try
		{
			object? obj3 = method.Invoke(uidocument_0.Selection, new object[2] { obj, string_0 });
			Reference val2 = (Reference)((obj3 is Reference) ? obj3 : null);
			if (val2 != null)
			{
				return new List<Reference> { val2 };
			}
			return new List<Reference>(0);
		}
		catch (TargetInvocationException ex3) when (ex3.InnerException is OperationCanceledException)
		{
			throw;
		}
	}

	private static IList<Reference> smethod_1(UIDocument uidocument_0, string string_0, string string_1, ILogger ilogger_0)
	{
		Assembly assembly = ((object)uidocument_0.Selection).GetType().Assembly;
		Type type = assembly.GetType("Autodesk.Revit.UI.Selection.ObjectType");
		if (type == null)
		{
			ilogger_0.Error("[CADElementSelectionExternalEventHandler] 无法找到 ObjectType 枚举类型", (Exception)null);
			return new List<Reference>(0);
		}
		object obj;
		try
		{
			obj = Enum.Parse(type, string_1);
		}
		catch (ArgumentException)
		{
			ilogger_0.Warning("[CADElementSelectionExternalEventHandler] ObjectType." + string_1 + " 不存在，回退到 ObjectType.Element");
			obj = Enum.Parse(type, "Element");
		}
		CADImportSelectionFilter cADImportSelectionFilter = new CADImportSelectionFilter(string_1);
		MethodInfo method = ((object)uidocument_0.Selection).GetType().GetMethod("PickObjects", new Type[3]
		{
			type,
			typeof(ISelectionFilter),
			typeof(string)
		});
		if (method != null)
		{
			try
			{
				IList<Reference> list = method.Invoke(uidocument_0.Selection, new object[3] { obj, cADImportSelectionFilter, string_0 }) as IList<Reference>;
				return list ?? new List<Reference>(0);
			}
			catch (TargetInvocationException ex2)
			{
				if (ex2.InnerException is OperationCanceledException)
				{
					throw;
				}
				ilogger_0.Error("[CADElementSelectionExternalEventHandler] PickObjects 调用失败: " + ex2.InnerException?.Message, (Exception)null);
				ilogger_0.Error("[CADElementSelectionExternalEventHandler] 内部异常类型: " + ex2.InnerException?.GetType().FullName, (Exception)null);
				ilogger_0.Error("[CADElementSelectionExternalEventHandler] 内部异常堆栈: " + ex2.InnerException?.StackTrace, (Exception)null);
				ilogger_0.Warning("[CADElementSelectionExternalEventHandler] 尝试使用不带过滤器的 PickObjects 方法");
				method = ((object)uidocument_0.Selection).GetType().GetMethod("PickObjects", new Type[2]
				{
					type,
					typeof(string)
				});
				if (method != null)
				{
					try
					{
						IList<Reference> list2 = method.Invoke(uidocument_0.Selection, new object[2] { obj, string_0 }) as IList<Reference>;
						return list2 ?? new List<Reference>(0);
					}
					catch (TargetInvocationException ex3)
					{
						ilogger_0.Error("[CADElementSelectionExternalEventHandler] 不带过滤器的 PickObjects 也失败: " + ex3.InnerException?.Message, (Exception)null);
						throw;
					}
				}
				return new List<Reference>(0);
			}
		}
		ilogger_0.Warning("[CADElementSelectionExternalEventHandler] 未找到带 ISelectionFilter 的 PickObjects 方法，使用不带过滤器的方法");
		method = ((object)uidocument_0.Selection).GetType().GetMethod("PickObjects", new Type[2]
		{
			type,
			typeof(string)
		});
		if (method == null)
		{
			ilogger_0.Error("[CADElementSelectionExternalEventHandler] 无法找到 PickObjects 方法", (Exception)null);
			return new List<Reference>(0);
		}
		try
		{
			IList<Reference> list3 = method.Invoke(uidocument_0.Selection, new object[2] { obj, string_0 }) as IList<Reference>;
			return list3 ?? new List<Reference>(0);
		}
		catch (TargetInvocationException ex4)
		{
			if (ex4.InnerException is OperationCanceledException)
			{
				throw;
			}
			ilogger_0.Error("[CADElementSelectionExternalEventHandler] PickObjects(不带过滤器) 调用失败: " + ex4.InnerException?.Message, (Exception)null);
			ilogger_0.Error("[CADElementSelectionExternalEventHandler] 内部异常类型: " + ex4.InnerException?.GetType().FullName, (Exception)null);
			throw;
		}
	}

	private static HashSet<string> smethod_2(IList<Reference> ilist_0, Document document_0, UIApplication uiapplication_0, string string_0, ILogger ilogger_0)
	{
		HashSet<string> hashSet = new HashSet<string>();
		ElementService elementService = new ElementService(uiapplication_0);
		foreach (Reference item in ilist_0)
		{
			try
			{
				Element element = document_0.GetElement(item);
				if (element == null)
				{
					continue;
				}
				ImportInstance val = (ImportInstance)(object)((element is ImportInstance) ? element : null);
				if (val != null)
				{
					if ((string_0 == "PointOnElement" || string_0 == "Edge") && item != null)
					{
						try
						{
							GeometryObject geometryObjectFromReference = element.GetGeometryObjectFromReference(item);
							if (geometryObjectFromReference != (GeometryObject)null && geometryObjectFromReference.GraphicsStyleId != (ElementId)null && geometryObjectFromReference.GraphicsStyleId != ElementId.InvalidElementId)
							{
								Element element2 = document_0.GetElement(geometryObjectFromReference.GraphicsStyleId);
								GraphicsStyle val2 = (GraphicsStyle)(object)((element2 is GraphicsStyle) ? element2 : null);
								if (val2 != null)
								{
									Category graphicsStyleCategory = val2.GraphicsStyleCategory;
									string text = ((graphicsStyleCategory != null) ? graphicsStyleCategory.Name : null);
									if (!string.IsNullOrEmpty(text))
									{
										hashSet.Add(text);
									}
									else if (((Element)val2).Name != null)
									{
										hashSet.Add(((Element)val2).Name);
										ilogger_0.Warning("[CADElementSelectionExternalEventHandler] GraphicsStyleCategory 为空，使用 Name: " + ((Element)val2).Name);
									}
									if (ilist_0.Count != 1)
									{
										continue;
									}
									return hashSet;
								}
								goto end_IL_007a;
							}
							end_IL_007a:;
						}
						catch (Exception ex)
						{
							ilogger_0.Warning("[CADElementSelectionExternalEventHandler] 从 Reference 获取图层失败: " + ex.Message + "，尝试提取所有图层");
						}
					}
					HashSet<string> hashSet2 = smethod_3(val, document_0, ilogger_0);
					foreach (string item2 in hashSet2)
					{
						hashSet.Add(item2);
					}
				}
				else
				{
					string cadElementLayerName = elementService.GetCadElementLayerName(element);
					if (!string.IsNullOrEmpty(cadElementLayerName))
					{
						hashSet.Add(cadElementLayerName);
					}
					else if (element.Category != null && !string.IsNullOrEmpty(element.Category.Name))
					{
						hashSet.Add(element.Category.Name);
					}
				}
			}
			catch (Exception ex2)
			{
				ilogger_0.Warning("[CADElementSelectionExternalEventHandler] 提取图层名称失败: " + ex2.Message);
			}
		}
		return hashSet;
	}

	private static HashSet<string> smethod_3(ImportInstance importInstance_0, Document document_0, ILogger ilogger_0)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		HashSet<string> hashSet = new HashSet<string>();
		try
		{
			if (((Element)importInstance_0).Category != null)
			{
				hashSet.Add(((Element)importInstance_0).Category.Name);
			}
			Options val = new Options
			{
				DetailLevel = (ViewDetailLevel)3,
				ComputeReferences = true,
				IncludeNonVisibleObjects = true
			};
			GeometryElement val2 = ((Element)importInstance_0).get_Geometry(val);
			if ((GeometryObject)(object)val2 != (GeometryObject)null)
			{
				smethod_4(val2, document_0, hashSet, ilogger_0);
			}
			Category category = ((Element)importInstance_0).Category;
			smethod_5(document_0, hashSet, (category != null) ? category.Name : null, ilogger_0);
		}
		catch (Exception ex)
		{
			ilogger_0.Warning("[CADElementSelectionExternalEventHandler] 从 ImportInstance 提取图层失败: " + ex.Message);
		}
		return hashSet;
	}

	private static void smethod_4(GeometryElement geometryElement_0, Document document_0, HashSet<string> hashSet_0, ILogger ilogger_0)
	{
		try
		{
			foreach (GeometryObject item in geometryElement_0)
			{
				GeometryInstance val = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
				if (val != null)
				{
					try
					{
						GeometryElement instanceGeometry = val.GetInstanceGeometry();
						if ((GeometryObject)(object)instanceGeometry != (GeometryObject)null)
						{
							smethod_4(instanceGeometry, document_0, hashSet_0, ilogger_0);
						}
					}
					catch
					{
					}
				}
				else
				{
					if (item == null)
					{
						continue;
					}
					GeometryObject val2 = item;
					if (!(val2.GraphicsStyleId != (ElementId)null) || !(val2.GraphicsStyleId != ElementId.InvalidElementId))
					{
						continue;
					}
					Element element = document_0.GetElement(val2.GraphicsStyleId);
					GraphicsStyle val3 = (GraphicsStyle)(object)((element is GraphicsStyle) ? element : null);
					if (val3 != null)
					{
						Category graphicsStyleCategory = val3.GraphicsStyleCategory;
						string text = ((graphicsStyleCategory != null) ? graphicsStyleCategory.Name : null);
						if (!string.IsNullOrEmpty(text))
						{
							hashSet_0.Add(text);
						}
						else if (((Element)val3).Name != null)
						{
							hashSet_0.Add(((Element)val3).Name);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ilogger_0.Warning("[CADElementSelectionExternalEventHandler] 从几何对象提取图层失败: " + ex.Message);
		}
	}

	private static void smethod_5(Document document_0, HashSet<string> hashSet_0, string? string_0, ILogger ilogger_0)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		try
		{
			Categories categories = document_0.Settings.Categories;
			List<string> list = new List<string>();
			foreach (Category item in (CategoryNameMap)categories)
			{
				Category val = item;
				string name = val.Name;
				if (!string.IsNullOrEmpty(string_0) && (name.Contains(string_0) || name.Contains("CAD") || name.Contains("DWG") || name.Contains("导入")))
				{
					_ = val.Id;
					list.Add(name);
				}
			}
			foreach (string item2 in list)
			{
				hashSet_0.Add(item2);
			}
		}
		catch (Exception ex)
		{
			ilogger_0.Warning("[CADElementSelectionExternalEventHandler] 从类别提取图层失败: " + ex.Message);
		}
	}

	private static List<int> smethod_6(IList<Reference> ilist_0)
	{
		return (from reference_0 in ilist_0
			where reference_0 != null && reference_0.ElementId != (ElementId)null && reference_0.ElementId != ElementId.InvalidElementId
			select (int)reference_0.ElementId.Value).Distinct().ToList();
	}
}
