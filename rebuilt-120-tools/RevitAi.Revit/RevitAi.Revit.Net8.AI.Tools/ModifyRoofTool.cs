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

[AITool("modify_roof", Category = "元素修改", Description = "修改迹线屋面的边坡度。所有距离单位为毫米，角度单位为度", RequiresTransaction = true, RequiresModification = true)]
public sealed class ModifyRoofTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class612 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ModifyRoofTool modifyRoofTool_0;

		private IElementService ielementService_0;

		private int int_1;

		private object object_0;

		private bool bool_0;

		private double double_0;

		private bool bool_1;

		private IEnumerable<(int EdgeIndex, bool DefinesSlope, double SlopeAngle)> ienumerable_0;

		private int int_2;

		private IEnumerator<(int EdgeIndex, bool DefinesSlope, double SlopeAngle)> ienumerator_0;

		private (int EdgeIndex, bool DefinesSlope, double SlopeAngle) valueTuple_0;

		private int int_3;

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
					Class612 stateMachine = this;
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
					int_1 = aitoolContext_0.GetParameter<int>("roof_id", 0);
					object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
					if (object_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到屋面 ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						bool_0 = aitoolContext_0.GetParameter<bool>("defines_slope", false);
						double_0 = (aitoolContext_0.HasParameter("slope_angle") ? aitoolContext_0.GetParameter<double>("slope_angle", 0.0) : 0.0);
						bool_1 = aitoolContext_0.HasParameter("apply_to_all_edges") && aitoolContext_0.GetParameter<bool>("apply_to_all_edges", false);
						if (bool_1)
						{
							ienumerable_0 = ielementService_0.GetFootPrintRoofEdges(object_0);
							if (ienumerable_0 == null)
							{
								result = AIToolResult.Fail("获取屋面边信息失败，可能不是迹线屋面");
							}
							else
							{
								int_2 = 0;
								ienumerator_0 = ienumerable_0.GetEnumerator();
								try
								{
									while (ienumerator_0.MoveNext())
									{
										valueTuple_0 = ienumerator_0.Current;
										if (ielementService_0.SetRoofEdgeSlope(aitoolContext_0.Document, object_0, valueTuple_0.EdgeIndex, bool_0, double_0))
										{
											int_2++;
										}
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
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("成功修改屋面 ");
								defaultInterpolatedStringHandler2.AppendFormatted(int_1);
								defaultInterpolatedStringHandler2.AppendLiteral(" 的所有 ");
								defaultInterpolatedStringHandler2.AppendFormatted(int_2);
								defaultInterpolatedStringHandler2.AppendLiteral(" 条边的坡度");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class279<int, int, bool, double>(int_1, int_2, bool_0, double_0));
							}
						}
						else if (!aitoolContext_0.HasParameter("edge_index"))
						{
							result = AIToolResult.Fail("未指定 edge_index 时，必须设置 apply_to_all_edges 为 true");
						}
						else
						{
							int_3 = aitoolContext_0.GetParameter<int>("edge_index", 0);
							if (!ielementService_0.SetRoofEdgeSlope(aitoolContext_0.Document, object_0, int_3, bool_0, double_0))
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
								defaultInterpolatedStringHandler3.AppendLiteral("修改屋面边 ");
								defaultInterpolatedStringHandler3.AppendFormatted(int_3);
								defaultInterpolatedStringHandler3.AppendLiteral(" 失败");
								result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(14, 2);
								defaultInterpolatedStringHandler4.AppendLiteral("成功修改屋面 ");
								defaultInterpolatedStringHandler4.AppendFormatted(int_1);
								defaultInterpolatedStringHandler4.AppendLiteral(" 边 ");
								defaultInterpolatedStringHandler4.AppendFormatted(int_3);
								defaultInterpolatedStringHandler4.AppendLiteral(" 的坡度");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class280<int, int, bool, double>(int_1, int_3, bool_0, double_0));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("修改屋面失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "modify_roof";

	public string Category => "元素修改";

	public string Description => "修改迹线屋面的边坡度等属性";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"roof_id\": {\n                \"type\": \"integer\",\n                \"description\": \"屋面元素 ID\"\n            },\n            \"edge_index\": {\n                \"type\": \"integer\",\n                \"description\": \"要修改的边索引（从 0 开始）。使用 get_roof_edges 查询可用索引\"\n            },\n            \"defines_slope\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否定义坡度\"\n            },\n            \"slope_angle\": {\n                \"type\": \"number\",\n                \"description\": \"坡度角度（度）。0 表示平顶。可选\"\n            },\n            \"apply_to_all_edges\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否应用到所有边。可选，默认 false。为 true 时忽略 edge_index\"\n            }\n        },\n        \"required\": [\"roof_id\", \"defines_slope\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class612))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class612 stateMachine = new Class612();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.modifyRoofTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
