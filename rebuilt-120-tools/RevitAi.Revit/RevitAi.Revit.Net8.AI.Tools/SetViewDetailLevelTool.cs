using System;
using System.Collections.Generic;
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

[AITool("set_view_detail_level", Category = "视图管理", Description = "设置视图的详细程度（粗略、中等、精细）", RequiresTransaction = true, RequiresModification = true)]
public sealed class SetViewDetailLevelTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class635 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SetViewDetailLevelTool setViewDetailLevelTool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private int int_1;

		private string string_0;

		private Dictionary<string, int> dictionary_0;

		private int int_2;

		private object object_0;

		private string string_1;

		private bool bool_0;

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
					Class635 stateMachine = this;
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
					Logger.Error("[" + setViewDetailLevelTool_0.Name + "] 无法获取 ViewService");
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (ielementService_0 == null)
				{
					Logger.Error("[" + setViewDetailLevelTool_0.Name + "] 无法获取 ElementService");
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					Logger.Error("[" + setViewDetailLevelTool_0.Name + "] 文档对象为空");
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					int_1 = aitoolContext_0.GetParameter<int>("view_id", 0);
					string_0 = aitoolContext_0.GetParameter<string>("detail_level", (string)null);
					if (string.IsNullOrEmpty(string_0))
					{
						Logger.Warning("[" + setViewDetailLevelTool_0.Name + "] detail_level 参数为空");
						result = AIToolResult.Fail("详细程度参数不能为空");
					}
					else
					{
						dictionary_0 = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
						{
							{
								"粗略",
								1
							},
							{
								"中等",
								2
							},
							{
								"精细",
								3
							}
						};
						if (!dictionary_0.TryGetValue(string_0, out int_2))
						{
							Logger.Warning("[" + setViewDetailLevelTool_0.Name + "] 不支持的详细程度: " + string_0);
							result = AIToolResult.Fail("不支持的详细程度: " + string_0 + "。可选值：粗略、中等、精细");
						}
						else
						{
							object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
							if (object_0 == null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[");
								defaultInterpolatedStringHandler.AppendFormatted(setViewDetailLevelTool_0.Name);
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
								bool_0 = iviewService_0.SetViewDetailLevel(object_0, int_2);
								if (!bool_0)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(30, 3);
									defaultInterpolatedStringHandler3.AppendLiteral("[");
									defaultInterpolatedStringHandler3.AppendFormatted(setViewDetailLevelTool_0.Name);
									defaultInterpolatedStringHandler3.AppendLiteral("] 设置视图详细程度失败: viewId=");
									defaultInterpolatedStringHandler3.AppendFormatted(int_1);
									defaultInterpolatedStringHandler3.AppendLiteral(", level=");
									defaultInterpolatedStringHandler3.AppendFormatted(string_0);
									Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
									result = AIToolResult.Fail("设置视图详细程度失败");
								}
								else
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(41, 4);
									defaultInterpolatedStringHandler4.AppendLiteral("[");
									defaultInterpolatedStringHandler4.AppendFormatted(setViewDetailLevelTool_0.Name);
									defaultInterpolatedStringHandler4.AppendLiteral("] 成功设置视图详细程度: viewId=");
									defaultInterpolatedStringHandler4.AppendFormatted(int_1);
									defaultInterpolatedStringHandler4.AppendLiteral(", viewName=");
									defaultInterpolatedStringHandler4.AppendFormatted(string_1);
									defaultInterpolatedStringHandler4.AppendLiteral(", level=");
									defaultInterpolatedStringHandler4.AppendFormatted(string_0);
									Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
									result = AIToolResult.Ok("成功将视图 '" + string_1 + "' 的详细程度设置为 " + string_0, (object)new Class299<int, string, string>(int_1, string_1, string_0));
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[" + setViewDetailLevelTool_0.Name + "] 设置视图详细程度异常: " + exception_0.Message);
				result = AIToolResult.Fail("设置视图详细程度失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "set_view_detail_level";

	public string Category => "视图管理";

	public string Description => "设置视图的详细程度";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"view_id\": {\n                \"type\": \"integer\",\n                \"description\": \"要设置的视图 ID（必选）\"\n            },\n            \"detail_level\": {\n                \"type\": \"string\",\n                \"description\": \"详细程度（必选）。可选值：粗略、中等、精细\",\n                \"enum\": [\"粗略\", \"中等\", \"精细\"]\n            }\n        },\n        \"required\": [\"view_id\", \"detail_level\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class635))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class635 stateMachine = new Class635();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setViewDetailLevelTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
