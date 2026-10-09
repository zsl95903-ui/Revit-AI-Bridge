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

[AITool("copy_element", Category = "元素修改", Description = "复制指定的元素（支持单个或批量复制）。可以指定复制位置的偏移量", RequiresTransaction = true, RequiresModification = true)]
public sealed class CopyElementTool : IAITool
{
	[CompilerGenerated]
	private static class Class379
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class380 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CopyElementTool copyElementTool_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private List<int> list_0;

		private IElementService ielementService_0;

		private IModificationService imodificationService_0;

		private List<object> list_1;

		private List<int> list_2;

		private double double_3;

		private double double_4;

		private double double_5;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_3;

		private string string_0;

		private string string_1;

		private object object_0;

		private IEnumerable ienumerable_1;

		private IEnumerator ienumerator_0;

		private object object_1;

		private PropertyInfo propertyInfo_0;

		private int int_1;

		private int int_2;

		private object object_2;

		private int int_3;

		private long long_0;

		private int[] int_4;

		private long[] long_1;

		private List<int> list_4;

		private List<long> list_5;

		private List<object> list_6;

		private List<int>.Enumerator enumerator_0;

		private int int_5;

		private object object_3;

		private IEnumerator<object> ienumerator_1;

		private object object_4;

		private int? nullable_0;

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
					Class380 stateMachine = this;
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
					ienumerable_1 = object_0 as IEnumerable;
					if (ienumerable_1 != null)
					{
						ienumerator_0 = ienumerable_1.GetEnumerator();
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
											int_1 = (int)value;
											if (true)
											{
												list_0.Add(int_1);
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
					ienumerable_1 = null;
					object_0 = null;
					goto IL_02a1;
				}
				result = AIToolResult.Fail("缓存 ID '" + string_1 + "' 无效或已过期，请重新查询元素");
				goto end_IL_0067;
				IL_057b:
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("元素 ID 列表不能为空。请提供 elementId（单个）、elementIds（数组）或 cacheId 参数");
				}
				else
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
						list_1 = new List<object>();
						list_2 = new List<int>();
						enumerator_0 = list_0.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								int_5 = enumerator_0.Current;
								object_3 = ielementService_0.GetElementById(aitoolContext_0.Document, int_5);
								if (object_3 != null)
								{
									list_1.Add(object_3);
								}
								else
								{
									list_2.Add(int_5);
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
						if (list_1.Count == 0)
						{
							result = AIToolResult.Fail("找不到任何指定的元素");
						}
						else
						{
							double_3 = double_0 / 304.8;
							double_4 = double_1 / 304.8;
							double_5 = double_2 / 304.8;
							ienumerable_0 = imodificationService_0.CopyElements(aitoolContext_0.Document, (IEnumerable<int>)list_0, double_3, double_4, double_5);
							list_3 = new List<object>();
							ienumerator_1 = ienumerable_0.GetEnumerator();
							try
							{
								while (ienumerator_1.MoveNext())
								{
									object_4 = ienumerator_1.Current;
									nullable_0 = ielementService_0.GetElementId(object_4);
									if (nullable_0.HasValue)
									{
										list_3.Add(new Class33<int, string>(nullable_0.Value, ielementService_0.GetElementName(object_4)));
									}
									object_4 = null;
								}
							}
							finally
							{
								if (num < 0 && ienumerator_1 != null)
								{
									ienumerator_1.Dispose();
								}
							}
							ienumerator_1 = null;
							string text;
							if (list_0.Count != 1)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
								defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
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
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(17, 2);
							defaultInterpolatedStringHandler3.AppendLiteral("✅ 成功复制 ");
							defaultInterpolatedStringHandler3.AppendFormatted(string_0);
							defaultInterpolatedStringHandler3.AppendLiteral("，创建了 ");
							defaultInterpolatedStringHandler3.AppendFormatted(list_3.Count);
							defaultInterpolatedStringHandler3.AppendLiteral(" 个新副本");
							string text2 = defaultInterpolatedStringHandler3.ToStringAndClear();
							string text3;
							if (list_2.Count <= 0)
							{
								text3 = "";
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(8, 1);
								defaultInterpolatedStringHandler4.AppendLiteral("，");
								defaultInterpolatedStringHandler4.AppendFormatted(list_2.Count);
								defaultInterpolatedStringHandler4.AppendLiteral(" 个元素未找到");
								text3 = defaultInterpolatedStringHandler4.ToStringAndClear();
							}
							string text4;
							if (double_0 == 0.0 && double_1 == 0.0 && double_2 == 0.0)
							{
								text4 = "";
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(14, 3);
								defaultInterpolatedStringHandler5.AppendLiteral(" (偏移: ");
								defaultInterpolatedStringHandler5.AppendFormatted(double_0, "F0");
								defaultInterpolatedStringHandler5.AppendLiteral(", ");
								defaultInterpolatedStringHandler5.AppendFormatted(double_1, "F0");
								defaultInterpolatedStringHandler5.AppendLiteral(", ");
								defaultInterpolatedStringHandler5.AppendFormatted(double_2, "F0");
								defaultInterpolatedStringHandler5.AppendLiteral(" 毫米)");
								text4 = defaultInterpolatedStringHandler5.ToStringAndClear();
							}
							result = AIToolResult.Ok(text2 + text3 + text4, (object)new Class34<int, int, int, List<int>, List<object>, Class35<double, double, double, string>>(list_0.Count, list_3.Count, list_2.Count, (list_2.Count > 0) ? list_2 : null, list_3, new Class35<double, double, double, string>(Math.Round(double_0, 2), Math.Round(double_1, 2), Math.Round(double_2, 2), "millimeters")));
						}
					}
				}
				goto end_IL_0067;
				IL_055e:
				list_0 = list_0.Distinct().ToList();
				object_2 = null;
				goto IL_057b;
				IL_02a1:
				string_1 = null;
				goto IL_02a8;
				IL_02a8:
				if (list_0.Count == 0 && aitoolContext_0.HasParameter("elementId"))
				{
					int_2 = aitoolContext_0.GetParameter<int>("elementId", 0);
					if (int_2 > 0)
					{
						list_0.Add(int_2);
					}
				}
				if (list_0.Count == 0 && aitoolContext_0.HasParameter("elementIds"))
				{
					object_2 = aitoolContext_0.GetParameter<object>("elementIds", (object)null);
					if (object_2 is int)
					{
						int_3 = (int)object_2;
						if (true)
						{
							list_0.Add(int_3);
							goto IL_055e;
						}
					}
					if (object_2 is long)
					{
						long_0 = (long)object_2;
						if (true)
						{
							list_0.Add((int)long_0);
							goto IL_055e;
						}
					}
					int_4 = object_2 as int[];
					if (int_4 != null)
					{
						list_0.AddRange(int_4);
					}
					else
					{
						long_1 = object_2 as long[];
						if (long_1 != null)
						{
							list_0.AddRange(Array.ConvertAll(long_1, (long long_0) => (int)long_0));
						}
						else
						{
							list_4 = object_2 as List<int>;
							if (list_4 != null)
							{
								list_0.AddRange(list_4);
							}
							else
							{
								list_5 = object_2 as List<long>;
								if (list_5 != null)
								{
									list_0.AddRange(list_5.ConvertAll((long long_0) => (int)long_0));
								}
								else
								{
									list_6 = object_2 as List<object>;
									if (list_6 != null)
									{
										try
										{
											list_0.AddRange(Array.ConvertAll(list_6.ToArray(), Convert.ToInt32));
										}
										catch
										{
											result = AIToolResult.Fail("elementIds 数组中包含非整数值");
											goto end_IL_0067;
										}
									}
									list_6 = null;
								}
								list_5 = null;
							}
							list_4 = null;
						}
						long_1 = null;
					}
					int_4 = null;
					goto IL_055e;
				}
				goto IL_057b;
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("复制元素失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "copy_element";

	public string Category => "元素修改";

	public string Description => "复制指定的元素（支持单个或批量复制）。可以指定复制位置的偏移量";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"单个元素 ID（可选）。复制单个元素，例如：12345\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（可选）。批量复制多个元素，例如：[12345, 12346, 12347]\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）。从上一个查询工具（如 element_query）的返回结果中获取 cache_id 字段。使用缓存可以批量操作之前查询到的所有元素。注意：只需提供单个 cache_id 字符串，不需要数组。\"\n            },\n            \"x\": {\n                \"type\": \"number\",\n                \"description\": \"X 方向复制偏移量（毫米，默认 0）\"\n            },\n            \"y\": {\n                \"type\": \"number\",\n                \"description\": \"Y 方向复制偏移量（毫米，默认 0）\"\n            },\n            \"z\": {\n                \"type\": \"number\",\n                \"description\": \"Z 方向复制偏移量（毫米，默认 0）\"\n            }\n        },\n        \"required\": []\n    }";

	[AsyncStateMachine(typeof(Class380))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class380 stateMachine = new Class380();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.copyElementTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
