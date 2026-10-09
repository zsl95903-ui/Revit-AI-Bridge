using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using Newtonsoft.Json.Linq;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("create_floor_by_profile", Category = "元素创建", Description = "通过定义的轮廓点创建楼板。所有距离单位为毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateFloorByProfileTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class402 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateFloorByProfileTool createFloorByProfileTool_0;

		private List<object> list_0;

		private List<object> list_1;

		private List<(int index, string reason)> list_2;

		private Class59<int, int, int, List<object>, List<(int index, string reason)>> class59_0;

		private string string_0;

		private int int_1;

		private IDictionary<string, object> idictionary_0;

		private object object_0;

		private object[] object_1;

		private List<(double X, double Y)> list_3;

		private bool bool_0;

		private string string_1;

		private object object_2;

		private int int_2;

		private int? nullable_0;

		private object object_3;

		private object object_4;

		private int? nullable_1;

		private object[] object_5;

		private int int_3;

		private object object_6;

		private double double_0;

		private double double_1;

		private bool bool_1;

		private JObject jobject_0;

		private JToken jtoken_0;

		private JToken jtoken_1;

		private IDictionary<string, object> idictionary_1;

		private object object_7;

		private object object_8;

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
					Class402 stateMachine = this;
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
			list_0 = aitoolContext_0.GetParameter<List<object>>("floors", (List<object>)null);
			AIToolResult result;
			if (list_0 == null || list_0.Count == 0)
			{
				result = AIToolResult.Fail("floors 参数不能为空");
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
						else if (!idictionary_0.TryGetValue("points", out object_0) || object_0 == null)
						{
							list_2.Add((int_1, "缺少 points 参数"));
						}
						else
						{
							object_1 = object_0 as object[];
							if (object_1 == null || object_1.Length < 3)
							{
								list_2.Add((int_1, "轮廓点至少需要 3 个点"));
							}
							else
							{
								list_3 = new List<(double, double)>();
								bool_0 = true;
								string_1 = "";
								object_5 = object_1;
								int_3 = 0;
								while (int_3 < object_5.Length)
								{
									object_6 = object_5[int_3];
									double_0 = 0.0;
									double_1 = 0.0;
									bool_1 = false;
									object obj = object_6;
									jobject_0 = (JObject)((obj is JObject) ? obj : null);
									if (jobject_0 != null)
									{
										jtoken_0 = jobject_0["x"];
										jtoken_1 = jobject_0["y"];
										if (jtoken_0 != null && jtoken_1 != null)
										{
											double_0 = jtoken_0.ToObject<double>() / 304.8;
											double_1 = jtoken_1.ToObject<double>() / 304.8;
											bool_1 = true;
										}
										jtoken_0 = null;
										jtoken_1 = null;
									}
									else
									{
										idictionary_1 = object_6 as IDictionary<string, object>;
										if (idictionary_1 != null)
										{
											if (idictionary_1.TryGetValue("x", out object_7) && idictionary_1.TryGetValue("y", out object_8))
											{
												double_0 = Convert.ToDouble(object_7) / 304.8;
												double_1 = Convert.ToDouble(object_8) / 304.8;
												bool_1 = true;
											}
											object_7 = null;
											object_8 = null;
										}
										idictionary_1 = null;
									}
									if (bool_1)
									{
										list_3.Add((double_0, double_1));
										jobject_0 = null;
										object_6 = null;
										int_3++;
										continue;
									}
									bool_0 = false;
									string_1 = "点的格式不正确";
									break;
								}
								object_5 = null;
								if (!bool_0)
								{
									list_2.Add((int_1, string_1));
								}
								else if (list_3.Count >= 3)
								{
									if (!idictionary_0.TryGetValue("level_id", out object_2))
									{
										list_2.Add((int_1, "缺少 level_id 参数"));
									}
									else
									{
										int_2 = Convert.ToInt32(object_2);
										nullable_0 = null;
										if (idictionary_0.TryGetValue("floor_type_id", out object_3) && object_3 != null)
										{
											nullable_0 = Convert.ToInt32(object_3);
										}
										object_4 = ielementService_0.CreateFloorByProfile(aitoolContext_0.Document, (IEnumerable<ValueTuple<double, double>>)list_3, int_2, nullable_0);
										if (object_4 == null)
										{
											list_2.Add((int_1, "创建失败"));
										}
										else
										{
											nullable_1 = ielementService_0.GetElementId(object_4);
											list_1.Add(new Class58<int, int?, int, int?, int>(int_1, nullable_1, int_2, nullable_0, list_3.Count));
											idictionary_0 = null;
											object_0 = null;
											object_1 = null;
											list_3 = null;
											string_1 = null;
											object_2 = null;
											object_3 = null;
											object_4 = null;
										}
									}
								}
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
				class59_0 = new Class59<int, int, int, List<object>, List<(int, string)>>(list_0.Count, list_1.Count, list_2.Count, list_1, list_2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
				defaultInterpolatedStringHandler.AppendLiteral("批量创建楼板完成：成功 ");
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
				result = AIToolResult.Ok(string_0, (object)class59_0);
			}
			int_0 = -2;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			class59_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class403 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public CreateFloorByProfileTool createFloorByProfileTool_0;

		private object object_0;

		private object[] object_1;

		private object[] object_2;

		private int int_1;

		private int? nullable_0;

		private List<(double X, double Y)> list_0;

		private object object_3;

		private int? nullable_1;

		private string string_0;

		private IList ilist_0;

		private object[] object_4;

		private int int_2;

		private object object_5;

		private double double_0;

		private double double_1;

		private bool bool_0;

		private JObject jobject_0;

		private JToken jtoken_0;

		private JToken jtoken_1;

		private IDictionary<string, object> idictionary_0;

		private object object_6;

		private object object_7;

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
					Class403 stateMachine = this;
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
			object_0 = aitoolContext_0.GetParameter<object>("points", (object)null);
			AIToolResult result;
			if (object_0 == null)
			{
				result = AIToolResult.Fail("缺少 points 参数。楼板需要至少 3 个轮廓点来定义形状。请提供 points 数组，例如：[{\"x\": 0, \"y\": 0}, {\"x\": 5000, \"y\": 0}, {\"x\": 5000, \"y\": 3000}]");
			}
			else
			{
				object_2 = object_0 as object[];
				if (object_2 != null)
				{
					object_1 = object_2;
				}
				else
				{
					ilist_0 = object_0 as IList;
					if (ilist_0 == null)
					{
						result = AIToolResult.Fail("points 参数格式不正确。应该是数组或列表格式，例如：[{\"x\": 0, \"y\": 0}, {\"x\": 5000, \"y\": 0}]");
						goto IL_04db;
					}
					object_1 = new object[ilist_0.Count];
					ilist_0.CopyTo(object_1, 0);
					ilist_0 = null;
				}
				if (object_1.Length < 3)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 1);
					defaultInterpolatedStringHandler.AppendLiteral("轮廓点至少需要 3 个点，当前只提供了 ");
					defaultInterpolatedStringHandler.AppendFormatted(object_1.Length);
					defaultInterpolatedStringHandler.AppendLiteral(" 个点。请提供至少 3 个点来定义楼板轮廓");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					int_1 = aitoolContext_0.GetParameter<int>("level_id", 0);
					nullable_0 = aitoolContext_0.GetParameter<int?>("floor_type_id", (int?)null);
					list_0 = new List<(double, double)>();
					object_4 = object_1;
					int_2 = 0;
					while (true)
					{
						if (int_2 < object_4.Length)
						{
							object_5 = object_4[int_2];
							double_0 = 0.0;
							double_1 = 0.0;
							bool_0 = false;
							object obj = object_5;
							jobject_0 = (JObject)((obj is JObject) ? obj : null);
							if (jobject_0 != null)
							{
								jtoken_0 = jobject_0["x"];
								jtoken_1 = jobject_0["y"];
								if (jtoken_0 != null && jtoken_1 != null)
								{
									double_0 = jtoken_0.ToObject<double>() / 304.8;
									double_1 = jtoken_1.ToObject<double>() / 304.8;
									bool_0 = true;
								}
								jtoken_0 = null;
								jtoken_1 = null;
							}
							else
							{
								idictionary_0 = object_5 as IDictionary<string, object>;
								if (idictionary_0 != null)
								{
									if (idictionary_0.TryGetValue("x", out object_6) && idictionary_0.TryGetValue("y", out object_7))
									{
										double_0 = Convert.ToDouble(object_6) / 304.8;
										double_1 = Convert.ToDouble(object_7) / 304.8;
										bool_0 = true;
									}
									object_6 = null;
									object_7 = null;
								}
								idictionary_0 = null;
							}
							if (bool_0)
							{
								list_0.Add((double_0, double_1));
								jobject_0 = null;
								object_5 = null;
								int_2++;
								continue;
							}
							result = AIToolResult.Fail("点的格式不正确。每个点必须是包含 x 和 y 坐标的对象，例如：{\"x\": 0, \"y\": 0}");
							break;
						}
						object_4 = null;
						object_3 = ielementService_0.CreateFloorByProfile(aitoolContext_0.Document, (IEnumerable<ValueTuple<double, double>>)list_0, int_1, nullable_0);
						if (object_3 == null)
						{
							result = AIToolResult.Fail("创建楼板失败");
							break;
						}
						nullable_1 = ielementService_0.GetElementId(object_3);
						string_0 = ielementService_0.GetElementName(object_3);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(11, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("成功创建楼板，ID: ");
						defaultInterpolatedStringHandler2.AppendFormatted(nullable_1);
						result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class57<int?, string, int>(nullable_1, string_0 ?? "未命名", list_0.Count));
						break;
					}
				}
			}
			goto IL_04db;
			IL_04db:
			int_0 = -2;
			object_0 = null;
			object_1 = null;
			object_2 = null;
			list_0 = null;
			object_3 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class404 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateFloorByProfileTool createFloorByProfileTool_0;

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
					Class404 stateMachine = this;
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
						if (aitoolContext_0.HasParameter("floors"))
						{
							awaiter2 = createFloorByProfileTool_0.method_1(aitoolContext_0, ielementService_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class404 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_01e4;
						}
						awaiter3 = createFloorByProfileTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter3;
							Class404 stateMachine = this;
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
				result = AIToolResult.Fail("创建楼板失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_floor_by_profile";

	public string Category => "元素创建";

	public string Description => "通过定义的轮廓点创建楼板";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"points\": {\n                \"type\": \"array\",\n                \"description\": \"轮廓点列表（必填，至少3个点，按顺序连接形成闭合轮廓）。每个点包含 x 和 y 坐标（毫米）。例如：创建矩形楼板：[{\\\"x\\\": 0, \\\"y\\\": 0}, {\\\"x\\\": 5000, \\\"y\\\": 0}, {\\\"x\\\": 5000, \\\"y\\\": 3000}, {\\\"x\\\": 0, \\\"y\\\": 3000}]\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"x\": { \"type\": \"number\", \"description\": \"X 坐标（毫米）\" },\n                        \"y\": { \"type\": \"number\", \"description\": \"Y 坐标（毫米）\" }\n                    },\n                    \"required\": [\"x\", \"y\"]\n                }\n            },\n            \"level_id\": {\n                \"type\": \"integer\",\n                \"description\": \"楼板所在的标高 ID。单个创建时使用\"\n            },\n            \"floor_type_id\": {\n                \"type\": \"integer\",\n                \"description\": \"楼板类型 ID（必选）。可使用 get_family_types 工具获取可用的楼板类型 ID（单个创建时使用）\"\n            },\n            \"floors\": {\n                \"type\": \"array\",\n                \"description\": \"楼板数据列表（批量创建时使用）。如果提供此参数，将忽略单个创建参数。每个对象包含 points, level_id, floor_type_id\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"points\": {\n                            \"type\": \"array\",\n                            \"description\": \"轮廓点列表（必填，至少3个点）。例如：[{\\\"x\\\": 0, \\\"y\\\": 0}, {\\\"x\\\": 5000, \\\"y\\\": 0}, {\\\"x\\\": 5000, \\\"y\\\": 3000}]\",\n                            \"items\": {\n                                \"type\": \"object\",\n                                \"properties\": {\n                                    \"x\": { \"type\": \"number\", \"description\": \"X 坐标（毫米）\" },\n                                    \"y\": { \"type\": \"number\", \"description\": \"Y 坐标（毫米）\" }\n                                },\n                                \"required\": [\"x\", \"y\"]\n                            }\n                        },\n                        \"level_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"楼板所在的标高 ID\"\n                        },\n                        \"floor_type_id\": {\n                            \"type\": \"integer\",\n                            \"description\": \"楼板类型 ID（必选）\"\n                        }\n                    },\n                    \"required\": [\"points\", \"level_id\", \"floor_type_id\"]\n                }\n            }\n        }\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class404))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class404 stateMachine = new Class404();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createFloorByProfileTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class403))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class403 stateMachine = new Class403();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createFloorByProfileTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class402))]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0)
	{
		Class402 stateMachine = new Class402();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createFloorByProfileTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
