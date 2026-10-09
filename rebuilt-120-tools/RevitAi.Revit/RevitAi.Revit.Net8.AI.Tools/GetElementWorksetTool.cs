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

[AITool("get_element_workset", Category = "工作集管理", Description = "获取元素所在的工作集信息", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetElementWorksetTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class499 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetElementWorksetTool getElementWorksetTool_0;

		private int int_1;

		private IElementService ielementService_0;

		private object object_0;

		private object object_1;

		private int int_2;

		private string string_0;

		private bool bool_0;

		private bool bool_1;

		private bool bool_2;

		private string string_1;

		private string string_2;

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
					Class499 stateMachine = this;
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
				if (ielementService_0 == null)
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
						defaultInterpolatedStringHandler.AppendLiteral(" 的元素");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						object_1 = ielementService_0.GetElementWorkset(object_0);
						if (object_1 == null)
						{
							result = AIToolResult.Fail("无法获取元素的工作集信息");
						}
						else
						{
							int_2 = ielementService_0.GetWorksetId(object_1);
							string_0 = ielementService_0.GetWorksetName(object_1);
							bool_0 = ielementService_0.IsWorksetVisible(object_1);
							bool_1 = ielementService_0.IsWorksetOpen(object_1);
							bool_2 = ielementService_0.IsDefaultWorkset(object_1);
							string_1 = ielementService_0.GetElementName(object_0);
							string_2 = ielementService_0.GetElementCategory(object_0);
							result = AIToolResult.Ok("成功获取元素的工作集信息", (object)new Class176<int, string, string, Class177<int, string, bool, bool, bool>>(int_1, string_1 ?? "未命名", string_2 ?? "未知", new Class177<int, string, bool, bool, bool>(int_2, string_0 ?? "未命名", bool_0, bool_1, bool_2)));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取元素工作集失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_element_workset";

	public string Category => "工作集管理";

	public string Description => "获取元素工作集";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elementId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"元素 ID\"\r\n            }\r\n        },\r\n        \"required\": [\"elementId\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class499))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class499 stateMachine = new Class499();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getElementWorksetTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
