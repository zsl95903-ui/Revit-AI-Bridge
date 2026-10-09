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
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_pipe_types", Category = "机电查询", Description = "获取文档中所有可用的管道类型定义", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetPipeTypesTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class503 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetPipeTypesTool getPipeTypesTool_0;

		private Document document_0;

		private List<PipeType> list_0;

		private List<object> list_1;

		private List<PipeType>.Enumerator enumerator_0;

		private PipeType pipeType_0;

		private long long_0;

		private string string_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_0;

		private object object_0;

		private PropertyInfo propertyInfo_1;

		private string string_2;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
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
					Class503 stateMachine = this;
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
					object document = aitoolContext_0.Document;
					document_0 = (Document)((document is Document) ? document : null);
					if (document_0 == null)
					{
						result = AIToolResult.Fail("文档对象类型不正确");
					}
					else
					{
						list_0 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(PipeType))).Cast<PipeType>().ToList();
						if (list_0.Count == 0)
						{
							result = AIToolResult.Fail("文档中没有找到管道类型。请确保文档中包含管道系统，或使用包含管道系统的模板创建文档。");
						}
						else
						{
							list_1 = new List<object>();
							enumerator_0 = list_0.GetEnumerator();
							try
							{
								while (enumerator_0.MoveNext())
								{
									pipeType_0 = enumerator_0.Current;
									long_0 = ((Element)pipeType_0).Id.Value;
									string_0 = ((Element)pipeType_0).Name;
									list_1.Add(new Class190<long, string>(long_0, string_0 ?? "未命名"));
									string_0 = null;
									pipeType_0 = null;
								}
							}
							finally
							{
								if (num < 0)
								{
									((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
								}
							}
							enumerator_0 = default(List<PipeType>.Enumerator);
							if (aitoolContext_0.SessionId != null && aitoolContext_0.DataCache != null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
								defaultInterpolatedStringHandler.AppendLiteral("pipe_types_");
								defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
								string_1 = defaultInterpolatedStringHandler.ToStringAndClear();
								aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_1, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_1, "个管道类型", 200);
								if (aitoolResult_0.Data != null)
								{
									try
									{
										propertyInfo_0 = aitoolResult_0.Data.GetType().GetProperty("cache_info");
										if (propertyInfo_0 != null)
										{
											object_0 = propertyInfo_0.GetValue(aitoolResult_0.Data);
											if (object_0 != null)
											{
												propertyInfo_1 = object_0.GetType().GetProperty("cache_id");
												if (propertyInfo_1 != null)
												{
													string_2 = propertyInfo_1.GetValue(object_0)?.ToString();
													if (!string.IsNullOrEmpty(string_2))
													{
														AIToolResult obj = aitoolResult_0;
														DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(83, 2);
														defaultInterpolatedStringHandler2.AppendLiteral("✅ 找到 ");
														defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
														defaultInterpolatedStringHandler2.AppendLiteral(" 个管道类型\n\n💡 在 create_pipe 工具中使用 pipe_type_id 参数指定管道类型\n\n📋 使用 cacheId=\"");
														defaultInterpolatedStringHandler2.AppendFormatted(string_2);
														defaultInterpolatedStringHandler2.AppendLiteral("\" 来引用这些类型");
														obj.Message = defaultInterpolatedStringHandler2.ToStringAndClear();
													}
													string_2 = null;
												}
												propertyInfo_1 = null;
											}
											object_0 = null;
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
								defaultInterpolatedStringHandler3.AppendLiteral("找到 ");
								defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
								defaultInterpolatedStringHandler3.AppendLiteral(" 个管道类型");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class156<int, List<object>>(list_1.Count, list_1));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(18, 2);
				defaultInterpolatedStringHandler4.AppendLiteral("获取管道类型失败: ");
				defaultInterpolatedStringHandler4.AppendFormatted(exception_0.Message);
				defaultInterpolatedStringHandler4.AppendLiteral("\n\n详细错误: ");
				defaultInterpolatedStringHandler4.AppendFormatted(exception_0);
				result = AIToolResult.Fail(defaultInterpolatedStringHandler4.ToStringAndClear());
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_pipe_types";

	public string Category => "机电查询";

	public string Description => "获取文档中所有可用的管道类型定义";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {}\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class503))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class503 stateMachine = new Class503();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getPipeTypesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
