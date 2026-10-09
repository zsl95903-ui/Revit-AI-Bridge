using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitAi.UI.Views.Controls;

public class FavoriteBackgroundConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			return new SolidColorBrush(((bool)value) ? Color.FromRgb(249, 215, 125) : Color.FromRgb(240, 240, 240));
		}
		return new SolidColorBrush(Color.FromRgb(240, 240, 240));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
