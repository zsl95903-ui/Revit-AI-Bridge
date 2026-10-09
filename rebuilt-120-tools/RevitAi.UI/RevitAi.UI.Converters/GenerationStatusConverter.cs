using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public sealed class GenerationStatusConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "未生成";
			}
			return "已生成";
		}
		return "未生成";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
