using System;

namespace RevitAi.Abstractions.UI;

public interface IUIApplication
{
	string VersionNumber { get; }

	string VersionBuild { get; }

	void CreateRibbonTab(string tabName);

	IRibbonPanel CreateRibbonPanel(string tabName, string panelName);

	void RegisterDockablePane(string paneId, string title, Type uiType);

	object? GetActiveDocument();

	object? GetUnderlyingObject();
}
