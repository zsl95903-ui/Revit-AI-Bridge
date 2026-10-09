using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ns0;
using ns7;

namespace RevitAi.Core.FamilyLibrary;

public class FamilyLibraryService : IFamilyLibraryService
{
	[CompilerGenerated]
	public sealed class Class77
	{
		public FamilyLibraryService familyLibraryService_0;

		public AuthorizationState authorizationState_0;

		internal Task<Result<DownloadQuotaInfo>>? method_0()
		{
			return familyLibraryService_0.CalculateDownloadQuotaLocallyAsync(authorizationState_0);
		}
	}

	[CompilerGenerated]
	public sealed class Class78
	{
		[StructLayout(LayoutKind.Auto)]
		public struct Struct58 : IAsyncStateMachine
		{
			public int int_0;

			public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

			public Class78 class78_0;

			private TaskAwaiter taskAwaiter_0;

			void IAsyncStateMachine.MoveNext()
			{
				int num = int_0;
				Class78 @class = class78_0;
				TaskAwaiter awaiter;
				if (num != 0)
				{
					awaiter = @class.familyLibraryService_0.method_4(@class.guid_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				int_0 = -2;
				asyncTaskMethodBuilder_0.SetResult();
			}

			[DebuggerHidden]
			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
			{
				asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
			}
		}

		public Guid guid_0;

		public FamilyLibraryService familyLibraryService_0;

		[AsyncStateMachine(typeof(Struct58))]
		internal Task? method_0()
		{
			Struct58 stateMachine = default(Struct58);
			stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
			stateMachine.class78_0 = this;
			stateMachine.int_0 = -1;
			stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
			return stateMachine.asyncTaskMethodBuilder_0.Task;
		}
	}

	[CompilerGenerated]
	public sealed class Class79
	{
		public DateTime dateTime_0;

		public DateTime dateTime_1;

		public Class78 class78_0;

		internal bool method_0(DownloadRecordItem downloadRecordItem_0)
		{
			if (downloadRecordItem_0.FamilyId == class78_0.guid_0)
			{
				return (dateTime_0 - DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null)).TotalHours <= 24.0;
			}
			return false;
		}

		internal bool method_1(DownloadRecordItem downloadRecordItem_0)
		{
			return DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null) < dateTime_1;
		}
	}

	[CompilerGenerated]
	public sealed class Class80
	{
		[StructLayout(LayoutKind.Auto)]
		public struct Struct59 : IAsyncStateMachine
		{
			public int int_0;

			public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

			public Class80 class80_0;

			private TaskAwaiter taskAwaiter_0;

			void IAsyncStateMachine.MoveNext()
			{
				int num = int_0;
				Class80 @class = class80_0;
				TaskAwaiter awaiter;
				if (num != 0)
				{
					awaiter = @class.familyLibraryService_0.RecordDownloadLocallyAsync(@class.guid_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				int_0 = -2;
				asyncTaskMethodBuilder_0.SetResult();
			}

			[DebuggerHidden]
			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
			{
				asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
			}
		}

		public FamilyLibraryService familyLibraryService_0;

		public Guid guid_0;

		[AsyncStateMachine(typeof(Struct59))]
		internal Task? method_0()
		{
			Struct59 stateMachine = default(Struct59);
			stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
			stateMachine.class80_0 = this;
			stateMachine.int_0 = -1;
			stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
			return stateMachine.asyncTaskMethodBuilder_0.Task;
		}
	}

	[CompilerGenerated]
	public sealed class Class81
	{
		public Guid guid_0;

		public DateTime dateTime_0;

		internal bool method_0(DownloadRecordItem downloadRecordItem_0)
		{
			if (downloadRecordItem_0.FamilyId == guid_0)
			{
				return (dateTime_0 - DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null)).TotalHours <= 24.0;
			}
			return false;
		}
	}

	[CompilerGenerated]
	public sealed class Class82
	{
		public DateTime dateTime_0;

