using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitAi.UI.Views.Windows;

public sealed class StringNotEqualsVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = value as string;
		string value2 = parameter as string;
		if (string.IsNullOrEmpty(text) || (text != null && text.Equals(value2, StringComparison.Ordinal)))
		{
			return Visibility.Collapsed;
		}
		return Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
