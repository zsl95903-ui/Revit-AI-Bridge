using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public sealed class BoolToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (!(value is bool flag))
		{
			return Visibility.Collapsed;
		}
		return (!((parameter is string text && (text.Equals("Inverse", StringComparison.OrdinalIgnoreCase) || text.Equals("Not", StringComparison.OrdinalIgnoreCase))) ? (!flag) : flag)) ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is Visibility visibility && visibility == Visibility.Visible;
	}
}
