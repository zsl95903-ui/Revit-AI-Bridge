using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Revit.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using ns0;
using ns6;

namespace RevitAi.Revit.AI.Tools;

[AITool("create_stair", Category = "建模工具", Description = "创建参数化楼梯，支持自定义踏步宽度、楼梯高度、跑数配置、梯段宽度、梯井宽度、平台宽度、上楼方向等参数。注意：此工具使用 StairsEditScope API，会自己管理事务，不需要外层事务", RequiresTransaction = false, RequiresModification = true)]
public sealed class CreateStairTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class341
	{
		public Level level_0;

		internal bool method_0(Level level_1)
		{
			return level_1.Elevation > level_0.Elevation;
		}
	}

	[CompilerGenerated]
	public sealed class Class342 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateStairTool createStairTool_0;

		private double double_0;

		private int int_1;

		private double double_1;

		private double double_2;

		private double double_3;

		private double double_4;

		private string string_0;

		private string string_1;

		private double double_5;

		private double double_6;

		private double double_7;

		private StairDirection stairDirection_0;

		private XYZ xyz_0;

		private StairCreationParameters stairCreationParameters_0;

		private string string_2;

		private object object_0;

		private IStairCreationService istairCreationService_0;

		private Document document_0;

		private Level level_0;

		private Level level_1;

		private ElementId elementId_0;

		private Stairs stairs_0;

		private string string_3;

		private Exception exception_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Expected O, but got Unknown
			AIToolResult result;
			try
			{
				double_0 = aitoolContext_0.GetParameter<double>("totalHeightMm", 0.0);
				int_1 = aitoolContext_0.GetParameter<int>("runCount", 0);
				double_1 = aitoolContext_0.GetParameter<double>("treadDepthMm", 300.0);
				double_2 = aitoolContext_0.GetParameter<double>("runWidthMm", 1200.0);
				double_3 = aitoolContext_0.GetParameter<double>("wellWidthMm", 200.0);
				double_4 = aitoolContext_0.GetParameter<double>("landingWidthMm", 1200.0);
				string_0 = aitoolContext_0.GetParameter<string>("direction", "Left");
				string_1 = aitoolContext_0.GetParameter<string>("startDirection", "X+");
				double_5 = aitoolContext_0.GetParameter<double>("insertPointX", 0.0);
				double_6 = aitoolContext_0.GetParameter<double>("insertPointY", 0.0);
				double_7 = aitoolContext_0.GetParameter<double>("baseElevationM", 0.0);
				object obj3;
				if (double_0 <= 0.0)
				{
					result = AIToolResult.Fail("楼梯总高度必须大于0");
				}
				else if (int_1 < 1 || int_1 > 10)
				{
					result = AIToolResult.Fail("跑数必须在 1-10 之间");
				}
				else
				{
					if (double_1 < 260.0 || double_1 > 350.0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[AI工具] 踏步宽度 ");
						defaultInterpolatedStringHandler.AppendFormatted(double_1);
						defaultInterpolatedStringHandler.AppendLiteral("mm 超出推荐范围（260-350mm）");
						Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					stairDirection_0 = (string_0.Equals("Right", StringComparison.OrdinalIgnoreCase) ? StairDirection.Right : StairDirection.Left);
					xyz_0 = createStairTool_0.method_0(string_1);
					stairCreationParameters_0 = new StairCreationParameters
					{
						TotalHeightMm = double_0,
						TreadDepthMm = double_1,
						StepsPerRun = StairCreationParameters.AutoDistributeSteps(double_0, int_1),
						RunWidthMm = double_2,
						WellWidthMm = double_3,
						LandingWidthMm = double_4,
						Direction = stairDirection_0,
						StartDirection = xyz_0,
						BaseElevationM = double_7,
						InsertPoint = new XYZ(double_5 / 304.8, double_6 / 304.8, 0.0)
					};
					if (!stairCreationParameters_0.Validate(out string_2))
					{
						result = AIToolResult.Fail("参数验证失败：" + string_2);
					}
					else
					{
						Logger.Info("[AI工具] 开始创建楼梯: " + stairCreationParameters_0.GetSummary());
						object_0 = createStairTool_0.irevitAdapter_0.GetStairCreationService();
						istairCreationService_0 = object_0 as IStairCreationService;
						if (istairCreationService_0 == null)
						{
							result = AIToolResult.Fail("无法获取楼梯创建服务");
						}
						else
						{
							object document = aitoolContext_0.Document;
							document_0 = (Document)((document is Document) ? document : null);
							if (document_0 == null)
							{
								result = AIToolResult.Fail("无法获取 Revit 文档");
							}
							else
							{
								level_0 = createStairTool_0.method_1(document_0);
								if (level_0 == null)
								{
									result = AIToolResult.Fail("无法获取底部标高");
								}
								else
								{
									level_1 = createStairTool_0.method_2(document_0, level_0);
									IStairCreationService stairCreationService = istairCreationService_0;
									StairCreationParameters parameters = stairCreationParameters_0;
									ElementId id = ((Element)level_0).Id;
									Level obj = level_1;
									elementId_0 = stairCreationService.CreateStair(parameters, id, (obj != null) ? ((Element)obj).Id : null);
									if (!(elementId_0 == (ElementId)null) && !(elementId_0 == ElementId.InvalidElementId))
									{
										Element element = document_0.GetElement(elementId_0);
										stairs_0 = (Stairs)(object)((element is Stairs) ? element : null);
										Stairs obj2 = stairs_0;
										if (obj2 == null)
										{
											obj3 = null;
										}
										else
										{
											obj3 = ((Element)obj2).Name;
											if (obj3 != null)
											{
												goto IL_0531;
											}
										}
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
										defaultInterpolatedStringHandler2.AppendLiteral("楼梯_");
										defaultInterpolatedStringHandler2.AppendFormatted(elementId_0.Value);
										obj3 = defaultInterpolatedStringHandler2.ToStringAndClear();
										goto IL_0531;
									}
									result = AIToolResult.Fail("楼梯创建失败");
								}
							}
						}
					}
				}
				goto end_IL_0002;
				IL_0531:
				string_3 = (string)obj3;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(23, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("[AI工具] 楼梯创建成功: ID=");
				defaultInterpolatedStringHandler3.AppendFormatted(elementId_0.Value);
				defaultInterpolatedStringHandler3.AppendLiteral(", 名称=");
				defaultInterpolatedStringHandler3.AppendFormatted(string_3);
				Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
				result = AIToolResult.Ok("成功创建楼梯：" + string_3 + "，参数：" + stairCreationParameters_0.GetSummary(), (object)new Class82<long, string, Class83<double, int, double, double, int, int[]>>(elementId_0.Value, string_3, new Class83<double, int, double, double, int, int[]>(double_0, stairCreationParameters_0.TotalSteps, stairCreationParameters_0.RiserHeightMm, double_1, int_1, stairCreationParameters_0.StepsPerRun.ToArray())));
				end_IL_0002:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[AI工具] 创建楼梯失败", exception_0);
				result = AIToolResult.Fail("创建失败：" + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private readonly IRevitAdapter irevitAdapter_0;

	public string Name => "create_stair";

	public string Category => "建模工具";

	public string Description => "创建参数化楼梯";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"totalHeightMm\": {\n                \"type\": \"number\",\n                \"description\": \"楼梯总高度（毫米），例如 3000 表示 3米高的楼梯\"\n            },\n            \"runCount\": {\n                \"type\": \"number\",\n                \"description\": \"楼梯跑数，例如 2 表示两跑楼梯（折返楼梯），3 表示三跑楼梯（U型楼梯）\"\n            },\n            \"treadDepthMm\": {\n                \"type\": \"number\",\n                \"description\": \"踏步宽度（毫米），默认 300mm，推荐范围 260-350mm\"\n            },\n            \"runWidthMm\": {\n                \"type\": \"number\",\n                \"description\": \"梯段宽度（毫米），默认 1200mm\"\n            },\n            \"wellWidthMm\": {\n                \"type\": \"number\",\n                \"description\": \"梯井宽度（毫米），默认 200mm，设为 0 表示无梯井\"\n            },\n            \"landingWidthMm\": {\n                \"type\": \"number\",\n                \"description\": \"平台宽度（毫米），默认 1200mm\"\n            },\n            \"direction\": {\n                \"type\": \"string\",\n                \"description\": \"上楼方向，Left 表示左旋（逆时针），Right 表示右旋（顺时针）\",\n                \"enum\": [\"Left\", \"Right\"]\n            },\n            \"startDirection\": {\n                \"type\": \"string\",\n                \"description\": \"起始方向向量，第一跑的梯段方向，默认 X 正方向\",\n                \"enum\": [\"X+\", \"Y+\", \"X-\", \"Y-\"]\n            },\n            \"insertPointX\": {\n                \"type\": \"number\",\n                \"description\": \"插入点 X 坐标（毫米），默认为 0\"\n            },\n            \"insertPointY\": {\n                \"type\": \"number\",\n                \"description\": \"插入点 Y 坐标（毫米），默认为 0\"\n            },\n            \"baseElevationM\": {\n                \"type\": \"number\",\n                \"description\": \"楼梯底标高偏移（米），相对于底部标高，默认为 0\"\n            }\n        },\n        \"required\": [\"totalHeightMm\", \"runCount\"]\n    }";

	public CreateStairTool(IRevitAdapter revitAdapter)
	{
		irevitAdapter_0 = revitAdapter ?? throw new ArgumentNullException("revitAdapter");
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class342))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class342 stateMachine = new Class342();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createStairTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private XYZ method_0(string string_0)
	{
		if (!(string_0 == "Y+"))
		{
			if (!(string_0 == "X-"))
			{
				if (!(string_0 == "Y-"))
				{
					return XYZ.BasisX;
				}
				return -XYZ.BasisY;
			}
			return -XYZ.BasisX;
		}
		return XYZ.BasisY;
	}

	private Level? method_1(Document document_0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		List<Level> source = (from Level level_0 in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Level)).WhereElementIsNotElementType()
			orderby level_0.Elevation
			select level_0).ToList();
		return source.FirstOrDefault();
	}

	private Level? method_2(Document document_0, Level level_0)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (level_0 == null)
		{
			return null;
		}
		List<Level> source = (from Level val in (IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Level)).WhereElementIsNotElementType()
			orderby val.Elevation
			select val).ToList();
		return source.FirstOrDefault((Level val) => val.Elevation > level_0.Elevation) ?? source.Skip(1).FirstOrDefault();
	}
}
