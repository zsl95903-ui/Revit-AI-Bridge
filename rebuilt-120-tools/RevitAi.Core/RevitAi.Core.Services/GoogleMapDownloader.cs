using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Network;
using RevitAi.Abstractions.Services;
using ns7;

namespace RevitAi.Core.Services;

public class GoogleMapDownloader : IDisposable
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct31 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<GoogleMapResult> asyncTaskMethodBuilder_0;

		public double double_0;

		public int int_1;

		public double? nullable_0;

		public double? nullable_1;

		public string string_0;

		public GoogleMapDownloader googleMapDownloader_0;

		public double double_1;

		public int int_2;

		private int int_3;

		private int int_4;

		private double double_2;

		private HttpResponseMessage httpResponseMessage_0;

		private string string_1;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		private FileStream fileStream_0;

		private TaskAwaiter taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Expected O, but got Unknown
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Expected O, but got Unknown
			int num = int_0;
			GoogleMapDownloader googleMapDownloader = googleMapDownloader_0;
			GoogleMapResult result3;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				HttpResponseMessage result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2;
				string path;
				string result2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4;
				switch (num)
				{
				default:
				{
					double d = double_0 * Math.PI / 180.0;
					double_2 = 156543.03392 * Math.Cos(d) / Math.Pow(2.0, int_1);
					if (nullable_0.HasValue && nullable_1.HasValue)
					{
						int_3 = (int)Math.Round(nullable_0.Value / double_2);
						int_4 = (int)Math.Round(nullable_1.Value / double_2);
						int_3 = Math.Min(int_3, 640);
						int_4 = Math.Min(int_4, 640);
						int_3 = Math.Max(int_3, 64);
						int_4 = Math.Max(int_4, 64);
					}
					else
					{
						int_3 = 640;
						int_4 = 640;
					}
					object obj = string_0;
					if (obj == null)
					{
						IApiKeyService? iapiKeyService_ = googleMapDownloader.iapiKeyService_0;
						obj = ((iapiKeyService_ != null) ? iapiKeyService_.GetApiKey("google_maps_api_key") : null);
					}
					string value = (string)obj;
					if (string.IsNullOrEmpty(value))
					{
						throw new InvalidOperationException("Google Maps API Key 未提供，请检查 Supabase 数据库配置");
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(98, 7);
					defaultInterpolatedStringHandler.AppendLiteral("https://maps.googleapis.com/maps/api/staticmap?");
					defaultInterpolatedStringHandler.AppendLiteral("center=");
					defaultInterpolatedStringHandler.AppendFormatted(double_0);
					defaultInterpolatedStringHandler.AppendLiteral(",");
					defaultInterpolatedStringHandler.AppendFormatted(double_1);
					defaultInterpolatedStringHandler.AppendLiteral("&");
					defaultInterpolatedStringHandler.AppendLiteral("zoom=");
					defaultInterpolatedStringHandler.AppendFormatted(int_1);
					defaultInterpolatedStringHandler.AppendLiteral("&");
					defaultInterpolatedStringHandler.AppendLiteral("size=");
					defaultInterpolatedStringHandler.AppendFormatted(int_3);
					defaultInterpolatedStringHandler.AppendLiteral("x");
					defaultInterpolatedStringHandler.AppendFormatted(int_4);
					defaultInterpolatedStringHandler.AppendLiteral("&");
					defaultInterpolatedStringHandler.AppendLiteral("maptype=satellite&");
					defaultInterpolatedStringHandler.AppendLiteral("scale=");
					defaultInterpolatedStringHandler.AppendFormatted(int_2);
					defaultInterpolatedStringHandler.AppendLiteral("&");
					defaultInterpolatedStringHandler.AppendLiteral("key=");
					defaultInterpolatedStringHandler.AppendFormatted(value);
					string requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
					awaiter2 = googleMapDownloader.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0314;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0314;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0401;
				case 2:
					break;
					IL_0314:
					result = awaiter2.GetResult();
					httpResponseMessage_0 = result;
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0401;
					}
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(14, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("GoogleMap_");
					defaultInterpolatedStringHandler2.AppendFormatted(Guid.NewGuid());
					defaultInterpolatedStringHandler2.AppendLiteral(".png");
					path = defaultInterpolatedStringHandler2.ToStringAndClear();
					string_1 = Path.Combine(googleMapDownloader.string_0, path);
					fileStream_0 = new FileStream(string_1, FileMode.Create);
					break;
					IL_0401:
					result2 = awaiter.GetResult();
					defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(33, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("[GoogleMapDownloader] HTTP 请求失败: ");
					defaultInterpolatedStringHandler3.AppendFormatted(httpResponseMessage_0.StatusCode);
					Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
					Logger.Error("[GoogleMapDownloader] 错误内容: " + result2);
					defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(20, 2);
					defaultInterpolatedStringHandler4.AppendLiteral("Google Maps 请求失败: ");
					defaultInterpolatedStringHandler4.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler4.AppendLiteral(", ");
					defaultInterpolatedStringHandler4.AppendFormatted(result2);
					throw new Exception(defaultInterpolatedStringHandler4.ToStringAndClear());
				}
				try
				{
					TaskAwaiter awaiter3;
					if (num != 2)
					{
						awaiter3 = httpResponseMessage_0.Content.CopyToAsync(fileStream_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_2 = awaiter3;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
							return;
						}
					}
					else
					{
						awaiter3 = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter);
						num = -1;
						int_0 = -1;
					}
					awaiter3.GetResult();
				}
				finally
				{
					if (num < 0 && fileStream_0 != null)
					{
						((IDisposable)fileStream_0).Dispose();
					}
				}
				fileStream_0 = null;
				int num2 = int_3 * int_2;
				int num3 = int_4 * int_2;
				int imageWidth = num2;
				int imageHeight = num3 - 80;
				double num4 = double_2 * (double)int_3;
				double num5 = double_2 * (double)int_4;
				double widthInFeet = num4 * 3.2808399;
				double heightInFeet = num5 * 3.2808399;
				double meters = num4 / 2.0;
				double num6 = GeoCoordinateConverter.MetersToLatDelta(num5 / 2.0);
				double num7 = GeoCoordinateConverter.MetersToLonDelta(double_0, meters);
				BoundingBox geoBounds = new BoundingBox
				{
					minLon = double_1 - num7,
					maxLon = double_1 + num7,
					minLat = double_0 - num6,
					maxLat = double_0 + num6
				};
				result3 = new GoogleMapResult
				{
					LocalImagePath = string_1,
					ImageWidth = imageWidth,
					ImageHeight = imageHeight,
					WidthInFeet = widthInFeet,
					HeightInFeet = heightInFeet,
					GeoBounds = geoBounds,
					CenterLat = double_0,
					CenterLon = double_1
				};
			}
			catch (Exception ex)
			{
				Logger.Error("[GoogleMapDownloader] 下载失败: " + ex.Message);
				throw;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result3);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private const int int_0 = 640;

	private const int int_1 = 640;

	private const int int_2 = 2;

	private const int int_3 = 80;

	private const int int_4 = 1280;

	private const int int_5 = 1200;

	private readonly HttpClient httpClient_0;

	private readonly string string_0;

	private bool bool_0;

	private readonly IApiKeyService? iapiKeyService_0;

	private readonly IHttpClientFactory? ihttpClientFactory_0;

	public GoogleMapDownloader(IApiKeyService? apiKeyService = null, IHttpClientFactory? httpClientFactory = null)
	{
		iapiKeyService_0 = apiKeyService;
		ihttpClientFactory_0 = httpClientFactory;
		if (ihttpClientFactory_0 != null)
		{
			httpClient_0 = ihttpClientFactory_0.CreateClient((string)null, 60);
		}
		else
		{
			httpClient_0 = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(60L)
			};
		}
		string_0 = Path.Combine(Path.GetTempPath(), "RevitAi", "GoogleMaps");
		Directory.CreateDirectory(string_0);
	}

	public GoogleMapDownloader()
		: this(null, null)
	{
	}

	[AsyncStateMachine(typeof(Struct31))]
	public Task<GoogleMapResult> DownloadSatelliteMap(double centerLat, double centerLon, int zoom = 15, double? targetWidthMeters = null, double? targetHeightMeters = null, int scale = 2, string? apiKey = null)
	{
		Struct31 stateMachine = default(Struct31);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<GoogleMapResult>.Create();
		stateMachine.googleMapDownloader_0 = this;
		stateMachine.double_0 = centerLat;
		stateMachine.double_1 = centerLon;
		stateMachine.int_1 = zoom;
		stateMachine.nullable_0 = targetWidthMeters;
		stateMachine.nullable_1 = targetHeightMeters;
		stateMachine.int_2 = scale;
		stateMachine.string_0 = apiKey;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public void Cleanup()
	{
		try
		{
			if (Directory.Exists(string_0))
			{
				Directory.Delete(string_0, recursive: true);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[GoogleMapDownloader] 清理临时目录失败: " + ex.Message);
		}
	}

	public void Dispose()
	{
		if (!bool_0)
		{
			httpClient_0?.Dispose();
			bool_0 = true;
		}
	}
}
