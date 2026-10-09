using System;
using System.Diagnostics;
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

[AITool("set_category_visibility", Category = "视图管理", Description = "设置指定类别在视图中的可见性（显示或隐藏）", RequiresTransaction = true, RequiresModification = true)]
public sealed class SetCategoryVisibilityTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class627 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SetCategoryVisibilityTool setCategoryVisibilityTool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private int int_1;

		private string string_0;

		private bool bool_0;

		private object object_0;

		private string string_1;

		private bool bool_1;

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
					Class627 stateMachine = this;
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
					Logger.Error("[" + setCategoryVisibilityTool_0.Name + "] 无法获取 ViewService");
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (ielementService_0 == null)
				{
					Logger.Error("[" + setCategoryVisibilityTool_0.Name + "] 无法获取 ElementService");
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					Logger.Error("[" + setCategoryVisibilityTool_0.Name + "] 文档对象为空");
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					int_1 = aitoolContext_0.GetParameter<int>("view_id", 0);
					string_0 = aitoolContext_0.GetParameter<string>("category_name", (string)null);
					bool_0 = aitoolContext_0.GetParameter<bool>("visible", false);
					if (string.IsNullOrEmpty(string_0))
					{
						Logger.Warning("[" + setCategoryVisibilityTool_0.Name + "] category_name 参数为空");
						result = AIToolResult.Fail("类别名称参数不能为空");
					}
					else
					{
						object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
						if (object_0 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
							defaultInterpolatedStringHandler.AppendLiteral("[");
							defaultInterpolatedStringHandler.AppendFormatted(setCategoryVisibilityTool_0.Name);
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
							bool_1 = iviewService_0.SetCategoryVisibility(aitoolContext_0.Document, object_0, string_0, bool_0);
							if (!bool_1)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(42, 4);
								defaultInterpolatedStringHandler3.AppendLiteral("[");
								defaultInterpolatedStringHandler3.AppendFormatted(setCategoryVisibilityTool_0.Name);
								defaultInterpolatedStringHandler3.AppendLiteral("] 设置类别可见性失败: viewId=");
								defaultInterpolatedStringHandler3.AppendFormatted(int_1);
								defaultInterpolatedStringHandler3.AppendLiteral(", category=");
								defaultInterpolatedStringHandler3.AppendFormatted(string_0);
								defaultInterpolatedStringHandler3.AppendLiteral(", visible=");
								defaultInterpolatedStringHandler3.AppendFormatted(bool_0);
								Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
								result = AIToolResult.Fail("设置类别可见性失败。请检查类别名称是否正确，以及该类别是否允许在当前视图中控制可见性。");
							}
							else
							{
								string_2 = (bool_0 ? "显示" : "隐藏");
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(53, 5);
								defaultInterpolatedStringHandler4.AppendLiteral("[");
								defaultInterpolatedStringHandler4.AppendFormatted(setCategoryVisibilityTool_0.Name);
								defaultInterpolatedStringHandler4.AppendLiteral("] 成功设置类别可见性: viewId=");
								defaultInterpolatedStringHandler4.AppendFormatted(int_1);
								defaultInterpolatedStringHandler4.AppendLiteral(", viewName=");
								defaultInterpolatedStringHandler4.AppendFormatted(string_1);
								defaultInterpolatedStringHandler4.AppendLiteral(", category=");
								defaultInterpolatedStringHandler4.AppendFormatted(string_0);
								defaultInterpolatedStringHandler4.AppendLiteral(", visible=");
								defaultInterpolatedStringHandler4.AppendFormatted(bool_0);
								Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(15, 3);
								defaultInterpolatedStringHandler5.AppendLiteral("成功在视图 '");
								defaultInterpolatedStringHandler5.AppendFormatted(string_1);
								defaultInterpolatedStringHandler5.AppendLiteral("' 中");
								defaultInterpolatedStringHandler5.AppendFormatted(string_2);
								defaultInterpolatedStringHandler5.AppendLiteral("类别 '");
								defaultInterpolatedStringHandler5.AppendFormatted(string_0);
								defaultInterpolatedStringHandler5.AppendLiteral("'");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler5.ToStringAndClear(), (object)new Class159<int, string, string, bool>(int_1, string_1, string_0, bool_0));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[" + setCategoryVisibilityTool_0.Name + "] 设置类别可见性异常: " + exception_0.Message);
				result = AIToolResult.Fail("设置类别可见性失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "set_category_visibility";

	public string Category => "视图管理";

	public string Description => "设置指定类别在视图中的可见性（显示或隐藏）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"view_id\": {\n                \"type\": \"integer\",\n                \"description\": \"视图 ID（必选）\"\n            },\n            \"category_name\": {\n                \"type\": \"string\",\n                \"description\": \"类别名称，如 墙、门、窗、管道、桥架 等。支持子类别，格式：父类别/子类别（如：场地/项目基点）或直接使用子类别名称（如：项目基点）（必选）\"\n            },\n            \"visible\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否可见（必选）。true=显示，false=隐藏\"\n            }\n        },\n        \"required\": [\"view_id\", \"category_name\", \"visible\"]\n    }";

	[AsyncStateMachine(typeof(Class627))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class627 stateMachine = new Class627();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setCategoryVisibilityTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
