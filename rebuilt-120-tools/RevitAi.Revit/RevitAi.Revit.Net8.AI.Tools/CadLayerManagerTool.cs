using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

[AITool("cad_layer_manager", Category = "CAD 图纸识别", Description = "管理 CAD 图层的查询、分析和内容提取。支持获取图层信息、按图层获取线条/文字/图块、分析图层内容统计。", RequiresTransaction = false, RequiresModification = false)]
public sealed class CadLayerManagerTool : IAITool
{
	private class LayerStatistics
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string string_0 = string.Empty;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int int_0;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private int int_1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private int int_2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private int int_3;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private int? nullable_0;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool? nullable_1;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool? nullable_2;

		public string LayerName
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		public int BlockCount
		{
			[CompilerGenerated]
			get
			{
				return int_0;
			}
			[CompilerGenerated]
			set
			{
				int_0 = value;
			}
		}

		public int LineCount
		{
			[CompilerGenerated]
			get
			{
				return int_1;
			}
			[CompilerGenerated]
			set
			{
				int_1 = value;
			}
		}

		public int TextCount
		{
			[CompilerGenerated]
			get
			{
				return int_2;
			}
			[CompilerGenerated]
			set
			{
				int_2 = value;
			}
		}

		public int TotalCount
		{
			[CompilerGenerated]
			get
			{
				return int_3;
			}
			[CompilerGenerated]
			set
			{
				int_3 = value;
			}
		}

		public int? ColorIndex
		{
			[CompilerGenerated]
			get
			{
				return nullable_0;
			}
			[CompilerGenerated]
			set
			{
				nullable_0 = value;
			}
		}

		public bool? IsVisible
		{
			[CompilerGenerated]
			get
			{
				return nullable_1;
			}
			[CompilerGenerated]
			set
			{
				nullable_1 = value;
			}
		}

