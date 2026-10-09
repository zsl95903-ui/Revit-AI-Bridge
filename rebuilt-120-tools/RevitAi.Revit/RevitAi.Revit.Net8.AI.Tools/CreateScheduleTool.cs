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

[AITool("create_schedule", Category = "视图高级操作", Description = "创建明细表（如门窗表、材料表等）。\r\n\r\n工作流程：\r\n1. 如果用户想查看可用字段，先调用 create_schedule 创建一个临时明细表（不指定字段）\r\n2. 然后调用 get_schedule_fields 获取该明细表的所有可用字段\r\n3. 根据用户需求，再次调用 create_schedule 创建最终明细表，并指定所需字段\r\n\r\n示例：\r\n- 创建临时明细表查看字段：category='结构柱'，不指定 fieldUniqueIds\r\n- 创建带字段的明细表：category='结构柱'，fieldUniqueIds=['-1002000', '-1002002']\r\n\r\n⚠\ufe0f 注意：必须使用与 Revit 界面中完全一致的中文类别名称", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateScheduleTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class415 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateScheduleTool createScheduleTool_0;

		private string string_0;

		private string string_1;

		private IList<string> ilist_0;

		private string string_2;

		private string string_3;

		private string string_4;

		private string string_5;

		private string string_6;

		private string string_7;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private Class76<IList<string>, string, string, string, string, string, string> class76_0;

		private object object_0;

		private int? nullable_0;

		private string string_8;

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
					Class415 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("category", (string)null);
				string_1 = aitoolContext_0.GetParameter<string>("scheduleName", (string)null);
				ilist_0 = aitoolContext_0.GetParameter<IList<string>>("fieldUniqueIds", (IList<string>)null);
				string_2 = aitoolContext_0.GetParameter<string>("sortByUniqueId", (string)null);
				string_3 = aitoolContext_0.GetParameter<string>("sortOrder", "ascending");
				string_4 = aitoolContext_0.GetParameter<string>("filterFieldUniqueId", (string)null);
				string_5 = aitoolContext_0.GetParameter<string>("filterType", "contains");
				string_6 = aitoolContext_0.GetParameter<string>("filterValue", (string)null);
				string_7 = aitoolContext_0.GetParameter<string>("groupByUniqueId", (string)null);
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
					class76_0 = new Class76<IList<string>, string, string, string, string, string, string>(ilist_0, string_2, string_3, string_4, string_5, string_6, string_7);
					object_0 = iviewService_0.CreateScheduleAdvanced(aitoolContext_0.Document, string_0, string_1 ?? (string_0 + " 明细表"), (object)class76_0);
					if (object_0 == null)
					{
						result = AIToolResult.Fail("创建 " + string_0 + " 明细表失败");
					}
					else
					{
						nullable_0 = ielementService_0.GetElementId(object_0);
						string_8 = ielementService_0.GetElementName(object_0);
						string text = "成功创建明细表: ";
						string obj = string_8 ?? "未命名";
						string text2;
						if (ilist_0 != null && ilist_0.Count > 0)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
							defaultInterpolatedStringHandler.AppendLiteral("（包含 ");
							defaultInterpolatedStringHandler.AppendFormatted(ilist_0.Count);
							defaultInterpolatedStringHandler.AppendLiteral(" 个字段）");
							text2 = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						else
						{
							text2 = "";
						}
						result = AIToolResult.Ok(text + obj + text2, (object)new Class77<int?, string, string, int>(nullable_0, string_8, string_0, ilist_0?.Count ?? 0));
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建明细表失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_schedule";

	public string Category => "视图高级操作";

	public string Description => "创建明细表（支持字段、排序、过滤）";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"category\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"要创建明细表的类别（如 Doors, Windows, Walls, Rooms, 结构柱 等），使用 'MultiCategory' 创建多类别明细表。⚠️ 注意：必须使用与 Revit 界面中完全一致的中文类别名称\"\r\n            },\r\n            \"scheduleName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"明细表名称（可选，默认为 '{category} 明细表'）\"\r\n            },\r\n            \"fieldUniqueIds\": {\r\n                \"type\": \"array\",\r\n                \"items\": { \"type\": \"string\" },\r\n                \"description\": \"要添加的字段 UniqueId 列表（可选）。⚠️ 重要：这些 ID 必须从 get_schedule_fields 工具返回的 uniqueId 字段获取。例如：['-1002000', '-1002002', '-1002003']。如果不指定，将创建空明细表，然后可以调用 get_schedule_fields 查看可用字段\"\r\n            },\r\n            \"sortByUniqueId\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"排序字段的 UniqueId（可选）。⚠️ 必须是 get_schedule_fields 返回的 uniqueId 之一\"\r\n            },\r\n            \"sortOrder\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"排序方向：'ascending'（升序）或 'descending'（降序），默认升序\",\r\n                \"enum\": [\"ascending\", \"descending\"],\r\n                \"default\": \"ascending\"\r\n            },\r\n            \"filterFieldUniqueId\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"过滤字段的 UniqueId（可选，与 filterValue 配合使用）。⚠️ 必须是 get_schedule_fields 返回的 uniqueId 之一\"\r\n            },\r\n            \"filterType\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"过滤类型：'equal', 'greater_than', 'less_than', 'contains'（默认）\",\r\n                \"enum\": [\"equal\", \"greater_than\", \"less_than\", \"contains\"],\r\n                \"default\": \"contains\"\r\n            },\r\n            \"filterValue\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"过滤值（可选，与 filterFieldUniqueId 配合使用）\"\r\n            },\r\n            \"groupByUniqueId\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"分组字段的 UniqueId（可选，指定后该字段将显示分组标题）。⚠️ 必须是 get_schedule_fields 返回的 uniqueId 之一\"\r\n            }\r\n        },\r\n        \"required\": [\"category\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class415))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class415 stateMachine = new Class415();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createScheduleTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
