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

[AITool("create_cable_tray", Category = "元素创建", Description = "在指定位置创建桥架。所有距离单位为毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateCableTrayTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class385 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateCableTrayTool createCableTrayTool_0;

		private List<object> list_0;

		private List<object> list_1;

		private List<(int index, string reason)> list_2;

		private Class42<int, int, int, List<object>, List<(int index, string reason)>> class42_0;

		private string string_0;

		private int int_1;

		private IDictionary<string, object> idictionary_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private double double_4;

		private double double_5;

		private int int_2;

		private double double_6;

		private double double_7;

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
					Class385 stateMachine = this;
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
			list_0 = aitoolContext_0.GetParameter<List<object>>("cable_trays", (List<object>)null);
			AIToolResult result;
			if (list_0 == null || list_0.Count == 0)
			{
				result = AIToolResult.Fail("cable_trays 参数不能为空");
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
							double_2 = Convert.ToDouble(idictionary_0["start_z"]) / 304.8;
							double_3 = Convert.ToDouble(idictionary_0["end_x"]) / 304.8;
							double_4 = Convert.ToDouble(idictionary_0["end_y"]) / 304.8;
							double_5 = Convert.ToDouble(idictionary_0["end_z"]) / 304.8;
							int_2 = Convert.ToInt32(idictionary_0["cable_tray_type_id"]);
							double_6 = ((!idictionary_0.ContainsKey("width") || idictionary_0["width"] == null) ? 300.0 : Convert.ToDouble(idictionary_0["width"]));
							double_7 = ((!idictionary_0.ContainsKey("height") || idictionary_0["height"] == null) ? 150.0 : Convert.ToDouble(idictionary_0["height"]));
							nullable_0 = null;
							if (idictionary_0.ContainsKey("level_id") && idictionary_0["level_id"] != null)
							{
								nullable_0 = Convert.ToInt32(idictionary_0["level_id"]);
								goto IL_0413;
							}
							nullable_0 = createCableTrayTool_0.method_2(aitoolContext_0, double_2 * 304.8);
							if (nullable_0.HasValue)
							{
								goto IL_0413;
							}
							List<(int index, string reason)> list = list_2;
							int item = int_1;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
							defaultInterpolatedStringHandler.AppendLiteral("无法找到标高（startZ=");
							defaultInterpolatedStringHandler.AppendFormatted(double_2 * 304.8, "F2");
							defaultInterpolatedStringHandler.AppendLiteral("mm）");
							list.Add((item, defaultInterpolatedStringHandler.ToStringAndClear()));
						}
						goto end_IL_00e9;
						IL_0413:
						object_0 = ielementService_0.CreateCableTray(aitoolContext_0.Document, double_0, double_1, double_2, double_3, double_4, double_5, nullable_0.Value, int_2, double_6, double_7);
						if (object_0 == null)
						{
							list_2.Add((int_1, "创建失败"));
						}
						else
						{
							nullable_1 = ielementService_0.GetElementId(object_0);
							list_1.Add(new Class41<int, int?, double, double, double, double, double, double, double, double, int>(int_1, nullable_1, double_0 * 304.8, double_1 * 304.8, double_2 * 304.8, double_3 * 304.8, double_4 * 304.8, double_5 * 304.8, double_6, double_7, int_2));
							idictionary_0 = null;
							object_0 = null;
						}
						end_IL_00e9:;
					}
					catch (Exception ex)
					{
						exception_0 = ex;
						list_2.Add((int_1, exception_0.Message));
					}
					int_1++;
				}
				class42_0 = new Class42<int, int, int, List<object>, List<(int, string)>>(list_0.Count, list_1.Count, list_2.Count, list_1, list_2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("批量创建桥架完成：成功 ");
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
				result = AIToolResult.Ok(string_0, (object)class42_0);
			}
			int_0 = -2;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			class42_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class386 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateCableTrayTool createCableTrayTool_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private double double_4;

		private double double_5;

		private int int_1;

		private double double_6;

		private double double_7;

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
					Class386 stateMachine = this;
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
			double_2 = aitoolContext_0.GetParameter<double>("start_z", 0.0) / 304.8;
			double_3 = aitoolContext_0.GetParameter<double>("end_x", 0.0) / 304.8;
			double_4 = aitoolContext_0.GetParameter<double>("end_y", 0.0) / 304.8;
			double_5 = aitoolContext_0.GetParameter<double>("end_z", 0.0) / 304.8;
			int_1 = aitoolContext_0.GetParameter<int>("cable_tray_type_id", 0);
			double_6 = aitoolContext_0.GetParameter<double>("width", 300.0);
			double_7 = aitoolContext_0.GetParameter<double>("height", 150.0);
			nullable_0 = null;
			AIToolResult result;
			if (aitoolContext_0.HasParameter("level_id"))
			{
				nullable_0 = aitoolContext_0.GetParameter<int>("level_id", 0);
			}
			else
			{
				nullable_0 = createCableTrayTool_0.method_2(aitoolContext_0, double_2 * 304.8);
				if (!nullable_0.HasValue)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
					defaultInterpolatedStringHandler.AppendLiteral("无法找到标高（startZ=");
					defaultInterpolatedStringHandler.AppendFormatted(double_2 * 304.8, "F2");
					defaultInterpolatedStringHandler.AppendLiteral("mm）");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					goto IL_042d;
				}
			}
			object_0 = ielementService_0.CreateCableTray(aitoolContext_0.Document, double_0, double_1, double_2, double_3, double_4, double_5, nullable_0.Value, int_1, double_6, double_7);
			if (object_0 == null)
			{
				result = AIToolResult.Fail("创建桥架失败");
			}
			else
			{
				nullable_1 = ielementService_0.GetElementId(object_0);
				string_0 = ielementService_0.GetElementName(object_0);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("成功创建桥架，ID: ");
				defaultInterpolatedStringHandler2.AppendFormatted(nullable_1);
				result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class40<int?, string, double, double, double, double, double, double, double, double>(nullable_1, string_0 ?? "未命名", double_0 * 304.8, double_1 * 304.8, double_2 * 304.8, double_3 * 304.8, double_4 * 304.8, double_5 * 304.8, double_6, double_7));
			}
			goto IL_042d;
			IL_042d:
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
	public sealed class Class387 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateCableTrayTool createCableTrayTool_0;

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
					Class387 stateMachine = this;
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
						if (aitoolContext_0.HasParameter("cable_trays"))
						{
							awaiter2 = createCableTrayTool_0.method_1(aitoolContext_0, ielementService_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class387 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_01e4;
						}
						awaiter3 = createCableTrayTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter3;
							Class387 stateMachine = this;
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
				result = AIToolResult.Fail("创建桥架失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_cable_tray";

	public string Category => "元素创建";

	public string Description => "在指定位置创建桥架。所有距离单位为毫米";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"start_x\": {\n                \"type\": \"number\",\n                \"description\": \"起点 X 坐标（毫米）。单个创建时使用\"\n            },\n            \"start_y\": {\n                \"type\": \"number\",\n                \"description\": \"起点 Y 坐标（毫米）。单个创建时使用\"\n            },\n            \"start_z\": {\n                \"type\": \"number\",\n                \"description\": \"起点 Z 坐标（毫米）。单个创建时使用\"\n            },\n            \"end_x\": {\n                \"type\": \"number\",\n                \"description\": \"终点 X 坐标（毫米）。单个创建时使用\"\n            },\n            \"end_y\": {\n                \"type\": \"number\",\n                \"description\": \"终点 Y 坐标（毫米）。单个创建时使用\"\n            },\n            \"end_z\": {\n                \"type\": \"number\",\n                \"description\": \"终点 Z 坐标（毫米）。单个创建时使用\"\n            },\n            \"level_id\": {\n                \"type\": \"integer\",\n                \"description\": \"桥架所在的标高 ID（单个创建时使用）。如果不指定，将根据 start_z 自动查找最近的标高\"\n            },\n            \"cable_tray_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"桥架类型 ID（单个创建时使用，必选）。可使用 get_cable_tray_types 工具获取可用的桥架类型 ID\"\n            },\n            \"width\": {\n                \"type\": \"number\",\n                \"description\": \"桥架宽度（毫米），默认为 300 毫米。单个创建时使用\",\n                \"default\": 300.0\n            },\n            \"height\": {\n                \"type\": \"number\",\n                \"description\": \"桥架高度（毫米），默认为 150 毫米。单个创建时使用\",\n                \"default\": 150.0\n            },\n            \"cable_trays\": {\n                \"type\": \"array\",\n                \"description\": \"桥架数据列表（批量创建时使用）。如果提供此参数，将忽略单个创建参数\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"start_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"起点 X 坐标（毫米）\"\n                        },\n                        \"start_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"起点 Y 坐标（毫米）\"\n                        },\n                        \"start_z\": {\n                            \"type\": \"number\",\n                            \"description\": \"起点 Z 坐标（毫米）\"\n                        },\n                        \"end_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"终点 X 坐标（毫米）\"\n                        },\n                        \"end_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"终点 Y 坐标（毫米）\"\n                        },\n                        \"end_z\": {\n                            \"type\": \"number\",\n                            \"description\": \"终点 Z 坐标（毫米）\"\n                        },\n                        \"level_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"桥架所在的标高 ID。如果不指定，将根据 start_z 自动查找最近的标高\"\n                        },\n                        \"cable_tray_type_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"桥架类型 ID（必选）\"\n                        },\n                        \"width\": {\n                            \"type\": \"number\",\n                            \"description\": \"桥架宽度（毫米），默认为 300 毫米\"\n                        },\n                        \"height\": {\n                            \"type\": \"number\",\n                            \"description\": \"桥架高度（毫米），默认为 150 毫米\"\n                        }\n                    },\n                    \"required\": [\"start_x\", \"start_y\", \"start_z\", \"end_x\", \"end_y\", \"end_z\", \"cable_tray_type_id\"]\n                }\n            }\n        }\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class387))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class387 stateMachine = new Class387();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createCableTrayTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class386))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class386 stateMachine = new Class386();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createCableTrayTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class385))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class385 stateMachine = new Class385();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createCableTrayTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private int? method_2(AIToolContext aitoolContext_0, double double_0)
	{
		try
		{
			double num = double_0 / 304.8;
			IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
			object obj;
			if (revitAdapter == null)
			{
				obj = null;
			}
			else
			{
				IElementService elementService = revitAdapter.ElementService;
				obj = ((elementService != null) ? elementService.GetElementsByType(aitoolContext_0.Document, "Level") : null);
			}
			IEnumerable<object> enumerable = (IEnumerable<object>)obj;
			if (enumerable == null)
			{
				return null;
			}
			int? result = null;
			double num2 = double.MaxValue;
			foreach (object item in enumerable)
			{
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				object obj2;
				if (revitAdapter2 == null)
				{
					obj2 = null;
				}
				else
				{
					IElementService elementService2 = revitAdapter2.ElementService;
					obj2 = ((elementService2 != null) ? elementService2.GetAllParameters(item) : null);
				}
				IEnumerable<(string, object, string)> enumerable2 = (IEnumerable<(string, object, string)>)obj2;
				if (enumerable2 == null)
				{
					continue;
				}
				(string, object, string) tuple = enumerable2.FirstOrDefault<(string, object, string)>(((string Name, object Value, string Type) valueTuple_0) => valueTuple_0.Name == "标高" || valueTuple_0.Name == "Elevation");
				if (tuple.Item2 == null || !(tuple.Item2 is double num3))
				{
					continue;
				}
				double num4 = Math.Abs(num3 - num);
				if (num4 < num2)
				{
					num2 = num4;
					IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
					int? obj3;
					if (revitAdapter3 == null)
					{
						obj3 = null;
					}
					else
					{
						IElementService elementService3 = revitAdapter3.ElementService;
						obj3 = ((elementService3 != null) ? elementService3.GetElementId(item) : ((int?)null));
					}
					result = obj3;
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			Logger.Error("[CreateCableTrayTool] 查找标高失败: " + ex.Message);
			return null;
		}
	}
}
