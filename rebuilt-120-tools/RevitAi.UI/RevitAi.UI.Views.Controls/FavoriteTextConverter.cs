using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Views.Controls;

public class FavoriteTextConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "⭐ 收藏";
			}
			return "⭐ 已收藏";
		}
		return "⭐ 收藏";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
