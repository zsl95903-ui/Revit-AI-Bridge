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

[AITool("select_elements", Category = "元素选择", Description = "选中并高亮显示指定的构件。当用户要求'选择'、'选中'、'高亮'、'显示'某些构件时必须使用此工具。支持通过缓存 ID（之前查询的结果）或元素 ID 列表来指定构件。如果用户有过滤条件（如特定楼层、类型等），需先用 filter_elements 过滤再用此工具选择。", RequiresTransaction = false, RequiresModification = false)]
public sealed class SelectElementsTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class625 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SelectElementsTool selectElementsTool_0;

		private ISelectionService iselectionService_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private bool bool_0;

		private bool bool_1;

		private List<int> list_0;

		private List<int> list_1;

		private List<string> list_2;

		private int int_1;

		private bool bool_2;

		private string string_0;

		private string string_1;

		private string string_2;

		private object object_0;

		private int[] int_2;

		private List<int>.Enumerator enumerator_0;

		private int int_3;

		private object object_1;

		private string string_3;

		private Exception exception_0;

		private Exception exception_1;

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
					Class625 stateMachine = this;
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
				iselectionService_0 = ((revitAdapter != null) ? revitAdapter.SelectionService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				iviewService_0 = ((revitAdapter2 != null) ? revitAdapter2.ViewService : null);
				IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter3 != null) ? revitAdapter3.ElementService : null);
				if (iselectionService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 SelectionService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					bool_0 = aitoolContext_0.GetParameter<bool>("append", false);
					bool_1 = aitoolContext_0.GetParameter<bool>("zoomToFit", true);
					list_0 = new List<int>();
					if (!aitoolContext_0.HasParameter("cacheId"))
					{
						if (aitoolContext_0.HasParameter("elementIds"))
						{
							int_2 = aitoolContext_0.GetParameter<int[]>("elementIds", (int[])null);
							if (int_2 != null && int_2.Length != 0)
							{
								list_0.AddRange(int_2);
							}
							int_2 = null;
						}
						goto IL_02c2;
					}
					string_2 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
					if (string.IsNullOrEmpty(string_2))
					{
						result = AIToolResult.Fail("cacheId 参数不能为空");
					}
					else if (aitoolContext_0.DataCache == null)
					{
						result = AIToolResult.Fail("数据缓存服务不可用");
					}
					else
					{
						object_0 = aitoolContext_0.DataCache.Retrieve<object>(string_2);
						if (object_0 == null)
						{
							result = AIToolResult.Fail("缓存 '" + string_2 + "' 不存在或已过期");
						}
						else
						{
							list_0 = selectElementsTool_0.method_0(object_0);
							if (list_0.Count != 0)
							{
								string_2 = null;
								object_0 = null;
								goto IL_02c2;
							}
							result = AIToolResult.Fail("缓存数据中未找到任何元素 ID");
						}
					}
				}
				goto end_IL_0067;
				IL_02c2:
				if (list_0.Count == 0)
				{
					result = AIToolResult.Fail("请提供 cacheId 或 elementIds 参数");
				}
				else
				{
					list_1 = new List<int>();
					list_2 = new List<string>();
					if (ielementService_0 != null)
					{
						enumerator_0 = list_0.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								int_3 = enumerator_0.Current;
								object_1 = ielementService_0.GetElementById(aitoolContext_0.Document, int_3);
								if (object_1 != null)
								{
									list_1.Add(int_3);
									string text = selectElementsTool_0.method_2(object_1);
									if (text == null)
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
										defaultInterpolatedStringHandler.AppendLiteral("#");
										defaultInterpolatedStringHandler.AppendFormatted(int_3);
										text = defaultInterpolatedStringHandler.ToStringAndClear();
									}
									string_3 = text;
									list_2.Add(string_3);
									string_3 = null;
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
					}
					else
					{
						list_1 = list_0;
					}
					if (list_1.Count == 0)
					{
						result = AIToolResult.Fail("找不到任何指定的元素，它们可能已被删除");
					}
					else
					{
						int_1 = list_0.Count - list_1.Count;
						bool_2 = iselectionService_0.SelectElements(aitoolContext_0.Document, (IEnumerable<int>)list_1, bool_0);
						if (!bool_2)
						{
							result = AIToolResult.Fail("选择元素失败");
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("[select_elements] 已选中 ");
							defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler2.AppendLiteral(" 个元素");
							Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear() + (bool_0 ? "（追加模式）" : "（替换模式）"));
							if (bool_1 && iviewService_0 != null)
							{
								try
								{
									iviewService_0.ZoomToElements(aitoolContext_0.Document, (IEnumerable<int>)list_1, (object)null);
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(27, 1);
									defaultInterpolatedStringHandler3.AppendLiteral("[select_elements] 已缩放到 ");
									defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
									defaultInterpolatedStringHandler3.AppendLiteral(" 个元素");
									Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
								}
								catch (Exception ex)
								{
									exception_0 = ex;
									Logger.Warning("[select_elements] 缩放视图失败: " + exception_0.Message);
								}
							}
							string_0 = (bool_0 ? "已追加选择" : "已选中");
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(7, 2);
							defaultInterpolatedStringHandler4.AppendLiteral("✅ ");
							defaultInterpolatedStringHandler4.AppendFormatted(string_0);
							defaultInterpolatedStringHandler4.AppendLiteral(" ");
							defaultInterpolatedStringHandler4.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler4.AppendLiteral(" 个构件");
							string_1 = defaultInterpolatedStringHandler4.ToStringAndClear();
							if (int_1 > 0)
							{
								string text2 = string_1;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(12, 1);
								defaultInterpolatedStringHandler5.AppendLiteral("，");
								defaultInterpolatedStringHandler5.AppendFormatted(int_1);
								defaultInterpolatedStringHandler5.AppendLiteral(" 个 ID 无效已跳过");
								string_1 = text2 + defaultInterpolatedStringHandler5.ToStringAndClear();
							}
							result = AIToolResult.Ok(string_1, (object)new Class293<int, int, bool, bool, List<string>, int[]>(list_1.Count, int_1, bool_0, bool_1, list_2, list_1.ToArray()));
						}
					}
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_1 = ex;
				Logger.Error("[select_elements] 选择失败", exception_1);
				result = AIToolResult.Fail("选择失败: " + exception_1.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "select_elements";

	public string Category => "元素选择";

	public string Description => "选中并高亮显示指定的构件";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（可选）。如果要选择之前查询缓存的元素，使用此参数。工具会从缓存中提取所有元素的 ID 并选中它们。\"\n            },\n            \"elementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"要选择的元素 ID 列表（可选）。如果不提供 cacheId，则必须提供此参数。\"\n            },\n            \"append\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否追加到当前选择（可选，默认 false）。true = 追加到当前选择，false = 替换当前选择。\",\n                \"default\": false\n            },\n            \"zoomToFit\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否缩放到构件（可选，默认 true）。true = 同时缩放视图以显示选中的构件，false = 只选择不缩放。\",\n                \"default\": true\n            }\n        },\n        \"required\": []\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class625))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class625 stateMachine = new Class625();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.selectElementsTool_0 = this;
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

	private string? method_2(object object_0)
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
