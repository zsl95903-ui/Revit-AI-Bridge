using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public class ThumbnailConverter : IMultiValueConverter
{
	private static readonly HashSet<Guid> _downloadingThumbnails = new HashSet<Guid>();

	private static readonly HashSet<Guid> _pendingRefresh = new HashSet<Guid>();

	private static int _refreshToken = 0;

	public static int RefreshToken => _refreshToken;

	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length < 3 || values[0] == null)
		{
			return Binding.DoNothing;
		}
		object obj = values[0];
		if (obj is Guid)
		{
			Guid familyId = (Guid)obj;
			string text = values[1] as string;
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string cacheDir = Path.Combine(folderPath, "RevitAi", "FamilyLibrary", "Thumbnails");
			string localPath = Path.Combine(cacheDir, $"{familyId}.jpg");
			if (File.Exists(localPath + ".tmp"))
			{
				return DependencyProperty.UnsetValue;
			}
			if (File.Exists(localPath))
			{
				if (new FileInfo(localPath).Length >= 1024)
				{
					try
					{
						using FileStream fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
						byte[] array = new byte[4];
						int num = fileStream.Read(array, 0, 4);
						bool flag = false;
						if (num >= 2)
						{
							if (array[0] == byte.MaxValue && array[1] == 216)
							{
								flag = true;
							}
							else if (array[0] == 137 && array[1] == 80 && array[2] == 78 && array[3] == 71)
							{
								flag = true;
							}
							else if (array[0] == 71 && array[1] == 73 && array[2] == 70)
							{
								flag = true;
							}
							else if (array[0] == 66 && array[1] == 77)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							throw new InvalidDataException("无效的图片格式");
						}
						fileStream.Seek(0L, SeekOrigin.Begin);
						BitmapImage bitmapImage = new BitmapImage();
						bitmapImage.BeginInit();
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
						bitmapImage.StreamSource = fileStream;
						bitmapImage.EndInit();
						((Freezable)bitmapImage).Freeze();
						if (bitmapImage.PixelWidth == 0 || bitmapImage.PixelHeight == 0)
						{
							throw new InvalidDataException("图片尺寸无效");
						}
						obj = bitmapImage;
					}
					catch (IOException)
					{
						obj = DependencyProperty.UnsetValue;
					}
					catch (InvalidDataException)
					{
						try
						{
							File.Delete(localPath);
						}
						catch
						{
						}
						obj = DependencyProperty.UnsetValue;
					}
					catch (Exception)
					{
						try
						{
							File.Delete(localPath);
						}
						catch
						{
						}
						obj = DependencyProperty.UnsetValue;
					}
					return obj;
				}
				try
				{
					File.Delete(localPath);
				}
				catch
				{
				}
			}
			if (!string.IsNullOrEmpty(text) && !_downloadingThumbnails.Contains(familyId))
			{
				_downloadingThumbnails.Add(familyId);
				_pendingRefresh.Add(familyId);
				string urlToDownload = text;
				ThreadPool.QueueUserWorkItem(delegate
				{
					try
					{
						DownloadThumbnailAsync(familyId, urlToDownload, localPath, cacheDir);
					}
					finally
					{
						_downloadingThumbnails.Remove(familyId);
					}
				});
			}
		}
		return DependencyProperty.UnsetValue;
	}

	private async void DownloadThumbnailAsync(Guid familyId, string remoteUrl, string localPath, string cacheDir)
	{
		_ = 3;
		try
		{
			using HttpClient client = new HttpClient();
			client.Timeout = TimeSpan.FromSeconds(30L);
			HttpResponseMessage httpResponseMessage = await client.GetAsync(remoteUrl);
			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				return;
			}
			Directory.CreateDirectory(cacheDir);
			byte[] bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();
			Directory.CreateDirectory(cacheDir);
			string text = localPath + ".tmp";
			File.WriteAllBytes(text, bytes);
			if (File.Exists(localPath))
			{
				File.Delete(localPath);
			}
			File.Move(text, localPath);
			if (!_pendingRefresh.Contains(familyId))
			{
				return;
			}
			_pendingRefresh.Remove(familyId);
			await Task.Delay(300);
			if (Application.Current == null || ((DispatcherObject)Application.Current).Dispatcher == null)
			{
				return;
			}
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
			{
				try
				{
					foreach (object window in Application.Current.Windows)
					{
						if (window is FamilyLibraryWindow { DataContext: FamilyLibraryViewModel dataContext })
						{
							dataContext.ThumbnailRefreshToken++;
							break;
						}
					}
				}
				catch (Exception ex2)
				{
					Logger.Error("[ThumbnailConverter] 刷新 UI 失败", ex2);
				}
			});
		}
		catch (Exception ex)
		{
			Logger.Error($"[ThumbnailConverter] 缩略图下载异常: {familyId}", ex);
		}
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
