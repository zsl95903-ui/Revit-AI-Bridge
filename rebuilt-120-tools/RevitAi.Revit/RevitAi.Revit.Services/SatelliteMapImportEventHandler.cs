using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Core.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

public class SatelliteMapImportEventHandler : IExternalEventHandler
{
	[CompilerGenerated]
	public sealed class _003C_003Ec__DisplayClass30_0
	{
		public sealed class _003C_003CDownloadGoogleMap_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<GoogleMapResult> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass30_0 _003C_003E4__this;

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
						awaiter = _003Cdownloader_003E5__1.DownloadSatelliteMap(_003C_003E4__this.centerLat, _003C_003E4__this.centerLon, _003C_003E4__this.calculatedZoom, _003C_003E4__this.widthMeters, _003C_003E4__this.heightMeters, 2, _003C_003E4__this._003C_003E4__this.GoogleMapsApiKey).GetAwaiter();
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

		public int calculatedZoom;

		public double widthMeters;

		public double heightMeters;

		public SatelliteMapImportEventHandler _003C_003E4__this;

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

	private static ExternalEvent? _currentEvent;

	public volatile bool IsCompleted = false;

	public BoundingBox? BoundingBox { get; set; }

	public int ZoomLevel { get; set; } = 18;

	public string MapSource { get; set; } = "tianditu";

	public string? TiandituToken { get; set; }

	public string? GoogleMapsApiKey { get; set; }

	public Action? OnCompleted { get; set; }

	public string GetName()
	{
		return "卫星图导入（支持天地图/Google Maps）";
	}

	public void Execute(UIApplication app)
	{
		try
		{
			Logger.Info("========== [SatelliteMapImportEventHandler] Execute 方法被调用 ==========");
			UIDocument activeUIDocument = app.ActiveUIDocument;
			Document val = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val == null)
			{
				Logger.Error("[SatelliteMapImportHandler] 没有打开的文档");
				return;
			}
			if (BoundingBox == null || !BoundingBox.IsValid())
			{
				Logger.Error("[SatelliteMapImportHandler] 边界框无效");
				return;
			}
			MergedImageResult val2 = null;
			string text = "";
			if (MapSource == "google")
			{
				val2 = DownloadGoogleMap();
				text = "Google Maps";
			}
			else
			{
				val2 = DownloadTiandituMap();
				text = "天地图";
			}
			if (val2 == null)
			{
				Logger.Error("[SatelliteMapImportHandler] 图像下载失败");
				return;
			}
			string text2 = CopyImageToProjectDirectory(val, val2.ImagePath);
			if (string.IsNullOrEmpty(text2))
			{
				Logger.Error("[SatelliteMapImportHandler] 复制图片到项目目录失败");
				return;
			}
			val2.ImagePath = text2;
			if (!File.Exists(text2))
			{
				Logger.Error("[SatelliteMapImportHandler] 永久图片文件不存在: " + text2);
				return;
			}
			FileInfo fileInfo = new FileInfo(text2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[SatelliteMapImportHandler] 永久图片文件大小: ");
			defaultInterpolatedStringHandler.AppendFormatted(fileInfo.Length / 1024L);
			defaultInterpolatedStringHandler.AppendLiteral(" KB");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			if (fileInfo.Length == 0L)
			{
				Logger.Error("[SatelliteMapImportHandler] 永久图片文件为空: " + text2);
				return;
			}
			using (RevitSatelliteMapImporter revitSatelliteMapImporter = new RevitSatelliteMapImporter())
			{
				List<ElementId> list = revitSatelliteMapImporter.ImportSatelliteMap(val, val2);
				if (list == null || list.Count == 0)
				{
					Logger.Error("[SatelliteMapImportHandler] " + text + " 导入失败");
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(42, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("[SatelliteMapImportHandler] ");
					defaultInterpolatedStringHandler2.AppendFormatted(text);
					defaultInterpolatedStringHandler2.AppendLiteral(" 导入成功，创建了 ");
					defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler2.AppendLiteral(" 个元素");
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					foreach (ElementId item in list)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(35, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("[SatelliteMapImportHandler] 元素 ID: ");
						defaultInterpolatedStringHandler3.AppendFormatted(item.Value);
						Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
					}
				}
			}
			SetCurrentViewRealistic(app);
			Logger.Info("========== 卫星图导入工作流完成 ==========");
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapImportHandler] 执行失败: " + ex.Message);
			Logger.Error("[SatelliteMapImportHandler] 堆栈跟踪: " + ex.StackTrace);
		}
		finally
		{
			Logger.Info("[SatelliteMapImportHandler] 清理临时文件...");
			CleanupTempFiles();
			IsCompleted = true;
			Logger.Info("[SatelliteMapImportHandler] 执行完成");
			OnCompleted?.Invoke();
			OnCompleted = null;
		}
	}

