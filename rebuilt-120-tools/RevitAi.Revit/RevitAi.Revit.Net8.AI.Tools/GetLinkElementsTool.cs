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

[AITool("get_link_elements", Category = "链接管理", Description = "获取指定 Revit 链接模型中的元素。支持按类别名称过滤（如墙、楼板等）。返回元素列表并提供缓存 ID，可直接用于碰撞检查等后续操作。", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetLinkElementsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class501 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetLinkElementsTool getLinkElementsTool_0;

		private ILinkService ilinkService_0;

		private IElementService ielementService_0;

		private int int_1;

		private object object_0;

		private object object_1;

		private List<string> list_0;

		private List<object> list_1;

		private List<object> list_2;

		private string string_0;

		private string string_1;

		private string string_2;

		private string string_3;

		private string string_4;

		private object object_2;

		private List<string> list_3;

		private string[] string_5;

		private IEnumerable<object> ienumerable_0;

		private List<string>.Enumerator enumerator_0;

		private string string_6;

		private IEnumerable<object> ienumerable_1;

		private string string_7;

		private List<object>.Enumerator enumerator_1;

		private object object_3;

		private int? nullable_0;

		private string string_8;

		private string string_9;

		private string string_10;

		private Exception exception_0;

		private Exception exception_1;

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
					Class501 stateMachine = this;
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
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ilinkService_0 = ((revitAdapter != null) ? revitAdapter.LinkService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				object obj;
				if (ilinkService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 LinkService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					int_1 = aitoolContext_0.GetParameter<int>("linkInstanceId", 0);
					if (int_1 <= 0)
					{
						result = AIToolResult.Fail("链接实例 ID 无效");
					}
					else
					{
						object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
						if (object_0 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
							defaultInterpolatedStringHandler.AppendLiteral("找不到链接实例 ID ");
							defaultInterpolatedStringHandler.AppendFormatted(int_1);
							result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							object_1 = ilinkService_0.GetLinkDocument(object_0);
							if (object_1 == null)
							{
								result = AIToolResult.Fail("无法获取链接文档。请检查链接是否已加载。");
							}
							else
							{
								list_0 = new List<string>();
								if (aitoolContext_0.HasParameter("categoryName"))
								{
									string_4 = aitoolContext_0.GetParameter<string>("categoryName", (string)null);
									if (!string.IsNullOrEmpty(string_4))
									{
										list_0.Add(string_4);
									}
									string_4 = null;
								}
								else if (aitoolContext_0.HasParameter("categoryNames"))
								{
									object_2 = aitoolContext_0.GetParameter<object>("categoryNames", (object)null);
									list_3 = object_2 as List<string>;
									if (list_3 != null)
									{
										list_0.AddRange(list_3.Where((string string_0) => !string.IsNullOrEmpty(string_0)));
									}
									else
									{
										string_5 = object_2 as string[];
										if (string_5 != null)
										{
											list_0.AddRange(string_5.Where((string string_0) => !string.IsNullOrEmpty(string_0)));
										}
										string_5 = null;
									}
									object_2 = null;
									list_3 = null;
								}
								list_1 = new List<object>();
								if (list_0.Count == 0)
								{
									Logger.Info("[GetLinkElements] 获取链接模型中的所有元素");
									ienumerable_0 = ielementService_0.GetAllElements(object_1);
									list_1 = ienumerable_0.ToList();
									ienumerable_0 = null;
								}
								else
								{
									enumerator_0 = list_0.GetEnumerator();
									try
									{
										while (enumerator_0.MoveNext())
										{
											string_6 = enumerator_0.Current;
											Logger.Info("[GetLinkElements] 获取链接模型中的类别 '" + string_6 + "' 元素");
											ienumerable_1 = ielementService_0.GetElementsByCategory(object_1, string_6);
											list_1.AddRange(ienumerable_1);
											ienumerable_1 = null;
											string_6 = null;
										}
									}
									finally
									{
										if (num < 0)
										{
											((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
										}
									}
									enumerator_0 = default(List<string>.Enumerator);
								}
								if (list_1.Count != 0)
								{
									list_2 = new List<object>();
									enumerator_1 = list_1.GetEnumerator();
									try
									{
										while (enumerator_1.MoveNext())
										{
											object_3 = enumerator_1.Current;
											try
											{
												nullable_0 = ielementService_0.GetElementId(object_3);
												if (!nullable_0.HasValue)
												{
													continue;
												}
												string_8 = ielementService_0.GetElementName(object_3);
												string_9 = ielementService_0.GetElementCategory(object_3);
												string_10 = ielementService_0.GetElementTypeName(object_3);
												list_2.Add(new Class182<int, string, string, string>(nullable_0.Value, string_8 ?? "", string_9 ?? "", string_10 ?? ""));
												string_8 = null;
												string_9 = null;
												string_10 = null;
												goto IL_0644;
											}
											catch (Exception ex)
											{
												exception_0 = ex;
												Logger.Warning("[GetLinkElements] 处理元素时出错: " + exception_0.Message);
												goto IL_0644;
											}
											IL_0644:
											object_3 = null;
										}
									}
									finally
									{
										if (num < 0)
										{
											((IDisposable)enumerator_1/*cast due to constrained. prefix*/).Dispose();
										}
									}
									enumerator_1 = default(List<object>.Enumerator);
									string_0 = aitoolContext_0.SessionId ?? Guid.NewGuid().ToString("N");
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 2);
									defaultInterpolatedStringHandler2.AppendLiteral("link_elements_");
									defaultInterpolatedStringHandler2.AppendFormatted(string_0);
									defaultInterpolatedStringHandler2.AppendLiteral("_");
									defaultInterpolatedStringHandler2.AppendFormatted(Guid.NewGuid(), "N");
									string_1 = defaultInterpolatedStringHandler2.ToStringAndClear();
									IAIToolDataCache dataCache = aitoolContext_0.DataCache;
									if (dataCache == null)
									{
										obj = null;
									}
									else
									{
										obj = dataCache.Store<List<object>>(string_0, string_1, list_2);
										if (obj != null)
										{
											goto IL_073b;
										}
									}
									obj = string.Empty;
									goto IL_073b;
								}
								string_7 = ((list_0.Count == 0) ? "链接模型中没有元素" : ("链接模型中没有类别 '" + string.Join("、", list_0) + "' 的元素"));
								result = AIToolResult.Ok(string_7, (object)new Class181<int, string, List<string>, int, object[]>(int_1, (list_0.Count == 1) ? list_0[0] : null, (list_0.Count > 1) ? list_0 : null, 0, Array.Empty<object>()));
							}
						}
					}
				}
				goto end_IL_0067;
				IL_073b:
				string_2 = (string)obj;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("成功获取链接模型中的 ");
				defaultInterpolatedStringHandler3.AppendFormatted(list_2.Count);
				defaultInterpolatedStringHandler3.AppendLiteral(" 个元素");
				string_3 = defaultInterpolatedStringHandler3.ToStringAndClear() + ((list_0.Count > 0) ? ("（类别：" + string.Join("、", list_0) + "）") : "");
				if (string.IsNullOrEmpty(string_2))
				{
					Logger.Warning("[GetLinkElements] 缓存服务不可用，直接返回结果");
					result = AIToolResult.Ok(string_3, (object)new Class181<int, string, List<string>, int, List<object>>(int_1, (list_0.Count == 1) ? list_0[0] : null, (list_0.Count > 1) ? list_0 : null, list_2.Count, list_2.Take(100).ToList()));
				}
				else
				{
					Logger.Info("[GetLinkElements] " + string_3 + "，已缓存: " + string_2);
					result = AIToolResult.Ok(string_3 + "。已缓存结果，可直接用于碰撞检查等后续操作。\n\n💡 使用 cache_id=" + string_2 + " 在后续工具中引用这些元素", (object)new Class183<int, string, List<string>, int, List<object>, bool, string, Class184<string, string, int, int, int>>(int_1, (list_0.Count == 1) ? list_0[0] : null, (list_0.Count > 1) ? list_0 : null, list_2.Count, list_2.Take(100).ToList(), list_2.Count > 100, string_2, new Class184<string, string, int, int, int>(string_2, "链接模型元素已缓存，可直接用于后续工具", list_2.Count, Math.Min(100, list_2.Count), Math.Max(0, list_2.Count - 100))));
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_1 = ex;
				Logger.Error("[GetLinkElements] 获取链接元素失败: " + exception_1.Message);
				result = AIToolResult.Fail("获取链接元素失败: " + exception_1.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_link_elements";

	public string Category => "链接管理";

	public string Description => "获取指定 Revit 链接模型中的元素。支持按类别过滤。";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"linkInstanceId\": {\n                \"type\": \"integer\",\n                \"description\": \"链接实例 ID（必需）。从 get_links 工具的返回结果中的 instanceId 字段获取\"\n            },\n            \"categoryName\": {\n                \"type\": \"string\",\n                \"description\": \"类别名称（可选，中文）。例如：墙、楼板、结构柱等。不提供则获取链接模型中的所有元素\"\n            },\n            \"categoryNames\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"类别名称数组（可选，中文）。可同时获取多个类别的元素，如 [墙, 楼板]\"\n            }\n        },\n        \"required\": [\"linkInstanceId\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class501))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class501 stateMachine = new Class501();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getLinkElementsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
