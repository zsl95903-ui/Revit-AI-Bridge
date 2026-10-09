using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_cache_data", Category = "数据管理", Description = "读取之前工具生成的缓存数据。⚠\ufe0f 重要提示：大多数情况下您不需要使用此工具！许多工具（如 set_element_color、create_3d_view、export_excel 等）可以直接使用缓存 ID 作为参数，只有当您需要查看缓存具体内容或进行复杂分析时才需要调用此工具。", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetCacheDataTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class490 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetCacheDataTool getCacheDataTool_0;

		private string string_0;

		private int int_1;

		private int int_2;

		private string string_1;

		private object object_0;

		private AIToolResult aitoolResult_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if (num == 1)
				{
					goto IL_006c;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class490 stateMachine = this;
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
			goto IL_006c;
			IL_006c:
			AIToolResult result;
			try
			{
				TaskAwaiter<AIToolResult> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0243;
				}
				string_0 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
				int_1 = aitoolContext_0.GetParameter<int>("limit", 50);
				int_2 = aitoolContext_0.GetParameter<int>("offset", 0);
				if (string.IsNullOrEmpty(string_0))
				{
					result = AIToolResult.Fail("缓存 ID 不能为空");
				}
				else
				{
					string_1 = (string_0.StartsWith("@") ? string_0.Substring(1) : string_0);
					object_0 = aitoolContext_0.GetCachedData<object>(string_1);
					if (object_0 != null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 3);
						defaultInterpolatedStringHandler.AppendLiteral("[GetCacheData] 读取缓存 '");
						defaultInterpolatedStringHandler.AppendFormatted(string_1);
						defaultInterpolatedStringHandler.AppendLiteral("', limit=");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(", offset=");
						defaultInterpolatedStringHandler.AppendFormatted(int_2);
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						awaiter2 = getCacheDataTool_0.method_0(object_0, string_1, int_1, int_2).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							Class490 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
							return;
						}
						goto IL_0243;
					}
					result = AIToolResult.Fail("找不到缓存数据：" + string_1);
				}
				goto end_IL_006c;
				IL_0243:
				aitoolResult_0 = awaiter2.GetResult();
				result = aitoolResult_0;
				end_IL_006c:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[GetCacheData] 读取缓存失败: " + exception_0.Message);
				result = AIToolResult.Fail("读取缓存失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class491 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public object object_0;

		public string string_0;

		public int int_1;

		public int int_2;

		public GetCacheDataTool getCacheDataTool_0;

		private IEnumerable ienumerable_0;

		private List<object> list_0;

		private int int_3;

		private List<object> list_1;

		private string string_1;

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
					Class491 stateMachine = this;
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
			ienumerable_0 = object_0 as IEnumerable;
			AIToolResult result;
			if (ienumerable_0 != null)
			{
				list_0 = ienumerable_0.Cast<object>().ToList();
				int_3 = list_0.Count;
				list_1 = list_0.Skip(int_2).Take(int_1).ToList();
				if (int_3 == 0)
				{
					string_1 = "缓存中没有数据";
				}
				else if (list_1.Count == 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 2);
					defaultInterpolatedStringHandler.AppendLiteral("起始位置 ");
					defaultInterpolatedStringHandler.AppendFormatted(int_2 + 1);
					defaultInterpolatedStringHandler.AppendLiteral(" 超出范围，共 ");
					defaultInterpolatedStringHandler.AppendFormatted(int_3);
					defaultInterpolatedStringHandler.AppendLiteral(" 项数据");
					string_1 = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				else if (int_2 == 0 && list_1.Count == int_3)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("返回缓存中的全部 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_3);
					defaultInterpolatedStringHandler2.AppendLiteral(" 项数据");
					string_1 = defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(19, 3);
					defaultInterpolatedStringHandler3.AppendLiteral("返回缓存中的第 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_2 + 1);
					defaultInterpolatedStringHandler3.AppendLiteral("-");
					defaultInterpolatedStringHandler3.AppendFormatted(int_2 + list_1.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 项数据（共 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_3);
					defaultInterpolatedStringHandler3.AppendLiteral(" 项）");
					string_1 = defaultInterpolatedStringHandler3.ToStringAndClear();
				}
				result = AIToolResult.Ok(string_1, (object)new Class157<string, List<object>, int, int, bool>(string_0, list_1, list_1.Count, int_3, int_2 + list_1.Count < int_3));
			}
			else
			{
				result = AIToolResult.Ok("返回缓存数据: " + string_0, (object)new Class158<string, object>(string_0, object_0));
			}
			int_0 = -2;
			ienumerable_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_cache_data";

	public string Category => "数据管理";

	public string Description => "读取缓存数据，支持字段选择和分页";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（由之前工具返回，如 @1 格式或完整 ID）。⚠️ 使用前请确认：目标工具是否已支持直接使用缓存 ID？如果支持，请优先直接使用缓存 ID，无需调用此工具。\"\n            },\n            \"limit\": {\n                \"type\": \"integer\",\n                \"description\": \"分页大小（可选，默认 50）。控制返回数量。\"\n            },\n            \"offset\": {\n                \"type\": \"integer\",\n                \"description\": \"起始位置（可选，默认 0）。用于分页。\"\n            }\n        },\n        \"required\": [\"cacheId\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class490))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class490 stateMachine = new Class490();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getCacheDataTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class491))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(object object_0, string string_0, int int_0, int int_1)
	{
		Class491 stateMachine = new Class491();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getCacheDataTool_0 = this;
		stateMachine.object_0 = object_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_1 = int_0;
		stateMachine.int_2 = int_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
