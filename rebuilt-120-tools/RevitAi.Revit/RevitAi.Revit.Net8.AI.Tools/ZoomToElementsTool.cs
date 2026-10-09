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
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("zoom_to_elements", Category = "视图管理", Description = "缩放当前视图以显示指定的元素", RequiresTransaction = false, RequiresModification = false)]
public sealed class ZoomToElementsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class651 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ZoomToElementsTool zoomToElementsTool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private double double_0;

		private List<int> list_0;

		private List<object> list_1;

		private bool bool_0;

		private string string_0;

		private object object_0;

		private int[] int_1;

		private List<int>.Enumerator enumerator_0;

		private int int_2;

		private object object_1;

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
					Class651 stateMachine = this;
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
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					double_0 = aitoolContext_0.GetParameter<double>("fitFactor", 1.0);
					list_0 = new List<int>();
					if (!aitoolContext_0.HasParameter("cacheId"))
					{
						if (aitoolContext_0.HasParameter("elementIds"))
						{
							int_1 = aitoolContext_0.GetParameter<int[]>("elementIds", (int[])null);
							if (int_1 != null && int_1.Length != 0)
							{
								list_0.AddRange(int_1);
							}
							int_1 = null;
						}
						goto IL_02b1;
					}
					string_0 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
					if (string.IsNullOrEmpty(string_0))
					{
						result = AIToolResult.Fail("cacheId 参数不能为空");
					}
					else if (aitoolContext_0.DataCache == null)
					{
						result = AIToolResult.Fail("数据缓存服务不可用");
					}
					else
					{
						object_0 = aitoolContext_0.DataCache.Retrieve<object>(string_0);
						if (object_0 == null)
						{
							result = AIToolResult.Fail("缓存 '" + string_0 + "' 不存在或已过期");
						}
						else
						{
							list_0 = zoomToElementsTool_0.method_0(object_0);
							if (list_0.Count != 0)
							{
								string_0 = null;
								object_0 = null;
								goto IL_02b1;
							}
							result = AIToolResult.Fail("缓存数据中未找到任何元素 ID");
						}
					}
				}
				goto end_IL_0067;
				IL_02b1:
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("请提供 cacheId 或 elementIds 参数");
				}
				else
				{
					list_1 = new List<object>();
					enumerator_0 = list_0.GetEnumerator();
					try
					{
						while (enumerator_0.MoveNext())
						{
							int_2 = enumerator_0.Current;
							object_1 = ielementService_0.GetElementById(aitoolContext_0.Document, int_2);
							if (object_1 != null)
							{
								list_1.Add(object_1);
							}
							object_1 = null;
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
						result = AIToolResult.Fail("找不到任何指定的元素");
					}
					else
					{
						bool_0 = iviewService_0.ZoomToElements(aitoolContext_0.Document, (IEnumerable<int>)list_0, (object)null);
						if (!bool_0)
						{
							result = AIToolResult.Fail("缩放失败");
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
							defaultInterpolatedStringHandler.AppendLiteral("已缩放到 ");
							defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class310<int, double>(list_1.Count, double_0));
						}
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("缩放失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "zoom_to_elements";

	public string Category => "视图管理";

	public string Description => "缩放当前视图以显示指定的元素";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"cacheId\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"缓存 ID（可选）。如果要缩放到之前查询缓存的元素，使用此参数。工具会从缓存中提取所有元素的 ID 并缩放到这些元素。\"\r\n            },\r\n            \"elementIds\": {\r\n                \"type\": \"array\",\r\n                \"items\": { \"type\": \"integer\" },\r\n                \"description\": \"要缩放显示的元素 ID 列表（可选）。如果不提供 cacheId，则必须提供此参数。\"\r\n            },\r\n            \"fitFactor\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"缩放因子（可选，1.0 = 适合视图，0.5 = 放大 2 倍，2.0 = 缩小 2 倍）\",\r\n                \"default\": 1.0\r\n            }\r\n        },\r\n        \"required\": []\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class651))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class651 stateMachine = new Class651();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.zoomToElementsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<int> method_0(object object_0)
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
						int? num = method_1(item);
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

	private int? method_1(object object_0)
	{
		try
		{
			if (object_0 == null)
			{
				return null;
			}
			Type type = object_0.GetType();
			string[] array = new string[7]
			{
				"id",
				"elementId",
				"column_id",
				"wall_id",
				"floor_id",
				"beam_id",
				"brace_id"
			};
			string[] array2 = array;
			foreach (string name in array2)
			{
				PropertyInfo property = type.GetProperty(name);
				if (property != null && property.GetValue(object_0) is int num && num > 0)
				{
					return num;
				}
			}
		}
		catch
		{
		}
		return null;
	}
}
