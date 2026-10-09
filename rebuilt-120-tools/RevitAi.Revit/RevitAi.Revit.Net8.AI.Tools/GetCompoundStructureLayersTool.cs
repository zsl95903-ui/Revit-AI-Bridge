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

[AITool("get_compound_structure_layers", Category = "复合层结构", Description = "获取墙、楼板或屋顶类型的复合层结构信息，包括每层的厚度、材质、功能等详细信息", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetCompoundStructureLayersTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class493
	{
		public IParameterService iparameterService_0;

		internal bool method_0(object object_0)
		{
			string parameterName = iparameterService_0.GetParameterName(object_0);
			return (parameterName != null && parameterName.Contains("厚度")) || (parameterName != null && parameterName.Equals("Width", StringComparison.OrdinalIgnoreCase)) || (parameterName?.Equals("Thickness", StringComparison.OrdinalIgnoreCase) ?? false);
		}
	}

	[CompilerGenerated]
	public sealed class Class494 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetCompoundStructureLayersTool getCompoundStructureLayersTool_0;

		private IElementService ielementService_0;

		private int int_1;

		private string string_0;

		private object object_0;

		private string string_1;

		private object object_1;

		private int? nullable_0;

		private bool bool_0;

		private IEnumerable<CompoundLayerInfo> ienumerable_0;

		private List<object> list_0;

		private double? nullable_1;

		private string string_2;

		private int? nullable_2;

		private object object_2;

		private string string_3;

		private Class493 class493_0;

		private string string_4;

		private IEnumerable<object> ienumerable_1;

		private object object_3;

		private string string_5;

		private IEnumerator<CompoundLayerInfo> ienumerator_0;

		private CompoundLayerInfo compoundLayerInfo_0;

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
					Class494 stateMachine = this;
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
					string_0 = aitoolContext_0.GetParameter<string>("elementTypeName", (string)null);
					object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
					if (object_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 的元素");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						string_1 = ielementService_0.GetElementCategory(object_0);
						object_1 = object_0;
						nullable_0 = int_1;
						if (string_1 == "墙" || string_1 == "楼板" || string_1 == "屋顶")
						{
							nullable_2 = ielementService_0.GetElementTypeId(object_0);
							if (nullable_2.HasValue && nullable_2.Value > 0)
							{
								object_2 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_2.Value);
								if (object_2 != null)
								{
									object_1 = object_2;
									nullable_0 = nullable_2.Value;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(49, 2);
									defaultInterpolatedStringHandler2.AppendLiteral("[GetCompoundStructureLayers] 自动从墙/楼板/屋顶实例 ");
									defaultInterpolatedStringHandler2.AppendFormatted(int_1);
									defaultInterpolatedStringHandler2.AppendLiteral(" 转换为类型 ");
									defaultInterpolatedStringHandler2.AppendFormatted(nullable_2.Value);
									Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
								}
								object_2 = null;
							}
						}
						string_1 = ielementService_0.GetElementCategory(object_1);
						bool_0 = string_1 == "墙类型" || string_1 == "楼板类型" || string_1 == "屋顶类型" || string_1 == "墙" || string_1 == "楼板" || string_1 == "屋顶";
						if (!bool_0)
						{
							string_3 = ielementService_0.GetElementName(object_0) ?? "未知";
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(128, 3);
							defaultInterpolatedStringHandler3.AppendLiteral("元素 '");
							defaultInterpolatedStringHandler3.AppendFormatted(string_3);
							defaultInterpolatedStringHandler3.AppendLiteral("'（ID: ");
							defaultInterpolatedStringHandler3.AppendFormatted(int_1);
							defaultInterpolatedStringHandler3.AppendLiteral("）不是墙类型、楼板类型或屋顶类型，而是 '");
							defaultInterpolatedStringHandler3.AppendFormatted(string_1);
							defaultInterpolatedStringHandler3.AppendLiteral("'。\n\n");
							defaultInterpolatedStringHandler3.AppendLiteral("提示：复合层结构仅适用于墙、楼板和屋顶的类型定义。\n");
							defaultInterpolatedStringHandler3.AppendLiteral("如果您选择的是墙/楼板/屋顶实例，系统会自动获取其类型。\n");
							defaultInterpolatedStringHandler3.AppendLiteral("请使用 get_family_types 工具查看可用的墙/楼板/屋顶类型。");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
						}
						else
						{
							ienumerable_0 = ielementService_0.GetCompoundStructureLayers(object_1);
							if (ienumerable_0 == null)
							{
								class493_0 = new Class493();
								string_4 = ielementService_0.GetElementName(object_1) ?? "未知";
								Class493 @class = class493_0;
								IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
								@class.iparameterService_0 = ((revitAdapter2 != null) ? revitAdapter2.ParameterService : null);
								if (class493_0.iparameterService_0 == null)
								{
									goto IL_060e;
								}
								ienumerable_1 = class493_0.iparameterService_0.GetParameters(object_1);
								object_3 = ienumerable_1?.FirstOrDefault(delegate(object object_0)
								{
									string parameterName = class493_0.iparameterService_0.GetParameterName(object_0);
									return (parameterName != null && parameterName.Contains("厚度")) || (parameterName != null && parameterName.Equals("Width", StringComparison.OrdinalIgnoreCase)) || (parameterName?.Equals("Thickness", StringComparison.OrdinalIgnoreCase) ?? false);
								});
								if (object_3 == null)
								{
									ienumerable_1 = null;
									object_3 = null;
									goto IL_060e;
								}
								string_5 = class493_0.iparameterService_0.GetParameterValueAsString(object_3);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(112, 2);
								defaultInterpolatedStringHandler4.AppendLiteral("元素 '");
								defaultInterpolatedStringHandler4.AppendFormatted(string_4);
								defaultInterpolatedStringHandler4.AppendLiteral("' 是简单墙类型，没有复合层结构。它的厚度由参数直接控制：");
								defaultInterpolatedStringHandler4.AppendFormatted(string_5);
								defaultInterpolatedStringHandler4.AppendLiteral("。\n\n");
								defaultInterpolatedStringHandler4.AppendLiteral("如需修改厚度，请使用 set_parameter_values 工具设置 '厚度' 参数。\n\n");
								defaultInterpolatedStringHandler4.AppendLiteral("提示：只有多层复合墙（如外墙、保温墙等）才使用复合层结构。");
								result = AIToolResult.Fail(defaultInterpolatedStringHandler4.ToStringAndClear());
							}
							else
							{
								list_0 = new List<object>();
								ienumerator_0 = ienumerable_0.GetEnumerator();
								try
								{
									while (ienumerator_0.MoveNext())
									{
										compoundLayerInfo_0 = ienumerator_0.Current;
										list_0.Add(new Class161<int, double, string, string, int?, string, bool, bool>(compoundLayerInfo_0.Index, Math.Round(compoundLayerInfo_0.WidthMM, 2), compoundLayerInfo_0.Function, compoundLayerInfo_0.FunctionName, compoundLayerInfo_0.MaterialId, compoundLayerInfo_0.MaterialName ?? "（无材质）", compoundLayerInfo_0.IsCore, compoundLayerInfo_0.ParticipatesInWrapping));
										compoundLayerInfo_0 = null;
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
								nullable_1 = ielementService_0.GetCompoundStructureTotalThickness(object_1);
								string_2 = ielementService_0.GetElementName(object_1) ?? "未知";
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(30, 3);
								defaultInterpolatedStringHandler5.AppendLiteral("✅ 成功获取 '");
								defaultInterpolatedStringHandler5.AppendFormatted(string_2);
								defaultInterpolatedStringHandler5.AppendLiteral("' 的复合层结构信息，共 ");
								defaultInterpolatedStringHandler5.AppendFormatted(list_0.Count);
								defaultInterpolatedStringHandler5.AppendLiteral(" 层，总厚度 ");
								defaultInterpolatedStringHandler5.AppendFormatted(Math.Round(nullable_1.GetValueOrDefault(), 2));
								defaultInterpolatedStringHandler5.AppendLiteral("mm");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler5.ToStringAndClear(), (object)new Class162<int?, string, string, int, double, List<object>>(nullable_0, string_2, string_1, list_0.Count, Math.Round(nullable_1.GetValueOrDefault(), 2), list_0));
							}
						}
					}
				}
				goto end_IL_0067;
				IL_060e:
				result = AIToolResult.Fail("元素 '" + string_4 + "' 没有复合层结构。这通常是基本墙或结构楼板，它们的厚度和材质由参数直接控制，不使用复合层结构。请使用 get_parameter_values 查看可用参数。");
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取复合层结构信息失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_compound_structure_layers";

	public string Category => "复合层结构";

	public string Description => "获取墙、楼板或屋顶类型的复合层结构信息，包括每层的厚度、材质、功能等详细信息";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elementTypeId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"墙类型、楼板类型或屋顶类型的元素 ID（必选）。可使用 get_family_types 工具获取可用的类型 ID\"\r\n            },\r\n            \"elementTypeName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"类型名称（可选，用于验证）。如'基本墙'、'楼板'等\"\r\n            }\r\n        },\r\n        \"required\": [\"elementTypeId\"]\r\n    }";

	[AsyncStateMachine(typeof(Class494))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class494 stateMachine = new Class494();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getCompoundStructureLayersTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
