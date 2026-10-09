using System;
using System.IO;
using System.Windows.Media.Imaging;
using RevitAi.Abstractions.UI;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.UI;

public sealed class RevitPushButtonAdapter : IPushButton
{
	private readonly PushButton pushButton_0;

	public string Text
	{
		get
		{
			return ((RibbonItem)pushButton_0).Name;
		}
		set
		{
		}
	}

	public string? ToolTip
	{
		get
		{
			return ((RibbonItem)pushButton_0).ToolTip;
		}
		set
		{
			((RibbonItem)pushButton_0).ToolTip = value;
		}
	}

	public string? LongDescription
	{
		get
		{
			return ((RibbonItem)pushButton_0).LongDescription;
		}
		set
		{
			((RibbonItem)pushButton_0).LongDescription = value;
		}
	}

	public string? IconPath
	{
		get
		{
			return ((object)((RibbonButton)pushButton_0).LargeImage)?.ToString();
		}
		set
		{
			if (!string.IsNullOrEmpty(value) && File.Exists(value))
			{
				try
				{
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.UriSource = new Uri(value);
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapImage.EndInit();
					((RibbonButton)pushButton_0).LargeImage = bitmapImage;
				}
				catch (Exception)
				{
				}
			}
		}
	}

	public bool Enabled
	{
		get
		{
			return ((RibbonItem)pushButton_0).Enabled;
		}
		set
		{
			((RibbonItem)pushButton_0).Enabled = value;
		}
	}

	public string? AvailabilityClassName
	{
		get
		{
			return pushButton_0.AvailabilityClassName;
		}
		set
		{
			pushButton_0.AvailabilityClassName = value;
		}
	}

	public bool Visible
	{
		get
		{
			return ((RibbonItem)pushButton_0).Visible;
		}
		set
		{
			((RibbonItem)pushButton_0).Visible = value;
		}
	}

	public RevitPushButtonAdapter(PushButton button)
	{
		pushButton_0 = button ?? throw new ArgumentNullException("button");
	}
}
