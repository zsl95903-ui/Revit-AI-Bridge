using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using ns7;

namespace RevitAi.Core.AI;

public sealed class RevitExternalEventHandler
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct206 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public Func<AIToolContext, Task<AIToolResult>> func_0;

		public bool bool_0;

		public RevitExternalEventHandler revitExternalEventHandler_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			int num = int_0;
			RevitExternalEventHandler revitExternalEventHandler = revitExternalEventHandler_0;
			TaskCompletionSource<AIToolResult> taskCompletionSource = default(TaskCompletionSource<AIToolResult>);
			RevitExternalEventRequest request = default(RevitExternalEventRequest);
			if (num != 0)
			{
				if (aitoolContext_0 == null)
				{
					throw new ArgumentNullException("context");
				}
				if (func_0 == null)
				{
					throw new ArgumentNullException("action");
				}
				taskCompletionSource = new TaskCompletionSource<AIToolResult>();
				request = new RevitExternalEventRequest
				{
					Action = func_0,
					Context = aitoolContext_0,
					TaskSource = taskCompletionSource,
					RequiresTransaction = bool_0
				};
			}
			AIToolResult result;
			try
			{
				TaskAwaiter<AIToolResult> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0134;
				}
				AIRequestManager.SetRequest((IRevitExternalEventRequest)(object)request);
				MethodInfo method = revitExternalEventHandler.object_0.GetType().GetMethod("Raise");
				if (!(method == null))
				{
					method.Invoke(revitExternalEventHandler.object_0, null);
					awaiter = taskCompletionSource.Task.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0134;
				}
				Logger.Error("[ExternalEvent] 无法找到 Raise 方法");
				result = AIToolResult.Fail("无法触发 Revit ExternalEvent");
				goto end_IL_007a;
				IL_0134:
				result = awaiter.GetResult();
				end_IL_007a:;
			}
			catch (Exception ex)
			{
				Logger.Error("[ExternalEvent] 执行失败: " + revitExternalEventHandler.string_0, ex);
				result = AIToolResult.Fail("执行工具时发生异常: " + ex.Message);
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

	private readonly object object_0;

	private readonly Action<object?, object?>? action_0;

	private readonly string string_0;

	private readonly IAIToolDataCache? iaitoolDataCache_0;

	private readonly RevitDocumentChangeMonitor? revitDocumentChangeMonitor_0;

	public RevitExternalEventHandler(object revitExternalEvent, Action<object?, object?>? externalEventExecute, string eventName = "AI Tool Event", IAIToolDataCache? dataCache = null, RevitDocumentChangeMonitor? documentChangeMonitor = null)
	{
		object_0 = revitExternalEvent ?? throw new ArgumentNullException("revitExternalEvent");
		action_0 = externalEventExecute;
		string_0 = eventName;
		iaitoolDataCache_0 = dataCache;
		revitDocumentChangeMonitor_0 = documentChangeMonitor;
	}

	[AsyncStateMachine(typeof(Struct206))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, bool requiresTransaction, Func<AIToolContext, Task<AIToolResult>> action)
	{
		Struct206 stateMachine = default(Struct206);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.revitExternalEventHandler_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.bool_0 = requiresTransaction;
		stateMachine.func_0 = action;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public void NotifyDocumentChanged()
	{
		if (revitDocumentChangeMonitor_0 != null)
		{
			revitDocumentChangeMonitor_0.OnDocumentChanged();
			Logger.Info("[ExternalEvent] 文档已变更，已触发缓存清理");
		}
	}
}
