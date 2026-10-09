using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Services;
using ns7;

namespace RevitAi.Core.Services;

public class TileCalculator
{
	private const string string_0 = "https://t0.tianditu.gov.cn/img_w/wmts";

	private readonly IApiKeyService? iapiKeyService_0;

	public TileCalculator(IApiKeyService? apiKeyService = null)
	{
		iapiKeyService_0 = apiKeyService;
	}

	public static (int x, int y) LatLonToTile(double lat, double lon, int zoom)
	{
		int item = (int)Math.Floor((lon + 180.0) / 360.0 * Math.Pow(2.0, zoom));
		int item2 = (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * Math.Pow(2.0, zoom));
		return (x: item, y: item2);
	}

	public static (double minLon, double minLat, double maxLon, double maxLat) TileToLatLon(int x, int y, int zoom)
	{
		double num = Math.Pow(2.0, zoom);
		double item = (double)x / num * 360.0 - 180.0;
		double item2 = (double)(x + 1) / num * 360.0 - 180.0;
		double val = Math.Atan(Math.Sinh(Math.PI * (1.0 - (double)(2 * y) / num))) * 180.0 / Math.PI;
		double val2 = Math.Atan(Math.Sinh(Math.PI * (1.0 - (double)(2 * (y + 1)) / num))) * 180.0 / Math.PI;
		return (minLon: item, minLat: Math.Min(val, val2), maxLon: item2, maxLat: Math.Max(val, val2));
	}

	public static (int xCount, int yCount) GetTileCount(double minLon, double minLat, double maxLon, double maxLat, int zoom)
	{
		(int x, int y) tuple = LatLonToTile(minLat, minLon, zoom);
		int item = tuple.x;
		int item2 = tuple.y;
		(int x, int y) tuple2 = LatLonToTile(maxLat, maxLon, zoom);
		int item3 = tuple2.x;
		int item4 = tuple2.y;
		int item5 = item3 - item + 1;
		int item6 = item2 - item4 + 1;
		return (xCount: item5, yCount: item6);
	}