	private MergedImageResult? DownloadTiandituMap()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		Logger.Info("[SatelliteMapImportHandler] 使用天地图下载流程");
		TileCalculator tileCalculator = new TileCalculator();
		(int, int, int, int)? tuple = tileCalculator.CalculateTileRange(BoundingBox, ZoomLevel);
		if (!tuple.HasValue)
		{
			Logger.Error("[SatelliteMapImportHandler] 计算瓦片范围失败");
			return null;
		}
		List<TileCoordinate> tiles = tileCalculator.GenerateTileUrls(tuple.Value, ZoomLevel, TiandituToken);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[SatelliteMapImportHandler] 需要下载 ");
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
		TileDownloader downloader = new TileDownloader(config);
		MergedImageResult val = null;
		List<TileCoordinate> list = null;
		try
		{
			list = Task.Run(() => downloader.DownloadTilesAsync(tiles)).GetAwaiter().GetResult();
			if (list.Count == 0)
			{
				Logger.Error("[SatelliteMapImportHandler] 没有成功下载任何瓦片");
				downloader.Dispose();
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[SatelliteMapImportHandler] 成功下载 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral("/");
			defaultInterpolatedStringHandler2.AppendFormatted(tiles.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个瓦片");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			using (TileImageMerger tileImageMerger = new TileImageMerger())
			{
				string outputPath = TileImageMerger.GenerateTempOutputPath();
				val = tileImageMerger.MergeTiles(list, outputPath);
			}
			if (val == null)
			{
				Logger.Error("[SatelliteMapImportHandler] 图像拼接失败");
				downloader.Dispose();
				return null;
			}
			Logger.Info("[SatelliteMapImportHandler] 拼接图像已保存: " + val.ImagePath);
			downloader.Dispose();
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapImportHandler] 下载或拼接失败: " + ex.Message);
			downloader?.Dispose();
			throw;
		}
		return val;
	}

