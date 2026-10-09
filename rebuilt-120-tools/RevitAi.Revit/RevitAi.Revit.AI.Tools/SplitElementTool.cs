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
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.AI.Tools;

[AITool("split_element", Category = "修改", Description = "拆分模型元素为多段。支持管道、风管（自动补齐管件）、斜柱（两点放置）、结构框架/梁。垂直柱（基于标高放置）和墙不支持拆分。支持三种模式：byLength按等长拆分（每段指定长度），byPoints按绝对坐标点拆分，byParameters按曲线归一化参数0-1拆分。支持单元素、多元素批量、缓存ID三种输入方式。单位均为毫米。", RequiresTransaction = true, RequiresModification = true)]
public sealed class SplitElementTool : IAITool
{
	[CompilerGenerated]
	private static class Class349
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class350 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SplitElementTool splitElementTool_0;

		private IElementService ielementService_0;

		private IModificationService imodificationService_0;

		private object object_0;

		private List<int> list_0;

		private string string_0;

		private bool bool_0;

		private IEnumerable<(double X, double Y, double Z)> ienumerable_0;

		private double double_0;

		private IEnumerable<double> ienumerable_1;

		private string string_1;

		private int int_1;

		private int int_2;

		private int int_3;

		private List<int> list_1;

		private List<int> list_2;

		private List<string> list_3;

		private List<string> list_4;

		private List<object> list_5;

		private string string_2;

		private double double_1;

		private List<int>.Enumerator enumerator_0;

		private int int_4;

		private object object_1;

		private string string_3;

		private SplitResult splitResult_0;

		private Exception exception_0;

		private string string_4;

