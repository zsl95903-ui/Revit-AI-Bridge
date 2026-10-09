using System;
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

[AITool("add_compound_structure_layer", Category = "复合层结构", Description = "在墙、楼板或屋顶类型的复合结构中添加新层。所有厚度单位为毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class AddCompoundStructureLayerTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class352 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public AddCompoundStructureLayerTool addCompoundStructureLayerTool_0;

		private IElementService ielementService_0;

		private int int_1;

		private double double_0;

		private string string_0;

		private int? nullable_0;

		private string string_1;

		private int? nullable_1;

		private object object_0;

		private string string_2;

		private int? nullable_2;

		private int int_2;

		private double? nullable_3;

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
					Class352 stateMachine = this;
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
					double_0 = aitoolContext_0.GetParameter<double>("thicknessMM", 0.0);
					string_0 = aitoolContext_0.GetParameter<string>("function", (string)null);
					nullable_0 = aitoolContext_0.GetParameter<int?>("materialId", (int?)null);
					string_1 = aitoolContext_0.GetParameter<string>("materialName", (string)null);
					nullable_1 = aitoolContext_0.GetParameter<int?>("insertIndex", (int?)null);
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
							nullable_2 = nullable_0;
							if (string.IsNullOrEmpty(string_1) || nullable_0.HasValue)
							{
								goto IL_035e;
							}
							nullable_2 = ielementService_0.GetMaterialIdByName(aitoolContext_0.Document, string_1);
							if (nullable_2.HasValue)
							{
								goto IL_035e;
							}
							result = AIToolResult.Fail("找不到名称为 '" + string_1 + "' 的材质。请使用 get_materials 工具查看可用的材质。");
						}
					}
				}
				goto end_IL_0067;
				IL_035e:
				int_2 = ielementService_0.AddCompoundStructureLayer(object_0, aitoolContext_0.Document, double_0, string_0, nullable_2, nullable_1);
				if (int_2 < 0)
				{
					result = AIToolResult.Fail("添加复合层失败。请检查参数是否正确。");
				}
				else
				{
					nullable_3 = ielementService_0.GetCompoundStructureTotalThickness(object_0);
					string_3 = ielementService_0.GetElementName(object_0) ?? "未知";
					string text;
					if (!nullable_1.HasValue)
					{
						text = "末尾";
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(3, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("位置 ");
						defaultInterpolatedStringHandler3.AppendFormatted(nullable_1.Value);
						text = defaultInterpolatedStringHandler3.ToStringAndClear();
					}
					string_4 = text;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(35, 5);
					defaultInterpolatedStringHandler4.AppendLiteral("✅ 成功在 '");
					defaultInterpolatedStringHandler4.AppendFormatted(string_3);
					defaultInterpolatedStringHandler4.AppendLiteral("' 的 ");
					defaultInterpolatedStringHandler4.AppendFormatted(string_4);
					defaultInterpolatedStringHandler4.AppendLiteral(" 添加新层（索引 ");
					defaultInterpolatedStringHandler4.AppendFormatted(int_2);
					defaultInterpolatedStringHandler4.AppendLiteral("），厚度 ");
					defaultInterpolatedStringHandler4.AppendFormatted(double_0);
					defaultInterpolatedStringHandler4.AppendLiteral("mm，新总厚度 ");
					defaultInterpolatedStringHandler4.AppendFormatted(Math.Round(nullable_3.GetValueOrDefault(), 2));
					defaultInterpolatedStringHandler4.AppendLiteral("mm");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class4<int, string, string, int, double, string, int?, double>(int_1, string_3, string_2, int_2, Math.Round(double_0, 2), string_0, nullable_2, Math.Round(nullable_3.GetValueOrDefault(), 2)));
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("添加复合层失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "add_compound_structure_layer";

	public string Category => "复合层结构";

	public string Description => "在墙、楼板或屋顶类型的复合结构中添加新层";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elementTypeId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"墙类型、楼板类型或屋顶类型的元素 ID（必选）。可使用 get_family_types 工具获取可用的类型 ID\"\r\n            },\r\n            \"thicknessMM\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"新层的厚度（毫米，必选）。例如：100 表示 100mm 厚的保温层\"\r\n            },\r\n            \"function\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"新层的功能（必选）。可选值：'Structure'（结构层）、'Finish1'（面层1）、'Finish2'（面层2）、'Insulation'（保温层）、'Membrane'（防潮层）、'Substrate'（基层）、'StructuralDeck'（结构板）\"\r\n            },\r\n            \"materialId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"材质 ID（可选）。可使用 get_materials 工具获取可用的材质 ID。如果不提供，则不设置材质\"\r\n            },\r\n            \"materialName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"材质名称（可选，中文）。例如：'混凝土'、'XPS 保温板'、'水泥砂浆'等。如果同时提供 materialId 和 materialName，优先使用 materialId\"\r\n            },\r\n            \"insertIndex\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"插入位置索引（可选，从 0 开始）。如果不提供或超出范围，则添加到末尾。可使用 get_compound_structure_layers 工具查看当前层的索引\"\r\n            }\r\n        },\r\n        \"required\": [\"elementTypeId\", \"thicknessMM\", \"function\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class352))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class352 stateMachine = new Class352();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.addCompoundStructureLayerTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
