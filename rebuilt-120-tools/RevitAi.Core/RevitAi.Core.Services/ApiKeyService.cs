using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Network;
using RevitAi.Abstractions.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ns0;
using ns7;

namespace RevitAi.Core.Services;

public class ApiKeyService : IApiKeyService
{
	[CompilerGenerated]
	public sealed class Class70
	{
		public ApiKeyService apiKeyService_0;

		public string string_0;

		internal Task? method_0()
		{
			return apiKeyService_0.UpdateLastUsedAsync(string_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct8 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ApiKeyService apiKeyService_0;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ApiKeyService apiKeyService = apiKeyService_0;
			TaskAwaiter<string> awaiter;
			if (num != 0)
			{
				awaiter = apiKeyService.GetApiKeyAsync("amap_api_key").GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<string>);
				num = -1;
				int_0 = -1;
			}
			string result = awaiter.GetResult();
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
	public struct Struct9 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ApiKeyService apiKeyService_0;

		public string string_0;

		private Class70 class70_0;

		private TaskAwaiter<JToken> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ApiKeyService apiKeyService = apiKeyService_0;
			if (num != 0)
			{
				this.class70_0 = new Class70();
				this.class70_0.apiKeyService_0 = apiKeyService_0;
				this.class70_0.string_0 = string_0;
			}
			string result2;
			try
			{
				if (num == 0 || !concurrentDictionary_0.TryGetValue(this.class70_0.string_0, out string value))
				{
					try
					{
						TaskAwaiter<JToken> awaiter;
						if (num != 0)
						{
							awaiter = apiKeyService.method_0("get_api_key", new Class62<string>(this.class70_0.string_0)).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = taskAwaiter_0;
							taskAwaiter_0 = default(TaskAwaiter<JToken>);
							num = -1;
							int_0 = -1;
						}
						JToken result = awaiter.GetResult();
						if (result != null && result.HasValues)
						{
							JToken? obj = smethod_2(result);
							JObject val = (JObject)(object)((obj is JObject) ? obj : null);
							if (val != null)
							{
								string text = ((object)val["key_value"])?.ToString();
								if (string.IsNullOrEmpty(text))
								{
									Logger.Error("[ApiKeyService] Key 值为空: " + this.class70_0.string_0);
									result2 = smethod_3(this.class70_0.string_0);
								}
								else
								{
									concurrentDictionary_0.TryAdd(this.class70_0.string_0, text);
				var class70_0 = this.class70_0;
									Task.Run(() => class70_0.apiKeyService_0.UpdateLastUsedAsync(class70_0.string_0));
									result2 = text;
								}
							}
							else
							{
								Logger.Warning("[ApiKeyService] Key 未激活或不存在: " + class70_0.string_0 + "，使用默认 Key");
								result2 = smethod_3(class70_0.string_0);
							}
						}
						else
						{
							Logger.Warning("[ApiKeyService] 数据库中未找到 Key: " + class70_0.string_0 + "，使用默认 Key");
							result2 = smethod_3(class70_0.string_0);
						}
					}
					catch (Exception ex)
					{
						Logger.Error("[ApiKeyService] 从数据库获取 Key 失败: " + ex.Message);
						result2 = smethod_3(class70_0.string_0);
					}
				}
				else
				{
					result2 = value;
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("[ApiKeyService] 获取 Key 失败: " + ex2.Message);
				goto IL_0266;
			}
			goto IL_0279;
			IL_0279:
			int_0 = -2;
			class70_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result2);
			return;
			IL_0266:
			result2 = smethod_3(class70_0.string_0);
			goto IL_0279;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct10 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ApiKeyService apiKeyService_0;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ApiKeyService apiKeyService = apiKeyService_0;
			TaskAwaiter<string> awaiter;
			if (num != 0)
			{
				awaiter = apiKeyService.GetApiKeyAsync("google_maps_api_key").GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<string>);
				num = -1;
				int_0 = -1;
			}
			string result = awaiter.GetResult();
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
	public struct Struct11 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ApiKeyService apiKeyService_0;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ApiKeyService apiKeyService = apiKeyService_0;
			TaskAwaiter<string> awaiter;
			if (num != 0)
			{
				awaiter = apiKeyService.GetApiKeyAsync("tianditu_token").GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<string>);
				num = -1;
				int_0 = -1;
			}
			string result = awaiter.GetResult();
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
	public struct Struct12 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<JToken> asyncTaskMethodBuilder_0;

		public ApiKeyService apiKeyService_0;

		public string string_0;

