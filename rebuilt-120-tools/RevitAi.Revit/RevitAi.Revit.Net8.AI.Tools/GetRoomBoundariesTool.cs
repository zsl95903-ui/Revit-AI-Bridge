using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_room_boundaries", Category = "房间与分区", Description = "获取房间的边界线和顶点信息", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetRoomBoundariesTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class506 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetRoomBoundariesTool getRoomBoundariesTool_0;

		private int int_1;

		private IElementService ielementService_0;

		private IGeometryService igeometryService_0;

		private object object_0;

		private string string_0;

		private IList<IList<IDictionary<string, double>>> ilist_0;

		private List<object> list_0;

		private IEnumerator<IList<IDictionary<string, double>>> ienumerator_0;

		private IList<IDictionary<string, double>> ilist_1;

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
					Class506 stateMachine = this;
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
				int_1 = aitoolContext_0.GetParameter<int>("roomId", 0);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				igeometryService_0 = ((revitAdapter2 != null) ? revitAdapter2.GeometryService : null);
				if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (igeometryService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 GeometryService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
					if (object_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 的房间");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						string_0 = ielementService_0.GetElementName(object_0);
						ilist_0 = igeometryService_0.GetRoomBoundaries(aitoolContext_0.Document, object_0);
						if (ilist_0 == null || !ilist_0.Any())
						{
							result = AIToolResult.Ok("房间 '" + (string_0 ?? "未命名") + "' 没有找到边界信息", (object)new Class197<int, string, bool, object[]>(int_1, string_0 ?? "未命名", gparam_6: false, Array.Empty<object>()));
						}
						else
						{
							list_0 = new List<object>();
							ienumerator_0 = ilist_0.GetEnumerator();
							try
							{
								while (ienumerator_0.MoveNext())
								{
									ilist_1 = ienumerator_0.Current;
									list_0.Add(new Class198<string, IList<IDictionary<string, double>>>("boundary", ilist_1));
									ilist_1 = null;
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
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(22, 2);
							defaultInterpolatedStringHandler2.AppendLiteral("成功获取房间 '");
							defaultInterpolatedStringHandler2.AppendFormatted(string_0 ?? "未命名");
							defaultInterpolatedStringHandler2.AppendLiteral("' 的边界信息（");
							defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
							defaultInterpolatedStringHandler2.AppendLiteral(" 条边界线）");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class199<int, string, int, List<object>>(int_1, string_0 ?? "未命名", list_0.Count, list_0));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取房间边界失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_room_boundaries";

	public string Category => "房间与分区";

	public string Description => "获取房间的边界线和顶点信息（坐标单位：毫米）";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"roomId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"房间元素 ID\"\r\n            }\r\n        },\r\n        \"required\": [\"roomId\"]\r\n    }";

	[AsyncStateMachine(typeof(Class506))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class506 stateMachine = new Class506();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getRoomBoundariesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
