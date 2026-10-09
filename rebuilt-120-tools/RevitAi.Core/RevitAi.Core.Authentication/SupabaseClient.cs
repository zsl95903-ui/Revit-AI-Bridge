using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Network;
using RevitAi.Core.AI;
using RevitAi.Core.Authentication.Models;
using RevitAi.Core.Configuration;
using RevitAi.Core.Feedback.Models;
using RevitAi.Core.Security;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ns0;
using ns7;

namespace RevitAi.Core.Authentication;

public sealed class SupabaseClient : IDisposable, ISupabaseClient
{
	private class Class95
	{
		[CompilerGenerated]
		private string string_0 = string.Empty;

		[CompilerGenerated]
		private string string_1 = string.Empty;

		[CompilerGenerated]
		private int int_0;

		[CompilerGenerated]
		private Class96? class96_0;

		[JsonProperty("access_token")]
		public string AccessToken
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		[JsonProperty("refresh_token")]
		public string RefreshToken
		{
			[CompilerGenerated]
			get
			{
				return string_1;
			}
			[CompilerGenerated]
			set
			{
				string_1 = value;
			}
		}

		[JsonProperty("expires_in")]
		public int ExpiresIn
		{
			[CompilerGenerated]
			get
			{
				return int_0;
			}
			[CompilerGenerated]
			set
			{
				int_0 = value;
			}
		}

		[JsonProperty("user")]
		public Class96? User
		{
			[CompilerGenerated]
			get
			{
				return class96_0;
			}
			[CompilerGenerated]
			set
			{
				class96_0 = value;
			}
		}
	}

	private class Class96
	{
		[CompilerGenerated]
		private string string_0 = string.Empty;

		[CompilerGenerated]
		private string string_1 = string.Empty;

		[CompilerGenerated]
		private string? string_2;

		[CompilerGenerated]
		private string? string_3;

		[JsonProperty("id")]
		public string Id
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		[JsonProperty("email")]
		public string Email
		{
			[CompilerGenerated]
			get
			{
				return string_1;
			}
			[CompilerGenerated]
			set
			{
				string_1 = value;
			}
		}

		[JsonProperty("created_at")]
		public string? CreatedAt
		{
			[CompilerGenerated]
			get
			{
				return string_2;
			}
			[CompilerGenerated]
			set
			{
				string_2 = value;
			}
		}

		[JsonProperty("confirmed_at")]
		public string? ConfirmedAt
		{
			[CompilerGenerated]
			get
			{
				return string_3;
			}
			[CompilerGenerated]
			set
			{
				string_3 = value;
			}
		}
	}

	private class Class97
	{
		[CompilerGenerated]
		private string? string_0;

		[CompilerGenerated]
		private string string_1 = string.Empty;

		[CompilerGenerated]
		private string? string_2;

		[CompilerGenerated]
		private Class98 class98_0;

		[CompilerGenerated]
		private bool bool_0;

		[CompilerGenerated]
		private string string_3 = string.Empty;

		[JsonProperty("id")]
		public string? Id
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		[JsonProperty("api_url")]
		public string ApiUrl
		{
			[CompilerGenerated]
			get
			{
				return string_1;
			}
			[CompilerGenerated]
			set
			{
				string_1 = value;
			}
		}

		[JsonProperty("notes")]
		public string? Notes
		{
			[CompilerGenerated]
			get
			{
				return string_2;
			}
			[CompilerGenerated]
			set
			{
				string_2 = value;
			}
		}

		[JsonProperty("encrypted_key")]
		public Class98 EncryptedKey
		{
			[CompilerGenerated]
			get
			{
				return class98_0;
			}
			[CompilerGenerated]
			set
			{
				class98_0 = value;
			}
		}

		[JsonProperty("is_active")]
		public bool IsActive
		{
			[CompilerGenerated]
			get
			{
				return bool_0;
			}
			[CompilerGenerated]
			set
			{
				bool_0 = value;
			}
		}

		[JsonProperty("created_at")]
		public string CreatedAt
		{
			[CompilerGenerated]
			get
			{
				return string_3;
			}
			[CompilerGenerated]
			set
			{
				string_3 = value;
			}
		}
	}

	private class Class98
	{
		[CompilerGenerated]
		private string string_0 = string.Empty;

		[CompilerGenerated]
		private string string_1 = string.Empty;

		[CompilerGenerated]
		private string string_2 = string.Empty;

		[CompilerGenerated]
		private string string_3 = string.Empty;

		public string encrypted_data
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		public string salt
		{
			[CompilerGenerated]
			get
			{
				return string_1;
			}
			[CompilerGenerated]
			set
			{
				string_1 = value;
			}
		}

		public string iv
		{
			[CompilerGenerated]
			get
			{
				return string_2;
			}
			[CompilerGenerated]
			set
			{
				string_2 = value;
			}
		}

		public string tag
		{
			[CompilerGenerated]
			get
			{
				return string_3;
			}
			[CompilerGenerated]
			set
			{
				string_3 = value;
			}
		}
	}

	private class Class99
	{
		[CompilerGenerated]
		private bool bool_0;

		[CompilerGenerated]
		private int int_0;

		[CompilerGenerated]
		private int int_1;

		[CompilerGenerated]
		private string string_0 = string.Empty;

		public bool can_use
		{
			[CompilerGenerated]
			get
			{
				return bool_0;
			}
			[CompilerGenerated]
			set
			{
				bool_0 = value;
			}
		}

		public int remaining_count
		{
			[CompilerGenerated]
			get
			{
				return int_0;
			}
			[CompilerGenerated]
			set
			{
				int_0 = value;
			}
		}

		public int max_usage_count
		{
			[CompilerGenerated]
			get
			{
				return int_1;
			}
			[CompilerGenerated]
			set
			{
				int_1 = value;
			}
		}

