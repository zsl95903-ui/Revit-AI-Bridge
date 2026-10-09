using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public class ButtonWidthToVisibilityConverter : IValueConverter
{
	public double Threshold { get; set; } = 60.0;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is double num)
		{
			return (num < Threshold) ? Visibility.Collapsed : Visibility.Visible;
		}
		return Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
