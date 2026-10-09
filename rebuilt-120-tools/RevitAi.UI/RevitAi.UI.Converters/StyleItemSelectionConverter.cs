using System;
using System.Globalization;
using System.Windows.Data;
using RevitAi.UI.Models;

namespace RevitAi.UI.Converters;

public class StyleItemSelectionConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is BridgeComponentStyleItem bridgeComponentStyleItem && parameter is BridgeComponentStyleItem bridgeComponentStyleItem2)
		{
			return bridgeComponentStyleItem.Id == bridgeComponentStyleItem2.Id;
		}
		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
