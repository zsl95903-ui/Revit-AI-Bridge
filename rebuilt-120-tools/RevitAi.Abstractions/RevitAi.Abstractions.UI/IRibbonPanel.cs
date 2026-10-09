namespace RevitAi.Abstractions.UI;

public interface IRibbonPanel
{
	string Name { get; }

	bool Visible { get; set; }

	IPushButton AddPushButton(string commandName, string displayName, string? iconPath = null, string? tooltip = null);

	ISplitButton AddSplitButton(string commandName, string displayName, string? iconPath = null);
}