		internal bool method_0(DownloadRecordItem downloadRecordItem_0)
		{
			return DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null) < dateTime_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class83
	{
		public DateTime dateTime_0;

		internal bool method_0(DownloadRecordItem downloadRecordItem_0)
		{
			return DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null) < dateTime_0;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct60 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<DownloadQuotaInfo>> asyncTaskMethodBuilder_0;

		public AuthorizationState authorizationState_0;

		public FamilyLibraryService familyLibraryService_0;

		private int int_1;

		private int int_2;

		private TaskAwaiter<int> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Expected O, but got Unknown
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			Result<DownloadQuotaInfo> result2;
			try
			{
				TaskAwaiter<int> awaiter;
				if (num != 0)
				{
					int_1 = 0;
					if (authorizationState_0.ExpiryDate.HasValue)
					{
						if (authorizationState_0.ExpiryDate.Value >= new DateTime(2100, 1, 1))
						{
							int_1 = 99999;
						}
						else
						{
							int_1 = Math.Max(0, (authorizationState_0.ExpiryDate.Value - DateTime.UtcNow).Days);
						}
					}
					else if (authorizationState_0.HasValidLicense)
					{
						int_1 = 99999;
					}
					int_2 = familyLibraryService.method_2(int_1);
					awaiter = familyLibraryService.method_1().GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<int>);
					num = -1;
					int_0 = -1;
				}
				int result = awaiter.GetResult();
				result2 = Result<DownloadQuotaInfo>.Success(new DownloadQuotaInfo
				{
					DailyLimit = int_2,
					DownloadedToday = result,
					Remaining = Math.Max(0, int_2 - result),
					DaysRemaining = int_1,
					UserType = familyLibraryService.method_3(int_1)
				});
			}
			catch (Exception ex)
			{
				Logger.Error("[FamilyLibraryService] 查询配额失败", ex);
				result2 = Result<DownloadQuotaInfo>.Failure(ex.Message);
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
	public struct Struct61 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<FamilyCategory>>> asyncTaskMethodBuilder_0;

		public bool bool_0;

		public FamilyLibraryService familyLibraryService_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			Result<List<FamilyCategory>> result2;
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
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class57<bool>(bool_0)), Encoding.UTF8, "application/json");
					awaiter2 = familyLibraryService.isupabaseClient_0.HttpClient.PostAsync(familyLibraryService.isupabaseClient_0.BaseUrl + "/rest/v1/rpc/get_family_categories", content).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00c2;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00c2;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_018b;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_018b:
					result = awaiter.GetResult();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] 获取分类列表失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(10, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("获取分类列表失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
					result2 = Result<List<FamilyCategory>>.Failure(defaultInterpolatedStringHandler2.ToStringAndClear());
					goto end_IL_000f;
					IL_00c2:
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
						goto IL_018b;
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
				string result4 = awaiter.GetResult();
				List<FamilyCategory> list = new List<FamilyCategory>();
				try
				{
					IEnumerator<JToken> enumerator = JArray.Parse(result4).GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							JToken val = enumerator.Current[(object)"categories"];
							if (val != null)
							{
								FamilyCategory val2 = val.ToObject<FamilyCategory>();
								if (val2 != null)
								{
									list.Add(val2);
								}
							}
						}
					}
					finally
					{
						if (num < 0)
						{
							enumerator?.Dispose();
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("[FamilyLibraryService] 解析表格格式失败，尝试直接解析: " + ex.Message);
					list = JsonConvert.DeserializeObject<List<FamilyCategory>>(result4) ?? new List<FamilyCategory>();
				}
				result2 = Result<List<FamilyCategory>>.Success(list);
				end_IL_000f:;
			}
			catch (Exception ex2)
			{
				Logger.Error("[FamilyLibraryService] 获取分类列表异常", ex2);
				result2 = Result<List<FamilyCategory>>.Failure(ex2.Message);
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
	public struct Struct62 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<FamilyDownloadHistoryItem>>> asyncTaskMethodBuilder_0;

		public FamilyLibraryService familyLibraryService_0;

		public int int_1;

		public int int_2;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			Result<List<FamilyDownloadHistoryItem>> result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					Guid? currentUserId = familyLibraryService.CurrentUserId;
					if (currentUserId.HasValue)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] 获取下载历史: 用户=");
						defaultInterpolatedStringHandler.AppendFormatted(currentUserId);
						defaultInterpolatedStringHandler.AppendLiteral(", 页码=");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
						StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class58<Guid?, int, int>(currentUserId, int_1, int_2)), Encoding.UTF8, "application/json");
						awaiter2 = familyLibraryService.isupabaseClient_0.HttpClient.PostAsync(familyLibraryService.isupabaseClient_0.BaseUrl + "/rest/v1/rpc/get_user_download_history", content).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_013d;
					}
					result = Result<List<FamilyDownloadHistoryItem>>.Failure("用户未登录");
					goto end_IL_000f;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_013d;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0206;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0206:
					result2 = awaiter.GetResult();
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(35, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("[FamilyLibraryService] 获取下载历史失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendFormatted(result2);
					Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
					result = Result<List<FamilyDownloadHistoryItem>>.Success(new List<FamilyDownloadHistoryItem>());
					goto end_IL_000f;
					IL_013d:
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
						goto IL_0206;
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
				result = Result<List<FamilyDownloadHistoryItem>>.Success(JsonConvert.DeserializeObject<List<FamilyDownloadHistoryItem>>(awaiter.GetResult()) ?? new List<FamilyDownloadHistoryItem>());
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[FamilyLibraryService] 获取下载历史异常: " + ex.Message);
				result = Result<List<FamilyDownloadHistoryItem>>.Success(new List<FamilyDownloadHistoryItem>());
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
	public struct Struct63 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<DownloadQuotaInfo>> asyncTaskMethodBuilder_0;

		public FamilyLibraryService familyLibraryService_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Expected O, but got Unknown
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Expected O, but got Unknown
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			Result<DownloadQuotaInfo> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter2;
				TaskAwaiter<string> awaiter;
				string result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3;
				HttpResponseMessage result3;
				switch (num)
				{
				default:
				{
					Guid? currentUserId = familyLibraryService.CurrentUserId;
					string currentDeviceId = familyLibraryService.CurrentDeviceId;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] 获取下载配额: 用户=");
					defaultInterpolatedStringHandler.AppendFormatted(currentUserId);
					defaultInterpolatedStringHandler.AppendLiteral(", 设备=");
					defaultInterpolatedStringHandler.AppendFormatted(currentDeviceId);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[FamilyLibraryService] 当前时间(UTC): ");
					defaultInterpolatedStringHandler2.AppendFormatted(DateTime.UtcNow, "yyyy-MM-dd HH:mm:ss");
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class46<Guid?, string>(currentUserId, currentDeviceId)), Encoding.UTF8, "application/json");
					awaiter2 = familyLibraryService.isupabaseClient_0.HttpClient.PostAsync(familyLibraryService.isupabaseClient_0.BaseUrl + "/rest/v1/rpc/get_download_quota", content).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_0156;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0156;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_021f;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_021f:
					result = awaiter.GetResult();
					defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(35, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("[FamilyLibraryService] 获取下载配额失败: ");
					defaultInterpolatedStringHandler3.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler3.AppendLiteral(", ");
					defaultInterpolatedStringHandler3.AppendFormatted(result);
					Logger.Warning(defaultInterpolatedStringHandler3.ToStringAndClear());
					result2 = Result<DownloadQuotaInfo>.Success(new DownloadQuotaInfo
					{
						DailyLimit = 2,
						DownloadedToday = 0,
						Remaining = 2,
						DaysRemaining = 0,
						UserType = "trial"
					});
					goto end_IL_000f;
					IL_0156:
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
						goto IL_021f;
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
				string result4 = awaiter.GetResult();
				Logger.Debug("[FamilyLibraryService] 配额响应: " + result4);
				List<DownloadQuotaInfo> list = JsonConvert.DeserializeObject<List<DownloadQuotaInfo>>(result4);
				object obj;
				if (list == null)
				{
					obj = null;
				}
				else
				{
					obj = list[0];
					if (obj != null)
					{
						goto IL_033c;
					}
				}
				obj = (object)new DownloadQuotaInfo
				{
					DailyLimit = 2,
					DownloadedToday = 0,
					Remaining = 2,
					DaysRemaining = 0,
					UserType = "trial"
				};
				goto IL_033c;
				IL_033c:
				DownloadQuotaInfo val = (DownloadQuotaInfo)obj;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(57, 5);
				defaultInterpolatedStringHandler4.AppendLiteral("[FamilyLibraryService] 配额信息: 限额=");
				defaultInterpolatedStringHandler4.AppendFormatted(val.DailyLimit);
				defaultInterpolatedStringHandler4.AppendLiteral(", 已下载=");
				defaultInterpolatedStringHandler4.AppendFormatted(val.DownloadedToday);
				defaultInterpolatedStringHandler4.AppendLiteral(", 剩余=");
				defaultInterpolatedStringHandler4.AppendFormatted(val.Remaining);
				defaultInterpolatedStringHandler4.AppendLiteral(", 剩余天数=");
				defaultInterpolatedStringHandler4.AppendFormatted(val.DaysRemaining);
				defaultInterpolatedStringHandler4.AppendLiteral(", 用户类型=");
				defaultInterpolatedStringHandler4.AppendFormatted(val.UserType);
				Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
				result2 = Result<DownloadQuotaInfo>.Success(val);
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[FamilyLibraryService] 获取下载配额异常: " + ex.Message);
				result2 = Result<DownloadQuotaInfo>.Success(new DownloadQuotaInfo
				{
					DailyLimit = 2,
					DownloadedToday = 0,
					Remaining = 2,
					DaysRemaining = 0,
					UserType = "trial"
				});
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
	public struct Struct64 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<FamilyLibraryItem>>> asyncTaskMethodBuilder_0;

		public Guid? nullable_0;

		public string string_0;

		public string[] string_1;

		public string string_2;

		public string string_3;

		public bool? nullable_1;

		public int int_1;

		public int int_2;

		public bool bool_0;

		public FamilyLibraryService familyLibraryService_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			Result<List<FamilyLibraryItem>> result2;
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
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class56<Guid?, string, string[], string, string, bool?, int, int, bool>(nullable_0, string_0, string_1, string_2, string_3, nullable_1, int_1, int_2, bool_0)), Encoding.UTF8, "application/json");
					awaiter2 = familyLibraryService.isupabaseClient_0.HttpClient.PostAsync(familyLibraryService.isupabaseClient_0.BaseUrl + "/rest/v1/rpc/get_family_library_items", content).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00f2;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00f2;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01bb;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01bb:
					result = awaiter.GetResult();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] 获取族列表失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(9, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("获取族列表失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
					result2 = Result<List<FamilyLibraryItem>>.Failure(defaultInterpolatedStringHandler2.ToStringAndClear());
					goto end_IL_000f;
					IL_00f2:
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
						goto IL_01bb;
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
				string result4 = awaiter.GetResult();
				List<FamilyLibraryItem> list = new List<FamilyLibraryItem>();
				try
				{
					IEnumerator<JToken> enumerator = JArray.Parse(result4).GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							JToken val = enumerator.Current[(object)"items"];
							if (val != null)
							{
								FamilyLibraryItem val2 = val.ToObject<FamilyLibraryItem>();
								if (val2 != null)
								{
									list.Add(val2);
								}
							}
						}
					}
					finally
					{
						if (num < 0)
						{
							enumerator?.Dispose();
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("[FamilyLibraryService] 解析表格格式失败，尝试直接解析: " + ex.Message);
					list = JsonConvert.DeserializeObject<List<FamilyLibraryItem>>(result4) ?? new List<FamilyLibraryItem>();
				}
				result2 = Result<List<FamilyLibraryItem>>.Success(list);
				end_IL_000f:;
			}
			catch (Exception ex2)
			{
				Logger.Error("[FamilyLibraryService] 获取族列表异常", ex2);
				result2 = Result<List<FamilyLibraryItem>>.Failure(ex2.Message);
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
	public struct Struct65 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<FamilyLibraryDetail>> asyncTaskMethodBuilder_0;

		public Guid guid_0;

		public FamilyLibraryService familyLibraryService_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Expected O, but got Unknown
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Expected O, but got Unknown
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			Result<FamilyLibraryDetail> result2;
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
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class60<Guid>(guid_0)), Encoding.UTF8, "application/json");
					awaiter2 = familyLibraryService.isupabaseClient_0.HttpClient.PostAsync(familyLibraryService.isupabaseClient_0.BaseUrl + "/rest/v1/rpc/get_family_details", content).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00c2;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00c2;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_018b;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_018b:
					result = awaiter.GetResult();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] 获取族详情失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(9, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("获取族详情失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
					result2 = Result<FamilyLibraryDetail>.Failure(defaultInterpolatedStringHandler2.ToStringAndClear());
					goto end_IL_000f;
					IL_00c2:
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
						goto IL_018b;
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
				JObject val = JObject.Parse(awaiter.GetResult());
				JToken obj = val["success"];
				object obj3;
				if (obj == null || !Extensions.Value<bool>((IEnumerable<JToken>)obj))
				{
					JToken obj2 = val["error"];
					if (obj2 == null)
					{
						obj3 = null;
					}
					else
					{
						obj3 = Extensions.Value<string>((IEnumerable<JToken>)obj2);
						if (obj3 != null)
						{
							goto IL_029b;
						}
					}
					obj3 = "未知错误";
					goto IL_029b;
				}
				JToken obj4 = val["family"];
				FamilyLibraryItem val2 = ((obj4 != null) ? obj4.ToObject<FamilyLibraryItem>() : null);
				object obj6;
				if (val2 != null)
				{
					JToken obj5 = val["parameters"];
					if (obj5 == null)
					{
						obj6 = null;
					}
					else
					{
						obj6 = obj5.ToObject<List<FamilyParameter>>();
						if (obj6 != null)
						{
							goto IL_0304;
						}
					}
					obj6 = new List<FamilyParameter>();
					goto IL_0304;
				}
				result2 = Result<FamilyLibraryDetail>.Failure("族信息为空");
				goto end_IL_000f;
				IL_029b:
				result2 = Result<FamilyLibraryDetail>.Failure((string)obj3);
				goto end_IL_000f;
				IL_0304:
				List<FamilyParameter> list = (List<FamilyParameter>)obj6;
				List<FamilyTypeParameterGroup> list2 = new List<FamilyTypeParameterGroup>();
				JToken val3 = val["types"];
				if (val3 != null)
				{
					List<FamilyTypeParameterGroup> list3 = val3.ToObject<List<FamilyTypeParameterGroup>>();
					if (list3 != null)
					{
						list2 = list3;
					}
				}
				if (list2.Count == 0 && list.Count > 0)
				{
					List<FamilyTypeParameterGroup> list4 = ((!list.Any((FamilyParameter familyParameter_0) => !string.IsNullOrEmpty(familyParameter_0.TypeName))) ? new List<FamilyTypeParameterGroup>
					{
						new FamilyTypeParameterGroup
						{
							TypeName = "默认",
							Parameters = (from familyParameter_0 in list
								orderby familyParameter_0.DisplayOrder, familyParameter_0.ParameterName
								select familyParameter_0).ToList()
						}
					} : (from familyTypeParameterGroup_0 in (from familyParameter_0 in list
							group familyParameter_0 by familyParameter_0.TypeName ?? "(未分类)").Select((Func<IGrouping<string, FamilyParameter>, FamilyTypeParameterGroup>)delegate(IGrouping<string, FamilyParameter> igrouping_0)
						{
							//IL_0000: Unknown result type (might be due to invalid IL or missing references)
							//IL_0005: Unknown result type (might be due to invalid IL or missing references)
							//IL_0011: Unknown result type (might be due to invalid IL or missing references)
							//IL_0066: Expected O, but got Unknown
							return new FamilyTypeParameterGroup
							{
								TypeName = igrouping_0.Key,
								Parameters = (from familyParameter_0 in igrouping_0
									orderby familyParameter_0.DisplayOrder, familyParameter_0.ParameterName
									select familyParameter_0).ToList()
							};
						})
						orderby familyTypeParameterGroup_0.TypeName
						select familyTypeParameterGroup_0).ToList());
					list2 = list4;
				}
				JToken obj7 = val["param_stats"];
				object obj8;
				if (obj7 == null)
				{
					obj8 = null;
				}
				else
				{
					obj8 = obj7.ToObject<FamilyParameterStats>();
					if (obj8 != null)
					{
						goto IL_048e;
					}
				}
				obj8 = (object)new FamilyParameterStats();
				goto IL_048e;
				IL_048e:
				FamilyParameterStats paramStats = (FamilyParameterStats)obj8;
				result2 = Result<FamilyLibraryDetail>.Success(new FamilyLibraryDetail
				{
					Family = val2,
					Parameters = list,
					TypeGroups = list2,
					ParamStats = paramStats
				});
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[FamilyLibraryService] 获取族详情异常", ex);
				result2 = Result<FamilyLibraryDetail>.Failure(ex.Message);
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
	public struct Struct66 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<FamilyLibraryItem>>> asyncTaskMethodBuilder_0;

		public int int_1;

		public FamilyLibraryService familyLibraryService_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			Result<List<FamilyLibraryItem>> result2;
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
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] 获取热门族: 数量=");
					defaultInterpolatedStringHandler.AppendFormatted(int_1);
					Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class59<int>(int_1)), Encoding.UTF8, "application/json");
					awaiter2 = familyLibraryService.isupabaseClient_0.HttpClient.PostAsync(familyLibraryService.isupabaseClient_0.BaseUrl + "/rest/v1/rpc/get_popular_families", content).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00f6;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00f6;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01bf;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01bf:
					result = awaiter.GetResult();
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("[FamilyLibraryService] 获取热门族失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendFormatted(result);
					Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
					result2 = Result<List<FamilyLibraryItem>>.Success(new List<FamilyLibraryItem>());
					goto end_IL_000f;
					IL_00f6:
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
						goto IL_01bf;
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
				string result4 = awaiter.GetResult();
				List<FamilyLibraryItem> list = new List<FamilyLibraryItem>();
				try
				{
					IEnumerator<JToken> enumerator = JArray.Parse(result4).GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							JToken val = enumerator.Current[(object)"families"];
							if (val != null)
							{
								FamilyLibraryItem val2 = val.ToObject<FamilyLibraryItem>();
								if (val2 != null)
								{
									list.Add(val2);
								}
							}
						}
					}
					finally
					{
						if (num < 0)
						{
							enumerator?.Dispose();
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("[FamilyLibraryService] 解析表格格式失败，尝试直接解析: " + ex.Message);
					list = JsonConvert.DeserializeObject<List<FamilyLibraryItem>>(result4) ?? new List<FamilyLibraryItem>();
				}
				result2 = Result<List<FamilyLibraryItem>>.Success(list);
				end_IL_000f:;
			}
			catch (Exception ex2)
			{
				Logger.Warning("[FamilyLibraryService] 获取热门族异常: " + ex2.Message);
				result2 = Result<List<FamilyLibraryItem>>.Success(new List<FamilyLibraryItem>());
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
	public struct Struct67 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<RevitCategoryItem>>> asyncTaskMethodBuilder_0;

		public FamilyLibraryService familyLibraryService_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			Result<List<RevitCategoryItem>> result2;
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
					awaiter2 = familyLibraryService.isupabaseClient_0.HttpClient.GetAsync(familyLibraryService.isupabaseClient_0.BaseUrl + "/rest/v1/revit_category_stats?select=revit_category_name,family_count&order=revit_category_name.asc").GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_009c;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_009c;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_0165;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0165:
					result = awaiter.GetResult();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] 获取 Revit 类别失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("获取 Revit 类别失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(httpResponseMessage_0.StatusCode);
					result2 = Result<List<RevitCategoryItem>>.Failure(defaultInterpolatedStringHandler2.ToStringAndClear());
					goto end_IL_000f;
					IL_009c:
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
						goto IL_0165;
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
				List<RevitCategoryItem> list = JsonConvert.DeserializeObject<List<RevitCategoryItem>>(awaiter.GetResult());
				result2 = ((list != null) ? Result<List<RevitCategoryItem>>.Success(list) : Result<List<RevitCategoryItem>>.Success(new List<RevitCategoryItem>()));
				end_IL_000f:;
			}
			catch (HttpRequestException ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(41, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("[FamilyLibraryService] HTTP 请求异常: ");
				defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
				defaultInterpolatedStringHandler3.AppendLiteral(" (内部: ");
				defaultInterpolatedStringHandler3.AppendFormatted(ex.InnerException?.Message);
				defaultInterpolatedStringHandler3.AppendLiteral(")");
				Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
				result2 = Result<List<RevitCategoryItem>>.Failure("HTTP 请求异常: " + ex.Message);
			}
			catch (Exception ex2)
			{
				Logger.Error("[FamilyLibraryService] 获取 Revit 类别异常", ex2);
				result2 = Result<List<RevitCategoryItem>>.Failure(ex2.Message);
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
	public struct Struct68 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public string string_0;

		public FamilyLibraryService familyLibraryService_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			string result2;
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
					string stringToEscape = new Uri(string_0).AbsolutePath.TrimStart('/');
					string requestUri = "https://astools.tech/api/family/download-url?object_key=" + Uri.EscapeDataString(stringToEscape);
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri)
					{
						Headers = { 
						{
							"User-Agent",
							"RevitAi.Revit/1.0"
						} }
					};
					awaiter2 = familyLibraryService.HttpClient.SendAsync(request).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_00dc;
				}
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00dc;
				case 1:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01a5;
				case 2:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01a5:
					result = awaiter.GetResult();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] 获取签名 URL 失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(result);
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					result2 = string.Empty;
					goto end_IL_000f;
					IL_00dc:
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
						goto IL_01a5;
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
				string result4 = awaiter.GetResult();
				JObject val = JObject.Parse(result4);
				JToken obj = val["success"];
				object obj3;
				if (obj != null && Extensions.Value<bool>((IEnumerable<JToken>)obj))
				{
					JToken obj2 = val["downloadUrl"];
					if (obj2 == null)
					{
						obj3 = null;
					}
					else
					{
						obj3 = Extensions.Value<string>((IEnumerable<JToken>)obj2);
						if (obj3 != null)
						{
							goto IL_0280;
						}
					}
					obj3 = string.Empty;
					goto IL_0280;
				}
				Logger.Warning("[FamilyLibraryService] 签名 URL 返回失败: " + result4);
				result2 = string.Empty;
				goto end_IL_000f;
				IL_0280:
				result2 = (string)obj3;
				end_IL_000f:;
			}
			catch (HttpRequestException ex)
			{
				Logger.Warning("[FamilyLibraryService] 网络请求异常: " + ex.Message);
				result2 = string.Empty;
			}
			catch (TaskCanceledException)
			{
				Logger.Warning("[FamilyLibraryService] 请求超时");
				result2 = string.Empty;
			}
			catch (Exception ex3)
			{
				Logger.Warning("[FamilyLibraryService] 获取签名 URL 异常: " + ex3.Message);
				result2 = string.Empty;
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
	public struct Struct69 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<int> asyncTaskMethodBuilder_0;

		public FamilyLibraryService familyLibraryService_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			int result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0167;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01bc;
				}
				Guid? currentUserId = familyLibraryService.CurrentUserId;
				string currentDeviceId = familyLibraryService.CurrentDeviceId;
				if (!string.IsNullOrEmpty(currentDeviceId))
				{
					string requestUri = familyLibraryService.isupabaseClient_0.BaseUrl + "/rest/v1/rpc/get_recent_family_downloads";
					Dictionary<string, object> dictionary = new Dictionary<string, object>
					{
						{
							"p_device_id",
							currentDeviceId
						},
						{
							"p_hours_ago",
							24
						}
					};
					if (currentUserId.HasValue)
					{
						dictionary["p_user_id"] = currentUserId.Value.ToString();
					}
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)dictionary), Encoding.UTF8, "application/json");
					awaiter = familyLibraryService.isupabaseClient_0.HttpClient.PostAsync(requestUri, content).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0167;
				}
				Logger.Warning("[FamilyLibraryService] 无 device_id，无法查询下载记录");
				result = 0;
				goto end_IL_000f;
				IL_0167:
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
				goto IL_01bc;
				IL_01bc:
				string result3 = awaiter2.GetResult();
				if (httpResponseMessage_0.IsSuccessStatusCode)
				{
					result = ((JContainer)JArray.Parse(result3)).Count;
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] RPC 查询失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(result3);
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					result = 0;
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[FamilyLibraryService] 获取最近24h下载数量失败", ex);
				result = 0;
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
	public struct Struct70 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public Guid guid_0;

		public FamilyLibraryService familyLibraryService_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Expected O, but got Unknown
			FamilyLibraryService familyLibraryService = this.familyLibraryService_0;
			Guid guid_0 = this.guid_0;
			FamilyLibraryService familyLibraryService_0 = this.familyLibraryService_0;
			try
			{
				if (familyLibraryService.downloadRecord_0 == null)
				{
					familyLibraryService.method_5();
				}
				DateTime dateTime = DateTime.ParseExact(familyLibraryService.downloadRecord_0.WindowStartTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null);
				DateTime dateTime_0 = DateTime.UtcNow;
				if (dateTime_0 > dateTime.AddHours(24.0))
				{
					DateTime dateTime2 = dateTime_0.AddHours(-24.0);
					familyLibraryService.downloadRecord_0 = new DownloadRecord
					{
						WindowStartTime = dateTime2.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
						Downloads = new List<DownloadRecordItem>()
					};
				}
				if (!familyLibraryService.downloadRecord_0.Downloads.Any((DownloadRecordItem downloadRecordItem_0) => downloadRecordItem_0.FamilyId == guid_0 && (dateTime_0 - DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null)).TotalHours <= 24.0))
				{
					familyLibraryService.downloadRecord_0.Downloads.Add(new DownloadRecordItem
					{
						FamilyId = guid_0,
						DownloadTime = dateTime_0.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
					});
					DateTime dateTime_1 = dateTime_0.AddHours(-24.0);
					familyLibraryService.downloadRecord_0.Downloads.RemoveAll((DownloadRecordItem downloadRecordItem_0) => DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null) < dateTime_1);
					familyLibraryService.method_6();
					Task.Run([AsyncStateMachine(typeof(Class78.Struct58))] () =>
					{
						Class78.Struct58 stateMachine = default(Class78.Struct58);
						stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
						Class78 class78_ = default;
						stateMachine.class78_0 = class78_;
						stateMachine.int_0 = -1;
						stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
						return stateMachine.asyncTaskMethodBuilder_0.Task;
					});
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[FamilyLibraryService] 记录下载失败: " + ex.Message);
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
	public struct Struct71 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public FamilyLibraryService familyLibraryService_0;

		public Guid guid_0;

		private HttpResponseMessage httpResponseMessage_0;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_0125;
				}
				TaskAwaiter<string> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_017a;
				}
				Guid? currentUserId = familyLibraryService.CurrentUserId;
				string currentDeviceId = familyLibraryService.CurrentDeviceId;
				if (!string.IsNullOrEmpty(currentDeviceId))
				{
					StringContent content = new StringContent(JsonConvert.SerializeObject((object)new Class61<Guid, object, string>(guid_0, currentUserId.HasValue ? ((object)currentUserId.Value) : null, currentDeviceId)), Encoding.UTF8, "application/json");
					string requestUri = familyLibraryService.isupabaseClient_0.BaseUrl + "/rest/v1/rpc/record_family_download_client";
					awaiter = familyLibraryService.isupabaseClient_0.HttpClient.PostAsync(requestUri, content).GetAwaiter();
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
				Logger.Warning("[FamilyLibraryService] 无法记录到数据库：设备ID缺失");
				goto end_IL_000f;
				IL_0125:
				HttpResponseMessage result = awaiter.GetResult();
				httpResponseMessage_0 = result;
				awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter2;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
					return;
				}
				goto IL_017a;
				IL_017a:
				string result2 = awaiter2.GetResult();
				if (httpResponseMessage_0.IsSuccessStatusCode)
				{
					JObject val = JObject.Parse(result2);
					JToken obj = val["success"];
					if (obj == null || !Extensions.Value<bool>((IEnumerable<JToken>)obj))
					{
						JToken obj2 = val["error"];
						string text = ((obj2 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj2) : null);
						Logger.Warning("[FamilyLibraryService] 同步下载记录失败: " + text);
					}
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyLibraryService] 同步下载记录失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(result2);
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				httpResponseMessage_0 = null;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[FamilyLibraryService] 记录到数据库异常: " + ex.Message);
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
	public struct Struct72 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter);
				int num = -1;
				int_0 = -1;
			}
			awaiter.GetResult();
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
	public struct Struct73 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<FamilyDownloadInfo>> asyncTaskMethodBuilder_0;

		public FamilyLibraryService familyLibraryService_0;

		public Guid guid_0;

		private HttpResponseMessage httpResponseMessage_0;

		private JToken jtoken_0;

		private bool bool_0;

		private string string_0;

		private DownloadQuotaInfo downloadQuotaInfo_0;

		private bool bool_1;

		private string string_1;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		private TaskAwaiter<Result<DownloadQuotaInfo>> taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Expected O, but got Unknown
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Expected O, but got Unknown
			int num = int_0;
			FamilyLibraryService familyLibraryService = familyLibraryService_0;
			Result<FamilyDownloadInfo> result2;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter3;
				TaskAwaiter<Result<DownloadQuotaInfo>> awaiter;
				TaskAwaiter<string> awaiter2;
				string text;
				object obj;
				AuthorizationState authorizationState;
				HttpResponseMessage result;
				JArray val;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2;
				Result<DownloadQuotaInfo> result3;
				string result4;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5;
				switch (num)
				{
				default:
				{
					HttpClient httpClient = familyLibraryService.isupabaseClient_0.HttpClient;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(106, 2);
					defaultInterpolatedStringHandler.AppendFormatted(familyLibraryService.isupabaseClient_0.BaseUrl);
					defaultInterpolatedStringHandler.AppendLiteral("/rest/v1/family_library_items?id=eq.");
					defaultInterpolatedStringHandler.AppendFormatted(guid_0);
					defaultInterpolatedStringHandler.AppendLiteral("&select=id,display_name,file_url,file_size_bytes,thumbnail_url,is_free");
					awaiter3 = httpClient.GetAsync(defaultInterpolatedStringHandler.ToStringAndClear()).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter3;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
						return;
					}
					goto IL_00dc;
				}
				case 0:
					awaiter3 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
					goto IL_00dc;
				case 1:
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_01a5;
				case 2:
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
					goto IL_025d;
				case 3:
					awaiter = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<Result<DownloadQuotaInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_03ba;
				case 4:
					{
						try
						{
							if (num != 4)
							{
								awaiter2 = familyLibraryService.method_0(string_0).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 4;
									int_0 = 4;
									taskAwaiter_1 = awaiter2;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
							}
							else
							{
								awaiter2 = taskAwaiter_1;
								taskAwaiter_1 = default(TaskAwaiter<string>);
								num = -1;
								int_0 = -1;
							}
							text = awaiter2.GetResult();
							if (string.IsNullOrEmpty(text))
							{
								Logger.Warning("[FamilyLibraryService] 获取签名 URL 失败，使用原 URL");
								text = string_0;
							}
						}
						catch (Exception ex)
						{
							Logger.Warning("[FamilyLibraryService] 获取签名 URL 异常: " + ex.Message + "，使用原 URL");
							text = string_0;
						}
						break;
					}
					IL_02da:
					string_0 = (string)obj;
					authorizationState = familyLibraryService.iauthManager_0.GetAuthorizationState();
					awaiter = familyLibraryService.CalculateDownloadQuotaLocallyAsync(authorizationState).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 3;
						int_0 = 3;
						taskAwaiter_2 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_03ba;
					IL_00dc:
					result = awaiter3.GetResult();
					httpResponseMessage_0 = result;
					if (!httpResponseMessage_0.IsSuccessStatusCode)
					{
						awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_01a5;
					}
					awaiter2 = httpResponseMessage_0.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_025d;
					IL_025d:
					val = JArray.Parse(awaiter2.GetResult());
					if (val != null && ((JContainer)val).Count != 0)
					{
						jtoken_0 = val[0];
						JToken obj2 = jtoken_0[(object)"is_free"];
						bool_0 = obj2 != null && Extensions.Value<bool>((IEnumerable<JToken>)obj2);
						JToken obj3 = jtoken_0[(object)"file_url"];
						if (obj3 == null)
						{
							obj = null;
						}
						else
						{
							obj = Extensions.Value<string>((IEnumerable<JToken>)obj3);
							if (obj != null)
							{
								goto IL_02da;
							}
						}
						obj = string.Empty;
						goto IL_02da;
					}
					defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(29, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[FamilyLibraryService] 族不存在: ");
					defaultInterpolatedStringHandler2.AppendFormatted(guid_0);
					Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
					result2 = Result<FamilyDownloadInfo>.Success(new FamilyDownloadInfo
					{
						Success = false,
						CanDownload = false,
						Error = "Family not found",
						Message = "族不存在或已下架"
					});
					goto end_IL_000f;
					IL_03ba:
					result3 = awaiter.GetResult();
					if (result3.IsSuccess)
					{
						downloadQuotaInfo_0 = result3.Value;
						bool_1 = false;
						string_1 = string.Empty;
						text = string_0;
						if (bool_0)
						{
							bool_1 = true;
							string_1 = "免费族，可下载";
						}
						else
						{
							DownloadQuotaInfo obj4 = downloadQuotaInfo_0;
							if (((obj4 != null && obj4.Remaining != 0) ? 1 : 0) > (false ? 1 : 0))
							{
								bool_1 = true;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
								defaultInterpolatedStringHandler3.AppendLiteral("剩余 ");
								DownloadQuotaInfo obj5 = downloadQuotaInfo_0;
								defaultInterpolatedStringHandler3.AppendFormatted((obj5 != null) ? obj5.Remaining : 0);
								defaultInterpolatedStringHandler3.AppendLiteral(" 次下载机会");
								string_1 = defaultInterpolatedStringHandler3.ToStringAndClear();
							}
							else
							{
								bool_1 = false;
								string_1 = "今日下载次数已用完";
							}
						}
						if (!bool_1 || string.IsNullOrEmpty(string_0))
						{
							break;
						}
						goto case 4;
					}
					result2 = Result<FamilyDownloadInfo>.Failure(result3.Error ?? "计算配额失败");
					goto end_IL_000f;
					IL_01a5:
					result4 = awaiter2.GetResult();
					defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(34, 2);
					defaultInterpolatedStringHandler4.AppendLiteral("[FamilyLibraryService] 获取族信息失败: ");
					defaultInterpolatedStringHandler4.AppendFormatted(httpResponseMessage_0.StatusCode);
					defaultInterpolatedStringHandler4.AppendLiteral(", ");
					defaultInterpolatedStringHandler4.AppendFormatted(result4);
					Logger.Error(defaultInterpolatedStringHandler4.ToStringAndClear());
					defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(9, 1);
					defaultInterpolatedStringHandler5.AppendLiteral("获取族信息失败: ");
					defaultInterpolatedStringHandler5.AppendFormatted(httpResponseMessage_0.StatusCode);
					result2 = Result<FamilyDownloadInfo>.Failure(defaultInterpolatedStringHandler5.ToStringAndClear());
					goto end_IL_000f;
				}
				FamilyDownloadInfo val2 = new FamilyDownloadInfo
				{
					Success = true,
					CanDownload = bool_1,
					IsFree = bool_0
				};
				JToken obj6 = jtoken_0[(object)"id"];
				val2.FamilyId = ((obj6 != null) ? obj6.ToObject<Guid>() : guid_0);
				JToken obj7 = jtoken_0[(object)"display_name"];
				object obj8;
				if (obj7 == null)
				{
					obj8 = null;
				}
				else
				{
					obj8 = Extensions.Value<string>((IEnumerable<JToken>)obj7);
					if (obj8 != null)
					{
						goto IL_0617;
					}
				}
				obj8 = string.Empty;
				goto IL_0617;
				IL_0617:
				val2.DisplayName = (string)obj8;
				val2.FileUrl = text;
				JToken obj9 = jtoken_0[(object)"file_size_bytes"];
				val2.FileSize = ((obj9 != null) ? obj9.ToObject<long>() : 0L);
				JToken obj10 = jtoken_0[(object)"thumbnail_url"];
				val2.ThumbnailUrl = ((obj10 != null) ? Extensions.Value<string>((IEnumerable<JToken>)obj10) : null);
				val2.QuotaInfo = downloadQuotaInfo_0;
				val2.Message = string_1;
				result2 = Result<FamilyDownloadInfo>.Success(val2);
				end_IL_000f:;
			}
			catch (Exception ex2)
			{
				Logger.Error("[FamilyLibraryService] 请求下载异常", ex2);
				result2 = Result<FamilyDownloadInfo>.Failure(ex2.Message);
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

	private readonly ISupabaseClient isupabaseClient_0;

	private readonly IAuthManager iauthManager_0;

	private readonly string string_0;

	private DownloadRecord? downloadRecord_0;

	private Guid? CurrentUserId
	{
		get
		{
			IUserIdentity currentUser = iauthManager_0.CurrentUser;
			if (currentUser == null)
			{
				return null;
			}
			return currentUser.UserId;
		}
	}

	private string? CurrentDeviceId
	{
		get
		{
			IDeviceInfo currentDevice = iauthManager_0.CurrentDevice;
			if (currentDevice == null)
			{
				return null;
			}
			return currentDevice.DeviceId;
		}
	}

	private HttpClient HttpClient => isupabaseClient_0.HttpClient;

	public FamilyLibraryService(ISupabaseClient supabaseClient, IAuthManager authManager)
	{
		isupabaseClient_0 = supabaseClient ?? throw new ArgumentNullException("supabaseClient");
		iauthManager_0 = authManager ?? throw new ArgumentNullException("authManager");
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "FamilyLibrary");
		string_0 = Path.Combine(path, "download_records.json");
		method_5();
	}

	[AsyncStateMachine(typeof(Struct64))]
	public Task<Result<List<FamilyLibraryItem>>> GetFamiliesAsync(Guid? categoryId = null, string? searchText = null, string[]? tags = null, string? revitVersion = null, string? revitCategoryName = null, bool? isFree = null, int page = 1, int pageSize = 50, bool includeNonPublic = false)
	{
		Struct64 stateMachine = default(Struct64);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<FamilyLibraryItem>>>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.nullable_0 = categoryId;
		stateMachine.string_0 = searchText;
		stateMachine.string_1 = tags;
		stateMachine.string_2 = revitVersion;
		stateMachine.string_3 = revitCategoryName;
		stateMachine.nullable_1 = isFree;
		stateMachine.int_1 = page;
		stateMachine.int_2 = pageSize;
		stateMachine.bool_0 = includeNonPublic;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct61))]
	public Task<Result<List<FamilyCategory>>> GetCategoriesAsync(bool includeInactive = false)
	{
		Struct61 stateMachine = default(Struct61);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<FamilyCategory>>>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.bool_0 = includeInactive;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct67))]
	public Task<Result<List<RevitCategoryItem>>> GetRevitCategoriesAsync()
	{
		Struct67 stateMachine = default(Struct67);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<RevitCategoryItem>>>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct63))]
	public Task<Result<DownloadQuotaInfo>> GetDownloadQuotaAsync()
	{
		Struct63 stateMachine = default(Struct63);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<DownloadQuotaInfo>>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct73))]
	public Task<Result<FamilyDownloadInfo>> RequestDownloadAsync(Guid familyId)
	{
		Struct73 stateMachine = default(Struct73);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<FamilyDownloadInfo>>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.guid_0 = familyId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct68))]
	private Task<string> method_0(string string_1)
	{
		Struct68 stateMachine = default(Struct68);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.string_0 = string_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct72))]
	public Task RecordLocalCacheUsageAsync(Guid familyId)
	{
		Struct72 stateMachine = default(Struct72);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct62))]
	public Task<Result<List<FamilyDownloadHistoryItem>>> GetDownloadHistoryAsync(int page = 1, int pageSize = 20)
	{
		Struct62 stateMachine = default(Struct62);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<FamilyDownloadHistoryItem>>>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.int_1 = page;
		stateMachine.int_2 = pageSize;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct66))]
	public Task<Result<List<FamilyLibraryItem>>> GetPopularFamiliesAsync(int limit = 20)
	{
		Struct66 stateMachine = default(Struct66);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<FamilyLibraryItem>>>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.int_1 = limit;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct65))]
	public Task<Result<FamilyLibraryDetail>> GetFamilyDetailsAsync(Guid familyId)
	{
		Struct65 stateMachine = default(Struct65);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<FamilyLibraryDetail>>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.guid_0 = familyId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct60))]
	public Task<Result<DownloadQuotaInfo>> CalculateDownloadQuotaLocallyAsync(AuthorizationState authState)
	{
		Struct60 stateMachine = default(Struct60);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<DownloadQuotaInfo>>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.authorizationState_0 = authState;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public Result<DownloadQuotaInfo> CalculateDownloadQuotaLocally(AuthorizationState authState)
	{
		try
		{
			return Task.Run(() => CalculateDownloadQuotaLocallyAsync(authState)).GetAwaiter().GetResult();
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibraryService] 本地计算配额失败", ex);
			return Result<DownloadQuotaInfo>.Failure(ex.Message);
		}
	}

	[AsyncStateMachine(typeof(Struct69))]
	private Task<int> method_1()
	{
		Struct69 stateMachine = default(Struct69);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<int>.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private int method_2(int int_0)
	{
		if (int_0 <= 0)
		{
			return 0;
		}
		if (int_0 >= 99999)
		{
			return 99999;
		}
		return Math.Max(3, (int)Math.Floor((double)int_0 / 10.0 * 3.0));
	}

	private string method_3(int int_0)
	{
		if (int_0 >= 99999)
		{
			return "permanent";
		}
		if (int_0 > 0)
		{
			return "paid";
		}
		return "trial";
	}

	[AsyncStateMachine(typeof(Struct70))]
	public Task RecordDownloadLocallyAsync(Guid familyId)
	{
		Struct70 stateMachine = default(Struct70);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.guid_0 = familyId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public void RecordDownloadLocally(Guid familyId)
	{
		Class80 @class = new Class80();
		@class.familyLibraryService_0 = this;
		@class.guid_0 = familyId;
		Task.Run([AsyncStateMachine(typeof(Class80.Struct59))] () =>
		{
			Class80.Struct59 stateMachine = default(Class80.Struct59);
			stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
			stateMachine.class80_0 = @class;
			stateMachine.int_0 = -1;
			stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
			return stateMachine.asyncTaskMethodBuilder_0.Task;
		});
	}

	[AsyncStateMachine(typeof(Struct71))]
	private Task method_4(Guid guid_0)
	{
		Struct71 stateMachine = default(Struct71);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.familyLibraryService_0 = this;
		stateMachine.guid_0 = guid_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public bool IsFamilyDownloadedToday(Guid familyId)
	{
		if (downloadRecord_0 == null)
		{
			method_5();
		}
		if (downloadRecord_0 == null)
		{
			return false;
		}
		DateTime dateTime_0 = DateTime.UtcNow;
		return downloadRecord_0.Downloads.Any((DownloadRecordItem downloadRecordItem_0) => downloadRecordItem_0.FamilyId == familyId && (dateTime_0 - DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null)).TotalHours <= 24.0);
	}

	private void method_5()
	{
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		try
		{
			if (File.Exists(string_0))
			{
				string text = File.ReadAllText(string_0);
				downloadRecord_0 = JsonConvert.DeserializeObject<DownloadRecord>(text);
				if (downloadRecord_0 != null)
				{
					DateTime utcNow = DateTime.UtcNow;
					DateTime dateTime_0 = utcNow.AddHours(-24.0);
					downloadRecord_0.Downloads.RemoveAll((DownloadRecordItem downloadRecordItem_0) => DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null) < dateTime_0);
					if (downloadRecord_0.Downloads.Count == 0)
					{
						downloadRecord_0.WindowStartTime = utcNow.AddHours(-24.0).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
					}
					method_6();
				}
			}
			if (downloadRecord_0 == null)
			{
				DateTime utcNow2 = DateTime.UtcNow;
				downloadRecord_0 = new DownloadRecord
				{
					WindowStartTime = utcNow2.AddHours(-24.0).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
					Downloads = new List<DownloadRecordItem>()
				};
				method_6();
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyLibraryService] 加载下载记录失败: " + ex.Message);
			DateTime utcNow3 = DateTime.UtcNow;
			downloadRecord_0 = new DownloadRecord
			{
				WindowStartTime = utcNow3.AddHours(-24.0).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
				Downloads = new List<DownloadRecordItem>()
			};
		}
	}

	private void method_6()
	{
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(string_0));
			string contents = JsonConvert.SerializeObject((object)downloadRecord_0, (Formatting)1);
			File.WriteAllText(string_0, contents);
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyLibraryService] 保存下载记录失败: " + ex.Message);
		}
	}

	private int method_7()
	{
		if (downloadRecord_0 == null)
		{
			method_5();
		}
		if (downloadRecord_0 == null)
		{
			return 0;
		}
		DateTime dateTime_0 = DateTime.UtcNow.AddHours(-24.0);
		downloadRecord_0.Downloads.RemoveAll((DownloadRecordItem downloadRecordItem_0) => DateTime.ParseExact(downloadRecordItem_0.DownloadTime, "yyyy-MM-ddTHH:mm:ss.fffZ", null) < dateTime_0);
		return downloadRecord_0.Downloads.Count;
	}
}
