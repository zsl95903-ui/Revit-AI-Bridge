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

[AITool("create_beam", Category = "元素创建", Description = "在指定位置创建结构梁。所有距离单位为毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateBeamTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class382 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateBeamTool createBeamTool_0;

		private List<object> list_0;

		private List<object> list_1;

		private List<(int index, string reason)> list_2;

		private ns0.Class39<int, int, int, List<object>, List<(int index, string reason)>> class39_0;

		private string string_0;

		private int int_1;

		private IDictionary<string, object> idictionary_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private int int_2;

		private int? nullable_0;

		private object object_0;

		private int? nullable_1;

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
					Class382 stateMachine = this;
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
			list_0 = aitoolContext_0.GetParameter<List<object>>("beams", (List<object>)null);
			AIToolResult result;
			if (list_0 == null || list_0.Count == 0)
			{
				result = AIToolResult.Fail("beams 参数不能为空");
			}
			else
			{
				list_1 = new List<object>();
				list_2 = new List<(int, string)>();
				int_1 = 0;
				while (int_1 < list_0.Count)
				{
					try
					{
						idictionary_0 = list_0[int_1] as IDictionary<string, object>;
						if (idictionary_0 == null)
						{
							list_2.Add((int_1, "数据格式错误"));
						}
						else
						{
							double_0 = Convert.ToDouble(idictionary_0["start_x"]) / 304.8;
							double_1 = Convert.ToDouble(idictionary_0["start_y"]) / 304.8;
							double_2 = Convert.ToDouble(idictionary_0["end_x"]) / 304.8;
							double_3 = Convert.ToDouble(idictionary_0["end_y"]) / 304.8;
							int_2 = Convert.ToInt32(idictionary_0["level_id"]);
							nullable_0 = ((!idictionary_0.ContainsKey("beam_type_id") || idictionary_0["beam_type_id"] == null) ? ((int?)null) : new int?(Convert.ToInt32(idictionary_0["beam_type_id"])));
							object_0 = ielementService_0.CreateBeam(aitoolContext_0.Document, double_0, double_1, double_2, double_3, int_2, nullable_0);
							if (object_0 == null)
							{
								list_2.Add((int_1, "创建失败"));
							}
							else
							{
								nullable_1 = ielementService_0.GetElementId(object_0);
								list_1.Add(new Class38<int, int?, double, double, double, double, int, int?>(int_1, nullable_1, double_0 * 304.8, double_1 * 304.8, double_2 * 304.8, double_3 * 304.8, int_2, nullable_0));
								idictionary_0 = null;
								object_0 = null;
							}
						}
					}
					catch (Exception ex)
					{
						exception_0 = ex;
						list_2.Add((int_1, exception_0.Message));
					}
					int_1++;
				}
				class39_0 = new ns0.Class39<int, int, int, List<object>, List<(int, string)>>(list_0.Count, list_1.Count, list_2.Count, list_1, list_2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
				defaultInterpolatedStringHandler.AppendLiteral("批量创建梁完成：成功 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
				if (list_2.Count > 0)
				{
					string text = string_0;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("，失败 ");
					defaultInterpolatedStringHandler2.AppendFormatted(list_2.Count);
					defaultInterpolatedStringHandler2.AppendLiteral(" 个");
					string_0 = text + defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				result = AIToolResult.Ok(string_0, (object)class39_0);
			}
			int_0 = -2;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			class39_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class383 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateBeamTool createBeamTool_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private int int_1;

		private int? nullable_0;

		private object object_0;

		private int? nullable_1;

		private string string_0;

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
					Class383 stateMachine = this;
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
			double_0 = aitoolContext_0.GetParameter<double>("start_x", 0.0) / 304.8;
			double_1 = aitoolContext_0.GetParameter<double>("start_y", 0.0) / 304.8;
			double_2 = aitoolContext_0.GetParameter<double>("end_x", 0.0) / 304.8;
			double_3 = aitoolContext_0.GetParameter<double>("end_y", 0.0) / 304.8;
			int_1 = aitoolContext_0.GetParameter<int>("level_id", 0);
			nullable_0 = aitoolContext_0.GetParameter<int?>("beam_type_id", (int?)null);
			object_0 = ielementService_0.CreateBeam(aitoolContext_0.Document, double_0, double_1, double_2, double_3, int_1, nullable_0);
			AIToolResult result;
			if (object_0 == null)
			{
				result = AIToolResult.Fail("创建梁失败");
			}
			else
			{
				nullable_1 = ielementService_0.GetElementId(object_0);
				string_0 = ielementService_0.GetElementName(object_0);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("成功创建梁，ID: ");
				defaultInterpolatedStringHandler.AppendFormatted(nullable_1);
				result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class37<int?, string>(nullable_1, string_0 ?? "未命名"));
			}
			int_0 = -2;
			object_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class384 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateBeamTool createBeamTool_0;

		private IElementService ielementService_0;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 1u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class384 stateMachine = this;
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
			goto IL_006e;
			IL_006e:
			AIToolResult result;
			try
			{
				TaskAwaiter<AIToolResult> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_01e4;
				}
				TaskAwaiter<AIToolResult> awaiter3;
				if (num == 2)
				{
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_01b1;
				}
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else
				{
					if (aitoolContext_0.Document != null)
					{
						if (aitoolContext_0.HasParameter("beams"))
						{
							awaiter2 = createBeamTool_0.method_1(aitoolContext_0, ielementService_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class384 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_01e4;
						}
						awaiter3 = createBeamTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter3;
							Class384 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
							return;
						}
						goto IL_01b1;
					}
					result = AIToolResult.Fail("文档对象为空");
				}
				goto end_IL_006e;
				IL_01e4:
				aitoolResult_0 = awaiter2.GetResult();
				result = aitoolResult_0;
				goto end_IL_006e;
				IL_01b1:
				aitoolResult_1 = awaiter3.GetResult();
				result = aitoolResult_1;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建梁失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_beam";

	public string Category => "元素创建";

	public string Description => "在指定位置创建结构梁";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"start_x\": {\n                \"type\": \"number\",\n                \"description\": \"起点 X 坐标（毫米）。单个创建时使用\"\n            },\n            \"start_y\": {\n                \"type\": \"number\",\n                \"description\": \"起点 Y 坐标（毫米）。单个创建时使用\"\n            },\n            \"end_x\": {\n                \"type\": \"number\",\n                \"description\": \"终点 X 坐标（毫米）。单个创建时使用\"\n            },\n            \"end_y\": {\n                \"type\": \"number\",\n                \"description\": \"终点 Y 坐标（毫米）。单个创建时使用\"\n            },\n            \"level_id\": {\n                \"type\": \"integer\",\n                \"description\": \"梁所在的标高 ID。单个创建时使用\"\n            },\n            \"beam_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"梁类型 ID（必选）。可使用 get_family_types 工具获取可用的梁类型 ID（单个创建时使用）\"\n            },\n            \"beams\": {\n                \"type\": \"array\",\n                \"description\": \"梁数据列表（批量创建时使用）。如果提供此参数，将忽略单个创建参数。每个对象包含 start_x, start_y, end_x, end_y, level_id, beam_type_id\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"start_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"起点 X 坐标（毫米）\"\n                        },\n                        \"start_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"起点 Y 坐标（毫米）\"\n                        },\n                        \"end_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"终点 X 坐标（毫米）\"\n                        },\n                        \"end_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"终点 Y 坐标（毫米）\"\n                        },\n                        \"level_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"梁所在的标高 ID\"\n                        },\n                        \"beam_type_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"梁类型 ID（必选）\"\n                        }\n                    },\n                    \"required\": [\"start_x\", \"start_y\", \"end_x\", \"end_y\", \"level_id\", \"beam_type_id\"]\n                }\n            }\n        }\n    }";

	[AsyncStateMachine(typeof(Class384))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class384 stateMachine = new Class384();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createBeamTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class383))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class383 stateMachine = new Class383();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createBeamTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class382))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class382 stateMachine = new Class382();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createBeamTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
