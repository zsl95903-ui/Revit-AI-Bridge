using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using ns7;

namespace RevitAi.Core.Services;

public class TileImageMerger : IDisposable
{
	private bool bool_0;

	public static string GenerateTempOutputPath()
	{
		string text = Path.Combine(Path.GetTempPath(), "RevitAi", "MergedImages");
		Directory.CreateDirectory(text);
		return Path.Combine(text, string.Concat(str1: DateTime.Now.ToString("yyyyMMdd_HHmmss"), str0: "MergedImage_", str2: ".png"));
	}

	public MergedImageResult? MergeTiles(List<TileCoordinate> downloadedTiles, string outputPath)
	{
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Expected O, but got Unknown
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Expected O, but got Unknown
		//IL_044d: Expected O, but got Unknown
		try
		{
			if (downloadedTiles != null && downloadedTiles.Count != 0)
			{
				int num = downloadedTiles.Min((TileCoordinate tileCoordinate_0) => tileCoordinate_0.X);
				int num2 = downloadedTiles.Max((TileCoordinate tileCoordinate_0) => tileCoordinate_0.X);
				int num3 = downloadedTiles.Min((TileCoordinate tileCoordinate_0) => tileCoordinate_0.Y);
				int num4 = downloadedTiles.Max((TileCoordinate tileCoordinate_0) => tileCoordinate_0.Y);
				int num5 = num2 - num + 1;
				int num6 = num4 - num3 + 1;
				int num7 = num5 * 256;
				int num8 = num6 * 256;
				if (num7 <= 20000 && num8 <= 20000)
				{
					long num9 = (long)num7 * (long)num8;
					if (num9 > 100000000L)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[TileImageMerger] 合并像素数超出限制: ");
						defaultInterpolatedStringHandler.AppendFormatted(num9, "N0");
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
						return null;
					}
					using (Bitmap bitmap = new Bitmap(num7, num8))
					{
						using (Graphics graphics = Graphics.FromImage(bitmap))
						{
							graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
							graphics.SmoothingMode = SmoothingMode.HighQuality;
							foreach (TileCoordinate downloadedTile in downloadedTiles)
							{
								if (!downloadedTile.IsDownloaded || string.IsNullOrEmpty(downloadedTile.LocalPath) || !File.Exists(downloadedTile.LocalPath))
								{
									continue;
								}
								try
								{
									int x = (downloadedTile.X - num) * 256;
									int y = (downloadedTile.Y - num3) * 256;
									using Bitmap image = new Bitmap(downloadedTile.LocalPath);
									graphics.DrawImage(image, x, y, 256, 256);
								}
								catch (Exception ex)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 2);
									defaultInterpolatedStringHandler2.AppendLiteral("[TileImageMerger] 绘制瓦片失败: ");
									defaultInterpolatedStringHandler2.AppendFormatted<TileCoordinate>(downloadedTile);
									defaultInterpolatedStringHandler2.AppendLiteral(", 错误: ");
									defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
									Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
								}
							}
						}
						bitmap.Save(outputPath, ImageFormat.Png);
					}
					int zoom = downloadedTiles.First().Zoom;
					(double minLon, double minLat, double maxLon, double maxLat) tuple = TileCalculator.TileToLatLon(num, num3, zoom);
					double item = tuple.minLon;
					double item2 = tuple.minLat;
					double item3 = tuple.maxLon;
					double item4 = tuple.maxLat;
					(double minLon, double minLat, double maxLon, double maxLat) tuple2 = TileCalculator.TileToLatLon(num2, num4, zoom);
					double item5 = tuple2.maxLon;
					double item6 = tuple2.maxLat;
					item3 = item5;
					item4 = item6;
					BoundingBox geoBounds = new BoundingBox
					{
						minLon = item,
						maxLon = item3,
						minLat = item2,
						maxLat = item4
					};
					double lat = (item2 + item4) / 2.0;
					double lonDelta = 360.0 / Math.Pow(2.0, zoom);
					double num10 = GeoCoordinateConverter.LonDeltaToMeters(lat, lonDelta);
					double num11 = (double)num5 * num10;
					double num12 = (double)num6 * num10;
					double num13 = num11 / 0.3048;
					double num14 = num12 / 0.3048;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(36, 4);
					defaultInterpolatedStringHandler3.AppendLiteral("[TileImageMerger] 合并完成: ");
					defaultInterpolatedStringHandler3.AppendFormatted(num13, "F0");
					defaultInterpolatedStringHandler3.AppendLiteral("ft x ");
					defaultInterpolatedStringHandler3.AppendFormatted(num14, "F0");
					defaultInterpolatedStringHandler3.AppendLiteral("ft, ");
					defaultInterpolatedStringHandler3.AppendFormatted(num7);
					defaultInterpolatedStringHandler3.AppendLiteral("x");
					defaultInterpolatedStringHandler3.AppendFormatted(num8);
					defaultInterpolatedStringHandler3.AppendLiteral("px");
					Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
					return new MergedImageResult
					{
						ImagePath = outputPath,
						ImageWidth = num7,
						ImageHeight = num8,
						GeoBounds = geoBounds,
						RevitSize = new RevitSize
						{
							widthFeet = num13,
							heightFeet = num14
						}
					};
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(29, 2);
				defaultInterpolatedStringHandler4.AppendLiteral("[TileImageMerger] 合并尺寸超出限制: ");
				defaultInterpolatedStringHandler4.AppendFormatted(num7);
				defaultInterpolatedStringHandler4.AppendLiteral("x");
				defaultInterpolatedStringHandler4.AppendFormatted(num8);
				Logger.Error(defaultInterpolatedStringHandler4.ToStringAndClear());
				return null;
			}
			Logger.Error("[TileImageMerger] 没有可合并的瓦片");
			return null;
		}
		catch (Exception ex2)
		{
			Logger.Error("[TileImageMerger] 合并失败: " + ex2.Message);
			return null;
		}
	}

	public void Dispose()
	{
		if (!bool_0)
		{
			bool_0 = true;
		}
	}
}
