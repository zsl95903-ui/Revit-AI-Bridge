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
using Microsoft.CSharp.RuntimeBinder;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("element_query", Category = "元素查询", Description = "查询元素信息。支持按 ID 查询（单个或批量）、获取选中元素、按类别查询、按族类型查询。", RequiresTransaction = false, RequiresModification = false)]
public sealed class ElementQueryTool : IAITool
{
	[CompilerGenerated]
	private static class Class451
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class452
	{
		public List<object> list_0;

		internal string method_0(string string_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 2);
			defaultInterpolatedStringHandler.AppendLiteral("✅ 成功获取 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个选中元素\n\n💡 在后续工具调用中使用 cacheId=\"");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些元素");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[CompilerGenerated]
	public sealed class Class453
	{
		public List<object> list_0;

		public string string_0;

		internal string method_0(string string_1)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
			defaultInterpolatedStringHandler.AppendLiteral("✅ 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个 ");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("\n\n💡 在后续工具调用中使用 cacheId=\"");
			defaultInterpolatedStringHandler.AppendFormatted(string_1);
			defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些元素");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[CompilerGenerated]
	public sealed class Class454
	{
		public int int_0;

		public string string_0;

		public string string_1;

		internal string method_0(string string_2)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 4);
			defaultInterpolatedStringHandler.AppendLiteral("✅ 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(int_0);
			defaultInterpolatedStringHandler.AppendLiteral(" 个使用类型 '");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("' 的");
			defaultInterpolatedStringHandler.AppendFormatted(string_1);
			defaultInterpolatedStringHandler.AppendLiteral("实例\n\n💡 在后续工具调用中使用 cacheId=\"");
			defaultInterpolatedStringHandler.AppendFormatted(string_2);
			defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些元素");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[CompilerGenerated]
	public sealed class Class455
	{
		public List<int> list_0;

		public List<object> list_1;

		public List<int> list_2;

		internal string method_0(string string_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 2);
			defaultInterpolatedStringHandler.AppendLiteral("✅ 成功查询 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个元素，找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个");
			string text = defaultInterpolatedStringHandler.ToStringAndClear();
			if (list_2.Count > 0)
			{
				string text2 = text;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("，");
				defaultInterpolatedStringHandler2.AppendFormatted(list_2.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个未找到");
				text = text2 + defaultInterpolatedStringHandler2.ToStringAndClear();
			}
			return text + "\n\n💡 在后续工具调用中使用 cacheId=\"" + string_0 + "\" 参数来操作这些元素";
		}
	}

	[CompilerGenerated]
	private static class Class456
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;
	}

	[CompilerGenerated]
	private static class Class457
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, object>> callSite_1;

		public static CallSite<Func<CallSite, object, object>> callSite_2;
	}

	[CompilerGenerated]
	public sealed class Class458 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ElementQueryTool elementQueryTool_0;

		private IElementService ielementService_0;

		private IFamilyService ifamilyService_0;

		private ISelectionService iselectionService_0;

		private string string_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private AIToolResult aitoolResult_3;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 3u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class458 stateMachine = this;
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
				AIToolResult val;
				TaskAwaiter<AIToolResult> awaiter2;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter4;
				TaskAwaiter<AIToolResult> awaiter5;
				switch (num)
				{
				default:
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					ifamilyService_0 = ((revitAdapter2 != null) ? revitAdapter2.FamilyService : null);
					IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
					iselectionService_0 = ((revitAdapter3 != null) ? revitAdapter3.SelectionService : null);
					if (ielementService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ElementService");
					}
					else if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						string_0 = aitoolContext_0.GetParameter<string>("operation", (string)null);
						if (!string.IsNullOrEmpty(string_0))
						{
							string_1 = string_0.ToLower();
							string text = string_1;
							if (!(text == "by_id"))
							{
								if (!(text == "selected"))
								{
									if (!(text == "by_category"))
									{
										if (!(text == "by_family_type"))
										{
											val = AIToolResult.Fail("不支持的操作类型: " + string_0);
											break;
										}
										awaiter2 = elementQueryTool_0.method_3(aitoolContext_0, ielementService_0, ifamilyService_0).GetAwaiter();
										if (!awaiter2.IsCompleted)
										{
											num = 4;
											int_0 = 4;
											taskAwaiter_1 = awaiter2;
											Class458 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
											return;
										}
										goto IL_040d;
									}
									awaiter3 = elementQueryTool_0.method_2(aitoolContext_0, ielementService_0).GetAwaiter();
									if (!awaiter3.IsCompleted)
									{
										num = 3;
										int_0 = 3;
										taskAwaiter_1 = awaiter3;
										Class458 stateMachine = this;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
										return;
									}
									goto IL_03d2;
								}
								awaiter4 = elementQueryTool_0.method_1(aitoolContext_0, ielementService_0, iselectionService_0).GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = 2;
									int_0 = 2;
									taskAwaiter_1 = awaiter4;
									Class458 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
									return;
								}
								goto IL_0397;
							}
							awaiter5 = elementQueryTool_0.method_0(aitoolContext_0, ielementService_0).GetAwaiter();
							if (!awaiter5.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter5;
								Class458 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
								return;
							}
							goto IL_0359;
						}
						result = AIToolResult.Fail("必须指定 operation 参数");
					}
					goto end_IL_006e;
				}
				case 1:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0359;
				case 2:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0397;
				case 3:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03d2;
				case 4:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						goto IL_040d;
					}
					IL_040d:
					aitoolResult_3 = awaiter2.GetResult();
					val = aitoolResult_3;
					aitoolResult_3 = null;
					break;
					IL_0359:
					aitoolResult_0 = awaiter5.GetResult();
					val = aitoolResult_0;
					aitoolResult_0 = null;
					break;
					IL_0397:
					aitoolResult_1 = awaiter4.GetResult();
					val = aitoolResult_1;
					aitoolResult_1 = null;
					break;
					IL_03d2:
					aitoolResult_2 = awaiter3.GetResult();
					val = aitoolResult_2;
					aitoolResult_2 = null;
					break;
				}
				result = val;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("查询元素失败: " + exception_0.Message);
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
	public sealed class Class459 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public ElementQueryTool elementQueryTool_0;

		private Class452 class452_0;

		private MethodInfo methodInfo_0;

		private IEnumerable ienumerable_0;

		private string string_0;

		private IEnumerator ienumerator_0;

		private object object_2;

		private object object_3;

		private string string_1;

		private string string_2;

		private object object_4;

		private object object_5;

		private object object_6;

		private PropertyInfo propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private object object_7;

		private object object_8;

		private object object_9;

		private string string_3;

		private AIToolResult aitoolResult_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				class452_0 = new Class452();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class459 stateMachine = this;
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
			if (object_1 == null)
			{
				result = AIToolResult.Fail("无法获取 SelectionService");
			}
			else
			{
				methodInfo_0 = object_1.GetType().GetMethod("GetSelectedElements");
				ienumerable_0 = methodInfo_0?.Invoke(object_1, new object[1] { aitoolContext_0.Document }) as IEnumerable;
				class452_0.list_0 = new List<object>();
				if (ienumerable_0 != null)
				{
					ienumerator_0 = ienumerable_0.GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							object_2 = ienumerator_0.Current;
							if (object_2 == null)
							{
								continue;
							}
							object_3 = object_0.GetType().GetMethod("GetElementId")?.Invoke(object_0, new object[1] { object_2 });
							string_1 = object_0.GetType().GetMethod("GetElementCategory")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
							string_2 = object_0.GetType().GetMethod("GetElementName")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
							object_4 = object_0.GetType().GetMethod("GetElementLocation")?.Invoke(object_0, new object[1] { object_2 });
							object_5 = null;
							if (object_4 != null && object_4.GetType().GetProperty("Value") != null)
							{
								object_6 = object_4.GetType().GetProperty("Value")?.GetValue(object_4);
								if (object_6 != null)
								{
									propertyInfo_0 = object_6.GetType().GetProperty("X");
									propertyInfo_1 = object_6.GetType().GetProperty("Y");
									propertyInfo_2 = object_6.GetType().GetProperty("Z");
									if (propertyInfo_0 != null && propertyInfo_1 != null && propertyInfo_2 != null)
									{
										object_7 = propertyInfo_0.GetValue(object_6);
										object_8 = propertyInfo_1.GetValue(object_6);
										object_9 = propertyInfo_2.GetValue(object_6);
										object_5 = new
										{
											x = Math.Round(Convert.ToDouble(object_7) * 304.8, 0),
											y = Math.Round(Convert.ToDouble(object_8) * 304.8, 0),
											z = Math.Round(Convert.ToDouble(object_9) * 304.8, 0)
										};
										object_7 = null;
										object_8 = null;
										object_9 = null;
									}
									propertyInfo_0 = null;
									propertyInfo_1 = null;
									propertyInfo_2 = null;
								}
								object_6 = null;
							}
							class452_0.list_0.Add(new Class121<int, string, string, object>(Convert.ToInt32(object_3 ?? ((object)(-1))), string_1 ?? "未知", string_2 ?? "未命名", object_5));
							object_3 = null;
							string_1 = null;
							string_2 = null;
							object_4 = null;
							object_5 = null;
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
				if (!string.IsNullOrEmpty(aitoolContext_0.SessionId) && aitoolContext_0.DataCache != null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
					defaultInterpolatedStringHandler.AppendLiteral("selected_elements_");
					defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
					string_3 = defaultInterpolatedStringHandler.ToStringAndClear();
					aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)class452_0.list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_3, "个选中元素", 50);
					elementQueryTool_0.method_4(aitoolResult_0, delegate(string string_0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(49, 2);
						defaultInterpolatedStringHandler4.AppendLiteral("✅ 成功获取 ");
						defaultInterpolatedStringHandler4.AppendFormatted(class452_0.list_0.Count);
						defaultInterpolatedStringHandler4.AppendLiteral(" 个选中元素\n\n💡 在后续工具调用中使用 cacheId=\"");
						defaultInterpolatedStringHandler4.AppendFormatted(string_0);
						defaultInterpolatedStringHandler4.AppendLiteral("\" 参数来操作这些元素");
						return defaultInterpolatedStringHandler4.ToStringAndClear();
					});
					result = aitoolResult_0;
				}
				else
				{
					string text;
					if (class452_0.list_0.Count != 0)
					{
						if (class452_0.list_0.Count > 50)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(22, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("成功获取 ");
							defaultInterpolatedStringHandler2.AppendFormatted(class452_0.list_0.Count);
							defaultInterpolatedStringHandler2.AppendLiteral(" 个选中元素（已返回前50个摘要）");
							text = defaultInterpolatedStringHandler2.ToStringAndClear();
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(11, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("成功获取 ");
							defaultInterpolatedStringHandler3.AppendFormatted(class452_0.list_0.Count);
							defaultInterpolatedStringHandler3.AppendLiteral(" 个选中元素");
							text = defaultInterpolatedStringHandler3.ToStringAndClear();
						}
					}
					else
					{
						text = "当前未选中任何元素";
					}
					string_0 = text;
					result = AIToolResult.Ok(string_0, (object)new Class122<string, int, List<object>, string>("selected", class452_0.list_0.Count, class452_0.list_0.Take(50).ToList(), "millimeters"));
				}
			}
			int_0 = -2;
			class452_0 = null;
			methodInfo_0 = null;
			ienumerable_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class460 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public ElementQueryTool elementQueryTool_0;

		private Class453 class453_0;

		private MethodInfo methodInfo_0;

		private IEnumerable ienumerable_0;

		private string string_0;

		private IEnumerator ienumerator_0;

		private object object_1;

		private MethodInfo methodInfo_1;

		private bool? nullable_0;

		private object object_2;

		private string string_1;

		private string string_2;

		private object object_3;

		private object object_4;

		private object object_5;

		private PropertyInfo propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private object object_6;

		private object object_7;

		private object object_8;

		private string string_3;

		private AIToolResult aitoolResult_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				class453_0 = new Class453();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class460 stateMachine = this;
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
			class453_0.string_0 = aitoolContext_0.GetParameter<string>("categoryName", (string)null);
			AIToolResult result;
			if (string.IsNullOrEmpty(class453_0.string_0))
			{
				result = AIToolResult.Fail("类别名称不能为空");
			}
			else
			{
				methodInfo_0 = object_0.GetType().GetMethod("GetElementsByCategory");
				ienumerable_0 = methodInfo_0?.Invoke(object_0, new object[2] { aitoolContext_0.Document, class453_0.string_0 }) as IEnumerable;
				class453_0.list_0 = new List<object>();
				if (ienumerable_0 != null)
				{
					ienumerator_0 = ienumerable_0.GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							object_1 = ienumerator_0.Current;
							if (object_1 == null)
							{
								continue;
							}
							methodInfo_1 = object_0.GetType().GetMethod("IsElementType");
							nullable_0 = methodInfo_1?.Invoke(object_0, new object[1] { object_1 }) as bool?;
							if (nullable_0 == true)
							{
								continue;
							}
							object_2 = object_0.GetType().GetMethod("GetElementId")?.Invoke(object_0, new object[1] { object_1 });
							string_1 = object_0.GetType().GetMethod("GetElementName")?.Invoke(object_0, new object[1] { object_1 })?.ToString();
							string_2 = object_0.GetType().GetMethod("GetElementCategory")?.Invoke(object_0, new object[1] { object_1 })?.ToString();
							object_3 = object_0.GetType().GetMethod("GetElementLocation")?.Invoke(object_0, new object[1] { object_1 });
							object_4 = null;
							if (object_3 != null && object_3.GetType().GetProperty("Value") != null)
							{
								object_5 = object_3.GetType().GetProperty("Value")?.GetValue(object_3);
								if (object_5 != null)
								{
									propertyInfo_0 = object_5.GetType().GetProperty("X");
									propertyInfo_1 = object_5.GetType().GetProperty("Y");
									propertyInfo_2 = object_5.GetType().GetProperty("Z");
									if (propertyInfo_0 != null && propertyInfo_1 != null && propertyInfo_2 != null)
									{
										object_6 = propertyInfo_0.GetValue(object_5);
										object_7 = propertyInfo_1.GetValue(object_5);
										object_8 = propertyInfo_2.GetValue(object_5);
										object_4 = new
										{
											x = Math.Round(Convert.ToDouble(object_6) * 304.8, 0),
											y = Math.Round(Convert.ToDouble(object_7) * 304.8, 0),
											z = Math.Round(Convert.ToDouble(object_8) * 304.8, 0)
										};
										object_6 = null;
										object_7 = null;
										object_8 = null;
									}
									propertyInfo_0 = null;
									propertyInfo_1 = null;
									propertyInfo_2 = null;
								}
								object_5 = null;
							}
							class453_0.list_0.Add(new Class118<int, string, string, object>(Convert.ToInt32(object_2 ?? ((object)(-1))), string_1 ?? "未命名", string_2 ?? class453_0.string_0, object_4));
							methodInfo_1 = null;
							object_2 = null;
							string_1 = null;
							string_2 = null;
							object_3 = null;
							object_4 = null;
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
				if (!string.IsNullOrEmpty(aitoolContext_0.SessionId) && aitoolContext_0.DataCache != null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 2);
					defaultInterpolatedStringHandler.AppendLiteral("query_");
					defaultInterpolatedStringHandler.AppendFormatted(class453_0.string_0);
					defaultInterpolatedStringHandler.AppendLiteral("_");
					defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
					string_3 = defaultInterpolatedStringHandler.ToStringAndClear();
					aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)class453_0.list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_3, class453_0.string_0, 50);
					elementQueryTool_0.method_4(aitoolResult_0, delegate(string string_1)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(44, 3);
						defaultInterpolatedStringHandler4.AppendLiteral("✅ 找到 ");
						defaultInterpolatedStringHandler4.AppendFormatted(class453_0.list_0.Count);
						defaultInterpolatedStringHandler4.AppendLiteral(" 个 ");
						defaultInterpolatedStringHandler4.AppendFormatted(class453_0.string_0);
						defaultInterpolatedStringHandler4.AppendLiteral("\n\n💡 在后续工具调用中使用 cacheId=\"");
						defaultInterpolatedStringHandler4.AppendFormatted(string_1);
						defaultInterpolatedStringHandler4.AppendLiteral("\" 参数来操作这些元素");
						return defaultInterpolatedStringHandler4.ToStringAndClear();
					});
					result = aitoolResult_0;
				}
				else
				{
					if (class453_0.list_0.Count == 0)
					{
						string_0 = "找到 0 个 " + class453_0.string_0 + " 类别的元素";
					}
					else if (class453_0.list_0.Count <= 50)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(12, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("找到 ");
						defaultInterpolatedStringHandler2.AppendFormatted(class453_0.list_0.Count);
						defaultInterpolatedStringHandler2.AppendLiteral(" 个 ");
						defaultInterpolatedStringHandler2.AppendFormatted(class453_0.string_0);
						defaultInterpolatedStringHandler2.AppendLiteral(" 类别的元素");
						string_0 = defaultInterpolatedStringHandler2.ToStringAndClear();
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(23, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("找到 ");
						defaultInterpolatedStringHandler3.AppendFormatted(class453_0.list_0.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个 ");
						defaultInterpolatedStringHandler3.AppendFormatted(class453_0.string_0);
						defaultInterpolatedStringHandler3.AppendLiteral(" 类别的元素（已返回前50个摘要）");
						string_0 = defaultInterpolatedStringHandler3.ToStringAndClear();
					}
					result = AIToolResult.Ok(string_0, (object)new Class123<string, string, int, List<object>, string>("by_category", class453_0.string_0, class453_0.list_0.Count, class453_0.list_0.Take(50).ToList(), "millimeters"));
				}
			}
			int_0 = -2;
			class453_0 = null;
			methodInfo_0 = null;
			ienumerable_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class461 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public object object_1;

		public ElementQueryTool elementQueryTool_0;

		private Class454 class454_0;

		private int int_1;

		private string string_0;

		private MethodInfo methodInfo_0;

		private object object_2;

		private string string_1;

		private object object_3;

		private MethodInfo methodInfo_1;

		private IEnumerable ienumerable_0;

		private List<object> list_0;

		private List<object> list_1;

		private string string_2;

		private IEnumerator ienumerator_0;

		private object object_4;

		private MethodInfo methodInfo_2;

		private bool? nullable_0;

		private MethodInfo methodInfo_3;

		private object object_5;

		private object object_6;

		private object object_7;

		private string string_3;

		private object object_8;

		private object object_9;

		private object object_10;

		private PropertyInfo propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private object object_11;

		private object object_12;

		private object object_13;

		private string string_4;

		private AIToolResult aitoolResult_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				class454_0 = new Class454();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class461 stateMachine = this;
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
			if (object_1 == null)
			{
				result = AIToolResult.Fail("无法获取 FamilyService");
			}
			else
			{
				int_1 = aitoolContext_0.GetParameter<int>("typeId", 0);
				if (int_1 <= 0)
				{
					result = AIToolResult.Fail("typeId 必须大于 0");
				}
				else
				{
					string_0 = aitoolContext_0.GetParameter<string>("typeName", (string)null);
					methodInfo_0 = object_1.GetType().GetMethod("GetFamilyOfTypeId");
					object_2 = methodInfo_0?.Invoke(object_1, new object[2] { aitoolContext_0.Document, int_1 });
					if (object_2 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 的族类型对应的族");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						string_1 = object_0.GetType().GetMethod("GetElementName")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
						class454_0.string_1 = object_0.GetType().GetMethod("GetElementCategory")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
						object_3 = object_0.GetType().GetMethod("GetElementById")?.Invoke(object_0, new object[2] { aitoolContext_0.Document, int_1 });
						if (object_3 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(16, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("找不到 ID 为 ");
							defaultInterpolatedStringHandler2.AppendFormatted(int_1);
							defaultInterpolatedStringHandler2.AppendLiteral(" 的族类型元素");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
						}
						else
						{
							class454_0.string_0 = object_0.GetType().GetMethod("GetElementName")?.Invoke(object_0, new object[1] { object_3 })?.ToString();
							if (!string.IsNullOrEmpty(string_0) && !string.Equals(class454_0.string_0, string_0, StringComparison.OrdinalIgnoreCase))
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(19, 2);
								defaultInterpolatedStringHandler3.AppendLiteral("类型名称不匹配：期望 '");
								defaultInterpolatedStringHandler3.AppendFormatted(string_0);
								defaultInterpolatedStringHandler3.AppendLiteral("'，实际 '");
								defaultInterpolatedStringHandler3.AppendFormatted(class454_0.string_0);
								defaultInterpolatedStringHandler3.AppendLiteral("'");
								result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
							}
							else
							{
								methodInfo_1 = object_0.GetType().GetMethod("GetAllElements");
								ienumerable_0 = methodInfo_1?.Invoke(object_0, new object[1] { aitoolContext_0.Document }) as IEnumerable;
								if (ienumerable_0 == null)
								{
									result = AIToolResult.Fail("无法获取文档元素");
								}
								else
								{
									list_0 = new List<object>();
									class454_0.int_0 = 0;
									ienumerator_0 = ienumerable_0.GetEnumerator();
									try
									{
										while (ienumerator_0.MoveNext())
										{
											object_4 = ienumerator_0.Current;
											if (object_4 == null)
											{
												continue;
											}
											methodInfo_2 = object_0.GetType().GetMethod("IsElementType");
											nullable_0 = methodInfo_2?.Invoke(object_0, new object[1] { object_4 }) as bool?;
											if (nullable_0 == true)
											{
												continue;
											}
											methodInfo_3 = object_1.GetType().GetMethod("GetElementFamilySymbol");
											object_5 = methodInfo_3?.Invoke(object_1, new object[1] { object_4 });
											if (object_5 == null)
											{
												continue;
											}
											object_6 = object_0.GetType().GetMethod("GetElementId")?.Invoke(object_0, new object[1] { object_5 });
											if (object_6 == null)
											{
												continue;
											}
											if (Convert.ToInt32(object_6) == int_1)
											{
												int num2 = class454_0.int_0;
												class454_0.int_0 = num2 + 1;
												object_7 = object_0.GetType().GetMethod("GetElementId")?.Invoke(object_0, new object[1] { object_4 });
												string_3 = object_0.GetType().GetMethod("GetElementName")?.Invoke(object_0, new object[1] { object_4 })?.ToString();
												object_8 = object_0.GetType().GetMethod("GetElementLocation")?.Invoke(object_0, new object[1] { object_4 });
												object_9 = null;
												if (object_8 != null && object_8.GetType().GetProperty("Value") != null)
												{
													object_10 = object_8.GetType().GetProperty("Value")?.GetValue(object_8);
													if (object_10 != null)
													{
														propertyInfo_0 = object_10.GetType().GetProperty("X");
														propertyInfo_1 = object_10.GetType().GetProperty("Y");
														propertyInfo_2 = object_10.GetType().GetProperty("Z");
														if (propertyInfo_0 != null && propertyInfo_1 != null && propertyInfo_2 != null)
														{
															object_11 = propertyInfo_0.GetValue(object_10);
															object_12 = propertyInfo_1.GetValue(object_10);
															object_13 = propertyInfo_2.GetValue(object_10);
															object_9 = new
															{
																x = Math.Round(Convert.ToDouble(object_11) * 304.8, 0),
																y = Math.Round(Convert.ToDouble(object_12) * 304.8, 0),
																z = Math.Round(Convert.ToDouble(object_13) * 304.8, 0)
															};
															object_11 = null;
															object_12 = null;
															object_13 = null;
														}
														propertyInfo_0 = null;
														propertyInfo_1 = null;
														propertyInfo_2 = null;
													}
													object_10 = null;
												}
												list_0.Add(new Class124<int, string, string, string, object>(Convert.ToInt32(object_7 ?? ((object)(-1))), string_3 ?? "未命名", class454_0.string_0 ?? "未知类型", class454_0.string_1 ?? "未分类", object_9));
												object_7 = null;
												string_3 = null;
												object_8 = null;
												object_9 = null;
											}
											methodInfo_2 = null;
											methodInfo_3 = null;
											object_5 = null;
											object_6 = null;
											object_4 = null;
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
									list_1 = list_0.OrderBy(delegate(object object_0)
									{
										if (Class456.callSite_0 == null)
										{
											Class456.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "name", typeof(ElementQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
										}
										return (dynamic)Class456.callSite_0.Target(Class456.callSite_0, object_0);
									}).ToList();
									if (aitoolContext_0.DataCache != null && !string.IsNullOrEmpty(aitoolContext_0.SessionId) && list_1.Count > 0)
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(17, 1);
										defaultInterpolatedStringHandler4.AppendLiteral("elements_by_type_");
										defaultInterpolatedStringHandler4.AppendFormatted(int_1);
										string_4 = defaultInterpolatedStringHandler4.ToStringAndClear();
										aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_1, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_4, "元素实例", 50);
										elementQueryTool_0.method_4(aitoolResult_0, delegate(string string_2)
										{
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(54, 4);
											defaultInterpolatedStringHandler7.AppendLiteral("✅ 找到 ");
											defaultInterpolatedStringHandler7.AppendFormatted(class454_0.int_0);
											defaultInterpolatedStringHandler7.AppendLiteral(" 个使用类型 '");
											defaultInterpolatedStringHandler7.AppendFormatted(class454_0.string_0);
											defaultInterpolatedStringHandler7.AppendLiteral("' 的");
											defaultInterpolatedStringHandler7.AppendFormatted(class454_0.string_1);
											defaultInterpolatedStringHandler7.AppendLiteral("实例\n\n💡 在后续工具调用中使用 cacheId=\"");
											defaultInterpolatedStringHandler7.AppendFormatted(string_2);
											defaultInterpolatedStringHandler7.AppendLiteral("\" 参数来操作这些元素");
											return defaultInterpolatedStringHandler7.ToStringAndClear();
										});
										result = aitoolResult_0;
									}
									else
									{
										string text;
										if (class454_0.int_0 != 0)
										{
											if (class454_0.int_0 > 50)
											{
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(27, 3);
												defaultInterpolatedStringHandler5.AppendLiteral("找到 ");
												defaultInterpolatedStringHandler5.AppendFormatted(class454_0.int_0);
												defaultInterpolatedStringHandler5.AppendLiteral(" 个使用类型 '");
												defaultInterpolatedStringHandler5.AppendFormatted(class454_0.string_0);
												defaultInterpolatedStringHandler5.AppendLiteral("' 的");
												defaultInterpolatedStringHandler5.AppendFormatted(class454_0.string_1);
												defaultInterpolatedStringHandler5.AppendLiteral("实例（已返回前50个摘要）");
												text = defaultInterpolatedStringHandler5.ToStringAndClear();
											}
											else
											{
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(16, 3);
												defaultInterpolatedStringHandler6.AppendLiteral("找到 ");
												defaultInterpolatedStringHandler6.AppendFormatted(class454_0.int_0);
												defaultInterpolatedStringHandler6.AppendLiteral(" 个使用类型 '");
												defaultInterpolatedStringHandler6.AppendFormatted(class454_0.string_0);
												defaultInterpolatedStringHandler6.AppendLiteral("' 的");
												defaultInterpolatedStringHandler6.AppendFormatted(class454_0.string_1);
												defaultInterpolatedStringHandler6.AppendLiteral("实例");
												text = defaultInterpolatedStringHandler6.ToStringAndClear();
											}
										}
										else
										{
											text = "没有找到使用类型 '" + class454_0.string_0 + "' 的元素实例";
										}
										string_2 = text;
										result = AIToolResult.Ok(string_2, (object)new Class125<string, int, int, string, string, string, IEnumerable<object>>("by_family_type", class454_0.int_0, int_1, class454_0.string_0 ?? "未知", string_1 ?? "未知", class454_0.string_1 ?? "未分类", list_1.Take(50)));
									}
								}
							}
						}
					}
				}
			}
			int_0 = -2;
			class454_0 = null;
			string_0 = null;
			methodInfo_0 = null;
			object_2 = null;
			string_1 = null;
			object_3 = null;
			methodInfo_1 = null;
			ienumerable_0 = null;
			list_0 = null;
			list_1 = null;
			string_2 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class462 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public ElementQueryTool elementQueryTool_0;

		private Class455 class455_0;

		private string string_0;

		private int int_1;

		private object object_1;

		private int int_2;

		private long long_0;

		private int[] int_3;

		private long[] long_1;

		private List<int> list_0;

		private List<long> list_1;

		private List<object> list_2;

		private List<int>.Enumerator enumerator_0;

		private int int_4;

		private object object_2;

		private string string_1;

		private string string_2;

		private object object_3;

		private object object_4;

		private object object_5;

		private PropertyInfo propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private object object_6;

		private object object_7;

		private object object_8;

		private string string_3;

		private AIToolResult aitoolResult_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				class455_0 = new Class455();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class462 stateMachine = this;
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
			class455_0.list_0 = new List<int>();
			if (aitoolContext_0.HasParameter("elementId"))
			{
				int_1 = aitoolContext_0.GetParameter<int>("elementId", 0);
				if (int_1 > 0)
				{
					class455_0.list_0.Add(int_1);
				}
			}
			AIToolResult result;
			if (aitoolContext_0.HasParameter("elementIds"))
			{
				object_1 = aitoolContext_0.GetParameter<object>("elementIds", (object)null);
				if (object_1 is int)
				{
					int_2 = (int)object_1;
					if (true)
					{
						class455_0.list_0.Add(int_2);
						goto IL_033f;
					}
				}
				if (object_1 is long)
				{
					long_0 = (long)object_1;
					if (true)
					{
						class455_0.list_0.Add((int)long_0);
						goto IL_033f;
					}
				}
				int_3 = object_1 as int[];
				if (int_3 != null)
				{
					class455_0.list_0.AddRange(int_3);
				}
				else
				{
					long_1 = object_1 as long[];
					if (long_1 != null)
					{
						class455_0.list_0.AddRange(Array.ConvertAll(long_1, (long long_0) => (int)long_0));
					}
					else
					{
						list_0 = object_1 as List<int>;
						if (list_0 != null)
						{
							class455_0.list_0.AddRange(list_0);
						}
						else
						{
							list_1 = object_1 as List<long>;
							if (list_1 != null)
							{
								class455_0.list_0.AddRange(list_1.ConvertAll((long long_0) => (int)long_0));
							}
							else
							{
								list_2 = object_1 as List<object>;
								if (list_2 != null)
								{
									try
									{
										class455_0.list_0.AddRange(Array.ConvertAll(list_2.ToArray(), Convert.ToInt32));
									}
									catch
									{
										result = AIToolResult.Fail("elementIds 数组中包含非整数值");
										goto IL_0c20;
									}
								}
								list_2 = null;
							}
							list_1 = null;
						}
						list_0 = null;
					}
					long_1 = null;
				}
				int_3 = null;
				goto IL_033f;
			}
			goto IL_0366;
			IL_0366:
			if (class455_0.list_0.Count == 0)
			{
				result = AIToolResult.Fail("by_id 操作需要提供 elementId 或 elementIds 参数");
			}
			else
			{
				class455_0.list_1 = new List<object>();
				class455_0.list_2 = new List<int>();
				enumerator_0 = class455_0.list_0.GetEnumerator();
				try
				{
					while (enumerator_0.MoveNext())
					{
						int_4 = enumerator_0.Current;
						object_2 = object_0.GetType().GetMethod("GetElementById")?.Invoke(object_0, new object[2] { aitoolContext_0.Document, int_4 });
						if (object_2 == null)
						{
							class455_0.list_2.Add(int_4);
							continue;
						}
						string_1 = object_0.GetType().GetMethod("GetElementCategory")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
						string_2 = object_0.GetType().GetMethod("GetElementName")?.Invoke(object_0, new object[1] { object_2 })?.ToString();
						object_3 = object_0.GetType().GetMethod("GetElementLocation")?.Invoke(object_0, new object[1] { object_2 });
						object_4 = null;
						if (object_3 != null && object_3.GetType().GetProperty("Value") != null)
						{
							object_5 = object_3.GetType().GetProperty("Value")?.GetValue(object_3);
							if (object_5 != null)
							{
								propertyInfo_0 = object_5.GetType().GetProperty("X");
								propertyInfo_1 = object_5.GetType().GetProperty("Y");
								propertyInfo_2 = object_5.GetType().GetProperty("Z");
								if (propertyInfo_0 != null && propertyInfo_1 != null && propertyInfo_2 != null)
								{
									object_6 = propertyInfo_0.GetValue(object_5);
									object_7 = propertyInfo_1.GetValue(object_5);
									object_8 = propertyInfo_2.GetValue(object_5);
									object_4 = new
									{
										x = Math.Round(Convert.ToDouble(object_6) * 304.8, 0),
										y = Math.Round(Convert.ToDouble(object_7) * 304.8, 0),
										z = Math.Round(Convert.ToDouble(object_8) * 304.8, 0)
									};
									object_6 = null;
									object_7 = null;
									object_8 = null;
								}
								propertyInfo_0 = null;
								propertyInfo_1 = null;
								propertyInfo_2 = null;
							}
							object_5 = null;
						}
						class455_0.list_1.Add(new Class118<int, string, string, object>(int_4, string_2 ?? "未命名", string_1 ?? "未知", object_4));
						object_2 = null;
						string_1 = null;
						string_2 = null;
						object_3 = null;
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
				if (class455_0.list_1.Count == 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
					defaultInterpolatedStringHandler.AppendLiteral("找不到任何指定的元素（");
					defaultInterpolatedStringHandler.AppendFormatted(class455_0.list_0.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个 ID 均无效）");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else if (class455_0.list_0.Count == 1 && class455_0.list_2.Count == 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(16, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("成功获取元素 ");
					defaultInterpolatedStringHandler2.AppendFormatted(class455_0.list_0[0]);
					defaultInterpolatedStringHandler2.AppendLiteral("（位置单位：毫米）");
					string text = defaultInterpolatedStringHandler2.ToStringAndClear();
					string gparam_ = "by_id";
					int gparam_2 = class455_0.list_0[0];
					if (Class457.callSite_0 == null)
					{
						Class457.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "name", typeof(ElementQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					object gparam_3 = Class457.callSite_0.Target(Class457.callSite_0, class455_0.list_1[0]);
					if (Class457.callSite_1 == null)
					{
						Class457.callSite_1 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "category", typeof(ElementQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					object gparam_4 = Class457.callSite_1.Target(Class457.callSite_1, class455_0.list_1[0]);
					if (Class457.callSite_2 == null)
					{
						Class457.callSite_2 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "location", typeof(ElementQueryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					result = AIToolResult.Ok(text, (object)new Class119<string, int, object, object, object>(gparam_, gparam_2, gparam_3, gparam_4, Class457.callSite_2.Target(Class457.callSite_2, class455_0.list_1[0])));
				}
				else if (!string.IsNullOrEmpty(aitoolContext_0.SessionId) && aitoolContext_0.DataCache != null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("elements_by_id_");
					defaultInterpolatedStringHandler3.AppendFormatted(DateTime.Now.Ticks);
					string_3 = defaultInterpolatedStringHandler3.ToStringAndClear();
					aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)class455_0.list_1, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_3, "个元素", 50);
					elementQueryTool_0.method_4(aitoolResult_0, delegate(string string_0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(17, 2);
						defaultInterpolatedStringHandler6.AppendLiteral("✅ 成功查询 ");
						defaultInterpolatedStringHandler6.AppendFormatted(class455_0.list_0.Count);
						defaultInterpolatedStringHandler6.AppendLiteral(" 个元素，找到 ");
						defaultInterpolatedStringHandler6.AppendFormatted(class455_0.list_1.Count);
						defaultInterpolatedStringHandler6.AppendLiteral(" 个");
						string text3 = defaultInterpolatedStringHandler6.ToStringAndClear();
						if (class455_0.list_2.Count > 0)
						{
							string text4 = text3;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(6, 1);
							defaultInterpolatedStringHandler7.AppendLiteral("，");
							defaultInterpolatedStringHandler7.AppendFormatted(class455_0.list_2.Count);
							defaultInterpolatedStringHandler7.AppendLiteral(" 个未找到");
							text3 = text4 + defaultInterpolatedStringHandler7.ToStringAndClear();
						}
						return text3 + "\n\n💡 在后续工具调用中使用 cacheId=\"" + string_0 + "\" 参数来操作这些元素";
					});
					result = aitoolResult_0;
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(17, 2);
					defaultInterpolatedStringHandler4.AppendLiteral("✅ 成功查询 ");
					defaultInterpolatedStringHandler4.AppendFormatted(class455_0.list_0.Count);
					defaultInterpolatedStringHandler4.AppendLiteral(" 个元素，找到 ");
					defaultInterpolatedStringHandler4.AppendFormatted(class455_0.list_1.Count);
					defaultInterpolatedStringHandler4.AppendLiteral(" 个");
					string_0 = defaultInterpolatedStringHandler4.ToStringAndClear();
					if (class455_0.list_2.Count > 0)
					{
						string text2 = string_0;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(6, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("，");
						defaultInterpolatedStringHandler5.AppendFormatted(class455_0.list_2.Count);
						defaultInterpolatedStringHandler5.AppendLiteral(" 个未找到");
						string_0 = text2 + defaultInterpolatedStringHandler5.ToStringAndClear();
					}
					result = AIToolResult.Ok(string_0, (object)new Class120<string, int, int, int, List<int>, List<object>, string>("by_id", class455_0.list_0.Count, class455_0.list_1.Count, class455_0.list_2.Count, (class455_0.list_2.Count > 0) ? class455_0.list_2 : null, class455_0.list_1.Take(50).ToList(), "millimeters"));
				}
			}
			goto IL_0c20;
			IL_0c20:
			int_0 = -2;
			class455_0 = null;
			string_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_033f:
			class455_0.list_0 = class455_0.list_0.Distinct().ToList();
			object_1 = null;
			goto IL_0366;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "element_query";

	public string Category => "元素查询";

	public string Description => "查询元素信息，支持多种查询方式";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\"by_id\", \"selected\", \"by_category\", \"by_family_type\"],\n                \"description\": \"查询操作类型：by_id(按ID查询单个或批量元素)、selected(获取选中元素)、by_category(按类别查询)、by_family_type(按族类型查询)\"\n            },\n            \"elementId\": {\n                \"type\": \"integer\",\n                \"description\": \"元素 ID（仅 by_id 操作使用，可选）。查询单个元素。\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"元素 ID 数组（仅 by_id 操作使用，可选）。批量查询多个元素。\"\n            },\n            \"categoryName\": {\n                \"type\": \"string\",\n                \"description\": \"类别名称（仅 by_category 操作使用，必需）。常用类别：墙、楼板、门、窗、柱、梁、楼梯、屋顶、天花板、幕墙、栏杆、扶手、家具、卫浴装置、照明设备、地形等。注意：必须使用中文类别名称，与 Revit 界面中的类别名称完全一致。\"\n            },\n            \"typeId\": {\n                \"type\": \"integer\",\n                \"description\": \"族类型 ID（仅 by_family_type 操作使用，必需）。从 get_family_types 工具返回的 typeId 字段获取\"\n            },\n            \"typeName\": {\n                \"type\": \"string\",\n                \"description\": \"族类型名称（仅 by_family_type 操作使用，可选）。用于验证（如 '基本墙 200mm'、'M_门-普通' 等）。\"\n            }\n        },\n        \"required\": [\"operation\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class458))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class458 stateMachine = new Class458();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementQueryTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class462))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, object object_0)
	{
		Class462 stateMachine = new Class462();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementQueryTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class459))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, object object_0, object? object_1)
	{
		Class459 stateMachine = new Class459();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementQueryTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class460))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, object object_0)
	{
		Class460 stateMachine = new Class460();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementQueryTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class461))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_3(AIToolContext aitoolContext_0, object object_0, object? object_1)
	{
		Class461 stateMachine = new Class461();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.elementQueryTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private void method_4(AIToolResult aitoolResult_0, Func<string, string> func_0)
	{
		try
		{
			if (aitoolResult_0.Data == null)
			{
				return;
			}
			PropertyInfo property = aitoolResult_0.Data.GetType().GetProperty("cache_info");
			if (!(property != null))
			{
				return;
			}
			object value = property.GetValue(aitoolResult_0.Data);
			if (value == null)
			{
				return;
			}
			PropertyInfo property2 = value.GetType().GetProperty("cache_id");
			if (property2 != null)
			{
				string text = property2.GetValue(value)?.ToString();
				if (!string.IsNullOrEmpty(text))
				{
					aitoolResult_0.Message = func_0(text);
				}
			}
		}
		catch
		{
		}
	}
}
