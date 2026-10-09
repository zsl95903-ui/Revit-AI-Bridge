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

[AITool("create_column", Category = "元素创建", Description = "在指定位置创建结构柱。所有距离单位为毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateColumnTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class388 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateColumnTool createColumnTool_0;

		private List<object> list_0;

		private List<object> list_1;

		private List<(int index, string reason)> list_2;

		private Class45<int, int, int, List<object>, List<(int index, string reason)>> class45_0;

		private string string_0;

		private int int_1;

		private IDictionary<string, object> idictionary_0;

		private double double_0;

		private double double_1;

		private int int_2;

		private int int_3;

		private object object_0;

		private int? nullable_0;

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
					Class388 stateMachine = this;
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
			list_0 = aitoolContext_0.GetParameter<List<object>>("columns", (List<object>)null);
			AIToolResult result;
			if (list_0 == null || list_0.Count == 0)
			{
				result = AIToolResult.Fail("columns 参数不能为空");
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
							double_0 = Convert.ToDouble(idictionary_0["position_x"]) / 304.8;
							double_1 = Convert.ToDouble(idictionary_0["position_y"]) / 304.8;
							int_2 = Convert.ToInt32(idictionary_0["level_id"]);
							int_3 = Convert.ToInt32(idictionary_0["column_type_id"]);
							object_0 = ielementService_0.CreateColumn(aitoolContext_0.Document, double_0, double_1, int_2, 0.0, (int?)int_3);
							if (object_0 == null)
							{
								list_2.Add((int_1, "创建失败"));
							}
							else
							{
								nullable_0 = ielementService_0.GetElementId(object_0);
								list_1.Add(new Class44<int, int?, double, double, int, int>(int_1, nullable_0, double_0 * 304.8, double_1 * 304.8, int_2, int_3));
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
				class45_0 = new Class45<int, int, int, List<object>, List<(int, string)>>(list_0.Count, list_1.Count, list_2.Count, list_1, list_2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
				defaultInterpolatedStringHandler.AppendLiteral("批量创建柱完成：成功 ");
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
				result = AIToolResult.Ok(string_0, (object)class45_0);
			}
			int_0 = -2;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			class45_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class389 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateColumnTool createColumnTool_0;

		private double double_0;

		private double double_1;

		private int int_1;

		private int int_2;

		private object object_0;

		private int? nullable_0;

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
					Class389 stateMachine = this;
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
			double_0 = aitoolContext_0.GetParameter<double>("position_x", 0.0) / 304.8;
			double_1 = aitoolContext_0.GetParameter<double>("position_y", 0.0) / 304.8;
			int_1 = aitoolContext_0.GetParameter<int>("level_id", 0);
			int_2 = aitoolContext_0.GetParameter<int>("column_type_id", 0);
			object_0 = ielementService_0.CreateColumn(aitoolContext_0.Document, double_0, double_1, int_1, 0.0, (int?)int_2);
			AIToolResult result;
			if (object_0 == null)
			{
				result = AIToolResult.Fail("创建柱失败");
			}
			else
			{
				nullable_0 = ielementService_0.GetElementId(object_0);
				string_0 = ielementService_0.GetElementName(object_0);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("成功创建柱，ID: ");
				defaultInterpolatedStringHandler.AppendFormatted(nullable_0);
				result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class43<int?, string>(nullable_0, string_0 ?? "未命名"));
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
	public sealed class Class390 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateColumnTool createColumnTool_0;

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
					Class390 stateMachine = this;
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
						if (aitoolContext_0.HasParameter("columns"))
						{
							awaiter2 = createColumnTool_0.method_1(aitoolContext_0, ielementService_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class390 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_01e4;
						}
						awaiter3 = createColumnTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter3;
							Class390 stateMachine = this;
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
				result = AIToolResult.Fail("创建柱失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_column";

	public string Category => "元素创建";

	public string Description => "在指定位置创建结构柱";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"position_x\": {\n                \"type\": \"number\",\n                \"description\": \"柱的 X 坐标（毫米）。单个创建时使用\"\n            },\n            \"position_y\": {\n                \"type\": \"number\",\n                \"description\": \"柱的 Y 坐标（毫米）。单个创建时使用\"\n            },\n            \"level_id\": {\n                \"type\": \"integer\",\n                \"description\": \"柱所在的标高 ID。单个创建时使用\"\n            },\n            \"column_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"柱类型 ID。单个创建时使用（必选）。可使用 get_family_types 工具获取可用的柱类型 ID\"\n            },\n            \"columns\": {\n                \"type\": \"array\",\n                \"description\": \"柱数据列表（批量创建时使用）。如果提供此参数，将忽略单个创建参数。每个对象包含 position_x, position_y, level_id, column_type_id\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"position_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"柱的 X 坐标（毫米）\"\n                        },\n                        \"position_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"柱的 Y 坐标（毫米）\"\n                        },\n                        \"level_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"柱所在的标高 ID\"\n                        },\n                        \"column_type_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"柱类型 ID（必选）\"\n                        }\n                    },\n                    \"required\": [\"position_x\", \"position_y\", \"level_id\", \"column_type_id\"]\n                }\n            }\n        }\n    }";

	[AsyncStateMachine(typeof(Class390))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class390 stateMachine = new Class390();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createColumnTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class389))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class389 stateMachine = new Class389();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createColumnTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class388))]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class388 stateMachine = new Class388();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createColumnTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
