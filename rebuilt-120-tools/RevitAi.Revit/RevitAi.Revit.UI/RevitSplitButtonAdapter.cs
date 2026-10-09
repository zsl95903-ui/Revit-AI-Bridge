using System;
using RevitAi.Abstractions.UI;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.UI;

public sealed class RevitSplitButtonAdapter : ISplitButton
{
	private readonly SplitButton splitButton_0;

	public RevitSplitButtonAdapter(SplitButton splitButton)
	{
		splitButton_0 = splitButton ?? throw new ArgumentNullException("splitButton");
	}

	public IPushButton AddPushButton(string commandName, string displayName)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		try
		{
			PushButtonData val = new PushButtonData(commandName, displayName, typeof(RevitRibbonPanelAdapter).Assembly.Location, commandName);
			PushButton button = ((PulldownButton)splitButton_0).AddPushButton(val);
			return (IPushButton)(object)new RevitPushButtonAdapter(button);
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("添加下拉按钮失败: " + commandName, innerException);
		}
	}
}
