using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Network;
using ns0;
using ns7;

namespace RevitAi.Core.AI.Tools;

[AITool("web_fetch", Category = "互联网搜索", Description = "获取指定网页URL的内容，提取纯文本。用于在搜索到网页后深入阅读和分析具体网页的内容。", RequiresTransaction = false, RequiresActiveDocument = false)]
public sealed class WebFetchTool : IAITool
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct223 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public WebFetchTool webFetchTool_0;

		public CancellationToken cancellationToken_0;

		private string string_0;

		private int int_1;

		private HttpClient httpClient_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			WebFetchTool webFetchTool = webFetchTool_0;
			AIToolResult result;
			try
			{
				if ((uint)num <= 1u)
				{
					goto IL_00ec;
				}
				string_0 = aitoolContext_0.GetParameter<string>("url", (string)null);
				if (!string.IsNullOrWhiteSpace(string_0))
				{
					if (!string_0.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !string_0.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
					{
						string_0 = "https://" + string_0;
					}
					int_1 = aitoolContext_0.GetParameter<int>("maxLength", 8000);
					if (int_1 < 100)
					{
						int_1 = 100;
					}
					if (int_1 > 50000)
					{
						int_1 = 50000;
					}
					httpClient_0 = webFetchTool.ihttpClientFactory_0.CreateClient((string)null, 20);
					goto IL_00ec;
				}
				result = AIToolResult.Fail("URL 不能为空");
				goto end_IL_000f;
				IL_00ec:
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
							goto IL_035b;
						}
						HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string_0)
						{
							Headers = 
							{
								{
									"User-Agent",
									"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
								},
								{
									"Accept",
									"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
								},
								{
									"Accept-Language",
									"zh-CN,zh;q=0.9,en;q=0.8"
								}
							}
						};
						awaiter2 = httpClient_0.SendAsync(request, cancellationToken_0).GetAwaiter();
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
					HttpResponseMessage result2 = awaiter2.GetResult();
					object obj;
					if (result2.IsSuccessStatusCode)
					{
						MediaTypeHeaderValue? contentType = result2.Content.Headers.ContentType;
						if (contentType == null)
						{
							obj = null;
						}
						else
						{
							obj = contentType.MediaType;
							if (obj != null)
							{
								goto IL_02ba;
							}
						}
						obj = "";
						goto IL_02ba;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[WebFetch] HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted(result2.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(": ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("无法获取网页内容: HTTP ");
					defaultInterpolatedStringHandler2.AppendFormatted((int)result2.StatusCode);
					result = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
					goto end_IL_00ec;
					IL_035b:
					string result3 = awaiter.GetResult();
					if (string.IsNullOrWhiteSpace(result3))
					{
						result = AIToolResult.Ok("网页内容为空。", (object)null);
					}
					else
					{
						string text = smethod_0(result3);
						int length = text.Length;
						bool flag = false;
						if (text.Length > int_1)
						{
							text = text.Substring(0, int_1);
							flag = true;
						}
						string text2;
						if (!flag)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("成功获取网页内容，共 ");
							defaultInterpolatedStringHandler3.AppendFormatted(length, "N0");
							defaultInterpolatedStringHandler3.AppendLiteral(" 字符。");
							text2 = defaultInterpolatedStringHandler3.ToStringAndClear();
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(25, 2);
							defaultInterpolatedStringHandler4.AppendLiteral("成功获取网页内容，原始 ");
							defaultInterpolatedStringHandler4.AppendFormatted(length, "N0");
							defaultInterpolatedStringHandler4.AppendLiteral(" 字符，已截取前 ");
							defaultInterpolatedStringHandler4.AppendFormatted(int_1, "N0");
							defaultInterpolatedStringHandler4.AppendLiteral(" 字符。");
							text2 = defaultInterpolatedStringHandler4.ToStringAndClear();
						}
						result = AIToolResult.Ok(text2, (object)new Class24<string, int, bool, string>(string_0, length, flag, text));
					}
					goto end_IL_00ec;
					IL_02ba:
					string text3 = (string)obj;
					if (text3.Contains("html") || text3.Contains("text") || text3.Contains("xml"))
					{
						awaiter = result2.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_035b;
					}
					result = AIToolResult.Fail("不支持的网页类型: " + text3 + "，仅支持 HTML/文本页面");
					end_IL_00ec:;
				}
				finally
				{
					if (num < 0 && httpClient_0 != null)
					{
						((IDisposable)httpClient_0).Dispose();
					}
				}
				end_IL_000f:;
			}
			catch (TaskCanceledException)
			{
				Logger.Warning("[WebFetch] 请求超时");
				result = AIToolResult.Fail("网页请求超时，请稍后重试或尝试其他 URL");
			}
			catch (HttpRequestException ex2)
			{
				Logger.Warning("[WebFetch] 请求异常: " + ex2.Message);
				result = AIToolResult.Fail("无法访问该网页: " + ex2.Message);
			}
			catch (Exception ex3)
			{
				Logger.Error("[WebFetch] 抓取失败", ex3);
				result = AIToolResult.Fail("抓取网页失败: " + ex3.Message);
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

	private readonly IHttpClientFactory ihttpClientFactory_0;

	public string Name => "web_fetch";

	public string Category => "互联网搜索";

	public string Description => "获取指定网页URL的内容，自动提取纯文本返回。适合在搜索到相关网页后，进一步获取和阅读网页的详细内容。";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"url\": {\n                \"type\": \"string\",\n                \"description\": \"要抓取的网页 URL 地址，例如：'https://example.com/article'\"\n            },\n            \"maxLength\": {\n                \"type\": \"number\",\n                \"description\": \"可选，返回内容的最大字符数，默认 8000，超出部分会截断并标注\"\n            }\n        },\n        \"required\": [\"url\"]\n    }";

	public WebFetchTool(IHttpClientFactory httpClientFactory)
	{
		ihttpClientFactory_0 = httpClientFactory ?? throw new ArgumentNullException("httpClientFactory");
	}

	[AsyncStateMachine(typeof(Struct223))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct223 stateMachine = default(Struct223);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.webFetchTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private static string smethod_0(string string_0)
	{
		if (string.IsNullOrEmpty(string_0))
		{
			return "";
		}
		string_0 = Regex.Replace(string_0, "<script[^>]*>[\\s\\S]*?</script>", "", RegexOptions.IgnoreCase);
		string_0 = Regex.Replace(string_0, "<style[^>]*>[\\s\\S]*?</style>", "", RegexOptions.IgnoreCase);
		string_0 = Regex.Replace(string_0, "<noscript[^>]*>[\\s\\S]*?</noscript>", "", RegexOptions.IgnoreCase);
		string_0 = Regex.Replace(string_0, "<!--[\\s\\S]*?-->", "");
		string_0 = Regex.Replace(string_0, "</?(br|hr)[^>]*/?>", "\n", RegexOptions.IgnoreCase);
		string_0 = Regex.Replace(string_0, "</?(p|div|h[1-6]|li|tr|section|article|header|footer|nav|main|aside)[^>]*>", "\n", RegexOptions.IgnoreCase);
		string_0 = Regex.Replace(string_0, "<[^>]+>", "");
		string_0 = WebUtility.HtmlDecode(string_0);
		string_0 = Regex.Replace(string_0, "[ \\t]+", " ");
		string_0 = Regex.Replace(string_0, "\\n\\s*\\n", "\n\n");
		string_0 = Regex.Replace(string_0, "^\\s+|\\s+$", "", RegexOptions.Multiline);
		string_0 = Regex.Replace(string_0, "\\n{3,}", "\n\n");
		return string_0.Trim();
	}
}
