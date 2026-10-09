using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("filter_elements", Category = "元素查询", Description = "对缓存中的元素或元素 ID 数组按类别、类型、楼层、参数值等条件进行过滤，结果存入新缓存。支持链式过滤操作。当用户要求'选择某些满足条件的构件'时，先用此工具过滤，再用 select_elements 选中。按参数值过滤时使用 customFilter，需先调用 query_parameter_info (operation='schema') 获取准确的参数名称。customFilter 格式示例: {\"parameterName\":\"宽度\",\"operator\":\">\",\"value\":1500,\"unit\":\"mm\"}。重要：必须指定 unit 字段（长度参数用 mm/m/cm/ft/in，面积用 m2/m2/ft2，体积用 m3/ft3，角度用 deg/rad），否则过滤结果可能不正确。", RequiresTransaction = false, RequiresModification = false)]
public sealed class FilterElementsTool : IAITool
{
	private class Class469
	{
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string string_0 = string.Empty;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string string_1 = "=";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private object? object_0;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string? string_2;

		public string ParameterName
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		public string Operator
		{
			[CompilerGenerated]
			get
			{
				return string_1;
			}
			[CompilerGenerated]
			set
			{
				string_1 = value;
			}
		}

		public object? Value
		{
			[CompilerGenerated]
			get
			{
				return object_0;
			}
			[CompilerGenerated]
			set
			{
				object_0 = value;
			}
		}

		public string? Unit
		{
			[CompilerGenerated]
			get
			{
				return string_2;
			}
			[CompilerGenerated]
			set
			{
				string_2 = value;
			}
		}
	}

	[CompilerGenerated]
	public sealed class Class470
	{
		public ILevelService ilevelService_0;

		public string string_0;

		public IElementService ielementService_0;

		internal bool method_0(object object_0)
		{
			string levelName = ilevelService_0.GetLevelName(object_0);
			return string.Equals(levelName, string_0, StringComparison.OrdinalIgnoreCase) || string.Equals(levelName, string_0, StringComparison.CurrentCultureIgnoreCase);
		}

		internal Class130<int?, string, string, string> method_1(object object_0)
		{
			return new Class130<int?, string, string, string>(ielementService_0.GetElementId(object_0), ielementService_0.GetElementName(object_0), ielementService_0.GetElementCategory(object_0), ielementService_0.GetElementTypeName(object_0));
		}
	}

	[CompilerGenerated]
	public sealed class Class471 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public FilterElementsTool filterElementsTool_0;

		private Class470 class470_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private List<int> list_0;

		private Class469 class469_0;

		private object object_0;

		private List<object> list_1;

		private Dictionary<string, int> dictionary_0;

		private List<Class130<int?, string, string, string>> list_2;

		private string string_3;

		private string string_4;

		private object object_1;

		private int[] int_1;

		private Exception exception_0;

		private IEnumerable<object> ienumerable_0;

		private List<int>.Enumerator enumerator_0;

		private int int_2;

		private object object_2;

		private bool bool_0;

		private string string_5;

		private int int_3;

		private string string_6;

		private int int_4;

		private int? nullable_0;

		private int? nullable_1;

		private int int_5;

		private int int_6;

		private object object_3;

		private int int_7;

		private Exception exception_1;

		private string string_7;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_0;

		private object object_4;

		private PropertyInfo propertyInfo_1;

