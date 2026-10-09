using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public sealed class EnumToIndexConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (parameter is string text && text == "Visibility")
		{
			if (value is Enum)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}
		if (value != null && value.GetType().IsEnum)
		{
			return (int)value;
		}
		return 0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is int value2 && targetType.IsEnum)
		{
			return Enum.ToObject(targetType, value2);
		}
		return 0;
	}
}
