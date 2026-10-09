using System;
using System.Collections;
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
using RevitAi.Abstractions.Units;
using Newtonsoft.Json.Linq;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("create_grid", Category = "元素创建", Description = "创建网格线（轴线），支持单个或批量创建。自动从项目设置中获取长度单位。", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateGridTool : IAITool
{
	private struct Struct3
	{
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int int_0;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double double_0;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double double_1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private double double_2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private double double_3;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double double_4;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private double double_5;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private double double_6;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double double_7;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string? string_0;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string string_1;

		public int Index
		{
			[CompilerGenerated]
			readonly get
			{
				return int_0;
			}
			[CompilerGenerated]
			set
			{
				int_0 = value;
			}
		}

		public double StartX
		{
			[CompilerGenerated]
			readonly get
			{
				return double_0;
			}
			[CompilerGenerated]
			set
			{
				double_0 = value;
			}
		}

		public double StartY
		{
			[CompilerGenerated]
			readonly get
			{
				return double_1;
			}
			[CompilerGenerated]
			set
			{
				double_1 = value;
			}
		}

		public double EndX
		{
			[CompilerGenerated]
			readonly get
			{
				return double_2;
			}
			[CompilerGenerated]
			set
			{
				double_2 = value;
			}
		}

		public double EndY
		{
			[CompilerGenerated]
			readonly get
			{
				return double_3;
			}
			[CompilerGenerated]
			set
			{
				double_3 = value;
			}
		}

		public double OriginalStartX
		{
			[CompilerGenerated]
			readonly get
			{
				return double_4;
			}
			[CompilerGenerated]
			set
			{
				double_4 = value;
			}
		}

		public double OriginalStartY
		{
			[CompilerGenerated]
			readonly get
			{
				return double_5;
			}
			[CompilerGenerated]
			set
			{
				double_5 = value;
			}
		}

		public double OriginalEndX
		{
			[CompilerGenerated]
			readonly get
			{
				return double_6;
			}
			[CompilerGenerated]
			set
			{
				double_6 = value;
			}
		}

		public double OriginalEndY
		{
			[CompilerGenerated]
			readonly get
			{
				return double_7;
			}
			[CompilerGenerated]
			set
			{
				double_7 = value;
			}
		}

		public string? Name
		{
			[CompilerGenerated]
			readonly get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		public string Unit
		{
			[CompilerGenerated]
			readonly get
			{
				return string_1;
			}
			[CompilerGenerated]
			set
			{
				string_1 = value;
			}
		}
	}

	[CompilerGenerated]
	private sealed class Class407 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateGridTool createGridTool_0;

		private ICreationService icreationService_0;

		private IElementService ielementService_0;

		private ProjectUnitInfo projectUnitInfo_0;

		private string string_0;

		private object object_0;

		private IList ilist_0;

		private List<Struct3> list_0;

		private List<string> list_1;

		private List<object> list_2;

		private List<string> list_3;

		private int int_1;

		private int int_2;

		private string string_1;

		private int int_3;

		private object object_1;

		private Struct3? nullable_0;

		private Exception exception_0;

		private List<Struct3>.Enumerator enumerator_0;

		private Struct3 struct3_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private _003C_003Ef__AnonymousType27<double, double, double> _003C_003Ef__AnonymousType27_0;

		private _003C_003Ef__AnonymousType27<double, double, double> _003C_003Ef__AnonymousType27_1;

		private object object_2;

		private int? nullable_1;

		private string string_2;

		private double double_3;

		private string string_3;

		private string string_4;

		private Exception exception_1;

		private string string_5;

		private Exception exception_2;

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
					Class407 stateMachine = this;
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
				icreationService_0 = ((revitAdapter != null) ? revitAdapter.CreationService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				string text;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8;
				object obj;
				if (icreationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 CreationService");
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
					projectUnitInfo_0 = null;
					if (aitoolContext_0.ProjectUnitService != null)
					{
						projectUnitInfo_0 = aitoolContext_0.ProjectUnitService.GetProjectUnit(aitoolContext_0.Document, (UnitType)1);
					}
					string_0 = smethod_0(projectUnitInfo_0);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[CreateGridTool] 使用单位: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral(" (来源: ");
					defaultInterpolatedStringHandler.AppendFormatted((projectUnitInfo_0 != null) ? "项目设置" : "默认");
					defaultInterpolatedStringHandler.AppendLiteral(")");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					object_0 = aitoolContext_0.GetParameter<object>("grids", (object)null);
					ilist_0 = object_0 as IList;
					if (ilist_0 == null)
					{
						result = AIToolResult.Fail("grids 参数格式错误：必须是数组");
					}
					else if (ilist_0.Count == 0)
					{
						result = AIToolResult.Fail("grids 数组不能为空");
					}
					else
					{
						list_0 = new List<Struct3>();
						list_1 = new List<string>();
						int_3 = 0;
						while (int_3 < ilist_0.Count)
						{
							try
							{
								object_1 = ilist_0[int_3];
								if (object_1 == null)
								{
									List<string> list = list_1;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(11, 1);
									defaultInterpolatedStringHandler2.AppendLiteral("第 ");
									defaultInterpolatedStringHandler2.AppendFormatted(int_3 + 1);
									defaultInterpolatedStringHandler2.AppendLiteral(" 条网格线数据为空");
									list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
								}
								else
								{
									nullable_0 = createGridTool_0.method_0(object_1, int_3 + 1, string_0);
									if (nullable_0.HasValue)
									{
										list_0.Add(nullable_0.Value);
									}
									else
									{
										List<string> list2 = list_1;
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 1);
										defaultInterpolatedStringHandler3.AppendLiteral("第 ");
										defaultInterpolatedStringHandler3.AppendFormatted(int_3 + 1);
										defaultInterpolatedStringHandler3.AppendLiteral(" 条网格线数据格式无法识别");
										list2.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
									}
									object_1 = null;
									nullable_0 = null;
								}
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								List<string> list3 = list_1;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(13, 2);
								defaultInterpolatedStringHandler4.AppendLiteral("第 ");
								defaultInterpolatedStringHandler4.AppendFormatted(int_3 + 1);
								defaultInterpolatedStringHandler4.AppendLiteral(" 条网格线解析失败: ");
								defaultInterpolatedStringHandler4.AppendFormatted(exception_0.Message);
								list3.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(28, 1);
								defaultInterpolatedStringHandler5.AppendLiteral("[CreateGridTool] 第 ");
								defaultInterpolatedStringHandler5.AppendFormatted(int_3 + 1);
								defaultInterpolatedStringHandler5.AppendLiteral(" 条网格线解析失败");
								Logger.Error(defaultInterpolatedStringHandler5.ToStringAndClear(), exception_0);
							}
							int_3++;
						}
						if (list_0.Count != 0)
						{
							list_2 = new List<object>();
							list_3 = new List<string>();
							enumerator_0 = list_0.GetEnumerator();
							try
							{
								while (enumerator_0.MoveNext())
								{
									struct3_0 = enumerator_0.Current;
									try
									{
										double_0 = struct3_0.EndX - struct3_0.StartX;
										double_1 = struct3_0.EndY - struct3_0.StartY;
										double_2 = double_0 * double_0 + double_1 * double_1;
										if (double_2 < 1.0 / 256.0)
										{
											double_3 = Math.Sqrt(double_2) * 304.8;
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(17, 2);
											defaultInterpolatedStringHandler6.AppendLiteral("网格线 '");
											defaultInterpolatedStringHandler6.AppendFormatted(struct3_0.Name ?? "未命名");
											defaultInterpolatedStringHandler6.AppendLiteral("' 长度过短 (");
											defaultInterpolatedStringHandler6.AppendFormatted(double_3, "F2");
											defaultInterpolatedStringHandler6.AppendLiteral(" mm)");
											string_3 = defaultInterpolatedStringHandler6.ToStringAndClear();
											Logger.Warning("[CreateGridTool] " + string_3);
											list_3.Add(string_3);
											continue;
										}
										_003C_003Ef__AnonymousType27_0 = new _003C_003Ef__AnonymousType27<double, double, double>(struct3_0.StartX, struct3_0.StartY, 0.0);
										_003C_003Ef__AnonymousType27_1 = new _003C_003Ef__AnonymousType27<double, double, double>(struct3_0.EndX, struct3_0.EndY, 0.0);
										object_2 = icreationService_0.CreateGrid(aitoolContext_0.Document, (object)_003C_003Ef__AnonymousType27_0, (object)_003C_003Ef__AnonymousType27_1, struct3_0.Name);
										if (object_2 == null)
										{
											string_4 = "网格线 '" + (struct3_0.Name ?? "未命名") + "' 创建失败";
											Logger.Error("[CreateGridTool] " + string_4);
											list_3.Add(string_4);
											continue;
										}
										nullable_1 = ielementService_0.GetElementId(object_2);
										string_2 = ielementService_0.GetElementName(object_2);
										list_2.Add(new Class62<int?, string, string, Class63<double, double>, Class63<double, double>, string>(nullable_1, string_2, struct3_0.Name, new Class63<double, double>(struct3_0.OriginalStartX, struct3_0.OriginalStartY), new Class63<double, double>(struct3_0.OriginalEndX, struct3_0.OriginalEndY), struct3_0.Unit));
										_003C_003Ef__AnonymousType27_0 = null;
										_003C_003Ef__AnonymousType27_1 = null;
										object_2 = null;
										string_2 = null;
									}
									catch (Exception ex)
									{
										exception_1 = ex;
										string_5 = "网格线 '" + (struct3_0.Name ?? "未命名") + "' 创建失败: " + exception_1.Message;
										Logger.Error("[CreateGridTool] " + string_5, exception_1);
										list_3.Add(string_5);
										string_5 = null;
									}
									struct3_0 = default(Struct3);
								}
							}
							finally
							{
								if (num < 0)
								{
									((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
								}
							}
							enumerator_0 = default(List<Struct3>.Enumerator);
							int_1 = list_2.Count;
							int_2 = list_3.Count;
							if (int_1 != 1)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(15, 1);
								defaultInterpolatedStringHandler7.AppendLiteral("批量创建网格线完成：成功 ");
								defaultInterpolatedStringHandler7.AppendFormatted(int_1);
								defaultInterpolatedStringHandler7.AppendLiteral(" 个");
								text = defaultInterpolatedStringHandler7.ToStringAndClear();
								goto IL_0977;
							}
							defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(9, 1);
							defaultInterpolatedStringHandler8.AppendLiteral("成功创建网格线: ");
							PropertyInfo? property = list_2[0].GetType().GetProperty("gridName");
							if ((object)property == null)
							{
								obj = null;
							}
							else
							{
								obj = property.GetValue(list_2[0]);
								if (obj != null)
								{
									goto IL_096b;
								}
							}
							obj = "未命名";
							goto IL_096b;
						}
						result = AIToolResult.Fail("没有有效的网格线数据。解析错误: " + string.Join("; ", list_1));
					}
				}
				goto end_IL_0067;
				IL_096b:
				defaultInterpolatedStringHandler8.AppendFormatted<object>(obj);
				text = defaultInterpolatedStringHandler8.ToStringAndClear();
				goto IL_0977;
				IL_0977:
				string_1 = text;
				if (int_2 > 0)
				{
					string text2 = string_1;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler9.AppendLiteral("，失败 ");
					defaultInterpolatedStringHandler9.AppendFormatted(int_2);
					defaultInterpolatedStringHandler9.AppendLiteral(" 个");
					string_1 = text2 + defaultInterpolatedStringHandler9.ToStringAndClear();
				}
				result = AIToolResult.Ok(string_1, (object)new Class64<int, int, string, List<object>, List<string>, List<string>>(int_1, int_2, string_0, list_2, (int_2 > 0) ? list_3 : null, (list_1.Count > 0) ? list_1 : null));
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_2 = ex;
				Logger.Error("[CreateGridTool] 执行失败", exception_2);
				result = AIToolResult.Fail("创建网格线失败: " + exception_2.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private static readonly Dictionary<string, double> dictionary_0 = new Dictionary<string, double>
	{
		{
			"mm",
			0.0032808398950131233
		},
		{
			"millimeter",
			0.0032808398950131233
		},
		{
			"millimeters",
			0.0032808398950131233
		},
		{
			"cm",
			25.0 / 762.0
		},
		{
			"centimeter",
			25.0 / 762.0
		},
		{
			"centimeters",
			25.0 / 762.0
		},
		{
			"m",
			3.280839895013123
		},
		{
			"meter",
			3.280839895013123
		},
		{
			"meters",
			3.280839895013123
		},
		{
			"ft",
			1.0
		},
		{
			"feet",
			1.0
		},
		{
			"foot",
			1.0
		},
		{
			"inch",
			1.0 / 12.0
		},
		{
			"inches",
			1.0 / 12.0
		}
	};

	public string Name => "create_grid";

	public string Category => "元素创建";

	public string Description => "创建网格线（轴线），支持单个或批量创建。自动从项目设置中获取长度单位。";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"grids\": {\n                \"type\": \"array\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"start_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"起点 X 坐标（使用项目长度单位）\"\n                        },\n                        \"start_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"起点 Y 坐标（使用项目长度单位）\"\n                        },\n                        \"end_x\": {\n                            \"type\": \"number\",\n                            \"description\": \"终点 X 坐标（使用项目长度单位）\"\n                        },\n                        \"end_y\": {\n                            \"type\": \"number\",\n                            \"description\": \"终点 Y 坐标（使用项目长度单位）\"\n                        },\n                        \"grid_name\": {\n                            \"type\": \"string\",\n                            \"description\": \"网格线名称（可选），例如：1、A、2、B等。如果不提供，Revit会自动命名\"\n                        }\n                    },\n                    \"required\": [\"start_x\", \"start_y\", \"end_x\", \"end_y\"]\n                },\n                \"description\": \"网格线数组，支持多种格式：标准格式 [{start_x, start_y, end_x, end_y, grid_name}]、扁平数组格式 [[x1, y1, x2, y2, name]]、线条对象格式 [{line, name}]\"\n            }\n        },\n        \"required\": [\"grids\"]\n    }";

	private static string smethod_0(ProjectUnitInfo? projectUnitInfo_0)
	{
		if (projectUnitInfo_0 == null)
		{
			return "mm";
		}
		string displayUnitSymbol = projectUnitInfo_0.DisplayUnitSymbol;
		object obj;
		if (displayUnitSymbol == null)
		{
			obj = null;
		}
		else
		{
			obj = displayUnitSymbol.ToLowerInvariant();
			if (obj != null)
			{
				goto IL_0036;
			}
		}
		obj = "";
		goto IL_0036;
		IL_0036:
		string text = (string)obj;
		if (text.Contains("mm") || text.Contains("millimeter"))
		{
			return "mm";
		}
		if (text.Contains("cm") || text.Contains("centimeter"))
		{
			return "cm";
		}
		if (text.Contains("m") && !text.Contains("mm"))
		{
			return "m";
		}
		if (text.Contains("ft") || text.Contains("foot"))
		{
			return "ft";
		}
		if (text.Contains("in") || text.Contains("inch"))
		{
			return "inch";
		}
		if (projectUnitInfo_0.IsMetric)
		{
			return "mm";
		}
		return "ft";
	}

	private static double smethod_1(double double_0, string string_0)
	{
		string key = string_0.ToLowerInvariant();
		if (dictionary_0.TryGetValue(key, out var value))
		{
			return double_0 * value;
		}
		Logger.Warning("[CreateGridTool] 未知单位: " + string_0 + "，默认使用毫米");
		return double_0 / 304.8;
	}

	[AsyncStateMachine(typeof(Class407))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class407 stateMachine = new Class407();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createGridTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private Struct3? method_0(object object_0, int int_0, string string_0)
	{
		if (object_0 is Dictionary<string, object> dictionary_)
		{
			return method_1(dictionary_, int_0, string_0);
		}
		JObject val = (JObject)((object_0 is JObject) ? object_0 : null);
		if (val != null)
		{
			return method_2(val, int_0, string_0);
		}
		if (object_0 is IList ilist_)
		{
			return method_3(ilist_, int_0, string_0);
		}
		return null;
	}

	private Struct3? method_1(Dictionary<string, object> dictionary_1, int int_0, string string_0)
	{
		double? num = smethod_2(dictionary_1, "start_x") ?? smethod_2(dictionary_1, "startX");
		double? num2 = smethod_2(dictionary_1, "start_y") ?? smethod_2(dictionary_1, "startY");
		double? num3 = smethod_2(dictionary_1, "end_x") ?? smethod_2(dictionary_1, "endX");
		double? num4 = smethod_2(dictionary_1, "end_y") ?? smethod_2(dictionary_1, "endY");
		string text = smethod_3(dictionary_1, "grid_name") ?? smethod_3(dictionary_1, "name");
		string text2 = smethod_3(dictionary_1, "unit") ?? string_0;
		if (num.HasValue && num2.HasValue && num3.HasValue && num4.HasValue)
		{
			return new Struct3
			{
				Index = int_0,
				StartX = smethod_1(num.Value, text2),
				StartY = smethod_1(num2.Value, text2),
				EndX = smethod_1(num3.Value, text2),
				EndY = smethod_1(num4.Value, text2),
				OriginalStartX = num.Value,
				OriginalStartY = num2.Value,
				OriginalEndX = num3.Value,
				OriginalEndY = num4.Value,
				Name = text,
				Unit = text2
			};
		}
		if (dictionary_1.TryGetValue("line", out object value) && value is Dictionary<string, object> dictionary_2)
		{
			double? num5 = smethod_2(dictionary_2, "x");
			double? num6 = smethod_2(dictionary_2, "y");
			object obj = smethod_4(dictionary_2, "end");
			if (num5.HasValue && num6.HasValue && obj is Dictionary<string, object> dictionary_3)
			{
				double? num7 = smethod_2(dictionary_3, "x");
				double? num8 = smethod_2(dictionary_3, "y");
				if (num7.HasValue && num8.HasValue)
				{
					return new Struct3
					{
						Index = int_0,
						StartX = smethod_1(num5.Value, text2),
						StartY = smethod_1(num6.Value, text2),
						EndX = smethod_1(num7.Value, text2),
						EndY = smethod_1(num8.Value, text2),
						OriginalStartX = num5.Value,
						OriginalStartY = num6.Value,
						OriginalEndX = num7.Value,
						OriginalEndY = num8.Value,
						Name = (text ?? smethod_3(dictionary_1, "name")),
						Unit = text2
					};
				}
			}
		}
		return null;
	}

	private Struct3? method_2(JObject jobject_0, int int_0, string string_0)
	{
		JToken val = jobject_0["start_x"];
		JToken val2 = jobject_0["start_y"];
		JToken val3 = jobject_0["end_x"];
		JToken val4 = jobject_0["end_y"];
		JToken val5 = jobject_0["grid_name"] ?? jobject_0["name"];
		JToken val6 = jobject_0["unit"];
		object obj;
		if (val != null && val2 != null && val3 != null && val4 != null)
		{
			if (val6 == null)
			{
				obj = null;
			}
			else
			{
				obj = val6.ToObject<string>();
				if (obj != null)
				{
					goto IL_00a4;
				}
			}
			obj = string_0;
			goto IL_00a4;
		}
		return null;
		IL_00a4:
		string text = (string)obj;
		double num = val.ToObject<double>();
		double num2 = val2.ToObject<double>();
		double num3 = val3.ToObject<double>();
		double num4 = val4.ToObject<double>();
		string name = ((val5 != null) ? val5.ToObject<string>() : null);
		return new Struct3
		{
			Index = int_0,
			StartX = smethod_1(num, text),
			StartY = smethod_1(num2, text),
			EndX = smethod_1(num3, text),
			EndY = smethod_1(num4, text),
			OriginalStartX = num,
			OriginalStartY = num2,
			OriginalEndX = num3,
			OriginalEndY = num4,
			Name = name,
			Unit = text
		};
	}

	private Struct3? method_3(IList ilist_0, int int_0, string string_0)
	{
		if (ilist_0.Count < 4)
		{
			return null;
		}
		object obj = ilist_0[0];
		object obj2 = ilist_0[1];
		object obj3 = ilist_0[2];
		object obj4 = ilist_0[3];
		if (obj == null || obj2 == null || obj3 == null || obj4 == null)
		{
			return null;
		}
		double? num = smethod_5(obj);
		double? num2 = smethod_5(obj2);
		double? num3 = smethod_5(obj3);
		double? num4 = smethod_5(obj4);
		string name = ((ilist_0.Count <= 4) ? null : ilist_0[4]?.ToString());
		if (num.HasValue && num2.HasValue && num3.HasValue && num4.HasValue)
		{
			return new Struct3
			{
				Index = int_0,
				StartX = smethod_1(num.Value, string_0),
				StartY = smethod_1(num2.Value, string_0),
				EndX = smethod_1(num3.Value, string_0),
				EndY = smethod_1(num4.Value, string_0),
				OriginalStartX = num.Value,
				OriginalStartY = num2.Value,
				OriginalEndX = num3.Value,
				OriginalEndY = num4.Value,
				Name = name,
				Unit = string_0
			};
		}
		return null;
	}

	private static double? smethod_2(Dictionary<string, object> dictionary_1, string string_0)
	{
		if (!dictionary_1.TryGetValue(string_0, out object value))
		{
			return null;
		}
		if (value is double value2)
		{
			return value2;
		}
		if (value is decimal num)
		{
			return (double)num;
		}
		if (value is long num2)
		{
			return num2;
		}
		if (value is int num3)
		{
			return num3;
		}
		if (value is float num4)
		{
			return num4;
		}
		return null;
	}

	private static string? smethod_3(Dictionary<string, object> dictionary_1, string string_0)
	{
		object value;
		return (!dictionary_1.TryGetValue(string_0, out value)) ? null : value?.ToString();
	}

	private static object? smethod_4(Dictionary<string, object> dictionary_1, string string_0)
	{
		object value;
		return dictionary_1.TryGetValue(string_0, out value) ? value : null;
	}

	private static double? smethod_5(object object_0)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Invalid comparison between Unknown and I4
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Invalid comparison between Unknown and I4
		if (object_0 is double value)
		{
			return value;
		}
		if (object_0 is decimal num)
		{
			return (double)num;
		}
		if (object_0 is long num2)
		{
			return num2;
		}
		if (object_0 is int num3)
		{
			return num3;
		}
		if (object_0 is float num4)
		{
			return num4;
		}
		JValue val = (JValue)((object_0 is JValue) ? object_0 : null);
		if (val != null && (int)((JToken)val).Type == 7)
		{
			return ((JToken)val).ToObject<double>();
		}
		JValue val2 = (JValue)((object_0 is JValue) ? object_0 : null);
		if (val2 != null && (int)((JToken)val2).Type == 6)
		{
			return ((JToken)val2).ToObject<double>();
		}
		return null;
	}
}
