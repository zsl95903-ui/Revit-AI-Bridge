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

[AITool("set_element_color", Category = "视图高级操作", Description = "给 Revit 元素着色。支持单个元素、元素数组、缓存 ID 输入方式。可设置颜色和实体填充图案突出显示元素，或清除元素着色恢复为按材质显示。适用于碰撞检查后的结果突出显示等场景。", RequiresTransaction = true, RequiresModification = true)]
public sealed class SetElementColorTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class628 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IViewService iviewService_0;

		public object object_0;

		public List<int> list_0;

		public SetElementColorTool setElementColorTool_0;

		private bool bool_0;

		private string string_0;

		private Exception exception_0;

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
					Class628 stateMachine = this;
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
			try
			{
				bool_0 = iviewService_0.ClearElementOverrides(aitoolContext_0.Document, object_0, (IEnumerable<int>)list_0);
				if (bool_0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
					defaultInterpolatedStringHandler.AppendLiteral("成功清除 ");
					defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个元素的着色");
					string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
					Logger.Info("[SetElementColorTool] " + string_0);
					result = AIToolResult.Ok(string_0, (object)new Class295<int, List<int>, string>(list_0.Count, list_0, "clear"));
				}
				else
				{
					result = AIToolResult.Fail("清除元素着色失败");
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[SetElementColorTool] 清除着色失败: " + exception_0.Message);
				result = AIToolResult.Fail("清除着色失败: " + exception_0.Message);
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
	public sealed class Class629 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SetElementColorTool setElementColorTool_0;

		private object object_0;

		private string string_0;

		private int? nullable_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private List<int> list_0;

		private object object_1;

		private List<int> list_1;

		private object object_2;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<List<int>> taskAwaiter_1;

		private TaskAwaiter<AIToolResult> taskAwaiter_2;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 2u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class629 stateMachine = this;
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
				TaskAwaiter<List<int>> awaiter4;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter2;
				string text;
				string text2;
				switch (num)
				{
				default:
				{
					object_0 = aitoolContext_0.GetParameter<object>("elements", (object)null);
					string_0 = aitoolContext_0.GetParameter<string>("action", "color");
					nullable_0 = aitoolContext_0.GetParameter<int?>("viewId", (int?)null);
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
					if (iviewService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ViewService");
					}
					else if (ielementService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ElementService");
					}
					else
					{
						if (aitoolContext_0.Document != null)
						{
							awaiter4 = setElementColorTool_0.method_0(aitoolContext_0, ielementService_0, object_0).GetAwaiter();
							if (!awaiter4.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter4;
								Class629 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
								return;
							}
							goto IL_01fa;
						}
						result = AIToolResult.Fail("文档对象为空");
					}
					goto end_IL_006e;
				}
				case 1:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<List<int>>);
					num = -1;
					int_0 = -1;
					goto IL_01fa;
				case 2:
					awaiter3 = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0460;
				case 3:
					{
						awaiter2 = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0325:
					text = string_0.ToLower();
					string_1 = text;
					text2 = string_1;
					if (text2 == "color")
					{
						awaiter3 = setElementColorTool_0.method_2(aitoolContext_0, iviewService_0, object_1, list_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_2 = awaiter3;
							Class629 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
							return;
						}
						goto IL_0460;
					}
					if (text2 == "clear")
					{
						awaiter2 = setElementColorTool_0.method_3(aitoolContext_0, iviewService_0, object_1, list_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 3;
							int_0 = 3;
							taskAwaiter_2 = awaiter2;
							Class629 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
							return;
						}
						break;
					}
					result = AIToolResult.Fail("不支持的操作类型: " + string_0);
					goto end_IL_006e;
					IL_0460:
					aitoolResult_0 = awaiter3.GetResult();
					result = aitoolResult_0;
					goto end_IL_006e;
					IL_01fa:
					list_1 = awaiter4.GetResult();
					list_0 = list_1;
					list_1 = null;
					if (list_0.Count == 0)
					{
						result = AIToolResult.Fail("无法解析元素：请提供有效的元素 ID、ID 数组或缓存 ID");
					}
					else if (nullable_0.HasValue)
					{
						object_2 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_0.Value);
						if (object_2 != null)
						{
							object_1 = object_2;
							object_2 = null;
							goto IL_0325;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(nullable_0.Value);
						defaultInterpolatedStringHandler.AppendLiteral(" 的视图");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						object_1 = iviewService_0.GetActiveView(aitoolContext_0.Document);
						if (object_1 != null)
						{
							goto IL_0325;
						}
						result = AIToolResult.Fail("无法获取当前活动视图");
					}
					goto end_IL_006e;
				}
				aitoolResult_1 = awaiter2.GetResult();
				result = aitoolResult_1;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[SetElementColorTool] 操作失败: " + exception_0.Message);
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
	public sealed class Class630 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<int>> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public object object_0;

		public SetElementColorTool setElementColorTool_0;

		private List<int> list_0;

		private long long_0;

		private int int_1;

		private IEnumerable ienumerable_0;

		private string string_0;

		private IEnumerator ienumerator_0;

		private object object_1;

		private IConvertible iconvertible_0;

		private int int_2;

		private object object_2;

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
					Class630 stateMachine = this;
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
			list_0 = new List<int>();
			List<int> result;
			try
			{
				if (object_0 is long)
				{
					long_0 = (long)object_0;
					if (long_0 > 0L)
					{
						list_0.Add((int)long_0);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[SetElementColorTool] 单个元素 ID ");
						defaultInterpolatedStringHandler.AppendFormatted(long_0);
						Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
						result = list_0;
						goto IL_0406;
					}
				}
				if (object_0 is int)
				{
					int_1 = (int)object_0;
					if (int_1 > 0)
					{
						list_0.Add(int_1);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(30, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("[SetElementColorTool] 单个元素 ID ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_1);
						Logger.Debug(defaultInterpolatedStringHandler2.ToStringAndClear());
						result = list_0;
						goto IL_0406;
					}
				}
				ienumerable_0 = object_0 as IEnumerable;
				if (ienumerable_0 != null && !(object_0 is string))
				{
					ienumerator_0 = ienumerable_0.GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							object_1 = ienumerator_0.Current;
							if (object_1 != null)
							{
								iconvertible_0 = object_1 as IConvertible;
								if (iconvertible_0 != null)
								{
									try
									{
										int_2 = Convert.ToInt32(iconvertible_0);
										if (int_2 > 0)
										{
											list_0.Add(int_2);
										}
									}
									catch
									{
									}
								}
								iconvertible_0 = null;
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
					list_0 = list_0.Distinct().ToList();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(35, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("[SetElementColorTool] 元素 ID 数组，共 ");
					defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个");
					Logger.Debug(defaultInterpolatedStringHandler3.ToStringAndClear());
					result = list_0;
					goto IL_0406;
				}
				string_0 = object_0 as string;
				if (string_0 != null && !string.IsNullOrEmpty(string_0))
				{
					object_2 = aitoolContext_0.GetCachedData<object>(string_0);
					if (object_2 != null)
					{
						list_0 = setElementColorTool_0.method_1(object_2);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(39, 2);
						defaultInterpolatedStringHandler4.AppendLiteral("[SetElementColorTool] 缓存 ID '");
						defaultInterpolatedStringHandler4.AppendFormatted(string_0);
						defaultInterpolatedStringHandler4.AppendLiteral("'，提取到 ");
						defaultInterpolatedStringHandler4.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler4.AppendLiteral(" 个元素");
						Logger.Debug(defaultInterpolatedStringHandler4.ToStringAndClear());
						result = list_0;
						goto IL_0406;
					}
					object_2 = null;
				}
				ienumerable_0 = null;
				string_0 = null;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Warning("[SetElementColorTool] 解析元素 ID 失败: " + exception_0.Message);
			}
			result = list_0;
			goto IL_0406;
			IL_0406:
			int_0 = -2;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class631 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IViewService iviewService_0;

		public object object_0;

		public List<int> list_0;

		public SetElementColorTool setElementColorTool_0;

		private object object_1;

		private int? nullable_0;

		private bool bool_0;

		private string string_0;

		private Exception exception_0;

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
					Class631 stateMachine = this;
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
			try
			{
				object_1 = aitoolContext_0.GetParameter<object>("color", (object)null);
				nullable_0 = aitoolContext_0.GetParameter<int?>("transparency", (int?)null);
				if (object_1 == null)
				{
					result = AIToolResult.Fail("设置颜色需要提供 color 参数");
				}
				else
				{
					bool_0 = iviewService_0.OverrideElementColors(aitoolContext_0.Document, object_0, (IEnumerable<int>)list_0, object_1, nullable_0);
					if (bool_0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler.AppendLiteral("成功为 ");
						defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个元素设置颜色");
						string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
						if (nullable_0.HasValue)
						{
							string text = string_0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("，透明度 ");
							defaultInterpolatedStringHandler2.AppendFormatted(nullable_0.Value);
							defaultInterpolatedStringHandler2.AppendLiteral("%");
							string_0 = text + defaultInterpolatedStringHandler2.ToStringAndClear();
						}
						Logger.Info("[SetElementColorTool] " + string_0);
						result = AIToolResult.Ok(string_0, (object)new Class295<int, List<int>, string>(list_0.Count, list_0, "color"));
					}
					else
					{
						result = AIToolResult.Fail("设置元素颜色失败");
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[SetElementColorTool] 设置颜色失败: " + exception_0.Message);
				result = AIToolResult.Fail("设置颜色失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "set_element_color";

	public string Category => "视图高级操作";

	public string Description => "给 Revit 元素着色，支持设置颜色和填充图案，或清除着色";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"elements\": {\n                \"description\": \"目标元素：可以是单个元素 ID、元素 ID 数组或缓存 ID（着色或清除着色的目标）\",\n                \"oneOf\": [\n                    { \"type\": \"integer\", \"description\": \"单个元素 ID\" },\n                    { \"type\": \"array\", \"items\": { \"type\": \"integer\" }, \"description\": \"元素 ID 数组\" },\n                    { \"type\": \"string\", \"description\": \"元素缓存 ID\" }\n                ]\n            },\n            \"action\": {\n                \"type\": \"string\",\n                \"description\": \"操作类型：color=设置颜色和实体填充图案，clear=清除着色恢复为按材质显示\",\n                \"enum\": [\"color\", \"clear\"],\n                \"default\": \"color\"\n            },\n            \"color\": {\n                \"type\": \"object\",\n                \"properties\": {\n                    \"red\": {\n                        \"type\": \"integer\",\n                        \"minimum\": 0,\n                        \"maximum\": 255,\n                        \"description\": \"红色分量 (0-255)\"\n                    },\n                    \"green\": {\n                        \"type\": \"integer\",\n                        \"minimum\": 0,\n                        \"maximum\": 255,\n                        \"description\": \"绿色分量 (0-255)\"\n                    },\n                    \"blue\": {\n                        \"type\": \"integer\",\n                        \"minimum\": 0,\n                        \"maximum\": 255,\n                        \"description\": \"蓝色分量 (0-255)\"\n                    }\n                },\n                \"required\": [\"red\", \"green\", \"blue\"],\n                \"description\": \"着色颜色（RGB）。仅当 action 为 color 时有效。\"\n            },\n            \"transparency\": {\n                \"type\": \"integer\",\n                \"minimum\": 0,\n                \"maximum\": 100,\n                \"description\": \"透明度（0-100，0=完全不透明，100=完全透明）。可选。仅当 action 为 color 时有效。\"\n            },\n            \"viewId\": {\n                \"type\": \"integer\",\n                \"description\": \"视图 ID（可选，默认使用当前活动视图）\"\n            }\n        },\n        \"required\": [\"elements\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class629))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class629 stateMachine = new Class629();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setElementColorTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class630))]
	[DebuggerStepThrough]
	private Task<List<int>> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0, object object_0)
	{
		Class630 stateMachine = new Class630();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<int>>.Create();
		stateMachine.setElementColorTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.object_0 = object_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<int> method_1(object object_0)
	{
		List<int> list = new List<int>();
		try
		{
			if (object_0 is IEnumerable enumerable)
			{
				foreach (object item in enumerable)
				{
					if (item == null)
					{
						continue;
					}
					PropertyInfo property = item.GetType().GetProperty("id");
					if (property != null)
					{
						object value = property.GetValue(item);
						if (value is int num && num > 0)
						{
							list.Add(num);
						}
						else if (value is long num2 && num2 > 0L)
						{
							list.Add((int)num2);
						}
					}
					PropertyInfo property2 = item.GetType().GetProperty("elementId");
					if (property2 != null)
					{
						object value2 = property2.GetValue(item);
						if (value2 is int num3 && num3 > 0)
						{
							list.Add(num3);
						}
						else if (value2 is long num4 && num4 > 0L)
						{
							list.Add((int)num4);
						}
					}
					PropertyInfo property3 = item.GetType().GetProperty("source_element_id");
					if (property3 != null)
					{
						object value3 = property3.GetValue(item);
						if (value3 is int num5 && num5 > 0)
						{
							list.Add(num5);
						}
						else if (value3 is long num6 && num6 > 0L)
						{
							list.Add((int)num6);
						}
					}
					PropertyInfo property4 = item.GetType().GetProperty("target_element_id");
					if (property4 != null)
					{
						object value4 = property4.GetValue(item);
						if (value4 is int num7 && num7 > 0)
						{
							list.Add(num7);
						}
						else if (value4 is long num8 && num8 > 0L)
						{
							list.Add((int)num8);
						}
					}
				}
			}
			list = list.Distinct().ToList();
		}
		catch (Exception ex)
		{
			Logger.Warning("[SetElementColorTool] 提取元素 ID 失败: " + ex.Message);
		}
		return list;
	}

	[AsyncStateMachine(typeof(Class631))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, IViewService iviewService_0, object object_0, List<int> list_0)
	{
		Class631 stateMachine = new Class631();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setElementColorTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.iviewService_0 = iviewService_0;
		stateMachine.object_0 = object_0;
		stateMachine.list_0 = list_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class628))]
	private Task<AIToolResult> method_3(AIToolContext aitoolContext_0, IViewService iviewService_0, object object_0, List<int> list_0)
	{
		Class628 stateMachine = new Class628();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setElementColorTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.iviewService_0 = iviewService_0;
		stateMachine.object_0 = object_0;
		stateMachine.list_0 = list_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
