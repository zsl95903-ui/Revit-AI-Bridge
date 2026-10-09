using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

[AITool("element_lock_manager", Category = "元素操作", Description = "管理元素的锁定状态。支持锁定元素（防止被选择和修改）和解锁元素（恢复选择和修改功能）。支持通过缓存 ID、单个元素 ID 或元素 ID 数组来指定构件。", RequiresTransaction = true, RequiresModification = true)]
public sealed class ElementLockManagerTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class448 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ElementLockManagerTool elementLockManagerTool_0;

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
					Class448 stateMachine = this;
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
						Logger.Info("[ElementLockManagerTool] 执行操作: " + string_0);
						string_1 = string_0.ToLower();
						string text = string_1;
						if (!(text == "lock"))
						{
							if (!(text == "unlock"))
							{
								val = AIToolResult.Fail("不支持的操作类型: " + string_0);
								goto IL_025f;
							}
							awaiter3 = elementLockManagerTool_0.method_1(aitoolContext_0).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = 2;
								int_0 = 2;
								taskAwaiter_1 = awaiter3;
								Class448 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
								return;
							}
							goto IL_0209;
						}
						awaiter2 = elementLockManagerTool_0.method_0(aitoolContext_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter2;
							Class448 stateMachine = this;
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
				Logger.Error("[ElementLockManagerTool] 执行失败: " + exception_0.Message);
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
	public sealed class Class449 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public ElementLockManagerTool elementLockManagerTool_0;

		private IElementService ielementService_0;

		private List<int> list_0;

		private List<int> list_1;

		private List<int> list_2;

		private List<string> list_3;

		private string string_0;

		private List<int>.Enumerator enumerator_0;

		private int int_1;

		private object object_0;

		private string string_1;

		private bool bool_0;

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
					Class449 stateMachine = this;
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
			IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
			ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
			AIToolResult result;
			if (ielementService_0 == null)
			{
				result = AIToolResult.Fail("无法获取 ElementService");
			}
			else
			{
				list_0 = elementLockManagerTool_0.method_2(aitoolContext_0);
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("请提供 cacheId、elementId 或 elementIds 参数");
				}
				else
				{
					list_1 = new List<int>();
					list_2 = new List<int>();
					list_3 = new List<string>();
					enumerator_0 = list_0.GetEnumerator();
					try
					{
						while (enumerator_0.MoveNext())
						{
							int_1 = enumerator_0.Current;
							try
							{
								object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
								if (object_0 == null)
								{
									list_2.Add(int_1);
									continue;
								}
								string text = elementLockManagerTool_0.method_5(object_0);
								if (text == null)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
									defaultInterpolatedStringHandler.AppendLiteral("#");
									defaultInterpolatedStringHandler.AppendFormatted(int_1);
									text = defaultInterpolatedStringHandler.ToStringAndClear();
								}
								string_1 = text;
								list_3.Add(string_1);
								bool_0 = ielementService_0.LockElement(aitoolContext_0.Document, int_1);
								if (bool_0)
								{
									list_1.Add(int_1);
								}
								else
								{
									list_2.Add(int_1);
								}
								object_0 = null;
								string_1 = null;
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(41, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("[ElementLockManagerTool] Lock: 锁定元素 ");
								defaultInterpolatedStringHandler2.AppendFormatted(int_1);
								defaultInterpolatedStringHandler2.AppendLiteral(" 失败: ");
								defaultInterpolatedStringHandler2.AppendFormatted(exception_0.Message);
								Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
								list_2.Add(int_1);
							}
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
						result = AIToolResult.Fail("没有任何元素被锁定，请检查元素 ID 是否正确");
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(46, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("[ElementLockManagerTool] Lock: 成功锁定 ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个元素，失败 ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_2.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个");
						Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(10, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("✅ 已锁定 ");
						defaultInterpolatedStringHandler4.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler4.AppendLiteral(" 个构件");
						string_0 = defaultInterpolatedStringHandler4.ToStringAndClear();
						if (list_2.Count > 0)
						{
							string text2 = string_0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(5, 1);
							defaultInterpolatedStringHandler5.AppendLiteral("，");
							defaultInterpolatedStringHandler5.AppendFormatted(list_2.Count);
							defaultInterpolatedStringHandler5.AppendLiteral(" 个失败");
							string_0 = text2 + defaultInterpolatedStringHandler5.ToStringAndClear();
						}
						result = AIToolResult.Ok(string_0, (object)new Class116<string, int, int, List<string>, int[], int[]>("lock", list_1.Count, list_2.Count, list_3, list_1.ToArray(), list_2.ToArray()));
					}
				}
			}
			int_0 = -2;
			ielementService_0 = null;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			list_3 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class450 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public ElementLockManagerTool elementLockManagerTool_0;

		private IElementService ielementService_0;

		private List<int> list_0;

		private List<int> list_1;

		private List<int> list_2;

		private List<string> list_3;

		private string string_0;

		private List<int>.Enumerator enumerator_0;

		private int int_1;

		private object object_0;

		private string string_1;

		private bool bool_0;

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
					Class450 stateMachine = this;
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
			IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
			ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
			AIToolResult result;
			if (ielementService_0 == null)
			{
				result = AIToolResult.Fail("无法获取 ElementService");
			}
			else
			{
				list_0 = elementLockManagerTool_0.method_2(aitoolContext_0);
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("请提供 cacheId、elementId 或 elementIds 参数");
				}
				else
				{
					list_1 = new List<int>();
					list_2 = new List<int>();
					list_3 = new List<string>();
					enumerator_0 = list_0.GetEnumerator();
					try
					{
						while (enumerator_0.MoveNext())
						{
							int_1 = enumerator_0.Current;
							try
							{
								object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
								if (object_0 == null)
								{
									list_2.Add(int_1);
									continue;
								}
								string text = elementLockManagerTool_0.method_5(object_0);
								if (text == null)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
									defaultInterpolatedStringHandler.AppendLiteral("#");
									defaultInterpolatedStringHandler.AppendFormatted(int_1);
									text = defaultInterpolatedStringHandler.ToStringAndClear();
								}
								string_1 = text;
								list_3.Add(string_1);
								bool_0 = ielementService_0.UnlockElement(aitoolContext_0.Document, int_1);
								if (bool_0)
								{
									list_1.Add(int_1);
								}
								else
								{
									list_2.Add(int_1);
								}
								object_0 = null;
								string_1 = null;
							}
							catch (Exception ex)
							{
								exception_0 = ex;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("[ElementLockManagerTool] Unlock: 解锁元素 ");
								defaultInterpolatedStringHandler2.AppendFormatted(int_1);
								defaultInterpolatedStringHandler2.AppendLiteral(" 失败: ");
								defaultInterpolatedStringHandler2.AppendFormatted(exception_0.Message);
								Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
								list_2.Add(int_1);
							}
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
						result = AIToolResult.Fail("没有任何元素被解锁，请检查元素 ID 是否正确或元素是否已被锁定");
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(48, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("[ElementLockManagerTool] Unlock: 成功解锁 ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个元素，失败 ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_2.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个");
						Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(10, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("✅ 已解锁 ");
						defaultInterpolatedStringHandler4.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler4.AppendLiteral(" 个构件");
						string_0 = defaultInterpolatedStringHandler4.ToStringAndClear();
						if (list_2.Count > 0)
						{
							string text2 = string_0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(5, 1);
							defaultInterpolatedStringHandler5.AppendLiteral("，");
							defaultInterpolatedStringHandler5.AppendFormatted(list_2.Count);
							defaultInterpolatedStringHandler5.AppendLiteral(" 个失败");
							string_0 = text2 + defaultInterpolatedStringHandler5.ToStringAndClear();
						}
						result = AIToolResult.Ok(string_0, (object)new Class117<string, int, int, List<string>, int[], int[]>("unlock", list_1.Count, list_2.Count, list_3, list_1.ToArray(), list_2.ToArray()));
					}
				}
			}
			int_0 = -2;
			ielementService_0 = null;
			list_0 = null;
			list_1 = null;
			list_2 = null;
			list_3 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "element_lock_manager";

	public string Category => "元素操作";

	public string Description => "管理元素的锁定状态，支持锁定和解锁操作";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\"lock\", \"unlock\"],\n                \"description\": \"锁定操作类型：lock(锁定元素，防止被选择和修改)、unlock(解锁元素，恢复选择和修改功能)\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）。如果要操作之前查询缓存的元素，使用此参数。\"\n            },\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"要操作的单个元素 ID（可选）。\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"要操作的多个元素 ID 列表（可选）。\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class448))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class448 stateMachine = new Class448();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementLockManagerTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class449))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0)
	{
		Class449 stateMachine = new Class449();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementLockManagerTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class450))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0)
	{
		Class450 stateMachine = new Class450();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementLockManagerTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<int> method_2(AIToolContext aitoolContext_0)
	{
		List<int> list = new List<int>();
		if (aitoolContext_0.HasParameter("cacheId"))
		{
			string parameter = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
			if (!string.IsNullOrEmpty(parameter))
			{
				if (aitoolContext_0.DataCache == null)
				{
					return list;
				}
				object obj = aitoolContext_0.DataCache.Retrieve<object>(parameter);
				if (obj != null)
				{
					list = method_3(obj);
				}
			}
		}
		else if (aitoolContext_0.HasParameter("elementId"))
		{
			int parameter2 = aitoolContext_0.GetParameter<int>("elementId", 0);
			if (parameter2 > 0)
			{
				list.Add(parameter2);
			}
		}
		else if (aitoolContext_0.HasParameter("elementIds"))
		{
			int[] parameter3 = aitoolContext_0.GetParameter<int[]>("elementIds", (int[])null);
			if (parameter3 != null && parameter3.Length != 0)
			{
				list.AddRange(parameter3);
			}
		}
		return list;
	}

	private List<int> method_3(object object_0)
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
						int? num = method_4(item);
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

	private int? method_4(object object_0)
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

	private string? method_5(object object_0)
	{
		try
		{
			Type type = object_0.GetType();
			PropertyInfo property = type.GetProperty("Name");
			if (property != null)
			{
				object value = property.GetValue(object_0);
				if (value is string text && !string.IsNullOrWhiteSpace(text))
				{
					return text;
				}
			}
		}
		catch
		{
		}
		return null;
	}
}
