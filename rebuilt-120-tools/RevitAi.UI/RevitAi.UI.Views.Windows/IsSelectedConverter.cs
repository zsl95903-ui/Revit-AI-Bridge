using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitAi.UI.Views.Windows;

public class IsSelectedConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values.Length >= 2 && values[0] is Guid guid && values[1] is Guid guid2)
		{
			return guid == guid2;
		}
		return false;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