	private int CalculateOptimalZoomLevel(double centerLat, double widthMeters, double heightMeters)
	{
		double d = centerLat * Math.PI / 180.0;
		double num = Math.Cos(d);
		int num2 = 20;
		double num3;
		int num4;
		int num5;
		while (true)
		{
			if (num2 >= 1)
			{
				num3 = 156543.03392 * num / Math.Pow(2.0, num2);
				num4 = (int)Math.Ceiling(widthMeters / num3);
				num5 = (int)Math.Ceiling(heightMeters / num3);
				if (num4 <= 640 && num5 <= 640)
				{
					break;
				}
				num2--;
				continue;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[CalculateOptimalZoomLevel] 范围过大 (");
			defaultInterpolatedStringHandler.AppendFormatted(widthMeters, "F0");
			defaultInterpolatedStringHandler.AppendLiteral("m x ");
			defaultInterpolatedStringHandler.AppendFormatted(heightMeters, "F0");
			defaultInterpolatedStringHandler.AppendLiteral("m)，使用最低 zoom 级别 1");
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			return 1;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(51, 4);
		defaultInterpolatedStringHandler2.AppendLiteral("[CalculateOptimalZoomLevel] zoom=");
		defaultInterpolatedStringHandler2.AppendFormatted(num2);
		defaultInterpolatedStringHandler2.AppendLiteral(", 分辨率=");
		defaultInterpolatedStringHandler2.AppendFormatted(num3, "F2");
		defaultInterpolatedStringHandler2.AppendLiteral("m/px, 需要像素=");
		defaultInterpolatedStringHandler2.AppendFormatted(num4);
		defaultInterpolatedStringHandler2.AppendLiteral("x");
		defaultInterpolatedStringHandler2.AppendFormatted(num5);
		Logger.Debug(defaultInterpolatedStringHandler2.ToStringAndClear());
		return num2;
	}

	private MergedImageResult? DownloadGoogleMap()
	{
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		//IL_0237: Expected O, but got Unknown
		Logger.Info("[SatelliteMapImportHandler] 使用 Google Maps 下载流程");
		try
		{
			_003C_003Ec__DisplayClass30_0 CS_0024_003C_003E8__locals14 = new _003C_003Ec__DisplayClass30_0();
			CS_0024_003C_003E8__locals14._003C_003E4__this = this;
			CS_0024_003C_003E8__locals14.centerLat = (BoundingBox.minLat + BoundingBox.maxLat) / 2.0;
			CS_0024_003C_003E8__locals14.centerLon = (BoundingBox.minLon + BoundingBox.maxLon) / 2.0;
			CS_0024_003C_003E8__locals14.widthMeters = GeoCoordinateConverter.LonDeltaToMeters(CS_0024_003C_003E8__locals14.centerLat, BoundingBox.maxLon - BoundingBox.minLon);
			CS_0024_003C_003E8__locals14.heightMeters = GeoCoordinateConverter.LatDeltaToMeters(BoundingBox.maxLat - BoundingBox.minLat);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[SatelliteMapImportHandler] 范围尺寸: ");
			defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals14.widthMeters, "F0");
			defaultInterpolatedStringHandler.AppendLiteral("m x ");
			defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals14.heightMeters, "F0");
			defaultInterpolatedStringHandler.AppendLiteral("m");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			CS_0024_003C_003E8__locals14.calculatedZoom = CalculateOptimalZoomLevel(CS_0024_003C_003E8__locals14.centerLat, CS_0024_003C_003E8__locals14.widthMeters, CS_0024_003C_003E8__locals14.heightMeters);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(50, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[SatelliteMapImportHandler] 用户设置 zoom=");
			defaultInterpolatedStringHandler2.AppendFormatted(ZoomLevel);
			defaultInterpolatedStringHandler2.AppendLiteral("，自动调整为 zoom=");
			defaultInterpolatedStringHandler2.AppendFormatted(CS_0024_003C_003E8__locals14.calculatedZoom);
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			GoogleMapResult result = Task.Run([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass30_0._003C_003CDownloadGoogleMap_003Eb__0_003Ed))] [DebuggerStepThrough] () =>
			{
				_003C_003Ec__DisplayClass30_0._003C_003CDownloadGoogleMap_003Eb__0_003Ed stateMachine = new _003C_003Ec__DisplayClass30_0._003C_003CDownloadGoogleMap_003Eb__0_003Ed();
				stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<GoogleMapResult>.Create();
				stateMachine._003C_003E4__this = CS_0024_003C_003E8__locals14;
				stateMachine._003C_003E1__state = -1;
				stateMachine._003C_003Et__builder.Start(ref stateMachine);
				return stateMachine._003C_003Et__builder.Task;
			}).GetAwaiter().GetResult();
			if (result == null)
			{
				Logger.Error("[SatelliteMapImportHandler] Google Maps 下载失败");
				return null;
			}
			return new MergedImageResult
			{
				ImagePath = result.LocalImagePath,
				ImageWidth = result.ImageWidth,
				ImageHeight = result.ImageHeight,
				GeoBounds = result.GeoBounds,
				RevitSize = new RevitSize
				{
					widthFeet = result.WidthInFeet,
					heightFeet = result.HeightInFeet
				}
			};
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapImportHandler] Google Maps 下载失败: " + ex.Message);
			Logger.Error("[SatelliteMapImportHandler] 堆栈跟踪: " + ex.StackTrace);
			return null;
		}
	}

	public static ExternalEvent CreateEvent(SatelliteMapImportEventHandler handler)
	{
		_currentEvent = ExternalEvent.Create((IExternalEventHandler)(object)handler);
		return _currentEvent;
	}

