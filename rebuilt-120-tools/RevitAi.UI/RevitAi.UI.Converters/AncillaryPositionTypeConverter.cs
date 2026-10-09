using System;
using System.Globalization;
using System.Windows.Data;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.Converters;

public class AncillaryPositionTypeConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is AncillaryPositionType)
		{
			return (AncillaryPositionType)value switch
			{
				AncillaryPositionType.Left => "左侧", 
				AncillaryPositionType.Right => "右侧", 
				AncillaryPositionType.Both => "两侧", 
				_ => "", 
			};
		}
		return "";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
