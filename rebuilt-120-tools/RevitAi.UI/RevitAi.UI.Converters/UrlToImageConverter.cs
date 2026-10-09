using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using RevitAi.Abstractions.Logging;

namespace RevitAi.UI.Converters;

public class UrlToImageConverter : IValueConverter
{
	private static readonly HttpClient _httpClient = new HttpClient();

	private const int TimeoutSeconds = 10;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (!(value is string text) || string.IsNullOrEmpty(text))
		{
			return null;
		}
		try
		{
			if (text.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || text.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				byte[] array = DownloadImageSync(text);
				if (array == null || array.Length == 0)
				{
					return null;
				}
				BitmapImage bitmapImage = new BitmapImage();
				using (MemoryStream streamSource = new MemoryStream(array))
				{
					bitmapImage.BeginInit();
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapImage.StreamSource = streamSource;
					bitmapImage.EndInit();
				}
				((Freezable)bitmapImage).Freeze();
				return bitmapImage;
			}
			if (File.Exists(text))
			{
				BitmapImage bitmapImage2 = new BitmapImage();
				bitmapImage2.BeginInit();
				bitmapImage2.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage2.UriSource = new Uri(text, UriKind.Absolute);
				bitmapImage2.EndInit();
				((Freezable)bitmapImage2).Freeze();
				return bitmapImage2;
			}
			BitmapImage bitmapImage3 = new BitmapImage();
			bitmapImage3.BeginInit();
			bitmapImage3.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage3.UriSource = new Uri(text, UriKind.RelativeOrAbsolute);
			bitmapImage3.EndInit();
			((Freezable)bitmapImage3).Freeze();
			return bitmapImage3;
		}
		catch (Exception ex)
		{
			Logger.Debug("[UrlToImageConverter] 转换失败: " + text);
			Logger.Debug("[UrlToImageConverter] 异常类型: " + ex.GetType().Name);
			Logger.Debug("[UrlToImageConverter] 异常消息: " + ex.Message);
			if (ex.InnerException != null)
			{
				Logger.Debug("[UrlToImageConverter] 内部异常: " + ex.InnerException.Message);
			}
			return null;
		}
	}

	private byte[]? DownloadImageSync(string url)
	{
		try
		{
			using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10L));
			HttpResponseMessage result = _httpClient.GetAsync(url, cancellationTokenSource.Token).GetAwaiter().GetResult();
			if (!result.IsSuccessStatusCode)
			{
				Logger.Debug($"[UrlToImageConverter] HTTP错误: {result.StatusCode}");
				return null;
			}
			return result.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
		}
		catch (Exception ex)
		{
			Logger.Debug("[UrlToImageConverter] 下载图片异常: " + url + ", 错误: " + ex.Message);
			return null;
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
