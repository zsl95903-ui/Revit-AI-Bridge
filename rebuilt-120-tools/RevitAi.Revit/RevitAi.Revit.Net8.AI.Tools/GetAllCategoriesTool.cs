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
using Microsoft.CSharp.RuntimeBinder;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_all_categories", Category = "查询", Description = "获取项目中所有可用的 Revit 类别（如墙、门、窗、管道、桥架等）", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetAllCategoriesTool : IAITool
{
	[CompilerGenerated]
	private static class Class476
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;
	}

	[CompilerGenerated]
	public sealed class Class477 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetAllCategoriesTool getAllCategoriesTool_0;

		private Type type_0;

		private PropertyInfo propertyInfo_0;

		private object object_0;

		private PropertyInfo propertyInfo_1;

		private object object_1;

		private Type type_1;

		private IEnumerable ienumerable_0;

		private List<object> list_0;

		private List<object> list_1;

		private IEnumerator ienumerator_0;

		private object object_2;

		private PropertyInfo propertyInfo_2;

		private string string_0;

		private PropertyInfo propertyInfo_3;

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
					Class477 stateMachine = this;
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
				if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					type_0 = aitoolContext_0.Document.GetType();
					propertyInfo_0 = type_0.GetProperty("Settings");
					if (propertyInfo_0 == null)
					{
						result = AIToolResult.Fail("无法访问文档设置");
					}
					else
					{
						object_0 = propertyInfo_0.GetValue(aitoolContext_0.Document);
						if (object_0 == null)
						{
							result = AIToolResult.Fail("无法获取文档设置");
						}
						else
						{
							propertyInfo_1 = object_0.GetType().GetProperty("Categories");
							if (propertyInfo_1 == null)
							{
								result = AIToolResult.Fail("无法获取类别集合");
							}
							else
							{
								object_1 = propertyInfo_1.GetValue(object_0);
								if (object_1 == null)
								{
									result = AIToolResult.Fail("类别集合为空");
								}
								else
								{
									type_1 = object_1.GetType();
									ienumerable_0 = object_1 as IEnumerable;
									if (ienumerable_0 == null)
									{
										result = AIToolResult.Fail("无法枚举类别");
									}
									else
									{
										list_0 = new List<object>();
										ienumerator_0 = ienumerable_0.GetEnumerator();
										try
										{
											while (ienumerator_0.MoveNext())
											{
												object_2 = ienumerator_0.Current;
												if (object_2 != null)
												{
													propertyInfo_2 = object_2.GetType().GetProperty("Name");
													string_0 = propertyInfo_2?.GetValue(object_2) as string;
													propertyInfo_3 = object_2.GetType().GetProperty("Id");
													object_3 = propertyInfo_3?.GetValue(object_2);
													if (!string.IsNullOrEmpty(string_0))
													{
														list_0.Add(new Class137<string, string>(string_0, object_3?.ToString()));
													}
													propertyInfo_2 = null;
													string_0 = null;
													propertyInfo_3 = null;
													object_3 = null;
													object_2 = null;
												}
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
											if (Class476.callSite_0 == null)
											{
												Class476.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "name", typeof(GetAllCategoriesTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
											}
											return (dynamic)Class476.callSite_0.Target(Class476.callSite_0, object_0);
										}).ToList();
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
										defaultInterpolatedStringHandler.AppendLiteral("成功获取 ");
										defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
										defaultInterpolatedStringHandler.AppendLiteral(" 个类别");
										result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class138<int, List<object>>(list_1.Count, list_1));
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取类别失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_all_categories";

	public string Category => "查询";

	public string Description => "获取项目中所有可用的 Revit 类别（如墙、门、窗、管道、桥架等）";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {},\r\n        \"required\": []\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class477))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class477 stateMachine = new Class477();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getAllCategoriesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
