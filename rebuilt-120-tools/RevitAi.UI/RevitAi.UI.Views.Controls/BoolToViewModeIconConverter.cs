using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Views.Controls;

public class BoolToViewModeIconConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "☰";
			}
			return "▦";
		}
		return "☰";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
