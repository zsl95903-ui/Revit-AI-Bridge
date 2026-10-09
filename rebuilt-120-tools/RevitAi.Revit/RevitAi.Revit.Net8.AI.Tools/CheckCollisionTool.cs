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
using RevitAi.Abstractions.Collision;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("check_collision", Category = "碰撞检查", Description = "检查 Revit 元素之间的碰撞关系。支持主模型与链接模型之间的碰撞检查。支持元素 ID、元素 ID 数组、缓存 ID、类别名称等多种输入方式，程序自动识别输入类型。", RequiresTransaction = false, RequiresModification = false)]
public sealed class CheckCollisionTool : IAITool
{
	[CompilerGenerated]
	private sealed class Class370
	{
		public HashSet<int> hashSet_0;

		internal bool method_0(int int_0)
		{
			return hashSet_0.Contains(int_0);
		}
	}

	[CompilerGenerated]
	private sealed class Class371 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CheckCollisionTool checkCollisionTool_0;

		private IElementService ielementService_0;

		private ILinkService ilinkService_0;

		private IAnalysisService ianalysisService_0;

		private int? nullable_0;

		private int? nullable_1;

		private bool bool_0;

		private object object_0;

		private object object_1;

		private List<int> list_0;

		private List<int> list_1;

		private object object_2;

		private object object_3;

		private List<int> list_2;

		private List<int> list_3;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<List<int>> taskAwaiter_1;

