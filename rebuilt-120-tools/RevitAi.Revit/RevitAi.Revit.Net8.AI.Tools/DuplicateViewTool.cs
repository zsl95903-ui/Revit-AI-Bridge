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

[AITool("duplicate_view", Category = "视图高级操作", Description = "复制现有视图并创建副本", RequiresTransaction = true, RequiresModification = true)]
public sealed class DuplicateViewTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class441 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public DuplicateViewTool duplicateViewTool_0;

		private int int_1;

		private string string_0;

		private bool bool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private object object_0;

		private string string_1;

		private object object_1;

		private int? nullable_0;

		private string string_2;

		private string string_3;

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
					Class441 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("newViewName", (string)null);
				bool_0 = aitoolContext_0.GetParameter<bool>("duplicateDetail", true);
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
						string_1 = (bool_0 ? "WithDetailing" : "WithoutDetailing");
						object_1 = iviewService_0.DuplicateView(aitoolContext_0.Document, object_0, string_0 ?? "", string_1);
						if (object_1 == null)
						{
							result = AIToolResult.Fail("复制视图失败");
						}
						else
						{
							nullable_0 = ielementService_0.GetElementId(object_1);
							string_2 = ielementService_0.GetElementName(object_0);
							string_3 = ielementService_0.GetElementName(object_1);
							result = AIToolResult.Ok("成功复制视图: " + (string_2 ?? "未命名") + " → " + (string_3 ?? "未命名"), (object)new Class112<int, string, int?, string, bool>(int_1, string_2, nullable_0, string_3, bool_0));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("复制视图失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "duplicate_view";

	public string Category => "视图高级操作";

	public string Description => "复制现有视图并创建副本";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"viewId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"要复制的视图 ID\"\r\n            },\r\n            \"newViewName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"新视图的名称（可选）\"\r\n            },\r\n            \"duplicateDetail\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否复制详图构件（默认 true）\",\r\n                \"default\": true\r\n            }\r\n        },\r\n        \"required\": [\"viewId\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class441))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class441 stateMachine = new Class441();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.duplicateViewTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
