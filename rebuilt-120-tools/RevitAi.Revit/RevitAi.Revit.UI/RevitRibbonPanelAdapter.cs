using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using RevitAi.Abstractions.UI;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.UI;

public sealed class RevitRibbonPanelAdapter : IRibbonPanel
{
	private readonly RibbonPanel ribbonPanel_0;

	public string Name => ribbonPanel_0.Name;

	public bool Visible
	{
		get
		{
			return ribbonPanel_0.Visible;
		}
		set
		{
			ribbonPanel_0.Visible = value;
		}
	}

	public RevitRibbonPanelAdapter(RibbonPanel panel)
	{
		ribbonPanel_0 = panel ?? throw new ArgumentNullException("panel");
	}

	public IPushButton AddPushButton(string commandName, string displayName, string? iconPath = null, string? tooltip = null)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		try
		{
			string text = "RevitAi.Revit.Commands." + commandName + "Wrapper";
			Assembly assembly = typeof(RevitRibbonPanelAdapter).Assembly;
			string location = assembly.Location;
			PushButtonData val = new PushButtonData(commandName, displayName, location, text);
			if (!string.IsNullOrEmpty(tooltip))
			{
				((ItemData)val).ToolTip = tooltip;
			}
			RibbonItem obj = ribbonPanel_0.AddItem((RibbonItemData)(object)val);
			PushButton val2 = (PushButton)(object)((obj is PushButton) ? obj : null);
			if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
			{
				try
				{
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.UriSource = new Uri(iconPath);
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapImage.EndInit();
					((RibbonButton)val2).LargeImage = bitmapImage;
				}
				catch (Exception)
				{
				}
			}
			return (IPushButton)(object)new RevitPushButtonAdapter(val2);
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("添加按钮失败: " + commandName, innerException);
		}
	}

	public ISplitButton AddSplitButton(string commandName, string displayName, string? iconPath = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			new PushButtonData(commandName, displayName, typeof(RevitRibbonPanelAdapter).Assembly.Location, commandName);
			throw new NotImplementedException("SplitButton 功能需要进一步实现");
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("添加分隔按钮失败: " + commandName, innerException);
		}
	}
}
