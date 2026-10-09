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
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_family_types", Category = "族管理", Description = "获取指定族的所有类型定义。注意：必须提供 familyName 参数", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetFamilyTypesTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class500 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetFamilyTypesTool getFamilyTypesTool_0;

		private string string_0;

		private IFamilyService ifamilyService_0;

		private IElementService ielementService_0;

		private IEnumerable<object> ienumerable_0;

		private List<object> list_0;

		private IEnumerable<object> ienumerable_1;

		private IEnumerator<object> ienumerator_0;

		private object object_0;

		private PropertyInfo[] propertyInfo_0;

		private PropertyInfo propertyInfo_1;

		private PropertyInfo propertyInfo_2;

		private PropertyInfo propertyInfo_3;

		private object object_1;

		private string string_1;

		private string string_2;

		private IEnumerator<object> ienumerator_1;

		private object object_2;

		private int? nullable_0;

		private string string_3;

		private string string_4;

		private object object_3;

		private string string_5;

		private string string_6;

		private AIToolResult aitoolResult_0;

		private PropertyInfo propertyInfo_4;

		private object object_4;

		private PropertyInfo propertyInfo_5;

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
					Class500 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("familyName", (string)null);
				if (string.IsNullOrEmpty(string_0))
				{
					result = AIToolResult.Fail("familyName 参数不能为空");
				}
				else
				{
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
						ienumerable_0 = ifamilyService_0.GetFamilyTypes(aitoolContext_0.Document, string_0);
						list_0 = new List<object>();
						if (ienumerable_0.Any())
						{
							ienumerator_1 = ienumerable_0.GetEnumerator();
							try
							{
								while (ienumerator_1.MoveNext())
								{
									object_2 = ienumerator_1.Current;
									nullable_0 = ielementService_0.GetElementId(object_2);
									string_3 = ielementService_0.GetElementName(object_2);
									string_4 = ielementService_0.GetElementCategory(object_2);
									if (nullable_0.HasValue)
									{
										object_3 = ifamilyService_0.GetFamilyOfTypeId(aitoolContext_0.Document, nullable_0.Value);
										string_5 = ((object_3 != null) ? ielementService_0.GetElementName(object_3) : null);
										list_0.Add(new Class178<int?, string, string, string, bool>(nullable_0, string_3 ?? "未命名", string_4 ?? "未知", string_5 ?? "未知", gparam_9: false));
										string_3 = null;
										string_4 = null;
										object_3 = null;
										string_5 = null;
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
							goto IL_057a;
						}
						ienumerable_1 = ifamilyService_0.GetTypesByCategory(aitoolContext_0.Document, string_0);
						ienumerator_0 = ienumerable_1.GetEnumerator();
						try
						{
							while (ienumerator_0.MoveNext())
							{
								object_0 = ienumerator_0.Current;
								propertyInfo_0 = object_0?.GetType().GetProperties();
								if (propertyInfo_0 == null)
								{
									continue;
								}
								propertyInfo_1 = Array.Find(propertyInfo_0, (PropertyInfo propertyInfo_0) => propertyInfo_0.Name.Equals("typeId"));
								propertyInfo_2 = Array.Find(propertyInfo_0, (PropertyInfo propertyInfo_0) => propertyInfo_0.Name.Equals("typeName"));
								propertyInfo_3 = Array.Find(propertyInfo_0, (PropertyInfo propertyInfo_0) => propertyInfo_0.Name.Equals("categoryName"));
								if (propertyInfo_1 == null || propertyInfo_2 == null)
								{
									continue;
								}
								object_1 = propertyInfo_1.GetValue(object_0);
								string_1 = propertyInfo_2.GetValue(object_0)?.ToString();
								PropertyInfo propertyInfo = propertyInfo_3;
								object obj;
								if ((object)propertyInfo == null)
								{
									obj = null;
								}
								else
								{
									object? value = propertyInfo.GetValue(object_0);
									if (value == null)
									{
										obj = null;
									}
									else
									{
										obj = value.ToString();
										if (obj != null)
										{
											goto IL_0314;
										}
									}
								}
								obj = string_0;
								goto IL_0314;
								IL_0314:
								string_2 = (string)obj;
								if (object_1 != null)
								{
									list_0.Add(new Class178<object, string, string, string, bool>(object_1, string_1 ?? "未命名", string_2, string_0, gparam_9: true));
									propertyInfo_0 = null;
									propertyInfo_1 = null;
									propertyInfo_2 = null;
									propertyInfo_3 = null;
									object_1 = null;
									string_1 = null;
									string_2 = null;
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
						if (list_0.Count != 0)
						{
							ienumerable_1 = null;
							goto IL_057a;
						}
						result = AIToolResult.Fail("找不到族 '" + string_0 + "' 的类型。请确认族名称是否正确。");
					}
				}
				goto end_IL_0067;
				IL_057a:
				if (!string.IsNullOrEmpty(aitoolContext_0.SessionId) && aitoolContext_0.DataCache != null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
					defaultInterpolatedStringHandler.AppendLiteral("family_types_");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral("_");
					defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now.Ticks);
					string_6 = defaultInterpolatedStringHandler.ToStringAndClear();
					aitoolResult_0 = AIToolResult.OkWithSmartSummary<object>((IEnumerable<object>)list_0, aitoolContext_0.DataCache, aitoolContext_0.SessionId, string_6, "个族类型", 200);
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
										string_7 = propertyInfo_5.GetValue(object_4)?.ToString();
										if (!string.IsNullOrEmpty(string_7))
										{
											AIToolResult obj2 = aitoolResult_0;
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(54, 3);
											defaultInterpolatedStringHandler2.AppendLiteral("✅ 找到族 '");
											defaultInterpolatedStringHandler2.AppendFormatted(string_0);
											defaultInterpolatedStringHandler2.AppendLiteral("' 的 ");
											defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
											defaultInterpolatedStringHandler2.AppendLiteral(" 个类型定义\n\n💡 在后续工具调用中使用 cacheId=\"");
											defaultInterpolatedStringHandler2.AppendFormatted(string_7);
											defaultInterpolatedStringHandler2.AppendLiteral("\" 参数来操作这些族类型");
											obj2.Message = defaultInterpolatedStringHandler2.ToStringAndClear();
										}
										string_7 = null;
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
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("找到族 '");
					defaultInterpolatedStringHandler3.AppendFormatted(string_0);
					defaultInterpolatedStringHandler3.AppendLiteral("' 的 ");
					defaultInterpolatedStringHandler3.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个类型定义");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler3.ToStringAndClear(), (object)new Class156<int, List<object>>(list_0.Count, list_0));
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取族类型失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_family_types";

	public string Category => "族管理";

	public string Description => "获取指定族的所有类型定义";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"familyName\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"族名称（必需）。例如：'门'、'窗'、'墙' 等。必须使用与 Revit 界面中完全一致的族名称\"\r\n            }\r\n        },\r\n        \"required\": [\"familyName\"]\r\n    }";

	[AsyncStateMachine(typeof(Class500))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class500 stateMachine = new Class500();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getFamilyTypesTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
