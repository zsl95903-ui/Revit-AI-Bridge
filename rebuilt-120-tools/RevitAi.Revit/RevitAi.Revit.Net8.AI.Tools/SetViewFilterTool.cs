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
using Autodesk.Revit.DB;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("set_view_filter", Category = "视图高级操作", Description = "管理视图中的过滤器。支持应用过滤器到视图、从视图中移除过滤器、设置过滤器可见性和覆盖颜色（包括线条颜色和填充图案颜色）。", RequiresTransaction = true, RequiresModification = true)]
public sealed class SetViewFilterTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class637
	{
		public string string_0;

		internal bool method_0(ParameterFilterElement parameterFilterElement_0)
		{
			return ((Element)parameterFilterElement_0).Name.Equals(string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	public sealed class Class638
	{
		public string string_0;

		internal bool method_0(ParameterFilterElement parameterFilterElement_0)
		{
			return ((Element)parameterFilterElement_0).Name.Equals(string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	public sealed class Class639
	{
		public string string_0;

		internal bool method_0(ParameterFilterElement parameterFilterElement_0)
		{
			return ((Element)parameterFilterElement_0).Name.Equals(string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	public sealed class Class640
	{
		public string string_0;

		internal bool method_0(ParameterFilterElement parameterFilterElement_0)
		{
			return ((Element)parameterFilterElement_0).Name.Equals(string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	public sealed class Class641 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public string string_0;

		public SetViewFilterTool setViewFilterTool_0;

		private Class640 class640_0;

		private object object_1;

		private object object_2;

		private int? nullable_0;

		private Document document_0;

		private View view_0;

		private ParameterFilterElement parameterFilterElement_0;

		private OverrideGraphicSettings overrideGraphicSettings_0;

		private bool bool_0;

		private Color color_0;

		private Color color_1;

		private string string_1;

		private FillPatternElement fillPatternElement_0;

		private Exception exception_0;

		private FillPatternElement fillPatternElement_1;

		private Exception exception_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Expected O, but got Unknown
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Expected O, but got Unknown
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				class640_0 = new Class640();
				class640_0.string_0 = string_0;
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class641 stateMachine = this;
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
				object_1 = aitoolContext_0.GetParameter<object>("color", (object)null);
				object_2 = aitoolContext_0.GetParameter<object>("patternColor", (object)null);
				nullable_0 = aitoolContext_0.GetParameter<int?>("patternId", (int?)null);
				if (object_2 == null && object_1 != null)
				{
					object_2 = object_1;
				}
				object document = aitoolContext_0.Document;
				document_0 = (Document)((document is Document) ? document : null);
				if (document_0 == null)
				{
					result = AIToolResult.Fail("文档对象类型无效");
				}
				else
				{
					object obj = object_0;
					view_0 = (View)((obj is View) ? obj : null);
					if (view_0 == null)
					{
						result = AIToolResult.Fail("视图对象类型无效");
					}
					else
					{
						parameterFilterElement_0 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(ParameterFilterElement))).Cast<ParameterFilterElement>().FirstOrDefault((ParameterFilterElement parameterFilterElement_0) => ((Element)parameterFilterElement_0).Name.Equals(class640_0.string_0, StringComparison.OrdinalIgnoreCase));
						if (parameterFilterElement_0 == null)
						{
							result = AIToolResult.Fail("找不到过滤器 '" + class640_0.string_0 + "'。请先使用 create_view_filter 工具创建过滤器。");
						}
						else
						{
							view_0.AddFilter(((Element)parameterFilterElement_0).Id);
							view_0.SetFilterVisibility(((Element)parameterFilterElement_0).Id, true);
							overrideGraphicSettings_0 = new OverrideGraphicSettings();
							bool_0 = false;
							if (object_1 != null && setViewFilterTool_0.method_4(object_1, out color_0))
							{
								overrideGraphicSettings_0.SetProjectionLineColor(color_0);
								bool_0 = true;
							}
							if (object_2 != null && setViewFilterTool_0.method_4(object_2, out color_1))
							{
								if (nullable_0.HasValue && nullable_0.Value > 0)
								{
									try
									{
										Element element = document_0.GetElement(new ElementId((long)nullable_0.Value));
										fillPatternElement_0 = (FillPatternElement)(object)((element is FillPatternElement) ? element : null);
										if (fillPatternElement_0 != null)
										{
											overrideGraphicSettings_0.SetCutForegroundPatternColor(color_1);
											overrideGraphicSettings_0.SetSurfaceForegroundPatternColor(color_1);
											overrideGraphicSettings_0.SetCutForegroundPatternId(((Element)fillPatternElement_0).Id);
											overrideGraphicSettings_0.SetSurfaceForegroundPatternId(((Element)fillPatternElement_0).Id);
											bool_0 = true;
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
											defaultInterpolatedStringHandler.AppendLiteral("[SetViewFilterTool] 使用指定填充图案 ID: ");
											defaultInterpolatedStringHandler.AppendFormatted(nullable_0.Value);
											Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
										}
										fillPatternElement_0 = null;
									}
									catch (Exception ex)
									{
										exception_0 = ex;
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 2);
										defaultInterpolatedStringHandler2.AppendLiteral("[SetViewFilterTool] 无法获取填充图案 ID ");
										defaultInterpolatedStringHandler2.AppendFormatted(nullable_0.Value);
										defaultInterpolatedStringHandler2.AppendLiteral(": ");
										defaultInterpolatedStringHandler2.AppendFormatted(exception_0.Message);
										Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
									}
								}
								else
								{
									fillPatternElement_1 = smethod_1(document_0);
									overrideGraphicSettings_0.SetCutForegroundPatternColor(color_1);
									overrideGraphicSettings_0.SetSurfaceForegroundPatternColor(color_1);
									if (fillPatternElement_1 != null)
									{
										overrideGraphicSettings_0.SetCutForegroundPatternId(((Element)fillPatternElement_1).Id);
										overrideGraphicSettings_0.SetSurfaceForegroundPatternId(((Element)fillPatternElement_1).Id);
									}
									bool_0 = true;
									if (fillPatternElement_1 == null)
									{
										Logger.Warning("[SetViewFilterTool] 无法获取实体填充图案，仅设置了颜色");
									}
									else
									{
										Logger.Info("[SetViewFilterTool] 使用实体填充图案: " + ((Element)fillPatternElement_1).Name);
									}
									fillPatternElement_1 = null;
								}
							}
							if (bool_0)
							{
								view_0.SetFilterOverrides(((Element)parameterFilterElement_0).Id, overrideGraphicSettings_0);
							}
							string_1 = "成功应用过滤器 '" + class640_0.string_0 + "' 到视图";
							if (object_1 != null || object_2 != null)
							{
								string_1 += " 并设置颜色";
							}
							Logger.Info("[SetViewFilterTool] " + string_1);
							result = AIToolResult.Ok(string_1, (object)null);
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_1 = ex;
				Logger.Error("[SetViewFilterTool] 应用过滤器失败: " + exception_1.Message);
				result = AIToolResult.Fail("应用过滤器失败: " + exception_1.Message);
			}
			int_0 = -2;
			class640_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class642 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SetViewFilterTool setViewFilterTool_0;

		private string string_0;

		private string string_1;

		private int? nullable_0;

		private IViewService iviewService_0;

		private IElementService ielementService_0;

		private object object_0;

		private object object_1;

		private string string_2;

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
					Class642 stateMachine = this;
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
				string text;
				string text2;
				switch (num)
				{
				default:
					string_0 = aitoolContext_0.GetParameter<string>("filterName", (string)null);
					string_1 = aitoolContext_0.GetParameter<string>("action", "apply");
					nullable_0 = aitoolContext_0.GetParameter<int?>("viewId", (int?)null);
					if (string.IsNullOrEmpty(string_0))
					{
						result = AIToolResult.Fail("过滤器名称不能为空");
					}
					else
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
						else if (nullable_0.HasValue)
						{
							object_1 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_0.Value);
							if (object_1 != null)
							{
								object_0 = object_1;
								object_1 = null;
								goto IL_0295;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
							defaultInterpolatedStringHandler.AppendFormatted(nullable_0.Value);
							defaultInterpolatedStringHandler.AppendLiteral(" 的视图");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							object_0 = iviewService_0.GetActiveView(aitoolContext_0.Document);
							if (object_0 != null)
							{
								goto IL_0295;
							}
							result = AIToolResult.Fail("无法获取当前活动视图");
						}
					}
					goto end_IL_006e;
				case 1:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_049e;
				case 2:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_04d5;
				case 3:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_050c;
				case 4:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_050c:
					aitoolResult_2 = awaiter3.GetResult();
					result = aitoolResult_2;
					goto end_IL_006e;
					IL_049e:
					aitoolResult_0 = awaiter5.GetResult();
					result = aitoolResult_0;
					goto end_IL_006e;
					IL_0295:
					text = string_1.ToLower();
					string_2 = text;
					text2 = string_2;
					if (text2 == "apply")
					{
						awaiter5 = setViewFilterTool_0.method_0(aitoolContext_0, object_0, string_0).GetAwaiter();
						if (!awaiter5.IsCompleted)
						{
							num = 1;
							int_0 = 1;
							taskAwaiter_1 = awaiter5;
							Class642 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
							return;
						}
						goto IL_049e;
					}
					if (text2 == "remove")
					{
						awaiter4 = setViewFilterTool_0.method_1(aitoolContext_0, object_0, string_0).GetAwaiter();
						if (!awaiter4.IsCompleted)
						{
							num = 2;
							int_0 = 2;
							taskAwaiter_1 = awaiter4;
							Class642 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
							return;
						}
						goto IL_04d5;
					}
					if (text2 == "set_visibility")
					{
						awaiter3 = setViewFilterTool_0.method_2(aitoolContext_0, object_0, string_0).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = 3;
							int_0 = 3;
							taskAwaiter_1 = awaiter3;
							Class642 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
							return;
						}
						goto IL_050c;
					}
					if (text2 == "set_color")
					{
						awaiter2 = setViewFilterTool_0.method_3(aitoolContext_0, object_0, string_0).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = 4;
							int_0 = 4;
							taskAwaiter_1 = awaiter2;
							Class642 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
							return;
						}
						break;
					}
					result = AIToolResult.Fail("不支持的操作类型: " + string_1);
					goto end_IL_006e;
					IL_04d5:
					aitoolResult_1 = awaiter4.GetResult();
					result = aitoolResult_1;
					goto end_IL_006e;
				}
				aitoolResult_3 = awaiter2.GetResult();
				result = aitoolResult_3;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[SetViewFilterTool] 操作失败: " + exception_0.Message);
				result = AIToolResult.Fail("操作失败: " + exception_0.Message);
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
	public sealed class Class643 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public string string_0;

		public SetViewFilterTool setViewFilterTool_0;

		private Class637 class637_0;

		private Document document_0;

		private View view_0;

		private ParameterFilterElement parameterFilterElement_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				class637_0 = new Class637();
				class637_0.string_0 = string_0;
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class643 stateMachine = this;
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
				object document = aitoolContext_0.Document;
				document_0 = (Document)((document is Document) ? document : null);
				if (document_0 == null)
				{
					result = AIToolResult.Fail("文档对象类型无效");
				}
				else
				{
					object obj = object_0;
					view_0 = (View)((obj is View) ? obj : null);
					if (view_0 == null)
					{
						result = AIToolResult.Fail("视图对象类型无效");
					}
					else
					{
						parameterFilterElement_0 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(ParameterFilterElement))).Cast<ParameterFilterElement>().FirstOrDefault((ParameterFilterElement parameterFilterElement_0) => ((Element)parameterFilterElement_0).Name.Equals(class637_0.string_0, StringComparison.OrdinalIgnoreCase));
						if (parameterFilterElement_0 == null)
						{
							result = AIToolResult.Fail("找不到过滤器 '" + class637_0.string_0 + "'");
						}
						else
						{
							view_0.RemoveFilter(((Element)parameterFilterElement_0).Id);
							Logger.Info("[SetViewFilterTool] 成功从视图中移除过滤器 '" + class637_0.string_0 + "'");
							result = AIToolResult.Ok("成功从视图中移除过滤器 '" + class637_0.string_0 + "'", (object)null);
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[SetViewFilterTool] 移除过滤器失败: " + exception_0.Message);
				result = AIToolResult.Fail("移除过滤器失败: " + exception_0.Message);
			}
			int_0 = -2;
			class637_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class644 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public string string_0;

		public SetViewFilterTool setViewFilterTool_0;

		private Class639 class639_0;

		private object object_1;

		private object object_2;

		private int? nullable_0;

		private Document document_0;

		private View view_0;

		private ParameterFilterElement parameterFilterElement_0;

		private OverrideGraphicSettings overrideGraphicSettings_0;

		private bool bool_0;

		private string string_1;

		private Color color_0;

		private Color color_1;

		private FillPatternElement fillPatternElement_0;

		private Exception exception_0;

		private FillPatternElement fillPatternElement_1;

		private Exception exception_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Expected O, but got Unknown
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Expected O, but got Unknown
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				class639_0 = new Class639();
				class639_0.string_0 = string_0;
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class644 stateMachine = this;
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
				object_1 = aitoolContext_0.GetParameter<object>("color", (object)null);
				object_2 = aitoolContext_0.GetParameter<object>("patternColor", (object)null);
				nullable_0 = aitoolContext_0.GetParameter<int?>("patternId", (int?)null);
				if (object_2 == null && object_1 != null)
				{
					object_2 = object_1;
				}
				if (object_1 == null && object_2 == null)
				{
					result = AIToolResult.Fail("至少需要提供 color 或 patternColor 参数");
				}
				else
				{
					object document = aitoolContext_0.Document;
					document_0 = (Document)((document is Document) ? document : null);
					if (document_0 == null)
					{
						result = AIToolResult.Fail("文档对象类型无效");
					}
					else
					{
						object obj = object_0;
						view_0 = (View)((obj is View) ? obj : null);
						if (view_0 == null)
						{
							result = AIToolResult.Fail("视图对象类型无效");
						}
						else
						{
							parameterFilterElement_0 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(ParameterFilterElement))).Cast<ParameterFilterElement>().FirstOrDefault((ParameterFilterElement parameterFilterElement_0) => ((Element)parameterFilterElement_0).Name.Equals(class639_0.string_0, StringComparison.OrdinalIgnoreCase));
							if (parameterFilterElement_0 == null)
							{
								result = AIToolResult.Fail("找不到过滤器 '" + class639_0.string_0 + "'");
							}
							else
							{
								overrideGraphicSettings_0 = new OverrideGraphicSettings();
								bool_0 = false;
								if (object_1 == null)
								{
									goto IL_0285;
								}
								if (setViewFilterTool_0.method_4(object_1, out color_0))
								{
									overrideGraphicSettings_0.SetProjectionLineColor(color_0);
									bool_0 = true;
									color_0 = null;
									goto IL_0285;
								}
								result = AIToolResult.Fail("无法解析 color 参数");
							}
						}
					}
				}
				goto end_IL_0083;
				IL_0285:
				if (object_2 == null)
				{
					goto IL_04e5;
				}
				if (setViewFilterTool_0.method_4(object_2, out color_1))
				{
					if (nullable_0.HasValue && nullable_0.Value > 0)
					{
						try
						{
							Element element = document_0.GetElement(new ElementId((long)nullable_0.Value));
							fillPatternElement_0 = (FillPatternElement)(object)((element is FillPatternElement) ? element : null);
							if (fillPatternElement_0 != null)
							{
								overrideGraphicSettings_0.SetCutForegroundPatternColor(color_1);
								overrideGraphicSettings_0.SetSurfaceForegroundPatternColor(color_1);
								overrideGraphicSettings_0.SetCutForegroundPatternId(((Element)fillPatternElement_0).Id);
								overrideGraphicSettings_0.SetSurfaceForegroundPatternId(((Element)fillPatternElement_0).Id);
								bool_0 = true;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
								defaultInterpolatedStringHandler.AppendLiteral("[SetViewFilterTool] 使用指定填充图案 ID: ");
								defaultInterpolatedStringHandler.AppendFormatted(nullable_0.Value);
								Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							}
							fillPatternElement_0 = null;
						}
						catch (Exception ex)
						{
							exception_0 = ex;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 2);
							defaultInterpolatedStringHandler2.AppendLiteral("[SetViewFilterTool] 无法获取填充图案 ID ");
							defaultInterpolatedStringHandler2.AppendFormatted(nullable_0.Value);
							defaultInterpolatedStringHandler2.AppendLiteral(": ");
							defaultInterpolatedStringHandler2.AppendFormatted(exception_0.Message);
							Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
						}
					}
					else
					{
						fillPatternElement_1 = smethod_1(document_0);
						overrideGraphicSettings_0.SetCutForegroundPatternColor(color_1);
						overrideGraphicSettings_0.SetSurfaceForegroundPatternColor(color_1);
						if (fillPatternElement_1 != null)
						{
							overrideGraphicSettings_0.SetCutForegroundPatternId(((Element)fillPatternElement_1).Id);
							overrideGraphicSettings_0.SetSurfaceForegroundPatternId(((Element)fillPatternElement_1).Id);
						}
						bool_0 = true;
						if (fillPatternElement_1 == null)
						{
							Logger.Warning("[SetViewFilterTool] 无法获取实体填充图案，仅设置了颜色");
						}
						else
						{
							Logger.Info("[SetViewFilterTool] 使用实体填充图案: " + ((Element)fillPatternElement_1).Name);
						}
						fillPatternElement_1 = null;
					}
					color_1 = null;
					goto IL_04e5;
				}
				result = AIToolResult.Fail("无法解析 patternColor 参数");
				goto end_IL_0083;
				IL_04e5:
				if (!bool_0)
				{
					result = AIToolResult.Fail("没有设置任何覆盖选项");
				}
				else
				{
					view_0.SetFilterOverrides(((Element)parameterFilterElement_0).Id, overrideGraphicSettings_0);
					string_1 = "成功设置过滤器 '" + class639_0.string_0 + "' 颜色";
					Logger.Info("[SetViewFilterTool] " + string_1);
					result = AIToolResult.Ok(string_1, (object)null);
				}
				end_IL_0083:;
			}
			catch (Exception ex)
			{
				exception_1 = ex;
				Logger.Error("[SetViewFilterTool] 设置过滤器颜色失败: " + exception_1.Message);
				result = AIToolResult.Fail("设置过滤器颜色失败: " + exception_1.Message);
			}
			int_0 = -2;
			class639_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class645 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public object object_0;

		public string string_0;

		public SetViewFilterTool setViewFilterTool_0;

		private Class638 class638_0;

		private bool bool_0;

		private Document document_0;

		private View view_0;

		private ParameterFilterElement parameterFilterElement_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				class638_0 = new Class638();
				class638_0.string_0 = string_0;
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class645 stateMachine = this;
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
				bool_0 = aitoolContext_0.GetParameter<bool>("visible", true);
				object document = aitoolContext_0.Document;
				document_0 = (Document)((document is Document) ? document : null);
				if (document_0 == null)
				{
					result = AIToolResult.Fail("文档对象类型无效");
				}
				else
				{
					object obj = object_0;
					view_0 = (View)((obj is View) ? obj : null);
					if (view_0 == null)
					{
						result = AIToolResult.Fail("视图对象类型无效");
					}
					else
					{
						parameterFilterElement_0 = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(ParameterFilterElement))).Cast<ParameterFilterElement>().FirstOrDefault((ParameterFilterElement parameterFilterElement_0) => ((Element)parameterFilterElement_0).Name.Equals(class638_0.string_0, StringComparison.OrdinalIgnoreCase));
						if (parameterFilterElement_0 == null)
						{
							result = AIToolResult.Fail("找不到过滤器 '" + class638_0.string_0 + "'");
						}
						else
						{
							view_0.SetFilterVisibility(((Element)parameterFilterElement_0).Id, bool_0);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
							defaultInterpolatedStringHandler.AppendLiteral("[SetViewFilterTool] 成功设置过滤器 '");
							defaultInterpolatedStringHandler.AppendFormatted(class638_0.string_0);
							defaultInterpolatedStringHandler.AppendLiteral("' 可见性为 ");
							defaultInterpolatedStringHandler.AppendFormatted(bool_0);
							Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
							result = AIToolResult.Ok("成功设置过滤器 '" + class638_0.string_0 + "' 可见性为 " + (bool_0 ? "显示" : "隐藏"), (object)null);
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[SetViewFilterTool] 设置过滤器可见性失败: " + exception_0.Message);
				result = AIToolResult.Fail("设置过滤器可见性失败: " + exception_0.Message);
			}
			int_0 = -2;
			class638_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "set_view_filter";

	public string Category => "视图高级操作";

	public string Description => "管理视图中的过滤器（支持线条颜色和填充图案颜色）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"viewId\": {\n                \"type\": \"integer\",\n                \"description\": \"视图 ID（可选，默认使用当前活动视图）\"\n            },\n            \"filterName\": {\n                \"type\": \"string\",\n                \"description\": \"过滤器名称\"\n            },\n            \"action\": {\n                \"type\": \"string\",\n                \"description\": \"操作类型。apply: 应用过滤器到视图并设置颜色；remove: 从视图中移除过滤器；set_visibility: 设置过滤器可见性；set_color: 更新过滤器颜色\",\n                \"enum\": [\"apply\", \"remove\", \"set_visibility\", \"set_color\"],\n                \"default\": \"apply\"\n            },\n            \"visible\": {\n                \"type\": \"boolean\",\n                \"description\": \"过滤器可见性（true=显示过滤结果，false=隐藏过滤结果）。仅当 action 为 set_visibility 时有效。\"\n            },\n            \"color\": {\n                \"type\": \"object\",\n                \"properties\": {\n                    \"red\": {\n                        \"type\": \"integer\",\n                        \"minimum\": 0,\n                        \"maximum\": 255,\n                        \"description\": \"红色分量 (0-255)\"\n                    },\n                    \"green\": {\n                        \"type\": \"integer\",\n                        \"minimum\": 0,\n                        \"maximum\": 255,\n                        \"description\": \"绿色分量 (0-255)\"\n                    },\n                    \"blue\": {\n                        \"type\": \"integer\",\n                        \"minimum\": 0,\n                        \"maximum\": 255,\n                        \"description\": \"蓝色分量 (0-255)\"\n                    }\n                },\n                \"required\": [\"red\", \"green\", \"blue\"],\n                \"description\": \"过滤器的颜色（RGB）。同时设置元素线条颜色和填充图案颜色。仅当 action 为 apply 或 set_color 时有效。\"\n            },\n            \"patternColor\": {\n                \"type\": \"object\",\n                \"properties\": {\n                    \"red\": {\n                        \"type\": \"integer\",\n                        \"minimum\": 0,\n                        \"maximum\": 255,\n                        \"description\": \"红色分量 (0-255)\"\n                    },\n                    \"green\": {\n                        \"type\": \"integer\",\n                        \"minimum\": 0,\n                        \"maximum\": 255,\n                        \"description\": \"绿色分量 (0-255)\"\n                    },\n                    \"blue\": {\n                        \"type\": \"integer\",\n                        \"minimum\": 0,\n                        \"maximum\": 255,\n                        \"description\": \"蓝色分量 (0-255)\"\n                    }\n                },\n                \"required\": [\"red\", \"green\", \"blue\"],\n                \"description\": \"过滤器的填充图案颜色（RGB）。设置元素截面和表面填充的颜色。仅当 action 为 apply 或 set_color 时有效。如果只提供 color 参数，系统会自动将其用作填充图案颜色，无需重复传递。\"\n            },\n            \"patternId\": {\n                \"type\": \"integer\",\n                \"description\": \"填充图案元素 ID（可选）。指定使用特定的填充图案。如果不指定，将使用类别的默认填充图案。⚠️ 注意：需要先获取填充图案的元素 ID。\"\n            }\n        },\n        \"required\": [\"filterName\"]\n    }";

	[AsyncStateMachine(typeof(Class642))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class642 stateMachine = new Class642();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setViewFilterTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class641))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_0(AIToolContext aitoolContext_0, object object_0, string string_0)
	{
		Class641 stateMachine = new Class641();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setViewFilterTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class643))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(AIToolContext aitoolContext_0, object object_0, string string_0)
	{
		Class643 stateMachine = new Class643();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setViewFilterTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class645))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, object object_0, string string_0)
	{
		Class645 stateMachine = new Class645();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setViewFilterTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class644))]
	private Task<AIToolResult> method_3(AIToolContext aitoolContext_0, object object_0, string string_0)
	{
		Class644 stateMachine = new Class644();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setViewFilterTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.object_0 = object_0;
		stateMachine.string_0 = string_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private bool method_4(object object_0, out Color color_0)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Expected O, but got Unknown
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		color_0 = new Color((byte)0, (byte)0, (byte)0);
		try
		{
			if (object_0 == null)
			{
				Logger.Warning("[SetViewFilterTool] TryParseColor: colorObj is null");
				return false;
			}
			Type type = object_0.GetType();
			string? fullName = type.FullName;
			if (fullName != null && fullName.StartsWith("Newtonsoft.Json.Linq."))
			{
				PropertyInfo property = type.GetProperty("Item", new Type[1] { typeof(string) });
				if (property != null)
				{
					byte? b = method_5(property, object_0, "R") ?? method_5(property, object_0, "r") ?? method_5(property, object_0, "red");
					byte? b2 = method_5(property, object_0, "G") ?? method_5(property, object_0, "g") ?? method_5(property, object_0, "green");
					byte? b3 = method_5(property, object_0, "B") ?? method_5(property, object_0, "b") ?? method_5(property, object_0, "blue");
					if (b.HasValue && b2.HasValue && b3.HasValue)
					{
						color_0 = new Color(b.Value, b2.Value, b3.Value);
						return true;
					}
					Logger.Warning("[SetViewFilterTool] TryParseColor: JObject 缺少 RGB 值");
					return false;
				}
				Logger.Warning("[SetViewFilterTool] TryParseColor: JObject 无法获取索引器");
				return false;
			}
			if (object_0 is Dictionary<string, object> dictionary)
			{
				if (smethod_0(dictionary, out var byte_, "red", "r", "R") && smethod_0(dictionary, out var byte_2, "green", "g", "G") && smethod_0(dictionary, out var byte_3, "blue", "b", "B"))
				{
					color_0 = new Color(byte_, byte_2, byte_3);
					return true;
				}
				Logger.Warning("[SetViewFilterTool] TryParseColor: Dictionary 缺少 RGB 值, keys=[" + string.Join(",", dictionary.Keys) + "]");
				return false;
			}
			PropertyInfo propertyInfo = type.GetProperty("R") ?? type.GetProperty("r") ?? type.GetProperty("red");
			PropertyInfo propertyInfo2 = type.GetProperty("G") ?? type.GetProperty("g") ?? type.GetProperty("green");
			PropertyInfo propertyInfo3 = type.GetProperty("B") ?? type.GetProperty("b") ?? type.GetProperty("blue");
			if (propertyInfo != null && propertyInfo2 != null && propertyInfo3 != null)
			{
				byte b4 = Convert.ToByte(propertyInfo.GetValue(object_0) ?? ((object)0));
				byte b5 = Convert.ToByte(propertyInfo2.GetValue(object_0) ?? ((object)0));
				byte b6 = Convert.ToByte(propertyInfo3.GetValue(object_0) ?? ((object)0));
				color_0 = new Color(b4, b5, b6);
				return true;
			}
			Logger.Warning("[SetViewFilterTool] TryParseColor: 无法找到 RGB 属性, type=" + type.FullName);
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("[SetViewFilterTool] TryParseColor 异常: " + ex.Message);
			return false;
		}
	}

	private static bool smethod_0(Dictionary<string, object> dictionary_0, out byte byte_0, params string[] string_0)
	{
		foreach (string key in string_0)
		{
			if (dictionary_0.TryGetValue(key, out object value) && value != null)
			{
				try
				{
					byte_0 = Convert.ToByte(value);
					return true;
				}
				catch
				{
				}
			}
		}
		byte_0 = 0;
		return false;
	}

	private byte? method_5(PropertyInfo propertyInfo_0, object object_0, string string_0)
	{
		try
		{
			object value = propertyInfo_0.GetValue(object_0, new object[1] { string_0 });
			if (value == null)
			{
				return null;
			}
			Type type = value.GetType();
			if (type.FullName == "Newtonsoft.Json.Linq.JValue")
			{
				PropertyInfo property = type.GetProperty("Value", typeof(object));
				if (property != null)
				{
					object value2 = property.GetValue(value);
					if (value2 != null && value2 is int num)
					{
						return (byte)num;
					}
				}
			}
			string text = value.ToString();
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (text.StartsWith("\"") && text.EndsWith("\""))
			{
				text = text.Substring(1, text.Length - 2);
			}
			if (int.TryParse(text, out var result))
			{
				return (byte)result;
			}
		}
		catch
		{
		}
		return null;
	}

	private static FillPatternElement? smethod_1(Document document_0)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			FillPatternElement val = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(FillPatternElement))).Cast<FillPatternElement>().FirstOrDefault(delegate(FillPatternElement fillPatternElement_0)
			{
				try
				{
					FillPattern fillPattern = fillPatternElement_0.GetFillPattern();
					if (fillPattern != null)
					{
						int count = fillPattern.GetFillGrids().Count;
						return count == 0;
					}
				}
				catch (Exception ex2)
				{
					Logger.Warning("[SetViewFilterTool] 检查填充图案失败: " + ex2.Message);
				}
				return false;
			});
			if (val != null)
			{
				Logger.Info("[SetViewFilterTool] 找到实体填充图案: " + ((Element)val).Name);
			}
			else
			{
				Logger.Warning("[SetViewFilterTool] 未能找到实体填充图案");
			}
			return val ?? null;
		}
		catch (Exception ex)
		{
			Logger.Error("[SetViewFilterTool] 获取实体填充图案失败: " + ex.Message);
			return null;
		}
	}
}
