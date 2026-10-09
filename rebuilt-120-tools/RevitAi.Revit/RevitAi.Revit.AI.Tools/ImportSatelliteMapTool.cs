using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Services;
using RevitAi.Revit.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.AI.Tools;

[AITool("import_satellite_map", Category = "场地地形", Description = "根据位置名称或坐标自动导入卫星图到 Revit 项目。location_name 与 latitude+longitude 二选一。默认天地图卫星源，zoom_level 默认18，radius_km 默认0.25。", RequiresTransaction = true, RequiresModification = true)]
public sealed class ImportSatelliteMapTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class347 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ImportSatelliteMapTool importSatelliteMapTool_0;

		private SatelliteMapImportRequest satelliteMapImportRequest_0;

		private SatelliteMapImportResult satelliteMapImportResult_0;

		private string string_0;

		private string string_1;

		private Exception exception_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			AIToolResult result;
			try
			{
				satelliteMapImportRequest_0 = new SatelliteMapImportRequest
				{
					Document = aitoolContext_0.Document
				};
				if (aitoolContext_0.HasParameter("location_name"))
				{
					satelliteMapImportRequest_0.LocationName = aitoolContext_0.GetParameter<string>("location_name", (string)null);
				}
				if (aitoolContext_0.HasParameter("latitude"))
				{
					satelliteMapImportRequest_0.Latitude = aitoolContext_0.GetParameter<double>("latitude", 0.0);
				}
				if (aitoolContext_0.HasParameter("longitude"))
				{
					satelliteMapImportRequest_0.Longitude = aitoolContext_0.GetParameter<double>("longitude", 0.0);
				}
				satelliteMapImportRequest_0.MapSource = aitoolContext_0.GetParameter<string>("map_source", "tianditu");
				satelliteMapImportRequest_0.ZoomLevel = aitoolContext_0.GetParameter<int>("zoom_level", 18);
				satelliteMapImportRequest_0.RadiusKm = aitoolContext_0.GetParameter<double>("radius_km", 0.5);
				string text2;
				int createdFloorElementId;
				int createdMaterialElementId;
				object obj;
				if (!satelliteMapImportRequest_0.IsValid())
				{
					result = AIToolResult.Fail("请求参数无效。请提供位置名称 (location_name) 或坐标 (latitude + longitude)");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					if (aitoolContext_0.HasTransaction && aitoolContext_0.Transaction != null)
					{
						satelliteMapImportResult_0 = importSatelliteMapTool_0.isatelliteMapAIService_0.ImportSatelliteMapWithTransaction(satelliteMapImportRequest_0, aitoolContext_0.Transaction);
					}
					else
					{
						satelliteMapImportResult_0 = importSatelliteMapTool_0.isatelliteMapAIService_0.ImportSatelliteMap(satelliteMapImportRequest_0);
					}
					if (satelliteMapImportResult_0.IsSuccess)
					{
						string text;
						if (satelliteMapImportResult_0.UsedLocation == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 2);
							defaultInterpolatedStringHandler.AppendLiteral("坐标 (");
							defaultInterpolatedStringHandler.AppendFormatted(satelliteMapImportRequest_0.Latitude, "F4");
							defaultInterpolatedStringHandler.AppendLiteral(", ");
							defaultInterpolatedStringHandler.AppendFormatted(satelliteMapImportRequest_0.Longitude, "F4");
							defaultInterpolatedStringHandler.AppendLiteral(")");
							text = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(5, 3);
							defaultInterpolatedStringHandler2.AppendFormatted(satelliteMapImportResult_0.UsedLocation.DisplayName);
							defaultInterpolatedStringHandler2.AppendLiteral(" (");
							defaultInterpolatedStringHandler2.AppendFormatted(satelliteMapImportResult_0.UsedLocation.Latitude, "F4");
							defaultInterpolatedStringHandler2.AppendLiteral(", ");
							defaultInterpolatedStringHandler2.AppendFormatted(satelliteMapImportResult_0.UsedLocation.Longitude, "F4");
							defaultInterpolatedStringHandler2.AppendLiteral(")");
							text = defaultInterpolatedStringHandler2.ToStringAndClear();
						}
						string_0 = text;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(71, 5);
						defaultInterpolatedStringHandler3.AppendLiteral("成功导入卫星图：");
						defaultInterpolatedStringHandler3.AppendFormatted(string_0);
						defaultInterpolatedStringHandler3.AppendLiteral("\n");
						defaultInterpolatedStringHandler3.AppendLiteral("图像尺寸：");
						defaultInterpolatedStringHandler3.AppendFormatted(satelliteMapImportResult_0.ImageWidthFeet, "F0");
						defaultInterpolatedStringHandler3.AppendLiteral("ft x ");
						defaultInterpolatedStringHandler3.AppendFormatted(satelliteMapImportResult_0.ImageHeightFeet, "F0");
						defaultInterpolatedStringHandler3.AppendLiteral("ft\n");
						defaultInterpolatedStringHandler3.AppendLiteral("地图源：");
						defaultInterpolatedStringHandler3.AppendFormatted(satelliteMapImportResult_0.UsedMapSource);
						defaultInterpolatedStringHandler3.AppendLiteral("，缩放级别：");
						defaultInterpolatedStringHandler3.AppendFormatted(satelliteMapImportResult_0.UsedZoomLevel);
						defaultInterpolatedStringHandler3.AppendLiteral("\n");
						defaultInterpolatedStringHandler3.AppendLiteral("提示：视图样式已自动切换为 真实，如未看到贴图请手动设置视图样式为 真实 。");
						string_1 = defaultInterpolatedStringHandler3.ToStringAndClear();
						text2 = string_1;
						createdFloorElementId = satelliteMapImportResult_0.CreatedFloorElementId;
						createdMaterialElementId = satelliteMapImportResult_0.CreatedMaterialElementId;
						GeocodingResult usedLocation = satelliteMapImportResult_0.UsedLocation;
						if (usedLocation == null)
						{
							obj = null;
						}
						else
						{
							obj = usedLocation.DisplayName;
							if (obj != null)
							{
								goto IL_04bf;
							}
						}
						obj = satelliteMapImportRequest_0.LocationName;
						goto IL_04bf;
					}
					result = AIToolResult.Fail(satelliteMapImportResult_0.ErrorMessage ?? "卫星图导入失败");
				}
				goto end_IL_0002;
				IL_04bf:
				GeocodingResult usedLocation2 = satelliteMapImportResult_0.UsedLocation;
				double? gparam_ = ((usedLocation2 != null) ? new double?(usedLocation2.Latitude) : satelliteMapImportRequest_0.Latitude);
				GeocodingResult usedLocation3 = satelliteMapImportResult_0.UsedLocation;
				result = AIToolResult.Ok(text2, (object)new Class211<int, int, string, double?, double?, double, double, string, int>(createdFloorElementId, createdMaterialElementId, (string)obj, gparam_, (usedLocation3 != null) ? new double?(usedLocation3.Longitude) : satelliteMapImportRequest_0.Longitude, satelliteMapImportResult_0.ImageWidthFeet, satelliteMapImportResult_0.ImageHeightFeet, satelliteMapImportResult_0.UsedMapSource, satelliteMapImportResult_0.UsedZoomLevel));
				end_IL_0002:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[ImportSatelliteMapTool] 工具执行失败", exception_0);
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

	private readonly ISatelliteMapAIService isatelliteMapAIService_0;

	public string Name => "import_satellite_map";

	public string Category => "场地地形";

	public string Description => "根据位置名称或坐标自动导入卫星图到 Revit 项目";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"location_name\": {\n                \"type\": \"string\",\n                \"description\": \"位置名称，如 天府广场、北京市朝阳区。与 latitude+longitude 二选一\"\n            },\n            \"latitude\": {\n                \"type\": \"number\",\n                \"description\": \"纬度，-90~90，与 location_name 二选一\"\n            },\n            \"longitude\": {\n                \"type\": \"number\",\n                \"description\": \"经度，-180~180，与 location_name 二选一\"\n            },\n            \"map_source\": {\n                \"type\": \"string\",\n                \"description\": \"地图源：tianditu(天地图) 或 google(需代理)，默认 tianditu\",\n                \"enum\": [\"tianditu\", \"google\"]\n            },\n            \"zoom_level\": {\n                \"type\": \"integer\",\n                \"description\": \"缩放级别，1-18，默认 18\",\n                \"minimum\": 1,\n                \"maximum\": 18,\n                \"default\": 18\n            },\n            \"radius_km\": {\n                \"type\": \"number\",\n                \"description\": \"覆盖半径(公里)，0.1-10，默认 0.25\",\n                \"minimum\": 0.1,\n                \"maximum\": 10,\n                \"default\": 0.25\n            }\n        }\n    }";

	public ImportSatelliteMapTool()
	{
		isatelliteMapAIService_0 = (ISatelliteMapAIService)(object)new SatelliteMapAIService();
	}

	[AsyncStateMachine(typeof(Class347))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class347 stateMachine = new Class347();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.importSatelliteMapTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
