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

[AITool("get_roof_types", Category = "元素查询", Description = "获取文档中所有可用的屋面类型，用于创建屋面时指定类型", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetRoofTypesTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class505 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetRoofTypesTool getRoofTypesTool_0;

		private IElementService ielementService_0;

		private List<object> list_0;

		private List<object> list_1;

		private List<object>.Enumerator enumerator_0;

		private object object_0;

		private int? nullable_0;

		private string string_0;

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
					Class505 stateMachine = this;
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
					list_0 = ielementService_0.GetRoofTypes(aitoolContext_0.Document).ToList();
					if (list_0.Count == 0)
					{
						result = AIToolResult.Ok("当前文档没有可用的屋面类型", (object)new Class196<int, object[]>(0, Array.Empty<object>()));
					}
					else
					{
						list_1 = new List<object>();
						enumerator_0 = list_0.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								object_0 = enumerator_0.Current;
								nullable_0 = ielementService_0.GetElementId(object_0);
								string_0 = ielementService_0.GetElementName(object_0);
								list_1.Add(new Class190<int?, string>(nullable_0, string_0 ?? "未命名"));
								string_0 = null;
								object_0 = null;
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
							}
						}
						enumerator_0 = default(List<object>.Enumerator);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
						defaultInterpolatedStringHandler.AppendLiteral("共找到 ");
						defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个屋面类型");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class196<int, List<object>>(list_1.Count, list_1));
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取屋面类型失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_roof_types";

	public string Category => "元素查询";

	public string Description => "获取所有可用的屋面类型";

	public string ParametersSchema => "{ \"type\": \"object\", \"properties\": {} }";

	[AsyncStateMachine(typeof(Class505))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class505 stateMachine = new Class505();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getRoofTypesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
