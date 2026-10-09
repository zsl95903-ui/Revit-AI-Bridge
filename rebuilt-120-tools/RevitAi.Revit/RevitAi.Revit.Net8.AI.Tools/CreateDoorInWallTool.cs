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

[AITool("create_door_in_wall", Category = "元素创建", Description = "在指定墙上创建门。所有距离单位为毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateDoorInWallTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class392 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateDoorInWallTool createDoorInWallTool_0;

		private List<object> list_0;

		private List<object> list_1;

		private List<(int index, string reason)> list_2;

		private Class49<int, int, int, List<object>, List<(int index, string reason)>> class49_0;

		private string string_0;

		private int int_1;

		private IDictionary<string, object> idictionary_0;

		private int int_2;

		private double double_0;

		private double double_1;

		private int? nullable_0;

		private bool bool_0;

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
					Class392 stateMachine = this;
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
			list_0 = aitoolContext_0.GetParameter<List<object>>("doors", (List<object>)null);
			AIToolResult result;
			if (list_0 == null || list_0.Count == 0)
			{
				result = AIToolResult.Fail("doors 参数不能为空");
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
							int_2 = Convert.ToInt32(idictionary_0["wall_id"]);
							double_0 = Convert.ToDouble(idictionary_0["position_x"]) / 304.8;
							double_1 = Convert.ToDouble(idictionary_0["position_y"]) / 304.8;
							nullable_0 = null;
							if (idictionary_0.ContainsKey("door_type_id") && idictionary_0["door_type_id"] != null)
							{
								nullable_0 = Convert.ToInt32(idictionary_0["door_type_id"]);
							}
							bool_0 = false;
							if (idictionary_0.ContainsKey("flip") && idictionary_0["flip"] != null)
							{
								bool_0 = Convert.ToBoolean(idictionary_0["flip"]);
							}
							object_0 = ielementService_0.CreateDoorInWall(aitoolContext_0.Document, int_2, double_0, double_1, nullable_0, bool_0);
							if (object_0 == null)
							{
								list_2.Add((int_1, "创建失败"));
							}
							else
							{
								nullable_1 = ielementService_0.GetElementId(object_0);
								list_1.Add(new Class48<int, int?, int, double, double, int?, bool>(int_1, nullable_1, int_2, double_0 * 304.8, double_1 * 304.8, nullable_0, bool_0));
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
				class49_0 = new Class49<int, int, int, List<object>, List<(int, string)>>(list_0.Count, list_1.Count, list_2.Count, list_1, list_2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
				defaultInterpolatedStringHandler.AppendLiteral("批量创建门完成：成功 ");
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
				result = AIToolResult.Ok(string_0, (object)class49_0);
			}
			int_0 = -2;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			class49_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class393 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateDoorInWallTool createDoorInWallTool_0;

		private int int_1;

		private double double_0;

		private double double_1;

		private int? nullable_0;

		private bool bool_0;

		private object object_0;

		private int? nullable_1;

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
					Class393 stateMachine = this;
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
			int_1 = aitoolContext_0.GetParameter<int>("wall_id", 0);
			double_0 = aitoolContext_0.GetParameter<double>("position_x", 0.0) / 304.8;
			double_1 = aitoolContext_0.GetParameter<double>("position_y", 0.0) / 304.8;
			nullable_0 = aitoolContext_0.GetParameter<int?>("door_type_id", (int?)null);
			bool_0 = aitoolContext_0.GetParameter<bool>("flip", false);
			object_0 = ielementService_0.CreateDoorInWall(aitoolContext_0.Document, int_1, double_0, double_1, nullable_0, bool_0);
			AIToolResult result;
			if (object_0 == null)
			{
				result = AIToolResult.Fail("创建门失败");
			}
			else
			{
				nullable_1 = ielementService_0.GetElementId(object_0);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("成功创建门，ID: ");
				defaultInterpolatedStringHandler.AppendFormatted(nullable_1);
				result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class47<int?, int, double, double, int?, bool>(nullable_1, int_1, double_0 * 304.8, double_1 * 304.8, nullable_0, bool_0));
			}
			int_0 = -2;
			object_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class394 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateDoorInWallTool createDoorInWallTool_0;

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
					Class394 stateMachine = this;
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
						if (aitoolContext_0.HasParameter("doors"))
						{
							awaiter2 = createDoorInWallTool_0.method_1(aitoolContext_0, ielementService_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class394 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_01e4;
						}
						awaiter3 = createDoorInWallTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter3;
							Class394 stateMachine = this;
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
				result = AIToolResult.Fail("创建门失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_door_in_wall";

	public string Category => "元素创建";

	public string Description => "在指定墙上创建门。所有距离单位为毫米";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"wall_id\": {\n                \"type\": \"integer\",\n                \"description\": \"墙体元素 ID（单个创建时使用）\"\n            },\n            \"position_x\": {\n                \"type\": \"number\",\n                \"description\": \"门的插入位置 X 坐标（毫米）（单个创建时使用）\"\n            },\n            \"position_y\": {\n                \"type\": \"number\",\n                \"description\": \"门的插入位置 Y 坐标（毫米）（单个创建时使用）\"\n            },\n            \"door_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"门类型 ID。可使用 get_family_types 工具获取可用的门类型 ID（单个创建时使用）\"\n            },\n            \"flip\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否翻转门的方向（单个创建时使用）\",\n                \"default\": false\n            },\n            \"doors\": {\n                \"type\": \"array\",\n                \"description\": \"门数据列表（批量创建时使用）。如果提供此参数，将忽略单个创建参数。每个对象包含 wall_id, position_x, position_y, door_type_id, flip\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"wall_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"墙体元素 ID\"\n                        },\n                        \"position_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"门的插入位置 X 坐标（毫米）\"\n                        },\n                        \"position_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"门的插入位置 Y 坐标（毫米）\"\n                        },\n                        \"door_type_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"门类型 ID（必选）\"\n                        },\n                        \"flip\": {\n                            \"type\": \"boolean\",\n                            \"description\": \"是否翻转门的方向\",\n                            \"default\": false\n                        }\n                    },\n                    \"required\": [\"wall_id\", \"position_x\", \"position_y\", \"door_type_id\"]\n                }\n            }\n        },\n        \"required\": [\"wall_id\", \"position_x\", \"position_y\", \"door_type_id\"]\n    }";

	[AsyncStateMachine(typeof(Class394))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class394 stateMachine = new Class394();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createDoorInWallTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class393))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class393 stateMachine = new Class393();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createDoorInWallTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class392))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class392 stateMachine = new Class392();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createDoorInWallTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