	public (int minX, int maxX, int minY, int maxY)? CalculateTileRange(BoundingBox bounds, int zoom)
	{
		try
		{
			(int x, int y) tuple = LatLonToTile(bounds.minLat, bounds.minLon, zoom);
			int item = tuple.x;
			int item2 = tuple.y;
			(int x, int y) tuple2 = LatLonToTile(bounds.maxLat, bounds.maxLon, zoom);
			int item3 = tuple2.x;
			int item4 = tuple2.y;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 4);
			defaultInterpolatedStringHandler.AppendLiteral("[TileCalculator] 瓦片范围: X[");
			defaultInterpolatedStringHandler.AppendFormatted(item);
			defaultInterpolatedStringHandler.AppendLiteral("-");
			defaultInterpolatedStringHandler.AppendFormatted(item3);
			defaultInterpolatedStringHandler.AppendLiteral("], Y[");
			defaultInterpolatedStringHandler.AppendFormatted(item4);
			defaultInterpolatedStringHandler.AppendLiteral("-");
			defaultInterpolatedStringHandler.AppendFormatted(item2);
			defaultInterpolatedStringHandler.AppendLiteral("]");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("[TileCalculator] 瓦片数量: ");
			defaultInterpolatedStringHandler2.AppendFormatted(item3 - item + 1);
			defaultInterpolatedStringHandler2.AppendLiteral(" x ");
			defaultInterpolatedStringHandler2.AppendFormatted(item2 - item4 + 1);
			defaultInterpolatedStringHandler2.AppendLiteral(" = ");
			defaultInterpolatedStringHandler2.AppendFormatted((item3 - item + 1) * (item2 - item4 + 1));
			defaultInterpolatedStringHandler2.AppendLiteral(" 个");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return (item, item3, item4, item2);
		}
		catch (Exception ex)
		{
			Logger.Error("[TileCalculator] 计算瓦片范围失败: " + ex.Message);
			return null;
		}
	}

	public List<TileCoordinate> GenerateTileUrls((int minX, int maxX, int minY, int maxY) tileRange, int zoom)
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		List<TileCoordinate> list = new List<TileCoordinate>();
		try
		{
			for (int i = tileRange.minY; i <= tileRange.maxY; i++)
			{
				var (j, _, _, _) = tileRange;
				for (; j <= tileRange.maxX; j++)
				{
					if (iapiKeyService_0 != null)
					{
						string apiKey = iapiKeyService_0.GetApiKey("tianditu_token");
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(130, 5);
						defaultInterpolatedStringHandler.AppendFormatted("https://t0.tianditu.gov.cn/img_w/wmts");
						defaultInterpolatedStringHandler.AppendLiteral("?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0");
						defaultInterpolatedStringHandler.AppendLiteral("&LAYER=img&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles");
						defaultInterpolatedStringHandler.AppendLiteral("&TILEMATRIX=");
						defaultInterpolatedStringHandler.AppendFormatted(zoom);
						defaultInterpolatedStringHandler.AppendLiteral("&TILEROW=");
						defaultInterpolatedStringHandler.AppendFormatted(i);
						defaultInterpolatedStringHandler.AppendLiteral("&TILECOL=");
						defaultInterpolatedStringHandler.AppendFormatted(j);
						defaultInterpolatedStringHandler.AppendLiteral("&tk=");
						defaultInterpolatedStringHandler.AppendFormatted(apiKey);
						string text = defaultInterpolatedStringHandler.ToStringAndClear();
						list.Add(new TileCoordinate(j, i, zoom, text));
						continue;
					}
					Logger.Error("[TileCalculator] ApiKeyService 未注入，无法获取天地图 Token");
					throw new InvalidOperationException("天地图 Token 未配置，请在 Supabase 数据库中配置 tianditu_token");
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[TileCalculator] 生成了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个瓦片 URL (Zoom: ");
			defaultInterpolatedStringHandler2.AppendFormatted(zoom);
			defaultInterpolatedStringHandler2.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			Logger.Error("[TileCalculator] 生成瓦片 URL 失败: " + ex.Message);
			return new List<TileCoordinate>();
		}
	}

	public List<TileCoordinate> GenerateTileUrls((int minX, int maxX, int minY, int maxY) tileRange, int zoom, string tiandituToken)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		List<TileCoordinate> list = new List<TileCoordinate>();
		try
		{
			for (int i = tileRange.minY; i <= tileRange.maxY; i++)
			{
				var (j, _, _, _) = tileRange;
				for (; j <= tileRange.maxX; j++)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(130, 5);
					defaultInterpolatedStringHandler.AppendFormatted("https://t0.tianditu.gov.cn/img_w/wmts");
					defaultInterpolatedStringHandler.AppendLiteral("?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0");
					defaultInterpolatedStringHandler.AppendLiteral("&LAYER=img&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles");
					defaultInterpolatedStringHandler.AppendLiteral("&TILEMATRIX=");
					defaultInterpolatedStringHandler.AppendFormatted(zoom);
					defaultInterpolatedStringHandler.AppendLiteral("&TILEROW=");
					defaultInterpolatedStringHandler.AppendFormatted(i);
					defaultInterpolatedStringHandler.AppendLiteral("&TILECOL=");
					defaultInterpolatedStringHandler.AppendFormatted(j);
					defaultInterpolatedStringHandler.AppendLiteral("&tk=");
					defaultInterpolatedStringHandler.AppendFormatted(tiandituToken);
					string text = defaultInterpolatedStringHandler.ToStringAndClear();
					list.Add(new TileCoordinate(j, i, zoom, text));
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[TileCalculator] 生成了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个瓦片 URL (Zoom: ");
			defaultInterpolatedStringHandler2.AppendFormatted(zoom);
			defaultInterpolatedStringHandler2.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			Logger.Error("[TileCalculator] 生成瓦片 URL 失败: " + ex.Message);
			return new List<TileCoordinate>();
		}
	}
}
