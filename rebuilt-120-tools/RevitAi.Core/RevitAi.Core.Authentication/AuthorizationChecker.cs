using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using ns7;

namespace RevitAi.Core.Authentication;

public class AuthorizationChecker
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct113 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AuthorizationResult> asyncTaskMethodBuilder_0;

		public AuthorizationChecker authorizationChecker_0;

		private TaskAwaiter<bool> taskAwaiter_0;

		private TaskAwaiter<AuthorizationResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			AuthorizationChecker authorizationChecker = authorizationChecker_0;
			AuthorizationResult result;
			try
			{
				TaskAwaiter<AuthorizationResult> awaiter;
				TaskAwaiter<bool> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AuthorizationResult>);
						num = -1;
						int_0 = -1;
						goto IL_017f;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[授权检查] 开始检查功能授权: ");
					defaultInterpolatedStringHandler.AppendFormatted(authorizationChecker.string_0);
					defaultInterpolatedStringHandler.AppendLiteral(" (功能组: ");
					defaultInterpolatedStringHandler.AppendFormatted(FeatureGroupExtensions.GetDisplayName(authorizationChecker.featureGroup_0));
					defaultInterpolatedStringHandler.AppendLiteral(")");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					awaiter2 = authorizationChecker.method_0().GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<bool>);
					num = -1;
					int_0 = -1;
				}
				if (!awaiter2.GetResult())
				{
					Logger.Info("[授权检查] 未找到有效许可证，检查试用次数...");
					awaiter = authorizationChecker.method_1().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_017f;
				}
				Logger.Info("[授权检查] ✓ 功能已授权（许可证有效）: " + authorizationChecker.string_0);
				result = AuthorizationResult.Granted("已授权（许可证）");
				goto end_IL_000f;
				IL_017f:
				AuthorizationResult result2 = awaiter.GetResult();
				if (result2.IsGranted)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(25, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("[授权检查] ✓ 功能已授权（试用剩余 ");
					defaultInterpolatedStringHandler2.AppendFormatted(result2.RemainingCount);
					defaultInterpolatedStringHandler2.AppendLiteral(" 次）: ");
					defaultInterpolatedStringHandler2.AppendFormatted(authorizationChecker.string_0);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					result = result2;
				}
				else
				{
					Logger.Warning("[授权检查] ✗ 功能授权失败（试用次数已用尽）: " + authorizationChecker.string_0);
					result = AuthorizationResult.Denied("试用次数已用尽，请购买授权");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[授权检查] 检查授权异常: " + authorizationChecker.string_0, ex);
				result = AuthorizationResult.Denied("授权检查异常: " + ex.Message);
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
	public struct Struct114 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AuthorizationResult> asyncTaskMethodBuilder_0;

		public AuthorizationChecker authorizationChecker_0;

		private TaskAwaiter<Result<int>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			AuthorizationChecker authorizationChecker = authorizationChecker_0;
			AuthorizationResult result2;
			try
			{
				TaskAwaiter<Result<int>> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<Result<int>>);
						num = -1;
						int_0 = -1;
						goto IL_0228;
					}
					awaiter = authorizationChecker.authManager_0.GetRemainingTrialUsageAsync(authorizationChecker.string_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<int>>);
					num = -1;
					int_0 = -1;
				}
				Result<int> result = awaiter.GetResult();
				if (!result.IsSuccess)
				{
					goto IL_0186;
				}
				int value = result.Value;
				if (value < 0)
				{
					goto IL_0186;
				}
				if (value <= 0)
				{
					Logger.Warning("[试用检查] 功能试用次数已用尽: " + authorizationChecker.string_0);
					goto IL_0186;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[试用检查] 找到功能试用次数: ");
				defaultInterpolatedStringHandler.AppendFormatted(authorizationChecker.string_0);
				defaultInterpolatedStringHandler.AppendLiteral(" -> 剩余 ");
				defaultInterpolatedStringHandler.AppendFormatted(value);
				defaultInterpolatedStringHandler.AppendLiteral(" 次");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("试用剩余 ");
				defaultInterpolatedStringHandler2.AppendFormatted(value);
				defaultInterpolatedStringHandler2.AppendLiteral(" 次");
				result2 = AuthorizationResult.Granted(defaultInterpolatedStringHandler2.ToStringAndClear(), value);
				goto end_IL_000f;
				IL_0228:
				Result<int> result3 = awaiter.GetResult();
				if (!result3.IsSuccess)
				{
					goto IL_0312;
				}
				int value2 = result3.Value;
				if (value2 <= 0)
				{
					goto IL_0312;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(25, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("[试用检查] 找到功能组配置: ");
				defaultInterpolatedStringHandler3.AppendFormatted(FeatureGroupExtensions.GetDisplayName(authorizationChecker.featureGroup_0));
				defaultInterpolatedStringHandler3.AppendLiteral(" -> 剩余 ");
				defaultInterpolatedStringHandler3.AppendFormatted(value2);
				defaultInterpolatedStringHandler3.AppendLiteral(" 次");
				Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
				Logger.Info("[试用检查] 将使用功能组的次数初始化功能 " + authorizationChecker.string_0);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(16, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("试用剩余 ");
				defaultInterpolatedStringHandler4.AppendFormatted(value2);
				defaultInterpolatedStringHandler4.AppendLiteral(" 次（使用功能组配置）");
				result2 = AuthorizationResult.Granted(defaultInterpolatedStringHandler4.ToStringAndClear(), value2);
				goto end_IL_000f;
				IL_0312:
				Logger.Warning("[试用检查] 未找到功能或功能组的试用配置，使用默认值 10 次");
				result2 = AuthorizationResult.Granted("试用（默认配置）", 10);
				goto end_IL_000f;
				IL_0186:
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(31, 2);
				defaultInterpolatedStringHandler5.AppendLiteral("[试用检查] 缓存中没有功能 ");
				defaultInterpolatedStringHandler5.AppendFormatted(authorizationChecker.string_0);
				defaultInterpolatedStringHandler5.AppendLiteral("，尝试查找功能组 ");
				defaultInterpolatedStringHandler5.AppendFormatted(FeatureGroupExtensions.GetDisplayName(authorizationChecker.featureGroup_0));
				defaultInterpolatedStringHandler5.AppendLiteral(" 的配置...");
				Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
				awaiter = authorizationChecker.method_2().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_0228;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[试用检查] 检查试用次数异常", ex);
				result2 = AuthorizationResult.Denied("检查试用次数异常: " + ex.Message);
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
	public struct Struct115 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<int>> asyncTaskMethodBuilder_0;

		public AuthorizationChecker authorizationChecker_0;

		private TaskAwaiter<Result<int>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			AuthorizationChecker authorizationChecker = authorizationChecker_0;
			Result<int> result;
			try
			{
				TaskAwaiter<Result<int>> awaiter;
				if (num != 0)
				{
					string identifier = FeatureGroupExtensions.GetIdentifier(authorizationChecker.featureGroup_0);
					Logger.Debug("[试用检查] 查询功能组试用次数: " + identifier);
					awaiter = authorizationChecker.authManager_0.GetRemainingTrialUsageByGroupAsync(identifier).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<int>>);
					num = -1;
					int_0 = -1;
				}
				result = awaiter.GetResult();
			}
			catch (Exception ex)
			{
				Logger.Warning("[试用检查] 查询功能组试用次数失败: " + ex.Message);
				result = Result<int>.Failure("查询失败: " + ex.Message);
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
	public struct Struct116 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public AuthorizationChecker authorizationChecker_0;

		private TaskAwaiter<Result<ILicenseInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			AuthorizationChecker authorizationChecker = authorizationChecker_0;
			bool result2;
			try
			{
				TaskAwaiter<Result<ILicenseInfo>> awaiter;
				if (num != 0)
				{
					awaiter = authorizationChecker.authManager_0.GetLicenseInfoAsync().GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<ILicenseInfo>>);
					num = -1;
					int_0 = -1;
				}
				Result<ILicenseInfo> result = awaiter.GetResult();
				if (result.IsSuccess && result.Value != null)
				{
					ILicenseInfo value = result.Value;
					if (!value.IsValid())
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[授权检查] 许可证无效（状态: ");
						defaultInterpolatedStringHandler.AppendFormatted(value.IsActive);
						defaultInterpolatedStringHandler.AppendLiteral(")");
						Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
						result2 = false;
					}
					else
					{
						bool flag = false;
						if (value.Features != null && value.Features.Length != 0)
						{
							if (value.Features.Contains("*"))
							{
								flag = true;
								Logger.Debug("[授权检查] 许可证包含通配符 '*'，授权所有功能");
							}
							else if (value.Features.Contains(authorizationChecker.string_0))
							{
								flag = true;
								Logger.Debug("[授权检查] 许可证包含功能 ID: " + authorizationChecker.string_0);
							}
							else
							{
								string identifier = FeatureGroupExtensions.GetIdentifier(authorizationChecker.featureGroup_0);
								if (value.Features.Contains(identifier))
								{
									flag = true;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(20, 2);
									defaultInterpolatedStringHandler2.AppendLiteral("[授权检查] 许可证包含功能组: ");
									defaultInterpolatedStringHandler2.AppendFormatted(FeatureGroupExtensions.GetDisplayName(authorizationChecker.featureGroup_0));
									defaultInterpolatedStringHandler2.AppendLiteral(" (");
									defaultInterpolatedStringHandler2.AppendFormatted(identifier);
									defaultInterpolatedStringHandler2.AppendLiteral(")");
									Logger.Debug(defaultInterpolatedStringHandler2.ToStringAndClear());
								}
							}
						}
						if (!flag)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(25, 2);
							defaultInterpolatedStringHandler3.AppendLiteral("[授权检查] 许可证不包含功能: ");
							defaultInterpolatedStringHandler3.AppendFormatted(authorizationChecker.string_0);
							defaultInterpolatedStringHandler3.AppendLiteral(" (功能组: ");
							defaultInterpolatedStringHandler3.AppendFormatted(FeatureGroupExtensions.GetDisplayName(authorizationChecker.featureGroup_0));
							defaultInterpolatedStringHandler3.AppendLiteral(")");
							Logger.Debug(defaultInterpolatedStringHandler3.ToStringAndClear());
						}
						result2 = flag;
					}
				}
				else
				{
					Logger.Debug("[授权检查] 未找到许可证信息");
					result2 = false;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[授权检查] 检查许可证异常: " + ex.Message);
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

	private readonly AuthManager authManager_0;

	private readonly string string_0;

	private readonly FeatureGroup featureGroup_0;

	public AuthorizationChecker(AuthManager authManager, string featureId, FeatureGroup featureGroup)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		authManager_0 = authManager ?? throw new ArgumentNullException("authManager");
		string_0 = featureId ?? throw new ArgumentNullException("featureId");
		featureGroup_0 = featureGroup;
	}

	[AsyncStateMachine(typeof(Struct113))]
	public Task<AuthorizationResult> CheckAuthorizationAsync()
	{
		Struct113 stateMachine = default(Struct113);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AuthorizationResult>.Create();
		stateMachine.authorizationChecker_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct116))]
	private Task<bool> method_0()
	{
		Struct116 stateMachine = default(Struct116);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.authorizationChecker_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct114))]
	private Task<AuthorizationResult> method_1()
	{
		Struct114 stateMachine = default(Struct114);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AuthorizationResult>.Create();
		stateMachine.authorizationChecker_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct115))]
	private Task<Result<int>> method_2()
	{
		Struct115 stateMachine = default(Struct115);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<int>>.Create();
		stateMachine.authorizationChecker_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
