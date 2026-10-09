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

[AITool("delete_compound_structure_layer", Category = "复合层结构", Description = "从墙、楼板或屋顶类型的复合结构中删除指定层", RequiresTransaction = true, RequiresModification = true)]
public sealed class DeleteCompoundStructureLayerTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class434 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public DeleteCompoundStructureLayerTool deleteCompoundStructureLayerTool_0;

		private IElementService ielementService_0;

		private int int_1;

		private int int_2;

		private object object_0;

		private string string_0;

		private IEnumerable<CompoundLayerInfo> ienumerable_0;

		private List<CompoundLayerInfo> list_0;

		private CompoundLayerInfo compoundLayerInfo_0;

		private string string_1;

		private bool bool_0;

		private double? nullable_0;

		private string string_2;

		private string string_3;

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
					Class434 stateMachine = this;
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
					int_1 = aitoolContext_0.GetParameter<int>("elementTypeId", 0);
					int_2 = aitoolContext_0.GetParameter<int>("layerIndex", 0);
					object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
					if (object_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到类型 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 的元素");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						string_0 = ielementService_0.GetElementCategory(object_0);
						if (string_0 != "墙" && string_0 != "楼板" && string_0 != "屋顶")
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(45, 2);
							defaultInterpolatedStringHandler2.AppendLiteral("元素 '");
							defaultInterpolatedStringHandler2.AppendFormatted(ielementService_0.GetElementName(object_0));
							defaultInterpolatedStringHandler2.AppendLiteral("' 不是墙类型、楼板类型或屋顶类型，而是 '");
							defaultInterpolatedStringHandler2.AppendFormatted(string_0);
							defaultInterpolatedStringHandler2.AppendLiteral("'。复合层结构仅适用于墙、楼板和屋顶。");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
						}
						else
						{
							ienumerable_0 = ielementService_0.GetCompoundStructureLayers(object_0);
							if (ienumerable_0 == null)
							{
								string_3 = ielementService_0.GetElementName(object_0) ?? "未知";
								result = AIToolResult.Fail("元素 '" + string_3 + "' 没有复合层结构。");
							}
							else
							{
								list_0 = ienumerable_0.ToList();
								if (int_2 < 0 || int_2 >= list_0.Count)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(28, 3);
									defaultInterpolatedStringHandler3.AppendLiteral("层索引 ");
									defaultInterpolatedStringHandler3.AppendFormatted(int_2);
									defaultInterpolatedStringHandler3.AppendLiteral(" 超出范围。该元素共有 ");
									defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
									defaultInterpolatedStringHandler3.AppendLiteral(" 层，有效索引为 0-");
									defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count - 1);
									defaultInterpolatedStringHandler3.AppendLiteral("。");
									result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
								}
								else
								{
									compoundLayerInfo_0 = list_0[int_2];
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(5, 2);
									defaultInterpolatedStringHandler4.AppendFormatted(compoundLayerInfo_0.FunctionName);
									defaultInterpolatedStringHandler4.AppendLiteral(" (");
									defaultInterpolatedStringHandler4.AppendFormatted(compoundLayerInfo_0.WidthMM);
									defaultInterpolatedStringHandler4.AppendLiteral("mm)");
									string_1 = defaultInterpolatedStringHandler4.ToStringAndClear();
									if (list_0.Count <= 1)
									{
										result = AIToolResult.Fail("不能删除最后一层。复合结构至少需要保留一层。");
									}
									else
									{
										bool_0 = ielementService_0.DeleteCompoundStructureLayer(object_0, int_2, aitoolContext_0.Document);
										if (!bool_0)
										{
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(10, 1);
											defaultInterpolatedStringHandler5.AppendLiteral("删除复合层 ");
											defaultInterpolatedStringHandler5.AppendFormatted(int_2);
											defaultInterpolatedStringHandler5.AppendLiteral(" 失败。");
											result = AIToolResult.Fail(defaultInterpolatedStringHandler5.ToStringAndClear());
										}
										else
										{
											nullable_0 = ielementService_0.GetCompoundStructureTotalThickness(object_0);
											string_2 = ielementService_0.GetElementName(object_0) ?? "未知";
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(32, 5);
											defaultInterpolatedStringHandler6.AppendLiteral("✅ 成功从 '");
											defaultInterpolatedStringHandler6.AppendFormatted(string_2);
											defaultInterpolatedStringHandler6.AppendLiteral("' 中删除第 ");
											defaultInterpolatedStringHandler6.AppendFormatted(int_2);
											defaultInterpolatedStringHandler6.AppendLiteral(" 层（");
											defaultInterpolatedStringHandler6.AppendFormatted(string_1);
											defaultInterpolatedStringHandler6.AppendLiteral("），剩余 ");
											defaultInterpolatedStringHandler6.AppendFormatted(list_0.Count - 1);
											defaultInterpolatedStringHandler6.AppendLiteral(" 层，新总厚度 ");
											defaultInterpolatedStringHandler6.AppendFormatted(Math.Round(nullable_0.GetValueOrDefault(), 2));
											defaultInterpolatedStringHandler6.AppendLiteral("mm");
											result = AIToolResult.Ok(defaultInterpolatedStringHandler6.ToStringAndClear(), (object)new Class108<int, string, string, int, string, int, double>(int_1, string_2, string_0, int_2, string_1, list_0.Count - 1, Math.Round(nullable_0.GetValueOrDefault(), 2)));
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("删除复合层失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "delete_compound_structure_layer";

	public string Category => "复合层结构";

	public string Description => "从墙、楼板或屋顶类型的复合结构中删除指定层";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elementTypeId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"墙类型、楼板类型或屋顶类型的元素 ID（必选）。可使用 get_family_types 工具获取可用的类型 ID\"\r\n            },\r\n            \"layerIndex\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"要删除的层索引（必选，从 0 开始）。可使用 get_compound_structure_layers 工具查看层的索引\"\r\n            }\r\n        },\r\n        \"required\": [\"elementTypeId\", \"layerIndex\"]\r\n    }";

	[AsyncStateMachine(typeof(Class434))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class434 stateMachine = new Class434();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.deleteCompoundStructureLayerTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
