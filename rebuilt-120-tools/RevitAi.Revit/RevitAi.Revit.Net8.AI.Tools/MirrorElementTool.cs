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

[AITool("mirror_element", Category = "元素修改", Description = "镜像元素（支持单个或批量镜像）。所有距离单位：毫米（工具内部会转换为英尺）", RequiresTransaction = true, RequiresModification = true)]
public sealed class MirrorElementTool : IAITool
{
	[CompilerGenerated]
	private static class Class609
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class610 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public MirrorElementTool mirrorElementTool_0;

		private string string_0;

		private List<int> list_0;

		private IModificationService imodificationService_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private double double_4;

		private double double_5;

		private MirrorResult mirrorResult_0;

		private string string_1;

		private string string_2;

		private object object_0;

		private IEnumerable ienumerable_0;

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

		private List<int> list_1;

		private List<long> list_2;

		private List<object> list_3;

		private double double_6;

		private double double_7;

		private double double_8;

		private double double_9;

		private double double_10;

		private double double_11;

		private double double_12;

		private double double_13;

		private double double_14;

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
					Class610 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("mirrorType", "line");
				list_0 = new List<int>();
				if (!aitoolContext_0.HasParameter("cacheId"))
				{
					goto IL_0261;
				}
				string_2 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
				if (string.IsNullOrEmpty(string_2))
				{
					goto IL_025a;
				}
				object_0 = aitoolContext_0.GetCachedData<object>(string_2);
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
					ienumerable_0 = null;
					object_0 = null;
					goto IL_025a;
				}
				result = AIToolResult.Fail("缓存 ID '" + string_2 + "' 无效或已过期，请重新查询元素");
				goto end_IL_0067;
				IL_0517:
				list_0 = list_0.Distinct().ToList();
				object_2 = null;
				goto IL_0534;
				IL_0534:
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("元素 ID 列表不能为空。请提供 elementId（单个）、elementIds（数组）或 cacheId 参数");
				}
				else
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					imodificationService_0 = ((revitAdapter != null) ? revitAdapter.ModificationService : null);
					if (imodificationService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ModificationService");
					}
					else if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						if (string_0 == "line")
						{
							double_6 = aitoolContext_0.GetParameter<double>("lineStartX", 0.0);
							double_7 = aitoolContext_0.GetParameter<double>("lineStartY", 0.0);
							double_8 = aitoolContext_0.GetParameter<double>("lineEndX", 0.0);
							double_9 = aitoolContext_0.GetParameter<double>("lineEndY", 0.0);
							double_0 = double_6 / 304.8;
							double_1 = double_7 / 304.8;
							double_2 = 0.0;
							double_10 = double_8 - double_6;
							double_11 = double_9 - double_7;
							double_3 = double_11;
							double_4 = 0.0 - double_10;
							double_5 = 0.0;
							double_12 = Math.Sqrt(double_3 * double_3 + double_4 * double_4);
							if (double_12 > 0.0)
							{
								double_3 /= double_12;
								double_4 /= double_12;
							}
							goto IL_0822;
						}
						if (string_0 == "point")
						{
							double_13 = aitoolContext_0.GetParameter<double>("pointX", 0.0);
							double_14 = aitoolContext_0.GetParameter<double>("pointY", 0.0);
							double_0 = double_13 / 304.8;
							double_1 = double_14 / 304.8;
							double_2 = 0.0;
							double_3 = 0.0;
							double_4 = 0.0;
							double_5 = 1.0;
							goto IL_0822;
						}
						result = AIToolResult.Fail("不支持的镜像类型: " + string_0);
					}
				}
				goto end_IL_0067;
				IL_0261:
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
							goto IL_0517;
						}
					}
					if (object_2 is long)
					{
						long_0 = (long)object_2;
						if (true)
						{
							list_0.Add((int)long_0);
							goto IL_0517;
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
							list_1 = object_2 as List<int>;
							if (list_1 != null)
							{
								list_0.AddRange(list_1);
							}
							else
							{
								list_2 = object_2 as List<long>;
								if (list_2 != null)
								{
									list_0.AddRange(list_2.ConvertAll((long long_0) => (int)long_0));
								}
								else
								{
									list_3 = object_2 as List<object>;
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
					int_4 = null;
					goto IL_0517;
				}
				goto IL_0534;
				IL_025a:
				string_2 = null;
				goto IL_0261;
				IL_0822:
				mirrorResult_0 = imodificationService_0.MirrorElements(aitoolContext_0.Document, (IEnumerable<int>)list_0, double_0, double_1, double_2, double_3, double_4, double_5, true);
				if (!mirrorResult_0.Success || mirrorResult_0.MirrorElementIds.Count() == 0)
				{
					result = AIToolResult.Fail("镜像元素失败");
				}
				else
				{
					string text;
					if (list_0.Count != 1)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
						defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
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
					string_1 = text;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(17, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("✅ 成功镜像 ");
					defaultInterpolatedStringHandler3.AppendFormatted(string_1);
					defaultInterpolatedStringHandler3.AppendLiteral("，创建了 ");
					defaultInterpolatedStringHandler3.AppendFormatted(mirrorResult_0.MirrorElementIds.Count());
					defaultInterpolatedStringHandler3.AppendLiteral(" 个新元素");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class277<int, int, IEnumerable<int>, string, bool>(list_0.Count, mirrorResult_0.MirrorElementIds.Count(), mirrorResult_0.MirrorElementIds, string_0, mirrorResult_0.IsMirrorCopy));
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("镜像元素失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "mirror_element";

	public string Category => "元素修改";

	public string Description => "镜像元素（支持单个或批量镜像）。所有距离单位：毫米（工具内部会转换为英尺）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"单个元素 ID（可选）。镜像单个元素，例如：12345\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（可选）。批量镜像多个元素，例如：[12345, 12346, 12347]\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）。从上一个查询工具（如 element_query）的返回结果中获取 cache_id 字段。使用缓存可以批量操作之前查询到的所有元素。注意：只需提供单个 cache_id 字符串，不需要数组。\"\n            },\n            \"mirrorType\": {\n                \"type\": \"string\",\n                \"description\": \"镜像类型（line 或 point）\",\n                \"enum\": [\"line\", \"point\"],\n                \"default\": \"line\"\n            },\n            \"lineStartX\": {\n                \"type\": \"number\",\n                \"description\": \"镜像线起点 X 坐标（毫米，当 mirrorType=line 时必需）\"\n            },\n            \"lineStartY\": {\n                \"type\": \"number\",\n                \"description\": \"镜像线起点 Y 坐标（毫米，当 mirrorType=line 时必需）\"\n            },\n            \"lineEndX\": {\n                \"type\": \"number\",\n                \"description\": \"镜像线终点 X 坐标（毫米，当 mirrorType=line 时必需）\"\n            },\n            \"lineEndY\": {\n                \"type\": \"number\",\n                \"description\": \"镜像线终点 Y 坐标（毫米，当 mirrorType=line 时必需）\"\n            },\n            \"pointX\": {\n                \"type\": \"number\",\n                \"description\": \"镜像中心点 X 坐标（毫米，当 mirrorType=point 时必需）\"\n            },\n            \"pointY\": {\n                \"type\": \"number\",\n                \"description\": \"镜像中心点 Y 坐标（毫米，当 mirrorType=point 时必需）\"\n            }\n        },\n        \"required\": []\n    }";

	[AsyncStateMachine(typeof(Class610))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class610 stateMachine = new Class610();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.mirrorElementTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
