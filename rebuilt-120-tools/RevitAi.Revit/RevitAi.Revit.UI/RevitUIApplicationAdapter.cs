using System;
using RevitAi.Abstractions.UI;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using ns6;

using ArgumentException = System.ArgumentException;
using InvalidOperationException = System.InvalidOperationException;
using ArgumentNullException = System.ArgumentNullException;
namespace RevitAi.Revit.UI;

public sealed class RevitUIApplicationAdapter : IUIApplication
{
	private readonly UIControlledApplication uicontrolledApplication_0;

	public string VersionNumber => uicontrolledApplication_0.ControlledApplication.VersionNumber;

	public string VersionBuild => uicontrolledApplication_0.ControlledApplication.VersionBuild;

	public RevitUIApplicationAdapter(UIControlledApplication application)
	{
		uicontrolledApplication_0 = application ?? throw new ArgumentNullException("application");
	}

	public void CreateRibbonTab(string tabName)
	{
		try
		{
			uicontrolledApplication_0.CreateRibbonTab(tabName);
		}
		catch (ArgumentException)
		{
		}
	}

	public IRibbonPanel CreateRibbonPanel(string tabName, string panelName)
	{
		try
		{
			RibbonPanel panel = uicontrolledApplication_0.CreateRibbonPanel(tabName, panelName);
			return (IRibbonPanel)(object)new RevitRibbonPanelAdapter(panel);
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("创建 Ribbon 面板失败: " + tabName + "/" + panelName, innerException);
		}
	}

	public void RegisterDockablePane(string paneId, string title, Type uiType)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		try
		{
			Guid guid = Guid.Parse(paneId);
			DockablePaneId val = new DockablePaneId(guid);
			uicontrolledApplication_0.RegisterDockablePane(val, title, (IDockablePaneProvider)Activator.CreateInstance(uiType));
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("注册 DockablePane 失败: " + paneId, innerException);
		}
	}

	public object? GetActiveDocument()
	{
		return null;
	}

	public object? GetUnderlyingObject()
	{
		return uicontrolledApplication_0;
	}
}
