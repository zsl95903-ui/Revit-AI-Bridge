using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Views.Controls;

public class BoolToViewModeToolTipConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "切换到紧凑视图";
			}
			return "切换到卡片视图";
		}
		return "切换到紧凑视图";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
