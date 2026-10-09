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

[AITool("create_family_instance", Category = "元素创建", Description = "在指定位置创建族的实例。距离单位：毫米（工具内部会转换为英尺）", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateFamilyInstanceTool : IAITool
{
	[CompilerGenerated]
	private sealed class Class399 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IFamilyService ifamilyService_0;

		public IElementService ielementService_0;

		public CreateFamilyInstanceTool createFamilyInstanceTool_0;

		private object object_0;

		private List<object> list_0;

		private List<object> list_1;

		private List<(int index, string reason)> list_2;

		private Class56<int, int, int, List<object>, List<(int index, string reason)>> class56_0;

		private string string_0;

		private int int_1;

		private IDictionary<string, object> idictionary_0;

		private int int_2;

		private double double_0;

		private double double_1;

		private double double_2;

		private int int_3;

		private double double_3;

		private double double_4;

		private double double_5;

		private object object_1;

		private int? nullable_0;

		private object object_2;

		private string string_1;

		private string string_2;

		private object object_3;

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
					Class399 stateMachine = this;
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
			object_0 = aitoolContext_0.Document;
			list_0 = aitoolContext_0.GetParameter<List<object>>("instances", (List<object>)null);
			AIToolResult result;
			if (list_0 == null || list_0.Count == 0)
			{
				result = AIToolResult.Fail("instances 参数不能为空");
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
							int_2 = Convert.ToInt32(idictionary_0["type_id"]);
							double_0 = Convert.ToDouble(idictionary_0["x"]);
							double_1 = Convert.ToDouble(idictionary_0["y"]);
							double_2 = ((!idictionary_0.ContainsKey("z") || idictionary_0["z"] == null) ? 0.0 : Convert.ToDouble(idictionary_0["z"]));
							int_3 = Convert.ToInt32(idictionary_0["level_id"]);
							double_3 = double_0 / 304.8;
							double_4 = double_1 / 304.8;
							double_5 = double_2 / 304.8;
							object_1 = ifamilyService_0.CreateFamilyInstance(object_0, int_2, double_3, double_4, double_5, (int?)int_3);
							if (object_1 == null)
							{
								list_2.Add((int_1, "创建失败"));
							}
							else
							{
								nullable_0 = ielementService_0.GetElementId(object_1);
								object_2 = ielementService_0.GetElementById(object_0, int_2);
								string_1 = ((object_2 != null) ? ielementService_0.GetElementName(object_2) : "未知类型");
								string_2 = "未知族";
								object_3 = ifamilyService_0.GetFamilyOfTypeId(object_0, int_2);
								if (object_3 != null)
								{
									string_2 = ielementService_0.GetElementName(object_3) ?? "未知族";
								}
								list_1.Add(new Class55<int, int?, string, string, int, _003C_003Ef__AnonymousType27<double, double, double>, string, int>(int_1, nullable_0, string_2, string_1, int_2, new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(double_0, 0), Math.Round(double_1, 0), Math.Round(double_2, 0)), "millimeters", int_3));
								idictionary_0 = null;
								object_1 = null;
								object_2 = null;
								string_1 = null;
								string_2 = null;
								object_3 = null;
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
				class56_0 = new Class56<int, int, int, List<object>, List<(int, string)>>(list_0.Count, list_1.Count, list_2.Count, list_1, list_2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
				defaultInterpolatedStringHandler.AppendLiteral("批量创建族实例完成：成功 ");
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
				result = AIToolResult.Ok(string_0, (object)class56_0);
			}
			int_0 = -2;
			object_0 = null;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			class56_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class400 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IFamilyService ifamilyService_0;

		public IElementService ielementService_0;

		public CreateFamilyInstanceTool createFamilyInstanceTool_0;

		private object object_0;

		private int int_1;

		private double double_0;

		private double double_1;

		private double double_2;

		private int int_2;

		private double double_3;

		private double double_4;

		private double double_5;

		private object object_1;

		private int? nullable_0;

		private object object_2;

		private string string_0;

		private string string_1;

		private object object_3;

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
					Class400 stateMachine = this;
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
			object_0 = aitoolContext_0.Document;
			int_1 = aitoolContext_0.GetParameter<int>("type_id", 0);
			double_0 = aitoolContext_0.GetParameter<double>("x", 0.0);
			double_1 = aitoolContext_0.GetParameter<double>("y", 0.0);
			double_2 = aitoolContext_0.GetParameter<double>("z", 0.0);
			int_2 = aitoolContext_0.GetParameter<int>("level_id", 0);
			double_3 = double_0 / 304.8;
			double_4 = double_1 / 304.8;
			double_5 = double_2 / 304.8;
			object_1 = ifamilyService_0.CreateFamilyInstance(object_0, int_1, double_3, double_4, double_5, (int?)int_2);
			AIToolResult result;
			if (object_1 == null)
			{
				result = AIToolResult.Fail("创建族实例失败");
			}
			else
			{
				nullable_0 = ielementService_0.GetElementId(object_1);
				object_2 = ielementService_0.GetElementById(object_0, int_1);
				string_0 = ((object_2 != null) ? ielementService_0.GetElementName(object_2) : "未知类型");
				string_1 = "未知族";
				object_3 = ifamilyService_0.GetFamilyOfTypeId(object_0, int_1);
				if (object_3 != null)
				{
					string_1 = ielementService_0.GetElementName(object_3) ?? "未知族";
				}
				result = AIToolResult.Ok("成功创建族实例: " + string_1 + ": " + string_0, (object)new Class54<int?, string, string, _003C_003Ef__AnonymousType27<double, double, double>, string, int>(nullable_0, string_1, string_0, new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(double_0, 0), Math.Round(double_1, 0), Math.Round(double_2, 0)), "millimeters", int_2));
			}
			int_0 = -2;
			object_0 = null;
			object_1 = null;
			object_2 = null;
			string_0 = null;
			string_1 = null;
			object_3 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class401 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateFamilyInstanceTool createFamilyInstanceTool_0;

		private IFamilyService ifamilyService_0;

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
					Class401 stateMachine = this;
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
					goto IL_022e;
				}
				TaskAwaiter<AIToolResult> awaiter3;
				if (num == 2)
				{
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_01fb;
				}
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ifamilyService_0 = ((revitAdapter != null) ? revitAdapter.FamilyService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				if (ifamilyService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 FamilyService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else
				{
					if (aitoolContext_0.Document != null)
					{
						if (aitoolContext_0.HasParameter("instances"))
						{
							awaiter2 = createFamilyInstanceTool_0.method_1(aitoolContext_0, ifamilyService_0, ielementService_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class401 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_022e;
						}
						awaiter3 = createFamilyInstanceTool_0.method_0(aitoolContext_0, ifamilyService_0, ielementService_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter3;
							Class401 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
							return;
						}
						goto IL_01fb;
					}
					result = AIToolResult.Fail("文档对象为空");
				}
				goto end_IL_006e;
				IL_01fb:
				aitoolResult_1 = awaiter3.GetResult();
				result = aitoolResult_1;
				goto end_IL_006e;
				IL_022e:
				aitoolResult_0 = awaiter2.GetResult();
				result = aitoolResult_0;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建族实例失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_family_instance";

	public string Category => "元素创建";

	public string Description => "在指定位置创建族的实例。距离单位：毫米（工具内部会转换为英尺）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"族类型 ID（必选）。可使用 get_family_types 工具获取可用的族类型 ID（单个创建时使用）\"\n            },\n            \"x\": {\n                \"type\": \"number\",\n                \"description\": \"放置位置的 X 坐标（毫米）。单个创建时使用\"\n            },\n            \"y\": {\n                \"type\": \"number\",\n                \"description\": \"放置位置的 Y 坐标（毫米）。单个创建时使用\"\n            },\n            \"z\": {\n                \"type\": \"number\",\n                \"description\": \"放置位置的 Z 坐标（毫米，可选）。单个创建时使用\",\n                \"default\": 0.0\n            },\n            \"level_id\": {\n                \"type\": \"integer\",\n                \"description\": \"标高 ID（必需）。单个创建时使用\"\n            },\n            \"instances\": {\n                \"type\": \"array\",\n                \"description\": \"族实例数据列表（批量创建时使用）。如果提供此参数，将忽略单个创建参数。每个对象包含 type_id, x, y, z, level_id\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"type_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"族类型 ID（必选）\"\n                        },\n                        \"x\": {\n                            \"type\": \"number\",\n                            \"description\": \"放置位置的 X 坐标（毫米）\"\n                        },\n                        \"y\": {\n                            \"type\": \"number\",\n                            \"description\": \"放置位置的 Y 坐标（毫米）\"\n                        },\n                        \"z\": {\n                            \"type\": \"number\",\n                            \"description\": \"放置位置的 Z 坐标（毫米，可选）\",\n                            \"default\": 0.0\n                        },\n                        \"level_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"标高 ID（必需）\"\n                        }\n                    },\n                    \"required\": [\"type_id\", \"x\", \"y\", \"level_id\"]\n                }\n            }\n        }\n    }";

	[AsyncStateMachine(typeof(Class401))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class401 stateMachine = new Class401();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createFamilyInstanceTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class400))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IFamilyService ifamilyService_0, IElementService ielementService_0)
	{
		Class400 stateMachine = new Class400();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createFamilyInstanceTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ifamilyService_0 = ifamilyService_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class399))]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IFamilyService ifamilyService_0, IElementService ielementService_0)
	{
		Class399 stateMachine = new Class399();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createFamilyInstanceTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ifamilyService_0 = ifamilyService_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
