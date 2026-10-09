using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit.FloorTopography;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.AI.Tools;

[AITool("create_topography_from_floors", Category = "地形建模", Description = "根据楼板轮廓从地形中裁剪出新地形子面域。\n\n适用场景：\n- 已有楼板轮廓作为地形划分边界\n- 需要将地形按楼板范围进行划分\n- 每个楼板轮廓创建独立的地形子面域\n\n使用流程：\n1. 使用 element_query(operation='by_category', categoryName='地形表面') 获取地形元素 ID\n2. 使用 element_query(operation='by_category', categoryName='楼板') 获取楼板元素 ID（可选：单个、数组或缓存ID）\n3. 使用 create_topography_from_floors 创建地形子面域\n\n输入方式（三选一）：\n- floor_element_id: 单个楼板元素 ID\n- floor_element_ids: 楼板元素 ID 数组\n- floor_cache_id: 楼板缓存 ID（从 element_query 返回的 cache_id）\n\n安全机制：\n- 当楼板数量超过 100 个时，需要添加 confirmed=true 参数确认执行\n- 这是为了避免误操作导致处理大量楼板\n\n版本支持：\n- 支持 Revit 2018-2024 版本（使用 TopographySurface + SiteSubRegion API）\n- 暂不支持 Revit 2025+ 版本（地形 API 已变更为 Toposolid，待实现）\n\n注意：工具会自动处理楼板轮廓的嵌套关系，只创建外轮廓的子面域。", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateTopographyFromFloorsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class344 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateTopographyFromFloorsTool createTopographyFromFloorsTool_0;

		private List<int> list_0;

		private int int_1;

		private FloorTopographyRequest floorTopographyRequest_0;

		private ITopographyService itopographyService_0;

		private Result<FloorTopographyResult> result_0;

		private FloorTopographyResult floorTopographyResult_0;

		private string string_0;

		private string string_1;

		private object object_0;

		private IEnumerable ienumerable_0;

		private IEnumerator ienumerator_0;

		private object object_1;

		private PropertyInfo propertyInfo_0;

		private int int_2;

		private int int_3;

		private object object_2;

		private int int_4;

		private long long_0;

		private int[] int_5;

		private long[] long_1;

		private List<int> list_1;

		private List<long> list_2;

		private bool bool_0;

		private Exception exception_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Expected O, but got Unknown
			int num = int_0;
			AIToolResult result;
			try
			{
				list_0 = new List<int>();
				if (!aitoolContext_0.HasParameter("floor_cache_id"))
				{
					goto IL_01d9;
				}
				string_1 = aitoolContext_0.GetParameter<string>("floor_cache_id", (string)null);
				if (string.IsNullOrEmpty(string_1))
				{
					goto IL_01d2;
				}
				object_0 = aitoolContext_0.GetCachedData<object>(string_1);
				if (object_0 != null)
				{
					ienumerable_0 = object_0 as IEnumerable;
					if (ienumerable_0 != null)
					{
						ienumerator_0 = ienumerable_0.GetEnumerator();
						try
						{
							while (ienumerator_0.MoveNext())
							{
								object_1 = ienumerator_0.Current;
								if (object_1 != null)
								{
									propertyInfo_0 = object_1.GetType().GetProperty("id");
									if (propertyInfo_0 != null)
									{
										object value = propertyInfo_0.GetValue(object_1);
										if (value is int)
										{
											int_2 = (int)value;
											if (true)
											{
												list_0.Add(int_2);
											}
										}
									}
									propertyInfo_0 = null;
								}
								object_1 = null;
							}
						}
						finally
						{
							if (num < 0 && ienumerator_0 is IDisposable disposable)
							{
								disposable.Dispose();
							}
						}
						ienumerator_0 = null;
					}
					list_0 = list_0.Distinct().ToList();
					ienumerable_0 = null;
					object_0 = null;
					goto IL_01d2;
				}
				result = AIToolResult.Fail("缓存 ID '" + string_1 + "' 无效或已过期，请重新查询楼板元素");
				goto end_IL_0008;
				IL_041e:
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("未找到有效的楼板元素 ID。请提供以下参数之一：floor_element_id（单个）、floor_element_ids（数组）或 floor_cache_id（缓存ID）");
				}
				else
				{
					if (list_0.Count <= 100)
					{
						goto IL_050d;
					}
					bool_0 = aitoolContext_0.GetParameter<bool>("confirmed", false);
					if (bool_0)
					{
						goto IL_050d;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(74, 2);
					defaultInterpolatedStringHandler.AppendLiteral("检测到 ");
					defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个楼板元素，超过 ");
					defaultInterpolatedStringHandler.AppendFormatted(100);
					defaultInterpolatedStringHandler.AppendLiteral(" 个阈值。\n");
					defaultInterpolatedStringHandler.AppendLiteral("为了避免误操作，处理大量楼板需要您明确确认。\n");
					defaultInterpolatedStringHandler.AppendLiteral("如果您确定要继续，请在参数中添加 confirmed=true");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				goto end_IL_0008;
				IL_050d:
				int_1 = aitoolContext_0.GetParameter<int>("topography_element_id", 0);
				if (int_1 <= 0)
				{
					result = AIToolResult.Fail("地形元素 ID 无效，请使用 element_query(operation='by_category', categoryName='地形表面') 获取地形元素");
				}
				else
				{
					floorTopographyRequest_0 = new FloorTopographyRequest
					{
						FloorElementIds = list_0,
						TopographyElementId = int_1
					};
					if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						floorTopographyRequest_0.Document = aitoolContext_0.Document;
						itopographyService_0 = createTopographyFromFloorsTool_0.itopographyService_0;
						if (itopographyService_0 == null)
						{
							result = AIToolResult.Fail("无法获取地形服务");
						}
						else
						{
							if (aitoolContext_0.HasTransaction && aitoolContext_0.Transaction != null)
							{
								result_0 = itopographyService_0.CreateTopographiesFromFloorProfilesWithTransaction(aitoolContext_0.Document, floorTopographyRequest_0, aitoolContext_0.Transaction);
							}
							else
							{
								result_0 = itopographyService_0.CreateTopographiesFromFloorProfiles(aitoolContext_0.Document, floorTopographyRequest_0);
							}
							if (!result_0.IsSuccess || result_0.Value == null)
							{
								result = AIToolResult.Fail(result_0.Error ?? "创建地形子面域失败");
							}
							else
							{
								floorTopographyResult_0 = result_0.Value;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("成功创建地形子面域，共生成 ");
								defaultInterpolatedStringHandler2.AppendFormatted(floorTopographyResult_0.SuccessCount);
								defaultInterpolatedStringHandler2.AppendLiteral(" 个元素");
								string_0 = defaultInterpolatedStringHandler2.ToStringAndClear();
								if (floorTopographyResult_0.Errors.Count > 0)
								{
									string text = string_0;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
									defaultInterpolatedStringHandler3.AppendLiteral("，");
									defaultInterpolatedStringHandler3.AppendFormatted(floorTopographyResult_0.Errors.Count);
									defaultInterpolatedStringHandler3.AppendLiteral(" 个轮廓创建失败");
									string_0 = text + defaultInterpolatedStringHandler3.ToStringAndClear();
								}
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(40, 2);
								defaultInterpolatedStringHandler4.AppendLiteral("[CreateTopographyFromFloorsTool] ");
								defaultInterpolatedStringHandler4.AppendFormatted(string_0);
								defaultInterpolatedStringHandler4.AppendLiteral("，楼板数量: ");
								defaultInterpolatedStringHandler4.AppendFormatted(list_0.Count);
								Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
								result = AIToolResult.Ok(string_0, (object)new Class94<int, int, int, List<int>, List<string>>(floorTopographyResult_0.SuccessCount, list_0.Count, floorTopographyResult_0.Errors.Count, floorTopographyResult_0.CreatedSubRegionIds, (floorTopographyResult_0.Errors.Count > 0) ? floorTopographyResult_0.Errors : null));
							}
						}
					}
				}
				goto end_IL_0008;
				IL_01d9:
				if (list_0.Count == 0 && aitoolContext_0.HasParameter("floor_element_id"))
				{
					int_3 = aitoolContext_0.GetParameter<int>("floor_element_id", 0);
					if (int_3 > 0)
					{
						list_0.Add(int_3);
					}
				}
				if (list_0.Count == 0 && aitoolContext_0.HasParameter("floor_element_ids"))
				{
					object_2 = aitoolContext_0.GetParameter<object>("floor_element_ids", (object)null);
					if (object_2 is int)
					{
						int_4 = (int)object_2;
						if (true)
						{
							list_0.Add(int_4);
							goto IL_0417;
						}
					}
					if (object_2 is long)
					{
						long_0 = (long)object_2;
						if (true)
						{
							list_0.Add((int)long_0);
							goto IL_0417;
						}
					}
					int_5 = object_2 as int[];
					if (int_5 != null)
					{
						list_0.AddRange(int_5);
					}
					else
					{
						long_1 = object_2 as long[];
						if (long_1 != null)
						{
							list_0.AddRange(Array.ConvertAll(long_1, (long long_0) => (int)long_0));
						}
						else
						{
							list_1 = object_2 as List<int>;
							if (list_1 != null)
							{
								list_0.AddRange(list_1);
							}
							else
							{
								list_2 = object_2 as List<long>;
								if (list_2 != null)
								{
									list_0.AddRange(list_2.ConvertAll((long long_0) => (int)long_0));
								}
								list_2 = null;
							}
							list_1 = null;
						}
						long_1 = null;
					}
					int_5 = null;
					goto IL_0417;
				}
				goto IL_041e;
				IL_01d2:
				string_1 = null;
				goto IL_01d9;
				IL_0417:
				object_2 = null;
				goto IL_041e;
				end_IL_0008:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[CreateTopographyFromFloorsTool] 工具执行失败", exception_0);
				result = AIToolResult.Fail("执行失败：" + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private readonly ITopographyService itopographyService_0;

	public string Name => "create_topography_from_floors";

	public string Category => "地形建模";

	public string Description => "根据楼板轮廓从地形中裁剪出新地形";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"floor_element_id\": {\n                \"type\": \"integer\",\n                \"description\": \"单个楼板元素 ID（可选，三选一）。如果要处理单个楼板，使用此参数。\"\n            },\n            \"floor_element_ids\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"楼板元素 ID 数组（可选，三选一）。如果要批量处理多个楼板，使用此参数。\"\n            },\n            \"floor_cache_id\": {\n                \"type\": \"string\",\n                \"description\": \"楼板缓存 ID（可选，三选一）。如果要处理之前查询缓存的楼板元素，使用此参数。工具会从缓存中提取所有楼板的 ID 并处理。\"\n            },\n            \"topography_element_id\": {\n                \"type\": \"integer\",\n                \"description\": \"地形元素 ID（必选）。使用 element_query(operation='by_category', categoryName='地形表面') 获取\"\n            },\n            \"confirmed\": {\n                \"type\": \"boolean\",\n                \"description\": \"确认处理大量楼板（可选）。当楼板数量超过 100 个时，必须设置此参数为 true 才能继续执行。这是为了避免误操作导致处理大量楼板。\",\n                \"default\": false\n            }\n        },\n        \"required\": [\"topography_element_id\"]\n    }";

	public CreateTopographyFromFloorsTool(ITopographyService topographyService)
	{
		itopographyService_0 = topographyService ?? throw new ArgumentNullException("topographyService");
	}

	[AsyncStateMachine(typeof(Class344))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class344 stateMachine = new Class344();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createTopographyFromFloorsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
