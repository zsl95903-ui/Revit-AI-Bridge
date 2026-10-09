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

[AITool("delete_elements", Category = "元素修改", Description = "删除元素（支持单个或批量删除）。可以提供单个元素 ID、元素 ID 数组，或使用缓存 ID 删除之前查询的元素。", RequiresTransaction = true, RequiresModification = true)]
public sealed class DeleteElementsTool : IAITool
{
	[CompilerGenerated]
	private static class Class435
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class436 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public DeleteElementsTool deleteElementsTool_0;

		private IElementService ielementService_0;

		private IModificationService imodificationService_0;

		private List<int> list_0;

		private List<int> list_1;

		private List<int> list_2;

		private int int_1;

		private string string_0;

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

		private List<int> list_3;

		private List<long> list_4;

		private List<object> list_5;

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
					Class436 stateMachine = this;
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
					list_0 = new List<int>();
					if (!aitoolContext_0.HasParameter("cacheId"))
					{
						goto IL_02dd;
					}
					string_0 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
					if (string.IsNullOrEmpty(string_0))
					{
						goto IL_02d6;
					}
					object_0 = aitoolContext_0.GetCachedData<object>(string_0);
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
						goto IL_02d6;
					}
					result = AIToolResult.Fail("缓存 ID '" + string_0 + "' 无效或已过期，请重新查询元素");
				}
				goto end_IL_0067;
				IL_05af:
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("元素 ID 列表不能为空。请提供 elementId（单个）、elementIds（数组）或 cacheId 参数");
				}
				else
				{
					list_1 = new List<int>();
					list_2 = new List<int>();
					enumerator_0 = list_0.GetEnumerator();
					try
					{
						while (enumerator_0.MoveNext())
						{
							int_6 = enumerator_0.Current;
							object_3 = ielementService_0.GetElementById(aitoolContext_0.Document, int_6);
							if (object_3 != null)
							{
								list_1.Add(int_6);
							}
							else
							{
								list_2.Add(int_6);
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
						int_1 = imodificationService_0.DeleteElements(aitoolContext_0.Document, (IEnumerable<int>)list_1);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
						defaultInterpolatedStringHandler.AppendLiteral("✅ 成功删除 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
						string text = defaultInterpolatedStringHandler.ToStringAndClear();
						string text2;
						if (list_2.Count <= 0)
						{
							text2 = "";
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(8, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("，");
							defaultInterpolatedStringHandler2.AppendFormatted(list_2.Count);
							defaultInterpolatedStringHandler2.AppendLiteral(" 个元素未找到");
							text2 = defaultInterpolatedStringHandler2.ToStringAndClear();
						}
						result = AIToolResult.Ok(text + text2, (object)new Class109<int, int, int, List<int>>(list_0.Count, int_1, list_2.Count, (list_2.Count > 0) ? list_2 : null));
					}
				}
				goto end_IL_0067;
				IL_0592:
				list_0 = list_0.Distinct().ToList();
				object_2 = null;
				goto IL_05af;
				IL_02dd:
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
							goto IL_0592;
						}
					}
					if (object_2 is long)
					{
						long_0 = (long)object_2;
						if (true)
						{
							list_0.Add((int)long_0);
							goto IL_0592;
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
							list_0.AddRange(Array.ConvertAll(long_1, (long long_0) => (int)long_0));
						}
						else
						{
							list_3 = object_2 as List<int>;
							if (list_3 != null)
							{
								list_0.AddRange(list_3);
							}
							else
							{
								list_4 = object_2 as List<long>;
								if (list_4 != null)
								{
									list_0.AddRange(list_4.ConvertAll((long long_0) => (int)long_0));
								}
								else
								{
									list_5 = object_2 as List<object>;
									if (list_5 != null)
									{
										try
										{
											list_0.AddRange(Array.ConvertAll(list_5.ToArray(), Convert.ToInt32));
										}
										catch
										{
											result = AIToolResult.Fail("elementIds 数组中包含非整数值");
											goto end_IL_0067;
										}
									}
									list_5 = null;
								}
								list_4 = null;
							}
							list_3 = null;
						}
						long_1 = null;
					}
					int_5 = null;
					goto IL_0592;
				}
				goto IL_05af;
				IL_02d6:
				string_0 = null;
				goto IL_02dd;
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("删除元素失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "delete_elements";

	public string Category => "元素修改";

	public string Description => "删除元素（支持单个或批量删除）";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elementId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"单个元素 ID（可选）。如果要删除单个元素，使用此参数。\"\r\n            },\r\n            \"elementIds\": {\r\n                \"type\": \"array\",\r\n                \"items\": { \"type\": \"integer\" },\r\n                \"description\": \"元素 ID 数组（可选）。如果要批量删除，使用此参数。\"\r\n            },\r\n            \"cacheId\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"缓存 ID（可选）。如果要删除之前查询缓存的元素，使用此参数。工具会从缓存中提取所有元素的 ID 并删除。\"\r\n            }\r\n        },\r\n        \"required\": []\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class436))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class436 stateMachine = new Class436();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.deleteElementsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
