using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Product;
using RevitAi.Core.Configuration;
using ns7;

namespace RevitAi.Core.Update;

public class UpdateNotificationService
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct2 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public UpdateNotificationService updateNotificationService_0;

		private TaskAwaiter<Result> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			UpdateNotificationService updateNotificationService = updateNotificationService_0;
			try
			{
				TaskAwaiter<Result> awaiter;
				if (num != 0)
				{
					awaiter = updateNotificationService.iconfigManager_0.SetAsync("UpdateNotification_LastShownVersion", ProductInfo.Version).GetAwaiter();
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
				Logger.Info("[UpdateNotificationService] 已标记版本 " + ProductInfo.Version + " 为已显示");
			}
			catch (Exception ex)
			{
				Logger.Warning("[UpdateNotificationService] 标记版本失败: " + ex.Message);
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
	public struct Struct3 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public UpdateNotificationService updateNotificationService_0;

		private TaskAwaiter<Result<string>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			UpdateNotificationService updateNotificationService = updateNotificationService_0;
			bool result2;
			try
			{
				TaskAwaiter<Result<string>> awaiter;
				if (num != 0)
				{
					awaiter = updateNotificationService.iconfigManager_0.GetAsync("UpdateNotification_LastShownVersion", string.Empty).GetAwaiter();
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
					taskAwaiter_0 = default(TaskAwaiter<Result<string>>);
					num = -1;
					int_0 = -1;
				}
				Result<string> result = awaiter.GetResult();
				if (!result.IsSuccess)
				{
					result2 = true;
				}
				else
				{
					string text = result.Value ?? string.Empty;
					result2 = string.IsNullOrEmpty(text) || updateNotificationService.method_0(ProductInfo.Version, text);
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("[UpdateNotificationService] 检查更新通知失败: " + ex.Message);
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

	private const string string_0 = "UpdateNotification_LastShownVersion";

	private readonly IConfigManager iconfigManager_0;

	public UpdateNotificationService(IConfigManager configManager)
	{
		iconfigManager_0 = configManager ?? throw new ArgumentNullException("configManager");
	}

	[AsyncStateMachine(typeof(Struct3))]
	public Task<bool> ShouldShowUpdateNotificationAsync()
	{
		Struct3 stateMachine = default(Struct3);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.updateNotificationService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct2))]
	public Task MarkVersionAsShownAsync()
	{
		Struct2 stateMachine = default(Struct2);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.updateNotificationService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public string GetCurrentVersionReleaseNotes()
	{
		return ProductInfo.GetCurrentVersionReleaseNotes();
	}

	private bool method_0(string string_1, string string_2)
	{
		try
		{
			Version version = new Version(string_1);
			Version version2 = new Version(string_2);
			return version > version2;
		}
		catch
		{
			return string.CompareOrdinal(string_1, string_2) > 0;
		}
	}
}
