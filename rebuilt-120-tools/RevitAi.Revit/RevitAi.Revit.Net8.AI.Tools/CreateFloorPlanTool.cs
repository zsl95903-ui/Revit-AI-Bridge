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

[AITool("create_floor_plan", Category = "视图创建", Description = "为指定标高创建平面图视图", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateFloorPlanTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class405 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateFloorPlanTool createFloorPlanTool_0;

		private IViewService iviewService_0;

		private int int_1;

		private string string_0;

		private object object_0;

		private int? nullable_0;

		private string string_1;

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
					Class405 stateMachine = this;
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
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
				if (iviewService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					int_1 = aitoolContext_0.GetParameter<int>("level_id", 0);
					string_0 = aitoolContext_0.GetParameter<string>("view_name", (string)null);
					object_0 = iviewService_0.CreateFloorPlan(aitoolContext_0.Document, int_1, string_0);
					if (object_0 == null)
					{
						result = AIToolResult.Fail("创建平面图失败");
					}
					else
					{
						nullable_0 = iviewService_0.GetViewId(object_0);
						string_1 = iviewService_0.GetViewName(object_0);
						result = AIToolResult.Ok("成功创建平面图视图：" + (string_1 ?? "未命名"), (object)new Class60<int?, string, string>(nullable_0, string_1 ?? "未命名", "平面图创建成功"));
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建平面图失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_floor_plan";

	public string Category => "视图创建";

	public string Description => "为指定标高创建平面图视图";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"level_id\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"标高 ID\"\r\n            },\r\n            \"view_name\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"视图名称（可选）\"\r\n            }\r\n        },\r\n        \"required\": [\"level_id\"]\r\n    }";

	[AsyncStateMachine(typeof(Class405))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class405 stateMachine = new Class405();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createFloorPlanTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
