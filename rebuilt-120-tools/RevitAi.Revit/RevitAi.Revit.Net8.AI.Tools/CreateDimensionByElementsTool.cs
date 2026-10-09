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

[AITool("create_dimension_by_elements", Category = "注释与标记", Description = "基于元素创建尺寸标注，自动提取几何参考（墙、门窗、管道等）", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateDimensionByElementsTool : IAITool
{
	[CompilerGenerated]
	private sealed class Class391 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateDimensionByElementsTool createDimensionByElementsTool_0;

		private int int_1;

		private IList<object> ilist_0;

		private string string_0;

		private double? nullable_0;

		private IElementService ielementService_0;

		private IAnnotationService iannotationService_0;

		private object object_0;

		private List<int> list_0;

		private List<object> list_1;

		private _003C_003Ef__AnonymousType27<int, int, int> _003C_003Ef__AnonymousType27_0;

		private _003C_003Ef__AnonymousType27<int, int, int> _003C_003Ef__AnonymousType27_1;

		private object object_1;

		private int? nullable_1;

		private string string_1;

		private IEnumerator<object> ienumerator_0;

		private object object_2;

		private int int_2;

		private object object_3;

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
					Class391 stateMachine = this;
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
				int_1 = aitoolContext_0.GetParameter<int>("viewId", 0);
				ilist_0 = aitoolContext_0.GetParameter<IList<object>>("elementIds", (IList<object>)null);
				string_0 = aitoolContext_0.GetParameter<string>("position", (string)null) ?? "auto";
				nullable_0 = aitoolContext_0.GetParameter<double?>("autoOffsetDistance", (double?)null);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				iannotationService_0 = ((revitAdapter2 != null) ? revitAdapter2.AnnotationService : null);
				if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (iannotationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 AnnotationService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
					if (object_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 的视图");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						list_0 = new List<int>();
						list_1 = new List<object>();
						ienumerator_0 = ilist_0.GetEnumerator();
						try
						{
							while (ienumerator_0.MoveNext())
							{
								object_2 = ienumerator_0.Current;
								int_2 = Convert.ToInt32(object_2);
								list_0.Add(int_2);
								object_3 = ielementService_0.GetElementById(aitoolContext_0.Document, int_2);
								if (object_3 != null)
								{
									list_1.Add(object_3);
								}
								object_3 = null;
								object_2 = null;
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
						if (list_1.Count < 2)
						{
							result = AIToolResult.Fail("至少需要2个有效元素才能创建标注");
						}
						else
						{
							_003C_003Ef__AnonymousType27_0 = new _003C_003Ef__AnonymousType27<int, int, int>(0, 0, 0);
							_003C_003Ef__AnonymousType27_1 = new _003C_003Ef__AnonymousType27<int, int, int>(0, 0, 0);
							object_1 = iannotationService_0.CreateDimensionByElements(aitoolContext_0.Document, object_0, (IList<int>)list_0, (object)_003C_003Ef__AnonymousType27_0, (object)_003C_003Ef__AnonymousType27_1, (int?)null, (double?)null, string_0, nullable_0);
							if (object_1 == null)
							{
								result = AIToolResult.Fail("创建尺寸标注失败");
							}
							else
							{
								nullable_1 = ielementService_0.GetElementId(object_1);
								string_1 = ielementService_0.GetElementName(object_0);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("成功在视图 '");
								defaultInterpolatedStringHandler2.AppendFormatted(string_1 ?? "未命名");
								defaultInterpolatedStringHandler2.AppendLiteral("' 中创建尺寸标注（标注了 ");
								defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
								defaultInterpolatedStringHandler2.AppendLiteral(" 个元素）");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class46<int?, int, string, int>(nullable_1, int_1, string_1, list_0.Count));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建尺寸标注失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_dimension_by_elements";

	public string Category => "注释与标记";

	public string Description => "基于元素创建尺寸标注";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"viewId\": {\n                \"type\": \"integer\",\n                \"description\": \"视图 ID\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"要标注的元素 ID 列表（墙、门窗、管道等）\"\n            },\n            \"position\": {\n                \"type\": \"string\",\n                \"description\": \"标注位置（可选）：上、下、左、右。默认为自动选择\",\n                \"enum\": [\"上\", \"下\", \"左\", \"右\", \"auto\"]\n            },\n            \"autoOffsetDistance\": {\n                \"type\": \"number\",\n                \"description\": \"自动检测时标注线的外部偏移距离，单位：毫米（可选，默认2000）\"\n            }\n        },\n        \"required\": [\"viewId\", \"elementIds\"]\n    }";

	[AsyncStateMachine(typeof(Class391))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class391 stateMachine = new Class391();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createDimensionByElementsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
