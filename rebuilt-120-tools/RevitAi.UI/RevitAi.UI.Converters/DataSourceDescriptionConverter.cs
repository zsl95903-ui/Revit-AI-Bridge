using System;
using System.Globalization;
using System.Windows.Data;
using RevitAi.Abstractions.Infrastructure;

namespace RevitAi.UI.Converters;

public sealed class DataSourceDescriptionConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is RoadCenterlineDataSource)
		{
			return (RoadCenterlineDataSource)value switch
			{
				RoadCenterlineDataSource.None => "无数据源", 
				RoadCenterlineDataSource.StationElevation => "桩号高程表", 
				RoadCenterlineDataSource.HorizontalVerticalCurve => "平纵曲线表", 
				_ => "未知数据源", 
			};
		}
		return "无数据源";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
