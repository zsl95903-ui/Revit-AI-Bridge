using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Converters;

public class WechatHintTextConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "请使用微信扫一扫";
			}
			return "扫码添加开发者微信";
		}
		return "请使用微信扫一扫";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
