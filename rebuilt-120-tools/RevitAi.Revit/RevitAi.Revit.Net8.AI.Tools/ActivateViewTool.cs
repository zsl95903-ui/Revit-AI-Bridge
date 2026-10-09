using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("activate_view", Category = "视图管理", Description = "激活指定的视图，使其成为当前活动视图", RequiresTransaction = false, RequiresModification = false)]
public sealed class ActivateViewTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class351 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ActivateViewTool activateViewTool_0;

		private int int_1;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private object object_0;

		private bool bool_0;

		private string string_0;

		private Exception exception_0;

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
					Class351 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
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
			AIToolResult result;
			try
			{
				int_1 = aitoolContext_0.GetParameter<int>("viewId", 0);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				if (iviewService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
					if (object_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 的视图");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						bool_0 = iviewService_0.ActivateView(aitoolContext_0.Document, object_0);
						if (!bool_0)
						{
							result = AIToolResult.Fail("激活视图失败");
						}
						else
						{
							string_0 = ielementService_0.GetElementName(object_0);
							result = AIToolResult.Ok("已激活视图: " + (string_0 ?? "未命名视图"), (object)new ns0.Class3<int, string>(int_1, string_0));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("激活视图失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "activate_view";

	public string Category => "视图管理";

	public string Description => "激活指定的视图";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"viewId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"要激活的视图 ID\"\r\n            }\r\n        },\r\n        \"required\": [\"viewId\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class351))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class351 stateMachine = new Class351();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.activateViewTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
