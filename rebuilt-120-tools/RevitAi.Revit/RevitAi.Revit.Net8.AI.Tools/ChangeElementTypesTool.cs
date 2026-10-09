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

[AITool("change_element_types", Category = "元素修改", Description = "批量更改多个元素的族类型。例如：将所有 '旧墙 200mm' 替换为 '新墙 250mm'，或将多个柱的类型更改为新的尺寸类型", RequiresTransaction = true, RequiresModification = true)]
public sealed class ChangeElementTypesTool : IAITool
{
	[CompilerGenerated]
	private static class Class368
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class369 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ChangeElementTypesTool changeElementTypesTool_0;

		private int int_1;

		private List<int> list_0;

		private IModificationService imodificationService_0;

		private IElementService ielementService_0;

		private object object_0;

		private string string_0;

		private int int_2;

		private string string_1;

		private string string_2;

		private string string_3;

		private object object_1;

		private IEnumerable ienumerable_0;

		private IEnumerator ienumerator_0;

		private object object_2;

		private PropertyInfo propertyInfo_0;

		private int int_3;

		private int int_4;

		private object object_3;

		private int int_5;

		private long long_0;

		private int[] int_6;

		private long[] long_1;

		private List<int> list_1;

		private List<long> list_2;

		private List<object> list_3;

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
					Class369 stateMachine = this;
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
				int_1 = aitoolContext_0.GetParameter<int>("newTypeId", 0);
				list_0 = new List<int>();
				if (!aitoolContext_0.HasParameter("cacheId"))
				{
					goto IL_0258;
				}
				string_3 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
				if (string.IsNullOrEmpty(string_3))
				{
					goto IL_0251;
				}
				object_1 = aitoolContext_0.GetCachedData<object>(string_3);
				if (object_1 != null)
				{
					ienumerable_0 = object_1 as IEnumerable;
					if (ienumerable_0 != null)
					{
						ienumerator_0 = ienumerable_0.GetEnumerator();
						try
						{
							while (ienumerator_0.MoveNext())
							{
								object_2 = ienumerator_0.Current;
								if (object_2 != null)
								{
									propertyInfo_0 = object_2.GetType().GetProperty("id");
									if (propertyInfo_0 != null)
									{
										object value = propertyInfo_0.GetValue(object_2);
										if (value is int)
										{
											int_3 = (int)value;
											if (true)
											{
												list_0.Add(int_3);
											}
										}
									}
									propertyInfo_0 = null;
								}
								object_2 = null;
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
					object_1 = null;
					goto IL_0251;
				}
				result = AIToolResult.Fail("缓存 ID '" + string_3 + "' 无效或已过期，请重新查询元素");
				goto end_IL_0067;
				IL_052b:
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
						object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
						if (object_0 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
							defaultInterpolatedStringHandler.AppendLiteral("找不到新类型 ID ");
							defaultInterpolatedStringHandler.AppendFormatted(int_1);
							result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							string_0 = ielementService_0.GetElementName(object_0) ?? "未知类型";
							int_2 = imodificationService_0.ChangeElementTypes(aitoolContext_0.Document, (IEnumerable<int>)list_0, int_1);
							string text;
							if (list_0.Count != 1)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 1);
								defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
								defaultInterpolatedStringHandler2.AppendLiteral(" 个元素");
								text = defaultInterpolatedStringHandler2.ToStringAndClear();
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(3, 1);
								defaultInterpolatedStringHandler3.AppendLiteral("元素 ");
								defaultInterpolatedStringHandler3.AppendFormatted(list_0[0]);
								text = defaultInterpolatedStringHandler3.ToStringAndClear();
							}
							string_1 = text;
							string text2;
							if (int_2 != list_0.Count)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(2, 1);
								defaultInterpolatedStringHandler4.AppendFormatted(int_2);
								defaultInterpolatedStringHandler4.AppendLiteral(" 个");
								text2 = defaultInterpolatedStringHandler4.ToStringAndClear();
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(5, 1);
								defaultInterpolatedStringHandler5.AppendLiteral("所有 ");
								defaultInterpolatedStringHandler5.AppendFormatted(list_0.Count);
								defaultInterpolatedStringHandler5.AppendLiteral(" 个");
								text2 = defaultInterpolatedStringHandler5.ToStringAndClear();
							}
							string_2 = text2;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(18, 3);
							defaultInterpolatedStringHandler6.AppendLiteral("✅ 成功将 ");
							defaultInterpolatedStringHandler6.AppendFormatted(string_2);
							defaultInterpolatedStringHandler6.AppendLiteral(" ");
							defaultInterpolatedStringHandler6.AppendFormatted(string_1);
							defaultInterpolatedStringHandler6.AppendLiteral("元素的类型更改为 '");
							defaultInterpolatedStringHandler6.AppendFormatted(string_0);
							defaultInterpolatedStringHandler6.AppendLiteral("'");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler6.ToStringAndClear(), (object)new Class22<int, int, int, int, string>(list_0.Count, int_2, list_0.Count - int_2, int_1, string_0));
						}
					}
				}
				goto end_IL_0067;
				IL_050e:
				list_0 = list_0.Distinct().ToList();
				object_3 = null;
				goto IL_052b;
				IL_0251:
				string_3 = null;
				goto IL_0258;
				IL_0258:
				if (list_0.Count == 0 && aitoolContext_0.HasParameter("elementId"))
				{
					int_4 = aitoolContext_0.GetParameter<int>("elementId", 0);
					if (int_4 > 0)
					{
						list_0.Add(int_4);
					}
				}
				if (list_0.Count == 0 && aitoolContext_0.HasParameter("elementIds"))
				{
					object_3 = aitoolContext_0.GetParameter<object>("elementIds", (object)null);
					if (object_3 is int)
					{
						int_5 = (int)object_3;
						if (true)
						{
							list_0.Add(int_5);
							goto IL_050e;
						}
					}
					if (object_3 is long)
					{
						long_0 = (long)object_3;
						if (true)
						{
							list_0.Add((int)long_0);
							goto IL_050e;
						}
					}
					int_6 = object_3 as int[];
					if (int_6 != null)
					{
						list_0.AddRange(int_6);
					}
					else
					{
						long_1 = object_3 as long[];
						if (long_1 != null)
						{
							list_0.AddRange(Array.ConvertAll(long_1, (long long_0) => (int)long_0));
						}
						else
						{
							list_1 = object_3 as List<int>;
							if (list_1 != null)
							{
								list_0.AddRange(list_1);
							}
							else
							{
								list_2 = object_3 as List<long>;
								if (list_2 != null)
								{
									list_0.AddRange(list_2.ConvertAll((long long_0) => (int)long_0));
								}
								else
								{
									list_3 = object_3 as List<object>;
									if (list_3 != null)
									{
										try
										{
											list_0.AddRange(Array.ConvertAll(list_3.ToArray(), Convert.ToInt32));
										}
										catch
										{
											result = AIToolResult.Fail("elementIds 数组中包含非整数值");
											goto end_IL_0067;
										}
									}
									list_3 = null;
								}
								list_2 = null;
							}
							list_1 = null;
						}
						long_1 = null;
					}
					int_6 = null;
					goto IL_050e;
				}
				goto IL_052b;
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("更改元素类型失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "change_element_types";

	public string Category => "元素修改";

	public string Description => "批量更改多个元素的族类型";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"单个元素 ID（可选）。更改单个元素的类型，例如：12345\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（可选）。批量更改多个元素的类型，例如：[12345, 12346, 12347]\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）。从上一个查询工具（如 element_query）的返回结果中获取 cache_id 字段。使用缓存可以批量操作之前查询到的所有元素。注意：只需提供单个 cache_id 字符串，不需要数组。\"\n            },\n            \"newTypeId\": {\n                \"type\": \"integer\",\n                \"description\": \"新的族类型 ID（目标类型 ID）\"\n            }\n        },\n        \"required\": [\"newTypeId\"]\n    }";

	[AsyncStateMachine(typeof(Class369))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class369 stateMachine = new Class369();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.changeElementTypesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