		public string reason
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}
	}

	private class Class100
	{
		[CompilerGenerated]
		private string string_0 = string.Empty;

		[CompilerGenerated]
		private string? string_1;

		[CompilerGenerated]
		private string? string_2;

		[CompilerGenerated]
		private decimal decimal_0;

		[CompilerGenerated]
		private decimal decimal_1;

		[CompilerGenerated]
		private decimal decimal_2;

		[CompilerGenerated]
		private string? string_3;

		[CompilerGenerated]
		private string? string_4;

		[JsonProperty("id")]
		public string id
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		[JsonProperty("user_id")]
		public string? user_id
		{
			[CompilerGenerated]
			get
			{
				return string_1;
			}
			[CompilerGenerated]
			set
			{
				string_1 = value;
			}
		}

		[JsonProperty("device_id")]
		public string? device_id
		{
			[CompilerGenerated]
			get
			{
				return string_2;
			}
			[CompilerGenerated]
			set
			{
				string_2 = value;
			}
		}

		[JsonProperty("balance")]
		public decimal balance
		{
			[CompilerGenerated]
			get
			{
				return decimal_0;
			}
			[CompilerGenerated]
			set
			{
				decimal_0 = value;
			}
		}

		[JsonProperty("total_purchased")]
		public decimal total_purchased
		{
			[CompilerGenerated]
			get
			{
				return decimal_1;
			}
			[CompilerGenerated]
			set
			{
				decimal_1 = value;
			}
		}

		[JsonProperty("total_consumed")]
		public decimal total_consumed
		{
			[CompilerGenerated]
			get
			{
				return decimal_2;
			}
			[CompilerGenerated]
			set
			{
				decimal_2 = value;
			}
		}

		[JsonProperty("last_recharge_at")]
		public string? last_recharge_at
		{
			[CompilerGenerated]
			get
			{
				return string_3;
			}
			[CompilerGenerated]
			set
			{
				string_3 = value;
			}
		}

		[JsonProperty("last_usage_at")]
		public string? last_usage_at
		{
			[CompilerGenerated]
			get
			{
				return string_4;
			}
			[CompilerGenerated]
			set
			{
				string_4 = value;
			}
		}
	}

	private class Class101
	{
		[CompilerGenerated]
		private string string_0 = string.Empty;

		[CompilerGenerated]
		private string? string_1;

		[CompilerGenerated]
		private string? string_2;

		[CompilerGenerated]
		private string string_3 = string.Empty;

		[CompilerGenerated]
		private string string_4 = string.Empty;

		[CompilerGenerated]
		private int int_0;

		[CompilerGenerated]
		private int int_1;

		[CompilerGenerated]
		private int int_2;

		[CompilerGenerated]
		private decimal decimal_0;

		[CompilerGenerated]
		private string string_5 = string.Empty;

		[JsonProperty("id")]
		public string id
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		[JsonProperty("user_id")]
		public string? user_id
		{
			[CompilerGenerated]
			get
			{
				return string_1;
			}
			[CompilerGenerated]
			set
			{
				string_1 = value;
			}
		}

		[JsonProperty("device_id")]
		public string? device_id
		{
			[CompilerGenerated]
			get
			{
				return string_2;
			}
			[CompilerGenerated]
			set
			{
				string_2 = value;
			}
		}

		[JsonProperty("provider")]
		public string provider
		{
			[CompilerGenerated]
			get
			{
				return string_3;
			}
			[CompilerGenerated]
			set
			{
				string_3 = value;
			}
		}

		[JsonProperty("model_name")]
		public string model_name
		{
			[CompilerGenerated]
			get
			{
				return string_4;
			}
			[CompilerGenerated]
			set
			{
				string_4 = value;
			}
		}

		[JsonProperty("input_tokens")]
		public int input_tokens
		{
			[CompilerGenerated]
			get
			{
				return int_0;
			}
			[CompilerGenerated]
			set
			{
				int_0 = value;
			}
		}

		[JsonProperty("output_tokens")]
		public int output_tokens
		{
			[CompilerGenerated]
			get
			{
				return int_1;
			}
			[CompilerGenerated]
			set
			{
				int_1 = value;
			}
		}

		[JsonProperty("total_tokens")]
		public int total_tokens
		{
			[CompilerGenerated]
			get
			{
				return int_2;
			}
			[CompilerGenerated]
			set
			{
				int_2 = value;
			}
		}

		[JsonProperty("total_cost")]
		public decimal total_cost
		{
			[CompilerGenerated]
			get
			{
				return decimal_0;
			}
			[CompilerGenerated]
			set
			{
				decimal_0 = value;
			}
		}

		[JsonProperty("created_at")]
		public string created_at
		{
			[CompilerGenerated]
			get
			{
				return string_5;
			}
			[CompilerGenerated]
			set
			{
				string_5 = value;
			}
		}
	}

	private class Class102
	{
		[CompilerGenerated]
		private string string_0 = string.Empty;

		[JsonProperty("id")]
		public string id
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}
	}

	private class Class103
	{
		[CompilerGenerated]
		private bool bool_0;

		[CompilerGenerated]
		private object? object_0;

		[CompilerGenerated]
		private string? string_0;

		[CompilerGenerated]
		private decimal? nullable_0;

		[CompilerGenerated]
		private decimal? nullable_1;

		public bool is_valid
		{
			[CompilerGenerated]
			get
			{
				return bool_0;
			}
			[CompilerGenerated]
			set
			{
				bool_0 = value;
			}
		}

		public object? promotion_code
		{
			[CompilerGenerated]
			get
			{
				return object_0;
			}
			[CompilerGenerated]
			set
			{
				object_0 = value;
			}
		}

		public string? error_message
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		public decimal? discounted_price
		{
			[CompilerGenerated]
			get
			{
				return nullable_0;
			}
			[CompilerGenerated]
			set
			{
				nullable_0 = value;
			}
		}

		public decimal? saved_amount
		{
			[CompilerGenerated]
			get
			{
				return nullable_1;
			}
			[CompilerGenerated]
			set
			{
				nullable_1 = value;
			}
		}
	}

	private class Class104
	{
		[CompilerGenerated]
		private bool bool_0;

		[CompilerGenerated]
		private string? string_0;

		public bool success
		{
			[CompilerGenerated]
			get
			{
				return bool_0;
			}
			[CompilerGenerated]
			set
			{
				bool_0 = value;
			}
		}

		public string? error
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}
	}

	[CompilerGenerated]
	private static class Class105
	{
		public static CallSite<Func<CallSite, object, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, bool>> callSite_1;

		public static CallSite<Func<CallSite, object, object>> callSite_2;

		public static CallSite<Func<CallSite, object, object>> callSite_3;

		public static CallSite<Func<CallSite, object, string>> callSite_4;

		public static CallSite<Func<CallSite, object, object>> callSite_5;

		public static CallSite<Func<CallSite, object, object>> callSite_6;

		public static CallSite<Func<CallSite, object, string>> callSite_7;
	}

	[CompilerGenerated]
	private static class Class106
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, object>> callSite_1;

		public static CallSite<Func<CallSite, object, bool>> callSite_2;

		public static CallSite<Func<CallSite, object, object>> callSite_3;

		public static CallSite<Func<CallSite, object, object>> callSite_4;

		public static CallSite<Func<CallSite, object, string>> callSite_5;
	}

	[CompilerGenerated]
	private static class Class107
	{
		public static CallSite<Func<CallSite, object, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, bool>> callSite_1;

		public static CallSite<Func<CallSite, object, object>> callSite_2;

		public static CallSite<Func<CallSite, object, string, object>> callSite_3;

		public static CallSite<Func<CallSite, object, string, object>> callSite_4;

		public static CallSite<Func<CallSite, object, object, object>> callSite_5;

		public static CallSite<Func<CallSite, object, bool>> callSite_6;

		public static CallSite<Func<CallSite, object, bool>> callSite_7;

		public static CallSite<Func<CallSite, object, object>> callSite_8;

		public static CallSite<Func<CallSite, object, int, object>> callSite_9;

		public static CallSite<Func<CallSite, object, int, object, object>> callSite_10;

		public static CallSite<Func<CallSite, object, string>> callSite_11;

		public static CallSite<Func<CallSite, object, object, object>> callSite_12;

		public static CallSite<Func<CallSite, object, bool>> callSite_13;

		public static CallSite<Func<CallSite, object, object, object>> callSite_14;

		public static CallSite<Func<CallSite, object, bool>> callSite_15;

		public static CallSite<Func<CallSite, object, object>> callSite_16;

		public static CallSite<Func<CallSite, object, object>> callSite_17;

		public static CallSite<Func<CallSite, object, object>> callSite_18;

		public static CallSite<Func<CallSite, object, object>> callSite_19;

		public static CallSite<Func<CallSite, object, object>> callSite_20;

		public static CallSite<Func<CallSite, object, object>> callSite_21;

		public static CallSite<Func<CallSite, object, object>> callSite_22;

		public static CallSite<Func<CallSite, object, object>> callSite_23;

		public static CallSite<Func<CallSite, object, bool, object>> callSite_24;

		public static CallSite<Func<CallSite, object, object>> callSite_25;

		public static CallSite<Func<CallSite, object, object, object>> callSite_26;

		public static CallSite<Func<CallSite, object, object, object>> callSite_27;

		public static CallSite<Func<CallSite, object, bool>> callSite_28;

		public static CallSite<Func<CallSite, bool, object, object>> callSite_29;

		public static CallSite<Func<CallSite, object, bool>> callSite_30;

		public static CallSite<Func<CallSite, object, object>> callSite_31;
	}

	[CompilerGenerated]
	private static class Class108
	{
		public static CallSite<Func<CallSite, object, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, bool>> callSite_1;

		public static CallSite<Func<CallSite, object, object>> callSite_2;

		public static CallSite<Func<CallSite, object, string, object>> callSite_3;

		public static CallSite<Func<CallSite, object, string, object>> callSite_4;

		public static CallSite<Func<CallSite, object, object, object>> callSite_5;

		public static CallSite<Func<CallSite, object, bool>> callSite_6;

		public static CallSite<Func<CallSite, object, bool>> callSite_7;

		public static CallSite<Func<CallSite, object, object>> callSite_8;

		public static CallSite<Func<CallSite, object, int, object>> callSite_9;

		public static CallSite<Func<CallSite, object, int, object, object>> callSite_10;

		public static CallSite<Func<CallSite, object, string>> callSite_11;

		public static CallSite<Func<CallSite, object, object, object>> callSite_12;

		public static CallSite<Func<CallSite, object, bool>> callSite_13;

		public static CallSite<Func<CallSite, object, object, object>> callSite_14;

		public static CallSite<Func<CallSite, object, bool>> callSite_15;

		public static CallSite<Func<CallSite, object, object>> callSite_16;

		public static CallSite<Func<CallSite, object, object>> callSite_17;

		public static CallSite<Func<CallSite, object, object>> callSite_18;

		public static CallSite<Func<CallSite, object, object>> callSite_19;

		public static CallSite<Func<CallSite, object, object>> callSite_20;

		public static CallSite<Func<CallSite, object, object>> callSite_21;

		public static CallSite<Func<CallSite, object, object>> callSite_22;

		public static CallSite<Func<CallSite, object, object>> callSite_23;

		public static CallSite<Func<CallSite, object, bool, object>> callSite_24;

		public static CallSite<Func<CallSite, object, object>> callSite_25;

		public static CallSite<Func<CallSite, object, object, object>> callSite_26;

		public static CallSite<Func<CallSite, object, object, object>> callSite_27;

		public static CallSite<Func<CallSite, object, bool>> callSite_28;

		public static CallSite<Func<CallSite, bool, object, object>> callSite_29;

		public static CallSite<Func<CallSite, object, bool>> callSite_30;

		public static CallSite<Func<CallSite, object, object>> callSite_31;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct117 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public Guid? nullable_0;

		public string string_2;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result2;
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
						goto IL_019c;
					}
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class55<string, string, Guid?, string>(string_0.Trim(), string_1, nullable_0, string_2)), Encoding.UTF8, "application/json");
					HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/activate_promotion_code")
					{
						Content = content
					};
					if (!string.IsNullOrEmpty(supabaseClient.string_1))
					{
						httpRequestMessage.Headers.Add("Authorization", "Bearer " + supabaseClient.string_1);
						httpRequestMessage.Headers.Add("apikey", supabaseClient.string_1);
					}
					awaiter2 = supabaseClient.httpClient_0.SendAsync(httpRequestMessage).GetAwaiter();
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
				goto IL_019c;
				IL_0238:
				object obj;
				result2 = Result.Failure((string)obj);
				goto end_IL_000f;
				IL_019c:
				string result3 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Warning("[Supabase] 激活优惠码失败: " + result3);
					result2 = Result.Failure("激活失败：" + result3);
				}
				else
				{
					Class104 @class = JsonConvert.DeserializeObject<Class104>(result3);
					if (@class == null || !@class.success)
					{
						if (@class == null)
						{
							obj = null;
						}
						else
						{
							obj = @class.error;
							if (obj != null)
							{
								goto IL_0238;
							}
						}
						obj = "激活失败";
						goto IL_0238;
					}
					Logger.Info("[Supabase] 优惠码激活成功: " + string_0);
					result2 = Result.Success();
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 激活优惠码异常", ex);
				result2 = Result.Failure("激活异常：" + ex.Message);
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
	public struct Struct118 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public Guid guid_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_014f;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01a4;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					Class35<string, Guid> @class = new Class35<string, Guid>(string_0, guid_0);
					string requestUri = "/rest/v1/rpc/bind_device_to_user";
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_014f;
				}
				result = Result.Failure("设备 ID 不能为空");
				goto end_IL_000f;
				IL_014f:
				HttpResponseMessage result2 = awaiter.GetResult();
				httpResponseMessage_0 = result2;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_01a4;
				IL_0402:
				dynamic val;
				string text = val;
				bool flag;
				if (!flag)
				{
					Logger.Error("[Supabase] 绑定设备失败: " + text);
					result = Result.Failure("绑定设备失败：" + text);
				}
				else
				{
					result = Result.Success();
				}
				goto end_IL_000f;
				IL_030c:
				dynamic val2;
				flag = val2;
				if (Class106.callSite_3 == null)
				{
					Class106.callSite_3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "message", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				object arg;
				object obj = Class106.callSite_3.Target(Class106.callSite_3, arg);
				if (obj == null)
				{
					val = null;
				}
				else
				{
					if (Class106.callSite_4 == null)
					{
						Class106.callSite_4 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					val = Class106.callSite_4.Target(Class106.callSite_4, obj);
					if ((object)val != null)
					{
						goto IL_0402;
					}
				}
				val = string.Empty;
				goto IL_0402;
				IL_01a4:
				string result3 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] RPC 绑定设备失败: " + result3);
					result = Result.Failure("RPC 绑定设备失败：" + result3);
				}
				else
				{
					object[] array = JsonConvert.DeserializeObject<object[]>(result3);
					if (array != null && array.Length != 0)
					{
						arg = array[0];
						if (Class106.callSite_0 == null)
						{
							Class106.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "success", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						obj = Class106.callSite_0.Target(Class106.callSite_0, arg);
						if (obj == null)
						{
							val2 = null;
						}
						else
						{
							if (Class106.callSite_1 == null)
							{
								Class106.callSite_1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToObject", new Type[1] { typeof(bool) }, typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							val2 = Class106.callSite_1.Target(Class106.callSite_1, obj);
							if ((object)val2 != null)
							{
								goto IL_030c;
							}
						}
						val2 = false;
						goto IL_030c;
					}
					result = Result.Failure("绑定设备：无效的响应");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 绑定设备异常", ex);
				result = Result.Failure("绑定设备异常：" + ex.Message);
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
	public struct Struct119 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<bool>> asyncTaskMethodBuilder_0;

		public decimal decimal_0;

		public SupabaseClient supabaseClient_0;

		public Guid? nullable_0;

		public Guid? nullable_1;

		private TaskAwaiter<Result<CombinedCreditsInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<bool> result;
			try
			{
				TaskAwaiter<Result<CombinedCreditsInfo>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<CombinedCreditsInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_0093;
				}
				if (!(decimal_0 <= 0m))
				{
					awaiter = supabaseClient.GetUserCreditsAsync(nullable_0, nullable_1).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0093;
				}
				result = Result<bool>.Success(true);
				goto end_IL_000f;
				IL_0093:
				Result<CombinedCreditsInfo> result2 = awaiter.GetResult();
				result = (result2.IsSuccess ? Result<bool>.Success((result2.Value ?? throw new InvalidOperationException("电量信息不能为空")).TotalBalance >= decimal_0) : Result<bool>.Failure(result2.Error ?? "获取电量信息失败"));
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 检查电量异常", ex);
				result = Result<bool>.Failure("检查电量异常：" + ex.Message);
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
	public struct Struct120 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<TrialEligibility>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public Guid? nullable_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<TrialEligibility> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				Class99 class2;
				HttpResponseMessage result3;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				string requestUri;
				switch (num)
				{
				default:
					if (string.IsNullOrEmpty(string_0))
					{
						result = Result<TrialEligibility>.Failure("设备 ID 不能为空");
					}
					else
					{
						if (!string.IsNullOrEmpty(string_1))
						{
							Class43<string, string, string> @class = new Class43<string, string, string>(string_0, nullable_0?.ToString(), string_1);
							HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/check_trial_eligibility")
							{
								Headers = { 
								{
									"Authorization",
									"Bearer " + supabaseClient.string_1
								} },
								Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
							};
							awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_0161;
						}
						result = Result<TrialEligibility>.Failure("功能 ID 不能为空");
					}
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0161;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01d3;
				case 2:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_032a;
				case 3:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_032a:
					awaiter = awaiter2.GetResult().Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 3;
						int_0 = 3;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					break;
					IL_01d3:
					result2 = awaiter.GetResult();
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						goto IL_0256;
					}
					class2 = JsonConvert.DeserializeObject<Class99>(result2);
					if (class2 == null)
					{
						goto IL_0256;
					}
					result = Result<TrialEligibility>.Success(new TrialEligibility
					{
						CanUse = class2.can_use,
						RemainingCount = class2.remaining_count,
						MaxUsageCount = class2.max_usage_count,
						Reason = class2.reason,
						IsFirstUse = class2.reason.Contains("首次使用")
					});
					goto end_IL_000f;
					IL_0161:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_01d3;
					IL_0256:
					Logger.Warning("[Supabase] RPC 调用失败，使用备用方案: " + result2);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(60, 2);
					defaultInterpolatedStringHandler.AppendLiteral("/rest/v1/trial_records?device_id=eq.");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral("&feature_id=eq.");
					defaultInterpolatedStringHandler.AppendFormatted(string_1);
					defaultInterpolatedStringHandler.AppendLiteral("&select=*");
					requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_032a;
				}
				List<TrialRecordDetail> list = JsonConvert.DeserializeObject<List<TrialRecordDetail>>(awaiter.GetResult());
				TrialEligibility trialEligibility;
				if (list != null && list.Count > 0)
				{
					TrialRecordDetail trialRecordDetail = list[0];
					trialEligibility = new TrialEligibility
					{
						CanUse = (trialRecordDetail.RemainingCount > 0),
						RemainingCount = trialRecordDetail.RemainingCount,
						MaxUsageCount = trialRecordDetail.MaxUsageCount,
						Reason = trialRecordDetail.GetStatusMessage(),
						IsFirstUse = false
					};
				}
				else
				{
					trialEligibility = new TrialEligibility
					{
						CanUse = true,
						RemainingCount = 10,
						MaxUsageCount = 10,
						Reason = "首次使用，默认试用次数：10",
						IsFirstUse = true
					};
				}
				result = Result<TrialEligibility>.Success(trialEligibility);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 检查试用资格异常", ex);
				result = Result<TrialEligibility>.Failure("检查试用资格异常：" + ex.Message);
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
	public struct Struct121 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<Guid>> asyncTaskMethodBuilder_0;

		public string string_0;

		public decimal decimal_0;

		public int int_1;

		public Guid? nullable_0;

		public Guid? nullable_1;

		public decimal? nullable_2;

		public int? nullable_3;

		public decimal decimal_1;

		public string string_1;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<Guid> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				Guid result3;
				HttpResponseMessage result4;
				switch (num)
				{
				default:
					if (string.IsNullOrEmpty(string_0))
					{
						result = Result<Guid>.Failure("订单号不能为空");
					}
					else if (decimal_0 <= 0m)
					{
						result = Result<Guid>.Failure("充值金额必须大于0");
					}
					else
					{
						if (int_1 > 0)
						{
							Class48<string, decimal, int, Guid?, Guid?, decimal?, int?, decimal, string> @class = new Class48<string, decimal, int, Guid?, Guid?, decimal?, int?, decimal, string>(string_0, decimal_0, int_1, nullable_0, nullable_1, nullable_2, nullable_3, decimal_1, string_1);
							HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/create_credit_recharge")
							{
								Headers = { 
								{
									"Authorization",
									"Bearer " + supabaseClient.string_1
								} },
								Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
							};
							awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_0184;
						}
						result = Result<Guid>.Failure("充值电量必须大于0");
					}
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0184;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_024d;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_024d:
					result2 = awaiter.GetResult();
					if (Guid.TryParse(result2.Trim('"'), out result3))
					{
						result = Result<Guid>.Success(result3);
					}
					else
					{
						Logger.Warning("[Supabase] RPC 返回格式异常: " + result2);
						result = Result<Guid>.Failure("充值记录创建失败：返回格式异常");
					}
					goto end_IL_000f;
					IL_0184:
					result4 = awaiter2.GetResult();
					httpResponseMessage_0 = result4;
					if (httpResponseMessage_0.IsSuccessStatusCode)
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
						goto IL_024d;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				string result5 = awaiter.GetResult();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[Supabase] 创建充值记录失败: ");
				defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
				defaultInterpolatedStringHandler.AppendLiteral(" - ");
				defaultInterpolatedStringHandler.AppendFormatted(result5);
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				result = Result<Guid>.Failure("创建充值记录失败：" + result5);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 创建充值记录异常", ex);
				result = Result<Guid>.Failure("创建充值记录异常：" + ex.Message);
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
	public struct Struct122 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public string string_2;

		public Guid? nullable_0;

		public int int_1;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0152;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01a7;
				}
				if (string.IsNullOrEmpty(string_0))
				{
					result = Result.Failure("设备 ID 不能为空");
				}
				else
				{
					if (!string.IsNullOrEmpty(string_1))
					{
						StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class45<string, string, string, string, string, int, int>(string_0, string_1, string_2, nullable_0.HasValue ? nullable_0.Value.ToString() : null, "v1", 0, int_1)), Encoding.UTF8, "application/json");
						awaiter = supabaseClient.httpClient_0.PostAsync("/rest/v1/trial_records", content).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0152;
					}
					result = Result.Failure("功能 ID 不能为空");
				}
				goto end_IL_000f;
				IL_01a7:
				string result2 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					if (!result2.Contains("duplicate key") && !result2.Contains("unique constraint") && !result2.Contains("duplicate") && httpResponseMessage_0.StatusCode != HttpStatusCode.Conflict)
					{
						Logger.Error("[Supabase] 创建试用记录失败: " + result2);
						result = Result.Failure("创建试用记录失败：" + result2);
					}
					else
					{
						result = Result.Success();
					}
				}
				else
				{
					result = Result.Success();
				}
				goto end_IL_000f;
				IL_0152:
				HttpResponseMessage result3 = awaiter.GetResult();
				httpResponseMessage_0 = result3;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_01a7;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 创建试用记录异常", ex);
				result = Result.Failure("创建试用记录异常：" + ex.Message);
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
	public struct Struct123 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<CreditDeductionResult>> asyncTaskMethodBuilder_0;

		public decimal decimal_0;

		public Guid? nullable_0;

		public Guid? nullable_1;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<CreditDeductionResult> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0203;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0258;
				}
				if (decimal_0 <= 0m)
				{
					result = Result<CreditDeductionResult>.Failure("扣减金额必须大于 0");
				}
				else
				{
					if (nullable_0.HasValue || nullable_1.HasValue)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
						defaultInterpolatedStringHandler.AppendLiteral("[Supabase] 安全扣减: UserId=");
						defaultInterpolatedStringHandler.AppendFormatted(nullable_0);
						defaultInterpolatedStringHandler.AppendLiteral(", DeviceId=");
						defaultInterpolatedStringHandler.AppendFormatted(nullable_1);
						defaultInterpolatedStringHandler.AppendLiteral(", Amount=");
						defaultInterpolatedStringHandler.AppendFormatted(decimal_0, "F2");
						Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
						string content = JsonConvert.SerializeObject((object)new Class47<Guid?, Guid?, decimal>(nullable_0, nullable_1, decimal_0));
						string requestUri = "/rest/v1/rpc/deduct_credits_safe";
						HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
						{
							Headers = 
							{
								{
									"Authorization",
									"Bearer " + supabaseClient.string_1
								},
								{
									"apikey",
									supabaseClient.string_1
								}
							},
							Content = new StringContent(content, Encoding.UTF8, "application/json")
						};
						awaiter = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0203;
					}
					result = Result<CreditDeductionResult>.Failure("必须提供 userId 或 deviceId");
				}
				goto end_IL_000f;
				IL_0203:
				HttpResponseMessage result2 = awaiter.GetResult();
				httpResponseMessage_0 = result2;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_0258;
				IL_0258:
				string result3 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 安全扣减失败: " + result3);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(14, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("安全扣减失败（HTTP ");
					defaultInterpolatedStringHandler2.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler2.AppendLiteral("）：");
					defaultInterpolatedStringHandler2.AppendFormatted(result3);
					result = Result<CreditDeductionResult>.Failure(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				else
				{
					List<CreditDeductionResult> list = JsonConvert.DeserializeObject<List<CreditDeductionResult>>(result3);
					result = ((list == null || list.Count <= 0) ? Result<CreditDeductionResult>.Failure("无效的响应") : Result<CreditDeductionResult>.Success(list[0]));
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 安全扣减异常", ex);
				result = Result<CreditDeductionResult>.Failure("安全扣减异常：" + ex.Message);
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
	public struct Struct124 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<CreditDeductionResult>>> asyncTaskMethodBuilder_0;

		public decimal decimal_0;

		public SupabaseClient supabaseClient_0;

		public Guid? nullable_0;

		public Guid? nullable_1;

		private CombinedCreditsInfo combinedCreditsInfo_0;

		private decimal decimal_1;

		private List<CreditDeductionResult> list_0;

		private TaskAwaiter<Result<CombinedCreditsInfo>> taskAwaiter_0;

		private decimal decimal_2;

		private TaskAwaiter<Result<CreditDeductionResult>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<List<CreditDeductionResult>> result;
			try
			{
				TaskAwaiter<Result<CombinedCreditsInfo>> awaiter2;
				TaskAwaiter<Result<CreditDeductionResult>> awaiter;
				Result<CreditDeductionResult> result2;
				Result<CreditDeductionResult> result3;
				Result<CombinedCreditsInfo> result4;
				switch (num)
				{
				default:
					if (!(decimal_0 <= 0m))
					{
						awaiter2 = supabaseClient.GetUserCreditsAsync(nullable_0, nullable_1).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_00ab;
					}
					result = Result<List<CreditDeductionResult>>.Failure("扣减金额必须大于 0");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<CombinedCreditsInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_00ab;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<CreditDeductionResult>>);
					num = -1;
					int_0 = -1;
					goto IL_01d9;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<Result<CreditDeductionResult>>);
						num = -1;
						int_0 = -1;
						goto IL_031d;
					}
					IL_01d9:
					result2 = awaiter.GetResult();
					if (result2.IsSuccess && result2.Value != null)
					{
						list_0.Add(result2.Value);
						decimal_1 -= decimal_2;
					}
					goto IL_021d;
					IL_031d:
					result3 = awaiter.GetResult();
					if (result3.IsSuccess && result3.Value != null)
					{
						list_0.Add(result3.Value);
						decimal_1 -= decimal_2;
					}
					break;
					IL_021d:
					if (!(decimal_1 > 0.0001m) || !(nullable_1 != Guid.Empty) || !(combinedCreditsInfo_0.DeviceBalance > 0m))
					{
						break;
					}
					decimal_2 = Math.Min(combinedCreditsInfo_0.DeviceBalance, decimal_1);
					if (!(decimal_2 > 0m))
					{
						break;
					}
					awaiter = supabaseClient.DeductCreditsSafeAsync(null, nullable_1, decimal_2).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_031d;
					IL_00ab:
					result4 = awaiter2.GetResult();
					if (result4.IsSuccess)
					{
						combinedCreditsInfo_0 = result4.Value ?? throw new InvalidOperationException("电量信息不能为空");
						decimal_1 = decimal_0;
						list_0 = new List<CreditDeductionResult>();
						if (nullable_0.HasValue && combinedCreditsInfo_0.UserBalance > 0m)
						{
							decimal_2 = Math.Min(combinedCreditsInfo_0.UserBalance, decimal_1);
							if (decimal_2 > 0m)
							{
								awaiter = supabaseClient.DeductCreditsSafeAsync(nullable_0, null, decimal_2).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
									return;
								}
								goto IL_01d9;
							}
						}
						goto IL_021d;
					}
					result = Result<List<CreditDeductionResult>>.Failure(result4.Error ?? "获取电量信息失败");
					goto end_IL_000f;
				}
				if (decimal_1 > 0.0001m)
				{
					decimal value = list_0.Sum((CreditDeductionResult creditDeductionResult_0) => creditDeductionResult_0.ActualDeducted);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 5);
					defaultInterpolatedStringHandler.AppendLiteral("电量不足：需要");
					defaultInterpolatedStringHandler.AppendFormatted(decimal_0, "F2");
					defaultInterpolatedStringHandler.AppendLiteral("，可用");
					defaultInterpolatedStringHandler.AppendFormatted(combinedCreditsInfo_0.TotalBalance, "F2");
					defaultInterpolatedStringHandler.AppendLiteral("（用户");
					defaultInterpolatedStringHandler.AppendFormatted(combinedCreditsInfo_0.UserBalance, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(" + 设备");
					defaultInterpolatedStringHandler.AppendFormatted(combinedCreditsInfo_0.DeviceBalance, "F2");
					defaultInterpolatedStringHandler.AppendLiteral("），已扣减");
					defaultInterpolatedStringHandler.AppendFormatted(value, "F2");
					result = Result<List<CreditDeductionResult>>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					result = Result<List<CreditDeductionResult>>.Success(list_0);
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 智能扣减异常", ex);
				result = Result<List<CreditDeductionResult>>.Failure("智能扣减异常：" + ex.Message);
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
	public struct Struct125 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public Guid guid_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result3;
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
						goto IL_0176;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
					defaultInterpolatedStringHandler.AppendLiteral("/rest/v1/feedback?id=eq.");
					defaultInterpolatedStringHandler.AppendFormatted(guid_0);
					string requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"Prefer",
								"return=representation"
							}
						}
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_0176;
				IL_0176:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 删除反馈失败: " + result2);
					result3 = Result.Failure("删除反馈失败：" + result2);
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(19, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[Supabase] 删除反馈成功: ");
					defaultInterpolatedStringHandler2.AppendFormatted(guid_0);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					result3 = Result.Success();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 删除反馈异常", ex);
				result3 = Result.Failure("删除反馈异常：" + ex.Message);
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
	public struct Struct126 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_018e;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01e3;
				}
				if (string.IsNullOrEmpty(string_0))
				{
					result = Result.Failure("存储桶名称不能为空");
				}
				else
				{
					if (!string.IsNullOrEmpty(string_1))
					{
						string value = supabaseClient.string_0.Remove(supabaseClient.string_0.LastIndexOf("/rest"));
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 3);
						defaultInterpolatedStringHandler.AppendFormatted(value);
						defaultInterpolatedStringHandler.AppendLiteral("/storage/v1/object/");
						defaultInterpolatedStringHandler.AppendFormatted(string_0);
						defaultInterpolatedStringHandler.AppendLiteral("/");
						defaultInterpolatedStringHandler.AppendFormatted(string_1);
						string requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
						HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
						{
							Headers = { 
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							} }
						};
						awaiter = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_018e;
					}
					result = Result.Failure("文件路径不能为空");
				}
				goto end_IL_000f;
				IL_01e3:
				string result2 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 删除文件失败: " + result2);
					result = Result.Failure("删除文件失败：" + result2);
				}
				else
				{
					result = Result.Success();
				}
				goto end_IL_000f;
				IL_018e:
				HttpResponseMessage result3 = awaiter.GetResult();
				httpResponseMessage_0 = result3;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_01e3;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 删除文件异常", ex);
				result = Result.Failure("删除文件异常：" + ex.Message);
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
	public struct Struct127 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<AIConfig>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public string string_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<AIConfig> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
					supabaseClient.method_1();
					if (!string.IsNullOrEmpty(string_0))
					{
						string requestUri = "/rest/v1/ai_config?model_name=eq." + Uri.EscapeDataString(string_0) + "&is_active=eq.true&limit=1";
						awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_00cb;
					}
					result = Result<AIConfig>.Failure("模型名称不能为空");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00cb;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0194;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0194:
					result2 = awaiter.GetResult();
					Logger.Error("[Supabase] 查询 AI 配置失败: " + result2);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
					defaultInterpolatedStringHandler.AppendLiteral("查询 AI 配置失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					result = Result<AIConfig>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_00cb:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_0194;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<AIConfig> list = JsonConvert.DeserializeObject<List<AIConfig>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					result = Result<AIConfig>.Success(list[0]);
				}
				else
				{
					Logger.Warning("[Supabase] 未找到模型 " + string_0 + " 的 AI 配置");
					result = Result<AIConfig>.Failure("未找到模型 " + string_0 + " 的 AI 配置");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询 AI 配置异常", ex);
				result = Result<AIConfig>.Failure("查询 AI 配置异常：" + ex.Message);
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
	public struct Struct128 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<AIConfig>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public string string_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<AIConfig> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
					supabaseClient.method_1();
					if (!string.IsNullOrEmpty(string_0))
					{
						string requestUri = "/rest/v1/ai_config?provider=eq." + string_0 + "&is_active=eq.true&limit=1";
						awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_00c6;
					}
					result = Result<AIConfig>.Failure("供应商名称不能为空");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00c6;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_018f;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_018f:
					result2 = awaiter.GetResult();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
					defaultInterpolatedStringHandler.AppendLiteral("查询 AI 配置失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					result = Result<AIConfig>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_00c6:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_018f;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<AIConfig> list = JsonConvert.DeserializeObject<List<AIConfig>>(awaiter.GetResult());
				result = ((list == null || list.Count <= 0) ? Result<AIConfig>.Failure("未找到供应商 " + string_0 + " 的 AI 配置") : Result<AIConfig>.Success(list[0]));
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result<AIConfig>.Failure("查询 AI 配置异常：" + ex.Message);
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
	public struct Struct129 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<AIConfig>>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<List<AIConfig>> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					supabaseClient.method_1();
					string requestUri = "/rest/v1/ai_config?is_active=eq.true&order=priority.asc";
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_008f;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_008f;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0158;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0158:
					result = awaiter.GetResult();
					Logger.Error("[Supabase] 查询 AI 配置失败: " + result);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
					defaultInterpolatedStringHandler.AppendLiteral("查询 AI 配置失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					result2 = Result<List<AIConfig>>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_008f:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_0158;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<AIConfig> list = JsonConvert.DeserializeObject<List<AIConfig>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					result2 = Result<List<AIConfig>>.Success(list);
				}
				else
				{
					Logger.Warning("[Supabase] 未找到任何激活的 AI 配置");
					result2 = Result<List<AIConfig>>.Failure("未找到任何激活的 AI 配置");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询 AI 配置异常", ex);
				result2 = Result<List<AIConfig>>.Failure("查询 AI 配置异常：" + ex.Message);
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
	public struct Struct130 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<TrialLimitConfig>>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<List<TrialLimitConfig>> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					supabaseClient.method_1();
					string requestUri = "/rest/v1/max_trial_limits?is_enabled=eq.true&order=feature_id.desc.nullslast,feature_group.desc.nullslast";
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_008f;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_008f;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0158;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0158:
					result = awaiter.GetResult();
					Logger.Error("[Supabase] 查询试用配置失败: " + result);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
					defaultInterpolatedStringHandler.AppendLiteral("查询试用配置失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					result2 = Result<List<TrialLimitConfig>>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_008f:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_0158;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<TrialLimitConfig> list = JsonConvert.DeserializeObject<List<TrialLimitConfig>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					result2 = Result<List<TrialLimitConfig>>.Success(list);
				}
				else
				{
					Logger.Warning("[Supabase] 未找到任何启用的试用配置，将使用默认值");
					result2 = Result<List<TrialLimitConfig>>.Success(new List<TrialLimitConfig>());
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询试用配置异常", ex);
				result2 = Result<List<TrialLimitConfig>>.Failure("查询试用配置异常：" + ex.Message);
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
	public struct Struct131 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<CosPresignedUrlInfo>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<CosPresignedUrlInfo> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0132;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0187;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					string presignedUrlUrl = CosProxyConfig.GetPresignedUrlUrl();
					Logger.Info("[Supabase] 请求预签名 URL: " + string_0);
					Logger.Info("[Supabase] 请求地址: " + presignedUrlUrl);
					Class53<string, string> @class = new Class53<string, string>(string_0, string_1);
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, presignedUrlUrl)
					{
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0132;
				}
				result = Result<CosPresignedUrlInfo>.Failure("文件名不能为空");
				goto end_IL_000f;
				IL_02e7:
				object obj;
				string text = (string)obj;
				object value;
				object obj2;
				if (value == null)
				{
					obj2 = null;
				}
				else
				{
					obj2 = value.ToString();
					if (obj2 != null)
					{
						goto IL_0300;
					}
				}
				obj2 = string.Empty;
				goto IL_0300;
				IL_0187:
				string result2 = awaiter2.GetResult();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[Supabase] COS Proxy 响应状态: ");
				defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				object value3;
				object value4;
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 获取预签名 URL 失败: " + result2);
					result = Result<CosPresignedUrlInfo>.Failure("获取预签名 URL 失败：" + result2);
				}
				else
				{
					Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result2);
					if (dictionary == null)
					{
						Logger.Error("[Supabase] 解析响应失败: " + result2);
						result = Result<CosPresignedUrlInfo>.Failure("解析响应失败");
					}
					else
					{
						if (dictionary.ContainsKey("success") && !(dictionary["success"].ToString() != "True"))
						{
							dictionary.TryGetValue("presignedUrl", out var value2);
							dictionary.TryGetValue("objectKey", out value);
							dictionary.TryGetValue("publicUrl", out value3);
							dictionary.TryGetValue("expiresIn", out value4);
							if (value2 == null)
							{
								obj = null;
							}
							else
							{
								obj = value2.ToString();
								if (obj != null)
								{
									goto IL_02e7;
								}
							}
							obj = string.Empty;
							goto IL_02e7;
						}
						result = Result<CosPresignedUrlInfo>.Failure("获取预签名 URL 失败");
					}
				}
				goto end_IL_000f;
				IL_0300:
				string text2 = (string)obj2;
				object obj3;
				if (value3 == null)
				{
					obj3 = null;
				}
				else
				{
					obj3 = value3.ToString();
					if (obj3 != null)
					{
						goto IL_0319;
					}
				}
				obj3 = string.Empty;
				goto IL_0319;
				IL_0337:
				object obj4;
				string s = (string)obj4;
				string publicUrl;
				if (string.IsNullOrEmpty(text))
				{
					Logger.Error("[Supabase] 预签名 URL 为空: " + result2);
					result = Result<CosPresignedUrlInfo>.Failure("预签名 URL 为空");
				}
				else
				{
					CosPresignedUrlInfo obj5 = new CosPresignedUrlInfo
					{
						PresignedUrl = text,
						ObjectKey = text2,
						PublicUrl = publicUrl,
						ExpiresIn = int.Parse(s)
					};
					Logger.Info("[Supabase] 成功获取预签名 URL: " + text2);
					result = Result<CosPresignedUrlInfo>.Success(obj5);
				}
				goto end_IL_000f;
				IL_0319:
				publicUrl = (string)obj3;
				if (value4 == null)
				{
					obj4 = null;
				}
				else
				{
					obj4 = value4.ToString();
					if (obj4 != null)
					{
						goto IL_0337;
					}
				}
				obj4 = "3600";
				goto IL_0337;
				IL_0132:
				HttpResponseMessage result3 = awaiter.GetResult();
				httpResponseMessage_0 = result3;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_0187;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 获取预签名 URL 异常", ex);
				result = Result<CosPresignedUrlInfo>.Failure("获取预签名 URL 异常：" + ex.Message);
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
	public struct Struct132 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<AIConfig>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<AIConfig> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					string requestUri = "/rest/v1/ai_config?is_default=eq.true&is_active=eq.true&limit=1";
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0089;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0089;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0152;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0152:
					result = awaiter.GetResult();
					Logger.Error("[Supabase] 查询 AI 配置失败: " + result);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
					defaultInterpolatedStringHandler.AppendLiteral("查询 AI 配置失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					result2 = Result<AIConfig>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_0089:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_0152;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<AIConfig> list = JsonConvert.DeserializeObject<List<AIConfig>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					result2 = Result<AIConfig>.Success(list[0]);
				}
				else
				{
					Logger.Warning("[Supabase] 未找到默认 AI 配置");
					result2 = Result<AIConfig>.Failure("未找到默认 AI 配置");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询 AI 配置异常", ex);
				result2 = Result<AIConfig>.Failure("查询 AI 配置异常：" + ex.Message);
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
	public struct Struct133 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<DeviceInfo>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public string string_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<DeviceInfo> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
					supabaseClient.method_1();
					if (!string.IsNullOrEmpty(string_0))
					{
						string requestUri = "/rest/v1/devices?device_id=eq." + string_0;
						awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_00bc;
					}
					result = Result<DeviceInfo>.Failure("设备 ID 不能为空");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00bc;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0185;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0185:
					result2 = awaiter.GetResult();
					Logger.Warning("[Supabase] 查找设备返回失败状态: " + result2);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
					defaultInterpolatedStringHandler.AppendLiteral("获取设备信息失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					result = Result<DeviceInfo>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_00bc:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_0185;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<DeviceInfo> list = JsonConvert.DeserializeObject<List<DeviceInfo>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					result = Result<DeviceInfo>.Success(list[0]);
				}
				else
				{
					Logger.Warning("[Supabase] 设备未找到");
					result = Result<DeviceInfo>.Failure("设备未找到");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 获取设备信息异常", ex);
				result = Result<DeviceInfo>.Failure("获取设备信息异常：" + ex.Message);
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
	public struct Struct134 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<EncryptedApiKeyRecord>> asyncTaskMethodBuilder_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<EncryptedApiKeyRecord> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
					if (!string.IsNullOrEmpty(string_0))
					{
						string requestUri = "/rest/v1/admin_api_keys?api_url=like." + Uri.EscapeDataString(string_0) + "%&is_active=eq.true&order=created_at&limit=1";
						awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_00c5;
					}
					result = Result<EncryptedApiKeyRecord>.Failure("API URL 不能为空");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00c5;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_018e;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_018e:
					result2 = awaiter.GetResult();
					Logger.Error("[Supabase] 查询 API Key 失败: " + result2);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
					defaultInterpolatedStringHandler.AppendLiteral("查询 API Key 失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					result = Result<EncryptedApiKeyRecord>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_00c5:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_018e;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<Class97> list = JsonConvert.DeserializeObject<List<Class97>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					if (list[0].EncryptedKey == null)
					{
						Logger.Error("[Supabase] encrypted_key 字段为 null");
						result = Result<EncryptedApiKeyRecord>.Failure("加密密钥数据为空");
					}
					else
					{
						EncryptedApiKey encryptedKey = new EncryptedApiKey
						{
							EncryptedData = (list[0].EncryptedKey.encrypted_data ?? string.Empty),
							Salt = (list[0].EncryptedKey.salt ?? string.Empty),
							IV = (list[0].EncryptedKey.iv ?? string.Empty),
							Tag = (list[0].EncryptedKey.tag ?? string.Empty)
						};
						result = Result<EncryptedApiKeyRecord>.Success(new EncryptedApiKeyRecord
						{
							Id = (list[0].Id ?? string.Empty),
							ApiUrl = (list[0].ApiUrl ?? string.Empty),
							Notes = (list[0].Notes ?? string.Empty),
							EncryptedKey = encryptedKey,
							IsActive = list[0].IsActive,
							CreatedAt = (list[0].CreatedAt ?? string.Empty)
						});
					}
				}
				else
				{
					Logger.Warning("[Supabase] 未找到 API URL " + string_0 + " 的加密密钥");
					result = Result<EncryptedApiKeyRecord>.Failure("未找到 API URL " + string_0 + " 的加密密钥");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询 API Key 异常", ex);
				result = Result<EncryptedApiKeyRecord>.Failure("查询 API Key 异常：" + ex.Message);
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
	public struct Struct135 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<EncryptedApiKeyRecord>> asyncTaskMethodBuilder_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<EncryptedApiKeyRecord> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
					if (!string.IsNullOrEmpty(string_0))
					{
						string requestUri = "/rest/v1/admin_api_keys?notes=eq." + Uri.EscapeDataString(string_0) + "&is_active=eq.true&limit=1";
						awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_00c5;
					}
					result = Result<EncryptedApiKeyRecord>.Failure("备注不能为空");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00c5;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_018e;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_018e:
					result2 = awaiter.GetResult();
					Logger.Error("[Supabase] 查询 API Key 失败: " + result2);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
					defaultInterpolatedStringHandler.AppendLiteral("查询 API Key 失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					result = Result<EncryptedApiKeyRecord>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_00c5:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_018e;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<Class97> list = JsonConvert.DeserializeObject<List<Class97>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					if (list[0].EncryptedKey == null)
					{
						Logger.Error("[Supabase] encrypted_key 字段为 null");
						result = Result<EncryptedApiKeyRecord>.Failure("加密密钥数据为空");
					}
					else
					{
						EncryptedApiKey encryptedKey = new EncryptedApiKey
						{
							EncryptedData = (list[0].EncryptedKey.encrypted_data ?? string.Empty),
							Salt = (list[0].EncryptedKey.salt ?? string.Empty),
							IV = (list[0].EncryptedKey.iv ?? string.Empty),
							Tag = (list[0].EncryptedKey.tag ?? string.Empty)
						};
						result = Result<EncryptedApiKeyRecord>.Success(new EncryptedApiKeyRecord
						{
							Id = (list[0].Id ?? string.Empty),
							ApiUrl = (list[0].ApiUrl ?? string.Empty),
							Notes = (list[0].Notes ?? string.Empty),
							EncryptedKey = encryptedKey,
							IsActive = list[0].IsActive,
							CreatedAt = (list[0].CreatedAt ?? string.Empty)
						});
					}
				}
				else
				{
					Logger.Warning("[Supabase] 未找到备注 " + string_0 + " 的加密密钥");
					result = Result<EncryptedApiKeyRecord>.Failure("未找到备注 " + string_0 + " 的加密密钥");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询 API Key 异常", ex);
				result = Result<EncryptedApiKeyRecord>.Failure("查询 API Key 异常：" + ex.Message);
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
	public struct Struct136 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<FeedbackInfo>>> asyncTaskMethodBuilder_0;

		public Guid? nullable_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<List<FeedbackInfo>> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					string requestUri;
					if (nullable_0.HasValue)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 1);
						defaultInterpolatedStringHandler.AppendLiteral("/rest/v1/feedback?user_id=eq.");
						defaultInterpolatedStringHandler.AppendFormatted(nullable_0.Value);
						defaultInterpolatedStringHandler.AppendLiteral("&order=created_at.desc");
						requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else
					{
						requestUri = "/rest/v1/feedback?device_id=eq." + string_0 + "&order=created_at.desc";
					}
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00f3;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00f3;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01a8;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01a8:
					result = awaiter.GetResult();
					result2 = Result<List<FeedbackInfo>>.Failure("获取反馈列表失败：" + result);
					goto end_IL_000f;
					IL_00f3:
					result3 = awaiter2.GetResult();
					if (!result3.IsSuccessStatusCode)
					{
						awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_01a8;
					}
					awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
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
				result2 = Result<List<FeedbackInfo>>.Success(JsonConvert.DeserializeObject<List<FeedbackInfo>>(awaiter.GetResult()) ?? new List<FeedbackInfo>());
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 获取反馈列表异常", ex);
				result2 = Result<List<FeedbackInfo>>.Failure("获取反馈列表异常：" + ex.Message);
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
	public struct Struct137 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<InitialCreditsInfo>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<InitialCreditsInfo> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					string requestUri = "/rest/v1/initial_credits?is_enabled=eq.true&limit=1";
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0089;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0089;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0152;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0152:
					result = awaiter.GetResult();
					Logger.Error("[Supabase] 查询初始电量配置失败: " + result);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
					defaultInterpolatedStringHandler.AppendLiteral("查询初始电量配置失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					result2 = Result<InitialCreditsInfo>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_0089:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_0152;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<InitialCreditsInfo> list = JsonConvert.DeserializeObject<List<InitialCreditsInfo>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					result2 = Result<InitialCreditsInfo>.Success(list[0]);
				}
				else
				{
					Logger.Warning("[Supabase] 未找到初始电量配置，返回默认值");
					result2 = Result<InitialCreditsInfo>.Success(new InitialCreditsInfo
					{
						Id = Guid.Empty,
						UserCredit = 0m,
						DeviceCredit = 0m,
						Description = "默认配置（未从数据库加载）",
						IsEnabled = true
					});
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询初始电量配置异常", ex);
				result2 = Result<InitialCreditsInfo>.Failure("查询初始电量配置异常：" + ex.Message);
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
	public struct Struct138 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<LicenseInfo>> asyncTaskMethodBuilder_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Expected O, but got Unknown
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<LicenseInfo> result3;
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
						goto IL_0178;
					}
					string content = JsonConvert.SerializeObject((object)new Class37<string>(string_0));
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/get_license_by_device")
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(content, Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_0178;
				IL_0178:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					string text = string_0;
					if (text != null && text.Length >= 8)
					{
						string_0.Substring(0, 8);
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[Supabase] ✗ RPC 查询许可证失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", 返回: ");
					defaultInterpolatedStringHandler.AppendFormatted(result2?.Substring(0, Math.Min(200, result2?.Length ?? 0)));
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					result3 = Result<LicenseInfo>.Failure("许可证未找到");
				}
				else
				{
					JsonSerializerSettings val = new JsonSerializerSettings
					{
						DateTimeZoneHandling = (DateTimeZoneHandling)1
					};
					List<object> list = JsonConvert.DeserializeObject<List<object>>(result2, val);
					if ((list?.Count ?? 0) > 0)
					{
						object arg = list[0];
						if (Class108.callSite_16 == null)
						{
							Class108.callSite_16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "valid_from", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						DateTime validFrom = smethod_5(Class108.callSite_16.Target(Class108.callSite_16, arg)) ?? DateTime.UtcNow;
						if (Class108.callSite_17 == null)
						{
							Class108.callSite_17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "valid_to", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						DateTime? validTo = smethod_5(Class108.callSite_17.Target(Class108.callSite_17, arg));
						LicenseInfo licenseInfo = new LicenseInfo();
						if (Class108.callSite_18 == null)
						{
							Class108.callSite_18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.LicenseId = smethod_4(Class108.callSite_18.Target(Class108.callSite_18, arg), "id");
						if (Class108.callSite_19 == null)
						{
							Class108.callSite_19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "device_uuid", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.UserId = smethod_4(Class108.callSite_19.Target(Class108.callSite_19, arg), "device_uuid");
						if (Class108.callSite_20 == null)
						{
							Class108.callSite_20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "device_id", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.DeviceFingerprint = smethod_3(Class108.callSite_20.Target(Class108.callSite_20, arg), "");
						if (Class108.callSite_21 == null)
						{
							Class108.callSite_21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "license_type", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.LicenseType = smethod_3(Class108.callSite_21.Target(Class108.callSite_21, arg), "") ?? string.Empty;
						if (Class108.callSite_22 == null)
						{
							Class108.callSite_22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "license_type", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						bool flag;
						dynamic val4;
						if (!(flag = smethod_3(Class108.callSite_22.Target(Class108.callSite_22, arg), "") == "trial"))
						{
							if (Class108.callSite_23 == null)
							{
								Class108.callSite_23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "is_permanent", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							dynamic val2 = (dynamic)Class108.callSite_23.Target(Class108.callSite_23, arg) == false;
							dynamic val3;
							if (!(val2 ? false : true))
							{
								if (Class108.callSite_25 == null)
								{
									Class108.callSite_25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "valid_to", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								val3 = val2 & ((dynamic)Class108.callSite_25.Target(Class108.callSite_25, arg) != null);
							}
							else
							{
								val3 = val2;
							}
							val4 = flag | val3;
						}
						else
						{
							val4 = flag;
						}
						licenseInfo.IsTrial = val4;
						licenseInfo.ValidFrom = validFrom;
						licenseInfo.ValidTo = validTo;
						if (Class108.callSite_31 == null)
						{
							Class108.callSite_31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.IsActive = smethod_3(Class108.callSite_31.Target(Class108.callSite_31, arg), "") == "active";
						_ = DateTime.UtcNow;
						result3 = Result<LicenseInfo>.Success(licenseInfo);
					}
					else
					{
						string text2 = string_0;
						string text3 = ((text2 != null && text2.Length >= 8) ? string_0.Substring(0, 8) : "???");
						Logger.Warning("[Supabase] ✗ 未找到授权，device_id=" + text3 + "...");
						result3 = Result<LicenseInfo>.Failure("许可证未找到");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询设备许可证异常: " + ex.Message);
				result3 = Result<LicenseInfo>.Failure("获取设备许可证异常：" + ex.Message);
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
	public struct Struct139 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<LicenseInfo>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public Guid guid_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Expected O, but got Unknown
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<LicenseInfo> result3;
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
						goto IL_017e;
					}
					supabaseClient.method_1();
					string content = JsonConvert.SerializeObject((object)new Class44<Guid>(guid_0));
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/get_license_by_user")
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(content, Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_017e;
				IL_017e:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[Supabase] ✗ RPC 查询许可证失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", 返回: ");
					defaultInterpolatedStringHandler.AppendFormatted(result2?.Substring(0, Math.Min(200, result2?.Length ?? 0)));
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					result3 = Result<LicenseInfo>.Failure("许可证未找到");
				}
				else
				{
					JsonSerializerSettings val = new JsonSerializerSettings
					{
						DateTimeZoneHandling = (DateTimeZoneHandling)1
					};
					List<object> list = JsonConvert.DeserializeObject<List<object>>(result2, val);
					if ((list?.Count ?? 0) > 0)
					{
						object arg = list[0];
						if (Class107.callSite_16 == null)
						{
							Class107.callSite_16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "valid_from", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						DateTime validFrom = smethod_2(Class107.callSite_16.Target(Class107.callSite_16, arg)) ?? DateTime.UtcNow;
						if (Class107.callSite_17 == null)
						{
							Class107.callSite_17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "valid_to", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						DateTime? validTo = smethod_2(Class107.callSite_17.Target(Class107.callSite_17, arg));
						LicenseInfo licenseInfo = new LicenseInfo();
						if (Class107.callSite_18 == null)
						{
							Class107.callSite_18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.LicenseId = smethod_1(Class107.callSite_18.Target(Class107.callSite_18, arg), "id");
						if (Class107.callSite_19 == null)
						{
							Class107.callSite_19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user_id", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.UserId = smethod_1(Class107.callSite_19.Target(Class107.callSite_19, arg), "user_id");
						if (Class107.callSite_20 == null)
						{
							Class107.callSite_20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "device_id", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.DeviceFingerprint = smethod_0(Class107.callSite_20.Target(Class107.callSite_20, arg), "");
						if (Class107.callSite_21 == null)
						{
							Class107.callSite_21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "license_type", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.LicenseType = smethod_0(Class107.callSite_21.Target(Class107.callSite_21, arg), "") ?? string.Empty;
						if (Class107.callSite_22 == null)
						{
							Class107.callSite_22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "license_type", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						bool flag;
						dynamic val4;
						if (!(flag = smethod_0(Class107.callSite_22.Target(Class107.callSite_22, arg), "") == "trial"))
						{
							if (Class107.callSite_23 == null)
							{
								Class107.callSite_23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "is_permanent", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							dynamic val2 = (dynamic)Class107.callSite_23.Target(Class107.callSite_23, arg) == false;
							dynamic val3;
							if (!(val2 ? false : true))
							{
								if (Class107.callSite_25 == null)
								{
									Class107.callSite_25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "valid_to", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								val3 = val2 & ((dynamic)Class107.callSite_25.Target(Class107.callSite_25, arg) != null);
							}
							else
							{
								val3 = val2;
							}
							val4 = flag | val3;
						}
						else
						{
							val4 = flag;
						}
						licenseInfo.IsTrial = val4;
						licenseInfo.ValidFrom = validFrom;
						licenseInfo.ValidTo = validTo;
						if (Class107.callSite_31 == null)
						{
							Class107.callSite_31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "status", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						licenseInfo.IsActive = smethod_0(Class107.callSite_31.Target(Class107.callSite_31, arg), "") == "active";
						_ = DateTime.UtcNow;
						result3 = Result<LicenseInfo>.Success(licenseInfo);
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("[Supabase] ✗ 未找到授权，user_id=");
						defaultInterpolatedStringHandler2.AppendFormatted(guid_0);
						Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
						result3 = Result<LicenseInfo>.Failure("许可证未找到");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询用户许可证异常: " + ex.Message);
				result3 = Result<LicenseInfo>.Failure("获取用户许可证异常：" + ex.Message);
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
	public struct Struct140 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<LicensePackage>>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Expected O, but got Unknown
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<List<LicensePackage>> result3;
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
						goto IL_016b;
					}
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/get_license_packages")
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent("{}", Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_016b;
				IL_016b:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[Supabase] ✗ RPC 查询套餐列表失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", 返回: ");
					defaultInterpolatedStringHandler.AppendFormatted(result2?.Substring(0, Math.Min(200, result2?.Length ?? 0)));
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					result3 = Result<List<LicensePackage>>.Failure("获取套餐列表失败");
				}
				else
				{
					JsonSerializerSettings val = new JsonSerializerSettings
					{
						DateTimeZoneHandling = (DateTimeZoneHandling)1,
						NullValueHandling = (NullValueHandling)1
					};
					List<LicensePackage> list = JsonConvert.DeserializeObject<List<LicensePackage>>(result2, val);
					if (list != null && list.Count > 0)
					{
						result3 = Result<List<LicensePackage>>.Success(list);
					}
					else
					{
						Logger.Warning("[Supabase] ✗ 套餐列表为空");
						result3 = Result<List<LicensePackage>>.Failure("套餐列表为空");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询套餐列表异常: " + ex.Message);
				result3 = Result<List<LicensePackage>>.Failure("获取套餐列表异常：" + ex.Message);
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
	public struct Struct141 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<int>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public string string_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Invalid comparison between Unknown and I4
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<int> result2;
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
						goto IL_00f8;
					}
					awaiter2 = supabaseClient.httpClient_0.GetAsync("/rest/v1/max_trial_limits?feature_id=eq." + string_0).GetAwaiter();
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
				if (result.IsSuccessStatusCode)
				{
					awaiter = result.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00f8;
				}
				goto IL_0150;
				IL_00f8:
				List<JToken> list = JsonConvert.DeserializeObject<List<JToken>>(awaiter.GetResult());
				if (list == null || list.Count <= 0)
				{
					goto IL_0150;
				}
				JToken val = list[0][(object)"max_usage"];
				if (val == null || (int)val.Type == 10)
				{
					goto IL_0150;
				}
				result2 = Result<int>.Success(Convert.ToInt32(((object)val).ToString()));
				goto end_IL_000f;
				IL_0150:
				result2 = Result<int>.Success(10);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result2 = Result<int>.Failure("获取试用限制异常：" + ex.Message);
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
	public struct Struct142 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<TokenUsageInfo>>> asyncTaskMethodBuilder_0;

		public int int_1;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<List<TokenUsageInfo>> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 1);
					defaultInterpolatedStringHandler.AppendLiteral("/rest/v1/token_usage?order=created_at.desc&limit=");
					defaultInterpolatedStringHandler.AppendFormatted(int_1);
					defaultInterpolatedStringHandler.AppendLiteral("&select=*");
					string requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00bf;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00bf;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0188;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0188:
					result = awaiter.GetResult();
					Logger.Error("[Supabase] 查询 Token 使用记录失败: " + result);
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("查询 Token 使用记录失败（HTTP ");
					defaultInterpolatedStringHandler2.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler2.AppendLiteral("）：");
					defaultInterpolatedStringHandler2.AppendFormatted(result);
					result2 = Result<List<TokenUsageInfo>>.Failure(defaultInterpolatedStringHandler2.ToStringAndClear());
					goto end_IL_000f;
					IL_00bf:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_0188;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<Class101> list = JsonConvert.DeserializeObject<List<Class101>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					List<TokenUsageInfo> list2 = new List<TokenUsageInfo>();
					List<Class101>.Enumerator enumerator = list.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Class101 current = enumerator.Current;
							list2.Add(new TokenUsageInfo
							{
								Id = Guid.Parse(current.id),
								UserId = ((current.user_id != null) ? new Guid?(Guid.Parse(current.user_id)) : ((Guid?)null)),
								DeviceId = ((current.device_id != null) ? new Guid?(Guid.Parse(current.device_id)) : ((Guid?)null)),
								Provider = current.provider,
								ModelName = current.model_name,
								InputTokens = current.input_tokens,
								OutputTokens = current.output_tokens,
								TotalTokens = current.total_tokens,
								TotalCost = current.total_cost,
								CreatedAt = DateTime.Parse(current.created_at)
							});
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
						}
					}
					result2 = Result<List<TokenUsageInfo>>.Success(list2);
				}
				else
				{
					Logger.Warning("[Supabase] 未找到 Token 使用记录");
					result2 = Result<List<TokenUsageInfo>>.Success(new List<TokenUsageInfo>());
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询 Token 使用记录异常", ex);
				result2 = Result<List<TokenUsageInfo>>.Failure("查询 Token 使用记录异常：" + ex.Message);
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
	public struct Struct143 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<TrialRecordDetail?>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public Guid? nullable_0;

		public SupabaseClient supabaseClient_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<TrialRecordDetail> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
					if (string.IsNullOrEmpty(string_0))
					{
						result = Result<TrialRecordDetail>.Failure("设备 ID 不能为空");
					}
					else
					{
						if (!string.IsNullOrEmpty(string_1))
						{
							string text = "/rest/v1/trial_records?device_id=eq." + string_0 + "&feature_id=eq." + string_1;
							if (nullable_0.HasValue)
							{
								string text2 = text;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
								defaultInterpolatedStringHandler.AppendLiteral("&user_id=eq.");
								defaultInterpolatedStringHandler.AppendFormatted(nullable_0.Value);
								text = text2 + defaultInterpolatedStringHandler.ToStringAndClear();
							}
							else
							{
								text += "&user_id=is.null";
							}
							text += "&order=usage_count.desc&limit=1";
							awaiter2 = supabaseClient.httpClient_0.GetAsync(text).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_0154;
						}
						result = Result<TrialRecordDetail>.Failure("功能 ID 不能为空");
					}
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0154;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0209;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0209:
					result2 = awaiter.GetResult();
					Logger.Warning("[Supabase] 查询试用记录失败: " + result2);
					result = Result<TrialRecordDetail>.Failure("查询失败：" + result2);
					goto end_IL_000f;
					IL_0154:
					result3 = awaiter2.GetResult();
					if (!result3.IsSuccessStatusCode)
					{
						awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0209;
					}
					awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
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
				string result4 = awaiter.GetResult();
				if (!string.IsNullOrWhiteSpace(result4) && !(result4 == "[]"))
				{
					List<TrialRecordDetail> list = JsonConvert.DeserializeObject<List<TrialRecordDetail>>(result4);
					result = ((list == null || list.Count <= 0) ? Result<TrialRecordDetail>.Success((TrialRecordDetail)null) : Result<TrialRecordDetail>.Success(list[0]));
				}
				else
				{
					result = Result<TrialRecordDetail>.Success((TrialRecordDetail)null);
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询试用记录异常", ex);
				result = Result<TrialRecordDetail>.Failure("查询异常：" + ex.Message);
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
	public struct Struct144 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<TrialRecordDetail>>> asyncTaskMethodBuilder_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<List<TrialRecordDetail>> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					string requestUri = "/rest/v1/trial_records?device_id=eq." + string_0 + "&select=*";
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_009e;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_009e;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0153;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0153:
					result = awaiter.GetResult();
					Logger.Warning("[Supabase] 查询设备试用记录失败: " + result);
					result2 = Result<List<TrialRecordDetail>>.Failure("查询失败：" + result);
					goto end_IL_000f;
					IL_009e:
					result3 = awaiter2.GetResult();
					if (!result3.IsSuccessStatusCode)
					{
						awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0153;
					}
					awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
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
				List<TrialRecordDetail> list = JsonConvert.DeserializeObject<List<TrialRecordDetail>>(awaiter.GetResult());
				result2 = ((list == null || list.Count <= 0) ? Result<List<TrialRecordDetail>>.Success(new List<TrialRecordDetail>()) : Result<List<TrialRecordDetail>>.Success(list));
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询设备试用记录异常", ex);
				result2 = Result<List<TrialRecordDetail>>.Failure("查询异常：" + ex.Message);
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
	public struct Struct145 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<TrialRecordDetail>>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public Guid guid_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<List<TrialRecordDetail>> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					supabaseClient.method_1();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
					defaultInterpolatedStringHandler.AppendLiteral("/rest/v1/trial_records?user_id=eq.");
					defaultInterpolatedStringHandler.AppendFormatted(guid_0);
					defaultInterpolatedStringHandler.AppendLiteral("&select=*");
					string requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00c5;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00c5;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_017a;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_017a:
					result = awaiter.GetResult();
					Logger.Warning("[Supabase] 查询用户试用记录失败: " + result);
					result2 = Result<List<TrialRecordDetail>>.Failure("查询失败：" + result);
					goto end_IL_000f;
					IL_00c5:
					result3 = awaiter2.GetResult();
					if (!result3.IsSuccessStatusCode)
					{
						awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_017a;
					}
					awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
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
				List<TrialRecordDetail> list = JsonConvert.DeserializeObject<List<TrialRecordDetail>>(awaiter.GetResult());
				result2 = ((list == null || list.Count <= 0) ? Result<List<TrialRecordDetail>>.Success(new List<TrialRecordDetail>()) : Result<List<TrialRecordDetail>>.Success(list));
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询用户试用记录异常", ex);
				result2 = Result<List<TrialRecordDetail>>.Failure("查询异常：" + ex.Message);
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
	public struct Struct146 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<int>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public string string_0;

		public Guid? nullable_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		private TaskAwaiter<Result<int>> taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			TaskAwaiter<Result<int>> awaiter;
			int num2;
			if ((uint)num > 2u)
			{
				if (num == 3)
				{
					awaiter = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<Result<int>>);
					num = -1;
					int_0 = -1;
					goto IL_02ca;
				}
				num2 = 0;
			}
			Result<int> result2;
			object obj;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter3;
				TaskAwaiter<string> awaiter2;
				string result;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					supabaseClient.method_1();
					Class35<string, Guid?> @class = new Class35<string, Guid?>(string_0, nullable_0);
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/get_unread_reply_count")
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter3 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter3;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
						return;
					}
					goto IL_0146;
				}
				case 0:
					awaiter3 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0146;
				case 1:
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01b8;
				case 2:
					{
						awaiter = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter<Result<int>>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01b8:
					result = awaiter2.GetResult();
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						Logger.Error("[Supabase] 获取未读回复数量失败: " + result);
						awaiter = supabaseClient.method_2(string_0, nullable_0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_2 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						break;
					}
					result2 = Result<int>.Success(JsonConvert.DeserializeObject<int>(result));
					goto end_IL_003e;
					IL_0146:
					result3 = awaiter3.GetResult();
					httpResponseMessage_0 = result3;
					awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_01b8;
				}
				result2 = awaiter.GetResult();
				end_IL_003e:;
			}
			catch (Exception ex)
			{
				obj = ex;
				num2 = 1;
				goto IL_0269;
			}
			goto IL_02d6;
			IL_0269:
			if (num2 == 1)
			{
				Exception ex2 = (Exception)obj;
				Logger.Error("[Supabase] 获取未读回复数量异常", ex2);
				awaiter = supabaseClient.method_2(string_0, nullable_0).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 3;
					int_0 = 3;
					taskAwaiter_2 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_02ca;
			}
			throw null;
			IL_02ca:
			result2 = awaiter.GetResult();
			goto IL_02d6;
			IL_02d6:
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
	public struct Struct147 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<int>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public Guid? nullable_0;

		public string string_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<int> result2;
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
						goto IL_0168;
					}
					supabaseClient.method_1();
					string requestUri;
					if (nullable_0.HasValue)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(60, 1);
						defaultInterpolatedStringHandler.AppendLiteral("/rest/v1/feedback?user_id=eq.");
						defaultInterpolatedStringHandler.AppendFormatted(nullable_0.Value);
						defaultInterpolatedStringHandler.AppendLiteral("&status=eq.replied&select=count");
						requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else
					{
						requestUri = "/rest/v1/feedback?device_id=eq." + string_0 + "&status=eq.replied&select=count";
					}
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
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
				if (result.IsSuccessStatusCode)
				{
					awaiter = result.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0168;
				}
				goto IL_01c1;
				IL_0168:
				List<Dictionary<string, object>> list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(awaiter.GetResult());
				if (list == null || list.Count <= 0 || !list[0].ContainsKey("count"))
				{
					goto IL_01c1;
				}
				result2 = Result<int>.Success(Convert.ToInt32(list[0]["count"]));
				goto end_IL_000f;
				IL_01c1:
				result2 = Result<int>.Success(0);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 获取未读回复数量（降级方法）异常", ex);
				result2 = Result<int>.Success(0);
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
	public struct Struct148 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<CombinedCreditsInfo>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public Guid? nullable_0;

		public Guid? nullable_1;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Expected O, but got Unknown
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<CombinedCreditsInfo> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					supabaseClient.method_1();
					string content = JsonConvert.SerializeObject((object)new Class46<Guid?, Guid?>(nullable_0, nullable_1), new JsonSerializerSettings
					{
						NullValueHandling = (NullValueHandling)1
					});
					string requestUri = "/rest/v1/rpc/get_credit_balance";
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(content, Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0125;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0125;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01ee;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01ee:
					result = awaiter.GetResult();
					Logger.Error("[Supabase] 查询电量失败: " + result);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
					defaultInterpolatedStringHandler.AppendLiteral("查询电量失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					result2 = Result<CombinedCreditsInfo>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
					goto end_IL_000f;
					IL_0125:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
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
						goto IL_01ee;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				List<CombinedCreditsInfo> list = JsonConvert.DeserializeObject<List<CombinedCreditsInfo>>(awaiter.GetResult());
				if (list != null && list.Count > 0)
				{
					result2 = Result<CombinedCreditsInfo>.Success(list[0]);
				}
				else
				{
					Logger.Warning("[Supabase] 未找到电量记录，返回默认值");
					result2 = Result<CombinedCreditsInfo>.Success(new CombinedCreditsInfo
					{
						TotalBalance = 0m,
						UserBalance = 0m,
						DeviceBalance = 0m,
						UserId = nullable_0,
						DeviceId = nullable_1,
						UserLastRechargeAt = null,
						DeviceLastRechargeAt = null,
						UserLastUsageAt = null,
						DeviceLastUsageAt = null
					});
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 查询电量异常", ex);
				result2 = Result<CombinedCreditsInfo>.Failure("查询电量异常：" + ex.Message);
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
	public struct Struct149 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<DeviceInfo>>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public Guid guid_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<List<DeviceInfo>> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
					supabaseClient.method_1();
					if (!(guid_0 == Guid.Empty))
					{
						HttpClient httpClient_ = supabaseClient.httpClient_0;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(84, 1);
						defaultInterpolatedStringHandler.AppendLiteral("/rest/v1/devices?user_id=eq.");
						defaultInterpolatedStringHandler.AppendFormatted(guid_0);
						defaultInterpolatedStringHandler.AppendLiteral("&is_disabled=eq.false&order=last_active_at.desc&select=*");
						awaiter2 = httpClient_.GetAsync(defaultInterpolatedStringHandler.ToStringAndClear()).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_00ea;
					}
					result = Result<List<DeviceInfo>>.Failure("用户 ID 不能为空");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00ea;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_019b;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_019b:
					result2 = awaiter.GetResult();
					result = Result<List<DeviceInfo>>.Failure("获取设备列表失败：" + result2);
					goto end_IL_000f;
					IL_00ea:
					result3 = awaiter2.GetResult();
					if (!result3.IsSuccessStatusCode)
					{
						awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_019b;
					}
					awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
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
				result = Result<List<DeviceInfo>>.Success(JsonConvert.DeserializeObject<List<DeviceInfo>>(awaiter.GetResult()) ?? new List<DeviceInfo>());
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result<List<DeviceInfo>>.Failure("获取设备列表异常：" + ex.Message);
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
	public struct Struct150 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<DeviceCreditsResult>> asyncTaskMethodBuilder_0;

		public Guid guid_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<DeviceCreditsResult> result3;
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
						goto IL_017c;
					}
					Class37<Guid> @class = new Class37<Guid>(guid_0);
					string requestUri = "/rest/v1/rpc/initialize_credits_for_new_device";
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_017c;
				IL_017c:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 初始化电量记录失败: " + result2);
					result3 = Result<DeviceCreditsResult>.Failure("初始化电量记录失败：" + result2);
				}
				else
				{
					List<DeviceCreditsResult> list = JsonConvert.DeserializeObject<List<DeviceCreditsResult>>(result2);
					if (list != null && list.Count > 0)
					{
						result3 = Result<DeviceCreditsResult>.Success(list[0]);
					}
					else
					{
						Logger.Warning("[Supabase] 初始化电量记录响应为空");
						result3 = Result<DeviceCreditsResult>.Failure("初始化电量记录失败：响应为空");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 初始化电量记录异常", ex);
				result3 = Result<DeviceCreditsResult>.Failure("初始化电量记录异常：" + ex.Message);
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
	public struct Struct151 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<UserCreditsResult>> asyncTaskMethodBuilder_0;

		public Guid guid_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<UserCreditsResult> result3;
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
						goto IL_017c;
					}
					Class44<Guid> @class = new Class44<Guid>(guid_0);
					string requestUri = "/rest/v1/rpc/initialize_credits_for_new_user";
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_017c;
				IL_017c:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 初始化用户电量记录失败: " + result2);
					result3 = Result<UserCreditsResult>.Failure("初始化用户电量记录失败：" + result2);
				}
				else
				{
					List<UserCreditsResult> list = JsonConvert.DeserializeObject<List<UserCreditsResult>>(result2);
					if (list != null && list.Count > 0)
					{
						result3 = Result<UserCreditsResult>.Success(list[0]);
					}
					else
					{
						Logger.Warning("[Supabase] 初始化用户电量记录响应为空");
						result3 = Result<UserCreditsResult>.Failure("初始化用户电量记录失败：响应为空");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 初始化用户电量记录异常", ex);
				result3 = Result<UserCreditsResult>.Failure("初始化用户电量记录异常：" + ex.Message);
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
	public struct Struct152 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public string string_2;

		public Guid? nullable_0;

		public int int_1;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result3;
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
						goto IL_0163;
					}
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class45<string, string, string, string, string, int, int>(string_0, string_1, string_2, nullable_0.HasValue ? nullable_0.Value.ToString() : null, "v1", 0, int_1)), Encoding.UTF8, "application/json");
					awaiter2 = supabaseClient.httpClient_0.PostAsync("/rest/v1/trial_records", content).GetAwaiter();
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
				goto IL_0163;
				IL_0163:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					if (!result2.Contains("duplicate key") && !result2.Contains("unique constraint"))
					{
						Logger.Error("[Supabase] 插入试用记录失败: " + result2);
						result3 = Result.Failure("插入试用记录失败：" + result2);
					}
					else
					{
						result3 = Result.Success();
					}
				}
				else
				{
					result3 = Result.Success();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 插入试用记录异常", ex);
				result3 = Result.Failure("插入试用记录异常：" + ex.Message);
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
	public struct Struct153 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public Guid guid_0;

		public Guid guid_1;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result3;
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
						goto IL_017e;
					}
					Class51<Guid, Guid> @class = new Class51<Guid, Guid>(guid_0, guid_1);
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/mark_feedback_as_read_user")
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_017e;
				IL_017e:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 标记反馈已读失败（用户级别）: " + result2);
					result3 = Result.Failure("标记反馈已读失败：" + result2);
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[Supabase] 标记反馈已读成功（用户级别）: ");
					defaultInterpolatedStringHandler.AppendFormatted(guid_0);
					defaultInterpolatedStringHandler.AppendLiteral(", 用户: ");
					defaultInterpolatedStringHandler.AppendFormatted(guid_1);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					result3 = Result.Success();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 标记反馈已读异常（用户级别）", ex);
				result3 = Result.Failure("标记反馈已读异常：" + ex.Message);
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
	public struct Struct154 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public Guid guid_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result3;
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
						goto IL_017e;
					}
					Class52<Guid, string> @class = new Class52<Guid, string>(guid_0, string_0);
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/mark_feedback_as_read_device")
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_017e;
				IL_017e:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 标记反馈已读失败（设备级别）: " + result2);
					result3 = Result.Failure("标记反馈已读失败：" + result2);
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[Supabase] 标记反馈已读成功（设备级别）: ");
					defaultInterpolatedStringHandler.AppendFormatted(guid_0);
					defaultInterpolatedStringHandler.AppendLiteral(", 设备: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					result3 = Result.Success();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 标记反馈已读异常（设备级别）", ex);
				result3 = Result.Failure("标记反馈已读异常：" + ex.Message);
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
	public struct Struct155 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<MigrateDeviceResult>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public string string_2;

		public string string_3;

		public string string_4;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<MigrateDeviceResult> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_017f;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01d4;
				}
				if (string.IsNullOrEmpty(string_0))
				{
					result = Result<MigrateDeviceResult>.Failure("旧设备 ID 不能为空");
				}
				else
				{
					if (!string.IsNullOrEmpty(string_1))
					{
						Class39<string, string, string, string, string> @class = new Class39<string, string, string, string, string>(string_0, string_1, string_2, string_3, string_4);
						HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/migrate_device_id_copy")
						{
							Headers = 
							{
								{
									"Authorization",
									"Bearer " + supabaseClient.string_1
								},
								{
									"apikey",
									supabaseClient.string_1
								}
							},
							Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
						};
						awaiter = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_017f;
					}
					result = Result<MigrateDeviceResult>.Failure("新设备 ID 不能为空");
				}
				goto end_IL_000f;
				IL_01d4:
				string result2 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 设备迁移失败: " + result2);
					result = Result<MigrateDeviceResult>.Failure("迁移失败: " + result2);
				}
				else
				{
					MigrateDeviceResult migrateDeviceResult = JsonConvert.DeserializeObject<MigrateDeviceResult>(result2);
					if (migrateDeviceResult != null)
					{
						if (migrateDeviceResult.Success)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 3);
							defaultInterpolatedStringHandler.AppendLiteral("[Supabase] 设备迁移成功: ");
							defaultInterpolatedStringHandler.AppendFormatted(string_0.Substring(0, 8));
							defaultInterpolatedStringHandler.AppendLiteral("... -> ");
							defaultInterpolatedStringHandler.AppendFormatted(string_1.Substring(0, 8));
							defaultInterpolatedStringHandler.AppendLiteral("..., 复制电量: ");
							defaultInterpolatedStringHandler.AppendFormatted(migrateDeviceResult.CopiedCredits);
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							result = Result<MigrateDeviceResult>.Success(migrateDeviceResult);
						}
						else
						{
							Logger.Warning("[Supabase] 设备迁移返回失败: " + migrateDeviceResult.Error);
							result = Result<MigrateDeviceResult>.Failure(migrateDeviceResult.Error ?? "迁移失败");
						}
					}
					else
					{
						result = Result<MigrateDeviceResult>.Failure("迁移失败：无法解析返回结果");
					}
				}
				goto end_IL_000f;
				IL_017f:
				HttpResponseMessage result3 = awaiter.GetResult();
				httpResponseMessage_0 = result3;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_01d4;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 设备迁移异常", ex);
				result = Result<MigrateDeviceResult>.Failure("迁移异常：" + ex.Message);
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
	public struct Struct156 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<Guid>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public string string_0;

		public string string_1;

		public string string_2;

		public string string_3;

		public string string_4;

		public Guid? nullable_0;

		public Guid? nullable_1;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<Guid> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				Guid result3;
				HttpResponseMessage result4;
				switch (num)
				{
				default:
					supabaseClient.method_1();
					if (!string.IsNullOrWhiteSpace(string_0))
					{
						Class49<string, string, string, string, string, Guid?, Guid?> @class = new Class49<string, string, string, string, string, Guid?, Guid?>(string_0.Trim(), string_1, string_2, string_3, string_4, nullable_0, nullable_1);
						HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/record_ai_message")
						{
							Headers = { 
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							} },
							Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
						};
						awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_013e;
					}
					result = Result<Guid>.Failure("消息内容不能为空");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_013e;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0207;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0207:
					result2 = awaiter.GetResult();
					if (Guid.TryParse(result2.Trim().Trim('"').Trim(new char[2] { '[', ']' }), out result3))
					{
						result = Result<Guid>.Success(result3);
					}
					else
					{
						Logger.Warning("[Supabase] AI 消息记录返回格式异常: " + result2);
						result = Result<Guid>.Failure("AI 消息记录失败：返回格式异常");
					}
					goto end_IL_000f;
					IL_013e:
					result4 = awaiter2.GetResult();
					httpResponseMessage_0 = result4;
					if (httpResponseMessage_0.IsSuccessStatusCode)
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
						goto IL_0207;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
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
				string result5 = awaiter.GetResult();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[Supabase] 记录 AI 消息失败: ");
				defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
				defaultInterpolatedStringHandler.AppendLiteral(" - ");
				defaultInterpolatedStringHandler.AppendFormatted(result5);
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				result = Result<Guid>.Failure("记录 AI 消息失败：" + result5);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[Supabase] 记录 AI 消息异常", ex);
				result = Result<Guid>.Failure("记录 AI 消息异常：" + ex.Message);
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
	public struct Struct157 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<Guid>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public string string_0;

		public string string_1;

		public Guid? nullable_0;

		public Guid? nullable_1;

		public string string_2;

		public int int_1;

		public int int_2;

		public string string_3;

		public bool bool_0;

		public int int_3;

		public int? nullable_2;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<Guid> result3;
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
						goto IL_0291;
					}
					supabaseClient.method_1();
					string content = JsonConvert.SerializeObject((object)new Dictionary<string, object>
					{
						["p_provider"] = string_0,
						["p_model_name"] = string_1,
						["p_user_id"] = nullable_0,
						["p_device_id"] = nullable_1,
						["p_session_id"] = string_2,
						["p_input_tokens"] = int_1,
						["p_output_tokens"] = int_2,
						["p_request_type"] = string_3,
						["p_is_tool_call"] = bool_0,
						["p_tool_count"] = int_3,
						["p_duration_ms"] = nullable_2
					});
					string requestUri = "/rest/v1/rpc/record_token_usage_only";
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(content, Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_0291;
				IL_0291:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 记录 Token 使用失败: " + result2);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
					defaultInterpolatedStringHandler.AppendLiteral("记录 Token 使用失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					result3 = Result<Guid>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					result3 = Result<Guid>.Success(JsonConvert.DeserializeObject<Guid>(result2));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 记录 Token 使用异常", ex);
				result3 = Result<Guid>.Failure("记录 Token 使用异常：" + ex.Message);
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
	public struct Struct158 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<int>> asyncTaskMethodBuilder_0;

		public string string_0;

		public Guid? nullable_0;

		public string string_1;

		public string string_2;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<int> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				string requestUri;
				string text;
				string requestUri2;
				HttpResponseMessage result3;
				List<Dictionary<string, object>> list2;
				int num2;
				int num3;
				string result5;
				int result6;
				List<Dictionary<string, object>> list;
				HttpResponseMessage result4;
				HttpResponseMessage result7;
				switch (num)
				{
				default:
				{
					Class40<string, string, string, string, string> @class = new Class40<string, string, string, string, string>(string_0, nullable_0.HasValue ? nullable_0.Value.ToString() : null, string_1, string_2, "v1");
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/record_trial_usage")
					{
						Headers = { 
						{
							"Authorization",
							"Bearer " + supabaseClient.string_1
						} },
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0145;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0145;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_020e;
				case 2:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0266;
				case 3:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0413;
				case 4:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0485;
				case 5:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_05ae;
				case 6:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0620;
				case 7:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_070d;
				case 8:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						goto IL_077f;
					}
					IL_0620:
					list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(awaiter.GetResult());
					if (list == null || list.Count <= 0 || !list[0].ContainsKey("max_usage_count"))
					{
						if (string.IsNullOrEmpty(string_1))
						{
							break;
						}
						requestUri = "/rest/v1/max_trial_limits?feature_group=eq." + string_1 + "&feature_id=is.null&is_enabled=eq.true&select=max_usage_count&limit=1";
						awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 7;
							int_0 = 7;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_070d;
					}
					result = Result<int>.Success(Convert.ToInt32(list[0]["max_usage_count"]) - 1);
					goto end_IL_000f;
					IL_0266:
					result2 = awaiter.GetResult();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[Supabase] RPC 调用失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("），错误: ");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					goto IL_02c2;
					IL_052f:
					requestUri = "/rest/v1/max_trial_limits?feature_id=eq." + string_2 + "&is_enabled=eq.true&select=max_usage_count&order=feature_group&limit=1";
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 5;
						int_0 = 5;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_05ae;
					IL_02c2:
					if (!nullable_0.HasValue)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(102, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("/rest/v1/trial_records?device_id=eq.");
						defaultInterpolatedStringHandler2.AppendFormatted(string_0);
						defaultInterpolatedStringHandler2.AppendLiteral("&user_id=is.null&feature_id=eq.");
						defaultInterpolatedStringHandler2.AppendFormatted(string_2);
						defaultInterpolatedStringHandler2.AppendLiteral("&select=usage_count,max_usage_count");
						text = defaultInterpolatedStringHandler2.ToStringAndClear();
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(98, 3);
						defaultInterpolatedStringHandler3.AppendLiteral("/rest/v1/trial_records?device_id=eq.");
						defaultInterpolatedStringHandler3.AppendFormatted(string_0);
						defaultInterpolatedStringHandler3.AppendLiteral("&user_id=eq.");
						defaultInterpolatedStringHandler3.AppendFormatted(nullable_0.Value);
						defaultInterpolatedStringHandler3.AppendLiteral("&feature_id=eq.");
						defaultInterpolatedStringHandler3.AppendFormatted(string_2);
						defaultInterpolatedStringHandler3.AppendLiteral("&select=usage_count,max_usage_count");
						text = defaultInterpolatedStringHandler3.ToStringAndClear();
					}
					requestUri2 = text;
					awaiter2 = supabaseClient.httpClient_0.GetAsync(requestUri2).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 3;
						int_0 = 3;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0413;
					IL_0145:
					result3 = awaiter2.GetResult();
					httpResponseMessage_0 = result3;
					if (httpResponseMessage_0.IsSuccessStatusCode)
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
						goto IL_020e;
					}
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0266;
					IL_05ae:
					result4 = awaiter2.GetResult();
					if (!result4.IsSuccessStatusCode)
					{
						break;
					}
					awaiter = result4.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 6;
						int_0 = 6;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0620;
					IL_0485:
					list2 = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(awaiter.GetResult());
					if (list2 == null || list2.Count <= 0 || !list2[0].ContainsKey("usage_count"))
					{
						goto IL_052f;
					}
					num2 = Convert.ToInt32(list2[0]["usage_count"]);
					num3 = (list2[0].ContainsKey("max_usage_count") ? Convert.ToInt32(list2[0]["max_usage_count"]) : 10);
					result = Result<int>.Success(Math.Max(0, num3 - num2));
					goto end_IL_000f;
					IL_020e:
					result5 = awaiter.GetResult();
					if (string.IsNullOrEmpty(result5) || !int.TryParse(result5, out result6))
					{
						Logger.Warning("[Supabase] RPC 返回成功但格式无法解析，使用备用方案");
						goto IL_02c2;
					}
					result = Result<int>.Success(result6);
					goto end_IL_000f;
					IL_077f:
					list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(awaiter.GetResult());
					if (list == null || list.Count <= 0 || !list[0].ContainsKey("max_usage_count"))
					{
						break;
					}
					result = Result<int>.Success(Convert.ToInt32(list[0]["max_usage_count"]) - 1);
					goto end_IL_000f;
					IL_070d:
					result4 = awaiter2.GetResult();
					if (!result4.IsSuccessStatusCode)
					{
						break;
					}
					awaiter = result4.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 8;
						int_0 = 8;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_077f;
					IL_0413:
					result7 = awaiter2.GetResult();
					if (result7.IsSuccessStatusCode)
					{
						awaiter = result7.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 4;
							int_0 = 4;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0485;
					}
					goto IL_052f;
				}
				Logger.Warning("[Supabase] 查询试用记录和配置均失败，返回默认值 9（默认10次-1）");
				result = Result<int>.Success(9);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 记录试用使用异常", ex);
				result = Result<int>.Failure("记录试用使用异常：" + ex.Message);
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
	public struct Struct159 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<TrialRecordDetail>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public Guid? nullable_0;

		public string string_2;

		public string string_3;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private string string_4;

		private TrialRecordDetail trialRecordDetail_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		private HttpResponseMessage httpResponseMessage_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<TrialRecordDetail> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result3;
				string result4;
				TrialRecordDetail trialRecordDetail;
				List<TrialRecordDetail> list2;
				Class42<string, string, string, string, string, int> class3;
				HttpRequestMessage request3;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				HttpResponseMessage result2;
				switch (num)
				{
				default:
					if (string.IsNullOrEmpty(string_0))
					{
						result = Result<TrialRecordDetail>.Failure("设备 ID 不能为空");
					}
					else
					{
						if (!string.IsNullOrEmpty(string_1))
						{
							Class40<string, string, string, string, string> @class = new Class40<string, string, string, string, string>(string_0, nullable_0?.ToString(), string_2, string_1, string_3);
							HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/record_trial_usage")
							{
								Headers = { 
								{
									"Authorization",
									"Bearer " + supabaseClient.string_1
								} },
								Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
							};
							awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_0179;
						}
						result = Result<TrialRecordDetail>.Failure("功能 ID 不能为空");
					}
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0179;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01eb;
				case 2:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_02f7;
				case 3:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0359;
				case 4:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0654;
				case 5:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_067e;
				case 6:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						goto IL_06f0;
					}
					IL_0179:
					result2 = awaiter2.GetResult();
					httpResponseMessage_0 = result2;
					awaiter = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_01eb;
					IL_06f0:
					result3 = awaiter.GetResult();
					if (httpResponseMessage_1.IsSuccessStatusCode)
					{
						List<TrialRecordDetail> list = JsonConvert.DeserializeObject<List<TrialRecordDetail>>(result3);
						if (list != null && list.Count > 0)
						{
							trialRecordDetail_0 = list[0];
						}
					}
					httpResponseMessage_1 = null;
					break;
					IL_01eb:
					result4 = awaiter.GetResult();
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						goto IL_021b;
					}
					trialRecordDetail = JsonConvert.DeserializeObject<TrialRecordDetail>(result4);
					if (trialRecordDetail == null)
					{
						goto IL_021b;
					}
					result = Result<TrialRecordDetail>.Success(trialRecordDetail);
					goto end_IL_000f;
					IL_0359:
					list2 = JsonConvert.DeserializeObject<List<TrialRecordDetail>>(awaiter.GetResult());
					if (list2 != null && list2.Count > 0)
					{
						trialRecordDetail_0 = list2[0];
						trialRecordDetail_0.UsageCount++;
						trialRecordDetail_0.LastUsedAt = DateTime.UtcNow;
						Class41<int, DateTime, string> class2 = new Class41<int, DateTime, string>(trialRecordDetail_0.UsageCount, trialRecordDetail_0.LastUsedAt, nullable_0?.ToString());
						HttpRequestMessage request2 = new HttpRequestMessage(new HttpMethod("PATCH"), string_4 + "&limit=1")
						{
							Headers = { 
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							} },
							Content = new StringContent(JsonConvert.SerializeObject((object)class2), Encoding.UTF8, "application/json")
						};
						awaiter2 = supabaseClient.httpClient_0.SendAsync(request2).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 4;
							int_0 = 4;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_0654;
					}
					trialRecordDetail_0 = new TrialRecordDetail
					{
						Id = Guid.NewGuid(),
						DeviceId = string_0,
						UserId = nullable_0,
						FeatureGroup = string_2,
						FeatureId = string_1,
						TrialVersion = string_3,
						UsageCount = 1,
						MaxUsageCount = 10,
						FirstUsedAt = DateTime.UtcNow,
						LastUsedAt = DateTime.UtcNow
					};
					class3 = new Class42<string, string, string, string, string, int>(trialRecordDetail_0.DeviceId, trialRecordDetail_0.UserId?.ToString(), trialRecordDetail_0.TrialVersion, trialRecordDetail_0.FeatureGroup, trialRecordDetail_0.FeatureId, trialRecordDetail_0.UsageCount);
					request3 = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/trial_records")
					{
						Headers = { 
						{
							"Authorization",
							"Bearer " + supabaseClient.string_1
						} },
						Content = new StringContent(JsonConvert.SerializeObject((object)class3), Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request3).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 5;
						int_0 = 5;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_067e;
					IL_021b:
					Logger.Warning("[Supabase] RPC 调用失败，使用备用方案: " + result4);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(60, 2);
					defaultInterpolatedStringHandler.AppendLiteral("/rest/v1/trial_records?device_id=eq.");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral("&feature_id=eq.");
					defaultInterpolatedStringHandler.AppendFormatted(string_1);
					defaultInterpolatedStringHandler.AppendLiteral("&select=*");
					string_4 = defaultInterpolatedStringHandler.ToStringAndClear();
					awaiter2 = supabaseClient.httpClient_0.GetAsync(string_4).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_02f7;
					IL_067e:
					result2 = awaiter2.GetResult();
					httpResponseMessage_1 = result2;
					awaiter = httpResponseMessage_1.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 6;
						int_0 = 6;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_06f0;
					IL_0654:
					awaiter2.GetResult();
					break;
					IL_02f7:
					awaiter = awaiter2.GetResult().Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 3;
						int_0 = 3;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0359;
				}
				result = Result<TrialRecordDetail>.Success(trialRecordDetail_0);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 记录试用使用异常", ex);
				result = Result<TrialRecordDetail>.Failure("记录试用使用异常：" + ex.Message);
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
	public struct Struct160 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<UserIdentity>> asyncTaskMethodBuilder_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<UserIdentity> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00e9;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_013e;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					Class32<string> @class = new Class32<string>(string_0);
					awaiter = supabaseClient.httpClient_0.PostAsync("/auth/v1/token?grant_type=refresh_token", new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00e9;
				}
				result = Result<UserIdentity>.Failure("刷新令牌不能为空");
				goto end_IL_000f;
				IL_013e:
				string result2 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					result = Result<UserIdentity>.Failure("刷新令牌失败：" + result2);
				}
				else
				{
					Class95 class2 = JsonConvert.DeserializeObject<Class95>(result2);
					result = ((class2 == null || class2.User == null) ? Result<UserIdentity>.Failure("无效的响应") : Result<UserIdentity>.Success(new UserIdentity
					{
						UserId = Guid.Parse(class2.User.Id),
						Email = class2.User.Email,
						AuthToken = class2.AccessToken,
						RefreshToken = class2.RefreshToken,
						TokenExpiresAt = ((class2.ExpiresIn > 0) ? new DateTime?(DateTime.UtcNow.AddSeconds(class2.ExpiresIn)) : ((DateTime?)null))
					}));
				}
				goto end_IL_000f;
				IL_00e9:
				HttpResponseMessage result3 = awaiter.GetResult();
				httpResponseMessage_0 = result3;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_013e;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result<UserIdentity>.Failure("刷新令牌异常：" + ex.Message);
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
	public struct Struct161 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<DeviceInfo>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public DeviceInfo deviceInfo_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Expected O, but got Unknown
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<DeviceInfo> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_025a;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_02af;
				}
				supabaseClient.method_1();
				if (deviceInfo_0 != null)
				{
					string content = JsonConvert.SerializeObject((object)new Class34<string, string, string, string, string, string, bool, Guid?, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, DateTime?, DateTime?, int>(deviceInfo_0.DeviceId, deviceInfo_0.DeviceName, deviceInfo_0.OsVersion, deviceInfo_0.IpAddress, deviceInfo_0.FirstIpAddress, deviceInfo_0.RevitVersion, deviceInfo_0.IsBound, deviceInfo_0.UserId, deviceInfo_0.IsDisabled, deviceInfo_0.Revit2018, deviceInfo_0.Revit2019, deviceInfo_0.Revit2020, deviceInfo_0.Revit2021, deviceInfo_0.Revit2022, deviceInfo_0.Revit2023, deviceInfo_0.Revit2024, deviceInfo_0.Revit2025, deviceInfo_0.Revit2026, deviceInfo_0.RegisteredAt, deviceInfo_0.LastSeenAt, deviceInfo_0.IpChangesCount), new JsonSerializerSettings
					{
						NullValueHandling = (NullValueHandling)1,
						Formatting = (Formatting)1
					});
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/devices")
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							},
							{
								"Prefer",
								"return=representation"
							}
						},
						Content = new StringContent(content, Encoding.UTF8, "application/json")
					};
					awaiter = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_025a;
				}
				result = Result<DeviceInfo>.Failure("设备信息不能为空");
				goto end_IL_000f;
				IL_02af:
				string result2 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 注册设备失败");
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
					defaultInterpolatedStringHandler.AppendLiteral("注册设备失败（HTTP ");
					defaultInterpolatedStringHandler.AppendFormatted((int)httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral("）：");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					result = Result<DeviceInfo>.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					List<DeviceInfo> list = JsonConvert.DeserializeObject<List<DeviceInfo>>(result2);
					if (list != null && list.Count > 0)
					{
						result = Result<DeviceInfo>.Success(list[0]);
					}
					else if (!string.IsNullOrWhiteSpace(result2) && !(result2.Trim() == "[]") && !(result2.Trim() == "{}"))
					{
						Logger.Error("[Supabase] 反序列化失败，响应内容: " + result2);
						result = Result<DeviceInfo>.Failure("无效的响应: " + result2);
					}
					else
					{
						result = Result<DeviceInfo>.Success(deviceInfo_0);
					}
				}
				goto end_IL_000f;
				IL_025a:
				HttpResponseMessage result3 = awaiter.GetResult();
				httpResponseMessage_0 = result3;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_02af;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 注册设备异常", ex);
				result = Result<DeviceInfo>.Failure("注册设备异常：" + ex.Message);
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
	public struct Struct162 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00e9;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_013e;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					string content = JsonConvert.SerializeObject((object)new Class33<string>(string_0));
					awaiter = supabaseClient.httpClient_0.PostAsync("/auth/v1/recover", new StringContent(content, Encoding.UTF8, "application/json")).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00e9;
				}
				result = Result.Failure("电子邮件地址不能为空");
				goto end_IL_000f;
				IL_013e:
				string result2 = awaiter2.GetResult();
				result = (httpResponseMessage_0.IsSuccessStatusCode ? Result.Success() : ((result2.Contains("User not found") || result2.Contains("invalid_email")) ? Result.Failure("发送失败：该邮箱尚未注册。\n\n提示：请先注册账户，或检查邮箱地址是否正确。") : Result.Failure("发送重置邮件失败：" + result2)));
				goto end_IL_000f;
				IL_00e9:
				HttpResponseMessage result3 = awaiter.GetResult();
				httpResponseMessage_0 = result3;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_013e;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 请求密码重置异常", ex);
				result = Result.Failure("请求密码重置异常：" + ex.Message);
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
	public struct Struct163 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<UserIdentity>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public string string_0;

		public string string_1;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<UserIdentity> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0105;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_015a;
				}
				supabaseClient.method_1();
				if (!string.IsNullOrEmpty(string_0) && !string.IsNullOrEmpty(string_1))
				{
					Class30<string, string> @class = new Class30<string, string>(string_0, string_1);
					awaiter = supabaseClient.httpClient_0.PostAsync("/auth/v1/token?grant_type=password", new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0105;
				}
				result = Result<UserIdentity>.Failure("电子邮件和密码不能为空");
				goto end_IL_000f;
				IL_0105:
				HttpResponseMessage result2 = awaiter.GetResult();
				httpResponseMessage_0 = result2;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_015a;
				IL_015a:
				string result3 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					result = ((result3.Contains("Invalid login credentials") || result3.Contains("invalid_credentials")) ? Result<UserIdentity>.Failure("登录失败：邮箱或密码错误。\n\n提示：请先注册账户，或检查邮箱和密码是否正确。") : Result<UserIdentity>.Failure("登录失败：" + result3));
				}
				else
				{
					Class95 class2 = JsonConvert.DeserializeObject<Class95>(result3);
					result = ((class2 == null || class2.User == null) ? Result<UserIdentity>.Failure("无效的响应") : Result<UserIdentity>.Success(new UserIdentity
					{
						UserId = Guid.Parse(class2.User.Id),
						Email = class2.User.Email,
						AuthToken = class2.AccessToken,
						RefreshToken = class2.RefreshToken,
						TokenExpiresAt = ((class2.ExpiresIn > 0) ? new DateTime?(DateTime.UtcNow.AddSeconds(class2.ExpiresIn)) : ((DateTime?)null)),
						CreatedAt = ((class2.User.CreatedAt != null) ? DateTime.Parse(class2.User.CreatedAt) : DateTime.UtcNow),
						LastSignInAt = DateTime.UtcNow
					}));
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result<UserIdentity>.Failure("登录异常：" + ex.Message);
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
	public struct Struct164 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<UserIdentity>> asyncTaskMethodBuilder_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<UserIdentity> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00e7;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_013c;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					awaiter = supabaseClient.httpClient_0.PostAsync("/rest/v1/rpc/sign_in_by_device", new StringContent(JsonConvert.SerializeObject((object)new Class31<string>(string_0)), Encoding.UTF8, "application/json")).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00e7;
				}
				result = Result<UserIdentity>.Failure("设备 ID 不能为空");
				goto end_IL_000f;
				IL_013c:
				string result2 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					result = ((httpResponseMessage_0.StatusCode == HttpStatusCode.NotFound || result2.Contains("Device not found") || result2.Contains("Device not bound")) ? Result<UserIdentity>.Failure("设备未绑定到用户") : Result<UserIdentity>.Failure("设备自动登录失败：" + result2));
				}
				else
				{
					dynamic val = JsonConvert.DeserializeObject<object>(result2);
					if (!((val == null) ? true : false))
					{
						try
						{
							JArray val2 = (JArray)(object)((val is JArray) ? val : null);
							object arg;
							if (val2 != null && ((JContainer)val2).Count > 0)
							{
								arg = val2[0];
								goto IL_0293;
							}
							if (val is JObject)
							{
								arg = val;
								goto IL_0293;
							}
							result = Result<UserIdentity>.Failure("响应格式不正确");
							goto end_IL_0260;
							IL_0293:
							if (Class105.callSite_2 == null)
							{
								Class105.callSite_2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user_id", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							object obj = Class105.callSite_2.Target(Class105.callSite_2, arg);
							dynamic val3;
							if (obj == null)
							{
								val3 = null;
							}
							else
							{
								if (Class105.callSite_3 == null)
								{
									Class105.callSite_3 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								val3 = Class105.callSite_3.Target(Class105.callSite_3, obj);
								if ((object)val3 != null)
								{
									goto IL_0382;
								}
							}
							val3 = string.Empty;
							goto IL_0382;
							IL_0382:
							string text = val3;
							if (Class105.callSite_5 == null)
							{
								Class105.callSite_5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							obj = Class105.callSite_5.Target(Class105.callSite_5, arg);
							dynamic val4;
							if (obj == null)
							{
								val4 = null;
							}
							else
							{
								if (Class105.callSite_6 == null)
								{
									Class105.callSite_6 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								val4 = Class105.callSite_6.Target(Class105.callSite_6, obj);
								if ((object)val4 != null)
								{
									goto IL_0478;
								}
							}
							val4 = string.Empty;
							goto IL_0478;
							IL_0478:
							string text2 = val4;
							if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
							{
								result = Result<UserIdentity>.Success(new UserIdentity
								{
									UserId = Guid.Parse(text),
									Email = text2,
									AuthToken = string.Empty,
									RefreshToken = string.Empty,
									TokenExpiresAt = null,
									CreatedAt = DateTime.UtcNow,
									LastSignInAt = DateTime.UtcNow
								});
							}
							else
							{
								Logger.Warning("[SupabaseClient] 响应中缺少用户信息: userId=" + text + ", email=" + text2);
								result = Result<UserIdentity>.Failure("响应中缺少用户信息");
							}
							end_IL_0260:;
						}
						catch (Exception ex)
						{
							Logger.Error("[SupabaseClient] 解析用户信息失败", ex);
							result = Result<UserIdentity>.Failure("解析用户信息失败：" + ex.Message);
						}
					}
					else
					{
						result = Result<UserIdentity>.Failure("无效的响应");
					}
				}
				goto end_IL_000f;
				IL_00e7:
				HttpResponseMessage result3 = awaiter.GetResult();
				httpResponseMessage_0 = result3;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_013c;
				end_IL_000f:;
			}
			catch (Exception ex2)
			{
				Logger.Error("[SupabaseClient] 设备自动登录异常", ex2);
				result = Result<UserIdentity>.Failure("设备自动登录异常：" + ex2.Message);
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
	public struct Struct165 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<UserIdentity>> asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		public string string_0;

		public string string_1;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<UserIdentity> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0117;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_016c;
				}
				supabaseClient.method_1();
				if (!string.IsNullOrEmpty(string_0) && !string.IsNullOrEmpty(string_1))
				{
					Class28<string, string, string, Class29> @class = new Class28<string, string, string, Class29>(string_0, string_1, "https://astools.tech/auth/confirm", new Class29());
					awaiter = supabaseClient.httpClient_0.PostAsync("/auth/v1/signup", new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0117;
				}
				result = Result<UserIdentity>.Failure("电子邮件和密码不能为空");
				goto end_IL_000f;
				IL_0117:
				HttpResponseMessage result2 = awaiter.GetResult();
				httpResponseMessage_0 = result2;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_016c;
				IL_016c:
				string result3 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					result = ((result3.Contains("User already registered") || result3.Contains("already_registered")) ? Result<UserIdentity>.Failure("注册失败：该邮箱已被注册。\n\n提示：请直接登录，或使用其他邮箱注册。") : ((!result3.Contains("Password should be at least")) ? Result<UserIdentity>.Failure("注册失败：" + result3) : Result<UserIdentity>.Failure("注册失败：密码强度不足。\n\n提示：密码至少需要 8 个字符。")));
				}
				else
				{
					Class95 class2 = JsonConvert.DeserializeObject<Class95>(result3);
					if (class2 != null && class2.User != null)
					{
						result = ((!string.IsNullOrEmpty(class2.AccessToken)) ? Result<UserIdentity>.Success(new UserIdentity
						{
							UserId = Guid.Parse(class2.User.Id),
							Email = class2.User.Email,
							AuthToken = class2.AccessToken,
							RefreshToken = class2.RefreshToken,
							TokenExpiresAt = ((class2.ExpiresIn > 0) ? new DateTime?(DateTime.UtcNow.AddSeconds(class2.ExpiresIn)) : ((DateTime?)null)),
							CreatedAt = ((class2.User.CreatedAt != null) ? DateTime.Parse(class2.User.CreatedAt) : DateTime.UtcNow),
							LastSignInAt = ((class2.User.ConfirmedAt != null) ? DateTime.Parse(class2.User.ConfirmedAt) : DateTime.UtcNow)
						}) : Result<UserIdentity>.Success(new UserIdentity
						{
							UserId = Guid.Parse(class2.User.Id),
							Email = class2.User.Email,
							AuthToken = string.Empty,
							RefreshToken = string.Empty,
							TokenExpiresAt = null,
							CreatedAt = ((class2.User.CreatedAt != null) ? DateTime.Parse(class2.User.CreatedAt) : DateTime.UtcNow),
							LastSignInAt = null
						}));
					}
					else
					{
						Logger.Error("[Supabase] 注册响应反序列化失败，原始内容: " + result3);
						result = Result<UserIdentity>.Failure("无效的响应");
					}
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 注册异常", ex);
				result = Result<UserIdentity>.Failure("注册异常：" + ex.Message);
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
	public struct Struct166 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<Guid>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public Guid? nullable_0;

		public string string_2;

		public string string_3;

		public string string_4;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<Guid> result3;
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
						goto IL_01c5;
					}
					object obj = string_0;
					if (!string.IsNullOrEmpty(string_0))
					{
						try
						{
							obj = JToken.Parse(string_0);
						}
						catch
						{
							obj = string_0;
						}
					}
					else
					{
						obj = null;
					}
					Class50<string, string, string, string, string, object> @class = new Class50<string, string, string, string, string, object>(string_1, nullable_0?.ToString(), string_2, string_3, string_4, obj);
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/feedback")
					{
						Headers = { 
						{
							"Authorization",
							"Bearer " + supabaseClient.string_1
						} },
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_01c5;
				IL_01c5:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 提交反馈失败: " + result2);
					result3 = Result<Guid>.Failure("提交反馈失败：" + result2);
				}
				else
				{
					List<Dictionary<string, object>> list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(result2);
					result3 = ((list == null || list.Count <= 0 || !list[0].ContainsKey("id")) ? Result<Guid>.Failure("提交反馈失败：无法获取记录 ID") : Result<Guid>.Success(Guid.Parse(list[0]["id"].ToString() ?? Guid.Empty.ToString())));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 提交反馈异常", ex);
				result3 = Result<Guid>.Failure("提交反馈异常：" + ex.Message);
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
	public struct Struct167 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<LicenseTransferResult>> asyncTaskMethodBuilder_0;

		public Guid guid_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<LicenseTransferResult> result3;
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
						goto IL_0182;
					}
					Class46<Guid, string> @class = new Class46<Guid, string>(guid_0, string_0);
					string requestUri = "/rest/v1/rpc/transfer_device_license_to_user";
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
					{
						Headers = 
						{
							{
								"Authorization",
								"Bearer " + supabaseClient.string_1
							},
							{
								"apikey",
								supabaseClient.string_1
							}
						},
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter2 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
				goto IL_0182;
				IL_0182:
				string result2 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 转移授权失败: " + result2);
					result3 = Result<LicenseTransferResult>.Failure("转移授权失败：" + result2);
				}
				else
				{
					List<LicenseTransferResult> list = JsonConvert.DeserializeObject<List<LicenseTransferResult>>(result2);
					if (list != null && list.Count > 0)
					{
						result3 = Result<LicenseTransferResult>.Success(list[0]);
					}
					else
					{
						Logger.Warning("[Supabase] 转移授权响应为空");
						result3 = Result<LicenseTransferResult>.Failure("转移授权失败：响应为空");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 转移授权异常", ex);
				result3 = Result<LicenseTransferResult>.Failure("转移授权异常：" + ex.Message);
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
	public struct Struct168 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
					if (!string.IsNullOrEmpty(string_0))
					{
						Class37<string> @class = new Class37<string>(string_0);
						awaiter2 = supabaseClient.httpClient_0.PostAsync("/rest/v1/rpc/unbind_device", new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_00cf;
					}
					result = Result.Failure("设备 ID 不能为空");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00cf;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0184;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0184:
					result2 = awaiter.GetResult();
					Logger.Error("[Supabase] 解绑设备失败: " + result2);
					result = Result.Failure("解绑设备失败：" + result2);
					goto end_IL_000f;
					IL_00cf:
					result3 = awaiter2.GetResult();
					if (!result3.IsSuccessStatusCode)
					{
						awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0184;
					}
					awaiter = result3.Content.ReadAsStringAsync().GetAwaiter();
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
				string result4 = awaiter.GetResult();
				if (!string.IsNullOrEmpty(result4) && !(result4 == "[]"))
				{
					result = Result.Success();
				}
				else
				{
					Logger.Warning("[Supabase] 设备不存在或已解绑: " + string_0.Substring(0, Math.Min(16, string_0.Length)) + "...");
					result = Result.Failure("设备不存在");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 解绑设备异常: " + string_0, ex);
				result = Result.Failure("解绑设备异常：" + ex.Message);
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
	public struct Struct169 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<DeviceInfo>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public string string_2;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		private TaskAwaiter<Result<DeviceInfo>> taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<DeviceInfo> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter3;
				TaskAwaiter<string> awaiter2;
				TaskAwaiter<Result<DeviceInfo>> awaiter;
				string result2;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
					if (!string.IsNullOrEmpty(string_0))
					{
						Class38<string, string, string> @class = new Class38<string, string, string>(string_0, string_1, string_2);
						HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/update_device_activity")
						{
							Headers = 
							{
								{
									"Authorization",
									"Bearer " + supabaseClient.string_1
								},
								{
									"apikey",
									supabaseClient.string_1
								}
							},
							Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
						};
						awaiter3 = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter3;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
							return;
						}
						goto IL_0137;
					}
					result = Result<DeviceInfo>.Failure("设备 ID 不能为空");
					goto end_IL_000f;
				case 0:
					awaiter3 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0137;
				case 1:
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01a9;
				case 2:
					{
						awaiter = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter<Result<DeviceInfo>>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01a9:
					result2 = awaiter2.GetResult();
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						Logger.Error("[Supabase] RPC 更新设备失败: " + result2);
						result = Result<DeviceInfo>.Failure("更新设备失败：" + result2);
					}
					else
					{
						DeviceInfo deviceInfo = JsonConvert.DeserializeObject<DeviceInfo>(result2);
						if (deviceInfo == null)
						{
							awaiter = supabaseClient.GetDeviceInfoAsync(string_0).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 2;
								int_0 = 2;
								taskAwaiter_2 = awaiter;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							break;
						}
						result = Result<DeviceInfo>.Success(deviceInfo);
					}
					goto end_IL_000f;
					IL_0137:
					result3 = awaiter3.GetResult();
					httpResponseMessage_0 = result3;
					awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_01a9;
				}
				Result<DeviceInfo> result4 = awaiter.GetResult();
				result = ((!result4.IsSuccess || result4.Value == null) ? Result<DeviceInfo>.Failure("更新设备失败：无法获取更新结果") : Result<DeviceInfo>.Success(result4.Value));
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 更新设备异常", ex);
				result = Result<DeviceInfo>.Failure("更新设备异常：" + ex.Message);
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
	public struct Struct170 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public SupabaseClient supabaseClient_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_012a;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					Class36<string> @class = new Class36<string>(DateTime.UtcNow.ToString("o"));
					HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), "/rest/v1/devices?device_id=eq." + string_0)
					{
						Headers = { 
						{
							"Authorization",
							"Bearer " + supabaseClient.string_1
						} },
						Content = new StringContent(JsonConvert.SerializeObject((object)@class), Encoding.UTF8, "application/json")
					};
					awaiter = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_012a;
				}
				result = Result.Failure("设备 ID 不能为空");
				goto end_IL_000f;
				IL_012a:
				HttpResponseMessage result2 = awaiter.GetResult();
				Result obj;
				if (!result2.IsSuccessStatusCode)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
					defaultInterpolatedStringHandler.AppendLiteral("更新设备失败：");
					defaultInterpolatedStringHandler.AppendFormatted(result2.StatusCode);
					obj = Result.Failure(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					obj = Result.Success();
				}
				result = obj;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result.Failure("更新设备异常：" + ex.Message);
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
	public struct Struct171 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<string>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public byte[] byte_0;

		public SupabaseClient supabaseClient_0;

		public string string_2;

		private string string_3;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<string> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_01f0;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0245;
				}
				if (string.IsNullOrEmpty(string_0))
				{
					result = Result<string>.Failure("存储桶名称不能为空");
				}
				else if (string.IsNullOrEmpty(string_1))
				{
					result = Result<string>.Failure("文件路径不能为空");
				}
				else
				{
					if (byte_0 != null && byte_0.Length != 0)
					{
						string_3 = supabaseClient.string_0.Remove(supabaseClient.string_0.LastIndexOf("/rest"));
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 3);
						defaultInterpolatedStringHandler.AppendFormatted(string_3);
						defaultInterpolatedStringHandler.AppendLiteral("/storage/v1/object/");
						defaultInterpolatedStringHandler.AppendFormatted(string_0);
						defaultInterpolatedStringHandler.AppendLiteral("/");
						defaultInterpolatedStringHandler.AppendFormatted(string_1);
						string requestUri = defaultInterpolatedStringHandler.ToStringAndClear();
						HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
						httpRequestMessage.Headers.Add("Authorization", "Bearer " + supabaseClient.string_1);
						httpRequestMessage.Content = new ByteArrayContent(byte_0);
						httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(string_2);
						awaiter = supabaseClient.httpClient_0.SendAsync(httpRequestMessage).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_01f0;
					}
					result = Result<string>.Failure("文件内容不能为空");
				}
				goto end_IL_000f;
				IL_0245:
				string result2 = awaiter2.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Error("[Supabase] 上传文件失败: " + result2);
					result = Result<string>.Failure("上传文件失败：" + result2);
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 3);
					defaultInterpolatedStringHandler2.AppendFormatted(string_3);
					defaultInterpolatedStringHandler2.AppendLiteral("/storage/v1/object/public/");
					defaultInterpolatedStringHandler2.AppendFormatted(string_0);
					defaultInterpolatedStringHandler2.AppendLiteral("/");
					defaultInterpolatedStringHandler2.AppendFormatted(string_1);
					result = Result<string>.Success(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				goto end_IL_000f;
				IL_01f0:
				HttpResponseMessage result3 = awaiter.GetResult();
				httpResponseMessage_0 = result3;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_0245;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 上传文件异常", ex);
				result = Result<string>.Failure("上传文件异常：" + ex.Message);
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
	public struct Struct172 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public byte[] byte_0;

		public string string_1;

		public SupabaseClient supabaseClient_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_012d;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_017f;
				}
				if (string.IsNullOrEmpty(string_0))
				{
					result = Result.Failure("预签名 URL 不能为空");
				}
				else
				{
					if (byte_0 != null && byte_0.Length != 0)
					{
						HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod("PUT"), string_0);
						httpRequestMessage.Content = new ByteArrayContent(byte_0);
						httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(string_1);
						awaiter = supabaseClient.httpClient_0.SendAsync(httpRequestMessage).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_012d;
					}
					result = Result.Failure("文件内容不能为空");
				}
				goto end_IL_000f;
				IL_012d:
				HttpResponseMessage result2 = awaiter.GetResult();
				if (!result2.IsSuccessStatusCode)
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
					goto IL_017f;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[Supabase] 成功上传 ");
				defaultInterpolatedStringHandler.AppendFormatted(byte_0.Length);
				defaultInterpolatedStringHandler.AppendLiteral(" 字节到 COS");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				result = Result.Success();
				goto end_IL_000f;
				IL_017f:
				string result3 = awaiter2.GetResult();
				Logger.Error("[Supabase] 上传到 COS 失败: " + result3);
				result = Result.Failure("上传到 COS 失败：" + result3);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 上传到 COS 异常", ex);
				result = Result.Failure("上传到 COS 异常：" + ex.Message);
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
	public struct Struct173 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<PromotionCodeValidationResult>> asyncTaskMethodBuilder_0;

		public string string_0;

		public decimal decimal_0;

		public string string_1;

		public Guid? nullable_0;

		public string string_2;

		public SupabaseClient supabaseClient_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Expected O, but got Unknown
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			Result<PromotionCodeValidationResult> result2;
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
						goto IL_01a2;
					}
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class54<string, decimal, string, Guid?, string>(string_0.Trim(), decimal_0, string_1, nullable_0, string_2)), Encoding.UTF8, "application/json");
					HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/rpc/validate_promotion_code")
					{
						Content = content
					};
					if (!string.IsNullOrEmpty(supabaseClient.string_1))
					{
						httpRequestMessage.Headers.Add("Authorization", "Bearer " + supabaseClient.string_1);
						httpRequestMessage.Headers.Add("apikey", supabaseClient.string_1);
					}
					awaiter2 = supabaseClient.httpClient_0.SendAsync(httpRequestMessage).GetAwaiter();
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
				goto IL_01a2;
				IL_0361:
				PromotionCode promotionCode;
				Class103 @class;
				result2 = Result<PromotionCodeValidationResult>.Success(new PromotionCodeValidationResult
				{
					IsValid = true,
					PromotionCode = promotionCode,
					DiscountedPrice = @class.discounted_price,
					SavedAmount = @class.saved_amount
				});
				goto end_IL_000f;
				IL_025a:
				object obj;
				string text = (string)obj;
				object obj3;
				if (string.IsNullOrEmpty(text))
				{
					result2 = Result<PromotionCodeValidationResult>.Failure("优惠码信息为空");
				}
				else
				{
					promotionCode = JsonConvert.DeserializeObject<PromotionCode>(text, new JsonSerializerSettings
					{
						FloatParseHandling = (FloatParseHandling)1
					});
					if (promotionCode != null)
					{
						JObject val = JObject.Parse(text);
						if (promotionCode.DiscountValue == 0m && val["discount_value"] != null)
						{
							string text2 = ((object)val["discount_value"])?.ToString();
							if (!string.IsNullOrEmpty(text2) && decimal.TryParse(text2, out var result3))
							{
								promotionCode.DiscountValue = result3;
							}
						}
						if (string.IsNullOrEmpty(promotionCode.DiscountType) && val["discount_type"] != null)
						{
							JToken obj2 = val["discount_type"];
							if (obj2 == null)
							{
								obj3 = null;
							}
							else
							{
								obj3 = ((object)obj2).ToString();
								if (obj3 != null)
								{
									goto IL_035c;
								}
							}
							obj3 = string.Empty;
							goto IL_035c;
						}
						goto IL_0361;
					}
					result2 = Result<PromotionCodeValidationResult>.Failure("解析优惠码信息失败");
				}
				goto end_IL_000f;
				IL_035c:
				promotionCode.DiscountType = (string)obj3;
				goto IL_0361;
				IL_01a2:
				string result4 = awaiter.GetResult();
				if (!httpResponseMessage_0.IsSuccessStatusCode)
				{
					Logger.Warning("[Supabase] 验证优惠码失败: " + result4);
					result2 = Result<PromotionCodeValidationResult>.Failure("验证失败：" + result4);
				}
				else
				{
					List<Class103> list = JsonConvert.DeserializeObject<List<Class103>>(result4);
					if (list != null && list.Count > 0)
					{
						@class = list[0];
						if (@class.is_valid)
						{
							object? promotion_code = @class.promotion_code;
							if (promotion_code == null)
							{
								obj = null;
							}
							else
							{
								obj = promotion_code.ToString();
								if (obj != null)
								{
									goto IL_025a;
								}
							}
							obj = string.Empty;
							goto IL_025a;
						}
						result2 = Result<PromotionCodeValidationResult>.Success(new PromotionCodeValidationResult
						{
							IsValid = false,
							ErrorMessage = @class.error_message
						});
					}
					else
					{
						result2 = Result<PromotionCodeValidationResult>.Failure("无效的响应");
					}
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[Supabase] 验证优惠码异常", ex);
				result2 = Result<PromotionCodeValidationResult>.Failure("验证异常：" + ex.Message);
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
	public struct Struct174 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public SupabaseClient supabaseClient_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SupabaseClient supabaseClient = supabaseClient_0;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num != 0)
				{
					supabaseClient.method_1();
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, "/rest/v1/");
					awaiter = supabaseClient.httpClient_0.SendAsync(request).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
			}
			catch
			{
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

	private readonly INativeCryptoService inativeCryptoService_0;

	private readonly string string_0;

	private string? string_1;

	private static IHttpClientFactory? ihttpClientFactory_0;

	private bool bool_0;

	public string BaseUrl => string_0;

	public HttpClient HttpClient => httpClient_0;

	public SupabaseClient(INativeCryptoService cryptoService, IHttpClientFactory? httpClientFactory = null)
	{
		inativeCryptoService_0 = cryptoService ?? throw new ArgumentNullException("cryptoService");
		string_0 = inativeCryptoService_0.GetSupabaseUrl();
		NativeCryptoService obj = inativeCryptoService_0 as NativeCryptoService;
		object obj2;
		if (obj == null)
		{
			obj2 = null;
		}
		else
		{
			obj2 = obj.method_0();
			if (obj2 != null)
			{
				string_1 = (string?)obj2;
				ihttpClientFactory_0 = httpClientFactory;
				httpClient_0 = method_0(ihttpClientFactory_0);
				httpClient_0.DefaultRequestHeaders.Add("apikey", string_1);
				httpClient_0.DefaultRequestHeaders.Add("Prefer", "return=representation");
				httpClient_0.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
				return;
			}
		}
		throw new InvalidOperationException("无法获取 API Key");
	}

	private HttpClient method_0(IHttpClientFactory? ihttpClientFactory_1)
	{
		if (ihttpClientFactory_1 != null)
		{
			Logger.Info("[SupabaseClient] 使用 HttpClientFactory 创建 HttpClient");
			return ihttpClientFactory_1.CreateClient(string_0, 30);
		}
		Logger.Info("[SupabaseClient] 使用默认方式创建 HttpClient（建议注入 HttpClientFactory）");
		HttpClientHandler httpClientHandler = new HttpClientHandler
		{
			UseProxy = true,
			UseCookies = false
		};
		try
		{
			IWebProxy systemWebProxy = WebRequest.GetSystemWebProxy();
			if (systemWebProxy != null)
			{
				httpClientHandler.Proxy = systemWebProxy;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[SupabaseClient] 获取系统代理失败: " + ex.Message);
		}
		return new HttpClient(httpClientHandler)
		{
			BaseAddress = new Uri(string_0),
			Timeout = TimeSpan.FromSeconds(30L)
		};
	}

	[AsyncStateMachine(typeof(Struct174))]
	public Task WarmUpConnectionAsync()
	{
		Struct174 stateMachine = default(Struct174);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private void method_1()
	{
	}

	[AsyncStateMachine(typeof(Struct165))]
	public Task<Result<UserIdentity>> SignUpAsync(string email, string password)
	{
		Struct165 stateMachine = default(Struct165);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<UserIdentity>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = email;
		stateMachine.string_1 = password;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct163))]
	public Task<Result<UserIdentity>> SignInAsync(string email, string password)
	{
		Struct163 stateMachine = default(Struct163);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<UserIdentity>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = email;
		stateMachine.string_1 = password;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct164))]
	public Task<Result<UserIdentity>> SignInByDeviceAsync(string deviceId)
	{
		Struct164 stateMachine = default(Struct164);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<UserIdentity>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public Task<Result> SignOutAsync()
	{
		try
		{
			return Task.FromResult<Result>(Result.Success());
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result>(Result.Failure("登出异常：" + ex.Message));
		}
	}

	[AsyncStateMachine(typeof(Struct160))]
	public Task<Result<UserIdentity>> RefreshTokenAsync(string refreshToken)
	{
		Struct160 stateMachine = default(Struct160);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<UserIdentity>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = refreshToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct162))]
	public Task<Result> RequestPasswordResetAsync(string email)
	{
		Struct162 stateMachine = default(Struct162);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = email;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct161))]
	public Task<Result<DeviceInfo>> RegisterDeviceAsync(DeviceInfo deviceInfo)
	{
		Struct161 stateMachine = default(Struct161);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<DeviceInfo>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.deviceInfo_0 = deviceInfo;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct133))]
	public Task<Result<DeviceInfo>> GetDeviceInfoAsync(string deviceId)
	{
		Struct133 stateMachine = default(Struct133);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<DeviceInfo>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct118))]
	public Task<Result> BindDeviceAsync(string deviceId, Guid userId)
	{
		Struct118 stateMachine = default(Struct118);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.guid_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct170))]
	public Task<Result> UpdateDeviceLastSeenAsync(string deviceId)
	{
		Struct170 stateMachine = default(Struct170);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct149))]
	public Task<Result<List<DeviceInfo>>> GetUserDevicesAsync(Guid userId)
	{
		Struct149 stateMachine = default(Struct149);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<DeviceInfo>>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.guid_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct168))]
	public Task<Result> UnbindDeviceAsync(string deviceId)
	{
		Struct168 stateMachine = default(Struct168);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct169))]
	public Task<Result<DeviceInfo>> UpdateDeviceAsync(string deviceId, string revitVersion, string ipAddress)
	{
		Struct169 stateMachine = default(Struct169);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<DeviceInfo>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.string_1 = revitVersion;
		stateMachine.string_2 = ipAddress;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct155))]
	public Task<Result<MigrateDeviceResult>> MigrateDeviceIdCopyAsync(string oldDeviceId, string newDeviceId, string? deviceName = null, string? osInfo = null, string? ipAddress = null)
	{
		Struct155 stateMachine = default(Struct155);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<MigrateDeviceResult>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = oldDeviceId;
		stateMachine.string_1 = newDeviceId;
		stateMachine.string_2 = deviceName;
		stateMachine.string_3 = osInfo;
		stateMachine.string_4 = ipAddress;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct158))]
	public Task<Result<int>> RecordTrialUsageAsync(string deviceId, string featureId, Guid? userId = null, string? featureGroup = null)
	{
		Struct158 stateMachine = default(Struct158);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<int>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.string_2 = featureId;
		stateMachine.nullable_0 = userId;
		stateMachine.string_1 = featureGroup;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct159))]
	public Task<Result<TrialRecordDetail>> RecordTrialUsageV2Async(string deviceId, string featureGroup, string featureId, Guid? userId = null, string trialVersion = "v1.0")
	{
		Struct159 stateMachine = default(Struct159);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<TrialRecordDetail>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.string_2 = featureGroup;
		stateMachine.string_1 = featureId;
		stateMachine.nullable_0 = userId;
		stateMachine.string_3 = trialVersion;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct120))]
	public Task<Result<TrialEligibility>> CheckTrialEligibilityAsync(string deviceId, string featureId, Guid? userId = null)
	{
		Struct120 stateMachine = default(Struct120);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<TrialEligibility>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.string_1 = featureId;
		stateMachine.nullable_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct141))]
	public Task<Result<int>> GetMaxTrialLimitAsync(string featureGroup, string? featureId = null)
	{
		Struct141 stateMachine = default(Struct141);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<int>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = featureId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct139))]
	public Task<Result<LicenseInfo>> GetLicenseInfoAsync(Guid userId)
	{
		Struct139 stateMachine = default(Struct139);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<LicenseInfo>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.guid_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct138))]
	public Task<Result<LicenseInfo>> GetLicenseByDeviceAsync(string deviceId)
	{
		Struct138 stateMachine = default(Struct138);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<LicenseInfo>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct140))]
	public Task<Result<List<LicensePackage>>> GetLicensePackagesAsync()
	{
		Struct140 stateMachine = default(Struct140);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<LicensePackage>>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct145))]
	public Task<Result<List<TrialRecordDetail>>> GetTrialRecordsByUserAsync(Guid userId)
	{
		Struct145 stateMachine = default(Struct145);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<TrialRecordDetail>>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.guid_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct144))]
	public Task<Result<List<TrialRecordDetail>>> GetTrialRecordsByDeviceAsync(string deviceId)
	{
		Struct144 stateMachine = default(Struct144);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<TrialRecordDetail>>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct130))]
	public Task<Result<List<TrialLimitConfig>>> GetAllTrialLimitsAsync()
	{
		Struct130 stateMachine = default(Struct130);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<TrialLimitConfig>>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct152))]
	public Task<Result> InsertTrialRecordAsync(string deviceId, string featureId, string? featureGroup, int maxUsageCount, Guid? userId = null)
	{
		Struct152 stateMachine = default(Struct152);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.string_1 = featureId;
		stateMachine.string_2 = featureGroup;
		stateMachine.int_1 = maxUsageCount;
		stateMachine.nullable_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct143))]
	public Task<Result<TrialRecordDetail?>> GetTrialRecordAsync(string deviceId, string featureId, Guid? userId = null, string? featureGroup = null)
	{
		Struct143 stateMachine = default(Struct143);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<TrialRecordDetail>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.string_1 = featureId;
		stateMachine.nullable_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct122))]
	public Task<Result> CreateTrialRecordAsync(string deviceId, string featureId, string? featureGroup, int maxUsageCount, Guid? userId = null)
	{
		Struct122 stateMachine = default(Struct122);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.string_1 = featureId;
		stateMachine.string_2 = featureGroup;
		stateMachine.int_1 = maxUsageCount;
		stateMachine.nullable_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct129))]
	public Task<Result<List<AIConfig>>> GetAllActiveAIConfigsAsync()
	{
		Struct129 stateMachine = default(Struct129);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<AIConfig>>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct127))]
	public Task<Result<AIConfig>> GetAIConfigByModelNameAsync(string modelName)
	{
		Struct127 stateMachine = default(Struct127);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<AIConfig>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = modelName;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct132))]
	public Task<Result<AIConfig>> GetDefaultAIConfigAsync()
	{
		Struct132 stateMachine = default(Struct132);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<AIConfig>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct128))]
	public Task<Result<AIConfig>> GetAIConfigByProviderAsync(string provider)
	{
		Struct128 stateMachine = default(Struct128);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<AIConfig>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = provider;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct134))]
	public Task<Result<EncryptedApiKeyRecord>> GetEncryptedApiKeyAsync(string apiUrl)
	{
		Struct134 stateMachine = default(Struct134);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<EncryptedApiKeyRecord>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = apiUrl;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct135))]
	public Task<Result<EncryptedApiKeyRecord>> GetEncryptedApiKeyByNotesAsync(string notes)
	{
		Struct135 stateMachine = default(Struct135);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<EncryptedApiKeyRecord>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = notes;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct148))]
	public Task<Result<CombinedCreditsInfo>> GetUserCreditsAsync(Guid? userId = null, Guid? deviceId = null)
	{
		Struct148 stateMachine = default(Struct148);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<CombinedCreditsInfo>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.nullable_0 = userId;
		stateMachine.nullable_1 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct137))]
	public Task<Result<InitialCreditsInfo>> GetInitialCreditsAsync()
	{
		Struct137 stateMachine = default(Struct137);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<InitialCreditsInfo>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct150))]
	public Task<Result<DeviceCreditsResult>> InitializeDeviceCreditsAsync(Guid deviceId)
	{
		Struct150 stateMachine = default(Struct150);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<DeviceCreditsResult>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.guid_0 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct151))]
	public Task<Result<UserCreditsResult>> InitializeUserCreditsAsync(Guid userId)
	{
		Struct151 stateMachine = default(Struct151);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<UserCreditsResult>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.guid_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct167))]
	public Task<Result<LicenseTransferResult>> TransferLicenseToUserOnLoginAsync(string deviceId, Guid userId)
	{
		Struct167 stateMachine = default(Struct167);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<LicenseTransferResult>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.guid_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct119))]
	public Task<Result<bool>> CheckCreditsSufficientAsync(decimal requiredAmount, Guid? userId = null, Guid? deviceId = null)
	{
		Struct119 stateMachine = default(Struct119);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<bool>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.decimal_0 = requiredAmount;
		stateMachine.nullable_0 = userId;
		stateMachine.nullable_1 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct157))]
	public Task<Result<Guid>> RecordTokenUsageOnlyAsync(Guid? userId, Guid? deviceId, string? sessionId, string provider, string modelName, int inputTokens, int outputTokens, string requestType = "chat", bool isToolCall = false, int toolCount = 0, int? durationMs = null)
	{
		Struct157 stateMachine = default(Struct157);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<Guid>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.nullable_0 = userId;
		stateMachine.nullable_1 = deviceId;
		stateMachine.string_2 = sessionId;
		stateMachine.string_0 = provider;
		stateMachine.string_1 = modelName;
		stateMachine.int_1 = inputTokens;
		stateMachine.int_2 = outputTokens;
		stateMachine.string_3 = requestType;
		stateMachine.bool_0 = isToolCall;
		stateMachine.int_3 = toolCount;
		stateMachine.nullable_2 = durationMs;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct142))]
	public Task<Result<List<TokenUsageInfo>>> GetRecentTokenUsageAsync(int limit = 50)
	{
		Struct142 stateMachine = default(Struct142);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<TokenUsageInfo>>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.int_1 = limit;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct124))]
	public Task<Result<List<CreditDeductionResult>>> DeductCreditsSmartAsync(Guid? userId, Guid? deviceId, decimal amount)
	{
		Struct124 stateMachine = default(Struct124);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<CreditDeductionResult>>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.nullable_0 = userId;
		stateMachine.nullable_1 = deviceId;
		stateMachine.decimal_0 = amount;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct123))]
	public Task<Result<CreditDeductionResult>> DeductCreditsSafeAsync(Guid? userId, Guid? deviceId, decimal amount)
	{
		Struct123 stateMachine = default(Struct123);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<CreditDeductionResult>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.nullable_0 = userId;
		stateMachine.nullable_1 = deviceId;
		stateMachine.decimal_0 = amount;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct121))]
	public Task<Result<Guid>> CreateCreditRechargeAsync(string orderId, decimal amount, int credits, Guid? userId = null, Guid? deviceId = null, decimal? originalPrice = null, int? originalCredits = null, decimal discountRate = 1.0m, string paymentMethod = "alipay")
	{
		Struct121 stateMachine = default(Struct121);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<Guid>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = orderId;
		stateMachine.decimal_0 = amount;
		stateMachine.int_1 = credits;
		stateMachine.nullable_0 = userId;
		stateMachine.nullable_1 = deviceId;
		stateMachine.nullable_2 = originalPrice;
		stateMachine.nullable_3 = originalCredits;
		stateMachine.decimal_1 = discountRate;
		stateMachine.string_1 = paymentMethod;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct156))]
	public Task<Result<Guid>> RecordAIMessageAsync(string messageText, string featureId = "AI_Send", string? sessionId = null, string? aiProvider = null, string? aiModel = null, Guid? deviceId = null, Guid? userId = null)
	{
		Struct156 stateMachine = default(Struct156);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<Guid>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = messageText;
		stateMachine.string_1 = featureId;
		stateMachine.string_2 = sessionId;
		stateMachine.string_3 = aiProvider;
		stateMachine.string_4 = aiModel;
		stateMachine.nullable_0 = deviceId;
		stateMachine.nullable_1 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct166))]
	public Task<Result<Guid>> SubmitFeedbackAsync(string deviceId, Guid? userId, string title, string content, string? email = null, string? attachments = null)
	{
		Struct166 stateMachine = default(Struct166);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<Guid>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_1 = deviceId;
		stateMachine.nullable_0 = userId;
		stateMachine.string_2 = title;
		stateMachine.string_3 = content;
		stateMachine.string_4 = email;
		stateMachine.string_0 = attachments;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct136))]
	public Task<Result<List<FeedbackInfo>>> GetFeedbackListAsync(string deviceId, Guid? userId = null)
	{
		Struct136 stateMachine = default(Struct136);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<FeedbackInfo>>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.nullable_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct146))]
	public Task<Result<int>> GetUnreadReplyCountAsync(string deviceId, Guid? userId = null)
	{
		Struct146 stateMachine = default(Struct146);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<int>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.nullable_0 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct147))]
	private Task<Result<int>> method_2(string string_2, Guid? nullable_0)
	{
		Struct147 stateMachine = default(Struct147);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<int>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = string_2;
		stateMachine.nullable_0 = nullable_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct125))]
	public Task<Result> DeleteFeedbackAsync(Guid feedbackId)
	{
		Struct125 stateMachine = default(Struct125);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.guid_0 = feedbackId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct153))]
	public Task<Result> MarkFeedbackAsReadAsync(Guid feedbackId, Guid userId)
	{
		Struct153 stateMachine = default(Struct153);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.guid_0 = feedbackId;
		stateMachine.guid_1 = userId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct154))]
	public Task<Result> MarkFeedbackAsReadAsync(Guid feedbackId, string deviceId)
	{
		Struct154 stateMachine = default(Struct154);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.guid_0 = feedbackId;
		stateMachine.string_0 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct171))]
	public Task<Result<string>> UploadFileAsync(string bucketName, string path, byte[] fileBytes, string contentType = "application/octet-stream")
	{
		Struct171 stateMachine = default(Struct171);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<string>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = bucketName;
		stateMachine.string_1 = path;
		stateMachine.byte_0 = fileBytes;
		stateMachine.string_2 = contentType;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct126))]
	public Task<Result> DeleteFileAsync(string bucketName, string path)
	{
		Struct126 stateMachine = default(Struct126);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = bucketName;
		stateMachine.string_1 = path;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct131))]
	public Task<Result<CosPresignedUrlInfo>> GetCosPresignedUrlAsync(string filename, string contentType = "application/octet-stream")
	{
		Struct131 stateMachine = default(Struct131);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<CosPresignedUrlInfo>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = filename;
		stateMachine.string_1 = contentType;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct172))]
	public Task<Result> UploadToCosWithPresignedUrlAsync(string presignedUrl, byte[] fileBytes, string contentType)
	{
		Struct172 stateMachine = default(Struct172);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = presignedUrl;
		stateMachine.byte_0 = fileBytes;
		stateMachine.string_1 = contentType;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public void Dispose()
	{
		method_3(bool_1: true);
		GC.SuppressFinalize(this);
	}

	private void method_3(bool bool_1)
	{
		if (!bool_0)
		{
			if (bool_1)
			{
				httpClient_0?.Dispose();
			}
			bool_0 = true;
		}
	}

	~SupabaseClient()
	{
		method_3(bool_1: false);
	}

	[AsyncStateMachine(typeof(Struct173))]
	public Task<Result<PromotionCodeValidationResult>> ValidatePromotionCodeAsync(string code, decimal originalPrice, string featureType, Guid? userId = null, string? deviceId = null)
	{
		Struct173 stateMachine = default(Struct173);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<PromotionCodeValidationResult>>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = code;
		stateMachine.decimal_0 = originalPrice;
		stateMachine.string_1 = featureType;
		stateMachine.nullable_0 = userId;
		stateMachine.string_2 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct117))]
	public Task<Result> ActivatePromotionCodeAsync(string code, string orderId, Guid? userId = null, string? deviceId = null)
	{
		Struct117 stateMachine = default(Struct117);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.supabaseClient_0 = this;
		stateMachine.string_0 = code;
		stateMachine.string_1 = orderId;
		stateMachine.nullable_0 = userId;
		stateMachine.string_2 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[CompilerGenerated]
	internal static string smethod_0(dynamic object_0, string string_2 = "")
	{
		if (object_0 == null)
		{
			return string.Empty;
		}
		if (Class107.callSite_2 == null)
		{
			Class107.callSite_2 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		dynamic val = Class107.callSite_2.Target(Class107.callSite_2, (object)object_0);
		if (Class107.callSite_3 == null)
		{
			Class107.callSite_3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "StartsWith", null, typeof(SupabaseClient), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		dynamic val2 = Class107.callSite_3.Target(Class107.callSite_3, (object)val, "\"");
		dynamic val3;
		if (!(val2 ? false : true))
		{
			if (Class107.callSite_4 == null)
			{
				Class107.callSite_4 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "EndsWith", null, typeof(SupabaseClient), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			val3 = val2 & (dynamic)Class107.callSite_4.Target(Class107.callSite_4, (object)val, "\"");
		}
		else
		{
			val3 = val2;
		}
		if (val3)
		{
			if (Class107.callSite_10 == null)
			{
				Class107.callSite_10 = CallSite<Func<CallSite, object, int, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Substring", null, typeof(SupabaseClient), new CSharpArgumentInfo[3]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, object, int, object, object> target = Class107.callSite_10.Target;
			CallSite<Func<CallSite, object, int, object, object>> callSite_ = Class107.callSite_10;
			dynamic arg = val;
			if (Class107.callSite_8 == null)
			{
				Class107.callSite_8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Length", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			val = target(callSite_, (object)arg, 1, (object)((dynamic)Class107.callSite_8.Target(Class107.callSite_8, (object)val) - 2));
		}
		return val;
	}

	[CompilerGenerated]
	internal static Guid smethod_1(dynamic object_0, string string_2 = "")
	{
		if (object_0 == null)
		{
			return Guid.Empty;
		}
		return Guid.Parse(smethod_0((object)object_0, string_2));
	}

	[CompilerGenerated]
	internal static DateTime? smethod_2(dynamic object_0)
	{
		if (object_0 == null)
		{
			return null;
		}
		string text = smethod_0((object)object_0, "");
		string[] array = new string[17]
		{
			"yyyy-MM-ddTHH:mm:ssZ",
			"yyyy-MM-ddTHH:mm:ss.fffZ",
			"yyyy-MM-ddTHH:mm:ss",
			"yyyy-MM-ddTHH:mm:ss.fff",
			"yyyy-MM-dd HH:mm:ss",
			"yyyy-MM-dd HH:mm:ss.fff",
			"yyyy/M/d HH:mm:ss",
			"yyyy/M/d HH:mm:ss.fff",
			"yyyy/MM/dd HH:mm:ss",
			"yyyy/MM/dd HH:mm:ss.fff",
			"yyyy-MM-dd",
			"yyyy/M/d",
			"yyyy/MM/dd",
			"yyyy/M/d/ddd H:mm:ss",
			"yyyy/M/d/ddd HH:mm:ss",
			"yyyy/M/d/dddd H:mm:ss",
			"yyyy/M/d/dddd HH:mm:ss"
		};
		string[] array2 = array;
		int num = 0;
		DateTime result;
		while (true)
		{
			if (num < array2.Length)
			{
				string format = array2[num];
				if (DateTime.TryParseExact(text, format, null, DateTimeStyles.None, out result))
				{
					break;
				}
				num++;
				continue;
			}
			string s = Regex.Replace(text, "/[周一周ニ周三周四周五六日日]+", "");
			array2 = array;
			num = 0;
			DateTime result2;
			while (true)
			{
				if (num < array2.Length)
				{
					string format2 = array2[num];
					if (DateTime.TryParseExact(s, format2, null, DateTimeStyles.None, out result2))
					{
						break;
					}
					num++;
					continue;
				}
				try
				{
					return DateTime.SpecifyKind(DateTime.Parse(text), DateTimeKind.Utc);
				}
				catch (Exception ex)
				{
					Logger.Warning("[Supabase] 无法解析日期字符串 '" + text + "': " + ex.Message);
					return null;
				}
			}
			return DateTime.SpecifyKind(result2, DateTimeKind.Utc);
		}
		return DateTime.SpecifyKind(result, DateTimeKind.Utc);
	}

	[CompilerGenerated]
	internal static string smethod_3(dynamic object_0, string string_2 = "")
	{
		if (object_0 == null)
		{
			return string.Empty;
		}
		if (Class108.callSite_2 == null)
		{
			Class108.callSite_2 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		dynamic val = Class108.callSite_2.Target(Class108.callSite_2, (object)object_0);
		if (Class108.callSite_3 == null)
		{
			Class108.callSite_3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "StartsWith", null, typeof(SupabaseClient), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		dynamic val2 = Class108.callSite_3.Target(Class108.callSite_3, (object)val, "\"");
		dynamic val3;
		if (!(val2 ? false : true))
		{
			if (Class108.callSite_4 == null)
			{
				Class108.callSite_4 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "EndsWith", null, typeof(SupabaseClient), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			val3 = val2 & (dynamic)Class108.callSite_4.Target(Class108.callSite_4, (object)val, "\"");
		}
		else
		{
			val3 = val2;
		}
		if (val3)
		{
			if (Class108.callSite_10 == null)
			{
				Class108.callSite_10 = CallSite<Func<CallSite, object, int, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Substring", null, typeof(SupabaseClient), new CSharpArgumentInfo[3]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, object, int, object, object> target = Class108.callSite_10.Target;
			CallSite<Func<CallSite, object, int, object, object>> callSite_ = Class108.callSite_10;
			dynamic arg = val;
			if (Class108.callSite_8 == null)
			{
				Class108.callSite_8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Length", typeof(SupabaseClient), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			val = target(callSite_, (object)arg, 1, (object)((dynamic)Class108.callSite_8.Target(Class108.callSite_8, (object)val) - 2));
		}
		return val;
	}

	[CompilerGenerated]
	internal static Guid smethod_4(dynamic object_0, string string_2 = "")
	{
		if (object_0 == null)
		{
			return Guid.Empty;
		}
		return Guid.Parse(smethod_3((object)object_0, string_2));
	}

	[CompilerGenerated]
	internal static DateTime? smethod_5(dynamic object_0)
	{
		if (object_0 == null)
		{
			return null;
		}
		string text = smethod_3((object)object_0, "");
		string[] array = new string[17]
		{
			"yyyy-MM-ddTHH:mm:ssZ",
			"yyyy-MM-ddTHH:mm:ss.fffZ",
			"yyyy-MM-ddTHH:mm:ss",
			"yyyy-MM-ddTHH:mm:ss.fff",
			"yyyy-MM-dd HH:mm:ss",
			"yyyy-MM-dd HH:mm:ss.fff",
			"yyyy/M/d HH:mm:ss",
			"yyyy/M/d HH:mm:ss.fff",
			"yyyy/MM/dd HH:mm:ss",
			"yyyy/MM/dd HH:mm:ss.fff",
			"yyyy-MM-dd",
			"yyyy/M/d",
			"yyyy/MM/dd",
			"yyyy/M/d/ddd H:mm:ss",
			"yyyy/M/d/ddd HH:mm:ss",
			"yyyy/M/d/dddd H:mm:ss",
			"yyyy/M/d/dddd HH:mm:ss"
		};
		string[] array2 = array;
		int num = 0;
		DateTime result;
		while (true)
		{
			if (num < array2.Length)
			{
				string format = array2[num];
				if (DateTime.TryParseExact(text, format, null, DateTimeStyles.None, out result))
				{
					break;
				}
				num++;
				continue;
			}
			string s = Regex.Replace(text, "/[周一周ニ周三周四周五六日日]+", "");
			array2 = array;
			num = 0;
			DateTime result2;
			while (true)
			{
				if (num < array2.Length)
				{
					string format2 = array2[num];
					if (DateTime.TryParseExact(s, format2, null, DateTimeStyles.None, out result2))
					{
						break;
					}
					num++;
					continue;
				}
				try
				{
					return DateTime.SpecifyKind(DateTime.Parse(text), DateTimeKind.Utc);
				}
				catch (Exception ex)
				{
					Logger.Warning("[Supabase] 无法解析日期字符串 '" + text + "': " + ex.Message);
					return null;
				}
			}
			return DateTime.SpecifyKind(result2, DateTimeKind.Utc);
		}
		return DateTime.SpecifyKind(result, DateTimeKind.Utc);
	}
}
