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

[AITool("create_text_note", Category = "注释与标记", Description = "在视图中创建文字注释", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateTextNoteTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class424 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateTextNoteTool createTextNoteTool_0;

		private int int_1;

		private string string_0;

		private object object_0;

		private int? nullable_0;

		private bool bool_0;

		private IAnnotationService iannotationService_0;

		private IElementService ielementService_0;

		private object object_1;

		private object object_2;

		private int? nullable_1;

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
					Class424 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("text", (string)null);
				object_0 = aitoolContext_0.GetParameter<object>("position", (object)null);
				nullable_0 = aitoolContext_0.GetParameter<int?>("textTypeId", (int?)null);
				bool_0 = aitoolContext_0.GetParameter<bool>("leader", false);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				iannotationService_0 = ((revitAdapter != null) ? revitAdapter.AnnotationService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				if (iannotationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 AnnotationService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else if (string.IsNullOrEmpty(string_0))
				{
					result = AIToolResult.Fail("注释文字内容不能为空");
				}
				else
				{
					object_1 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
					if (object_1 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 的视图");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						object_2 = iannotationService_0.CreateTextNote(aitoolContext_0.Document, object_1, string_0, object_0, nullable_0, bool_0);
						if (object_2 == null)
						{
							result = AIToolResult.Fail("创建文字注释失败");
						}
						else
						{
							nullable_1 = ielementService_0.GetElementId(object_2);
							string_1 = ielementService_0.GetElementName(object_1);
							result = AIToolResult.Ok("成功在视图 '" + (string_1 ?? "未命名") + "' 中创建文字注释: " + string_0, (object)new Class93<int?, string, int, string, object, bool>(nullable_1, string_0, int_1, string_1, object_0, bool_0));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建文字注释失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_text_note";

	public string Category => "注释与标记";

	public string Description => "创建文字注释";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"viewId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"视图 ID\"\r\n            },\r\n            \"text\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"注释文字内容\"\r\n            },\r\n            \"position\": {\r\n                \"type\": \"object\",\r\n                \"description\": \"文字位置 {x: number, y: number, z: number}\",\r\n                \"properties\": {\r\n                    \"x\": { \"type\": \"number\" },\r\n                    \"y\": { \"type\": \"number\" },\r\n                    \"z\": { \"type\": \"number\" }\r\n                },\r\n                \"required\": [\"x\", \"y\", \"z\"]\r\n            },\r\n            \"textTypeId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"文字类型 ID（可选，使用默认类型）\"\r\n            },\r\n            \"leader\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否添加引线（默认 false）\",\r\n                \"default\": false\r\n            }\r\n        },\r\n        \"required\": [\"viewId\", \"text\", \"position\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class424))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class424 stateMachine = new Class424();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createTextNoteTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
