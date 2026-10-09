using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.Core.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ns7;

namespace RevitAi.Core.Services;

public sealed class CodeMarketService : ICodeMarketService
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct15 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<CodeMarketSearchResult>> asyncTaskMethodBuilder_0;

		public CodeMarketSearchOptions codeMarketSearchOptions_0;

		public CodeMarketService codeMarketService_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Expected O, but got Unknown
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<CodeMarketSearchResult> result2;
			try
			{
				TaskAwaiter<Result<JObject>> awaiter;
				if (num != 0)
				{
					JObject val = new JObject
					{
						["p_order_by"] = ((JToken)(((object)codeMarketSearchOptions_0.SortOrder/*cast due to constrained. prefix*/).ToString().ToLower())),
						["p_order_direction"] = ((JToken)(((int)codeMarketSearchOptions_0.SortDirection == 0) ? "ASC" : "DESC")),
						["p_limit"] = ((JToken)(codeMarketSearchOptions_0.Limit)),
						["p_offset"] = ((JToken)(codeMarketSearchOptions_0.Offset))
					};
					if (!string.IsNullOrWhiteSpace(codeMarketSearchOptions_0.SearchKeyword))
					{
						val["p_search_keyword"] = ((JToken)(codeMarketSearchOptions_0.SearchKeyword));
					}
					List<string> tags = codeMarketSearchOptions_0.Tags;
					if (tags != null && tags.Count > 0)
					{
						val["p_tags"] = (JToken)(object)JArray.FromObject((object)codeMarketSearchOptions_0.Tags);
					}
					List<string> categories = codeMarketSearchOptions_0.Categories;
					if (categories != null && categories.Count > 0)
					{
						val["p_categories"] = (JToken)(object)JArray.FromObject((object)codeMarketSearchOptions_0.Categories);
					}
					if (codeMarketSearchOptions_0.MinPrice.HasValue)
					{
						val["p_min_price"] = ((JToken)(codeMarketSearchOptions_0.MinPrice.Value));
					}
					if (codeMarketSearchOptions_0.MaxPrice.HasValue)
					{
						val["p_max_price"] = ((JToken)(codeMarketSearchOptions_0.MaxPrice.Value));
					}
					if (codeMarketSearchOptions_0.MinRating.HasValue)
					{
						val["p_min_rating"] = ((JToken)(codeMarketSearchOptions_0.MinRating.Value));
					}
					awaiter = codeMarketService.method_0("browse_market", val, cancellationToken_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<JObject>>);
					num = -1;
					int_0 = -1;
				}
				Result<JObject> result = awaiter.GetResult();
				JObject value;
				object obj2;
				if (!result.IsSuccess)
				{
					result2 = CodeMarketResult<CodeMarketSearchResult>.Fail(result.Error ?? "浏览失败");
				}
				else
				{
					value = result.Value;
					if (value != null)
					{
						JToken val2 = default(JToken);
						object obj;
						if (!value.TryGetValue("snippets", out val2))
						{
							obj = null;
						}
						else
						{
							obj = ((val2 is JArray) ? val2 : null);
							if (obj != null)
							{
								obj2 = ((IEnumerable<JToken>)obj).Select((Func<JToken, CodeSnippetMarketItem>)delegate(JToken jtoken_0)
								{
									//IL_0000: Unknown result type (might be due to invalid IL or missing references)
									//IL_0005: Unknown result type (might be due to invalid IL or missing references)
									//IL_0030: Unknown result type (might be due to invalid IL or missing references)
									//IL_005b: Unknown result type (might be due to invalid IL or missing references)
									//IL_0086: Unknown result type (might be due to invalid IL or missing references)
									//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
									//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
									//IL_0107: Unknown result type (might be due to invalid IL or missing references)
									//IL_0132: Unknown result type (might be due to invalid IL or missing references)
									//IL_0158: Unknown result type (might be due to invalid IL or missing references)
									//IL_017a: Unknown result type (might be due to invalid IL or missing references)
									//IL_019c: Unknown result type (might be due to invalid IL or missing references)
									//IL_01be: Unknown result type (might be due to invalid IL or missing references)
									//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
									//IL_0206: Unknown result type (might be due to invalid IL or missing references)
									//IL_0228: Unknown result type (might be due to invalid IL or missing references)
									//IL_024e: Unknown result type (might be due to invalid IL or missing references)
									//IL_0270: Unknown result type (might be due to invalid IL or missing references)
									//IL_0297: Expected O, but got Unknown
									CodeSnippetMarketItem val4 = new CodeSnippetMarketItem();
									JToken obj6 = jtoken_0[(object)"id"];
									object obj7;
									if (obj6 == null)
									{
										obj7 = null;
									}
									else
									{
										obj7 = ((object)obj6).ToString();
										if (obj7 != null)
										{
											goto IL_002b;
										}
									}
									obj7 = string.Empty;
									goto IL_002b;
									IL_0102:
									object obj8;
									val4.Tags = (List<string>)obj8;
									JToken obj9 = jtoken_0[(object)"category"];
									object obj10;
									if (obj9 == null)
									{
										obj10 = null;
									}
									else
									{
										obj10 = ((object)obj9).ToString();
										if (obj10 != null)
										{
											goto IL_012d;
										}
									}
									obj10 = string.Empty;
									goto IL_012d;
									IL_00ac:
									object obj11;
									val4.Name = (string)obj11;
									JToken obj12 = jtoken_0[(object)"description"];
									object obj13;
									if (obj12 == null)
									{
										obj13 = null;
									}
									else
									{
										obj13 = ((object)obj12).ToString();
										if (obj13 != null)
										{
											goto IL_00d7;
										}
									}
									obj13 = string.Empty;
									goto IL_00d7;
									IL_0056:
									object obj14;
									val4.AuthorId = (string)obj14;
									JToken obj15 = jtoken_0[(object)"author_name"];
									object obj16;
									if (obj15 == null)
									{
										obj16 = null;
									}
									else
									{
										obj16 = ((object)obj15).ToString();
										if (obj16 != null)
										{
											goto IL_0081;
										}
									}
									obj16 = string.Empty;
									goto IL_0081;
									IL_002b:
									val4.Id = (string)obj7;
									JToken obj17 = jtoken_0[(object)"author_id"];
									if (obj17 == null)
									{
										obj14 = null;
									}
									else
									{
										obj14 = ((object)obj17).ToString();
										if (obj14 != null)
										{
											goto IL_0056;
										}
									}
									obj14 = string.Empty;
									goto IL_0056;
									IL_00d7:
									val4.Description = (string)obj13;
									JToken obj18 = jtoken_0[(object)"tags"];
									if (obj18 == null)
									{
										obj8 = null;
									}
									else
									{
										obj8 = obj18.ToObject<List<string>>();
										if (obj8 != null)
										{
											goto IL_0102;
										}
									}
									obj8 = new List<string>();
									goto IL_0102;
									IL_0081:
									val4.AuthorName = (string)obj16;
									JToken obj19 = jtoken_0[(object)"name"];
									if (obj19 == null)
									{
										obj11 = null;
									}
									else
									{
										obj11 = ((object)obj19).ToString();
										if (obj11 != null)
										{
											goto IL_00ac;
										}
									}
									obj11 = string.Empty;
									goto IL_00ac;
									IL_012d:
									val4.Category = (string)obj10;
									JToken obj20 = jtoken_0[(object)"price"];
									val4.Price = ((obj20 != null) ? obj20.ToObject<decimal>() : 0m);
									JToken obj21 = jtoken_0[(object)"is_free"];
									val4.IsFree = obj21 != null && obj21.ToObject<bool>();
									JToken obj22 = jtoken_0[(object)"download_count"];
									val4.DownloadCount = ((obj22 != null) ? obj22.ToObject<int>() : 0);
									JToken obj23 = jtoken_0[(object)"view_count"];
									val4.ViewCount = ((obj23 != null) ? obj23.ToObject<int>() : 0);
									JToken obj24 = jtoken_0[(object)"favorite_count"];
									val4.FavoriteCount = ((obj24 != null) ? obj24.ToObject<int>() : 0);
									JToken obj25 = jtoken_0[(object)"average_rating"];
									val4.AverageRating = ((obj25 != null) ? obj25.ToObject<decimal>() : 0m);
									JToken obj26 = jtoken_0[(object)"rating_count"];
									val4.RatingCount = ((obj26 != null) ? obj26.ToObject<int>() : 0);
									JToken obj27 = jtoken_0[(object)"hot_score"];
									val4.HotScore = ((obj27 != null) ? obj27.ToObject<decimal>() : 0m);
									JToken obj28 = jtoken_0[(object)"is_new"];
									val4.IsNew = obj28 != null && obj28.ToObject<bool>();
									JToken obj29 = jtoken_0[(object)"created_at"];
									val4.CreatedAt = ((obj29 != null) ? obj29.ToObject<DateTime>() : DateTime.MinValue);
									return val4;
								}).ToList();
								if (obj2 == null)
								{
									goto IL_033c;
								}
								goto IL_0342;
							}
						}
						obj2 = null;
						goto IL_033c;
					}
					result2 = CodeMarketResult<CodeMarketSearchResult>.Fail("服务器返回空数据");
				}
				goto end_IL_000f;
				IL_0342:
				List<CodeSnippetMarketItem> snippets = (List<CodeSnippetMarketItem>)obj2;
				CodeMarketSearchResult val3 = new CodeMarketSearchResult
				{
					Snippets = snippets
				};
				JToken obj3 = value["count"];
				val3.Count = ((obj3 != null) ? obj3.ToObject<int>() : 0);
				JToken obj4 = value["total"];
				val3.Total = ((obj4 != null) ? obj4.ToObject<int>() : 0);
				JToken obj5 = value["has_more"];
				val3.HasMore = obj5 != null && obj5.ToObject<bool>();
				result2 = CodeMarketResult<CodeMarketSearchResult>.Ok(val3, (string)null);
				goto end_IL_000f;
				IL_033c:
				obj2 = new List<CodeSnippetMarketItem>();
				goto IL_0342;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 浏览市场失败: " + ex.Message, ex);
				result2 = CodeMarketResult<CodeMarketSearchResult>.Fail("浏览失败: " + ex.Message);
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
	public struct Struct16 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<JObject>> asyncTaskMethodBuilder_0;

		public JObject jobject_0;

		public CodeMarketService codeMarketService_0;

		public string string_0;

		public CancellationToken cancellationToken_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			Result<JObject> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)jobject_0), Encoding.UTF8, "application/json");
					awaiter2 = codeMarketService.isupabaseClient_0.HttpClient.PostAsync(codeMarketService.isupabaseClient_0.BaseUrl + "/rest/v1/rpc/" + string_0, content, cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00c9;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00c9;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_019e;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_019e:
					result = awaiter.GetResult();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 3);
					defaultInterpolatedStringHandler.AppendLiteral("[CodeMarketService] RPC 调用失败 ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral(": ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(10, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("RPC 调用失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
					result2 = Result<JObject>.Failure(defaultInterpolatedStringHandler2.ToStringAndClear());
					goto end_IL_000f;
					IL_00c9:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						awaiter = smethod_0(httpResponseMessage_0.Content, cancellationToken_0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_019e;
					}
					awaiter = smethod_0(httpResponseMessage_0.Content, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					break;
				}
				JObject val = JsonConvert.DeserializeObject<JObject>(awaiter.GetResult());
				object obj3;
				if (val == null)
				{
					result2 = Result<JObject>.Failure("RPC 返回数据解析失败");
				}
				else
				{
					JToken obj = val["success"];
					bool? flag = ((obj != null) ? new bool?(Extensions.Value<bool>((IEnumerable<JToken>)obj)) : ((bool?)null));
					if (flag.HasValue && !flag.Value)
					{
						JToken obj2 = val["error"];
						if (obj2 == null)
						{
							obj3 = null;
						}
						else
						{
							obj3 = ((object)obj2).ToString();
							if (obj3 != null)
							{
								goto IL_0305;
							}
						}
						obj3 = "未知错误";
						goto IL_0305;
					}
					result2 = Result<JObject>.Success(val);
				}
				goto end_IL_000f;
				IL_0305:
				result2 = Result<JObject>.Failure((string)obj3);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] RPC 调用异常 " + string_0 + ": " + ex.Message, ex);
				result2 = Result<JObject>.Failure("RPC 调用异常: " + ex.Message);
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
	public struct Struct17 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<HashSet<string>>> asyncTaskMethodBuilder_0;

		public CodeMarketService codeMarketService_0;

		public List<string> list_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<(Guid? userId, string? deviceId, Result? error)> taskAwaiter_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Expected O, but got Unknown
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<HashSet<string>> result2;
			try
			{
				TaskAwaiter<Result<JObject>> awaiter;
				TaskAwaiter<(Guid?, string, Result)> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<Result<JObject>>);
						num = -1;
						int_0 = -1;
						goto IL_0175;
					}
					awaiter2 = codeMarketService.method_1().GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<(Guid?, string, Result)>);
					num = -1;
					int_0 = -1;
				}
				(Guid?, string, Result) result = awaiter2.GetResult();
				var (guid, text, _) = result;
				if (result.Item3 == null && (guid.HasValue || text != null))
				{
					JObject val = new JObject { ["p_snippet_ids"] = (JToken)(object)JArray.FromObject((object)list_0) };
					if (guid.HasValue)
					{
						val["p_user_id"] = ((JToken)(guid.ToString()));
					}
					if (text != null)
					{
						val["p_device_id"] = ((JToken)(text));
					}
					awaiter = codeMarketService.method_0("check_snippet_favorited", val, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0175;
				}
				result2 = CodeMarketResult<HashSet<string>>.Ok(new HashSet<string>(), (string)null);
				goto end_IL_000f;
				IL_0175:
				Result<JObject> result3 = awaiter.GetResult();
				object obj2;
				if (!result3.IsSuccess)
				{
					result2 = CodeMarketResult<HashSet<string>>.Fail(result3.Error ?? "检查收藏状态失败");
				}
				else
				{
					JObject value = result3.Value;
					if (value != null)
					{
						JToken val2 = default(JToken);
						object obj;
						if (!value.TryGetValue("favorited_ids", out val2))
						{
							obj = null;
						}
						else
						{
							obj = ((val2 is JArray) ? val2 : null);
							if (obj != null)
							{
								obj2 = ((IEnumerable<JToken>)obj).Select(delegate(JToken jtoken_0)
								{
									object obj3;
									if (jtoken_0 == null)
									{
										obj3 = null;
									}
									else
									{
										obj3 = ((object)jtoken_0).ToString();
										if (obj3 != null)
										{
											goto IL_0015;
										}
									}
									obj3 = string.Empty;
									goto IL_0015;
									IL_0015:
									return (string)obj3;
								}).ToHashSet();
								if (obj2 == null)
								{
									goto IL_021b;
								}
								goto IL_0221;
							}
						}
						obj2 = null;
						goto IL_021b;
					}
					result2 = CodeMarketResult<HashSet<string>>.Fail("服务器返回空数据");
				}
				goto end_IL_000f;
				IL_0221:
				result2 = CodeMarketResult<HashSet<string>>.Ok((HashSet<string>)obj2, (string)null);
				goto end_IL_000f;
				IL_021b:
				obj2 = new HashSet<string>();
				goto IL_0221;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 检查收藏状态失败: " + ex.Message, ex);
				result2 = CodeMarketResult<HashSet<string>>.Fail("检查收藏状态失败: " + ex.Message);
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
	public struct Struct18 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<bool>> asyncTaskMethodBuilder_0;

		public string string_0;

		public CodeMarketService codeMarketService_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<(Guid? userId, string? deviceId, Result? error)> taskAwaiter_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Expected O, but got Unknown
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<bool> result;
			try
			{
				TaskAwaiter<(Guid?, string, Result)> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<(Guid?, string, Result)>);
					num = -1;
					int_0 = -1;
					goto IL_00b4;
				}
				TaskAwaiter<Result<JObject>> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<JObject>>);
					num = -1;
					int_0 = -1;
					goto IL_01ab;
				}
				if (!string.IsNullOrWhiteSpace(string_0))
				{
					awaiter = codeMarketService.method_1().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00b4;
				}
				result = CodeMarketResult<bool>.Fail("片段 ID 不能为空");
				goto end_IL_000f;
				IL_023c:
				object obj;
				string text = (string)obj;
				result = CodeMarketResult<bool>.Ok(true, text);
				goto end_IL_000f;
				IL_00b4:
				var (guid, text2, val) = awaiter.GetResult();
				if (val == null)
				{
					JObject val2 = new JObject { ["p_snippet_id"] = ((JToken)(string_0)) };
					if (guid.HasValue)
					{
						val2["p_user_id"] = ((JToken)(guid.ToString()));
					}
					if (text2 != null)
					{
						val2["p_device_id"] = ((JToken)(text2));
					}
					awaiter2 = codeMarketService.method_0("delete_snippet_from_market", val2, cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_01ab;
				}
				result = CodeMarketResult<bool>.Fail(val.Error ?? "认证失败，请重新登录");
				goto end_IL_000f;
				IL_01ab:
				Result<JObject> result2 = awaiter2.GetResult();
				if (!result2.IsSuccess)
				{
					result = CodeMarketResult<bool>.Fail(result2.Error ?? "删除失败");
				}
				else
				{
					JObject value = result2.Value;
					if (value != null)
					{
						JToken val3 = default(JToken);
						if (!value.TryGetValue("message", out val3))
						{
							obj = "删除成功";
						}
						else
						{
							if (val3 == null)
							{
								obj = null;
							}
							else
							{
								obj = ((object)val3).ToString();
								if (obj != null)
								{
									goto IL_023c;
								}
							}
							obj = "删除成功";
						}
						goto IL_023c;
					}
					result = CodeMarketResult<bool>.Fail("服务器返回空数据");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 删除片段失败: " + ex.Message, ex);
				result = CodeMarketResult<bool>.Fail("删除失败: " + ex.Message);
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
	public struct Struct19 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<CodeSnippetDownloadResult>> asyncTaskMethodBuilder_0;

		public string string_0;

		public CodeMarketService codeMarketService_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<(Guid? userId, string? deviceId, Result? error)> taskAwaiter_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Expected O, but got Unknown
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Expected O, but got Unknown
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<CodeSnippetDownloadResult> result;
			try
			{
				TaskAwaiter<(Guid?, string, Result)> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<(Guid?, string, Result)>);
					num = -1;
					int_0 = -1;
					goto IL_00b4;
				}
				TaskAwaiter<Result<JObject>> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<JObject>>);
					num = -1;
					int_0 = -1;
					goto IL_01cd;
				}
				if (!string.IsNullOrWhiteSpace(string_0))
				{
					awaiter = codeMarketService.method_1().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00b4;
				}
				result = CodeMarketResult<CodeSnippetDownloadResult>.Fail("片段 ID 不能为空");
				goto end_IL_000f;
				IL_01cd:
				Result<JObject> result2 = awaiter2.GetResult();
				JObject value;
				JObject val;
				CodeSnippetDownloadResult val3;
				object snippet;
				CodeSnippetMarketItem val8;
				object obj2;
				if (!result2.IsSuccess)
				{
					result = CodeMarketResult<CodeSnippetDownloadResult>.Fail(result2.Error ?? "下载失败");
				}
				else
				{
					value = result2.Value;
					if (value != null)
					{
						JToken val2 = default(JToken);
						val = (JObject)(object)(value.TryGetValue("snippet", out val2) ? ((val2 is JObject) ? val2 : null) : null);
						JToken val4 = default(JToken);
						JToken val5 = default(JToken);
						JToken val6 = default(JToken);
						JToken val7 = default(JToken);
						val3 = new CodeSnippetDownloadResult
						{
							Owned = (value.TryGetValue("owned", out val4) && val4 != null && val4.ToObject<bool>()),
							IsFree = (value.TryGetValue("is_free", out val5) && val5 != null && val5.ToObject<bool>()),
							TransactionId = ((!value.TryGetValue("transaction_id", out val6)) ? null : ((object)val6)?.ToString()),
							PricePaid = ((!value.TryGetValue("price_paid", out val7)) ? ((decimal?)null) : ((val7 != null) ? new decimal?(val7.ToObject<decimal>()) : ((decimal?)null)))
						};
						if (val == null)
						{
							snippet = null;
							goto IL_0470;
						}
						val8 = new CodeSnippetMarketItem();
						JToken obj = val["id"];
						if (obj == null)
						{
							obj2 = null;
						}
						else
						{
							obj2 = ((object)obj).ToString();
							if (obj2 != null)
							{
								goto IL_0345;
							}
						}
						obj2 = string.Empty;
						goto IL_0345;
					}
					result = CodeMarketResult<CodeSnippetDownloadResult>.Fail("服务器返回空数据");
				}
				goto end_IL_000f;
				IL_039d:
				object obj3;
				val8.Name = (string)obj3;
				JToken obj4 = val["description"];
				object obj5;
				if (obj4 == null)
				{
					obj5 = null;
				}
				else
				{
					obj5 = ((object)obj4).ToString();
					if (obj5 != null)
					{
						goto IL_03c9;
					}
				}
				obj5 = string.Empty;
				goto IL_03c9;
				IL_0418:
				object obj6;
				val8.Tags = (List<string>)obj6;
				JToken obj7 = val["category"];
				object obj8;
				if (obj7 == null)
				{
					obj8 = null;
				}
				else
				{
					obj8 = ((object)obj7).ToString();
					if (obj8 != null)
					{
						goto IL_0444;
					}
				}
				obj8 = string.Empty;
				goto IL_0444;
				IL_0345:
				val8.Id = (string)obj2;
				JToken obj9 = val["author_id"];
				object obj10;
				if (obj9 == null)
				{
					obj10 = null;
				}
				else
				{
					obj10 = ((object)obj9).ToString();
					if (obj10 != null)
					{
						goto IL_0371;
					}
				}
				obj10 = string.Empty;
				goto IL_0371;
				IL_0444:
				val8.Category = (string)obj8;
				JToken obj11 = val["price"];
				val8.Price = ((obj11 != null) ? obj11.ToObject<decimal>() : 0m);
				snippet = (object)val8;
				goto IL_0470;
				IL_00b4:
				var (guid, text, val9) = awaiter.GetResult();
				if (val9 != null)
				{
					result = CodeMarketResult<CodeSnippetDownloadResult>.Fail(val9.Error ?? "认证失败，请重新登录");
				}
				else
				{
					if (guid.HasValue || text != null)
					{
						JObject val10 = new JObject { ["p_snippet_id"] = ((JToken)(string_0)) };
						if (guid.HasValue)
						{
							val10["p_user_id"] = ((JToken)(guid.ToString()));
						}
						if (text != null)
						{
							val10["p_device_id"] = ((JToken)(text));
						}
						awaiter2 = codeMarketService.method_0("download_snippet_from_market", val10, cancellationToken_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_01cd;
					}
					result = CodeMarketResult<CodeSnippetDownloadResult>.Fail("需要用户登录或设备标识");
				}
				goto end_IL_000f;
				IL_0371:
				val8.AuthorId = (string)obj10;
				JToken obj12 = val["name"];
				if (obj12 == null)
				{
					obj3 = null;
				}
				else
				{
					obj3 = ((object)obj12).ToString();
					if (obj3 != null)
					{
						goto IL_039d;
					}
				}
				obj3 = string.Empty;
				goto IL_039d;
				IL_03c9:
				val8.Description = (string)obj5;
				val8.CodeContent = ((object)val["code_content"])?.ToString();
				JToken obj13 = val["tags"];
				if (obj13 == null)
				{
					obj6 = null;
				}
				else
				{
					obj6 = obj13.ToObject<List<string>>();
					if (obj6 != null)
					{
						goto IL_0418;
					}
				}
				obj6 = new List<string>();
				goto IL_0418;
				IL_0470:
				val3.Snippet = (CodeSnippetMarketItem)snippet;
				result = CodeMarketResult<CodeSnippetDownloadResult>.Ok(val3, ((object)value["message"])?.ToString());
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 下载片段失败: " + ex.Message, ex);
				result = CodeMarketResult<CodeSnippetDownloadResult>.Fail("下载失败: " + ex.Message);
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
	public struct Struct20 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<List<CodeSnippetMarketItem>>> asyncTaskMethodBuilder_0;

		public CodeMarketService codeMarketService_0;

		public int int_1;

		public int int_2;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<(Guid? userId, string? deviceId, Result? error)> taskAwaiter_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Expected O, but got Unknown
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<List<CodeSnippetMarketItem>> result;
			try
			{
				TaskAwaiter<Result<JObject>> awaiter;
				TaskAwaiter<(Guid?, string, Result)> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<Result<JObject>>);
						num = -1;
						int_0 = -1;
						goto IL_01a1;
					}
					awaiter2 = codeMarketService.method_1().GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<(Guid?, string, Result)>);
					num = -1;
					int_0 = -1;
				}
				var (guid, text, val) = awaiter2.GetResult();
				if (val == null)
				{
					JObject val2 = new JObject
					{
						["p_limit"] = ((JToken)(int_1)),
						["p_offset"] = ((JToken)(int_2))
					};
					if (guid.HasValue)
					{
						val2["p_user_id"] = ((JToken)(guid.ToString()));
					}
					if (text != null)
					{
						val2["p_device_id"] = ((JToken)(text));
					}
					awaiter = codeMarketService.method_0("get_my_favorites", val2, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_01a1;
				}
				result = CodeMarketResult<List<CodeSnippetMarketItem>>.Fail(val.Error ?? "认证失败，请重新登录");
				goto end_IL_000f;
				IL_01a1:
				Result<JObject> result2 = awaiter.GetResult();
				object obj2;
				if (!result2.IsSuccess)
				{
					result = CodeMarketResult<List<CodeSnippetMarketItem>>.Fail(result2.Error ?? "获取收藏失败");
				}
				else
				{
					JObject value = result2.Value;
					if (value != null)
					{
						JToken val3 = default(JToken);
						object obj;
						if (!value.TryGetValue("snippets", out val3))
						{
							obj = null;
						}
						else
						{
							obj = ((val3 is JArray) ? val3 : null);
							if (obj != null)
							{
								obj2 = ((IEnumerable<JToken>)obj).Select((Func<JToken, CodeSnippetMarketItem>)delegate(JToken jtoken_0)
								{
									//IL_0000: Unknown result type (might be due to invalid IL or missing references)
									//IL_0005: Unknown result type (might be due to invalid IL or missing references)
									//IL_0030: Unknown result type (might be due to invalid IL or missing references)
									//IL_005b: Unknown result type (might be due to invalid IL or missing references)
									//IL_0086: Unknown result type (might be due to invalid IL or missing references)
									//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
									//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
									//IL_0102: Unknown result type (might be due to invalid IL or missing references)
									//IL_0124: Unknown result type (might be due to invalid IL or missing references)
									//IL_014a: Unknown result type (might be due to invalid IL or missing references)
									//IL_016c: Unknown result type (might be due to invalid IL or missing references)
									//IL_018e: Unknown result type (might be due to invalid IL or missing references)
									//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
									//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
									//IL_0202: Expected O, but got Unknown
									CodeSnippetMarketItem val4 = new CodeSnippetMarketItem();
									JToken obj3 = jtoken_0[(object)"id"];
									object obj4;
									if (obj3 == null)
									{
										obj4 = null;
									}
									else
									{
										obj4 = ((object)obj3).ToString();
										if (obj4 != null)
										{
											goto IL_002b;
										}
									}
									obj4 = string.Empty;
									goto IL_002b;
									IL_0056:
									object obj5;
									val4.Name = (string)obj5;
									JToken obj6 = jtoken_0[(object)"description"];
									object obj7;
									if (obj6 == null)
									{
										obj7 = null;
									}
									else
									{
										obj7 = ((object)obj6).ToString();
										if (obj7 != null)
										{
											goto IL_0081;
										}
									}
									obj7 = string.Empty;
									goto IL_0081;
									IL_00ac:
									object obj8;
									val4.Tags = (List<string>)obj8;
									JToken obj9 = jtoken_0[(object)"category"];
									object obj10;
									if (obj9 == null)
									{
										obj10 = null;
									}
									else
									{
										obj10 = ((object)obj9).ToString();
										if (obj10 != null)
										{
											goto IL_00d7;
										}
									}
									obj10 = string.Empty;
									goto IL_00d7;
									IL_00d7:
									val4.Category = (string)obj10;
									JToken obj11 = jtoken_0[(object)"price"];
									val4.Price = ((obj11 != null) ? obj11.ToObject<decimal>() : 0m);
									JToken obj12 = jtoken_0[(object)"is_free"];
									val4.IsFree = obj12 != null && obj12.ToObject<bool>();
									JToken obj13 = jtoken_0[(object)"average_rating"];
									val4.AverageRating = ((obj13 != null) ? obj13.ToObject<decimal>() : 0m);
									JToken obj14 = jtoken_0[(object)"rating_count"];
									val4.RatingCount = ((obj14 != null) ? obj14.ToObject<int>() : 0);
									JToken obj15 = jtoken_0[(object)"download_count"];
									val4.DownloadCount = ((obj15 != null) ? obj15.ToObject<int>() : 0);
									JToken obj16 = jtoken_0[(object)"favorite_count"];
									val4.FavoriteCount = ((obj16 != null) ? obj16.ToObject<int>() : 0);
									JToken obj17 = jtoken_0[(object)"view_count"];
									val4.ViewCount = ((obj17 != null) ? obj17.ToObject<int>() : 0);
									JToken obj18 = jtoken_0[(object)"favorited_at"];
									val4.FavoritedAt = ((obj18 != null) ? new DateTime?(obj18.ToObject<DateTime>()) : ((DateTime?)null));
									return val4;
									IL_002b:
									val4.Id = (string)obj4;
									JToken obj19 = jtoken_0[(object)"name"];
									if (obj19 == null)
									{
										obj5 = null;
									}
									else
									{
										obj5 = ((object)obj19).ToString();
										if (obj5 != null)
										{
											goto IL_0056;
										}
									}
									obj5 = string.Empty;
									goto IL_0056;
									IL_0081:
									val4.Description = (string)obj7;
									JToken obj20 = jtoken_0[(object)"tags"];
									if (obj20 == null)
									{
										obj8 = null;
									}
									else
									{
										obj8 = obj20.ToObject<List<string>>();
										if (obj8 != null)
										{
											goto IL_00ac;
										}
									}
									obj8 = new List<string>();
									goto IL_00ac;
								}).ToList();
								if (obj2 == null)
								{
									goto IL_0247;
								}
								goto IL_024d;
							}
						}
						obj2 = null;
						goto IL_0247;
					}
					result = CodeMarketResult<List<CodeSnippetMarketItem>>.Fail("服务器返回空数据");
				}
				goto end_IL_000f;
				IL_024d:
				result = CodeMarketResult<List<CodeSnippetMarketItem>>.Ok((List<CodeSnippetMarketItem>)obj2, (string)null);
				goto end_IL_000f;
				IL_0247:
				obj2 = new List<CodeSnippetMarketItem>();
				goto IL_024d;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 获取收藏失败: " + ex.Message, ex);
				result = CodeMarketResult<List<CodeSnippetMarketItem>>.Fail("获取收藏失败: " + ex.Message);
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
	public struct Struct21 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<List<CodeSnippetMarketItem>>> asyncTaskMethodBuilder_0;

		public CodeMarketService codeMarketService_0;

		public bool bool_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<(Guid? userId, string? deviceId, Result? error)> taskAwaiter_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Expected O, but got Unknown
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<List<CodeSnippetMarketItem>> result;
			try
			{
				TaskAwaiter<Result<JObject>> awaiter;
				TaskAwaiter<(Guid?, string, Result)> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<Result<JObject>>);
						num = -1;
						int_0 = -1;
						goto IL_0186;
					}
					awaiter2 = codeMarketService.method_1().GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<(Guid?, string, Result)>);
					num = -1;
					int_0 = -1;
				}
				var (guid, text, val) = awaiter2.GetResult();
				if (val == null)
				{
					JObject val2 = new JObject { ["p_include_deleted"] = ((JToken)(bool_0)) };
					if (guid.HasValue)
					{
						val2["p_user_id"] = ((JToken)(guid.ToString()));
					}
					if (text != null)
					{
						val2["p_device_id"] = ((JToken)(text));
					}
					awaiter = codeMarketService.method_0("get_my_published_snippets", val2, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0186;
				}
				result = CodeMarketResult<List<CodeSnippetMarketItem>>.Fail(val.Error ?? "认证失败，请重新登录");
				goto end_IL_000f;
				IL_0186:
				Result<JObject> result2 = awaiter.GetResult();
				object obj2;
				if (!result2.IsSuccess)
				{
					result = CodeMarketResult<List<CodeSnippetMarketItem>>.Fail(result2.Error ?? "获取发布失败");
				}
				else
				{
					JObject value = result2.Value;
					if (value != null)
					{
						JToken val3 = default(JToken);
						object obj;
						if (!value.TryGetValue("snippets", out val3))
						{
							obj = null;
						}
						else
						{
							obj = ((val3 is JArray) ? val3 : null);
							if (obj != null)
							{
								obj2 = ((IEnumerable<JToken>)obj).Select((Func<JToken, CodeSnippetMarketItem>)delegate(JToken jtoken_0)
								{
									//IL_0000: Unknown result type (might be due to invalid IL or missing references)
									//IL_0005: Unknown result type (might be due to invalid IL or missing references)
									//IL_0030: Unknown result type (might be due to invalid IL or missing references)
									//IL_005b: Unknown result type (might be due to invalid IL or missing references)
									//IL_0086: Unknown result type (might be due to invalid IL or missing references)
									//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
									//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
									//IL_0102: Unknown result type (might be due to invalid IL or missing references)
									//IL_0124: Unknown result type (might be due to invalid IL or missing references)
									//IL_0146: Unknown result type (might be due to invalid IL or missing references)
									//IL_0168: Unknown result type (might be due to invalid IL or missing references)
									//IL_018a: Unknown result type (might be due to invalid IL or missing references)
									//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
									//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
									//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
									//IL_021a: Unknown result type (might be due to invalid IL or missing references)
									//IL_0249: Unknown result type (might be due to invalid IL or missing references)
									//IL_0279: Expected O, but got Unknown
									CodeSnippetMarketItem val4 = new CodeSnippetMarketItem();
									JToken obj3 = jtoken_0[(object)"id"];
									object obj4;
									if (obj3 == null)
									{
										obj4 = null;
									}
									else
									{
										obj4 = ((object)obj3).ToString();
										if (obj4 != null)
										{
											goto IL_002b;
										}
									}
									obj4 = string.Empty;
									goto IL_002b;
									IL_0056:
									object obj5;
									val4.Name = (string)obj5;
									JToken obj6 = jtoken_0[(object)"description"];
									object obj7;
									if (obj6 == null)
									{
										obj7 = null;
									}
									else
									{
										obj7 = ((object)obj6).ToString();
										if (obj7 != null)
										{
											goto IL_0081;
										}
									}
									obj7 = string.Empty;
									goto IL_0081;
									IL_00ac:
									object obj8;
									val4.Tags = (List<string>)obj8;
									JToken obj9 = jtoken_0[(object)"category"];
									object obj10;
									if (obj9 == null)
									{
										obj10 = null;
									}
									else
									{
										obj10 = ((object)obj9).ToString();
										if (obj10 != null)
										{
											goto IL_00d7;
										}
									}
									obj10 = string.Empty;
									goto IL_00d7;
									IL_00d7:
									val4.Category = (string)obj10;
									JToken obj11 = jtoken_0[(object)"price"];
									val4.Price = ((obj11 != null) ? obj11.ToObject<decimal>() : 0m);
									JToken obj12 = jtoken_0[(object)"is_free"];
									val4.IsFree = obj12 != null && obj12.ToObject<bool>();
									JToken obj13 = jtoken_0[(object)"download_count"];
									val4.DownloadCount = ((obj13 != null) ? obj13.ToObject<int>() : 0);
									JToken obj14 = jtoken_0[(object)"view_count"];
									val4.ViewCount = ((obj14 != null) ? obj14.ToObject<int>() : 0);
									JToken obj15 = jtoken_0[(object)"favorite_count"];
									val4.FavoriteCount = ((obj15 != null) ? obj15.ToObject<int>() : 0);
									JToken obj16 = jtoken_0[(object)"average_rating"];
									val4.AverageRating = ((obj16 != null) ? obj16.ToObject<decimal>() : 0m);
									JToken obj17 = jtoken_0[(object)"rating_count"];
									val4.RatingCount = ((obj17 != null) ? obj17.ToObject<int>() : 0);
									val4.Status = ((object)jtoken_0[(object)"status"])?.ToString();
									JToken obj18 = jtoken_0[(object)"created_at"];
									val4.CreatedAt = ((obj18 != null) ? obj18.ToObject<DateTime>() : DateTime.MinValue);
									JToken obj19 = jtoken_0[(object)"updated_at"];
									val4.UpdatedAt = ((obj19 != null) ? new DateTime?(obj19.ToObject<DateTime>()) : ((DateTime?)null));
									JToken obj20 = jtoken_0[(object)"earnings"];
									val4.Earnings = ((obj20 != null) ? new int?(obj20.ToObject<int>()) : ((int?)null));
									return val4;
									IL_002b:
									val4.Id = (string)obj4;
									JToken obj21 = jtoken_0[(object)"name"];
									if (obj21 == null)
									{
										obj5 = null;
									}
									else
									{
										obj5 = ((object)obj21).ToString();
										if (obj5 != null)
										{
											goto IL_0056;
										}
									}
									obj5 = string.Empty;
									goto IL_0056;
									IL_0081:
									val4.Description = (string)obj7;
									JToken obj22 = jtoken_0[(object)"tags"];
									if (obj22 == null)
									{
										obj8 = null;
									}
									else
									{
										obj8 = obj22.ToObject<List<string>>();
										if (obj8 != null)
										{
											goto IL_00ac;
										}
									}
									obj8 = new List<string>();
									goto IL_00ac;
								}).ToList();
								if (obj2 == null)
								{
									goto IL_022c;
								}
								goto IL_0232;
							}
						}
						obj2 = null;
						goto IL_022c;
					}
					result = CodeMarketResult<List<CodeSnippetMarketItem>>.Fail("服务器返回空数据");
				}
				goto end_IL_000f;
				IL_0232:
				result = CodeMarketResult<List<CodeSnippetMarketItem>>.Ok((List<CodeSnippetMarketItem>)obj2, (string)null);
				goto end_IL_000f;
				IL_022c:
				obj2 = new List<CodeSnippetMarketItem>();
				goto IL_0232;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 获取发布失败: " + ex.Message, ex);
				result = CodeMarketResult<List<CodeSnippetMarketItem>>.Fail("获取发布失败: " + ex.Message);
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
	public struct Struct22 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<(Guid? userId, string? deviceId, Result? error)> asyncTaskMethodBuilder_0;

		public CodeMarketService codeMarketService_0;

		private Guid? nullable_0;

		private string string_0;

		private TaskAwaiter<Result<IUserIdentity?>> taskAwaiter_0;

		private TaskAwaiter<string?> taskAwaiter_1;

		private TaskAwaiter<Result<IDeviceInfo>> taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			TaskAwaiter<Result<IUserIdentity>> awaiter3;
			TaskAwaiter<string> awaiter2;
			TaskAwaiter<Result<IDeviceInfo>> awaiter;
			string result;
			(Guid?, string, Result) result2;
			Result<IUserIdentity> result3;
			Guid? obj;
			Result<IDeviceInfo> result4;
			switch (num)
			{
			default:
				awaiter3 = codeMarketService.iauthManager_0.GetCurrentUserAsync().GetAwaiter();
				if (!awaiter3.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter3;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
					return;
				}
				goto IL_007c;
			case 0:
				awaiter3 = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<Result<IUserIdentity>>);
				num = -1;
				int_0 = -1;
				goto IL_007c;
			case 1:
				awaiter2 = taskAwaiter_1;
				taskAwaiter_1 = default(TaskAwaiter<string>);
				num = -1;
				int_0 = -1;
				goto IL_011a;
			case 2:
				{
					awaiter = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<Result<IDeviceInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_01d1;
				}
				IL_011a:
				result = awaiter2.GetResult();
				string_0 = result;
				if (!nullable_0.HasValue && string_0 == null)
				{
					result2 = (null, null, Result.Failure("需要用户登录或设备标识"));
					break;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					awaiter = codeMarketService.iauthManager_0.EnsureDeviceRegisteredAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_2 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_01d1;
				}
				goto IL_0220;
				IL_007c:
				result3 = awaiter3.GetResult();
				if (!result3.IsSuccess)
				{
					obj = null;
				}
				else
				{
					IUserIdentity value = result3.Value;
					obj = ((value != null) ? new Guid?(value.UserId) : ((Guid?)null));
				}
				nullable_0 = obj;
				awaiter2 = codeMarketService.ideviceService_0.GetDeviceIdAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_011a;
				IL_01d1:
				result4 = awaiter.GetResult();
				if (!result4.IsSuccess)
				{
					Logger.Error("[CodeMarketService] 设备注册失败: " + result4.Error);
					result2 = (null, null, Result.Failure("设备注册失败，请检查网络连接"));
					break;
				}
				goto IL_0220;
				IL_0220:
				result2 = (nullable_0, string_0, null);
				break;
			}
			int_0 = -2;
			string_0 = null;
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
	public struct Struct23 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<bool>> asyncTaskMethodBuilder_0;

		public int int_1;

		public string string_0;

		public CodeMarketService codeMarketService_0;

		public string string_1;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<(Guid? userId, string? deviceId, Result? error)> taskAwaiter_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Expected O, but got Unknown
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<bool> result;
			try
			{
				TaskAwaiter<(Guid?, string, Result)> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<(Guid?, string, Result)>);
					num = -1;
					int_0 = -1;
					goto IL_00de;
				}
				TaskAwaiter<Result<JObject>> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<JObject>>);
					num = -1;
					int_0 = -1;
					goto IL_0219;
				}
				if (int_1 >= 1 && int_1 <= 5)
				{
					if (!string.IsNullOrWhiteSpace(string_0))
					{
						awaiter = codeMarketService.method_1().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00de;
					}
					result = CodeMarketResult<bool>.Fail("片段 ID 不能为空");
				}
				else
				{
					result = CodeMarketResult<bool>.Fail("评分必须在 1-5 之间");
				}
				goto end_IL_000f;
				IL_00de:
				var (guid, text, val) = awaiter.GetResult();
				if (val == null)
				{
					JObject val2 = new JObject
					{
						["p_snippet_id"] = ((JToken)(string_0)),
						["p_rating"] = ((JToken)(int_1))
					};
					if (guid.HasValue)
					{
						val2["p_user_id"] = ((JToken)(guid.ToString()));
					}
					if (text != null)
					{
						val2["p_device_id"] = ((JToken)(text));
					}
					if (!string.IsNullOrWhiteSpace(string_1))
					{
						val2["p_comment"] = ((JToken)(string_1));
					}
					awaiter2 = codeMarketService.method_0("rate_snippet", val2, cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0219;
				}
				result = CodeMarketResult<bool>.Fail(val.Error ?? "认证失败，请重新登录");
				goto end_IL_000f;
				IL_02aa:
				object obj;
				string text2 = (string)obj;
				result = CodeMarketResult<bool>.Ok(true, text2);
				goto end_IL_000f;
				IL_0219:
				Result<JObject> result2 = awaiter2.GetResult();
				if (!result2.IsSuccess)
				{
					result = CodeMarketResult<bool>.Fail(result2.Error ?? "评分失败");
				}
				else
				{
					JObject value = result2.Value;
					if (value != null)
					{
						JToken val3 = default(JToken);
						if (!value.TryGetValue("message", out val3))
						{
							obj = "评分成功";
						}
						else
						{
							if (val3 == null)
							{
								obj = null;
							}
							else
							{
								obj = ((object)val3).ToString();
								if (obj != null)
								{
									goto IL_02aa;
								}
							}
							obj = "评分成功";
						}
						goto IL_02aa;
					}
					result = CodeMarketResult<bool>.Fail("服务器返回空数据");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 评分失败: " + ex.Message, ex);
				result = CodeMarketResult<bool>.Fail("评分失败: " + ex.Message);
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
	public struct Struct24 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public HttpContent httpContent_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			TaskAwaiter<string> awaiter;
			if (int_0 != 0)
			{
				awaiter = httpContent_0.ReadAsStringAsync(cancellationToken_0).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
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
				int num = -1;
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
	public struct Struct25 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<bool>> asyncTaskMethodBuilder_0;

		public string string_0;

		public CodeMarketService codeMarketService_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<(Guid? userId, string? deviceId, Result? error)> taskAwaiter_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Expected O, but got Unknown
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<bool> result;
			try
			{
				TaskAwaiter<(Guid?, string, Result)> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<(Guid?, string, Result)>);
					num = -1;
					int_0 = -1;
					goto IL_00b4;
				}
				TaskAwaiter<Result<JObject>> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<JObject>>);
					num = -1;
					int_0 = -1;
					goto IL_01ab;
				}
				if (!string.IsNullOrWhiteSpace(string_0))
				{
					awaiter = codeMarketService.method_1().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00b4;
				}
				result = CodeMarketResult<bool>.Fail("片段 ID 不能为空");
				goto end_IL_000f;
				IL_0284:
				object obj;
				string text = (string)obj;
				bool flag;
				result = CodeMarketResult<bool>.Ok(flag, text);
				goto end_IL_000f;
				IL_00b4:
				var (guid, text2, val) = awaiter.GetResult();
				if (val == null)
				{
					JObject val2 = new JObject { ["p_snippet_id"] = ((JToken)(string_0)) };
					if (guid.HasValue)
					{
						val2["p_user_id"] = ((JToken)(guid.ToString()));
					}
					if (text2 != null)
					{
						val2["p_device_id"] = ((JToken)(text2));
					}
					awaiter2 = codeMarketService.method_0("toggle_snippet_favorite", val2, cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_01ab;
				}
				result = CodeMarketResult<bool>.Fail(val.Error ?? "认证失败，请重新登录");
				goto end_IL_000f;
				IL_01ab:
				Result<JObject> result2 = awaiter2.GetResult();
				if (!result2.IsSuccess)
				{
					result = CodeMarketResult<bool>.Fail(result2.Error ?? "收藏操作失败");
				}
				else
				{
					JObject value = result2.Value;
					if (value != null)
					{
						JToken val3 = default(JToken);
						flag = value.TryGetValue("favorited", out val3) && val3 != null && val3.ToObject<bool>();
						JToken val4 = default(JToken);
						if (!value.TryGetValue("message", out val4))
						{
							obj = (flag ? "收藏成功" : "已取消收藏");
						}
						else
						{
							if (val4 == null)
							{
								obj = null;
							}
							else
							{
								obj = ((object)val4).ToString();
								if (obj != null)
								{
									goto IL_0284;
								}
							}
							obj = (flag ? "收藏成功" : "已取消收藏");
						}
						goto IL_0284;
					}
					result = CodeMarketResult<bool>.Fail("服务器返回空数据");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 收藏操作失败: " + ex.Message, ex);
				result = CodeMarketResult<bool>.Fail("收藏操作失败: " + ex.Message);
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
	public struct Struct26 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<bool>> asyncTaskMethodBuilder_0;

		public string string_0;

		public CodeMarketService codeMarketService_0;

		public string string_1;

		public string string_2;

		public List<string> list_0;

		public string string_3;

		public int? nullable_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<(Guid? userId, string? deviceId, Result? error)> taskAwaiter_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Expected O, but got Unknown
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<bool> result;
			try
			{
				TaskAwaiter<(Guid?, string, Result)> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<(Guid?, string, Result)>);
					num = -1;
					int_0 = -1;
					goto IL_00b4;
				}
				TaskAwaiter<Result<JObject>> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<JObject>>);
					num = -1;
					int_0 = -1;
					goto IL_0278;
				}
				if (!string.IsNullOrWhiteSpace(string_0))
				{
					awaiter = codeMarketService.method_1().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00b4;
				}
				result = CodeMarketResult<bool>.Fail("片段 ID 不能为空");
				goto end_IL_000f;
				IL_0309:
				object obj;
				string text = (string)obj;
				result = CodeMarketResult<bool>.Ok(true, text);
				goto end_IL_000f;
				IL_00b4:
				var (guid, text2, val) = awaiter.GetResult();
				if (val == null)
				{
					JObject val2 = new JObject { ["p_snippet_id"] = ((JToken)(string_0)) };
					if (guid.HasValue)
					{
						val2["p_user_id"] = ((JToken)(guid.ToString()));
					}
					if (text2 != null)
					{
						val2["p_device_id"] = ((JToken)(text2));
					}
					if (!string.IsNullOrWhiteSpace(string_1))
					{
						val2["p_name"] = ((JToken)(string_1));
					}
					if (!string.IsNullOrWhiteSpace(string_2))
					{
						val2["p_description"] = ((JToken)(string_2));
					}
					if (list_0 != null)
					{
						val2["p_tags"] = (JToken)(object)JArray.FromObject((object)list_0);
					}
					if (!string.IsNullOrWhiteSpace(string_3))
					{
						val2["p_category"] = ((JToken)(string_3));
					}
					if (nullable_0.HasValue)
					{
						val2["p_price"] = ((JToken)(nullable_0.Value));
					}
					awaiter2 = codeMarketService.method_0("update_snippet_in_market", val2, cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0278;
				}
				result = CodeMarketResult<bool>.Fail(val.Error ?? "认证失败，请重新登录");
				goto end_IL_000f;
				IL_0278:
				Result<JObject> result2 = awaiter2.GetResult();
				if (!result2.IsSuccess)
				{
					result = CodeMarketResult<bool>.Fail(result2.Error ?? "更新失败");
				}
				else
				{
					JObject value = result2.Value;
					if (value != null)
					{
						JToken val3 = default(JToken);
						if (!value.TryGetValue("message", out val3))
						{
							obj = "更新成功";
						}
						else
						{
							if (val3 == null)
							{
								obj = null;
							}
							else
							{
								obj = ((object)val3).ToString();
								if (obj != null)
								{
									goto IL_0309;
								}
							}
							obj = "更新成功";
						}
						goto IL_0309;
					}
					result = CodeMarketResult<bool>.Fail("服务器返回空数据");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 更新片段失败: " + ex.Message, ex);
				result = CodeMarketResult<bool>.Fail("更新失败: " + ex.Message);
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
	public struct Struct27 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CodeMarketResult<CodeSnippetUploadResult>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public CodeMarketService codeMarketService_0;

		public string string_2;

		public List<string> list_0;

		public string string_3;

		public decimal decimal_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<(Guid? userId, string? deviceId, Result? error)> taskAwaiter_0;

		private TaskAwaiter<Result<JObject>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Expected O, but got Unknown
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Expected O, but got Unknown
			int num = int_0;
			CodeMarketService codeMarketService = codeMarketService_0;
			CodeMarketResult<CodeSnippetUploadResult> result;
			try
			{
				TaskAwaiter<(Guid?, string, Result)> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<(Guid?, string, Result)>);
					num = -1;
					int_0 = -1;
					goto IL_00d6;
				}
				TaskAwaiter<Result<JObject>> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<JObject>>);
					num = -1;
					int_0 = -1;
					goto IL_0274;
				}
				if (string.IsNullOrWhiteSpace(string_0))
				{
					result = CodeMarketResult<CodeSnippetUploadResult>.Fail("片段名称不能为空");
				}
				else
				{
					if (!string.IsNullOrWhiteSpace(string_1))
					{
						awaiter = codeMarketService.method_1().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00d6;
					}
					result = CodeMarketResult<CodeSnippetUploadResult>.Fail("代码内容不能为空");
				}
				goto end_IL_000f;
				IL_033a:
				object val = default;
				object obj;
				((CodeSnippetUploadResult)val).AuthorName = (string)obj;
				JObject value;
				JToken val2 = default(JToken);
				result = CodeMarketResult<CodeSnippetUploadResult>.Ok((CodeSnippetUploadResult)val, (!value.TryGetValue("message", out val2)) ? string.Empty : ((object)val2)?.ToString());
				goto end_IL_000f;
				IL_0274:
				Result<JObject> result2 = awaiter2.GetResult();
				object obj2;
				if (!result2.IsSuccess)
				{
					result = CodeMarketResult<CodeSnippetUploadResult>.Fail(result2.Error ?? "上传失败");
				}
				else
				{
					value = result2.Value;
					if (value != null)
					{
						val = new CodeSnippetUploadResult();
						JToken val3 = default(JToken);
						if (!value.TryGetValue("snippet_id", out val3))
						{
							obj2 = string.Empty;
						}
						else
						{
							if (val3 == null)
							{
								obj2 = null;
							}
							else
							{
								obj2 = ((object)val3).ToString();
								if (obj2 != null)
								{
									goto IL_0301;
								}
							}
							obj2 = string.Empty;
						}
						goto IL_0301;
					}
					result = CodeMarketResult<CodeSnippetUploadResult>.Fail("服务器返回空数据");
				}
				goto end_IL_000f;
				IL_0301:
				((CodeSnippetUploadResult)val).SnippetId = (string)obj2;
				JToken val4 = default(JToken);
				if (!value.TryGetValue("author_name", out val4))
				{
					obj = string.Empty;
				}
				else
				{
					if (val4 == null)
					{
						obj = null;
					}
					else
					{
						obj = ((object)val4).ToString();
						if (obj != null)
						{
							goto IL_033a;
						}
					}
					obj = string.Empty;
				}
				goto IL_033a;
				IL_00d6:
				var (guid, text, val5) = awaiter.GetResult();
				if (val5 == null)
				{
					JObject val6 = new JObject
					{
						["p_name"] = ((JToken)(string_0)),
						["p_description"] = ((JToken)(string_2 ?? string.Empty)),
						["p_code_content"] = ((JToken)(string_1)),
						["p_tags"] = (JToken)(object)JArray.FromObject((object)(list_0 ?? new List<string>())),
						["p_category"] = ((JToken)(string_3 ?? "other")),
						["p_price"] = ((JToken)(decimal_0))
					};
					if (guid.HasValue)
					{
						val6["p_user_id"] = ((JToken)(guid.ToString()));
					}
					if (text != null)
					{
						val6["p_device_id"] = ((JToken)(text));
					}
					awaiter2 = codeMarketService.method_0("upload_snippet_to_market", val6, cancellationToken_0).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0274;
				}
				result = CodeMarketResult<CodeSnippetUploadResult>.Fail(val5.Error ?? "认证失败，请重新登录");
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeMarketService] 上传片段失败: " + ex.Message, ex);
				result = CodeMarketResult<CodeSnippetUploadResult>.Fail("上传失败: " + ex.Message);
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

	private readonly ISupabaseClient isupabaseClient_0;

	private readonly IAuthManager iauthManager_0;

	private readonly IDeviceService ideviceService_0;

	public CodeMarketService(ISupabaseClient supabaseClient, IAuthManager authManager, IDeviceService deviceService)
	{
		isupabaseClient_0 = supabaseClient ?? throw new ArgumentNullException("supabaseClient");
		iauthManager_0 = authManager ?? throw new ArgumentNullException("authManager");
		ideviceService_0 = deviceService ?? throw new ArgumentNullException("deviceService");
	}

	[AsyncStateMachine(typeof(Struct16))]
	private Task<Result<JObject>> method_0(string string_0, JObject jobject_0, CancellationToken cancellationToken_0 = default(CancellationToken))
	{
		Struct16 stateMachine = default(Struct16);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<JObject>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.string_0 = string_0;
		stateMachine.jobject_0 = jobject_0;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct22))]
	private Task<(Guid? userId, string? deviceId, Result? error)> method_1()
	{
		Struct22 stateMachine = default(Struct22);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<(Guid?, string, Result)>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct24))]
	private static Task<string> smethod_0(HttpContent httpContent_0, CancellationToken cancellationToken_0)
	{
		Struct24 stateMachine = default(Struct24);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.httpContent_0 = httpContent_0;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct27))]
	public Task<CodeMarketResult<CodeSnippetUploadResult>> UploadSnippetAsync(string name, string description, string codeContent, List<string> tags, string category, decimal price, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct27 stateMachine = default(Struct27);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<CodeSnippetUploadResult>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.string_0 = name;
		stateMachine.string_2 = description;
		stateMachine.string_1 = codeContent;
		stateMachine.list_0 = tags;
		stateMachine.string_3 = category;
		stateMachine.decimal_0 = price;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct15))]
	public Task<CodeMarketResult<CodeMarketSearchResult>> BrowseMarketAsync(CodeMarketSearchOptions options, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct15 stateMachine = default(Struct15);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<CodeMarketSearchResult>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.codeMarketSearchOptions_0 = options;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct19))]
	public Task<CodeMarketResult<CodeSnippetDownloadResult>> DownloadSnippetAsync(string snippetId, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct19 stateMachine = default(Struct19);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<CodeSnippetDownloadResult>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.string_0 = snippetId;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct25))]
	public Task<CodeMarketResult<bool>> ToggleFavoriteAsync(string snippetId, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct25 stateMachine = default(Struct25);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<bool>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.string_0 = snippetId;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct20))]
	public Task<CodeMarketResult<List<CodeSnippetMarketItem>>> GetMyFavoritesAsync(int limit = 20, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct20 stateMachine = default(Struct20);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<List<CodeSnippetMarketItem>>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.int_1 = limit;
		stateMachine.int_2 = offset;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct17))]
	public Task<CodeMarketResult<HashSet<string>>> CheckFavoritedStatusAsync(List<string> snippetIds, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct17 stateMachine = default(Struct17);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<HashSet<string>>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.list_0 = snippetIds;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct23))]
	public Task<CodeMarketResult<bool>> RateSnippetAsync(string snippetId, int rating, string? comment = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct23 stateMachine = default(Struct23);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<bool>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.string_0 = snippetId;
		stateMachine.int_1 = rating;
		stateMachine.string_1 = comment;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct21))]
	public Task<CodeMarketResult<List<CodeSnippetMarketItem>>> GetMyPublishedSnippetsAsync(bool includeDeleted = false, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct21 stateMachine = default(Struct21);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<List<CodeSnippetMarketItem>>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.bool_0 = includeDeleted;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct18))]
	public Task<CodeMarketResult<bool>> DeleteSnippetAsync(string snippetId, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct18 stateMachine = default(Struct18);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<bool>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.string_0 = snippetId;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct26))]
	public Task<CodeMarketResult<bool>> UpdateSnippetAsync(string snippetId, string? name = null, string? description = null, List<string>? tags = null, string? category = null, int? price = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct26 stateMachine = default(Struct26);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CodeMarketResult<bool>>.Create();
		stateMachine.codeMarketService_0 = this;
		stateMachine.string_0 = snippetId;
		stateMachine.string_1 = name;
		stateMachine.string_2 = description;
		stateMachine.list_0 = tags;
		stateMachine.string_3 = category;
		stateMachine.nullable_0 = price;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
