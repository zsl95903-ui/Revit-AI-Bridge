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

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("element_group_manager", Category = "元素修改", Description = "管理元素的分组状态。支持将多个元素组合成一个组，或将组解散释放其中的所有元素。", RequiresTransaction = true, RequiresModification = true)]
public sealed class ElementGroupManagerTool : IAITool
{
	[CompilerGenerated]
	private static class Class444
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class445 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ElementGroupManagerTool elementGroupManagerTool_0;

		private string string_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 1u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class445 stateMachine = this;
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
			goto IL_006e;
			IL_006e:
			AIToolResult result;
			try
			{
				TaskAwaiter<AIToolResult> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0243;
				}
				TaskAwaiter<AIToolResult> awaiter3;
				if (num == 2)
				{
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0209;
				}
				AIToolResult val;
				if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					string_0 = aitoolContext_0.GetParameter<string>("operation", (string)null);
					if (!string.IsNullOrEmpty(string_0))
					{
						Logger.Info("[ElementGroupManagerTool] 执行操作: " + string_0);
						string_1 = string_0.ToLower();
						string text = string_1;
						if (!(text == "group"))
						{
							if (!(text == "ungroup"))
							{
								val = AIToolResult.Fail("不支持的操作类型: " + string_0);
								goto IL_025f;
							}
							awaiter3 = elementGroupManagerTool_0.method_1(aitoolContext_0).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = 2;
								int_0 = 2;
								taskAwaiter_1 = awaiter3;
								Class445 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
								return;
							}
							goto IL_0209;
						}
						awaiter2 = elementGroupManagerTool_0.method_0(aitoolContext_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							Class445 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
							return;
						}
						goto IL_0243;
					}
					result = AIToolResult.Fail("必须指定 operation 参数");
				}
				goto end_IL_006e;
				IL_0243:
				aitoolResult_0 = awaiter2.GetResult();
				val = aitoolResult_0;
				aitoolResult_0 = null;
				goto IL_025f;
				IL_025f:
				result = val;
				goto end_IL_006e;
				IL_0209:
				aitoolResult_1 = awaiter3.GetResult();
				val = aitoolResult_1;
				aitoolResult_1 = null;
				goto IL_025f;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[ElementGroupManagerTool] 执行失败: " + exception_0.Message);
				result = AIToolResult.Fail("操作失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class446 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public ElementGroupManagerTool elementGroupManagerTool_0;

		private string string_0;

		private List<int> list_0;

		private IElementService ielementService_0;

		private IModificationService imodificationService_0;

		private List<object> list_1;

		private List<int> list_2;

		private object object_0;

		private int? nullable_0;

		private string string_1;

		private string string_2;

		private object object_1;

		private IEnumerable ienumerable_0;

		private IEnumerator ienumerator_0;

		private object object_2;

		private PropertyInfo propertyInfo_0;

		private int int_1;

		private int int_2;

		private object object_3;

		private int int_3;

		private long long_0;

		private int[] int_4;

		private long[] long_1;

		private List<int> list_3;

		private List<long> list_4;

		private List<object> list_5;

		private List<int>.Enumerator enumerator_0;

		private int int_5;

		private object object_4;

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
					Class446 stateMachine = this;
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
			string_0 = aitoolContext_0.GetParameter<string>("groupName", (string)null);
			list_0 = new List<int>();
			AIToolResult result;
			if (aitoolContext_0.HasParameter("cacheId"))
			{
				string_2 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
				if (!string.IsNullOrEmpty(string_2))
				{
					object_1 = aitoolContext_0.GetCachedData<object>(string_2);
					if (object_1 == null)
					{
						result = AIToolResult.Fail("缓存 ID '" + string_2 + "' 无效或已过期，请重新查询元素");
						goto IL_0962;
					}
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
											int_1 = (int)value;
											if (true)
											{
												list_0.Add(int_1);
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
				}
				string_2 = null;
			}
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
				object_3 = aitoolContext_0.GetParameter<object>("elementIds", (object)null);
				if (object_3 is int)
				{
					int_3 = (int)object_3;
					if (true)
					{
						list_0.Add(int_3);
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
				int_4 = object_3 as int[];
				if (int_4 != null)
				{
					list_0.AddRange(int_4);
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
						list_3 = object_3 as List<int>;
						if (list_3 != null)
						{
							list_0.AddRange(list_3);
						}
						else
						{
							list_4 = object_3 as List<long>;
							if (list_4 != null)
							{
								list_0.AddRange(list_4.ConvertAll((long long_0) => (int)long_0));
							}
							else
							{
								list_5 = object_3 as List<object>;
								if (list_5 != null)
								{
									try
									{
										list_0.AddRange(Array.ConvertAll(list_5.ToArray(), Convert.ToInt32));
									}
									catch
									{
										result = AIToolResult.Fail("elementIds 数组中包含非整数值");
										goto IL_0962;
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
				int_4 = null;
				goto IL_050e;
			}
			goto IL_052b;
			IL_052b:
			if (list_0.Count == 0)
			{
				result = AIToolResult.Fail("元素 ID 列表不能为空。请提供 elementIds（数组）或 cacheId 参数");
			}
			else if (list_0.Count < 2)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("至少需要 2 个元素才能创建组，当前只提供了 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
				result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
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
							object_4 = ielementService_0.GetElementById(aitoolContext_0.Document, int_5);
							if (object_4 != null)
							{
								list_1.Add(object_4);
							}
							else
							{
								list_2.Add(int_5);
							}
							object_4 = null;
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
					if (list_1.Count < 2)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("有效元素不足 2 个（找到 ");
						defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler2.AppendLiteral(" 个），无法创建组");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
					}
					else
					{
						object_0 = imodificationService_0.GroupElements(aitoolContext_0.Document, (IEnumerable<int>)list_0, string_0);
						if (object_0 == null)
						{
							result = AIToolResult.Fail("创建组失败");
						}
						else
						{
							nullable_0 = ielementService_0.GetElementId(object_0);
							string_1 = ielementService_0.GetElementName(object_0);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(49, 2);
							defaultInterpolatedStringHandler3.AppendLiteral("[ElementGroupManagerTool] Group: 成功创建组 '");
							defaultInterpolatedStringHandler3.AppendFormatted(string_1);
							defaultInterpolatedStringHandler3.AppendLiteral("'，包含 ");
							defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler3.AppendLiteral(" 个元素");
							Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(17, 2);
							defaultInterpolatedStringHandler4.AppendLiteral("✅ 成功创建组: ");
							defaultInterpolatedStringHandler4.AppendFormatted(string_1 ?? "未命名");
							defaultInterpolatedStringHandler4.AppendLiteral("，包含 ");
							defaultInterpolatedStringHandler4.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler4.AppendLiteral(" 个元素");
							string text = defaultInterpolatedStringHandler4.ToStringAndClear();
							string text2;
							if (list_2.Count <= 0)
							{
								text2 = "";
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(8, 1);
								defaultInterpolatedStringHandler5.AppendLiteral("，");
								defaultInterpolatedStringHandler5.AppendFormatted(list_2.Count);
								defaultInterpolatedStringHandler5.AppendLiteral(" 个元素未找到");
								text2 = defaultInterpolatedStringHandler5.ToStringAndClear();
							}
							result = AIToolResult.Ok(text + text2, (object)new Class114<string, int?, string, int, int, int, List<int>>("group", nullable_0, string_1, list_0.Count, list_1.Count, list_2.Count, (list_2.Count > 0) ? list_2 : null));
						}
					}
				}
			}
			goto IL_0962;
			IL_0962:
			int_0 = -2;
			string_0 = null;
			list_0 = null;
			ielementService_0 = null;
			imodificationService_0 = null;
			list_1 = null;
			list_2 = null;
			object_0 = null;
			string_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_050e:
			list_0 = list_0.Distinct().ToList();
			object_3 = null;
			goto IL_052b;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class447 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public ElementGroupManagerTool elementGroupManagerTool_0;

		private int int_1;

		private IElementService ielementService_0;

		private IModificationService imodificationService_0;

		private object object_0;

		private string string_0;

		private IEnumerable<int> ienumerable_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class447 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter);
				int num = -1;
				int_0 = -1;
			}
			awaiter.GetResult();
			AIToolResult result;
			if (!aitoolContext_0.HasParameter("groupId"))
			{
				result = AIToolResult.Fail("ungroup 操作缺少必需参数 groupId");
			}
			else
			{
				int_1 = aitoolContext_0.GetParameter<int>("groupId", 0);
				if (int_1 <= 0)
				{
					result = AIToolResult.Fail("组 ID 必须大于 0");
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
					else
					{
						object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
						if (object_0 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
							defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
							defaultInterpolatedStringHandler.AppendFormatted(int_1);
							defaultInterpolatedStringHandler.AppendLiteral(" 的组元素");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							string_0 = ielementService_0.GetElementName(object_0);
							ienumerable_0 = imodificationService_0.UngroupElements(aitoolContext_0.Document, int_1);
							if (!ienumerable_0.Any())
							{
								result = AIToolResult.Fail("解组失败或组中没有元素");
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(50, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("[ElementGroupManagerTool] Ungroup: 成功解组 '");
								defaultInterpolatedStringHandler2.AppendFormatted(string_0);
								defaultInterpolatedStringHandler2.AppendLiteral("'，释放 ");
								defaultInterpolatedStringHandler2.AppendFormatted(ienumerable_0.Count());
								defaultInterpolatedStringHandler2.AppendLiteral(" 个元素");
								Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(16, 2);
								defaultInterpolatedStringHandler3.AppendLiteral("✅ 成功解组: ");
								string text = string_0;
								if (text == null)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(2, 1);
									defaultInterpolatedStringHandler4.AppendLiteral("组 ");
									defaultInterpolatedStringHandler4.AppendFormatted(int_1);
									text = defaultInterpolatedStringHandler4.ToStringAndClear();
								}
								defaultInterpolatedStringHandler3.AppendFormatted(text);
								defaultInterpolatedStringHandler3.AppendLiteral("，释放 ");
								defaultInterpolatedStringHandler3.AppendFormatted(ienumerable_0.Count());
								defaultInterpolatedStringHandler3.AppendLiteral(" 个元素");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class115<string, int, string, int, List<int>>("ungroup", int_1, string_0, ienumerable_0.Count(), ienumerable_0.Take(100).ToList()));
							}
						}
					}
				}
			}
			int_0 = -2;
			ielementService_0 = null;
			imodificationService_0 = null;
			object_0 = null;
			string_0 = null;
			ienumerable_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "element_group_manager";

	public string Category => "元素修改";

	public string Description => "管理元素的分组状态，支持创建组和解散组";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\"group\", \"ungroup\"],\n                \"description\": \"分组操作类型：group(将多个元素组合成一个组)、ungroup(将组解散，释放其中的所有元素)\"\n            },\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"单个元素 ID（仅 group 操作使用，可选）。至少需要 2 个元素才能创建组，建议使用 elementIds 或 cacheId 参数\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（仅 group 操作使用，可选）。批量组合多个元素，例如：[12345, 12346, 12347]\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（仅 group 操作使用，可选）。从上一个查询工具（如 element_query）的返回结果中获取 cache_id 字段。使用缓存可以批量操作之前查询到的所有元素。注意：只需提供单个 cache_id 字符串，不需要数组。\"\n            },\n            \"groupName\": {\n                \"type\": \"string\",\n                \"description\": \"组名称（仅 group 操作使用，建议提供）。请提供有意义的名称来描述组的用途，例如：'桩号标注组'、'一层柱'、'南立面墙体' 等。如果不提供，Revit 会自动生成 'Group 1'、'Group 2' 等默认名称，不利于后续识别和管理。\"\n            },\n            \"groupId\": {\n                \"type\": \"integer\",\n                \"description\": \"组元素 ID（仅 ungroup 操作使用，必选）。需要解组的组元素的 ID。\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[AsyncStateMachine(typeof(Class445))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class445 stateMachine = new Class445();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementGroupManagerTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class446))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0)
	{
		Class446 stateMachine = new Class446();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementGroupManagerTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class447))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0)
	{
		Class447 stateMachine = new Class447();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementGroupManagerTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
