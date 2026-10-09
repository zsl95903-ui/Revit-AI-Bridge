using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("create_straight_wall", Category = "元素创建", Description = "在指定标高处创建直墙。所有距离单位为毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateStraightWallTool : IAITool
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct4
	{
		public IDictionary<string, object> idictionary_0;
	}

	[CompilerGenerated]
	public sealed class Class419 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateStraightWallTool createStraightWallTool_0;

		private List<object> list_0;

		private List<object> list_1;

		private List<(int index, string reason)> list_2;

		private Class87<int, int, int, List<object>, List<(int index, string reason)>> class87_0;

		private string string_0;

		private int int_1;

		private Struct4 struct4_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private int int_2;

		private double double_4;

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
					Class419 stateMachine = this;
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
			list_0 = aitoolContext_0.GetParameter<List<object>>("walls", (List<object>)null);
			AIToolResult result;
			if (list_0 == null || list_0.Count == 0)
			{
				result = AIToolResult.Fail("walls 参数不能为空");
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
						struct4_0.idictionary_0 = list_0[int_1] as IDictionary<string, object>;
						if (struct4_0.idictionary_0 == null)
						{
							list_2.Add((int_1, "数据格式错误"));
						}
						else
						{
							double_0 = smethod_0("start_x", 0.0, ref struct4_0) / 304.8;
							double_1 = smethod_0("start_y", 0.0, ref struct4_0) / 304.8;
							double_2 = smethod_0("end_x", 0.0, ref struct4_0) / 304.8;
							double_3 = smethod_0("end_y", 0.0, ref struct4_0) / 304.8;
							int_2 = smethod_1("level_id", 0, ref struct4_0);
							double_4 = smethod_0("height", 3000.0, ref struct4_0) / 304.8;
							nullable_0 = smethod_2("wall_type_id", ref struct4_0);
							object_0 = ielementService_0.CreateStraightWall(aitoolContext_0.Document, double_0, double_1, double_2, double_3, int_2, double_4, nullable_0);
							if (object_0 == null)
							{
								List<(int index, string reason)> list = list_2;
								int item = int_1;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 7);
								defaultInterpolatedStringHandler.AppendLiteral("创建失败 (起点=(");
								defaultInterpolatedStringHandler.AppendFormatted(double_0, "F4");
								defaultInterpolatedStringHandler.AppendLiteral(",");
								defaultInterpolatedStringHandler.AppendFormatted(double_1, "F4");
								defaultInterpolatedStringHandler.AppendLiteral("), 终点=(");
								defaultInterpolatedStringHandler.AppendFormatted(double_2, "F4");
								defaultInterpolatedStringHandler.AppendLiteral(",");
								defaultInterpolatedStringHandler.AppendFormatted(double_3, "F4");
								defaultInterpolatedStringHandler.AppendLiteral("), 标高=");
								defaultInterpolatedStringHandler.AppendFormatted(int_2);
								defaultInterpolatedStringHandler.AppendLiteral(", 高度=");
								defaultInterpolatedStringHandler.AppendFormatted(double_4, "F4");
								defaultInterpolatedStringHandler.AppendLiteral(", 类型=");
								defaultInterpolatedStringHandler.AppendFormatted(nullable_0);
								defaultInterpolatedStringHandler.AppendLiteral(")");
								list.Add((item, defaultInterpolatedStringHandler.ToStringAndClear()));
							}
							else
							{
								nullable_1 = ielementService_0.GetElementId(object_0);
								list_1.Add(new Class86<int, int?, double, double, double, double, int, double, int?>(int_1, nullable_1, double_0 * 304.8, double_1 * 304.8, double_2 * 304.8, double_3 * 304.8, int_2, double_4 * 304.8, nullable_0));
								struct4_0 = default(Struct4);
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
				class87_0 = new Class87<int, int, int, List<object>, List<(int, string)>>(list_0.Count, list_1.Count, list_2.Count, list_1, list_2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("批量创建墙体完成：成功 ");
				defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler2.AppendLiteral("/");
				defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
				string_0 = defaultInterpolatedStringHandler2.ToStringAndClear();
				if (list_2.Count > 0)
				{
					string text = string_0;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("，失败 ");
					defaultInterpolatedStringHandler3.AppendFormatted(list_2.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个");
					string_0 = text + defaultInterpolatedStringHandler3.ToStringAndClear();
				}
				result = AIToolResult.Ok(string_0, (object)class87_0);
			}
			int_0 = -2;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			class87_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class420 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateStraightWallTool createStraightWallTool_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private int int_1;

		private double double_4;

		private int? nullable_0;

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
					Class420 stateMachine = this;
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
			double_4 = aitoolContext_0.GetParameter<double>("height", 3000.0) / 304.8;
			nullable_0 = aitoolContext_0.GetParameter<int?>("wall_type_id", (int?)null);
			object_0 = ielementService_0.CreateStraightWall(aitoolContext_0.Document, double_0, double_1, double_2, double_3, int_1, double_4, nullable_0);
			AIToolResult result;
			if (object_0 == null)
			{
				result = AIToolResult.Fail("创建墙体失败");
			}
			else
			{
				nullable_1 = ielementService_0.GetElementId(object_0);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler.AppendLiteral("成功创建墙体，ID: ");
				defaultInterpolatedStringHandler.AppendFormatted(nullable_1);
				result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class85<int?, string>(nullable_1, "墙体创建成功"));
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
	public sealed class Class421 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateStraightWallTool createStraightWallTool_0;

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
					Class421 stateMachine = this;
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
						if (aitoolContext_0.HasParameter("walls"))
						{
							awaiter2 = createStraightWallTool_0.method_1(aitoolContext_0, ielementService_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class421 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_01e4;
						}
						awaiter3 = createStraightWallTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter3;
							Class421 stateMachine = this;
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
				result = AIToolResult.Fail("创建墙体失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_straight_wall";

	public string Category => "元素创建";

	public string Description => "在指定标高处创建直墙。所有距离单位为毫米";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"start_x\": {\n                \"type\": \"number\",\n                \"description\": \"起点 X 坐标（毫米）。单个创建时使用\"\n            },\n            \"start_y\": {\n                \"type\": \"number\",\n                \"description\": \"起点 Y 坐标（毫米）。单个创建时使用\"\n            },\n            \"end_x\": {\n                \"type\": \"number\",\n                \"description\": \"终点 X 坐标（毫米）。单个创建时使用\"\n            },\n            \"end_y\": {\n                \"type\": \"number\",\n                \"description\": \"终点 Y 坐标（毫米）。单个创建时使用\"\n            },\n            \"level_id\": {\n                \"type\": \"integer\",\n                \"description\": \"墙所在的标高 ID。单个创建时使用\"\n            },\n            \"height\": {\n                \"type\": \"number\",\n                \"description\": \"墙的高度（毫米），默认为 3000 毫米。单个创建时使用\",\n                \"default\": 3000.0\n            },\n            \"wall_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"墙类型 ID（必选）。可使用 get_family_types 工具获取可用的墙类型 ID（单个创建时使用）\"\n            },\n            \"walls\": {\n                \"type\": \"array\",\n                \"description\": \"墙数据列表（批量创建时使用）。如果提供此参数，将忽略单个创建参数。每个对象包含 start_x, start_y, end_x, end_y, level_id, height, wall_type_id\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"start_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"起点 X 坐标（毫米）\"\n                        },\n                        \"start_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"起点 Y 坐标（毫米）\"\n                        },\n                        \"end_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"终点 X 坐标（毫米）\"\n                        },\n                        \"end_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"终点 Y 坐标（毫米）\"\n                        },\n                        \"level_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"墙所在的标高 ID\"\n                        },\n                        \"height\": {\n                            \"type\": \"number\",\n                            \"description\": \"墙的高度（毫米），默认为 3000 毫米\",\n                            \"default\": 3000.0\n                        },\n                        \"wall_type_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"墙类型 ID（必选）\"\n                        }\n                    },\n                    \"required\": [\"start_x\", \"start_y\", \"end_x\", \"end_y\", \"level_id\", \"wall_type_id\"]\n                }\n            }\n        }\n    }";

	[AsyncStateMachine(typeof(Class421))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class421 stateMachine = new Class421();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createStraightWallTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class420))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class420 stateMachine = new Class420();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createStraightWallTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class419))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class419 stateMachine = new Class419();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createStraightWallTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[CompilerGenerated]
	internal static double smethod_0(string string_0, double double_0, ref Struct4 struct4_0)
	{
		if (!struct4_0.idictionary_0.ContainsKey(string_0) || struct4_0.idictionary_0[string_0] == null)
		{
			return double_0;
		}
		object obj = struct4_0.idictionary_0[string_0];
		if (obj is double result)
		{
			return result;
		}
		if (obj is long num)
		{
			return num;
		}
		if (obj is int num2)
		{
			return num2;
		}
		if (obj is decimal num3)
		{
			return (double)num3;
		}
		return Convert.ToDouble(obj);
	}

	[CompilerGenerated]
	internal static int smethod_1(string string_0, int int_0, ref Struct4 struct4_0)
	{
		if (!struct4_0.idictionary_0.ContainsKey(string_0) || struct4_0.idictionary_0[string_0] == null)
		{
			return int_0;
		}
		object obj = struct4_0.idictionary_0[string_0];
		if (obj is int result)
		{
			return result;
		}
		if (obj is long num)
		{
			return (int)num;
		}
		return Convert.ToInt32(obj);
	}

	[CompilerGenerated]
	internal static int? smethod_2(string string_0, ref Struct4 struct4_0)
	{
		if (!struct4_0.idictionary_0.ContainsKey(string_0) || struct4_0.idictionary_0[string_0] == null)
		{
			return null;
		}
		object obj = struct4_0.idictionary_0[string_0];
		if (obj is int value)
		{
			return value;
		}
		if (obj is long num)
		{
			return (int)num;
		}
		return Convert.ToInt32(obj);
	}
}
