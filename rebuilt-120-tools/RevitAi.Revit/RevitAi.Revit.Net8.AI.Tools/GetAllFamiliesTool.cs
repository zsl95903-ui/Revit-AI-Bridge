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

[AITool("get_all_families", Category = "族管理", Description = "获取项目中所有的族（包括系统族和标准族）。支持缓存机制，返回族名称和类别信息", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetAllFamiliesTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class478
	{
		public string string_0;

		internal bool method_0(object object_0)
		{
			string a = object_0?.GetType().GetProperty("category")?.GetValue(object_0)?.ToString();
			return string.Equals(a, string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	private static class Class479
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, object>> callSite_1;
	}

	[CompilerGenerated]
	public sealed class Class480 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetAllFamiliesTool getAllFamiliesTool_0;

		private string string_0;

		private IFamilyService ifamilyService_0;

		private IElementService ielementService_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private IEnumerable<object> ienumerable_1;

		private List<object> list_1;

		private string string_1;

		private IEnumerator<object> ienumerator_0;

		private object object_0;

		private string string_2;

		private string string_3;

		private int? nullable_0;

		private IEnumerable<int> ienumerable_2;

		private int int_1;

		private object object_1;

		private IEnumerator<object> ienumerator_1;

		private object object_2;

		private Class478 class478_0;

		private PropertyInfo[] propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private PropertyInfo propertyInfo_3;

		private object object_3;

		private string string_4;

		private bool bool_0;

		private string string_5;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_4;

		private object object_4;

		private PropertyInfo propertyInfo_5;

		private string string_6;

		private string string_7;

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
					Class480 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("categoryName", (string)null);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ifamilyService_0 = ((revitAdapter != null) ? revitAdapter.FamilyService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				if (ifamilyService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 FamilyService");
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
					ienumerable_0 = ifamilyService_0.GetAllFamilies(aitoolContext_0.Document);
					if (ienumerable_0 == null)
					{
						result = AIToolResult.Fail("无法获取族列表");
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
								if (object_0 == null)
								{
									continue;
								}
								string_2 = ielementService_0.GetElementName(object_0);
								string_3 = null;
								string_3 = ielementService_0.GetElementCategory(object_0);
								if (string.IsNullOrEmpty(string_3))
								{
									try
									{
										IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
										object obj;
										if (revitAdapter3 == null)
										{
											obj = null;
										}
										else
										{
											IFamilyService familyService = revitAdapter3.FamilyService;
											obj = ((familyService != null) ? familyService.GetFamilySymbolIds(object_0) : null);
										}
										ienumerable_2 = (IEnumerable<int>)obj;
										if (ienumerable_2 != null && ienumerable_2.Any())
										{
											int_1 = ienumerable_2.First();
											object_1 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
											if (object_1 != null)
											{
												string_3 = ielementService_0.GetElementCategory(object_1);
											}
											object_1 = null;
										}
										ienumerable_2 = null;
									}
									catch
									{
									}
								}
								if (string.IsNullOrEmpty(string_0) || string.Equals(string_3, string_0, StringComparison.OrdinalIgnoreCase))
								{
									nullable_0 = ielementService_0.GetElementId(object_0);
									list_0.Add(new Class140<int, string, string, string>(nullable_0.HasValue ? nullable_0.Value : (-1), string_2 ?? "未命名", string_3 ?? "未分类", "标准族"));
									string_2 = null;
									string_3 = null;
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
						ienumerable_1 = ifamilyService_0.GetAllSystemElementTypes(aitoolContext_0.Document);
						if (ienumerable_1 != null)
						{
							ienumerator_1 = ienumerable_1.GetEnumerator();
							try
							{
								while (ienumerator_1.MoveNext())
								{
									object_2 = ienumerator_1.Current;
									class478_0 = new Class478();
									propertyInfo_0 = object_2?.GetType().GetProperties();
									if (propertyInfo_0 == null)
									{
										continue;
									}
									propertyInfo_1 = Array.Find(propertyInfo_0, (PropertyInfo propertyInfo_0) => propertyInfo_0.Name.Equals("typeId"));
									propertyInfo_2 = Array.Find(propertyInfo_0, (PropertyInfo propertyInfo_0) => propertyInfo_0.Name.Equals("typeName"));
									propertyInfo_3 = Array.Find(propertyInfo_0, (PropertyInfo propertyInfo_0) => propertyInfo_0.Name.Equals("categoryName"));
									if (!(propertyInfo_1 == null) && !(propertyInfo_2 == null) && !(propertyInfo_3 == null))
									{
										object_3 = propertyInfo_1.GetValue(object_2);
										string_4 = propertyInfo_2.GetValue(object_2)?.ToString();
										class478_0.string_0 = propertyInfo_3.GetValue(object_2)?.ToString();
										bool_0 = list_0.Any(delegate(object object_0)
										{
											string a = object_0?.GetType().GetProperty("category")?.GetValue(object_0)?.ToString();
											return string.Equals(a, class478_0.string_0, StringComparison.OrdinalIgnoreCase);
										});
										if (!bool_0)
										{
											list_0.Add(new Class140<int, string, string, string>(-1, "系统族 - " + class478_0.string_0, class478_0.string_0 ?? "未分类", "系统族"));
										}
										class478_0 = null;
										propertyInfo_0 = null;
										propertyInfo_1 = null;
										propertyInfo_2 = null;
										propertyInfo_3 = null;
										object_3 = null;
										string_4 = null;
										object_2 = null;
									}
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
						}
						list_1 = list_0.OrderBy(delegate(object object_0)
						{
							if (Class479.callSite_0 == null)
							{
								Class479.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "category", typeof(GetAllFamiliesTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							return (dynamic)Class479.callSite_0.Target(Class479.callSite_0, object_0);
						}).ThenBy(delegate(object object_0)
						{
							if (Class479.callSite_1 == null)
							{
								Class479.callSite_1 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "familyName", typeof(GetAllFamiliesTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							return (dynamic)Class479.callSite_1.Target(Class479.callSite_1, object_0);
						}).ToList();
						if (aitoolContext_0.DataCache != null && !string.IsNullOrEmpty(aitoolContext_0.SessionId))
						{
							string_5 = (string.IsNullOrEmpty(string_0) ? "all_families" : ("families_" + string_0));
							aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_1, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_5, "族", 200);
							if (aitoolResult_0.Data != null)
							{
								try
								{
									propertyInfo_4 = aitoolResult_0.Data.GetType().GetProperty("cache_info");
									if (propertyInfo_4 != null)
									{
										object_4 = propertyInfo_4.GetValue(aitoolResult_0.Data);
										if (object_4 != null)
										{
											propertyInfo_5 = object_4.GetType().GetProperty("cache_id");
											if (propertyInfo_5 != null)
											{
												string_6 = propertyInfo_5.GetValue(object_4)?.ToString();
												if (!string.IsNullOrEmpty(string_6))
												{
													string_7 = (string.IsNullOrEmpty(string_0) ? "" : ("类别 '" + string_0 + "' 的 "));
													AIToolResult obj3 = aitoolResult_0;
													DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
													defaultInterpolatedStringHandler.AppendLiteral("✅ 找到 ");
													defaultInterpolatedStringHandler.AppendFormatted(string_7);
													defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
													defaultInterpolatedStringHandler.AppendLiteral(" 个族\n\n💡 在后续工具调用中使用 cacheId=\"");
													defaultInterpolatedStringHandler.AppendFormatted(string_6);
													defaultInterpolatedStringHandler.AppendLiteral("\" 参数来操作这些族");
													obj3.Message = defaultInterpolatedStringHandler.ToStringAndClear();
													string_7 = null;
												}
												string_6 = null;
											}
											propertyInfo_5 = null;
										}
										object_4 = null;
									}
									propertyInfo_4 = null;
								}
								catch
								{
								}
							}
							result = aitoolResult_0;
						}
						else
						{
							string text;
							if (!string.IsNullOrEmpty(string_0))
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("找到类别 '");
								defaultInterpolatedStringHandler2.AppendFormatted(string_0);
								defaultInterpolatedStringHandler2.AppendLiteral("' 的 ");
								defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
								defaultInterpolatedStringHandler2.AppendLiteral(" 个族");
								text = defaultInterpolatedStringHandler2.ToStringAndClear();
							}
							else
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(6, 1);
								defaultInterpolatedStringHandler3.AppendLiteral("找到 ");
								defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
								defaultInterpolatedStringHandler3.AppendLiteral(" 个族");
								text = defaultInterpolatedStringHandler3.ToStringAndClear();
							}
							string_1 = text;
							result = AIToolResult.Ok(string_1, (object)new Class141<int, string, IEnumerable<object>>(list_1.Count, string_0 ?? "(全部)", list_1.Take(200)));
						}
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取族列表失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_all_families";

	public string Category => "族管理";

	public string Description => "获取项目中所有的族（包括系统族和标准族）";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"categoryName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"可选参数：筛选指定类别的族（如 '墙'、'门'、'窗' 等）。如果不提供，则返回所有类别的族\"\r\n            }\r\n        },\r\n        \"required\": []\r\n    }";

	[AsyncStateMachine(typeof(Class480))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class480 stateMachine = new Class480();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getAllFamiliesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
