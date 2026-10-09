using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Network;
using RevitAi.Core.Authentication;
using RevitAi.Core.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ns0;
using ns7;

namespace RevitAi.Core.AI;

public sealed class ClaudeAIService : IDisposable, IAIService, IAIServiceEx
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct187 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			string result = default;
			try
			{
				JArray val = JArray.Parse(string_0);
				if (val != null)
				{
					JArray val2 = val;
					val2.Add((JToken)new JObject
					{
						["role"] = ((JToken)("assistant")),
						["content"] = ((JToken)("(思考过程: " + string_1 + ")"))
					});
					val2.Add((JToken)new JObject
					{
						["role"] = ((JToken)("user")),
						["content"] = ((JToken)("你刚才已经完成了思考。现在请直接执行工具调用或给出最终回复，不要再输出思考过程。"))
					});
					result = ((JToken)val).ToString((Formatting)0, Array.Empty<JsonConverter>());
					goto IL_00f7;
				}
			}
			catch
			{
			}
			result = string_0 + "\n\n【之前的思考】" + string_1 + "\n\n【现在请】基于上述思考，直接执行工具调用或给出最终回复，不要再输出思考过程。";
			goto IL_00f7;
			IL_00f7:
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
	public struct Struct188 : IAsyncStateMachine
	{
			private string result;
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public AIConfig aiconfig_0;

		public ClaudeAIService claudeAIService_0;

		private TaskAwaiter<Result<EncryptedApiKeyRecord>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			TaskAwaiter<Result<EncryptedApiKeyRecord>> awaiter;
			Result<EncryptedApiKeyRecord> result2;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			Result<EncryptedApiKeyRecord> result4;
			switch (num)
			{
			default:
				if (!string.IsNullOrEmpty(aiconfig_0.CachedApiKey))
				{
					result = aiconfig_0.CachedApiKey;
					break;
				}
				awaiter = claudeAIService.isupabaseClient_0.GetEncryptedApiKeyByNotesAsync(aiconfig_0.Provider).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_00aa;
			case 0:
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<Result<EncryptedApiKeyRecord>>);
				num = -1;
				int_0 = -1;
				goto IL_00aa;
			case 1:
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<Result<EncryptedApiKeyRecord>>);
				num = -1;
				int_0 = -1;
				goto IL_014b;
			case 2:
				{
					try
					{
						if (num != 2)
						{
							Uri uri = new Uri(aiconfig_0.ProviderEndpoint);
							string apiUrl = uri.Scheme + "://" + uri.Host;
							awaiter = claudeAIService.isupabaseClient_0.GetEncryptedApiKeyAsync(apiUrl).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 2;
								int_0 = 2;
								taskAwaiter_0 = awaiter;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = taskAwaiter_0;
							taskAwaiter_0 = default(TaskAwaiter<Result<EncryptedApiKeyRecord>>);
							num = -1;
							int_0 = -1;
						}
						Result<EncryptedApiKeyRecord> result3 = awaiter.GetResult();
						if (result3.IsSuccess && result3.Value != null)
						{
							result = claudeAIService.apiKeyManager_0.DecryptApiKey(result3.Value.EncryptedKey, MasterKeyProvider.GetMasterKey());
							break;
						}
					}
					catch (Exception ex)
					{
						Logger.Warning("[ClaudeAI] 解析 API URL 失败: " + ex.Message);
					}
					goto IL_0291;
				}
				IL_014b:
				result2 = awaiter.GetResult();
				if (result2.IsSuccess && result2.Value != null)
				{
					result = claudeAIService.apiKeyManager_0.DecryptApiKey(result2.Value.EncryptedKey, MasterKeyProvider.GetMasterKey());
					break;
				}
				if (!string.IsNullOrEmpty(aiconfig_0.ProviderEndpoint))
				{
					goto case 2;
				}
				goto IL_0291;
				IL_0291:
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] 未找到配置的 API Key (Provider: ");
				defaultInterpolatedStringHandler.AppendFormatted(aiconfig_0.Provider);
				defaultInterpolatedStringHandler.AppendLiteral(", Model: ");
				defaultInterpolatedStringHandler.AppendFormatted(aiconfig_0.ModelName);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				result = null;
				break;
				IL_00aa:
				result4 = awaiter.GetResult();
				if (result4.IsSuccess && result4.Value != null)
				{
					result = claudeAIService.apiKeyManager_0.DecryptApiKey(result4.Value.EncryptedKey, MasterKeyProvider.GetMasterKey());
					break;
				}
				awaiter = claudeAIService.isupabaseClient_0.GetEncryptedApiKeyByNotesAsync(aiconfig_0.ModelName).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_014b;
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

	[CompilerGenerated]
	public sealed class Class113 : IAsyncEnumerable<string>, IAsyncEnumerator<string>, IValueTaskSource<bool>, IAsyncStateMachine, IAsyncDisposable, IValueTaskSource
	{
		public int int_0;

		public AsyncIteratorMethodBuilder asyncIteratorMethodBuilder_0;

		public ManualResetValueTaskSourceCore<bool> manualResetValueTaskSourceCore_0;

		private string string_0;

		private bool bool_0;

		private CancellationTokenSource cancellationTokenSource_0;

		private int int_1;

		private CancellationToken cancellationToken_0;

		public CancellationToken cancellationToken_1;

		private AIConfig aiconfig_0;

		public AIConfig aiconfig_1;

		private StreamReader streamReader_0;

		public StreamReader streamReader_1;

		private TaskAwaiter<string?> taskAwaiter_0;

		string IAsyncEnumerator<string>.Current
		{
			[DebuggerHidden]
			get
			{
				return string_0;
			}
		}

		[DebuggerHidden]
		public Class113(int int_2)
		{
			asyncIteratorMethodBuilder_0 = AsyncIteratorMethodBuilder.Create();
			int_0 = int_2;
			int_1 = Environment.CurrentManagedThreadId;
		}

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			try
			{
				TaskAwaiter<string> awaiter;
				string result;
				switch (num)
				{
				case -5:
					num = -1;
					int_0 = -1;
					if (bool_0)
					{
						break;
					}
					goto IL_02dc;
				case -4:
					num = -1;
					int_0 = -1;
					if (bool_0)
					{
						break;
					}
					goto IL_02dc;
				default:
					if (bool_0)
					{
						break;
					}
					num = -1;
					int_0 = -1;
					goto IL_02dc;
				case 0:
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						goto IL_0094;
					}
					IL_0094:
					if ((result = awaiter.GetResult()) == null)
					{
						break;
					}
					if (!cancellationToken_0.IsCancellationRequested)
					{
						if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
						{
							if (result.StartsWith("data: "))
							{
								string text = result.Substring(6);
								if (!(text == "[DONE]"))
								{
									JObject val = null;
									try
									{
										val = JsonConvert.DeserializeObject<JObject>(text);
									}
									catch (JsonException)
									{
										goto IL_02dc;
									}
									if (val != null && ((object)val["type"])?.ToString() == "content_block_delta")
									{
										JToken obj = val["delta"];
										string value = ((obj == null) ? null : ((object)obj[(object)"text"])?.ToString());
										if (!string.IsNullOrEmpty(value))
										{
											string_0 = value;
											num = -4;
											int_0 = -4;
											goto IL_03a8;
										}
									}
								}
							}
						}
						else if (smethod_1(aiconfig_0.Provider) && result.StartsWith("data: "))
						{
							string text2 = result.Substring(6);
							if (!(text2 == "[DONE]"))
							{
								JObject val2 = null;
								try
								{
									val2 = JsonConvert.DeserializeObject<JObject>(text2);
								}
								catch (JsonException)
								{
									goto IL_02dc;
								}
								if (val2 != null)
								{
									string value2 = null;
									try
									{
										JToken val3 = val2["choices"];
										JArray val4 = (JArray)(object)((val3 is JArray) ? val3 : null);
										if (val4 != null && ((JContainer)val4).Count > 0)
										{
											JToken? obj2 = ((IEnumerable<JToken>)val4).FirstOrDefault();
											object obj3;
											if (obj2 == null)
											{
												obj3 = null;
											}
											else
											{
												JToken obj4 = obj2[(object)"delta"];
												obj3 = ((obj4 == null) ? null : ((object)obj4[(object)"content"])?.ToString());
											}
											value2 = (string)obj3;
										}
										else
										{
											JObject val5 = (JObject)(object)((val3 is JObject) ? val3 : null);
											if (val5 != null)
											{
												JToken obj5 = val5["delta"];
												value2 = ((obj5 == null) ? null : ((object)obj5[(object)"content"])?.ToString());
											}
										}
										if (string.IsNullOrEmpty(value2))
										{
											value2 = ((object)val2["content"])?.ToString();
										}
									}
									catch
									{
									}
									if (!string.IsNullOrEmpty(value2))
									{
										string_0 = value2;
										num = -5;
										int_0 = -5;
										goto IL_03a8;
									}
								}
							}
						}
						goto IL_02dc;
					}
					bool_0 = true;
					break;
					IL_02dc:
					string_0 = null;
					awaiter = streamReader_0.ReadLineAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						Class113 stateMachine = this;
						asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
						return;
					}
					goto IL_0094;
				}
			}
			catch (Exception exception)
			{
				int_0 = -2;
				if (cancellationTokenSource_0 != null)
				{
					cancellationTokenSource_0.Dispose();
					cancellationTokenSource_0 = null;
				}
				string_0 = null;
				asyncIteratorMethodBuilder_0.Complete();
				manualResetValueTaskSourceCore_0.SetException(exception);
				return;
			}
			int_0 = -2;
			if (cancellationTokenSource_0 != null)
			{
				cancellationTokenSource_0.Dispose();
				cancellationTokenSource_0 = null;
			}
			string_0 = null;
			asyncIteratorMethodBuilder_0.Complete();
			manualResetValueTaskSourceCore_0.SetResult(result: false);
			return;
			IL_03a8:
			manualResetValueTaskSourceCore_0.SetResult(result: true);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}

		[DebuggerHidden]
		IAsyncEnumerator<string> IAsyncEnumerable<string>.GetAsyncEnumerator(CancellationToken cancellationToken_2 = default(CancellationToken))
		{
			Class113 @class;
			if (int_0 == -2 && int_1 == Environment.CurrentManagedThreadId)
			{
				int_0 = -3;
				asyncIteratorMethodBuilder_0 = AsyncIteratorMethodBuilder.Create();
				bool_0 = false;
				@class = this;
			}
			else
			{
				@class = new Class113(-3);
			}
			@class.streamReader_0 = streamReader_1;
			@class.aiconfig_0 = aiconfig_1;
			if (cancellationToken_1.Equals(default(CancellationToken)))
			{
				@class.cancellationToken_0 = cancellationToken_2;
			}
			else if (!cancellationToken_2.Equals(cancellationToken_1) && !cancellationToken_2.Equals(default(CancellationToken)))
			{
				cancellationTokenSource_0 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken_1, cancellationToken_2);
				@class.cancellationToken_0 = cancellationTokenSource_0.Token;
			}
			else
			{
				@class.cancellationToken_0 = cancellationToken_1;
			}
			return @class;
		}

		[DebuggerHidden]
		ValueTask<bool> IAsyncEnumerator<string>.MoveNextAsync()
		{
			if (int_0 == -2)
			{
				return default(ValueTask<bool>);
			}
			manualResetValueTaskSourceCore_0.Reset();
			Class113 stateMachine = this;
			asyncIteratorMethodBuilder_0.MoveNext(ref stateMachine);
			short version = manualResetValueTaskSourceCore_0.Version;
			if (manualResetValueTaskSourceCore_0.GetStatus(version) == ValueTaskSourceStatus.Succeeded)
			{
				return new ValueTask<bool>(manualResetValueTaskSourceCore_0.GetResult(version));
			}
			return new ValueTask<bool>(this, version);
		}

		[DebuggerHidden]
		bool IValueTaskSource<bool>.GetResult(short short_0)
		{
			return manualResetValueTaskSourceCore_0.GetResult(short_0);
		}

		[DebuggerHidden]
		ValueTaskSourceStatus IValueTaskSource<bool>.GetStatus(short short_0)
		{
			return manualResetValueTaskSourceCore_0.GetStatus(short_0);
		}

		[DebuggerHidden]
		void IValueTaskSource<bool>.OnCompleted(Action<object?> action_0, object? object_0, short short_0, ValueTaskSourceOnCompletedFlags valueTaskSourceOnCompletedFlags_0)
		{
			manualResetValueTaskSourceCore_0.OnCompleted(action_0, object_0, short_0, valueTaskSourceOnCompletedFlags_0);
		}

		[DebuggerHidden]
		void IValueTaskSource.GetResult(short short_0)
		{
			manualResetValueTaskSourceCore_0.GetResult(short_0);
		}

		[DebuggerHidden]
		ValueTaskSourceStatus IValueTaskSource.GetStatus(short short_0)
		{
			return manualResetValueTaskSourceCore_0.GetStatus(short_0);
		}

		[DebuggerHidden]
		void IValueTaskSource.OnCompleted(Action<object?> action_0, object? object_0, short short_0, ValueTaskSourceOnCompletedFlags valueTaskSourceOnCompletedFlags_0)
		{
			manualResetValueTaskSourceCore_0.OnCompleted(action_0, object_0, short_0, valueTaskSourceOnCompletedFlags_0);
		}

		[DebuggerHidden]
		ValueTask IAsyncDisposable.DisposeAsync()
		{
			if (int_0 >= -1)
			{
				throw new NotSupportedException();
			}
			if (int_0 == -2)
			{
				return default(ValueTask);
			}
			bool_0 = true;
			manualResetValueTaskSourceCore_0.Reset();
			Class113 stateMachine = this;
			asyncIteratorMethodBuilder_0.MoveNext(ref stateMachine);
			return new ValueTask(this, manualResetValueTaskSourceCore_0.Version);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct189 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public AIConfig aiconfig_0;

		public ClaudeAIService claudeAIService_0;

		public string string_0;

		public string string_1;

		public CancellationToken cancellationToken_0;

		private HttpRequestMessage httpRequestMessage_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<string?> taskAwaiter_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			string result = default;
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			string result2;
			try
			{
				TaskAwaiter<string> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_016c;
					}
					if (aiconfig_0 == null)
					{
						throw new ArgumentNullException("config");
					}
					awaiter = claudeAIService.method_0(aiconfig_0).GetAwaiter();
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
				result = awaiter.GetResult();
				if (!string.IsNullOrEmpty(result))
				{
					string text = claudeAIService.method_3(string_0, aiconfig_0, string_1);
					string uriString = claudeAIService.method_1(aiconfig_0);
					if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
					{
						try
						{
							JToken.Parse(text);
						}
						catch (Exception ex)
						{
							Logger.Error("[ClaudeAI] Ollama 请求体 JSON 格式错误: " + ex.Message);
							result2 = "请求体 JSON 格式错误: " + ex.Message;
							goto end_IL_000f;
						}
					}
					httpRequestMessage_0 = new HttpRequestMessage
					{
						Method = HttpMethod.Post,
						RequestUri = new Uri(uriString),
						Content = new StringContent(text, Encoding.UTF8, "application/json")
					};
					goto IL_016c;
				}
				result2 = "抱歉，AI 服务密钥配置错误。请联系管理员。";
				goto end_IL_000f;
				IL_016c:
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
							goto IL_0266;
						}
						claudeAIService.method_2(httpRequestMessage_0, aiconfig_0, result);
						awaiter2 = claudeAIService.httpClient_0.SendAsync(httpRequestMessage_0, cancellationToken_0).GetAwaiter();
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
					goto IL_0266;
					IL_0266:
					string result4 = awaiter.GetResult();
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] API 调用失败: ");
						defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("抱歉，AI 服务暂时不可用。错误代码: ");
						defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
						defaultInterpolatedStringHandler2.AppendLiteral("\n详细信息: ");
						defaultInterpolatedStringHandler2.AppendFormatted(claudeAIService.method_14(result4));
						result2 = defaultInterpolatedStringHandler2.ToStringAndClear();
					}
					else
					{
						string text2 = claudeAIService.method_15(result4, aiconfig_0);
						List<ToolCallInfo> list = claudeAIService.method_20(result4, aiconfig_0);
						if (list.Count > 0)
						{
							result2 = "TOOL_CALLS:" + JsonConvert.SerializeObject((object)list, (Formatting)1);
						}
						else if (string.IsNullOrEmpty(text2))
						{
							Logger.Error("[ClaudeAI] 响应解析失败或内容为空");
							result2 = "抱歉，AI 服务返回了无效的响应。";
						}
						else
						{
							result2 = text2;
						}
					}
				}
				finally
				{
					if (num < 0 && httpRequestMessage_0 != null)
					{
						((IDisposable)httpRequestMessage_0).Dispose();
					}
				}
				end_IL_000f:;
			}
			catch (TimeoutException)
			{
				Logger.Error("[ClaudeAI] 请求超时");
				result2 = "抱歉，AI 响应超时。请稍后重试或简化您的问题。";
			}
			catch (TaskCanceledException)
			{
				Logger.Error("[ClaudeAI] 请求被取消（超时）");
				result2 = "抱歉，AI 响应超时。请稍后重试或简化您的问题。";
			}
			catch (Exception ex4)
			{
				Logger.Error("[ClaudeAI] 发送消息失败", ex4);
				result2 = "抱歉，我遇到了一些问题：" + ex4.Message;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct190 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ClaudeAIService claudeAIService_0;

		public string string_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<Result<AIConfig>> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			TaskAwaiter<string> awaiter;
			TaskAwaiter<Result<AIConfig>> awaiter2;
			if (num != 0)
			{
				if (num == 1)
				{
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_00f1;
				}
				awaiter2 = claudeAIService.isupabaseClient_0.GetDefaultAIConfigAsync().GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result<AIConfig>>);
				num = -1;
				int_0 = -1;
			}
			Result<AIConfig> result = awaiter2.GetResult();
			if (result.IsSuccess && result.Value != null)
			{
				awaiter = claudeAIService.SendMessageAsync(string_0, result.Value, cancellationToken_0).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_00f1;
			}
			Logger.Error("[ClaudeAI] 获取 AI 配置失败: " + result.Error);
			string result2 = "抱歉，AI 服务配置错误。请联系管理员。";
			goto IL_0122;
			IL_00f1:
			result2 = awaiter.GetResult();
			goto IL_0122;
			IL_0122:
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct191 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ClaudeAIService claudeAIService_0;

		public string string_0;

		public AIConfig aiconfig_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			TaskAwaiter<string> awaiter;
			if (num != 0)
			{
				awaiter = claudeAIService.SendMessageAsync(string_0, aiconfig_0, null, cancellationToken_0).GetAwaiter();
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
	public struct Struct192 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ClaudeAIService claudeAIService_0;

		public List<AIMessage> list_0;

		public AIConfig aiconfig_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			TaskAwaiter<string> awaiter;
			if (num != 0)
			{
				awaiter = claudeAIService.SendMessageAsync(list_0, aiconfig_0, null, cancellationToken_0).GetAwaiter();
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
	public struct Struct193 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public ClaudeAIService claudeAIService_0;

		public List<AIMessage> list_0;

		public AIConfig aiconfig_0;

		public string string_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<AIResponseWithThinking> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			TaskAwaiter<AIResponseWithThinking> awaiter;
			if (num != 0)
			{
				awaiter = claudeAIService.SendMessageWithUsageAsync(list_0, aiconfig_0, string_0, cancellationToken_0).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<AIResponseWithThinking>);
				num = -1;
				int_0 = -1;
			}
			string result = awaiter.GetResult().Content ?? string.Empty;
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[CompilerGenerated]
	public sealed class Class114 : IAsyncEnumerable<string>, IAsyncEnumerator<string>, IValueTaskSource<bool>, IAsyncStateMachine, IAsyncDisposable, IValueTaskSource
	{
		public int int_0;

		public AsyncIteratorMethodBuilder asyncIteratorMethodBuilder_0;

		public ManualResetValueTaskSourceCore<bool> manualResetValueTaskSourceCore_0;

		private string string_0;

		private bool bool_0;

		private CancellationTokenSource cancellationTokenSource_0;

		private int int_1;

		private AIConfig aiconfig_0;

		public AIConfig aiconfig_1;

		public ClaudeAIService claudeAIService_0;

		private string string_1;

		public string string_2;

		private CancellationToken cancellationToken_0;

		public CancellationToken cancellationToken_1;

		private StreamReader streamReader_0;

		private string string_3;

		private TaskAwaiter<string> taskAwaiter_0;

		private HttpRequestMessage httpRequestMessage_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_1;

		private TaskAwaiter<Stream> taskAwaiter_2;

		private IAsyncEnumerator<string> iasyncEnumerator_0;

		private object object_0;

		private int int_2;

		private ValueTaskAwaiter<bool> valueTaskAwaiter_0;

		private ValueTaskAwaiter valueTaskAwaiter_1;

		string IAsyncEnumerator<string>.Current
		{
			[DebuggerHidden]
			get
			{
				return string_0;
			}
		}

		[DebuggerHidden]
		public Class114(int int_3)
		{
			asyncIteratorMethodBuilder_0 = AsyncIteratorMethodBuilder.Create();
			int_0 = int_3;
			int_1 = Environment.CurrentManagedThreadId;
		}

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			try
			{
				string uriString = default(string);
				string content = default(string);
				string result = default(string);
				TaskAwaiter<string> awaiter2;
				ValueTaskAwaiter awaiter;
				string result5;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3;
				Stream result4;
				switch (num)
				{
				case -6:
					num = -1;
					int_0 = -1;
					if (!bool_0)
					{
						bool_0 = true;
					}
					goto end_IL_000e;
				case -5:
					num = -1;
					int_0 = -1;
					if (!bool_0)
					{
						bool_0 = true;
					}
					goto end_IL_000e;
				case -4:
					num = -1;
					int_0 = -1;
					if (!bool_0)
					{
						bool_0 = true;
					}
					goto end_IL_000e;
				default:
					if (!bool_0)
					{
						num = -1;
						int_0 = -1;
						if (aiconfig_0 == null)
						{
							throw new ArgumentNullException("config");
						}
						if (!aiconfig_0.SupportsStreaming)
						{
							string_0 = null;
							awaiter2 = claudeAIService.SendMessageAsync(string_1, aiconfig_0, cancellationToken_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter2;
								Class114 stateMachine = this;
								asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_01b7;
						}
						string_0 = null;
						awaiter2 = claudeAIService.method_0(aiconfig_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_0 = awaiter2;
							Class114 stateMachine = this;
							asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
							return;
						}
						goto IL_01f5;
					}
					goto end_IL_000e;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01b7;
				case 1:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01f5;
				case 2:
				case 3:
				case 4:
					try
					{
						if ((uint)(num - 2) > 2u)
						{
							httpRequestMessage_0 = new HttpRequestMessage
							{
								Method = HttpMethod.Post,
								RequestUri = new Uri(uriString),
								Content = new StringContent(content, Encoding.UTF8, "application/json")
							};
						}
						try
						{
							TaskAwaiter<HttpResponseMessage> awaiter5;
							TaskAwaiter<Stream> awaiter4;
							string result2;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2;
							HttpResponseMessage result3;
							switch (num)
							{
							default:
								claudeAIService.method_2(httpRequestMessage_0, aiconfig_0, result);
								string_0 = null;
								awaiter5 = claudeAIService.httpClient_0.SendAsync(httpRequestMessage_0, HttpCompletionOption.ResponseHeadersRead, cancellationToken_0).GetAwaiter();
								if (!awaiter5.IsCompleted)
								{
									num = 2;
									int_0 = 2;
									taskAwaiter_1 = awaiter5;
									Class114 stateMachine = this;
									asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
									return;
								}
								goto IL_03a5;
							case 2:
								awaiter5 = taskAwaiter_1;
								taskAwaiter_1 = default(TaskAwaiter<HttpResponseMessage>);
								num = -1;
								int_0 = -1;
								goto IL_03a5;
							case 3:
								awaiter2 = taskAwaiter_0;
								taskAwaiter_0 = default(TaskAwaiter<string>);
								num = -1;
								int_0 = -1;
								goto IL_0484;
							case 4:
								{
									awaiter4 = taskAwaiter_2;
									taskAwaiter_2 = default(TaskAwaiter<Stream>);
									num = -1;
									int_0 = -1;
									break;
								}
								IL_0484:
								result2 = awaiter2.GetResult();
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
								defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] 流式 API 调用失败: ");
								defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
								Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
								defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("抱歉，AI 服务暂时不可用。错误代码: ");
								defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
								defaultInterpolatedStringHandler2.AppendLiteral("\n详细信息: ");
								defaultInterpolatedStringHandler2.AppendFormatted(claudeAIService.method_14(result2));
								string_3 = defaultInterpolatedStringHandler2.ToStringAndClear();
								goto end_IL_030a;
								IL_03a5:
								result3 = awaiter5.GetResult();
								httpResponseMessage_0 = result3;
								if (!httpResponseMessage_0.IsSuccessStatusCode)
								{
									string_0 = null;
									awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
									if (!awaiter2.IsCompleted)
									{
										num = 3;
										int_0 = 3;
										taskAwaiter_0 = awaiter2;
										Class114 stateMachine = this;
										asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
										return;
									}
									goto IL_0484;
								}
								string_0 = null;
								awaiter4 = httpResponseMessage_0.Content.ReadAsStreamAsync().GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = 4;
									int_0 = 4;
									taskAwaiter_2 = awaiter4;
									Class114 stateMachine = this;
									asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
									return;
								}
								break;
							}
							result4 = awaiter4.GetResult();
							streamReader_0 = new StreamReader(result4);
							end_IL_030a:;
						}
						finally
						{
							if (num == -1 && httpRequestMessage_0 != null)
							{
								((IDisposable)httpRequestMessage_0).Dispose();
							}
						}
						if (!bool_0)
						{
							httpRequestMessage_0 = null;
							httpResponseMessage_0 = null;
							goto IL_05b1;
						}
					}
					catch (Exception ex)
					{
						Logger.Error("[ClaudeAI] 流式响应失败", ex);
						string_3 = "抱歉，我遇到了一些问题，请稍后再试。";
						goto IL_05b1;
					}
					goto end_IL_000e;
				case -7:
				case 5:
					try
					{
						ValueTaskAwaiter<bool> awaiter3;
						if (num != -7)
						{
							if (num != 5)
							{
								goto IL_065c;
							}
							awaiter3 = valueTaskAwaiter_0;
							valueTaskAwaiter_0 = default(ValueTaskAwaiter<bool>);
							num = -1;
							int_0 = -1;
							goto IL_06aa;
						}
						num = -1;
						int_0 = -1;
						if (!bool_0)
						{
							goto IL_065c;
						}
						goto end_IL_061c;
						IL_065c:
						string_0 = null;
						awaiter3 = iasyncEnumerator_0.MoveNextAsync().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 5;
							int_0 = 5;
							valueTaskAwaiter_0 = awaiter3;
							Class114 stateMachine = this;
							asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
							return;
						}
						goto IL_06aa;
						IL_06aa:
						if (awaiter3.GetResult())
						{
							string current = iasyncEnumerator_0.Current;
							string_0 = current;
							num = -7;
							int_0 = -7;
							goto IL_0829;
						}
						end_IL_061c:;
					}
					catch (Exception obj)
					{
						object_0 = obj;
					}
					if (iasyncEnumerator_0 == null)
					{
						break;
					}
					string_0 = null;
					awaiter = iasyncEnumerator_0.DisposeAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 6;
						int_0 = 6;
						valueTaskAwaiter_1 = awaiter;
						Class114 stateMachine = this;
						asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
						return;
					}
					goto IL_0759;
				case 6:
					{
						awaiter = valueTaskAwaiter_1;
						valueTaskAwaiter_1 = default(ValueTaskAwaiter);
						num = -1;
						int_0 = -1;
						goto IL_0759;
					}
					IL_05b1:
					if (string_3 != null)
					{
						string_0 = string_3;
						num = -6;
						int_0 = -6;
						goto IL_0829;
					}
					if (streamReader_0 != null)
					{
						iasyncEnumerator_0 = claudeAIService.method_21(streamReader_0, aiconfig_0, cancellationToken_0).GetAsyncEnumerator();
						object_0 = null;
						int_2 = 0;
						goto case -7;
					}
					goto end_IL_000e;
					IL_01b7:
					result5 = awaiter2.GetResult();
					string_0 = result5;
					num = -4;
					int_0 = -4;
					goto IL_0829;
					IL_0759:
					awaiter.GetResult();
					break;
					IL_01f5:
					result = awaiter2.GetResult();
					if (string.IsNullOrEmpty(result))
					{
						string_0 = "抱歉，AI 服务密钥配置错误。请联系管理员。";
						num = -5;
						int_0 = -5;
						goto IL_0829;
					}
					defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(24, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("[ClaudeAI] 流式请求: ");
					defaultInterpolatedStringHandler3.AppendFormatted(aiconfig_0.DisplayName);
					defaultInterpolatedStringHandler3.AppendLiteral(" (模型: ");
					defaultInterpolatedStringHandler3.AppendFormatted(aiconfig_0.ModelName);
					defaultInterpolatedStringHandler3.AppendLiteral(")");
					Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
					content = claudeAIService.method_12(string_1, aiconfig_0);
					uriString = claudeAIService.method_1(aiconfig_0);
					result4 = null;
					streamReader_0 = null;
					string_3 = null;
					goto case 2;
				}
				object obj2 = object_0;
				if (obj2 != null)
				{
					ExceptionDispatchInfo.Capture((obj2 as Exception) ?? throw (Exception)obj2).Throw();
				}
				if (!bool_0)
				{
					object_0 = null;
					iasyncEnumerator_0 = null;
					Logger.Info("[ClaudeAI] 流式响应完成");
					streamReader_0.Dispose();
				}
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				int_0 = -2;
				streamReader_0 = null;
				string_3 = null;
				httpRequestMessage_0 = null;
				httpResponseMessage_0 = null;
				iasyncEnumerator_0 = null;
				object_0 = null;
				if (cancellationTokenSource_0 != null)
				{
					cancellationTokenSource_0.Dispose();
					cancellationTokenSource_0 = null;
				}
				string_0 = null;
				asyncIteratorMethodBuilder_0.Complete();
				manualResetValueTaskSourceCore_0.SetException(exception);
				return;
			}
			int_0 = -2;
			streamReader_0 = null;
			string_3 = null;
			httpRequestMessage_0 = null;
			httpResponseMessage_0 = null;
			iasyncEnumerator_0 = null;
			object_0 = null;
			if (cancellationTokenSource_0 != null)
			{
				cancellationTokenSource_0.Dispose();
				cancellationTokenSource_0 = null;
			}
			string_0 = null;
			asyncIteratorMethodBuilder_0.Complete();
			manualResetValueTaskSourceCore_0.SetResult(result: false);
			return;
			IL_0829:
			manualResetValueTaskSourceCore_0.SetResult(result: true);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}

		[DebuggerHidden]
		IAsyncEnumerator<string> IAsyncEnumerable<string>.GetAsyncEnumerator(CancellationToken cancellationToken_2 = default(CancellationToken))
		{
			Class114 @class;
			if (int_0 == -2 && int_1 == Environment.CurrentManagedThreadId)
			{
				int_0 = -3;
				asyncIteratorMethodBuilder_0 = AsyncIteratorMethodBuilder.Create();
				bool_0 = false;
				@class = this;
			}
			else
			{
				@class = new Class114(-3)
				{
					claudeAIService_0 = claudeAIService_0
				};
			}
			@class.string_1 = string_2;
			@class.aiconfig_0 = aiconfig_1;
			if (cancellationToken_1.Equals(default(CancellationToken)))
			{
				@class.cancellationToken_0 = cancellationToken_2;
			}
			else if (!cancellationToken_2.Equals(cancellationToken_1) && !cancellationToken_2.Equals(default(CancellationToken)))
			{
				cancellationTokenSource_0 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken_1, cancellationToken_2);
				@class.cancellationToken_0 = cancellationTokenSource_0.Token;
			}
			else
			{
				@class.cancellationToken_0 = cancellationToken_1;
			}
			return @class;
		}

		[DebuggerHidden]
		ValueTask<bool> IAsyncEnumerator<string>.MoveNextAsync()
		{
			if (int_0 == -2)
			{
				return default(ValueTask<bool>);
			}
			manualResetValueTaskSourceCore_0.Reset();
			Class114 stateMachine = this;
			asyncIteratorMethodBuilder_0.MoveNext(ref stateMachine);
			short version = manualResetValueTaskSourceCore_0.Version;
			if (manualResetValueTaskSourceCore_0.GetStatus(version) == ValueTaskSourceStatus.Succeeded)
			{
				return new ValueTask<bool>(manualResetValueTaskSourceCore_0.GetResult(version));
			}
			return new ValueTask<bool>(this, version);
		}

		[DebuggerHidden]
		bool IValueTaskSource<bool>.GetResult(short short_0)
		{
			return manualResetValueTaskSourceCore_0.GetResult(short_0);
		}

		[DebuggerHidden]
		ValueTaskSourceStatus IValueTaskSource<bool>.GetStatus(short short_0)
		{
			return manualResetValueTaskSourceCore_0.GetStatus(short_0);
		}

		[DebuggerHidden]
		void IValueTaskSource<bool>.OnCompleted(Action<object?> action_0, object? object_1, short short_0, ValueTaskSourceOnCompletedFlags valueTaskSourceOnCompletedFlags_0)
		{
			manualResetValueTaskSourceCore_0.OnCompleted(action_0, object_1, short_0, valueTaskSourceOnCompletedFlags_0);
		}

		[DebuggerHidden]
		void IValueTaskSource.GetResult(short short_0)
		{
			manualResetValueTaskSourceCore_0.GetResult(short_0);
		}

		[DebuggerHidden]
		ValueTaskSourceStatus IValueTaskSource.GetStatus(short short_0)
		{
			return manualResetValueTaskSourceCore_0.GetStatus(short_0);
		}

		[DebuggerHidden]
		void IValueTaskSource.OnCompleted(Action<object?> action_0, object? object_1, short short_0, ValueTaskSourceOnCompletedFlags valueTaskSourceOnCompletedFlags_0)
		{
			manualResetValueTaskSourceCore_0.OnCompleted(action_0, object_1, short_0, valueTaskSourceOnCompletedFlags_0);
		}

		[DebuggerHidden]
		ValueTask IAsyncDisposable.DisposeAsync()
		{
			if (int_0 >= -1)
			{
				throw new NotSupportedException();
			}
			if (int_0 == -2)
			{
				return default(ValueTask);
			}
			bool_0 = true;
			manualResetValueTaskSourceCore_0.Reset();
			Class114 stateMachine = this;
			asyncIteratorMethodBuilder_0.MoveNext(ref stateMachine);
			return new ValueTask(this, manualResetValueTaskSourceCore_0.Version);
		}
	}

	[CompilerGenerated]
	public sealed class Class115 : IAsyncEnumerable<string>, IAsyncEnumerator<string>, IValueTaskSource<bool>, IAsyncStateMachine, IAsyncDisposable, IValueTaskSource
	{
		public int int_0;

		public AsyncIteratorMethodBuilder asyncIteratorMethodBuilder_0;

		public ManualResetValueTaskSourceCore<bool> manualResetValueTaskSourceCore_0;

		private string string_0;

		private bool bool_0;

		private CancellationTokenSource cancellationTokenSource_0;

		private int int_1;

		private AIConfig aiconfig_0;

		public AIConfig aiconfig_1;

		public ClaudeAIService claudeAIService_0;

		private string string_1;

		public string string_2;

		private string string_3;

		public string string_4;

		private CancellationToken cancellationToken_0;

		public CancellationToken cancellationToken_1;

		private StreamReader streamReader_0;

		private string string_5;

		private TaskAwaiter<string> taskAwaiter_0;

		private HttpRequestMessage httpRequestMessage_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_1;

		private TaskAwaiter<Stream> taskAwaiter_2;

		private IAsyncEnumerator<string> iasyncEnumerator_0;

		private object object_0;

		private int int_2;

		private ValueTaskAwaiter<bool> valueTaskAwaiter_0;

		private ValueTaskAwaiter valueTaskAwaiter_1;

		string IAsyncEnumerator<string>.Current
		{
			[DebuggerHidden]
			get
			{
				return string_0;
			}
		}

		[DebuggerHidden]
		public Class115(int int_3)
		{
			asyncIteratorMethodBuilder_0 = AsyncIteratorMethodBuilder.Create();
			int_0 = int_3;
			int_1 = Environment.CurrentManagedThreadId;
		}

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			try
			{
				string uriString = default(string);
				string content = default(string);
				string result = default(string);
				TaskAwaiter<string> awaiter2;
				ValueTaskAwaiter awaiter;
				string result5;
				Stream result4;
				switch (num)
				{
				case -6:
					num = -1;
					int_0 = -1;
					if (!bool_0)
					{
						bool_0 = true;
					}
					goto end_IL_000e;
				case -5:
					num = -1;
					int_0 = -1;
					if (!bool_0)
					{
						bool_0 = true;
					}
					goto end_IL_000e;
				case -4:
					num = -1;
					int_0 = -1;
					if (!bool_0)
					{
						bool_0 = true;
					}
					goto end_IL_000e;
				default:
					if (!bool_0)
					{
						num = -1;
						int_0 = -1;
						if (aiconfig_0 == null)
						{
							throw new ArgumentNullException("config");
						}
						if (!aiconfig_0.SupportsStreaming)
						{
							string_0 = null;
							awaiter2 = claudeAIService.SendMessageAsync(string_1, aiconfig_0, string_3, cancellationToken_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter2;
								Class115 stateMachine = this;
								asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_01bd;
						}
						string_0 = null;
						awaiter2 = claudeAIService.method_0(aiconfig_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_0 = awaiter2;
							Class115 stateMachine = this;
							asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
							return;
						}
						goto IL_01fb;
					}
					goto end_IL_000e;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01bd;
				case 1:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01fb;
				case 2:
				case 3:
				case 4:
					try
					{
						if ((uint)(num - 2) > 2u)
						{
							httpRequestMessage_0 = new HttpRequestMessage
							{
								Method = HttpMethod.Post,
								RequestUri = new Uri(uriString),
								Content = new StringContent(content, Encoding.UTF8, "application/json")
							};
						}
						try
						{
							TaskAwaiter<HttpResponseMessage> awaiter5;
							TaskAwaiter<Stream> awaiter4;
							string result2;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2;
							HttpResponseMessage result3;
							switch (num)
							{
							default:
								claudeAIService.method_2(httpRequestMessage_0, aiconfig_0, result);
								Logger.Info("[ClaudeAI] 发送流式请求到 " + aiconfig_0.Provider + " API（含对话历史）");
								string_0 = null;
								awaiter5 = claudeAIService.httpClient_0.SendAsync(httpRequestMessage_0, HttpCompletionOption.ResponseHeadersRead, cancellationToken_0).GetAwaiter();
								if (!awaiter5.IsCompleted)
								{
									num = 2;
									int_0 = 2;
									taskAwaiter_1 = awaiter5;
									Class115 stateMachine = this;
									asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
									return;
								}
								goto IL_036d;
							case 2:
								awaiter5 = taskAwaiter_1;
								taskAwaiter_1 = default(TaskAwaiter<HttpResponseMessage>);
								num = -1;
								int_0 = -1;
								goto IL_036d;
							case 3:
								awaiter2 = taskAwaiter_0;
								taskAwaiter_0 = default(TaskAwaiter<string>);
								num = -1;
								int_0 = -1;
								goto IL_044c;
							case 4:
								{
									awaiter4 = taskAwaiter_2;
									taskAwaiter_2 = default(TaskAwaiter<Stream>);
									num = -1;
									int_0 = -1;
									break;
								}
								IL_044c:
								result2 = awaiter2.GetResult();
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] API 调用失败: ");
								defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
								defaultInterpolatedStringHandler.AppendLiteral(" - ");
								defaultInterpolatedStringHandler.AppendFormatted(result2);
								Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
								defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("抱歉，AI 服务暂时不可用。错误代码: ");
								defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
								defaultInterpolatedStringHandler2.AppendLiteral("\n详细信息: ");
								defaultInterpolatedStringHandler2.AppendFormatted(claudeAIService.method_14(result2));
								string_5 = defaultInterpolatedStringHandler2.ToStringAndClear();
								goto end_IL_02a9;
								IL_036d:
								result3 = awaiter5.GetResult();
								httpResponseMessage_0 = result3;
								if (!httpResponseMessage_0.IsSuccessStatusCode)
								{
									string_0 = null;
									awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
									if (!awaiter2.IsCompleted)
									{
										num = 3;
										int_0 = 3;
										taskAwaiter_0 = awaiter2;
										Class115 stateMachine = this;
										asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
										return;
									}
									goto IL_044c;
								}
								string_0 = null;
								awaiter4 = httpResponseMessage_0.Content.ReadAsStreamAsync().GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = 4;
									int_0 = 4;
									taskAwaiter_2 = awaiter4;
									Class115 stateMachine = this;
									asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
									return;
								}
								break;
							}
							result4 = awaiter4.GetResult();
							streamReader_0 = new StreamReader(result4);
							end_IL_02a9:;
						}
						finally
						{
							if (num == -1 && httpRequestMessage_0 != null)
							{
								((IDisposable)httpRequestMessage_0).Dispose();
							}
						}
						if (!bool_0)
						{
							httpRequestMessage_0 = null;
							httpResponseMessage_0 = null;
							goto IL_0593;
						}
					}
					catch (Exception ex)
					{
						Logger.Error("[ClaudeAI] 流式响应失败", ex);
						string_5 = "抱歉，我遇到了一些问题，请稍后再试。";
						goto IL_0593;
					}
					goto end_IL_000e;
				case -7:
				case 5:
					try
					{
						ValueTaskAwaiter<bool> awaiter3;
						if (num != -7)
						{
							if (num != 5)
							{
								goto IL_063e;
							}
							awaiter3 = valueTaskAwaiter_0;
							valueTaskAwaiter_0 = default(ValueTaskAwaiter<bool>);
							num = -1;
							int_0 = -1;
							goto IL_068c;
						}
						num = -1;
						int_0 = -1;
						if (!bool_0)
						{
							goto IL_063e;
						}
						goto end_IL_05fe;
						IL_063e:
						string_0 = null;
						awaiter3 = iasyncEnumerator_0.MoveNextAsync().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 5;
							int_0 = 5;
							valueTaskAwaiter_0 = awaiter3;
							Class115 stateMachine = this;
							asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
							return;
						}
						goto IL_068c;
						IL_068c:
						if (awaiter3.GetResult())
						{
							string current = iasyncEnumerator_0.Current;
							string_0 = current;
							num = -7;
							int_0 = -7;
							goto IL_07fc;
						}
						end_IL_05fe:;
					}
					catch (Exception obj)
					{
						object_0 = obj;
					}
					if (iasyncEnumerator_0 == null)
					{
						break;
					}
					string_0 = null;
					awaiter = iasyncEnumerator_0.DisposeAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 6;
						int_0 = 6;
						valueTaskAwaiter_1 = awaiter;
						Class115 stateMachine = this;
						asyncIteratorMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
						return;
					}
					goto IL_073b;
				case 6:
					{
						awaiter = valueTaskAwaiter_1;
						valueTaskAwaiter_1 = default(ValueTaskAwaiter);
						num = -1;
						int_0 = -1;
						goto IL_073b;
					}
					IL_0593:
					if (string_5 != null)
					{
						string_0 = string_5;
						num = -6;
						int_0 = -6;
						goto IL_07fc;
					}
					if (streamReader_0 != null)
					{
						iasyncEnumerator_0 = claudeAIService.method_21(streamReader_0, aiconfig_0, cancellationToken_0).GetAsyncEnumerator();
						object_0 = null;
						int_2 = 0;
						goto case -7;
					}
					goto end_IL_000e;
					IL_01bd:
					result5 = awaiter2.GetResult();
					string_0 = result5;
					num = -4;
					int_0 = -4;
					goto IL_07fc;
					IL_073b:
					awaiter.GetResult();
					break;
					IL_01fb:
					result = awaiter2.GetResult();
					if (string.IsNullOrEmpty(result))
					{
						string_0 = "抱歉，AI 服务密钥配置错误。请联系管理员。";
						num = -5;
						int_0 = -5;
						goto IL_07fc;
					}
					content = claudeAIService.method_11(string_1, aiconfig_0, string_3);
					uriString = claudeAIService.method_1(aiconfig_0);
					result4 = null;
					streamReader_0 = null;
					string_5 = null;
					goto case 2;
				}
				object obj2 = object_0;
				if (obj2 != null)
				{
					ExceptionDispatchInfo.Capture((obj2 as Exception) ?? throw (Exception)obj2).Throw();
				}
				if (!bool_0)
				{
					object_0 = null;
					iasyncEnumerator_0 = null;
					streamReader_0.Dispose();
				}
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				int_0 = -2;
				streamReader_0 = null;
				string_5 = null;
				httpRequestMessage_0 = null;
				httpResponseMessage_0 = null;
				iasyncEnumerator_0 = null;
				object_0 = null;
				if (cancellationTokenSource_0 != null)
				{
					cancellationTokenSource_0.Dispose();
					cancellationTokenSource_0 = null;
				}
				string_0 = null;
				asyncIteratorMethodBuilder_0.Complete();
				manualResetValueTaskSourceCore_0.SetException(exception);
				return;
			}
			int_0 = -2;
			streamReader_0 = null;
			string_5 = null;
			httpRequestMessage_0 = null;
			httpResponseMessage_0 = null;
			iasyncEnumerator_0 = null;
			object_0 = null;
			if (cancellationTokenSource_0 != null)
			{
				cancellationTokenSource_0.Dispose();
				cancellationTokenSource_0 = null;
			}
			string_0 = null;
			asyncIteratorMethodBuilder_0.Complete();
			manualResetValueTaskSourceCore_0.SetResult(result: false);
			return;
			IL_07fc:
			manualResetValueTaskSourceCore_0.SetResult(result: true);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}

		[DebuggerHidden]
		IAsyncEnumerator<string> IAsyncEnumerable<string>.GetAsyncEnumerator(CancellationToken cancellationToken_2 = default(CancellationToken))
		{
			Class115 @class;
			if (int_0 == -2 && int_1 == Environment.CurrentManagedThreadId)
			{
				int_0 = -3;
				asyncIteratorMethodBuilder_0 = AsyncIteratorMethodBuilder.Create();
				bool_0 = false;
				@class = this;
			}
			else
			{
				@class = new Class115(-3)
				{
					claudeAIService_0 = claudeAIService_0
				};
			}
			@class.string_1 = string_2;
			@class.aiconfig_0 = aiconfig_1;
			@class.string_3 = string_4;
			if (cancellationToken_1.Equals(default(CancellationToken)))
			{
				@class.cancellationToken_0 = cancellationToken_2;
			}
			else if (!cancellationToken_2.Equals(cancellationToken_1) && !cancellationToken_2.Equals(default(CancellationToken)))
			{
				cancellationTokenSource_0 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken_1, cancellationToken_2);
				@class.cancellationToken_0 = cancellationTokenSource_0.Token;
			}
			else
			{
				@class.cancellationToken_0 = cancellationToken_1;
			}
			return @class;
		}

		[DebuggerHidden]
		ValueTask<bool> IAsyncEnumerator<string>.MoveNextAsync()
		{
			if (int_0 == -2)
			{
				return default(ValueTask<bool>);
			}
			manualResetValueTaskSourceCore_0.Reset();
			Class115 stateMachine = this;
			asyncIteratorMethodBuilder_0.MoveNext(ref stateMachine);
			short version = manualResetValueTaskSourceCore_0.Version;
			if (manualResetValueTaskSourceCore_0.GetStatus(version) == ValueTaskSourceStatus.Succeeded)
			{
				return new ValueTask<bool>(manualResetValueTaskSourceCore_0.GetResult(version));
			}
			return new ValueTask<bool>(this, version);
		}

		[DebuggerHidden]
		bool IValueTaskSource<bool>.GetResult(short short_0)
		{
			return manualResetValueTaskSourceCore_0.GetResult(short_0);
		}

		[DebuggerHidden]
		ValueTaskSourceStatus IValueTaskSource<bool>.GetStatus(short short_0)
		{
			return manualResetValueTaskSourceCore_0.GetStatus(short_0);
		}

		[DebuggerHidden]
		void IValueTaskSource<bool>.OnCompleted(Action<object?> action_0, object? object_1, short short_0, ValueTaskSourceOnCompletedFlags valueTaskSourceOnCompletedFlags_0)
		{
			manualResetValueTaskSourceCore_0.OnCompleted(action_0, object_1, short_0, valueTaskSourceOnCompletedFlags_0);
		}

		[DebuggerHidden]
		void IValueTaskSource.GetResult(short short_0)
		{
			manualResetValueTaskSourceCore_0.GetResult(short_0);
		}

		[DebuggerHidden]
		ValueTaskSourceStatus IValueTaskSource.GetStatus(short short_0)
		{
			return manualResetValueTaskSourceCore_0.GetStatus(short_0);
		}

		[DebuggerHidden]
		void IValueTaskSource.OnCompleted(Action<object?> action_0, object? object_1, short short_0, ValueTaskSourceOnCompletedFlags valueTaskSourceOnCompletedFlags_0)
		{
			manualResetValueTaskSourceCore_0.OnCompleted(action_0, object_1, short_0, valueTaskSourceOnCompletedFlags_0);
		}

		[DebuggerHidden]
		ValueTask IAsyncDisposable.DisposeAsync()
		{
			if (int_0 >= -1)
			{
				throw new NotSupportedException();
			}
			if (int_0 == -2)
			{
				return default(ValueTask);
			}
			bool_0 = true;
			manualResetValueTaskSourceCore_0.Reset();
			Class115 stateMachine = this;
			asyncIteratorMethodBuilder_0.MoveNext(ref stateMachine);
			return new ValueTask(this, manualResetValueTaskSourceCore_0.Version);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct194 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIResponseWithThinking> asyncTaskMethodBuilder_0;

		public AIConfig aiconfig_0;

		public ClaudeAIService claudeAIService_0;

		public string string_0;

		public string string_1;

		public List<FileAttachment> list_0;

		public CancellationToken cancellationToken_0;

		private AIResponseWithThinking airesponseWithThinking_0;

		private HttpRequestMessage httpRequestMessage_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<string?> taskAwaiter_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_1;

		private TaskAwaiter<AIResponseWithThinking> taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			string result = default;
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			if ((uint)num > 4u)
			{
				airesponseWithThinking_0 = new AIResponseWithThinking();
			}
			AIResponseWithThinking result2;
			try
			{
				TaskAwaiter<string> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 3u)
					{
						goto IL_0201;
					}
					if (aiconfig_0 == null)
					{
						throw new ArgumentNullException("config");
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] 使用配置: ");
					defaultInterpolatedStringHandler.AppendFormatted(aiconfig_0.DisplayName);
					defaultInterpolatedStringHandler.AppendLiteral(" (模型: ");
					defaultInterpolatedStringHandler.AppendFormatted(aiconfig_0.ModelName);
					defaultInterpolatedStringHandler.AppendLiteral(")");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					awaiter = claudeAIService.method_0(aiconfig_0).GetAwaiter();
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
				result = awaiter.GetResult();
				if (!string.IsNullOrEmpty(result))
				{
					string text = claudeAIService.method_3(string_0, aiconfig_0, string_1, list_0);
					string uriString = claudeAIService.method_1(aiconfig_0);
					if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
					{
						try
						{
							JToken.Parse(text);
						}
						catch (Exception ex)
						{
							Logger.Error("[ClaudeAI] Ollama 请求体 JSON 格式错误: " + ex.Message);
							airesponseWithThinking_0.Content = "请求体 JSON 格式错误: " + ex.Message;
							result2 = airesponseWithThinking_0;
							goto end_IL_001e;
						}
					}
					httpRequestMessage_0 = new HttpRequestMessage
					{
						Method = HttpMethod.Post,
						RequestUri = new Uri(uriString),
						Content = new StringContent(text, Encoding.UTF8, "application/json")
					};
					goto IL_0201;
				}
				airesponseWithThinking_0.Content = "抱歉，AI 服务密钥配置错误。请联系管理员。";
				result2 = airesponseWithThinking_0;
				goto end_IL_001e;
				IL_0201:
				try
				{
					TaskAwaiter<HttpResponseMessage> awaiter3;
					TaskAwaiter<AIResponseWithThinking> awaiter2;
					string result3;
					HttpResponseMessage result4;
					string result5;
					switch (num)
					{
					default:
						claudeAIService.method_2(httpRequestMessage_0, aiconfig_0, result);
						awaiter3 = claudeAIService.httpClient_0.SendAsync(httpRequestMessage_0, cancellationToken_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter3;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
							return;
						}
						goto IL_0294;
					case 1:
						awaiter3 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<HttpResponseMessage>);
						num = -1;
						int_0 = -1;
						goto IL_0294;
					case 2:
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						goto IL_0306;
					case 3:
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						goto IL_0569;
					case 4:
						{
							awaiter2 = taskAwaiter_2;
							taskAwaiter_2 = default(TaskAwaiter<AIResponseWithThinking>);
							num = -1;
							int_0 = -1;
							break;
						}
						IL_0306:
						result3 = awaiter.GetResult();
						if (!httpResponseMessage_0.IsSuccessStatusCode)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(21, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("[ClaudeAI] API 调用失败: ");
							defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
							Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
							AIResponseWithThinking aIResponseWithThinking = airesponseWithThinking_0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(27, 2);
							defaultInterpolatedStringHandler3.AppendLiteral("抱歉，AI 服务暂时不可用。错误代码: ");
							defaultInterpolatedStringHandler3.AppendFormatted(httpResponseMessage_0.StatusCode);
							defaultInterpolatedStringHandler3.AppendLiteral("\n详细信息: ");
							defaultInterpolatedStringHandler3.AppendFormatted(claudeAIService.method_14(result3));
							aIResponseWithThinking.Content = defaultInterpolatedStringHandler3.ToStringAndClear();
							result2 = airesponseWithThinking_0;
						}
						else
						{
							airesponseWithThinking_0 = claudeAIService.method_13(result3, aiconfig_0);
							List<ToolCallInfo> list = claudeAIService.method_20(result3, aiconfig_0);
							if (list.Count > 0)
							{
								airesponseWithThinking_0.ToolCalls = list.Select((ToolCallInfo toolCallInfo_0) => new ToolCallInfo
								{
									ToolName = toolCallInfo_0.ToolName,
									Parameters = toolCallInfo_0.Parameters,
									CallId = toolCallInfo_0.CallId
								}).ToList();
							}
							if (string.IsNullOrEmpty(airesponseWithThinking_0.Content) && airesponseWithThinking_0.ToolCalls == null)
							{
								if (!string.IsNullOrEmpty(airesponseWithThinking_0.ReasoningContent))
								{
									Logger.Warning("[ClaudeAI] content 为空但有 reasoning_content");
									awaiter = claudeAIService.method_22(string_0, airesponseWithThinking_0.ReasoningContent).GetAwaiter();
									if (!awaiter.IsCompleted)
									{
										num = 3;
										int_0 = 3;
										taskAwaiter_0 = awaiter;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
										return;
									}
									goto IL_0569;
								}
								Logger.Error("[ClaudeAI] 响应解析失败或内容为空");
								Logger.Error("[ClaudeAI] Provider: " + aiconfig_0.Provider + ", Model: " + aiconfig_0.ModelName);
								Logger.Error("[ClaudeAI] 原始响应（前 500 字符）: " + result3.Substring(0, Math.Min(500, result3.Length)));
								airesponseWithThinking_0.Content = "抱歉，AI 服务返回了无效的响应。可能是模型返回了空响应或格式不匹配。";
							}
							result2 = airesponseWithThinking_0;
						}
						goto end_IL_0201;
						IL_0294:
						result4 = awaiter3.GetResult();
						httpResponseMessage_0 = result4;
						awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0306;
						IL_0569:
						result5 = awaiter.GetResult();
						awaiter2 = claudeAIService.SendMessageWithUsageAsync(result5, aiconfig_0, string_1, list_0, cancellationToken_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 4;
							int_0 = 4;
							taskAwaiter_2 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						break;
					}
					result2 = awaiter2.GetResult();
					end_IL_0201:;
				}
				finally
				{
					if (num < 0 && httpRequestMessage_0 != null)
					{
						((IDisposable)httpRequestMessage_0).Dispose();
					}
				}
				end_IL_001e:;
			}
			catch (TimeoutException)
			{
				Logger.Error("[ClaudeAI] 请求超时");
				airesponseWithThinking_0.Content = "抱歉，AI 响应超时。请稍后重试或简化您的问题。";
				result2 = airesponseWithThinking_0;
			}
			catch (TaskCanceledException)
			{
				Logger.Error("[ClaudeAI] 请求被取消（超时）");
				airesponseWithThinking_0.Content = "抱歉，AI 响应超时。请稍后重试或简化您的问题。";
				result2 = airesponseWithThinking_0;
			}
			catch (Exception ex4)
			{
				Logger.Error("[ClaudeAI] 发送消息失败", ex4);
				airesponseWithThinking_0.Content = "抱歉，我遇到了一些问题：" + ex4.Message;
				result2 = airesponseWithThinking_0;
			}
			int_0 = -2;
			airesponseWithThinking_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct195 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIResponseWithThinking> asyncTaskMethodBuilder_0;

		public AIConfig aiconfig_0;

		public List<AIMessage> list_0;

		public ClaudeAIService claudeAIService_0;

		public string string_0;

		public CancellationToken cancellationToken_0;

		private HttpRequestMessage httpRequestMessage_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<string?> taskAwaiter_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			string result = default;
			int num = int_0;
			ClaudeAIService claudeAIService = claudeAIService_0;
			AIResponseWithThinking result2;
			try
			{
				TaskAwaiter<string> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_01f4;
					}
					if (aiconfig_0 == null)
					{
						throw new ArgumentNullException("config");
					}
					if (list_0 == null || list_0.Count == 0)
					{
						throw new ArgumentNullException("messages");
					}
					awaiter = claudeAIService.method_0(aiconfig_0).GetAwaiter();
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
				result = awaiter.GetResult();
				if (string.IsNullOrEmpty(result))
				{
					result2 = new AIResponseWithThinking
					{
						Content = "抱歉，AI 服务密钥配置错误。请联系管理员。"
					};
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] 开始构建请求体，消息数量: ");
					defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					string text = claudeAIService.method_5(list_0, aiconfig_0, string_0);
					int byteCount = Encoding.UTF8.GetByteCount(text);
					if (byteCount <= 50331648L)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("[ClaudeAI] 请求体构建完成，大小: ");
						defaultInterpolatedStringHandler2.AppendFormatted(byteCount / 1024, "F2");
						defaultInterpolatedStringHandler2.AppendLiteral(" KB");
						Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
						string uriString = claudeAIService.method_1(aiconfig_0);
						httpRequestMessage_0 = new HttpRequestMessage
						{
							Method = HttpMethod.Post,
							RequestUri = new Uri(uriString),
							Content = new StringContent(text, Encoding.UTF8, "application/json")
						};
						goto IL_01f4;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(30, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("[ClaudeAI] 请求体过大: ");
					defaultInterpolatedStringHandler3.AppendFormatted(byteCount / 1024 / 1024, "F2");
					defaultInterpolatedStringHandler3.AppendLiteral(" MB，超过限制 ");
					defaultInterpolatedStringHandler3.AppendFormatted(48L);
					defaultInterpolatedStringHandler3.AppendLiteral(" MB");
					Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
					AIResponseWithThinking aIResponseWithThinking = new AIResponseWithThinking();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(42, 2);
					defaultInterpolatedStringHandler4.AppendLiteral("抱歉，请求内容过大（");
					defaultInterpolatedStringHandler4.AppendFormatted((double)byteCount / 1024.0 / 1024.0, "F2");
					defaultInterpolatedStringHandler4.AppendLiteral(" MB）。请减少附件数量或缩小图片尺寸，确保总大小小于 ");
					defaultInterpolatedStringHandler4.AppendFormatted(48L);
					defaultInterpolatedStringHandler4.AppendLiteral(" MB。");
					aIResponseWithThinking.Content = defaultInterpolatedStringHandler4.ToStringAndClear();
					result2 = aIResponseWithThinking;
				}
				goto end_IL_000f;
				IL_01f4:
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
							goto IL_02ee;
						}
						claudeAIService.method_2(httpRequestMessage_0, aiconfig_0, result);
						awaiter2 = claudeAIService.httpClient_0.SendAsync(httpRequestMessage_0, cancellationToken_0).GetAwaiter();
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
					goto IL_02ee;
					IL_02ee:
					string result4 = awaiter.GetResult();
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(21, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("[ClaudeAI] API 调用失败: ");
						defaultInterpolatedStringHandler5.AppendFormatted(httpResponseMessage_0.StatusCode);
						Logger.Error(defaultInterpolatedStringHandler5.ToStringAndClear());
						AIResponseWithThinking aIResponseWithThinking2 = new AIResponseWithThinking();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(27, 2);
						defaultInterpolatedStringHandler6.AppendLiteral("抱歉，AI 服务暂时不可用。错误代码: ");
						defaultInterpolatedStringHandler6.AppendFormatted(httpResponseMessage_0.StatusCode);
						defaultInterpolatedStringHandler6.AppendLiteral("\n详细信息: ");
						defaultInterpolatedStringHandler6.AppendFormatted(claudeAIService.method_14(result4));
						aIResponseWithThinking2.Content = defaultInterpolatedStringHandler6.ToStringAndClear();
						result2 = aIResponseWithThinking2;
					}
					else
					{
						AIResponseWithThinking aIResponseWithThinking3 = claudeAIService.method_13(result4, aiconfig_0);
						List<ToolCallInfo> list = claudeAIService.method_20(result4, aiconfig_0);
						if (list.Count > 0)
						{
							string value = string.Join(", ", list.Select((ToolCallInfo toolCallInfo_0) => toolCallInfo_0.ToolName));
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(23, 2);
							defaultInterpolatedStringHandler7.AppendLiteral("[ClaudeAI] 检测到 ");
							defaultInterpolatedStringHandler7.AppendFormatted(list.Count);
							defaultInterpolatedStringHandler7.AppendLiteral(" 个工具调用: ");
							defaultInterpolatedStringHandler7.AppendFormatted(value);
							Logger.Info(defaultInterpolatedStringHandler7.ToStringAndClear());
							aIResponseWithThinking3.ToolCalls = list.Select((ToolCallInfo toolCallInfo_0) => new ToolCallInfo
							{
								ToolName = toolCallInfo_0.ToolName,
								Parameters = toolCallInfo_0.Parameters,
								CallId = toolCallInfo_0.CallId
							}).ToList();
						}
						result2 = aIResponseWithThinking3;
					}
				}
				finally
				{
					if (num < 0 && httpRequestMessage_0 != null)
					{
						((IDisposable)httpRequestMessage_0).Dispose();
					}
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[ClaudeAI] 发送消息异常: " + ex.Message);
				result2 = new AIResponseWithThinking
				{
					Content = "抱歉，AI 服务发生异常: " + ex.Message
				};
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly ApiKeyManager apiKeyManager_0;

	private readonly ISupabaseClient isupabaseClient_0;

	private readonly HttpClient httpClient_0;

	private static IHttpClientFactory? ihttpClientFactory_0;

	public ClaudeAIService(ApiKeyManager apiKeyManager, ISupabaseClient supabase, IHttpClientFactory? httpClientFactory = null)
	{
		apiKeyManager_0 = apiKeyManager ?? throw new ArgumentNullException("apiKeyManager");
		isupabaseClient_0 = supabase ?? throw new ArgumentNullException("supabase");
		ihttpClientFactory_0 = httpClientFactory;
		if (ihttpClientFactory_0 != null)
		{
			httpClient_0 = ihttpClientFactory_0.CreateClient((string)null, 300);
			return;
		}
		httpClient_0 = new HttpClient
		{
			Timeout = TimeSpan.FromMinutes(5L)
		};
	}

	[AsyncStateMachine(typeof(Struct190))]
	public Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct190 stateMachine = default(Struct190);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.claudeAIService_0 = this;
		stateMachine.string_0 = message;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct191))]
	public Task<string> SendMessageAsync(string message, AIConfig config, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct191 stateMachine = default(Struct191);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.claudeAIService_0 = this;
		stateMachine.string_0 = message;
		stateMachine.aiconfig_0 = config;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct192))]
	public Task<string> SendMessageAsync(List<AIMessage> messages, AIConfig config, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct192 stateMachine = default(Struct192);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.claudeAIService_0 = this;
		stateMachine.list_0 = messages;
		stateMachine.aiconfig_0 = config;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct193))]
	public Task<string> SendMessageAsync(List<AIMessage> messages, AIConfig config, string? toolsDefinition, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct193 stateMachine = default(Struct193);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.claudeAIService_0 = this;
		stateMachine.list_0 = messages;
		stateMachine.aiconfig_0 = config;
		stateMachine.string_0 = toolsDefinition;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct195))]
	public Task<AIResponseWithThinking> SendMessageWithUsageAsync(List<AIMessage> messages, AIConfig config, string? toolsDefinition = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct195 stateMachine = default(Struct195);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIResponseWithThinking>.Create();
		stateMachine.claudeAIService_0 = this;
		stateMachine.list_0 = messages;
		stateMachine.aiconfig_0 = config;
		stateMachine.string_0 = toolsDefinition;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct189))]
	public Task<string> SendMessageAsync(string message, AIConfig config, string? toolsDefinition, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct189 stateMachine = default(Struct189);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.claudeAIService_0 = this;
		stateMachine.string_0 = message;
		stateMachine.aiconfig_0 = config;
		stateMachine.string_1 = toolsDefinition;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct194))]
	public Task<AIResponseWithThinking> SendMessageWithUsageAsync(string message, AIConfig config, string? toolsDefinition = null, List<FileAttachment>? attachments = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct194 stateMachine = default(Struct194);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIResponseWithThinking>.Create();
		stateMachine.claudeAIService_0 = this;
		stateMachine.string_0 = message;
		stateMachine.aiconfig_0 = config;
		stateMachine.string_1 = toolsDefinition;
		stateMachine.list_0 = attachments;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncIteratorStateMachine(typeof(Class114))]
	public IAsyncEnumerable<string> SendMessageStreamAsync(string message, AIConfig config, [EnumeratorCancellation] CancellationToken cancellationToken = default(CancellationToken))
	{
		return new Class114(-2)
		{
			claudeAIService_0 = this,
			string_2 = message,
			aiconfig_1 = config,
			cancellationToken_1 = cancellationToken
		};
	}

	[AsyncIteratorStateMachine(typeof(Class115))]
	public IAsyncEnumerable<string> SendMessageStreamAsync(string conversationHistory, AIConfig config, string? toolsDefinition = null, [EnumeratorCancellation] CancellationToken cancellationToken = default(CancellationToken))
	{
		return new Class115(-2)
		{
			claudeAIService_0 = this,
			string_2 = conversationHistory,
			aiconfig_1 = config,
			string_4 = toolsDefinition,
			cancellationToken_1 = cancellationToken
		};
	}

	[AsyncStateMachine(typeof(Struct188))]
	private Task<string?> method_0(AIConfig aiconfig_0)
	{
		Struct188 stateMachine = default(Struct188);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.claudeAIService_0 = this;
		stateMachine.aiconfig_0 = aiconfig_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private string method_1(AIConfig aiconfig_0)
	{
		string text = aiconfig_0.ProviderEndpoint ?? "https://api.anthropic.com/v1/messages";
		if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
		{
			if (!text.EndsWith("/messages", StringComparison.OrdinalIgnoreCase))
			{
				text = text.TrimEnd('/') + "/messages";
			}
		}
		else if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
		{
			if (!text.Contains("/api/chat"))
			{
				text = text.TrimEnd('/') + "/api/chat";
			}
		}
		else if (smethod_1(aiconfig_0.Provider) && !text.Contains("/chat/completions"))
		{
			text = text.TrimEnd('/') + "/chat/completions";
		}
		return text;
	}

	private void method_2(HttpRequestMessage httpRequestMessage_0, AIConfig aiconfig_0, string string_0)
	{
		if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		if (smethod_0(aiconfig_0.Provider))
		{
			if (!string.IsNullOrEmpty(string_0))
			{
				httpRequestMessage_0.Headers.Add("Authorization", "Bearer " + string_0);
			}
		}
		else if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
		{
			httpRequestMessage_0.Headers.Add("x-api-key", string_0);
			httpRequestMessage_0.Headers.Add("anthropic-version", "2023-06-01");
		}
		else if (aiconfig_0.Provider.Equals("openai", StringComparison.OrdinalIgnoreCase))
		{
			httpRequestMessage_0.Headers.Add("Authorization", "Bearer " + string_0);
		}
		else
		{
			httpRequestMessage_0.Headers.Add("Authorization", "Bearer " + string_0);
		}
	}

	private string method_3(string string_0, AIConfig aiconfig_0, string? string_1 = null, List<FileAttachment>? list_0 = null)
	{
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Expected O, but got Unknown
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Expected O, but got Unknown
		object obj;
		try
		{
			obj = JArray.Parse(string_0);
		}
		catch
		{
			obj = new Class0<string, string>[1]
			{
				new Class0<string, string>("user", string_0)
			};
		}
		if (list_0 != null && list_0.Count > 0)
		{
			obj = method_4(obj, string_0, aiconfig_0, list_0);
		}
		if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
		{
			JArray val = null;
			if (!string.IsNullOrEmpty(string_1))
			{
				JArray obj3 = JArray.Parse(string_1);
				val = new JArray();
				foreach (JToken item in obj3)
				{
					JToken val2 = item[(object)"function"];
					if (val2 != null)
					{
						val.Add((JToken)new JObject
						{
							["name"] = val2[(object)"name"],
							["description"] = val2[(object)"description"],
							["input_schema"] = val2[(object)"parameters"]
						});
					}
				}
			}
			return JsonConvert.SerializeObject((object)new Class1<string, int, double, object, JArray>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, obj, val), new JsonSerializerSettings
			{
				ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
				NullValueHandling = (NullValueHandling)1,
				Formatting = (Formatting)0
			});
		}
		if (smethod_1(aiconfig_0.Provider))
		{
			if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
			{
				return JsonConvert.SerializeObject((object)new Class2<string, bool, object, JArray>(aiconfig_0.ModelName, gparam_5: false, obj, (!string.IsNullOrEmpty(string_1)) ? JArray.Parse(string_1) : null), new JsonSerializerSettings
				{
					NullValueHandling = (NullValueHandling)1,
					Formatting = (Formatting)0
				});
			}
			Class1<string, int, double, object, JArray> @class = new Class1<string, int, double, object, JArray>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, obj, (!string.IsNullOrEmpty(string_1)) ? JArray.Parse(string_1) : null);
			if (!aiconfig_0.Provider.Equals("deepseek", StringComparison.OrdinalIgnoreCase) && !aiconfig_0.Provider.Equals("vllm", StringComparison.OrdinalIgnoreCase))
			{
				return JsonConvert.SerializeObject((object)@class, new JsonSerializerSettings
				{
					ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
					NullValueHandling = (NullValueHandling)1,
					Formatting = (Formatting)0
				});
			}
			JObject obj4 = JObject.FromObject((object)new Class1<string, int, double, object, JArray>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, obj, (!string.IsNullOrEmpty(string_1)) ? JArray.Parse(string_1) : null));
			obj4["thinking"] = (JToken)new JObject { ["type"] = ((JToken)("enabled")) };
			return ((JToken)obj4).ToString((Formatting)0, Array.Empty<JsonConverter>());
		}
		throw new NotSupportedException("不支持的 AI 供应商: " + aiconfig_0.Provider);
	}

	private object method_4(object object_0, string string_0, AIConfig aiconfig_0, List<FileAttachment> list_0)
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Invalid comparison between Unknown and I4
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_0265: Expected O, but got Unknown
		try
		{
			if (aiconfig_0.FileCapability == null)
			{
				aiconfig_0.FileCapability = PredefinedFileCapabilities.GetCapability(aiconfig_0.Provider, aiconfig_0.SupportsVision);
			}
			JObject val = null;
			string text = string_0;
			JArray val2 = (JArray)((object_0 is JArray) ? object_0 : null);
			if (val2 != null && ((JContainer)val2).Count > 0)
			{
				int num = ((JContainer)val2).Count - 1;
				while (num >= 0)
				{
					JToken obj = val2[num];
					JObject val3 = (JObject)(object)((obj is JObject) ? obj : null);
					if (val3 == null || !(((object)val3["role"])?.ToString() == "user"))
					{
						num--;
						continue;
					}
					val = val3;
					break;
				}
				if (val == null)
				{
					Logger.Warning("[ClaudeAI] 对话历史中未找到 user 消息，无法注入附件，按纯文本发送");
					return object_0;
				}
				JToken val4 = val["content"];
				if (val4 != null && (int)val4.Type == 8)
				{
					text = ((object)val4).ToString();
				}
			}
			JArray val5 = new JArray();
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = text + "\n\n(重要：随本条消息提供的附件内容仅在当前这一轮可见，之后的步骤你将无法再次查看。请务必先完整阅读全部附件并提取关键信息——如尺寸、文字标注、数量、结论等；若这些信息在后续步骤中还会用到，请调用 save_memory 工具保存，或明确写入你的分析说明中，然后再执行其他操作。)";
				val5.Add((JToken)new JObject
				{
					["type"] = ((JToken)("text")),
					["text"] = ((JToken)(text2))
				});
			}
			int num2 = 0;
			foreach (FileAttachment item in list_0)
			{
				object obj2 = method_6(item, aiconfig_0);
				if (obj2 != null)
				{
					val5.Add(JToken.FromObject(obj2));
					num2++;
				}
			}
			if (num2 == 0)
			{
				Logger.Warning("[ClaudeAI] 所有附件处理失败，按纯文本消息发送");
				return object_0;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] 已注入 ");
			defaultInterpolatedStringHandler.AppendFormatted(num2);
			defaultInterpolatedStringHandler.AppendLiteral("/");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个附件到最后一条 user 消息");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			if (val != null)
			{
				val["content"] = (JToken)(object)val5;
				return object_0;
			}
			JArray val6 = new JArray();
			val6.Add((JToken)new JObject
			{
				["role"] = ((JToken)("user")),
				["content"] = (JToken)(object)val5
			});
			return (object)val6;
		}
		catch (Exception ex)
		{
			Logger.Error("[ClaudeAI] 注入附件失败: " + ex.Message, ex);
			return object_0;
		}
	}

	private string method_5(List<AIMessage> list_0, AIConfig aiconfig_0, string? string_0 = null)
	{
		if (aiconfig_0.FileCapability == null)
		{
			aiconfig_0.FileCapability = PredefinedFileCapabilities.GetCapability(aiconfig_0.Provider, aiconfig_0.SupportsVision);
		}
		List<object> list = new List<object>();
		foreach (AIMessage item in list_0)
		{
			if (item.Attachments != null && item.Attachments.Any())
			{
				List<object> list2 = new List<object>();
				if (!string.IsNullOrEmpty(item.Content))
				{
					list2.Add(new Class3<string, string>("text", item.Content));
				}
				foreach (FileAttachment attachment in item.Attachments)
				{
					object obj = method_6(attachment, aiconfig_0);
					if (obj != null)
					{
						list2.Add(obj);
					}
				}
				list.Add(new Class0<string, List<object>>(item.Role, list2));
			}
			else
			{
				list.Add(new Class0<string, string>(item.Role, item.Content));
			}
		}
		return method_9(list, aiconfig_0, string_0);
	}

	private object? method_6(FileAttachment fileAttachment_0, AIConfig aiconfig_0)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Invalid comparison between Unknown and I4
		AIProviderFileCapability val = aiconfig_0.FileCapability ?? PredefinedFileCapabilities.GetCapability(aiconfig_0.Provider, aiconfig_0.SupportsVision);
		FileTypeCapability capability = val.GetCapability(fileAttachment_0.FileType);
		if ((int)capability.CapabilityType == 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] 厂商 ");
			defaultInterpolatedStringHandler.AppendFormatted(aiconfig_0.Provider);
			defaultInterpolatedStringHandler.AppendLiteral(" 不支持文件类型: ");
			defaultInterpolatedStringHandler.AppendFormatted<FileAttachmentType>(fileAttachment_0.FileType);
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			return null;
		}
		try
		{
			FileCapabilityType capabilityType = capability.CapabilityType;
			return ((int)capabilityType == 1) ? method_7(fileAttachment_0, val.ContentFormat, capability.ApiContentType) : (((int)capabilityType == 3) ? method_8(fileAttachment_0) : null);
		}
		catch (Exception ex)
		{
			Logger.Error("[ClaudeAI] 处理附件失败: " + fileAttachment_0.FileName + ", 错误: " + ex.Message);
			return null;
		}
	}

	private object method_7(FileAttachment fileAttachment_0, ContentFormatType contentFormatType_0, string? string_0)
	{
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Invalid comparison between Unknown and I4
		try
		{
			if (!File.Exists(fileAttachment_0.FilePath))
			{
				Logger.Error("[ClaudeAI] 附件文件不存在: " + fileAttachment_0.FilePath);
				throw new FileNotFoundException("附件文件不存在: " + fileAttachment_0.FilePath);
			}
			FileInfo fileInfo = new FileInfo(fileAttachment_0.FilePath);
			if (fileInfo.Length > 33554432L)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 3);
				defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] 附件文件过大: ");
				defaultInterpolatedStringHandler.AppendFormatted(fileAttachment_0.FileName);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted(fileInfo.Length);
				defaultInterpolatedStringHandler.AppendLiteral(" 字节)，超过限制 ");
				defaultInterpolatedStringHandler.AppendFormatted(33554432L);
				defaultInterpolatedStringHandler.AppendLiteral(" 字节");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(21, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("附件文件过大 (");
				defaultInterpolatedStringHandler2.AppendFormatted(fileInfo.Length / 1024L / 1024L, "F2");
				defaultInterpolatedStringHandler2.AppendLiteral(" MB)，超过限制 ");
				defaultInterpolatedStringHandler2.AppendFormatted(32L);
				defaultInterpolatedStringHandler2.AppendLiteral(" MB");
				throw new InvalidOperationException(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(22, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("[ClaudeAI] 开始读取附件: ");
			defaultInterpolatedStringHandler3.AppendFormatted(fileAttachment_0.FileName);
			defaultInterpolatedStringHandler3.AppendLiteral(" (");
			defaultInterpolatedStringHandler3.AppendFormatted(fileAttachment_0.FileSizeDisplay);
			defaultInterpolatedStringHandler3.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			string text = Convert.ToBase64String(File.ReadAllBytes(fileAttachment_0.FilePath));
			string gparam_ = "data:" + fileAttachment_0.MimeType + ";base64," + text;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(43, 3);
			defaultInterpolatedStringHandler4.AppendLiteral("[ClaudeAI] 成功添加 Base64 附件: ");
			defaultInterpolatedStringHandler4.AppendFormatted(fileAttachment_0.FileName);
			defaultInterpolatedStringHandler4.AppendLiteral(" (");
			defaultInterpolatedStringHandler4.AppendFormatted(fileAttachment_0.FileSizeDisplay);
			defaultInterpolatedStringHandler4.AppendLiteral("), Base64 长度: ");
			defaultInterpolatedStringHandler4.AppendFormatted(text.Length);
			Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
			if ((int)contentFormatType_0 == 1)
			{
				return new Class4<string, Class5<string, string, string>>(string_0 ?? "image", new Class5<string, string, string>("base64", fileAttachment_0.MimeType, text));
			}
			return new Class6<string, Class7<string>>(string_0 ?? "image_url", new Class7<string>(gparam_));
		}
		catch (Exception ex)
		{
			Logger.Error("[ClaudeAI] 处理附件失败: " + fileAttachment_0.FileName + ", 错误: " + ex.Message, ex);
			throw new InvalidOperationException("处理附件 " + fileAttachment_0.FileName + " 失败: " + ex.Message, ex);
		}
	}

	private object method_8(FileAttachment fileAttachment_0)
	{
		string gparam_ = File.ReadAllText(fileAttachment_0.FilePath);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
		defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] 添加文本附件: ");
		defaultInterpolatedStringHandler.AppendFormatted(fileAttachment_0.FileName);
		defaultInterpolatedStringHandler.AppendLiteral(" (");
		defaultInterpolatedStringHandler.AppendFormatted(fileAttachment_0.FileSizeDisplay);
		defaultInterpolatedStringHandler.AppendLiteral(")");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		return new Class3<string, string>("text", gparam_);
	}

	private string method_9(List<object> list_0, AIConfig aiconfig_0, string? string_0)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
		{
			JArray gparam_ = null;
			if (!string.IsNullOrEmpty(string_0))
			{
				JArray jarray_ = JArray.Parse(string_0);
				gparam_ = method_10(jarray_);
			}
			return JsonConvert.SerializeObject((object)new Class1<string, int, double, List<object>, JArray>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, list_0, gparam_), new JsonSerializerSettings
			{
				ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
				NullValueHandling = (NullValueHandling)1,
				Formatting = (Formatting)0
			});
		}
		if (smethod_1(aiconfig_0.Provider))
		{
			object obj;
			if (aiconfig_0.Provider.Equals("deepseek", StringComparison.OrdinalIgnoreCase))
			{
				obj = new Class8<string, int, double, List<object>, JArray, bool, bool>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, list_0, (!string.IsNullOrEmpty(string_0)) ? JArray.Parse(string_0) : null, gparam_12: true, gparam_13: false);
				return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
				{
					ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
					NullValueHandling = (NullValueHandling)1,
					Formatting = (Formatting)0
				});
			}
			if (aiconfig_0.Provider.Equals("vllm", StringComparison.OrdinalIgnoreCase))
			{
				JObject obj2 = JObject.FromObject((object)new Class1<string, int, double, List<object>, JArray>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, list_0, (!string.IsNullOrEmpty(string_0)) ? JArray.Parse(string_0) : null));
				obj2["thinking"] = (JToken)new JObject { ["type"] = ((JToken)("enabled")) };
				return ((JToken)obj2).ToString((Formatting)0, Array.Empty<JsonConverter>());
			}
			obj = ((!aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase)) ? ((object)new Class1<string, int, double, List<object>, JArray>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, list_0, (!string.IsNullOrEmpty(string_0)) ? JArray.Parse(string_0) : null)) : ((object)new Class2<string, bool, List<object>, JArray>(aiconfig_0.ModelName, gparam_5: false, list_0, (!string.IsNullOrEmpty(string_0)) ? JArray.Parse(string_0) : null)));
			return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
			{
				ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
				NullValueHandling = (NullValueHandling)1,
				Formatting = (Formatting)0
			});
		}
		throw new NotSupportedException("不支持的 AI 供应商: " + aiconfig_0.Provider);
	}

	private JArray method_10(JArray jarray_0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		JArray val = new JArray();
		foreach (JToken item in jarray_0)
		{
			JToken val2 = item[(object)"function"];
			if (val2 != null)
			{
				val.Add((JToken)new JObject
				{
					["name"] = val2[(object)"name"],
					["description"] = val2[(object)"description"],
					["input_schema"] = val2[(object)"parameters"]
				});
			}
		}
		return val;
	}

	private string method_11(string string_0, AIConfig aiconfig_0, string? string_1 = null)
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Expected O, but got Unknown
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		object obj;
		try
		{
			obj = JArray.Parse(string_0);
		}
		catch
		{
			obj = new Class0<string, string>[1]
			{
				new Class0<string, string>("user", string_0)
			};
		}
		if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
		{
			JArray val = null;
			if (!string.IsNullOrEmpty(string_1))
			{
				JArray obj3 = JArray.Parse(string_1);
				val = new JArray();
				foreach (JToken item in obj3)
				{
					JToken val2 = item[(object)"function"];
					if (val2 != null)
					{
						val.Add((JToken)new JObject
						{
							["name"] = val2[(object)"name"],
							["description"] = val2[(object)"description"],
							["input_schema"] = val2[(object)"parameters"]
						});
					}
				}
			}
			return JsonConvert.SerializeObject((object)new Class9<string, int, double, bool, object, JArray>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, gparam_9: true, obj, val), new JsonSerializerSettings
			{
				ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
				NullValueHandling = (NullValueHandling)1
			});
		}
		if (smethod_1(aiconfig_0.Provider))
		{
			if (aiconfig_0.Provider.Equals("deepseek", StringComparison.OrdinalIgnoreCase))
			{
				return JsonConvert.SerializeObject((object)new Class10<string, int, double, bool, object, JArray, bool>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, gparam_10: true, obj, (!string.IsNullOrEmpty(string_1)) ? JArray.Parse(string_1) : null, gparam_13: true), new JsonSerializerSettings
				{
					ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
					NullValueHandling = (NullValueHandling)1,
					Formatting = (Formatting)0
				});
			}
			if (aiconfig_0.Provider.Equals("vllm", StringComparison.OrdinalIgnoreCase))
			{
				JObject obj4 = JObject.FromObject((object)new Class9<string, int, double, bool, object, JArray>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, gparam_9: true, obj, (!string.IsNullOrEmpty(string_1)) ? JArray.Parse(string_1) : null));
				obj4["thinking"] = (JToken)new JObject { ["type"] = ((JToken)("enabled")) };
				return ((JToken)obj4).ToString((Formatting)0, Array.Empty<JsonConverter>());
			}
			return JsonConvert.SerializeObject((object)new Class9<string, int, double, bool, object, JArray>(aiconfig_0.ModelName, aiconfig_0.MaxTokens, aiconfig_0.Temperature, gparam_9: true, obj, (!string.IsNullOrEmpty(string_1)) ? JArray.Parse(string_1) : null), new JsonSerializerSettings
			{
				ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
				NullValueHandling = (NullValueHandling)1
			});
		}
		throw new NotSupportedException("不支持的 AI 供应商: " + aiconfig_0.Provider);
	}

	private string method_12(string string_0, AIConfig aiconfig_0)
	{
		return method_11(string_0, aiconfig_0);
	}

	private AIResponseWithThinking method_13(string string_0, AIConfig aiconfig_0)
	{
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		JObject val = JsonConvert.DeserializeObject<JObject>(string_0);
		AIResponseWithThinking aIResponseWithThinking = new AIResponseWithThinking();
		bool flag = false;
		if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
		{
			JToken val2 = ((val != null) ? val["usage"] : null);
			AIResponseWithThinking aIResponseWithThinking2 = aIResponseWithThinking;
			int? obj;
			if (val2 == null)
			{
				obj = null;
			}
			else
			{
				JToken obj2 = val2[(object)"input_tokens"];
				obj = ((obj2 != null) ? new int?(obj2.ToObject<int>()) : ((int?)null));
			}
			int? num = obj;
			aIResponseWithThinking2.InputTokens = num.GetValueOrDefault();
			AIResponseWithThinking aIResponseWithThinking3 = aIResponseWithThinking;
			int? obj3;
			if (val2 == null)
			{
				obj3 = null;
			}
			else
			{
				JToken obj4 = val2[(object)"output_tokens"];
				obj3 = ((obj4 != null) ? new int?(obj4.ToObject<int>()) : ((int?)null));
			}
			num = obj3;
			aIResponseWithThinking3.OutputTokens = num.GetValueOrDefault();
			if (!(flag = aIResponseWithThinking.InputTokens > 0 || aIResponseWithThinking.OutputTokens > 0))
			{
			}
		}
		else if (smethod_1(aiconfig_0.Provider))
		{
			JToken val3 = ((val != null) ? val["usage"] : null);
			if (val3 != null)
			{
				aIResponseWithThinking.InputTokens = smethod_2(val3, new string[2]
				{
					"prompt_tokens",
					"input_tokens"
				}).GetValueOrDefault();
				aIResponseWithThinking.OutputTokens = smethod_2(val3, new string[2]
				{
					"completion_tokens",
					"output_tokens"
				}).GetValueOrDefault();
				aIResponseWithThinking.CachedTokens = smethod_2(val3, new string[3]
				{
					"prompt_cache_hit_tokens",
					"cached_tokens",
					"cache_read_tokens"
				}).GetValueOrDefault();
				if (aIResponseWithThinking.CachedTokens == 0)
				{
					JToken val4 = ((val3 != null) ? val3[(object)"prompt_tokens_details"] : null);
					if (val4 != null)
					{
						aIResponseWithThinking.CachedTokens = smethod_2(val4, new string[2]
						{
							"cached_tokens",
							"cache_hit_tokens"
						}).GetValueOrDefault();
					}
				}
				if (!(flag = aIResponseWithThinking.InputTokens > 0 || aIResponseWithThinking.OutputTokens > 0))
				{
				}
			}
			else
			{
				Logger.Warning("[ClaudeAI] 响应中没有 usage 字段，提供商: " + aiconfig_0.Provider);
			}
		}
		if (!flag || (aIResponseWithThinking.InputTokens == 0 && aIResponseWithThinking.OutputTokens == 0))
		{
			Logger.Warning("[ClaudeAI] Token 解析失败，使用兜底策略估算");
			aIResponseWithThinking = method_23(aIResponseWithThinking, val);
		}
		if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
		{
			JToken obj5 = ((val != null) ? val["content"] : null);
			foreach (JToken item in (JArray)(((object)((obj5 is JArray) ? obj5 : null)) ?? ((object)new JArray())))
			{
				string text = ((object)item[(object)"type"])?.ToString();
				if (text == "thinking")
				{
					AIResponseWithThinking aIResponseWithThinking4 = aIResponseWithThinking;
					aIResponseWithThinking4.ReasoningContent = aIResponseWithThinking4.ReasoningContent + ((object)item[(object)"thinking"])?.ToString() + "\n";
				}
				else if (text == "text")
				{
					aIResponseWithThinking.Content = ((object)item[(object)"text"])?.ToString();
				}
			}
		}
		else if (smethod_1(aiconfig_0.Provider))
		{
			if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
			{
				object obj6;
				if (val == null)
				{
					obj6 = null;
				}
				else
				{
					JToken obj7 = val["message"];
					obj6 = ((obj7 == null) ? null : ((object)obj7[(object)"thinking"])?.ToString());
				}
				string text2 = (string)obj6;
				if (!string.IsNullOrEmpty(text2))
				{
					aIResponseWithThinking.ReasoningContent = text2;
				}
				AIResponseWithThinking aIResponseWithThinking5 = aIResponseWithThinking;
				object content;
				if (val == null)
				{
					content = null;
				}
				else
				{
					JToken obj8 = val["message"];
					content = ((obj8 == null) ? null : ((object)obj8[(object)"content"])?.ToString());
				}
				aIResponseWithThinking5.Content = (string?)content;
			}
			else if (smethod_0(aiconfig_0.Provider))
			{
				try
				{
					JToken val5 = ((val != null) ? val["choices"] : null);
					JArray val6 = (JArray)(object)((val5 is JArray) ? val5 : null);
					if (val6 != null && ((JContainer)val6).Count > 0)
					{
						JToken val7 = ((IEnumerable<JToken>)val6).FirstOrDefault();
						if (val7 != null)
						{
							JToken val8 = val7[(object)"message"];
							aIResponseWithThinking.Content = ((val8 == null) ? null : ((object)val8[(object)"content"])?.ToString());
							string text3 = ((val8 == null) ? null : ((object)val8[(object)"reasoning_content"])?.ToString());
							if (!string.IsNullOrEmpty(text3))
							{
								aIResponseWithThinking.ReasoningContent = text3;
							}
						}
					}
					else
					{
						JObject val9 = (JObject)(object)((val5 is JObject) ? val5 : null);
						if (val9 != null)
						{
							JToken val10 = val9["message"];
							aIResponseWithThinking.Content = ((val10 == null) ? null : ((object)val10[(object)"content"])?.ToString());
							string text4 = ((val10 == null) ? null : ((object)val10[(object)"reasoning_content"])?.ToString());
							if (!string.IsNullOrEmpty(text4))
							{
								aIResponseWithThinking.ReasoningContent = text4;
							}
						}
					}
					if (string.IsNullOrEmpty(aIResponseWithThinking.Content))
					{
						aIResponseWithThinking.Content = ((val == null) ? null : ((object)val["content"])?.ToString());
					}
					if (string.IsNullOrEmpty(aIResponseWithThinking.ReasoningContent))
					{
						string text5 = ((val == null) ? null : ((object)val["reasoning_content"])?.ToString());
						if (!string.IsNullOrEmpty(text5))
						{
							aIResponseWithThinking.ReasoningContent = text5;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error("[ClaudeAI] 解析 vLLM 响应时出错: " + ex.Message);
					Logger.Error("[ClaudeAI] vLLM 原始响应（前 500 字符）: " + string_0.Substring(0, Math.Min(500, string_0.Length)));
				}
			}
			else
			{
				try
				{
					JToken val11 = ((val != null) ? val["choices"] : null);
					JArray val12 = (JArray)(object)((val11 is JArray) ? val11 : null);
					if (val12 != null && ((JContainer)val12).Count > 0)
					{
						JToken val13 = ((IEnumerable<JToken>)val12).FirstOrDefault();
						if (val13 != null)
						{
							AIResponseWithThinking aIResponseWithThinking6 = aIResponseWithThinking;
							JToken obj9 = val13[(object)"message"];
							aIResponseWithThinking6.Content = ((obj9 == null) ? null : ((object)obj9[(object)"content"])?.ToString());
							JToken obj10 = val13[(object)"message"];
							string text6 = ((obj10 == null) ? null : ((object)obj10[(object)"reasoning_content"])?.ToString());
							if (!string.IsNullOrEmpty(text6))
							{
								aIResponseWithThinking.ReasoningContent = text6;
							}
						}
					}
					else
					{
						JObject val14 = (JObject)(object)((val11 is JObject) ? val11 : null);
						if (val14 != null)
						{
							AIResponseWithThinking aIResponseWithThinking7 = aIResponseWithThinking;
							JToken obj11 = val14["message"];
							aIResponseWithThinking7.Content = ((obj11 == null) ? null : ((object)obj11[(object)"content"])?.ToString());
							JToken obj12 = val14["message"];
							string text7 = ((obj12 == null) ? null : ((object)obj12[(object)"reasoning_content"])?.ToString());
							if (!string.IsNullOrEmpty(text7))
							{
								aIResponseWithThinking.ReasoningContent = text7;
							}
						}
					}
				}
				catch
				{
				}
				if (string.IsNullOrEmpty(aIResponseWithThinking.ReasoningContent))
				{
					string text8 = ((val == null) ? null : ((object)val["reasoning_content"])?.ToString());
					if (!string.IsNullOrEmpty(text8))
					{
						aIResponseWithThinking.ReasoningContent = text8;
					}
				}
				if (string.IsNullOrEmpty(aIResponseWithThinking.Content))
				{
					aIResponseWithThinking.Content = ((val == null) ? null : ((object)val["content"])?.ToString());
				}
			}
		}
		return aIResponseWithThinking;
	}

	private string method_14(string string_0)
	{
		if (string.IsNullOrWhiteSpace(string_0))
		{
			return "未知错误";
		}
		try
		{
			JObject val = JsonConvert.DeserializeObject<JObject>(string_0);
			object obj;
			if (val == null)
			{
				obj = null;
			}
			else
			{
				JToken obj2 = val["error"];
				obj = ((obj2 != null) ? obj2[(object)"message"] : null);
			}
			if (obj != null)
			{
				JToken obj3 = val["error"];
				string text = ((obj3 == null) ? null : ((object)obj3[(object)"message"])?.ToString());
				JToken obj4 = val["error"];
				string text2 = ((obj4 == null) ? null : ((object)obj4[(object)"code"])?.ToString());
				JToken obj5 = val["error"];
				string text3 = ((obj5 == null) ? null : ((object)obj5[(object)"type"])?.ToString());
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(text3))
				{
					list.Add("类型: " + text3);
				}
				if (!string.IsNullOrEmpty(text2))
				{
					list.Add("代码: " + text2);
				}
				if (!string.IsNullOrEmpty(text))
				{
					list.Add("消息: " + text);
				}
				return (list.Count > 0) ? string.Join(", ", list) : (text ?? "未知错误");
			}
			if (((val != null) ? val["error_msg"] : null) != null)
			{
				string text4 = ((object)val["error_code"])?.ToString();
				string text5 = ((object)val["error_msg"])?.ToString();
				if (!string.IsNullOrEmpty(text5))
				{
					return (!string.IsNullOrEmpty(text4)) ? ("代码: " + text4 + ", 消息: " + text5) : text5;
				}
			}
			if (((val != null) ? val["message"] : null) != null && ((val != null) ? val["code"] : null) != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
				defaultInterpolatedStringHandler.AppendLiteral("代码: ");
				defaultInterpolatedStringHandler.AppendFormatted<JToken>(val["code"]);
				defaultInterpolatedStringHandler.AppendLiteral(", 消息: ");
				defaultInterpolatedStringHandler.AppendFormatted<JToken>(val["message"]);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			return (string_0.Length > 200) ? (string_0.Substring(0, 200) + "...") : string_0;
		}
		catch (Exception)
		{
			return (string_0.Length > 200) ? (string_0.Substring(0, 200) + "...") : string_0;
		}
	}

	private string? method_15(string string_0, AIConfig aiconfig_0)
	{
		return method_13(string_0, aiconfig_0).Content;
	}

	private bool method_16(string string_0, AIConfig aiconfig_0)
	{
		try
		{
			JObject val = JsonConvert.DeserializeObject<JObject>(string_0);
			if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
			{
				JToken obj = ((val != null) ? val["content"] : null);
				return ((IEnumerable<JToken>)((obj is JArray) ? obj : null))?.Any((JToken jtoken_0) => ((object)jtoken_0[(object)"type"])?.ToString() == "thinking") ?? false;
			}
			if (smethod_1(aiconfig_0.Provider))
			{
				if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
				{
					object obj2;
					if (val == null)
					{
						obj2 = null;
					}
					else
					{
						JToken obj3 = val["message"];
						obj2 = ((obj3 != null) ? obj3[(object)"thinking"] : null);
					}
					return obj2 != null;
				}
				return ((val != null) ? val["reasoning_content"] : null) != null;
			}
		}
		catch
		{
		}
		return false;
	}

	private string method_17(string string_0, AIConfig aiconfig_0)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			JObject val = JsonConvert.DeserializeObject<JObject>(string_0);
			if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
			{
				JToken obj = ((val != null) ? val["content"] : null);
				object obj2 = ((obj is JArray) ? obj : null);
				StringBuilder stringBuilder = new StringBuilder();
				if (obj2 == null)
				{
					obj2 = (object)new JArray();
				}
				foreach (JToken item in (JArray)obj2)
				{
					if (((object)item[(object)"type"])?.ToString() == "thinking")
					{
						stringBuilder.AppendLine(((object)item[(object)"thinking"])?.ToString());
					}
				}
				return stringBuilder.ToString();
			}
			object obj3;
			object obj6;
			if (smethod_1(aiconfig_0.Provider))
			{
				if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
				{
					if (val == null)
					{
						obj3 = null;
					}
					else
					{
						JToken obj4 = val["message"];
						if (obj4 == null)
						{
							obj3 = null;
						}
						else
						{
							JToken obj5 = obj4[(object)"thinking"];
							if (obj5 == null)
							{
								obj3 = null;
							}
							else
							{
								obj3 = ((object)obj5).ToString();
								if (obj3 != null)
								{
									goto IL_013b;
								}
							}
						}
					}
					obj3 = "";
					goto IL_013b;
				}
				if (val == null)
				{
					obj6 = null;
				}
				else
				{
					JToken obj7 = val["reasoning_content"];
					if (obj7 == null)
					{
						obj6 = null;
					}
					else
					{
						obj6 = ((object)obj7).ToString();
						if (obj6 != null)
						{
							goto IL_016f;
						}
					}
				}
				obj6 = "";
				goto IL_016f;
			}
			goto end_IL_0000;
			IL_016f:
			return (string)obj6;
			IL_013b:
			return (string)obj3;
			end_IL_0000:;
		}
		catch (Exception)
		{
		}
		return "";
	}

	private string method_18(string string_0, AIConfig aiconfig_0)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrWhiteSpace(string_0) && string_0.TrimStart().StartsWith("{"))
		{
			try
			{
				JObject val = JsonConvert.DeserializeObject<JObject>(string_0);
				object obj4;
				if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
				{
					JToken obj = ((val != null) ? val["content"] : null);
					foreach (JToken item in (JArray)(((object)((obj is JArray) ? obj : null)) ?? ((object)new JArray())))
					{
						if (!(((object)item[(object)"type"])?.ToString() == "text"))
						{
							continue;
						}
						JToken obj2 = item[(object)"text"];
						object obj3;
						if (obj2 == null)
						{
							obj3 = null;
						}
						else
						{
							obj3 = ((object)obj2).ToString();
							if (obj3 != null)
							{
								goto IL_00e0;
							}
						}
						obj3 = "";
						goto IL_00e0;
						IL_00e0:
						return (string)obj3;
					}
				}
				else if (smethod_1(aiconfig_0.Provider))
				{
					if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
					{
						if (val == null)
						{
							obj4 = null;
						}
						else
						{
							JToken obj5 = val["message"];
							if (obj5 == null)
							{
								obj4 = null;
							}
							else
							{
								JToken obj6 = obj5[(object)"content"];
								if (obj6 == null)
								{
									obj4 = null;
								}
								else
								{
									obj4 = ((object)obj6).ToString();
									if (obj4 != null)
									{
										goto IL_015e;
									}
								}
							}
						}
						obj4 = "";
						goto IL_015e;
					}
					try
					{
						JToken val2 = ((val != null) ? val["choices"] : null);
						JArray val3 = (JArray)(object)((val2 is JArray) ? val2 : null);
						object obj8;
						if (val3 != null && ((JContainer)val3).Count > 0)
						{
							JToken? obj7 = ((IEnumerable<JToken>)val3).FirstOrDefault();
							if (obj7 == null)
							{
								obj8 = null;
							}
							else
							{
								JToken obj9 = obj7[(object)"message"];
								if (obj9 == null)
								{
									obj8 = null;
								}
								else
								{
									JToken obj10 = obj9[(object)"content"];
									if (obj10 == null)
									{
										obj8 = null;
									}
									else
									{
										obj8 = ((object)obj10).ToString();
										if (obj8 != null)
										{
											goto IL_01e1;
										}
									}
								}
							}
							obj8 = "";
							goto IL_01e1;
						}
						JObject val4 = (JObject)(object)((val2 is JObject) ? val2 : null);
						object obj12;
						if (val4 != null)
						{
							JToken obj11 = val4["message"];
							if (obj11 == null)
							{
								obj12 = null;
							}
							else
							{
								JToken obj13 = obj11[(object)"content"];
								if (obj13 == null)
								{
									obj12 = null;
								}
								else
								{
									obj12 = ((object)obj13).ToString();
									if (obj12 != null)
									{
										goto IL_0235;
									}
								}
							}
							obj12 = "";
							goto IL_0235;
						}
						goto end_IL_0165;
						IL_01e1:
						return (string)obj8;
						IL_0235:
						return (string)obj12;
						end_IL_0165:;
					}
					catch (Exception)
					{
					}
					return "";
				}
				goto end_IL_0026;
				IL_015e:
				return (string)obj4;
				end_IL_0026:;
			}
			catch (Exception)
			{
			}
			if (string_0.Contains("\"reasoning_content\":"))
			{
				try
				{
					JObject obj14 = JsonConvert.DeserializeObject<JObject>(string_0);
					if (obj14 == null)
					{
						if (obj14 == null)
						{
							goto IL_0284;
						}
					}
					else
					{
						obj14.Remove("reasoning_content");
						if (obj14 == null)
						{
							goto IL_0284;
						}
					}
					object obj15 = ((object)obj14).ToString();
					if (obj15 == null)
					{
						goto IL_0290;
					}
					goto IL_0292;
					IL_0292:
					return (string)obj15;
					IL_0290:
					obj15 = string_0;
					goto IL_0292;
					IL_0284:
					obj15 = null;
					goto IL_0290;
				}
				catch
				{
				}
			}
			return string_0;
		}
		return string_0;
	}

	private bool method_19(string string_0, AIConfig aiconfig_0, out ToolCallInfo? toolCallInfo_0)
	{
		toolCallInfo_0 = null;
		try
		{
			JObject val = JsonConvert.DeserializeObject<JObject>(string_0);
			ToolCallInfo obj11;
			object obj13;
			if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
			{
				JToken obj = ((val != null) ? val["content"] : null);
				JArray val2 = (JArray)(object)((obj is JArray) ? obj : null);
				if (val2 == null)
				{
					return false;
				}
				foreach (JToken item in val2)
				{
					if (((object)item[(object)"type"])?.ToString() == "tool_use")
					{
						string text = ((object)item[(object)"name"])?.ToString();
						JToken obj2 = item[(object)"input"];
						JObject val3 = (JObject)(object)((obj2 is JObject) ? obj2 : null);
						if (!string.IsNullOrEmpty(text))
						{
							toolCallInfo_0 = new ToolCallInfo
							{
								ToolName = text,
								Parameters = ((val3 != null) ? ((JToken)val3).ToObject<IDictionary<string, object>>() : null),
								CallId = ((object)item[(object)"id"])?.ToString()
							};
							return true;
						}
					}
				}
			}
			else if (smethod_1(aiconfig_0.Provider))
			{
				JArray val4 = null;
				if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
				{
					object obj3;
					if (val == null)
					{
						obj3 = null;
					}
					else
					{
						JToken obj4 = val["message"];
						obj3 = ((obj4 != null) ? obj4[(object)"tool_calls"] : null);
					}
					val4 = (JArray)((obj3 is JArray) ? obj3 : null);
				}
				else
				{
					try
					{
						JToken val5 = ((val != null) ? val["choices"] : null);
						JArray val6 = (JArray)(object)((val5 is JArray) ? val5 : null);
						object obj7;
						if (val6 != null && ((JContainer)val6).Count > 0)
						{
							JToken? obj5 = ((IEnumerable<JToken>)val6).FirstOrDefault();
							object obj6;
							if (obj5 == null)
							{
								obj6 = null;
							}
							else
							{
								obj6 = obj5[(object)"message"];
								if (obj6 != null)
								{
									obj7 = ((JToken)obj6)[(object)"tool_calls"];
									goto IL_01f5;
								}
							}
							obj7 = null;
							goto IL_01f5;
						}
						JObject val7 = (JObject)(object)((val5 is JObject) ? val5 : null);
						if (val7 != null)
						{
							JToken obj8 = val7["message"];
							JToken obj9 = ((obj8 != null) ? obj8[(object)"tool_calls"] : null);
							val4 = (JArray)(object)((obj9 is JArray) ? obj9 : null);
						}
						goto end_IL_0193;
						IL_01f5:
						val4 = (JArray)((obj7 is JArray) ? obj7 : null);
						end_IL_0193:;
					}
					catch (Exception)
					{
					}
				}
				if (val4 != null && ((IEnumerable<JToken>)val4).Any())
				{
					JToken first = ((JToken)val4).First;
					JToken val8 = first[(object)"function"];
					if (val8 == null)
					{
						return false;
					}
					string text2 = ((object)val8[(object)"name"])?.ToString();
					JToken val9 = val8[(object)"arguments"];
					IDictionary<string, object> dictionary = null;
					if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
					{
						JToken obj10 = ((val9 is JObject) ? val9 : null);
						dictionary = ((obj10 != null) ? obj10.ToObject<IDictionary<string, object>>() : null);
					}
					else
					{
						string text3 = ((object)val9)?.ToString();
						if (!string.IsNullOrEmpty(text3))
						{
							dictionary = JsonConvert.DeserializeObject<IDictionary<string, object>>(text3);
						}
					}
					if (!string.IsNullOrEmpty(text2) && dictionary != null)
					{
						obj11 = new ToolCallInfo
						{
							ToolName = text2,
							Parameters = dictionary
						};
						JToken obj12 = first[(object)"id"];
						if (obj12 == null)
						{
							obj13 = null;
						}
						else
						{
							obj13 = ((object)obj12).ToString();
							if (obj13 != null)
							{
								goto IL_0357;
							}
						}
						obj13 = Guid.NewGuid().ToString();
						goto IL_0357;
					}
				}
			}
			goto end_IL_0003;
			IL_0357:
			obj11.CallId = (string?)obj13;
			toolCallInfo_0 = obj11;
			return true;
			end_IL_0003:;
		}
		catch (Exception)
		{
		}
		return false;
	}

	private List<ToolCallInfo> method_20(string string_0, AIConfig aiconfig_0)
	{
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		List<ToolCallInfo> list = new List<ToolCallInfo>();
		try
		{
			JObject val = JsonConvert.DeserializeObject<JObject>(string_0);
			if (aiconfig_0.Provider.Equals("claude", StringComparison.OrdinalIgnoreCase))
			{
				JToken obj = ((val != null) ? val["content"] : null);
				JArray val2 = (JArray)(object)((obj is JArray) ? obj : null);
				if (val2 != null)
				{
					foreach (JToken item in val2)
					{
						if (!(((object)item[(object)"type"])?.ToString() == "tool_use"))
						{
							continue;
						}
						JToken obj2 = item[(object)"name"];
						object obj3;
						if (obj2 == null)
						{
							obj3 = null;
						}
						else
						{
							obj3 = ((object)obj2).ToString();
							if (obj3 != null)
							{
								goto IL_00b6;
							}
						}
						obj3 = string.Empty;
						goto IL_00b6;
						IL_00b6:
						string toolName = (string)obj3;
						ToolCallInfo obj4 = new ToolCallInfo
						{
							ToolName = toolName
						};
						JToken obj5 = item[(object)"input"];
						JToken obj6 = ((obj5 is JObject) ? obj5 : null);
						obj4.Parameters = ((obj6 != null) ? obj6.ToObject<IDictionary<string, object>>() : null);
						obj4.CallId = ((object)item[(object)"id"])?.ToString();
						list.Add(obj4);
					}
				}
				else
				{
					Logger.Warning("[ClaudeAI] content_array 为 null");
				}
			}
			else if (smethod_1(aiconfig_0.Provider))
			{
				JArray val3 = null;
				if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
				{
					object obj7;
					if (val == null)
					{
						obj7 = null;
					}
					else
					{
						JToken obj8 = val["message"];
						obj7 = ((obj8 != null) ? obj8[(object)"tool_calls"] : null);
					}
					val3 = (JArray)((obj7 is JArray) ? obj7 : null);
				}
				else if (smethod_0(aiconfig_0.Provider))
				{
					try
					{
						JToken val4 = ((val != null) ? val["choices"] : null);
						JArray val5 = (JArray)(object)((val4 is JArray) ? val4 : null);
						if (val5 != null && ((JContainer)val5).Count > 0)
						{
							JToken? obj9 = ((IEnumerable<JToken>)val5).FirstOrDefault();
							object obj10;
							if (obj9 == null)
							{
								obj10 = null;
							}
							else
							{
								JToken obj11 = obj9[(object)"message"];
								obj10 = ((obj11 != null) ? obj11[(object)"tool_calls"] : null);
							}
							val3 = (JArray)((obj10 is JArray) ? obj10 : null);
						}
						else
						{
							JObject val6 = (JObject)(object)((val4 is JObject) ? val4 : null);
							if (val6 != null)
							{
								JToken obj12 = val6["message"];
								JToken obj13 = ((obj12 != null) ? obj12[(object)"tool_calls"] : null);
								val3 = (JArray)(object)((obj13 is JArray) ? obj13 : null);
							}
						}
					}
					catch (Exception)
					{
					}
				}
				else
				{
					try
					{
						JToken val7 = ((val != null) ? val["choices"] : null);
						JArray val8 = (JArray)(object)((val7 is JArray) ? val7 : null);
						if (val8 != null && ((JContainer)val8).Count > 0)
						{
							JToken? obj14 = ((IEnumerable<JToken>)val8).FirstOrDefault();
							object obj15;
							if (obj14 == null)
							{
								obj15 = null;
							}
							else
							{
								JToken obj16 = obj14[(object)"message"];
								obj15 = ((obj16 != null) ? obj16[(object)"tool_calls"] : null);
							}
							val3 = (JArray)((obj15 is JArray) ? obj15 : null);
						}
						else
						{
							JObject val9 = (JObject)(object)((val7 is JObject) ? val7 : null);
							if (val9 != null)
							{
								JToken obj17 = val9["message"];
								JToken obj18 = ((obj17 != null) ? obj17[(object)"tool_calls"] : null);
								val3 = (JArray)(object)((obj18 is JArray) ? obj18 : null);
							}
						}
					}
					catch (Exception)
					{
					}
				}
				if (val3 != null)
				{
					foreach (JToken item2 in val3)
					{
						JToken val10 = item2[(object)"function"];
						if (val10 == null)
						{
							continue;
						}
						string text = ((object)val10[(object)"name"])?.ToString();
						JToken val11 = val10[(object)"arguments"];
						IDictionary<string, object> dictionary = null;
						if (aiconfig_0.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
						{
							JToken obj19 = ((val11 is JObject) ? val11 : null);
							dictionary = ((obj19 != null) ? obj19.ToObject<IDictionary<string, object>>() : null);
						}
						else if (smethod_0(aiconfig_0.Provider))
						{
							JObject val12 = (JObject)(object)((val11 is JObject) ? val11 : null);
							if (val12 != null)
							{
								dictionary = ((JToken)val12).ToObject<IDictionary<string, object>>();
							}
							else if (val11 != null)
							{
								try
								{
									string text2 = ((object)val11).ToString();
									if (!string.IsNullOrEmpty(text2))
									{
										dictionary = JsonConvert.DeserializeObject<IDictionary<string, object>>(text2);
									}
								}
								catch
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
									defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] vLLM arguments 解析失败，类型: ");
									defaultInterpolatedStringHandler.AppendFormatted<JTokenType>(val11.Type);
									Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
								}
							}
						}
						else
						{
							string text3 = ((object)val11)?.ToString();
							if (!string.IsNullOrEmpty(text3))
							{
								dictionary = JsonConvert.DeserializeObject<IDictionary<string, object>>(text3);
							}
						}
						if (string.IsNullOrEmpty(text) || dictionary == null)
						{
							continue;
						}
						ToolCallInfo obj21 = new ToolCallInfo
						{
							ToolName = text,
							Parameters = dictionary
						};
						JToken obj22 = item2[(object)"id"];
						object obj23;
						if (obj22 == null)
						{
							obj23 = null;
						}
						else
						{
							obj23 = ((object)obj22).ToString();
							if (obj23 != null)
							{
								goto IL_04b6;
							}
						}
						obj23 = Guid.NewGuid().ToString();
						goto IL_04b6;
						IL_04b6:
						obj21.CallId = (string?)obj23;
						list.Add(obj21);
					}
				}
			}
		}
		catch (Exception ex3)
		{
			Logger.Error("[ClaudeAI] 提取工具调用时出错: " + ex3.Message);
		}
		return list;
	}

	[AsyncIteratorStateMachine(typeof(Class113))]
	private IAsyncEnumerable<string> method_21(StreamReader streamReader_0, AIConfig aiconfig_0, [EnumeratorCancellation] CancellationToken cancellationToken_0)
	{
		return new Class113(-2)
		{
			streamReader_1 = streamReader_0,
			aiconfig_1 = aiconfig_0,
			cancellationToken_1 = cancellationToken_0
		};
	}

	public void Dispose()
	{
		httpClient_0?.Dispose();
	}

	[AsyncStateMachine(typeof(Struct187))]
	private Task<string> method_22(string string_0, string string_1)
	{
		Struct187 stateMachine = default(Struct187);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.string_0 = string_0;
		stateMachine.string_1 = string_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private static bool smethod_0(string string_0)
	{
		return string_0.Equals("vllm", StringComparison.OrdinalIgnoreCase);
	}

	private static bool smethod_1(string string_0)
	{
		if (!string_0.Equals("openai", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("bigmodel", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("zhipu", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("qwen", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("dashscope", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("baichuan", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("minimax", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("moonshot", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("deepseek", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("siliconflow", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("ollama", StringComparison.OrdinalIgnoreCase) && !string_0.Equals("vllm", StringComparison.OrdinalIgnoreCase))
		{
			return string_0.Equals("custom", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static int? smethod_2(JToken? jtoken_0, string[] string_0)
	{
		if (jtoken_0 == null)
		{
			return null;
		}
		int num = 0;
		int? num2;
		while (true)
		{
			if (num < string_0.Length)
			{
				string text = string_0[num];
				JToken obj = jtoken_0[(object)text];
				num2 = ((obj != null) ? new int?(obj.ToObject<int>()) : ((int?)null));
				if (num2.HasValue && num2.Value > 0)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return num2.Value;
	}

	private AIResponseWithThinking method_23(AIResponseWithThinking airesponseWithThinking_0, JObject? jobject_0)
	{
		try
		{
			Logger.Info("[ClaudeAI] 使用 Token 估算兜底策略");
			string text = airesponseWithThinking_0.Content ?? string.Empty;
			string text2 = airesponseWithThinking_0.ReasoningContent ?? string.Empty;
			airesponseWithThinking_0.OutputTokens = ImageTokenCalculator.EstimateTokensFromText(text + text2);
			airesponseWithThinking_0.InputTokens = ((airesponseWithThinking_0.OutputTokens > 0) ? Math.Max(100, airesponseWithThinking_0.OutputTokens / 2) : 0);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ClaudeAI] ⚠️ Token 估算（不精确）: 输入=");
			defaultInterpolatedStringHandler.AppendFormatted(airesponseWithThinking_0.InputTokens);
			defaultInterpolatedStringHandler.AppendLiteral(", 输出=");
			defaultInterpolatedStringHandler.AppendFormatted(airesponseWithThinking_0.OutputTokens);
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			Logger.Warning("[ClaudeAI] 提示: Token 统计不准确，实际消耗可能不同。建议检查 API 响应格式。");
			return airesponseWithThinking_0;
		}
		catch (Exception ex)
		{
			Logger.Error("[ClaudeAI] Token 估算失败: " + ex.Message);
			return airesponseWithThinking_0;
		}
	}
}
