using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_view_filters", Category = "视图高级操作", Description = "获取当前文档中的所有视图过滤器，包括过滤器名称、类别、规则等详细信息。", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetViewFiltersTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class508 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetViewFiltersTool getViewFiltersTool_0;

		private Document document_0;

		private bool bool_0;

		private ElementClassFilter elementClassFilter_0;

		private FilteredElementCollector filteredElementCollector_0;

		private List<ParameterFilterElement> list_0;

		private List<object> list_1;

		private Class201<int, List<object>> class201_0;

		private string string_0;

		private List<ParameterFilterElement>.Enumerator enumerator_0;

		private ParameterFilterElement parameterFilterElement_0;

		private object object_0;

		private Exception exception_0;

		private Exception exception_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Expected O, but got Unknown
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Expected O, but got Unknown
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
					Class508 stateMachine = this;
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
				if (aitoolContext_0.Document == null)
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
						bool_0 = aitoolContext_0.GetParameter<bool>("includeRules", false);
						elementClassFilter_0 = new ElementClassFilter(typeof(ParameterFilterElement));
						filteredElementCollector_0 = new FilteredElementCollector(document_0);
						list_0 = filteredElementCollector_0.WherePasses((ElementFilter)(object)elementClassFilter_0).ToElements().Cast<ParameterFilterElement>()
							.ToList();
						if (list_0.Count == 0)
						{
							Logger.Info("[GetViewFiltersTool] 当前文档中没有视图过滤器");
							result = AIToolResult.Ok("当前文档中没有视图过滤器", (object)new Class201<int, object[]>(0, Array.Empty<object>()));
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
							defaultInterpolatedStringHandler.AppendLiteral("[GetViewFiltersTool] 找到 ");
							defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
							defaultInterpolatedStringHandler.AppendLiteral(" 个视图过滤器");
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							list_1 = new List<object>();
							enumerator_0 = list_0.GetEnumerator();
							try
							{
								while (enumerator_0.MoveNext())
								{
									parameterFilterElement_0 = enumerator_0.Current;
									try
									{
										object_0 = getViewFiltersTool_0.method_0(parameterFilterElement_0, bool_0, document_0);
										list_1.Add(object_0);
										object_0 = null;
									}
									catch (Exception ex)
									{
										exception_0 = ex;
										Logger.Warning("[GetViewFiltersTool] 处理过滤器 " + ((Element)parameterFilterElement_0).Name + " 时出错: " + exception_0.Message);
									}
									parameterFilterElement_0 = null;
								}
							}
							finally
							{
								if (num < 0)
								{
									((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
								}
							}
							enumerator_0 = default(List<ParameterFilterElement>.Enumerator);
							class201_0 = new Class201<int, List<object>>(list_1.Count, list_1);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(10, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("找到 ");
							defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler2.AppendLiteral(" 个视图过滤器");
							string_0 = defaultInterpolatedStringHandler2.ToStringAndClear();
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(33, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("[GetViewFiltersTool] 成功获取 ");
							defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler3.AppendLiteral(" 个过滤器信息");
							Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
							result = AIToolResult.Ok(string_0, (object)class201_0);
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_1 = ex;
				Logger.Error("[GetViewFiltersTool] 获取过滤器失败: " + exception_1.Message);
				result = AIToolResult.Fail("获取过滤器失败: " + exception_1.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_view_filters";

	public string Category => "视图高级操作";

	public string Description => "获取所有视图过滤器信息";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"includeRules\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否包含过滤规则详情（默认为 false，只返回基本信息）\",\n                \"default\": false\n            }\n        },\n        \"description\": \"获取所有视图过滤器的参数\"\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class508))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class508 stateMachine = new Class508();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getViewFiltersTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private object method_0(ParameterFilterElement parameterFilterElement_0, bool bool_0, Document document_0)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		ICollection<ElementId> categories = parameterFilterElement_0.GetCategories();
		List<string> list = new List<string>();
		foreach (ElementId item2 in categories)
		{
			try
			{
				string item = ((object)(BuiltInCategory)item2.Value/*cast due to constrained. prefix*/).ToString();
				list.Add(item);
			}
			catch
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Category ID ");
				defaultInterpolatedStringHandler.AppendFormatted(item2.Value);
				list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
		Class202<long, string, List<string>, int> @class = new Class202<long, string, List<string>, int>(((Element)parameterFilterElement_0).Id.Value, ((Element)parameterFilterElement_0).Name, list, list.Count);
		if (!bool_0)
		{
			return @class;
		}
		List<object> list2 = new List<object>();
		try
		{
			ElementFilter elementFilter = parameterFilterElement_0.GetElementFilter();
			if (elementFilter != null)
			{
				method_1(document_0, elementFilter, list2);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[GetViewFiltersTool] 获取过滤器 " + ((Element)parameterFilterElement_0).Name + " 的规则时出错: " + ex.Message);
		}
		return new Class203<long, string, List<string>, int, List<object>, int>(@class.id, @class.name, @class.categories, @class.categoryCount, list2, list2.Count);
	}

	private void method_1(Document document_0, ElementFilter elementFilter_0, List<object> list_0)
	{
		try
		{
			LogicalAndFilter val = (LogicalAndFilter)(object)((elementFilter_0 is LogicalAndFilter) ? elementFilter_0 : null);
			if (val != null)
			{
				IList<ElementFilter> filters = ((ElementLogicalFilter)val).GetFilters();
				{
					foreach (ElementFilter item2 in filters)
					{
						method_1(document_0, item2, list_0);
					}
					return;
				}
			}
			LogicalOrFilter val2 = (LogicalOrFilter)(object)((elementFilter_0 is LogicalOrFilter) ? elementFilter_0 : null);
			if (val2 != null)
			{
				IList<ElementFilter> filters2 = ((ElementLogicalFilter)val2).GetFilters();
				{
					foreach (ElementFilter item3 in filters2)
					{
						method_1(document_0, item3, list_0);
					}
					return;
				}
			}
			ElementParameterFilter val3 = (ElementParameterFilter)(object)((elementFilter_0 is ElementParameterFilter) ? elementFilter_0 : null);
			if (val3 == null)
			{
				return;
			}
			IList<FilterRule> rules = val3.GetRules();
			foreach (FilterRule item4 in rules)
			{
				try
				{
					ElementId ruleParameter = item4.GetRuleParameter();
					object item = method_2(item4, ruleParameter, document_0);
					list_0.Add(item);
				}
				catch (Exception ex)
				{
					Logger.Warning("[GetViewFiltersTool] 解析规则时出错: " + ex.Message);
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Warning("[GetViewFiltersTool] 递归解析 ElementFilter 时出错: " + ex2.Message);
		}
	}

	private object method_2(FilterRule filterRule_0, ElementId elementId_0, Document document_0)
	{
		try
		{
			string text = "Unknown";
			string text2 = "Unknown";
			try
			{
				Element element = document_0.GetElement(elementId_0);
				ParameterElement val = (ParameterElement)(object)((element is ParameterElement) ? element : null);
				if (val != null)
				{
					text = ((Element)val).Name;
					InternalDefinition definition = val.GetDefinition();
					if (definition != null)
					{
						text2 = "Unknown";
					}
				}
				else if (elementId_0.Value < 0L)
				{
					string text3 = method_3(elementId_0, document_0);
					if (!string.IsNullOrEmpty(text3))
					{
						text = text3;
						Logger.Info("[GetViewFiltersTool] 成功获取内置参数名称: " + text3);
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
						defaultInterpolatedStringHandler.AppendLiteral("内置参数 (ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(elementId_0.Value);
						defaultInterpolatedStringHandler.AppendLiteral(")");
						text = defaultInterpolatedStringHandler.ToStringAndClear();
						Logger.Info("[GetViewFiltersTool] 参数 ID 为负数，可能是内置参数");
					}
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("Parameter ID ");
					defaultInterpolatedStringHandler2.AppendFormatted(elementId_0.Value);
					defaultInterpolatedStringHandler2.AppendLiteral(" (未找到对应元素)");
					text = defaultInterpolatedStringHandler2.ToStringAndClear();
				}
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("Parameter ID ");
				defaultInterpolatedStringHandler3.AppendFormatted(elementId_0.Value);
				text = defaultInterpolatedStringHandler3.ToStringAndClear();
				Logger.Warning("[GetViewFiltersTool] 获取参数信息时出错: " + ex.Message);
			}
			FilterInverseRule val2 = (FilterInverseRule)(object)((filterRule_0 is FilterInverseRule) ? filterRule_0 : null);
			if (val2 != null)
			{
				FilterRule innerRule = val2.GetInnerRule();
				object gparam_ = method_2(innerRule, elementId_0, document_0);
				return new Class204<string, bool, object>("InverseRule", gparam_4: true, gparam_);
			}
			FilterStringRule val3 = (FilterStringRule)(object)((filterRule_0 is FilterStringRule) ? filterRule_0 : null);
			if (val3 != null)
			{
				FilterStringRuleEvaluator evaluator = val3.GetEvaluator();
				return new Class205<string, string, string, string, string, bool>("StringRule", text, text2, method_4(evaluator), val3.RuleString, val3.RuleString.ToLower() != val3.RuleString);
			}
			FilterDoubleRule val4 = (FilterDoubleRule)(object)((filterRule_0 is FilterDoubleRule) ? filterRule_0 : null);
			if (val4 != null)
			{
				FilterNumericRuleEvaluator evaluator2 = ((FilterNumericValueRule)val4).GetEvaluator();
				return new Class206<string, string, string, string, double, double>("DoubleRule", text, text2, method_4(evaluator2), val4.RuleValue, val4.Epsilon);
			}
			FilterIntegerRule val5 = (FilterIntegerRule)(object)((filterRule_0 is FilterIntegerRule) ? filterRule_0 : null);
			if (val5 != null)
			{
				FilterNumericRuleEvaluator evaluator3 = ((FilterNumericValueRule)val5).GetEvaluator();
				return new Class207<string, string, string, string, int>("IntegerRule", text, text2, method_4(evaluator3), val5.RuleValue);
			}
			FilterElementIdRule val6 = (FilterElementIdRule)(object)((filterRule_0 is FilterElementIdRule) ? filterRule_0 : null);
			if (val6 != null)
			{
				FilterNumericRuleEvaluator evaluator4 = ((FilterNumericValueRule)val6).GetEvaluator();
				return new Class207<string, string, string, string, long>("ElementIdRule", text, text2, method_4(evaluator4), val6.RuleValue.Value);
			}
			return new Class208<string, string, string>("UnknownRule", text, text2);
		}
		catch (Exception ex2)
		{
			Logger.Warning("[GetViewFiltersTool] 解析规则详情时出错: " + ex2.Message);
			return new Class209<string, string>("Error", ex2.Message);
		}
	}

	private string? method_3(ElementId elementId_0, Document document_0)
	{
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (elementId_0.Value >= 0L)
		{
			return null;
		}
		try
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[GetViewFiltersTool] 尝试获取内置参数 ID ");
			defaultInterpolatedStringHandler.AppendFormatted(elementId_0.Value);
			defaultInterpolatedStringHandler.AppendLiteral(" 的名称");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			try
			{
				WallType val = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(WallType))).Cast<WallType>().FirstOrDefault();
				if (val != null)
				{
					BuiltInParameter val2 = (BuiltInParameter)elementId_0.Value;
					Parameter val3 = ((Element)val).get_Parameter(val2);
					if (val3 != null && val3.Definition != null)
					{
						string name = val3.Definition.Name;
						Logger.Info("[GetViewFiltersTool] ✓ 成功! 参数名称: " + name);
						return name;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Info("[GetViewFiltersTool] 方法1失败: " + ex.Message);
			}
			try
			{
				Element val4 = ((IEnumerable<Element>)new FilteredElementCollector(document_0).OfClass(typeof(Wall)).WhereElementIsNotElementType()).FirstOrDefault();
				if (val4 != null)
				{
					BuiltInParameter val5 = (BuiltInParameter)elementId_0.Value;
					Parameter val6 = val4.get_Parameter(val5);
					if (val6 != null && val6.Definition != null)
					{
						string name2 = val6.Definition.Name;
						Logger.Info("[GetViewFiltersTool] ✓ 成功! 参数名称: " + name2);
						return name2;
					}
				}
			}
			catch (Exception ex2)
			{
				Logger.Info("[GetViewFiltersTool] 方法2失败: " + ex2.Message);
			}
			try
			{
				Element val7 = ((IEnumerable<Element>)new FilteredElementCollector(document_0).OfClass(typeof(ProjectInfo))).FirstOrDefault();
				if (val7 != null)
				{
					BuiltInParameter val8 = (BuiltInParameter)elementId_0.Value;
					Parameter val9 = val7.get_Parameter(val8);
					if (val9 != null && val9.Definition != null)
					{
						string name3 = val9.Definition.Name;
						Logger.Info("[GetViewFiltersTool] ✓ 成功! 参数名称: " + name3);
						return name3;
					}
				}
			}
			catch (Exception ex3)
			{
				Logger.Info("[GetViewFiltersTool] 方法3失败: " + ex3.Message);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(39, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[GetViewFiltersTool] ✗ 无法获取内置参数 ID ");
			defaultInterpolatedStringHandler2.AppendFormatted(elementId_0.Value);
			defaultInterpolatedStringHandler2.AppendLiteral(" 的名称");
			Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
			return null;
		}
		catch (Exception ex4)
		{
			Logger.Warning("[GetViewFiltersTool] 获取内置参数名称时出错: " + ex4.Message);
			return null;
		}
	}

	private string method_4(object object_0)
	{
		if (object_0 == null)
		{
			return "Unknown";
		}
		Type type = object_0.GetType();
		string name = type.Name;
		if (name.Contains("Equals"))
		{
			return "equals";
		}
		if (name.Contains("Greater"))
		{
			return "greater";
		}
		if (name.Contains("Less"))
		{
			return "less";
		}
		if (name.Contains("Not"))
		{
			return "not";
		}
		return name;
	}
}
