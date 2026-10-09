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

[AITool("modify_compound_structure_layer", Category = "复合层结构", Description = "修改墙、楼板或屋顶类型的指定复合层的厚度、材质或功能。所有厚度单位为毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class ModifyCompoundStructureLayerTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class611 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ModifyCompoundStructureLayerTool modifyCompoundStructureLayerTool_0;

		private IElementService ielementService_0;

		private int int_1;

		private int int_2;

		private double? nullable_0;

		private int? nullable_1;

		private string string_0;

		private string string_1;

		private object object_0;

		private string string_2;

		private IEnumerable<CompoundLayerInfo> ienumerable_0;

		private List<CompoundLayerInfo> list_0;

		private int? nullable_2;

		private bool bool_0;

		private List<string> list_1;

		private string string_3;

		private string string_4;

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
					Class611 stateMachine = this;
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
					nullable_0 = aitoolContext_0.GetParameter<double?>("thicknessMM", (double?)null);
					nullable_1 = aitoolContext_0.GetParameter<int?>("materialId", (int?)null);
					string_0 = aitoolContext_0.GetParameter<string>("materialName", (string)null);
					string_1 = aitoolContext_0.GetParameter<string>("function", (string)null);
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
						string_2 = ielementService_0.GetElementCategory(object_0);
						if (string_2 != "墙" && string_2 != "楼板" && string_2 != "屋顶")
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(45, 2);
							defaultInterpolatedStringHandler2.AppendLiteral("元素 '");
							defaultInterpolatedStringHandler2.AppendFormatted(ielementService_0.GetElementName(object_0));
							defaultInterpolatedStringHandler2.AppendLiteral("' 不是墙类型、楼板类型或屋顶类型，而是 '");
							defaultInterpolatedStringHandler2.AppendFormatted(string_2);
							defaultInterpolatedStringHandler2.AppendLiteral("'。复合层结构仅适用于墙、楼板和屋顶。");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
						}
						else
						{
							ienumerable_0 = ielementService_0.GetCompoundStructureLayers(object_0);
							if (ienumerable_0 == null)
							{
								string_4 = ielementService_0.GetElementName(object_0) ?? "未知";
								result = AIToolResult.Fail("元素 '" + string_4 + "' 没有复合层结构。");
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
									nullable_2 = nullable_1;
									if (string.IsNullOrEmpty(string_0) || nullable_1.HasValue)
									{
										goto IL_0492;
									}
									nullable_2 = ielementService_0.GetMaterialIdByName(aitoolContext_0.Document, string_0);
									if (nullable_2.HasValue)
									{
										goto IL_0492;
									}
									result = AIToolResult.Fail("找不到名称为 '" + string_0 + "' 的材质。请使用 get_materials 工具查看可用的材质。");
								}
							}
						}
					}
				}
				goto end_IL_0067;
				IL_0492:
				bool_0 = ielementService_0.ModifyCompoundStructureLayer(object_0, int_2, aitoolContext_0.Document, nullable_0, nullable_2, string_1);
				if (!bool_0)
				{
					if (list_0.Count == 1)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(248, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("❌ 修改复合层 ");
						defaultInterpolatedStringHandler4.AppendFormatted(int_2);
						defaultInterpolatedStringHandler4.AppendLiteral(" 失败。这个墙/楼板类型只有一层，Revit 可能限制了直接修改单层的属性。\n\n");
						defaultInterpolatedStringHandler4.AppendLiteral("💡 推荐解决方案：\n");
						defaultInterpolatedStringHandler4.AppendLiteral("1. 使用 add_compound_structure_layer 添加新层（如临时保温层）\n");
						defaultInterpolatedStringHandler4.AppendLiteral("2. 再使用 modify_compound_structure_layer 修改原层的厚度\n");
						defaultInterpolatedStringHandler4.AppendLiteral("3. 最后使用 delete_compound_structure_layer 删除临时添加的层\n\n");
						defaultInterpolatedStringHandler4.AppendLiteral("或者：尝试使用 set_parameter_values 工具直接修改 '厚度' 参数。");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler4.ToStringAndClear());
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(20, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("修改复合层 ");
						defaultInterpolatedStringHandler5.AppendFormatted(int_2);
						defaultInterpolatedStringHandler5.AppendLiteral(" 失败。请检查参数是否正确。");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler5.ToStringAndClear());
					}
				}
				else
				{
					list_1 = new List<string>();
					if (nullable_0.HasValue)
					{
						List<string> list = list_1;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(7, 1);
						defaultInterpolatedStringHandler6.AppendLiteral("厚度 → ");
						defaultInterpolatedStringHandler6.AppendFormatted(nullable_0.Value);
						defaultInterpolatedStringHandler6.AppendLiteral("mm");
						list.Add(defaultInterpolatedStringHandler6.ToStringAndClear());
					}
					if (nullable_2.HasValue)
					{
						List<string> list2 = list_1;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(8, 1);
						defaultInterpolatedStringHandler7.AppendLiteral("材质 → ID ");
						defaultInterpolatedStringHandler7.AppendFormatted(nullable_2.Value);
						list2.Add(defaultInterpolatedStringHandler7.ToStringAndClear());
					}
					if (!string.IsNullOrEmpty(string_1))
					{
						list_1.Add("功能 → " + string_1);
					}
					string_3 = ielementService_0.GetElementName(object_0) ?? "未知";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(16, 3);
					defaultInterpolatedStringHandler8.AppendLiteral("✅ 成功修改 '");
					defaultInterpolatedStringHandler8.AppendFormatted(string_3);
					defaultInterpolatedStringHandler8.AppendLiteral("' 的第 ");
					defaultInterpolatedStringHandler8.AppendFormatted(int_2);
					defaultInterpolatedStringHandler8.AppendLiteral(" 层：");
					defaultInterpolatedStringHandler8.AppendFormatted(string.Join("，", list_1));
					result = AIToolResult.Ok(defaultInterpolatedStringHandler8.ToStringAndClear(), (object)new Class278<int, string, string, int, double?, int?, string>(int_1, string_3, string_2, int_2, nullable_0.HasValue ? new double?(Math.Round(nullable_0.Value, 2)) : ((double?)null), nullable_2, string_1));
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("修改复合层失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "modify_compound_structure_layer";

	public string Category => "复合层结构";

	public string Description => "修改墙、楼板或屋顶类型的指定复合层的厚度、材质或功能";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elementTypeId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"墙类型、楼板类型或屋顶类型的元素 ID（必选）。可使用 get_family_types 工具获取可用的类型 ID\"\r\n            },\r\n            \"layerIndex\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"要修改的层索引（必选，从 0 开始）。可使用 get_compound_structure_layers 工具查看层的索引\"\r\n            },\r\n            \"thicknessMM\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"新的层厚度（毫米，可选）。例如：200 表示 200mm 厚的结构层\"\r\n            },\r\n            \"materialId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"新的材质 ID（可选）。可使用 get_materials 工具获取可用的材质 ID\"\r\n            },\r\n            \"materialName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"新的材质名称（可选，中文）。例如：'混凝土'、'XPS 保温板'、'水泥砂浆'等。如果同时提供 materialId 和 materialName，优先使用 materialId\"\r\n            },\r\n            \"function\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"新的层功能（可选）。可选值：'Structure'（结构层）、'Finish'（面层）、'Thermal'（保温层）、'Air'（空气层）、'Membrane'（防潮层）\"\r\n            }\r\n        },\r\n        \"required\": [\"elementTypeId\", \"layerIndex\"]\r\n    }";

	[AsyncStateMachine(typeof(Class611))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class611 stateMachine = new Class611();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.modifyCompoundStructureLayerTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
