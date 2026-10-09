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

[AITool("array_element", Category = "元素修改", Description = "阵列复制元素（线性阵列或环形阵列）。距离单位：毫米（工具内部会转换为英尺）；角度单位：度", RequiresTransaction = true, RequiresModification = true)]
public sealed class ArrayElementTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class353 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ArrayElementTool arrayElementTool_0;

		private int int_1;

		private string string_0;

		private int int_2;

		private IElementService ielementService_0;

		private IModificationService imodificationService_0;

		private object object_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private string string_1;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private double double_4;

		private double double_5;

		private double double_6;

		private double double_7;

		private double double_8;

		private double double_9;

		private double double_10;

		private double double_11;

		private IEnumerator<object> ienumerator_0;

		private object object_1;

		private int? nullable_0;

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
					Class353 stateMachine = this;
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
				int_1 = aitoolContext_0.GetParameter<int>("element_id", 0);
				string_0 = aitoolContext_0.GetParameter<string>("array_type", "linear");
				int_2 = aitoolContext_0.GetParameter<int>("count", 0);
				if (int_2 < 2)
				{
					result = AIToolResult.Fail("阵列数量必须大于或等于 2");
				}
				else
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					imodificationService_0 = ((revitAdapter2 != null) ? revitAdapter2.ModificationService : null);
					if (ielementService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ElementService");
					}
					else if (imodificationService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ModificationService");
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
							defaultInterpolatedStringHandler.AppendLiteral(" 的元素");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							if (string_0 == "linear")
							{
								double_0 = aitoolContext_0.GetParameter<double>("move_x", 0.0);
								double_1 = aitoolContext_0.GetParameter<double>("move_y", 0.0);
								double_2 = aitoolContext_0.GetParameter<double>("move_z", 0.0);
								double_3 = double_0 / 304.8;
								double_4 = double_1 / 304.8;
								double_5 = double_2 / 304.8;
								ienumerable_0 = imodificationService_0.ArrayElementLinear(aitoolContext_0.Document, object_0, int_2, double_3, double_4, double_5);
								goto IL_041f;
							}
							if (string_0 == "radial")
							{
								double_6 = aitoolContext_0.GetParameter<double>("origin_x", 0.0);
								double_7 = aitoolContext_0.GetParameter<double>("origin_y", 0.0);
								double_8 = aitoolContext_0.GetParameter<double>("origin_z", 0.0);
								double_9 = double_6 / 304.8;
								double_10 = double_7 / 304.8;
								double_11 = double_8 / 304.8;
								ienumerable_0 = imodificationService_0.ArrayElementRadial(aitoolContext_0.Document, object_0, int_2, 0.0, 0.0, 1.0, double_9, double_10, double_11);
								goto IL_041f;
							}
							result = AIToolResult.Fail("不支持的阵列类型: " + string_0);
						}
					}
				}
				goto end_IL_0067;
				IL_041f:
				if (ienumerable_0 == null)
				{
					result = AIToolResult.Fail("阵列元素失败");
				}
				else
				{
					list_0 = new List<object>();
					ienumerator_0 = ienumerable_0.GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							object_1 = ienumerator_0.Current;
							nullable_0 = ielementService_0.GetElementId(object_1);
							list_0.Add(new Class5<int?>(nullable_0));
							object_1 = null;
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
					string_1 = ielementService_0.GetElementName(object_0);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(11, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("成功阵列 ");
					defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler2.AppendLiteral(" 个元素: ");
					defaultInterpolatedStringHandler2.AppendFormatted(string_1 ?? "未命名");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class6<int, string, string, int, List<object>>(int_1, string_1, string_0, int_2, list_0));
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("阵列元素失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "array_element";

	public string Category => "元素修改";

	public string Description => "阵列复制元素（线性阵列或环形阵列）。距离单位：毫米（工具内部会转换为英尺）；角度单位：度";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"element_id\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"要阵列的元素 ID\"\r\n            },\r\n            \"array_type\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"阵列类型（linear 或 radial）\",\r\n                \"enum\": [\"linear\", \"radial\"],\r\n                \"default\": \"linear\"\r\n            },\r\n            \"count\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"阵列数量（包括原元素）\",\r\n                \"minimum\": 2,\r\n                \"default\": 2\r\n            },\r\n            \"move_x\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"线性阵列 X 方向偏移（毫米，当 array_type=linear 时必需）。偏移值指定第二个成员相对于第一个成员的位移。\"\r\n            },\r\n            \"move_y\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"线性阵列 Y 方向偏移（毫米，当 array_type=linear 时必需）。偏移值指定第二个成员相对于第一个成员的位移。\"\r\n            },\r\n            \"move_z\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"线性阵列 Z 方向偏移（毫米，当 array_type=linear 时可选）。偏移值指定第二个成员相对于第一个成员的位移。\",\r\n                \"default\": 0.0\r\n            },\r\n            \"origin_x\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"径向阵列旋转轴起点 X 坐标（毫米，当 array_type=radial 时必需）。默认为 0 表示使用元素位置。\"\r\n            },\r\n            \"origin_y\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"径向阵列旋转轴起点 Y 坐标（毫米，当 array_type=radial 时必需）。默认为 0 表示使用元素位置。\"\r\n            },\r\n            \"origin_z\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"径向阵列旋转轴起点 Z 坐标（毫米，当 array_type=radial 时可选）。默认为 0。\",\r\n                \"default\": 0.0\r\n            }\r\n        },\r\n        \"required\": [\"element_id\", \"count\"]\r\n    }";

	[AsyncStateMachine(typeof(Class353))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class353 stateMachine = new Class353();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.arrayElementTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
