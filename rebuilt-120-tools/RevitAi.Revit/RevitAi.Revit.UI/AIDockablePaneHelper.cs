using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.UI;

public static class AIDockablePaneHelper
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct0
	{
		public UIApplication uiapplication_0;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct1
	{
		public Assembly assembly_0;

		public Type type_0;

		public object object_0;
	}

	private static bool bool_0;

	public static bool WasPaneEverOpened
	{
		get
		{
			return bool_0;
		}
		set
		{
			bool_0 = value;
		}
	}

	public static bool ShowPane()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		try
		{
			UIApplication val = smethod_0();
			if (val == null)
			{
				Logger.Warning("[AIDockablePaneHelper] ❌ GetUIApplication() 返回 null");
				return false;
			}
			DockablePaneId val2 = new DockablePaneId(AIDockablePaneProvider.PaneId);
			DockablePane val3 = null;
			try
			{
				val3 = val.GetDockablePane(val2);
			}
			catch (ArgumentException ex) when (ex.Message.Contains("has not been created yet"))
			{
				Logger.Warning("[AIDockablePaneHelper] DockablePane 尚未创建，需要文档打开后方可使用");
				return false;
			}
			if (val3 == null)
			{
				Logger.Error("[AIDockablePaneHelper] ❌ GetDockablePane() 返回 null - DockablePane 可能未注册");
				return false;
			}
			smethod_1(val);
			smethod_2(val);
			smethod_4(val3);
			if (!val3.IsShown())
			{
				val3.Show();
				bool_0 = true;
				return true;
			}
			bool_0 = true;
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool ShowPane(UIApplication uiApp)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		try
		{
			if (uiApp == null)
			{
				return false;
			}
			DockablePaneId val = new DockablePaneId(AIDockablePaneProvider.PaneId);
			DockablePane val2 = null;
			try
			{
				val2 = uiApp.GetDockablePane(val);
			}
			catch (ArgumentException ex) when (ex.Message.Contains("has not been created yet"))
			{
				return false;
			}
			if (val2 == null)
			{
				return false;
			}
			smethod_1(uiApp);
			smethod_2(uiApp);
			smethod_4(val2);
			if (!val2.IsShown())
			{
				val2.Show();
				bool_0 = true;
				return true;
			}
			bool_0 = true;
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static void HidePane()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		try
		{
			UIApplication val = smethod_0();
			if (val != null)
			{
				DockablePaneId val2 = new DockablePaneId(AIDockablePaneProvider.PaneId);
				DockablePane dockablePane = val.GetDockablePane(val2);
				if (dockablePane != null && dockablePane.IsShown())
				{
					dockablePane.Hide();
					bool_0 = false;
				}
			}
		}
		catch
		{
		}
	}

	public static void RestorePaneIfNeeded(UIApplication uiApp)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		try
		{
			if (uiApp != null && bool_0)
			{
				DockablePaneId val = new DockablePaneId(AIDockablePaneProvider.PaneId);
				DockablePane val2 = null;
				try
				{
					val2 = uiApp.GetDockablePane(val);
				}
				catch (ArgumentException ex) when (ex.Message.Contains("has not been created yet"))
				{
					return;
				}
				if (val2 != null && !val2.IsShown())
				{
					val2.Show();
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("[AIDockablePaneHelper] 恢复 Pane 失败: " + ex2.Message);
		}
	}

	private static UIApplication? smethod_0()
	{
		try
		{
			Type type = typeof(AIDockablePaneHelper).Assembly.GetType("RevitAi.Revit.RevitAdapter");
			if (type == null)
			{
				Logger.Warning("[AIDockablePaneHelper] ❌ 未找到 RevitAdapter 类型");
				return null;
			}
			PropertyInfo property = type.GetProperty("CurrentUIApplication", BindingFlags.Static | BindingFlags.Public);
			if (property == null)
			{
				Logger.Warning("[AIDockablePaneHelper] ❌ 未找到 CurrentUIApplication 属性");
				return null;
			}
			object? value = property.GetValue(null);
			UIApplication val = (UIApplication)((value is UIApplication) ? value : null);
			if (val != null)
			{
				return val;
			}
			try
			{
				Type type2 = typeof(AIDockablePaneHelper).Assembly.GetType("RevitAi.Revit.UI.RevitAdapterManager");
				if (type2 == null)
				{
					Logger.Warning("[AIDockablePaneHelper] ❌ 未找到 RevitAdapterManager 类型");
					return null;
				}
				PropertyInfo property2 = type2.GetProperty("CurrentAdapter", BindingFlags.Static | BindingFlags.Public);
				if (property2 == null)
				{
					Logger.Warning("[AIDockablePaneHelper] ❌ 未找到 CurrentAdapter 属性");
					return null;
				}
				object value2 = property2.GetValue(null);
				if (value2 == null)
				{
					Logger.Warning("[AIDockablePaneHelper] ❌ CurrentAdapter 为 null");
					return null;
				}
				FieldInfo field = type.GetField("_application", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field != null)
				{
					object? value3 = field.GetValue(value2);
					UIApplication val2 = (UIApplication)((value3 is UIApplication) ? value3 : null);
					if (val2 != null)
					{
						property.SetValue(null, val2);
						return val2;
					}
				}
				FieldInfo field2 = type.GetField("_controlledApplication", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field2 != null)
				{
					object value4 = field2.GetValue(value2);
					if (value4 != null)
					{
						object obj = smethod_7(value4);
						if (obj != null)
						{
							property.SetValue(null, obj);
							if (field != null)
							{
								field.SetValue(value2, obj);
							}
							return (UIApplication?)((obj is UIApplication) ? obj : null);
						}
					}
				}
				Logger.Warning("[AIDockablePaneHelper] ❌ 所有备用方案均失败");
			}
			catch (Exception ex)
			{
				Logger.Warning("[AIDockablePaneHelper] 从适配器实例获取 UIApplication 失败: " + ex.Message);
			}
			return null;
		}
		catch (Exception ex2)
		{
			Logger.Error("[AIDockablePaneHelper] ❌ GetUIApplication() 异常: " + ex2.Message, ex2);
			return null;
		}
	}

	private static void smethod_1(UIApplication uiapplication_0)
	{
		try
		{
			Type type = typeof(AIDockablePaneHelper).Assembly.GetType("RevitAi.Revit.RevitAdapter");
			if (type == null)
			{
				Logger.Warning("[AIDockablePaneHelper] ❌ SetRevitAdapterCurrentUIApplication: 未找到 RevitAdapter 类型");
				return;
			}
			PropertyInfo property = type.GetProperty("CurrentUIApplication", BindingFlags.Static | BindingFlags.Public);
			if (property == null)
			{
				Logger.Warning("[AIDockablePaneHelper] ❌ SetRevitAdapterCurrentUIApplication: 未找到 CurrentUIApplication 属性");
			}
			else
			{
				property.SetValue(null, uiapplication_0);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIDockablePaneHelper] ❌ SetRevitAdapterCurrentUIApplication 异常: " + ex.Message, ex);
		}
	}

	private static void smethod_2(UIApplication uiapplication_0)
	{
		Struct0 struct0_ = default(Struct0);
		struct0_.uiapplication_0 = uiapplication_0;
		try
		{
			Assembly assembly = null;
			try
			{
				assembly = Assembly.Load("RevitAi.Main");
			}
			catch
			{
				return;
			}
			if (assembly == null)
			{
				return;
			}
			Type type = assembly.GetType("RevitAi.MainApplication");
			if (type == null)
			{
				return;
			}
			PropertyInfo property = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
			if (property == null)
			{
				return;
			}
			object value = property.GetValue(null);
			if (value == null)
			{
				return;
			}
			FieldInfo field = type.GetField("_adapter", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field == null)
			{
				return;
			}
			Struct1 struct1_ = default(Struct1);
			struct1_.object_0 = field.GetValue(value);
			if (struct1_.object_0 == null)
			{
				return;
			}
			struct1_.type_0 = struct1_.object_0.GetType();
			FieldInfo field2 = struct1_.type_0.GetField("_elementService", BindingFlags.Instance | BindingFlags.NonPublic);
			if (!(field2 == null))
			{
				object value2 = field2.GetValue(struct1_.object_0);
				if (value2 == null)
				{
					struct1_.assembly_0 = typeof(AIDockablePaneHelper).Assembly;
					smethod_8("RevitAi.Revit.Services.DocumentService", "_documentService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.ElementService", "_elementService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.ParameterService", "_parameterService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.SelectionService", "_selectionService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.GeometryService", "_geometryService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.ViewService", "_viewService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.LevelService", "_levelService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.ModificationService", "_modificationService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.CreationService", "_creationService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.AnnotationService", "_annotationService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.LinkService", "_linkService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.MaterialService", "_materialService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.AnalysisService", "_analysisService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.FamilyService", "_familyService", ref struct0_, ref struct1_);
					smethod_8("RevitAi.Revit.Services.PhaseService", "_phaseService", ref struct0_, ref struct1_);
				}
			}
		}
		catch
		{
		}
	}

	private static object? smethod_3()
	{
		try
		{
			UIApplication val = smethod_0();
			if (val == null)
			{
				return null;
			}
			Type type = typeof(AIDockablePaneHelper).Assembly.GetType("RevitAdapter");
			if (type == null)
			{
				return null;
			}
			ConstructorInfo constructor = type.GetConstructor(new Type[1] { typeof(UIApplication) });
			if (constructor != null)
			{
				return constructor.Invoke(new object[1] { val });
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	private static void smethod_4(DockablePane dockablePane_0)
	{
		try
		{
			PropertyInfo property = ((object)dockablePane_0).GetType().GetProperty("FrameworkElement");
			if (property == null)
			{
				return;
			}
			object value = property.GetValue(dockablePane_0);
			if (value == null)
			{
				return;
			}
			PropertyInfo property2 = value.GetType().GetProperty("DataContext");
			if (!(property2 == null))
			{
				object value2 = property2.GetValue(value);
				if (value2 != null)
				{
					smethod_6(value2);
				}
				else
				{
					smethod_5(value);
				}
			}
		}
		catch
		{
		}
	}

	private static void smethod_5(object object_0)
	{
		try
		{
			Assembly assembly = Assembly.Load("RevitAi.UI");
			if (assembly == null)
			{
				return;
			}
			Type type = assembly.GetType("RevitAi.UI.ViewModels.AIChatPanelViewModel");
			if (type == null)
			{
				return;
			}
			MethodInfo methodInfo = assembly.GetType("RevitAi.UI.Services.UIBootstrapper")?.GetMethod("GetService");
			if (methodInfo == null)
			{
				return;
			}
			Type type2 = assembly.GetType("RevitAi.UI.Services.IWindowManager");
			MethodInfo methodInfo2 = methodInfo.MakeGenericMethod(type2);
			object obj = methodInfo2.Invoke(null, null);
			Type type3 = assembly.GetType("RevitAi.UI.Services.IDialogService");
			MethodInfo methodInfo3 = methodInfo.MakeGenericMethod(type3);
			object obj2 = methodInfo3.Invoke(null, null);
			if (obj == null || obj2 == null)
			{
				return;
			}
			object obj3 = Activator.CreateInstance(type, obj, obj2, null);
			object_0.GetType().GetProperty("DataContext")?.SetValue(object_0, obj3);
			object currentAdapter = RevitAdapterManager.CurrentAdapter;
			if (currentAdapter == null)
			{
				return;
			}
			MethodInfo method = type.GetMethod("SetRevitAdapter");
			if (method != null)
			{
				object obj4 = null;
				MethodInfo method2 = currentAdapter.GetType().GetMethod("GetAIToolExternalEvent");
				if (method2 != null)
				{
					obj4 = method2.Invoke(currentAdapter, null);
				}
				method.Invoke(obj3, new object[2]
				{
					currentAdapter,
					obj4 ?? null
				});
			}
		}
		catch
		{
		}
	}

	private static void smethod_6(object object_0)
	{
		try
		{
			if (object_0 == null)
			{
				return;
			}
			Type type = object_0.GetType();
			FieldInfo field = type.GetField("_revitAdapter", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field == null)
			{
				return;
			}
			object value = field.GetValue(object_0);
			if (value != null)
			{
				return;
			}
			object currentAdapter = RevitAdapterManager.CurrentAdapter;
			if (currentAdapter == null)
			{
				Logger.Warning("[AIDockablePaneHelper] EnsureRevitAdapterIsSet: CurrentAdapter 为 null");
				return;
			}
			MethodInfo method = type.GetMethod("SetRevitAdapter");
			if (method != null)
			{
				object obj = null;
				MethodInfo method2 = currentAdapter.GetType().GetMethod("GetAIToolExternalEvent");
				if (method2 != null)
				{
					obj = method2.Invoke(currentAdapter, null);
				}
				method.Invoke(object_0, new object[2]
				{
					currentAdapter,
					obj ?? null
				});
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AIDockablePaneHelper] EnsureRevitAdapterIsSet 失败: " + ex.Message, ex);
		}
	}

	private static object? smethod_7(object object_0)
	{
		try
		{
			if (object_0 == null)
			{
				return null;
			}
			Type type = object_0.GetType();
			string[] array = new string[8]
			{
				"m_uiapplication",
				"m_application",
				"UIApplication",
				"_application",
				"m_uiApplication",
				"m_app",
				"_app",
				"application"
			};
			string[] array2 = array;
			foreach (string text in array2)
			{
				try
				{
					FieldInfo field = type.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (field != null)
					{
						object value = field.GetValue(object_0);
						if (value != null)
						{
							return value;
						}
						Logger.Debug("[AIDockablePaneHelper] 字段 '" + text + "' 存在但值为 null（可能没有打开文档）");
					}
				}
				catch
				{
				}
			}
			try
			{
				FieldInfo field2 = type.GetField("m_proxy", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field2 != null)
				{
					object value2 = field2.GetValue(object_0);
					if (value2 != null)
					{
						Type type2 = value2.GetType();
						FieldInfo field3 = type2.GetField("m_uiapplication", BindingFlags.Instance | BindingFlags.NonPublic);
						if (field3 != null)
						{
							object value3 = field3.GetValue(value2);
							if (value3 != null)
							{
								return value3;
							}
						}
					}
				}
			}
			catch
			{
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Warning("[AIDockablePaneHelper] 提取 UIApplication 失败: " + ex.Message);
			return null;
		}
	}

	[CompilerGenerated]
	internal static void smethod_8(string string_0, string string_1, ref Struct0 struct0_0, ref Struct1 struct1_0)
	{
		try
		{
			Type type = struct1_0.assembly_0.GetType(string_0);
			if (type != null)
			{
				object obj = Activator.CreateInstance(type, struct0_0.uiapplication_0);
				if (obj != null)
				{
					struct1_0.type_0.GetField(string_1, BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(struct1_0.object_0, obj);
				}
				else
				{
					Logger.Warning("[AIDockablePaneHelper] ❌ 服务 " + string_0 + " 创建失败（Activator.CreateInstance 返回 null）");
				}
			}
			else
			{
				Logger.Warning("[AIDockablePaneHelper] ❌ 未找到服务类型: " + string_0);
			}
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[AIDockablePaneHelper] ❌ 初始化服务 ");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
			defaultInterpolatedStringHandler.AppendFormatted(ex.GetType().Name);
			defaultInterpolatedStringHandler.AppendLiteral(" - ");
			defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
