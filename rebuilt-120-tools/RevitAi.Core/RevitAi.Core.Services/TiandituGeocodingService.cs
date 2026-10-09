using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using Newtonsoft.Json.Linq;
using ns7;

namespace RevitAi.Core.Services;

public class TiandituGeocodingService
{
	[CompilerGenerated]
	public sealed class Class71
	{
		public TiandituGeocodingService tiandituGeocodingService_0;

		public string string_0;

		public HttpResponseMessage httpResponseMessage_0;

		internal Task<HttpResponseMessage>? method_0()
		{
			return tiandituGeocodingService_0.httpClient_0.GetAsync(string_0);
		}

		internal Task<string>? method_1()
		{
			return httpResponseMessage_0.Content.ReadAsStringAsync();
		}

		internal Task<string>? method_2()
		{
			return httpResponseMessage_0.Content.ReadAsStringAsync();
		}
	}

	private readonly HttpClient httpClient_0;

	private const string string_0 = "http://api.tianditu.gov.cn/geocoder";

	public TiandituGeocodingService()
	{
		httpClient_0 = new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(10L)
		};
		httpClient_0.DefaultRequestHeaders.Add("Referer", "http://localhost");
		httpClient_0.DefaultRequestHeaders.Add("Origin", "http://localhost");
		httpClient_0.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
		httpClient_0.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
		httpClient_0.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
		httpClient_0.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
		httpClient_0.DefaultRequestHeaders.Add("Connection", "keep-alive");
	}

	public GeocodingResult? Geocode(string locationName, string token)
	{
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Expected O, but got Unknown
		try
		{
			if (string.IsNullOrWhiteSpace(locationName))
			{
				Logger.Error("[TiandituGeocoding] 位置名称为空");
				return null;
			}
			if (!string.IsNullOrWhiteSpace(token) && !(token == "YOUR_TIANDITU_TOKEN_HERE"))
			{
				string stringToEscape = "{\"keyWord\":\"" + locationName + "\"}";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 3);
				defaultInterpolatedStringHandler.AppendFormatted("http://api.tianditu.gov.cn/geocoder");
				defaultInterpolatedStringHandler.AppendLiteral("?ds=");
				defaultInterpolatedStringHandler.AppendFormatted(Uri.EscapeDataString(stringToEscape));
				defaultInterpolatedStringHandler.AppendLiteral("&tk=");
				defaultInterpolatedStringHandler.AppendFormatted(token);
				string string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
				Logger.Info("[TiandituGeocoding] 查询位置: " + locationName);
				HttpResponseMessage httpResponseMessage_0 = Task.Run(() => httpClient_0.GetAsync(string_0)).GetAwaiter().GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[TiandituGeocoding] HTTP 请求失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
					Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
					string result = Task.Run(() => httpResponseMessage_0.Content.ReadAsStringAsync()).GetAwaiter().GetResult();
					Logger.Error("[TiandituGeocoding] 响应内容: " + result);
					return null;
				}
				string result2 = Task.Run(() => httpResponseMessage_0.Content.ReadAsStringAsync()).GetAwaiter().GetResult();
				Logger.Debug("[TiandituGeocoding] API 响应: " + result2);
				JObject val = JObject.Parse(result2);
				object obj;
				if (val == null)
				{
					obj = null;
				}
				else
				{
					JToken obj2 = val["status"];
					obj = ((obj2 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj2) : null);
				}
				string text = (string)obj;
				if (text != "0")
				{
					object obj3;
					if (val == null)
					{
						obj3 = null;
					}
					else
					{
						JToken obj4 = val["msg"];
						obj3 = ((obj4 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj4) : null);
					}
					string text2 = (string)obj3;
					if (text == "101")
					{
						Logger.Warning("[TiandituGeocoding] 未找到位置: " + locationName);
					}
					else
					{
						Logger.Warning("[TiandituGeocoding] API 返回错误: status=" + text + ", msg=" + text2);
					}
					return null;
				}
				JToken val2 = ((val != null) ? val["location"] : null);
				if (val2 == null)
				{
					Logger.Warning("[TiandituGeocoding] 响应缺少 location 数据");
					return null;
				}
				JToken obj5 = val2[(object)"lon"];
				string text3 = ((obj5 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj5) : null);
				JToken obj6 = val2[(object)"lat"];
				string text4 = ((obj6 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj6) : null);
				JToken obj7 = val2[(object)"level"];
				string text5 = ((obj7 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj7) : null);
				JToken obj8 = val2[(object)"keyWord"];
				string text6 = ((obj8 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj8) : null);
				if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text4))
				{
					if (double.TryParse(text3, out var result3) && double.TryParse(text4, out var result4))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(37, 4);
						defaultInterpolatedStringHandler3.AppendLiteral("[TiandituGeocoding] 找到位置: ");
						defaultInterpolatedStringHandler3.AppendFormatted(text6 ?? locationName);
						defaultInterpolatedStringHandler3.AppendLiteral(" (");
						defaultInterpolatedStringHandler3.AppendFormatted(result4, "F6");
						defaultInterpolatedStringHandler3.AppendLiteral(", ");
						defaultInterpolatedStringHandler3.AppendFormatted(result3, "F6");
						defaultInterpolatedStringHandler3.AppendLiteral("), 级别: ");
						defaultInterpolatedStringHandler3.AppendFormatted(text5 ?? "未知");
						Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
						return new GeocodingResult
						{
							Latitude = result4,
							Longitude = result3,
							DisplayName = (text6 ?? locationName),
							OriginalQuery = locationName
						};
					}
					Logger.Error("[TiandituGeocoding] 坐标格式无效: lon=" + text3 + ", lat=" + text4);
					return null;
				}
				Logger.Error("[TiandituGeocoding] 响应缺少坐标数据");
				return null;
			}
			Logger.Error("[TiandituGeocoding] 天地图 Token 无效");
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("[TiandituGeocoding] 地理编码失败: " + ex.Message);
			Logger.Error("[TiandituGeocoding] 堆栈: " + ex.StackTrace);
			return null;
		}
	}
}
