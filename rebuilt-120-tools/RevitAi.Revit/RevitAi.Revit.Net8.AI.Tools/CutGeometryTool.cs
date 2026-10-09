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

[AITool("cut_geometry", Category = "几何操作", Description = "执行几何剪切操作，自动尝试空心剪切（InstanceVoidCutUtils）和实心剪切（SolidSolidCutUtils）。适用于空心族、窗、门等包含空心的族实例剪切其他元素，以及两个实心元素之间的布尔差集剪切。支持单个或批量操作。注意：如需处理墙、柱、梁、板等实心元素之间的连接关系，请使用 join_geometry 工具。", RequiresTransaction = true, RequiresModification = true)]
public sealed class CutGeometryTool : IAITool
{
	[CompilerGenerated]
	private static class Class429
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class430
	{
		public IElementService ielementService_0;

		public object object_0;

		internal object method_0(int int_0)
		{
			return ielementService_0.GetElementById(object_0, int_0);
		}

		internal string method_1(object object_1)
		{
			return ielementService_0.GetElementName(object_1);
		}
	}

	[CompilerGenerated]
	public sealed class Class431 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CutGeometryTool cutGeometryTool_0;

		private string string_0;

		private IElementService ielementService_0;

		private IModificationService imodificationService_0;

		private object object_0;

		private List<int> list_0;

