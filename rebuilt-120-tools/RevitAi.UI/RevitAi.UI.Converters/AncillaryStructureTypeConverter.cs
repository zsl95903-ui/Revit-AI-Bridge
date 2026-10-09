using System;
using System.Globalization;
using System.Windows.Data;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.Converters;

public class AncillaryStructureTypeConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is AncillaryStructureType)
		{
			return (AncillaryStructureType)value switch
			{
				AncillaryStructureType.Trapezoid => "梯形", 
				AncillaryStructureType.Rectangle => "矩形", 
				AncillaryStructureType.RectangleHollow => "矩形空心", 
				AncillaryStructureType.SingleSlopeSurface => "单坡面层", 
				AncillaryStructureType.DoubleSlopeSurface => "双坡面层", 
				AncillaryStructureType.RoundedCurb => "圆角路沿石", 
				_ => "其他", 
			};
		}
		return "其他";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
