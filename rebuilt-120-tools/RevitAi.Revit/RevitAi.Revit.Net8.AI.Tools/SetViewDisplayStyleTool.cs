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

[AITool("set_view_display_style", Category = "视图管理", Description = "设置视图的视觉样式（线框、隐藏线、着色、着色并显示边、真实等）", RequiresTransaction = true, RequiresModification = true)]
public sealed class SetViewDisplayStyleTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class636 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SetViewDisplayStyleTool setViewDisplayStyleTool_0;

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
					Class636 stateMachine = this;
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
					Logger.Error("[" + setViewDisplayStyleTool_0.Name + "] 无法获取 ViewService");
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (ielementService_0 == null)
				{
					Logger.Error("[" + setViewDisplayStyleTool_0.Name + "] 无法获取 ElementService");
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					Logger.Error("[" + setViewDisplayStyleTool_0.Name + "] 文档对象为空");
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					int_1 = aitoolContext_0.GetParameter<int>("view_id", 0);
					string_0 = aitoolContext_0.GetParameter<string>("display_style", (string)null);
					if (string.IsNullOrEmpty(string_0))
					{
						Logger.Warning("[" + setViewDisplayStyleTool_0.Name + "] display_style 参数为空");
						result = AIToolResult.Fail("视觉样式参数不能为空");
					}
					else
					{
						dictionary_0 = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
						{
							{
								"线框",
								1
							},
							{
								"隐藏线",
								2
							},
							{
								"着色",
								3
							},
							{
								"着色并显示边",
								4
							},
							{
								"渲染",
								5
							},
							{
								"真实",
								6
							},
							{
								"平面颜色",
								7
							},
							{
								"真实并显示边",
								8
							}
						};
						if (!dictionary_0.TryGetValue(string_0, out int_2))
						{
							Logger.Warning("[" + setViewDisplayStyleTool_0.Name + "] 不支持的视觉样式: " + string_0);
							result = AIToolResult.Fail("不支持的视觉样式: " + string_0 + "。可选值：线框、隐藏线、着色、着色并显示边、渲染、真实、平面颜色、真实并显示边");
						}
						else
						{
							object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
							if (object_0 == null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[");
								defaultInterpolatedStringHandler.AppendFormatted(setViewDisplayStyleTool_0.Name);
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
								bool_0 = iviewService_0.SetViewDisplayStyle(object_0, int_2);
								if (!bool_0)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(30, 3);
									defaultInterpolatedStringHandler3.AppendLiteral("[");
									defaultInterpolatedStringHandler3.AppendFormatted(setViewDisplayStyleTool_0.Name);
									defaultInterpolatedStringHandler3.AppendLiteral("] 设置视图视觉样式失败: viewId=");
									defaultInterpolatedStringHandler3.AppendFormatted(int_1);
									defaultInterpolatedStringHandler3.AppendLiteral(", style=");
									defaultInterpolatedStringHandler3.AppendFormatted(string_0);
									Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
									result = AIToolResult.Fail("设置视图视觉样式失败");
								}
								else
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(41, 4);
									defaultInterpolatedStringHandler4.AppendLiteral("[");
									defaultInterpolatedStringHandler4.AppendFormatted(setViewDisplayStyleTool_0.Name);
									defaultInterpolatedStringHandler4.AppendLiteral("] 成功设置视图视觉样式: viewId=");
									defaultInterpolatedStringHandler4.AppendFormatted(int_1);
									defaultInterpolatedStringHandler4.AppendLiteral(", viewName=");
									defaultInterpolatedStringHandler4.AppendFormatted(string_1);
									defaultInterpolatedStringHandler4.AppendLiteral(", style=");
									defaultInterpolatedStringHandler4.AppendFormatted(string_0);
									Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
									result = AIToolResult.Ok("成功将视图 '" + string_1 + "' 的视觉样式设置为 " + string_0, (object)new Class300<int, string, string>(int_1, string_1, string_0));
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[" + setViewDisplayStyleTool_0.Name + "] 设置视图视觉样式异常: " + exception_0.Message);
				result = AIToolResult.Fail("设置视图视觉样式失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "set_view_display_style";

	public string Category => "视图管理";

	public string Description => "设置视图的视觉样式";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"view_id\": {\n                \"type\": \"integer\",\n                \"description\": \"要设置的视图 ID（必选）\"\n            },\n            \"display_style\": {\n                \"type\": \"string\",\n                \"description\": \"视觉样式（必选）。可选值：线框、隐藏线、着色、着色并显示边、渲染、真实、平面颜色、真实并显示边\",\n                \"enum\": [\"线框\", \"隐藏线\", \"着色\", \"着色并显示边\", \"渲染\", \"真实\", \"平面颜色\", \"真实并显示边\"]\n            }\n        },\n        \"required\": [\"view_id\", \"display_style\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class636))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class636 stateMachine = new Class636();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setViewDisplayStyleTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