		private List<int> list_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 1u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class431 stateMachine = this;
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
				TaskAwaiter<AIToolResult> awaiter2;
				if (num == 1)
				{
					awaiter2 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0382;
				}
				TaskAwaiter<AIToolResult> awaiter3;
				if (num == 2)
				{
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_034f;
				}
				string_0 = aitoolContext_0.GetParameter<string>("operation", "cut");
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
					list_0 = cutGeometryTool_0.method_0(aitoolContext_0, ielementService_0, "elementToCutId", "elementToCutIds", "elementToCutCacheId");
					if (list_0 == null || list_0.Count == 0)
					{
						result = AIToolResult.Fail("被剪切元素 ID 列表不能为空。请提供 elementToCutId（单个）、elementToCutIds（数组）或 elementToCutCacheId（缓存）参数");
					}
					else
					{
						list_1 = cutGeometryTool_0.method_0(aitoolContext_0, ielementService_0, "cuttingElementId", "cuttingElementIds", "cuttingElementCacheId");
						if (list_1 != null && list_1.Count != 0)
						{
							if (list_0.Count == 1 && list_1.Count == 1)
							{
								awaiter2 = cutGeometryTool_0.method_1(object_0, ielementService_0, imodificationService_0, string_0, list_0[0], list_1[0]).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter2;
									Class431 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
									return;
								}
								goto IL_0382;
							}
							awaiter3 = cutGeometryTool_0.method_2(aitoolContext_0, object_0, ielementService_0, imodificationService_0, string_0, list_0, list_1, cancellationToken_0).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = 2;
								int_0 = 2;
								taskAwaiter_1 = awaiter3;
								Class431 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
								return;
							}
							goto IL_034f;
						}
						result = AIToolResult.Fail("剪切工具元素 ID 列表不能为空。请提供 cuttingElementId（单个）、cuttingElementIds（数组）或 cuttingElementCacheId（缓存）参数");
					}
				}
				goto end_IL_006e;
				IL_034f:
				aitoolResult_1 = awaiter3.GetResult();
				result = aitoolResult_1;
				goto end_IL_006e;
				IL_0382:
				aitoolResult_0 = awaiter2.GetResult();
				result = aitoolResult_0;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("几何剪切操作失败: " + exception_0.Message);
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
	public sealed class Class432 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public IElementService ielementService_0;

		public IModificationService imodificationService_0;

		public string string_0;

		public List<int> list_0;

		public List<int> list_1;

		public CancellationToken cancellationToken_0;

		public CutGeometryTool cutGeometryTool_0;

		private Class430 class430_0;

		private string string_1;

		private int int_1;

		private int int_2;

		private List<object> list_2;

		private int int_3;

		private List<object> list_3;

		private List<string> list_4;

		private bool bool_0;

		private object object_1;

		private string string_2;

		private List<int>.Enumerator enumerator_0;

		private int int_4;

		private object object_2;

		private string string_3;

		private List<int>.Enumerator enumerator_1;

		private int int_5;

		private object object_3;

		private bool bool_1;

		private string string_4;

		private string string_5;

		private string string_6;

		private List<object> list_5;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			class430_0 = new Class430();
			class430_0.ielementService_0 = ielementService_0;
			class430_0.object_0 = object_0;
			string_1 = (string_0.Equals("cut", StringComparison.OrdinalIgnoreCase) ? "剪切" : "取消剪切");
			int_1 = 0;
			int_2 = 0;
			list_2 = new List<object>();
			enumerator_0 = list_1.GetEnumerator();
			AIToolResult result;
			try
			{
				while (enumerator_0.MoveNext())
				{
					int_4 = enumerator_0.Current;
					if (!cancellationToken_0.IsCancellationRequested)
					{
						object_2 = class430_0.ielementService_0.GetElementById(class430_0.object_0, int_4);
						if (object_2 == null)
						{
							list_2.Add(new Class101<int, string>(int_4, "找不到剪切工具元素"));
							int_2 += list_0.Count;
							continue;
						}
						string_3 = class430_0.ielementService_0.GetElementName(object_2);
						enumerator_1 = list_0.GetEnumerator();
						try
						{
							while (enumerator_1.MoveNext())
							{
								int_5 = enumerator_1.Current;
								if (!cancellationToken_0.IsCancellationRequested)
								{
									object_3 = class430_0.ielementService_0.GetElementById(class430_0.object_0, int_5);
									if (object_3 == null)
									{
										list_2.Add(new Class102<int, int, string>(int_5, int_4, "找不到被剪切元素"));
										int_2++;
										continue;
									}
									if (string_0.Equals("cut", StringComparison.OrdinalIgnoreCase))
									{
										bool_1 = imodificationService_0.CutGeometry(class430_0.object_0, object_3, object_2);
									}
									else
									{
										if (!string_0.Equals("uncut", StringComparison.OrdinalIgnoreCase))
										{
											result = AIToolResult.Fail("不支持的操作类型: " + string_0);
											goto IL_0a03;
										}
										bool_1 = imodificationService_0.UncutGeometry(class430_0.object_0, object_3, object_2);
									}
									if (bool_1)
									{
										int_1++;
										list_2.Add(new Class103<int, int, string>(int_5, int_4, "成功"));
									}
									else
									{
										int_2++;
										list_2.Add(new Class103<int, int, string>(int_5, int_4, "失败"));
									}
									object_3 = null;
									continue;
								}
								result = AIToolResult.Fail("操作已取消");
								goto IL_0a03;
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
						string_3 = null;
						continue;
					}
					result = AIToolResult.Fail("操作已取消");
					goto IL_0a03;
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
			int_3 = list_0.Count * list_1.Count;
			list_3 = (from int_0 in list_1
				select class430_0.ielementService_0.GetElementById(class430_0.object_0, int_0) into object_0
				where object_0 != null
				select object_0).ToList();
			list_4 = list_3.Select((object object_1) => class430_0.ielementService_0.GetElementName(object_1)).ToList();
			bool_0 = list_2.Count > 20;
			if (!bool_0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 5);
				defaultInterpolatedStringHandler.AppendLiteral("批量");
				defaultInterpolatedStringHandler.AppendFormatted(string_1);
				defaultInterpolatedStringHandler.AppendLiteral("完成：");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个被剪切元素 × ");
				defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个剪切工具，成功 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(" 个，失败 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_2);
				defaultInterpolatedStringHandler.AppendLiteral(" 个");
				string_2 = defaultInterpolatedStringHandler.ToStringAndClear();
				object_1 = new Class104<int, int, List<string>, string, int, int, int, List<object>, bool>(list_0.Count, list_1.Count, list_4, string_0, int_3, int_1, int_2, list_2, gparam_17: false);
				goto IL_09d5;
			}
			string_4 = aitoolContext_0.SessionId ?? Guid.NewGuid().ToString("N");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("cut_results_");
			defaultInterpolatedStringHandler2.AppendFormatted(string_4);
			defaultInterpolatedStringHandler2.AppendLiteral("_");
			defaultInterpolatedStringHandler2.AppendFormatted(Guid.NewGuid(), "N");
			string_5 = defaultInterpolatedStringHandler2.ToStringAndClear();
			IAIToolDataCache dataCache = aitoolContext_0.DataCache;
			object obj;
			if (dataCache == null)
			{
				obj = null;
			}
			else
			{
				obj = dataCache.Store<List<object>>(string_4, string_5, list_2);
				if (obj != null)
				{
					goto IL_062b;
				}
			}
			obj = string.Empty;
			goto IL_062b;
			IL_062b:
			string_6 = (string)obj;
			list_5 = list_2.Take(20).ToList();
			if (string.IsNullOrEmpty(string_6))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(66, 6);
				defaultInterpolatedStringHandler3.AppendLiteral("批量");
				defaultInterpolatedStringHandler3.AppendFormatted(string_1);
				defaultInterpolatedStringHandler3.AppendLiteral("完成：");
				defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler3.AppendLiteral(" 个被剪切元素 × ");
				defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler3.AppendLiteral(" 个剪切工具，成功 ");
				defaultInterpolatedStringHandler3.AppendFormatted(int_1);
				defaultInterpolatedStringHandler3.AppendLiteral(" 个，失败 ");
				defaultInterpolatedStringHandler3.AppendFormatted(int_2);
				defaultInterpolatedStringHandler3.AppendLiteral(" 个。已返回前 20 条结果，其余 ");
				defaultInterpolatedStringHandler3.AppendFormatted(list_2.Count - 20);
				defaultInterpolatedStringHandler3.AppendLiteral(" 条结果因缓存服务不可用而未返回。");
				string_2 = defaultInterpolatedStringHandler3.ToStringAndClear();
				object_1 = new Class105<int, int, List<string>, string, int, int, int, List<object>, bool, int, int>(list_0.Count, list_1.Count, list_4, string_0, int_3, int_1, int_2, list_5, gparam_19: true, 20, list_2.Count);
				Logger.Warning("[CutGeometry] 缓存服务不可用，仅返回前 20 条结果");
				result = AIToolResult.Ok(string_2, object_1);
				goto IL_0a03;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(105, 7);
			defaultInterpolatedStringHandler4.AppendLiteral("批量");
			defaultInterpolatedStringHandler4.AppendFormatted(string_1);
			defaultInterpolatedStringHandler4.AppendLiteral("完成：");
			defaultInterpolatedStringHandler4.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler4.AppendLiteral(" 个被剪切元素 × ");
			defaultInterpolatedStringHandler4.AppendFormatted(list_1.Count);
			defaultInterpolatedStringHandler4.AppendLiteral(" 个剪切工具，成功 ");
			defaultInterpolatedStringHandler4.AppendFormatted(int_1);
			defaultInterpolatedStringHandler4.AppendLiteral(" 个，失败 ");
			defaultInterpolatedStringHandler4.AppendFormatted(int_2);
			defaultInterpolatedStringHandler4.AppendLiteral(" 个。已返回前 20 条结果，完整结果 (");
			defaultInterpolatedStringHandler4.AppendFormatted(list_2.Count);
			defaultInterpolatedStringHandler4.AppendLiteral(" 条) 已缓存。\n\n💡 使用 get_cache_data 工具并传入 cache_id=");
			defaultInterpolatedStringHandler4.AppendFormatted(string_6);
			defaultInterpolatedStringHandler4.AppendLiteral(" 获取完整结果");
			string_2 = defaultInterpolatedStringHandler4.ToStringAndClear();
			object_1 = new Class106<int, int, List<string>, string, int, int, int, List<object>, bool, int, int, string, Class107<string, string, int, int, int>>(list_0.Count, list_1.Count, list_4, string_0, int_3, int_1, int_2, list_5, gparam_21: true, 20, list_2.Count, string_6, new Class107<string, string, int, int, int>(string_6, "完整结果已缓存，可使用 get_cache_data 工具获取", list_2.Count, 20, list_2.Count - 20));
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(28, 2);
			defaultInterpolatedStringHandler5.AppendLiteral("[CutGeometry] 详细结果已缓存: ");
			defaultInterpolatedStringHandler5.AppendFormatted(string_6);
			defaultInterpolatedStringHandler5.AppendLiteral("，共 ");
			defaultInterpolatedStringHandler5.AppendFormatted(list_2.Count);
			defaultInterpolatedStringHandler5.AppendLiteral(" 条");
			Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
			string_4 = null;
			string_5 = null;
			string_6 = null;
			list_5 = null;
			goto IL_09d5;
			IL_0a03:
			int_0 = -2;
			class430_0 = null;
			string_1 = null;
			list_2 = null;
			list_3 = null;
			list_4 = null;
			object_1 = null;
			string_2 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_09d5:
			Logger.Info("[CutGeometry] " + string_2);
			result = AIToolResult.Ok(string_2, object_1);
			goto IL_0a03;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class433 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public object object_0;

		public IElementService ielementService_0;

		public IModificationService imodificationService_0;

		public string string_0;

		public int int_1;

		public int int_2;

		public CutGeometryTool cutGeometryTool_0;

		private object object_1;

		private object object_2;

		private string string_1;

		private string string_2;

		private bool bool_0;

		private string string_3;

		void IAsyncStateMachine.MoveNext()
		{
			object_1 = ielementService_0.GetElementById(object_0, int_1);
			AIToolResult result;
			if (object_1 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(" 的被剪切元素");
				result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				object_2 = ielementService_0.GetElementById(object_0, int_2);
				if (object_2 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("找不到 ID 为 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_2);
					defaultInterpolatedStringHandler2.AppendLiteral(" 的剪切工具元素");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				else
				{
					string_1 = ielementService_0.GetElementName(object_1);
					string_2 = ielementService_0.GetElementName(object_2);
					if (string_0.Equals("cut", StringComparison.OrdinalIgnoreCase))
					{
						bool_0 = imodificationService_0.CutGeometry(object_0, object_1, object_2);
						string_3 = "剪切";
					}
					else
					{
						if (!string_0.Equals("uncut", StringComparison.OrdinalIgnoreCase))
						{
							result = AIToolResult.Fail("不支持的操作类型: " + string_0);
							goto IL_033b;
						}
						bool_0 = imodificationService_0.UncutGeometry(object_0, object_1, object_2);
						string_3 = "取消剪切";
					}
					if (!bool_0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(23, 3);
						defaultInterpolatedStringHandler3.AppendFormatted(string_3);
						defaultInterpolatedStringHandler3.AppendLiteral("几何图形失败（被剪切元素: ");
						defaultInterpolatedStringHandler3.AppendFormatted(string_1 ?? int_1.ToString());
						defaultInterpolatedStringHandler3.AppendLiteral(", 剪切元素: ");
						defaultInterpolatedStringHandler3.AppendFormatted(string_2 ?? int_2.ToString());
						defaultInterpolatedStringHandler3.AppendLiteral("）");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(23, 3);
						defaultInterpolatedStringHandler4.AppendLiteral("成功");
						defaultInterpolatedStringHandler4.AppendFormatted(string_3);
						defaultInterpolatedStringHandler4.AppendLiteral("几何图形（被剪切元素: ");
						defaultInterpolatedStringHandler4.AppendFormatted(string_1 ?? int_1.ToString());
						defaultInterpolatedStringHandler4.AppendLiteral(", 剪切元素: ");
						defaultInterpolatedStringHandler4.AppendFormatted(string_2 ?? int_2.ToString());
						defaultInterpolatedStringHandler4.AppendLiteral("）");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class100<int, string, int, string, string, string>(int_1, string_1, int_2, string_2, string_0, "成功"));
					}
				}
			}
			goto IL_033b;
			IL_033b:
			int_0 = -2;
			object_1 = null;
			object_2 = null;
			string_1 = null;
			string_2 = null;
			string_3 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "cut_geometry";

	public string Category => "几何操作";

	public string Description => "执行几何剪切操作，自动尝试空心剪切（InstanceVoidCutUtils）和实心剪切（SolidSolidCutUtils）。适用于空心族、窗、门等包含空心的族实例剪切其他元素，以及两个实心元素之间的布尔差集剪切。支持单个或批量操作。注意：如需处理墙、柱、梁、板等实心元素之间的连接关系，请使用 join_geometry 工具。";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"elementToCutId\": {\n                \"type\": \"integer\",\n                \"description\": \"要被剪切的元素 ID（单个操作时使用）\"\n            },\n            \"elementToCutIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"要被剪切的元素 ID 列表（批量操作时使用）\"\n            },\n            \"elementToCutCacheId\": {\n                \"type\": \"string\",\n                \"description\": \"被剪切元素的缓存 ID（可选）。从上一个查询工具的返回结果中获取 cache_id 字段，用于批量操作之前查询到的元素\"\n            },\n            \"cuttingElementId\": {\n                \"type\": \"integer\",\n                \"description\": \"用作剪切工具的元素 ID（单个操作时使用）\"\n            },\n            \"cuttingElementIds\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"用作剪切工具的元素 ID 列表（批量操作时使用）\"\n            },\n            \"cuttingElementCacheId\": {\n                \"type\": \"string\",\n                \"description\": \"剪切工具元素的缓存 ID（可选）。从上一个查询工具的返回结果中获取 cache_id 字段，用于批量操作之前查询到的元素\"\n            },\n            \"operation\": {\n                \"type\": \"string\",\n                \"description\": \"操作类型：cut=执行剪切, uncut=取消剪切\",\n                \"enum\": [\"cut\", \"uncut\"],\n                \"default\": \"cut\"\n            }\n        },\n        \"required\": []\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class431))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class431 stateMachine = new Class431();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.cutGeometryTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<int>? method_0(AIToolContext aitoolContext_0, IElementService ielementService_0, string string_0, string string_1, string string_2)
	{
		List<int> list = new List<int>();
		if (aitoolContext_0.HasParameter(string_2))
		{
			string parameter = aitoolContext_0.GetParameter<string>(string_2, (string)null);
			if (!string.IsNullOrEmpty(parameter))
			{
				object cachedData = aitoolContext_0.GetCachedData<object>(parameter);
				if (cachedData != null)
				{
					if (cachedData is IEnumerable enumerable)
					{
						foreach (object item3 in enumerable)
						{
							if (item3 != null)
							{
								PropertyInfo property = item3.GetType().GetProperty("id");
								if (property != null && property.GetValue(item3) is int item)
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
			int parameter2 = aitoolContext_0.GetParameter<int>(string_0, 0);
			if (parameter2 > 0)
			{
				list.Add(parameter2);
				return list;
			}
		}
		if (aitoolContext_0.HasParameter(string_1))
		{
			object parameter3 = aitoolContext_0.GetParameter<object>(string_1, (object)null);
			if (parameter3 is int item2)
			{
				list.Add(item2);
			}
			else if (parameter3 is long num)
			{
				list.Add((int)num);
			}
			else if (parameter3 is int[] collection)
			{
				list.AddRange(collection);
			}
			else if (parameter3 is long[] source)
			{
				list.AddRange(source.Select((long long_0) => (int)long_0));
			}
			else if (parameter3 is List<int> collection2)
			{
				list.AddRange(collection2);
			}
			else if (parameter3 is List<long> source2)
			{
				list.AddRange(source2.Select((long long_0) => (int)long_0));
			}
			else if (parameter3 is List<object> list2)
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
		return (list.Count > 0) ? list.Distinct().ToList() : null;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class433))]
	private Task<AIToolResult> method_1(object object_0, IElementService ielementService_0, IModificationService imodificationService_0, string string_0, int int_0, int int_1)
	{
		Class433 stateMachine = new Class433();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.cutGeometryTool_0 = this;
		stateMachine.object_0 = object_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.imodificationService_0 = imodificationService_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_1 = int_0;
		stateMachine.int_2 = int_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class432))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, object object_0, IElementService ielementService_0, IModificationService imodificationService_0, string string_0, List<int> list_0, List<int> list_1, CancellationToken cancellationToken_0)
	{
		Class432 stateMachine = new Class432();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.cutGeometryTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.imodificationService_0 = imodificationService_0;
		stateMachine.string_0 = string_0;
		stateMachine.list_0 = list_0;
		stateMachine.list_1 = list_1;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
