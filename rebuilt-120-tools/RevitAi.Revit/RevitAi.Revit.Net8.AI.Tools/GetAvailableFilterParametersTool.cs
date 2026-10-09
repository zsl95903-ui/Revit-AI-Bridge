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
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_available_filter_parameters", Category = "视图高级操作", Description = "获取指定类别在过滤器中可用的参数列表（包括内置参数和自定义参数）。这是创建过滤器前的关键步骤，可以查询到参数名称、数据类型和支持的规则类型。", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetAvailableFilterParametersTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class488 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetAvailableFilterParametersTool getAvailableFilterParametersTool_0;

		private List<string> list_0;

		private Document document_0;

		private List<ElementId> list_1;

		private List<string> list_2;

		private ICollection<ElementId> icollection_0;

		private List<object> list_3;

		private Class154<List<string>, int, List<object>> class154_0;

		private string string_0;

		private List<string>.Enumerator enumerator_0;

		private string string_1;

		private Category category_0;

		private string string_2;

		private Exception exception_0;

		private IEnumerator<ElementId> ienumerator_0;

		private ElementId elementId_0;

		private object object_0;

		private Exception exception_1;

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
					Class488 stateMachine = this;
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
				list_0 = aitoolContext_0.GetParameter<List<string>>("categoryNames", (List<string>)null);
				if (list_0 == null || list_0.Count == 0)
				{
					result = AIToolResult.Fail("类别名称数组不能为空");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					object document = aitoolContext_0.Document;
					document_0 = (Document)((document is Document) ? document : null);
					if (document_0 == null)
					{
						result = AIToolResult.Fail("文档对象类型无效");
					}
					else
					{
						list_1 = new List<ElementId>();
						list_2 = new List<string>();
						enumerator_0 = list_0.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								string_1 = enumerator_0.Current;
								category_0 = getAvailableFilterParametersTool_0.method_0(document_0, string_1);
								if (category_0 != null)
								{
									list_1.Add(category_0.Id);
									list_2.Add(category_0.Name);
									Logger.Info("[GetAvailableFilterParametersTool] 找到类别: " + category_0.Name);
								}
								else
								{
									Logger.Warning("[GetAvailableFilterParametersTool] 找不到类别: " + string_1);
								}
								category_0 = null;
								string_1 = null;
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
						if (list_1.Count == 0)
						{
							string_2 = "没有找到任何有效的类别。提供的类别名称：" + string.Join(", ", list_0) + "\n\n";
							string_2 += "常见类别名称示例（必须使用中文）：\n";
							string_2 += "- 墙、门、窗、柱、房间、标高、轴网、楼板、屋顶、楼梯、栏杆、管道、风管、线管、设备、构件等\n";
							string_2 += "- 注意：类别名称必须与 Revit 界面中显示的名称完全一致";
							Logger.Error("[GetAvailableFilterParametersTool] " + string_2);
							result = AIToolResult.Fail(string_2);
						}
						else
						{
							try
							{
								icollection_0 = ParameterFilterUtilities.GetFilterableParametersInCommon(document_0, (ICollection<ElementId>)list_1);
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								Logger.Error("[GetAvailableFilterParametersTool] 获取可用参数失败: " + exception_0.Message);
								result = AIToolResult.Fail("获取可用参数失败: " + exception_0.Message);
								goto end_IL_0067;
							}
							if (icollection_0.Count == 0)
							{
								Logger.Warning("[GetAvailableFilterParametersTool] 类别 [" + string.Join(", ", list_2) + "] 没有共同的可用参数");
								result = AIToolResult.Ok("类别 [" + string.Join(", ", list_2) + "] 没有共同的可用参数", (object)new Class154<List<string>, int, object[]>(list_2, 0, Array.Empty<object>()));
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[GetAvailableFilterParametersTool] 类别 [");
								defaultInterpolatedStringHandler.AppendFormatted(string.Join(", ", list_2));
								defaultInterpolatedStringHandler.AppendLiteral("] 有 ");
								defaultInterpolatedStringHandler.AppendFormatted(icollection_0.Count);
								defaultInterpolatedStringHandler.AppendLiteral(" 个共同可用参数");
								Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
								list_3 = new List<object>();
								ienumerator_0 = icollection_0.GetEnumerator();
								try
								{
									while (ienumerator_0.MoveNext())
									{
										elementId_0 = ienumerator_0.Current;
										try
										{
											object_0 = getAvailableFilterParametersTool_0.method_1(document_0, elementId_0, list_1);
											if (object_0 != null)
											{
												list_3.Add(object_0);
											}
											object_0 = null;
										}
										catch (Exception ex)
										{
											exception_1 = ex;
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(49, 2);
											defaultInterpolatedStringHandler2.AppendLiteral("[GetAvailableFilterParametersTool] 处理参数 ID ");
											defaultInterpolatedStringHandler2.AppendFormatted(elementId_0.Value);
											defaultInterpolatedStringHandler2.AppendLiteral(" 时出错: ");
											defaultInterpolatedStringHandler2.AppendFormatted(exception_1.Message);
											Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
										}
										elementId_0 = null;
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
								list_3 = list_3.OrderBy<object, string>(delegate(object object_0)
								{
									PropertyInfo property = object_0.GetType().GetProperty("displayName");
									object obj;
									if ((object)property == null)
									{
										obj = null;
									}
									else
									{
										object? value = property.GetValue(object_0);
										if (value == null)
										{
											obj = null;
										}
										else
										{
											obj = value.ToString();
											if (obj != null)
											{
												goto IL_003d;
											}
										}
									}
									obj = "";
									goto IL_003d;
									IL_003d:
									return (string)obj;
								}, StringComparer.OrdinalIgnoreCase).ToList();
								class154_0 = new Class154<List<string>, int, List<object>>(list_2, list_3.Count, list_3);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(18, 2);
								defaultInterpolatedStringHandler3.AppendLiteral("类别 [");
								defaultInterpolatedStringHandler3.AppendFormatted(string.Join(", ", list_2));
								defaultInterpolatedStringHandler3.AppendLiteral("] 有 ");
								defaultInterpolatedStringHandler3.AppendFormatted(list_3.Count);
								defaultInterpolatedStringHandler3.AppendLiteral(" 个可用的过滤器参数");
								string_0 = defaultInterpolatedStringHandler3.ToStringAndClear();
								Logger.Info("[GetAvailableFilterParametersTool] " + string_0);
								result = AIToolResult.Ok(string_0, (object)class154_0);
							}
						}
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_2 = ex;
				Logger.Error("[GetAvailableFilterParametersTool] 获取可用参数失败: " + exception_2.Message);
				result = AIToolResult.Fail("获取可用参数失败: " + exception_2.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_available_filter_parameters";

	public string Category => "视图高级操作";

	public string Description => "获取指定类别可用的过滤器参数";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"categoryNames\": {\r\n                \"type\": \"array\",\r\n                \"items\": {\r\n                    \"type\": \"string\"\r\n                },\r\n                \"description\": \"类别名称数组（中文），例如：墙、门、窗等。必须使用与 Revit 界面一致的中文类别名称。可以传入多个类别以获取它们共同支持的参数。\"\r\n            }\r\n        },\r\n        \"required\": [\"categoryNames\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class488))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class488 stateMachine = new Class488();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getAvailableFilterParametersTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private Category? method_0(Document document_0, string string_0)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		try
		{
			foreach (Category item in (CategoryNameMap)document_0.Settings.Categories)
			{
				Category val = item;
				if (val.Name.Equals(string_0, StringComparison.OrdinalIgnoreCase))
				{
					return val;
				}
			}
			foreach (Category item2 in (CategoryNameMap)document_0.Settings.Categories)
			{
				Category val2 = item2;
				try
				{
					PropertyInfo property = ((object)val2).GetType().GetProperty("SubCategories");
					if (!(property != null) || !(property.GetValue(val2) is IEnumerable enumerable))
					{
						continue;
					}
					foreach (Category item3 in enumerable)
					{
						Category val3 = item3;
						if (val3 != null && val3.Name.Equals(string_0, StringComparison.OrdinalIgnoreCase))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 2);
							defaultInterpolatedStringHandler.AppendLiteral("[GetAvailableFilterParametersTool] 在父类别 '");
							defaultInterpolatedStringHandler.AppendFormatted(val2.Name);
							defaultInterpolatedStringHandler.AppendLiteral("' 下找到子类别 '");
							defaultInterpolatedStringHandler.AppendFormatted(string_0);
							defaultInterpolatedStringHandler.AppendLiteral("'");
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							return val3;
						}
					}
				}
				catch
				{
				}
			}
			Array values = Enum.GetValues(typeof(BuiltInCategory));
			foreach (BuiltInCategory item4 in values)
			{
				Autodesk.Revit.DB.Category category = Autodesk.Revit.DB.Category.GetCategory(document_0, item4);
				if (category != null && category.Name.Equals(string_0, StringComparison.OrdinalIgnoreCase))
				{
					return category;
				}
			}
			Logger.Warning("[GetAvailableFilterParametersTool] 找不到类别: " + string_0);
			return null;
		}
		catch (Exception ex)
		{
			Logger.Warning("[GetAvailableFilterParametersTool] 查找类别 '" + string_0 + "' 失败: " + ex.Message);
			return null;
		}
	}

	private object? method_1(Document document_0, ElementId elementId_0, IList<ElementId> ilist_0)
	{
		try
		{
			if (elementId_0.Value < 0L)
			{
				return method_2(document_0, elementId_0, ilist_0);
			}
			return method_3(document_0, elementId_0);
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[GetAvailableFilterParametersTool] 获取参数信息失败 (ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(elementId_0.Value);
			defaultInterpolatedStringHandler.AppendLiteral("): ");
			defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			return null;
		}
	}

	private unsafe object? method_2(Document document_0, ElementId elementId_0, IList<ElementId> ilist_0)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			BuiltInParameter val = (BuiltInParameter)elementId_0.Value;
			string text;
			try
			{
				text = LabelUtils.GetLabelFor(val);
			}
			catch
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
				defaultInterpolatedStringHandler.AppendLiteral("内置参数 ");
				defaultInterpolatedStringHandler.AppendFormatted<BuiltInParameter>(val);
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			string text2 = method_4(document_0, val, ilist_0);
			string[] gparam_ = method_5(text2 ?? "Unknown");
			return new Class155<long, string, string, string, string, string[], string>(elementId_0.Value, text, ((object)(*(BuiltInParameter*)(&val))/*cast due to constrained. prefix*/).ToString(), "BuiltInParameter", text2, gparam_, "内置参数 - " + text);
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(54, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[GetAvailableFilterParametersTool] 获取内置参数信息失败 (ID: ");
			defaultInterpolatedStringHandler2.AppendFormatted(elementId_0.Value);
			defaultInterpolatedStringHandler2.AppendLiteral("): ");
			defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
			Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
			long value = elementId_0.Value;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("未知内置参数 (ID: ");
			defaultInterpolatedStringHandler3.AppendFormatted(elementId_0.Value);
			defaultInterpolatedStringHandler3.AppendLiteral(")");
			string gparam_2 = defaultInterpolatedStringHandler3.ToStringAndClear();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(17, 1);
			defaultInterpolatedStringHandler4.AppendLiteral("BuiltInParameter_");
			defaultInterpolatedStringHandler4.AppendFormatted(elementId_0.Value);
			return new Class155<long, string, string, string, string, string[], string>(value, gparam_2, defaultInterpolatedStringHandler4.ToStringAndClear(), "BuiltInParameter", "Unknown", new string[0], "无法获取详细信息");
		}
	}

	private object? method_3(Document document_0, ElementId elementId_0)
	{
		try
		{
			Element element = document_0.GetElement(elementId_0);
			if (element == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[GetAvailableFilterParametersTool] 找不到参数元素 (ID: ");
				defaultInterpolatedStringHandler.AppendFormatted(elementId_0.Value);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			string text = "Unknown";
			string text2 = "CustomParameter";
			ParameterElement val = (ParameterElement)(object)((element is ParameterElement) ? element : null);
			string text3;
			if (val != null)
			{
				text3 = ((Element)val).Name;
				InternalDefinition definition = val.GetDefinition();
				if (definition != null)
				{
					try
					{
						PropertyInfo property = ((object)definition).GetType().GetProperty("StorageType");
						if (property != null)
						{
							object value = property.GetValue(definition);
							if (value != null)
							{
								text = value.ToString() ?? "Unknown";
							}
						}
					}
					catch
					{
						text = "Unknown";
					}
				}
				text2 = ((!(element is SharedParameterElement)) ? "ProjectParameter" : "SharedParameter");
			}
			else
			{
				string text4 = element.Name;
				if (text4 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("参数 ");
					defaultInterpolatedStringHandler2.AppendFormatted(elementId_0.Value);
					text4 = defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				text3 = text4;
			}
			string[] gparam_ = method_5(text ?? "Unknown");
			return new Class155<long, string, string, string, string, string[], string>(elementId_0.Value, text3, text3, text2, text, gparam_, text2 + " - " + text3);
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(55, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("[GetAvailableFilterParametersTool] 获取自定义参数信息失败 (ID: ");
			defaultInterpolatedStringHandler3.AppendFormatted(elementId_0.Value);
			defaultInterpolatedStringHandler3.AppendLiteral("): ");
			defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
			Logger.Warning(defaultInterpolatedStringHandler3.ToStringAndClear());
			return null;
		}
	}

	private string method_4(Document document_0, BuiltInParameter builtInParameter_0, IList<ElementId> ilist_0)
	{
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (ElementId item in ilist_0)
			{
				try
				{
					BuiltInCategory val = (BuiltInCategory)item.Value;
					Element val2 = ((IEnumerable<Element>)new FilteredElementCollector(document_0).OfCategory(val).WhereElementIsNotElementType()).FirstOrDefault();
					if (val2 != null)
					{
						Parameter val3 = val2.get_Parameter(builtInParameter_0);
						if (val3 != null)
						{
							return ((object)val3.StorageType/*cast due to constrained. prefix*/).ToString();
						}
					}
				}
				catch
				{
				}
			}
			foreach (ElementId item2 in ilist_0)
			{
				try
				{
					BuiltInCategory val4 = (BuiltInCategory)item2.Value;
					Element val5 = ((IEnumerable<Element>)new FilteredElementCollector(document_0).OfCategory(val4).WhereElementIsElementType()).FirstOrDefault();
					if (val5 != null)
					{
						Parameter val6 = val5.get_Parameter(builtInParameter_0);
						if (val6 != null)
						{
							return ((object)val6.StorageType/*cast due to constrained. prefix*/).ToString();
						}
					}
				}
				catch
				{
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(64, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[GetAvailableFilterParametersTool] 无法从类别 [");
			defaultInterpolatedStringHandler.AppendFormatted(string.Join(", ", ilist_0));
			defaultInterpolatedStringHandler.AppendLiteral("] 中获取参数 ");
			defaultInterpolatedStringHandler.AppendFormatted<BuiltInParameter>(builtInParameter_0);
			defaultInterpolatedStringHandler.AppendLiteral(" 的 StorageType");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return "Unknown";
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(74, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[GetAvailableFilterParametersTool] 获取 BuiltInParameter ");
			defaultInterpolatedStringHandler2.AppendFormatted<BuiltInParameter>(builtInParameter_0);
			defaultInterpolatedStringHandler2.AppendLiteral(" 的 StorageType 失败: ");
			defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
			Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
			return "Unknown";
		}
	}

	private string[] method_5(string? string_0)
	{
		if (!(string_0 == "String"))
		{
			if (!(string_0 == "Double"))
			{
				if (!(string_0 == "Integer"))
				{
					if (!(string_0 == "ElementId"))
					{
						return new string[2]
						{
							"equals",
							"not_equals"
						};
					}
					return new string[6]
					{
						"equals",
						"not_equals",
						"greater",
						"greater_or_equal",
						"less",
						"less_or_equal"
					};
				}
				return new string[6]
				{
					"equals",
					"not_equals",
					"greater",
					"greater_or_equal",
					"less",
					"less_or_equal"
				};
			}
			return new string[6]
			{
				"equals",
				"not_equals",
				"greater",
				"greater_or_equal",
				"less",
				"less_or_equal"
			};
		}
		return new string[8]
		{
			"equals",
			"not_equals",
			"contains",
			"not_contains",
			"begins",
			"not_begins",
			"ends",
			"not_ends"
		};
	}
}
