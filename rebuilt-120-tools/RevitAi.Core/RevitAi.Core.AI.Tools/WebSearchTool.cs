using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Network;
using RevitAi.Abstractions.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ns0;
using ns7;

namespace RevitAi.Core.AI.Tools;

[AITool("web_search", Category = "互联网搜索", Description = "搜索互联网上的网页信息，返回网页标题、URL、摘要等。可用于查询最新资讯、技术文档、行业动态等任何网络信息。", RequiresTransaction = false, RequiresActiveDocument = false)]
public sealed class WebSearchTool : IAITool
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct224 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public WebSearchTool webSearchTool_0;

		public CancellationToken cancellationToken_0;

		private string string_0;

		private string string_1;

		private int int_1;

		private HttpClient httpClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<string> taskAwaiter_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			WebSearchTool webSearchTool = webSearchTool_0;
			AIToolResult result;
			try
			{
				TaskAwaiter<string> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0125;
				}
				if ((uint)(num - 1) <= 1u)
				{
					goto IL_017c;
				}
				string_0 = aitoolContext_0.GetParameter<string>("query", (string)null);
				if (!string.IsNullOrWhiteSpace(string_0))
				{
					string_1 = aitoolContext_0.GetParameter<string>("freshness", "noLimit");
					int_1 = aitoolContext_0.GetParameter<int>("count", 8);
					if (int_1 < 1)
					{
						int_1 = 8;
					}
					if (int_1 > 20)
					{
						int_1 = 20;
					}
					awaiter = webSearchTool.iapiKeyService_0.GetApiKeyAsync("bocha_api_key").GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0125;
				}
				result = AIToolResult.Fail("搜索关键词不能为空");
				goto end_IL_000f;
				IL_017c:
				Class25<string, string, bool, int> @class = default(Class25<string, string, bool, int>);
				string result2 = default(string);
				try
				{
					TaskAwaiter<HttpResponseMessage> awaiter2;
					if (num != 1)
					{
						if (num == 2)
						{
							awaiter = taskAwaiter_0;
							taskAwaiter_0 = default(TaskAwaiter<string>);
							num = -1;
							int_0 = -1;
							goto IL_02c3;
						}
						StringContent content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json");
						HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.bocha.cn/v1/web-search")
						{
							Content = content
						};
						httpRequestMessage.Headers.Add("Authorization", "Bearer " + result2);
						awaiter2 = httpClient_0.SendAsync(httpRequestMessage, cancellationToken_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<HttpResponseMessage>);
						num = -1;
						int_0 = -1;
					}
					HttpResponseMessage result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_02c3;
					IL_03cd:
					object obj;
					string text = (string)obj;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[WebSearch] API 返回错误: code=");
					int num2;
					defaultInterpolatedStringHandler.AppendFormatted(num2);
					defaultInterpolatedStringHandler.AppendLiteral(", msg=");
					defaultInterpolatedStringHandler.AppendFormatted(text);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					result = AIToolResult.Fail("搜索失败: " + text);
					goto end_IL_017c;
					IL_02c3:
					string result4 = awaiter.GetResult();
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(24, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("[WebSearch] API 请求失败: ");
						defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
						defaultInterpolatedStringHandler2.AppendLiteral(", ");
						defaultInterpolatedStringHandler2.AppendFormatted(result4);
						Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("搜索请求失败: HTTP ");
						defaultInterpolatedStringHandler3.AppendFormatted((int)httpResponseMessage_0.StatusCode);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
					}
					else
					{
						JObject val = JObject.Parse(result4);
						JToken obj2 = val["code"];
						num2 = ((obj2 != null) ? Extensions.Value<int>((IEnumerable<JToken>)obj2) : (-1));
						if (num2 != 200)
						{
							JToken obj3 = val["msg"];
							if (obj3 == null)
							{
								obj = null;
							}
							else
							{
								obj = Extensions.Value<string>((IEnumerable<JToken>)obj3);
								if (obj != null)
								{
									goto IL_03cd;
								}
							}
							obj = "未知错误";
							goto IL_03cd;
						}
						JToken obj4 = val["data"];
						JToken obj5 = ((obj4 != null) ? obj4[(object)"webPages"] : null);
						JToken obj6 = ((obj5 != null) ? obj5[(object)"value"] : null);
						JArray val2 = (JArray)(object)((obj6 is JArray) ? obj6 : null);
						int? obj7;
						if (obj5 == null)
						{
							obj7 = null;
						}
						else
						{
							JToken obj8 = obj5[(object)"totalEstimatedMatches"];
							obj7 = ((obj8 != null) ? new int?(Extensions.Value<int>((IEnumerable<JToken>)obj8)) : ((int?)null));
						}
						int? num3 = obj7;
						int valueOrDefault = num3.GetValueOrDefault();
						if (val2 != null && ((JContainer)val2).Count != 0)
						{
							List<object> list = new List<object>();
							IEnumerator<JToken> enumerator = val2.GetEnumerator();
							try
							{
								object obj10;
								object obj18;
								object obj14;
								object obj12;
								object obj16;
								object obj20;
								for (; enumerator.MoveNext(); list.Add(new Class26<string, string, string, string, string, string>((string)obj10, (string)obj18, (string)obj14, (string)obj12, (string)obj16, (string)obj20)))
								{
									JToken current = enumerator.Current;
									JToken obj9 = current[(object)"name"];
									if (obj9 == null)
									{
										obj10 = null;
									}
									else
									{
										obj10 = Extensions.Value<string>((IEnumerable<JToken>)obj9);
										if (obj10 != null)
										{
											goto IL_051d;
										}
									}
									obj10 = "";
									goto IL_051d;
									IL_0573:
									JToken obj11 = current[(object)"summary"];
									if (obj11 == null)
									{
										obj12 = null;
									}
									else
									{
										obj12 = Extensions.Value<string>((IEnumerable<JToken>)obj11);
										if (obj12 != null)
										{
											goto IL_059e;
										}
									}
									obj12 = "";
									goto IL_059e;
									IL_0548:
									JToken obj13 = current[(object)"snippet"];
									if (obj13 == null)
									{
										obj14 = null;
									}
									else
									{
										obj14 = Extensions.Value<string>((IEnumerable<JToken>)obj13);
										if (obj14 != null)
										{
											goto IL_0573;
										}
									}
									obj14 = "";
									goto IL_0573;
									IL_059e:
									JToken obj15 = current[(object)"siteName"];
									if (obj15 == null)
									{
										obj16 = null;
									}
									else
									{
										obj16 = Extensions.Value<string>((IEnumerable<JToken>)obj15);
										if (obj16 != null)
										{
											goto IL_05c9;
										}
									}
									obj16 = "";
									goto IL_05c9;
									IL_051d:
									JToken obj17 = current[(object)"url"];
									if (obj17 == null)
									{
										obj18 = null;
									}
									else
									{
										obj18 = Extensions.Value<string>((IEnumerable<JToken>)obj17);
										if (obj18 != null)
										{
											goto IL_0548;
										}
									}
									obj18 = "";
									goto IL_0548;
									IL_05c9:
									JToken obj19 = current[(object)"datePublished"];
									if (obj19 == null)
									{
										obj20 = null;
									}
									else
									{
										obj20 = Extensions.Value<string>((IEnumerable<JToken>)obj19);
										if (obj20 != null)
										{
											continue;
										}
									}
									obj20 = "";
								}
							}
							finally
							{
								if (num < 0)
								{
									enumerator?.Dispose();
								}
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(22, 2);
							defaultInterpolatedStringHandler4.AppendLiteral("搜索完成！共找到约 ");
							defaultInterpolatedStringHandler4.AppendFormatted(valueOrDefault, "N0");
							defaultInterpolatedStringHandler4.AppendLiteral(" 条结果，返回前 ");
							defaultInterpolatedStringHandler4.AppendFormatted(list.Count);
							defaultInterpolatedStringHandler4.AppendLiteral(" 条。");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class27<string, int, int, List<object>>(string_0, valueOrDefault, list.Count, list));
						}
						else
						{
							result = AIToolResult.Ok("未找到相关搜索结果。请尝试更换搜索关键词。", (object)null);
						}
					}
					end_IL_017c:;
				}
				finally
				{
					if (num < 0 && httpClient_0 != null)
					{
						((IDisposable)httpClient_0).Dispose();
					}
				}
				goto end_IL_000f;
				IL_0125:
				result2 = awaiter.GetResult();
				if (!string.IsNullOrEmpty(result2) && !(result2 == "UNKNOWN_KEY"))
				{
					@class = new Class25<string, string, bool, int>(string_0, string_1, gparam_6: true, int_1);
					httpClient_0 = webSearchTool.ihttpClientFactory_0.CreateClient((string)null, 30);
					goto IL_017c;
				}
				result = AIToolResult.Fail("博查搜索 API Key 未配置，请联系管理员在 api_keys 表中添加 bocha_api_key");
				end_IL_000f:;
			}
			catch (TaskCanceledException)
			{
				Logger.Warning("[WebSearch] 搜索请求超时");
				result = AIToolResult.Fail("搜索请求超时，请稍后重试");
			}
			catch (Exception ex2)
			{
				Logger.Error("[WebSearch] 搜索失败", ex2);
				result = AIToolResult.Fail("搜索失败: " + ex2.Message);
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

	private const string string_0 = "https://api.bocha.cn/v1/web-search";

	private readonly IApiKeyService iapiKeyService_0;

	private readonly IHttpClientFactory ihttpClientFactory_0;

	public string Name => "web_search";

	public string Category => "互联网搜索";

	public string Description => "搜索互联网上的网页信息，返回网页的标题、URL、摘要、来源等详细内容。适合查询最新资讯、技术文档、行业动态等任何网络信息。";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"query\": {\n                \"type\": \"string\",\n                \"description\": \"搜索关键词或语句，例如：'Revit 2026 新功能'、'BIM 行业发展趋势'\"\n            },\n            \"freshness\": {\n                \"type\": \"string\",\n                \"description\": \"可选，搜索时间范围：noLimit（不限）、oneDay（一天内）、oneWeek（一周内）、oneMonth（一个月内）、oneYear（一年内）。默认为 noLimit\",\n                \"enum\": [\"noLimit\", \"oneDay\", \"oneWeek\", \"oneMonth\", \"oneYear\"]\n            },\n            \"count\": {\n                \"type\": \"number\",\n                \"description\": \"可选，返回结果条数，1-20，默认 8\"\n            }\n        },\n        \"required\": [\"query\"]\n    }";

	public WebSearchTool(IApiKeyService apiKeyService, IHttpClientFactory httpClientFactory)
	{
		iapiKeyService_0 = apiKeyService ?? throw new ArgumentNullException("apiKeyService");
		ihttpClientFactory_0 = httpClientFactory ?? throw new ArgumentNullException("httpClientFactory");
	}

	[AsyncStateMachine(typeof(Struct224))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct224 stateMachine = default(Struct224);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.webSearchTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
