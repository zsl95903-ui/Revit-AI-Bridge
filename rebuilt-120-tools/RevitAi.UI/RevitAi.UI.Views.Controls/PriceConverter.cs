using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Views.Controls;

public class PriceConverter : IValueConverter
{
	public static readonly PriceConverter Instance = new PriceConverter();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is decimal num)
		{
			if (num == 0m)
			{
				return "免费";
			}
			return $"{num:G0} 电量";
		}
		if (value is int num2)
		{
			if (num2 != 0)
			{
				return $"{num2} 电量";
			}
			return "免费";
		}
		return "免费";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
