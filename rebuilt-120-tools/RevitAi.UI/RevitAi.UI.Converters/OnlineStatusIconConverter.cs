using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public class OnlineStatusIconConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "\ud83d\udd34 离线";
			}
			return "\ud83d\udfe2 在线";
		}
		return "\ud83d\udd34 离线";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
