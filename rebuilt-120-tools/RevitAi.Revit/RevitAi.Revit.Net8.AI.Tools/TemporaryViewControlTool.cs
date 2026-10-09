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

[AITool("temporary_view_control", Category = "视图管理", Description = "控制 Revit 当前视图的临时隐藏和隔离状态，支持按元素或类别进行隔离、隐藏、查询状态以及一键重置视图。支持多种输入方式：当前选择、指定元素 ID（单个或数组）、类别名称、缓存 ID（从之前查询工具的输出缓存中获取）", RequiresTransaction = true, RequiresModification = true)]
public sealed class TemporaryViewControlTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class646 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public TemporaryViewControlTool temporaryViewControlTool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private object object_0;

		private int? nullable_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private bool bool_0;

		private string string_3;

		private bool bool_1;

		private bool bool_2;

		private string string_4;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private AIToolResult aitoolResult_3;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

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
					Class646 stateMachine = this;
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
				TaskAwaiter<AIToolResult> awaiter5;
				TaskAwaiter<AIToolResult> awaiter4;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter2;
				switch (num)
				{
				default:
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
					if (iviewService_0 == null)
					{
						Logger.Error("[" + temporaryViewControlTool_0.Name + "] 无法获取 ViewService");
						result = AIToolResult.Fail("无法获取 ViewService");
					}
					else if (ielementService_0 == null)
					{
						Logger.Error("[" + temporaryViewControlTool_0.Name + "] 无法获取 ElementService");
						result = AIToolResult.Fail("无法获取 ElementService");
					}
					else if (aitoolContext_0.Document == null)
					{
						Logger.Error("[" + temporaryViewControlTool_0.Name + "] 文档对象为空");
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						object_0 = iviewService_0.GetActiveView(aitoolContext_0.Document);
						if (object_0 == null)
						{
							Logger.Warning("[" + temporaryViewControlTool_0.Name + "] 无法获取当前活动视图");
							result = AIToolResult.Fail("无法获取当前活动视图");
						}
						else
						{
							nullable_0 = iviewService_0.GetViewId(object_0);
							string_0 = iviewService_0.GetViewName(object_0);
							string_1 = aitoolContext_0.GetParameter<string>("action", (string)null);
							if (string.IsNullOrEmpty(string_1))
							{
								Logger.Warning("[" + temporaryViewControlTool_0.Name + "] action 参数为空");
								result = AIToolResult.Fail("操作类型参数不能为空");
							}
							else if (string_1 == "QUERY_STATUS")
							{
								bool_0 = iviewService_0.IsTemporaryHideIsolateActive(object_0);
								string_3 = (bool_0 ? ("当前视图 '" + string_0 + "' 正处于【临时隐藏/隔离】模式中。") : ("当前视图 '" + string_0 + "' 正常，未开启临时隐藏/隔离。"));
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[");
								defaultInterpolatedStringHandler.AppendFormatted(temporaryViewControlTool_0.Name);
								defaultInterpolatedStringHandler.AppendLiteral("] 查询结果: isActive=");
								defaultInterpolatedStringHandler.AppendFormatted(bool_0);
								Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
								result = AIToolResult.Ok(string_3, (object)new Class306<int?, string, bool>(nullable_0, string_0, bool_0));
							}
							else if (string_1 == "RESET")
							{
								bool_1 = iviewService_0.IsTemporaryHideIsolateActive(object_0);
								if (!bool_1)
								{
									Logger.Info("[" + temporaryViewControlTool_0.Name + "] 视图当前没有临时隐藏/隔离，无需重置");
									result = AIToolResult.Ok("视图 '" + string_0 + "' 当前没有临时隐藏/隔离，无需重置。", (object)new Class307<int?, string>(nullable_0, string_0));
								}
								else
								{
									bool_2 = iviewService_0.ResetTemporaryViewMode(aitoolContext_0.Document, object_0);
									if (!bool_2)
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(20, 2);
										defaultInterpolatedStringHandler2.AppendLiteral("[");
										defaultInterpolatedStringHandler2.AppendFormatted(temporaryViewControlTool_0.Name);
										defaultInterpolatedStringHandler2.AppendLiteral("] 重置视图模式失败: viewId=");
										defaultInterpolatedStringHandler2.AppendFormatted(nullable_0);
										Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
										result = AIToolResult.Fail("重置视图模式失败");
									}
									else
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(20, 2);
										defaultInterpolatedStringHandler3.AppendLiteral("[");
										defaultInterpolatedStringHandler3.AppendFormatted(temporaryViewControlTool_0.Name);
										defaultInterpolatedStringHandler3.AppendLiteral("] 成功重置视图模式: viewId=");
										defaultInterpolatedStringHandler3.AppendFormatted(nullable_0);
										Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
										result = AIToolResult.Ok("成功恢复视图 '" + string_0 + "' 的正常显示。", (object)new Class307<int?, string>(nullable_0, string_0));
									}
								}
							}
							else
							{
								string_2 = aitoolContext_0.GetParameter<string>("target_scope", (string)null);
								if (string.IsNullOrEmpty(string_2))
								{
									Logger.Warning("[" + temporaryViewControlTool_0.Name + "] 隔离/隐藏操作需要指定 target_scope 参数");
									result = AIToolResult.Fail("隔离/隐藏操作需要指定 target_scope 参数（CURRENT_SELECTION/SPECIFIC_IDS/CATEGORY_NAMES）");
								}
								else
								{
									string text = string_1;
									string_4 = text;
									string text2 = string_4;
									if (text2 == "ISOLATE_ELEMENTS")
									{
										awaiter5 = temporaryViewControlTool_0.method_0(aitoolContext_0, string_2, object_0, nullable_0, string_0).GetAwaiter();
										if (!awaiter5.IsCompleted)
										{
											num = 1;
											int_0 = 1;
											taskAwaiter_1 = awaiter5;
											Class646 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
											return;
										}
										goto IL_0828;
									}
									if (text2 == "ISOLATE_CATEGORIES")
									{
										awaiter4 = temporaryViewControlTool_0.method_1(aitoolContext_0, object_0, nullable_0, string_0).GetAwaiter();
										if (!awaiter4.IsCompleted)
										{
											num = 2;
											int_0 = 2;
											taskAwaiter_1 = awaiter4;
											Class646 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
											return;
										}
										goto IL_085f;
									}
									if (text2 == "HIDE_ELEMENTS")
									{
										awaiter3 = temporaryViewControlTool_0.method_2(aitoolContext_0, string_2, object_0, nullable_0, string_0).GetAwaiter();
										if (!awaiter3.IsCompleted)
										{
											num = 3;
											int_0 = 3;
											taskAwaiter_1 = awaiter3;
											Class646 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
											return;
										}
										goto IL_0896;
									}
									if (text2 == "HIDE_CATEGORIES")
									{
										awaiter2 = temporaryViewControlTool_0.method_3(aitoolContext_0, object_0, nullable_0, string_0).GetAwaiter();
										if (!awaiter2.IsCompleted)
										{
											num = 4;
											int_0 = 4;
											taskAwaiter_1 = awaiter2;
											Class646 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
											return;
										}
										break;
									}
									Logger.Warning("[" + temporaryViewControlTool_0.Name + "] 未知的操作类型: " + string_1);
									result = AIToolResult.Fail("未知的操作类型: " + string_1);
								}
							}
						}
					}
					goto end_IL_006e;
				}
				case 1:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0828;
				case 2:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_085f;
				case 3:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0896;
				case 4:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_0896:
					aitoolResult_2 = awaiter3.GetResult();
					result = aitoolResult_2;
					goto end_IL_006e;
					IL_085f:
					aitoolResult_1 = awaiter4.GetResult();
					result = aitoolResult_1;
					goto end_IL_006e;
					IL_0828:
					aitoolResult_0 = awaiter5.GetResult();
					result = aitoolResult_0;
					goto end_IL_006e;
				}
				aitoolResult_3 = awaiter2.GetResult();
				result = aitoolResult_3;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[" + temporaryViewControlTool_0.Name + "] 执行异常: " + exception_0.Message);
				result = AIToolResult.Fail("执行失败: " + exception_0.Message);
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
	public sealed class Class647 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public int? nullable_0;

		public string string_0;

		public TemporaryViewControlTool temporaryViewControlTool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private object object_1;

		private IList<string> ilist_0;

		private IEnumerable<int> ienumerable_0;

		private bool bool_0;

		private string string_1;

		void IAsyncStateMachine.MoveNext()
		{
			IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
			iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
			IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
			ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
			AIToolResult result;
			if (iviewService_0 == null || ielementService_0 == null)
			{
				Logger.Error("[" + temporaryViewControlTool_0.Name + "][HIDE_CATEGORIES] 无法获取服务");
				result = AIToolResult.Fail("无法获取必要的服务，请稍后重试。");
			}
			else
			{
				object_1 = aitoolContext_0.Document;
				if (object_1 == null)
				{
					Logger.Error("[" + temporaryViewControlTool_0.Name + "][HIDE_CATEGORIES] Document 为空");
					result = AIToolResult.Fail("无法获取文档，请稍后重试。");
				}
				else
				{
					ilist_0 = aitoolContext_0.GetParameter<IList<string>>("category_names", (IList<string>)null);
					if (ilist_0 == null || !ilist_0.Any())
					{
						Logger.Warning("[" + temporaryViewControlTool_0.Name + "][HIDE_CATEGORIES] 未提供有效的类别名称");
						result = AIToolResult.Fail("未提供有效的类别名称！请提供要隐藏的类别中文名称列表（如 ['风管', '管道']）。");
					}
					else
					{
						ienumerable_0 = temporaryViewControlTool_0.method_7(ielementService_0, object_1, ilist_0);
						if (ienumerable_0 == null || !ienumerable_0.Any())
						{
							Logger.Warning("[" + temporaryViewControlTool_0.Name + "][HIDE_CATEGORIES] 未识别到有效的构件类别");
							result = AIToolResult.Fail("未识别到有效的构件类别！请检查类别名称是否正确。");
						}
						else
						{
							bool_0 = iviewService_0.HideCategories(object_1, object_0, ienumerable_0);
							if (!bool_0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[");
								defaultInterpolatedStringHandler.AppendFormatted(temporaryViewControlTool_0.Name);
								defaultInterpolatedStringHandler.AppendLiteral("][HIDE_CATEGORIES] 隐藏类别失败: count=");
								defaultInterpolatedStringHandler.AppendFormatted(ienumerable_0.Count());
								Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
								result = AIToolResult.Fail("隐藏类别失败");
							}
							else
							{
								string_1 = string.Join("、", ilist_0);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("成功隐藏 ");
								defaultInterpolatedStringHandler2.AppendFormatted(ienumerable_0.Count());
								defaultInterpolatedStringHandler2.AppendLiteral(" 个类别（");
								defaultInterpolatedStringHandler2.AppendFormatted(string_1);
								defaultInterpolatedStringHandler2.AppendLiteral("）的元素。");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class309<int?, string, string, int, IList<string>>(nullable_0, string_0, "HIDE_CATEGORIES", ienumerable_0.Count(), ilist_0));
							}
						}
					}
				}
			}
			int_0 = -2;
			iviewService_0 = null;
			ielementService_0 = null;
			object_1 = null;
			ilist_0 = null;
			ienumerable_0 = null;
			string_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class648 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public string string_0;

		public object object_0;

		public int? nullable_0;

		public string string_1;

		public TemporaryViewControlTool temporaryViewControlTool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private object object_1;

		private IEnumerable<int> ienumerable_0;

		private bool bool_0;

		void IAsyncStateMachine.MoveNext()
		{
			IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
			iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
			IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
			ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
			AIToolResult result;
			if (iviewService_0 == null)
			{
				Logger.Error("[" + temporaryViewControlTool_0.Name + "][HIDE_ELEMENTS] 无法获取 ViewService");
				result = AIToolResult.Fail("无法获取视图服务，请稍后重试。");
			}
			else
			{
				object_1 = aitoolContext_0.Document;
				if (object_1 == null)
				{
					Logger.Error("[" + temporaryViewControlTool_0.Name + "][HIDE_ELEMENTS] Document 为空");
					result = AIToolResult.Fail("无法获取文档，请稍后重试。");
				}
				else
				{
					ienumerable_0 = temporaryViewControlTool_0.method_4(aitoolContext_0, string_0);
					if (ienumerable_0 == null || !ienumerable_0.Any())
					{
						Logger.Warning("[" + temporaryViewControlTool_0.Name + "][HIDE_ELEMENTS] 未找到有效的操作图元");
						result = AIToolResult.Fail("未找到有效的操作图元！请确保已选中元素或提供了正确的元素 ID 列表。");
					}
					else
					{
						bool_0 = iviewService_0.HideElements(object_1, object_0, ienumerable_0);
						if (!bool_0)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
							defaultInterpolatedStringHandler.AppendLiteral("[");
							defaultInterpolatedStringHandler.AppendFormatted(temporaryViewControlTool_0.Name);
							defaultInterpolatedStringHandler.AppendLiteral("][HIDE_ELEMENTS] 隐藏元素失败: count=");
							defaultInterpolatedStringHandler.AppendFormatted(ienumerable_0.Count());
							Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
							result = AIToolResult.Fail("隐藏元素失败");
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(10, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("成功隐藏 ");
							defaultInterpolatedStringHandler2.AppendFormatted(ienumerable_0.Count());
							defaultInterpolatedStringHandler2.AppendLiteral(" 个元素。");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class308<int?, string, string, int>(nullable_0, string_1, "HIDE_ELEMENTS", ienumerable_0.Count()));
						}
					}
				}
			}
			int_0 = -2;
			iviewService_0 = null;
			ielementService_0 = null;
			object_1 = null;
			ienumerable_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class649 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public int? nullable_0;

		public string string_0;

		public TemporaryViewControlTool temporaryViewControlTool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private object object_1;

		private IList<string> ilist_0;

		private IEnumerable<int> ienumerable_0;

		private bool bool_0;

		private string string_1;

		void IAsyncStateMachine.MoveNext()
		{
			IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
			iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
			IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
			ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
			AIToolResult result;
			if (iviewService_0 == null || ielementService_0 == null)
			{
				Logger.Error("[" + temporaryViewControlTool_0.Name + "][ISOLATE_CATEGORIES] 无法获取服务");
				result = AIToolResult.Fail("无法获取必要的服务，请稍后重试。");
			}
			else
			{
				object_1 = aitoolContext_0.Document;
				if (object_1 == null)
				{
					Logger.Error("[" + temporaryViewControlTool_0.Name + "][ISOLATE_CATEGORIES] Document 为空");
					result = AIToolResult.Fail("无法获取文档，请稍后重试。");
				}
				else
				{
					ilist_0 = aitoolContext_0.GetParameter<IList<string>>("category_names", (IList<string>)null);
					if (ilist_0 == null || !ilist_0.Any())
					{
						Logger.Warning("[" + temporaryViewControlTool_0.Name + "][ISOLATE_CATEGORIES] 未提供有效的类别名称");
						result = AIToolResult.Fail("未提供有效的类别名称！请提供要隔离的类别中文名称列表（如 ['风管', '管道']）。");
					}
					else
					{
						ienumerable_0 = temporaryViewControlTool_0.method_7(ielementService_0, object_1, ilist_0);
						if (ienumerable_0 == null || !ienumerable_0.Any())
						{
							Logger.Warning("[" + temporaryViewControlTool_0.Name + "][ISOLATE_CATEGORIES] 未识别到有效的构件类别");
							result = AIToolResult.Fail("未识别到有效的构件类别！请检查类别名称是否正确。");
						}
						else
						{
							bool_0 = iviewService_0.IsolateCategories(object_1, object_0, ienumerable_0);
							if (!bool_0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[");
								defaultInterpolatedStringHandler.AppendFormatted(temporaryViewControlTool_0.Name);
								defaultInterpolatedStringHandler.AppendLiteral("][ISOLATE_CATEGORIES] 隔离类别失败: count=");
								defaultInterpolatedStringHandler.AppendFormatted(ienumerable_0.Count());
								Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
								result = AIToolResult.Fail("隔离类别失败");
							}
							else
							{
								string_1 = string.Join("、", ilist_0);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("成功隔离 ");
								defaultInterpolatedStringHandler2.AppendFormatted(ienumerable_0.Count());
								defaultInterpolatedStringHandler2.AppendLiteral(" 个类别（");
								defaultInterpolatedStringHandler2.AppendFormatted(string_1);
								defaultInterpolatedStringHandler2.AppendLiteral("），现在视图中只显示这些类别的元素。");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class309<int?, string, string, int, IList<string>>(nullable_0, string_0, "ISOLATE_CATEGORIES", ienumerable_0.Count(), ilist_0));
							}
						}
					}
				}
			}
			int_0 = -2;
			iviewService_0 = null;
			ielementService_0 = null;
			object_1 = null;
			ilist_0 = null;
			ienumerable_0 = null;
			string_1 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class650 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public string string_0;

		public object object_0;

		public int? nullable_0;

		public string string_1;

		public TemporaryViewControlTool temporaryViewControlTool_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private object object_1;

		private IEnumerable<int> ienumerable_0;

		private bool bool_0;

		void IAsyncStateMachine.MoveNext()
		{
			IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
			iviewService_0 = ((revitAdapter != null) ? revitAdapter.ViewService : null);
			IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
			ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
			AIToolResult result;
			if (iviewService_0 == null)
			{
				Logger.Error("[" + temporaryViewControlTool_0.Name + "][ISOLATE_ELEMENTS] 无法获取 ViewService");
				result = AIToolResult.Fail("无法获取视图服务，请稍后重试。");
			}
			else
			{
				object_1 = aitoolContext_0.Document;
				if (object_1 == null)
				{
					Logger.Error("[" + temporaryViewControlTool_0.Name + "][ISOLATE_ELEMENTS] Document 为空");
					result = AIToolResult.Fail("无法获取文档，请稍后重试。");
				}
				else
				{
					ienumerable_0 = temporaryViewControlTool_0.method_4(aitoolContext_0, string_0);
					if (ienumerable_0 == null || !ienumerable_0.Any())
					{
						Logger.Warning("[" + temporaryViewControlTool_0.Name + "][ISOLATE_ELEMENTS] 未找到有效的操作图元");
						result = AIToolResult.Fail("未找到有效的操作图元！请确保已选中元素或提供了正确的元素 ID 列表。");
					}
					else
					{
						bool_0 = iviewService_0.IsolateElements(object_1, object_0, ienumerable_0);
						if (!bool_0)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
							defaultInterpolatedStringHandler.AppendLiteral("[");
							defaultInterpolatedStringHandler.AppendFormatted(temporaryViewControlTool_0.Name);
							defaultInterpolatedStringHandler.AppendLiteral("][ISOLATE_ELEMENTS] 隔离元素失败: count=");
							defaultInterpolatedStringHandler.AppendFormatted(ienumerable_0.Count());
							Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
							result = AIToolResult.Fail("隔离元素失败");
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("成功隔离 ");
							defaultInterpolatedStringHandler2.AppendFormatted(ienumerable_0.Count());
							defaultInterpolatedStringHandler2.AppendLiteral(" 个元素，现在视图中只显示这些元素。");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class308<int?, string, string, int>(nullable_0, string_1, "ISOLATE_ELEMENTS", ienumerable_0.Count()));
						}
					}
				}
			}
			int_0 = -2;
			iviewService_0 = null;
			ielementService_0 = null;
			object_1 = null;
			ienumerable_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "temporary_view_control";

	public string Category => "视图管理";

	public string Description => "控制 Revit 当前视图的临时隐藏和隔离状态，支持按元素或类别进行隔离、隐藏、查询状态以及一键重置视图";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"action\": {\n                \"type\": \"string\",\n                \"enum\": [\"ISOLATE_ELEMENTS\", \"ISOLATE_CATEGORIES\", \"HIDE_ELEMENTS\", \"HIDE_CATEGORIES\", \"RESET\", \"QUERY_STATUS\"],\n                \"description\": \"操作指令：ISOLATE_ELEMENTS(隔离元素), ISOLATE_CATEGORIES(隔离类别), HIDE_ELEMENTS(隐藏元素), HIDE_CATEGORIES(隐藏类别), RESET(重置/恢复显示), QUERY_STATUS(查询当前是否有隐藏隔离)\"\n            },\n            \"target_scope\": {\n                \"type\": \"string\",\n                \"enum\": [\"CURRENT_SELECTION\", \"SPECIFIC_IDS\", \"CATEGORY_NAMES\", \"CACHE_ID\"],\n                \"description\": \"目标范围：CURRENT_SELECTION(当前UI选中的元素), SPECIFIC_IDS(指定的ElementId列表), CATEGORY_NAMES(指定类别名称), CACHE_ID(从缓存ID获取元素列表)\"\n            },\n            \"element_ids\": {\n                \"oneOf\": [\n                    { \"type\": \"integer\", \"description\": \"单个元素 ID\" },\n                    { \"type\": \"array\", \"items\": { \"type\": \"integer\" }, \"description\": \"元素 ID 列表\" }\n                ],\n                \"description\": \"目标元素的 ElementId（当 target_scope 为 SPECIFIC_IDS 时填写，支持单个 ID 或 ID 数组）\"\n            },\n            \"category_names\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"目标类别中文名称（如 ['风管', '墙']，当 target_scope 为 CATEGORY_NAMES 时填写）\"\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（当 target_scope 为 CACHE_ID 时填写，从之前查询工具的输出缓存中获取元素列表）\"\n            }\n        },\n        \"required\": [\"action\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class646))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class646 stateMachine = new Class646();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.temporaryViewControlTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class650))]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, string string_0, object object_0, int? nullable_0, string? string_1)
	{
		Class650 stateMachine = new Class650();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.temporaryViewControlTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.string_0 = string_0;
		stateMachine.object_0 = object_0;
		stateMachine.nullable_0 = nullable_0;
		stateMachine.string_1 = string_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class649))]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, object object_0, int? nullable_0, string? string_0)
	{
		Class649 stateMachine = new Class649();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.temporaryViewControlTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.nullable_0 = nullable_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class648))]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, string string_0, object object_0, int? nullable_0, string? string_1)
	{
		Class648 stateMachine = new Class648();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.temporaryViewControlTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.string_0 = string_0;
		stateMachine.object_0 = object_0;
		stateMachine.nullable_0 = nullable_0;
		stateMachine.string_1 = string_1;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class647))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_3(AIToolContext aitoolContext_0, object object_0, int? nullable_0, string? string_0)
	{
		Class647 stateMachine = new Class647();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.temporaryViewControlTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.nullable_0 = nullable_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private IEnumerable<int>? method_4(AIToolContext aitoolContext_0, string string_0)
	{
		IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
		ISelectionService val = ((revitAdapter != null) ? revitAdapter.SelectionService : null);
		IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
		IElementService val2 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
		object document = aitoolContext_0.Document;
		if (document == null)
		{
			Logger.Warning("[" + Name + "] Document 为空");
			return null;
		}
		if (string_0 == "CURRENT_SELECTION")
		{
			if (val == null)
			{
				Logger.Warning("[" + Name + "] 无法获取 SelectionService");
				return null;
			}
			IEnumerable<object> selectedElements = val.GetSelectedElements(document);
			List<int> list = new List<int>();
			foreach (object item2 in selectedElements)
			{
				int? num = ((val2 != null) ? val2.GetElementId(item2) : ((int?)null));
				if (num.HasValue && num.Value > 0)
				{
					list.Add(num.Value);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[");
			defaultInterpolatedStringHandler.AppendFormatted(Name);
			defaultInterpolatedStringHandler.AppendLiteral("] 获取当前选中元素: count=");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		if (string_0 == "SPECIFIC_IDS")
		{
			List<int> list2 = new List<int>();
			List<int> parameter = aitoolContext_0.GetParameter<List<int>>("element_ids", (List<int>)null);
			if (parameter != null && parameter.Any())
			{
				list2.AddRange(parameter);
				return list2;
			}
			IList<int> parameter2 = aitoolContext_0.GetParameter<IList<int>>("element_ids", (IList<int>)null);
			if (parameter2 != null && parameter2.Any())
			{
				list2.AddRange(parameter2);
				return list2;
			}
			int[] parameter3 = aitoolContext_0.GetParameter<int[]>("element_ids", (int[])null);
			if (parameter3 != null && parameter3.Length != 0)
			{
				list2.AddRange(parameter3);
				return list2;
			}
			List<object> parameter4 = aitoolContext_0.GetParameter<List<object>>("element_ids", (List<object>)null);
			if (parameter4 != null && parameter4.Any())
			{
				foreach (object item3 in parameter4)
				{
					if (item3 is int item)
					{
						list2.Add(item);
					}
					else if (item3 is long num2)
					{
						list2.Add((int)num2);
					}
				}
				if (list2.Any())
				{
					return list2;
				}
			}
			int? parameter5 = aitoolContext_0.GetParameter<int?>("element_ids", (int?)null);
			if (parameter5.HasValue && parameter5.Value > 0)
			{
				list2.Add(parameter5.Value);
				return list2;
			}
			Logger.Warning("[" + Name + "] 未提供有效的元素 ID，尝试的类型包括: List<int>, IList<int>, int[], List<object>, int?");
			return null;
		}
		if (string_0 == "CACHE_ID")
		{
			if (aitoolContext_0.DataCache == null)
			{
				Logger.Warning("[" + Name + "] 数据缓存服务不可用");
				return null;
			}
			string parameter6 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
			if (string.IsNullOrEmpty(parameter6))
			{
				Logger.Warning("[" + Name + "] cacheId 参数为空");
				return null;
			}
			object obj = aitoolContext_0.DataCache.Retrieve<object>(parameter6);
			if (obj == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(16, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("[");
				defaultInterpolatedStringHandler2.AppendFormatted(Name);
				defaultInterpolatedStringHandler2.AppendLiteral("] 缓存 '");
				defaultInterpolatedStringHandler2.AppendFormatted(parameter6);
				defaultInterpolatedStringHandler2.AppendLiteral("' 不存在或已过期");
				Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			IEnumerable<int> enumerable = method_5(obj);
			if (enumerable == null || !enumerable.Any())
			{
				Logger.Warning("[" + Name + "] 缓存数据中未找到任何元素 ID");
				return null;
			}
			return enumerable;
		}
		Logger.Warning("[" + Name + "] 不支持的 target_scope: " + string_0);
		return null;
	}

	private IEnumerable<int>? method_5(object object_0)
	{
		List<int> list = new List<int>();
		try
		{
			if (object_0 is IDictionary<string, object> dictionary && dictionary.TryGetValue("element_ids", out var value))
			{
				if (value is IEnumerable<int> collection)
				{
					list.AddRange(collection);
					return list;
				}
				if (value is IEnumerable<object> enumerable)
				{
					foreach (object item2 in enumerable)
					{
						if (item2 is int item)
						{
							list.Add(item);
						}
						else if (item2 is long num)
						{
							list.Add((int)num);
						}
					}
					return list.Any() ? list : null;
				}
			}
			if (object_0 is IList list2)
			{
				foreach (object item3 in list2)
				{
					if (item3 != null)
					{
						int? num2 = method_6(item3);
						if (num2.HasValue && num2.Value > 0)
						{
							list.Add(num2.Value);
						}
					}
				}
				return list.Any() ? list : null;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("[" + Name + "] 提取缓存元素 ID 失败: " + ex.Message);
			return null;
		}
	}

	private int? method_6(object object_0)
	{
		try
		{
			if (object_0 == null)
			{
				return null;
			}
			Type type = object_0.GetType();
			string[] array = new string[10]
			{
				"id",
				"elementId",
				"element_id",
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

	private IEnumerable<int>? method_7(IElementService? ielementService_0, object object_0, IList<string> ilist_0)
	{
		if (object_0 == null)
		{
			Logger.Error("[" + Name + "] Document 为空");
			return null;
		}
		List<int> list = new List<int>();
		try
		{
			Type type = object_0.GetType();
			PropertyInfo property = type.GetProperty("Settings");
			if (property == null)
			{
				Logger.Error("[" + Name + "] 无法获取 Document.Settings 属性");
				return null;
			}
			object value = property.GetValue(object_0);
			if (value == null)
			{
				Logger.Error("[" + Name + "] Settings 为空");
				return null;
			}
			Type type2 = value.GetType();
			PropertyInfo property2 = type2.GetProperty("Categories");
			if (property2 == null)
			{
				Logger.Error("[" + Name + "] 无法获取 Settings.Categories 属性");
				return null;
			}
			if (!(property2.GetValue(value) is IEnumerable enumerable))
			{
				Logger.Error("[" + Name + "] Categories 为空");
				return null;
			}
			foreach (string item in ilist_0)
			{
				bool flag = false;
				foreach (object item2 in enumerable)
				{
					if (item2 == null)
					{
						continue;
					}
					Type type3 = item2.GetType();
					PropertyInfo property3 = type3.GetProperty("Name");
					if (property3 == null || !(property3.GetValue(item2) is string text) || !text.Equals(item, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					PropertyInfo property4 = type3.GetProperty("Id");
					if (!(property4 != null))
					{
						continue;
					}
					object value2 = property4.GetValue(item2);
					if (value2 != null)
					{
						Type type4 = value2.GetType();
						PropertyInfo property5 = type4.GetProperty("Value");
						if (property5 != null && property5.GetValue(value2) is int num)
						{
							list.Add(num);
							flag = true;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 3);
							defaultInterpolatedStringHandler.AppendLiteral("[");
							defaultInterpolatedStringHandler.AppendFormatted(Name);
							defaultInterpolatedStringHandler.AppendLiteral("] 找到类别 '");
							defaultInterpolatedStringHandler.AppendFormatted(item);
							defaultInterpolatedStringHandler.AppendLiteral("'，ID: ");
							defaultInterpolatedStringHandler.AppendFormatted(num);
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							break;
						}
					}
				}
				if (!flag)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(11, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("[");
					defaultInterpolatedStringHandler2.AppendFormatted(Name);
					defaultInterpolatedStringHandler2.AppendLiteral("] 未找到类别 '");
					defaultInterpolatedStringHandler2.AppendFormatted(item);
					defaultInterpolatedStringHandler2.AppendLiteral("'");
					Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[" + Name + "] 获取类别 ID 失败: " + ex.Message);
			return null;
		}
		return list.Any() ? list : null;
	}
}
