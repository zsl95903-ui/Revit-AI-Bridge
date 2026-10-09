using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
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

[AITool("create_spot_dimension", Category = "注释与标记", Description = "在 Revit 视图中创建点高程 (Spot Elevation) 或点坐标 (Spot Coordinate) 标注。距离单位：毫米", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateSpotDimensionTool : IAITool
{
	[CompilerGenerated]
	private sealed class Class418 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateSpotDimensionTool createSpotDimensionTool_0;

		private IAnnotationService iannotationService_0;

		private IElementService ielementService_0;

		private IViewService iviewService_0;

		private string string_0;

		private string string_1;

		private int int_1;

		private double double_0;

		private double double_1;

		private double double_2;

		private _003C_003Ef__AnonymousType27<double, double, double> _003C_003Ef__AnonymousType27_0;

		private bool bool_0;

		private int int_2;

		private object object_0;

		private object object_1;

		private object object_2;

		private string string_2;

		private object object_3;

		private string string_3;

		private string string_4;

		private object object_4;

		private int? nullable_0;

		private int? nullable_1;

		private string string_5;

		private object object_5;

		private object object_6;

		private IDictionary<string, object> idictionary_0;

		private object object_7;

		private object object_8;

		private object object_9;

		private Exception exception_0;

		private object object_10;

		private IDictionary<string, object> idictionary_1;

		private double double_3;

		private double double_4;

		private double double_5;

		private object object_11;

		private object object_12;

		private object object_13;

		private Exception exception_1;

		private object object_14;

		private IDictionary<string, object> idictionary_2;

		private double double_6;

		private double double_7;

		private double double_8;

		private object object_15;

		private object object_16;

		private object object_17;

		private Exception exception_2;

		private Exception exception_3;

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
					Class418 stateMachine = this;
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
			AIToolResult result;
			try
			{
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				iannotationService_0 = ((revitAdapter != null) ? revitAdapter.AnnotationService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
				iviewService_0 = ((revitAdapter3 != null) ? revitAdapter3.ViewService : null);
				if (iannotationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 AnnotationService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (iviewService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ViewService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else if (!aitoolContext_0.HasParameter("operator"))
				{
					result = AIToolResult.Fail("缺少 operator 参数（必须指定 SPOT_ELEVATION 或 SPOT_COORDINATE）");
				}
				else
				{
					string_0 = aitoolContext_0.GetParameter<string>("operator", (string)null);
					if (string.IsNullOrEmpty(string_0))
					{
						result = AIToolResult.Fail("operator 参数不能为空");
					}
					else
					{
						string_1 = string_0.ToUpper();
						if (string_1 != "SPOT_ELEVATION" && string_1 != "SPOT_COORDINATE")
						{
							result = AIToolResult.Fail("operator 参数值无效: '" + string_0 + "'。必须是 SPOT_ELEVATION 或 SPOT_COORDINATE");
						}
						else if (!aitoolContext_0.HasParameter("elementId"))
						{
							result = AIToolResult.Fail("缺少 elementId 参数");
						}
						else
						{
							int_1 = aitoolContext_0.GetParameter<int>("elementId", 0);
							if (!aitoolContext_0.HasParameter("point"))
							{
								result = AIToolResult.Fail("缺少 point 参数");
							}
							else
							{
								double_0 = 0.0;
								double_1 = 0.0;
								double_2 = 0.0;
								try
								{
									object_6 = aitoolContext_0.GetParameter<object>("point", (object)null);
									idictionary_0 = object_6 as IDictionary<string, object>;
									if (idictionary_0 != null)
									{
										if (idictionary_0.TryGetValue("x", out object_7) && object_7 != null)
										{
											double_0 = Convert.ToDouble(object_7);
										}
										if (idictionary_0.TryGetValue("y", out object_8) && object_8 != null)
										{
											double_1 = Convert.ToDouble(object_8);
										}
										if (idictionary_0.TryGetValue("z", out object_9) && object_9 != null)
										{
											double_2 = Convert.ToDouble(object_9);
										}
										object_7 = null;
										object_8 = null;
										object_9 = null;
									}
									object_6 = null;
									idictionary_0 = null;
								}
								catch (Exception ex)
								{
									exception_0 = ex;
									result = AIToolResult.Fail("解析 point 参数失败: " + exception_0.Message);
									goto end_IL_0067;
								}
								_003C_003Ef__AnonymousType27_0 = new _003C_003Ef__AnonymousType27<double, double, double>(double_0, double_1, double_2);
								bool_0 = aitoolContext_0.GetParameter<bool>("hasLeader", true);
								int_2 = aitoolContext_0.GetParameter<int>("spotDimensionTypeId", 0);
								object_0 = null;
								if (aitoolContext_0.HasParameter("bend"))
								{
									try
									{
										object_10 = aitoolContext_0.GetParameter<object>("bend", (object)null);
										if (object_10 != null)
										{
											idictionary_1 = object_10 as IDictionary<string, object>;
											if (idictionary_1 != null)
											{
												double_3 = 0.0;
												double_4 = 0.0;
												double_5 = 0.0;
												if (idictionary_1.TryGetValue("x", out object_11) && object_11 != null)
												{
													double_3 = Convert.ToDouble(object_11);
												}
												if (idictionary_1.TryGetValue("y", out object_12) && object_12 != null)
												{
													double_4 = Convert.ToDouble(object_12);
												}
												if (idictionary_1.TryGetValue("z", out object_13) && object_13 != null)
												{
													double_5 = Convert.ToDouble(object_13);
												}
												object_0 = new
												{
													x = double_3,
													y = double_4,
													z = double_5
												};
												object_11 = null;
												object_12 = null;
												object_13 = null;
											}
										}
										object_10 = null;
										idictionary_1 = null;
									}
									catch (Exception ex)
									{
										exception_1 = ex;
										Logger.Warning("[CreateSpotDimensionTool] 解析 bend 参数失败，将使用默认值: " + exception_1.Message);
									}
								}
								object_1 = null;
								if (aitoolContext_0.HasParameter("end"))
								{
									try
									{
										object_14 = aitoolContext_0.GetParameter<object>("end", (object)null);
										if (object_14 != null)
										{
											idictionary_2 = object_14 as IDictionary<string, object>;
											if (idictionary_2 != null)
											{
												double_6 = 0.0;
												double_7 = 0.0;
												double_8 = 0.0;
												if (idictionary_2.TryGetValue("x", out object_15) && object_15 != null)
												{
													double_6 = Convert.ToDouble(object_15);
												}
												if (idictionary_2.TryGetValue("y", out object_16) && object_16 != null)
												{
													double_7 = Convert.ToDouble(object_16);
												}
												if (idictionary_2.TryGetValue("z", out object_17) && object_17 != null)
												{
													double_8 = Convert.ToDouble(object_17);
												}
												object_1 = new
												{
													x = double_6,
													y = double_7,
													z = double_8
												};
												object_15 = null;
												object_16 = null;
												object_17 = null;
											}
										}
										object_14 = null;
										idictionary_2 = null;
									}
									catch (Exception ex)
									{
										exception_2 = ex;
										Logger.Warning("[CreateSpotDimensionTool] 解析 end 参数失败，将使用默认值: " + exception_2.Message);
									}
								}
								object_2 = iviewService_0.GetActiveView(aitoolContext_0.Document);
								if (object_2 == null)
								{
									result = AIToolResult.Fail("无法获取当前激活视图");
								}
								else
								{
									string_2 = iviewService_0.GetViewType(object_2);
									if (string_2 != null && hashSet_0.Contains(string_2))
									{
										result = AIToolResult.Fail("当前视图类型 '" + string_2 + "' 不支持创建高程标注，请切换到平面/立面/剖面视图");
									}
									else
									{
										object_3 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
										if (object_3 == null)
										{
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
											defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
											defaultInterpolatedStringHandler.AppendFormatted(int_1);
											defaultInterpolatedStringHandler.AppendLiteral(" 的元素");
											result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
										}
										else
										{
											string_3 = ielementService_0.GetElementCategory(object_3);
											string_4 = ielementService_0.GetElementName(object_3);
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(57, 7);
											defaultInterpolatedStringHandler2.AppendLiteral("[CreateSpotDimensionTool] 创建 ");
											defaultInterpolatedStringHandler2.AppendFormatted(string_0);
											defaultInterpolatedStringHandler2.AppendLiteral(" 高程标注：元素 ");
											defaultInterpolatedStringHandler2.AppendFormatted(int_1);
											defaultInterpolatedStringHandler2.AppendLiteral(" (");
											defaultInterpolatedStringHandler2.AppendFormatted(string_3);
											defaultInterpolatedStringHandler2.AppendLiteral(" - ");
											defaultInterpolatedStringHandler2.AppendFormatted(string_4);
											defaultInterpolatedStringHandler2.AppendLiteral(")，位置 (");
											defaultInterpolatedStringHandler2.AppendFormatted(double_0, "F0");
											defaultInterpolatedStringHandler2.AppendLiteral(", ");
											defaultInterpolatedStringHandler2.AppendFormatted(double_1, "F0");
											defaultInterpolatedStringHandler2.AppendLiteral(", ");
											defaultInterpolatedStringHandler2.AppendFormatted(double_2, "F0");
											defaultInterpolatedStringHandler2.AppendLiteral(") mm");
											Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
											object_4 = createSpotDimensionTool_0.method_1(aitoolContext_0.Document, object_2, object_3, int_1, _003C_003Ef__AnonymousType27_0, string_0, object_0, object_1, bool_0, (int_2 > 0) ? new int?(int_2) : ((int?)null), iannotationService_0, ielementService_0, iviewService_0);
											if (object_4 == null)
											{
												result = AIToolResult.Fail("创建高程标注失败（服务返回 null）");
											}
											else
											{
												nullable_0 = ielementService_0.GetElementId(object_4);
												nullable_1 = iviewService_0.GetViewId(object_2);
												string_5 = iviewService_0.GetViewName(object_2);
												object_5 = createSpotDimensionTool_0.method_0(object_4, string_0);
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(27, 4);
												defaultInterpolatedStringHandler3.AppendLiteral("成功创建 ");
												defaultInterpolatedStringHandler3.AppendFormatted(string_0);
												defaultInterpolatedStringHandler3.AppendLiteral(" 高程标注: 标注 ID ");
												defaultInterpolatedStringHandler3.AppendFormatted(nullable_0);
												defaultInterpolatedStringHandler3.AppendLiteral("，目标元素 ");
												defaultInterpolatedStringHandler3.AppendFormatted(int_1);
												defaultInterpolatedStringHandler3.AppendLiteral(" (");
												defaultInterpolatedStringHandler3.AppendFormatted(string_3);
												defaultInterpolatedStringHandler3.AppendLiteral(")");
												result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class80<string, int?, int, string, string, int?, bool, int?, string, _003C_003Ef__AnonymousType27<double, double, double>, string, object>(string_0, nullable_0, int_1, string_3, string_4, (int_2 > 0) ? new int?(int_2) : ((int?)null), bool_0, nullable_1, string_5, new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(double_0, 0), Math.Round(double_1, 0), Math.Round(double_2, 0)), "millimeters", object_5));
											}
										}
									}
								}
							}
						}
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_3 = ex;
				Logger.Error("[CreateSpotDimensionTool] 执行失败: " + exception_3.Message);
				result = AIToolResult.Fail("创建高程标注失败: " + exception_3.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private static readonly HashSet<string> hashSet_0 = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"ThreeD",
		"DrawingSheet",
		"Legend",
		"Schedule",
		"ProjectBrowser",
		"SystemBrowser",
		"Report"
	};

	public string Name => "create_spot_dimension";

	public string Category => "注释与标记";

	public string Description => "在 Revit 视图中创建点高程 (Spot Elevation) 或点坐标 (Spot Coordinate) 标注。距离单位：毫米";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operator\": {\n                \"type\": \"string\",\n                \"enum\": [\"SPOT_ELEVATION\", \"SPOT_COORDINATE\"],\n                \"description\": \"操作符：SPOT_ELEVATION(高程点), SPOT_COORDINATE(高程点坐标/XYZ)\"\n            },\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"被标注的目标构件 ElementId（如楼板、屋顶、管道、集水坑族等）\"\n            },\n            \"point\": {\n                \"type\": \"object\",\n                \"description\": \"标注的具体 3D 物理点坐标（毫米）。包含 x/y/z\",\n                \"properties\": {\n                    \"x\": { \"type\": \"number\", \"description\": \"X 坐标（毫米）\" },\n                    \"y\": { \"type\": \"number\", \"description\": \"Y 坐标（毫米）\" },\n                    \"z\": { \"type\": \"number\", \"description\": \"Z 坐标（毫米）\" }\n                },\n                \"required\": [\"x\", \"y\", \"z\"]\n            },\n            \"bend\": {\n                \"type\": \"object\",\n                \"description\": \"引线折弯点 3D 坐标（可选，若不填则自动生成默认位置），单位：毫米\",\n                \"properties\": {\n                    \"x\": { \"type\": \"number\", \"description\": \"X 坐标（毫米）\" },\n                    \"y\": { \"type\": \"number\", \"description\": \"Y 坐标（毫米）\" },\n                    \"z\": { \"type\": \"number\", \"description\": \"Z 坐标（毫米）\" }\n                }\n            },\n            \"end\": {\n                \"type\": \"object\",\n                \"description\": \"引线端点 3D 坐标（可选，若不填则自动生成默认位置），单位：毫米\",\n                \"properties\": {\n                    \"x\": { \"type\": \"number\", \"description\": \"X 坐标（毫米）\" },\n                    \"y\": { \"type\": \"number\", \"description\": \"Y 坐标（毫米）\" },\n                    \"z\": { \"type\": \"number\", \"description\": \"Z 坐标（毫米）\" }\n                }\n            },\n            \"hasLeader\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否带引线（可选，默认 true）\",\n                \"default\": true\n            },\n            \"spotDimensionTypeId\": {\n                \"type\": \"integer\",\n                \"description\": \"高程标注类型 ID（可选，0 或不传表示自动选择默认类型）\",\n                \"default\": 0\n            }\n        },\n        \"required\": [\"operator\", \"elementId\", \"point\"]\n    }";

	[AsyncStateMachine(typeof(Class418))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class418 stateMachine = new Class418();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createSpotDimensionTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private object? method_0(object object_0, string string_0)
	{
		try
		{
			Type type = object_0.GetType();
			string text = string_0.ToUpper();
			string text2 = text;
			if (!(text2 == "SPOT_ELEVATION"))
			{
				if (text2 == "SPOT_COORDINATE")
				{
					PropertyInfo property = type.GetProperty("XCoordinate");
					PropertyInfo property2 = type.GetProperty("YCoordinate");
					PropertyInfo property3 = type.GetProperty("Elevations");
					if (property != null && property2 != null)
					{
						object value = property.GetValue(object_0);
						object value2 = property2.GetValue(object_0);
						double z = 0.0;
						if (property3 != null)
						{
							object value3 = property3.GetValue(object_0);
							if (value3 is IList<double> { Count: >0 } list)
							{
								z = list[0];
							}
						}
						return new
						{
							x = value,
							y = value2,
							z = z
						};
					}
				}
			}
			else
			{
				PropertyInfo property4 = type.GetProperty("Elevations");
				if (property4 != null)
				{
					object value4 = property4.GetValue(object_0);
					if (value4 is IList<double> { Count: >0 } list2)
					{
						return new Class81<double>(list2[0]);
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Warning("[CreateSpotDimensionTool] 获取高程标注值失败: " + ex.Message);
			return null;
		}
	}

	private object? method_1(object object_0, object object_1, object object_2, int int_0, object object_3, string string_0, object? object_4, object? object_5, bool bool_0, int? nullable_0, IAnnotationService iannotationService_0, IElementService ielementService_0, IViewService iviewService_0)
	{
		object obj = null;
		Exception ex = null;
		try
		{
			Logger.Info("[CreateSpotDimensionTool] 策略1：尝试使用服务方法创建 " + string_0 + " 高程标注...");
			obj = iannotationService_0.CreateSpotDimension(object_0, object_1, int_0, object_3, string_0, object_4, object_5, bool_0, nullable_0);
			if (obj != null)
			{
				Logger.Info("[CreateSpotDimensionTool] 策略1成功：使用服务方法成功创建 " + string_0 + " 高程标注");
				return obj;
			}
		}
		catch (Exception ex2)
		{
			ex = ex2;
			Logger.Warning("[CreateSpotDimensionTool] 策略1失败：服务方法创建 " + string_0 + " 高程标注失败: " + ex2.Message);
		}
		if (obj == null && ex != null)
		{
			Logger.Warning("[CreateSpotDimensionTool] 策略1失败，原因: " + ex.Message);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[CreateSpotDimensionTool] 元素 ");
			defaultInterpolatedStringHandler.AppendFormatted(int_0);
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted(ielementService_0.GetElementCategory(object_2));
			defaultInterpolatedStringHandler.AppendLiteral(") 可能不支持 ");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral(" 高程标注");
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			string elementCategory = ielementService_0.GetElementCategory(object_2);
			if ((elementCategory != null && elementCategory.Contains("管道")) || (elementCategory != null && elementCategory.Contains("风管")))
			{
				Logger.Info("[CreateSpotDimensionTool] 检测到管线元素，建议：1) 切换到合适视图 2) 选择管线中心位置");
			}
		}
		if (obj == null && ex != null)
		{
			throw ex;
		}
		return obj;
	}
}
