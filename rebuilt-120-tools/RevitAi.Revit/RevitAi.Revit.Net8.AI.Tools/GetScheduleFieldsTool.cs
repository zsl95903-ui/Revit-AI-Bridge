using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using Autodesk.Revit.DB;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_schedule_fields", Category = "视图查询", Description = "获取已存在明细表的所有可调度字段列表。", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetScheduleFieldsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class507 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetScheduleFieldsTool getScheduleFieldsTool_0;

		private string string_0;

		private string string_1;

		private object object_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private int int_1;

		private FilteredElementCollector filteredElementCollector_0;

		private IEnumerator<Element> ienumerator_0;

		private ViewSchedule viewSchedule_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Expected O, but got Unknown
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Expected O, but got Unknown
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
					Class507 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("scheduleId", (string)null);
				string_1 = aitoolContext_0.GetParameter<string>("scheduleName", (string)null);
				if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					if (((revitAdapter != null) ? revitAdapter.ViewService : null) == null)
					{
						result = AIToolResult.Fail("无法获取 ViewService");
					}
					else
					{
						IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
						if (((revitAdapter2 != null) ? revitAdapter2.ElementService : null) == null)
						{
							result = AIToolResult.Fail("无法获取 ElementService");
						}
						else
						{
							object_0 = null;
							if (!string.IsNullOrEmpty(string_0))
							{
								if (int.TryParse(string_0, out int_1))
								{
									object_0 = aitoolContext_0.RevitAdapter.ElementService.GetElementById(aitoolContext_0.Document, int_1);
									goto IL_0272;
								}
								result = AIToolResult.Fail("无效的元素 ID: " + string_0);
							}
							else
							{
								if (!string.IsNullOrEmpty(string_1))
								{
									object document = aitoolContext_0.Document;
									filteredElementCollector_0 = new FilteredElementCollector((Document)((document is Document) ? document : null));
									filteredElementCollector_0.OfClass(typeof(ViewSchedule));
									ienumerator_0 = filteredElementCollector_0.GetEnumerator();
									try
									{
										while (ienumerator_0.MoveNext())
										{
											viewSchedule_0 = (ViewSchedule)ienumerator_0.Current;
											if (!string.Equals(((Element)viewSchedule_0).Name, string_1, StringComparison.OrdinalIgnoreCase))
											{
												viewSchedule_0 = null;
												continue;
											}
											object_0 = viewSchedule_0;
											break;
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
									filteredElementCollector_0 = null;
									goto IL_0272;
								}
								result = AIToolResult.Fail("必须提供 scheduleId 或 scheduleName 参数");
							}
						}
					}
				}
				goto end_IL_0067;
				IL_0272:
				if (object_0 == null)
				{
					result = AIToolResult.Fail("找不到明细表: " + (string_0 ?? string_1));
				}
				else
				{
					ienumerable_0 = aitoolContext_0.RevitAdapter.ViewService.GetScheduleFields(object_0);
					if (ienumerable_0 == null || !ienumerable_0.Any())
					{
						result = AIToolResult.Fail("明细表没有可调度字段");
					}
					else
					{
						list_0 = ienumerable_0.ToList();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
						defaultInterpolatedStringHandler.AppendLiteral("成功获取明细表的可调度字段，共 ");
						defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个字段");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class200<string, string, int, List<object>>(string_0 ?? "", string_1 ?? "", list_0.Count, list_0));
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取明细表字段失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_schedule_fields";

	public string Category => "视图查询";

	public string Description => "获取已存在明细表的所有可调度字段列表";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"scheduleId\": {\n                \"type\": \"string\",\n                \"description\": \"明细表的元素 ID（可选，优先使用）\"\n            },\n            \"scheduleName\": {\n                \"type\": \"string\",\n                \"description\": \"明细表的名称（可选，如果不提供 scheduleId 则使用此参数）\"\n            }\n        }\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class507))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class507 stateMachine = new Class507();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getScheduleFieldsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
