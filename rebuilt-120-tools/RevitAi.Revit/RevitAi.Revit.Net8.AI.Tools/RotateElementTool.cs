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

[AITool("rotate_element", Category = "元素操作", Description = "绕指定轴旋转 Revit 元素（单位：度）", RequiresTransaction = true, RequiresModification = true)]
public sealed class RotateElementTool : IAITool
{
	[CompilerGenerated]
	private static class Class623
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	private sealed class Class624 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public RotateElementTool rotateElementTool_0;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private double double_4;

		private double double_5;

		private double double_6;

		private List<int> list_0;

		private IModificationService imodificationService_0;

		private IElementService ielementService_0;

		private List<int> list_1;

		private int int_1;

		private string string_0;

		private string string_1;

		private string string_2;

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
					Class624 stateMachine = this;
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
				double_0 = aitoolContext_0.GetParameter<double>("axisX", 0.0);
				double_1 = aitoolContext_0.GetParameter<double>("axisY", 0.0);
				double_2 = aitoolContext_0.GetParameter<double>("axisZ", 0.0);
				double_3 = aitoolContext_0.GetParameter<double>("angleDegrees", 0.0);
				double_4 = (aitoolContext_0.HasParameter("originX") ? (aitoolContext_0.GetParameter<double>("originX", 0.0) / 304.8) : 0.0);
				double_5 = (aitoolContext_0.HasParameter("originY") ? (aitoolContext_0.GetParameter<double>("originY", 0.0) / 304.8) : 0.0);
				double_6 = (aitoolContext_0.HasParameter("originZ") ? (aitoolContext_0.GetParameter<double>("originZ", 0.0) / 304.8) : 0.0);
				list_0 = new List<int>();
				if (!aitoolContext_0.HasParameter("cacheId"))
				{
					goto IL_03bc;
				}
				string_2 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
				if (string.IsNullOrEmpty(string_2))
				{
					goto IL_03b5;
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
					goto IL_03b5;
				}
				result = AIToolResult.Fail("缓存 ID '" + string_2 + "' 无效或已过期，请重新查询元素");
				goto end_IL_0067;
				IL_03bc:
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
							goto IL_0672;
						}
					}
					if (object_2 is long)
					{
						long_0 = (long)object_2;
						if (true)
						{
							list_0.Add((int)long_0);
							goto IL_0672;
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
									list_0.AddRange(list_3.ConvertAll((long long_0) => (int)long_0));
								}
								else
								{
									list_4 = object_2 as List<object>;
									if (list_4 != null)
									{
										try
										{
											list_0.AddRange(Array.ConvertAll(list_4.ToArray(), Convert.ToInt32));
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
					goto IL_0672;
				}
				goto IL_068f;
				IL_03b5:
				string_2 = null;
				goto IL_03bc;
				IL_0672:
				list_0 = list_0.Distinct().ToList();
				object_2 = null;
				goto IL_068f;
				IL_068f:
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
						int_1 = imodificationService_0.RotateElements(aitoolContext_0.Document, (IEnumerable<int>)list_0, double_0, double_1, double_2, double_3, double_4, double_5, double_6);
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
						string text2;
						if (double_4 == 0.0 && double_5 == 0.0 && double_6 == 0.0)
						{
							text2 = "原点";
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(6, 3);
							defaultInterpolatedStringHandler3.AppendLiteral("(");
							defaultInterpolatedStringHandler3.AppendFormatted(double_4 * 304.8, "F1");
							defaultInterpolatedStringHandler3.AppendLiteral(", ");
							defaultInterpolatedStringHandler3.AppendFormatted(double_5 * 304.8, "F1");
							defaultInterpolatedStringHandler3.AppendLiteral(", ");
							defaultInterpolatedStringHandler3.AppendFormatted(double_6 * 304.8, "F1");
							defaultInterpolatedStringHandler3.AppendLiteral(")");
							text2 = defaultInterpolatedStringHandler3.ToStringAndClear();
						}
						string_1 = text2;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(29, 6);
						defaultInterpolatedStringHandler4.AppendLiteral("✅ 成功将 ");
						defaultInterpolatedStringHandler4.AppendFormatted(string_0);
						defaultInterpolatedStringHandler4.AppendLiteral(" 绕轴 (");
						defaultInterpolatedStringHandler4.AppendFormatted(double_0);
						defaultInterpolatedStringHandler4.AppendLiteral(", ");
						defaultInterpolatedStringHandler4.AppendFormatted(double_1);
						defaultInterpolatedStringHandler4.AppendLiteral(", ");
						defaultInterpolatedStringHandler4.AppendFormatted(double_2);
						defaultInterpolatedStringHandler4.AppendLiteral(") 旋转 ");
						defaultInterpolatedStringHandler4.AppendFormatted(double_3);
						defaultInterpolatedStringHandler4.AppendLiteral(" 度（旋转中心：");
						defaultInterpolatedStringHandler4.AppendFormatted(string_1);
						defaultInterpolatedStringHandler4.AppendLiteral("）");
						string text3 = defaultInterpolatedStringHandler4.ToStringAndClear();
						string text4;
						if (list_1.Count <= 0)
						{
							text4 = "";
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(8, 1);
							defaultInterpolatedStringHandler5.AppendLiteral("，");
							defaultInterpolatedStringHandler5.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler5.AppendLiteral(" 个元素未找到");
							text4 = defaultInterpolatedStringHandler5.ToStringAndClear();
						}
						result = AIToolResult.Ok(text3 + text4, (object)new Class292<int, int, int, List<int>, _003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>, double>(list_0.Count, int_1, list_1.Count, (list_1.Count > 0) ? list_1 : null, new _003C_003Ef__AnonymousType27<double, double, double>(double_0, double_1, double_2), new _003C_003Ef__AnonymousType27<double, double, double>(double_4 * 304.8, double_5 * 304.8, double_6 * 304.8), double_3));
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("旋转元素失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "rotate_element";

	public string Category => "元素操作";

	public string Description => "绕指定轴旋转 Revit 元素（单位：度）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"单个元素 ID（可选）。旋转单个元素，例如：12345\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（可选）。批量旋转多个元素，例如：[12345, 12346, 12347]\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）。从上一个查询工具（如 element_query）的返回结果中获取 cache_id 字段。使用缓存可以批量操作之前查询到的所有元素。注意：只需提供单个 cache_id 字符串，不需要数组。\"\n            },\n            \"axisX\": {\n                \"type\": \"number\",\n                \"description\": \"旋转轴 X 方向分量\"\n            },\n            \"axisY\": {\n                \"type\": \"number\",\n                \"description\": \"旋转轴 Y 方向分量\"\n            },\n            \"axisZ\": {\n                \"type\": \"number\",\n                \"description\": \"旋转轴 Z 方向分量\"\n            },\n            \"angleDegrees\": {\n                \"type\": \"number\",\n                \"description\": \"旋转角度（度，正值为逆时针）\"\n            },\n            \"originX\": {\n                \"type\": \"number\",\n                \"description\": \"旋转中心点 X 坐标（毫米，可选，默认为 0）\"\n            },\n            \"originY\": {\n                \"type\": \"number\",\n                \"description\": \"旋转中心点 Y 坐标（毫米，可选，默认为 0）\"\n            },\n            \"originZ\": {\n                \"type\": \"number\",\n                \"description\": \"旋转中心点 Z 坐标（毫米，可选，默认为 0）\"\n            }\n        },\n        \"required\": [\"axisX\", \"axisY\", \"axisZ\", \"angleDegrees\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class624))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class624 stateMachine = new Class624();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.rotateElementTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
