using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_all_rooms", Category = "房间与分区", Description = "获取文档中的所有房间及其信息", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetAllRoomsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class484 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetAllRoomsTool getAllRoomsTool_0;

		private bool bool_0;

		private bool bool_1;

		private bool bool_2;

		private IElementService ielementService_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private IEnumerator<object> ienumerator_0;

		private object object_0;

		private int? nullable_0;

		private string string_0;

		private object object_1;

		private object object_2;

		private Dictionary<string, object?> dictionary_0;

		private double? nullable_1;

		private double? nullable_2;

		private object object_3;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_0;

		private object object_4;

		private PropertyInfo propertyInfo_1;

		private string string_2;

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
					Class484 stateMachine = this;
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
				bool_0 = aitoolContext_0.GetParameter<bool>("includeArea", true);
				bool_1 = aitoolContext_0.GetParameter<bool>("includeVolume", false);
				bool_2 = aitoolContext_0.GetParameter<bool>("includePerimeter", false);
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
					ienumerable_0 = ielementService_0.GetElementsByCategory(aitoolContext_0.Document, "房间");
					if (ienumerable_0 == null || !ienumerable_0.Any())
					{
						result = AIToolResult.Ok("文档中没有找到任何房间", (object)new Class147<int, object[]>(0, Array.Empty<object>()));
					}
					else
					{
						list_0 = new List<object>();
						ienumerator_0 = ienumerable_0.GetEnumerator();
						try
						{
							for (; ienumerator_0.MoveNext(); list_0.Add(dictionary_0), string_0 = null, object_1 = null, object_2 = null, dictionary_0 = null, object_0 = null)
							{
								object_0 = ienumerator_0.Current;
								nullable_0 = ielementService_0.GetElementId(object_0);
								string_0 = ielementService_0.GetElementName(object_0);
								object_1 = ielementService_0.GetParameterValue(object_0, "编号");
								if (object_1 == null)
								{
									object_1 = ielementService_0.GetParameterValue(object_0, "Number");
								}
								object_2 = ielementService_0.GetParameterValue(object_0, "标高");
								if (object_2 == null)
								{
									object_2 = ielementService_0.GetParameterValue(object_0, "Level");
								}
								Dictionary<string, object> obj = new Dictionary<string, object>
								{
									["roomId"] = nullable_0,
									["name"] = string_0 ?? "未命名"
								};
								string key = "number";
								object obj2 = object_1;
								object obj3;
								if (obj2 == null)
								{
									obj3 = null;
								}
								else
								{
									obj3 = obj2.ToString();
									if (obj3 != null)
									{
										goto IL_02e9;
									}
								}
								obj3 = "";
								goto IL_02e9;
								IL_03fa:
								object dictionary;
								object key2;
								object obj4;
								((Dictionary<string, object>)dictionary)[(string)key2] = obj4;
								object_3 = null;
								continue;
								IL_02e9:
								obj[key] = obj3;
								string key3 = "level";
								object obj5 = object_2;
								object obj6;
								if (obj5 == null)
								{
									obj6 = null;
								}
								else
								{
									obj6 = obj5.ToString();
									if (obj6 != null)
									{
										goto IL_0319;
									}
								}
								obj6 = "";
								goto IL_0319;
								IL_0319:
								obj[key3] = obj6;
								dictionary_0 = obj;
								if (bool_0)
								{
									nullable_1 = ielementService_0.GetElementArea(object_0);
									dictionary_0["area"] = nullable_1;
								}
								if (bool_1)
								{
									nullable_2 = ielementService_0.GetElementVolume(object_0);
									dictionary_0["volume"] = nullable_2;
								}
								if (!bool_2)
								{
									continue;
								}
								object_3 = ielementService_0.GetParameterValue(object_0, "Perimeter");
								dictionary = dictionary_0;
								key2 = "perimeter";
								object obj7 = object_3;
								if (obj7 == null)
								{
									obj4 = null;
								}
								else
								{
									obj4 = obj7.ToString();
									if (obj4 != null)
									{
										goto IL_03fa;
									}
								}
								obj4 = "";
								goto IL_03fa;
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
						if (!string.IsNullOrEmpty(aitoolContext_0.SessionId) && aitoolContext_0.DataCache != null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
							defaultInterpolatedStringHandler.AppendLiteral("all_rooms_");
							defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
							string_1 = defaultInterpolatedStringHandler.ToStringAndClear();
							aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_1, "个房间", 200);
							if (aitoolResult_0.Data != null)
							{
								try
								{
									propertyInfo_0 = aitoolResult_0.Data.GetType().GetProperty("cache_info");
									if (propertyInfo_0 != null)
									{
										object_4 = propertyInfo_0.GetValue(aitoolResult_0.Data);
										if (object_4 != null)
										{
											propertyInfo_1 = object_4.GetType().GetProperty("cache_id");
											if (propertyInfo_1 != null)
											{
												string_2 = propertyInfo_1.GetValue(object_4)?.ToString();
												if (!string.IsNullOrEmpty(string_2))
												{
													AIToolResult obj8 = aitoolResult_0;
													DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(45, 2);
													defaultInterpolatedStringHandler2.AppendLiteral("✅ 找到 ");
													defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
													defaultInterpolatedStringHandler2.AppendLiteral(" 个房间\n\n💡 在后续工具调用中使用 cacheId=\"");
													defaultInterpolatedStringHandler2.AppendFormatted(string_2);
													defaultInterpolatedStringHandler2.AppendLiteral("\" 参数来操作这些元素");
													obj8.Message = defaultInterpolatedStringHandler2.ToStringAndClear();
												}
												string_2 = null;
											}
											propertyInfo_1 = null;
										}
										object_4 = null;
									}
									propertyInfo_0 = null;
								}
								catch
								{
								}
							}
							result = aitoolResult_0;
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("成功获取 ");
							defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
							defaultInterpolatedStringHandler3.AppendLiteral(" 个房间");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class148<int, List<object>, bool, bool, bool>(list_0.Count, list_0, bool_0, bool_1, bool_2));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取房间失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_all_rooms";

	public string Category => "房间与分区";

	public string Description => "获取所有房间";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"includeArea\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否包含房间面积（默认 true）\",\r\n                \"default\": true\r\n            },\r\n            \"includeVolume\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否包含房间体积（默认 false）\",\r\n                \"default\": false\r\n            },\r\n            \"includePerimeter\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否包含房间周长（默认 false）\",\r\n                \"default\": false\r\n            }\r\n        },\r\n        \"required\": []\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class484))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class484 stateMachine = new Class484();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getAllRoomsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
