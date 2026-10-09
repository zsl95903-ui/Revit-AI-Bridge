using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("delete_view_filter", Category = "视图高级操作", Description = "删除文档中的视图过滤器。注意：删除过滤器会自动将其从所有应用该过滤器的视图中移除。", RequiresTransaction = true, RequiresModification = true)]
public sealed class DeleteViewFilterTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class439 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public DeleteViewFilterTool deleteViewFilterTool_0;

		private string string_0;

		private IViewService iviewService_0;

		private bool bool_0;

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
					Class439 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("filterName", (string)null);
				if (string.IsNullOrEmpty(string_0))
				{
					result = AIToolResult.Fail("过滤器名称不能为空");
				}
				else
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
						bool_0 = iviewService_0.DeleteViewFilter(aitoolContext_0.Document, string_0);
						if (!bool_0)
						{
							result = AIToolResult.Fail("删除过滤器 '" + string_0 + "' 失败");
						}
						else
						{
							Logger.Info("[DeleteViewFilterTool] 成功删除过滤器 '" + string_0 + "'");
							result = AIToolResult.Ok("成功删除视图过滤器 '" + string_0 + "'", (object)null);
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[DeleteViewFilterTool] 删除视图过滤器失败: " + exception_0.Message);
				result = AIToolResult.Fail("删除视图过滤器失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "delete_view_filter";

	public string Category => "视图高级操作";

	public string Description => "删除视图过滤器";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"filterName\": {\n                \"type\": \"string\",\n                \"description\": \"要删除的过滤器名称\"\n            }\n        },\n        \"required\": [\"filterName\"]\n    }";

	[AsyncStateMachine(typeof(Class439))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class439 stateMachine = new Class439();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.deleteViewFilterTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
