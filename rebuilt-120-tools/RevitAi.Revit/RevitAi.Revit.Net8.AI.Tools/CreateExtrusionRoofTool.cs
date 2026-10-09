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

[AITool("create_extrusion_roof", Category = "元素创建", Description = "通过开放轮廓和挤出距离创建挤出屋面。所有距离单位为毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateExtrusionRoofTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class398 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateExtrusionRoofTool createExtrusionRoofTool_0;

		private IElementService ielementService_0;

		private List<object> list_0;

		private List<(double X, double Y)> list_1;

		private int int_1;

		private int? nullable_0;

		private double double_0;

		private double double_1;

		private object object_0;

		private int? nullable_1;

		private string string_0;

		private List<object>.Enumerator enumerator_0;

		private object object_1;

		private IDictionary<string, object> idictionary_0;

		private double double_2;

		private double double_3;

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
					Class398 stateMachine = this;
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
					if (list_0 == null || list_0.Count < 2)
					{
						result = AIToolResult.Fail("开放轮廓点列表至少需要 2 个点");
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
									double_2 = Convert.ToDouble(idictionary_0["x"]) / 304.8;
									double_3 = Convert.ToDouble(idictionary_0["y"]) / 304.8;
									list_1.Add((double_2, double_3));
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
						double_0 = (aitoolContext_0.HasParameter("extrusion_start") ? (aitoolContext_0.GetParameter<double>("extrusion_start", 0.0) / 304.8) : 0.0);
						double_1 = (aitoolContext_0.HasParameter("extrusion_end") ? (aitoolContext_0.GetParameter<double>("extrusion_end", 0.0) / 304.8) : 30.0);
						object_0 = ielementService_0.CreateExtrusionRoof(aitoolContext_0.Document, (IEnumerable<ValueTuple<double, double>>)list_1, int_1, nullable_0, double_0, double_1);
						if (object_0 == null)
						{
							result = AIToolResult.Fail("创建挤出屋面失败");
						}
						else
						{
							nullable_1 = ielementService_0.GetElementId(object_0);
							string_0 = ielementService_0.GetElementName(object_0);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler.AppendLiteral("成功创建挤出屋面，ID: ");
							defaultInterpolatedStringHandler.AppendFormatted(nullable_1);
							result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class53<int?, string, int, double, double>(nullable_1, string_0 ?? "未命名", list_1.Count, double_0 * 304.8, double_1 * 304.8));
						}
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建挤出屋面失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_extrusion_roof";

	public string Category => "元素创建";

	public string Description => "通过开放轮廓创建挤出屋面";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"points\": {\n                \"type\": \"array\",\n                \"description\": \"开放轮廓点列表（按顺序连接，不闭合）。至少需要 2 个点。X 为水平位置，Y 为高度（相对于标高）\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"x\": { \"type\": \"number\", \"description\": \"X 坐标（毫米）- 水平位置\" },\n                        \"y\": { \"type\": \"number\", \"description\": \"Y 坐标（毫米）- 高度（相对于标高）\" }\n                    },\n                    \"required\": [\"x\", \"y\"]\n                }\n            },\n            \"level_id\": {\n                \"type\": \"integer\",\n                \"description\": \"基准标高 ID\"\n            },\n            \"roof_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"屋面类型 ID（可选）。可使用 get_roof_types 工具获取可用类型\"\n            },\n            \"extrusion_start\": {\n                \"type\": \"number\",\n                \"description\": \"挤出起点偏移（毫米）。默认 0\"\n            },\n            \"extrusion_end\": {\n                \"type\": \"number\",\n                \"description\": \"挤出终点偏移（毫米）。默认 9000（约 9 米）\"\n            }\n        },\n        \"required\": [\"points\", \"level_id\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class398))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class398 stateMachine = new Class398();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createExtrusionRoofTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