		private string string_5;

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
					Class350 stateMachine = this;
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
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				imodificationService_0 = ((revitAdapter2 != null) ? revitAdapter2.ModificationService : null);
				if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (imodificationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ModificationService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					object_0 = aitoolContext_0.Document;
					list_0 = splitElementTool_0.method_0(aitoolContext_0, ielementService_0, "elementId", "elementIds", "elementCacheId");
					if (list_0 == null || list_0.Count == 0)
					{
						result = AIToolResult.Fail("未找到要拆分的元素。请提供 elementId（单个）、elementIds（数组）或 elementCacheId（缓存）参数");
					}
					else
					{
						string_0 = aitoolContext_0.GetParameter<string>("splitMode", (string)null);
						if (string.IsNullOrWhiteSpace(string_0))
						{
							result = AIToolResult.Fail("必须指定 splitMode 参数（byLength / byPoints / byParameters）");
						}
						else
						{
							bool_0 = aitoolContext_0.GetParameter<bool>("autoCreateFittings", true);
							ienumerable_0 = null;
							double_0 = 0.0;
							ienumerable_1 = null;
							string_1 = string_0.Trim();
							if (string_1.Equals("byLength", StringComparison.OrdinalIgnoreCase))
							{
								if (!aitoolContext_0.HasParameter("length"))
								{
									result = AIToolResult.Fail("byLength 模式必须提供 length 参数（毫米）");
								}
								else
								{
									double_1 = aitoolContext_0.GetParameter<double>("length", 0.0);
									if (!(double_1 <= 0.0))
									{
										double_0 = smethod_0(aitoolContext_0, double_1);
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
										defaultInterpolatedStringHandler.AppendLiteral("[SplitElementTool] byLength 模式：每段 ");
										defaultInterpolatedStringHandler.AppendFormatted(double_1);
										defaultInterpolatedStringHandler.AppendLiteral(" 毫米 (");
										defaultInterpolatedStringHandler.AppendFormatted(double_0, "F4");
										defaultInterpolatedStringHandler.AppendLiteral(" 英尺)");
										Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
										goto IL_04ca;
									}
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 1);
									defaultInterpolatedStringHandler2.AppendLiteral("length 必须大于 0，当前值：");
									defaultInterpolatedStringHandler2.AppendFormatted(double_1);
									result = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
								}
							}
							else if (string_1.Equals("byPoints", StringComparison.OrdinalIgnoreCase))
							{
								if (!aitoolContext_0.HasParameter("points"))
								{
									result = AIToolResult.Fail("byPoints 模式必须提供 points 参数（坐标点列表）");
								}
								else
								{
									ienumerable_0 = smethod_1(aitoolContext_0, "points");
									if (ienumerable_0 != null && ienumerable_0.Any())
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(36, 1);
										defaultInterpolatedStringHandler3.AppendLiteral("[SplitElementTool] byPoints 模式：");
										defaultInterpolatedStringHandler3.AppendFormatted(ienumerable_0.Count());
										defaultInterpolatedStringHandler3.AppendLiteral(" 个拆分点");
										Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
										goto IL_04ca;
									}
									result = AIToolResult.Fail("points 参数解析失败或为空");
								}
							}
							else if (string_1.Equals("byParameters", StringComparison.OrdinalIgnoreCase))
							{
								if (!aitoolContext_0.HasParameter("parameters"))
								{
									result = AIToolResult.Fail("byParameters 模式必须提供 parameters 参数（0-1 的归一化列表）");
								}
								else
								{
									ienumerable_1 = smethod_2(aitoolContext_0, "parameters");
									if (ienumerable_1 != null && ienumerable_1.Any())
									{
										goto IL_04ca;
									}
									result = AIToolResult.Fail("parameters 参数解析失败或为空");
								}
							}
							else
							{
								result = AIToolResult.Fail("不支持的 splitMode：" + string_0 + "，请使用 byLength / byPoints / byParameters");
							}
						}
					}
				}
				goto end_IL_0067;
				IL_04ca:
				int_1 = 0;
				int_2 = 0;
				int_3 = 0;
				list_1 = new List<int>();
				list_2 = new List<int>();
				list_3 = new List<string>();
				list_4 = new List<string>();
				list_5 = new List<object>();
				enumerator_0 = list_0.GetEnumerator();
				try
				{
					while (enumerator_0.MoveNext())
					{
						int_4 = enumerator_0.Current;
						if (!cancellationToken_0.IsCancellationRequested)
						{
							object_1 = ielementService_0.GetElementById(object_0, int_4);
							if (object_1 == null)
							{
								int_3++;
								List<string> list = list_4;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(7, 1);
								defaultInterpolatedStringHandler4.AppendLiteral("元素 ");
								defaultInterpolatedStringHandler4.AppendFormatted(int_4);
								defaultInterpolatedStringHandler4.AppendLiteral("：找不到");
								list.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
								list_5.Add(new Class301<int, string, string>(int_4, "失败", "找不到元素"));
								continue;
							}
							string_3 = ielementService_0.GetElementCategory(object_1);
							try
							{
								splitResult_0 = imodificationService_0.SplitElementAdvanced(object_0, object_1, string_1, ienumerable_0, double_0, ienumerable_1, bool_0);
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								int_3++;
								List<string> list2 = list_4;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(4, 2);
								defaultInterpolatedStringHandler5.AppendLiteral("元素 ");
								defaultInterpolatedStringHandler5.AppendFormatted(int_4);
								defaultInterpolatedStringHandler5.AppendLiteral("：");
								defaultInterpolatedStringHandler5.AppendFormatted(exception_0.Message);
								list2.Add(defaultInterpolatedStringHandler5.ToStringAndClear());
								list_5.Add(new Class302<int, string, string, string>(int_4, string_3, "失败", exception_0.Message));
								continue;
							}
							if (splitResult_0.Success)
							{
								int_1++;
								list_1.AddRange(splitResult_0.NewElementIds);
								list_2.AddRange(splitResult_0.CreatedFittingIds);
								list_5.Add(new Class303<int, string, string, int, List<int>, List<int>>(int_4, string_3, "成功", splitResult_0.SegmentCount, splitResult_0.NewElementIds.ToList(), splitResult_0.CreatedFittingIds.ToList()));
							}
							else if (!string.IsNullOrEmpty(splitResult_0.SkippedReason))
							{
								int_2++;
								List<string> list3 = list_3;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(6, 3);
								defaultInterpolatedStringHandler6.AppendLiteral("元素 ");
								defaultInterpolatedStringHandler6.AppendFormatted(int_4);
								defaultInterpolatedStringHandler6.AppendLiteral("（");
								defaultInterpolatedStringHandler6.AppendFormatted(string_3);
								defaultInterpolatedStringHandler6.AppendLiteral("）：");
								defaultInterpolatedStringHandler6.AppendFormatted(splitResult_0.SkippedReason);
								list3.Add(defaultInterpolatedStringHandler6.ToStringAndClear());
								list_5.Add(new Class304<int, string, string, string>(int_4, string_3, "跳过", splitResult_0.SkippedReason));
							}
							else
							{
								int_3++;
								string_4 = splitResult_0.Error ?? "未知错误";
								List<string> list4 = list_4;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(6, 3);
								defaultInterpolatedStringHandler7.AppendLiteral("元素 ");
								defaultInterpolatedStringHandler7.AppendFormatted(int_4);
								defaultInterpolatedStringHandler7.AppendLiteral("（");
								defaultInterpolatedStringHandler7.AppendFormatted(string_3);
								defaultInterpolatedStringHandler7.AppendLiteral("）：");
								defaultInterpolatedStringHandler7.AppendFormatted(string_4);
								list4.Add(defaultInterpolatedStringHandler7.ToStringAndClear());
								list_5.Add(new Class302<int, string, string, string>(int_4, string_3, "失败", string_4));
								string_4 = null;
							}
							object_1 = null;
							string_3 = null;
							splitResult_0 = null;
							continue;
						}
						result = AIToolResult.Fail("操作已取消");
						goto end_IL_0067;
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler8.AppendLiteral("拆分完成：成功 ");
				defaultInterpolatedStringHandler8.AppendFormatted(int_1);
				defaultInterpolatedStringHandler8.AppendLiteral(" 个");
				string_2 = defaultInterpolatedStringHandler8.ToStringAndClear();
				if (int_2 > 0)
				{
					string text = string_2;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(13, 1);
					defaultInterpolatedStringHandler9.AppendLiteral("，跳过 ");
					defaultInterpolatedStringHandler9.AppendFormatted(int_2);
					defaultInterpolatedStringHandler9.AppendLiteral(" 个（不支持拆分）");
					string_2 = text + defaultInterpolatedStringHandler9.ToStringAndClear();
				}
				if (int_3 > 0)
				{
					string text2 = string_2;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler10.AppendLiteral("，失败 ");
					defaultInterpolatedStringHandler10.AppendFormatted(int_3);
					defaultInterpolatedStringHandler10.AppendLiteral(" 个");
					string_2 = text2 + defaultInterpolatedStringHandler10.ToStringAndClear();
				}
				if (list_2.Count > 0)
				{
					string text3 = string_2;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(9, 1);
					defaultInterpolatedStringHandler11.AppendLiteral("；共创建管件 ");
					defaultInterpolatedStringHandler11.AppendFormatted(list_2.Count);
					defaultInterpolatedStringHandler11.AppendLiteral(" 个");
					string_2 = text3 + defaultInterpolatedStringHandler11.ToStringAndClear();
				}
				if (list_3.Count > 0)
				{
					string_5 = string.Join("; ", list_3.Take(3));
					if (list_3.Count > 3)
					{
						string text4 = string_5;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler12 = new DefaultInterpolatedStringHandler(5, 1);
						defaultInterpolatedStringHandler12.AppendLiteral(" 等 ");
						defaultInterpolatedStringHandler12.AppendFormatted(list_3.Count);
						defaultInterpolatedStringHandler12.AppendLiteral(" 条");
						string_5 = text4 + defaultInterpolatedStringHandler12.ToStringAndClear();
					}
					string_2 = string_2 + "。跳过原因：" + string_5;
					string_5 = null;
				}
				result = AIToolResult.Ok(string_2, (object)new Class305<int, int, int, int, string, List<int>, List<int>, List<string>, List<string>, List<object>>(list_0.Count, int_1, int_2, int_3, string_1, list_1, list_2, list_3, list_4, list_5));
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_1 = ex;
				Logger.Error("[SplitElementTool] 执行失败: " + exception_1.Message);
				result = AIToolResult.Fail("拆分操作失败: " + exception_1.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "split_element";

	public string Category => "修改";

	public string Description => "拆分模型元素（管道/风管/斜柱/梁）为多段，支持按等长、按坐标点、按曲线参数三种模式。垂直柱和墙不支持。";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"要拆分的单个元素 ID\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"要拆分的元素 ID 列表（批量操作时使用）\"\n            },\n            \"elementCacheId\": {\n                \"type\": \"string\",\n                \"description\": \"要拆分的元素的缓存 ID（可选）。从上一个查询工具的返回结果中获取 cache_id 字段\"\n            },\n            \"splitMode\": {\n                \"type\": \"string\",\n                \"enum\": [\"byLength\", \"byPoints\", \"byParameters\"],\n                \"description\": \"拆分模式：byLength=按等长拆分（每段指定长度）; byPoints=按绝对坐标点拆分; byParameters=按曲线归一化参数0-1拆分\"\n            },\n            \"length\": {\n                \"type\": \"number\",\n                \"description\": \"byLength 模式：每段长度（毫米）。例如 3000 表示每段 3 米。多元素时每个元素独立按此长度拆分\",\n                \"exclusiveMinimum\": 0\n            },\n            \"points\": {\n                \"type\": \"array\",\n                \"description\": \"byPoints 模式：拆分点坐标列表（毫米）。每个元素为 {x,y,z}。这些拆分点将作用于所有输入的元素\",\n                \"items\": {\n                    \"type\": \"object\",\n                    \"properties\": {\n                        \"x\": { \"type\": \"number\", \"description\": \"X 坐标（毫米）\" },\n                        \"y\": { \"type\": \"number\", \"description\": \"Y 坐标（毫米）\" },\n                        \"z\": { \"type\": \"number\", \"description\": \"Z 坐标（毫米）\" }\n                    },\n                    \"required\": [\"x\", \"y\", \"z\"]\n                }\n            },\n            \"parameters\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"number\", \"minimum\": 0, \"maximum\": 1 },\n                \"description\": \"byParameters 模式：曲线归一化位置 0-1 列表，如 [0.33, 0.67] 表示在 1/3 和 2/3 处拆分\"\n            },\n            \"autoCreateFittings\": {\n                \"type\": \"boolean\",\n                \"default\": true,\n                \"description\": \"管道/风管拆分时是否自动创建管件（管箍/法兰），默认 true。设置为 false 则只切断不补齐管件\"\n            }\n        },\n        \"required\": [\"splitMode\"]\n    }";

	[AsyncStateMachine(typeof(Class350))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class350 stateMachine = new Class350();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.splitElementTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private static double smethod_0(AIToolContext aitoolContext_0, double double_0)
	{
		if (aitoolContext_0.UnitService != null)
		{
			return aitoolContext_0.UnitService.MmToInternal(double_0);
		}
		return double_0 / 304.8;
	}

	private static List<(double X, double Y, double Z)>? smethod_1(AIToolContext aitoolContext_0, string string_0)
	{
		try
		{
			object parameter = aitoolContext_0.GetParameter<object>(string_0, (object)null);
			if (parameter == null)
			{
				return null;
			}
			List<(double, double, double)> list = new List<(double, double, double)>();
			if (parameter is IEnumerable enumerable && !(enumerable is string))
			{
				foreach (object item4 in enumerable)
				{
					if (item4 == null)
					{
						continue;
					}
					IDictionary<string, object> dictionary = item4 as IDictionary<string, object>;
					if (dictionary == null)
					{
						Type type = item4.GetType();
						dictionary = new Dictionary<string, object>();
						string[] array = new string[6]
						{
							"x",
							"y",
							"z",
							"X",
							"Y",
							"Z"
						};
						foreach (string text in array)
						{
							PropertyInfo property = type.GetProperty(text, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
							if (property != null)
							{
								dictionary[text.ToLowerInvariant()] = property.GetValue(item4);
							}
						}
					}
					if (dictionary != null)
					{
						double double_ = Convert.ToDouble(dictionary.TryGetValue("x", out var value) ? value : ((object)0));
						double double_2 = Convert.ToDouble(dictionary.TryGetValue("y", out var value2) ? value2 : ((object)0));
						double double_3 = Convert.ToDouble(dictionary.TryGetValue("z", out var value3) ? value3 : ((object)0));
						double item = smethod_0(aitoolContext_0, double_);
						double item2 = smethod_0(aitoolContext_0, double_2);
						double item3 = smethod_0(aitoolContext_0, double_3);
						list.Add((item, item2, item3));
					}
				}
			}
			return (list.Count > 0) ? list : null;
		}
		catch (Exception ex)
		{
			Logger.Warning("[SplitElementTool] 解析 points 失败: " + ex.Message);
			return null;
		}
	}

	private static List<double>? smethod_2(AIToolContext aitoolContext_0, string string_0)
	{
		try
		{
			object parameter = aitoolContext_0.GetParameter<object>(string_0, (object)null);
			if (parameter == null)
			{
				return null;
			}
			List<double> list = new List<double>();
			if (parameter is double[] collection)
			{
				list.AddRange(collection);
			}
			else if (parameter is List<double> collection2)
			{
				list.AddRange(collection2);
			}
			else if (parameter is List<object> list2)
			{
				foreach (object item in list2)
				{
					list.Add(Convert.ToDouble(item));
				}
			}
			else if (parameter is IEnumerable enumerable && !(enumerable is string))
			{
				foreach (object item2 in enumerable)
				{
					if (item2 != null)
					{
						list.Add(Convert.ToDouble(item2));
					}
				}
			}
			return (from double_0 in list.Where((double double_0) => double_0 > 0.0 && double_0 < 1.0).Distinct()
				orderby double_0
				select double_0).ToList();
		}
		catch (Exception ex)
		{
			Logger.Warning("[SplitElementTool] 解析 parameters 失败: " + ex.Message);
			return null;
		}
	}

	private List<int>? method_0(AIToolContext aitoolContext_0, IElementService ielementService_0, string string_0, string string_1, string string_2)
	{
		List<int> list = new List<int>();
		if (aitoolContext_0.HasParameter(string_2))
		{
			string parameter = aitoolContext_0.GetParameter<string>(string_2, (string)null);
			if (!string.IsNullOrEmpty(parameter))
			{
				object cachedData = aitoolContext_0.GetCachedData<object>(parameter);
				if (cachedData != null)
				{
					if (cachedData is IEnumerable enumerable)
					{
						foreach (object item3 in enumerable)
						{
							if (item3 != null)
							{
								PropertyInfo property = item3.GetType().GetProperty("id");
								if (property != null && property.GetValue(item3) is int item)
								{
									list.Add(item);
								}
							}
						}
					}
					return list.Distinct().ToList();
				}
			}
		}
		if (aitoolContext_0.HasParameter(string_0))
		{
			int parameter2 = aitoolContext_0.GetParameter<int>(string_0, 0);
			if (parameter2 > 0)
			{
				list.Add(parameter2);
				return list;
			}
		}
		if (aitoolContext_0.HasParameter(string_1))
		{
			object parameter3 = aitoolContext_0.GetParameter<object>(string_1, (object)null);
			if (parameter3 is int item2)
			{
				list.Add(item2);
			}
			else if (parameter3 is long num)
			{
				list.Add((int)num);
			}
			else if (parameter3 is int[] collection)
			{
				list.AddRange(collection);
			}
			else if (parameter3 is long[] source)
			{
				list.AddRange(source.Select((long long_0) => (int)long_0));
			}
			else if (parameter3 is List<int> collection2)
			{
				list.AddRange(collection2);
			}
			else if (parameter3 is List<long> source2)
			{
				list.AddRange(source2.Select((long long_0) => (int)long_0));
			}
			else if (parameter3 is List<object> list2)
			{
				try
				{
					list.AddRange(list2.ConvertAll(Convert.ToInt32));
				}
				catch
				{
					return null;
				}
			}
			return list.Distinct().ToList();
		}
		return (list.Count > 0) ? list.Distinct().ToList() : null;
	}
}
