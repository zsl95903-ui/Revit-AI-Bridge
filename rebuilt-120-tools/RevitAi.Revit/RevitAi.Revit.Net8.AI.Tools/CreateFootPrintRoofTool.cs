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

[AITool("create_footprint_roof", Category = "元素创建", Description = "通过闭合轮廓创建迹线屋面。所有距离单位为毫米，坡度单位为度", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateFootPrintRoofTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class406 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateFootPrintRoofTool createFootPrintRoofTool_0;

		private IElementService ielementService_0;

		private List<object> list_0;

		private List<(double X, double Y)> list_1;

		private int int_1;

		private int? nullable_0;

		private object object_0;

		private int? nullable_1;

		private string string_0;

		private bool bool_0;

		private double double_0;

		private List<object>.Enumerator enumerator_0;

		private object object_1;

		private IDictionary<string, object> idictionary_0;

		private double double_1;

		private double double_2;

		private IEnumerable<(int EdgeIndex, bool DefinesSlope, double SlopeAngle)> ienumerable_0;

		private IEnumerator<(int EdgeIndex, bool DefinesSlope, double SlopeAngle)> ienumerator_0;

		private (int EdgeIndex, bool DefinesSlope, double SlopeAngle) valueTuple_0;

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
					Class406 stateMachine = this;
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
					list_0 = aitoolContext_0.GetParameter<List<object>>("points", (List<object>)null);
					if (list_0 == null || list_0.Count < 3)
					{
						result = AIToolResult.Fail("轮廓点列表至少需要 3 个点");
					}
					else
					{
						list_1 = new List<(double, double)>();
						enumerator_0 = list_0.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								object_1 = enumerator_0.Current;
								idictionary_0 = object_1 as IDictionary<string, object>;
								if (idictionary_0 != null)
								{
									double_1 = Convert.ToDouble(idictionary_0["x"]) / 304.8;
									double_2 = Convert.ToDouble(idictionary_0["y"]) / 304.8;
									list_1.Add((double_1, double_2));
									idictionary_0 = null;
									object_1 = null;
									continue;
								}
								result = AIToolResult.Fail("轮廓点数据格式错误");
								goto end_IL_0067;
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
							}
						}
						enumerator_0 = default(List<object>.Enumerator);
						int_1 = aitoolContext_0.GetParameter<int>("level_id", 0);
						nullable_0 = (aitoolContext_0.HasParameter("roof_type_id") ? new int?(aitoolContext_0.GetParameter<int>("roof_type_id", 0)) : ((int?)null));
						object_0 = ielementService_0.CreateFootPrintRoof(aitoolContext_0.Document, (IEnumerable<ValueTuple<double, double>>)list_1, int_1, nullable_0);
						if (object_0 == null)
						{
							result = AIToolResult.Fail("创建迹线屋面失败");
						}
						else
						{
							nullable_1 = ielementService_0.GetElementId(object_0);
							string_0 = ielementService_0.GetElementName(object_0);
							bool_0 = aitoolContext_0.HasParameter("defines_slope") && aitoolContext_0.GetParameter<bool>("defines_slope", false);
							double_0 = (aitoolContext_0.HasParameter("slope_angle") ? aitoolContext_0.GetParameter<double>("slope_angle", 0.0) : 0.0);
							if (bool_0 && double_0 > 0.0)
							{
								ienumerable_0 = ielementService_0.GetFootPrintRoofEdges(object_0);
								if (ienumerable_0 != null)
								{
									ienumerator_0 = ienumerable_0.GetEnumerator();
									try
									{
										while (ienumerator_0.MoveNext())
										{
											valueTuple_0 = ienumerator_0.Current;
											ielementService_0.SetRoofEdgeSlope(aitoolContext_0.Document, object_0, valueTuple_0.EdgeIndex, true, double_0);
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
								}
								ienumerable_0 = null;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler.AppendLiteral("成功创建迹线屋面，ID: ");
							defaultInterpolatedStringHandler.AppendFormatted(nullable_1);
							result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class61<int?, string, int, double, bool>(nullable_1, string_0 ?? "未命名", list_1.Count, double_0, bool_0));
						}
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建迹线屋面失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_footprint_roof";

	public string Category => "元素创建";

	public string Description => "通过闭合轮廓创建迹线屋面";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"points\": {\n                \"type\": \"array\",\n                \"description\": \"轮廓点列表（按顺序连接构成闭合多边形）。至少需要 3 个点\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"x\": { \"type\": \"number\", \"description\": \"X 坐标（毫米）\" },\n                        \"y\": { \"type\": \"number\", \"description\": \"Y 坐标（毫米）\" }\n                    },\n                    \"required\": [\"x\", \"y\"]\n                }\n            },\n            \"level_id\": {\n                \"type\": \"integer\",\n                \"description\": \"基准标高 ID\"\n            },\n            \"roof_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"屋面类型 ID（可选）。可使用 get_roof_types 工具获取可用类型\"\n            },\n            \"slope_angle\": {\n                \"type\": \"number\",\n                \"description\": \"所有边的默认坡度角度（度）。0 表示平顶。可选，默认 0\"\n            },\n            \"defines_slope\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否定义坡度。可选，默认 false\"\n            }\n        },\n        \"required\": [\"points\", \"level_id\"]\n    }";

	[AsyncStateMachine(typeof(Class406))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class406 stateMachine = new Class406();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createFootPrintRoofTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
