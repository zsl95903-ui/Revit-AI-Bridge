using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_category_visibility", Category = "视图管理", Description = "获取视图中类别的可见性状态（可查询单个类别或获取所有类别）", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetCategoryVisibilityTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class492 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetCategoryVisibilityTool getCategoryVisibilityTool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private int int_1;

		private string string_0;

		private object object_0;

		private string string_1;

		private bool? nullable_0;

		private string string_2;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

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
					Class492 stateMachine = this;
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
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				if (iviewService_0 == null)
				{
					Logger.Error("[" + getCategoryVisibilityTool_0.Name + "] 无法获取 ViewService");
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (ielementService_0 == null)
				{
					Logger.Error("[" + getCategoryVisibilityTool_0.Name + "] 无法获取 ElementService");
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					Logger.Error("[" + getCategoryVisibilityTool_0.Name + "] 文档对象为空");
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					int_1 = aitoolContext_0.GetParameter<int>("view_id", 0);
					string_0 = aitoolContext_0.GetParameter<string>("category_name", (string)null);
					object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
					if (object_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[");
						defaultInterpolatedStringHandler.AppendFormatted(getCategoryVisibilityTool_0.Name);
						defaultInterpolatedStringHandler.AppendLiteral("] 找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 的视图");
						Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_1);
						defaultInterpolatedStringHandler2.AppendLiteral(" 的视图");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
					}
					else
					{
						string_1 = iviewService_0.GetViewName(object_0);
						if (!string.IsNullOrEmpty(string_0))
						{
							nullable_0 = iviewService_0.GetCategoryVisibility(aitoolContext_0.Document, object_0, string_0);
							if (!nullable_0.HasValue)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(17, 2);
								defaultInterpolatedStringHandler3.AppendLiteral("[");
								defaultInterpolatedStringHandler3.AppendFormatted(getCategoryVisibilityTool_0.Name);
								defaultInterpolatedStringHandler3.AppendLiteral("] 无法获取类别 '");
								defaultInterpolatedStringHandler3.AppendFormatted(string_0);
								defaultInterpolatedStringHandler3.AppendLiteral("' 的可见性");
								Logger.Warning(defaultInterpolatedStringHandler3.ToStringAndClear());
								result = AIToolResult.Fail("无法获取类别 '" + string_0 + "' 的可见性。请检查类别名称是否正确。");
							}
							else
							{
								string_2 = (nullable_0.Value ? "可见" : "隐藏");
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(53, 5);
								defaultInterpolatedStringHandler4.AppendLiteral("[");
								defaultInterpolatedStringHandler4.AppendFormatted(getCategoryVisibilityTool_0.Name);
								defaultInterpolatedStringHandler4.AppendLiteral("] 成功获取类别可见性: viewId=");
								defaultInterpolatedStringHandler4.AppendFormatted(int_1);
								defaultInterpolatedStringHandler4.AppendLiteral(", viewName=");
								defaultInterpolatedStringHandler4.AppendFormatted(string_1);
								defaultInterpolatedStringHandler4.AppendLiteral(", category=");
								defaultInterpolatedStringHandler4.AppendFormatted(string_0);
								defaultInterpolatedStringHandler4.AppendLiteral(", visible=");
								defaultInterpolatedStringHandler4.AppendFormatted(nullable_0.Value);
								Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(17, 3);
								defaultInterpolatedStringHandler5.AppendLiteral("在视图 '");
								defaultInterpolatedStringHandler5.AppendFormatted(string_1);
								defaultInterpolatedStringHandler5.AppendLiteral("' 中，类别 '");
								defaultInterpolatedStringHandler5.AppendFormatted(string_0);
								defaultInterpolatedStringHandler5.AppendLiteral("' 当前");
								defaultInterpolatedStringHandler5.AppendFormatted(string_2);
								result = AIToolResult.Ok(defaultInterpolatedStringHandler5.ToStringAndClear(), (object)new Class159<int, string, string, bool>(int_1, string_1, string_0, nullable_0.Value));
							}
						}
						else
						{
							ienumerable_0 = iviewService_0.GetAllCategoriesVisibility(aitoolContext_0.Document, object_0);
							list_0 = ienumerable_0.ToList();
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(42, 4);
							defaultInterpolatedStringHandler6.AppendLiteral("[");
							defaultInterpolatedStringHandler6.AppendFormatted(getCategoryVisibilityTool_0.Name);
							defaultInterpolatedStringHandler6.AppendLiteral("] 成功获取所有类别可见性: viewId=");
							defaultInterpolatedStringHandler6.AppendFormatted(int_1);
							defaultInterpolatedStringHandler6.AppendLiteral(", viewName=");
							defaultInterpolatedStringHandler6.AppendFormatted(string_1);
							defaultInterpolatedStringHandler6.AppendLiteral(", count=");
							defaultInterpolatedStringHandler6.AppendFormatted(list_0.Count);
							Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(22, 2);
							defaultInterpolatedStringHandler7.AppendLiteral("成功获取视图 '");
							defaultInterpolatedStringHandler7.AppendFormatted(string_1);
							defaultInterpolatedStringHandler7.AppendLiteral("' 中 ");
							defaultInterpolatedStringHandler7.AppendFormatted(list_0.Count);
							defaultInterpolatedStringHandler7.AppendLiteral(" 个类别的可见性状态");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler7.ToStringAndClear(), (object)new Class160<int, string, int, List<object>>(int_1, string_1, list_0.Count, list_0));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[" + getCategoryVisibilityTool_0.Name + "] 获取类别可见性异常: " + exception_0.Message);
				result = AIToolResult.Fail("获取类别可见性失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_category_visibility";

	public string Category => "视图管理";

	public string Description => "获取视图中类别的可见性状态（可查询单个类别或获取所有类别）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"view_id\": {\n                \"type\": \"integer\",\n                \"description\": \"视图 ID（必选）\"\n            },\n            \"category_name\": {\n                \"type\": \"string\",\n                \"description\": \"类别名称，如 墙、门、窗 等。支持子类别，格式：父类别/子类别（如：场地/项目基点）或直接使用子类别名称（如：项目基点）（可选）。如果指定，只返回该类别的可见性；如果不指定，返回所有类别的可见性\"\n            }\n        },\n        \"required\": [\"view_id\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class492))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class492 stateMachine = new Class492();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getCategoryVisibilityTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
