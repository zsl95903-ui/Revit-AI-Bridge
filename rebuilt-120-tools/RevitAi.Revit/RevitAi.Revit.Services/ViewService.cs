using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using ns0;
using ns6;

using ArgumentException = System.ArgumentException;
using InvalidOperationException = System.InvalidOperationException;
using ArgumentNullException = System.ArgumentNullException;
namespace RevitAi.Revit.Services;

internal sealed class ViewService : IViewService
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct _003C_003Ec__DisplayClass20_1
	{
		public Dictionary<string, SchedulableField> runtimeFieldMap;

		public Document doc;

		public ViewSchedule schedule;
	}

	private readonly UIApplication _uiApplication;

	public ViewService(UIApplication uiApplication)
	{
		_uiApplication = uiApplication ?? throw new ArgumentNullException("uiApplication");
	}

	public object? GetActiveView(object document)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			return val.ActiveView;
		}
		catch (Exception ex)
		{
			LogError("GetActiveView 失败: " + ex.Message);
			return null;
		}
	}

	public IEnumerable<object> GetAllViews(object document)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val);
			return val2.OfCategory((BuiltInCategory)(-2000279L)).ToElements().Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("GetAllViews 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public string? GetViewName(object view)
	{
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				return null;
			}
			return ((Element)val).Name;
		}
		catch (Exception ex)
		{
			LogError("GetViewName 失败: " + ex.Message);
			return null;
		}
	}

	public string? GetViewType(object view)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				return null;
			}
			return ((object)val.ViewType/*cast due to constrained. prefix*/).ToString();
		}
		catch (Exception ex)
		{
			LogError("GetViewType 失败: " + ex.Message);
			return null;
		}
	}

	public int? GetViewId(object view)
	{
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				return null;
			}
			return (int)((Element)val).Id.Value;
		}
		catch (Exception ex)
		{
			LogError("GetViewId 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateFloorPlan(object document, int levelId, string? viewName = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateFloorPlan: document 不是 Document 类型");
				return null;
			}
			ElementId val2 = new ElementId((long)levelId);
			Element element = val.GetElement(val2);
			Level val3 = (Level)(object)((element is Level) ? element : null);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateFloorPlan: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			ViewFamilyType viewFamilyType = GetViewFamilyType(val, (ViewFamily)109);
			if (viewFamilyType == null)
			{
				LogError("CreateFloorPlan: 找不到平面图视图族类型");
				return null;
			}
			ViewPlan val4 = ViewPlan.Create(val, ((Element)viewFamilyType).Id, val2);
			if (!string.IsNullOrEmpty(viewName))
			{
				((Element)val4).Name = viewName;
			}
			return val4;
		}
		catch (Exception ex)
		{
			LogError("CreateFloorPlan 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateCeilingPlan(object document, int levelId, string? viewName = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateCeilingPlan: document 不是 Document 类型");
				return null;
			}
			ElementId val2 = new ElementId((long)levelId);
			Element element = val.GetElement(val2);
			Level val3 = (Level)(object)((element is Level) ? element : null);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateCeilingPlan: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			ViewFamilyType viewFamilyType = GetViewFamilyType(val, (ViewFamily)111);
			if (viewFamilyType == null)
			{
				LogError("CreateCeilingPlan: 找不到天花板平面图视图族类型");
				return null;
			}
			ViewPlan val4 = ViewPlan.Create(val, ((Element)viewFamilyType).Id, val2);
			if (!string.IsNullOrEmpty(viewName))
			{
				((Element)val4).Name = viewName;
			}
			return val4;
		}
		catch (Exception ex)
		{
			LogError("CreateCeilingPlan 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateElevationView(object document, double x, double y, string? viewName = null)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateElevationView: document 不是 Document 类型");
				return null;
			}
			LogError("CreateElevationView: 立面图创建功能暂未实现");
			return null;
		}
		catch (Exception ex)
		{
			LogError("CreateElevationView 失败: " + ex.Message);
			return null;
		}
	}

	public bool ActivateView(object document, object view)
	{
		//IL_01be: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Invalid comparison between Unknown and I4
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Invalid comparison between Unknown and I4
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Invalid comparison between Unknown and I4
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Invalid comparison between Unknown and I4
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Invalid comparison between Unknown and I4
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Invalid comparison between Unknown and I4
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Invalid comparison between Unknown and I4
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ActivateView: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("ActivateView: view 不是 View 类型");
				return false;
			}
			UIDocument activeUIDocument = _uiApplication.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				LogError("ActivateView: ActiveUIDocument 为 null（可能没有打开的活动文档）");
				return false;
			}
			if (!((Element)val2).IsValidObject)
			{
				LogError("ActivateView: 视图对象无效");
				return false;
			}
			if (val2.IsTemplate)
			{
				LogError("ActivateView: 不能激活视图模板");
				return false;
			}
			if ((int)val2.ViewType != 5 && (int)val2.ViewType != 11 && (int)val2.ViewType != 8 && (int)val2.ViewType != 119 && (int)val2.ViewType != 120 && (int)val2.ViewType != 122 && (int)val2.ViewType != 123)
			{
				try
				{
					MethodInfo method = ((object)activeUIDocument).GetType().GetMethod("RequestViewChange", BindingFlags.Instance | BindingFlags.Public);
					if (method != null)
					{
						method.Invoke(activeUIDocument, new object[1] { val2 });
						return true;
					}
					PropertyInfo property = ((object)activeUIDocument).GetType().GetProperty("ActiveView");
					if (property != null && property.CanWrite)
					{
						property.SetValue(activeUIDocument, val2);
						return true;
					}
					LogError("ActivateView: 无法激活视图（当前 Revit 版本不支持）");
					return false;
				}
				catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
				{
					LogError("ActivateView: 无法激活视图 - " + ex.Message);
					return false;
				}
				catch (InvalidOperationException ex2)
				{
					InvalidOperationException ex3 = ex2;
					LogError("ActivateView: Revit 无法激活视图 - " + ((Exception)(object)ex3).Message);
					return false;
				}
				catch (Exception ex4)
				{
					LogError("ActivateView: 调用失败 - " + ex4.GetType().Name + ": " + ex4.Message);
					return false;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ActivateView: 视图类型 '");
			defaultInterpolatedStringHandler.AppendFormatted<ViewType>(val2.ViewType);
			defaultInterpolatedStringHandler.AppendLiteral("' 不支持激活");
			LogError(defaultInterpolatedStringHandler.ToStringAndClear());
			return false;
		}
		catch (Exception ex5)
		{
			LogError("ActivateView 失败: " + ex5.Message);
			return false;
		}
	}

	public bool ZoomToElements(object document, IEnumerable<int> elementIds, object? view = null)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ZoomToElements: document 不是 Document 类型");
				return false;
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			UIDocument val2 = new UIDocument(val);
			val2.ShowElements((ICollection<ElementId>)list);
			return true;
		}
		catch (Exception ex)
		{
			LogError("ZoomToElements 失败: " + ex.Message);
			return false;
		}
	}

	public bool SetViewTemplate(object document, object view, string templateName)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("SetViewTemplate: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("SetViewTemplate: view 不是 View 类型");
				return false;
			}
			View val3 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(View))).Cast<View>().FirstOrDefault((View x) => ((Element)x).Name.Equals(templateName, StringComparison.OrdinalIgnoreCase) && x.IsTemplate);
			if (val3 == null)
			{
				LogError("SetViewTemplate: 找不到视图模板 '" + templateName + "'");
				return false;
			}
			val2.ViewTemplateId = ((Element)val3).Id;
			return true;
		}
		catch (Exception ex)
		{
			LogError("SetViewTemplate 失败: " + ex.Message);
			return false;
		}
	}

	public object? Create3DView(object document, string? viewName = null)
	{
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			LogError("Create3DView: document 不是 Document 类型");
			throw new ArgumentException("document 不是 Document 类型", "document");
		}
		ViewFamilyType viewFamilyType = GetViewFamilyType(val, (ViewFamily)102);
		if (viewFamilyType == null)
		{
			LogError("Create3DView: 找不到 3D 视图族类型");
			throw new InvalidOperationException("找不到 3D 视图族类型，请检查项目模板中是否包含 3D 视图类型");
		}
		try
		{
			View3D val2 = View3D.CreateIsometric(val, ((Element)viewFamilyType).Id);
			if (!string.IsNullOrEmpty(viewName))
			{
				((Element)val2).Name = viewName;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Create3DView: 成功创建 3D 视图 '");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Name);
			defaultInterpolatedStringHandler.AppendLiteral("' (ID: ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val2).Id);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return val2;
		}
		catch (Exception ex)
		{
			LogError("Create3DView: 创建 3D 视图失败", ex);
			throw;
		}
	}

	public bool SetSectionBox(object document, object view3D, double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("SetSectionBox: document 不是 Document 类型");
				return false;
			}
			View3D val2 = (View3D)((view3D is View3D) ? view3D : null);
			if (val2 == null)
			{
				LogError("SetSectionBox: view3D 不是 View3D 类型");
				return false;
			}
			if (!val2.IsSectionBoxActive)
			{
				val2.IsSectionBoxActive = true;
				Logger.Info("SetSectionBox: 启用剖面框");
			}
			XYZ min = new XYZ(minX, minY, minZ);
			XYZ max = new XYZ(maxX, maxY, maxZ);
			BoundingBoxXYZ val3 = new BoundingBoxXYZ();
			val3.Min = min;
			val3.Max = max;
			val2.SetSectionBox(val3);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 6);
			defaultInterpolatedStringHandler.AppendLiteral("SetSectionBox: 成功设置剖面框 (Min=(");
			defaultInterpolatedStringHandler.AppendFormatted(minX, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(minY, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(minZ, "F4");
			defaultInterpolatedStringHandler.AppendLiteral("), Max=(");
			defaultInterpolatedStringHandler.AppendFormatted(maxX, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(maxY, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(maxZ, "F4");
			defaultInterpolatedStringHandler.AppendLiteral("))");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("SetSectionBox 失败: " + ex.Message, ex);
			return false;
		}
	}

	public bool ApplyViewFilter(object document, object view, string filterName)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ApplyViewFilter: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("ApplyViewFilter: view 不是 View 类型");
				return false;
			}
			ParameterFilterElement val3 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(ParameterFilterElement))).Cast<ParameterFilterElement>().FirstOrDefault((ParameterFilterElement f) => ((Element)f).Name.Equals(filterName, StringComparison.OrdinalIgnoreCase));
			if (val3 == null)
			{
				LogError("ApplyViewFilter: 找不到过滤器 '" + filterName + "'");
				return false;
			}
			val2.AddFilter(((Element)val3).Id);
			return true;
		}
		catch (Exception ex)
		{
			LogError("ApplyViewFilter 失败: " + ex.Message);
			return false;
		}
	}

	public bool RemoveViewFilter(object document, object view, string filterName)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("RemoveViewFilter: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("RemoveViewFilter: view 不是 View 类型");
				return false;
			}
			ParameterFilterElement val3 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(ParameterFilterElement))).Cast<ParameterFilterElement>().FirstOrDefault((ParameterFilterElement f) => ((Element)f).Name.Equals(filterName, StringComparison.OrdinalIgnoreCase));
			if (val3 == null)
			{
				LogError("RemoveViewFilter: 找不到过滤器 '" + filterName + "'");
				return false;
			}
			val2.RemoveFilter(((Element)val3).Id);
			return true;
		}
		catch (Exception ex)
		{
			LogError("RemoveViewFilter 失败: " + ex.Message);
			return false;
		}
	}

	public object? DuplicateView(object document, object sourceView, string newViewName, string duplicateOptions = "WithDetailing")
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DuplicateView: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((sourceView is View) ? sourceView : null);
			if (val2 == null)
			{
				LogError("DuplicateView: sourceView 不是 View 类型");
				return null;
			}
			ViewDuplicateOption val3 = (duplicateOptions.Equals("WithDetailing", StringComparison.OrdinalIgnoreCase) ? ((ViewDuplicateOption)2) : ((!duplicateOptions.Equals("AsDependent", StringComparison.OrdinalIgnoreCase)) ? ((ViewDuplicateOption)0) : ((ViewDuplicateOption)1)));
			ElementId val4 = val2.Duplicate(val3);
			Element element = val.GetElement(val4);
			View val5 = (View)(object)((element is View) ? element : null);
			if (val5 != null && !string.IsNullOrEmpty(newViewName))
			{
				((Element)val5).Name = newViewName;
			}
			return val5;
		}
		catch (Exception ex)
		{
			LogError("DuplicateView 失败: " + ex.Message);
			return null;
		}
	}

	public bool OverrideElementColor(object document, object view, int elementId, object color)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("OverrideElementColor: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("OverrideElementColor: view 不是 View 类型");
				return false;
			}
			OverrideGraphicSettings val3 = new OverrideGraphicSettings();
			Type type = color.GetType();
			PropertyInfo propertyInfo = type.GetProperty("R") ?? type.GetProperty("r");
			PropertyInfo propertyInfo2 = type.GetProperty("G") ?? type.GetProperty("g");
			PropertyInfo propertyInfo3 = type.GetProperty("B") ?? type.GetProperty("b");
			if (propertyInfo != null && propertyInfo2 != null && propertyInfo3 != null)
			{
				byte b = (byte)(propertyInfo.GetValue(color) ?? ((object)0));
				byte b2 = (byte)(propertyInfo2.GetValue(color) ?? ((object)0));
				byte b3 = (byte)(propertyInfo3.GetValue(color) ?? ((object)0));
				Color projectionLineColor = new Color(b, b2, b3);
				val3.SetProjectionLineColor(projectionLineColor);
			}
			val2.SetElementOverrides(new ElementId((long)elementId), val3);
			return true;
		}
		catch (Exception ex)
		{
			LogError("OverrideElementColor 失败: " + ex.Message);
			return false;
		}
	}

	public object? CreateSchedule(object document, string categoryName, string scheduleName)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateSchedule: document 不是 Document 类型");
				return null;
			}
			Category val2 = null;
			foreach (Category item in (CategoryNameMap)val.Settings.Categories)
			{
				Category val3 = item;
				if (val3.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
				{
					val2 = val3;
					break;
				}
			}
			if (val2 == null)
			{
				LogError("CreateSchedule: 找不到类别 '" + categoryName + "'");
				return null;
			}
			ViewSchedule val4 = ViewSchedule.CreateSchedule(val, val2.Id);
			if (!string.IsNullOrEmpty(scheduleName))
			{
				((Element)val4).Name = scheduleName;
			}
			return val4;
		}
		catch (Exception ex)
		{
			LogError("CreateSchedule 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateScheduleAdvanced(object document, string categoryName, string scheduleName, object config)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Expected O, but got Unknown
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Expected O, but got Unknown
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Expected O, but got Unknown
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			_003C_003Ec__DisplayClass20_1 _003C_003Ec__DisplayClass20_1_ = default(_003C_003Ec__DisplayClass20_1);
			_003C_003Ec__DisplayClass20_1_.doc = (Document)((document is Document) ? document : null);
			if (_003C_003Ec__DisplayClass20_1_.doc == null)
			{
				LogError("CreateScheduleAdvanced: document 不是 Document 类型");
				return null;
			}
			List<ViewSchedule> source = new FilteredElementCollector(_003C_003Ec__DisplayClass20_1_.doc).OfClass(typeof(ViewSchedule)).ToElements().Cast<ViewSchedule>()
				.ToList();
			if (source.Any((ViewSchedule s) => ((Element)s).Name == scheduleName))
			{
				LogError("CreateScheduleAdvanced: 已存在同名明细表 '" + scheduleName + "'");
				return null;
			}
			Category val = null;
			bool flag;
			if (!(flag = string.Equals(categoryName, "MultiCategory", StringComparison.OrdinalIgnoreCase)))
			{
				Categories categories = _003C_003Ec__DisplayClass20_1_.doc.Settings.Categories;
				foreach (Category item in (CategoryNameMap)categories)
				{
					Category val2 = item;
					if (string.Equals(val2.Name, categoryName, StringComparison.OrdinalIgnoreCase))
					{
						val = val2;
						break;
					}
				}
				if (val == null)
				{
					LogError("CreateScheduleAdvanced: 找不到类别 '" + categoryName + "'");
					return null;
				}
			}
			if (flag)
			{
				_003C_003Ec__DisplayClass20_1_.schedule = ViewSchedule.CreateSchedule(_003C_003Ec__DisplayClass20_1_.doc, new ElementId((BuiltInCategory)(-1L)));
			}
			else
			{
				if (val == null)
				{
					LogError("CreateScheduleAdvanced: 找不到类别 '" + categoryName + "'");
					return null;
				}
				_003C_003Ec__DisplayClass20_1_.schedule = ViewSchedule.CreateSchedule(_003C_003Ec__DisplayClass20_1_.doc, val.Id);
			}
			((Element)_003C_003Ec__DisplayClass20_1_.schedule).Name = scheduleName;
			IList<string> configProperty = GetConfigProperty<IList<string>>(config, "FieldUniqueIds");
			string configProperty2 = GetConfigProperty<string>(config, "SortByUniqueId");
			string configProperty3 = GetConfigProperty(config, "SortOrder", "ascending");
			string configProperty4 = GetConfigProperty<string>(config, "FilterFieldUniqueId");
			string configProperty5 = GetConfigProperty(config, "FilterType", "contains");
			string configProperty6 = GetConfigProperty<string>(config, "FilterValue");
			string configProperty7 = GetConfigProperty<string>(config, "GroupByUniqueId");
			IList<SchedulableField> schedulableFields = _003C_003Ec__DisplayClass20_1_.schedule.Definition.GetSchedulableFields();
			_003C_003Ec__DisplayClass20_1_.runtimeFieldMap = new Dictionary<string, SchedulableField>();
			foreach (SchedulableField item2 in schedulableFields)
			{
				string key = item2.ParameterId.Value.ToString();
				if (!_003C_003Ec__DisplayClass20_1_.runtimeFieldMap.ContainsKey(key))
				{
					_003C_003Ec__DisplayClass20_1_.runtimeFieldMap.Add(key, item2);
				}
			}
			HashSet<long> hashSet = new HashSet<long>();
			if (configProperty != null && configProperty.Count > 0)
			{
				foreach (string item3 in configProperty)
				{
					SchedulableField val3 = smethod_0(item3, ref _003C_003Ec__DisplayClass20_1_);
					if (val3 != (SchedulableField)null)
					{
						long value = val3.ParameterId.Value;
						if (hashSet.Contains(value))
						{
							Logger.Info("[ViewService] UniqueId '" + item3 + "' 已存在，跳过重复添加");
							continue;
						}
						_003C_003Ec__DisplayClass20_1_.schedule.Definition.AddField(val3);
						hashSet.Add(value);
					}
				}
			}
			if (!string.IsNullOrEmpty(configProperty7))
			{
				SchedulableField val4 = smethod_0(configProperty7, ref _003C_003Ec__DisplayClass20_1_);
				if (val4 != (SchedulableField)null)
				{
					long value2 = val4.ParameterId.Value;
					ScheduleField val5 = null;
					if (hashSet.Contains(value2))
					{
						ScheduleField val6 = smethod_1(value2, ref _003C_003Ec__DisplayClass20_1_);
						if (val6 != null)
						{
							val5 = val6;
							Logger.Info("[ViewService] 分组字段 UniqueId '" + configProperty7 + "' 已存在，使用已有字段");
						}
						else
						{
							Logger.Error("[ViewService] 分组字段 UniqueId '" + configProperty7 + "' 理论上应存在但未找到");
						}
					}
					else
					{
						val5 = _003C_003Ec__DisplayClass20_1_.schedule.Definition.AddField(val4);
						hashSet.Add(value2);
						Logger.Info("[ViewService] 为分组功能自动添加字段，UniqueId: '" + configProperty7 + "'");
					}
					if (val5 != null)
					{
						ScheduleSortGroupField val7 = new ScheduleSortGroupField(val5.FieldId);
						val7.ShowHeader = true;
						_003C_003Ec__DisplayClass20_1_.schedule.Definition.AddSortGroupField(val7);
						Logger.Info("[ViewService] 成功添加分组，UniqueId: '" + configProperty7 + "'");
					}
				}
				else
				{
					Logger.Warning("[ViewService] 无法找到分组字段 UniqueId: '" + configProperty7 + "'");
				}
			}
			if (!string.IsNullOrEmpty(configProperty2))
			{
				SchedulableField val8 = smethod_0(configProperty2, ref _003C_003Ec__DisplayClass20_1_);
				if (val8 != (SchedulableField)null)
				{
					long value3 = val8.ParameterId.Value;
					ScheduleField val9 = null;
					if (hashSet.Contains(value3))
					{
						ScheduleField val10 = smethod_1(value3, ref _003C_003Ec__DisplayClass20_1_);
						if (val10 != null)
						{
							val9 = val10;
							Logger.Info("[ViewService] 排序字段 UniqueId '" + configProperty2 + "' 已存在，使用已有字段");
						}
						else
						{
							Logger.Error("[ViewService] 排序字段 UniqueId '" + configProperty2 + "' 理论上应存在但未找到");
						}
					}
					else
					{
						val9 = _003C_003Ec__DisplayClass20_1_.schedule.Definition.AddField(val8);
						hashSet.Add(value3);
						Logger.Info("[ViewService] 为排序功能自动添加字段，UniqueId: '" + configProperty2 + "'");
					}
					if (val9 != null)
					{
						ScheduleSortGroupField val11 = new ScheduleSortGroupField(val9.FieldId);
						val11.SortOrder = (ScheduleSortOrder)(string.Equals(configProperty3, "descending", StringComparison.OrdinalIgnoreCase) ? 1 : 0);
						_003C_003Ec__DisplayClass20_1_.schedule.Definition.AddSortGroupField(val11);
						Logger.Info("[ViewService] 成功添加排序，UniqueId: '" + configProperty2 + "' " + configProperty3);
					}
				}
				else
				{
					Logger.Warning("[ViewService] 无法找到排序字段 UniqueId: '" + configProperty2 + "'");
				}
			}
			if (!string.IsNullOrEmpty(configProperty4) && !string.IsNullOrEmpty(configProperty6) && !string.IsNullOrEmpty(configProperty5))
			{
				SchedulableField val12 = smethod_0(configProperty4, ref _003C_003Ec__DisplayClass20_1_);
				if (val12 != (SchedulableField)null)
				{
					long value4 = val12.ParameterId.Value;
					ScheduleField val13 = null;
					if (hashSet.Contains(value4))
					{
						ScheduleField val14 = smethod_1(value4, ref _003C_003Ec__DisplayClass20_1_);
						if (val14 != null)
						{
							val13 = val14;
							Logger.Info("[ViewService] 过滤字段 UniqueId '" + configProperty4 + "' 已存在，使用已有字段");
						}
						else
						{
							Logger.Error("[ViewService] 过滤字段 UniqueId '" + configProperty4 + "' 理论上应存在但未找到");
						}
					}
					else
					{
						val13 = _003C_003Ec__DisplayClass20_1_.schedule.Definition.AddField(val12);
						hashSet.Add(value4);
						Logger.Info("[ViewService] 为过滤功能自动添加字段，UniqueId: '" + configProperty4 + "'");
					}
					if (val13 != null)
					{
						string text = configProperty5.ToLower();
						ScheduleFilterType val15 = ((text == "equal") ? ((ScheduleFilterType)2) : ((text == "greater_than") ? ((ScheduleFilterType)4) : ((text == "less_than") ? ((ScheduleFilterType)6) : ((ScheduleFilterType)8))));
						ScheduleFilterType val16 = val15;
						ScheduleFilter val17 = new ScheduleFilter(val13.FieldId, val16, configProperty6);
						_003C_003Ec__DisplayClass20_1_.schedule.Definition.AddFilter(val17);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 3);
						defaultInterpolatedStringHandler.AppendLiteral("[ViewService] 成功添加过滤，UniqueId: '");
						defaultInterpolatedStringHandler.AppendFormatted(configProperty4);
						defaultInterpolatedStringHandler.AppendLiteral("' ");
						defaultInterpolatedStringHandler.AppendFormatted<ScheduleFilterType>(val16);
						defaultInterpolatedStringHandler.AppendLiteral(" '");
						defaultInterpolatedStringHandler.AppendFormatted(configProperty6);
						defaultInterpolatedStringHandler.AppendLiteral("'");
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				else
				{
					Logger.Warning("[ViewService] 无法找到过滤字段 UniqueId: '" + configProperty4 + "'");
				}
			}
			Logger.Info("CreateScheduleAdvanced: 成功创建明细表 '" + scheduleName + "'，类别: " + categoryName);
			return _003C_003Ec__DisplayClass20_1_.schedule;
		}
		catch (Exception ex)
		{
			LogError("CreateScheduleAdvanced 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateSectionView(object document, string viewName, string direction, object minPoint, object maxPoint)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateSectionView: document 不是 Document 类型");
				return null;
			}
			XYZ val2 = (XYZ)((minPoint is XYZ) ? minPoint : null);
			if (val2 == null)
			{
				LogError("CreateSectionView: minPoint 不是 XYZ 类型");
				return null;
			}
			XYZ val3 = (XYZ)((maxPoint is XYZ) ? maxPoint : null);
			if (val3 == null)
			{
				LogError("CreateSectionView: maxPoint 不是 XYZ 类型");
				return null;
			}
			ViewFamilyType sectionViewFamilyType = GetSectionViewFamilyType(val);
			if (sectionViewFamilyType == null)
			{
				LogError("CreateSectionView: 无法获取剖面视图类型");
				return null;
			}
			string text = direction.ToLower();
			string text2 = text;
			BoundingBoxXYZ val4;
			if (!(text2 == "left_to_right"))
			{
				if (!(text2 == "right_to_left"))
				{
					if (!(text2 == "bottom_to_top"))
					{
						if (!(text2 == "top_to_bottom"))
						{
							LogError("CreateSectionView: 不支持的方向 '" + direction + "'");
							return null;
						}
						val4 = CreateBoundingBoxFromTopToBottom(val2, val3);
					}
					else
					{
						val4 = CreateBoundingBoxFromBottomToTop(val2, val3);
					}
				}
				else
				{
					val4 = CreateBoundingBoxFromRightToLeft(val2, val3);
				}
			}
			else
			{
				val4 = CreateBoundingBoxFromLeftToRight(val2, val3);
			}
			ViewSection val5 = ViewSection.CreateSection(val, ((Element)sectionViewFamilyType).Id, val4);
			if (val5 == null)
			{
				LogError("CreateSectionView: ViewSection.CreateSection 返回 null");
				return null;
			}
			if (!string.IsNullOrEmpty(viewName))
			{
				((Element)val5).Name = viewName;
			}
			Logger.Info("CreateSectionView: 成功创建剖面视图 '" + viewName + "'，方向: " + direction);
			return val5;
		}
		catch (Exception ex)
		{
			LogError("CreateSectionView 失败: " + ex.Message);
			return null;
		}
	}

	private ViewFamilyType? GetSectionViewFamilyType(Document doc)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		try
		{
			FilteredElementCollector val = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType));
			foreach (ViewFamilyType item in val)
			{
				ViewFamilyType val2 = item;
				if ((int)val2.ViewFamily == 112)
				{
					return val2;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetSectionViewFamilyType 失败: " + ex.Message);
			return null;
		}
	}

	private BoundingBoxXYZ CreateBoundingBoxFromLeftToRight(XYZ modelMin, XYZ modelMax)
	{
		Transform identity = Transform.Identity;
		identity.BasisY = XYZ.BasisZ;
		identity.BasisZ = XYZ.BasisX;
		identity.BasisX = XYZ.BasisY;
		return CreateBoundingBoxFromTransform(identity, modelMin, modelMax);
	}

	private BoundingBoxXYZ CreateBoundingBoxFromRightToLeft(XYZ modelMin, XYZ modelMax)
	{
		Transform identity = Transform.Identity;
		identity.BasisY = XYZ.BasisZ;
		identity.BasisZ = -XYZ.BasisX;
		identity.BasisX = -XYZ.BasisY;
		return CreateBoundingBoxFromTransform(identity, modelMin, modelMax);
	}

	private BoundingBoxXYZ CreateBoundingBoxFromBottomToTop(XYZ modelMin, XYZ modelMax)
	{
		Transform identity = Transform.Identity;
		identity.BasisY = XYZ.BasisZ;
		identity.BasisZ = XYZ.BasisY;
		identity.BasisX = -XYZ.BasisX;
		return CreateBoundingBoxFromTransform(identity, modelMin, modelMax);
	}

	private BoundingBoxXYZ CreateBoundingBoxFromTopToBottom(XYZ modelMin, XYZ modelMax)
	{
		Transform identity = Transform.Identity;
		identity.BasisY = XYZ.BasisZ;
		identity.BasisZ = -XYZ.BasisY;
		identity.BasisX = XYZ.BasisX;
		return CreateBoundingBoxFromTransform(identity, modelMin, modelMax);
	}

	private BoundingBoxXYZ CreateBoundingBoxFromTransform(Transform transform, XYZ modelMin, XYZ modelMax)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b2: Expected O, but got Unknown
		XYZ val = (modelMin + modelMax) / 2.0;
		double num = Math.Abs(modelMax.X - modelMin.X);
		double num2 = Math.Abs(modelMax.Y - modelMin.Y);
		double num3 = Math.Abs(modelMax.Z - modelMin.Z);
		transform.Origin = val;
		BoundingBoxXYZ result = new BoundingBoxXYZ
		{
			Transform = transform,
			Min = new XYZ((0.0 - num) / 2.0, (0.0 - num2) / 2.0, 0.0),
			Max = new XYZ(num / 2.0, num2 / 2.0, num3)
		};
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 6);
		defaultInterpolatedStringHandler.AppendLiteral("CreateBoundingBoxFromTransform: center=(");
		defaultInterpolatedStringHandler.AppendFormatted(val.X, "F2");
		defaultInterpolatedStringHandler.AppendLiteral(",");
		defaultInterpolatedStringHandler.AppendFormatted(val.Y, "F2");
		defaultInterpolatedStringHandler.AppendLiteral(",");
		defaultInterpolatedStringHandler.AppendFormatted(val.Z, "F2");
		defaultInterpolatedStringHandler.AppendLiteral("), size=(");
		defaultInterpolatedStringHandler.AppendFormatted(num, "F2");
		defaultInterpolatedStringHandler.AppendLiteral(",");
		defaultInterpolatedStringHandler.AppendFormatted(num2, "F2");
		defaultInterpolatedStringHandler.AppendLiteral(",");
		defaultInterpolatedStringHandler.AppendFormatted(num3, "F2");
		defaultInterpolatedStringHandler.AppendLiteral(")");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		return result;
	}

	private XYZ GetActualMinPoint(XYZ p1, XYZ p2)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		return new XYZ(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
	}

	private XYZ GetActualMaxPoint(XYZ p1, XYZ p2)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		return new XYZ(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), Math.Max(p1.Z, p2.Z));
	}

	private static T? GetConfigProperty<T>(object config, string propertyName, T? defaultValue = default(T?))
	{
		try
		{
			PropertyInfo property = config.GetType().GetProperty(propertyName);
			if (property != null)
			{
				object value = property.GetValue(config);
				if (value != null)
				{
					return (T)value;
				}
			}
			return defaultValue;
		}
		catch
		{
			return defaultValue;
		}
	}

	private static string? GetFieldName(ScheduleField field, Document doc)
	{
		try
		{
			Element element = doc.GetElement(field.ParameterId);
			ParameterElement val = (ParameterElement)(object)((element is ParameterElement) ? element : null);
			return (val != null) ? ((Element)val).Name : null;
		}
		catch
		{
			return null;
		}
	}

	private static (string displayName, string enumName) GetSchedulableFieldInfo(Document doc, SchedulableField sf)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Element element = doc.GetElement(sf.ParameterId);
			ParameterElement val = (ParameterElement)(object)((element is ParameterElement) ? element : null);
			if (val != null)
			{
				return (displayName: ((Element)val).Name, enumName: string.Empty);
			}
			var (text, item) = GetParameterNameAndEnumFromElement(doc, sf.ParameterId);
			if (!string.IsNullOrEmpty(text))
			{
				return (displayName: text, enumName: item);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
			defaultInterpolatedStringHandler.AppendFormatted(GetFieldTypeName(sf.FieldType));
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted(sf.ParameterId.Value);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			string item2 = defaultInterpolatedStringHandler.ToStringAndClear();
			return (displayName: item2, enumName: string.Empty);
		}
		catch
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("字段 ");
			defaultInterpolatedStringHandler2.AppendFormatted(sf.ParameterId.Value);
			return (displayName: defaultInterpolatedStringHandler2.ToStringAndClear(), enumName: string.Empty);
		}
	}

	private static (string displayName, string enumName) GetParameterNameAndEnumFromElement(Document doc, ElementId paramId)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		try
		{
			BuiltInCategory[] array = new BuiltInCategory[33];
			RuntimeHelpers.InitializeArray(array, RuntimeHandleHelper.FieldOf(typeof(ns0.Class653), "struct10_0"));
			BuiltInCategory[] array2 = (BuiltInCategory[])(object)array;
			string item = string.Empty;
			if (paramId.Value < 0L && Enum.IsDefined(typeof(BuiltInParameter), paramId.Value))
			{
				item = ((object)(BuiltInParameter)paramId.Value/*cast due to constrained. prefix*/).ToString();
			}
			BuiltInCategory[] array3 = array2;
			int num = 0;
			Parameter val4;
			while (true)
			{
				if (num < array3.Length)
				{
					BuiltInCategory val = array3[num];
					FilteredElementCollector val2 = new FilteredElementCollector(doc);
					val2.OfCategory(val);
					val2.WhereElementIsNotElementType();
					Element val3 = ((IEnumerable<Element>)val2).FirstOrDefault();
					if (val3 != null)
					{
						val4 = null;
						try
						{
							if (paramId.Value < 0L)
							{
								if (Enum.IsDefined(typeof(BuiltInParameter), paramId.Value))
								{
									val4 = val3.get_Parameter((BuiltInParameter)paramId.Value);
								}
							}
							else
							{
								foreach (Parameter parameter in val3.Parameters)
								{
									Parameter val5 = parameter;
									if (val5.Id.Value == paramId.Value)
									{
										val4 = val5;
										break;
									}
								}
							}
						}
						catch
						{
						}
						if (val4 != null && val4.Definition != null)
						{
							break;
						}
					}
					num++;
					continue;
				}
				ICollection<ElementId> source = new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElementIds();
				foreach (ElementId item2 in source.Take(300))
				{
					Element element = doc.GetElement(item2);
					if (element == null)
					{
						continue;
					}
					Parameter val6 = null;
					try
					{
						if (paramId.Value < 0L)
						{
							if (Enum.IsDefined(typeof(BuiltInParameter), paramId.Value))
							{
								val6 = element.get_Parameter((BuiltInParameter)paramId.Value);
							}
						}
						else
						{
							foreach (Parameter parameter2 in element.Parameters)
							{
								Parameter val7 = parameter2;
								if (val7.Id.Value == paramId.Value)
								{
									val6 = val7;
									break;
								}
							}
						}
					}
					catch
					{
					}
					if (val6 != null && val6.Definition != null)
					{
						return (displayName: val6.Definition.Name, enumName: item);
					}
				}
				return (displayName: string.Empty, enumName: item);
			}
			return (displayName: val4.Definition.Name, enumName: item);
		}
		catch
		{
			return (displayName: string.Empty, enumName: string.Empty);
		}
	}

	private unsafe static string GetFieldTypeName(ScheduleFieldType fieldType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected I4, but got Unknown
		return (int)fieldType switch
		{
			0 => "实例参数", 
			1 => "类型参数", 
			2 => "统计", 
			_ => ((object)(*(ScheduleFieldType*)(&fieldType))/*cast due to constrained. prefix*/).ToString(), 
		};
	}

	public object? CreateLegend(object document, string viewName)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateLegend: document 不是 Document 类型");
				return null;
			}
			ViewFamilyType viewFamilyType = GetViewFamilyType(val, (ViewFamily)117);
			if (viewFamilyType == null)
			{
				LogError("CreateLegend: 找不到图例视图族类型");
				return null;
			}
			ViewDrafting val2 = ViewDrafting.Create(val, ((Element)viewFamilyType).Id);
			if (val2 != null && !string.IsNullOrEmpty(viewName))
			{
				((Element)val2).Name = viewName;
			}
			return val2;
		}
		catch (Exception ex)
		{
			LogError("CreateLegend 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateLegend(object document, string viewName, int? legendId)
	{
		return CreateLegend(document, viewName);
	}

	public object? CreateSheet(object document, int sheetNumber, string sheetName)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateSheet: document 不是 Document 类型");
				return null;
			}
			Element obj = new FilteredElementCollector(val).OfClass(typeof(FamilySymbol)).OfCategory((BuiltInCategory)(-2000280L)).FirstElement();
			FamilySymbol val2 = (FamilySymbol)(object)((obj is FamilySymbol) ? obj : null);
			if (val2 == null)
			{
				LogError("CreateSheet: 找不到标题栏族类型");
				return null;
			}
			ViewSheet val3 = ViewSheet.Create(val, ((Element)val2).Id);
			((Element)val3).Name = sheetName;
			val3.SheetNumber = sheetNumber.ToString();
			return val3;
		}
		catch (Exception ex)
		{
			LogError("CreateSheet 失败: " + ex.Message);
			return null;
		}
	}

	public object? PlaceViewOnSheet(object document, object sheet, object viewToPlace, object position)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PlaceViewOnSheet: document 不是 Document 类型");
				return null;
			}
			ViewSheet val2 = (ViewSheet)((sheet is ViewSheet) ? sheet : null);
			if (val2 == null)
			{
				LogError("PlaceViewOnSheet: sheet 不是 ViewSheet 类型");
				return null;
			}
			View val3 = (View)((viewToPlace is View) ? viewToPlace : null);
			if (val3 == null)
			{
				LogError("PlaceViewOnSheet: viewToPlace 不是 View 类型");
				return null;
			}
			if (!ParsePoint(position, out XYZ xyz))
			{
				LogError("PlaceViewOnSheet: 无法解析位置参数");
				return null;
			}
			return Viewport.Create(val, ((Element)val2).Id, ((Element)val3).Id, xyz);
		}
		catch (Exception ex)
		{
			LogError("PlaceViewOnSheet 失败: " + ex.Message);
			return null;
		}
	}

	public object? AddViewToSheet(object document, object sheet, object view, object? position)
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("AddViewToSheet: document 不是 Document 类型");
				return null;
			}
			ViewSheet val2 = (ViewSheet)((sheet is ViewSheet) ? sheet : null);
			if (val2 == null)
			{
				LogError("AddViewToSheet: sheet 不是 ViewSheet 类型");
				return null;
			}
			View val3 = (View)((view is View) ? view : null);
			if (val3 == null)
			{
				LogError("AddViewToSheet: view 不是 View 类型");
				return null;
			}
			XYZ val4 = (XYZ)((position == null || !ParsePoint(position, out XYZ xyz) || xyz == null) ? ((object)new XYZ(0.0, 0.0, 0.0)) : ((object)xyz));
			return Viewport.Create(val, ((Element)val2).Id, ((Element)val3).Id, val4);
		}
		catch (Exception ex)
		{
			LogError("AddViewToSheet 失败: " + ex.Message);
			return null;
		}
	}

	public object? PlaceViewOnSheet(object document, object sheet, object viewToPlace, double x, double y)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PlaceViewOnSheet: document 不是 Document 类型");
				return null;
			}
			ViewSheet val2 = (ViewSheet)((sheet is ViewSheet) ? sheet : null);
			if (val2 == null)
			{
				LogError("PlaceViewOnSheet: sheet 不是 ViewSheet 类型");
				return null;
			}
			View val3 = (View)((viewToPlace is View) ? viewToPlace : null);
			if (val3 == null)
			{
				LogError("PlaceViewOnSheet: viewToPlace 不是 View 类型");
				return null;
			}
			XYZ val4 = new XYZ(x, y, 0.0);
			return Viewport.Create(val, ((Element)val2).Id, ((Element)val3).Id, val4);
		}
		catch (Exception ex)
		{
			LogError("PlaceViewOnSheet 失败: " + ex.Message);
			return null;
		}
	}

	public bool OverrideElementColor(object document, object view, int elementId, object color, int? patternId)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("OverrideElementColor: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("OverrideElementColor: view 不是 View 类型");
				return false;
			}
			OverrideGraphicSettings val3 = new OverrideGraphicSettings();
			Type type = color.GetType();
			PropertyInfo propertyInfo = type.GetProperty("R") ?? type.GetProperty("r");
			PropertyInfo propertyInfo2 = type.GetProperty("G") ?? type.GetProperty("g");
			PropertyInfo propertyInfo3 = type.GetProperty("B") ?? type.GetProperty("b");
			if (propertyInfo != null && propertyInfo2 != null && propertyInfo3 != null)
			{
				byte b = (byte)(propertyInfo.GetValue(color) ?? ((object)0));
				byte b2 = (byte)(propertyInfo2.GetValue(color) ?? ((object)0));
				byte b3 = (byte)(propertyInfo3.GetValue(color) ?? ((object)0));
				Color projectionLineColor = new Color(b, b2, b3);
				val3.SetProjectionLineColor(projectionLineColor);
			}
			if (!patternId.HasValue)
			{
			}
			val2.SetElementOverrides(new ElementId((long)elementId), val3);
			return true;
		}
		catch (Exception ex)
		{
			LogError("OverrideElementColor 失败: " + ex.Message);
			return false;
		}
	}

	public object? CreateSchedule(object document, string categoryName, string scheduleName, int? viewTypeId)
	{
		return CreateSchedule(document, categoryName, scheduleName);
	}

	private bool ParsePoint(object pointObj, out XYZ? xyz)
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		try
		{
			if (pointObj == null)
			{
				xyz = null;
				return false;
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (pointObj is IDictionary<string, object> dictionary)
			{
				if (dictionary.TryGetValue("x", out var value) && value != null)
				{
					num = Convert.ToDouble(value);
					flag = true;
				}
				if (dictionary.TryGetValue("y", out var value2) && value2 != null)
				{
					num2 = Convert.ToDouble(value2);
					flag2 = true;
				}
				if (dictionary.TryGetValue("z", out var value3) && value3 != null)
				{
					num3 = Convert.ToDouble(value3);
					flag3 = true;
				}
				if (flag & flag2 & flag3)
				{
					xyz = new XYZ(num, num2, num3);
					return true;
				}
			}
			Type type = pointObj.GetType();
			PropertyInfo property = type.GetProperty("x");
			PropertyInfo property2 = type.GetProperty("y");
			PropertyInfo property3 = type.GetProperty("z");
			if (property != null && property2 != null && property3 != null)
			{
				num = Convert.ToDouble(property.GetValue(pointObj));
				num2 = Convert.ToDouble(property2.GetValue(pointObj));
				num3 = Convert.ToDouble(property3.GetValue(pointObj));
				xyz = new XYZ(num, num2, num3);
				return true;
			}
			xyz = null;
			return false;
		}
		catch
		{
			xyz = null;
			return false;
		}
	}

	private ViewFamilyType? GetViewFamilyType(Document doc, ViewFamily viewFamily)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType))).Cast<ViewFamilyType>().FirstOrDefault(delegate(ViewFamilyType vft)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return vft.ViewFamily == viewFamily;
		});
	}

	public bool IsSystemView(object view)
	{
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				return false;
			}
			return val.IsTemplate;
		}
		catch (Exception ex)
		{
			LogError("IsSystemView 失败: " + ex.Message);
			return false;
		}
	}

	public bool IsTemplate(object view)
	{
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				return false;
			}
			return val.IsTemplate;
		}
		catch (Exception ex)
		{
			LogError("IsTemplate 失败: " + ex.Message);
			return false;
		}
	}

	public bool CanPrint(object view)
	{
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				return false;
			}
			return !val.IsTemplate;
		}
		catch (Exception ex)
		{
			LogError("CanPrint 失败: " + ex.Message);
			return false;
		}
	}

	private static IEnumerable<object> AutoCorrectParameterNames(IEnumerable<object> rules, IEnumerable<string> categoryNames)
	{
		Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
		{
			["窗"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["宽度"] = "窗宽度",
				["高度"] = "窗高度",
				["厚度"] = "窗厚度",
				["粗略宽度"] = "窗宽度",
				["粗略高度"] = "窗高度"
			},
			["门"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["宽度"] = "门宽度",
				["高度"] = "门高度",
				["厚度"] = "门厚度",
				["粗略宽度"] = "门宽度",
				["粗略高度"] = "门高度"
			},
			["墙"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["厚度"] = "墙厚度" },
			["楼板"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["厚度"] = "楼板厚度" },
			["房间"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["面积"] = "房间面积",
				["体积"] = "房间体积",
				["周长"] = "房间周长"
			}
		};
		List<object> list = new List<object>();
		foreach (object rule in rules)
		{
			Type type = rule.GetType();
			if (type.FullName == "Newtonsoft.Json.Linq.JObject")
			{
				PropertyInfo property = type.GetProperty("Item", new Type[1] { typeof(string) });
				if (property != null)
				{
					string text = property.GetValue(rule, new object[1] { "parameterName" })?.ToString();
					if (!string.IsNullOrEmpty(text))
					{
						text = text.Trim('"');
						foreach (string categoryName in categoryNames)
						{
							if (!dictionary.TryGetValue(categoryName, out var value) || !value.TryGetValue(text, out var value2))
							{
								continue;
							}
							try
							{
								object obj = type.GetMethod("Property", new Type[1] { typeof(string) })?.Invoke(rule, new object[1] { "parameterName" });
								if (obj == null)
								{
									break;
								}
								Type type2 = obj.GetType();
								PropertyInfo property2 = type2.GetProperty("Value");
								if (property2 != null)
								{
									Type type3 = type.Assembly.GetType("Newtonsoft.Json.Linq.JValue");
									if (type3 != null)
									{
										object value3 = Activator.CreateInstance(type3, value2);
										property2.SetValue(obj, value3);
									}
									else
									{
										property2.SetValue(obj, value2);
									}
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 3);
									defaultInterpolatedStringHandler.AppendLiteral("[ViewService] AutoCorrectParameterNames: '");
									defaultInterpolatedStringHandler.AppendFormatted(text);
									defaultInterpolatedStringHandler.AppendLiteral("' → '");
									defaultInterpolatedStringHandler.AppendFormatted(value2);
									defaultInterpolatedStringHandler.AppendLiteral("' (类别=");
									defaultInterpolatedStringHandler.AppendFormatted(categoryName);
									defaultInterpolatedStringHandler.AppendLiteral(")");
									Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
								}
							}
							catch (Exception ex)
							{
								Logger.Warning("[ViewService] AutoCorrectParameterNames 失败: " + ex.Message);
							}
							break;
						}
					}
				}
			}
			list.Add(rule);
		}
		return list;
	}

	public int? CreateViewFilter(object document, string filterName, IEnumerable<string> categoryNames, IEnumerable<object> rules)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateViewFilter: document 不是 Document 类型");
				return null;
			}
			ParameterFilterElement val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(ParameterFilterElement))).Cast<ParameterFilterElement>().FirstOrDefault((ParameterFilterElement f) => ((Element)f).Name.Equals(filterName, StringComparison.OrdinalIgnoreCase));
			if (val2 != null)
			{
				Logger.Warning("[ViewService] CreateViewFilter: 过滤器 '" + filterName + "' 已存在，将返回现有过滤器 ID");
				return (int)((Element)val2).Id.Value;
			}
			List<ElementId> list = new List<ElementId>();
			foreach (string categoryName in categoryNames)
			{
				Category val3 = FindCategory(val, categoryName);
				if (val3 != null)
				{
					list.Add(val3.Id);
				}
				else
				{
					Logger.Warning("[ViewService] CreateViewFilter: 找不到类别 '" + categoryName + "'");
				}
			}
			if (list.Count == 0)
			{
				LogError("CreateViewFilter: 没有找到任何有效的类别。提供的类别名称: " + string.Join(", ", categoryNames));
				return null;
			}
			IEnumerable<object> enumerable = AutoCorrectParameterNames(rules, categoryNames);
			List<FilterRule> list2 = new List<FilterRule>();
			foreach (object item in enumerable)
			{
				FilterRule val4 = CreateFilterRule(val, item, list);
				if (val4 != null)
				{
					list2.Add(val4);
				}
				else
				{
					Logger.Warning("[ViewService] CreateFilterRule: 规则创建失败，跳过");
				}
			}
			if (list2.Count == 0)
			{
				LogError("CreateViewFilter: 没有创建任何有效的过滤规则");
				return null;
			}
			ElementFilter val5;
			try
			{
				val5 = (ElementFilter)new ElementParameterFilter((IList<FilterRule>)list2);
			}
			catch (Exception ex)
			{
				LogError("CreateViewFilter: 创建 ElementParameterFilter 失败 - " + ex.Message);
				return null;
			}
			ParameterFilterElement val6;
			try
			{
				val6 = ParameterFilterElement.Create(val, filterName, (ICollection<ElementId>)list, val5);
			}
			catch (Exception ex2)
			{
				LogError("CreateViewFilter: ParameterFilterElement.Create 失败 - " + ex2.Message);
				return null;
			}
			return (int)((Element)val6).Id.Value;
		}
		catch (Exception ex3)
		{
			LogError("CreateViewFilter 失败: " + ex3.Message);
			return null;
		}
	}

	public bool DeleteViewFilter(object document, string filterName)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DeleteViewFilter: document 不是 Document 类型");
				return false;
			}
			ParameterFilterElement val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(ParameterFilterElement))).Cast<ParameterFilterElement>().FirstOrDefault((ParameterFilterElement f) => ((Element)f).Name.Equals(filterName, StringComparison.OrdinalIgnoreCase));
			if (val2 == null)
			{
				LogError("DeleteViewFilter: 找不到过滤器 '" + filterName + "'");
				return false;
			}
			val.Delete(((Element)val2).Id);
			Logger.Info("[ViewService] DeleteViewFilter: 成功删除过滤器 '" + filterName + "'");
			return true;
		}
		catch (Exception ex)
		{
			LogError("DeleteViewFilter 失败: " + ex.Message);
			return false;
		}
	}

	public bool SetViewDetailLevel(object view, int detailLevel)
	{
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				LogError("SetViewDetailLevel: view 不是 View 类型");
				return false;
			}
			if (detailLevel < 1 || detailLevel > 3)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
				defaultInterpolatedStringHandler.AppendLiteral("SetViewDetailLevel: 无效的 detailLevel 值: ");
				defaultInterpolatedStringHandler.AppendFormatted(detailLevel);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			Parameter val2 = ((Element)val).get_Parameter((BuiltInParameter)(-1011002L));
			if (val2 == null)
			{
				LogError("SetViewDetailLevel: 无法获取 VIEW_DETAIL_LEVEL 参数");
				return false;
			}
			val2.Set(detailLevel);
			string text = detailLevel switch
			{
				2 => "中等", 
				1 => "粗略", 
				_ => "精细", 
			};
			Logger.Info("[ViewService] SetViewDetailLevel: 成功设置视图 '" + ((Element)val).Name + "' 详细程度为 " + text);
			return true;
		}
		catch (Exception ex)
		{
			LogError("SetViewDetailLevel 失败: " + ex.Message);
			return false;
		}
	}

	public bool SetViewDisplayStyle(object view, int displayStyle)
	{
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				LogError("SetViewDisplayStyle: view 不是 View 类型");
				return false;
			}
			int[] array = new int[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
			if (!array.Contains(displayStyle))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 1);
				defaultInterpolatedStringHandler.AppendLiteral("SetViewDisplayStyle: 无效的 displayStyle 值: ");
				defaultInterpolatedStringHandler.AppendFormatted(displayStyle);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			Parameter val2 = ((Element)val).get_Parameter((BuiltInParameter)(-1005165L));
			if (val2 == null)
			{
				LogError("SetViewDisplayStyle: 无法获取 MODEL_GRAPHICS_STYLE 参数");
				return false;
			}
			val2.Set(displayStyle);
			Dictionary<int, string> dictionary = new Dictionary<int, string>
			{
				{
					1,
					"线框"
				},
				{
					2,
					"隐藏线"
				},
				{
					3,
					"着色"
				},
				{
					4,
					"着色并显示边"
				},
				{
					5,
					"渲染"
				},
				{
					6,
					"真实"
				},
				{
					7,
					"平面颜色"
				},
				{
					8,
					"真实并显示边"
				}
			};
			string valueOrDefault = dictionary.GetValueOrDefault(displayStyle, displayStyle.ToString());
			Logger.Info("[ViewService] SetViewDisplayStyle: 成功设置视图 '" + ((Element)val).Name + "' 视觉样式为 " + valueOrDefault);
			return true;
		}
		catch (Exception ex)
		{
			LogError("SetViewDisplayStyle 失败: " + ex.Message);
			return false;
		}
	}

	public bool SetCategoryVisibility(object document, object view, string categoryName, bool visible)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("SetCategoryVisibility: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("SetCategoryVisibility: view 不是 View 类型");
				return false;
			}
			if (string.IsNullOrEmpty(categoryName))
			{
				LogError("SetCategoryVisibility: categoryName 为空");
				return false;
			}
			Category val3 = FindCategory(val, categoryName);
			if (val3 == null)
			{
				LogError("SetCategoryVisibility: 找不到类别 '" + categoryName + "'");
				return false;
			}
			bool flag = true;
			try
			{
				PropertyInfo property = ((object)val3).GetType().GetProperty("AllowsVisibilityControl");
				if (property != null && property.PropertyType == typeof(bool))
				{
					if (property.GetValue(val3) is bool flag2)
					{
						flag = flag2;
					}
				}
				else
				{
					MethodInfo method = ((object)val3).GetType().GetMethod("AllowsVisibilityControl", new Type[2]
					{
						typeof(Document),
						typeof(View)
					});
					if (method != null && method.Invoke(val3, new object[2] { val, val2 }) is bool flag3)
					{
						flag = flag3;
					}
				}
			}
			catch
			{
				flag = true;
			}
			if (!flag)
			{
				string text = "SetCategoryVisibility: 类别 '" + categoryName + "' 不允许在当前视图中控制可见性";
				Logger.Warning("[ViewService] " + text);
				return false;
			}
			val2.SetCategoryHidden(val3.Id, !visible);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[ViewService] SetCategoryVisibility: 成功");
			defaultInterpolatedStringHandler.AppendFormatted(visible ? "显示" : "隐藏");
			defaultInterpolatedStringHandler.AppendLiteral("类别 '");
			defaultInterpolatedStringHandler.AppendFormatted(categoryName);
			defaultInterpolatedStringHandler.AppendLiteral("' 在视图 '");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Name);
			defaultInterpolatedStringHandler.AppendLiteral("' 中");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("SetCategoryVisibility 失败: " + ex.Message);
			return false;
		}
	}

	public bool? GetCategoryVisibility(object document, object view, string categoryName)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetCategoryVisibility: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("GetCategoryVisibility: view 不是 View 类型");
				return null;
			}
			if (string.IsNullOrEmpty(categoryName))
			{
				LogError("GetCategoryVisibility: categoryName 为空");
				return null;
			}
			Category val3 = FindCategory(val, categoryName);
			if (val3 == null)
			{
				Logger.Warning("[ViewService] GetCategoryVisibility: 找不到类别 '" + categoryName + "'");
				return null;
			}
			bool flag = false;
			try
			{
				MethodInfo method = ((object)val2).GetType().GetMethod("IsCategoryHidden", new Type[1] { typeof(ElementId) });
				if (method != null)
				{
					if (method.Invoke(val2, new object[1] { val3.Id }) is bool flag2)
					{
						flag = flag2;
					}
				}
				else
				{
					MethodInfo method2 = ((object)val2).GetType().GetMethod("TryGetCategoryHidden", new Type[2]
					{
						typeof(ElementId),
						typeof(bool).MakeByRefType()
					});
					if (method2 != null)
					{
						object[] array = new object[2] { val3.Id, false };
						object obj = method2.Invoke(val2, array);
						bool flag3 = default(bool);
						int num;
						if (obj is bool)
						{
							flag3 = (bool)obj;
							num = 1;
						}
						else
						{
							num = 0;
						}
						if (((uint)num & (flag3 ? 1u : 0u)) != 0 && array[1] is bool flag4)
						{
							flag = flag4;
						}
					}
					else
					{
						flag = false;
					}
				}
			}
			catch
			{
				flag = false;
			}
			return !flag;
		}
		catch (Exception ex)
		{
			LogError("GetCategoryVisibility 失败: " + ex.Message);
			return null;
		}
	}

	public IEnumerable<object> GetAllCategoriesVisibility(object document, object view)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetAllCategoriesVisibility: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("GetAllCategoriesVisibility: view 不是 View 类型");
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			bool flag5 = default(bool);
			foreach (Category item in (CategoryNameMap)val.Settings.Categories)
			{
				Category val3 = item;
				if (val3 == null)
				{
					continue;
				}
				try
				{
					bool flag = true;
					try
					{
						PropertyInfo property = ((object)val3).GetType().GetProperty("AllowsVisibilityControl");
						if (property != null && property.PropertyType == typeof(bool) && property.GetValue(val3) is bool flag2)
						{
							flag = flag2;
						}
					}
					catch
					{
						flag = true;
					}
					if (!flag)
					{
						continue;
					}
					bool flag3 = false;
					try
					{
						MethodInfo method = ((object)val2).GetType().GetMethod("IsCategoryHidden", new Type[1] { typeof(ElementId) });
						if (method != null)
						{
							if (method.Invoke(val2, new object[1] { val3.Id }) is bool flag4)
							{
								flag3 = flag4;
							}
						}
						else
						{
							MethodInfo method2 = ((object)val2).GetType().GetMethod("TryGetCategoryHidden", new Type[2]
							{
								typeof(ElementId),
								typeof(bool).MakeByRefType()
							});
							if (method2 != null)
							{
								object[] array = new object[2] { val3.Id, false };
								object obj2 = method2.Invoke(val2, array);
								int num;
								if (obj2 is bool)
								{
									flag5 = (bool)obj2;
									num = 1;
								}
								else
								{
									num = 0;
								}
								if (((uint)num & (flag5 ? 1u : 0u)) != 0 && array[1] is bool flag6)
								{
									flag3 = flag6;
								}
							}
						}
					}
					catch
					{
						flag3 = false;
					}
					list.Add(new Class330<string, bool>(val3.Name, !flag3));
				}
				catch
				{
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(64, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ViewService] GetAllCategoriesVisibility: 成功获取视图 '");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Name);
			defaultInterpolatedStringHandler.AppendLiteral("' 中 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个类别的可见性状态");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetAllCategoriesVisibility 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	private Category? FindCategory(Document doc, string categoryName)
	{
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		try
		{
			if (categoryName.Contains('/'))
			{
				string[] array = categoryName.Split('/');
				if (array.Length == 2)
				{
					string value = array[0].Trim();
					string value2 = array[1].Trim();
					Category val = null;
					foreach (Category item in (CategoryNameMap)doc.Settings.Categories)
					{
						Category val2 = item;
						if (val2.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							val = val2;
							break;
						}
					}
					if (val != null)
					{
						try
						{
							PropertyInfo property = ((object)val).GetType().GetProperty("SubCategories");
							if (property != null && property.GetValue(val) is IEnumerable enumerable)
							{
								foreach (Category item2 in enumerable)
								{
									Category val3 = item2;
									if (val3 != null && val3.Name.Equals(value2, StringComparison.OrdinalIgnoreCase))
									{
										Logger.Info("[ViewService] FindCategory: 找到子类别 '" + categoryName + "'");
										return val3;
									}
								}
							}
						}
						catch
						{
						}
					}
				}
			}
			foreach (Category item3 in (CategoryNameMap)doc.Settings.Categories)
			{
				Category val4 = item3;
				if (val4.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
				{
					return val4;
				}
			}
			foreach (Category item4 in (CategoryNameMap)doc.Settings.Categories)
			{
				Category val5 = item4;
				try
				{
					PropertyInfo property2 = ((object)val5).GetType().GetProperty("SubCategories");
					if (!(property2 != null) || !(property2.GetValue(val5) is IEnumerable enumerable2))
					{
						continue;
					}
					foreach (Category item5 in enumerable2)
					{
						Category val6 = item5;
						if (val6 != null && val6.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
							defaultInterpolatedStringHandler.AppendLiteral("[ViewService] FindCategory: 在父类别 '");
							defaultInterpolatedStringHandler.AppendFormatted(val5.Name);
							defaultInterpolatedStringHandler.AppendLiteral("' 下找到子类别 '");
							defaultInterpolatedStringHandler.AppendFormatted(categoryName);
							defaultInterpolatedStringHandler.AppendLiteral("'");
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							return val6;
						}
					}
				}
				catch
				{
				}
			}
			Array values = Enum.GetValues(typeof(BuiltInCategory));
			foreach (BuiltInCategory item6 in values)
			{
				Category category = Category.GetCategory(doc, item6);
				if (category != null && category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
				{
					return category;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("FindCategory 失败: " + ex.Message);
			return null;
		}
	}

	private bool IsParameterElementIdType(Document doc, ElementId parameterId)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (parameterId.Value < 0L)
			{
				string text = ((object)(BuiltInParameter)parameterId.Value/*cast due to constrained. prefix*/).ToString();
				return text.Contains("Type") || text.Contains("Id");
			}
			try
			{
				Element element = doc.GetElement(parameterId);
				ParameterElement val = (ParameterElement)(object)((element is ParameterElement) ? element : null);
				if (val != null)
				{
					InternalDefinition definition = val.GetDefinition();
					if (definition != null)
					{
						PropertyInfo property = ((object)definition).GetType().GetProperty("StorageType");
						if (property != null)
						{
							object value = property.GetValue(definition);
							if (value != null && value.ToString() == "ElementId")
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
		catch
		{
			return false;
		}
	}

	private FilterRule? CreateFilterRule(Document doc, object ruleObj, IList<ElementId> categories)
	{
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Expected O, but got Unknown
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Expected O, but got Unknown
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Expected O, but got Unknown
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Expected O, but got Unknown
		try
		{
			string parameterName = null;
			string text = null;
			object obj = null;
			string text2 = null;
			Type type = ruleObj.GetType();
			if (type.FullName == "Newtonsoft.Json.Linq.JObject")
			{
				PropertyInfo property = type.GetProperty("Item", new Type[1] { typeof(string) });
				if (property != null)
				{
					string[] array = new string[2]
					{
						"parameterName",
						"parameter_name"
					};
					foreach (string text3 in array)
					{
						object value = property.GetValue(ruleObj, new object[1] { text3 });
						if (value == null)
						{
							continue;
						}
						object obj2 = smethod_2(value);
						if (obj2 != null)
						{
							parameterName = obj2.ToString();
							if (!string.IsNullOrEmpty(parameterName))
							{
								break;
							}
						}
					}
					string[] array2 = new string[4]
					{
						"@operator",
						"operator",
						"op",
						"opName"
					};
					foreach (string text4 in array2)
					{
						object value2 = property.GetValue(ruleObj, new object[1] { text4 });
						if (value2 == null)
						{
							continue;
						}
						object obj3 = smethod_2(value2);
						if (obj3 != null)
						{
							text = obj3.ToString();
							if (!string.IsNullOrEmpty(text))
							{
								break;
							}
						}
					}
					object value3 = property.GetValue(ruleObj, new object[1] { "value" });
					if (value3 != null)
					{
						obj = smethod_2(value3, preserveOriginal: true);
					}
					object value4 = property.GetValue(ruleObj, new object[1] { "unit" });
					if (value4 != null)
					{
						object obj4 = smethod_2(value4);
						if (obj4 != null)
						{
							text2 = obj4.ToString();
						}
					}
				}
			}
			else if (ruleObj is IDictionary<string, object> dictionary)
			{
				object value6;
				if (dictionary.TryGetValue("parameterName", out var value5))
				{
					parameterName = value5?.ToString();
				}
				else if (dictionary.TryGetValue("parameter_name", out value6))
				{
					parameterName = value6?.ToString();
				}
				object value8;
				object value9;
				object value10;
				if (dictionary.TryGetValue("@operator", out var value7))
				{
					text = value7?.ToString();
				}
				else if (dictionary.TryGetValue("operator", out value8))
				{
					text = value8?.ToString();
				}
				else if (dictionary.TryGetValue("op", out value9))
				{
					text = value9?.ToString();
				}
				else if (dictionary.TryGetValue("opName", out value10))
				{
					text = value10?.ToString();
				}
				if (dictionary.TryGetValue("value", out var value11))
				{
					obj = value11;
				}
				if (dictionary.TryGetValue("unit", out var value12))
				{
					text2 = value12?.ToString();
				}
			}
			else
			{
				PropertyInfo propertyInfo = type.GetProperty("parameterName") ?? type.GetProperty("parameter_name");
				PropertyInfo propertyInfo2 = type.GetProperty("operator") ?? type.GetProperty("@operator") ?? type.GetProperty("opName") ?? type.GetProperty("op");
				PropertyInfo property2 = type.GetProperty("value");
				PropertyInfo property3 = type.GetProperty("unit");
				if (propertyInfo == null || propertyInfo2 == null || property2 == null)
				{
					LogError("CreateFilterRule: 规则对象缺少必要的属性");
					return null;
				}
				parameterName = propertyInfo.GetValue(ruleObj)?.ToString();
				text = propertyInfo2.GetValue(ruleObj)?.ToString();
				obj = property2.GetValue(ruleObj);
				if (property3 != null)
				{
					text2 = property3.GetValue(ruleObj)?.ToString();
				}
			}
			if (string.IsNullOrEmpty(parameterName) || string.IsNullOrEmpty(text) || obj == null)
			{
				LogError("CreateFilterRule: 规则属性值为空");
				return null;
			}
			ElementId val;
			if (int.TryParse(parameterName, out var result))
			{
				val = new ElementId((long)result);
			}
			else
			{
				BuiltInParameter? builtInParameter = BuiltInParameterMapper.GetBuiltInParameter(parameterName);
				if (builtInParameter.HasValue)
				{
					val = new ElementId(builtInParameter.Value);
				}
				else
				{
					ParameterElement val2 = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(ParameterElement))).Cast<ParameterElement>().FirstOrDefault((ParameterElement pe) => ((Element)pe).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
					if (val2 == null)
					{
						SharedParameterElement val3 = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(SharedParameterElement))).Cast<SharedParameterElement>().FirstOrDefault((SharedParameterElement pe) => ((Element)pe).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
						if (val3 == null)
						{
							Logger.Warning("CreateFilterRule: 无法找到参数 '" + parameterName + "'，尝试将其作为参数ID处理");
							try
							{
								long num = long.Parse(parameterName);
								val = new ElementId(num);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
								defaultInterpolatedStringHandler.AppendLiteral("CreateFilterRule: 成功将 '");
								defaultInterpolatedStringHandler.AppendFormatted(parameterName);
								defaultInterpolatedStringHandler.AppendLiteral("' 作为参数ID ");
								defaultInterpolatedStringHandler.AppendFormatted(num);
								defaultInterpolatedStringHandler.AppendLiteral(" 使用");
								Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							}
							catch (Exception ex)
							{
								LogError("CreateFilterRule: 无法将 '" + parameterName + "' 作为参数ID处理: " + ex.Message);
								return null;
							}
						}
						else
						{
							val = ((Element)val3).Id;
						}
					}
					else
					{
						val = ((Element)val2).Id;
					}
				}
			}
			if (!string.IsNullOrEmpty(text2) && obj != null && !(obj is string) && !(obj is ElementId))
			{
				obj = ConvertToInternalUnits(obj, text2);
			}
			if (IsParameterElementIdType(doc, val) && smethod_3(obj, out var intResult, out var _))
			{
				Element element = doc.GetElement(new ElementId((long)intResult));
				if (element != null && !string.IsNullOrEmpty(element.Name))
				{
					obj = element.Name;
				}
			}
			string text5 = text.ToLower();
			string text6 = text5;
			uint num2 = Class653.smethod_0(text6);
			if (num2 <= 1362922900)
			{
				if (num2 <= 560682936)
				{
					if (num2 != 397759095)
					{
						if (num2 != 560682936 || !(text6 == "less"))
						{
							goto IL_0bae;
						}
						ElementId val4 = (ElementId)((obj is ElementId) ? obj : null);
						if (val4 != null)
						{
							return ParameterFilterRuleFactory.CreateLessRule(val, val4);
						}
						if (smethod_3(obj, out var intResult2, out var doubleResult2))
						{
							if (Math.Abs(doubleResult2 - (double)intResult2) < 0.0001)
							{
								return ParameterFilterRuleFactory.CreateLessRule(val, intResult2);
							}
							return ParameterFilterRuleFactory.CreateLessRule(val, doubleResult2, 0.0001);
						}
					}
					else
					{
						if (!(text6 == "begins"))
						{
							goto IL_0bae;
						}
						if (obj is string text7)
						{
							if (string.IsNullOrEmpty(text7))
							{
								LogError("CreateFilterRule: 空字符串无法用于 begins 规则 - 应指定具体的起始文本");
								return null;
							}
							return ParameterFilterRuleFactory.CreateBeginsWithRule(val, text7);
						}
						LogError("CreateFilterRule: begins 操作符需要字符串值，当前值类型: " + obj?.GetType().FullName);
					}
				}
				else if (num2 != 1355287449)
				{
					if (num2 != 1362922900 || !(text6 == "equals"))
					{
						goto IL_0bae;
					}
					ElementId val5 = (ElementId)((obj is ElementId) ? obj : null);
					if (val5 != null)
					{
						return ParameterFilterRuleFactory.CreateEqualsRule(val, val5);
					}
					if (smethod_3(obj, out var intResult3, out var doubleResult3))
					{
						if (Math.Abs(doubleResult3 - (double)intResult3) < 0.0001)
						{
							return ParameterFilterRuleFactory.CreateEqualsRule(val, intResult3);
						}
						return ParameterFilterRuleFactory.CreateEqualsRule(val, doubleResult3, 0.0001);
					}
					if (obj is string text8)
					{
						return ParameterFilterRuleFactory.CreateEqualsRule(val, text8);
					}
				}
				else
				{
					if (!(text6 == "greater"))
					{
						goto IL_0bae;
					}
					ElementId val6 = (ElementId)((obj is ElementId) ? obj : null);
					if (val6 != null)
					{
						return ParameterFilterRuleFactory.CreateGreaterRule(val, val6);
					}
					if (smethod_3(obj, out var intResult4, out var doubleResult4))
					{
						if (Math.Abs(doubleResult4 - (double)intResult4) < 0.0001)
						{
							return ParameterFilterRuleFactory.CreateGreaterRule(val, intResult4);
						}
						return ParameterFilterRuleFactory.CreateGreaterRule(val, doubleResult4, 0.0001);
					}
				}
			}
			else if (num2 <= 2017020315)
			{
				if (num2 != 1825239352)
				{
					if (num2 != 2017020315 || !(text6 == "less_or_equal"))
					{
						goto IL_0bae;
					}
					ElementId val7 = (ElementId)((obj is ElementId) ? obj : null);
					if (val7 != null)
					{
						return ParameterFilterRuleFactory.CreateLessOrEqualRule(val, val7);
					}
					if (smethod_3(obj, out var intResult5, out var doubleResult5))
					{
						if (Math.Abs(doubleResult5 - (double)intResult5) < 0.0001)
						{
							return ParameterFilterRuleFactory.CreateLessOrEqualRule(val, intResult5);
						}
						return ParameterFilterRuleFactory.CreateLessOrEqualRule(val, doubleResult5, 0.0001);
					}
				}
				else
				{
					if (!(text6 == "contains"))
					{
						goto IL_0bae;
					}
					if (obj is string text9)
					{
						if (string.IsNullOrEmpty(text9))
						{
							LogError("CreateFilterRule: 空字符串无法用于 contains 规则 - 通常您应该在规则中指定具体的匹配值");
							return null;
						}
						return ParameterFilterRuleFactory.CreateContainsRule(val, text9);
					}
					LogError("CreateFilterRule: contains 操作符需要字符串值，当前值类型: " + obj?.GetType().FullName);
				}
			}
			else if (num2 != 2079777522)
			{
				if (num2 != 2537784475u)
				{
					if (num2 != 2621950312u || !(text6 == "greater_or_equal"))
					{
						goto IL_0bae;
					}
					ElementId val8 = (ElementId)((obj is ElementId) ? obj : null);
					if (val8 != null)
					{
						return ParameterFilterRuleFactory.CreateGreaterOrEqualRule(val, val8);
					}
					if (smethod_3(obj, out var intResult6, out var doubleResult6))
					{
						if (Math.Abs(doubleResult6 - (double)intResult6) < 0.0001)
						{
							return ParameterFilterRuleFactory.CreateGreaterOrEqualRule(val, intResult6);
						}
						return ParameterFilterRuleFactory.CreateGreaterOrEqualRule(val, doubleResult6, 0.0001);
					}
				}
				else
				{
					if (!(text6 == "ends"))
					{
						goto IL_0bae;
					}
					if (obj is string text10)
					{
						if (string.IsNullOrEmpty(text10))
						{
							LogError("CreateFilterRule: 空字符串无法用于 ends 规则 - 应指定具体的结束文本");
							return null;
						}
						return ParameterFilterRuleFactory.CreateEndsWithRule(val, text10);
					}
					LogError("CreateFilterRule: ends 操作符需要字符串值，当前值类型: " + obj?.GetType().FullName);
				}
			}
			else
			{
				if (!(text6 == "not_equals"))
				{
					goto IL_0bae;
				}
				ElementId val9 = (ElementId)((obj is ElementId) ? obj : null);
				if (val9 != null)
				{
					return ParameterFilterRuleFactory.CreateNotEqualsRule(val, val9);
				}
				if (smethod_3(obj, out var intResult7, out var doubleResult7))
				{
					if (Math.Abs(doubleResult7 - (double)intResult7) < 0.0001)
					{
						return ParameterFilterRuleFactory.CreateNotEqualsRule(val, intResult7);
					}
					return ParameterFilterRuleFactory.CreateNotEqualsRule(val, doubleResult7, 0.0001);
				}
				if (obj is string text11)
				{
					return ParameterFilterRuleFactory.CreateNotEqualsRule(val, text11);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("CreateFilterRule: 无法为参数 '");
			defaultInterpolatedStringHandler2.AppendFormatted(parameterName);
			defaultInterpolatedStringHandler2.AppendLiteral("' 和值 '");
			defaultInterpolatedStringHandler2.AppendFormatted<object>(obj);
			defaultInterpolatedStringHandler2.AppendLiteral("' 创建规则");
			LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
			return null;
			IL_0bae:
			LogError("CreateFilterRule: 不支持的操作符 '" + text + "'");
			return null;
		}
		catch (Exception ex2)
		{
			LogError("CreateFilterRule 失败: " + ex2.Message);
			return null;
		}
	}

	private static double ConvertToInternalUnits(object value, string? unit)
	{
		if (string.IsNullOrEmpty(unit))
		{
			return 0.0;
		}
		double result;
		if (value is int num)
		{
			result = num;
		}
		else if (value is long num2)
		{
			result = num2;
		}
		else if (value is double num3)
		{
			result = num3;
		}
		else if (value is float num4)
		{
			result = num4;
		}
		else if (value is decimal num5)
		{
			result = (double)num5;
		}
		else if (!double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return 0.0;
		}
		string text = unit.ToLowerInvariant();
		uint num6 = Class653.smethod_0(text);
		double result2;
		if (num6 <= 1613635087)
		{
			if (num6 <= 655135397)
			{
				if (num6 <= 372194449)
				{
					if (num6 != 355416830)
					{
						if (num6 == 372194449 && text == "m³")
						{
							goto IL_03b1;
						}
					}
					else if (text == "m²")
					{
						goto IL_037b;
					}
				}
				else if (num6 != 571247302)
				{
					if (num6 == 655135397 && text == "\"")
					{
						goto IL_01f1;
					}
				}
				else if (text == "'")
				{
					goto IL_0258;
				}
			}
			else if (num6 <= 1358899048)
			{
				if (num6 != 1094220446)
				{
					if (num6 == 1358899048 && text == "rad")
					{
						result2 = result;
						goto IL_03e5;
					}
				}
				else if (text == "in")
				{
					goto IL_01f1;
				}
			}
			else if (num6 != 1495456279)
			{
				if (num6 == 1613635087 && text == "mm")
				{
					result2 = result / 304.8;
					goto IL_03e5;
				}
			}
			else if (text == "ft")
			{
				goto IL_0258;
			}
		}
		else if (num6 <= 3037454127u)
		{
			if (num6 <= 2502848894u)
			{
				if (num6 != 1680451373)
				{
					if (num6 == 2502848894u && text == "m2")
					{
						goto IL_037b;
					}
				}
				else if (text == "cm")
				{
					result2 = result / 30.48;
					goto IL_03e5;
				}
			}
			else if (num6 != 2519626513u)
			{
				if (num6 == 3037454127u && text == "°")
				{
					goto IL_034f;
				}
			}
			else if (text == "m3")
			{
				goto IL_03b1;
			}
		}
		else if (num6 <= 3327754271u)
		{
			if (num6 != 3175419372u)
			{
				if (num6 == 3327754271u && text == "deg")
				{
					goto IL_034f;
				}
			}
			else if (text == "sqm")
			{
				goto IL_037b;
			}
		}
		else if (num6 != 3893112696u)
		{
			if (num6 == 3981844376u && text == "cum")
			{
				goto IL_03b1;
			}
		}
		else if (text == "m")
		{
			result2 = result / 0.3048;
			goto IL_03e5;
		}
		result2 = result;
		goto IL_03e5;
		IL_0258:
		result2 = result;
		goto IL_03e5;
		IL_034f:
		result2 = result * Math.PI / 180.0;
		goto IL_03e5;
		IL_037b:
		result2 = result * 10.7639;
		goto IL_03e5;
		IL_01f1:
		result2 = result / 12.0;
		goto IL_03e5;
		IL_03b1:
		result2 = result * 35.3147;
		goto IL_03e5;
		IL_03e5:
		return result2;
	}

	private byte? GetJTokenColorValue(PropertyInfo indexer, object jtokenObj, string key)
	{
		try
		{
			object value = indexer.GetValue(jtokenObj, new object[1] { key });
			if (value == null)
			{
				return null;
			}
			Type type = value.GetType();
			if (type.FullName == "Newtonsoft.Json.Linq.JValue")
			{
				PropertyInfo property = type.GetProperty("Value", typeof(object));
				if (property != null)
				{
					object value2 = property.GetValue(value);
					if (value2 != null && value2 is int num)
					{
						return (byte)num;
					}
				}
			}
			string text = value.ToString();
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (text.StartsWith("\"") && text.EndsWith("\""))
			{
				text = text.Substring(1, text.Length - 2);
			}
			if (int.TryParse(text, out var result))
			{
				return (byte)result;
			}
		}
		catch
		{
		}
		return null;
	}

	private bool TryParseColor(object colorObj, out Color revitColor)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		revitColor = new Color((byte)0, (byte)0, (byte)0);
		try
		{
			if (colorObj == null)
			{
				Logger.Warning("[ViewService] TryParseColor: colorObj is null");
				return false;
			}
			Type type = colorObj.GetType();
			string? fullName = type.FullName;
			if (fullName != null && fullName.StartsWith("Newtonsoft.Json.Linq."))
			{
				PropertyInfo property = type.GetProperty("Item", new Type[1] { typeof(string) });
				if (property != null)
				{
					byte? b = GetJTokenColorValue(property, colorObj, "R") ?? GetJTokenColorValue(property, colorObj, "r") ?? GetJTokenColorValue(property, colorObj, "red");
					byte? b2 = GetJTokenColorValue(property, colorObj, "G") ?? GetJTokenColorValue(property, colorObj, "g") ?? GetJTokenColorValue(property, colorObj, "green");
					byte? b3 = GetJTokenColorValue(property, colorObj, "B") ?? GetJTokenColorValue(property, colorObj, "b") ?? GetJTokenColorValue(property, colorObj, "blue");
					if (b.HasValue && b2.HasValue && b3.HasValue)
					{
						revitColor = new Color(b.Value, b2.Value, b3.Value);
						return true;
					}
				}
				return false;
			}
			PropertyInfo propertyInfo = type.GetProperty("R") ?? type.GetProperty("r") ?? type.GetProperty("red");
			PropertyInfo propertyInfo2 = type.GetProperty("G") ?? type.GetProperty("g") ?? type.GetProperty("green");
			PropertyInfo propertyInfo3 = type.GetProperty("B") ?? type.GetProperty("b") ?? type.GetProperty("blue");
			if (propertyInfo != null && propertyInfo2 != null && propertyInfo3 != null)
			{
				byte b4 = Convert.ToByte(propertyInfo.GetValue(colorObj) ?? ((object)0));
				byte b5 = Convert.ToByte(propertyInfo2.GetValue(colorObj) ?? ((object)0));
				byte b6 = Convert.ToByte(propertyInfo3.GetValue(colorObj) ?? ((object)0));
				revitColor = new Color(b4, b5, b6);
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private void LogError(string message)
	{
		Logger.Error("[ViewService] " + message);
	}

	private void LogError(string message, Exception ex)
	{
		Logger.Error("[ViewService] " + message, ex);
	}

	public IEnumerable<object> GetSchedulableFields(object document, string categoryName)
	{
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		Document doc = (Document)((document is Document) ? document : null);
		if (doc == null)
		{
			LogError("GetSchedulableFields: document 不是 Document 类型");
			return Enumerable.Empty<object>();
		}
		Category val = null;
		Categories categories = doc.Settings.Categories;
		foreach (Category item3 in (CategoryNameMap)categories)
		{
			Category val2 = item3;
			if (string.Equals(val2.Name, categoryName, StringComparison.OrdinalIgnoreCase))
			{
				val = val2;
				break;
			}
		}
		if (val == null)
		{
			LogError("GetSchedulableFields: 找不到类别 '" + categoryName + "'");
			return Enumerable.Empty<object>();
		}
		Transaction val3 = new Transaction(doc, "获取可调度字段");
		try
		{
			val3.Start();
			try
			{
				ViewSchedule val4 = ViewSchedule.CreateSchedule(doc, val.Id);
				if (val4 == null)
				{
					LogError("GetSchedulableFields: 无法为类别 '" + categoryName + "' 创建临时明细表");
					val3.RollBack();
					return Enumerable.Empty<object>();
				}
				IList<SchedulableField> schedulableFields = val4.Definition.GetSchedulableFields();
				List<object> list = schedulableFields.Select(delegate(SchedulableField sf)
				{
					//IL_002f: Unknown result type (might be due to invalid IL or missing references)
					(string displayName, string enumName) schedulableFieldInfo = GetSchedulableFieldInfo(doc, sf);
					string item = schedulableFieldInfo.displayName;
					string item2 = schedulableFieldInfo.enumName;
					string gparam_ = sf.ParameterId.Value.ToString();
					string fieldTypeName = GetFieldTypeName(sf.FieldType);
					return new Class331<string, string, string, string>(gparam_, item, item2, fieldTypeName);
				}).Cast<object>().ToList();
				doc.Delete(((Element)val4).Id);
				val3.Commit();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 2);
				defaultInterpolatedStringHandler.AppendLiteral("GetSchedulableFields: 成功获取类别 '");
				defaultInterpolatedStringHandler.AppendFormatted(categoryName);
				defaultInterpolatedStringHandler.AppendLiteral("' 的 ");
				defaultInterpolatedStringHandler.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个可调度字段");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				return list;
			}
			catch (Exception ex)
			{
				val3.RollBack();
				LogError("GetSchedulableFields 失败: " + ex.Message, ex);
				return Enumerable.Empty<object>();
			}
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
	}

	public IEnumerable<object> GetScheduleFields(object scheduleView)
	{
		ViewSchedule val = (ViewSchedule)((scheduleView is ViewSchedule) ? scheduleView : null);
		if (val == null)
		{
			LogError("GetScheduleFields: scheduleView 不是 ViewSchedule 类型");
			return Enumerable.Empty<object>();
		}
		try
		{
			Document doc = ((Element)val).Document;
			IList<SchedulableField> schedulableFields = val.Definition.GetSchedulableFields();
			List<object> list = schedulableFields.Select(delegate(SchedulableField sf)
			{
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				(string displayName, string enumName) schedulableFieldInfo = GetSchedulableFieldInfo(doc, sf);
				string item = schedulableFieldInfo.displayName;
				string item2 = schedulableFieldInfo.enumName;
				string gparam_ = sf.ParameterId.Value.ToString();
				string fieldTypeName = GetFieldTypeName(sf.FieldType);
				return new Class331<string, string, string, string>(gparam_, item, item2, fieldTypeName);
			}).Cast<object>().ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetScheduleFields: 成功获取 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个可调度字段");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetScheduleFields 失败: " + ex.Message, ex);
			return Enumerable.Empty<object>();
		}
	}

	public bool OverrideElementColors(object document, object view, IEnumerable<int> elementIds, object color, int? transparency = null)
	{
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("OverrideElementColors: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("OverrideElementColors: view 不是 View 类型");
				return false;
			}
			if (!ParseColor(color, out Color revitColor))
			{
				LogError("OverrideElementColors: 无法解析颜色对象");
				return false;
			}
			OverrideGraphicSettings val3 = new OverrideGraphicSettings();
			val3.SetProjectionLineColor(revitColor);
			val3.SetSurfaceForegroundPatternColor(revitColor);
			FillPatternElement solidFillPattern = GetSolidFillPattern(val);
			if (solidFillPattern != null)
			{
				val3.SetSurfaceForegroundPatternId(((Element)solidFillPattern).Id);
			}
			val3.SetSurfaceForegroundPatternVisible(true);
			if (transparency.HasValue && transparency.Value >= 0 && transparency.Value <= 100)
			{
				val3.SetSurfaceTransparency(transparency.Value);
			}
			int num = 0;
			foreach (int elementId in elementIds)
			{
				try
				{
					val2.SetElementOverrides(new ElementId((long)elementId), val3);
					num++;
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
					defaultInterpolatedStringHandler.AppendLiteral("OverrideElementColors: 无法设置元素 ");
					defaultInterpolatedStringHandler.AppendFormatted(elementId);
					defaultInterpolatedStringHandler.AppendLiteral(" 的颜色: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(35, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("OverrideElementColors: 成功设置 ");
			defaultInterpolatedStringHandler2.AppendFormatted(num);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个元素的颜色");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return num > 0;
		}
		catch (Exception ex2)
		{
			LogError("OverrideElementColors 失败: " + ex2.Message, ex2);
			return false;
		}
	}

	public bool ClearElementOverrides(object document, object view, IEnumerable<int> elementIds)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ClearElementOverrides: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("ClearElementOverrides: view 不是 View 类型");
				return false;
			}
			OverrideGraphicSettings val3 = new OverrideGraphicSettings();
			int num = 0;
			foreach (int elementId in elementIds)
			{
				try
				{
					val2.SetElementOverrides(new ElementId((long)elementId), val3);
					num++;
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
					defaultInterpolatedStringHandler.AppendLiteral("ClearElementOverrides: 无法清除元素 ");
					defaultInterpolatedStringHandler.AppendFormatted(elementId);
					defaultInterpolatedStringHandler.AppendLiteral(" 的着色: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(35, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("ClearElementOverrides: 成功清除 ");
			defaultInterpolatedStringHandler2.AppendFormatted(num);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个元素的着色");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return num > 0;
		}
		catch (Exception ex2)
		{
			LogError("ClearElementOverrides 失败: " + ex2.Message, ex2);
			return false;
		}
	}

	private bool ParseColor(object colorObj, out Color? revitColor)
	{
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		revitColor = null;
		try
		{
			if (colorObj == null)
			{
				Logger.Warning("ParseColor: colorObj is null");
				return false;
			}
			Type type = colorObj.GetType();
			if (colorObj is IDictionary<string, object> dictionary)
			{
				if (TryGetDictValue(dictionary, out var result, "red", "r", "R") && TryGetDictValue(dictionary, out var result2, "green", "g", "G") && TryGetDictValue(dictionary, out var result3, "blue", "b", "B"))
				{
					revitColor = new Color(result, result2, result3);
					return true;
				}
				Logger.Warning("ParseColor: Dictionary 缺少 RGB 值, keys=[" + string.Join(",", dictionary.Keys) + "]");
				return false;
			}
			PropertyInfo propertyInfo = type.GetProperty("R") ?? type.GetProperty("r") ?? type.GetProperty("red");
			PropertyInfo propertyInfo2 = type.GetProperty("G") ?? type.GetProperty("g") ?? type.GetProperty("green");
			PropertyInfo propertyInfo3 = type.GetProperty("B") ?? type.GetProperty("b") ?? type.GetProperty("blue");
			if (propertyInfo != null && propertyInfo2 != null && propertyInfo3 != null)
			{
				byte b = Convert.ToByte(propertyInfo.GetValue(colorObj) ?? ((object)0));
				byte b2 = Convert.ToByte(propertyInfo2.GetValue(colorObj) ?? ((object)0));
				byte b3 = Convert.ToByte(propertyInfo3.GetValue(colorObj) ?? ((object)0));
				revitColor = new Color(b, b2, b3);
				return true;
			}
			Logger.Warning("ParseColor: 无法找到 RGB 属性, type=" + type.FullName);
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("ParseColor 异常: " + ex.Message);
			return false;
		}
	}

	private static bool TryGetDictValue(IDictionary<string, object> dict, out byte result, params string[] keys)
	{
		foreach (string key in keys)
		{
			if (dict.TryGetValue(key, out object value) && value != null)
			{
				try
				{
					result = Convert.ToByte(value);
					return true;
				}
				catch
				{
				}
			}
		}
		result = 0;
		return false;
	}

	private static FillPatternElement? GetSolidFillPattern(Document doc)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			FillPatternElement val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement))).Cast<FillPatternElement>().FirstOrDefault(delegate(FillPatternElement fp)
			{
				try
				{
					FillPattern fillPattern = fp.GetFillPattern();
					if (fillPattern != null)
					{
						int count = fillPattern.GetFillGrids().Count;
						return count == 0;
					}
				}
				catch (Exception ex2)
				{
					Logger.Warning("GetSolidFillPattern: 检查填充图案失败: " + ex2.Message);
				}
				return false;
			});
			if (val != null)
			{
				Logger.Info("GetSolidFillPattern: 找到实体填充图案: " + ((Element)val).Name);
			}
			else
			{
				Logger.Warning("GetSolidFillPattern: 未能找到实体填充图案");
			}
			return val;
		}
		catch (Exception ex)
		{
			Logger.Error("GetSolidFillPattern: 获取实体填充图案失败: " + ex.Message);
			return null;
		}
	}

	public bool IsTemporaryHideIsolateActive(object view)
	{
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				LogError("IsTemporaryHideIsolateActive: view 不是 View 类型");
				return false;
			}
			bool flag = val.IsTemporaryHideIsolateActive();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(62, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ViewService] IsTemporaryHideIsolateActive: 视图 '");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val).Name);
			defaultInterpolatedStringHandler.AppendLiteral("' 的临时隐藏/隔离状态: ");
			defaultInterpolatedStringHandler.AppendFormatted(flag);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return flag;
		}
		catch (Exception ex)
		{
			LogError("IsTemporaryHideIsolateActive 失败: " + ex.Message);
			return false;
		}
	}

	public bool IsolateCategories(object document, object view, IEnumerable<int> categoryIds)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("IsolateCategories: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("IsolateCategories: view 不是 View 类型");
				return false;
			}
			List<ElementId> list = categoryIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			val2.IsolateCategoriesTemporary((ICollection<ElementId>)list);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ViewService] IsolateCategories: 成功隔离 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个类别");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("IsolateCategories 失败: " + ex.Message);
			return false;
		}
	}

	public bool IsolateElements(object document, object view, IEnumerable<int> elementIds)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("IsolateElements: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("IsolateElements: view 不是 View 类型");
				return false;
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			ICollection<ElementId> collection = list;
			val2.IsolateElementsTemporary(collection);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ViewService] IsolateElements: 成功隔离 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("IsolateElements 失败: " + ex.Message);
			return false;
		}
	}

	public bool HideCategories(object document, object view, IEnumerable<int> categoryIds)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("HideCategories: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("HideCategories: view 不是 View 类型");
				return false;
			}
			List<ElementId> list = categoryIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			val2.HideCategoriesTemporary((ICollection<ElementId>)list);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ViewService] HideCategories: 成功隐藏 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个类别");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("HideCategories 失败: " + ex.Message);
			return false;
		}
	}

	public bool HideElements(object document, object view, IEnumerable<int> elementIds)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("HideElements: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("HideElements: view 不是 View 类型");
				return false;
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			ICollection<ElementId> collection = list;
			val2.HideElementsTemporary(collection);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ViewService] HideElements: 成功隐藏 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("HideElements 失败: " + ex.Message);
			return false;
		}
	}

	public bool ResetTemporaryViewMode(object document, object view)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ResetTemporaryViewMode: document 不是 Document 类型");
				return false;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("ResetTemporaryViewMode: view 不是 View 类型");
				return false;
			}
			val2.DisableTemporaryViewMode((TemporaryViewMode)2);
			Logger.Info("[ViewService] ResetTemporaryViewMode: 成功重置视图 '" + ((Element)val2).Name + "' 的临时隐藏/隔离模式");
			return true;
		}
		catch (Exception ex)
		{
			LogError("ResetTemporaryViewMode 失败: " + ex.Message);
			return false;
		}
	}

	[CompilerGenerated]
	internal static SchedulableField? smethod_0(string uniqueId, ref _003C_003Ec__DisplayClass20_1 _003C_003Ec__DisplayClass20_1_0)
	{
		if (_003C_003Ec__DisplayClass20_1_0.runtimeFieldMap.TryGetValue(uniqueId, out SchedulableField value))
		{
			string name = value.GetName(_003C_003Ec__DisplayClass20_1_0.doc);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ViewService] 通过 UniqueId '");
			defaultInterpolatedStringHandler.AppendFormatted(uniqueId);
			defaultInterpolatedStringHandler.AppendLiteral("' 找到字段: '");
			defaultInterpolatedStringHandler.AppendFormatted(name);
			defaultInterpolatedStringHandler.AppendLiteral("'");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return value;
		}
		Logger.Warning("[ViewService] 无法找到 UniqueId 为 '" + uniqueId + "' 的字段");
		return null;
	}

	[CompilerGenerated]
	internal static ScheduleField? smethod_1(long parameterIdValue, ref _003C_003Ec__DisplayClass20_1 _003C_003Ec__DisplayClass20_1_0)
	{
		int fieldCount = _003C_003Ec__DisplayClass20_1_0.schedule.Definition.GetFieldCount();
		int num = 0;
		ScheduleField field;
		while (true)
		{
			if (num < fieldCount)
			{
				field = _003C_003Ec__DisplayClass20_1_0.schedule.Definition.GetField(num);
				ElementId parameterId = field.ParameterId;
				if (parameterId.Value == parameterIdValue)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return field;
	}

	[CompilerGenerated]
	internal static object? smethod_2(object? jtoken, bool preserveOriginal = false)
	{
		if (jtoken == null)
		{
			return null;
		}
		try
		{
			Type type = jtoken.GetType();
			string text = jtoken.ToString();
			if (preserveOriginal)
			{
				if (type.FullName == "Newtonsoft.Json.Linq.JValue")
				{
					PropertyInfo property = type.GetProperty("Value", typeof(object));
					if (property != null)
					{
						object value = property.GetValue(jtoken);
						if (value != null)
						{
							return value;
						}
					}
				}
				type.GetProperty("Type")?.GetValue(jtoken)?.ToString();
				try
				{
					Type type2 = Type.GetType("Newtonsoft.Json.Linq.JTokenExtensions");
					if (type2 != null)
					{
						MethodInfo method = type2.GetMethod("Value", new Type[2]
						{
							type,
							typeof(int)
						});
						if (method != null)
						{
							object obj = method.Invoke(null, new object[1] { jtoken });
							if (obj != null)
							{
								return obj;
							}
						}
						MethodInfo method2 = type2.GetMethod("Value", new Type[2]
						{
							type,
							typeof(double)
						});
						if (method2 != null)
						{
							object obj2 = method2.Invoke(null, new object[1] { jtoken });
							if (obj2 != null)
							{
								return obj2;
							}
						}
						MethodInfo method3 = type2.GetMethod("Value", new Type[2]
						{
							type,
							typeof(string)
						});
						if (method3 != null)
						{
							object obj3 = method3.Invoke(null, new object[1] { jtoken });
							if (obj3 != null)
							{
								return obj3;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("[ViewService] GetJTokenValue: 调用 Value<T>() 扩展方法失败: " + ex.Message);
				}
			}
			if (text.StartsWith("\"") && text.EndsWith("\"") && text.Length > 1)
			{
				return text.Substring(1, text.Length - 2);
			}
			if (preserveOriginal)
			{
				return jtoken;
			}
			return text;
		}
		catch (Exception ex2)
		{
			Logger.Warning("[ViewService] GetJTokenValue 异常: " + ex2.Message);
			return jtoken.ToString();
		}
	}

	[CompilerGenerated]
	internal static bool smethod_3(object? rawValue, out int intResult, out double doubleResult)
	{
		intResult = 0;
		doubleResult = 0.0;
		if (rawValue == null)
		{
			return false;
		}
		if (rawValue is int num)
		{
			intResult = num;
			doubleResult = num;
			return true;
		}
		if (rawValue is long num2)
		{
			intResult = (int)num2;
			doubleResult = num2;
			return true;
		}
		if (rawValue is double num3)
		{
			intResult = (int)num3;
			doubleResult = num3;
			return true;
		}
		if (rawValue is float num4)
		{
			intResult = (int)num4;
			doubleResult = num4;
			return true;
		}
		if (rawValue is decimal num5)
		{
			intResult = (int)num5;
			doubleResult = (double)num5;
			return true;
		}
		if (rawValue is short num6)
		{
			intResult = num6;
			doubleResult = num6;
			return true;
		}
		if (rawValue is byte b)
		{
			intResult = b;
			doubleResult = (int)b;
			return true;
		}
		string s = rawValue.ToString();
		if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleResult))
		{
			intResult = (int)doubleResult;
			return true;
		}
		return false;
	}
}
