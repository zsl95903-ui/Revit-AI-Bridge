using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Controls;

public class DownloadButtonBackgroundConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is CodeSnippetMarketItemViewModel codeSnippetMarketItemViewModel)
		{
			if (codeSnippetMarketItemViewModel.IsFree)
			{
				return new SolidColorBrush(Color.FromRgb(34, 197, 94));
			}
			return new SolidColorBrush(Color.FromRgb(0, 120, 212));
		}
		return new SolidColorBrush(Color.FromRgb(34, 197, 94));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
