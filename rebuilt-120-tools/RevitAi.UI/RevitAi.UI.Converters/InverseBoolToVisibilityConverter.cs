using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public sealed class InverseBoolToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool flag)
		{
			return (!((parameter is string text && text.Equals("Inverse", StringComparison.OrdinalIgnoreCase)) ? flag : (!flag))) ? Visibility.Collapsed : Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is Visibility visibility && visibility == Visibility.Collapsed;
	}
}
