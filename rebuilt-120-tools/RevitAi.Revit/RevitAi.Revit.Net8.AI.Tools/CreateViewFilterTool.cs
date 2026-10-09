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

[AITool("create_view_filter", Category = "视图高级操作", Description = "创建视图过滤器（文档级别的对象）。⚠\ufe0f 重要：创建前强烈建议先使用 get_available_filter_parameters 工具查询指定类别的可用参数列表，以获取准确的参数名称和类型。创建后需要使用 set_view_filter 工具将其应用到视图并设置颜色。", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateViewFilterTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class425 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateViewFilterTool createViewFilterTool_0;

		private string string_0;

		private List<string> list_0;

		private List<object> list_1;

		private IViewService iviewService_0;

		private int? nullable_0;

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
					Class425 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("filterName", (string)null);
				list_0 = aitoolContext_0.GetParameter<List<string>>("categoryNames", (List<string>)null);
				list_1 = aitoolContext_0.GetParameter<List<object>>("rules", (List<object>)null);
				if (string.IsNullOrEmpty(string_0))
				{
					result = AIToolResult.Fail("过滤器名称不能为空");
				}
				else if (list_0 == null || list_0.Count == 0)
				{
					result = AIToolResult.Fail("类别名称数组不能为空");
				}
				else
				{
					if (list_1 == null || list_1.Count == 0)
					{
						Logger.Warning("[CreateViewFilterTool] 未提供过滤规则，将创建包含所有元素的过滤器");
						list_1 = new List<object>
						{
							new Class96<string, string, int>("Id", "greater", 0)
						};
					}
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
					if (iviewService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ViewService");
					}
					else if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						nullable_0 = iviewService_0.CreateViewFilter(aitoolContext_0.Document, string_0, (IEnumerable<string>)list_0, (IEnumerable<object>)list_1);
						if (!nullable_0.HasValue)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 3);
							defaultInterpolatedStringHandler.AppendLiteral("[CreateViewFilterTool] 创建过滤器 '");
							defaultInterpolatedStringHandler.AppendFormatted(string_0);
							defaultInterpolatedStringHandler.AppendLiteral("' 失败 - 类别: [");
							defaultInterpolatedStringHandler.AppendFormatted(string.Join(", ", list_0));
							defaultInterpolatedStringHandler.AppendLiteral("], 规则数: ");
							defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
							Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
							result = AIToolResult.Fail("创建过滤器 '" + string_0 + "' 失败。请检查类别名称和参数是否正确，建议先使用 get_available_filter_parameters 工具查询可用参数。");
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(56, 2);
							defaultInterpolatedStringHandler2.AppendLiteral("成功创建视图过滤器 '");
							defaultInterpolatedStringHandler2.AppendFormatted(string_0);
							defaultInterpolatedStringHandler2.AppendLiteral("' (ID: ");
							defaultInterpolatedStringHandler2.AppendFormatted(nullable_0);
							defaultInterpolatedStringHandler2.AppendLiteral(")。使用 set_view_filter 工具将过滤器应用到视图并设置颜色。");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)null);
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[CreateViewFilterTool] 创建视图过滤器失败: " + exception_0.Message);
				result = AIToolResult.Fail("创建视图过滤器失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_view_filter";

	public string Category => "视图高级操作";

	public string Description => "创建视图过滤器";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"filterName\": {\n                \"type\": \"string\",\n                \"description\": \"过滤器名称（唯一标识）\"\n            },\n            \"categoryNames\": {\n                \"type\": \"array\",\n                \"items\": {\n                    \"type\": \"string\"\n                },\n                \"description\": \"类别名称数组（中文），例如：墙、门、窗等。必须使用与 Revit 界面一致的中文类别名称。\"\n            },\n            \"rules\": {\n                \"type\": \"array\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"parameterName\": {\n                            \"description\": \"参数名称（中文）或参数ID（整数）。⚠️ 优先使用 get_available_filter_parameters 工具返回的 parameterId 字段（整数）。例如：如果工具返回 parameterId 为 -1140334 且 displayName 为 '系统类型' 时，请使用 -1140334。\"\n                        },\n                        \"operator\": {\n                            \"type\": \"string\",\n                            \"description\": \"操作符。支持：equals（等于）、not_equals（不等于）、greater（大于）、greater_or_equal（大于等于）、less（小于）、less_or_equal（小于等于）、contains（包含）、begins（开始于）、ends（结束于）。⚠️ 重要：对于 ElementId 类型参数（如系统类型、材质等），请使用 contains 而不是 equals，因为 equals 无法在视图中正确渲染。\",\n                            \"enum\": [\"equals\", \"not_equals\", \"greater\", \"greater_or_equal\", \"less\", \"less_or_equal\", \"contains\", \"begins\", \"ends\"]\n                        },\n                        \"value\": {\n                            \"description\": \"参数值（数字、字符串或元素 ID）。⚠️ ElementId类型参数请使用元素名称（如 '废水'）而非ID。字符串参数需指定具体的匹配文本，例如：'结构柱'、'混凝土'等。\"\n                        },\n                        \"unit\": {\n                            \"type\": \"string\",\n                            \"description\": \"值单位（数值过滤时必须指定！）。长度用 mm/cm/m/ft/in，面积用 m2/ft2，体积用 m3/ft3，角度用 deg/rad。例如：unit='mm' 将 2000mm 转换为 Revit 内部英尺。字符串过滤不需要此字段。\"\n                        }\n                    },\n                    \"required\": [\"parameterName\", \"operator\", \"value\"]\n                },\n                \"description\": \"过滤规则数组。数值过滤时每条规则建议填写 unit 字段。示例: {\\\"parameterName\\\":\\\"宽度\\\",\\\"operator\\\":\\\"greater\\\",\\\"value\\\":2000,\\\"unit\\\":\\\"mm\\\"}\"\n            }\n        },\n        \"required\": [\"filterName\", \"categoryNames\", \"rules\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class425))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class425 stateMachine = new Class425();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createViewFilterTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
