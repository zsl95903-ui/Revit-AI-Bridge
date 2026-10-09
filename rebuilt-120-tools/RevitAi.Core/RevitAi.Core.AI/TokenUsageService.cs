using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.AI.Models;
using RevitAi.Core.Authentication;
using RevitAi.Core.Authentication.Models;
using ns7;

namespace RevitAi.Core.AI;

public sealed class TokenUsageService
{
	[CompilerGenerated]
	public sealed class Class135
	{
		public string string_0;

		internal bool method_0(string string_1)
		{
			return string_0.IndexOf(string_1, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}

	[CompilerGenerated]
	public sealed class Class136
	{
		public string string_0;

		internal bool method_0(string string_1)
		{
			return string_0.IndexOf(string_1, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct211 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<decimal> asyncTaskMethodBuilder_0;

		public string string_0;

		public TokenUsageService tokenUsageService_0;

		public string string_1;

		public int int_1;

		public int int_2;

		public int int_3;

		private Class135 class135_0;

		private TaskAwaiter<decimal?> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TokenUsageService tokenUsageService = tokenUsageService_0;
			if (num != 0)
			{
				this.class135_0 = new Class135();
				this.class135_0.string_0 = string_0;
			}
			decimal result2;
			try
			{
				TaskAwaiter<decimal?> awaiter;
				if (num != 0)
				{
					awaiter = tokenUsageService.method_3(string_1, this.class135_0.string_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<decimal?>);
					num = -1;
					int_0 = -1;
				}
				decimal? result = awaiter.GetResult();
				decimal num5;
				if (result.HasValue && result.Value > 0m)
				{
					int num2 = int_1 - int_2;
					decimal num3 = result.Value * (decimal)int_2 / 1000m * 0.2m;
					decimal num4 = result.Value * (decimal)(num2 + int_3) / 1000m;
					num5 = num3 + num4;
					if (int_2 > 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 4);
						defaultInterpolatedStringHandler.AppendLiteral("[TokenUsage] 缓存折扣计算: ");
						defaultInterpolatedStringHandler.AppendFormatted(int_2);
						defaultInterpolatedStringHandler.AppendLiteral(" 缓存 tokens (");
						defaultInterpolatedStringHandler.AppendFormatted(num3, "F6");
						defaultInterpolatedStringHandler.AppendLiteral("元) + ");
						defaultInterpolatedStringHandler.AppendFormatted(num2 + int_3);
						defaultInterpolatedStringHandler.AppendLiteral(" 正常 tokens (");
						defaultInterpolatedStringHandler.AppendFormatted(num4, "F6");
						defaultInterpolatedStringHandler.AppendLiteral("元)");
						Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				else
				{
				var class135_0 = this.class135_0;
					string text = tokenUsageService.dictionary_1.Keys.FirstOrDefault((string string_1) => class135_0.string_0.IndexOf(string_1, StringComparison.OrdinalIgnoreCase) >= 0);
					if (text != null)
					{
						int num6 = int_1 - int_2;
						decimal num7 = tokenUsageService.dictionary_1[text] * (decimal)int_2 / 1000m * 0.2m;
						decimal num8 = tokenUsageService.dictionary_1[text] * (decimal)(num6 + int_3) / 1000m;
						num5 = num7 + num8;
					}
					else
					{
						int num9 = int_1 - int_2;
						decimal num10 = 0.001m * (decimal)int_2 / 1000m * 0.2m;
						decimal num11 = 0.001m * (decimal)(num9 + int_3) / 1000m;
						num5 = num10 + num11;
					}
				}
				decimal num12 = num5 * 2.0m;
				string text2;
				if (int_2 <= 0)
				{
					text2 = "";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 1);
					defaultInterpolatedStringHandler2.AppendLiteral(" (含 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_2);
					defaultInterpolatedStringHandler2.AppendLiteral(" 缓存 tokens)");
					text2 = defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				string value = text2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(36, 6);
				defaultInterpolatedStringHandler3.AppendLiteral("[TokenUsage] 成本计算: ");
				defaultInterpolatedStringHandler3.AppendFormatted(class135_0.string_0);
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(int_1);
				defaultInterpolatedStringHandler3.AppendLiteral("+");
				defaultInterpolatedStringHandler3.AppendFormatted(int_3);
				defaultInterpolatedStringHandler3.AppendLiteral("tokens");
				defaultInterpolatedStringHandler3.AppendFormatted(value);
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(num5, "F6");
				defaultInterpolatedStringHandler3.AppendLiteral("元 = ");
				defaultInterpolatedStringHandler3.AppendFormatted(num12, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral("电量");
				Logger.Debug(defaultInterpolatedStringHandler3.ToStringAndClear());
				result2 = num12;
			}
			catch (Exception ex)
			{
				Logger.Warning("[TokenUsage] 成本计算失败: " + ex.Message);
				result2 = (decimal)(int_1 + int_3) / 1000m * 0.001m * 2.0m;
			}
			int_0 = -2;
			class135_0 = null;
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
	public struct Struct212 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public TokenUsageService tokenUsageService_0;

		public Guid? nullable_0;

		public Guid? nullable_1;

		public decimal decimal_0;

		private TaskAwaiter<CombinedCreditsInfo?> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TokenUsageService tokenUsageService = tokenUsageService_0;
			bool result2;
			try
			{
				TaskAwaiter<CombinedCreditsInfo> awaiter;
				if (num != 0)
				{
					awaiter = tokenUsageService.GetUserCreditsAsync(nullable_0, nullable_1).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<CombinedCreditsInfo>);
					num = -1;
					int_0 = -1;
				}
				CombinedCreditsInfo result = awaiter.GetResult();
				if (result == null)
				{
					result2 = false;
				}
				else if (result.TotalBalance < decimal_0)
				{
					result2 = false;
				}
				else if (tokenUsageService.sessionTokenAccumulator_0 != null)
				{
					decimal num2 = tokenUsageService.CalculateCostSync(tokenUsageService.sessionTokenAccumulator_0.Provider, tokenUsageService.sessionTokenAccumulator_0.ModelName, tokenUsageService.sessionTokenAccumulator_0.TotalInputTokens, tokenUsageService.sessionTokenAccumulator_0.TotalOutputTokens);
					result2 = result.TotalBalance >= decimal_0 + num2;
				}
				else
				{
					result2 = true;
				}
			}
			catch
			{
				result2 = false;
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
	public struct Struct213 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

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
			bool result = true;
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
	public struct Struct214 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CreditRecharge> asyncTaskMethodBuilder_0;

		public decimal decimal_0;

		public string string_0;

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
			CreditRecharge result = new CreditRecharge
			{
				Id = Guid.NewGuid(),
				Amount = decimal_0,
				PaymentMethod = string_0,
				Status = "pending"
			};
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
	public struct Struct215 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Guid?> asyncTaskMethodBuilder_0;

		public TokenUsageService tokenUsageService_0;

		private int int_1;

		private TokenUsageRequest tokenUsageRequest_0;

		private Guid guid_0;

		private int int_2;

		private decimal decimal_0;

		private TaskAwaiter<Result<Guid>> taskAwaiter_0;

		private TaskAwaiter<decimal> taskAwaiter_1;

		private TaskAwaiter<Result<List<CreditDeductionResult>>> taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TokenUsageService tokenUsageService = tokenUsageService_0;
			Guid? result;
			if ((uint)num > 2u && tokenUsageService.sessionTokenAccumulator_0 == null)
			{
				Logger.Warning("[TokenUsage] 尝试结束会话，但没有活动的会话");
				result = null;
			}
			else
			{
				try
				{
					TaskAwaiter<Result<Guid>> awaiter3;
					TaskAwaiter<decimal> awaiter2;
					TaskAwaiter<Result<List<CreditDeductionResult>>> awaiter;
					decimal result2;
					Result<Guid> result3;
					Result<List<CreditDeductionResult>> result4;
					List<CreditDeductionResult> list;
					switch (num)
					{
					default:
						tokenUsageService.sessionTokenAccumulator_0.EndTime = DateTime.Now;
						int_1 = tokenUsageService.sessionTokenAccumulator_0.TotalDurationMs;
						tokenUsageRequest_0 = tokenUsageService.sessionTokenAccumulator_0.ToTokenUsageRequest();
						awaiter3 = tokenUsageService.isupabaseClient_0.RecordTokenUsageOnlyAsync(tokenUsageRequest_0.UserId, tokenUsageRequest_0.DeviceId, tokenUsageRequest_0.SessionId, tokenUsageRequest_0.Provider, tokenUsageRequest_0.ModelName, tokenUsageRequest_0.InputTokens, tokenUsageRequest_0.OutputTokens, tokenUsageRequest_0.RequestType, tokenUsageRequest_0.IsToolCall, tokenUsageRequest_0.ToolCount, int_1).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter3;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
							return;
						}
						goto IL_0155;
					case 0:
						awaiter3 = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<Result<Guid>>);
						num = -1;
						int_0 = -1;
						goto IL_0155;
					case 1:
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<decimal>);
						num = -1;
						int_0 = -1;
						goto IL_0239;
					case 2:
						{
							awaiter = taskAwaiter_2;
							taskAwaiter_2 = default(TaskAwaiter<Result<List<CreditDeductionResult>>>);
							num = -1;
							int_0 = -1;
							goto IL_02f5;
						}
						IL_0239:
						result2 = awaiter2.GetResult();
						decimal_0 = result2;
						if (!(decimal_0 > 0m))
						{
							break;
						}
						awaiter = tokenUsageService.isupabaseClient_0.DeductCreditsSmartAsync(tokenUsageRequest_0.UserId, tokenUsageRequest_0.DeviceId ?? Guid.Empty, decimal_0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_2 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_02f5;
						IL_0155:
						result3 = awaiter3.GetResult();
						if (result3.IsSuccess)
						{
							guid_0 = result3.Value;
							int_2 = tokenUsageService.sessionTokenAccumulator_0.TotalCachedTokens;
							tokenUsageService.sessionTokenAccumulator_0 = null;
							awaiter2 = tokenUsageService.CalculateCostAsync(tokenUsageRequest_0.Provider, tokenUsageRequest_0.ModelName, tokenUsageRequest_0.InputTokens, tokenUsageRequest_0.OutputTokens, int_2).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_0239;
						}
						Logger.Error("[TokenUsage] ❌ 记录 Token 使用失败: " + result3.Error);
						result = null;
						goto end_IL_0037;
						IL_02f5:
						result4 = awaiter.GetResult();
						if (!result4.IsSuccess)
						{
							Logger.Warning("[TokenUsage] ⚠ 智能扣减失败（但已记录 Token）: " + result4.Error);
							break;
						}
						list = result4.Value ?? new List<CreditDeductionResult>();
						list.Sum((CreditDeductionResult creditDeductionResult_0) => creditDeductionResult_0.ActualDeducted);
						tokenUsageService.method_2(list);
						break;
					}
					string text;
					if (int_2 <= 0)
					{
						text = "";
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
						defaultInterpolatedStringHandler.AppendLiteral(", 缓存命中=");
						defaultInterpolatedStringHandler.AppendFormatted(int_2);
						text = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					string value = text;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(59, 7);
					defaultInterpolatedStringHandler2.AppendLiteral("[TokenUsage] ✅ 会话结束并同步: ");
					defaultInterpolatedStringHandler2.AppendLiteral("输入=");
					defaultInterpolatedStringHandler2.AppendFormatted(tokenUsageRequest_0.InputTokens);
					defaultInterpolatedStringHandler2.AppendLiteral(", 输出=");
					defaultInterpolatedStringHandler2.AppendFormatted(tokenUsageRequest_0.OutputTokens);
					defaultInterpolatedStringHandler2.AppendFormatted(value);
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendLiteral("请求数=");
					defaultInterpolatedStringHandler2.AppendFormatted(tokenUsageRequest_0.RequestCount);
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendLiteral("工具数=");
					defaultInterpolatedStringHandler2.AppendFormatted(tokenUsageRequest_0.ToolCount);
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendLiteral("耗时=");
					defaultInterpolatedStringHandler2.AppendFormatted(int_1);
					defaultInterpolatedStringHandler2.AppendLiteral("ms, ");
					defaultInterpolatedStringHandler2.AppendLiteral("消耗=");
					defaultInterpolatedStringHandler2.AppendFormatted(decimal_0, "F4");
					defaultInterpolatedStringHandler2.AppendLiteral(" 电量");
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					result = guid_0;
					end_IL_0037:;
				}
				catch (Exception ex)
				{
					Logger.Error("[TokenUsage] ❌ 结束会话异常: " + ex.Message, ex);
					tokenUsageService.sessionTokenAccumulator_0 = null;
					result = null;
				}
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
	public struct Struct216 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<decimal?> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public TokenUsageService tokenUsageService_0;

		private string string_2;

		private TaskAwaiter<Result<AIConfig>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TokenUsageService tokenUsageService = tokenUsageService_0;
			decimal? result;
			if (num != 0)
			{
				string_2 = string_0 + ":" + string_1;
				if (tokenUsageService.dictionary_0.ContainsKey(string_2) && DateTime.Now - tokenUsageService.dateTime_1 < tokenUsageService.timeSpan_1)
				{
					result = tokenUsageService.dictionary_0[string_2].CostPer1kTokens;
					goto IL_0157;
				}
			}
			try
			{
				TaskAwaiter<Result<AIConfig>> awaiter;
				if (num != 0)
				{
					awaiter = tokenUsageService.isupabaseClient_0.GetAIConfigByModelNameAsync(string_1).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<AIConfig>>);
					num = -1;
					int_0 = -1;
				}
				Result<AIConfig> result2 = awaiter.GetResult();
				if (result2.IsSuccess && result2.Value != null)
				{
					tokenUsageService.dictionary_0[string_2] = result2.Value;
					tokenUsageService.dateTime_1 = DateTime.Now;
					result = result2.Value.CostPer1kTokens;
					goto IL_0157;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[TokenUsage] 查询 AI 配置失败: " + ex.Message);
			}
			result = null;
			goto IL_0157;
			IL_0157:
			int_0 = -2;
			string_2 = null;
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
	public struct Struct217 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<CreditRecharge>> asyncTaskMethodBuilder_0;

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
			List<CreditRecharge> result = new List<CreditRecharge>();
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
	public struct Struct218 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CombinedCreditsInfo> asyncTaskMethodBuilder_0;

		public bool bool_0;

		public TokenUsageService tokenUsageService_0;

		public Guid? nullable_0;

		public Guid? nullable_1;

		private TaskAwaiter<Result<CombinedCreditsInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Expected O, but got Unknown
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Expected O, but got Unknown
			int num = int_0;
			TokenUsageService tokenUsageService = tokenUsageService_0;
			CombinedCreditsInfo result;
			try
			{
				TaskAwaiter<Result<CombinedCreditsInfo>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<CombinedCreditsInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_00e6;
				}
				if (bool_0 || tokenUsageService.combinedCreditsInfo_0 == null || !(DateTime.Now - tokenUsageService.dateTime_0 < tokenUsageService.timeSpan_0))
				{
					Guid? userId = nullable_0 ?? tokenUsageService.method_0();
					Guid? deviceId = nullable_1 ?? tokenUsageService.method_1();
					awaiter = tokenUsageService.isupabaseClient_0.GetUserCreditsAsync(userId, deviceId).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00e6;
				}
				result = tokenUsageService.combinedCreditsInfo_0;
				goto end_IL_000f;
				IL_00e6:
				Result<CombinedCreditsInfo> result2 = awaiter.GetResult();
				if (result2.IsSuccess && result2.Value != null)
				{
					tokenUsageService.combinedCreditsInfo_0 = result2.Value;
					tokenUsageService.dateTime_0 = DateTime.Now;
					result = result2.Value;
				}
				else
				{
					result = new CombinedCreditsInfo
					{
						TotalBalance = 0m,
						UserBalance = 0m,
						DeviceBalance = 0m,
						TotalPurchased = 0m,
						TotalConsumed = 0m,
						UserPurchased = 0m,
						UserConsumed = 0m,
						DevicePurchased = 0m,
						DeviceConsumed = 0m
					};
				}
				end_IL_000f:;
			}
			catch (Exception)
			{
				result = new CombinedCreditsInfo
				{
					TotalBalance = 0m,
					UserBalance = 0m,
					DeviceBalance = 0m
				};
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

	private CombinedCreditsInfo? combinedCreditsInfo_0;

	private DateTime dateTime_0 = DateTime.MinValue;

	private readonly TimeSpan timeSpan_0 = TimeSpan.FromMinutes(5L);

	private readonly Dictionary<string, AIConfig> dictionary_0 = new Dictionary<string, AIConfig>();

	private DateTime dateTime_1 = DateTime.MinValue;

	private readonly TimeSpan timeSpan_1 = TimeSpan.FromMinutes(10L);

	private readonly Dictionary<string, decimal> dictionary_1 = new Dictionary<string, decimal>
	{
		{
			"claude-3-5-sonnet",
			0.003m
		},
		{
			"claude-3-opus",
			0.015m
		},
		{
			"claude-3-sonnet",
			0.003m
		},
		{
			"claude-3-haiku",
			0.00025m
		},
		{
			"gpt-4",
			0.03m
		},
		{
			"gpt-4-turbo",
			0.01m
		},
		{
			"gpt-4o",
			0.005m
		},
		{
			"gpt-4o-mini",
			0.00015m
		},
		{
			"gpt-3.5-turbo",
			0.0005m
		}
	};

	private const decimal decimal_0 = 2.0m;

	private SessionTokenAccumulator? sessionTokenAccumulator_0;

	public TokenUsageService(ISupabaseClient supabaseClient, IAuthManager authManager)
	{
		isupabaseClient_0 = supabaseClient ?? throw new ArgumentNullException("supabaseClient");
		iauthManager_0 = authManager ?? throw new ArgumentNullException("authManager");
	}

	public SessionTokenAccumulator StartSession(string provider, string modelName)
	{
		sessionTokenAccumulator_0 = new SessionTokenAccumulator
		{
			UserId = method_0(),
			DeviceId = method_1(),
			Provider = provider,
			ModelName = modelName,
			StartTime = DateTime.Now
		};
		return sessionTokenAccumulator_0;
	}

	private Guid? method_0()
	{
		IDeviceInfo currentDevice = iauthManager_0.CurrentDevice;
		if (currentDevice != null && currentDevice.IsBound && currentDevice is DeviceInfo deviceInfo)
		{
			return deviceInfo.UserId;
		}
		return null;
	}

	private Guid? method_1()
	{
		IDeviceInfo currentDevice = iauthManager_0.CurrentDevice;
		if (currentDevice != null && currentDevice.Id != Guid.Empty)
		{
			return currentDevice.Id;
		}
		return null;
	}

	public void AccumulateTokenUsage(int inputTokens, int outputTokens, int durationMs, bool isToolCall = false, int cachedTokens = 0)
	{
		if (sessionTokenAccumulator_0 == null)
		{
			Logger.Warning("[TokenUsage] 尝试累积 Token 使用，但没有活动的会话");
		}
		else
		{
			sessionTokenAccumulator_0.AddUsage(inputTokens, outputTokens, durationMs, isToolCall, cachedTokens);
		}
	}

	[AsyncStateMachine(typeof(Struct215))]
	public Task<Guid?> EndSessionAndSyncAsync()
	{
		Struct215 stateMachine = default(Struct215);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Guid?>.Create();
		stateMachine.tokenUsageService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public (int inputTokens, int outputTokens, decimal estimatedCost)? GetCurrentSessionUsage()
	{
		if (sessionTokenAccumulator_0 == null)
		{
			return null;
		}
		decimal item = CalculateCostSync(sessionTokenAccumulator_0.Provider, sessionTokenAccumulator_0.ModelName, sessionTokenAccumulator_0.TotalInputTokens, sessionTokenAccumulator_0.TotalOutputTokens);
		return (sessionTokenAccumulator_0.TotalInputTokens, sessionTokenAccumulator_0.TotalOutputTokens, item);
	}

	[AsyncStateMachine(typeof(Struct218))]
	public Task<CombinedCreditsInfo?> GetUserCreditsAsync(Guid? userId = null, Guid? deviceId = null, bool forceRefresh = false)
	{
		Struct218 stateMachine = default(Struct218);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CombinedCreditsInfo>.Create();
		stateMachine.tokenUsageService_0 = this;
		stateMachine.nullable_0 = userId;
		stateMachine.nullable_1 = deviceId;
		stateMachine.bool_0 = forceRefresh;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct212))]
	public Task<bool> CheckCreditsSufficientAsync(decimal requiredAmount, Guid? userId = null, Guid? deviceId = null)
	{
		Struct212 stateMachine = default(Struct212);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.tokenUsageService_0 = this;
		stateMachine.decimal_0 = requiredAmount;
		stateMachine.nullable_0 = userId;
		stateMachine.nullable_1 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private void method_2(List<CreditDeductionResult> list_0)
	{
		decimal num = 0m;
		decimal num2 = 0m;
		foreach (CreditDeductionResult item in list_0)
		{
			if (item.Success)
			{
				num += item.UserActualDeducted;
				num2 += item.DeviceActualDeducted;
			}
		}
		if (combinedCreditsInfo_0 != null)
		{
			CombinedCreditsInfo? obj = combinedCreditsInfo_0;
			obj.TotalConsumed += num + num2;
			CombinedCreditsInfo? obj2 = combinedCreditsInfo_0;
			obj2.UserConsumed += num;
			CombinedCreditsInfo? obj3 = combinedCreditsInfo_0;
			obj3.DeviceConsumed += num2;
		}
		if (combinedCreditsInfo_0 != null)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[TokenUsage] ✅ 电量缓存已直接更新: ");
			defaultInterpolatedStringHandler.AppendLiteral("总消耗=");
			defaultInterpolatedStringHandler.AppendFormatted(combinedCreditsInfo_0.TotalConsumed, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendLiteral("用户消耗=");
			defaultInterpolatedStringHandler.AppendFormatted(num, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendLiteral("设备消耗=");
			defaultInterpolatedStringHandler.AppendFormatted(num2, "F2");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}

	public void ClearCreditsCache()
	{
		combinedCreditsInfo_0 = null;
		dateTime_0 = DateTime.MinValue;
	}

	[AsyncStateMachine(typeof(Struct216))]
	private Task<decimal?> method_3(string string_0, string string_1)
	{
		Struct216 stateMachine = default(Struct216);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<decimal?>.Create();
		stateMachine.tokenUsageService_0 = this;
		stateMachine.string_0 = string_0;
		stateMachine.string_1 = string_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct211))]
	public Task<decimal> CalculateCostAsync(string provider, string modelName, int inputTokens, int outputTokens, int cachedTokens = 0)
	{
		Struct211 stateMachine = default(Struct211);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<decimal>.Create();
		stateMachine.tokenUsageService_0 = this;
		stateMachine.string_1 = provider;
		stateMachine.string_0 = modelName;
		stateMachine.int_1 = inputTokens;
		stateMachine.int_3 = outputTokens;
		stateMachine.int_2 = cachedTokens;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public decimal CalculateCostSync(string provider, string modelName, int inputTokens, int outputTokens, int cachedTokens = 0)
	{
		string key = provider + ":" + modelName;
		if (dictionary_0.ContainsKey(key) && dictionary_0[key].CostPer1kTokens.HasValue)
		{
			int num = inputTokens - cachedTokens;
			decimal value = dictionary_0[key].CostPer1kTokens.Value;
			decimal num2 = value * (decimal)cachedTokens / 1000m * 0.2m;
			decimal num3 = value * (decimal)(num + outputTokens) / 1000m;
			return (num2 + num3) * 2.0m;
		}
		string text = dictionary_1.Keys.FirstOrDefault((string string_1) => modelName.IndexOf(string_1, StringComparison.OrdinalIgnoreCase) >= 0);
		decimal obj = ((text != null) ? dictionary_1[text] : 0.001m);
		int num4 = inputTokens - cachedTokens;
		decimal num5 = obj * (decimal)cachedTokens / 1000m * 0.2m;
		decimal num6 = obj * (decimal)(num4 + outputTokens) / 1000m;
		return (num5 + num6) * 2.0m;
	}

	[AsyncStateMachine(typeof(Struct214))]
	public Task<CreditRecharge> CreateRechargeOrderAsync(decimal amount, string paymentMethod, Guid? userId = null, Guid? deviceId = null)
	{
		Struct214 stateMachine = default(Struct214);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CreditRecharge>.Create();
		stateMachine.decimal_0 = amount;
		stateMachine.string_0 = paymentMethod;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct213))]
	public Task<bool> CompleteRechargeAsync(Guid orderId, string transactionId)
	{
		Struct213 stateMachine = default(Struct213);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct217))]
	public Task<List<CreditRecharge>> GetRechargeHistoryAsync(int limit = 20)
	{
		Struct217 stateMachine = default(Struct217);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<CreditRecharge>>.Create();
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
