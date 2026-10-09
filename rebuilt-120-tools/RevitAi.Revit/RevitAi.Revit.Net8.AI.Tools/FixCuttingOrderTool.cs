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

[AITool("fix_cutting_order", Category = "几何操作", Description = "智能检查并修复两组元素之间的剪切顺序关系。例如：确保所有柱剪切所有梁（而不是梁剪切柱）。工具会自动检查每一对元素的当前连接状态，识别不符合要求的剪切关系，并自动切换顺序。适用于墙、柱、梁、楼板等实心元素之间的剪切关系修复。", RequiresTransaction = true, RequiresModification = true)]
public sealed class FixCuttingOrderTool : IAITool
{
	[CompilerGenerated]
	private static class Class472
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class473 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public FixCuttingOrderTool fixCuttingOrderTool_0;

		private bool bool_0;

		private bool bool_1;

		private IElementService ielementService_0;

		private IModificationService imodificationService_0;

		private object object_0;

		private List<int> list_0;

		private List<int> list_1;

		private AIToolResult aitoolResult_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if (num == 1)
				{
					goto IL_006c;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class473 stateMachine = this;
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
			goto IL_006c;
			IL_006c:
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
					goto IL_02b1;
				}
				bool_0 = aitoolContext_0.GetParameter<bool>("dryRun", false);
				bool_1 = aitoolContext_0.GetParameter<bool>("includeDetails", false);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				imodificationService_0 = ((revitAdapter2 != null) ? revitAdapter2.ModificationService : null);
				if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (imodificationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ModificationService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					object_0 = aitoolContext_0.Document;
					list_0 = fixCuttingOrderTool_0.method_0(aitoolContext_0, ielementService_0, "cuttingElementIds", "cuttingElementCacheId");
					if (list_0 == null || list_0.Count == 0)
					{
						result = AIToolResult.Fail("剪切工具元素 ID 列表不能为空。请提供 cuttingElementIds（数组）或 cuttingElementCacheId（缓存）参数");
					}
					else
					{
						list_1 = fixCuttingOrderTool_0.method_0(aitoolContext_0, ielementService_0, "elementToCutIds", "elementToCutCacheId");
						if (list_1 != null && list_1.Count != 0)
						{
							awaiter2 = fixCuttingOrderTool_0.method_1(aitoolContext_0, object_0, ielementService_0, imodificationService_0, list_0, list_1, bool_0, bool_1, cancellationToken_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_1 = awaiter2;
								Class473 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_02b1;
						}
						result = AIToolResult.Fail("被剪切元素 ID 列表不能为空。请提供 elementToCutIds（数组）或 elementToCutCacheId（缓存）参数");
					}
				}
				goto end_IL_006c;
				IL_02b1:
				aitoolResult_0 = awaiter2.GetResult();
				result = aitoolResult_0;
				end_IL_006c:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("修复剪切顺序失败: " + exception_0.Message);
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
	public sealed class Class474 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public IElementService ielementService_0;

		public IModificationService imodificationService_0;

		public List<int> list_0;

		public List<int> list_1;

		public bool bool_0;

		public bool bool_1;

		public CancellationToken cancellationToken_0;

		public FixCuttingOrderTool fixCuttingOrderTool_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private int int_6;

		private List<object> list_2;

		private string string_0;

		private bool bool_2;

		private object object_1;

		private string string_1;

		private List<int>.Enumerator enumerator_0;

		private int int_7;

		private object object_2;

		private string string_2;

		private List<int>.Enumerator enumerator_1;

		private int int_8;

		private object object_3;

		private string string_3;

		private bool bool_3;

		private bool bool_4;

		private bool bool_5;

		private string string_4;

		private string string_5;

		private string string_6;

		private string string_7;

		private List<object> list_3;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			int_1 = 0;
			int_2 = 0;
			int_3 = 0;
			int_4 = 0;
			int_5 = 0;
			int_6 = 0;
			list_2 = new List<object>();
			enumerator_0 = list_0.GetEnumerator();
			AIToolResult result;
			try
			{
				while (enumerator_0.MoveNext())
				{
					int_7 = enumerator_0.Current;
					if (!cancellationToken_0.IsCancellationRequested)
					{
						object_2 = ielementService_0.GetElementById(object_0, int_7);
						if (object_2 == null)
						{
							continue;
						}
						string_2 = ielementService_0.GetElementName(object_2);
						enumerator_1 = list_1.GetEnumerator();
						try
						{
							while (enumerator_1.MoveNext())
							{
								int_8 = enumerator_1.Current;
								if (!cancellationToken_0.IsCancellationRequested)
								{
									object_3 = ielementService_0.GetElementById(object_0, int_8);
									if (object_3 == null)
									{
										continue;
									}
									string_3 = ielementService_0.GetElementName(object_3);
									int_1++;
									bool_3 = imodificationService_0.AreElementsJoined(object_2, object_3);
									if (!bool_3)
									{
										int_2++;
										if (bool_1)
										{
											list_2.Add(new Class132<int, string, int, string, string, string>(int_7, string_2 ?? int_7.ToString(), int_8, string_3 ?? int_8.ToString(), "未连接", "两个元素之间没有连接关系"));
										}
										continue;
									}
									bool_4 = imodificationService_0.IsCuttingElementInJoin(object_2, object_3);
									if (bool_4)
									{
										int_3++;
										if (bool_1)
										{
											list_2.Add(new Class132<int, string, int, string, string, string>(int_7, string_2 ?? int_7.ToString(), int_8, string_3 ?? int_8.ToString(), "正确", "剪切顺序正确（无需修改）"));
										}
									}
									else
									{
										int_4++;
										if (bool_0)
										{
											list_2.Add(new Class132<int, string, int, string, string, string>(int_7, string_2 ?? int_7.ToString(), int_8, string_3 ?? int_8.ToString(), "需要修复", "剪切顺序不正确（检查模式，未实际修改）"));
										}
										else
										{
											bool_5 = imodificationService_0.SwitchJoinOrder(object_0, object_2, object_3);
											if (bool_5)
											{
												int_5++;
												list_2.Add(new Class132<int, string, int, string, string, string>(int_7, string_2 ?? int_7.ToString(), int_8, string_3 ?? int_8.ToString(), "已修复", "剪切顺序不正确，已成功切换"));
											}
											else
											{
												int_6++;
												list_2.Add(new Class132<int, string, int, string, string, string>(int_7, string_2 ?? int_7.ToString(), int_8, string_3 ?? int_8.ToString(), "修复失败", "剪切顺序不正确，但切换操作失败"));
											}
										}
									}
									object_3 = null;
									string_3 = null;
									continue;
								}
								result = AIToolResult.Fail("操作已取消");
								goto IL_0e4f;
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator_1/*cast due to constrained. prefix*/).Dispose();
							}
						}
						enumerator_1 = default(List<int>.Enumerator);
						object_2 = null;
						string_2 = null;
						continue;
					}
					result = AIToolResult.Fail("操作已取消");
					goto IL_0e4f;
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
			string_0 = (bool_0 ? "检查（不修复）" : "修复");
			bool_2 = bool_1 && list_2.Count > 20;
			if (!bool_2)
			{
				string_4 = ((bool_1 || list_2.Count <= 0) ? "" : "（仅显示需要修复和修复失败的结果）");
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 5);
				defaultInterpolatedStringHandler.AppendLiteral("剪切顺序");
				defaultInterpolatedStringHandler.AppendFormatted(string_0);
				defaultInterpolatedStringHandler.AppendLiteral("完成：共检查 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(" 对元素，");
				defaultInterpolatedStringHandler.AppendLiteral("正确 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_3);
				defaultInterpolatedStringHandler.AppendLiteral(" 对，");
				defaultInterpolatedStringHandler.AppendLiteral("未连接 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_2);
				defaultInterpolatedStringHandler.AppendLiteral(" 对，");
				defaultInterpolatedStringHandler.AppendLiteral("顺序错误 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_4);
				defaultInterpolatedStringHandler.AppendLiteral(" 对");
				string text = defaultInterpolatedStringHandler.ToStringAndClear();
				string text2;
				if (!bool_0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(7, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("，已修复 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_5);
					defaultInterpolatedStringHandler2.AppendLiteral(" 对");
					text2 = defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				else
				{
					text2 = "";
				}
				string text3;
				if (int_6 <= 0)
				{
					text3 = "";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(8, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("，修复失败 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_6);
					defaultInterpolatedStringHandler3.AppendLiteral(" 对");
					text3 = defaultInterpolatedStringHandler3.ToStringAndClear();
				}
				string_1 = text + text2 + text3 + string_4;
				object_1 = new Class133<string, int, int, int, int, int, int, int, int, List<object>, bool>(bool_0 ? "检查" : "修复", list_0.Count, list_1.Count, int_1, int_3, int_2, int_4, (!bool_0) ? int_5 : 0, (!bool_0) ? int_6 : 0, list_2, gparam_21: false);
				string_4 = null;
				goto IL_0e21;
			}
			string_5 = aitoolContext_0.SessionId ?? Guid.NewGuid().ToString("N");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(23, 2);
			defaultInterpolatedStringHandler4.AppendLiteral("cutting_order_results_");
			defaultInterpolatedStringHandler4.AppendFormatted(string_5);
			defaultInterpolatedStringHandler4.AppendLiteral("_");
			defaultInterpolatedStringHandler4.AppendFormatted(Guid.NewGuid(), "N");
			string_6 = defaultInterpolatedStringHandler4.ToStringAndClear();
			IAIToolDataCache dataCache = aitoolContext_0.DataCache;
			object obj;
			if (dataCache == null)
			{
				obj = null;
			}
			else
			{
				obj = dataCache.Store<List<object>>(string_5, string_6, list_2);
				if (obj != null)
				{
					goto IL_0815;
				}
			}
			obj = string.Empty;
			goto IL_0815;
			IL_0e21:
			Logger.Info("[FixCuttingOrder] " + string_1);
			result = AIToolResult.Ok(string_1, object_1);
			goto IL_0e4f;
			IL_0815:
			string_7 = (string)obj;
			list_3 = list_2.Take(20).ToList();
			if (string.IsNullOrEmpty(string_7))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(36, 5);
				defaultInterpolatedStringHandler5.AppendLiteral("剪切顺序");
				defaultInterpolatedStringHandler5.AppendFormatted(string_0);
				defaultInterpolatedStringHandler5.AppendLiteral("完成：共检查 ");
				defaultInterpolatedStringHandler5.AppendFormatted(int_1);
				defaultInterpolatedStringHandler5.AppendLiteral(" 对元素，");
				defaultInterpolatedStringHandler5.AppendLiteral("正确 ");
				defaultInterpolatedStringHandler5.AppendFormatted(int_3);
				defaultInterpolatedStringHandler5.AppendLiteral(" 对，");
				defaultInterpolatedStringHandler5.AppendLiteral("未连接 ");
				defaultInterpolatedStringHandler5.AppendFormatted(int_2);
				defaultInterpolatedStringHandler5.AppendLiteral(" 对，");
				defaultInterpolatedStringHandler5.AppendLiteral("顺序错误 ");
				defaultInterpolatedStringHandler5.AppendFormatted(int_4);
				defaultInterpolatedStringHandler5.AppendLiteral(" 对");
				string text4 = defaultInterpolatedStringHandler5.ToStringAndClear();
				string text5;
				if (!bool_0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(7, 1);
					defaultInterpolatedStringHandler6.AppendLiteral("，已修复 ");
					defaultInterpolatedStringHandler6.AppendFormatted(int_5);
					defaultInterpolatedStringHandler6.AppendLiteral(" 对");
					text5 = defaultInterpolatedStringHandler6.ToStringAndClear();
				}
				else
				{
					text5 = "";
				}
				string text6;
				if (int_6 <= 0)
				{
					text6 = "";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(8, 1);
					defaultInterpolatedStringHandler7.AppendLiteral("，修复失败 ");
					defaultInterpolatedStringHandler7.AppendFormatted(int_6);
					defaultInterpolatedStringHandler7.AppendLiteral(" 对");
					text6 = defaultInterpolatedStringHandler7.ToStringAndClear();
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(33, 1);
				defaultInterpolatedStringHandler8.AppendLiteral("。已返回前 20 条结果，其余 ");
				defaultInterpolatedStringHandler8.AppendFormatted(list_2.Count - 20);
				defaultInterpolatedStringHandler8.AppendLiteral(" 条结果因缓存服务不可用而未返回。");
				string_1 = text4 + text5 + text6 + defaultInterpolatedStringHandler8.ToStringAndClear();
				object_1 = new Class134<string, int, int, int, int, int, int, int, int, List<object>, bool, int, int>(bool_0 ? "检查" : "修复", list_0.Count, list_1.Count, int_1, int_3, int_2, int_4, (!bool_0) ? int_5 : 0, (!bool_0) ? int_6 : 0, list_3, gparam_23: true, 20, list_2.Count);
				Logger.Warning("[FixCuttingOrder] 缓存服务不可用，仅返回前 100 条结果");
				result = AIToolResult.Ok(string_1, object_1);
				goto IL_0e4f;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(36, 5);
			defaultInterpolatedStringHandler9.AppendLiteral("剪切顺序");
			defaultInterpolatedStringHandler9.AppendFormatted(string_0);
			defaultInterpolatedStringHandler9.AppendLiteral("完成：共检查 ");
			defaultInterpolatedStringHandler9.AppendFormatted(int_1);
			defaultInterpolatedStringHandler9.AppendLiteral(" 对元素，");
			defaultInterpolatedStringHandler9.AppendLiteral("正确 ");
			defaultInterpolatedStringHandler9.AppendFormatted(int_3);
			defaultInterpolatedStringHandler9.AppendLiteral(" 对，");
			defaultInterpolatedStringHandler9.AppendLiteral("未连接 ");
			defaultInterpolatedStringHandler9.AppendFormatted(int_2);
			defaultInterpolatedStringHandler9.AppendLiteral(" 对，");
			defaultInterpolatedStringHandler9.AppendLiteral("顺序错误 ");
			defaultInterpolatedStringHandler9.AppendFormatted(int_4);
			defaultInterpolatedStringHandler9.AppendLiteral(" 对");
			string text7 = defaultInterpolatedStringHandler9.ToStringAndClear();
			string text8;
			if (!bool_0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler10.AppendLiteral("，已修复 ");
				defaultInterpolatedStringHandler10.AppendFormatted(int_5);
				defaultInterpolatedStringHandler10.AppendLiteral(" 对");
				text8 = defaultInterpolatedStringHandler10.ToStringAndClear();
			}
			else
			{
				text8 = "";
			}
			string text9;
			if (int_6 <= 0)
			{
				text9 = "";
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(8, 1);
				defaultInterpolatedStringHandler11.AppendLiteral("，修复失败 ");
				defaultInterpolatedStringHandler11.AppendFormatted(int_6);
				defaultInterpolatedStringHandler11.AppendLiteral(" 对");
				text9 = defaultInterpolatedStringHandler11.ToStringAndClear();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler12 = new DefaultInterpolatedStringHandler(72, 2);
			defaultInterpolatedStringHandler12.AppendLiteral("。已返回前 20 条结果，完整结果 (");
			defaultInterpolatedStringHandler12.AppendFormatted(list_2.Count);
			defaultInterpolatedStringHandler12.AppendLiteral(" 条) 已缓存。\n\n💡 使用 get_cache_data 工具并传入 cache_id=");
			defaultInterpolatedStringHandler12.AppendFormatted(string_7);
			defaultInterpolatedStringHandler12.AppendLiteral(" 获取完整结果");
			string_1 = text7 + text8 + text9 + defaultInterpolatedStringHandler12.ToStringAndClear();
			object_1 = new Class135<string, int, int, int, int, int, int, int, int, List<object>, bool, int, int, string, Class107<string, string, int, int, int>>(bool_0 ? "检查" : "修复", list_0.Count, list_1.Count, int_1, int_3, int_2, int_4, (!bool_0) ? int_5 : 0, (!bool_0) ? int_6 : 0, list_3, gparam_25: true, 20, list_2.Count, string_7, new Class107<string, string, int, int, int>(string_7, "完整结果已缓存，可使用 get_cache_data 工具获取", list_2.Count, 20, list_2.Count - 20));
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler13 = new DefaultInterpolatedStringHandler(32, 2);
			defaultInterpolatedStringHandler13.AppendLiteral("[FixCuttingOrder] 详细结果已缓存: ");
			defaultInterpolatedStringHandler13.AppendFormatted(string_7);
			defaultInterpolatedStringHandler13.AppendLiteral("，共 ");
			defaultInterpolatedStringHandler13.AppendFormatted(list_2.Count);
			defaultInterpolatedStringHandler13.AppendLiteral(" 条");
			Logger.Info(defaultInterpolatedStringHandler13.ToStringAndClear());
			string_5 = null;
			string_6 = null;
			string_7 = null;
			list_3 = null;
			goto IL_0e21;
			IL_0e4f:
			int_0 = -2;
			list_2 = null;
			string_0 = null;
			object_1 = null;
			string_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "fix_cutting_order";

	public string Category => "几何操作";

	public string Description => "智能检查并修复两组元素之间的剪切顺序关系。例如：确保所有柱剪切所有梁（而不是梁剪切柱）。工具会自动检查每一对元素的当前连接状态，识别不符合要求的剪切关系，并自动切换顺序。适用于墙、柱、梁、楼板等实心元素之间的剪切关系修复。";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"cuttingElementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"应该作为剪切工具的元素 ID 列表（例如：所有柱的 ID）\"\n            },\n            \"cuttingElementCacheId\": {\n                \"type\": \"string\",\n                \"description\": \"剪切工具元素的缓存 ID（可选）。从上一个查询工具的返回结果中获取 cache_id 字段\"\n            },\n            \"elementToCutIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"应该被剪切的元素 ID 列表（例如：所有梁的 ID）\"\n            },\n            \"elementToCutCacheId\": {\n                \"type\": \"string\",\n                \"description\": \"被剪切元素的缓存 ID（可选）。从上一个查询工具的返回结果中获取 cache_id 字段\"\n            },\n            \"dryRun\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否只检查不修复。默认为 false（执行修复）。设置为 true 时仅返回检查结果，不实际修改模型\",\n                \"default\": false\n            },\n            \"includeDetails\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否在结果中包含每对元素的详细信息。默认为 false（仅返回汇总统计）。设置为 true 时返回所有元素的详细结果（可能导致数据量过大）\",\n                \"default\": false\n            }\n        },\n        \"required\": []\n    }";

	[AsyncStateMachine(typeof(Class473))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class473 stateMachine = new Class473();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.fixCuttingOrderTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<int>? method_0(AIToolContext aitoolContext_0, IElementService ielementService_0, string string_0, string string_1)
	{
		List<int> list = new List<int>();
		if (aitoolContext_0.HasParameter(string_1))
		{
			string parameter = aitoolContext_0.GetParameter<string>(string_1, (string)null);
			if (!string.IsNullOrEmpty(parameter))
			{
				object cachedData = aitoolContext_0.GetCachedData<object>(parameter);
				if (cachedData != null)
				{
					if (cachedData is IEnumerable enumerable)
					{
						foreach (object item2 in enumerable)
						{
							if (item2 != null)
							{
								PropertyInfo property = item2.GetType().GetProperty("id");
								if (property != null && property.GetValue(item2) is int item)
								{
									list.Add(item);
								}
							}
						}
					}
					return list.Distinct().ToList();
				}
			}
		}
		if (aitoolContext_0.HasParameter(string_0))
		{
			object parameter2 = aitoolContext_0.GetParameter<object>(string_0, (object)null);
			if (parameter2 is int[] collection)
			{
				list.AddRange(collection);
			}
			else if (parameter2 is long[] source)
			{
				list.AddRange(source.Select((long long_0) => (int)long_0));
			}
			else if (parameter2 is List<int> collection2)
			{
				list.AddRange(collection2);
			}
			else if (parameter2 is List<long> source2)
			{
				list.AddRange(source2.Select((long long_0) => (int)long_0));
			}
			else if (parameter2 is List<object> list2)
			{
				try
				{
					list.AddRange(list2.ConvertAll(Convert.ToInt32));
				}
				catch
				{
					return null;
				}
			}
			return list.Distinct().ToList();
		}
		return null;
	}

	[AsyncStateMachine(typeof(Class474))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, object object_0, IElementService ielementService_0, IModificationService imodificationService_0, List<int> list_0, List<int> list_1, bool bool_0, bool bool_1, CancellationToken cancellationToken_0)
	{
		Class474 stateMachine = new Class474();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.fixCuttingOrderTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.imodificationService_0 = imodificationService_0;
		stateMachine.list_0 = list_0;
		stateMachine.list_1 = list_1;
		stateMachine.bool_0 = bool_0;
		stateMachine.bool_1 = bool_1;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
