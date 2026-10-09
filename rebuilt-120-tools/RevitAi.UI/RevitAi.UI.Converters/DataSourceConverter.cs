using System;
using System.Globalization;
using System.Windows.Data;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.Converters;

public sealed class DataSourceConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is RoadCenterlineDataSource)
		{
			return (RoadCenterlineDataSource)value switch
			{
				RoadCenterlineDataSource.None => "无", 
				RoadCenterlineDataSource.StationElevation => "桩号高程表", 
				RoadCenterlineDataSource.HorizontalVerticalCurve => "平纵曲线表", 
				_ => "未知", 
			};
		}
		return "无";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
