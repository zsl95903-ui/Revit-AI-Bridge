using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitAi.UI.Views.Windows;

public sealed class ColorConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is string text && text.StartsWith("#"))
		{
			try
			{
				string text2 = text.TrimStart('#');
				byte a = byte.MaxValue;
				byte r;
				byte g;
				byte b;
				if (text2.Length == 8)
				{
					a = byte.Parse(text2.Substring(0, 2), NumberStyles.HexNumber);
					r = byte.Parse(text2.Substring(2, 2), NumberStyles.HexNumber);
					g = byte.Parse(text2.Substring(4, 2), NumberStyles.HexNumber);
					b = byte.Parse(text2.Substring(6, 2), NumberStyles.HexNumber);
				}
				else
				{
					if (text2.Length != 6)
					{
						return Color.FromRgb(33, 150, 243);
					}
					r = byte.Parse(text2.Substring(0, 2), NumberStyles.HexNumber);
					g = byte.Parse(text2.Substring(2, 2), NumberStyles.HexNumber);
					b = byte.Parse(text2.Substring(4, 2), NumberStyles.HexNumber);
				}
				return Color.FromArgb(a, r, g, b);
			}
			catch
			{
				return Color.FromRgb(33, 150, 243);
			}
		}
		return Color.FromRgb(33, 150, 243);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
