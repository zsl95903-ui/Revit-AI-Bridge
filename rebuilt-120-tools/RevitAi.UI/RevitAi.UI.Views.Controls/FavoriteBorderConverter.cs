using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitAi.UI.Views.Controls;

public class FavoriteBorderConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			return new SolidColorBrush(((bool)value) ? Color.FromRgb(232, 197, 72) : Color.FromRgb(224, 224, 224));
		}
		return new SolidColorBrush(Color.FromRgb(224, 224, 224));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
