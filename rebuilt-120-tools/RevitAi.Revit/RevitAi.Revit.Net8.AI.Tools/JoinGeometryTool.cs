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

[AITool("join_geometry", Category = "几何操作", Description = "执行实心元素之间的几何连接操作，包括连接、取消连接和切换连接顺序。适用于墙、柱、梁、楼板等实心元素之间的几何关系处理（如墙与墙连接、墙与楼板连接、梁与柱连接等）。注意：如需使用空心模型剪切实心模型，请使用 cut_geometry 工具。", RequiresTransaction = true, RequiresModification = true)]
public sealed class JoinGeometryTool : IAITool
{
	[CompilerGenerated]
	private static class Class542
	{
		public static Converter<object, int> converter_0;
	}

	[CompilerGenerated]
	public sealed class Class543 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public JoinGeometryTool joinGeometryTool_0;

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
					Class543 stateMachine = this;
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
					goto IL_039b;
				}
				TaskAwaiter<AIToolResult> awaiter3;
				if (num == 2)
				{
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0368;
				}
				string_0 = aitoolContext_0.GetParameter<string>("operation", "join");
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
					list_0 = joinGeometryTool_0.method_0(aitoolContext_0, ielementService_0, "element1Id", "element1Ids", "element1CacheId");
					if (list_0 == null || list_0.Count == 0)
					{
						result = AIToolResult.Fail("第一个元素 ID 列表不能为空。请提供 element1Id（单个）、element1Ids（数组）或 element1CacheId（缓存）参数");
					}
					else
					{
						list_1 = joinGeometryTool_0.method_0(aitoolContext_0, ielementService_0, "element2Id", "element2Ids", "element2CacheId");
						if (list_1 != null && list_1.Count != 0)
						{
							if (list_0.Count == 1 && list_1.Count == 1 && !string_0.Equals("switch", StringComparison.OrdinalIgnoreCase))
							{
								awaiter2 = joinGeometryTool_0.method_1(object_0, ielementService_0, imodificationService_0, string_0, list_0[0], list_1[0]).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = 1;
									int_0 = 1;
									taskAwaiter_1 = awaiter2;
									Class543 stateMachine = this;
									asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
									return;
								}
								goto IL_039b;
							}
							awaiter3 = joinGeometryTool_0.method_2(aitoolContext_0, object_0, ielementService_0, imodificationService_0, string_0, list_0, list_1, cancellationToken_0).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = 2;
								int_0 = 2;
								taskAwaiter_1 = awaiter3;
								Class543 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
								return;
							}
							goto IL_0368;
						}
						result = AIToolResult.Fail("第二个元素 ID 列表不能为空。请提供 element2Id（单个）、element2Ids（数组）或 element2CacheId（缓存）参数");
					}
				}
				goto end_IL_006e;
				IL_0368:
				aitoolResult_1 = awaiter3.GetResult();
				result = aitoolResult_1;
				goto end_IL_006e;
				IL_039b:
				aitoolResult_0 = awaiter2.GetResult();
				result = aitoolResult_0;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("几何连接操作失败: " + exception_0.Message);
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
	public sealed class Class544 : IAsyncStateMachine
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

		public JoinGeometryTool joinGeometryTool_0;

		private string string_1;

		private int int_1;

		private int int_2;

		private int int_3;

		private List<object> list_2;

		private int int_4;

		private bool bool_0;

		private object object_1;

		private string string_2;

		private List<int>.Enumerator enumerator_0;

		private int int_5;

		private object object_2;

		private string string_3;

		private List<int>.Enumerator enumerator_1;

		private int int_6;

		private List<int>.Enumerator enumerator_2;

		private int int_7;

		private object object_3;

		private string string_4;

		private bool bool_1;

		private bool bool_2;

		private bool bool_3;

		private bool bool_4;

		private string string_5;

		private string string_6;

		private string string_7;

		private List<object> list_3;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			string_1 = (string_0.Equals("join", StringComparison.OrdinalIgnoreCase) ? "连接" : (string_0.Equals("unjoin", StringComparison.OrdinalIgnoreCase) ? "取消连接" : (string_0.Equals("switch", StringComparison.OrdinalIgnoreCase) ? "切换连接顺序" : "操作")));
			int_1 = 0;
			int_2 = 0;
			int_3 = 0;
			list_2 = new List<object>();
			enumerator_0 = list_0.GetEnumerator();
			AIToolResult result;
			try
			{
				while (enumerator_0.MoveNext())
				{
					int_5 = enumerator_0.Current;
					if (!cancellationToken_0.IsCancellationRequested)
					{
						object_2 = ielementService_0.GetElementById(object_0, int_5);
						if (object_2 == null)
						{
							enumerator_1 = list_1.GetEnumerator();
							try
							{
								while (enumerator_1.MoveNext())
								{
									int_6 = enumerator_1.Current;
									list_2.Add(new Class234<int, int, string>(int_5, int_6, "找不到第一个元素"));
									int_2++;
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
							continue;
						}
						string_3 = ielementService_0.GetElementName(object_2);
						enumerator_2 = list_1.GetEnumerator();
						try
						{
							while (enumerator_2.MoveNext())
							{
								int_7 = enumerator_2.Current;
								if (!cancellationToken_0.IsCancellationRequested)
								{
									object_3 = ielementService_0.GetElementById(object_0, int_7);
									if (object_3 == null)
									{
										list_2.Add(new Class234<int, int, string>(int_5, int_7, "找不到第二个元素"));
										int_2++;
										continue;
									}
									string_4 = ielementService_0.GetElementName(object_3);
									bool_2 = false;
									if (string_0.Equals("join", StringComparison.OrdinalIgnoreCase))
									{
										bool_1 = imodificationService_0.JoinGeometry(object_0, object_2, object_3);
									}
									else if (string_0.Equals("unjoin", StringComparison.OrdinalIgnoreCase))
									{
										bool_3 = imodificationService_0.AreElementsJoined(object_2, object_3);
										if (!bool_3)
										{
											bool_2 = true;
											bool_1 = true;
										}
										else
										{
											bool_1 = imodificationService_0.UnjoinGeometry(object_0, object_2, object_3);
										}
									}
									else
									{
										if (!string_0.Equals("switch", StringComparison.OrdinalIgnoreCase))
										{
											result = AIToolResult.Fail("不支持的操作类型: " + string_0);
											goto IL_0ce9;
										}
										bool_4 = imodificationService_0.AreElementsJoined(object_2, object_3);
										if (!bool_4)
										{
											bool_1 = false;
										}
										else
										{
											bool_1 = imodificationService_0.SwitchJoinOrder(object_0, object_2, object_3);
										}
									}
									if (bool_1)
									{
										if (bool_2)
										{
											int_3++;
											list_2.Add(new Class235<int, int, string>(int_5, int_7, "已跳过（未连接）"));
										}
										else
										{
											int_1++;
											list_2.Add(new Class235<int, int, string>(int_5, int_7, "成功"));
										}
									}
									else
									{
										int_2++;
										list_2.Add(new Class235<int, int, string>(int_5, int_7, "失败"));
									}
									object_3 = null;
									string_4 = null;
									continue;
								}
								result = AIToolResult.Fail("操作已取消");
								goto IL_0ce9;
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator_2/*cast due to constrained. prefix*/).Dispose();
							}
						}
						enumerator_2 = default(List<int>.Enumerator);
						object_2 = null;
						string_3 = null;
						continue;
					}
					result = AIToolResult.Fail("操作已取消");
					goto IL_0ce9;
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
			int_4 = list_0.Count * list_1.Count;
			bool_0 = list_2.Count > 20;
			if (!bool_0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 4);
				defaultInterpolatedStringHandler.AppendLiteral("批量");
				defaultInterpolatedStringHandler.AppendFormatted(string_1);
				defaultInterpolatedStringHandler.AppendLiteral("完成：");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个第一组元素 × ");
				defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个第二组元素，成功 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(" 个");
				string text = defaultInterpolatedStringHandler.ToStringAndClear();
				string text2;
				if (int_3 <= 0)
				{
					text2 = "";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("，跳过 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_3);
					defaultInterpolatedStringHandler2.AppendLiteral(" 个");
					text2 = defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				string text3;
				if (int_2 <= 0)
				{
					text3 = "";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("，失败 ");
					defaultInterpolatedStringHandler3.AppendFormatted(int_2);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个");
					text3 = defaultInterpolatedStringHandler3.ToStringAndClear();
				}
				string_2 = text + text2 + text3;
				object_1 = new Class236<int, int, string, int, int, int, int, List<object>, bool>(list_0.Count, list_1.Count, string_0, int_4, int_1, int_3, int_2, list_2, gparam_17: false);
				goto IL_0cbb;
			}
			string_5 = aitoolContext_0.SessionId ?? Guid.NewGuid().ToString("N");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(14, 2);
			defaultInterpolatedStringHandler4.AppendLiteral("join_results_");
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
					goto IL_07af;
				}
			}
			obj = string.Empty;
			goto IL_07af;
			IL_07af:
			string_7 = (string)obj;
			list_3 = list_2.Take(20).ToList();
			if (string.IsNullOrEmpty(string_7))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(28, 4);
				defaultInterpolatedStringHandler5.AppendLiteral("批量");
				defaultInterpolatedStringHandler5.AppendFormatted(string_1);
				defaultInterpolatedStringHandler5.AppendLiteral("完成：");
				defaultInterpolatedStringHandler5.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler5.AppendLiteral(" 个第一组元素 × ");
				defaultInterpolatedStringHandler5.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler5.AppendLiteral(" 个第二组元素，成功 ");
				defaultInterpolatedStringHandler5.AppendFormatted(int_1);
				defaultInterpolatedStringHandler5.AppendLiteral(" 个");
				string text4 = defaultInterpolatedStringHandler5.ToStringAndClear();
				string text5;
				if (int_3 <= 0)
				{
					text5 = "";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler6.AppendLiteral("，跳过 ");
					defaultInterpolatedStringHandler6.AppendFormatted(int_3);
					defaultInterpolatedStringHandler6.AppendLiteral(" 个");
					text5 = defaultInterpolatedStringHandler6.ToStringAndClear();
				}
				string text6;
				if (int_2 <= 0)
				{
					text6 = "";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler7.AppendLiteral("，失败 ");
					defaultInterpolatedStringHandler7.AppendFormatted(int_2);
					defaultInterpolatedStringHandler7.AppendLiteral(" 个");
					text6 = defaultInterpolatedStringHandler7.ToStringAndClear();
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(33, 1);
				defaultInterpolatedStringHandler8.AppendLiteral("。已返回前 20 条结果，其余 ");
				defaultInterpolatedStringHandler8.AppendFormatted(list_2.Count - 20);
				defaultInterpolatedStringHandler8.AppendLiteral(" 条结果因缓存服务不可用而未返回。");
				string_2 = text4 + text5 + text6 + defaultInterpolatedStringHandler8.ToStringAndClear();
				object_1 = new Class237<int, int, string, int, int, int, int, List<object>, bool, int, int>(list_0.Count, list_1.Count, string_0, int_4, int_1, int_3, int_2, list_3, gparam_19: true, 20, list_2.Count);
				Logger.Warning("[JoinGeometry] 缓存服务不可用，仅返回前 20 条结果");
				result = AIToolResult.Ok(string_2, object_1);
				goto IL_0ce9;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(28, 4);
			defaultInterpolatedStringHandler9.AppendLiteral("批量");
			defaultInterpolatedStringHandler9.AppendFormatted(string_1);
			defaultInterpolatedStringHandler9.AppendLiteral("完成：");
			defaultInterpolatedStringHandler9.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler9.AppendLiteral(" 个第一组元素 × ");
			defaultInterpolatedStringHandler9.AppendFormatted(list_1.Count);
			defaultInterpolatedStringHandler9.AppendLiteral(" 个第二组元素，成功 ");
			defaultInterpolatedStringHandler9.AppendFormatted(int_1);
			defaultInterpolatedStringHandler9.AppendLiteral(" 个");
			string text7 = defaultInterpolatedStringHandler9.ToStringAndClear();
			string text8;
			if (int_3 <= 0)
			{
				text8 = "";
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler10.AppendLiteral("，跳过 ");
				defaultInterpolatedStringHandler10.AppendFormatted(int_3);
				defaultInterpolatedStringHandler10.AppendLiteral(" 个");
				text8 = defaultInterpolatedStringHandler10.ToStringAndClear();
			}
			string text9;
			if (int_2 <= 0)
			{
				text9 = "";
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler11.AppendLiteral("，失败 ");
				defaultInterpolatedStringHandler11.AppendFormatted(int_2);
				defaultInterpolatedStringHandler11.AppendLiteral(" 个");
				text9 = defaultInterpolatedStringHandler11.ToStringAndClear();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler12 = new DefaultInterpolatedStringHandler(72, 2);
			defaultInterpolatedStringHandler12.AppendLiteral("。已返回前 20 条结果，完整结果 (");
			defaultInterpolatedStringHandler12.AppendFormatted(list_2.Count);
			defaultInterpolatedStringHandler12.AppendLiteral(" 条) 已缓存。\n\n💡 使用 get_cache_data 工具并传入 cache_id=");
			defaultInterpolatedStringHandler12.AppendFormatted(string_7);
			defaultInterpolatedStringHandler12.AppendLiteral(" 获取完整结果");
			string_2 = text7 + text8 + text9 + defaultInterpolatedStringHandler12.ToStringAndClear();
			object_1 = new Class238<int, int, string, int, int, int, int, List<object>, bool, int, int, string, Class107<string, string, int, int, int>>(list_0.Count, list_1.Count, string_0, int_4, int_1, int_3, int_2, list_3, gparam_21: true, 20, list_2.Count, string_7, new Class107<string, string, int, int, int>(string_7, "完整结果已缓存，可使用 get_cache_data 工具获取", list_2.Count, 20, list_2.Count - 20));
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler13 = new DefaultInterpolatedStringHandler(29, 2);
			defaultInterpolatedStringHandler13.AppendLiteral("[JoinGeometry] 详细结果已缓存: ");
			defaultInterpolatedStringHandler13.AppendFormatted(string_7);
			defaultInterpolatedStringHandler13.AppendLiteral("，共 ");
			defaultInterpolatedStringHandler13.AppendFormatted(list_2.Count);
			defaultInterpolatedStringHandler13.AppendLiteral(" 条");
			Logger.Info(defaultInterpolatedStringHandler13.ToStringAndClear());
			string_5 = null;
			string_6 = null;
			string_7 = null;
			list_3 = null;
			goto IL_0cbb;
			IL_0cbb:
			Logger.Info("[JoinGeometry] " + string_2);
			result = AIToolResult.Ok(string_2, object_1);
			goto IL_0ce9;
			IL_0ce9:
			int_0 = -2;
			string_1 = null;
			list_2 = null;
			object_1 = null;
			string_2 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class545 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public object object_0;

		public IElementService ielementService_0;

		public IModificationService imodificationService_0;

		public string string_0;

		public int int_1;

		public int int_2;

		public JoinGeometryTool joinGeometryTool_0;

		private object object_1;

		private object object_2;

		private string string_1;

		private string string_2;

		private bool bool_0;

		private string string_3;

		private bool bool_1;

		private string string_4;

		private object object_3;

		private Class233<int, string, int, string, string, string> class233_0;

		private Dictionary<string, object> dictionary_0;

		private PropertyInfo[] propertyInfo_0;

		private int int_3;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo[] propertyInfo_2;

		private int int_4;

		private PropertyInfo propertyInfo_3;

		void IAsyncStateMachine.MoveNext()
		{
			object_1 = ielementService_0.GetElementById(object_0, int_1);
			AIToolResult result;
			if (object_1 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(" 的第一个元素");
				result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				object_2 = ielementService_0.GetElementById(object_0, int_2);
				if (object_2 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(16, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("找不到 ID 为 ");
					defaultInterpolatedStringHandler2.AppendFormatted(int_2);
					defaultInterpolatedStringHandler2.AppendLiteral(" 的第二个元素");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				else
				{
					string_1 = ielementService_0.GetElementName(object_1);
					string_2 = ielementService_0.GetElementName(object_2);
					bool_0 = imodificationService_0.AreElementsJoined(object_1, object_2);
					string_3 = imodificationService_0.GetJoinOrder(object_1, object_2);
					object_3 = null;
					if (string_0.Equals("join", StringComparison.OrdinalIgnoreCase))
					{
						bool_1 = imodificationService_0.JoinGeometry(object_0, object_1, object_2);
						string_4 = "连接";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(12, 2);
						defaultInterpolatedStringHandler3.AppendFormatted(int_1);
						defaultInterpolatedStringHandler3.AppendLiteral(" (主) -> ");
						defaultInterpolatedStringHandler3.AppendFormatted(int_2);
						defaultInterpolatedStringHandler3.AppendLiteral(" (从)");
						object_3 = new Class229<string>(defaultInterpolatedStringHandler3.ToStringAndClear());
						goto IL_0473;
					}
					if (string_0.Equals("unjoin", StringComparison.OrdinalIgnoreCase))
					{
						if (bool_0)
						{
							bool_1 = imodificationService_0.UnjoinGeometry(object_0, object_1, object_2);
							string_4 = "取消连接";
							object_3 = new Class231<string>("已连接");
							goto IL_0473;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(22, 2);
						defaultInterpolatedStringHandler4.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler4.AppendFormatted(string_1 ?? int_1.ToString());
						defaultInterpolatedStringHandler4.AppendLiteral(" 和 ");
						defaultInterpolatedStringHandler4.AppendFormatted(string_2 ?? int_2.ToString());
						defaultInterpolatedStringHandler4.AppendLiteral(" 之间没有连接关系，无需取消连接");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class230<int, string, int, string, string, string, string>(int_1, string_1, int_2, string_2, string_0, "未连接", "无需操作"));
					}
					else if (string_0.Equals("switch", StringComparison.OrdinalIgnoreCase))
					{
						if (bool_0)
						{
							bool_1 = imodificationService_0.SwitchJoinOrder(object_0, object_1, object_2);
							string_4 = "切换连接顺序";
							string gparam_ = string_3;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(12, 2);
							defaultInterpolatedStringHandler5.AppendFormatted(int_2);
							defaultInterpolatedStringHandler5.AppendLiteral(" (主) -> ");
							defaultInterpolatedStringHandler5.AppendFormatted(int_1);
							defaultInterpolatedStringHandler5.AppendLiteral(" (从)");
							object_3 = new Class232<string, string>(gparam_, defaultInterpolatedStringHandler5.ToStringAndClear());
							goto IL_0473;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(22, 2);
						defaultInterpolatedStringHandler6.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler6.AppendFormatted(string_1 ?? int_1.ToString());
						defaultInterpolatedStringHandler6.AppendLiteral(" 和 ");
						defaultInterpolatedStringHandler6.AppendFormatted(string_2 ?? int_2.ToString());
						defaultInterpolatedStringHandler6.AppendLiteral(" 之间没有连接关系，无法切换顺序");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler6.ToStringAndClear());
					}
					else
					{
						result = AIToolResult.Fail("不支持的操作类型: " + string_0 + "，请使用 'join'、'unjoin' 或 'switch'");
					}
				}
			}
			goto IL_07f7;
			IL_0473:
			if (!bool_1)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(20, 3);
				defaultInterpolatedStringHandler7.AppendFormatted(string_4);
				defaultInterpolatedStringHandler7.AppendLiteral("几何图形失败（元素1: ");
				defaultInterpolatedStringHandler7.AppendFormatted(string_1 ?? int_1.ToString());
				defaultInterpolatedStringHandler7.AppendLiteral(", 元素2: ");
				defaultInterpolatedStringHandler7.AppendFormatted(string_2 ?? int_2.ToString());
				defaultInterpolatedStringHandler7.AppendLiteral("）");
				result = AIToolResult.Fail(defaultInterpolatedStringHandler7.ToStringAndClear());
			}
			else
			{
				class233_0 = new Class233<int, string, int, string, string, string>(int_1, string_1, int_2, string_2, string_0, "成功");
				if (object_3 != null)
				{
					dictionary_0 = new Dictionary<string, object>();
					propertyInfo_0 = class233_0.GetType().GetProperties();
					for (int_3 = 0; int_3 < propertyInfo_0.Length; int_3++)
					{
						propertyInfo_1 = propertyInfo_0[int_3];
						dictionary_0[propertyInfo_1.Name] = propertyInfo_1.GetValue(class233_0) ?? string.Empty;
						propertyInfo_1 = null;
					}
					propertyInfo_0 = null;
					propertyInfo_2 = object_3.GetType().GetProperties();
					for (int_4 = 0; int_4 < propertyInfo_2.Length; int_4++)
					{
						propertyInfo_3 = propertyInfo_2[int_4];
						dictionary_0[propertyInfo_3.Name] = propertyInfo_3.GetValue(object_3) ?? string.Empty;
						propertyInfo_3 = null;
					}
					propertyInfo_2 = null;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(20, 3);
					defaultInterpolatedStringHandler8.AppendLiteral("成功");
					defaultInterpolatedStringHandler8.AppendFormatted(string_4);
					defaultInterpolatedStringHandler8.AppendLiteral("几何图形（元素1: ");
					defaultInterpolatedStringHandler8.AppendFormatted(string_1 ?? int_1.ToString());
					defaultInterpolatedStringHandler8.AppendLiteral(", 元素2: ");
					defaultInterpolatedStringHandler8.AppendFormatted(string_2 ?? int_2.ToString());
					defaultInterpolatedStringHandler8.AppendLiteral("）");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler8.ToStringAndClear(), (object)dictionary_0);
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(20, 3);
					defaultInterpolatedStringHandler9.AppendLiteral("成功");
					defaultInterpolatedStringHandler9.AppendFormatted(string_4);
					defaultInterpolatedStringHandler9.AppendLiteral("几何图形（元素1: ");
					defaultInterpolatedStringHandler9.AppendFormatted(string_1 ?? int_1.ToString());
					defaultInterpolatedStringHandler9.AppendLiteral(", 元素2: ");
					defaultInterpolatedStringHandler9.AppendFormatted(string_2 ?? int_2.ToString());
					defaultInterpolatedStringHandler9.AppendLiteral("）");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler9.ToStringAndClear(), (object)class233_0);
				}
			}
			goto IL_07f7;
			IL_07f7:
			int_0 = -2;
			object_1 = null;
			object_2 = null;
			string_1 = null;
			string_2 = null;
			string_3 = null;
			string_4 = null;
			object_3 = null;
			class233_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "join_geometry";

	public string Category => "几何操作";

	public string Description => "执行实心元素之间的几何连接操作，包括连接、取消连接和切换连接顺序。适用于墙、柱、梁、楼板等实心元素之间的几何关系处理（如墙与墙连接、墙与楼板连接、梁与柱连接等）。注意：如需使用空心模型剪切实心模型，请使用 cut_geometry 工具。";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"element1Id\": {\n                \"type\": \"integer\",\n                \"description\": \"第一个元素 ID（单个操作时使用）\"\n            },\n            \"element1Ids\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"第一个元素 ID 列表（批量操作时使用，将与第二个元素列表进行组合连接）\"\n            },\n            \"element1CacheId\": {\n                \"type\": \"string\",\n                \"description\": \"第一个元素的缓存 ID（可选）。从上一个查询工具的返回结果中获取 cache_id 字段\"\n            },\n            \"element2Id\": {\n                \"type\": \"integer\",\n                \"description\": \"第二个元素 ID（单个操作时使用）\"\n            },\n            \"element2Ids\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"integer\" },\n                \"description\": \"第二个元素 ID 列表（批量操作时使用，将与第一个元素列表进行组合连接）\"\n            },\n            \"element2CacheId\": {\n                \"type\": \"string\",\n                \"description\": \"第二个元素的缓存 ID（可选）。从上一个查询工具的返回结果中获取 cache_id 字段\"\n            },\n            \"operation\": {\n                \"type\": \"string\",\n                \"description\": \"操作类型：join=连接几何, unjoin=取消连接, switch=切换连接顺序\",\n                \"enum\": [\"join\", \"unjoin\", \"switch\"],\n                \"default\": \"join\"\n            }\n        },\n        \"required\": []\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class543))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class543 stateMachine = new Class543();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.joinGeometryTool_0 = this;
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
	[AsyncStateMachine(typeof(Class545))]
	private Task<AIToolResult> method_1(object object_0, IElementService ielementService_0, IModificationService imodificationService_0, string string_0, int int_0, int int_1)
	{
		Class545 stateMachine = new Class545();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.joinGeometryTool_0 = this;
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

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class544))]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, object object_0, IElementService ielementService_0, IModificationService imodificationService_0, string string_0, List<int> list_0, List<int> list_1, CancellationToken cancellationToken_0)
	{
		Class544 stateMachine = new Class544();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.joinGeometryTool_0 = this;
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