		private string string_8;

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
					Class471 stateMachine = this;
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
				class470_0 = new Class470();
				Class470 @class = class470_0;
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				@class.ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				Class470 class2 = class470_0;
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				class2.ilevelService_0 = ((revitAdapter2 != null) ? revitAdapter2.LevelService : null);
				if (class470_0.ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else if (aitoolContext_0.DataCache == null)
				{
					result = AIToolResult.Fail("数据缓存服务不可用");
				}
				else
				{
					string_0 = aitoolContext_0.GetParameter<string>("categoryName", (string)null);
					string_1 = aitoolContext_0.GetParameter<string>("typeName", (string)null);
					class470_0.string_0 = aitoolContext_0.GetParameter<string>("levelName", (string)null);
					string_2 = aitoolContext_0.GetParameter<string>("customFilter", (string)null);
					list_0 = new List<int>();
					if (aitoolContext_0.HasParameter("cacheId"))
					{
						string_4 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
						if (string.IsNullOrEmpty(string_4))
						{
							result = AIToolResult.Fail("cacheId 参数不能为空");
						}
						else
						{
							object_1 = aitoolContext_0.DataCache.Retrieve<object>(string_4);
							if (object_1 == null)
							{
								result = AIToolResult.Fail("缓存 '" + string_4 + "' 不存在或已过期");
							}
							else
							{
								list_0 = filterElementsTool_0.method_0(object_1);
								if (list_0.Count != 0)
								{
									string_4 = null;
									object_1 = null;
									goto IL_02ff;
								}
								result = AIToolResult.Fail("缓存数据中未找到任何元素 ID");
							}
						}
					}
					else
					{
						if (aitoolContext_0.HasParameter("elementIds"))
						{
							int_1 = aitoolContext_0.GetParameter<int[]>("elementIds", (int[])null);
							if (int_1 != null && int_1.Length != 0)
							{
								list_0.AddRange(int_1);
							}
							int_1 = null;
							goto IL_02ff;
						}
						result = AIToolResult.Fail("请提供 cacheId 或 elementIds 参数");
					}
				}
				goto end_IL_0067;
				IL_02ff:
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("未找到任何元素");
				}
				else
				{
					class469_0 = null;
					if (!string.IsNullOrEmpty(string_2))
					{
						try
						{
							class469_0 = JsonSerializer.Deserialize<Class469>(string_2);
						}
						catch (Exception ex)
						{
							exception_0 = ex;
							result = AIToolResult.Fail("自定义过滤条件 JSON 格式错误: " + exception_0.Message);
							goto end_IL_0067;
						}
					}
					object_0 = null;
					if (string.IsNullOrEmpty(class470_0.string_0) || class470_0.ilevelService_0 == null)
					{
						goto IL_042f;
					}
					ienumerable_0 = class470_0.ilevelService_0.GetAllLevels(aitoolContext_0.Document);
					object_0 = ienumerable_0?.FirstOrDefault(delegate(object object_0)
					{
						string levelName = class470_0.ilevelService_0.GetLevelName(object_0);
						return string.Equals(levelName, class470_0.string_0, StringComparison.OrdinalIgnoreCase) || string.Equals(levelName, class470_0.string_0, StringComparison.CurrentCultureIgnoreCase);
					});
					if (object_0 != null)
					{
						ienumerable_0 = null;
						goto IL_042f;
					}
					result = AIToolResult.Fail("找不到楼层: " + class470_0.string_0);
				}
				goto end_IL_0067;
				IL_042f:
				list_1 = new List<object>();
				dictionary_0 = new Dictionary<string, int>();
				enumerator_0 = list_0.GetEnumerator();
				try
				{
					while (enumerator_0.MoveNext())
					{
						int_2 = enumerator_0.Current;
						object_2 = class470_0.ielementService_0.GetElementById(aitoolContext_0.Document, int_2);
						if (object_2 == null)
						{
							continue;
						}
						bool_0 = true;
						if (!string.IsNullOrEmpty(string_0))
						{
							string_5 = class470_0.ielementService_0.GetElementCategory(object_2);
							if (string_5 == null || !string.Equals(string_5, string_0, StringComparison.OrdinalIgnoreCase))
							{
								bool_0 = false;
								dictionary_0.TryGetValue("类别过滤", out int_3);
								dictionary_0["类别过滤"] = int_3 + 1;
							}
							string_5 = null;
						}
						if (bool_0 && !string.IsNullOrEmpty(string_1))
						{
							string_6 = class470_0.ielementService_0.GetElementTypeName(object_2);
							if (string_6 == null || !string.Equals(string_6, string_1, StringComparison.OrdinalIgnoreCase))
							{
								bool_0 = false;
								dictionary_0.TryGetValue("类型过滤", out int_4);
								dictionary_0["类型过滤"] = int_4 + 1;
							}
							string_6 = null;
						}
						if (bool_0 && object_0 != null)
						{
							try
							{
								FilterElementsTool filterElementsTool = filterElementsTool_0;
								object obj = object_2;
								IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
								nullable_0 = filterElementsTool.method_6(obj, (revitAdapter3 != null) ? revitAdapter3.ParameterService : null);
								if (!nullable_0.HasValue)
								{
									bool_0 = false;
								}
								else
								{
									ILevelService obj2 = class470_0.ilevelService_0;
									nullable_1 = ((obj2 != null) ? obj2.GetLevelId(object_0) : ((int?)null));
									if (!nullable_1.HasValue || nullable_0 != nullable_1.Value)
									{
										bool_0 = false;
										dictionary_0.TryGetValue("楼层过滤", out int_5);
										dictionary_0["楼层过滤"] = int_5 + 1;
									}
								}
							}
							catch
							{
								bool_0 = false;
								dictionary_0.TryGetValue("楼层过滤", out int_6);
								dictionary_0["楼层过滤"] = int_6 + 1;
							}
						}
						if (bool_0 && class469_0 != null)
						{
							try
							{
								object_3 = class470_0.ielementService_0.GetParameterValue(object_2, class469_0.ParameterName);
								FilterElementsTool filterElementsTool2 = filterElementsTool_0;
								object obj4 = object_3;
								Class469 class3 = class469_0;
								AIToolContext obj5 = aitoolContext_0;
								object obj6 = object_2;
								IRevitAdapter revitAdapter4 = aitoolContext_0.RevitAdapter;
								if (!filterElementsTool2.method_2(obj4, class3, obj5, obj6, (revitAdapter4 != null) ? revitAdapter4.ParameterService : null))
								{
									bool_0 = false;
									dictionary_0.TryGetValue("自定义过滤", out int_7);
									dictionary_0["自定义过滤"] = int_7 + 1;
								}
								object_3 = null;
							}
							catch (Exception ex)
							{
								exception_1 = ex;
								Logger.Debug("[filter_elements] 自定义过滤异常: " + exception_1.Message);
							}
						}
						if (bool_0)
						{
							list_1.Add(object_2);
						}
						object_2 = null;
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
					}
				}
				enumerator_0 = default(List<int>.Enumerator);
				if (list_1.Count == 0)
				{
					result = AIToolResult.Fail("过滤后没有匹配的元素。" + filterElementsTool_0.method_5(dictionary_0));
				}
				else
				{
					list_2 = list_1.Select((object object_0) => new Class130<int?, string, string, string>(class470_0.ielementService_0.GetElementId(object_0), class470_0.ielementService_0.GetElementName(object_0), class470_0.ielementService_0.GetElementCategory(object_0), class470_0.ielementService_0.GetElementTypeName(object_0))).ToList();
					string_3 = filterElementsTool_0.method_4(string_0, string_1, class470_0.string_0, class469_0 != null);
					if (aitoolContext_0.DataCache != null && !string.IsNullOrEmpty(aitoolContext_0.SessionId))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
						defaultInterpolatedStringHandler.AppendLiteral("filter_elements_");
						defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now, "yyyyMMddHHmmss");
						string_7 = defaultInterpolatedStringHandler.ToStringAndClear();
						aitoolResult_0 = AIToolResult.OkWithSmartSummary<Class130<int?, string, string, string>>((IEnumerable<Class130<int?, string, string, string>>)list_2, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_7, "个元素", 100);
						if (aitoolResult_0.Data != null)
						{
							try
							{
								propertyInfo_0 = aitoolResult_0.Data.GetType().GetProperty("cache_info");
								if (propertyInfo_0 != null)
								{
									object_4 = propertyInfo_0.GetValue(aitoolResult_0.Data);
									if (object_4 != null)
									{
										propertyInfo_1 = object_4.GetType().GetProperty("cache_id");
										if (propertyInfo_1 != null)
										{
											string_8 = propertyInfo_1.GetValue(object_4)?.ToString();
											if (!string.IsNullOrEmpty(string_8))
											{
												AIToolResult obj7 = aitoolResult_0;
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(57, 4);
												defaultInterpolatedStringHandler2.AppendLiteral("✅ 过滤完成：从 ");
												defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
												defaultInterpolatedStringHandler2.AppendLiteral(" 个元素中筛选出 ");
												defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
												defaultInterpolatedStringHandler2.AppendLiteral(" 个。");
												defaultInterpolatedStringHandler2.AppendFormatted(string_3);
												defaultInterpolatedStringHandler2.AppendLiteral("\n\n💡 在后续工具调用中使用 cacheId=\"");
												defaultInterpolatedStringHandler2.AppendFormatted(string_8);
												defaultInterpolatedStringHandler2.AppendLiteral("\" 参数来操作这些元素");
												obj7.Message = defaultInterpolatedStringHandler2.ToStringAndClear();
											}
											string_8 = null;
										}
										propertyInfo_1 = null;
									}
									object_4 = null;
								}
								propertyInfo_0 = null;
							}
							catch
							{
							}
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(32, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("[filter_elements] 过滤完成: ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" -> ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个元素");
						Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
						result = aitoolResult_0;
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(21, 3);
						defaultInterpolatedStringHandler4.AppendLiteral("✅ 过滤完成：从 ");
						defaultInterpolatedStringHandler4.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler4.AppendLiteral(" 个元素中筛选出 ");
						defaultInterpolatedStringHandler4.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler4.AppendLiteral(" 个。");
						defaultInterpolatedStringHandler4.AppendFormatted(string_3);
						result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class131<int, int, Dictionary<string, int>, List<Class130<int?, string, string, string>>>(list_0.Count, list_1.Count, dictionary_0, list_2));
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_2 = ex;
				Logger.Error("[filter_elements] 过滤失败", exception_2);
				result = AIToolResult.Fail("过滤失败: " + exception_2.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "filter_elements";

	public string Category => "元素查询";

	public string Description => "对缓存中的元素或元素 ID 数组按条件过滤。customFilter 格式: {\"parameterName\":\"参数名\",\"operator\":\">\",\"value\":1000}";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）。要过滤的缓存数据。与 elementIds 二选一。\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"要过滤的元素 ID 数组（可选）。与 cacheId 二选一。\"\n            },\n            \"categoryName\": {\n                \"type\": \"string\",\n                \"description\": \"类别名称过滤（可选，中文）。如：墙、门、窗、楼板、柱、梁等。\"\n            },\n            \"typeName\": {\n                \"type\": \"string\",\n                \"description\": \"类型名称过滤（可选）。如：基本墙、幕墙、栏杆等。\"\n            },\n            \"levelName\": {\n                \"type\": \"string\",\n                \"description\": \"楼层名称过滤（可选）。如：一层、二层、Level 1、Level 2 等。\"\n            },\n            \"customFilter\": {\n                \"type\": \"string\",\n                \"description\": \"自定义过滤条件（可选）。JSON 格式字符串。必须指定 unit 字段！格式: {\\\"parameterName\\\":\\\"参数名\\\",\\\"operator\\\":\\\">\\\",\\\"value\\\":数值,\\\"unit\\\":\\\"单位\\\"}。单位选项：长度参数用 mm/cm/m/ft/in，面积用 m2/ft2，体积用 m3/ft3，角度用 deg/rad。\"\n            }\n        },\n        \"required\": []\n    }";

	[AsyncStateMachine(typeof(Class471))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class471 stateMachine = new Class471();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.filterElementsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<int> method_0(object object_0)
	{
		List<int> list = new List<int>();
		try
		{
			if (object_0 is IList list2)
			{
				foreach (object item in list2)
				{
					if (item != null)
					{
						int? num = method_1(item);
						if (num.HasValue && num.Value > 0)
						{
							list.Add(num.Value);
						}
					}
				}
			}
		}
		catch
		{
			list.Clear();
		}
		return list;
	}

	private int? method_1(object object_0)
	{
		try
		{
			if (object_0 == null)
			{
				return null;
			}
			Type type = object_0.GetType();
			string[] array = new string[9]
			{
				"id",
				"elementId",
				"column_id",
				"wall_id",
				"floor_id",
				"beam_id",
				"brace_id",
				"Id",
				"ElementId"
			};
			string[] array2 = array;
			foreach (string name in array2)
			{
				PropertyInfo property = type.GetProperty(name);
				if (property != null)
				{
					object value = property.GetValue(object_0);
					if (value is int num && num > 0)
					{
						return num;
					}
					if (value is long num2 && num2 > 0L)
					{
						return (int)num2;
					}
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private bool method_2(object? object_0, Class469 class469_0, AIToolContext? aitoolContext_0 = null, object? object_1 = null, IParameterService? iparameterService_0 = null)
	{
		if (object_0 == null)
		{
			return false;
		}
		try
		{
			double num;
			if (double.TryParse(object_0.ToString(), out var result) && double.TryParse(class469_0.Value?.ToString(), out var result2))
			{
				if (string.IsNullOrEmpty(class469_0.Unit))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[filter_elements] customFilter 必须指定 unit 字段。参数: ");
					defaultInterpolatedStringHandler.AppendFormatted(class469_0.ParameterName);
					defaultInterpolatedStringHandler.AppendLiteral(", 值: ");
					defaultInterpolatedStringHandler.AppendFormatted<object>(class469_0.Value);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					return false;
				}
				num = method_3(result2, class469_0.Unit, aitoolContext_0);
				string text = class469_0.Operator;
				uint num2 = Class653.smethod_0(text);
				if (num2 <= 1142581827)
				{
					if (num2 <= 796199151)
					{
						if (num2 <= 284975636)
						{
							if (num2 != 138129653)
							{
								if (num2 == 284975636 && text == ">=")
								{
									goto IL_0393;
								}
							}
							else if (text == "not_equal")
							{
								goto IL_03f6;
							}
						}
						else if (num2 != 560682936)
						{
							if (num2 == 796199151 && text == "equal")
							{
								goto IL_0208;
							}
						}
						else if (text == "less")
						{
							goto IL_0357;
						}
					}
					else if (num2 <= 957132539)
					{
						if (num2 != 940354920)
						{
							if (num2 == 957132539 && text == "<")
							{
								goto IL_0357;
							}
						}
						else if (text == "=")
						{
							goto IL_0208;
						}
					}
					else if (num2 != 990687777)
					{
						if (num2 != 1008758233)
						{
							if (num2 == 1142581827 && text == "eq")
							{
								goto IL_0208;
							}
						}
						else if (text == "ge")
						{
							goto IL_0393;
						}
					}
					else if (text == ">")
					{
						goto IL_0300;
					}
				}
				else if (num2 <= 1479958588)
				{
					if (num2 <= 1278332970)
					{
						if (num2 != 1260422518)
						{
							if (num2 == 1278332970 && text == "le")
							{
								goto IL_03b2;
							}
						}
						else if (text == "gt")
						{
							goto IL_0300;
						}
					}
					else if (num2 != 1355287449)
					{
						if (num2 == 1479958588 && text == "ne")
						{
							goto IL_03f6;
						}
					}
					else if (text == "greater")
					{
						goto IL_0300;
					}
				}
				else if (num2 <= 2017020315)
				{
					if (num2 != 1563552493)
					{
						if (num2 == 2017020315 && text == "less_or_equal")
						{
							goto IL_03b2;
						}
					}
					else if (text == "lt")
					{
						goto IL_0357;
					}
				}
				else if (num2 != 2428715011u)
				{
					if (num2 != 2499223986u)
					{
						if (num2 == 2621950312u && text == "greater_or_equal")
						{
							goto IL_0393;
						}
					}
					else if (text == "<=")
					{
						goto IL_03b2;
					}
				}
				else if (text == "!=")
				{
					goto IL_03f6;
				}
				throw new ArgumentException("不支持的运算符: " + class469_0.Operator + "，支持的运算符: >, >=, <, <=, =, !=");
			}
			string text2 = object_0.ToString() ?? "";
			object? value = class469_0.Value;
			object obj;
			if (value == null)
			{
				obj = null;
			}
			else
			{
				obj = value.ToString();
				if (obj != null)
				{
					goto IL_044d;
				}
			}
			obj = "";
			goto IL_044d;
			IL_0208:
			bool result3 = Math.Abs(result - num) < 0.0001;
			goto IL_040f;
			IL_0393:
			result3 = result >= num;
			goto IL_040f;
			IL_0357:
			result3 = result < num;
			goto IL_040f;
			IL_044d:
			string text3 = (string)obj;
			string text4 = class469_0.Operator;
			return (text4 == "=") ? string.Equals(text2, text3, StringComparison.OrdinalIgnoreCase) : ((text4 == "!=") ? (!string.Equals(text2, text3, StringComparison.OrdinalIgnoreCase)) : (!(text4 == "contains") || text2.IndexOf(text3, StringComparison.OrdinalIgnoreCase) >= 0));
			IL_0300:
			result3 = result > num;
			goto IL_040f;
			IL_03b2:
			result3 = result <= num;
			goto IL_040f;
			IL_040f:
			return result3;
			IL_03f6:
			result3 = Math.Abs(result - num) >= 0.0001;
			goto IL_040f;
		}
		catch (Exception ex)
		{
			Logger.Error("[filter_elements] 自定义过滤评估失败: " + ex.Message);
			return false;
		}
	}

	private double method_3(double double_0, string string_0, AIToolContext? aitoolContext_0)
	{
		uint num;
		double result;
		if (((aitoolContext_0 != null) ? aitoolContext_0.UnitService : null) != null)
		{
			string text = string_0.ToLowerInvariant();
			num = Class653.smethod_0(text);
			if (num <= 1613635087)
			{
				if (num <= 655135397)
				{
					if (num <= 372194449)
					{
						if (num != 355416830)
						{
							if (num == 372194449 && text == "m³")
							{
								goto IL_02b8;
							}
						}
						else if (text == "m²")
						{
							goto IL_0285;
						}
					}
					else if (num != 571247302)
					{
						if (num == 655135397 && text == "\"")
						{
							goto IL_011d;
						}
					}
					else if (text == "'")
					{
						goto IL_017f;
					}
				}
				else if (num <= 1358899048)
				{
					if (num != 1094220446)
					{
						if (num == 1358899048 && text == "rad")
						{
							result = double_0;
							goto IL_02ea;
						}
					}
					else if (text == "in")
					{
						goto IL_011d;
					}
				}
				else if (num != 1495456279)
				{
					if (num == 1613635087 && text == "mm")
					{
						result = aitoolContext_0.UnitService.MmToInternal(double_0);
						goto IL_02ea;
					}
				}
				else if (text == "ft")
				{
					goto IL_017f;
				}
			}
			else if (num <= 3037454127u)
			{
				if (num <= 2502848894u)
				{
					if (num != 1680451373)
					{
						if (num == 2502848894u && text == "m2")
						{
							goto IL_0285;
						}
					}
					else if (text == "cm")
					{
						result = aitoolContext_0.UnitService.CmToInternal(double_0);
						goto IL_02ea;
					}
				}
				else if (num != 2519626513u)
				{
					if (num == 3037454127u && text == "°")
					{
						goto IL_0264;
					}
				}
				else if (text == "m3")
				{
					goto IL_02b8;
				}
			}
			else if (num <= 3327754271u)
			{
				if (num != 3175419372u)
				{
					if (num == 3327754271u && text == "deg")
					{
						goto IL_0264;
					}
				}
				else if (text == "sqm")
				{
					goto IL_0285;
				}
			}
			else if (num != 3893112696u)
			{
				if (num == 3981844376u && text == "cum")
				{
					goto IL_02b8;
				}
			}
			else if (text == "m")
			{
				result = aitoolContext_0.UnitService.MetersToInternal(double_0);
				goto IL_02ea;
			}
			result = double_0;
			goto IL_02ea;
		}
		string text2 = string_0.ToLowerInvariant();
		num = Class653.smethod_0(text2);
		if (num <= 1680451373)
		{
			if (num <= 655135397)
			{
				if (num != 355416830)
				{
					if (num != 372194449)
					{
						if (num == 655135397 && text2 == "\"")
						{
							goto IL_0403;
						}
					}
					else if (text2 == "m³")
					{
						goto IL_0512;
					}
				}
				else if (text2 == "m²")
				{
					goto IL_04df;
				}
			}
			else
			{
				if (num != 1094220446)
				{
					if (num != 1613635087)
					{
						if (num != 1680451373 || !(text2 == "cm"))
						{
							goto IL_0533;
						}
						result = double_0 / 30.48;
					}
					else
					{
						if (!(text2 == "mm"))
						{
							goto IL_0533;
						}
						result = double_0 / 304.8;
					}
					goto IL_0543;
				}
				if (text2 == "in")
				{
					goto IL_0403;
				}
			}
		}
		else if (num <= 3037454127u)
		{
			if (num != 2502848894u)
			{
				if (num != 2519626513u)
				{
					if (num == 3037454127u && text2 == "°")
					{
						goto IL_04b4;
					}
				}
				else if (text2 == "m3")
				{
					goto IL_0512;
				}
			}
			else if (text2 == "m2")
			{
				goto IL_04df;
			}
		}
		else if (num <= 3327754271u)
		{
			if (num != 3175419372u)
			{
				if (num == 3327754271u && text2 == "deg")
				{
					goto IL_04b4;
				}
			}
			else if (text2 == "sqm")
			{
				goto IL_04df;
			}
		}
		else if (num != 3893112696u)
		{
			if (num == 3981844376u && text2 == "cum")
			{
				goto IL_0512;
			}
		}
		else if (text2 == "m")
		{
			result = double_0 / 0.3048;
			goto IL_0543;
		}
		goto IL_0533;
		IL_04b4:
		result = double_0 * Math.PI / 180.0;
		goto IL_0543;
		IL_0512:
		result = double_0 * 35.3147;
		goto IL_0543;
		IL_0285:
		result = aitoolContext_0.UnitService.SquareMetersToInternal(double_0);
		goto IL_02ea;
		IL_0403:
		result = double_0 / 12.0;
		goto IL_0543;
		IL_017f:
		result = double_0;
		goto IL_02ea;
		IL_04df:
		result = double_0 * 10.7639;
		goto IL_0543;
		IL_0264:
		result = aitoolContext_0.UnitService.DegreesToRadians(double_0);
		goto IL_02ea;
		IL_02b8:
		result = aitoolContext_0.UnitService.CubicMetersToInternal(double_0);
		goto IL_02ea;
		IL_02ea:
		return result;
		IL_0533:
		result = double_0;
		goto IL_0543;
		IL_0543:
		return result;
		IL_011d:
		result = double_0 / 12.0;
		goto IL_02ea;
	}

	private string method_4(string? string_0, string? string_1, string? string_2, bool bool_0)
	{
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(string_0))
		{
			list.Add("类别=" + string_0);
		}
		if (!string.IsNullOrEmpty(string_1))
		{
			list.Add("类型=" + string_1);
		}
		if (!string.IsNullOrEmpty(string_2))
		{
			list.Add("楼层=" + string_2);
		}
		if (bool_0)
		{
			list.Add("自定义条件");
		}
		return (list.Count > 0) ? ("过滤条件: " + string.Join(", ", list)) : "无过滤条件";
	}

	private string method_5(Dictionary<string, int> dictionary_0)
	{
		if (dictionary_0.Count == 0)
		{
			return "";
		}
		IEnumerable<string> values = dictionary_0.Select<KeyValuePair<string, int>, string>(delegate(KeyValuePair<string, int> keyValuePair_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
			defaultInterpolatedStringHandler.AppendFormatted(keyValuePair_0.Key);
			defaultInterpolatedStringHandler.AppendLiteral("排除");
			defaultInterpolatedStringHandler.AppendFormatted(keyValuePair_0.Value);
			defaultInterpolatedStringHandler.AppendLiteral("个");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		});
		return " (" + string.Join(", ", values) + ")";
	}

	private int? method_6(object object_0, IParameterService? iparameterService_0)
	{
		try
		{
			if (object_0 == null)
			{
				return null;
			}
			Type type = object_0.GetType();
			PropertyInfo property = type.GetProperty("LevelId");
			if (property != null)
			{
				object value = property.GetValue(object_0);
				if (value != null)
				{
					if (value is int value2)
					{
						return value2;
					}
					if (value is long num)
					{
						return (int)num;
					}
					Type type2 = value.GetType();
					PropertyInfo propertyInfo = type2.GetProperty("IntegerValue") ?? type2.GetProperty("Value");
					if (propertyInfo != null)
					{
						object value3 = propertyInfo.GetValue(value);
						if (value3 is int value4)
						{
							return value4;
						}
						if (value3 is long num2)
						{
							return (int)num2;
						}
					}
				}
			}
			if (iparameterService_0 != null)
			{
				object parameter = iparameterService_0.GetParameter(object_0, "标高");
				if (parameter != null)
				{
					int? parameterValueAsInteger = iparameterService_0.GetParameterValueAsInteger(parameter);
					if (parameterValueAsInteger.HasValue)
					{
						return parameterValueAsInteger.Value;
					}
				}
			}
			return null;
		}
		catch
		{
			return null;
		}
	}
}
