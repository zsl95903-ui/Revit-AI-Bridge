using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit.WallToRoad;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.AI.Tools;

[AITool("create_road_from_walls", Category = "道路建模", Description = "从墙元素生成完整的道路网络，包括车道、人行道、路沿石、中心标线、人行横道、车道分界线、指向箭头和场地楼板。\n\n适用场景：\n- 已有墙元素作为道路中心线的道路设计\n- 需要创建包含完整道路设施的模型\n- 道路交叉口和转弯处自动圆角处理\n\n使用流程：\n1. 使用 element_query(operation='by_category', categoryName='墙') 获取墙元素 ID\n2. 使用 create_road_from_walls(wall_element_ids=[...]) 创建道路网络\n\n注意：距离参数使用米为单位，工具会自动处理单位转换和 Revit API 交互。", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateRoadFromWallsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class340 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateRoadFromWallsTool createRoadFromWallsTool_0;

		private int[] int_1;

		private WallToRoadRequest wallToRoadRequest_0;

		private IWallToRoadService iwallToRoadService_0;

		private Result<List<int>> result_0;

		private int int_2;

		private string string_0;

		private Exception exception_0;

		void IAsyncStateMachine.MoveNext()
		{
			AIToolResult result;
			try
			{
				int_1 = aitoolContext_0.GetParameter<int[]>("wall_element_ids", (int[])null);
				if (int_1 == null || int_1.Length == 0)
				{
					result = AIToolResult.Fail("墙元素 ID 列表不能为空，请使用 element_query 工具获取墙元素");
				}
				else
				{
					wallToRoadRequest_0 = createRoadFromWallsTool_0.method_0(aitoolContext_0, int_1);
					if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						wallToRoadRequest_0.Document = aitoolContext_0.Document;
						IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
						iwallToRoadService_0 = ((revitAdapter != null) ? revitAdapter.GetWallToRoadService() : null);
						if (iwallToRoadService_0 == null)
						{
							result = AIToolResult.Fail("无法获取道路建模服务");
						}
						else
						{
							if (aitoolContext_0.HasTransaction && aitoolContext_0.Transaction != null)
							{
								result_0 = iwallToRoadService_0.CreateRoadFromWallsWithTransaction(aitoolContext_0.Document, wallToRoadRequest_0, aitoolContext_0.Transaction);
							}
							else
							{
								result_0 = iwallToRoadService_0.CreateRoadFromWalls(aitoolContext_0.Document, wallToRoadRequest_0);
							}
							if (!result_0.IsSuccess || result_0.Value == null)
							{
								result = AIToolResult.Fail(result_0.Error ?? "创建道路网络失败");
							}
							else
							{
								int_2 = result_0.Value.Count;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
								defaultInterpolatedStringHandler.AppendLiteral("成功创建道路网络，共生成 ");
								defaultInterpolatedStringHandler.AppendFormatted(int_2);
								defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
								string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("[CreateRoadFromWallsTool] ");
								defaultInterpolatedStringHandler2.AppendFormatted(string_0);
								defaultInterpolatedStringHandler2.AppendLiteral("，墙数量: ");
								defaultInterpolatedStringHandler2.AppendFormatted(int_1.Length);
								Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
								result = AIToolResult.Ok(string_0, (object)new Class71<int, int, List<int>>(int_2, int_1.Length, result_0.Value));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[CreateRoadFromWallsTool] 工具执行失败", exception_0);
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

	private readonly IWallToRoadService iwallToRoadService_0;

	public string Name => "create_road_from_walls";

	public string Category => "道路建模";

	public string Description => "从墙元素生成完整的道路网络";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"wall_element_ids\": {\n                \"type\": \"array\",\n                \"description\": \"墙元素 ID 列表（必选）。使用 element_query 工具获取\",\n                \"items\": { \"type\": \"integer\" }\n            },\n            \"lane_width_meters\": {\n                \"type\": \"number\",\n                \"description\": \"车道宽度（米），默认 4.25 米\",\n                \"default\": 4.25\n            },\n            \"sidewalk_width_meters\": {\n                \"type\": \"number\",\n                \"description\": \"人行道宽度（米），默认 3.0 米\",\n                \"default\": 3.0\n            },\n            \"intersection_radius_meters\": {\n                \"type\": \"number\",\n                \"description\": \"路口倒角半径（米），默认 10 米\",\n                \"default\": 10.0\n            },\n            \"default_road_width_meters\": {\n                \"type\": \"number\",\n                \"description\": \"默认道路宽度（米），用于未设置宽度的墙，默认 9.0 米\",\n                \"default\": 9.0\n            },\n            \"sidewalk_options\": {\n                \"type\": \"object\",\n                \"description\": \"人行道详细参数（可选）\",\n                \"properties\": {\n                    \"create_sidewalks\": {\n                        \"type\": \"boolean\",\n                        \"description\": \"是否创建人行道\",\n                        \"default\": true\n                    },\n                    \"sidewalk_floor_offset_millimeters\": {\n                        \"type\": \"number\",\n                        \"description\": \"人行道楼板标高偏移（毫米）\",\n                        \"default\": 100.0\n                    }\n                }\n            },\n            \"curb_options\": {\n                \"type\": \"object\",\n                \"description\": \"路沿石详细参数（可选）\",\n                \"properties\": {\n                    \"create_curbs\": {\n                        \"type\": \"boolean\",\n                        \"description\": \"是否创建路沿石\",\n                        \"default\": true\n                    },\n                    \"curb_width_millimeters\": {\n                        \"type\": \"number\",\n                        \"description\": \"路沿石宽度（毫米）\",\n                        \"default\": 150.0\n                    },\n                    \"curb_height_millimeters\": {\n                        \"type\": \"number\",\n                        \"description\": \"路沿石墙高度（毫米）\",\n                        \"default\": 200.0\n                    },\n                    \"curb_base_offset_millimeters\": {\n                        \"type\": \"number\",\n                        \"description\": \"路沿石墙底标高偏移（毫米）\",\n                        \"default\": -100.0\n                    }\n                }\n            },\n            \"marking_options\": {\n                \"type\": \"object\",\n                \"description\": \"标线详细参数（可选）\",\n                \"properties\": {\n                    \"create_center_markings\": {\n                        \"type\": \"boolean\",\n                        \"description\": \"是否创建中心标线\",\n                        \"default\": true\n                    },\n                    \"center_marking_width_millimeters\": {\n                        \"type\": \"number\",\n                        \"description\": \"中心标线宽度（毫米）\",\n                        \"default\": 150.0\n                    },\n                    \"is_center_marking_double_line\": {\n                        \"type\": \"boolean\",\n                        \"description\": \"中心标线是否为双线\",\n                        \"default\": false\n                    },\n                    \"create_crosswalks\": {\n                        \"type\": \"boolean\",\n                        \"description\": \"是否创建人行横道\",\n                        \"default\": true\n                    },\n                    \"create_lane_markings\": {\n                        \"type\": \"boolean\",\n                        \"description\": \"是否创建车道分界线\",\n                        \"default\": true\n                    },\n                    \"create_direction_arrows\": {\n                        \"type\": \"boolean\",\n                        \"description\": \"是否创建指向箭头\",\n                        \"default\": true\n                    },\n                    \"arrow_spacing_meters\": {\n                        \"type\": \"number\",\n                        \"description\": \"箭头间距（米）\",\n                        \"default\": 20.0\n                    },\n                    \"arrow_size_meters\": {\n                        \"type\": \"number\",\n                        \"description\": \"箭头大小（米）：3, 6, 9\",\n                        \"default\": 6.0\n                    }\n                }\n            },\n            \"advanced_options\": {\n                \"type\": \"object\",\n                \"description\": \"高级参数（可选）\",\n                \"properties\": {\n                    \"delete_original_walls\": {\n                        \"type\": \"boolean\",\n                        \"description\": \"是否删除原始墙\",\n                        \"default\": false\n                    },\n                    \"create_site_floor\": {\n                        \"type\": \"boolean\",\n                        \"description\": \"是否创建场地楼板\",\n                        \"default\": true\n                    },\n                    \"site_floor_extension_meters\": {\n                        \"type\": \"number\",\n                        \"description\": \"场地外扩距离（米）\",\n                        \"default\": 20.0\n                    },\n                    \"fillet_radius_meters\": {\n                        \"type\": \"number\",\n                        \"description\": \"转弯圆角半径（米）\",\n                        \"default\": 10.0\n                    }\n                }\n            }\n        },\n        \"required\": [\"wall_element_ids\"]\n    }";

	public CreateRoadFromWallsTool(IWallToRoadService wallToRoadService)
	{
		iwallToRoadService_0 = wallToRoadService ?? throw new ArgumentNullException("wallToRoadService");
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class340))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class340 stateMachine = new Class340();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createRoadFromWallsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private WallToRoadRequest method_0(AIToolContext aitoolContext_0, int[] int_0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Expected O, but got Unknown
		return new WallToRoadRequest
		{
			WallElementIds = new List<int>(int_0),
			LaneWidthMeters = aitoolContext_0.GetParameter<double>("lane_width_meters", 4.25),
			SidewalkWidthMeters = aitoolContext_0.GetParameter<double>("sidewalk_width_meters", 3.0),
			IntersectionRadiusMeters = aitoolContext_0.GetParameter<double>("intersection_radius_meters", 10.0),
			DefaultRoadWidthMeters = aitoolContext_0.GetParameter<double>("default_road_width_meters", 9.0),
			CreateSidewalks = method_1(aitoolContext_0, "sidewalk_options", "create_sidewalks", gparam_0: true),
			SidewalkFloorOffsetMillimeters = method_1(aitoolContext_0, "sidewalk_options", "sidewalk_floor_offset_millimeters", 100.0),
			CreateCurbs = method_1(aitoolContext_0, "curb_options", "create_curbs", gparam_0: true),
			CurbWidthMillimeters = method_1(aitoolContext_0, "curb_options", "curb_width_millimeters", 150.0),
			CurbHeightMillimeters = method_1(aitoolContext_0, "curb_options", "curb_height_millimeters", 200.0),
			CurbBaseOffsetMillimeters = method_1(aitoolContext_0, "curb_options", "curb_base_offset_millimeters", -100.0),
			CreateCenterMarkings = method_1(aitoolContext_0, "marking_options", "create_center_markings", gparam_0: true),
			CenterMarkingWidthMillimeters = method_1(aitoolContext_0, "marking_options", "center_marking_width_millimeters", 150.0),
			IsCenterMarkingDoubleLine = method_1(aitoolContext_0, "marking_options", "is_center_marking_double_line", gparam_0: false),
			CenterMarkingThicknessMillimeters = 10.0,
			CreateCrosswalks = method_1(aitoolContext_0, "marking_options", "create_crosswalks", gparam_0: true),
			CrosswalkLineWidthMillimeters = 450.0,
			CrosswalkSpacingMillimeters = 1050.0,
			CrosswalkLengthMeters = 5.0,
			CrosswalkDistanceFromMarkingMeters = 1.0,
			CreateLaneMarkings = method_1(aitoolContext_0, "marking_options", "create_lane_markings", gparam_0: true),
			LaneMarkingWidthMillimeters = 150.0,
			LaneMarkingSolidLengthMeters = 4.0,
			LaneMarkingGapLengthMeters = 6.0,
			CreateDirectionArrows = method_1(aitoolContext_0, "marking_options", "create_direction_arrows", gparam_0: true),
			ArrowSpacingMeters = method_1(aitoolContext_0, "marking_options", "arrow_spacing_meters", 20.0),
			ArrowSizeMeters = method_1(aitoolContext_0, "marking_options", "arrow_size_meters", 6.0),
			DeleteOriginalWalls = method_1(aitoolContext_0, "advanced_options", "delete_original_walls", gparam_0: false),
			CreateSiteFloor = method_1(aitoolContext_0, "advanced_options", "create_site_floor", gparam_0: true),
			SiteFloorExtensionMeters = method_1(aitoolContext_0, "advanced_options", "site_floor_extension_meters", 20.0),
			FilletRadiusMeters = method_1(aitoolContext_0, "advanced_options", "fillet_radius_meters", 10.0)
		};
	}

	private T method_1<T>(AIToolContext aitoolContext_0, string string_0, string string_1, T gparam_0)
	{
		if (!aitoolContext_0.HasParameter(string_0))
		{
			return gparam_0;
		}
		try
		{
			object parameter = aitoolContext_0.GetParameter<object>(string_0, (object)null);
			if (parameter == null)
			{
				return gparam_0;
			}
			if (parameter is IDictionary dictionary && dictionary.Contains(string_1) && dictionary[string_1] != null)
			{
				object value = dictionary[string_1];
				return (T)((!(Convert.ChangeType(value, typeof(T)) is T val)) ? ((object)gparam_0) : ((object)val));
			}
			return gparam_0;
		}
		catch
		{
			return gparam_0;
		}
	}
}
