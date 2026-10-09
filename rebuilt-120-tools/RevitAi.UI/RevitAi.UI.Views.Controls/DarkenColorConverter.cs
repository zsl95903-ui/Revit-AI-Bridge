using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitAi.UI.Views.Controls;

public class DarkenColorConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is SolidColorBrush { Color: var color })
		{
			byte r = (byte)Math.Max(0.0, (double)(int)color.R * 0.8);
			byte g = (byte)Math.Max(0.0, (double)(int)color.G * 0.8);
			byte b = (byte)Math.Max(0.0, (double)(int)color.B * 0.8);
			return new SolidColorBrush(Color.FromRgb(r, g, b));
		}
		return value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
