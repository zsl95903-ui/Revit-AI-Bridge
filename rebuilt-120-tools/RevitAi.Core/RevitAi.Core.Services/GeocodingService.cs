using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Network;
using Newtonsoft.Json.Linq;
using ns7;

namespace RevitAi.Core.Services;

public class GeocodingService
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct29 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<GeocodingResult> asyncTaskMethodBuilder_0;

		public string string_0;

		public GeocodingService geocodingService_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Expected O, but got Unknown
			int num = int_0;
			GeocodingService geocodingService = geocodingService_0;
			GeocodingResult result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00e3;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0171;
				}
				if (!string.IsNullOrWhiteSpace(string_0))
				{
					string requestUri = "https://nominatim.openstreetmap.org/search?format=json&q=" + Uri.EscapeDataString(string_0) + "&limit=1&addressdetails=1";
					awaiter = geocodingService.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00e3;
				}
				Logger.Error("[GeocodingService] 位置名称为空");
				result = null;
				goto end_IL_000f;
				IL_0171:
				JArray val = JArray.Parse(awaiter2.GetResult());
				if (val != null && ((JContainer)val).Count != 0)
				{
					JToken obj = val[0];
					JToken obj2 = obj[(object)"lat"];
					double? num2 = ((obj2 != null) ? new double?(Extensions.Value<double>((IEnumerable<JToken>)obj2)) : ((double?)null));
					JToken obj3 = obj[(object)"lon"];
					double? num3 = ((obj3 != null) ? new double?(Extensions.Value<double>((IEnumerable<JToken>)obj3)) : ((double?)null));
					JToken obj4 = obj[(object)"display_name"];
					string text = ((obj4 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj4) : null);
					if (num2.HasValue && num3.HasValue)
					{
						Logger.Info("[GeocodingService] 找到位置: " + text);
						result = new GeocodingResult
						{
							Latitude = num2.Value,
							Longitude = num3.Value,
							DisplayName = (text ?? string_0),
							OriginalQuery = string_0
						};
					}
					else
					{
						Logger.Error("[GeocodingService] 响应缺少坐标数据");
						result = null;
					}
				}
				else
				{
					Logger.Warning("[GeocodingService] 未找到位置: " + string_0);
					result = null;
				}
				goto end_IL_000f;
				IL_00e3:
				HttpResponseMessage result2 = awaiter.GetResult();
				if (result2.IsSuccessStatusCode)
				{
					awaiter2 = result2.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0171;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[GeocodingService] HTTP 请求失败: ");
				defaultInterpolatedStringHandler.AppendFormatted(result2.StatusCode);
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				result = null;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[GeocodingService] 地理编码失败: " + ex.Message);
				result = null;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct30 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<GeocodingResult>> asyncTaskMethodBuilder_0;

		public List<string> list_0;

		public GeocodingService geocodingService_0;

		private List<GeocodingResult> list_1;

		private List<string>.Enumerator enumerator_0;

		private TaskAwaiter<GeocodingResult?> taskAwaiter_0;

		private TaskAwaiter taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			GeocodingService geocodingService = geocodingService_0;
			if ((uint)num > 1u)
			{
				list_1 = new List<GeocodingResult>();
				enumerator_0 = list_0.GetEnumerator();
			}
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if (num != 1)
					{
						goto IL_0097;
					}
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_0090;
				}
				TaskAwaiter<GeocodingResult> awaiter2 = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<GeocodingResult>);
				num = -1;
				int_0 = -1;
				goto IL_00d8;
				IL_0090:
				awaiter.GetResult();
				goto IL_0097;
				IL_0097:
				if (enumerator_0.MoveNext())
				{
					string current = enumerator_0.Current;
					awaiter2 = geocodingService.GeocodeAsync(current).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00d8;
				}
				goto end_IL_002f;
				IL_00d8:
				GeocodingResult result = awaiter2.GetResult();
				if (result != null)
				{
					list_1.Add(result);
				}
				awaiter = Task.Delay(1100).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_0090;
				end_IL_002f:;
			}
			finally
			{
				if (num < 0)
				{
					((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
				}
			}
			enumerator_0 = default(List<string>.Enumerator);
			List<GeocodingResult> result2 = list_1;
			int_0 = -2;
			list_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly HttpClient httpClient_0;

	private const string string_0 = "https://nominatim.openstreetmap.org/search";

	public GeocodingService(IHttpClientFactory? httpClientFactory = null)
	{
		if (httpClientFactory != null)
		{
			httpClient_0 = httpClientFactory.CreateClient((string)null, 10);
		}
		else
		{
			httpClient_0 = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(10L)
			};
		}
		httpClient_0.DefaultRequestHeaders.Add("User-Agent", "RevitAi Revit Plugin (Geocoding)");
	}

	[AsyncStateMachine(typeof(Struct29))]
	public Task<GeocodingResult?> GeocodeAsync(string locationName)
	{
		Struct29 stateMachine = default(Struct29);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<GeocodingResult>.Create();
		stateMachine.geocodingService_0 = this;
		stateMachine.string_0 = locationName;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct30))]
	public Task<List<GeocodingResult>> GeocodeBatchAsync(List<string> locationNames)
	{
		Struct30 stateMachine = default(Struct30);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<GeocodingResult>>.Create();
		stateMachine.geocodingService_0 = this;
		stateMachine.list_0 = locationNames;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public GeocodingResult? Geocode(string locationName)
	{
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		try
		{
			if (string.IsNullOrWhiteSpace(locationName))
			{
				Logger.Error("[GeocodingService] 位置名称为空");
				return null;
			}
			string requestUri = "https://nominatim.openstreetmap.org/search?format=json&q=" + Uri.EscapeDataString(locationName) + "&limit=1&addressdetails=1";
			HttpResponseMessage result = httpClient_0.GetAsync(requestUri).GetAwaiter().GetResult();
			if (!result.IsSuccessStatusCode)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[GeocodingService] HTTP 请求失败: ");
				defaultInterpolatedStringHandler.AppendFormatted(result.StatusCode);
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			JArray val = JArray.Parse(result.Content.ReadAsStringAsync().GetAwaiter().GetResult());
			if (val != null && ((JContainer)val).Count != 0)
			{
				JToken obj = val[0];
				JToken obj2 = obj[(object)"lat"];
				double? num = ((obj2 != null) ? new double?(Extensions.Value<double>((IEnumerable<JToken>)obj2)) : ((double?)null));
				JToken obj3 = obj[(object)"lon"];
				double? num2 = ((obj3 != null) ? new double?(Extensions.Value<double>((IEnumerable<JToken>)obj3)) : ((double?)null));
				JToken obj4 = obj[(object)"display_name"];
				string text = ((obj4 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj4) : null);
				if (num.HasValue && num2.HasValue)
				{
					Logger.Info("[GeocodingService] 找到位置: " + text);
					return new GeocodingResult
					{
						Latitude = num.Value,
						Longitude = num2.Value,
						DisplayName = (text ?? locationName),
						OriginalQuery = locationName
					};
				}
				Logger.Error("[GeocodingService] 响应缺少坐标数据");
				return null;
			}
			Logger.Warning("[GeocodingService] 未找到位置: " + locationName);
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("[GeocodingService] 地理编码失败: " + ex.Message);
			return null;
		}
	}
}