	private string? CopyImageToProjectDirectory(Document doc, string tempImagePath)
	{
		try
		{
			if (!File.Exists(tempImagePath))
			{
				Logger.Error("[SatelliteMapImportHandler] 临时图片文件不存在: " + tempImagePath);
				return null;
			}
			string pathName = doc.PathName;
			string text;
			if (!string.IsNullOrEmpty(pathName))
			{
				string directoryName = Path.GetDirectoryName(pathName);
				text = Path.Combine(directoryName, "SatelliteMaps");
				Logger.Info("[SatelliteMapImportHandler] 使用项目目录: " + directoryName);
			}
			else
			{
				text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "SatelliteMaps");
				Logger.Warning("[SatelliteMapImportHandler] 文档未保存，使用 RevitAi 用户数据目录");
			}
			Directory.CreateDirectory(text);
			string text2 = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			string path = "SatelliteMap_" + text2 + ".png";
			string text3 = Path.Combine(text, path);
			File.Copy(tempImagePath, text3, overwrite: true);
			Logger.Info("[SatelliteMapImportHandler] 图片已复制到: " + text3);
			return text3;
		}
		catch (Exception ex)
		{
			Logger.Error("[SatelliteMapImportHandler] 复制图片到项目目录失败: " + ex.Message);
			Logger.Error("[SatelliteMapImportHandler] 堆栈跟踪: " + ex.StackTrace);
			return null;
		}
	}

	private void SetCurrentViewRealistic(UIApplication app)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UIDocument activeUIDocument = app.ActiveUIDocument;
			Document val = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val == null)
			{
				Logger.Warning("[SatelliteMapImportHandler] 无法获取文档，跳过设置视图样式");
				return;
			}
			View activeView = val.ActiveView;
			if (activeView == null)
			{
				Logger.Warning("[SatelliteMapImportHandler] 无法获取活动视图，跳过设置视图样式");
				return;
			}
			Parameter val2 = ((Element)activeView).get_Parameter((BuiltInParameter)(-1005165L));
			if (val2 == null)
			{
				Logger.Warning("[SatelliteMapImportHandler] 无法获取视图样式参数，跳过设置");
				return;
			}
			Transaction val3 = new Transaction(val, "设置视图样式为真实");
			try
			{
				val3.Start();
				val2.Set(6);
				val3.Commit();
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			Logger.Info("[SatelliteMapImportHandler] ✅ 已将视图 '" + ((Element)activeView).Name + "' 设置为真实样式");
		}
		catch (Exception ex)
		{
			Logger.Warning("[SatelliteMapImportHandler] 设置视图样式失败: " + ex.Message);
			Logger.Info("[SatelliteMapImportHandler] 提示：您可以手动将视图样式设置为「真实」以查看卫星图");
		}
	}

	private void CleanupTempFiles()
	{
		try
		{
			string path = Path.Combine(Path.GetTempPath(), "RevitAi");
			string text = Path.Combine(path, "GoogleMaps");
			if (Directory.Exists(text))
			{
				try
				{
					Directory.Delete(text, recursive: true);
					Logger.Info("[SatelliteMapImportHandler] 已清理 GoogleMaps 临时目录: " + text);
				}
				catch (Exception ex)
				{
					Logger.Warning("[SatelliteMapImportHandler] 清理 GoogleMaps 临时目录失败: " + ex.Message);
				}
			}
			string text2 = Path.Combine(path, "MergedImages");
			if (Directory.Exists(text2))
			{
				try
				{
					Directory.Delete(text2, recursive: true);
					Logger.Info("[SatelliteMapImportHandler] 已清理 MergedImages 临时目录: " + text2);
				}
				catch (Exception ex2)
				{
					Logger.Warning("[SatelliteMapImportHandler] 清理 MergedImages 临时目录失败: " + ex2.Message);
				}
			}
			string path2 = Path.Combine(path, "Tiles");
			if (!Directory.Exists(path2))
			{
				return;
			}
			try
			{
				string[] directories = Directory.GetDirectories(path2);
				string[] array = directories;
				foreach (string text3 in array)
				{
					try
					{
						Directory.Delete(text3, recursive: true);
					}
					catch (Exception ex3)
					{
						Logger.Warning("[SatelliteMapImportHandler] 清理 Tiles 子目录失败: " + text3 + ", " + ex3.Message);
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[SatelliteMapImportHandler] 已清理 Tiles 临时目录（");
				defaultInterpolatedStringHandler.AppendFormatted(directories.Length);
				defaultInterpolatedStringHandler.AppendLiteral(" 个子目录）");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			catch (Exception ex4)
			{
				Logger.Warning("[SatelliteMapImportHandler] 清理 Tiles 临时目录失败: " + ex4.Message);
			}
		}
		catch (Exception ex5)
		{
			Logger.Warning("[SatelliteMapImportHandler] 清理临时文件失败: " + ex5.Message);
		}
	}
}