		private TaskAwaiter<AIToolResult> taskAwaiter_2;

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
					Class371 stateMachine = this;
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
				TaskAwaiter<List<int>> awaiter5;
				TaskAwaiter<List<int>> awaiter4;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter2;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4;
				switch (num)
				{
				default:
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					ilinkService_0 = ((revitAdapter2 != null) ? revitAdapter2.LinkService : null);
					IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
					ianalysisService_0 = ((revitAdapter3 != null) ? revitAdapter3.AnalysisService : null);
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
						nullable_0 = aitoolContext_0.GetParameter<int?>("sourceLinkInstanceId", (int?)null);
						nullable_1 = aitoolContext_0.GetParameter<int?>("targetLinkInstanceId", (int?)null);
						bool_0 = nullable_0.HasValue || nullable_1.HasValue;
						object_0 = aitoolContext_0.Document;
						object_1 = aitoolContext_0.Document;
						if (!nullable_0.HasValue)
						{
							goto IL_02b2;
						}
						object_2 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_0.Value);
						if (object_2 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
							defaultInterpolatedStringHandler.AppendLiteral("找不到源链接实例 ID ");
							defaultInterpolatedStringHandler.AppendFormatted(nullable_0.Value);
							result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							ILinkService obj = ilinkService_0;
							object_0 = ((obj != null) ? obj.GetLinkDocument(object_2) : null);
							if (object_0 != null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("[Collision] 源元素来自链接模型，链接实例 ID: ");
								defaultInterpolatedStringHandler2.AppendFormatted(nullable_0.Value);
								Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
								object_2 = null;
								goto IL_02b2;
							}
							result = AIToolResult.Fail("无法获取源链接文档，请检查链接是否已加载");
						}
					}
					goto end_IL_006e;
				}
				case 1:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<List<int>>);
					num = -1;
					int_0 = -1;
					goto IL_042b;
				case 2:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<List<int>>);
					num = -1;
					int_0 = -1;
					goto IL_04ec;
				case 3:
					awaiter3 = taskAwaiter_2;
					taskAwaiter_2 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_077f;
				case 4:
					{
						awaiter2 = taskAwaiter_2;
						taskAwaiter_2 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_042b:
					list_2 = awaiter5.GetResult();
					list_0 = list_2;
					list_2 = null;
					if (list_0.Count != 0)
					{
						awaiter4 = checkCollisionTool_0.method_0(aitoolContext_0, ielementService_0, "target", object_1).GetAwaiter();
						if (!awaiter4.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter4;
							Class371 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
							return;
						}
						goto IL_04ec;
					}
					result = AIToolResult.Fail("无法解析源元素：请提供有效的元素 ID、ID 数组、缓存 ID 或类别名称");
					goto end_IL_006e;
					IL_04ec:
					list_3 = awaiter4.GetResult();
					list_1 = list_3;
					list_3 = null;
					if (list_1.Count == 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(38, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("[Collision] 未提供目标元素，执行源元素自碰撞检查，源元素数量: ");
						defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
						Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
						list_1 = list_0;
						object_1 = object_0;
						nullable_1 = nullable_0;
					}
					defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(45, 4);
					defaultInterpolatedStringHandler4.AppendLiteral("[Collision] 开始碰撞检查：源元素 ");
					defaultInterpolatedStringHandler4.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler4.AppendLiteral(" 个（文档: ");
					defaultInterpolatedStringHandler4.AppendFormatted(nullable_0.HasValue ? "链接模型" : "主模型");
					defaultInterpolatedStringHandler4.AppendLiteral("），目标元素 ");
					defaultInterpolatedStringHandler4.AppendFormatted(list_1.Count);
					defaultInterpolatedStringHandler4.AppendLiteral(" 个（文档: ");
					defaultInterpolatedStringHandler4.AppendFormatted(nullable_1.HasValue ? "链接模型" : "主模型");
					defaultInterpolatedStringHandler4.AppendLiteral("）");
					Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
					if (bool_0 && ianalysisService_0 != null)
					{
						awaiter3 = checkCollisionTool_0.method_4(aitoolContext_0, ielementService_0, ianalysisService_0, object_0, object_1, list_0, list_1, nullable_0, nullable_1, cancellationToken_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 3;
							int_0 = 3;
							taskAwaiter_2 = awaiter3;
							Class371 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
							return;
						}
						goto IL_077f;
					}
					awaiter2 = checkCollisionTool_0.method_2(aitoolContext_0, ielementService_0, list_0, list_1, cancellationToken_0, nullable_0, nullable_1).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 4;
						int_0 = 4;
						taskAwaiter_2 = awaiter2;
						Class371 stateMachine = this;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
						return;
					}
					break;
					IL_02b2:
					if (!nullable_1.HasValue)
					{
						goto IL_03b3;
					}
					object_3 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_1.Value);
					if (object_3 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("找不到目标链接实例 ID ");
						defaultInterpolatedStringHandler5.AppendFormatted(nullable_1.Value);
						result = AIToolResult.Fail(defaultInterpolatedStringHandler5.ToStringAndClear());
					}
					else
					{
						ILinkService obj2 = ilinkService_0;
						object_1 = ((obj2 != null) ? obj2.GetLinkDocument(object_3) : null);
						if (object_1 != null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(32, 1);
							defaultInterpolatedStringHandler6.AppendLiteral("[Collision] 目标元素来自链接模型，链接实例 ID: ");
							defaultInterpolatedStringHandler6.AppendFormatted(nullable_1.Value);
							Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
							object_3 = null;
							goto IL_03b3;
						}
						result = AIToolResult.Fail("无法获取目标链接文档，请检查链接是否已加载");
					}
					goto end_IL_006e;
					IL_077f:
					aitoolResult_0 = awaiter3.GetResult();
					result = aitoolResult_0;
					goto end_IL_006e;
					IL_03b3:
					awaiter5 = checkCollisionTool_0.method_0(aitoolContext_0, ielementService_0, "source", object_0).GetAwaiter();
					if (!awaiter5.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter5;
						Class371 stateMachine = this;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
						return;
					}
					goto IL_042b;
				}
				aitoolResult_1 = awaiter2.GetResult();
				result = aitoolResult_1;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[Collision] 碰撞检查失败: " + exception_0.Message);
				result = AIToolResult.Fail("碰撞检查失败: " + exception_0.Message);
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
	private sealed class Class372 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public List<int> list_0;

		public List<int> list_1;

		public CancellationToken cancellationToken_0;

		public int? nullable_0;

		public int? nullable_1;

		public CheckCollisionTool checkCollisionTool_0;

		private List<CollisionResultItem> list_2;

		private int int_1;

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
					Class372 stateMachine = this;
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
			if (list_0.Count == 0 || list_1.Count == 0)
			{
				result = AIToolResult.Ok("碰撞检查完成，但没有有效的元素对", (object)new Class23<object[], int, int, int[]>(Array.Empty<object>(), 0, 0, Array.Empty<int>()));
			}
			else
			{
				list_2 = ielementService_0.CheckCollisionsBatch(aitoolContext_0.Document, (IEnumerable<int>)list_0, (IEnumerable<int>)list_1).ToList();
				if (list_2.Count == 0)
				{
					result = AIToolResult.Ok("碰撞检查完成，未发现碰撞", (object)new Class23<object[], int, int, int[]>(Array.Empty<object>(), 0, 0, Array.Empty<int>()));
				}
				else
				{
					int_1 = smethod_0(list_0, list_1);
					result = checkCollisionTool_0.method_5(aitoolContext_0, list_2, int_1, nullable_0, nullable_1);
				}
			}
			int_0 = -2;
			list_2 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class373 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public IAnalysisService ianalysisService_0;

		public object object_0;

		public object object_1;

		public List<int> list_0;

		public List<int> list_1;

		public int? nullable_0;

		public int? nullable_1;

		public CancellationToken cancellationToken_0;

		public CheckCollisionTool checkCollisionTool_0;

		private bool bool_0;

		private List<object> list_2;

		private Dictionary<int, IEnumerable<int>> dictionary_0;

		private int int_1;

		private List<(int DocumentIndex1, int ElementId1, int DocumentIndex2, int ElementId2, IEnumerable<(double X, double Y, double Z)> CollisionPoints)> list_3;

		private List<object> list_4;

		private HashSet<int> hashSet_0;

		private List<List<int>> list_5;

		private List<int> list_6;

		private int int_2;

		private string string_0;

		private string string_1;

		private string string_2;

		private string string_3;

		private List<CollisionResultItem> list_7;

		private int int_3;

		private int int_4;

		private List<(int DocumentIndex1, int ElementId1, int DocumentIndex2, int ElementId2, IEnumerable<(double X, double Y, double Z)> CollisionPoints)>.Enumerator enumerator_0;

		private (int DocumentIndex1, int ElementId1, int DocumentIndex2, int ElementId2, IEnumerable<(double X, double Y, double Z)> CollisionPoints) valueTuple_0;

		private int int_5;

		private int int_6;

		private object object_2;

		private object object_3;

		private object object_4;

		private object object_5;

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
					Class373 stateMachine = this;
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
			object obj;
			if (ianalysisService_0 == null)
			{
				result = AIToolResult.Fail("无法获取 AnalysisService，不支持跨文档碰撞检查");
			}
			else if (object_0 == null || object_1 == null)
			{
				result = AIToolResult.Fail("文档对象为空");
			}
			else
			{
				bool_0 = object_0 == object_1 || object_0.GetHashCode() == object_1.GetHashCode();
				if (bool_0)
				{
					list_7 = ielementService_0.CheckCollisionsBatch(aitoolContext_0.Document, (IEnumerable<int>)list_0, (IEnumerable<int>)list_1).ToList();
					int_3 = smethod_0(list_0, list_1);
					result = checkCollisionTool_0.method_5(aitoolContext_0, list_7, int_3, nullable_0, nullable_1);
				}
				else
				{
					list_2 = new List<object>();
					dictionary_0 = new Dictionary<int, IEnumerable<int>>();
					int_1 = 0;
					list_2.Add(object_0);
					dictionary_0.Add(int_1, list_0);
					if (!bool_0)
					{
						int_4 = 1;
						list_2.Add(object_1);
						dictionary_0.Add(int_4, list_1);
					}
					list_3 = ianalysisService_0.CheckCollisionsCrossDocument((IList<object>)list_2, (IDictionary<int, IEnumerable<int>>)dictionary_0).ToList();
					if (list_3.Count != 0)
					{
						list_4 = new List<object>();
						hashSet_0 = new HashSet<int>();
						list_5 = new List<List<int>>();
						enumerator_0 = list_3.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								valueTuple_0 = enumerator_0.Current;
								int_5 = valueTuple_0.DocumentIndex1;
								int_6 = valueTuple_0.DocumentIndex2;
								hashSet_0.Add(valueTuple_0.ElementId1);
								hashSet_0.Add(valueTuple_0.ElementId2);
								list_5.Add(new List<int> { valueTuple_0.ElementId1, valueTuple_0.ElementId2 });
								object_2 = list_2[int_5];
								object_3 = list_2[int_6];
								object_4 = ielementService_0.GetElementById(object_2, valueTuple_0.ElementId1);
								object_5 = ielementService_0.GetElementById(object_3, valueTuple_0.ElementId2);
								list_4.Add(new Class25<int, string, int?, string, string, int, string, int?, string, string, List<_003C_003Ef__AnonymousType27<double, double, double>>>(valueTuple_0.ElementId1, (int_5 == 0) ? "主模型" : "链接模型", (int_5 == 1) ? nullable_0 : nullable_1, (object_4 != null) ? ielementService_0.GetElementCategory(object_4) : "", (object_4 != null) ? ielementService_0.GetElementTypeName(object_4) : "", valueTuple_0.ElementId2, (int_6 == 0) ? "主模型" : "链接模型", (int_6 == 1) ? nullable_1 : nullable_0, (object_5 != null) ? ielementService_0.GetElementCategory(object_5) : "", (object_5 != null) ? ielementService_0.GetElementTypeName(object_5) : "", valueTuple_0.CollisionPoints.Select(((double X, double Y, double Z) valueTuple_0) => new _003C_003Ef__AnonymousType27<double, double, double>(valueTuple_0.X, valueTuple_0.Y, valueTuple_0.Z)).ToList()));
								object_2 = null;
								object_3 = null;
								object_4 = null;
								object_5 = null;
								valueTuple_0 = default((int, int, int, int, IEnumerable<(double, double, double)>));
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
							}
						}
						enumerator_0 = default(List<(int, int, int, int, IEnumerable<(double, double, double)>)>.Enumerator);
						list_6 = hashSet_0.ToList();
						int_2 = list_3.Count;
						string_0 = aitoolContext_0.SessionId ?? Guid.NewGuid().ToString("N");
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 2);
						defaultInterpolatedStringHandler.AppendLiteral("collision_items_cross_");
						defaultInterpolatedStringHandler.AppendFormatted(string_0);
						defaultInterpolatedStringHandler.AppendLiteral("_");
						defaultInterpolatedStringHandler.AppendFormatted(Guid.NewGuid(), "N");
						string_1 = defaultInterpolatedStringHandler.ToStringAndClear();
						IAIToolDataCache dataCache = aitoolContext_0.DataCache;
						if (dataCache == null)
						{
							obj = null;
						}
						else
						{
							obj = dataCache.Store<List<object>>(string_0, string_1, list_4);
							if (obj != null)
							{
								goto IL_060f;
							}
						}
						obj = string.Empty;
						goto IL_060f;
					}
					result = AIToolResult.Ok("跨文档碰撞检查完成，未发现碰撞", (object)new Class24<object[], int, int, int[], int?, int?, bool>(Array.Empty<object>(), 0, 0, Array.Empty<int>(), nullable_0, nullable_1, gparam_13: true));
				}
			}
			goto IL_0730;
			IL_0730:
			int_0 = -2;
			list_2 = null;
			dictionary_0 = null;
			list_3 = null;
			list_4 = null;
			hashSet_0 = null;
			list_5 = null;
			list_6 = null;
			string_0 = null;
			string_1 = null;
			string_2 = null;
			string_3 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_060f:
			string_2 = (string)obj;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(19, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("跨文档碰撞检查完成：发现 ");
			defaultInterpolatedStringHandler2.AppendFormatted(int_2);
			defaultInterpolatedStringHandler2.AppendLiteral(" 对存在碰撞");
			string_3 = defaultInterpolatedStringHandler2.ToStringAndClear() + ((nullable_0.HasValue || nullable_1.HasValue) ? "（涉及链接模型）" : "");
			result = AIToolResult.Ok(string_3 + "。已缓存完整结果。\n\n💡 使用 get_cache_data 工具并传入 cache_id=" + string_2 + " 获取完整结果", (object)new Class26<List<object>, int, int, List<int>, List<List<int>>, bool, string, int?, int?, bool, ns0.Class27<string, string, int, int>>(list_4.Take(50).ToList(), int_2, int_2, list_6, list_5, int_2 > 50, string_2, nullable_0, nullable_1, gparam_20: true, new ns0.Class27<string, string, int, int>(string_2, "跨文档碰撞结果已缓存", int_2, Math.Min(50, int_2))));
			goto IL_0730;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class374 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<int>> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public string string_0;

		public object object_0;

		public CheckCollisionTool checkCollisionTool_0;

		private object object_1;

		private List<int> list_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<List<int>> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter<List<int>> awaiter;
			TaskAwaiter awaiter2;
			if (num != 0)
			{
				if (num == 1)
				{
					awaiter = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<List<int>>);
					num = -1;
					int_0 = -1;
					goto IL_0121;
				}
				awaiter2 = Task.CompletedTask.GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter2;
					Class374 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
					return;
				}
			}
			else
			{
				awaiter2 = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter);
				num = -1;
				int_0 = -1;
			}
			awaiter2.GetResult();
			List<int> result;
			if (!aitoolContext_0.HasParameter(string_0))
			{
				result = new List<int>();
				goto IL_0138;
			}
			object_1 = aitoolContext_0.GetParameter<object>(string_0, (object)null);
			awaiter = checkCollisionTool_0.method_1(aitoolContext_0, ielementService_0, object_1, string_0, object_0).GetAwaiter();
			if (!awaiter.IsCompleted)
			{
				num = 1;
				int_0 = 1;
				taskAwaiter_1 = awaiter;
				Class374 stateMachine = this;
				asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
				return;
			}
			goto IL_0121;
			IL_0121:
			list_0 = awaiter.GetResult();
			result = list_0;
			goto IL_0138;
			IL_0138:
			int_0 = -2;
			object_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class375 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<List<int>> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public IElementService ielementService_0;

		public object object_0;

		public string string_0;

		public object object_1;

		public CheckCollisionTool checkCollisionTool_0;

		private List<int> list_0;

		private long long_0;

		private int int_1;

		private IEnumerable ienumerable_0;

		private string string_1;

		private bool bool_0;

		private List<string> list_1;

		private List<int> list_2;

		private IEnumerator ienumerator_0;

		private object object_2;

		private string string_2;

		private IConvertible iconvertible_0;

		private int int_2;

		private List<string>.Enumerator enumerator_0;

		private string string_3;

		private object object_3;

		private List<int> list_3;

		private IEnumerable<object> ienumerable_1;

		private IEnumerator<object> ienumerator_1;

		private object object_4;

		private int? nullable_0;

		private object object_5;

		private IEnumerable<object> ienumerable_2;

		private IEnumerator<object> ienumerator_2;

		private object object_6;

		private int? nullable_1;

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
					Class375 stateMachine = this;
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
			list_0 = new List<int>();
			List<int> result;
			try
			{
				if (object_0 is long)
				{
					long_0 = (long)object_0;
					if (long_0 > 0L)
					{
						list_0.Add((int)long_0);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[Collision] ");
						defaultInterpolatedStringHandler.AppendFormatted(string_0);
						defaultInterpolatedStringHandler.AppendLiteral(": 单个元素 ID ");
						defaultInterpolatedStringHandler.AppendFormatted(long_0);
						Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
						result = list_0;
						goto IL_099f;
					}
				}
				if (object_0 is int)
				{
					int_1 = (int)object_0;
					if (int_1 > 0)
					{
						list_0.Add(int_1);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(22, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("[Collision] ");
						defaultInterpolatedStringHandler2.AppendFormatted(string_0);
						defaultInterpolatedStringHandler2.AppendLiteral(": 单个元素 ID ");
						defaultInterpolatedStringHandler2.AppendFormatted(int_1);
						Logger.Debug(defaultInterpolatedStringHandler2.ToStringAndClear());
						result = list_0;
						goto IL_099f;
					}
				}
				ienumerable_0 = object_0 as IEnumerable;
				if (ienumerable_0 != null && !(object_0 is string))
				{
					bool_0 = true;
					list_1 = new List<string>();
					list_2 = new List<int>();
					ienumerator_0 = ienumerable_0.GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							object_2 = ienumerator_0.Current;
							if (object_2 != null)
							{
								string_2 = object_2 as string;
								if (string_2 != null && !string.IsNullOrEmpty(string_2))
								{
									list_1.Add(string_2);
								}
								else
								{
									bool_0 = false;
									iconvertible_0 = object_2 as IConvertible;
									if (iconvertible_0 != null)
									{
										try
										{
											int_2 = Convert.ToInt32(iconvertible_0);
											if (int_2 > 0)
											{
												list_2.Add(int_2);
											}
										}
										catch
										{
										}
									}
									iconvertible_0 = null;
								}
								string_2 = null;
							}
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
					if (bool_0 && list_1.Count > 0)
					{
						enumerator_0 = list_1.GetEnumerator();
						try
						{
							while (enumerator_0.MoveNext())
							{
								string_3 = enumerator_0.Current;
								object_3 = aitoolContext_0.GetCachedData<object>(string_3);
								if (object_3 != null)
								{
									list_3 = checkCollisionTool_0.method_3(object_3);
									list_0.AddRange(list_3);
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(31, 3);
									defaultInterpolatedStringHandler3.AppendLiteral("[Collision] ");
									defaultInterpolatedStringHandler3.AppendFormatted(string_0);
									defaultInterpolatedStringHandler3.AppendLiteral(": 缓存 ID '");
									defaultInterpolatedStringHandler3.AppendFormatted(string_3);
									defaultInterpolatedStringHandler3.AppendLiteral("'，提取到 ");
									defaultInterpolatedStringHandler3.AppendFormatted(list_3.Count);
									defaultInterpolatedStringHandler3.AppendLiteral(" 个元素");
									Logger.Debug(defaultInterpolatedStringHandler3.ToStringAndClear());
									list_3 = null;
								}
								else
								{
									ienumerable_1 = ielementService_0.GetElementsByCategory(object_1, string_3);
									ienumerator_1 = ienumerable_1.GetEnumerator();
									try
									{
										while (ienumerator_1.MoveNext())
										{
											object_4 = ienumerator_1.Current;
											nullable_0 = ielementService_0.GetElementId(object_4);
											if (nullable_0.HasValue && nullable_0.Value > 0)
											{
												list_0.Add(nullable_0.Value);
											}
											object_4 = null;
										}
									}
									finally
									{
										if (num < 0 && ienumerator_1 != null)
										{
											ienumerator_1.Dispose();
										}
									}
									ienumerator_1 = null;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(29, 3);
									defaultInterpolatedStringHandler4.AppendLiteral("[Collision] ");
									defaultInterpolatedStringHandler4.AppendFormatted(string_0);
									defaultInterpolatedStringHandler4.AppendLiteral(": 类别名称 '");
									defaultInterpolatedStringHandler4.AppendFormatted(string_3);
									defaultInterpolatedStringHandler4.AppendLiteral("'，找到 ");
									defaultInterpolatedStringHandler4.AppendFormatted(ielementService_0.GetElementsByCategory(object_1, string_3).Count());
									defaultInterpolatedStringHandler4.AppendLiteral(" 个元素");
									Logger.Debug(defaultInterpolatedStringHandler4.ToStringAndClear());
									ienumerable_1 = null;
								}
								object_3 = null;
								string_3 = null;
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
							}
						}
						enumerator_0 = default(List<string>.Enumerator);
						list_0 = list_0.Distinct().ToList();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(29, 2);
						defaultInterpolatedStringHandler5.AppendLiteral("[Collision] ");
						defaultInterpolatedStringHandler5.AppendFormatted(string_0);
						defaultInterpolatedStringHandler5.AppendLiteral(": 类别名称数组，共提取 ");
						defaultInterpolatedStringHandler5.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler5.AppendLiteral(" 个元素");
						Logger.Debug(defaultInterpolatedStringHandler5.ToStringAndClear());
						result = list_0;
					}
					else
					{
						list_0 = list_2.Distinct().ToList();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(27, 2);
						defaultInterpolatedStringHandler6.AppendLiteral("[Collision] ");
						defaultInterpolatedStringHandler6.AppendFormatted(string_0);
						defaultInterpolatedStringHandler6.AppendLiteral(": 元素 ID 数组，共 ");
						defaultInterpolatedStringHandler6.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler6.AppendLiteral(" 个");
						Logger.Debug(defaultInterpolatedStringHandler6.ToStringAndClear());
						result = list_0;
					}
					goto IL_099f;
				}
				string_1 = object_0 as string;
				if (string_1 != null && !string.IsNullOrEmpty(string_1))
				{
					object_5 = aitoolContext_0.GetCachedData<object>(string_1);
					if (object_5 != null)
					{
						list_0 = checkCollisionTool_0.method_3(object_5);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(31, 3);
						defaultInterpolatedStringHandler7.AppendLiteral("[Collision] ");
						defaultInterpolatedStringHandler7.AppendFormatted(string_0);
						defaultInterpolatedStringHandler7.AppendLiteral(": 缓存 ID '");
						defaultInterpolatedStringHandler7.AppendFormatted(string_1);
						defaultInterpolatedStringHandler7.AppendLiteral("'，提取到 ");
						defaultInterpolatedStringHandler7.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler7.AppendLiteral(" 个元素");
						Logger.Debug(defaultInterpolatedStringHandler7.ToStringAndClear());
						result = list_0;
					}
					else
					{
						ienumerable_2 = ielementService_0.GetElementsByCategory(object_1, string_1);
						ienumerator_2 = ienumerable_2.GetEnumerator();
						try
						{
							while (ienumerator_2.MoveNext())
							{
								object_6 = ienumerator_2.Current;
								nullable_1 = ielementService_0.GetElementId(object_6);
								if (nullable_1.HasValue && nullable_1.Value > 0)
								{
									list_0.Add(nullable_1.Value);
								}
								object_6 = null;
							}
						}
						finally
						{
							if (num < 0 && ienumerator_2 != null)
							{
								ienumerator_2.Dispose();
							}
						}
						ienumerator_2 = null;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(29, 3);
						defaultInterpolatedStringHandler8.AppendLiteral("[Collision] ");
						defaultInterpolatedStringHandler8.AppendFormatted(string_0);
						defaultInterpolatedStringHandler8.AppendLiteral(": 类别名称 '");
						defaultInterpolatedStringHandler8.AppendFormatted(string_1);
						defaultInterpolatedStringHandler8.AppendLiteral("'，找到 ");
						defaultInterpolatedStringHandler8.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler8.AppendLiteral(" 个元素");
						Logger.Debug(defaultInterpolatedStringHandler8.ToStringAndClear());
						result = list_0;
					}
					goto IL_099f;
				}
				ienumerable_0 = null;
				string_1 = null;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Warning("[Collision] 解析 " + string_0 + " 失败: " + exception_0.Message);
			}
			result = list_0;
			goto IL_099f;
			IL_099f:
			int_0 = -2;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "check_collision";

	public string Category => "碰撞检查";

	public string Description => "检查 Revit 元素之间的碰撞关系";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"source\": {\n                \"description\": \"源元素：可以是单个元素 ID、元素 ID 数组、缓存 ID 或类别名称（中文）\",\n                \"oneOf\": [\n                    { \"type\": \"integer\", \"description\": \"单个源元素 ID\" },\n                    { \"type\": \"array\", \"items\": { \"type\": \"integer\" }, \"description\": \"源元素 ID 数组\" },\n                    { \"type\": \"string\", \"description\": \"源元素缓存 ID 或类别名称（中文）\" },\n                    { \"type\": \"array\", \"items\": { \"type\": \"string\" }, \"description\": \"源类别名称数组（中文）\" }\n                ]\n            },\n            \"target\": {\n                \"description\": \"目标元素：可以是单个元素 ID、元素 ID 数组、缓存 ID 或类别名称（中文）。如果不提供，则检查源元素之间的自碰撞\",\n                \"oneOf\": [\n                    { \"type\": \"integer\", \"description\": \"单个目标元素 ID\" },\n                    { \"type\": \"array\", \"items\": { \"type\": \"integer\" }, \"description\": \"目标元素 ID 数组\" },\n                    { \"type\": \"string\", \"description\": \"目标元素缓存 ID 或类别名称（中文）\" },\n                    { \"type\": \"array\", \"items\": { \"type\": \"string\" }, \"description\": \"目标类别名称数组（中文）\" }\n                ]\n            },\n            \"sourceLinkInstanceId\": {\n                \"type\": \"integer\",\n                \"description\": \"源链接实例 ID（可选）。当源元素来自链接模型时，提供链接实例 ID。从 get_links 工具获取\"\n            },\n            \"targetLinkInstanceId\": {\n                \"type\": \"integer\",\n                \"description\": \"目标链接实例 ID（可选）。当目标元素来自链接模型时，提供链接实例 ID。从 get_links 工具获取\"\n            }\n        },\n        \"required\": [\"source\"]\n    }";

	[AsyncStateMachine(typeof(Class371))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class371 stateMachine = new Class371();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.checkCollisionTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class374))]
	[DebuggerStepThrough]
	private Task<List<int>> method_0(AIToolContext aitoolContext_0, IElementService ielementService_0, string string_0, object object_0)
	{
		Class374 stateMachine = new Class374();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<int>>.Create();
		stateMachine.checkCollisionTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.string_0 = string_0;
		stateMachine.object_0 = object_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class375))]
	[DebuggerStepThrough]
	private Task<List<int>> method_1(AIToolContext aitoolContext_0, IElementService ielementService_0, object object_0, string string_0, object object_1)
	{
		Class375 stateMachine = new Class375();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<List<int>>.Create();
		stateMachine.checkCollisionTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.object_0 = object_0;
		stateMachine.string_0 = string_0;
		stateMachine.object_1 = object_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class372))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, IElementService ielementService_0, List<int> list_0, List<int> list_1, CancellationToken cancellationToken_0, int? nullable_0 = null, int? nullable_1 = null)
	{
		Class372 stateMachine = new Class372();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.checkCollisionTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.list_0 = list_0;
		stateMachine.list_1 = list_1;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.nullable_0 = nullable_0;
		stateMachine.nullable_1 = nullable_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private static int smethod_0(List<int> list_0, List<int> list_1)
	{
		if (list_0.Count == list_1.Count && !list_0.Except(list_1).Any())
		{
			return list_0.Count * (list_0.Count - 1) / 2;
		}
		HashSet<int> hashSet_0 = new HashSet<int>(list_1);
		int num = list_0.Count((int int_0) => hashSet_0.Contains(int_0));
		return list_0.Count * list_1.Count - num;
	}

	private List<int> method_3(object object_0)
	{
		List<int> list = new List<int>();
		try
		{
			if (object_0 is IEnumerable enumerable)
			{
				foreach (object item in enumerable)
				{
					if (item == null)
					{
						continue;
					}
					PropertyInfo property = item.GetType().GetProperty("id");
					if (property != null)
					{
						object value = property.GetValue(item);
						if (value is int num && num > 0)
						{
							list.Add(num);
						}
						else if (value is long num2 && num2 > 0L)
						{
							list.Add((int)num2);
						}
					}
					PropertyInfo property2 = item.GetType().GetProperty("elementId");
					if (property2 != null)
					{
						object value2 = property2.GetValue(item);
						if (value2 is int num3 && num3 > 0)
						{
							list.Add(num3);
						}
						else if (value2 is long num4 && num4 > 0L)
						{
							list.Add((int)num4);
						}
					}
				}
			}
			list = list.Distinct().ToList();
		}
		catch (Exception ex)
		{
			Logger.Warning("[Collision] 提取元素 ID 失败: " + ex.Message);
		}
		return list;
	}

	[AsyncStateMachine(typeof(Class373))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_4(AIToolContext aitoolContext_0, IElementService ielementService_0, IAnalysisService ianalysisService_0, object? object_0, object? object_1, List<int> list_0, List<int> list_1, int? nullable_0, int? nullable_1, CancellationToken cancellationToken_0)
	{
		Class373 stateMachine = new Class373();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.checkCollisionTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.ielementService_0 = ielementService_0;
		stateMachine.ianalysisService_0 = ianalysisService_0;
		stateMachine.object_0 = object_0;
		stateMachine.object_1 = object_1;
		stateMachine.list_0 = list_0;
		stateMachine.list_1 = list_1;
		stateMachine.nullable_0 = nullable_0;
		stateMachine.nullable_1 = nullable_1;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private AIToolResult method_5(AIToolContext aitoolContext_0, List<CollisionResultItem> list_0, int int_0, int? nullable_0, int? nullable_1)
	{
		int count = list_0.Count;
		double num = ((int_0 > 0) ? ((double)count / (double)int_0 * 100.0) : 0.0);
		HashSet<int> hashSet = new HashSet<int>();
		List<List<int>> list = new List<List<int>>();
		foreach (CollisionResultItem item in list_0)
		{
			hashSet.Add(item.SourceElementId);
			hashSet.Add(item.TargetElementId);
			list.Add(new List<int> { item.SourceElementId, item.TargetElementId });
		}
		List<int> list2 = hashSet.OrderBy((int result) => result).ToList();
		int num2 = Math.Min(count, 50);
		var list3 = (from collisionResultItem_0 in list_0.Take(num2)
			select new _003C_003Ef__AnonymousType30<int, string, string, string, int, string, string, string>(collisionResultItem_0.SourceElementId, collisionResultItem_0.SourceCategoryName, collisionResultItem_0.SourceTypeName, collisionResultItem_0.SourceFamilyName, collisionResultItem_0.TargetElementId, collisionResultItem_0.TargetCategoryName, collisionResultItem_0.TargetTypeName, collisionResultItem_0.TargetFamilyName)).ToList();
		string text;
		object obj;
		if (count <= 50)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 3);
			defaultInterpolatedStringHandler.AppendLiteral("碰撞检查完成：共检查 ");
			defaultInterpolatedStringHandler.AppendFormatted(int_0);
			defaultInterpolatedStringHandler.AppendLiteral(" 对元素，发现 ");
			defaultInterpolatedStringHandler.AppendFormatted(count);
			defaultInterpolatedStringHandler.AppendLiteral(" 对存在碰撞（碰撞率 ");
			defaultInterpolatedStringHandler.AppendFormatted(num, "F1");
			defaultInterpolatedStringHandler.AppendLiteral("%）");
			text = defaultInterpolatedStringHandler.ToStringAndClear() + ((nullable_0.HasValue || nullable_1.HasValue) ? "（涉及链接模型）" : "");
			obj = new Class28<List<_003C_003Ef__AnonymousType30<int, string, string, string, int, string, string, string>>, int, int, double, int, List<int>, List<List<int>>, bool, int?, int?>(list3, count, count, num, int_0, list2, list, gparam_17: false, nullable_0, nullable_1);
			goto IL_051d;
		}
		string text2 = aitoolContext_0.SessionId ?? Guid.NewGuid().ToString("N");
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 2);
		defaultInterpolatedStringHandler2.AppendLiteral("collision_items_");
		defaultInterpolatedStringHandler2.AppendFormatted(text2);
		defaultInterpolatedStringHandler2.AppendLiteral("_");
		defaultInterpolatedStringHandler2.AppendFormatted(Guid.NewGuid(), "N");
		string text3 = defaultInterpolatedStringHandler2.ToStringAndClear();
		IAIToolDataCache dataCache = aitoolContext_0.DataCache;
		object obj2;
		if (dataCache == null)
		{
			obj2 = null;
		}
		else
		{
			obj2 = dataCache.Store<List<CollisionResultItem>>(text2, text3, list_0);
			if (obj2 != null)
			{
				goto IL_0273;
			}
		}
		obj2 = string.Empty;
		goto IL_0273;
		IL_0361:
		object obj3;
		string text4 = (string)obj3;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(45, 4);
		defaultInterpolatedStringHandler3.AppendLiteral("碰撞检查完成：共检查 ");
		defaultInterpolatedStringHandler3.AppendFormatted(int_0);
		defaultInterpolatedStringHandler3.AppendLiteral(" 对元素，发现 ");
		defaultInterpolatedStringHandler3.AppendFormatted(count);
		defaultInterpolatedStringHandler3.AppendLiteral(" 对存在碰撞（碰撞率 ");
		defaultInterpolatedStringHandler3.AppendFormatted(num, "F1");
		defaultInterpolatedStringHandler3.AppendLiteral("%）。已返回前 ");
		defaultInterpolatedStringHandler3.AppendFormatted(num2);
		defaultInterpolatedStringHandler3.AppendLiteral(" 对碰撞结果。");
		string text5 = defaultInterpolatedStringHandler3.ToStringAndClear();
		string obj4 = ((nullable_0.HasValue || nullable_1.HasValue) ? "（涉及链接模型）" : "");
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(62, 3);
		defaultInterpolatedStringHandler4.AppendLiteral("\n\n💡 碰撞数据已分别缓存：\n- 着色使用：elements=");
		string text6;
		defaultInterpolatedStringHandler4.AppendFormatted(text6);
		defaultInterpolatedStringHandler4.AppendLiteral("\n- 创建视图使用：pairs=");
		defaultInterpolatedStringHandler4.AppendFormatted(text4);
		defaultInterpolatedStringHandler4.AppendLiteral("\n- 完整详情：items=");
		string text7;
		defaultInterpolatedStringHandler4.AppendFormatted(text7);
		text = text5 + obj4 + defaultInterpolatedStringHandler4.ToStringAndClear();
		obj = new Class29<List<_003C_003Ef__AnonymousType30<int, string, string, string, int, string, string, string>>, int, int, int, double, int, bool, Class30<string, string, string>, Class31<string, string, string, string, int, int>, int?, int?>(list3, num2, count, count, num, int_0, gparam_17: true, new Class30<string, string, string>(text7, text6, text4), new Class31<string, string, string, string, int, int>(text7, text6, text4, "碰撞数据已分别缓存", count, num2), nullable_0, nullable_1);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(47, 3);
		defaultInterpolatedStringHandler5.AppendLiteral("[Collision] 缓存已创建: items=");
		defaultInterpolatedStringHandler5.AppendFormatted(text7);
		defaultInterpolatedStringHandler5.AppendLiteral(", element_ids=");
		defaultInterpolatedStringHandler5.AppendFormatted(text6);
		defaultInterpolatedStringHandler5.AppendLiteral(", pairs=");
		defaultInterpolatedStringHandler5.AppendFormatted(text4);
		Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
		goto IL_051d;
		IL_051d:
		Logger.Info("[Collision] " + text);
		return AIToolResult.Ok(text, obj);
		IL_0273:
		text7 = (string)obj2;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(23, 2);
		defaultInterpolatedStringHandler6.AppendLiteral("collision_element_ids_");
		defaultInterpolatedStringHandler6.AppendFormatted(text2);
		defaultInterpolatedStringHandler6.AppendLiteral("_");
		defaultInterpolatedStringHandler6.AppendFormatted(Guid.NewGuid(), "N");
		string text8 = defaultInterpolatedStringHandler6.ToStringAndClear();
		IAIToolDataCache dataCache2 = aitoolContext_0.DataCache;
		object obj5;
		if (dataCache2 == null)
		{
			obj5 = null;
		}
		else
		{
			obj5 = dataCache2.Store<List<int>>(text2, text8, list2);
			if (obj5 != null)
			{
				goto IL_02ea;
			}
		}
		obj5 = string.Empty;
		goto IL_02ea;
		IL_02ea:
		text6 = (string)obj5;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(17, 2);
		defaultInterpolatedStringHandler7.AppendLiteral("collision_pairs_");
		defaultInterpolatedStringHandler7.AppendFormatted(text2);
		defaultInterpolatedStringHandler7.AppendLiteral("_");
		defaultInterpolatedStringHandler7.AppendFormatted(Guid.NewGuid(), "N");
		string text9 = defaultInterpolatedStringHandler7.ToStringAndClear();
		IAIToolDataCache dataCache3 = aitoolContext_0.DataCache;
		if (dataCache3 == null)
		{
			obj3 = null;
		}
		else
		{
			obj3 = dataCache3.Store<List<List<int>>>(text2, text9, list);
			if (obj3 != null)
			{
				goto IL_0361;
			}
		}
		obj3 = string.Empty;
		goto IL_0361;
	}
}
