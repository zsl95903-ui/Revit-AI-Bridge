using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Network;
using RevitAi.Core.Authentication.Models;
using RevitAi.Core.Configuration;
using RevitAi.Core.Network;
using RevitAi.Core.Security;
using ns7;

namespace RevitAi.Core.Authentication;

public sealed class AuthManager : IAuthManager
{
	[CompilerGenerated]
	public sealed class Class92
	{
		public string string_0;

		public AuthManager authManager_0;

		internal bool method_0(TrialLimitConfig trialLimitConfig_0)
		{
			if (trialLimitConfig_0.IsGroupConfig)
			{
				return trialLimitConfig_0.FeatureGroup == string_0;
			}
			return false;
		}

		internal bool method_1(KeyValuePair<string, int> keyValuePair_0)
		{
			return authManager_0.method_4(keyValuePair_0.Key, string_0);
		}
	}

	[CompilerGenerated]
	public sealed class Class93
	{
		public TrialLimitConfig trialLimitConfig_0;

		internal int method_0(KeyValuePair<string, int> keyValuePair_0)
		{
			return trialLimitConfig_0.MaxUsageCount - keyValuePair_0.Value;
		}
	}

	[CompilerGenerated]
	public sealed class Class94
	{
		public string string_0;

		internal bool method_0(TrialLimitConfig trialLimitConfig_0)
		{
			if (trialLimitConfig_0.IsFeatureConfig)
			{
				return trialLimitConfig_0.FeatureId == string_0;
			}
			return false;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct76 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		public string string_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			TaskAwaiter<Result> awaiter;
			if (num != 0)
			{
				awaiter = authManager.RecordFeatureUsageAsync(string_0).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result>);
				num = -1;
				int_0 = -1;
			}
			Result result = awaiter.GetResult();
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
	public struct Struct77 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<IUserIdentity>> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<UserIdentity>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result<IUserIdentity> result;
			try
			{
				TaskAwaiter<Result<UserIdentity>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<UserIdentity>>);
					num = -1;
					int_0 = -1;
					goto IL_00d1;
				}
				if (authManager.deviceInfo_0 == null)
				{
					result = Result<IUserIdentity>.Failure("设备信息不可用");
				}
				else
				{
					if (authManager.deviceInfo_0.IsBound && authManager.deviceInfo_0.UserId.HasValue)
					{
						awaiter = authManager.isupabaseClient_0.SignInByDeviceAsync(authManager.deviceInfo_0.DeviceId).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00d1;
					}
					result = Result<IUserIdentity>.Failure("设备未绑定到用户");
				}
				goto end_IL_000f;
				IL_00d1:
				Result<UserIdentity> result2 = awaiter.GetResult();
				if (!result2.IsSuccess)
				{
					Logger.Info("[AuthManager] 自动登录失败: " + result2.Error);
					result = Result<IUserIdentity>.Failure(result2.Error ?? "自动登录失败");
				}
				else
				{
					authManager.userIdentity_0 = result2.Value;
					result = ((authManager.userIdentity_0 != null) ? Result<IUserIdentity>.Success((IUserIdentity)(object)authManager.userIdentity_0) : Result<IUserIdentity>.Failure("自动登录失败"));
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 自动登录异常", ex);
				result = Result<IUserIdentity>.Failure("自动登录异常：" + ex.Message);
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
	public struct Struct78 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		private TaskAwaiter<Result<LicenseTransferResult>> taskAwaiter_1;

		private TaskAwaiter taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter<Result> awaiter3;
				TaskAwaiter<Result<LicenseTransferResult>> awaiter2;
				TaskAwaiter awaiter;
				Result<LicenseTransferResult> result2;
				Result result3;
				switch (num)
				{
				default:
					if (authManager.userIdentity_0 == null)
					{
						result = Result.Failure("用户未登录");
					}
					else
					{
						if (authManager.deviceInfo_0 != null)
						{
							awaiter3 = authManager.isupabaseClient_0.BindDeviceAsync(authManager.deviceInfo_0.DeviceId, authManager.userIdentity_0.UserId).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter3;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
								return;
							}
							goto IL_00cd;
						}
						result = Result.Failure("设备信息不可用");
					}
					goto end_IL_000f;
				case 0:
					awaiter3 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_00cd;
				case 1:
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<LicenseTransferResult>>);
					num = -1;
					int_0 = -1;
					goto IL_01a8;
				case 2:
					{
						awaiter = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01a8:
					result2 = awaiter2.GetResult();
					if (result2.IsSuccess && result2.Value != null)
					{
						LicenseTransferResult value = result2.Value;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] ✓ 授权转移完成: ");
						defaultInterpolatedStringHandler.AppendFormatted(value);
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						if (value.HasLicense && value.LicenseId.HasValue)
						{
							authManager.licenseInfo_0 = new LicenseInfo
							{
								LicenseId = value.LicenseId.Value,
								LicenseType = (value.LicenseType ?? "unknown"),
								ValidFrom = (value.ValidFrom ?? DateTime.UtcNow),
								ValidTo = value.ValidTo,
								FeaturesList = ((value.Features != null) ? new List<string> { value.Features } : new List<string>())
							};
						}
					}
					authManager.deviceInfo_0.UserId = authManager.userIdentity_0.UserId;
					authManager.deviceInfo_0.IsBound = true;
					awaiter = authManager.method_6().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 2;
						int_0 = 2;
						taskAwaiter_2 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					break;
					IL_00cd:
					result3 = awaiter3.GetResult();
					if (result3.IsSuccess)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("[AuthManager] ✓ 设备已绑定到用户: ");
						defaultInterpolatedStringHandler2.AppendFormatted(authManager.userIdentity_0.UserId);
						Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
						awaiter2 = authManager.isupabaseClient_0.TransferLicenseToUserOnLoginAsync(authManager.deviceInfo_0.DeviceId, authManager.userIdentity_0.UserId).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_01a8;
					}
					result = Result.Failure("绑定设备失败：" + result3.Error);
					goto end_IL_000f;
				}
				awaiter.GetResult();
				result = Result.Success();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
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
	public struct Struct79 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<(bool canBind, string message)> taskAwaiter_0;

		private TaskAwaiter taskAwaiter_1;

		private TaskAwaiter<Result> taskAwaiter_2;

		private TaskAwaiter<Result<LicenseTransferResult>> taskAwaiter_3;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			try
			{
				TaskAwaiter<(bool, string)> awaiter4;
				TaskAwaiter awaiter3;
				TaskAwaiter<Result> awaiter2;
				TaskAwaiter<Result<LicenseTransferResult>> awaiter;
				(bool, string) result;
				Result result2;
				switch (num)
				{
				default:
					if (authManager.userIdentity_0 != null && authManager.deviceInfo_0 != null)
					{
						Logger.Info("----------------------------------------");
						Logger.Info("开始登录设备绑定和授权转移流程...");
						awaiter4 = authManager.CanBindCurrentDeviceAsync().GetAwaiter();
						if (!awaiter4.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter4;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref this);
							return;
						}
						goto IL_00be;
					}
					Logger.Warning("[AuthManager] 用户或设备信息为空，跳过绑定流程");
					goto end_IL_000f;
				case 0:
					awaiter4 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<(bool, string)>);
					num = -1;
					int_0 = -1;
					goto IL_00be;
				case 1:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_01bc;
				case 2:
					awaiter2 = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_01e5;
				case 3:
					{
						awaiter = taskAwaiter_3;
						taskAwaiter_3 = default(TaskAwaiter<Result<LicenseTransferResult>>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01bc:
					awaiter3.GetResult();
					goto end_IL_000f;
					IL_00be:
					result = awaiter4.GetResult();
					if (result.Item1)
					{
						awaiter2 = authManager.isupabaseClient_0.BindDeviceAsync(authManager.deviceInfo_0.DeviceId, authManager.userIdentity_0.UserId).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_2 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_01e5;
					}
					Logger.Warning("[AuthManager] 无法绑定设备: " + result.Item2);
					if (authManager.userIdentity_0.UserId != Guid.Empty)
					{
						awaiter3 = authManager.method_16(authManager.userIdentity_0.UserId).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter3;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
							return;
						}
						goto IL_01bc;
					}
					goto end_IL_000f;
					IL_01e5:
					result2 = awaiter2.GetResult();
					if (result2.IsSuccess)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] ✓ 设备已绑定到用户: ");
						defaultInterpolatedStringHandler.AppendFormatted(authManager.userIdentity_0.UserId);
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						awaiter = authManager.isupabaseClient_0.TransferLicenseToUserOnLoginAsync(authManager.deviceInfo_0.DeviceId, authManager.userIdentity_0.UserId).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 3;
							int_0 = 3;
							taskAwaiter_3 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						break;
					}
					Logger.Error("[AuthManager] 绑定设备失败: " + result2.Error);
					goto end_IL_000f;
				}
				Result<LicenseTransferResult> result3 = awaiter.GetResult();
				string text;
				string licenseType;
				string text2;
				object obj;
				if (result3.IsSuccess && result3.Value != null)
				{
					LicenseTransferResult value = result3.Value;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(24, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[AuthManager] ✓ 授权转移完成: ");
					defaultInterpolatedStringHandler2.AppendFormatted(value);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					if (value.HasLicense && value.LicenseId.HasValue)
					{
						authManager.licenseInfo_0 = new LicenseInfo
						{
							LicenseId = value.LicenseId.Value,
							LicenseType = (value.LicenseType ?? "unknown"),
							ValidFrom = (value.ValidFrom ?? DateTime.UtcNow),
							ValidTo = value.ValidTo,
							FeaturesList = ((value.Features != null) ? new List<string> { value.Features } : new List<string>())
						};
						text = "[AuthManager] ✓ 用户授权已缓存: ";
						licenseType = authManager.licenseInfo_0.LicenseType;
						text2 = ", 到期: ";
						DateTime? validTo = authManager.licenseInfo_0.ValidTo;
						if (!validTo.HasValue)
						{
							obj = null;
						}
						else
						{
							obj = validTo.GetValueOrDefault().ToString("yyyy-MM-dd");
							if (obj != null)
							{
								goto IL_0431;
							}
						}
						obj = "永久";
						goto IL_0431;
					}
					authManager.licenseInfo_0 = null;
					Logger.Info("[AuthManager] 用户无授权");
				}
				else
				{
					Logger.Warning("[AuthManager] 授权转移失败: " + result3.Error);
				}
				goto IL_0470;
				IL_0431:
				Logger.Info(text + licenseType + text2 + (string?)obj);
				goto IL_0470;
				IL_0470:
				Logger.Info("----------------------------------------");
				Logger.Info("登录设备绑定和授权转移流程完成");
				Logger.Info("  - 许可证状态: " + ((authManager.licenseInfo_0 == null || !authManager.licenseInfo_0.IsValid()) ? "无/过期" : "有效"));
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("  - 电量余额: ");
				defaultInterpolatedStringHandler3.AppendFormatted(authManager.decimal_0, "F2");
				Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
				Logger.Info("----------------------------------------");
				authManager.method_14();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 设备绑定和授权转移异常", ex);
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
	public struct Struct80 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<(bool canBind, string message)> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<List<DeviceInfo>>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager CS_0024_003C_003E8__locals5 = authManager_0;
			(bool, string) result;
			try
			{
				TaskAwaiter<Result<List<DeviceInfo>>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<List<DeviceInfo>>>);
					num = -1;
					int_0 = -1;
					goto IL_00b8;
				}
				if (CS_0024_003C_003E8__locals5.userIdentity_0 == null)
				{
					result = (false, "用户未登录");
				}
				else
				{
					if (CS_0024_003C_003E8__locals5.deviceInfo_0 != null)
					{
						awaiter = CS_0024_003C_003E8__locals5.isupabaseClient_0.GetUserDevicesAsync(CS_0024_003C_003E8__locals5.userIdentity_0.UserId).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00b8;
					}
					result = (false, "设备信息不可用");
				}
				goto end_IL_000f;
				IL_00b8:
				Result<List<DeviceInfo>> result2 = awaiter.GetResult();
				if (result2.IsSuccess && result2.Value != null)
				{
					int num2 = result2.Value.Count((DeviceInfo deviceInfo_1) => deviceInfo_1.DeviceId != CS_0024_003C_003E8__locals5.deviceInfo_0.DeviceId);
					if (num2 >= 2)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 2);
						defaultInterpolatedStringHandler.AppendLiteral("DEVICE_LIMIT_REACHED|您已绑定 ");
						defaultInterpolatedStringHandler.AppendFormatted(num2);
						defaultInterpolatedStringHandler.AppendLiteral(" 台其他设备，达到上限（");
						defaultInterpolatedStringHandler.AppendFormatted(2);
						defaultInterpolatedStringHandler.AppendLiteral("台）。请先解绑一台设备后再绑定当前设备。");
						result = (false, defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(19, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("可以绑定当前设备（已使用 ");
						defaultInterpolatedStringHandler2.AppendFormatted(num2);
						defaultInterpolatedStringHandler2.AppendLiteral("/");
						defaultInterpolatedStringHandler2.AppendFormatted(2);
						defaultInterpolatedStringHandler2.AppendLiteral(" 台设备）");
						result = (true, defaultInterpolatedStringHandler2.ToStringAndClear());
					}
				}
				else
				{
					result = (false, "无法获取设备列表");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 检查设备绑定权限失败", ex);
				result = (false, "检查失败：" + ex.Message);
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
	public struct Struct81 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		public string string_0;

		public string string_1;

		public int int_1;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			TaskAwaiter<Result> awaiter;
			if (num != 0)
			{
				if (authManager.deviceInfo_0 == null)
				{
					result = Result.Failure("设备未初始化");
					goto IL_00d7;
				}
				Guid? userId = (authManager.deviceInfo_0.IsBound ? authManager.deviceInfo_0.UserId : ((Guid?)null));
				awaiter = authManager.isupabaseClient_0.CreateTrialRecordAsync(authManager.deviceInfo_0.DeviceId, string_0, string_1, int_1, userId).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result>);
				num = -1;
				int_0 = -1;
			}
			result = awaiter.GetResult();
			goto IL_00d7;
			IL_00d7:
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
	public struct Struct82 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<IDeviceInfo>> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result<IDeviceInfo> result;
			try
			{
				TaskAwaiter<Result> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_00c0;
				}
				if (authManager.deviceInfo_0 == null)
				{
					result = Result<IDeviceInfo>.Failure("设备信息未初始化");
				}
				else
				{
					if (!(authManager.deviceInfo_0.Id != Guid.Empty))
					{
						Logger.Warning("[AuthManager] 设备未注册（Id 为空），尝试重新注册...");
						awaiter = authManager.RegisterOrFindDeviceAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00c0;
					}
					result = Result<IDeviceInfo>.Success((IDeviceInfo)(object)authManager.deviceInfo_0);
				}
				goto end_IL_000f;
				IL_00c0:
				Result result2 = awaiter.GetResult();
				if (!result2.IsSuccess)
				{
					result = Result<IDeviceInfo>.Failure("设备注册失败：" + result2.Error);
				}
				else if (authManager.deviceInfo_0 != null && !(authManager.deviceInfo_0.Id == Guid.Empty))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] 设备注册成功: ");
					defaultInterpolatedStringHandler.AppendFormatted(authManager.deviceInfo_0.Id);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					result = Result<IDeviceInfo>.Success((IDeviceInfo)(object)authManager.deviceInfo_0);
				}
				else
				{
					Logger.Error("[AuthManager] 设备注册后 Id 仍为空");
					result = Result<IDeviceInfo>.Failure("设备未注册成功，请检查网络连接后重试");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 确保设备注册异常", ex);
				result = Result<IDeviceInfo>.Failure("设备注册异常：" + ex.Message);
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
	public struct Struct83 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public AuthManager authManager_0;

		public string string_1;

		private TaskAwaiter<Result<TrialRecordDetail?>> taskAwaiter_0;

		private TaskAwaiter<Result> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter<Result<TrialRecordDetail>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<TrialRecordDetail>>);
					num = -1;
					int_0 = -1;
					goto IL_010d;
				}
				TaskAwaiter<Result> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_0286;
				}
				if (string.IsNullOrEmpty(string_0))
				{
					result = Result.Failure("功能 ID 不能为空");
				}
				else if (authManager.dictionary_0.ContainsKey(string_0))
				{
					result = Result.Success();
				}
				else
				{
					if (authManager.deviceInfo_0 != null)
					{
						awaiter = authManager.method_2(string_0, string_1).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_010d;
					}
					Logger.Warning("[试用记录] 设备未初始化，无法确保试用记录");
					result = Result.Failure("设备未初始化");
				}
				goto end_IL_000f;
				IL_010d:
				Result<TrialRecordDetail> result2 = awaiter.GetResult();
				if (result2.IsSuccess && result2.Value != null)
				{
					authManager.dictionary_0[string_0] = result2.Value.UsageCount;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[试用记录] 已缓存现有记录: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral(" -> 已使用 ");
					defaultInterpolatedStringHandler.AppendFormatted(result2.Value.UsageCount);
					defaultInterpolatedStringHandler.AppendLiteral(" 次");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					result = Result.Success();
				}
				else
				{
					int num2 = 0;
					if (!string.IsNullOrEmpty(string_1))
					{
						num2 = authManager.GetMaxTrialCountFromFeatureGroup(string_1);
					}
					if (num2 <= 0)
					{
						num2 = authManager.method_1(string_0);
					}
					if (num2 > 0)
					{
						awaiter2 = authManager.method_3(string_0, string_1, num2).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_0286;
					}
					Logger.Warning("[试用记录] 功能 " + string_0 + " 未配置试用次数，无法创建记录");
					result = Result.Failure("功能 " + string_0 + " 未配置试用次数");
				}
				goto end_IL_000f;
				IL_0286:
				Result result3 = awaiter2.GetResult();
				if (result3.IsSuccess)
				{
					authManager.dictionary_0[string_0] = 0;
					Logger.Info("[试用记录] 已创建新记录: " + string_0 + " -> 0 次");
					result = Result.Success();
				}
				else
				{
					result = Result.Failure(result3.Error ?? "创建试用记录失败");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[试用记录] 确保试用记录存在异常: " + string_0, ex);
				result = Result.Failure(ex.Message);
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
	public struct Struct84 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<ILicenseInfo>> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<LicenseInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result<ILicenseInfo> result;
			try
			{
				TaskAwaiter<Result<LicenseInfo>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<LicenseInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_00b2;
				}
				if (authManager.licenseInfo_0 != null)
				{
					result = Result<ILicenseInfo>.Success((ILicenseInfo)(object)authManager.licenseInfo_0);
				}
				else
				{
					if (authManager.userIdentity_0 != null)
					{
						awaiter = authManager.isupabaseClient_0.GetLicenseInfoAsync(authManager.userIdentity_0.UserId).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00b2;
					}
					result = Result<ILicenseInfo>.Failure("许可证信息不可用");
				}
				goto end_IL_000f;
				IL_00b2:
				Result<LicenseInfo> result2 = awaiter.GetResult();
				if (result2.IsSuccess)
				{
					authManager.licenseInfo_0 = result2.Value;
					result = Result<ILicenseInfo>.Success((ILicenseInfo)(object)authManager.licenseInfo_0);
				}
				else
				{
					result = Result<ILicenseInfo>.Failure(result2.Error ?? "获取许可证信息失败");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result<ILicenseInfo>.Failure("获取许可证信息异常：" + ex.Message);
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
	public struct Struct85 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<int>> asyncTaskMethodBuilder_0;

		public string string_0;

		public AuthManager authManager_0;

		private int int_1;

		private TaskAwaiter<Result<int>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result<int> result;
			try
			{
				if (num == 0)
				{
					goto IL_0193;
				}
				if (string.IsNullOrEmpty(string_0))
				{
					result = Result<int>.Failure("功能 ID 不能为空");
				}
				else if (authManager.bool_3)
				{
					Logger.Warning("[试用次数] 设备已被禁用，功能 " + string_0 + " 不可用");
					result = Result<int>.Success(0);
				}
				else if (authManager.dictionary_3.ContainsKey(string_0) && authManager.dictionary_3[string_0] == "core")
				{
					result = Result<int>.Success(int.MaxValue);
				}
				else if (authManager.dictionary_0.ContainsKey(string_0))
				{
					int num2 = authManager.dictionary_0[string_0];
					int num3 = authManager.method_1(string_0);
					if (num3 <= 0)
					{
						Logger.Warning("[试用次数] 功能 " + string_0 + " 未配置，返回 0 次");
						result = Result<int>.Success(0);
					}
					else
					{
						result = Result<int>.Success(Math.Max(0, num3 - num2));
					}
				}
				else
				{
					int_1 = authManager.method_1(string_0);
					if (int_1 <= 0)
					{
						Logger.Warning("[试用次数] 功能 " + string_0 + " 未找到配置，返回 0 次");
						result = Result<int>.Success(0);
					}
					else
					{
						if (!authManager.IsOfflineMode && authManager.deviceInfo_0 != null)
						{
							goto IL_0193;
						}
						Logger.Warning("[试用次数] 离线模式，功能 " + string_0 + " 不可用（无网络连接）");
						result = Result<int>.Success(0);
					}
				}
				goto end_IL_000f;
				IL_0193:
				try
				{
					TaskAwaiter<Result<int>> awaiter;
					if (num != 0)
					{
						awaiter = authManager.isupabaseClient_0.RecordTrialUsageAsync(authManager.deviceInfo_0.DeviceId, string_0, authManager.deviceInfo_0.IsBound ? authManager.deviceInfo_0.UserId : ((Guid?)null)).GetAwaiter();
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
					Result<int> result2 = awaiter.GetResult();
					if (result2.IsSuccess)
					{
						int value = result2.Value;
						int value2 = int_1 - value;
						authManager.dictionary_0[string_0] = value2;
						authManager.bool_4 = true;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 3);
						defaultInterpolatedStringHandler.AppendLiteral("[试用次数] 功能 ");
						defaultInterpolatedStringHandler.AppendFormatted(string_0);
						defaultInterpolatedStringHandler.AppendLiteral(" 剩余 ");
						defaultInterpolatedStringHandler.AppendFormatted(value);
						defaultInterpolatedStringHandler.AppendLiteral(" 次（已使用 ");
						defaultInterpolatedStringHandler.AppendFormatted(value2);
						defaultInterpolatedStringHandler.AppendLiteral(" 次）");
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						result = Result<int>.Success(value);
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(29, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("[试用次数] 记录使用失败（网络异常）: ");
						defaultInterpolatedStringHandler2.AppendFormatted(result2.Error);
						defaultInterpolatedStringHandler2.AppendLiteral("，功能 ");
						defaultInterpolatedStringHandler2.AppendFormatted(string_0);
						defaultInterpolatedStringHandler2.AppendLiteral(" 不可用");
						Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
						authManager.bool_4 = false;
						result = Result<int>.Success(0);
					}
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(29, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("[试用次数] 记录使用异常（网络错误）: ");
					defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
					defaultInterpolatedStringHandler3.AppendLiteral("，功能 ");
					defaultInterpolatedStringHandler3.AppendFormatted(string_0);
					defaultInterpolatedStringHandler3.AppendLiteral(" 不可用");
					Logger.Warning(defaultInterpolatedStringHandler3.ToStringAndClear());
					authManager.bool_4 = false;
					result = Result<int>.Success(0);
				}
				end_IL_000f:;
			}
			catch (Exception ex2)
			{
				result = Result<int>.Failure("获取剩余试用次数异常：" + ex2.Message);
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
	public struct Struct86 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<TrialRecordDetail?>> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		public string string_0;

		public string string_1;

		private TaskAwaiter<Result<TrialRecordDetail?>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result<TrialRecordDetail> result;
			TaskAwaiter<Result<TrialRecordDetail>> awaiter;
			if (num != 0)
			{
				if (authManager.deviceInfo_0 == null)
				{
					result = Result<TrialRecordDetail>.Failure("设备未初始化");
					goto IL_00d1;
				}
				Guid? userId = (authManager.deviceInfo_0.IsBound ? authManager.deviceInfo_0.UserId : ((Guid?)null));
				awaiter = authManager.isupabaseClient_0.GetTrialRecordAsync(authManager.deviceInfo_0.DeviceId, string_0, userId, string_1).GetAwaiter();
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
				taskAwaiter_0 = default(TaskAwaiter<Result<TrialRecordDetail>>);
				num = -1;
				int_0 = -1;
			}
			result = awaiter.GetResult();
			goto IL_00d1;
			IL_00d1:
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
	public struct Struct87 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<IDeviceInfo>>> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<List<DeviceInfo>>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result<List<IDeviceInfo>> result;
			try
			{
				TaskAwaiter<Result<List<DeviceInfo>>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<List<DeviceInfo>>>);
					num = -1;
					int_0 = -1;
					goto IL_0096;
				}
				if (authManager.userIdentity_0 != null)
				{
					awaiter = authManager.isupabaseClient_0.GetUserDevicesAsync(authManager.userIdentity_0.UserId).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0096;
				}
				result = Result<List<IDeviceInfo>>.Failure("用户未登录");
				goto end_IL_000f;
				IL_0096:
				Result<List<DeviceInfo>> result2 = awaiter.GetResult();
				result = (result2.IsSuccess ? Result<List<IDeviceInfo>>.Success(result2.Value.Cast<IDeviceInfo>().ToList()) : Result<List<IDeviceInfo>>.Failure(result2.Error ?? "获取用户设备失败"));
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result<List<IDeviceInfo>>.Failure("获取用户设备异常：" + ex.Message);
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
	public struct Struct88 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private string string_0;

		private TaskAwaiter<bool> taskAwaiter_0;

		private TaskAwaiter<(string? PublicIp, string? LocalIp)> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result2;
			try
			{
				TaskAwaiter<(string, string)> awaiter;
				TaskAwaiter<bool> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<(string, string)>);
						num = -1;
						int_0 = -1;
						goto IL_0146;
					}
					awaiter2 = authManager.networkStatusChecker_0.CheckNetworkAsync().GetAwaiter();
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
					authManager.bool_1 = true;
					authManager.bool_4 = false;
					Logger.Warning("[AuthManager] ✗ 网络不可用，无法使用试用功能");
					Logger.Warning("[AuthManager] 提示：请检查网络连接后重启 Revit");
					Logger.Warning(authManager.networkStatusChecker_0.GetDiagnosticInfo());
				}
				else
				{
					authManager.bool_1 = false;
					authManager.bool_4 = true;
					authManager.networkStatusChecker_0.StartPeriodicCheck(30);
				}
				string_0 = authManager.inativeCryptoService_0.GetHardwareFingerprintV2();
				awaiter = authManager.ipAddressService_0.GetIpAddressesAsync().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_1 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_0146;
				IL_0146:
				(string, string) result = awaiter.GetResult();
				string item = result.Item1;
				string item2 = result.Item2;
				string ipAddress = item ?? item2 ?? "Unknown";
				authManager.deviceInfo_0 = new DeviceInfo
				{
					DeviceId = string_0,
					DeviceName = Environment.MachineName,
					OsVersion = Environment.OSVersion.ToString(),
					IsActive = true,
					IpAddress = ipAddress
				};
				authManager.method_13(authManager.deviceInfo_0, authManager.string_0);
				authManager.bool_0 = true;
				result2 = Result.Success();
			}
			catch (Exception ex)
			{
				result2 = Result.Failure("初始化授权管理器失败：" + ex.Message);
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
	public struct Struct89 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<UserCreditsResult>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter<Result<UserCreditsResult>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<UserCreditsResult>>);
					num = -1;
					int_0 = -1;
					goto IL_00fe;
				}
				if (authManager.userIdentity_0 == null)
				{
					result = Result.Failure("用户未登录");
				}
				else
				{
					if (!(authManager.userIdentity_0.UserId == Guid.Empty))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] 初始化新用户电量: ");
						defaultInterpolatedStringHandler.AppendFormatted(authManager.userIdentity_0.UserId);
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						awaiter = authManager.isupabaseClient_0.InitializeUserCreditsAsync(authManager.userIdentity_0.UserId).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00fe;
					}
					result = Result.Failure("用户 ID 无效");
				}
				goto end_IL_000f;
				IL_00fe:
				Result<UserCreditsResult> result2 = awaiter.GetResult();
				if (result2.IsSuccess && result2.Value != null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[AuthManager] ✓ 用户电量初始化完成: ");
					defaultInterpolatedStringHandler2.AppendFormatted(result2.Value.CurrentBalance);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					result = Result.Success();
				}
				else
				{
					result = Result.Failure(result2.Error ?? "初始化用户电量失败");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 初始化用户电量异常", ex);
				result = Result.Failure("初始化用户电量异常：" + ex.Message);
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
	public struct Struct90 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private CombinedCreditsInfo combinedCreditsInfo_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<CombinedCreditsInfo?> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Expected O, but got Unknown
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Expected O, but got Unknown
			int num = int_0;
			AuthManager authManager = authManager_0;
			try
			{
				TaskAwaiter awaiter2;
				TaskAwaiter<CombinedCreditsInfo> awaiter;
				CombinedCreditsInfo result;
				Guid? nullable_;
				CombinedCreditsInfo result2;
				CombinedCreditsInfo result3;
				CombinedCreditsInfo obj;
				decimal num2;
				CombinedCreditsInfo obj2;
				decimal num3;
				decimal num4;
				decimal num5;
				CombinedCreditsInfo obj3;
				decimal num6;
				decimal num7;
				decimal totalBalance;
				switch (num)
				{
				default:
					Logger.Info("----------------------------------------");
					Logger.Info("开始加载授权数据...");
					if (authManager.deviceInfo_0 != null)
					{
						if (authManager.deviceInfo_0.IsBound && authManager.deviceInfo_0.UserId.HasValue)
						{
							if (authManager.deviceInfo_0.UserId.HasValue)
							{
								awaiter2 = authManager.method_7(authManager.deviceInfo_0.UserId.Value).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 0;
									int_0 = 0;
									taskAwaiter_0 = awaiter2;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
								goto IL_0174;
							}
							goto IL_017b;
						}
						Logger.Info("[授权数据] 设备未绑定用户，按设备字段查询");
						awaiter2 = authManager.method_8(authManager.deviceInfo_0.DeviceId).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 6;
							int_0 = 6;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_0550;
					}
					Logger.Warning("[授权数据] 设备信息为空，跳过加载");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_0174;
				case 1:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_01f4;
				case 2:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_0285;
				case 3:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<CombinedCreditsInfo>);
					num = -1;
					int_0 = -1;
					goto IL_0324;
				case 4:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<CombinedCreditsInfo>);
					num = -1;
					int_0 = -1;
					goto IL_03a9;
				case 5:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_0520;
				case 6:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_0550;
				case 7:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_05e1;
				case 8:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<CombinedCreditsInfo>);
						num = -1;
						int_0 = -1;
						goto IL_065c;
					}
					IL_0527:
					combinedCreditsInfo_0 = null;
					break;
					IL_028c:
					combinedCreditsInfo_0 = null;
					if (authManager.deviceInfo_0.UserId.HasValue)
					{
						awaiter = authManager.method_10(authManager.deviceInfo_0.UserId.Value).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 3;
							int_0 = 3;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0324;
					}
					goto IL_0335;
					IL_0550:
					awaiter2.GetResult();
					if (!authManager.method_11())
					{
						Logger.Info("[授权数据] 许可证无效或过期，加载设备试用记录");
						awaiter2 = authManager.method_9(authManager.deviceInfo_0.DeviceId).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 7;
							int_0 = 7;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_05e1;
					}
					Logger.Info("[授权数据] 许可证有效，无需加载试用记录");
					goto IL_05e8;
					IL_0324:
					result = awaiter.GetResult();
					combinedCreditsInfo_0 = result;
					goto IL_0335;
					IL_0335:
					nullable_ = authManager.deviceInfo_0.Id;
					awaiter = authManager.method_10(null, nullable_).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 4;
						int_0 = 4;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_03a9;
					IL_05e8:
					nullable_ = authManager.deviceInfo_0.Id;
					awaiter = authManager.method_10(null, nullable_).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 8;
						int_0 = 8;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_065c;
					IL_0174:
					awaiter2.GetResult();
					goto IL_017b;
					IL_017b:
					if (!authManager.method_11())
					{
						Logger.Info("[授权数据] 用户许可证无效或过期，尝试查询设备许可证");
						awaiter2 = authManager.method_8(authManager.deviceInfo_0.DeviceId).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_01f4;
					}
					goto IL_01fb;
					IL_065c:
					result2 = awaiter.GetResult();
					authManager.decimal_0 = ((result2 != null) ? result2.TotalBalance : 0m);
					authManager.combinedCreditsInfo_0 = new CombinedCreditsInfo
					{
						TotalBalance = ((result2 != null) ? result2.TotalBalance : 0m),
						UserBalance = 0m,
						DeviceBalance = ((result2 != null) ? result2.DeviceBalance : 0m),
						TotalPurchased = ((result2 != null) ? result2.TotalPurchased : 0m),
						TotalConsumed = ((result2 != null) ? result2.TotalConsumed : 0m),
						UserPurchased = 0m,
						UserConsumed = 0m,
						DevicePurchased = ((result2 != null) ? result2.DevicePurchased : 0m),
						DeviceConsumed = ((result2 != null) ? result2.DeviceConsumed : 0m)
					};
					break;
					IL_05e1:
					awaiter2.GetResult();
					goto IL_05e8;
					IL_01f4:
					awaiter2.GetResult();
					goto IL_01fb;
					IL_01fb:
					if (!authManager.method_11())
					{
						Logger.Info("[授权数据] 许可证无效或过期，加载设备试用记录");
						awaiter2 = authManager.method_9(authManager.deviceInfo_0.DeviceId).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_0285;
					}
					Logger.Info("[授权数据] 许可证有效，无需加载试用记录");
					goto IL_028c;
					IL_0520:
					awaiter2.GetResult();
					goto IL_0527;
					IL_03a9:
					result3 = awaiter.GetResult();
					obj = combinedCreditsInfo_0;
					num2 = ((obj != null) ? obj.TotalPurchased : 0m);
					obj2 = combinedCreditsInfo_0;
					num3 = ((obj2 != null) ? obj2.TotalConsumed : 0m);
					num4 = ((result3 != null) ? result3.TotalPurchased : 0m);
					num5 = ((result3 != null) ? result3.TotalConsumed : 0m);
					obj3 = combinedCreditsInfo_0;
					num6 = ((obj3 != null) ? obj3.UserBalance : 0m);
					num7 = ((result3 != null) ? result3.DeviceBalance : 0m);
					totalBalance = (authManager.decimal_0 = num6 + num7);
					authManager.combinedCreditsInfo_0 = new CombinedCreditsInfo
					{
						TotalBalance = totalBalance,
						UserBalance = num6,
						DeviceBalance = num7,
						TotalPurchased = num2 + num4,
						TotalConsumed = num3 + num5,
						UserPurchased = num2,
						UserConsumed = num3,
						DevicePurchased = num4,
						DeviceConsumed = num5
					};
					if (authManager.deviceInfo_0.UserId.HasValue)
					{
						awaiter2 = authManager.method_16(authManager.deviceInfo_0.UserId.Value).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 5;
							int_0 = 5;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_0520;
					}
					goto IL_0527;
					IL_0285:
					awaiter2.GetResult();
					goto IL_028c;
				}
				Logger.Info("----------------------------------------");
				Logger.Info("授权数据加载完成");
				Logger.Info("  - 许可证状态: " + ((authManager.licenseInfo_0 == null || !authManager.licenseInfo_0.IsValid()) ? "无/过期" : "有效"));
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler.AppendLiteral("  - 试用记录数: ");
				defaultInterpolatedStringHandler.AppendFormatted(authManager.dictionary_0.Count);
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("  - 电量余额: ");
				defaultInterpolatedStringHandler2.AppendFormatted(authManager.decimal_0, "F2");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				Logger.Info("----------------------------------------");
				authManager.method_14();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[授权数据] 加载失败（已忽略）", ex);
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
	public struct Struct91 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<CombinedCreditsInfo> asyncTaskMethodBuilder_0;

		public Guid? nullable_0;

		public Guid? nullable_1;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<CombinedCreditsInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			CombinedCreditsInfo result2;
			try
			{
				TaskAwaiter<Result<CombinedCreditsInfo>> awaiter;
				if (num != 0)
				{
					_ = nullable_0.HasValue;
					if (nullable_0?.ToString() == null)
					{
						nullable_1?.ToString();
					}
					awaiter = authManager.isupabaseClient_0.GetUserCreditsAsync(nullable_0, nullable_1).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<CombinedCreditsInfo>>);
					num = -1;
					int_0 = -1;
				}
				Result<CombinedCreditsInfo> result = awaiter.GetResult();
				if (result.IsSuccess && result.Value != null)
				{
					result2 = result.Value;
				}
				else
				{
					Logger.Warning("[电量信息] 查询失败: " + result.Error);
					result2 = null;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[电量信息] 查询失败", ex);
				result2 = null;
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
	public struct Struct92 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private CombinedCreditsInfo combinedCreditsInfo_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<CombinedCreditsInfo?> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Expected O, but got Unknown
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Expected O, but got Unknown
			int num = int_0;
			AuthManager authManager = authManager_0;
			try
			{
				TaskAwaiter awaiter2;
				TaskAwaiter<CombinedCreditsInfo> awaiter;
				CombinedCreditsInfo result;
				Guid? nullable_;
				CombinedCreditsInfo result2;
				CombinedCreditsInfo obj;
				decimal num2;
				CombinedCreditsInfo obj2;
				decimal num3;
				decimal num4;
				decimal num5;
				CombinedCreditsInfo obj3;
				decimal num6;
				decimal num7;
				decimal totalBalance;
				CombinedCreditsInfo result3;
				switch (num)
				{
				default:
					Logger.Info("----------------------------------------");
					Logger.Info("开始加载许可证和电量余额（不加载试用记录）...");
					if (authManager.deviceInfo_0 != null)
					{
						if (authManager.deviceInfo_0.IsBound && authManager.deviceInfo_0.UserId.HasValue)
						{
							Logger.Info("[授权数据] 设备已绑定用户，加载用户授权数据");
							if (authManager.deviceInfo_0.UserId.HasValue)
							{
								awaiter2 = authManager.method_7(authManager.deviceInfo_0.UserId.Value).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 0;
									int_0 = 0;
									taskAwaiter_0 = awaiter2;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
								goto IL_017b;
							}
							goto IL_0182;
						}
						Logger.Info("[授权数据] 设备未绑定用户，加载设备授权数据");
						awaiter2 = authManager.method_8(authManager.deviceInfo_0.DeviceId).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 5;
							int_0 = 5;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_04c6;
					}
					Logger.Warning("[授权数据] 设备信息为空，跳过加载");
					goto end_IL_000f;
				case 0:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_017b;
				case 1:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_01fb;
				case 2:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<CombinedCreditsInfo>);
					num = -1;
					int_0 = -1;
					goto IL_029a;
				case 3:
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<CombinedCreditsInfo>);
					num = -1;
					int_0 = -1;
					goto IL_031f;
				case 4:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_0496;
				case 5:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_04c6;
				case 6:
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<CombinedCreditsInfo>);
						num = -1;
						int_0 = -1;
						goto IL_0541;
					}
					IL_0202:
					combinedCreditsInfo_0 = null;
					if (authManager.deviceInfo_0.UserId.HasValue)
					{
						awaiter = authManager.method_10(authManager.deviceInfo_0.UserId.Value).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_029a;
					}
					goto IL_02ab;
					IL_049d:
					combinedCreditsInfo_0 = null;
					break;
					IL_029a:
					result = awaiter.GetResult();
					combinedCreditsInfo_0 = result;
					goto IL_02ab;
					IL_02ab:
					nullable_ = authManager.deviceInfo_0.Id;
					awaiter = authManager.method_10(null, nullable_).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 3;
						int_0 = 3;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_031f;
					IL_04c6:
					awaiter2.GetResult();
					nullable_ = authManager.deviceInfo_0.Id;
					awaiter = authManager.method_10(null, nullable_).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 6;
						int_0 = 6;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0541;
					IL_031f:
					result2 = awaiter.GetResult();
					obj = combinedCreditsInfo_0;
					num2 = ((obj != null) ? obj.TotalPurchased : 0m);
					obj2 = combinedCreditsInfo_0;
					num3 = ((obj2 != null) ? obj2.TotalConsumed : 0m);
					num4 = ((result2 != null) ? result2.TotalPurchased : 0m);
					num5 = ((result2 != null) ? result2.TotalConsumed : 0m);
					obj3 = combinedCreditsInfo_0;
					num6 = ((obj3 != null) ? obj3.UserBalance : 0m);
					num7 = ((result2 != null) ? result2.DeviceBalance : 0m);
					totalBalance = (authManager.decimal_0 = num6 + num7);
					authManager.combinedCreditsInfo_0 = new CombinedCreditsInfo
					{
						TotalBalance = totalBalance,
						UserBalance = num6,
						DeviceBalance = num7,
						TotalPurchased = num2 + num4,
						TotalConsumed = num3 + num5,
						UserPurchased = num2,
						UserConsumed = num3,
						DevicePurchased = num4,
						DeviceConsumed = num5
					};
					if (authManager.deviceInfo_0.UserId.HasValue)
					{
						awaiter2 = authManager.method_16(authManager.deviceInfo_0.UserId.Value).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 4;
							int_0 = 4;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_0496;
					}
					goto IL_049d;
					IL_017b:
					awaiter2.GetResult();
					goto IL_0182;
					IL_0182:
					if (!authManager.method_11())
					{
						Logger.Info("[授权数据] 用户许可证无效或过期，尝试查询设备许可证");
						awaiter2 = authManager.method_8(authManager.deviceInfo_0.DeviceId).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_0 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_01fb;
					}
					goto IL_0202;
					IL_0541:
					result3 = awaiter.GetResult();
					authManager.decimal_0 = ((result3 != null) ? result3.TotalBalance : 0m);
					authManager.combinedCreditsInfo_0 = new CombinedCreditsInfo
					{
						TotalBalance = ((result3 != null) ? result3.TotalBalance : 0m),
						UserBalance = 0m,
						DeviceBalance = ((result3 != null) ? result3.DeviceBalance : 0m),
						TotalPurchased = ((result3 != null) ? result3.TotalPurchased : 0m),
						TotalConsumed = ((result3 != null) ? result3.TotalConsumed : 0m),
						UserPurchased = 0m,
						UserConsumed = 0m,
						DevicePurchased = ((result3 != null) ? result3.DevicePurchased : 0m),
						DeviceConsumed = ((result3 != null) ? result3.DeviceConsumed : 0m)
					};
					break;
					IL_0496:
					awaiter2.GetResult();
					goto IL_049d;
					IL_01fb:
					awaiter2.GetResult();
					goto IL_0202;
				}
				Logger.Info("----------------------------------------");
				Logger.Info("许可证和电量余额加载完成");
				Logger.Info("  - 许可证状态: " + ((authManager.licenseInfo_0 == null || !authManager.licenseInfo_0.IsValid()) ? "无/过期" : "有效"));
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("  - 电量余额: ");
				defaultInterpolatedStringHandler.AppendFormatted(authManager.decimal_0, "F2");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(24, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("  - 试用记录数: ");
				defaultInterpolatedStringHandler2.AppendFormatted(authManager.dictionary_0.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" (使用缓存，未重新加载)");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				Logger.Info("----------------------------------------");
				authManager.method_14();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[授权数据] 加载许可证和电量余额失败（已忽略）", ex);
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
	public struct Struct93 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public string string_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<LicenseInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			try
			{
				TaskAwaiter<Result<LicenseInfo>> awaiter;
				if (num != 0)
				{
					Logger.Info("[许可证] 查询设备许可证: " + string_0.Substring(0, 8) + "...");
					awaiter = authManager.isupabaseClient_0.GetLicenseByDeviceAsync(string_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<LicenseInfo>>);
					num = -1;
					int_0 = -1;
				}
				Result<LicenseInfo> result = awaiter.GetResult();
				string text;
				string licenseType;
				string text2;
				object obj;
				if (result.IsSuccess && result.Value != null)
				{
					authManager.licenseInfo_0 = result.Value;
					text = "[许可证] ✓ 找到设备许可证: ";
					licenseType = authManager.licenseInfo_0.LicenseType;
					text2 = ", 到期: ";
					DateTime? validTo = authManager.licenseInfo_0.ValidTo;
					if (!validTo.HasValue)
					{
						obj = null;
					}
					else
					{
						obj = validTo.GetValueOrDefault().ToString("yyyy-MM-dd");
						if (obj != null)
						{
							goto IL_0126;
						}
					}
					obj = "永久";
					goto IL_0126;
				}
				Logger.Info("[许可证] 未找到设备许可证");
				goto end_IL_000f;
				IL_0126:
				Logger.Info(text + licenseType + text2 + (string?)obj);
				if (!authManager.licenseInfo_0.IsValid())
				{
					Logger.Warning("[许可证] ⚠️ 许可证已过期");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[许可证] 查询失败", ex);
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
	public struct Struct94 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public Guid guid_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<LicenseInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			try
			{
				TaskAwaiter<Result<LicenseInfo>> awaiter;
				if (num != 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[许可证] 查询用户许可证: ");
					defaultInterpolatedStringHandler.AppendFormatted(guid_0);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					awaiter = authManager.isupabaseClient_0.GetLicenseInfoAsync(guid_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<LicenseInfo>>);
					num = -1;
					int_0 = -1;
				}
				Result<LicenseInfo> result = awaiter.GetResult();
				string text;
				string licenseType;
				string text2;
				object obj;
				if (result.IsSuccess && result.Value != null)
				{
					authManager.licenseInfo_0 = result.Value;
					text = "[许可证] ✓ 找到用户许可证: ";
					licenseType = authManager.licenseInfo_0.LicenseType;
					text2 = ", 到期: ";
					DateTime? validTo = authManager.licenseInfo_0.ValidTo;
					if (!validTo.HasValue)
					{
						obj = null;
					}
					else
					{
						obj = validTo.GetValueOrDefault().ToString("yyyy-MM-dd");
						if (obj != null)
						{
							goto IL_0131;
						}
					}
					obj = "永久";
					goto IL_0131;
				}
				Logger.Info("[许可证] 未找到用户许可证");
				goto end_IL_000f;
				IL_0131:
				Logger.Info(text + licenseType + text2 + (string?)obj);
				if (!authManager.licenseInfo_0.IsValid())
				{
					Logger.Warning("[许可证] ⚠️ 许可证已过期");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[许可证] 查询失败", ex);
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
	public struct Struct95 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<List<TrialLimitConfig>>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			try
			{
				TaskAwaiter<Result<List<TrialLimitConfig>>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<List<TrialLimitConfig>>>);
					num = -1;
					int_0 = -1;
					goto IL_008c;
				}
				authManager.method_17();
				if (!authManager.bool_2 || authManager.list_0.Count <= 0)
				{
					awaiter = authManager.isupabaseClient_0.GetAllTrialLimitsAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_008c;
				}
				goto end_IL_000f;
				IL_008c:
				Result<List<TrialLimitConfig>> result = awaiter.GetResult();
				if (result.IsSuccess && result.Value != null)
				{
					authManager.list_0 = result.Value;
					authManager.bool_2 = true;
					authManager.dictionary_1.Clear();
					authManager.dictionary_2.Clear();
					List<TrialLimitConfig>.Enumerator enumerator = authManager.list_0.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							TrialLimitConfig current = enumerator.Current;
							if (current.IsFeatureConfig && !string.IsNullOrEmpty(current.FeatureId))
							{
								authManager.dictionary_1[current.FeatureId] = current.MaxUsageCount;
							}
							else if (current.IsGroupConfig && !string.IsNullOrEmpty(current.FeatureGroup))
							{
								authManager.dictionary_2[current.FeatureGroup] = current.MaxUsageCount;
							}
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
						}
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] ✓ 构建快速查找缓存：");
					defaultInterpolatedStringHandler.AppendFormatted(authManager.dictionary_1.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个功能配置，");
					defaultInterpolatedStringHandler.AppendFormatted(authManager.dictionary_2.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个功能组配置");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					var (flag, list) = FeatureGroupConfig.ValidateConfiguration(authManager.list_0);
					if (!flag)
					{
						Logger.Warning("[AuthManager] ⚠️ 试用配置验证发现问题:");
						List<string>.Enumerator enumerator2 = list.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								string current2 = enumerator2.Current;
								Logger.Warning("[AuthManager]   - " + current2);
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
							}
						}
						Logger.Warning("[AuthManager] 建议：检查数据库 max_trial_limits 表中的 feature_group 配置");
					}
					else
					{
						Logger.Info("[AuthManager] ✓ 试用配置验证通过");
					}
				}
				else
				{
					Logger.Warning("[AuthManager] 加载试用配置失败: " + (result.Error ?? "未知错误") + "，将使用默认功能列表");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[AuthManager] 加载试用配置异常（已忽略）", ex);
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
	public struct Struct96 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		public string string_0;

		private TaskAwaiter<Result<List<TrialRecordDetail>>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			try
			{
				TaskAwaiter<Result<List<TrialRecordDetail>>> awaiter;
				if (num != 0)
				{
					awaiter = authManager.isupabaseClient_0.GetTrialRecordsByDeviceAsync(string_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<List<TrialRecordDetail>>>);
					num = -1;
					int_0 = -1;
				}
				Result<List<TrialRecordDetail>> result = awaiter.GetResult();
				if (result.IsSuccess && result.Value != null)
				{
					var enumerable = from trialRecordDetail_0 in result.Value
						group trialRecordDetail_0 by trialRecordDetail_0.FeatureId into igrouping_0
						select new
						{
							FeatureId = igrouping_0.Key,
							UsageCount = igrouping_0.Max((TrialRecordDetail trialRecordDetail_0) => trialRecordDetail_0.UsageCount)
						};
					authManager.dictionary_0.Clear();
					var enumerator = enumerable.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							var current = enumerator.Current;
							authManager.dictionary_0[current.FeatureId] = current.UsageCount;
						}
					}
					finally
					{
						if (num < 0)
						{
							enumerator?.Dispose();
						}
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[试用记录] 已加载 ");
					defaultInterpolatedStringHandler.AppendFormatted(enumerable.Count());
					defaultInterpolatedStringHandler.AppendLiteral(" 个功能的试用记录");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					Logger.Warning("[试用记录] 查询失败（网络异常）: " + result.Error + "，清空试用记录缓存并标记网络不可用");
					authManager.dictionary_0.Clear();
					authManager.bool_4 = false;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[试用记录] 查询失败（网络错误），清空试用记录缓存并标记网络不可用", ex);
				authManager.dictionary_0.Clear();
				authManager.bool_4 = false;
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
	public struct Struct97 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		public Guid guid_0;

		private TaskAwaiter<Result<List<DeviceInfo>>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			try
			{
				TaskAwaiter<Result<List<DeviceInfo>>> awaiter;
				if (num != 0)
				{
					awaiter = authManager.isupabaseClient_0.GetUserDevicesAsync(guid_0).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<List<DeviceInfo>>>);
					num = -1;
					int_0 = -1;
				}
				Result<List<DeviceInfo>> result = awaiter.GetResult();
				if (result.IsSuccess && result.Value != null)
				{
					authManager.list_1 = result.Value;
				}
				else
				{
					Logger.Warning("[用户设备] 查询失败: " + result.Error);
					authManager.list_1 = null;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[用户设备] 查询失败", ex);
				authManager.list_1 = null;
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
	public struct Struct98 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private TaskAwaiter<(string? PublicIp, string? LocalIp)> taskAwaiter_0;

		private TaskAwaiter<Result<MigrateDeviceResult>> taskAwaiter_1;

		private TaskAwaiter<Result<DeviceInfo>> taskAwaiter_2;

		private TaskAwaiter<Result> taskAwaiter_3;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result2;
			try
			{
				TaskAwaiter<(string, string)> awaiter4;
				TaskAwaiter<Result<MigrateDeviceResult>> awaiter3;
				TaskAwaiter<Result<DeviceInfo>> awaiter2;
				TaskAwaiter<Result> awaiter;
				Result<MigrateDeviceResult> result;
				string error;
				(string, string) result3;
				string item;
				string item2;
				Result<DeviceInfo> result4;
				switch (num)
				{
				default:
				{
					Logger.Info("[硬件指纹迁移] ========== 开始迁移 ==========");
					string_0 = authManager.inativeCryptoService_0.GetHardwareFingerprintV1();
					string_1 = authManager.inativeCryptoService_0.GetHardwareFingerprintV2();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[硬件指纹迁移] 旧ID: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0.Substring(0, 8));
					defaultInterpolatedStringHandler.AppendLiteral("... -> 新ID: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_1.Substring(0, 8));
					defaultInterpolatedStringHandler.AppendLiteral("...");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					awaiter4 = authManager.ipAddressService_0.GetIpAddressesAsync().GetAwaiter();
					if (!awaiter4.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter4;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref this);
						return;
					}
					goto IL_012b;
				}
				case 0:
					awaiter4 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<(string, string)>);
					num = -1;
					int_0 = -1;
					goto IL_012b;
				case 1:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<MigrateDeviceResult>>);
					num = -1;
					int_0 = -1;
					goto IL_01d8;
				case 2:
					awaiter2 = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<Result<DeviceInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_050e;
				case 3:
					awaiter2 = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<Result<DeviceInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_0538;
				case 4:
					awaiter2 = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<Result<DeviceInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_0562;
				case 5:
					{
						awaiter = taskAwaiter_3;
						taskAwaiter_3 = default(TaskAwaiter<Result>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_01d8:
					result = awaiter3.GetResult();
					if (!result.IsFailure)
					{
						MigrateDeviceResult value = result.Value;
						if (value == null)
						{
							Logger.Warning("[硬件指纹迁移] 迁移结果为空，使用新设备ID注册");
							authManager.deviceInfo_0 = new DeviceInfo
							{
								DeviceId = string_1,
								DeviceName = Environment.MachineName,
								OsVersion = Environment.OSVersion.ToString(),
								IsActive = true,
								IpAddress = string_2
							};
							authManager.method_13(authManager.deviceInfo_0, authManager.string_0);
							awaiter2 = authManager.isupabaseClient_0.RegisterDeviceAsync(authManager.deviceInfo_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 3;
								int_0 = 3;
								taskAwaiter_2 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_0538;
						}
						Logger.Info("[硬件指纹迁移] 迁移成功:");
						Logger.Info("  - 新设备ID: " + value.NewDeviceId?.Substring(0, 8) + "...");
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(10, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("  - 复制电量: ");
						defaultInterpolatedStringHandler2.AppendFormatted(value.CopiedCredits);
						Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(14, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("  - 复制试用记录: ");
						defaultInterpolatedStringHandler3.AppendFormatted(value.CopiedTrialRecords);
						defaultInterpolatedStringHandler3.AppendLiteral(" 条");
						Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("  - 旧设备已解绑: ");
						defaultInterpolatedStringHandler4.AppendFormatted(value.OldDeviceUnbound);
						Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
						awaiter2 = authManager.isupabaseClient_0.GetDeviceInfoAsync(string_1).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 4;
							int_0 = 4;
							taskAwaiter_2 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_0562;
					}
					error = result.Error;
					if (error != null && error.Contains("旧设备不存在"))
					{
						Logger.Info("[硬件指纹迁移] 旧设备不存在，使用新设备ID注册");
						authManager.deviceInfo_0 = new DeviceInfo
						{
							DeviceId = string_1,
							DeviceName = Environment.MachineName,
							OsVersion = Environment.OSVersion.ToString(),
							IsActive = true,
							IpAddress = string_2
						};
						authManager.method_13(authManager.deviceInfo_0, authManager.string_0);
						awaiter2 = authManager.isupabaseClient_0.RegisterDeviceAsync(authManager.deviceInfo_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_2 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_050e;
					}
					Logger.Error("[硬件指纹迁移] 迁移失败: " + result.Error);
					result2 = Result.Failure("迁移失败: " + result.Error);
					goto end_IL_000f;
					IL_012b:
					result3 = awaiter4.GetResult();
					item = result3.Item1;
					item2 = result3.Item2;
					string_2 = item ?? item2 ?? "Unknown";
					awaiter3 = authManager.isupabaseClient_0.MigrateDeviceIdCopyAsync(string_0, string_1, Environment.MachineName, Environment.OSVersion.ToString(), string_2).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter3;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
						return;
					}
					goto IL_01d8;
					IL_05e3:
					awaiter = authManager.iconfigManager_0.SetAsync("HardwareFingerprintVersion", "2.0").GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 5;
						int_0 = 5;
						taskAwaiter_3 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					break;
					IL_0562:
					result4 = awaiter2.GetResult();
					if (result4.IsSuccess && result4.Value != null)
					{
						authManager.deviceInfo_0 = result4.Value;
					}
					else
					{
						authManager.deviceInfo_0 = new DeviceInfo
						{
							DeviceId = string_1,
							DeviceName = Environment.MachineName,
							OsVersion = Environment.OSVersion.ToString(),
							IsActive = true,
							IpAddress = string_2
						};
						authManager.method_13(authManager.deviceInfo_0, authManager.string_0);
					}
					goto IL_05e3;
					IL_050e:
					awaiter2.GetResult();
					goto IL_05e3;
					IL_0538:
					awaiter2.GetResult();
					goto IL_05e3;
				}
				awaiter.GetResult();
				Logger.Info("[硬件指纹迁移] 版本标记已更新: 2.0");
				Logger.Info("[硬件指纹迁移] ========== 迁移完成 ==========");
				result2 = Result.Success();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[硬件指纹迁移] 迁移失败", ex);
				result2 = Result.Failure("迁移失败: " + ex.Message);
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
	public struct Struct99 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		private TaskAwaiter<Result<bool>> taskAwaiter_1;

		private TaskAwaiter<Result<string>> taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			bool result;
			try
			{
				TaskAwaiter<Result> awaiter3;
				TaskAwaiter<Result<bool>> awaiter2;
				TaskAwaiter<Result<string>> awaiter;
				Result<bool> result2;
				switch (num)
				{
				default:
					if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "config.json")))
					{
						awaiter3 = authManager.iconfigManager_0.SetAsync("HardwareFingerprintVersion", "2.0").GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter3;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
							return;
						}
						goto IL_0104;
					}
					awaiter2 = authManager.iconfigManager_0.ExistsAsync("HardwareFingerprintVersion").GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
					goto IL_013f;
				case 0:
					awaiter3 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_0104;
				case 1:
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result<bool>>);
					num = -1;
					int_0 = -1;
					goto IL_013f;
				case 2:
					{
						awaiter = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter<Result<string>>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0104:
					awaiter3.GetResult();
					Logger.Info("[硬件指纹] 新安装（配置文件不存在），跳过迁移，直接标记为 v2.0");
					result = false;
					goto end_IL_000f;
					IL_013f:
					result2 = awaiter2.GetResult();
					if (result2.IsSuccess && result2.Value)
					{
						awaiter = authManager.iconfigManager_0.GetAsync("HardwareFingerprintVersion", "1.0").GetAwaiter();
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
					Logger.Info("[硬件指纹] 旧版本升级（配置文件存在但无版本标记），需要迁移");
					result = true;
					goto end_IL_000f;
				}
				string text = awaiter.GetResult().Value ?? "1.0";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[硬件指纹] 当前版本标记: ");
				defaultInterpolatedStringHandler.AppendFormatted(text);
				defaultInterpolatedStringHandler.AppendLiteral("，需要迁移: ");
				defaultInterpolatedStringHandler.AppendFormatted(text != "2.0");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				result = text != "2.0";
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Warning("[硬件指纹] 检查版本标记失败，假设需要迁移: " + ex.Message);
				result = true;
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
	public struct Struct100 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public AuthManager authManager_0;

		public FeatureGroup? nullable_0;

		private int int_1;

		private TaskAwaiter<Result<int>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter<Result<int>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<int>>);
					num = -1;
					int_0 = -1;
					goto IL_02bd;
				}
				if (string.IsNullOrEmpty(string_0))
				{
					result = Result.Failure("功能 ID 不能为空");
				}
				else if (!authManager.IsOfflineMode && authManager.deviceInfo_0 != null)
				{
					if (authManager.dictionary_3.Count == 0)
					{
						authManager.method_17();
					}
					string text = null;
					if (nullable_0.HasValue)
					{
						text = FeatureGroupExtensions.GetIdentifier(nullable_0.Value);
					}
					else if (authManager.dictionary_3.ContainsKey(string_0))
					{
						text = authManager.dictionary_3[string_0];
					}
					else
					{
						Logger.Warning("[AuthManager] 功能组映射表中未找到功能: " + string_0 + "，尝试动态构建...");
						authManager.method_17();
						if (authManager.dictionary_3.ContainsKey(string_0))
						{
							text = authManager.dictionary_3[string_0];
							Logger.Info("[AuthManager] 动态构建后找到功能组: " + string_0 + " -> " + text);
						}
						else
						{
							Logger.Warning("[AuthManager] 功能组映射表中未找到功能: " + string_0 + "，group=null");
						}
					}
					if (text == "core")
					{
						result = Result.Success();
					}
					else
					{
						int_1 = authManager.method_1(string_0);
						if (int_1 > 0)
						{
							Guid? userId = null;
							if (authManager.deviceInfo_0.IsBound && authManager.deviceInfo_0.UserId.HasValue)
							{
								userId = authManager.deviceInfo_0.UserId.Value;
							}
							awaiter = authManager.isupabaseClient_0.RecordTrialUsageAsync(authManager.deviceInfo_0.DeviceId, string_0, userId, text).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							goto IL_02bd;
						}
						Logger.Warning("[AuthManager] 功能 " + string_0 + " 未配置，禁止使用");
						result = Result.Failure("功能 " + string_0 + " 未配置试用次数");
					}
				}
				else
				{
					if (!authManager.IsOfflineMode && authManager.deviceInfo_0 != null)
					{
						goto IL_03f8;
					}
					Logger.Warning("[AuthManager] 离线模式，不记录功能使用: " + string_0);
					result = Result.Success();
				}
				goto end_IL_000f;
				IL_03f8:
				result = Result.Success();
				goto end_IL_000f;
				IL_02bd:
				Result<int> result2 = awaiter.GetResult();
				if (result2.IsSuccess)
				{
					int value = result2.Value;
					int value2 = ((value >= 0) ? (int_1 - value) : (int_1 + 1));
					authManager.dictionary_0[string_0] = value2;
					authManager.method_14();
				}
				else
				{
					Logger.Warning("[AuthManager] 记录功能使用失败: " + result2.Error + "，使用本地配置计算");
					int value3 = (authManager.dictionary_0.ContainsKey(string_0) ? authManager.dictionary_0[string_0] : 0) + 1;
					authManager.dictionary_0[string_0] = value3;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 3);
					defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] ✓ 使用本地配置更新缓存: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral(" -> 已使用 ");
					defaultInterpolatedStringHandler.AppendFormatted(value3);
					defaultInterpolatedStringHandler.AppendLiteral(" 次（配置最大值: ");
					defaultInterpolatedStringHandler.AppendFormatted(int_1);
					defaultInterpolatedStringHandler.AppendLiteral("）");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					authManager.method_14();
				}
				goto IL_03f8;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 记录功能使用异常: " + string_0, ex);
				result = Result.Failure("记录功能使用异常：" + ex.Message);
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
	public struct Struct101 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					Logger.Info("[AuthManager] 开始刷新授权数据...");
					awaiter = authManager.method_5().GetAwaiter();
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
				Logger.Info("[AuthManager] 授权数据刷新完成");
				result = Result.Success();
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 刷新授权数据失败", ex);
				result = Result.Failure("刷新授权数据失败：" + ex.Message);
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
	public struct Struct102 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<CombinedCreditsInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			try
			{
				TaskAwaiter<Result<CombinedCreditsInfo>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<CombinedCreditsInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_00ea;
				}
				Logger.Info("[AuthManager] 刷新电量缓存...");
				Guid? userId = authManager.userIdentity_0?.UserId;
				Guid? deviceId = authManager.deviceInfo_0?.Id;
				if (userId.HasValue || deviceId.HasValue)
				{
					awaiter = authManager.isupabaseClient_0.GetUserCreditsAsync(userId, deviceId).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00ea;
				}
				Logger.Warning("[AuthManager] 无法刷新电量缓存：用户和设备信息均为空");
				goto end_IL_000f;
				IL_00ea:
				Result<CombinedCreditsInfo> result = awaiter.GetResult();
				if (result.IsSuccess && result.Value != null)
				{
					CombinedCreditsInfo val = (authManager.combinedCreditsInfo_0 = result.Value);
					authManager.decimal_0 = val.TotalBalance;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 3);
					defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] ✅ 电量缓存已刷新: ");
					defaultInterpolatedStringHandler.AppendLiteral("总余额=");
					defaultInterpolatedStringHandler.AppendFormatted(val.TotalBalance, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendLiteral("总购买=");
					defaultInterpolatedStringHandler.AppendFormatted(val.TotalPurchased, "F2");
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendLiteral("总消耗=");
					defaultInterpolatedStringHandler.AppendFormatted(val.TotalConsumed, "F2");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					Logger.Warning("[AuthManager] 刷新电量缓存失败: " + result.Error);
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 刷新电量缓存异常", ex);
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
	public struct Struct103 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<DeviceInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter<Result<DeviceInfo>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<DeviceInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_0096;
				}
				if (authManager.deviceInfo_0 != null)
				{
					awaiter = authManager.isupabaseClient_0.GetDeviceInfoAsync(authManager.deviceInfo_0.DeviceId).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0096;
				}
				result = Result.Failure("设备信息不可用");
				goto end_IL_000f;
				IL_0096:
				Result<DeviceInfo> result2 = awaiter.GetResult();
				if (result2.IsSuccess && result2.Value != null)
				{
					DeviceInfo value = result2.Value;
					authManager.deviceInfo_0.IsBound = value.IsBound;
					authManager.deviceInfo_0.UserId = value.UserId;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] 设备信息已刷新: IsBound=");
					defaultInterpolatedStringHandler.AppendFormatted(authManager.deviceInfo_0.IsBound);
					defaultInterpolatedStringHandler.AppendLiteral(", UserId=");
					defaultInterpolatedStringHandler.AppendFormatted(authManager.deviceInfo_0.UserId);
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					result = Result.Success();
				}
				else
				{
					result = Result.Failure(result2.Error ?? "获取设备信息失败");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 刷新设备信息失败", ex);
				result = Result.Failure("刷新设备信息异常：" + ex.Message);
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
	public struct Struct104 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					Logger.Info("[AuthManager] 手动刷新试用配置缓存...");
					authManager.bool_2 = false;
					authManager.list_0.Clear();
					awaiter = authManager.method_18().GetAwaiter();
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
				result = ((authManager.list_0.Count > 0) ? Result.Success() : Result.Failure("配置缓存为空"));
			}
			catch (Exception ex)
			{
				result = Result.Failure("刷新配置缓存失败: " + ex.Message);
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
	public struct Struct105 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<IDeviceInfo>> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<DeviceInfo>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result<IDeviceInfo> result;
			try
			{
				TaskAwaiter<Result<DeviceInfo>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<DeviceInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_0091;
				}
				if (authManager.deviceInfo_0 != null)
				{
					awaiter = authManager.isupabaseClient_0.RegisterDeviceAsync(authManager.deviceInfo_0).GetAwaiter();
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
				result = Result<IDeviceInfo>.Failure("设备信息不可用");
				goto end_IL_000f;
				IL_0091:
				Result<DeviceInfo> result2 = awaiter.GetResult();
				if (result2.IsSuccess)
				{
					authManager.deviceInfo_0 = result2.Value;
					result = Result<IDeviceInfo>.Success((IDeviceInfo)(object)authManager.deviceInfo_0);
				}
				else
				{
					result = Result<IDeviceInfo>.Failure(result2.Error ?? "注册设备失败");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result<IDeviceInfo>.Failure("注册设备异常：" + ex.Message);
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
	public struct Struct106 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private Task<Result<DeviceInfo>> task_0;

		private Task<Result<DeviceInfo>> task_1;

		private TaskAwaiter<(string? PublicIp, string? LocalIp)> taskAwaiter_0;

		private TaskAwaiter<bool> taskAwaiter_1;

		private TaskAwaiter<Result> taskAwaiter_2;

		private TaskAwaiter<Task> taskAwaiter_3;

		private TaskAwaiter<Result<DeviceInfo>> taskAwaiter_4;

		private TaskAwaiter taskAwaiter_5;

		private TaskAwaiter<Result<DeviceCreditsResult>> taskAwaiter_6;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter<(string, string)> awaiter6;
				TaskAwaiter<bool> awaiter5;
				TaskAwaiter<Result> awaiter4;
				string revitVersion = default(string);
				string ipAddress = default(string);
				TaskAwaiter<Task> awaiter3;
				TaskAwaiter<Result<DeviceInfo>> awaiter2;
				TaskAwaiter awaiter;
				Result<DeviceInfo> result2;
				string text;
				string text2;
				Task task;
				Result<DeviceInfo> result4;
				Result result6;
				Task task2;
				switch (num)
				{
				default:
					Logger.Info("开始设备注册/查找流程...");
					if (authManager.deviceInfo_0 != null)
					{
						awaiter6 = authManager.ipAddressService_0.GetIpAddressesAsync().GetAwaiter();
						if (!awaiter6.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter6;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter6, ref this);
							return;
						}
						goto IL_00e0;
					}
					Logger.Error("当前设备信息为空，无法注册设备");
					result = Result.Failure("设备信息未初始化");
					goto end_IL_000f;
				case 0:
					awaiter6 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<(string, string)>);
					num = -1;
					int_0 = -1;
					goto IL_00e0;
				case 1:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<bool>);
					num = -1;
					int_0 = -1;
					goto IL_016c;
				case 2:
					awaiter4 = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_01de;
				case 3:
					awaiter3 = taskAwaiter_3;
					taskAwaiter_3 = default(TaskAwaiter<Task>);
					num = -1;
					int_0 = -1;
					goto IL_02a3;
				case 4:
					awaiter2 = taskAwaiter_4;
					taskAwaiter_4 = default(TaskAwaiter<Result<DeviceInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_032c;
				case 5:
					try
					{
						if (num != 5)
						{
							awaiter2 = authManager.isupabaseClient_0.UpdateDeviceAsync(authManager.deviceInfo_0.DeviceId ?? string.Empty, revitVersion, ipAddress).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 5;
								int_0 = 5;
								taskAwaiter_4 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = taskAwaiter_4;
							taskAwaiter_4 = default(TaskAwaiter<Result<DeviceInfo>>);
							num = -1;
							int_0 = -1;
						}
						Result<DeviceInfo> result3 = awaiter2.GetResult();
						if (result3.IsSuccess && result3.Value != null)
						{
							authManager.deviceInfo_0 = result3.Value;
							Logger.Info("✓ 设备信息已更新");
						}
						else
						{
							Logger.Warning("更新设备信息失败: " + (result3.Error ?? "未知错误"));
						}
					}
					catch (Exception ex)
					{
						Logger.Error("更新设备信息异常: " + ex.Message);
					}
					goto case 6;
				case 6:
					try
					{
						if (num != 6)
						{
							awaiter = authManager.method_18().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 6;
								int_0 = 6;
								taskAwaiter_5 = awaiter;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = taskAwaiter_5;
							taskAwaiter_5 = default(TaskAwaiter);
							num = -1;
							int_0 = -1;
						}
						awaiter.GetResult();
					}
					catch (Exception ex3)
					{
						Logger.Warning("加载试用限制配置失败（已忽略）: " + ex3.Message);
					}
					awaiter = authManager.method_5().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 7;
						int_0 = 7;
						taskAwaiter_5 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0635;
				case 7:
					awaiter = taskAwaiter_5;
					taskAwaiter_5 = default(TaskAwaiter);
					num = -1;
					int_0 = -1;
					goto IL_0635;
				case 8:
					awaiter3 = taskAwaiter_3;
					taskAwaiter_3 = default(TaskAwaiter<Task>);
					num = -1;
					int_0 = -1;
					goto IL_0673;
				case 9:
					awaiter2 = taskAwaiter_4;
					taskAwaiter_4 = default(TaskAwaiter<Result<DeviceInfo>>);
					num = -1;
					int_0 = -1;
					goto IL_06ec;
				case 10:
				case 11:
					try
					{
						TaskAwaiter<Result<DeviceCreditsResult>> awaiter7;
						if (num != 10)
						{
							if (num == 11)
							{
								awaiter7 = taskAwaiter_6;
								taskAwaiter_6 = default(TaskAwaiter<Result<DeviceCreditsResult>>);
								num = -1;
								int_0 = -1;
								goto IL_085b;
							}
							awaiter = authManager.method_18().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 10;
								int_0 = 10;
								taskAwaiter_5 = awaiter;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = taskAwaiter_5;
							taskAwaiter_5 = default(TaskAwaiter);
							num = -1;
							int_0 = -1;
						}
						awaiter.GetResult();
						if (authManager.deviceInfo_0.Id != Guid.Empty)
						{
							awaiter7 = authManager.isupabaseClient_0.InitializeDeviceCreditsAsync(authManager.deviceInfo_0.Id).GetAwaiter();
							if (!awaiter7.IsCompleted)
							{
								num = 11;
								int_0 = 11;
								taskAwaiter_6 = awaiter7;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter7, ref this);
								return;
							}
							goto IL_085b;
						}
						Logger.Warning("设备 ID 为空，跳过电量余额初始化");
						goto end_IL_0769;
						IL_085b:
						Result<DeviceCreditsResult> result5 = awaiter7.GetResult();
						if (result5.IsSuccess && result5.Value != null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler.AppendLiteral("✓ 电量余额初始化完成: ");
							defaultInterpolatedStringHandler.AppendFormatted(result5.Value.CurrentBalance);
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							Logger.Warning("初始化电量余额失败: " + result5.Error);
						}
						end_IL_0769:;
					}
					catch (Exception ex2)
					{
						Logger.Warning("初始化配置或电量余额失败（已忽略）: " + ex2.Message);
					}
					awaiter = authManager.method_5().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 12;
						int_0 = 12;
						taskAwaiter_5 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					break;
				case 12:
					{
						awaiter = taskAwaiter_5;
						taskAwaiter_5 = default(TaskAwaiter);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_06ec:
					result2 = awaiter2.GetResult();
					if (result2.IsSuccess && result2.Value != null)
					{
						authManager.deviceInfo_0 = result2.Value;
						goto case 10;
					}
					Logger.Error("注册设备失败: " + (result2.Error ?? "未知错误"));
					goto IL_073f;
					IL_0635:
					awaiter.GetResult();
					Logger.Info("========================================");
					result = Result.Success();
					goto end_IL_000f;
					IL_0673:
					if (awaiter3.GetResult() == task_1)
					{
						awaiter2 = task_1.GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 9;
							int_0 = 9;
							taskAwaiter_4 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_06ec;
					}
					Logger.Warning("注册设备请求超时（5秒）");
					goto IL_073f;
					IL_00e0:
					(text, text2) = awaiter6.GetResult();
					authManager.deviceInfo_0.IpAddress = text ?? text2 ?? "Unknown";
					awaiter5 = authManager.NeedsHardwareFingerprintMigrationAsync().GetAwaiter();
					if (!awaiter5.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter5;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref this);
						return;
					}
					goto IL_016c;
					IL_03ee:
					task_1 = authManager.isupabaseClient_0.RegisterDeviceAsync(authManager.deviceInfo_0);
					task = Task.Delay(TimeSpan.FromSeconds(5L));
					awaiter3 = Task.WhenAny(task_1, task).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = 8;
						int_0 = 8;
						taskAwaiter_3 = awaiter3;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
						return;
					}
					goto IL_0673;
					IL_02a3:
					if (awaiter3.GetResult() == task_0)
					{
						awaiter2 = task_0.GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 4;
							int_0 = 4;
							taskAwaiter_4 = awaiter2;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_032c;
					}
					Logger.Warning("查找设备请求超时（5秒）");
					Logger.Info("将继续尝试注册新设备...");
					goto IL_03ee;
					IL_032c:
					result4 = awaiter2.GetResult();
					if (result4.IsSuccess && result4.Value != null)
					{
						authManager.deviceInfo_0 = result4.Value;
						Result val = authManager.method_12();
						if (!val.IsSuccess)
						{
							Logger.Warning("[AuthManager] 设备已被禁用: " + val.Error);
						}
						revitVersion = authManager.string_0 ?? "Unknown";
						ipAddress = authManager.deviceInfo_0.IpAddress ?? "Unknown";
						goto case 5;
					}
					Logger.Warning("查找设备失败: " + (result4.Error ?? "未知错误"));
					Logger.Info("将继续尝试注册新设备...");
					goto IL_03ee;
					IL_016c:
					if (awaiter5.GetResult())
					{
						Logger.Info("[AuthManager] 检测到硬件指纹 v1.0，需要迁移到 v2.0");
						awaiter4 = authManager.MigrateHardwareFingerprintAsync().GetAwaiter();
						if (!awaiter4.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_2 = awaiter4;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref this);
							return;
						}
						goto IL_01de;
					}
					goto IL_020b;
					IL_073f:
					Logger.Warning("步骤 3/3: 设备注册流程失败，使用默认试用次数");
					Logger.Info("========================================");
					result = Result.Success();
					goto end_IL_000f;
					IL_01de:
					result6 = awaiter4.GetResult();
					if (result6.IsFailure)
					{
						Logger.Warning("[AuthManager] 硬件指纹迁移失败: " + result6.Error);
					}
					goto IL_020b;
					IL_020b:
					task_0 = authManager.isupabaseClient_0.GetDeviceInfoAsync(authManager.deviceInfo_0.DeviceId ?? string.Empty);
					task2 = Task.Delay(TimeSpan.FromSeconds(5L));
					awaiter3 = Task.WhenAny(task_0, task2).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = 3;
						int_0 = 3;
						taskAwaiter_3 = awaiter3;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
						return;
					}
					goto IL_02a3;
				}
				awaiter.GetResult();
				Logger.Info("========================================");
				result = Result.Success();
				end_IL_000f:;
			}
			catch (OperationCanceledException)
			{
				Logger.Warning("设备注册操作被取消（5秒），使用默认试用次数: 10");
				Logger.Info("========================================");
				result = Result.Success();
			}
			catch (Exception ex5)
			{
				Logger.Error("设备注册异常", ex5);
				Logger.Info("========================================");
				result = Result.Success();
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
	public struct Struct107 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter<Result> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_0090;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					awaiter = authManager.isupabaseClient_0.RequestPasswordResetAsync(string_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0090;
				}
				result = Result.Failure("电子邮件地址不能为空");
				goto end_IL_000f;
				IL_0090:
				result = awaiter.GetResult();
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[AuthManager] 请求密码重置异常", ex);
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
	public struct Struct108 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter<Result> awaiter;
				if (num != 0)
				{
					authManager.networkStatusChecker_0.StopPeriodicCheck();
					awaiter = authManager.iconfigManager_0.SaveAsync().GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
				}
				awaiter.GetResult();
				authManager.bool_0 = false;
				result = Result.Success();
			}
			catch (Exception ex)
			{
				result = Result.Failure("关闭授权管理器失败：" + ex.Message);
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
	public struct Struct109 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<IUserIdentity>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public AuthManager authManager_0;

		private Result result_0;

		private TaskAwaiter<Result<UserIdentity>> taskAwaiter_0;

		private TaskAwaiter<Result> taskAwaiter_1;

		private TaskAwaiter taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result<IUserIdentity> result;
			try
			{
				TaskAwaiter<Result<UserIdentity>> awaiter3;
				TaskAwaiter<Result> awaiter2;
				TaskAwaiter awaiter;
				Result<UserIdentity> result2;
				switch (num)
				{
				default:
					if (!string.IsNullOrEmpty(string_0) && !string.IsNullOrEmpty(string_1))
					{
						awaiter3 = authManager.isupabaseClient_0.SignInAsync(string_0, string_1).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter3;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
							return;
						}
						goto IL_00b8;
					}
					result = Result<IUserIdentity>.Failure("电子邮件和密码不能为空");
					goto end_IL_000f;
				case 0:
					awaiter3 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<UserIdentity>>);
					num = -1;
					int_0 = -1;
					goto IL_00b8;
				case 1:
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_020a;
				case 2:
					{
						awaiter = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter);
						num = -1;
						int_0 = -1;
						goto IL_0250;
					}
					IL_00b8:
					result2 = awaiter3.GetResult();
					if (result2.IsSuccess)
					{
						authManager.userIdentity_0 = result2.Value;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] ✓ 用户登录成功: ");
						defaultInterpolatedStringHandler.AppendFormatted(authManager.userIdentity_0.Email);
						defaultInterpolatedStringHandler.AppendLiteral(", UserId=");
						defaultInterpolatedStringHandler.AppendFormatted(authManager.userIdentity_0.UserId);
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						result_0 = authManager.method_12();
						if (!result_0.IsSuccess)
						{
							awaiter2 = authManager.SignOutAsync().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_020a;
						}
						if (authManager.deviceInfo_0 == null)
						{
							break;
						}
						awaiter = authManager.method_0().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_2 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0250;
					}
					result = Result<IUserIdentity>.Failure(result2.Error ?? "登录失败");
					goto end_IL_000f;
					IL_020a:
					awaiter2.GetResult();
					result = Result<IUserIdentity>.Failure(result_0.Error ?? "设备已被禁用");
					goto end_IL_000f;
					IL_0250:
					awaiter.GetResult();
					break;
				}
				result = ((authManager.userIdentity_0 != null) ? Result<IUserIdentity>.Success((IUserIdentity)(object)authManager.userIdentity_0) : Result<IUserIdentity>.Failure("用户登录失败"));
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result<IUserIdentity>.Failure("登录异常：" + ex.Message);
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
	public struct Struct110 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result2;
			try
			{
				TaskAwaiter<Result> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<Result>);
						num = -1;
						int_0 = -1;
						goto IL_01a6;
					}
					if (authManager.deviceInfo_0 == null || !authManager.deviceInfo_0.IsBound || !authManager.deviceInfo_0.UserId.HasValue)
					{
						goto IL_0133;
					}
					awaiter = authManager.isupabaseClient_0.UnbindDeviceAsync(authManager.deviceInfo_0.DeviceId).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
				}
				Result result = awaiter.GetResult();
				if (result.IsSuccess)
				{
					Logger.Info("[AuthManager] 当前设备解绑成功");
					authManager.deviceInfo_0.IsBound = false;
					authManager.deviceInfo_0.UserId = null;
				}
				else
				{
					Logger.Warning("[AuthManager] 设备解绑失败: " + result.Error);
				}
				goto IL_0133;
				IL_01a6:
				awaiter.GetResult();
				result2 = Result.Success();
				goto end_IL_000f;
				IL_0133:
				authManager.userIdentity_0 = null;
				authManager.licenseInfo_0 = null;
				authManager.decimal_0 = 0m;
				authManager.combinedCreditsInfo_0 = null;
				authManager.list_1 = null;
				Logger.Info("[AuthManager] 已清除用户数据和电量缓存");
				awaiter = authManager.isupabaseClient_0.SignOutAsync().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 1;
					int_0 = 1;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_01a6;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result2 = Result.Failure("登出异常：" + ex.Message);
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
	public struct Struct111 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<IUserIdentity>> asyncTaskMethodBuilder_0;

		public string string_0;

		public string string_1;

		public AuthManager authManager_0;

		private TaskAwaiter<Result<UserIdentity>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result<IUserIdentity> result;
			try
			{
				TaskAwaiter<Result<UserIdentity>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<UserIdentity>>);
					num = -1;
					int_0 = -1;
					goto IL_00a9;
				}
				if (!string.IsNullOrEmpty(string_0) && !string.IsNullOrEmpty(string_1))
				{
					awaiter = authManager.isupabaseClient_0.SignUpAsync(string_0, string_1).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00a9;
				}
				result = Result<IUserIdentity>.Failure("电子邮件和密码不能为空");
				goto end_IL_000f;
				IL_00a9:
				Result<UserIdentity> result2 = awaiter.GetResult();
				if (result2.IsSuccess)
				{
					authManager.userIdentity_0 = result2.Value;
					result = Result<IUserIdentity>.Success((IUserIdentity)(object)authManager.userIdentity_0);
				}
				else
				{
					result = Result<IUserIdentity>.Failure(result2.Error ?? "注册失败");
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				result = Result<IUserIdentity>.Failure("注册异常：" + ex.Message);
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
	public struct Struct112 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result> asyncTaskMethodBuilder_0;

		public string string_0;

		public AuthManager authManager_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			AuthManager authManager = authManager_0;
			Result result;
			try
			{
				TaskAwaiter<Result> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result>);
					num = -1;
					int_0 = -1;
					goto IL_0096;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					awaiter = authManager.isupabaseClient_0.UnbindDeviceAsync(string_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0096;
				}
				result = Result.Failure("设备 ID 不能为空");
				goto end_IL_000f;
				IL_0096:
				Result result2 = awaiter.GetResult();
				if (!result2.IsSuccess)
				{
					result = Result.Failure(result2.Error ?? "解绑设备失败");
				}
				else
				{
					if (authManager.deviceInfo_0 != null && authManager.deviceInfo_0.DeviceId == string_0)
					{
						authManager.deviceInfo_0.IsBound = false;
					}
					result = Result.Success();
				}
				end_IL_000f:;
			}
			catch (Exception ex)
			{
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

	private readonly ISupabaseClient isupabaseClient_0;

	private readonly INativeCryptoService inativeCryptoService_0;

	private readonly IConfigManager iconfigManager_0;

	private readonly IpAddressService ipAddressService_0;

	private readonly NetworkStatusChecker networkStatusChecker_0;

	private UserIdentity? userIdentity_0;

	private DeviceInfo? deviceInfo_0;

	private LicenseInfo? licenseInfo_0;

	private bool bool_0;

	private bool bool_1;

	private string? string_0;

	private Dictionary<string, int> dictionary_0 = new Dictionary<string, int>();

	private List<TrialLimitConfig> list_0 = new List<TrialLimitConfig>();

	private bool bool_2;

	private Dictionary<string, int> dictionary_1 = new Dictionary<string, int>();

	private Dictionary<string, int> dictionary_2 = new Dictionary<string, int>();

	private Dictionary<string, string> dictionary_3 = new Dictionary<string, string>();

	private decimal decimal_0;

	private CombinedCreditsInfo? combinedCreditsInfo_0;

	private List<DeviceInfo>? list_1;

	private bool bool_3;

	private bool bool_4 = true;

	private const string string_1 = "HardwareFingerprintVersion";

	private const string string_2 = "2.0";

	[CompilerGenerated]
	private EventHandler<AuthorizationStateChangedEventArgs>? eventHandler_0;

	IUserIdentity? IAuthManager.CurrentUser => (IUserIdentity?)(object)userIdentity_0;

	IDeviceInfo? IAuthManager.CurrentDevice => (IDeviceInfo?)(object)deviceInfo_0;

	ILicenseInfo? IAuthManager.CurrentLicense => (ILicenseInfo?)(object)licenseInfo_0;

	public bool IsInitialized => bool_0;

	public bool IsOfflineMode => bool_1;

	public bool IsNetworkAvailable => bool_4;

	public bool IsDeviceDisabled => bool_3;

	public event EventHandler<AuthorizationStateChangedEventArgs>? AuthorizationStateChanged
	{
		[CompilerGenerated]
		add
		{
			EventHandler<AuthorizationStateChangedEventArgs> eventHandler = eventHandler_0;
			EventHandler<AuthorizationStateChangedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<AuthorizationStateChangedEventArgs> value2 = (EventHandler<AuthorizationStateChangedEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler<AuthorizationStateChangedEventArgs> eventHandler = eventHandler_0;
			EventHandler<AuthorizationStateChangedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<AuthorizationStateChangedEventArgs> value2 = (EventHandler<AuthorizationStateChangedEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref eventHandler_0, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public AuthManager(ISupabaseClient supabaseClient, INativeCryptoService cryptoService, IConfigManager configManager, IHttpClientFactory? httpClientFactory = null)
	{
		isupabaseClient_0 = supabaseClient ?? throw new ArgumentNullException("supabaseClient");
		inativeCryptoService_0 = cryptoService ?? throw new ArgumentNullException("cryptoService");
		iconfigManager_0 = configManager ?? throw new ArgumentNullException("configManager");
		ipAddressService_0 = new IpAddressService(httpClientFactory);
		string supabaseUrl = inativeCryptoService_0.GetSupabaseUrl();
		networkStatusChecker_0 = new NetworkStatusChecker(supabaseUrl, httpClientFactory);
		networkStatusChecker_0.NetworkStatusChanged += method_15;
	}

	[AsyncStateMachine(typeof(Struct88))]
	public Task<Result> InitializeAsync()
	{
		Struct88 stateMachine = default(Struct88);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct108))]
	public Task<Result> ShutdownAsync()
	{
		Struct108 stateMachine = default(Struct108);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct109))]
	public Task<Result<IUserIdentity>> SignInAsync(string email, string password)
	{
		Struct109 stateMachine = default(Struct109);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<IUserIdentity>>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = email;
		stateMachine.string_1 = password;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct79))]
	private Task method_0()
	{
		Struct79 stateMachine = default(Struct79);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct77))]
	public Task<Result<IUserIdentity>> AutoSignInAsync()
	{
		Struct77 stateMachine = default(Struct77);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<IUserIdentity>>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct111))]
	public Task<Result<IUserIdentity>> SignUpAsync(string email, string password)
	{
		Struct111 stateMachine = default(Struct111);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<IUserIdentity>>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = email;
		stateMachine.string_1 = password;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct110))]
	public Task<Result> SignOutAsync()
	{
		Struct110 stateMachine = default(Struct110);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct107))]
	public Task<Result> RequestPasswordResetAsync(string email)
	{
		Struct107 stateMachine = default(Struct107);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = email;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public Task<Result<bool>> CanUseFeatureAsync(string featureId)
	{
		try
		{
			if (string.IsNullOrEmpty(featureId))
			{
				return Task.FromResult<Result<bool>>(Result<bool>.Failure("功能 ID 不能为空"));
			}
			if (userIdentity_0 != null && licenseInfo_0 != null)
			{
				return Task.FromResult<Result<bool>>(Result<bool>.Success(licenseInfo_0.HasFeature(featureId)));
			}
			return Task.FromResult<Result<bool>>(Result<bool>.Success(false));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<bool>>(Result<bool>.Failure("检查功能授权异常：" + ex.Message));
		}
	}

	public Task<Result<bool>> HasValidLicenseAsync()
	{
		try
		{
			if (licenseInfo_0 != null && licenseInfo_0.IsValid())
			{
				return Task.FromResult<Result<bool>>(Result<bool>.Success(true));
			}
			return Task.FromResult<Result<bool>>(Result<bool>.Success(false));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<bool>>(Result<bool>.Failure("检查许可证异常：" + ex.Message));
		}
	}

	public Task<Result<string>> GetLicenseTypeAsync()
	{
		try
		{
			if (licenseInfo_0 != null)
			{
				return Task.FromResult<Result<string>>(Result<string>.Success(licenseInfo_0.LicenseType));
			}
			return Task.FromResult<Result<string>>(Result<string>.Success("trial"));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<string>>(Result<string>.Failure("获取许可证类型异常：" + ex.Message));
		}
	}

	public Task<Result<DateTime?>> GetLicenseExpiryAsync()
	{
		try
		{
			if (licenseInfo_0 != null)
			{
				return Task.FromResult<Result<DateTime?>>(Result<DateTime?>.Success(licenseInfo_0.ValidTo));
			}
			return Task.FromResult<Result<DateTime?>>(Result<DateTime?>.Success((DateTime?)null));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<DateTime?>>(Result<DateTime?>.Failure("获取许可证到期日期异常：" + ex.Message));
		}
	}

	public Task<Result<IUserIdentity?>> GetCurrentUserAsync()
	{
		try
		{
			return Task.FromResult<Result<IUserIdentity>>(Result<IUserIdentity>.Success((IUserIdentity)(object)userIdentity_0));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<IUserIdentity>>(Result<IUserIdentity>.Failure("获取当前用户异常：" + ex.Message));
		}
	}

	[AsyncStateMachine(typeof(Struct100))]
	public Task<Result> RecordFeatureUsageAsync(string featureId, FeatureGroup? featureGroup = null)
	{
		Struct100 stateMachine = default(Struct100);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = featureId;
		stateMachine.nullable_0 = featureGroup;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct76))]
	Task<Result> IAuthManager.RecordFeatureUsageAsync(string featureId)
	{
		Struct76 stateMachine = default(Struct76);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = featureId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct85))]
	public Task<Result<int>> GetRemainingTrialUsageAsync(string featureId)
	{
		Struct85 stateMachine = default(Struct85);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<int>>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = featureId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private int method_1(string string_3)
	{
		if (dictionary_1.ContainsKey(string_3))
		{
			return dictionary_1[string_3];
		}
		if (dictionary_3.ContainsKey(string_3))
		{
			string key = dictionary_3[string_3];
			if (dictionary_2.ContainsKey(key))
			{
				return dictionary_2[key];
			}
		}
		return 0;
	}

	public int GetMaxTrialCountFromFeatureGroup(string featureGroup)
	{
		if (dictionary_2.TryGetValue(featureGroup, out var value))
		{
			return value;
		}
		return 0;
	}

	[AsyncStateMachine(typeof(Struct83))]
	public Task<Result> EnsureTrialRecordExistsAsync(string featureId, string? featureGroup = null)
	{
		Struct83 stateMachine = default(Struct83);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = featureId;
		stateMachine.string_1 = featureGroup;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct86))]
	private Task<Result<TrialRecordDetail?>> method_2(string string_3, string? string_4 = null)
	{
		Struct86 stateMachine = default(Struct86);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<TrialRecordDetail>>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = string_3;
		stateMachine.string_1 = string_4;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct81))]
	private Task<Result> method_3(string string_3, string? string_4, int int_0)
	{
		Struct81 stateMachine = default(Struct81);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = string_3;
		stateMachine.string_1 = string_4;
		stateMachine.int_1 = int_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public decimal GetCachedCreditsBalance()
	{
		return decimal_0;
	}

	public CombinedCreditsInfo GetCachedCreditsInfo()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		if (combinedCreditsInfo_0 != null)
		{
			return combinedCreditsInfo_0;
		}
		return new CombinedCreditsInfo
		{
			TotalBalance = decimal_0,
			UserBalance = decimal_0,
			DeviceBalance = 0m,
			TotalPurchased = 0m,
			TotalConsumed = 0m,
			UserPurchased = 0m,
			UserConsumed = 0m,
			DevicePurchased = 0m,
			DeviceConsumed = 0m
		};
	}

	[AsyncStateMachine(typeof(Struct102))]
	public Task RefreshCreditsCacheAsync()
	{
		Struct102 stateMachine = default(Struct102);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public Dictionary<string, int> GetCachedTrialUsage()
	{
		return new Dictionary<string, int>(dictionary_0);
	}

	public Task<Result<int>> GetRemainingTrialUsageByGroupAsync(string groupIdentifier)
	{
		try
		{
			if (string.IsNullOrEmpty(groupIdentifier))
			{
				return Task.FromResult<Result<int>>(Result<int>.Failure("功能组标识不能为空"));
			}
			if (!FeatureGroupConfig.IsValidGroupIdentifier(groupIdentifier))
			{
				Logger.Warning("[AuthManager] ⚠️ 查询的功能组 '" + groupIdentifier + "' 未在数据库中配置");
				Logger.Warning("[AuthManager]   有效功能组: " + string.Join(", ", FeatureGroupConfig.ValidGroupIdentifiers));
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[AuthManager]   将使用默认试用次数: ");
				defaultInterpolatedStringHandler.AppendFormatted(FeatureGroupConfig.GetDefaultTrialCount("default"));
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return Task.FromResult<Result<int>>(Result<int>.Success(FeatureGroupConfig.GetDefaultTrialCount("default")));
			}
			Logger.Info("[AuthManager] 查询功能组试用次数: " + groupIdentifier);
			TrialLimitConfig trialLimitConfig_0 = list_0.FirstOrDefault((TrialLimitConfig trialLimitConfig) => trialLimitConfig.IsGroupConfig && trialLimitConfig.FeatureGroup == groupIdentifier);
			if (trialLimitConfig_0 != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("[AuthManager] 找到功能组配置: ");
				defaultInterpolatedStringHandler2.AppendFormatted(groupIdentifier);
				defaultInterpolatedStringHandler2.AppendLiteral(" -> 最大 ");
				defaultInterpolatedStringHandler2.AppendFormatted(trialLimitConfig_0.MaxUsageCount);
				defaultInterpolatedStringHandler2.AppendLiteral(" 次");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				int num = dictionary_0.Where<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair_0) => method_4(keyValuePair_0.Key, groupIdentifier)).Sum((KeyValuePair<string, int> keyValuePair_0) => trialLimitConfig_0.MaxUsageCount - keyValuePair_0.Value);
				int num2 = trialLimitConfig_0.MaxUsageCount - num;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(45, 4);
				defaultInterpolatedStringHandler3.AppendLiteral("[AuthManager] 功能组试用次数估算: ");
				defaultInterpolatedStringHandler3.AppendFormatted(groupIdentifier);
				defaultInterpolatedStringHandler3.AppendLiteral(" -> 剩余 ");
				defaultInterpolatedStringHandler3.AppendFormatted(num2);
				defaultInterpolatedStringHandler3.AppendLiteral(" 次（最大 ");
				defaultInterpolatedStringHandler3.AppendFormatted(trialLimitConfig_0.MaxUsageCount);
				defaultInterpolatedStringHandler3.AppendLiteral(" - 已用 ");
				defaultInterpolatedStringHandler3.AppendFormatted(num);
				defaultInterpolatedStringHandler3.AppendLiteral("）");
				Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
				return Task.FromResult<Result<int>>(Result<int>.Success(Math.Max(0, num2)));
			}
			Logger.Warning("[AuthManager] 未找到功能组配置: " + groupIdentifier);
			return Task.FromResult<Result<int>>(Result<int>.Failure("未找到功能组配置: " + groupIdentifier));
		}
		catch (Exception ex)
		{
			Logger.Error("[AuthManager] 查询功能组试用次数异常: " + groupIdentifier, ex);
			return Task.FromResult<Result<int>>(Result<int>.Failure("查询功能组试用次数异常：" + ex.Message));
		}
	}

	private bool method_4(string string_3, string string_4)
	{
		if (dictionary_3.TryGetValue(string_3, out string value))
		{
			return value == string_4;
		}
		TrialLimitConfig trialLimitConfig = list_0.FirstOrDefault((TrialLimitConfig trialLimitConfig_0) => trialLimitConfig_0.IsFeatureConfig && trialLimitConfig_0.FeatureId == string_3);
		if (trialLimitConfig != null && !string.IsNullOrEmpty(trialLimitConfig.FeatureGroup))
		{
			return trialLimitConfig.FeatureGroup == string_4;
		}
		if (string_3.StartsWith("AI", StringComparison.OrdinalIgnoreCase))
		{
			return string_4 == "ai";
		}
		if (string_3.IndexOf("Room", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return string_4 == "room";
		}
		return false;
	}

	public Task<Result> EnableOfflineModeAsync()
	{
		try
		{
			bool_1 = true;
			return Task.FromResult<Result>(Result.Success());
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result>(Result.Failure("启用离线模式异常：" + ex.Message));
		}
	}

	public Task<Result> DisableOfflineModeAsync()
	{
		try
		{
			bool_1 = false;
			return Task.FromResult<Result>(Result.Success());
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result>(Result.Failure("禁用离线模式异常：" + ex.Message));
		}
	}

	[AsyncStateMachine(typeof(Struct84))]
	public Task<Result<ILicenseInfo>> GetLicenseInfoAsync()
	{
		Struct84 stateMachine = default(Struct84);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<ILicenseInfo>>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public Task<Result<bool>> IsLicenseExpiringSoonAsync(int daysThreshold = 7)
	{
		try
		{
			if (licenseInfo_0 != null)
			{
				return Task.FromResult<Result<bool>>(Result<bool>.Success(licenseInfo_0.IsExpiringSoon(daysThreshold)));
			}
			return Task.FromResult<Result<bool>>(Result<bool>.Success(false));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<bool>>(Result<bool>.Failure("检查许可证过期异常：" + ex.Message));
		}
	}

	public Task<Result<IDeviceInfo>> GetDeviceInfoAsync()
	{
		try
		{
			if (deviceInfo_0 != null)
			{
				return Task.FromResult<Result<IDeviceInfo>>(Result<IDeviceInfo>.Success((IDeviceInfo)(object)deviceInfo_0));
			}
			return Task.FromResult<Result<IDeviceInfo>>(Result<IDeviceInfo>.Failure("设备信息不可用"));
		}
		catch (Exception ex)
		{
			return Task.FromResult<Result<IDeviceInfo>>(Result<IDeviceInfo>.Failure("获取设备信息异常：" + ex.Message));
		}
	}

	[AsyncStateMachine(typeof(Struct82))]
	public Task<Result<IDeviceInfo>> EnsureDeviceRegisteredAsync()
	{
		Struct82 stateMachine = default(Struct82);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<IDeviceInfo>>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct105))]
	public Task<Result<IDeviceInfo>> RegisterDeviceAsync()
	{
		Struct105 stateMachine = default(Struct105);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<IDeviceInfo>>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct106))]
	public Task<Result> RegisterOrFindDeviceAsync()
	{
		Struct106 stateMachine = default(Struct106);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct90))]
	private Task method_5()
	{
		Struct90 stateMachine = default(Struct90);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct92))]
	private Task method_6()
	{
		Struct92 stateMachine = default(Struct92);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct94))]
	private Task method_7(Guid guid_0)
	{
		Struct94 stateMachine = default(Struct94);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.authManager_0 = this;
		stateMachine.guid_0 = guid_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct93))]
	private Task method_8(string string_3)
	{
		Struct93 stateMachine = default(Struct93);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = string_3;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct96))]
	private Task method_9(string string_3)
	{
		Struct96 stateMachine = default(Struct96);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = string_3;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct91))]
	private Task<CombinedCreditsInfo?> method_10(Guid? nullable_0 = null, Guid? nullable_1 = null, bool bool_5 = true)
	{
		Struct91 stateMachine = default(Struct91);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<CombinedCreditsInfo>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.nullable_0 = nullable_0;
		stateMachine.nullable_1 = nullable_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private bool method_11()
	{
		if (licenseInfo_0 != null)
		{
			return licenseInfo_0.IsValid();
		}
		return false;
	}

	[AsyncStateMachine(typeof(Struct101))]
	public Task<Result> RefreshAuthorizationDataAsync()
	{
		Struct101 stateMachine = default(Struct101);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct87))]
	public Task<Result<List<IDeviceInfo>>> GetUserDevicesAsync()
	{
		Struct87 stateMachine = default(Struct87);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<IDeviceInfo>>>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct80))]
	public Task<(bool canBind, string message)> CanBindCurrentDeviceAsync()
	{
		Struct80 stateMachine = default(Struct80);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<(bool, string)>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct78))]
	public Task<Result> BindCurrentDeviceAsync()
	{
		Struct78 stateMachine = default(Struct78);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct112))]
	public Task<Result> UnbindDeviceAsync(string deviceId)
	{
		Struct112 stateMachine = default(Struct112);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.string_0 = deviceId;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct89))]
	public Task<Result> InitializeUserCreditsAsync()
	{
		Struct89 stateMachine = default(Struct89);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct103))]
	public Task<Result> RefreshCurrentDeviceAsync()
	{
		Struct103 stateMachine = default(Struct103);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public void SetCurrentRevitVersion(string versionYear)
	{
		string_0 = versionYear;
		if (deviceInfo_0 != null)
		{
			method_13(deviceInfo_0, versionYear);
		}
	}

	private Result method_12()
	{
		if (deviceInfo_0 == null)
		{
			return Result.Failure("设备信息不可用");
		}
		if (deviceInfo_0.IsDisabled)
		{
			bool_3 = true;
			licenseInfo_0 = null;
			decimal_0 = 0m;
			foreach (string item in dictionary_0.Keys.ToList())
			{
				dictionary_0[item] = int.MaxValue;
			}
			Logger.Warning("[AuthManager] 设备 " + deviceInfo_0.DeviceId.Substring(0, 16) + "... 已被禁用，已禁用所有试用功能");
			return Result.Failure("设备已被禁用，请联系客服");
		}
		bool_3 = false;
		return Result.Success();
	}

	private void method_13(DeviceInfo deviceInfo_1, string? string_3)
	{
		if (deviceInfo_1 == null || string.IsNullOrEmpty(string_3))
		{
			return;
		}
		deviceInfo_1.RevitVersion = string_3;
		if (int.TryParse(string_3, out var result))
		{
			switch (result)
			{
			case 2018:
				deviceInfo_1.Revit2018 = true;
				break;
			case 2019:
				deviceInfo_1.Revit2019 = true;
				break;
			case 2020:
				deviceInfo_1.Revit2020 = true;
				break;
			case 2021:
				deviceInfo_1.Revit2021 = true;
				break;
			case 2022:
				deviceInfo_1.Revit2022 = true;
				break;
			case 2023:
				deviceInfo_1.Revit2023 = true;
				break;
			case 2024:
				deviceInfo_1.Revit2024 = true;
				break;
			case 2025:
				deviceInfo_1.Revit2025 = true;
				break;
			case 2026:
				deviceInfo_1.Revit2026 = true;
				break;
			}
		}
	}

	private void method_14()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		try
		{
			AuthorizationStateChangedEventArgs e = new AuthorizationStateChangedEventArgs
			{
				HasValidLicense = method_11(),
				LicenseType = licenseInfo_0?.LicenseType,
				ExpiryDate = licenseInfo_0?.ValidTo,
				CreditsBalance = decimal_0,
				TrialUsage = new Dictionary<string, int>(dictionary_0),
				UserDevices = list_1?.Cast<IDeviceInfo>().ToList()
			};
			eventHandler_0?.Invoke(this, e);
		}
		catch (Exception ex)
		{
			Logger.Warning("[AuthManager] 触发授权状态变化事件失败", ex);
		}
	}

	private void method_15(object? object_0, bool bool_5)
	{
		try
		{
			bool_4 = bool_5;
			if (bool_5)
			{
				if (bool_1)
				{
					bool_1 = false;
					dictionary_0.Clear();
					method_14();
				}
			}
			else if (!bool_1)
			{
				bool_1 = true;
				dictionary_0.Clear();
				method_14();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[AuthManager] 处理网络状态变化失败", ex);
		}
	}

	[AsyncStateMachine(typeof(Struct97))]
	private Task method_16(Guid guid_0)
	{
		Struct97 stateMachine = default(Struct97);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.authManager_0 = this;
		stateMachine.guid_0 = guid_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public AuthorizationState GetAuthorizationState()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		return new AuthorizationState
		{
			HasValidLicense = method_11(),
			LicenseType = licenseInfo_0?.LicenseType,
			ExpiryDate = licenseInfo_0?.ValidTo,
			CreditsBalance = decimal_0,
			TrialUsage = new Dictionary<string, int>(dictionary_0)
		};
	}

	public List<IDeviceInfo>? GetCachedUserDevices()
	{
		return list_1?.Cast<IDeviceInfo>().ToList();
	}

	private void method_17()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			dictionary_3.Clear();
			foreach (RegisteredCommand allCommand in CommandRegistry.GetAllCommands())
			{
				string identifier = FeatureGroupExtensions.GetIdentifier(allCommand.Attribute.Group);
				dictionary_3[allCommand.Name] = identifier;
			}
			dictionary_3["AI_Send"] = "ai";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[AuthManager] ✓ 构建功能组映射：");
			defaultInterpolatedStringHandler.AppendFormatted(dictionary_3.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个命令");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			Logger.Warning("[AuthManager] 构建功能组映射异常（已忽略）", ex);
		}
	}

	[AsyncStateMachine(typeof(Struct95))]
	private Task method_18()
	{
		Struct95 stateMachine = default(Struct95);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct104))]
	public Task<Result> RefreshTrialLimitConfigCacheAsync()
	{
		Struct104 stateMachine = default(Struct104);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct99))]
	public Task<bool> NeedsHardwareFingerprintMigrationAsync()
	{
		Struct99 stateMachine = default(Struct99);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct98))]
	public Task<Result> MigrateHardwareFingerprintAsync()
	{
		Struct98 stateMachine = default(Struct98);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result>.Create();
		stateMachine.authManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[CompilerGenerated]
	private bool method_19(DeviceInfo deviceInfo_1)
	{
		return deviceInfo_1.DeviceId != deviceInfo_0.DeviceId;
	}
}