		public bool? IsLocked
		{
			[CompilerGenerated]
			get
			{
				return nullable_2;
			}
			[CompilerGenerated]
			set
			{
				nullable_2 = value;
			}
		}
	}

	[CompilerGenerated]
	private sealed class Class354
	{
		public string string_0;
	}

	[CompilerGenerated]
	private sealed class Class355
	{
		public List<object> list_0;

		public Class354 class354_0;

		internal string method_0(string string_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 3);
			defaultInterpolatedStringHandler.AppendLiteral("从图层 '");
			defaultInterpolatedStringHandler.AppendFormatted(class354_0.string_0);
			defaultInterpolatedStringHandler.AppendLiteral("' 获取到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 条线条\n\n💡 在后续工具调用中使用 cacheId=\"");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些线条");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[CompilerGenerated]
	private sealed class Class356
	{
		public List<object> list_0;

		internal string method_0(string string_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 2);
			defaultInterpolatedStringHandler.AppendLiteral("从几何对象获取到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 条线条\n\n💡 在后续工具调用中使用 cacheId=\"");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些线条");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[CompilerGenerated]
	private sealed class Class357
	{
		public string string_0;
	}

	[CompilerGenerated]
	private sealed class Class358
	{
		public List<object> list_0;

		public Class357 class357_0;

		internal string method_0(string string_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 3);
			defaultInterpolatedStringHandler.AppendLiteral("从图层 '");
			defaultInterpolatedStringHandler.AppendFormatted(class357_0.string_0);
			defaultInterpolatedStringHandler.AppendLiteral("' 获取到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个文字\n\n💡 在后续工具调用中使用 cacheId=\"");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些文字");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[CompilerGenerated]
	private sealed class Class359
	{
		public string string_0;
	}

	[CompilerGenerated]
	private sealed class Class360
	{
		public List<object> list_0;

		public Class359 class359_0;

		internal string method_0(string string_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 3);
			defaultInterpolatedStringHandler.AppendLiteral("从图层 '");
			defaultInterpolatedStringHandler.AppendFormatted(class359_0.string_0);
			defaultInterpolatedStringHandler.AppendLiteral("' 获取到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个图块\n\n💡 在后续工具调用中使用 cacheId=\"");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些图块");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[CompilerGenerated]
	private sealed class Class361
	{
		public List<object> list_0;

		internal string method_0(string string_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 2);
			defaultInterpolatedStringHandler.AppendLiteral("从几何对象获取到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个图块\n\n💡 在后续工具调用中使用 cacheId=\"");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些图块");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[CompilerGenerated]
	private sealed class Class362 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public object object_0;

		public int int_1;

		public object object_1;

		public object object_2;

		public AIToolContext aitoolContext_0;

		public CadLayerManagerTool cadLayerManagerTool_0;

		private object object_3;

		private MethodInfo methodInfo_0;

		private IEnumerable ienumerable_0;

		private PropertyInfo propertyInfo_0;

		private string string_0;

		private MethodInfo methodInfo_1;

		private object object_4;

		private object object_5;

		private Exception exception_0;

		private object object_6;

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
					Class362 stateMachine = this;
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
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[CadLayerManagerTool] Analyze: 开始分析 CAD 链接 ");
			defaultInterpolatedStringHandler.AppendFormatted(int_1);
			defaultInterpolatedStringHandler.AppendLiteral(" 的图层内容");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			object_3 = object_1.GetType().GetMethod("GetCADFilePath")?.Invoke(object_1, new object[2] { object_0, aitoolContext_0.Document });
			AIToolResult result;
			if (object_3 != null)
			{
				propertyInfo_0 = object_3.GetType().GetProperty("FilePath");
				string_0 = propertyInfo_0?.GetValue(object_3)?.ToString();
				if (!string.IsNullOrEmpty(string_0) && File.Exists(string_0))
				{
					try
					{
						Logger.Info("[CadLayerManagerTool] Analyze: 从 CAD 文件分析: " + string_0);
						methodInfo_1 = object_1.GetType().GetMethod("ParseImportInstance");
						object_4 = methodInfo_1?.Invoke(object_1, new object[2] { object_0, aitoolContext_0.Document });
						if (object_4 != null)
						{
							object_5 = cadLayerManagerTool_0.method_5(object_4, int_1, "CAD 文件");
							Logger.Info("[CadLayerManagerTool] Analyze: 分析完成");
							result = AIToolResult.Ok(cadLayerManagerTool_0.method_8(object_5), object_5);
							goto IL_037b;
						}
						methodInfo_1 = null;
						object_4 = null;
					}
					catch (Exception ex)
					{
						exception_0 = ex;
						Logger.Warning("[CadLayerManagerTool] Analyze: 从 CAD 文件分析失败: " + exception_0.Message + "，尝试从几何对象分析");
					}
				}
				propertyInfo_0 = null;
				string_0 = null;
			}
			Logger.Info("[CadLayerManagerTool] Analyze: 从几何对象分析图层内容");
			methodInfo_0 = object_2.GetType().GetMethod("GetGeometryObjects");
			ienumerable_0 = methodInfo_0?.Invoke(object_2, new object[1] { object_0 }) as IEnumerable;
			if (ienumerable_0 != null)
			{
				object_6 = cadLayerManagerTool_0.method_6(ienumerable_0, object_2, aitoolContext_0.Document, int_1);
				Logger.Info("[CadLayerManagerTool] Analyze: 从几何对象分析完成");
				result = AIToolResult.Ok(cadLayerManagerTool_0.method_8(object_6), object_6);
			}
			else
			{
				result = AIToolResult.Fail("分析失败：无法获取几何对象");
			}
			goto IL_037b;
			IL_037b:
			int_0 = -2;
			object_3 = null;
			methodInfo_0 = null;
			ienumerable_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class363 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CadLayerManagerTool cadLayerManagerTool_0;

		private ICADFileService icadfileService_0;

		private IElementService ielementService_0;

		private ICADGeometryService icadgeometryService_0;

		private string string_0;

		private int int_1;

		private IEnumerable<object> ienumerable_0;

		private object object_0;

		private IEnumerator<object> ienumerator_0;

		private object object_1;

		private int? nullable_0;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private AIToolResult aitoolResult_3;

		private AIToolResult aitoolResult_4;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				if ((uint)(num - 1) <= 4u)
				{
					goto IL_006e;
				}
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class363 stateMachine = this;
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
				AIToolResult val;
				TaskAwaiter<AIToolResult> awaiter2;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter4;
				TaskAwaiter<AIToolResult> awaiter5;
				TaskAwaiter<AIToolResult> awaiter6;
				switch (num)
				{
				default:
				{
					IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
					icadfileService_0 = ((revitAdapter != null) ? revitAdapter.CADFileService : null);
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
					IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
					icadgeometryService_0 = ((revitAdapter3 != null) ? revitAdapter3.CADGeometryService : null);
					if (icadfileService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 CADFileService");
					}
					else if (ielementService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 ElementService");
					}
					else if (icadgeometryService_0 == null)
					{
						result = AIToolResult.Fail("无法获取 CADGeometryService");
					}
					else if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						string_0 = aitoolContext_0.GetParameter<string>("operation", (string)null);
						if (string.IsNullOrEmpty(string_0))
						{
							result = AIToolResult.Fail("必须指定 operation 参数");
						}
						else
						{
							int_1 = aitoolContext_0.GetParameter<int>("linkId", 0);
							if (int_1 <= 0)
							{
								result = AIToolResult.Fail("必须指定有效的 linkId");
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[CadLayerManagerTool] 执行操作: ");
								defaultInterpolatedStringHandler.AppendFormatted(string_0);
								defaultInterpolatedStringHandler.AppendLiteral(", linkId: ");
								defaultInterpolatedStringHandler.AppendFormatted(int_1);
								Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
								ienumerable_0 = icadgeometryService_0.GetCADImportInstances(aitoolContext_0.Document);
								object_0 = null;
								ienumerator_0 = ienumerable_0.GetEnumerator();
								try
								{
									while (ienumerator_0.MoveNext())
									{
										object_1 = ienumerator_0.Current;
										nullable_0 = ielementService_0.GetElementId(object_1);
										if (nullable_0 != int_1)
										{
											object_1 = null;
											continue;
										}
										object_0 = object_1;
										break;
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
								if (object_0 != null)
								{
									string_1 = string_0.ToLower();
									string text = string_1;
									if (!(text == "get_layers"))
									{
										if (!(text == "get_lines"))
										{
											if (!(text == "get_texts"))
											{
												if (!(text == "get_blocks"))
												{
													if (!(text == "analyze"))
													{
														val = AIToolResult.Fail("不支持的操作类型: " + string_0);
														break;
													}
													awaiter2 = cadLayerManagerTool_0.method_4(object_0, int_1, icadfileService_0, icadgeometryService_0, aitoolContext_0).GetAwaiter();
													if (!awaiter2.IsCompleted)
													{
														num = 5;
														int_0 = 5;
														taskAwaiter_1 = awaiter2;
														Class363 stateMachine = this;
														asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
														return;
													}
													goto IL_073e;
												}
												awaiter3 = cadLayerManagerTool_0.method_3(object_0, int_1, icadfileService_0, icadgeometryService_0, aitoolContext_0).GetAwaiter();
												if (!awaiter3.IsCompleted)
												{
													num = 4;
													int_0 = 4;
													taskAwaiter_1 = awaiter3;
													Class363 stateMachine = this;
													asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
													return;
												}
												goto IL_0703;
											}
											awaiter4 = cadLayerManagerTool_0.method_2(object_0, int_1, icadfileService_0, icadgeometryService_0, aitoolContext_0).GetAwaiter();
											if (!awaiter4.IsCompleted)
											{
												num = 3;
												int_0 = 3;
												taskAwaiter_1 = awaiter4;
												Class363 stateMachine = this;
												asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
												return;
											}
											goto IL_06c8;
										}
										awaiter5 = cadLayerManagerTool_0.method_1(object_0, int_1, icadfileService_0, icadgeometryService_0, aitoolContext_0).GetAwaiter();
										if (!awaiter5.IsCompleted)
										{
											num = 2;
											int_0 = 2;
											taskAwaiter_1 = awaiter5;
											Class363 stateMachine = this;
											asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
											return;
										}
										goto IL_068a;
									}
									awaiter6 = cadLayerManagerTool_0.method_0(object_0, int_1, icadfileService_0, icadgeometryService_0, aitoolContext_0).GetAwaiter();
									if (!awaiter6.IsCompleted)
									{
										num = 1;
										int_0 = 1;
										taskAwaiter_1 = awaiter6;
										Class363 stateMachine = this;
										asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter6, ref stateMachine);
										return;
									}
									goto IL_064c;
								}
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("[CadLayerManagerTool] 未找到 ID 为 ");
								defaultInterpolatedStringHandler2.AppendFormatted(int_1);
								defaultInterpolatedStringHandler2.AppendLiteral(" 的 CAD 链接");
								Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(18, 1);
								defaultInterpolatedStringHandler3.AppendLiteral("未找到 ID 为 ");
								defaultInterpolatedStringHandler3.AppendFormatted(int_1);
								defaultInterpolatedStringHandler3.AppendLiteral(" 的 CAD 链接");
								result = AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear());
							}
						}
					}
					goto end_IL_006e;
				}
				case 1:
					awaiter6 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_064c;
				case 2:
					awaiter5 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_068a;
				case 3:
					awaiter4 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_06c8;
				case 4:
					awaiter3 = taskAwaiter_1;
					taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0703;
				case 5:
					{
						awaiter2 = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						goto IL_073e;
					}
					IL_0703:
					aitoolResult_3 = awaiter3.GetResult();
					val = aitoolResult_3;
					aitoolResult_3 = null;
					break;
					IL_06c8:
					aitoolResult_2 = awaiter4.GetResult();
					val = aitoolResult_2;
					aitoolResult_2 = null;
					break;
					IL_068a:
					aitoolResult_1 = awaiter5.GetResult();
					val = aitoolResult_1;
					aitoolResult_1 = null;
					break;
					IL_064c:
					aitoolResult_0 = awaiter6.GetResult();
					val = aitoolResult_0;
					aitoolResult_0 = null;
					break;
					IL_073e:
					aitoolResult_4 = awaiter2.GetResult();
					val = aitoolResult_4;
					aitoolResult_4 = null;
					break;
				}
				result = val;
				end_IL_006e:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[CadLayerManagerTool] 执行失败: " + exception_0.Message);
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
	private sealed class Class364 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public object object_0;

		public int int_1;

		public object object_1;

		public object object_2;

		public AIToolContext aitoolContext_0;

		public CadLayerManagerTool cadLayerManagerTool_0;

		private Class359 class359_0;

		private object object_3;

		private MethodInfo methodInfo_0;

		private IEnumerable ienumerable_0;

		private PropertyInfo propertyInfo_0;

		private string string_0;

		private MethodInfo methodInfo_1;

		private object object_4;

		private MethodInfo methodInfo_2;

		private IEnumerable ienumerable_1;

		private Class360 class360_0;

		private IEnumerator ienumerator_0;

		private object object_5;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private PropertyInfo propertyInfo_3;

		private PropertyInfo propertyInfo_4;

		private PropertyInfo propertyInfo_5;

		private PropertyInfo propertyInfo_6;

		private PropertyInfo propertyInfo_7;

		private PropertyInfo propertyInfo_8;

		private PropertyInfo propertyInfo_9;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private Exception exception_0;

		private MethodInfo methodInfo_3;

		private IEnumerable ienumerable_2;

		private Class361 class361_0;

		private MethodInfo methodInfo_4;

		private MethodInfo methodInfo_5;

		private IEnumerator ienumerator_1;

		private object object_6;

		private string string_2;

		private object object_7;

		private PropertyInfo propertyInfo_10;

		private PropertyInfo propertyInfo_11;

		private object object_8;

		private PropertyInfo propertyInfo_12;

		private PropertyInfo propertyInfo_13;

		private PropertyInfo propertyInfo_14;

		private double double_0;

		private double double_1;

		private double double_2;

		private string string_3;

		private AIToolResult aitoolResult_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				class359_0 = new Class359();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class364 stateMachine = this;
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
			class359_0.string_0 = aitoolContext_0.GetParameter<string>("layerName", (string)null);
			AIToolResult result;
			if (string.IsNullOrWhiteSpace(class359_0.string_0))
			{
				result = AIToolResult.Fail("get_blocks 操作需要指定 layerName 参数");
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[CadLayerManagerTool] GetBlocks: 开始获取 CAD 链接 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(" 图层 '");
				defaultInterpolatedStringHandler.AppendFormatted(class359_0.string_0);
				defaultInterpolatedStringHandler.AppendLiteral("' 的图块信息");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				object_3 = object_1.GetType().GetMethod("GetCADFilePath")?.Invoke(object_1, new object[2] { object_0, aitoolContext_0.Document });
				if (object_3 != null)
				{
					propertyInfo_0 = object_3.GetType().GetProperty("FilePath");
					string_0 = propertyInfo_0?.GetValue(object_3)?.ToString();
					if (!string.IsNullOrEmpty(string_0) && File.Exists(string_0))
					{
						try
						{
							Logger.Info("[CadLayerManagerTool] GetBlocks: 从 CAD 文件获取图块: " + string_0);
							methodInfo_1 = object_1.GetType().GetMethod("ParseImportInstance");
							object_4 = methodInfo_1?.Invoke(object_1, new object[2] { object_0, aitoolContext_0.Document });
							if (object_4 != null)
							{
								methodInfo_2 = object_4.GetType().GetMethod("GetBlocksByLayer");
								MethodInfo methodInfo = methodInfo_2;
								object obj;
								if ((object)methodInfo == null)
								{
									obj = null;
								}
								else
								{
									object obj2 = object_4;
									object[] parameters = new string[1] { class359_0.string_0 };
									obj = methodInfo.Invoke(obj2, parameters);
								}
								ienumerable_1 = obj as IEnumerable;
								if (ienumerable_1 != null)
								{
									class360_0 = new Class360();
									class360_0.class359_0 = class359_0;
									class360_0.list_0 = new List<object>();
									ienumerator_0 = ienumerable_1.GetEnumerator();
									try
									{
										while (ienumerator_0.MoveNext())
										{
											object_5 = ienumerator_0.Current;
											try
											{
												propertyInfo_1 = object_5.GetType().GetProperty("Name");
												propertyInfo_2 = object_5.GetType().GetProperty("X");
												propertyInfo_3 = object_5.GetType().GetProperty("Y");
												propertyInfo_4 = object_5.GetType().GetProperty("Z");
												propertyInfo_5 = object_5.GetType().GetProperty("ScaleX");
												propertyInfo_6 = object_5.GetType().GetProperty("ScaleY");
												propertyInfo_7 = object_5.GetType().GetProperty("ScaleZ");
												propertyInfo_8 = object_5.GetType().GetProperty("Rotation");
												propertyInfo_9 = object_5.GetType().GetProperty("LayerName");
												List<object> list = class360_0.list_0;
												string? gparam_ = propertyInfo_1?.GetValue(object_5)?.ToString();
												PropertyInfo propertyInfo = propertyInfo_2;
												object obj3;
												if ((object)propertyInfo == null)
												{
													obj3 = null;
												}
												else
												{
													obj3 = propertyInfo.GetValue(object_5);
													if (obj3 != null)
													{
														goto IL_04a6;
													}
												}
												obj3 = 0;
												goto IL_04a6;
												IL_04d3:
												object obj4;
												double gparam_2 = Math.Round(Convert.ToDouble(obj4), 2);
												PropertyInfo propertyInfo2 = propertyInfo_4;
												object obj5;
												if ((object)propertyInfo2 == null)
												{
													obj5 = null;
												}
												else
												{
													obj5 = propertyInfo2.GetValue(object_5);
													if (obj5 != null)
													{
														goto IL_0500;
													}
												}
												obj5 = 0;
												goto IL_0500;
												IL_04a6:
												double gparam_3 = Math.Round(Convert.ToDouble(obj3), 2);
												PropertyInfo propertyInfo3 = propertyInfo_3;
												if ((object)propertyInfo3 == null)
												{
													obj4 = null;
												}
												else
												{
													obj4 = propertyInfo3.GetValue(object_5);
													if (obj4 != null)
													{
														goto IL_04d3;
													}
												}
												obj4 = 0;
												goto IL_04d3;
												IL_0500:
												list.Add(new Class14<string, double, double, double, double?, double?, double?, double?, string, string>(gparam_, gparam_3, gparam_2, Math.Round(Convert.ToDouble(obj5), 2), (propertyInfo_5?.GetValue(object_5) != null) ? new double?(Math.Round(Convert.ToDouble(propertyInfo_5.GetValue(object_5)), 3)) : ((double?)null), (propertyInfo_6?.GetValue(object_5) != null) ? new double?(Math.Round(Convert.ToDouble(propertyInfo_6.GetValue(object_5)), 3)) : ((double?)null), (propertyInfo_7?.GetValue(object_5) != null) ? new double?(Math.Round(Convert.ToDouble(propertyInfo_7.GetValue(object_5)), 3)) : ((double?)null), (propertyInfo_8?.GetValue(object_5) != null) ? new double?(Math.Round(Convert.ToDouble(propertyInfo_8.GetValue(object_5)) * 180.0 / Math.PI, 2)) : ((double?)null), propertyInfo_9?.GetValue(object_5)?.ToString(), "毫米"));
												propertyInfo_1 = null;
												propertyInfo_2 = null;
												propertyInfo_3 = null;
												propertyInfo_4 = null;
												propertyInfo_5 = null;
												propertyInfo_6 = null;
												propertyInfo_7 = null;
												propertyInfo_8 = null;
												propertyInfo_9 = null;
											}
											catch
											{
											}
											object_5 = null;
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
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(49, 1);
									defaultInterpolatedStringHandler2.AppendLiteral("[CadLayerManagerTool] GetBlocks: 从 CAD 文件获取到 ");
									defaultInterpolatedStringHandler2.AppendFormatted(class360_0.list_0.Count);
									defaultInterpolatedStringHandler2.AppendLiteral(" 个图块");
									Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
									if (aitoolContext_0.DataCache != null && !string.IsNullOrEmpty(aitoolContext_0.SessionId))
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 3);
										defaultInterpolatedStringHandler3.AppendLiteral("cad_blocks_");
										defaultInterpolatedStringHandler3.AppendFormatted(int_1);
										defaultInterpolatedStringHandler3.AppendLiteral("_");
										defaultInterpolatedStringHandler3.AppendFormatted(class360_0.class359_0.string_0);
										defaultInterpolatedStringHandler3.AppendLiteral("_");
										defaultInterpolatedStringHandler3.AppendFormatted(DateTime.Now.Ticks);
										string_1 = defaultInterpolatedStringHandler3.ToStringAndClear();
										aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)class360_0.list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_1, "个图块", 100);
										cadLayerManagerTool_0.method_9(aitoolResult_0, delegate(string string_0)
										{
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(51, 3);
											defaultInterpolatedStringHandler8.AppendLiteral("从图层 '");
											defaultInterpolatedStringHandler8.AppendFormatted(class360_0.class359_0.string_0);
											defaultInterpolatedStringHandler8.AppendLiteral("' 获取到 ");
											defaultInterpolatedStringHandler8.AppendFormatted(class360_0.list_0.Count);
											defaultInterpolatedStringHandler8.AppendLiteral(" 个图块\n\n💡 在后续工具调用中使用 cacheId=\"");
											defaultInterpolatedStringHandler8.AppendFormatted(string_0);
											defaultInterpolatedStringHandler8.AppendLiteral("\" 参数来操作这些图块");
											return defaultInterpolatedStringHandler8.ToStringAndClear();
										});
										result = aitoolResult_0;
									}
									else
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(15, 2);
										defaultInterpolatedStringHandler4.AppendLiteral("从图层 '");
										defaultInterpolatedStringHandler4.AppendFormatted(class360_0.class359_0.string_0);
										defaultInterpolatedStringHandler4.AppendLiteral("' 获取到 ");
										defaultInterpolatedStringHandler4.AppendFormatted(class360_0.list_0.Count);
										defaultInterpolatedStringHandler4.AppendLiteral(" 个图块");
										result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class15<string, int, string, string, int, List<object>>("get_blocks", int_1, class360_0.class359_0.string_0, "CAD 文件", class360_0.list_0.Count, class360_0.list_0));
									}
									goto IL_107c;
								}
								methodInfo_2 = null;
								ienumerable_1 = null;
							}
							methodInfo_1 = null;
							object_4 = null;
						}
						catch (Exception ex)
						{
							exception_0 = ex;
							Logger.Warning("[CadLayerManagerTool] GetBlocks: 从 CAD 文件获取图块失败: " + exception_0.Message + "，尝试从几何对象获取");
						}
					}
					propertyInfo_0 = null;
					string_0 = null;
				}
				Logger.Info("[CadLayerManagerTool] GetBlocks: 从几何对象获取图块信息");
				methodInfo_0 = object_2.GetType().GetMethod("GetGeometryObjects");
				ienumerable_0 = methodInfo_0?.Invoke(object_2, new object[1] { object_0 }) as IEnumerable;
				if (ienumerable_0 != null)
				{
					methodInfo_3 = object_2.GetType().GetMethod("FilterByLayer");
					ienumerable_2 = methodInfo_3?.Invoke(object_2, new object[3] { ienumerable_0, class359_0.string_0, aitoolContext_0.Document }) as IEnumerable;
					if (ienumerable_2 != null)
					{
						class361_0 = new Class361();
						class361_0.list_0 = new List<object>();
						methodInfo_4 = object_2.GetType().GetMethod("GetGeometryObjectType");
						methodInfo_5 = object_2.GetType().GetMethod("GetBlockInfo");
						ienumerator_1 = ienumerable_2.GetEnumerator();
						try
						{
							while (ienumerator_1.MoveNext())
							{
								object_6 = ienumerator_1.Current;
								try
								{
									string_2 = methodInfo_4?.Invoke(object_2, new object[1] { object_6 })?.ToString();
									object obj7;
									if (string_2 == "Block")
									{
										object_7 = methodInfo_5?.Invoke(object_2, new object[1] { object_6 });
										if (object_7 != null)
										{
											propertyInfo_10 = object_7.GetType().GetProperty("position");
											propertyInfo_11 = object_7.GetType().GetProperty("name");
											object_8 = propertyInfo_10?.GetValue(object_7);
											if (object_8 != null)
											{
												propertyInfo_12 = object_8.GetType().GetProperty("X");
												propertyInfo_13 = object_8.GetType().GetProperty("Y");
												propertyInfo_14 = object_8.GetType().GetProperty("Z");
												PropertyInfo propertyInfo4 = propertyInfo_12;
												if ((object)propertyInfo4 == null)
												{
													obj7 = null;
												}
												else
												{
													obj7 = propertyInfo4.GetValue(object_8);
													if (obj7 != null)
													{
														goto IL_0c6d;
													}
												}
												obj7 = 0;
												goto IL_0c6d;
											}
											goto IL_0dac;
										}
										goto IL_0dc1;
									}
									goto IL_0dc8;
									IL_0c9a:
									object obj8;
									double_1 = Convert.ToDouble(obj8);
									PropertyInfo propertyInfo5 = propertyInfo_14;
									object obj9;
									if ((object)propertyInfo5 == null)
									{
										obj9 = null;
									}
									else
									{
										obj9 = propertyInfo5.GetValue(object_8);
										if (obj9 != null)
										{
											goto IL_0cc7;
										}
									}
									obj9 = 0;
									goto IL_0cc7;
									IL_0dac:
									propertyInfo_10 = null;
									propertyInfo_11 = null;
									object_8 = null;
									goto IL_0dc1;
									IL_0cc7:
									double_2 = Convert.ToDouble(obj9);
									List<object> list2 = class361_0.list_0;
									PropertyInfo propertyInfo6 = propertyInfo_11;
									object obj10;
									if ((object)propertyInfo6 == null)
									{
										obj10 = null;
									}
									else
									{
										object? value = propertyInfo6.GetValue(object_7);
										if (value == null)
										{
											obj10 = null;
										}
										else
										{
											obj10 = value.ToString();
											if (obj10 != null)
											{
												goto IL_0d0e;
											}
										}
									}
									obj10 = "未知图块";
									goto IL_0d0e;
									IL_0dc8:
									string_2 = null;
									goto end_IL_0af0;
									IL_0dc1:
									object_7 = null;
									goto IL_0dc8;
									IL_0c6d:
									double_0 = Convert.ToDouble(obj7);
									PropertyInfo propertyInfo7 = propertyInfo_13;
									if ((object)propertyInfo7 == null)
									{
										obj8 = null;
									}
									else
									{
										obj8 = propertyInfo7.GetValue(object_8);
										if (obj8 != null)
										{
											goto IL_0c9a;
										}
									}
									obj8 = 0;
									goto IL_0c9a;
									IL_0d0e:
									list2.Add(new Class14<string, double, double, double, double?, double?, double?, double?, string, string>((string)obj10, Math.Round(double_0 * 304.8, 2), Math.Round(double_1 * 304.8, 2), Math.Round(double_2 * 304.8, 2), null, null, null, null, class359_0.string_0, "毫米"));
									propertyInfo_12 = null;
									propertyInfo_13 = null;
									propertyInfo_14 = null;
									goto IL_0dac;
									end_IL_0af0:;
								}
								catch
								{
								}
								object_6 = null;
							}
						}
						finally
						{
							if (num < 0 && ienumerator_1 is IDisposable disposable2)
							{
								disposable2.Dispose();
							}
						}
						ienumerator_1 = null;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(46, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("[CadLayerManagerTool] GetBlocks: 从几何对象获取到 ");
						defaultInterpolatedStringHandler5.AppendFormatted(class361_0.list_0.Count);
						defaultInterpolatedStringHandler5.AppendLiteral(" 个图块");
						Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
						if (class361_0.list_0.Count == 0)
						{
							result = AIToolResult.Fail("无法从图层 '" + class359_0.string_0 + "' 获取图块信息");
						}
						else if (aitoolContext_0.DataCache != null && !string.IsNullOrEmpty(aitoolContext_0.SessionId))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(18, 3);
							defaultInterpolatedStringHandler6.AppendLiteral("cad_geom_blocks_");
							defaultInterpolatedStringHandler6.AppendFormatted(int_1);
							defaultInterpolatedStringHandler6.AppendLiteral("_");
							defaultInterpolatedStringHandler6.AppendFormatted(class359_0.string_0);
							defaultInterpolatedStringHandler6.AppendLiteral("_");
							defaultInterpolatedStringHandler6.AppendFormatted(DateTime.Now.Ticks);
							string_3 = defaultInterpolatedStringHandler6.ToStringAndClear();
							aitoolResult_1 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)class361_0.list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_3, "个图块", 100);
							cadLayerManagerTool_0.method_9(aitoolResult_1, delegate(string string_0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(49, 2);
								defaultInterpolatedStringHandler8.AppendLiteral("从几何对象获取到 ");
								defaultInterpolatedStringHandler8.AppendFormatted(class361_0.list_0.Count);
								defaultInterpolatedStringHandler8.AppendLiteral(" 个图块\n\n💡 在后续工具调用中使用 cacheId=\"");
								defaultInterpolatedStringHandler8.AppendFormatted(string_0);
								defaultInterpolatedStringHandler8.AppendLiteral("\" 参数来操作这些图块");
								return defaultInterpolatedStringHandler8.ToStringAndClear();
							});
							result = aitoolResult_1;
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler7.AppendLiteral("从几何对象获取到 ");
							defaultInterpolatedStringHandler7.AppendFormatted(class361_0.list_0.Count);
							defaultInterpolatedStringHandler7.AppendLiteral(" 个图块");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler7.ToStringAndClear(), (object)new Class15<string, int, string, string, int, List<object>>("get_blocks", int_1, class359_0.string_0, "几何对象", class361_0.list_0.Count, class361_0.list_0));
						}
						goto IL_107c;
					}
					methodInfo_3 = null;
					ienumerable_2 = null;
				}
				result = AIToolResult.Fail("无法从图层 '" + class359_0.string_0 + "' 获取图块信息");
			}
			goto IL_107c;
			IL_107c:
			int_0 = -2;
			class359_0 = null;
			object_3 = null;
			methodInfo_0 = null;
			ienumerable_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class365 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public object object_0;

		public int int_1;

		public object object_1;

		public object object_2;

		public AIToolContext aitoolContext_0;

		public CadLayerManagerTool cadLayerManagerTool_0;

		private object object_3;

		private MethodInfo methodInfo_0;

		private IEnumerable ienumerable_0;

		private HashSet<string> hashSet_0;

		private List<_003C_003Ef__AnonymousType7<string, int?, bool, bool, string>> list_0;

		private PropertyInfo propertyInfo_0;

		private string string_0;

		private PropertyInfo propertyInfo_1;

		private MethodInfo methodInfo_1;

		private object object_4;

		private object object_5;

		private PropertyInfo propertyInfo_2;

		private IEnumerable ienumerable_1;

		private List<object> list_1;

		private IEnumerator ienumerator_0;

		private object object_6;

		private PropertyInfo propertyInfo_3;

		private PropertyInfo propertyInfo_4;

		private PropertyInfo propertyInfo_5;

		private PropertyInfo propertyInfo_6;

		private PropertyInfo propertyInfo_7;

		private Exception exception_0;

		private MethodInfo methodInfo_2;

		private IEnumerator ienumerator_1;

		private object object_7;

		private string string_1;

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
					Class365 stateMachine = this;
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
			object_3 = object_1.GetType().GetMethod("GetCADFilePath")?.Invoke(object_1, new object[2] { object_0, aitoolContext_0.Document });
			AIToolResult result;
			if (object_3 != null)
			{
				propertyInfo_0 = object_3.GetType().GetProperty("FilePath");
				string_0 = propertyInfo_0?.GetValue(object_3)?.ToString();
				propertyInfo_1 = object_3.GetType().GetProperty("ConversionFactorToMM");
				if (!string.IsNullOrEmpty(string_0) && File.Exists(string_0))
				{
					try
					{
						Logger.Info("[CadLayerManagerTool] GetLayers: 从 CAD 文件获取图层: " + string_0);
						methodInfo_1 = object_1.GetType().GetMethod("ParseCADFile");
						object_4 = propertyInfo_1?.GetValue(object_3);
						object_5 = methodInfo_1?.Invoke(object_1, new object[2] { string_0, object_4 });
						if (object_5 != null)
						{
							propertyInfo_2 = object_5.GetType().GetProperty("Layers");
							ienumerable_1 = propertyInfo_2?.GetValue(object_5) as IEnumerable;
							if (ienumerable_1 != null)
							{
								list_1 = new List<object>();
								ienumerator_0 = ienumerable_1.GetEnumerator();
								try
								{
									while (ienumerator_0.MoveNext())
									{
										object_6 = ienumerator_0.Current;
										try
										{
											propertyInfo_3 = object_6.GetType().GetProperty("Name");
											propertyInfo_4 = object_6.GetType().GetProperty("ColorIndex");
											propertyInfo_5 = object_6.GetType().GetProperty("IsVisible");
											propertyInfo_6 = object_6.GetType().GetProperty("IsLocked");
											propertyInfo_7 = object_6.GetType().GetProperty("LineTypeName");
											list_1.Add(new
											{
												name = propertyInfo_3?.GetValue(object_6)?.ToString(),
												colorIndex = (propertyInfo_4?.GetValue(object_6) as int?),
												isVisible = (propertyInfo_5?.GetValue(object_6) as bool?),
												isLocked = (propertyInfo_6?.GetValue(object_6) as bool?),
												lineTypeName = propertyInfo_7?.GetValue(object_6)?.ToString()
											});
											propertyInfo_3 = null;
											propertyInfo_4 = null;
											propertyInfo_5 = null;
											propertyInfo_6 = null;
											propertyInfo_7 = null;
										}
										catch
										{
										}
										object_6 = null;
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
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
								defaultInterpolatedStringHandler.AppendLiteral("[CadLayerManagerTool] GetLayers: 从 CAD 文件获取到 ");
								defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
								defaultInterpolatedStringHandler.AppendLiteral(" 个图层");
								Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(16, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("从 CAD 文件获取到 ");
								defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
								defaultInterpolatedStringHandler2.AppendLiteral(" 个图层");
								result = AIToolResult.Ok(defaultInterpolatedStringHandler2.ToStringAndClear(), (object)new Class7<string, int, string, string, int, List<object>>("get_layers", int_1, "CAD 文件", string_0, list_1.Count, list_1));
								goto IL_083d;
							}
							propertyInfo_2 = null;
							ienumerable_1 = null;
						}
						methodInfo_1 = null;
						object_4 = null;
						object_5 = null;
					}
					catch (Exception ex)
					{
						exception_0 = ex;
						Logger.Warning("[CadLayerManagerTool] GetLayers: 从 CAD 文件获取图层失败: " + exception_0.Message + "，尝试从几何对象获取");
					}
				}
				propertyInfo_0 = null;
				string_0 = null;
				propertyInfo_1 = null;
			}
			Logger.Info("[CadLayerManagerTool] GetLayers: 从几何对象获取图层信息");
			methodInfo_0 = object_2.GetType().GetMethod("GetGeometryObjects");
			ienumerable_0 = methodInfo_0?.Invoke(object_2, new object[1] { object_0 }) as IEnumerable;
			hashSet_0 = new HashSet<string>();
			if (ienumerable_0 != null)
			{
				methodInfo_2 = object_2.GetType().GetMethod("GetGeometryObjectLayer");
				ienumerator_1 = ienumerable_0.GetEnumerator();
				try
				{
					while (ienumerator_1.MoveNext())
					{
						object_7 = ienumerator_1.Current;
						try
						{
							string_1 = methodInfo_2?.Invoke(object_2, new object[2] { object_7, aitoolContext_0.Document })?.ToString();
							if (!string.IsNullOrEmpty(string_1))
							{
								hashSet_0.Add(string_1);
							}
							string_1 = null;
						}
						catch
						{
						}
						object_7 = null;
					}
				}
				finally
				{
					if (num < 0 && ienumerator_1 is IDisposable disposable2)
					{
						disposable2.Dispose();
					}
				}
				ienumerator_1 = null;
				methodInfo_2 = null;
			}
			list_0 = hashSet_0.Select((string string_0) => new _003C_003Ef__AnonymousType7<string, int?, bool, bool, string>(string_0, (int?)null, true, false, (string)null)).ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(46, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("[CadLayerManagerTool] GetLayers: 从几何对象获取到 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler3.AppendLiteral(" 个图层");
			Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			if (list_0.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(18, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("无法从 CAD 链接 ");
				defaultInterpolatedStringHandler4.AppendFormatted(int_1);
				defaultInterpolatedStringHandler4.AppendLiteral(" 获取图层信息");
				result = AIToolResult.Fail(defaultInterpolatedStringHandler4.ToStringAndClear());
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler5.AppendLiteral("从几何对象获取到 ");
				defaultInterpolatedStringHandler5.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler5.AppendLiteral(" 个图层");
				result = AIToolResult.Ok(defaultInterpolatedStringHandler5.ToStringAndClear(), (object)new Class8<string, int, string, int, List<_003C_003Ef__AnonymousType7<string, int?, bool, bool, string>>>("get_layers", int_1, "几何对象", list_0.Count, list_0));
			}
			goto IL_083d;
			IL_083d:
			int_0 = -2;
			object_3 = null;
			methodInfo_0 = null;
			ienumerable_0 = null;
			hashSet_0 = null;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class366 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public object object_0;

		public int int_1;

		public object object_1;

		public object object_2;

		public AIToolContext aitoolContext_0;

		public CadLayerManagerTool cadLayerManagerTool_0;

		private Class354 class354_0;

		private object object_3;

		private MethodInfo methodInfo_0;

		private IEnumerable ienumerable_0;

		private PropertyInfo propertyInfo_0;

		private string string_0;

		private MethodInfo methodInfo_1;

		private object object_4;

		private MethodInfo methodInfo_2;

		private IEnumerable ienumerable_1;

		private Class355 class355_0;

		private IEnumerator ienumerator_0;

		private object object_5;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private PropertyInfo propertyInfo_3;

		private PropertyInfo propertyInfo_4;

		private PropertyInfo propertyInfo_5;

		private PropertyInfo propertyInfo_6;

		private PropertyInfo propertyInfo_7;

		private PropertyInfo propertyInfo_8;

		private PropertyInfo propertyInfo_9;

		private PropertyInfo propertyInfo_10;

		private PropertyInfo propertyInfo_11;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private Exception exception_0;

		private MethodInfo methodInfo_3;

		private IEnumerable ienumerable_2;

		private Class356 class356_0;

		private MethodInfo methodInfo_4;

		private MethodInfo methodInfo_5;

		private IEnumerator ienumerator_1;

		private object object_6;

		private string string_2;

		private object object_7;

		private PropertyInfo propertyInfo_12;

		private PropertyInfo propertyInfo_13;

		private object object_8;

		private object object_9;

		private PropertyInfo propertyInfo_14;

		private PropertyInfo propertyInfo_15;

		private PropertyInfo propertyInfo_16;

		private double double_0;

		private double double_1;

		private double double_2;

		private double double_3;

		private double double_4;

		private double double_5;

		private double double_6;

		private double double_7;

		private double double_8;

		private double double_9;

		private string string_3;

		private AIToolResult aitoolResult_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				class354_0 = new Class354();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class366 stateMachine = this;
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
			class354_0.string_0 = aitoolContext_0.GetParameter<string>("layerName", (string)null);
			AIToolResult result;
			if (string.IsNullOrWhiteSpace(class354_0.string_0))
			{
				result = AIToolResult.Fail("get_lines 操作需要指定 layerName 参数");
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[CadLayerManagerTool] GetLines: 开始获取 CAD 链接 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(" 图层 '");
				defaultInterpolatedStringHandler.AppendFormatted(class354_0.string_0);
				defaultInterpolatedStringHandler.AppendLiteral("' 的线条信息");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				object_3 = object_1.GetType().GetMethod("GetCADFilePath")?.Invoke(object_1, new object[2] { object_0, aitoolContext_0.Document });
				if (object_3 != null)
				{
					propertyInfo_0 = object_3.GetType().GetProperty("FilePath");
					string_0 = propertyInfo_0?.GetValue(object_3)?.ToString();
					if (!string.IsNullOrEmpty(string_0) && File.Exists(string_0))
					{
						try
						{
							Logger.Info("[CadLayerManagerTool] GetLines: 从 CAD 文件获取线条: " + string_0);
							methodInfo_1 = object_1.GetType().GetMethod("ParseImportInstance");
							object_4 = methodInfo_1?.Invoke(object_1, new object[2] { object_0, aitoolContext_0.Document });
							if (object_4 != null)
							{
								methodInfo_2 = object_4.GetType().GetMethod("GetLinesByLayer");
								MethodInfo methodInfo = methodInfo_2;
								object obj;
								if ((object)methodInfo == null)
								{
									obj = null;
								}
								else
								{
									object obj2 = object_4;
									object[] parameters = new string[1] { class354_0.string_0 };
									obj = methodInfo.Invoke(obj2, parameters);
								}
								ienumerable_1 = obj as IEnumerable;
								if (ienumerable_1 != null)
								{
									class355_0 = new Class355();
									class355_0.class354_0 = class354_0;
									class355_0.list_0 = new List<object>();
									ienumerator_0 = ienumerable_1.GetEnumerator();
									try
									{
										while (ienumerator_0.MoveNext())
										{
											object_5 = ienumerator_0.Current;
											try
											{
												propertyInfo_1 = object_5.GetType().GetProperty("Id");
												propertyInfo_2 = object_5.GetType().GetProperty("StartX");
												propertyInfo_3 = object_5.GetType().GetProperty("StartY");
												propertyInfo_4 = object_5.GetType().GetProperty("StartZ");
												propertyInfo_5 = object_5.GetType().GetProperty("EndX");
												propertyInfo_6 = object_5.GetType().GetProperty("EndY");
												propertyInfo_7 = object_5.GetType().GetProperty("EndZ");
												propertyInfo_8 = object_5.GetType().GetProperty("Length");
												propertyInfo_9 = object_5.GetType().GetProperty("DirectionAngle");
												propertyInfo_10 = object_5.GetType().GetProperty("LineType");
												propertyInfo_11 = object_5.GetType().GetProperty("LayerName");
												List<object> list = class355_0.list_0;
												object? gparam_ = propertyInfo_1?.GetValue(object_5);
												PropertyInfo propertyInfo = propertyInfo_2;
												object obj3;
												if ((object)propertyInfo == null)
												{
													obj3 = null;
												}
												else
												{
													obj3 = propertyInfo.GetValue(object_5);
													if (obj3 != null)
													{
														goto IL_04da;
													}
												}
												obj3 = 0;
												goto IL_04da;
												IL_0507:
												object obj4;
												double gparam_2 = Math.Round(Convert.ToDouble(obj4), 2);
												PropertyInfo propertyInfo2 = propertyInfo_4;
												object obj5;
												if ((object)propertyInfo2 == null)
												{
													obj5 = null;
												}
												else
												{
													obj5 = propertyInfo2.GetValue(object_5);
													if (obj5 != null)
													{
														goto IL_0534;
													}
												}
												obj5 = 0;
												goto IL_0534;
												IL_0534:
												double gparam_3 = Math.Round(Convert.ToDouble(obj5), 2);
												PropertyInfo propertyInfo3 = propertyInfo_5;
												object obj6;
												if ((object)propertyInfo3 == null)
												{
													obj6 = null;
												}
												else
												{
													obj6 = propertyInfo3.GetValue(object_5);
													if (obj6 != null)
													{
														goto IL_0561;
													}
												}
												obj6 = 0;
												goto IL_0561;
												IL_058e:
												object obj7;
												double gparam_4 = Math.Round(Convert.ToDouble(obj7), 2);
												PropertyInfo propertyInfo4 = propertyInfo_7;
												object obj8;
												if ((object)propertyInfo4 == null)
												{
													obj8 = null;
												}
												else
												{
													obj8 = propertyInfo4.GetValue(object_5);
													if (obj8 != null)
													{
														goto IL_05bb;
													}
												}
												obj8 = 0;
												goto IL_05bb;
												IL_0615:
												double gparam_5;
												double gparam_6;
												double gparam_7;
												double gparam_8;
												object obj9;
												list.Add(new Class9<object, double, double, double, double, double, double, double, double, string, string, string>(gparam_, gparam_5, gparam_2, gparam_3, gparam_6, gparam_4, gparam_7, gparam_8, Math.Round(Convert.ToDouble(obj9) * 180.0 / Math.PI, 2), propertyInfo_10?.GetValue(object_5)?.ToString(), propertyInfo_11?.GetValue(object_5)?.ToString(), "毫米"));
												propertyInfo_1 = null;
												propertyInfo_2 = null;
												propertyInfo_3 = null;
												propertyInfo_4 = null;
												propertyInfo_5 = null;
												propertyInfo_6 = null;
												propertyInfo_7 = null;
												propertyInfo_8 = null;
												propertyInfo_9 = null;
												propertyInfo_10 = null;
												propertyInfo_11 = null;
												goto end_IL_0335;
												IL_0561:
												gparam_6 = Math.Round(Convert.ToDouble(obj6), 2);
												PropertyInfo propertyInfo5 = propertyInfo_6;
												if ((object)propertyInfo5 == null)
												{
													obj7 = null;
												}
												else
												{
													obj7 = propertyInfo5.GetValue(object_5);
													if (obj7 != null)
													{
														goto IL_058e;
													}
												}
												obj7 = 0;
												goto IL_058e;
												IL_05bb:
												gparam_7 = Math.Round(Convert.ToDouble(obj8), 2);
												PropertyInfo propertyInfo6 = propertyInfo_8;
												object obj10;
												if ((object)propertyInfo6 == null)
												{
													obj10 = null;
												}
												else
												{
													obj10 = propertyInfo6.GetValue(object_5);
													if (obj10 != null)
													{
														goto IL_05e8;
													}
												}
												obj10 = 0;
												goto IL_05e8;
												IL_04da:
												gparam_5 = Math.Round(Convert.ToDouble(obj3), 2);
												PropertyInfo propertyInfo7 = propertyInfo_3;
												if ((object)propertyInfo7 == null)
												{
													obj4 = null;
												}
												else
												{
													obj4 = propertyInfo7.GetValue(object_5);
													if (obj4 != null)
													{
														goto IL_0507;
													}
												}
												obj4 = 0;
												goto IL_0507;
												IL_05e8:
												gparam_8 = Math.Round(Convert.ToDouble(obj10), 2);
												PropertyInfo propertyInfo8 = propertyInfo_9;
												if ((object)propertyInfo8 == null)
												{
													obj9 = null;
												}
												else
												{
													obj9 = propertyInfo8.GetValue(object_5);
													if (obj9 != null)
													{
														goto IL_0615;
													}
												}
												obj9 = 0;
												goto IL_0615;
												end_IL_0335:;
											}
											catch
											{
											}
											object_5 = null;
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
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(48, 1);
									defaultInterpolatedStringHandler2.AppendLiteral("[CadLayerManagerTool] GetLines: 从 CAD 文件获取到 ");
									defaultInterpolatedStringHandler2.AppendFormatted(class355_0.list_0.Count);
									defaultInterpolatedStringHandler2.AppendLiteral(" 条线条");
									Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
									if (aitoolContext_0.DataCache != null && !string.IsNullOrEmpty(aitoolContext_0.SessionId))
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(12, 3);
										defaultInterpolatedStringHandler3.AppendLiteral("cad_lines_");
										defaultInterpolatedStringHandler3.AppendFormatted(int_1);
										defaultInterpolatedStringHandler3.AppendLiteral("_");
										defaultInterpolatedStringHandler3.AppendFormatted(class355_0.class354_0.string_0);
										defaultInterpolatedStringHandler3.AppendLiteral("_");
										defaultInterpolatedStringHandler3.AppendFormatted(DateTime.Now.Ticks);
										string_1 = defaultInterpolatedStringHandler3.ToStringAndClear();
										aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)class355_0.list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_1, "条线条", 100);
										cadLayerManagerTool_0.method_9(aitoolResult_0, delegate(string string_0)
										{
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(51, 3);
											defaultInterpolatedStringHandler8.AppendLiteral("从图层 '");
											defaultInterpolatedStringHandler8.AppendFormatted(class355_0.class354_0.string_0);
											defaultInterpolatedStringHandler8.AppendLiteral("' 获取到 ");
											defaultInterpolatedStringHandler8.AppendFormatted(class355_0.list_0.Count);
											defaultInterpolatedStringHandler8.AppendLiteral(" 条线条\n\n💡 在后续工具调用中使用 cacheId=\"");
											defaultInterpolatedStringHandler8.AppendFormatted(string_0);
											defaultInterpolatedStringHandler8.AppendLiteral("\" 参数来操作这些线条");
											return defaultInterpolatedStringHandler8.ToStringAndClear();
										});
										result = aitoolResult_0;
									}
									else
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(15, 2);
										defaultInterpolatedStringHandler4.AppendLiteral("从图层 '");
										defaultInterpolatedStringHandler4.AppendFormatted(class355_0.class354_0.string_0);
										defaultInterpolatedStringHandler4.AppendLiteral("' 获取到 ");
										defaultInterpolatedStringHandler4.AppendFormatted(class355_0.list_0.Count);
										defaultInterpolatedStringHandler4.AppendLiteral(" 条线条");
										result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class10<string, int, string, string, int, List<object>>("get_lines", int_1, class355_0.class354_0.string_0, "CAD 文件", class355_0.list_0.Count, class355_0.list_0));
									}
									goto IL_1220;
								}
								methodInfo_2 = null;
								ienumerable_1 = null;
							}
							methodInfo_1 = null;
							object_4 = null;
						}
						catch (Exception ex)
						{
							exception_0 = ex;
							Logger.Warning("[CadLayerManagerTool] GetLines: 从 CAD 文件获取线条失败: " + exception_0.Message + "，尝试从几何对象获取");
						}
					}
					propertyInfo_0 = null;
					string_0 = null;
				}
				Logger.Info("[CadLayerManagerTool] GetLines: 从几何对象获取线条信息");
				methodInfo_0 = object_2.GetType().GetMethod("GetGeometryObjects");
				ienumerable_0 = methodInfo_0?.Invoke(object_2, new object[1] { object_0 }) as IEnumerable;
				if (ienumerable_0 != null)
				{
					methodInfo_3 = object_2.GetType().GetMethod("FilterByLayer");
					ienumerable_2 = methodInfo_3?.Invoke(object_2, new object[3] { ienumerable_0, class354_0.string_0, aitoolContext_0.Document }) as IEnumerable;
					if (ienumerable_2 != null)
					{
						class356_0 = new Class356();
						class356_0.list_0 = new List<object>();
						methodInfo_4 = object_2.GetType().GetMethod("GetGeometryObjectType");
						methodInfo_5 = object_2.GetType().GetMethod("GetCurveEndpoints");
						ienumerator_1 = ienumerable_2.GetEnumerator();
						try
						{
							while (ienumerator_1.MoveNext())
							{
								object_6 = ienumerator_1.Current;
								try
								{
									string_2 = methodInfo_4?.Invoke(object_2, new object[1] { object_6 })?.ToString();
									object obj12;
									if (string_2 == "Line" || string_2 == "Arc" || string_2 == "Curve")
									{
										object_7 = methodInfo_5?.Invoke(object_2, new object[1] { object_6 });
										if (object_7 != null)
										{
											propertyInfo_12 = object_7.GetType().GetProperty("start");
											propertyInfo_13 = object_7.GetType().GetProperty("end");
											object_8 = propertyInfo_12?.GetValue(object_7);
											object_9 = propertyInfo_13?.GetValue(object_7);
											if (object_8 != null && object_9 != null)
											{
												propertyInfo_14 = object_8.GetType().GetProperty("X");
												propertyInfo_15 = object_8.GetType().GetProperty("Y");
												propertyInfo_16 = object_8.GetType().GetProperty("Z");
												PropertyInfo propertyInfo9 = propertyInfo_14;
												if ((object)propertyInfo9 == null)
												{
													obj12 = null;
												}
												else
												{
													obj12 = propertyInfo9.GetValue(object_8);
													if (obj12 != null)
													{
														goto IL_0cf2;
													}
												}
												obj12 = 0;
												goto IL_0cf2;
											}
											goto IL_0f49;
										}
										goto IL_0f65;
									}
									goto IL_0f6c;
									IL_0d4c:
									object obj13;
									double_2 = Convert.ToDouble(obj13);
									PropertyInfo propertyInfo10 = propertyInfo_14;
									object obj14;
									if ((object)propertyInfo10 == null)
									{
										obj14 = null;
									}
									else
									{
										obj14 = propertyInfo10.GetValue(object_9);
										if (obj14 != null)
										{
											goto IL_0d79;
										}
									}
									obj14 = 0;
									goto IL_0d79;
									IL_0dd3:
									object obj15;
									double_5 = Convert.ToDouble(obj15);
									double_6 = (double_3 - double_0) * 304.8;
									double_7 = (double_4 - double_1) * 304.8;
									double_8 = Math.Sqrt(double_6 * double_6 + double_7 * double_7);
									double_9 = Math.Atan2(double_7, double_6) * 180.0 / Math.PI;
									class356_0.list_0.Add(new Class11<double, double, double, double, double, double, double, double, string, string, string>(Math.Round(double_0 * 304.8, 2), Math.Round(double_1 * 304.8, 2), Math.Round(double_2 * 304.8, 2), Math.Round(double_3 * 304.8, 2), Math.Round(double_4 * 304.8, 2), Math.Round(double_5 * 304.8, 2), Math.Round(double_8, 2), Math.Round(double_9, 2), string_2, class354_0.string_0, "毫米"));
									propertyInfo_14 = null;
									propertyInfo_15 = null;
									propertyInfo_16 = null;
									goto IL_0f49;
									IL_0cf2:
									double_0 = Convert.ToDouble(obj12);
									PropertyInfo propertyInfo11 = propertyInfo_15;
									object obj16;
									if ((object)propertyInfo11 == null)
									{
										obj16 = null;
									}
									else
									{
										obj16 = propertyInfo11.GetValue(object_8);
										if (obj16 != null)
										{
											goto IL_0d1f;
										}
									}
									obj16 = 0;
									goto IL_0d1f;
									IL_0f65:
									object_7 = null;
									goto IL_0f6c;
									IL_0d1f:
									double_1 = Convert.ToDouble(obj16);
									PropertyInfo propertyInfo12 = propertyInfo_16;
									if ((object)propertyInfo12 == null)
									{
										obj13 = null;
									}
									else
									{
										obj13 = propertyInfo12.GetValue(object_8);
										if (obj13 != null)
										{
											goto IL_0d4c;
										}
									}
									obj13 = 0;
									goto IL_0d4c;
									IL_0f6c:
									string_2 = null;
									goto end_IL_0b1b;
									IL_0da6:
									object obj17;
									double_4 = Convert.ToDouble(obj17);
									PropertyInfo propertyInfo13 = propertyInfo_16;
									if ((object)propertyInfo13 == null)
									{
										obj15 = null;
									}
									else
									{
										obj15 = propertyInfo13.GetValue(object_9);
										if (obj15 != null)
										{
											goto IL_0dd3;
										}
									}
									obj15 = 0;
									goto IL_0dd3;
									IL_0d79:
									double_3 = Convert.ToDouble(obj14);
									PropertyInfo propertyInfo14 = propertyInfo_15;
									if ((object)propertyInfo14 == null)
									{
										obj17 = null;
									}
									else
									{
										obj17 = propertyInfo14.GetValue(object_9);
										if (obj17 != null)
										{
											goto IL_0da6;
										}
									}
									obj17 = 0;
									goto IL_0da6;
									IL_0f49:
									propertyInfo_12 = null;
									propertyInfo_13 = null;
									object_8 = null;
									object_9 = null;
									goto IL_0f65;
									end_IL_0b1b:;
								}
								catch
								{
								}
								object_6 = null;
							}
						}
						finally
						{
							if (num < 0 && ienumerator_1 is IDisposable disposable2)
							{
								disposable2.Dispose();
							}
						}
						ienumerator_1 = null;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(45, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("[CadLayerManagerTool] GetLines: 从几何对象获取到 ");
						defaultInterpolatedStringHandler5.AppendFormatted(class356_0.list_0.Count);
						defaultInterpolatedStringHandler5.AppendLiteral(" 条线条");
						Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
						if (class356_0.list_0.Count == 0)
						{
							result = AIToolResult.Fail("无法从图层 '" + class354_0.string_0 + "' 获取线条信息");
						}
						else if (aitoolContext_0.DataCache != null && !string.IsNullOrEmpty(aitoolContext_0.SessionId))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(17, 3);
							defaultInterpolatedStringHandler6.AppendLiteral("cad_geom_lines_");
							defaultInterpolatedStringHandler6.AppendFormatted(int_1);
							defaultInterpolatedStringHandler6.AppendLiteral("_");
							defaultInterpolatedStringHandler6.AppendFormatted(class354_0.string_0);
							defaultInterpolatedStringHandler6.AppendLiteral("_");
							defaultInterpolatedStringHandler6.AppendFormatted(DateTime.Now.Ticks);
							string_3 = defaultInterpolatedStringHandler6.ToStringAndClear();
							aitoolResult_1 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)class356_0.list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_3, "条线条", 100);
							cadLayerManagerTool_0.method_9(aitoolResult_1, delegate(string string_0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(49, 2);
								defaultInterpolatedStringHandler8.AppendLiteral("从几何对象获取到 ");
								defaultInterpolatedStringHandler8.AppendFormatted(class356_0.list_0.Count);
								defaultInterpolatedStringHandler8.AppendLiteral(" 条线条\n\n💡 在后续工具调用中使用 cacheId=\"");
								defaultInterpolatedStringHandler8.AppendFormatted(string_0);
								defaultInterpolatedStringHandler8.AppendLiteral("\" 参数来操作这些线条");
								return defaultInterpolatedStringHandler8.ToStringAndClear();
							});
							result = aitoolResult_1;
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler7.AppendLiteral("从几何对象获取到 ");
							defaultInterpolatedStringHandler7.AppendFormatted(class356_0.list_0.Count);
							defaultInterpolatedStringHandler7.AppendLiteral(" 条线条");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler7.ToStringAndClear(), (object)new Class10<string, int, string, string, int, List<object>>("get_lines", int_1, class354_0.string_0, "几何对象", class356_0.list_0.Count, class356_0.list_0));
						}
						goto IL_1220;
					}
					methodInfo_3 = null;
					ienumerable_2 = null;
				}
				result = AIToolResult.Fail("无法从图层 '" + class354_0.string_0 + "' 获取线条信息");
			}
			goto IL_1220;
			IL_1220:
			int_0 = -2;
			class354_0 = null;
			object_3 = null;
			methodInfo_0 = null;
			ienumerable_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	private sealed class Class367 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public object object_0;

		public int int_1;

		public object object_1;

		public object object_2;

		public AIToolContext aitoolContext_0;

		public CadLayerManagerTool cadLayerManagerTool_0;

		private Class357 class357_0;

		private object object_3;

		private PropertyInfo propertyInfo_0;

		private string string_0;

		private MethodInfo methodInfo_0;

		private object object_4;

		private MethodInfo methodInfo_1;

		private IEnumerable ienumerable_0;

		private Class358 class358_0;

		private IEnumerator ienumerator_0;

		private object object_5;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private PropertyInfo propertyInfo_3;

		private PropertyInfo propertyInfo_4;

		private PropertyInfo propertyInfo_5;

		private PropertyInfo propertyInfo_6;

		private PropertyInfo propertyInfo_7;

		private PropertyInfo propertyInfo_8;

		private PropertyInfo propertyInfo_9;

		private PropertyInfo propertyInfo_10;

		private PropertyInfo propertyInfo_11;

		private string string_1;

		private AIToolResult aitoolResult_0;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				class357_0 = new Class357();
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class367 stateMachine = this;
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
			class357_0.string_0 = aitoolContext_0.GetParameter<string>("layerName", (string)null);
			AIToolResult result;
			if (string.IsNullOrWhiteSpace(class357_0.string_0))
			{
				result = AIToolResult.Fail("get_texts 操作需要指定 layerName 参数");
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[CadLayerManagerTool] GetTexts: 开始获取 CAD 链接 ");
				defaultInterpolatedStringHandler.AppendFormatted(int_1);
				defaultInterpolatedStringHandler.AppendLiteral(" 图层 '");
				defaultInterpolatedStringHandler.AppendFormatted(class357_0.string_0);
				defaultInterpolatedStringHandler.AppendLiteral("' 的文字信息");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				object_3 = object_1.GetType().GetMethod("GetCADFilePath")?.Invoke(object_1, new object[2] { object_0, aitoolContext_0.Document });
				if (object_3 != null)
				{
					propertyInfo_0 = object_3.GetType().GetProperty("FilePath");
					string_0 = propertyInfo_0?.GetValue(object_3)?.ToString();
					if (!string.IsNullOrEmpty(string_0) && File.Exists(string_0))
					{
						try
						{
							Logger.Info("[CadLayerManagerTool] GetTexts: 从 CAD 文件获取文字: " + string_0);
							methodInfo_0 = object_1.GetType().GetMethod("ParseImportInstance");
							object_4 = methodInfo_0?.Invoke(object_1, new object[2] { object_0, aitoolContext_0.Document });
							if (object_4 != null)
							{
								methodInfo_1 = object_4.GetType().GetMethod("GetTextsByLayer");
								MethodInfo methodInfo = methodInfo_1;
								object obj;
								if ((object)methodInfo == null)
								{
									obj = null;
								}
								else
								{
									object obj2 = object_4;
									object[] parameters = new string[1] { class357_0.string_0 };
									obj = methodInfo.Invoke(obj2, parameters);
								}
								ienumerable_0 = obj as IEnumerable;
								if (ienumerable_0 != null)
								{
									class358_0 = new Class358();
									class358_0.class357_0 = class357_0;
									class358_0.list_0 = new List<object>();
									ienumerator_0 = ienumerable_0.GetEnumerator();
									try
									{
										while (ienumerator_0.MoveNext())
										{
											object_5 = ienumerator_0.Current;
											try
											{
												propertyInfo_1 = object_5.GetType().GetProperty("Id");
												propertyInfo_2 = object_5.GetType().GetProperty("Content");
												propertyInfo_3 = object_5.GetType().GetProperty("X");
												propertyInfo_4 = object_5.GetType().GetProperty("Y");
												propertyInfo_5 = object_5.GetType().GetProperty("Z");
												propertyInfo_6 = object_5.GetType().GetProperty("Height");
												propertyInfo_7 = object_5.GetType().GetProperty("Width");
												propertyInfo_8 = object_5.GetType().GetProperty("Rotation");
												propertyInfo_9 = object_5.GetType().GetProperty("HorizontalAlignment");
												propertyInfo_10 = object_5.GetType().GetProperty("VerticalAlignment");
												propertyInfo_11 = object_5.GetType().GetProperty("LayerName");
												List<object> list = class358_0.list_0;
												object? gparam_ = propertyInfo_1?.GetValue(object_5);
												string? gparam_2 = propertyInfo_2?.GetValue(object_5)?.ToString();
												PropertyInfo propertyInfo = propertyInfo_3;
												object obj3;
												if ((object)propertyInfo == null)
												{
													obj3 = null;
												}
												else
												{
													obj3 = propertyInfo.GetValue(object_5);
													if (obj3 != null)
													{
														goto IL_04fe;
													}
												}
												obj3 = 0;
												goto IL_04fe;
												IL_0558:
												double gparam_3;
												double gparam_4;
												object obj4;
												list.Add(new Class12<object, string, double, double, double, double?, double?, double?, string, string, string, string>(gparam_, gparam_2, gparam_3, gparam_4, Math.Round(Convert.ToDouble(obj4), 2), (propertyInfo_6?.GetValue(object_5) != null) ? new double?(Math.Round(Convert.ToDouble(propertyInfo_6.GetValue(object_5)), 2)) : ((double?)null), (propertyInfo_7?.GetValue(object_5) != null) ? new double?(Math.Round(Convert.ToDouble(propertyInfo_7.GetValue(object_5)), 2)) : ((double?)null), (propertyInfo_8?.GetValue(object_5) != null) ? new double?(Math.Round(Convert.ToDouble(propertyInfo_8.GetValue(object_5)) * 180.0 / Math.PI, 2)) : ((double?)null), propertyInfo_9?.GetValue(object_5)?.ToString(), propertyInfo_10?.GetValue(object_5)?.ToString(), propertyInfo_11?.GetValue(object_5)?.ToString(), "毫米"));
												propertyInfo_1 = null;
												propertyInfo_2 = null;
												propertyInfo_3 = null;
												propertyInfo_4 = null;
												propertyInfo_5 = null;
												propertyInfo_6 = null;
												propertyInfo_7 = null;
												propertyInfo_8 = null;
												propertyInfo_9 = null;
												propertyInfo_10 = null;
												propertyInfo_11 = null;
												goto end_IL_0335;
												IL_04fe:
												gparam_3 = Math.Round(Convert.ToDouble(obj3), 2);
												PropertyInfo propertyInfo2 = propertyInfo_4;
												object obj5;
												if ((object)propertyInfo2 == null)
												{
													obj5 = null;
												}
												else
												{
													obj5 = propertyInfo2.GetValue(object_5);
													if (obj5 != null)
													{
														goto IL_052b;
													}
												}
												obj5 = 0;
												goto IL_052b;
												IL_052b:
												gparam_4 = Math.Round(Convert.ToDouble(obj5), 2);
												PropertyInfo propertyInfo3 = propertyInfo_5;
												if ((object)propertyInfo3 == null)
												{
													obj4 = null;
												}
												else
												{
													obj4 = propertyInfo3.GetValue(object_5);
													if (obj4 != null)
													{
														goto IL_0558;
													}
												}
												obj4 = 0;
												goto IL_0558;
												end_IL_0335:;
											}
											catch
											{
											}
											object_5 = null;
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
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(48, 1);
									defaultInterpolatedStringHandler2.AppendLiteral("[CadLayerManagerTool] GetTexts: 从 CAD 文件获取到 ");
									defaultInterpolatedStringHandler2.AppendFormatted(class358_0.list_0.Count);
									defaultInterpolatedStringHandler2.AppendLiteral(" 个文字");
									Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
									if (aitoolContext_0.DataCache != null && !string.IsNullOrEmpty(aitoolContext_0.SessionId))
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(12, 3);
										defaultInterpolatedStringHandler3.AppendLiteral("cad_texts_");
										defaultInterpolatedStringHandler3.AppendFormatted(int_1);
										defaultInterpolatedStringHandler3.AppendLiteral("_");
										defaultInterpolatedStringHandler3.AppendFormatted(class358_0.class357_0.string_0);
										defaultInterpolatedStringHandler3.AppendLiteral("_");
										defaultInterpolatedStringHandler3.AppendFormatted(DateTime.Now.Ticks);
										string_1 = defaultInterpolatedStringHandler3.ToStringAndClear();
										aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)class358_0.list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_1, "个文字", 100);
										cadLayerManagerTool_0.method_9(aitoolResult_0, delegate(string string_0)
										{
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(51, 3);
											defaultInterpolatedStringHandler5.AppendLiteral("从图层 '");
											defaultInterpolatedStringHandler5.AppendFormatted(class358_0.class357_0.string_0);
											defaultInterpolatedStringHandler5.AppendLiteral("' 获取到 ");
											defaultInterpolatedStringHandler5.AppendFormatted(class358_0.list_0.Count);
											defaultInterpolatedStringHandler5.AppendLiteral(" 个文字\n\n💡 在后续工具调用中使用 cacheId=\"");
											defaultInterpolatedStringHandler5.AppendFormatted(string_0);
											defaultInterpolatedStringHandler5.AppendLiteral("\" 参数来操作这些文字");
											return defaultInterpolatedStringHandler5.ToStringAndClear();
										});
										result = aitoolResult_0;
									}
									else
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(15, 2);
										defaultInterpolatedStringHandler4.AppendLiteral("从图层 '");
										defaultInterpolatedStringHandler4.AppendFormatted(class358_0.class357_0.string_0);
										defaultInterpolatedStringHandler4.AppendLiteral("' 获取到 ");
										defaultInterpolatedStringHandler4.AppendFormatted(class358_0.list_0.Count);
										defaultInterpolatedStringHandler4.AppendLiteral(" 个文字");
										result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class13<string, int, string, string, int, List<object>>("get_texts", int_1, class358_0.class357_0.string_0, "CAD 文件", class358_0.list_0.Count, class358_0.list_0));
									}
									goto IL_0a0f;
								}
								methodInfo_1 = null;
								ienumerable_0 = null;
							}
							methodInfo_0 = null;
							object_4 = null;
						}
						catch (Exception ex)
						{
							exception_0 = ex;
							Logger.Warning("[CadLayerManagerTool] GetTexts: 从 CAD 文件获取文字失败: " + exception_0.Message);
						}
					}
					propertyInfo_0 = null;
					string_0 = null;
				}
				Logger.Warning("[CadLayerManagerTool] GetTexts: 无法从 CAD 文件获取文字，几何对象通常不包含文字信息");
				result = AIToolResult.Fail("无法从图层 '" + class357_0.string_0 + "' 获取文字信息。💡 解决方案：请确保 CAD 文件是通过链接方式导入，并且原始 CAD 文件在可访问的位置");
			}
			goto IL_0a0f;
			IL_0a0f:
			int_0 = -2;
			class357_0 = null;
			object_3 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "cad_layer_manager";

	public string Category => "CAD 图纸识别";

	public string Description => "管理 CAD 图层的查询、分析和内容提取";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"operation\": {\n                \"type\": \"string\",\n                \"enum\": [\"get_layers\", \"get_lines\", \"get_texts\", \"get_blocks\", \"analyze\"],\n                \"description\": \"图层操作类型：get_layers(获取所有图层信息)、get_lines(获取指定图层的线条)、get_texts(获取指定图层的文字)、get_blocks(获取指定图层的图块)、analyze(分析图层内容统计)\"\n            },\n            \"linkId\": {\n                \"type\": \"integer\",\n                \"description\": \"CAD 链接的元素 ID（所有操作必需）\"\n            },\n            \"layerName\": {\n                \"type\": \"string\",\n                \"description\": \"图层名称（get_lines、get_texts、get_blocks 操作必需）。从 get_layers 或 analyze 操作的结果中获取。\"\n            }\n        },\n        \"required\": [\"operation\", \"linkId\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class363))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class363 stateMachine = new Class363();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.cadLayerManagerTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class365))]
	private Task<AIToolResult> method_0(object object_0, int int_0, object object_1, object object_2, AIToolContext aitoolContext_0)
	{
		Class365 stateMachine = new Class365();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.cadLayerManagerTool_0 = this;
		stateMachine.object_0 = object_0;
		stateMachine.int_1 = int_0;
		stateMachine.object_1 = object_1;
		stateMachine.object_2 = object_2;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class366))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_1(object object_0, int int_0, object object_1, object object_2, AIToolContext aitoolContext_0)
	{
		Class366 stateMachine = new Class366();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.cadLayerManagerTool_0 = this;
		stateMachine.object_0 = object_0;
		stateMachine.int_1 = int_0;
		stateMachine.object_1 = object_1;
		stateMachine.object_2 = object_2;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class367))]
	private Task<AIToolResult> method_2(object object_0, int int_0, object object_1, object object_2, AIToolContext aitoolContext_0)
	{
		Class367 stateMachine = new Class367();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.cadLayerManagerTool_0 = this;
		stateMachine.object_0 = object_0;
		stateMachine.int_1 = int_0;
		stateMachine.object_1 = object_1;
		stateMachine.object_2 = object_2;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Class364))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_3(object object_0, int int_0, object object_1, object object_2, AIToolContext aitoolContext_0)
	{
		Class364 stateMachine = new Class364();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.cadLayerManagerTool_0 = this;
		stateMachine.object_0 = object_0;
		stateMachine.int_1 = int_0;
		stateMachine.object_1 = object_1;
		stateMachine.object_2 = object_2;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class362))]
	private Task<AIToolResult> method_4(object object_0, int int_0, object object_1, object object_2, AIToolContext aitoolContext_0)
	{
		Class362 stateMachine = new Class362();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.cadLayerManagerTool_0 = this;
		stateMachine.object_0 = object_0;
		stateMachine.int_1 = int_0;
		stateMachine.object_1 = object_1;
		stateMachine.object_2 = object_2;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private object method_5(object object_0, int int_0, string string_0)
	{
		try
		{
			IEnumerable enumerable = object_0.GetType().GetProperty("Layers")?.GetValue(object_0) as IEnumerable;
			PropertyInfo property = object_0.GetType().GetProperty("Blocks");
			PropertyInfo property2 = object_0.GetType().GetProperty("Lines");
			PropertyInfo property3 = object_0.GetType().GetProperty("Texts");
			IEnumerable enumerable2 = property?.GetValue(object_0) as IEnumerable;
			IEnumerable enumerable3 = property2?.GetValue(object_0) as IEnumerable;
			IEnumerable enumerable4 = property3?.GetValue(object_0) as IEnumerable;
			Dictionary<string, LayerStatistics> dictionary = new Dictionary<string, LayerStatistics>();
			if (enumerable2 != null)
			{
				foreach (object item in enumerable2)
				{
					try
					{
						PropertyInfo property4 = item.GetType().GetProperty("LayerName");
						object obj;
						if ((object)property4 == null)
						{
							obj = null;
						}
						else
						{
							object? value = property4.GetValue(item);
							if (value == null)
							{
								obj = null;
							}
							else
							{
								obj = value.ToString();
								if (obj != null)
								{
									goto IL_0116;
								}
							}
						}
						obj = "未知图层";
						goto IL_0116;
						IL_0116:
						string text = (string)obj;
						if (!dictionary.ContainsKey(text))
						{
							dictionary[text] = new LayerStatistics
							{
								LayerName = text
							};
						}
						dictionary[text].BlockCount++;
					}
					catch
					{
					}
				}
			}
			if (enumerable3 != null)
			{
				foreach (object item2 in enumerable3)
				{
					try
					{
						PropertyInfo property5 = item2.GetType().GetProperty("LayerName");
						object obj3;
						if ((object)property5 == null)
						{
							obj3 = null;
						}
						else
						{
							object? value2 = property5.GetValue(item2);
							if (value2 == null)
							{
								obj3 = null;
							}
							else
							{
								obj3 = value2.ToString();
								if (obj3 != null)
								{
									goto IL_01e2;
								}
							}
						}
						obj3 = "未知图层";
						goto IL_01e2;
						IL_01e2:
						string text2 = (string)obj3;
						if (!dictionary.ContainsKey(text2))
						{
							dictionary[text2] = new LayerStatistics
							{
								LayerName = text2
							};
						}
						dictionary[text2].LineCount++;
					}
					catch
					{
					}
				}
			}
			if (enumerable4 != null)
			{
				foreach (object item3 in enumerable4)
				{
					try
					{
						PropertyInfo property6 = item3.GetType().GetProperty("LayerName");
						object obj5;
						if ((object)property6 == null)
						{
							obj5 = null;
						}
						else
						{
							object? value3 = property6.GetValue(item3);
							if (value3 == null)
							{
								obj5 = null;
							}
							else
							{
								obj5 = value3.ToString();
								if (obj5 != null)
								{
									goto IL_02ae;
								}
							}
						}
						obj5 = "未知图层";
						goto IL_02ae;
						IL_02ae:
						string text3 = (string)obj5;
						if (!dictionary.ContainsKey(text3))
						{
							dictionary[text3] = new LayerStatistics
							{
								LayerName = text3
							};
						}
						dictionary[text3].TextCount++;
					}
					catch
					{
					}
				}
			}
			if (enumerable != null)
			{
				foreach (object item4 in enumerable)
				{
					try
					{
						string text4 = item4.GetType().GetProperty("Name")?.GetValue(item4)?.ToString();
						if (!string.IsNullOrEmpty(text4))
						{
							if (!dictionary.ContainsKey(text4))
							{
								dictionary[text4] = new LayerStatistics
								{
									LayerName = text4
								};
							}
							PropertyInfo property7 = item4.GetType().GetProperty("ColorIndex");
							PropertyInfo property8 = item4.GetType().GetProperty("IsVisible");
							PropertyInfo property9 = item4.GetType().GetProperty("IsLocked");
							dictionary[text4].ColorIndex = property7?.GetValue(item4) as int?;
							dictionary[text4].IsVisible = property8?.GetValue(item4) as bool?;
							dictionary[text4].IsLocked = property9?.GetValue(item4) as bool?;
						}
					}
					catch
					{
					}
				}
			}
			List<object> list = new List<object>();
			int num = 0;
			int count = dictionary.Count;
			int num2 = 0;
			foreach (KeyValuePair<string, LayerStatistics> item5 in dictionary)
			{
				LayerStatistics value4 = item5.Value;
				value4.TotalCount = value4.BlockCount + value4.LineCount + value4.TextCount;
				if (value4.TotalCount > 0)
				{
					num2++;
					num += value4.TotalCount;
				}
				list.Add(new Class16<string, int, int, int, int, bool, int?, bool, bool, List<string>>(value4.LayerName, value4.BlockCount, value4.LineCount, value4.TextCount, value4.TotalCount, value4.TotalCount == 0, value4.ColorIndex, value4.IsVisible ?? true, value4.IsLocked == true, method_7(value4)));
			}
			List<object> gparam_ = list.OrderByDescending((object obj8) => ((int?)obj8.GetType().GetProperty("totalCount")?.GetValue(obj8)).GetValueOrDefault()).ToList();
			return new Class17<string, int, string, Class18<int, int, int, int, int, int, int>, List<object>>("analyze", int_0, string_0, new Class18<int, int, int, int, int, int, int>(count, num2, count - num2, num, dictionary.Values.Sum((LayerStatistics layerStatistics_0) => layerStatistics_0.BlockCount), dictionary.Values.Sum((LayerStatistics layerStatistics_0) => layerStatistics_0.LineCount), dictionary.Values.Sum((LayerStatistics layerStatistics_0) => layerStatistics_0.TextCount)), gparam_);
		}
		catch (Exception ex)
		{
			Logger.Error("[CadLayerManagerTool] Analyze: 分析 CAD 数据失败: " + ex.Message);
			return new Class19<string, int, string, string>("analyze", int_0, string_0, "分析失败: " + ex.Message);
		}
	}

	private object method_6(IEnumerable ienumerable_0, object object_0, object object_1, int int_0)
	{
		try
		{
			Dictionary<string, LayerStatistics> dictionary = new Dictionary<string, LayerStatistics>();
			List<object> list = new List<object>();
			foreach (object item in ienumerable_0)
			{
				list.Add(item);
			}
			MethodInfo method = object_0.GetType().GetMethod("GetGeometryObjectLayer");
			MethodInfo method2 = object_0.GetType().GetMethod("GetGeometryObjectType");
			foreach (object item2 in list)
			{
				try
				{
					string text = method?.Invoke(object_0, new object[2] { item2, object_1 })?.ToString();
					if (string.IsNullOrEmpty(text))
					{
						text = "未知图层";
					}
					if (!dictionary.ContainsKey(text))
					{
						dictionary[text] = new LayerStatistics
						{
							LayerName = text
						};
					}
					string text2 = method2?.Invoke(object_0, new object[1] { item2 })?.ToString();
					if (text2 == "Block")
					{
						dictionary[text].BlockCount++;
					}
					else if (text2 == "Line" || text2 == "Arc" || text2 == "Curve")
					{
						dictionary[text].LineCount++;
					}
					dictionary[text].TotalCount++;
				}
				catch
				{
				}
			}
			List<object> list2 = new List<object>();
			int num = 0;
			int count = dictionary.Count;
			int num2 = 0;
			foreach (KeyValuePair<string, LayerStatistics> item3 in dictionary)
			{
				LayerStatistics value = item3.Value;
				if (value.TotalCount > 0)
				{
					num2++;
					num += value.TotalCount;
				}
				list2.Add(new Class20<string, int, int, int, int, bool, int?, bool, bool, List<string>, string>(value.LayerName, value.BlockCount, value.LineCount, value.TextCount, value.TotalCount, value.TotalCount == 0, null, gparam_18: true, gparam_19: false, method_7(value), "几何对象不包含文字信息"));
			}
			List<object> gparam_ = list2.OrderByDescending((object obj2) => ((int?)obj2.GetType().GetProperty("totalCount")?.GetValue(obj2)).GetValueOrDefault()).ToList();
			return new Class17<string, int, string, Class21<int, int, int, int, int, int, int, string>, List<object>>("analyze", int_0, "几何对象", new Class21<int, int, int, int, int, int, int, string>(count, num2, count - num2, num, dictionary.Values.Sum((LayerStatistics layerStatistics_0) => layerStatistics_0.BlockCount), dictionary.Values.Sum((LayerStatistics layerStatistics_0) => layerStatistics_0.LineCount), dictionary.Values.Sum((LayerStatistics layerStatistics_0) => layerStatistics_0.TextCount), "几何对象不包含文字信息"), gparam_);
		}
		catch (Exception ex)
		{
			Logger.Error("[CadLayerManagerTool] Analyze: 分析几何对象失败: " + ex.Message);
			return new Class19<string, int, string, string>("analyze", int_0, "几何对象", "分析失败: " + ex.Message);
		}
	}

	private List<string> method_7(LayerStatistics layerStatistics_0)
	{
		List<string> list = new List<string>();
		if (layerStatistics_0.BlockCount > 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
			defaultInterpolatedStringHandler.AppendLiteral("图块(");
			defaultInterpolatedStringHandler.AppendFormatted(layerStatistics_0.BlockCount);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		if (layerStatistics_0.LineCount > 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("线条(");
			defaultInterpolatedStringHandler2.AppendFormatted(layerStatistics_0.LineCount);
			defaultInterpolatedStringHandler2.AppendLiteral(")");
			list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
		}
		if (layerStatistics_0.TextCount > 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(4, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("文字(");
			defaultInterpolatedStringHandler3.AppendFormatted(layerStatistics_0.TextCount);
			defaultInterpolatedStringHandler3.AppendLiteral(")");
			list.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
		}
		return list;
	}

	private string method_8(object object_0)
	{
		try
		{
			PropertyInfo property = object_0.GetType().GetProperty("summary");
			object value;
			PropertyInfo property3;
			PropertyInfo property4;
			object obj;
			if (property != null)
			{
				value = property.GetValue(object_0);
				if (value != null)
				{
					PropertyInfo property2 = value.GetType().GetProperty("totalLayers");
					property3 = value.GetType().GetProperty("nonEmptyLayers");
					property4 = value.GetType().GetProperty("totalObjects");
					if ((object)property2 == null)
					{
						obj = null;
					}
					else
					{
						object? value2 = property2.GetValue(value);
						if (value2 == null)
						{
							obj = null;
						}
						else
						{
							obj = value2.ToString();
							if (obj != null)
							{
								goto IL_009e;
							}
						}
					}
					obj = "?";
					goto IL_009e;
				}
			}
			goto end_IL_0001;
			IL_00f2:
			object obj2;
			string value3 = (string)obj2;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 3);
			defaultInterpolatedStringHandler.AppendLiteral("已分析 ");
			string value4;
			defaultInterpolatedStringHandler.AppendFormatted(value4);
			defaultInterpolatedStringHandler.AppendLiteral(" 个图层，其中 ");
			string value5;
			defaultInterpolatedStringHandler.AppendFormatted(value5);
			defaultInterpolatedStringHandler.AppendLiteral(" 个包含内容，共识别 ");
			defaultInterpolatedStringHandler.AppendFormatted(value3);
			defaultInterpolatedStringHandler.AppendLiteral(" 个对象");
			return defaultInterpolatedStringHandler.ToStringAndClear();
			IL_00c7:
			object obj3;
			value5 = (string)obj3;
			if ((object)property4 == null)
			{
				obj2 = null;
			}
			else
			{
				object? value6 = property4.GetValue(value);
				if (value6 == null)
				{
					obj2 = null;
				}
				else
				{
					obj2 = value6.ToString();
					if (obj2 != null)
					{
						goto IL_00f2;
					}
				}
			}
			obj2 = "?";
			goto IL_00f2;
			IL_009e:
			value4 = (string)obj;
			if ((object)property3 == null)
			{
				obj3 = null;
			}
			else
			{
				object? value7 = property3.GetValue(value);
				if (value7 == null)
				{
					obj3 = null;
				}
				else
				{
					obj3 = value7.ToString();
					if (obj3 != null)
					{
						goto IL_00c7;
					}
				}
			}
			obj3 = "?";
			goto IL_00c7;
			end_IL_0001:;
		}
		catch
		{
		}
		return "CAD 图层分析完成";
	}

	private void method_9(AIToolResult aitoolResult_0, Func<string, string> func_0)
	{
		try
		{
			if (aitoolResult_0.Data == null)
			{
				return;
			}
			PropertyInfo property = aitoolResult_0.Data.GetType().GetProperty("cache_info");
			if (!(property != null))
			{
				return;
			}
			object value = property.GetValue(aitoolResult_0.Data);
			if (value == null)
			{
				return;
			}
			PropertyInfo property2 = value.GetType().GetProperty("cache_id");
			if (property2 != null)
			{
				string text = property2.GetValue(value)?.ToString();
				if (!string.IsNullOrEmpty(text))
				{
					aitoolResult_0.Message = func_0(text);
				}
			}
		}
		catch
		{
		}
	}
}
