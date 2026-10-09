using System;
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

[AITool("get_all_tags", Category = "注释与标记", Description = "获取视图中的所有标签", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetAllTagsTool : IAITool
{
	[CompilerGenerated]
	private static class Class485
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;
	}

	[CompilerGenerated]
	public sealed class Class486 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetAllTagsTool getAllTagsTool_0;

		private IAnnotationService iannotationService_0;

		private IElementService ielementService_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private List<_003C_003Ef__AnonymousType156<object, int>> list_1;

		private IEnumerator<object> ienumerator_0;

		private object object_0;

		private int? nullable_0;

		private string string_0;

		private string string_1;

		private int? nullable_1;

		private string string_2;

		private string string_3;

		private object object_1;

		private string string_4;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_0;

		private object object_2;

		private PropertyInfo propertyInfo_1;

		private string string_5;

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
					Class486 stateMachine = this;
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
				iannotationService_0 = ((revitAdapter != null) ? revitAdapter.AnnotationService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				if (iannotationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 AnnotationService");
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
					ienumerable_0 = iannotationService_0.GetAllTags(aitoolContext_0.Document);
					if (ienumerable_0 == null || !ienumerable_0.Any())
					{
						result = AIToolResult.Ok("文档中没有找到任何标签", (object)new Class149<int, object[]>(0, Array.Empty<object>()));
					}
					else
					{
						list_0 = new List<object>();
						ienumerator_0 = ienumerable_0.GetEnumerator();
						try
						{
							while (ienumerator_0.MoveNext())
							{
								object_0 = ienumerator_0.Current;
								nullable_0 = ielementService_0.GetElementId(object_0);
								string_0 = ielementService_0.GetElementName(object_0);
								string_1 = ielementService_0.GetElementTypeName(object_0);
								nullable_1 = iannotationService_0.GetTaggedElementId(object_0);
								string_2 = null;
								string_3 = null;
								if (nullable_1.HasValue)
								{
									try
									{
										object_1 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_1.Value);
										if (object_1 != null)
										{
											string_2 = ielementService_0.GetElementTypeName(object_1);
											string_3 = ielementService_0.GetElementName(object_1);
										}
										object_1 = null;
									}
									catch
									{
									}
								}
								list_0.Add(new Class150<int?, string, string, int?, string, string>(nullable_0, string_0, string_1, nullable_1, string_2, string_3));
								string_0 = null;
								string_1 = null;
								string_2 = null;
								string_3 = null;
								object_0 = null;
							}
						}
						finally
						{
							if (num < 0 && ienumerator_0 != null)
							{
								ienumerator_0.Dispose();
							}
						}
						ienumerator_0 = null;
						list_1 = (from igrouping_0 in list_0.GroupBy(delegate(object object_0)
							{
								if (Class485.callSite_0 == null)
								{
									Class485.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "taggedElementType", typeof(GetAllTagsTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								return (dynamic)(Class485.callSite_0.Target(Class485.callSite_0, object_0) ?? "未知");
							})
							select new _003C_003Ef__AnonymousType156<object, int>((object)igrouping_0.Key, igrouping_0.Count())).ToList();
						if (!string.IsNullOrEmpty(aitoolContext_0.SessionId) && aitoolContext_0.DataCache != null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
							defaultInterpolatedStringHandler.AppendLiteral("all_tags_");
							defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
							string_4 = defaultInterpolatedStringHandler.ToStringAndClear();
							aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_4, "个标签", 200);
							if (aitoolResult_0.Data != null)
							{
								try
								{
									propertyInfo_0 = aitoolResult_0.Data.GetType().GetProperty("cache_info");
									if (propertyInfo_0 != null)
									{
										object_2 = propertyInfo_0.GetValue(aitoolResult_0.Data);
										if (object_2 != null)
										{
											propertyInfo_1 = object_2.GetType().GetProperty("cache_id");
											if (propertyInfo_1 != null)
											{
												string_5 = propertyInfo_1.GetValue(object_2)?.ToString();
												if (!string.IsNullOrEmpty(string_5))
												{
													AIToolResult obj2 = aitoolResult_0;
													DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(47, 2);
													defaultInterpolatedStringHandler2.AppendLiteral("✅ 成功获取 ");
													defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
													defaultInterpolatedStringHandler2.AppendLiteral(" 个标签\n\n💡 在后续工具调用中使用 cacheId=\"");
													defaultInterpolatedStringHandler2.AppendFormatted(string_5);
													defaultInterpolatedStringHandler2.AppendLiteral("\" 参数来操作这些元素");
													obj2.Message = defaultInterpolatedStringHandler2.ToStringAndClear();
												}
												string_5 = null;
											}
											propertyInfo_1 = null;
										}
										object_2 = null;
									}
									propertyInfo_0 = null;
								}
								catch
								{
								}
							}
							result = aitoolResult_0;
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("成功获取 ");
							defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
							defaultInterpolatedStringHandler3.AppendLiteral(" 个标签");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class151<int, List<object>, List<_003C_003Ef__AnonymousType156<object, int>>>(list_0.Count, list_0, list_1));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取标签失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_all_tags";

	public string Category => "注释与标记";

	public string Description => "获取所有标签";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {},\r\n        \"required\": []\r\n    }";

	[AsyncStateMachine(typeof(Class486))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class486 stateMachine = new Class486();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getAllTagsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
