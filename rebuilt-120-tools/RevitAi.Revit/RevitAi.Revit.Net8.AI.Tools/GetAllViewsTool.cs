using System;
using System.Collections.Generic;
using System.Diagnostics;
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

[AITool("get_all_views", Category = "视图管理", Description = "获取文档中所有视图的列表，包括平面图、立面图、剖面图等", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetAllViewsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class487 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetAllViewsTool getAllViewsTool_0;

		private string string_0;

		private IViewService iviewService_0;

		private Dictionary<string, string> dictionary_0;

		private Dictionary<string, string> dictionary_1;

		private string string_1;

		private string string_2;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private IEnumerator<object> ienumerator_0;

		private object object_0;

		private int? nullable_0;

		private string string_3;

		private string string_4;

		private string string_5;

		private string string_6;

		private string string_7;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_0;

		private object object_1;

		private PropertyInfo propertyInfo_1;

		private string string_8;

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
					Class487 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("viewType", (string)null);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
				if (iviewService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					dictionary_0 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
					{
						{
							"平面图",
							"FloorPlan"
						},
						{
							"天花板平面图",
							"CeilingPlan"
						},
						{
							"立面图",
							"Elevation"
						},
						{
							"剖面图",
							"Section"
						},
						{
							"详图视图",
							"Detail"
						},
						{
							"绘图视图",
							"DraftingView"
						},
						{
							"三维视图",
							"ThreeD"
						},
						{
							"图例",
							"Legend"
						},
						{
							"面积平面图",
							"AreaPlan"
						},
						{
							"工程平面图",
							"EngineeringPlan"
						}
					};
					dictionary_1 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
					{
						{
							"FloorPlan",
							"平面图"
						},
						{
							"CeilingPlan",
							"天花板平面图"
						},
						{
							"Elevation",
							"立面图"
						},
						{
							"Section",
							"剖面图"
						},
						{
							"Detail",
							"详图视图"
						},
						{
							"DraftingView",
							"绘图视图"
						},
						{
							"ThreeD",
							"三维视图"
						},
						{
							"Legend",
							"图例"
						},
						{
							"AreaPlan",
							"面积平面图"
						},
						{
							"EngineeringPlan",
							"工程平面图"
						}
					};
					string_1 = null;
					if (!string.IsNullOrEmpty(string_0) && dictionary_0.TryGetValue(string_0, out string_2))
					{
						string_1 = string_2;
					}
					ienumerable_0 = iviewService_0.GetAllViews(aitoolContext_0.Document);
					list_0 = new List<object>();
					ienumerator_0 = ienumerable_0.GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							object_0 = ienumerator_0.Current;
							if (iviewService_0.IsSystemView(object_0) || iviewService_0.IsTemplate(object_0) || !iviewService_0.CanPrint(object_0))
							{
								continue;
							}
							nullable_0 = iviewService_0.GetViewId(object_0);
							string_3 = iviewService_0.GetViewName(object_0);
							string_4 = iviewService_0.GetViewType(object_0);
							if (string.IsNullOrEmpty(string_1) || string_4 == null || !(string_4 != string_1))
							{
								string_5 = null;
								if (string_4 != null && dictionary_1.TryGetValue(string_4, out string_6))
								{
									string_5 = string_6;
								}
								list_0.Add(new Class152<int?, string, string>(nullable_0, string_3 ?? "未命名视图", string_5 ?? string_4 ?? "未知类型"));
								string_3 = null;
								string_4 = null;
								string_5 = null;
								string_6 = null;
								object_0 = null;
							}
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
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 2);
						defaultInterpolatedStringHandler.AppendLiteral("all_views_");
						defaultInterpolatedStringHandler.AppendFormatted(string_0 ?? "all");
						defaultInterpolatedStringHandler.AppendLiteral("_");
						defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
						string_7 = defaultInterpolatedStringHandler.ToStringAndClear();
						aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_7, "个视图", 200);
						if (aitoolResult_0.Data != null)
						{
							try
							{
								propertyInfo_0 = aitoolResult_0.Data.GetType().GetProperty("cache_info");
								if (propertyInfo_0 != null)
								{
									object_1 = propertyInfo_0.GetValue(aitoolResult_0.Data);
									if (object_1 != null)
									{
										propertyInfo_1 = object_1.GetType().GetProperty("cache_id");
										if (propertyInfo_1 != null)
										{
											string_8 = propertyInfo_1.GetValue(object_1)?.ToString();
											if (!string.IsNullOrEmpty(string_8))
											{
												AIToolResult obj = aitoolResult_0;
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(45, 2);
												defaultInterpolatedStringHandler2.AppendLiteral("✅ 找到 ");
												defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
												defaultInterpolatedStringHandler2.AppendLiteral(" 个视图\n\n💡 在后续工具调用中使用 cacheId=\"");
												defaultInterpolatedStringHandler2.AppendFormatted(string_8);
												defaultInterpolatedStringHandler2.AppendLiteral("\" 参数来操作这些元素");
												obj.Message = defaultInterpolatedStringHandler2.ToStringAndClear();
											}
											string_8 = null;
										}
										propertyInfo_1 = null;
									}
									object_1 = null;
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
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(7, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("找到 ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个视图");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class153<int, List<object>>(list_0.Count, list_0));
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取视图列表失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_all_views";

	public string Category => "视图管理";

	public string Description => "获取文档中所有视图的列表";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"viewType\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"可选的视图类型过滤（平面图、天花板平面图、立面图、剖面图、详图视图、绘图视图、三维视图、图例、面积平面图、工程平面图等），不传则返回所有视图\",\r\n                \"enum\": [\"平面图\", \"天花板平面图\", \"立面图\", \"剖面图\", \"详图视图\", \"绘图视图\", \"三维视图\", \"图例\", \"面积平面图\", \"工程平面图\"]\r\n            }\r\n        },\r\n        \"required\": []\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class487))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class487 stateMachine = new Class487();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getAllViewsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
