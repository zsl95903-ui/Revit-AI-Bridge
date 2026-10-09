using System;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.UI;

public static class AIDockablePaneRegistrar
{
	public static void Register(UIControlledApplication application)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		try
		{
			DockablePaneId val = new DockablePaneId(AIDockablePaneProvider.PaneId);
			AIDockablePaneProvider aIDockablePaneProvider = new AIDockablePaneProvider();
			application.RegisterDockablePane(val, "ASTools AI 对话助手", (IDockablePaneProvider)(object)aIDockablePaneProvider);
		}
		catch (Exception ex)
		{
			Logger.Error("[AIDockablePaneRegistrar] ❌ AI 对话 DockablePane 注册失败: " + ex.Message, ex);
		}
	}
}
