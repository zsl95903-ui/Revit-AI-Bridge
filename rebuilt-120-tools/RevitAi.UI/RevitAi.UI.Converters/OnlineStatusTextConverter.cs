using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public class OnlineStatusTextConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "离线";
			}
			return "在线";
		}
		return "离线";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
