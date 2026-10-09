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

[AITool("get_element_phases", Category = "材料管理", Description = "获取元素的创建阶段和拆除阶段信息", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetElementPhasesTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class498 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetElementPhasesTool getElementPhasesTool_0;

		private int int_1;

		private IElementService ielementService_0;

		private IPhaseService iphaseService_0;

		private object object_0;

		private (object? CreatedPhase, object? DemolishedPhase)? nullable_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private object object_1;

		private object object_2;

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
					Class498 stateMachine = this;
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
				int_1 = aitoolContext_0.GetParameter<int>("elementId", 0);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				iphaseService_0 = ((revitAdapter2 != null) ? revitAdapter2.PhaseService : null);
				if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (iphaseService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 PhaseService");
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
						defaultInterpolatedStringHandler.AppendLiteral(" 的元素");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						nullable_0 = iphaseService_0.GetElementPhases(object_0);
						string_0 = ielementService_0.GetElementName(object_0);
						string_1 = null;
						string_2 = null;
						if (nullable_0.HasValue)
						{
							if (nullable_0.Value.CreatedPhase != null)
							{
								object_1 = nullable_0.Value.CreatedPhase;
								string_1 = ielementService_0.GetElementName(object_1);
								object_1 = null;
							}
							if (nullable_0.Value.DemolishedPhase != null)
							{
								object_2 = nullable_0.Value.DemolishedPhase;
								string_2 = ielementService_0.GetElementName(object_2);
								object_2 = null;
							}
						}
						result = AIToolResult.Ok("元素 '" + (string_0 ?? "未命名") + "' 的阶段信息", (object)new Class175<int, string, string, string>(int_1, string_0, string_1, string_2));
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取阶段信息失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_element_phases";

	public string Category => "材料管理";

	public string Description => "获取元素的创建阶段和拆除阶段信息";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elementId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"元素的 ID\"\r\n            }\r\n        },\r\n        \"required\": [\"elementId\"]\r\n    }";

	[AsyncStateMachine(typeof(Class498))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class498 stateMachine = new Class498();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getElementPhasesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
