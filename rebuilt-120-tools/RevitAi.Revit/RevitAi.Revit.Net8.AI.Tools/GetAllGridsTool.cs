using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using Microsoft.CSharp.RuntimeBinder;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_all_grids", Category = "轴网查询", Description = "获取文档中所有轴网的列表，包括直线网格（A~G轴等）和弧线网格。返回轴网 ID、名称、类型和位置信息", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetAllGridsTool : IAITool
{
	[CompilerGenerated]
	private static class Class481
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, object>> callSite_1;

		public static CallSite<Func<CallSite, object, string, object>> callSite_2;

		public static CallSite<Func<CallSite, object, bool>> callSite_3;

		public static CallSite<Func<CallSite, object, object>> callSite_4;

		public static CallSite<Func<CallSite, object, string, object>> callSite_5;

		public static CallSite<Func<CallSite, object, bool>> callSite_6;
	}

	[CompilerGenerated]
	private sealed class Class482 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetAllGridsTool getAllGridsTool_0;

		private IElementService ielementService_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private List<object> list_1;

		private int int_1;

		private int int_2;

		private IEnumerator<object> ienumerator_0;

		private object object_0;

		private int? nullable_0;

		private string string_0;

		private (double X, double Y, double Z)? nullable_1;

		private string string_1;

		private GridCurveInfo gridCurveInfo_0;

		private string string_2;

		private Class142<int, string, string, string, _003C_003Ef__AnonymousType27<double, double, double>, Class143<string, _003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>, double>> class142_0;

		private string string_3;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_0;

		private object object_1;

		private PropertyInfo propertyInfo_1;

		private string string_4;

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
					Class482 stateMachine = this;
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
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
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
					ienumerable_0 = ielementService_0.GetElementsByCategory(aitoolContext_0.Document, "轴网");
					if (ienumerable_0 == null)
					{
						result = AIToolResult.Fail("无法获取轴网元素");
					}
					else
					{
						list_0 = new List<object>();
						ienumerator_0 = ienumerable_0.GetEnumerator();
						try
						{
							while (ienumerator_0.MoveNext())
							{
								object_0 = ienumerator_0.Current;
								if (object_0 != null)
								{
									nullable_0 = ielementService_0.GetElementId(object_0);
									string_0 = ielementService_0.GetElementName(object_0);
									nullable_1 = ielementService_0.GetElementLocation(object_0);
									string_1 = ielementService_0.GetElementTypeName(object_0);
									gridCurveInfo_0 = ielementService_0.GetGridCurveInfo(object_0);
									string_2 = "直线网格";
									GridCurveInfo obj = gridCurveInfo_0;
									if (((obj != null) ? obj.CurveType : null) == "Arc")
									{
										string_2 = "弧线网格";
									}
									else if (string_1 != null && string_1.Contains("Arc", StringComparison.OrdinalIgnoreCase))
									{
										string_2 = "弧线网格";
									}
									class142_0 = new Class142<int, string, string, string, _003C_003Ef__AnonymousType27<double, double, double>, Class143<string, _003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>, double>>(nullable_0.HasValue ? nullable_0.Value : (-1), string_0 ?? "未命名", string_2, "millimeters", nullable_1.HasValue ? new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(nullable_1.Value.X * 304.8, 0), Math.Round(nullable_1.Value.Y * 304.8, 0), Math.Round(nullable_1.Value.Z * 304.8, 0)) : null, (gridCurveInfo_0 != null) ? new Class143<string, _003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>, double>(gridCurveInfo_0.CurveType, new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(gridCurveInfo_0.StartX * 304.8, 0), Math.Round(gridCurveInfo_0.StartY * 304.8, 0), Math.Round(gridCurveInfo_0.StartZ * 304.8, 0)), new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(gridCurveInfo_0.EndX * 304.8, 0), Math.Round(gridCurveInfo_0.EndY * 304.8, 0), Math.Round(gridCurveInfo_0.EndZ * 304.8, 0)), new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(gridCurveInfo_0.DirectionX, 6), Math.Round(gridCurveInfo_0.DirectionY, 6), Math.Round(gridCurveInfo_0.DirectionZ, 6)), Math.Round(gridCurveInfo_0.Length * 304.8, 0)) : null);
									list_0.Add(class142_0);
									string_0 = null;
									string_1 = null;
									gridCurveInfo_0 = null;
									string_2 = null;
									class142_0 = null;
									object_0 = null;
								}
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
						list_1 = list_0.OrderBy(delegate(object object_0)
						{
							if (Class481.callSite_0 == null)
							{
								Class481.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "name", typeof(GetAllGridsTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							return (dynamic)Class481.callSite_0.Target(Class481.callSite_0, object_0);
						}).ToList();
						int_1 = list_1.Count(delegate(object object_0)
						{
							if (Class481.callSite_1 == null)
							{
								Class481.callSite_1 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "type", typeof(GetAllGridsTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							return (dynamic)Class481.callSite_1.Target(Class481.callSite_1, object_0) == "直线网格";
						});
						int_2 = list_1.Count(delegate(object object_0)
						{
							if (Class481.callSite_4 == null)
							{
								Class481.callSite_4 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "type", typeof(GetAllGridsTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							return (dynamic)Class481.callSite_4.Target(Class481.callSite_4, object_0) == "弧线网格";
						});
						if (!string.IsNullOrEmpty(aitoolContext_0.SessionId) && aitoolContext_0.DataCache != null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
							defaultInterpolatedStringHandler.AppendLiteral("all_grids_");
							defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
							string_3 = defaultInterpolatedStringHandler.ToStringAndClear();
							aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_1, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_3, "个轴网", 200);
							if (aitoolResult_0.Data != null)
							{
								try
								{
									propertyInfo_0 = aitoolResult_0.Data.GetType().GetProperty("cache_info");
									if (propertyInfo_0 != null)
									{
										object_1 = propertyInfo_0.GetValue(aitoolResult_0.Data);
										if (object_1 != null)
										{
											propertyInfo_1 = object_1.GetType().GetProperty("cache_id");
											if (propertyInfo_1 != null)
											{
												string_4 = propertyInfo_1.GetValue(object_1)?.ToString();
												if (!string.IsNullOrEmpty(string_4))
												{
													AIToolResult obj2 = aitoolResult_0;
													DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(63, 4);
													defaultInterpolatedStringHandler2.AppendLiteral("✅ 成功获取 ");
													defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
													defaultInterpolatedStringHandler2.AppendLiteral(" 个轴网（直线网格: ");
													defaultInterpolatedStringHandler2.AppendFormatted(int_1);
													defaultInterpolatedStringHandler2.AppendLiteral(", 弧线网格: ");
													defaultInterpolatedStringHandler2.AppendFormatted(int_2);
													defaultInterpolatedStringHandler2.AppendLiteral("）\n\n💡 在后续工具调用中使用 cacheId=\"");
													defaultInterpolatedStringHandler2.AppendFormatted(string_4);
													defaultInterpolatedStringHandler2.AppendLiteral("\" 参数来操作这些元素");
													obj2.Message = defaultInterpolatedStringHandler2.ToStringAndClear();
												}
												string_4 = null;
											}
											propertyInfo_1 = null;
										}
										object_1 = null;
									}
									propertyInfo_0 = null;
								}
								catch
								{
								}
							}
							result = aitoolResult_0;
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(25, 3);
							defaultInterpolatedStringHandler3.AppendLiteral("成功获取 ");
							defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
							defaultInterpolatedStringHandler3.AppendLiteral(" 个轴网（直线网格: ");
							defaultInterpolatedStringHandler3.AppendFormatted(int_1);
							defaultInterpolatedStringHandler3.AppendLiteral(", 弧线网格: ");
							defaultInterpolatedStringHandler3.AppendFormatted(int_2);
							defaultInterpolatedStringHandler3.AppendLiteral("）");
							result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class144<int, int, int, string, List<object>>(list_1.Count, int_1, int_2, "millimeters", list_1));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取轴网列表失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_all_grids";

	public string Category => "轴网查询";

	public string Description => "获取文档中所有轴网的列表，包括直线网格和弧线网格";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {},\r\n        \"required\": []\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class482))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class482 stateMachine = new Class482();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getAllGridsTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
