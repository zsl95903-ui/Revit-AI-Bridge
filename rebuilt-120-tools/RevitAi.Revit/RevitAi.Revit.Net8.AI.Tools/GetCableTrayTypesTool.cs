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
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_cable_tray_types", Category = "机电查询", Description = "获取文档中所有可用的桥架类型定义", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetCableTrayTypesTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class489 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetCableTrayTypesTool getCableTrayTypesTool_0;

		private IElementService ielementService_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private string string_0;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_0;

		private object object_0;

		private PropertyInfo propertyInfo_1;

		private string string_1;

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
					Class489 stateMachine = this;
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
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
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
					ienumerable_0 = ielementService_0.GetCableTrayTypes(aitoolContext_0.Document);
					list_0 = ienumerable_0.ToList();
					if (list_0.Count == 0)
					{
						result = AIToolResult.Fail("文档中没有找到桥架类型。请确保文档中包含电气桥架系统，或使用包含电气桥架系统的模板创建文档。");
					}
					else if (aitoolContext_0.SessionId != null && aitoolContext_0.DataCache != null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
						defaultInterpolatedStringHandler.AppendLiteral("cable_tray_types_");
						defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
						string_0 = defaultInterpolatedStringHandler.ToStringAndClear();
						aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_0, "个桥架类型", 200);
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
											string_1 = propertyInfo_1.GetValue(object_0)?.ToString();
											if (!string.IsNullOrEmpty(string_1))
											{
												AIToolResult obj = aitoolResult_0;
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(106, 2);
												defaultInterpolatedStringHandler2.AppendLiteral("✅ 找到 ");
												defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
												defaultInterpolatedStringHandler2.AppendLiteral(" 个桥架类型（包含是否带配件信息）\n\n💡 在 create_cable_tray 工具中使用 cable_tray_type_id 参数指定桥架类型\n\n📋 使用 cacheId=\"");
												defaultInterpolatedStringHandler2.AppendFormatted(string_1);
												defaultInterpolatedStringHandler2.AppendLiteral("\" 来引用这些类型");
												obj.Message = defaultInterpolatedStringHandler2.ToStringAndClear();
											}
											string_1 = null;
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
						defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler3.AppendLiteral(" 个桥架类型");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class156<int, List<object>>(list_0.Count, list_0));
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(18, 2);
				defaultInterpolatedStringHandler4.AppendLiteral("获取桥架类型失败: ");
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

	public string Name => "get_cable_tray_types";

	public string Category => "机电查询";

	public string Description => "获取文档中所有可用的桥架类型定义，包括是否带配件（带配件/无配件）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {}\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class489))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class489 stateMachine = new Class489();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getCableTrayTypesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
