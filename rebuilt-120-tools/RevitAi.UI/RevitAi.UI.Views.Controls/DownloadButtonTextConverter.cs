using System;
using System.Globalization;
using System.Windows.Data;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Controls;

public class DownloadButtonTextConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is CodeSnippetMarketItemViewModel codeSnippetMarketItemViewModel)
		{
			if (codeSnippetMarketItemViewModel.IsFree)
			{
				return "下载";
			}
			return "购买并下载";
		}
		return "下载";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
