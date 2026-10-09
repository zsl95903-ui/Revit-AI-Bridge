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
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("move_elements", Category = "元素修改", Description = "移动单个或多个元素。参数单位：毫米。工具会自动将毫米转换为英尺后执行移动操作", RequiresTransaction = true, RequiresModification = true)]
public sealed class MoveTool : IAITool
{
	[CompilerGenerated]
	private static class Class613
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class614 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public MoveTool moveTool_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private List<int> list_0;

		private IModificationService imodificationService_0;

		private IElementService ielementService_0;

		private List<int> list_1;

		private double double_3;

		private double double_4;

		private double double_5;

		private int int_1;

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

		private List<int> list_2;

		private List<long> list_3;

		private List<object> list_4;

		private List<int>.Enumerator enumerator_0;

		private int int_6;

		private object object_3;

		private Exception exception_0;

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
					Class614 stateMachine = this;
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
				double_0 = aitoolContext_0.GetParameter<double>("x", 0.0);
				double_1 = aitoolContext_0.GetParameter<double>("y", 0.0);
				double_2 = aitoolContext_0.GetParameter<double>("z", 0.0);
				list_0 = new List<int>();
				if (!aitoolContext_0.HasParameter("cacheId"))
				{
					goto IL_02a8;
				}
				string_1 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
				if (string.IsNullOrEmpty(string_1))
				{
					goto IL_02a1;
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
					goto IL_02a1;
				}
				result = AIToolResult.Fail("缓存 ID '" + string_1 + "' 无效或已过期，请重新查询元素");
				goto end_IL_0067;
				IL_0576:
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("元素 ID 列表不能为空。请提供 elementId（单个）、elementIds（数组）或 cacheId 参数");
				}
				else
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					imodificationService_0 = ((revitAdapter != null) ? revitAdapter.ModificationService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
					if (imodificationService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ModificationService");
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
						list_1 = new List<int>();
						enumerator_0 = list_0.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								int_6 = enumerator_0.Current;
								object_3 = ielementService_0.GetElementById(aitoolContext_0.Document, int_6);
								if (object_3 == null)
								{
									list_1.Add(int_6);
								}
								object_3 = null;
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
						double_3 = double_0 / 304.8;
						double_4 = double_1 / 304.8;
						double_5 = double_2 / 304.8;
						int_1 = imodificationService_0.MoveElements(aitoolContext_0.Document, (IEnumerable<int>)list_0, double_3, double_4, double_5);
						string text;
						if (list_0.Count != 1)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
							defaultInterpolatedStringHandler.AppendFormatted(int_1);
							defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
							text = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("元素 ");
							defaultInterpolatedStringHandler2.AppendFormatted(list_0[0]);
							text = defaultInterpolatedStringHandler2.ToStringAndClear();
						}
						string_0 = text;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(17, 4);
						defaultInterpolatedStringHandler3.AppendLiteral("✅ 成功移动 ");
						defaultInterpolatedStringHandler3.AppendFormatted(string_0);
						defaultInterpolatedStringHandler3.AppendLiteral(" (");
						defaultInterpolatedStringHandler3.AppendFormatted(double_0, "F0");
						defaultInterpolatedStringHandler3.AppendLiteral(", ");
						defaultInterpolatedStringHandler3.AppendFormatted(double_1, "F0");
						defaultInterpolatedStringHandler3.AppendLiteral(", ");
						defaultInterpolatedStringHandler3.AppendFormatted(double_2, "F0");
						defaultInterpolatedStringHandler3.AppendLiteral(") 毫米");
						string text2 = defaultInterpolatedStringHandler3.ToStringAndClear();
						string text3;
						if (list_1.Count <= 0)
						{
							text3 = "";
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(8, 1);
							defaultInterpolatedStringHandler4.AppendLiteral("，");
							defaultInterpolatedStringHandler4.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler4.AppendLiteral(" 个元素未找到");
							text3 = defaultInterpolatedStringHandler4.ToStringAndClear();
						}
						result = AIToolResult.Ok(text2 + text3, (object)new Class281<int, int, int, List<int>, Class35<double, double, double, string>>(list_0.Count, int_1, list_1.Count, (list_1.Count > 0) ? list_1 : null, new Class35<double, double, double, string>(Math.Round(double_0, 2), Math.Round(double_1, 2), Math.Round(double_2, 2), "millimeters")));
					}
				}
				goto end_IL_0067;
				IL_0559:
				list_0 = list_0.Distinct().ToList();
				object_2 = null;
				goto IL_0576;
				IL_02a1:
				string_1 = null;
				goto IL_02a8;
				IL_02a8:
				if (list_0.Count == 0 && aitoolContext_0.HasParameter("elementId"))
				{
					int_3 = aitoolContext_0.GetParameter<int>("elementId", 0);
					if (int_3 > 0)
					{
						list_0.Add(int_3);
					}
				}
				if (list_0.Count == 0 && aitoolContext_0.HasParameter("elementIds"))
				{
					object_2 = aitoolContext_0.GetParameter<object>("elementIds", (object)null);
					if (object_2 is int)
					{
						int_4 = (int)object_2;
						if (true)
						{
							list_0.Add(int_4);
							goto IL_0559;
						}
					}
					if (object_2 is long)
					{
						long_0 = (long)object_2;
						if (true)
						{
							list_0.Add((int)long_0);
							goto IL_0559;
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
							list_0.AddRange(long_1.Select((long long_0) => (int)long_0));
						}
						else
						{
							list_2 = object_2 as List<int>;
							if (list_2 != null)
							{
								list_0.AddRange(list_2);
							}
							else
							{
								list_3 = object_2 as List<long>;
								if (list_3 != null)
								{
									list_0.AddRange(list_3.Select((long long_0) => (int)long_0));
								}
								else
								{
									list_4 = object_2 as List<object>;
									if (list_4 != null)
									{
										try
										{
											list_0.AddRange(list_4.ConvertAll(Convert.ToInt32));
										}
										catch
										{
											result = AIToolResult.Fail("elementIds 数组中包含非整数值");
											goto end_IL_0067;
										}
									}
									list_4 = null;
								}
								list_3 = null;
							}
							list_2 = null;
						}
						long_1 = null;
					}
					int_5 = null;
					goto IL_0559;
				}
				goto IL_0576;
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("移动元素失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "move_elements";

	public string Category => "元素修改";

	public string Description => "移动单个或多个元素。参数单位：毫米。工具会自动将毫米转换为英尺后执行移动操作";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elementId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"单个元素 ID（可选）。移动单个元素，例如：12345\"\r\n            },\r\n            \"elementIds\": {\r\n                \"type\": \"array\",\r\n                \"items\": { \"type\": \"integer\" },\r\n                \"description\": \"元素 ID 数组（可选）。批量移动多个元素，例如：[12345, 12346, 12347]\"\r\n            },\r\n            \"cacheId\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"缓存 ID（可选）。从上一个查询工具（如 element_query）的返回结果中获取 cache_id 字段。使用缓存可以批量操作之前查询到的所有元素。注意：只需提供单个 cache_id 字符串，不需要数组。\"\r\n            },\r\n            \"x\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"X 方向移动距离（毫米，正值为向右，负值为向左）\"\r\n            },\r\n            \"y\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"Y 方向移动距离（毫米，正值为向上，负值为向下）\"\r\n            },\r\n            \"z\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"Z 方向移动距离（毫米，正值为向上，负值为向下，默认 0）\"\r\n            }\r\n        },\r\n        \"required\": [\"x\", \"y\"]\r\n    }";

	[AsyncStateMachine(typeof(Class614))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class614 stateMachine = new Class614();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.moveTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
