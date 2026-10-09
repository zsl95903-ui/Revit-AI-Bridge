using System;
using System.Collections.Generic;
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

[AITool("get_all_levels", Category = "标高查询", Description = "获取文档中所有标高的列表，包括标高 ID、名称和高度值。返回单位：米(m)", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetAllLevelsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class483 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetAllLevelsTool getAllLevelsTool_0;

		private ILevelService ilevelService_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private IEnumerator<object> ienumerator_0;

		private object object_0;

		private int? nullable_0;

		private string string_0;

		private double? nullable_1;

		private double double_0;

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
					Class483 stateMachine = this;
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
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ilevelService_0 = ((revitAdapter != null) ? revitAdapter.LevelService : null);
				if (ilevelService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 LevelService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					ienumerable_0 = ilevelService_0.GetAllLevels(aitoolContext_0.Document);
					list_0 = new List<object>();
					ienumerator_0 = ienumerable_0.GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							object_0 = ienumerator_0.Current;
							nullable_0 = ilevelService_0.GetLevelId(object_0);
							if (nullable_0.HasValue)
							{
								string_0 = ilevelService_0.GetLevelName(object_0);
								nullable_1 = ilevelService_0.GetLevelElevation(object_0);
								double_0 = nullable_1.GetValueOrDefault() * 0.3048;
								list_0.Add(new Class145<int?, string, double>(nullable_0, string_0 ?? "未命名", Math.Round(double_0, 3)));
								string_0 = null;
								object_0 = null;
							}
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
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
					defaultInterpolatedStringHandler.AppendLiteral("✅ 成功获取 ");
					defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个标高");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class146<List<object>, string, int>(list_0, "meters", list_0.Count));
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取标高列表失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_all_levels";

	public string Category => "标高查询";

	public string Description => "获取文档中所有标高的列表，包括标高 ID、名称和高度值。返回单位：米(m)";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {},\r\n        \"required\": []\r\n    }";

	[AsyncStateMachine(typeof(Class483))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class483 stateMachine = new Class483();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getAllLevelsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
