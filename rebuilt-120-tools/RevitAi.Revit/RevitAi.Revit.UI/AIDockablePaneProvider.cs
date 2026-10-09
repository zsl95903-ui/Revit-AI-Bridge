using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.UI;

public class AIDockablePaneProvider : IDockablePaneProvider
{
	public static readonly Guid PaneId = new Guid("{E7F3A8D2-5C9B-4E2F-A1D6-8B4C9E7F3A5D}");

	public void SetupDockablePane(DockablePaneProviderData data)
	{
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Expected O, but got Unknown
		try
		{
			Assembly assembly = Assembly.Load("RevitAi.UI");
			if (assembly == null)
			{
				Logger.Error("[AIDockablePaneProvider] ❌ 无法加载 RevitAi.UI 程序集");
				return;
			}
			Type type = assembly.GetType("RevitAi.UI.Views.Controls.AIChatControl");
			if (type == null)
			{
				Logger.Error("[AIDockablePaneProvider] ❌ 无法找到 AIChatControl 类型");
				return;
			}
			object obj = Activator.CreateInstance(type);
			if (obj == null)
			{
				Logger.Error("[AIDockablePaneProvider] ❌ 无法创建 AIChatControl 实例");
				return;
			}
			Type type2 = assembly.GetType("RevitAi.UI.ViewModels.AIChatPanelViewModel");
			if (type2 != null)
			{
				try
				{
					MethodInfo methodInfo = assembly.GetType("RevitAi.UI.Services.UIBootstrapper")?.GetMethod("GetService");
					if (methodInfo != null)
					{
						Type type3 = assembly.GetType("RevitAi.UI.Services.IWindowManager");
						if (type3 != null)
						{
							MethodInfo methodInfo2 = methodInfo.MakeGenericMethod(type3);
							object obj2 = methodInfo2.Invoke(null, null);
							Type type4 = assembly.GetType("RevitAi.UI.Services.IDialogService");
							if (type4 != null)
							{
								MethodInfo methodInfo3 = methodInfo.MakeGenericMethod(type4);
								object obj3 = methodInfo3.Invoke(null, null);
								if (obj2 != null && obj3 != null)
								{
									object obj4 = Activator.CreateInstance(type2, obj2, obj3, null);
									PropertyInfo property = type2.GetProperty("Messages");
									if (property != null)
									{
										_ = property.GetValue(obj4) is IList;
									}
									type.GetProperty("DataContext")?.SetValue(obj, obj4);
									try
									{
										Type type5 = typeof(AIDockablePaneHelper).Assembly.GetType("RevitAi.Revit.UI.RevitAdapterManager");
										if (type5 != null)
										{
											PropertyInfo property2 = type5.GetProperty("CurrentAdapter", BindingFlags.Static | BindingFlags.Public);
											if (property2 != null)
											{
												object value = property2.GetValue(null);
												if (value != null)
												{
													MethodInfo method = type2.GetMethod("SetRevitAdapter");
													if (method != null)
													{
														object obj5 = null;
														MethodInfo method2 = value.GetType().GetMethod("GetAIToolExternalEvent");
														if (method2 != null)
														{
															obj5 = method2.Invoke(value, null);
														}
														method.Invoke(obj4, new object[2]
														{
															value,
															obj5 ?? null
														});
													}
												}
											}
										}
									}
									catch (Exception ex)
									{
										Logger.Warning("[AIDockablePaneProvider] 设置 RevitAdapter 失败: " + ex.Message);
									}
								}
								else
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(66, 2);
									defaultInterpolatedStringHandler.AppendLiteral("[AIDockablePaneProvider] ❌ 服务为空 - WindowManager: ");
									defaultInterpolatedStringHandler.AppendFormatted(obj2 != null);
									defaultInterpolatedStringHandler.AppendLiteral(", DialogService: ");
									defaultInterpolatedStringHandler.AppendFormatted(obj3 != null);
									Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
								}
							}
							else
							{
								Logger.Error("[AIDockablePaneProvider] ❌ 无法找到 IDialogService 类型");
							}
						}
						else
						{
							Logger.Error("[AIDockablePaneProvider] ❌ 无法找到 IWindowManager 类型");
						}
					}
					else
					{
						Logger.Error("[AIDockablePaneProvider] ❌ 无法找到 GetService 方法");
					}
				}
				catch (Exception ex2)
				{
					Logger.Error("[AIDockablePaneProvider] ❌ 创建 ViewModel 失败: " + ex2.GetType().Name + " - " + ex2.Message, ex2);
				}
			}
			else
			{
				Logger.Error("[AIDockablePaneProvider] ❌ 无法找到 AIChatPanelViewModel 类型");
			}
			PropertyInfo property3 = ((object)data).GetType().GetProperty("FrameworkElement");
			if (property3 != null && property3.CanWrite)
			{
				property3.SetValue(data, obj);
			}
			data.InitialState = new DockablePaneState
			{
				DockPosition = (DockPosition)59421,
				TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser
			};
		}
		catch (Exception ex3)
		{
			Logger.Error("[AIDockablePaneProvider] ❌ SetupDockablePane 异常: " + ex3.GetType().Name + " - " + ex3.Message, ex3);
		}
	}
}
