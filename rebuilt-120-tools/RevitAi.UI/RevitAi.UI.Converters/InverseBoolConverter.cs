using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public sealed class InverseBoolConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is bool flag && !flag;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is bool flag && !flag;
	}
}