		public object object_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Expected O, but got Unknown
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			ApiKeyService apiKeyService = apiKeyService_0;
			JToken result3;
			try
			{
				TaskAwaiter<string> awaiter;
				TaskAwaiter<HttpResponseMessage> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						goto IL_01af;
					}
					string requestUri = apiKeyService.string_0 + "/rest/v1/rpc/" + string_0;
					StringContent content = new StringContent(JsonConvert.SerializeObject(object_0), Encoding.UTF8, "application/json");
					apiKeyService.httpClient_0.DefaultRequestHeaders.Clear();
					apiKeyService.httpClient_0.DefaultRequestHeaders.Add("apikey", apiKeyService.string_1);
					apiKeyService.httpClient_0.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKeyService.string_1);
					apiKeyService.httpClient_0.DefaultRequestHeaders.Add("Prefer", "return=representation");
					awaiter2 = apiKeyService.httpClient_0.PostAsync(requestUri, content).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
				}
				HttpResponseMessage result = awaiter2.GetResult();
				httpResponseMessage_0 = result;
				awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_01af;
				IL_01af:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 3);
					defaultInterpolatedStringHandler.AppendLiteral("[ApiKeyService] RPC 调用失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral(", 状态: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", 响应: ");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					result3 = JToken.Parse("{}");
				}
				else
				{
					result3 = (JToken)((!string.IsNullOrEmpty(result2)) ? ((object)JToken.Parse(result2)) : ((object)new JObject()));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[ApiKeyService] RPC 调用异常: " + string_0 + ", 错误: " + ex.Message);
				result3 = (JToken)new JObject();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct13 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public ApiKeyService apiKeyService_0;

		public string string_0;

		public string string_1;

		public string string_2;

		public string string_3;

		private TaskAwaiter<JToken> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ApiKeyService apiKeyService = apiKeyService_0;
			try
			{
				TaskAwaiter<JToken> awaiter;
				if (num != 0)
				{
					IAuthManager iauthManager_ = apiKeyService.iauthManager_0;
					if (((iauthManager_ != null) ? iauthManager_.CurrentUser : null) == null)
					{
						throw new InvalidOperationException("用户未登录");
					}
					awaiter = apiKeyService.method_0("save_api_key", new Class63<string, string, string, string>(string_0, string_1, string_2, string_3)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<JToken>);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				concurrentDictionary_0.TryRemove(string_0, out string _);
				Logger.Info("[ApiKeyService] 已保存 Key: " + string_0);
			}
			catch (Exception ex)
			{
				Logger.Error("[ApiKeyService] 保存 Key 失败: " + ex.Message);
				throw;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct14 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public ApiKeyService apiKeyService_0;

		public string string_0;

		private TaskAwaiter<JToken> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ApiKeyService apiKeyService = apiKeyService_0;
			try
			{
				TaskAwaiter<JToken> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<JToken>);
					num = -1;
					int_0 = -1;
					goto IL_0091;
				}
				IAuthManager iauthManager_ = apiKeyService.iauthManager_0;
				if (((iauthManager_ != null) ? iauthManager_.CurrentUser : null) != null)
				{
					awaiter = apiKeyService.method_0("update_api_key_last_used", new Class62<string>(string_0)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0091;
				}
				goto end_IL_000f;
				IL_0091:
				awaiter.GetResult();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[ApiKeyService] 更新最后使用时间失败: " + ex.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly HttpClient httpClient_0;

	private readonly string string_0;

	private readonly string string_1;

	private readonly IAuthManager iauthManager_0;

	private static ConcurrentDictionary<string, string> concurrentDictionary_0 = new ConcurrentDictionary<string, string>();

	private const string string_2 = "YOUR_TIANDITU_TOKEN_HERE";

	private const string string_3 = "YOUR_GOOGLE_MAPS_KEY_HERE";

	private const string string_4 = "YOUR_AMAP_API_KEY_HERE";

	public ApiKeyService(HttpClient httpClient, string baseUrl, string apiKey, IAuthManager authManager)
	{
		httpClient_0 = httpClient ?? throw new ArgumentNullException("httpClient");
		string_0 = baseUrl ?? throw new ArgumentNullException("baseUrl");
		string_1 = apiKey ?? throw new ArgumentNullException("apiKey");
		iauthManager_0 = authManager ?? throw new ArgumentNullException("authManager");
	}

	public ApiKeyService(IAuthManager authManager, IHttpClientFactory? httpClientFactory = null, string baseUrl = "https://example.supabase.co", string? apiKey = null)
		: this(smethod_0(httpClientFactory), baseUrl, apiKey ?? smethod_1(), authManager)
	{
	}

	private static HttpClient smethod_0(IHttpClientFactory? ihttpClientFactory_0)
	{
		if (ihttpClientFactory_0 != null)
		{
			return ihttpClientFactory_0.CreateClient((string)null, 30);
		}
		return new HttpClient();
	}

	private static string smethod_1()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("SUPABASE_API_KEY");
		if (!string.IsNullOrEmpty(environmentVariable))
		{
			return environmentVariable;
		}
		return "YOUR_SUPABASE_KEY_HERE";
	}

	[AsyncStateMachine(typeof(Struct9))]
	public Task<string> GetApiKeyAsync(string keyName)
	{
		Struct9 stateMachine = default(Struct9);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.apiKeyService_0 = this;
		stateMachine.string_0 = keyName;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct12))]
	private Task<JToken> method_0(string string_5, object object_0)
	{
		Struct12 stateMachine = default(Struct12);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<JToken>.Create();
		stateMachine.apiKeyService_0 = this;
		stateMachine.string_0 = string_5;
		stateMachine.object_0 = object_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public string GetApiKey(string keyName)
	{
		try
		{
			if (!concurrentDictionary_0.TryGetValue(keyName, out string value))
			{
				try
				{
					string requestUri = string_0 + "/rest/v1/rpc/get_api_key";
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class62<string>(keyName)), Encoding.UTF8, "application/json");
					HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
					{
						Content = content
					};
					httpRequestMessage.Headers.Add("apikey", string_1);
					httpRequestMessage.Headers.Add("Authorization", "Bearer " + string_1);
					httpRequestMessage.Headers.Add("Prefer", "return=representation");
					HttpResponseMessage result = httpClient_0.SendAsync(httpRequestMessage).GetAwaiter().GetResult();
					string result2 = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
					if (!result.IsSuccessStatusCode)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[ApiKeyService] 同步 RPC 失败: ");
						defaultInterpolatedStringHandler.AppendFormatted(result.StatusCode);
						Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
						return smethod_3(keyName);
					}
					JToken? obj = smethod_2(JToken.Parse(result2));
					JObject val = (JObject)(object)((obj is JObject) ? obj : null);
					if (val != null)
					{
						string text = ((object)val["key_value"])?.ToString();
						if (!string.IsNullOrEmpty(text))
						{
							concurrentDictionary_0.TryAdd(keyName, text);
							Logger.Info("[ApiKeyService] 同步获取 Key 成功: " + keyName);
							return text;
						}
					}
					Logger.Warning("[ApiKeyService] 同步 RPC 响应格式异常或 Key 不存在: " + keyName);
					return smethod_3(keyName);
				}
				catch (Exception ex)
				{
					Logger.Warning("[ApiKeyService] 同步从数据库获取 Key 失败: " + ex.Message + "，使用默认 Key");
					return smethod_3(keyName);
				}
			}
			return value;
		}
		catch (Exception ex2)
		{
			Logger.Error("[ApiKeyService] 同步获取 Key 失败: " + ex2.Message);
			return smethod_3(keyName);
		}
	}

	private static JToken? smethod_2(JToken jtoken_0)
	{
		JArray val = (JArray)(object)((jtoken_0 is JArray) ? jtoken_0 : null);
		if (val != null)
		{
			foreach (JToken item in val)
			{
				JObject val2 = (JObject)(object)((item is JObject) ? item : null);
				if (val2 == null)
				{
					continue;
				}
				JToken obj = val2["data"];
				JArray val3 = (JArray)(object)((obj is JArray) ? obj : null);
				if (val3 == null)
				{
					continue;
				}
				foreach (JToken item2 in val3)
				{
					if (item2 is JObject && item2[(object)"key_value"] != null)
					{
						return item2;
					}
				}
			}
		}
		else
		{
			JObject val4 = (JObject)(object)((jtoken_0 is JObject) ? jtoken_0 : null);
			if (val4 != null)
			{
				JToken obj2 = val4["data"];
				JArray val5 = (JArray)(object)((obj2 is JArray) ? obj2 : null);
				if (val5 != null)
				{
					foreach (JToken item3 in val5)
					{
						if (item3 is JObject && item3[(object)"key_value"] != null)
						{
							return item3;
						}
					}
				}
			}
		}
		return null;
	}

	private static string smethod_3(string string_5)
	{
		string text = string_5.ToLower();
		if (!(text == "tianditu_token"))
		{
			if (!(text == "google_maps_api_key"))
			{
				if (!(text == "amap_api_key"))
				{
					Logger.Warning("[ApiKeyService] 未知的 Key 名称: " + string_5);
					return "UNKNOWN_KEY";
				}
				return "YOUR_AMAP_API_KEY_HERE";
			}
			return "YOUR_GOOGLE_MAPS_KEY_HERE";
		}
		return "YOUR_TIANDITU_TOKEN_HERE";
	}

	[AsyncStateMachine(typeof(Struct14))]
	public Task UpdateLastUsedAsync(string keyName)
	{
		Struct14 stateMachine = default(Struct14);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.apiKeyService_0 = this;
		stateMachine.string_0 = keyName;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct13))]
	public Task SaveApiKeyAsync(string keyName, string keyValue, string provider, string? description = null)
	{
		Struct13 stateMachine = default(Struct13);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.apiKeyService_0 = this;
		stateMachine.string_0 = keyName;
		stateMachine.string_1 = keyValue;
		stateMachine.string_2 = provider;
		stateMachine.string_3 = description;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct11))]
	public Task<string> GetTiandituTokenAsync()
	{
		Struct11 stateMachine = default(Struct11);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.apiKeyService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct10))]
	public Task<string> GetGoogleMapsApiKeyAsync()
	{
		Struct10 stateMachine = default(Struct10);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.apiKeyService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct8))]
	public Task<string> GetAmapApiKeyAsync()
	{
		Struct8 stateMachine = default(Struct8);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.apiKeyService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public void ClearCache()
	{
		concurrentDictionary_0.Clear();
		Logger.Info("[ApiKeyService] 已清除 Key 缓存");
	}

	public void ClearCache(string keyName)
	{
		concurrentDictionary_0.TryRemove(keyName, out string _);
		Logger.Info("[ApiKeyService] 已清除 Key 缓存: " + keyName);
	}
}
