using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Services;
using RevitAi.Core.Services;
using Autodesk.Revit.DB;
using ns6;

namespace RevitAi.Revit.Services;

public class SatelliteMapAIService : ISatelliteMapAIService
{
	private class BoundingBoxWithSize
	{
		public BoundingBox GeoBounds { get; set; } = null;

		public double RevitWidthFeet { get; set; }

		public double RevitHeightFeet { get; set; }
	}

	[CompilerGenerated]
	public sealed class _003C_003Ec__DisplayClass14_0
	{
		public int zoomLevel;

		public string googleMapsApiKey;
	}

	[CompilerGenerated]
	public sealed class _003C_003Ec__DisplayClass14_1
	{
		public sealed class _003C_003CDownloadGoogleMap_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<GoogleMapResult> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass14_1 _003C_003E4__this;

			private GoogleMapDownloader _003Cdownloader_003E5__1;

			private GoogleMapResult _003C_003Es__2;

			private TaskAwaiter<GoogleMapResult> _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				if (num != 0)
				{
					_003Cdownloader_003E5__1 = new GoogleMapDownloader();
				}
				GoogleMapResult result;
				try
				{
					TaskAwaiter<GoogleMapResult> awaiter;
					if (num != 0)
					{
						awaiter = _003Cdownloader_003E5__1.DownloadSatelliteMap(_003C_003E4__this.centerLat, _003C_003E4__this.centerLon, _003C_003E4__this.CS_0024_003C_003E8__locals1.zoomLevel, _003C_003E4__this.widthMeters, _003C_003E4__this.heightMeters, 2, _003C_003E4__this.CS_0024_003C_003E8__locals1.googleMapsApiKey).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							_003C_003E1__state = 0;
							_003C_003Eu__1 = awaiter;
							_003C_003CDownloadGoogleMap_003Eb__0_003Ed stateMachine = this;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter<GoogleMapResult>);
						num = -1;
						_003C_003E1__state = -1;
					}
					_003C_003Es__2 = awaiter.GetResult();
					result = _003C_003Es__2;
				}
				finally
				{
					if (num < 0 && _003Cdownloader_003E5__1 != null)
					{
						((IDisposable)_003Cdownloader_003E5__1).Dispose();
					}
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(result);
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		public double centerLat;

		public double centerLon;

		public double widthMeters;

		public double heightMeters;

		public _003C_003Ec__DisplayClass14_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003CDownloadGoogleMap_003Eb__0_003Ed))]
		[DebuggerStepThrough]
		internal Task<GoogleMapResult>? _003CDownloadGoogleMap_003Eb__0()
		{
			_003C_003CDownloadGoogleMap_003Eb__0_003Ed stateMachine = new _003C_003CDownloadGoogleMap_003Eb__0_003Ed();
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<GoogleMapResult>.Create();
			stateMachine._003C_003E4__this = this;
			stateMachine._003C_003E1__state = -1;
			stateMachine._003C_003Et__builder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}
	}

	[CompilerGenerated]
	public sealed class _003C_003Ec__DisplayClass6_0
	{
		public string keyName;
	}

	[CompilerGenerated]
	public sealed class _003C_003Ec__DisplayClass6_1
	{
		public sealed class _003C_003CGetApiKey_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass6_1 _003C_003E4__this;

			private Task<string> _003Ctask_003E5__1;

			private string _003C_003Es__2;

			private ConfiguredTaskAwaitable<string>.ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				string result;
				ConfiguredTaskAwaitable<string>.ConfiguredTaskAwaiter awaiter;
				if (_003C_003E1__state != 0)
				{
					_003Ctask_003E5__1 = _003C_003E4__this.getApiKeyMethod.Invoke(_003C_003E4__this.apiKeyService, new object[1] { _003C_003E4__this.CS_0024_003C_003E8__locals1.keyName }) as Task<string>;
					if (_003Ctask_003E5__1 == null)
					{
						result = null;
						goto IL_00cf;
					}
					awaiter = _003Ctask_003E5__1.ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						int num = 0;
						_003C_003E1__state = 0;
						_003C_003Eu__1 = awaiter;
						_003C_003CGetApiKey_003Eb__0_003Ed stateMachine = this;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaitable<string>.ConfiguredTaskAwaiter);
					int num = -1;
					_003C_003E1__state = -1;
				}
				_003C_003Es__2 = awaiter.GetResult();
				result = _003C_003Es__2;
				goto IL_00cf;
				IL_00cf:
				_003C_003E1__state = -2;
				_003Ctask_003E5__1 = null;
				_003C_003Et__builder.SetResult(result);
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		public MethodInfo getApiKeyMethod;

		public object apiKeyService;

		public _003C_003Ec__DisplayClass6_0 CS_0024_003C_003E8__locals1;

		[DebuggerStepThrough]
		[AsyncStateMachine(typeof(_003C_003CGetApiKey_003Eb__0_003Ed))]
		internal Task<string>? _003CGetApiKey_003Eb__0()
		{
			_003C_003CGetApiKey_003Eb__0_003Ed stateMachine = new _003C_003CGetApiKey_003Eb__0_003Ed();
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<string>.Create();
			stateMachine._003C_003E4__this = this;
			stateMachine._003C_003E1__state = -1;
			stateMachine._003C_003Et__builder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}
	}

	private readonly TiandituGeocodingService _tiandituGeocoding;

	public static (int FloorElementId, int MaterialElementId)? DeferredPaintInfo { get; set; }

	public SatelliteMapAIService()
	{
		_tiandituGeocoding = new TiandituGeocodingService();
	}

	private string? GetApiKey(string keyName)
	{
		_003C_003Ec__DisplayClass6_0 obj = new _003C_003Ec__DisplayClass6_0();
		obj.keyName = keyName;
		try
		{
			_003C_003Ec__DisplayClass6_1 CS_0024_003C_003E8__locals8 = new _003C_003Ec__DisplayClass6_1();
			CS_0024_003C_003E8__locals8.CS_0024_003C_003E8__locals1 = obj;
			Type type = Type.GetType("RevitAi.UI.Services.UIBootstrapper, RevitAi.UI");
			if (type == null)
			{
				Logger.Warning("[SatelliteMapAIService] 找不到 UIBootstrapper 类型");
				return null;
			}
			PropertyInfo property = type.GetProperty("Services", BindingFlags.Static | BindingFlags.Public);
			if (property == null)
			{
				Logger.Warning("[SatelliteMapAIService] 找不到 UIBootstrapper.Services 属性");
				return null;
			}
			if (!(property.GetValue(null) is IServiceProvider obj2))
			{
				Logger.Warning("[SatelliteMapAIService] UIBootstrapper.Services 为 null");
				return null;
			}
			Type type2 = Type.GetType("RevitAi.Abstractions.Services.IApiKeyService, RevitAi.Abstractions");
			if (type2 == null)
			{
				Logger.Warning("[SatelliteMapAIService] 找不到 IApiKeyService 类型");
				return null;
			}
			MethodInfo method = typeof(IServiceProvider).GetMethod("GetService");
			if (method == null)
			{
				Logger.Warning("[SatelliteMapAIService] 找不到 GetService 方法");
				return null;
			}
			CS_0024_003C_003E8__locals8.apiKeyService = method.Invoke(obj2, new object[1] { type2 });
			if (CS_0024_003C_003E8__locals8.apiKeyService == null)
			{
				Logger.Warning("[SatelliteMapAIService] IApiKeyService 未在 DI 容器中注册");
				return null;
			}
			CS_0024_003C_003E8__locals8.getApiKeyMethod = type2.GetMethod("GetApiKeyAsync");
			if (CS_0024_003C_003E8__locals8.getApiKeyMethod == null)
			{
				Logger.Warning("[SatelliteMapAIService] 找不到 GetApiKeyAsync 方法");
				return null;
			}
			string result = Task.Run([DebuggerStepThrough] [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass6_1._003C_003CGetApiKey_003Eb__0_003Ed))] () =>
			{
				_003C_003Ec__DisplayClass6_1._003C_003CGetApiKey_003Eb__0_003Ed stateMachine = new _003C_003Ec__DisplayClass6_1._003C_003CGetApiKey_003Eb__0_003Ed();
				stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<string>.Create();
				stateMachine._003C_003E4__this = CS_0024_003C_003E8__locals8;
				stateMachine._003C_003E1__state = -1;
				stateMachine._003C_003Et__builder.Start(ref stateMachine);
				return stateMachine._003C_003Et__builder.Task;
			}).GetAwaiter().GetResult();
			if (string.IsNullOrEmpty(result) || result == "YOUR_TIANDITU_TOKEN_HERE")
			{
				Logger.Warning("[SatelliteMapAIService] " + CS_0024_003C_003E8__locals8.CS_0024_003C_003E8__locals1.keyName + " 未配置或无效");
				return null;
			}
			Logger.Info("[SatelliteMapAIService] 成功获取 " + CS_0024_003C_003E8__locals8.CS_0024_003C_003E8__locals1.keyName);
			return result;
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapAIService] 获取 API Key 失败: " + ex.Message);
			Logger.Error("[SatelliteMapAIService] 堆栈: " + ex.StackTrace);
			return null;
		}
	}

	public SatelliteMapImportResult ImportSatelliteMap(SatelliteMapImportRequest request)
	{
		return ImportSatelliteMapInternal(request, null);
	}

	public SatelliteMapImportResult ImportSatelliteMapWithTransaction(SatelliteMapImportRequest request, object transaction)
	{
		return ImportSatelliteMapInternal(request, transaction);
	}

	private SatelliteMapImportResult ImportSatelliteMapInternal(SatelliteMapImportRequest request, object? externalTransaction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Expected O, but got Unknown
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		SatelliteMapImportResult val = new SatelliteMapImportResult();
		try
		{
			if (!request.IsValid())
			{
				val.IsSuccess = false;
				val.ErrorMessage = "请求参数无效。请提供位置名称 (location_name) 或坐标 (latitude + longitude)";
				return val;
			}
			object document = request.Document;
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				val.IsSuccess = false;
				val.ErrorMessage = "Revit 文档对象无效";
				return val;
			}
			GeocodingResult val3 = null;
			(double, double)? centerCoordinates = request.GetCenterCoordinates();
			(double, double) tuple;
			if (centerCoordinates.HasValue)
			{
				tuple = centerCoordinates.Value;
			}
			else
			{
				string text = GetApiKey("tianditu_token") ?? "";
				if (string.IsNullOrEmpty(text) || text == "YOUR_TIANDITU_TOKEN_HERE")
				{
					val.IsSuccess = false;
					val.ErrorMessage = "天地图 Token 未配置。请在 Supabase 数据库中配置 tianditu_token，或直接提供经纬度坐标。";
					return val;
				}
				val3 = _tiandituGeocoding.Geocode(request.LocationName, text);
				if (val3 == null)
				{
					val.IsSuccess = false;
					val.ErrorMessage = "无法找到位置: " + request.LocationName + "。请尝试更详细的位置名称（如\"天津市 天津站\"）或直接提供经纬度坐标。";
					return val;
				}
				tuple = (val3.Latitude, val3.Longitude);
			}
			val.UsedLocation = val3;
			val.UsedZoomLevel = request.ZoomLevel;
			val.UsedMapSource = request.MapSource;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[SatelliteMapAIService] 导入位置: (");
			defaultInterpolatedStringHandler.AppendFormatted(tuple.Item1, "F6");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(tuple.Item2, "F6");
			defaultInterpolatedStringHandler.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			BoundingBoxWithSize boundingBoxWithSize = CalculateBoundingBoxWithSize(tuple.Item1, tuple.Item2, request.RadiusKm);
			string text2 = null;
			string text3 = null;
			if (request.MapSource == "tianditu")
			{
				text2 = GetApiKey("tianditu_token");
				if (string.IsNullOrEmpty(text2) || text2 == "YOUR_TIANDITU_TOKEN_HERE")
				{
					val.IsSuccess = false;
					val.ErrorMessage = "天地图 Token 未配置。请在 Supabase 数据库中配置 tianditu_token。";
					return val;
				}
			}
			else if (request.MapSource == "google")
			{
				text3 = GetApiKey("google_maps_api_key");
				if (string.IsNullOrEmpty(text3) || text3 == "YOUR_GOOGLE_MAPS_KEY_HERE")
				{
					val.IsSuccess = false;
					val.ErrorMessage = "Google Maps API Key 未配置。请在 Supabase 数据库中配置 google_maps_api_key。";
					return val;
				}
			}
			RevitSatelliteMapImporter revitSatelliteMapImporter = new RevitSatelliteMapImporter();
			MergedImageResult val4 = new MergedImageResult
			{
				RevitSize = new RevitSize
				{
					widthFeet = boundingBoxWithSize.RevitWidthFeet,
					heightFeet = boundingBoxWithSize.RevitHeightFeet
				},
				GeoBounds = boundingBoxWithSize.GeoBounds,
				ImagePath = DownloadAndMergeImage(boundingBoxWithSize.GeoBounds, request.MapSource, request.ZoomLevel, text2, text3)
			};
			string text4 = CopyImageToProjectDirectory(val2, val4.ImagePath);
			if (!string.IsNullOrEmpty(text4))
			{
				val4.ImagePath = text4;
			}
			else
			{
				Logger.Warning("[SatelliteMapAIService] 无法复制图片到项目目录，将使用临时路径");
			}
			List<ElementId> list;
			if (externalTransaction != null)
			{
				Transaction val5 = (Transaction)((externalTransaction is Transaction) ? externalTransaction : null);
				if (val5 != null)
				{
					list = revitSatelliteMapImporter.ImportSatelliteMapWithTransaction(val2, val4, val5);
					if (revitSatelliteMapImporter.PaintDeferred && list != null && list.Count > 0)
					{
						DeferredPaintInfo = ((int)revitSatelliteMapImporter.DeferredFloorId.Value, (int)revitSatelliteMapImporter.DeferredMaterialId.Value);
						Logger.Info("[SatelliteMapAIService] 已记录延迟 Paint 信息，等待父事务提交");
					}
					goto IL_03e2;
				}
			}
			list = revitSatelliteMapImporter.ImportSatelliteMap(val2, val4);
			goto IL_03e2;
			IL_03e2:
			if (list != null && list.Count > 0)
			{
				val.IsSuccess = true;
				val.CreatedFloorElementId = (int)list[0].Value;
				val.ImageWidthFeet = boundingBoxWithSize.RevitWidthFeet;
				val.ImageHeightFeet = boundingBoxWithSize.RevitHeightFeet;
				Logger.Info("[SatelliteMapAIService] 导入成功");
			}
			else
			{
				val.IsSuccess = false;
				val.ErrorMessage = "卫星图导入失败，请检查日志";
			}
			return val;
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapAIService] 导入失败: " + ex.Message);
			val.IsSuccess = false;
			val.ErrorMessage = "导入失败: " + ex.Message;
			return val;
		}
	}

	private static BoundingBoxWithSize CalculateBoundingBoxWithSize(double centerLat, double centerLon, double radiusKm)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		double num = radiusKm / 111.0;
		double num2 = radiusKm / (111.0 * Math.Cos(centerLat * Math.PI / 180.0));
		double num3 = radiusKm * 2.0;
		double num4 = radiusKm * 2.0;
		double revitWidthFeet = num3 * 3280.84;
		double revitHeightFeet = num4 * 3280.84;
		return new BoundingBoxWithSize
		{
			GeoBounds = new BoundingBox
			{
				minLat = centerLat - num,
				maxLat = centerLat + num,
				minLon = centerLon - num2,
				maxLon = centerLon + num2
			},
			RevitWidthFeet = revitWidthFeet,
			RevitHeightFeet = revitHeightFeet
		};
	}

	private string DownloadAndMergeImage(BoundingBox bounds, string mapSource, int zoomLevel, string? tiandituToken, string? googleMapsApiKey)
	{
		try
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[SatelliteMapAIService] 开始下载图像: mapSource=");
			defaultInterpolatedStringHandler.AppendFormatted(mapSource);
			defaultInterpolatedStringHandler.AppendLiteral(", zoom=");
			defaultInterpolatedStringHandler.AppendFormatted(zoomLevel);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			if (mapSource == "google" && !string.IsNullOrEmpty(googleMapsApiKey))
			{
				return DownloadGoogleMap(bounds, zoomLevel, googleMapsApiKey);
			}
			if (!(mapSource == "tianditu") || string.IsNullOrEmpty(tiandituToken))
			{
				Logger.Error("[SatelliteMapAIService] 不支持的地图源或缺少密钥: " + mapSource);
				throw new InvalidOperationException("地图源 " + mapSource + " 需要相应的 API 密钥");
			}
			return DownloadTiandituMap(bounds, zoomLevel, tiandituToken);
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapAIService] 下载图像失败: " + ex.Message);
			throw;
		}
	}

	private string DownloadTiandituMap(BoundingBox bounds, int zoomLevel, string tiandituToken)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		Logger.Info("[SatelliteMapAIService] 使用天地图下载");
		TileCalculator tileCalculator = new TileCalculator();
		(int, int, int, int)? tuple = tileCalculator.CalculateTileRange(bounds, zoomLevel);
		if (!tuple.HasValue)
		{
			throw new InvalidOperationException("计算瓦片范围失败");
		}
		List<TileCoordinate> tiles = tileCalculator.GenerateTileUrls(tuple.Value, zoomLevel, tiandituToken);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[SatelliteMapAIService] 需要下载 ");
		defaultInterpolatedStringHandler.AppendFormatted(tiles.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个瓦片");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		TileDownloadConfig config = new TileDownloadConfig
		{
			MinDelayMs = 300,
			MaxDelayMs = 600,
			MaxConcurrentRequests = 1,
			MaxRetries = 5,
			RequestTimeoutSeconds = 30
		};
		TileDownloader tileDownloader = null;
		List<TileCoordinate> result;
		try
		{
			TileDownloader capturedDownloader;
			tileDownloader = (capturedDownloader = new TileDownloader(config));
			result = Task.Run(() => capturedDownloader.DownloadTilesAsync(tiles)).GetAwaiter().GetResult();
			if (result.Count == 0)
			{
				throw new InvalidOperationException("没有成功下载任何瓦片");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[SatelliteMapAIService] 成功下载 ");
			defaultInterpolatedStringHandler2.AppendFormatted(result.Count);
			defaultInterpolatedStringHandler2.AppendLiteral("/");
			defaultInterpolatedStringHandler2.AppendFormatted(tiles.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个瓦片");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		}
		finally
		{
			tileDownloader?.Dispose();
		}
		string text = TileImageMerger.GenerateTempOutputPath();
		using TileImageMerger tileImageMerger = new TileImageMerger();
		MergedImageResult val = tileImageMerger.MergeTiles(result, text);
		if (val == null)
		{
			throw new InvalidOperationException("图像拼接失败");
		}
		Logger.Info("[SatelliteMapAIService] 拼接完成: " + text);
		return text;
	}

	private string DownloadGoogleMap(BoundingBox bounds, int zoomLevel, string googleMapsApiKey)
	{
		_003C_003Ec__DisplayClass14_0 obj = new _003C_003Ec__DisplayClass14_0();
		obj.zoomLevel = zoomLevel;
		obj.googleMapsApiKey = googleMapsApiKey;
		Logger.Info("[SatelliteMapAIService] 使用 Google Maps 下载");
		try
		{
			_003C_003Ec__DisplayClass14_1 CS_0024_003C_003E8__locals9 = new _003C_003Ec__DisplayClass14_1();
			CS_0024_003C_003E8__locals9.CS_0024_003C_003E8__locals1 = obj;
			CS_0024_003C_003E8__locals9.centerLat = (bounds.minLat + bounds.maxLat) / 2.0;
			CS_0024_003C_003E8__locals9.centerLon = (bounds.minLon + bounds.maxLon) / 2.0;
			CS_0024_003C_003E8__locals9.widthMeters = GeoCoordinateConverter.LonDeltaToMeters(CS_0024_003C_003E8__locals9.centerLat, bounds.maxLon - bounds.minLon);
			CS_0024_003C_003E8__locals9.heightMeters = GeoCoordinateConverter.LatDeltaToMeters(bounds.maxLat - bounds.minLat);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[SatelliteMapAIService] 范围尺寸: ");
			defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals9.widthMeters, "F0");
			defaultInterpolatedStringHandler.AppendLiteral("m x ");
			defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals9.heightMeters, "F0");
			defaultInterpolatedStringHandler.AppendLiteral("m");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			GoogleMapResult result = Task.Run([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass14_1._003C_003CDownloadGoogleMap_003Eb__0_003Ed))] [DebuggerStepThrough] () =>
			{
				_003C_003Ec__DisplayClass14_1._003C_003CDownloadGoogleMap_003Eb__0_003Ed stateMachine = new _003C_003Ec__DisplayClass14_1._003C_003CDownloadGoogleMap_003Eb__0_003Ed();
				stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<GoogleMapResult>.Create();
				stateMachine._003C_003E4__this = CS_0024_003C_003E8__locals9;
				stateMachine._003C_003E1__state = -1;
				stateMachine._003C_003Et__builder.Start(ref stateMachine);
				return stateMachine._003C_003Et__builder.Task;
			}).GetAwaiter().GetResult();
			if (result == null)
			{
				throw new InvalidOperationException("Google Maps 下载失败");
			}
			Logger.Info("[SatelliteMapAIService] Google Maps 下载完成: " + result.LocalImagePath);
			return result.LocalImagePath;
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapAIService] Google Maps 下载失败: " + ex.Message);
			throw;
		}
	}

	private static string? CopyImageToProjectDirectory(Document doc, string tempImagePath)
	{
		try
		{
			if (!File.Exists(tempImagePath))
			{
				Logger.Error("[SatelliteMapAIService] 临时图片文件不存在: " + tempImagePath);
				return null;
			}
			string pathName = doc.PathName;
			string text;
			if (!string.IsNullOrEmpty(pathName))
			{
				string directoryName = Path.GetDirectoryName(pathName);
				text = Path.Combine(directoryName, "SatelliteMaps");
			}
			else
			{
				text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "SatelliteMaps");
			}
			Directory.CreateDirectory(text);
			string text2 = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			string path = "SatelliteMap_" + text2 + ".png";
			string text3 = Path.Combine(text, path);
			File.Copy(tempImagePath, text3, overwrite: true);
			Logger.Info("[SatelliteMapAIService] 图片已复制到项目目录: " + text3);
			return text3;
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapAIService] 复制图片到项目目录失败: " + ex.Message);
			return null;
		}
	}
}
