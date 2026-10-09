using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_all_worksets", Category = "工作集管理", Description = "获取文档中的所有工作集及其信息", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetWorksetsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class509 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetWorksetsTool getWorksetsTool_0;

		private bool bool_0;

		private bool bool_1;

		private IElementService ielementService_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private IEnumerator<object> ienumerator_0;

		private object object_0;

		private int int_1;

		private string string_0;

		private bool bool_2;

		private bool bool_3;

		private bool bool_4;

		private Dictionary<string, object?> dictionary_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class509 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
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
			AIToolResult result;
			try
			{
				bool_0 = aitoolContext_0.GetParameter<bool>("includeActive", true);
				bool_1 = aitoolContext_0.GetParameter<bool>("includeUserCreated", false);
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
					ienumerable_0 = ielementService_0.GetWorksets(aitoolContext_0.Document, bool_1);
					if (ienumerable_0 == null || !ienumerable_0.Any())
					{
						result = AIToolResult.Ok("文档中没有找到任何工作集", (object)new Class210<int, object[]>(0, Array.Empty<object>()));
					}
					else
					{
						list_0 = new List<object>();
						ienumerator_0 = ienumerable_0.GetEnumerator();
						try
						{
							while (ienumerator_0.MoveNext())
							{
								object_0 = ienumerator_0.Current;
								int_1 = ielementService_0.GetWorksetId(object_0);
								string_0 = ielementService_0.GetWorksetName(object_0);
								bool_2 = ielementService_0.IsWorksetVisible(object_0);
								bool_3 = ielementService_0.IsWorksetOpen(object_0);
								bool_4 = ielementService_0.IsDefaultWorkset(object_0);
								dictionary_0 = new Dictionary<string, object>
								{
									["worksetId"] = int_1,
									["name"] = string_0 ?? "未命名",
									["isVisible"] = bool_2,
									["isOpen"] = bool_3,
									["isDefault"] = bool_4
								};
								list_0.Add(dictionary_0);
								string_0 = null;
								dictionary_0 = null;
								object_0 = null;
							}
						}
						finally
						{
							if (num < 0 && ienumerator_0 != null)
							{
								ienumerator_0.Dispose();
							}
						}
						ienumerator_0 = null;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
						defaultInterpolatedStringHandler.AppendLiteral("成功获取 ");
						defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个工作集");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class210<int, List<object>>(list_0.Count, list_0));
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取工作集失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_all_worksets";

	public string Category => "工作集管理";

	public string Description => "获取所有工作集";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"includeActive\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否包含活动工作集标记（默认 true）\",\r\n                \"default\": true\r\n            },\r\n            \"includeUserCreated\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否只包含用户创建的工作集（默认 false）\",\r\n                \"default\": false\r\n            }\r\n        },\r\n        \"required\": []\r\n    }";

	[AsyncStateMachine(typeof(Class509))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class509 stateMachine = new Class509();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getWorksetsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
