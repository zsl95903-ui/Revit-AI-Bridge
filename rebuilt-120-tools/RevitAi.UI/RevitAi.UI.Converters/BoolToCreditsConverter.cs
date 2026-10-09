using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public class BoolToCreditsConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "使用此模型会消耗电量";
			}
			return "使用此模型不消耗电量";
		}
		return "使用此模型会消耗电量";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
