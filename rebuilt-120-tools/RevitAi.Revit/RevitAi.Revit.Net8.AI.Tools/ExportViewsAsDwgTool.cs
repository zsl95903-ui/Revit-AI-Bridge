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

[AITool("export_views_as_dwg", Category = "视图导出", Description = "批量导出多个视图为 DWG/DXF 文件。用户只需选择一次文件夹位置，支持从缓存导出视图列表或直接指定视图 ID。", RequiresTransaction = false, RequiresModification = false)]
public sealed class ExportViewsAsDwgTool : IAITool
{
	[CompilerGenerated]
	private sealed class Class468 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ExportViewsAsDwgTool exportViewsAsDwgTool_0;

		private IDwgExportService idwgExportService_0;

		private string string_0;

		private List<int> list_0;

		private string string_1;

		private string string_2;

		private bool bool_0;

		private bool bool_1;

		private bool bool_2;

		private DwgExportConfig dwgExportConfig_0;

		private object object_0;

		private DwgBatchExportResult dwgBatchExportResult_0;

		private string string_3;

		private string string_4;

		private object object_1;

		private object object_2;

		private string string_5;

		private object object_3;

		private IViewService iviewService_0;

		private object object_4;

		private int? nullable_0;

		private string string_6;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Expected O, but got Unknown
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class468 stateMachine = this;
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
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				idwgExportService_0 = ((revitAdapter != null) ? revitAdapter.DwgExportService : null);
				if (idwgExportService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 DWG 导出服务");
				}
				else
				{
					string_0 = aitoolContext_0.GetParameter<string>("dataSource", (string)null);
					if (string.IsNullOrEmpty(string_0))
					{
						string_0 = "cache";
					}
					list_0 = new List<int>();
					string_1 = null;
					string_2 = aitoolContext_0.GetParameter<string>("exportSettingName", (string)null);
					bool_0 = aitoolContext_0.GetParameter<bool>("exportAs3D", false);
					bool_1 = aitoolContext_0.GetParameter<bool>("sharedLevels", false);
					bool_2 = aitoolContext_0.GetParameter<bool>("exportAsDXF", false);
					if (string_0 == "cache")
					{
						string_3 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
						if (string.IsNullOrEmpty(string_3))
						{
							result = AIToolResult.Fail("缓存 ID 不能为空（当 dataSource 为 'cache' 时）");
						}
						else
						{
							string_4 = aitoolContext_0.GetParameter<string>("exportDirectory", (string)null);
							if (!string.IsNullOrEmpty(string_4))
							{
								string_1 = string_4;
							}
							if (aitoolContext_0.DataCache == null)
							{
								result = AIToolResult.Fail("数据缓存服务不可用");
							}
							else
							{
								object_1 = aitoolContext_0.DataCache.Retrieve<object>(string_3);
								if (object_1 == null)
								{
									result = AIToolResult.Fail("缓存 '" + string_3 + "' 不存在或已过期");
								}
								else
								{
									list_0 = exportViewsAsDwgTool_0.method_0(object_1);
									if (list_0.Count != 0)
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
										defaultInterpolatedStringHandler.AppendLiteral("[ExportViewsAsDwgTool] 从缓存导出 ");
										defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
										defaultInterpolatedStringHandler.AppendLiteral(" 个视图");
										Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
										string_3 = null;
										string_4 = null;
										object_1 = null;
										goto IL_05db;
									}
									result = AIToolResult.Fail("缓存中没有有效的视图数据");
								}
							}
						}
					}
					else if (string_0 == "ids")
					{
						object_2 = aitoolContext_0.GetParameter<object>("viewIds", (object)null);
						if (object_2 == null)
						{
							result = AIToolResult.Fail("视图 ID 列表不能为空（当 dataSource 为 'ids' 时）");
						}
						else
						{
							string_5 = aitoolContext_0.GetParameter<string>("exportDirectory", (string)null);
							if (!string.IsNullOrEmpty(string_5))
							{
								string_1 = string_5;
							}
							list_0 = exportViewsAsDwgTool_0.method_1(object_2);
							if (list_0.Count != 0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(33, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("[ExportViewsAsDwgTool] 导出指定的 ");
								defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
								defaultInterpolatedStringHandler2.AppendLiteral(" 个视图");
								Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
								object_2 = null;
								string_5 = null;
								goto IL_05db;
							}
							result = AIToolResult.Fail("无法解析视图 ID 列表");
						}
					}
					else if (string_0 == "current")
					{
						object_3 = aitoolContext_0.Document;
						if (object_3 == null)
						{
							result = AIToolResult.Fail("无法获取活动文档");
						}
						else
						{
							IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
							iviewService_0 = ((revitAdapter2 != null) ? revitAdapter2.ViewService : null);
							if (iviewService_0 == null)
							{
								result = AIToolResult.Fail("无法获取视图服务");
							}
							else
							{
								object_4 = iviewService_0.GetActiveView(object_3);
								if (object_4 == null)
								{
									result = AIToolResult.Fail("无法获取活动视图");
								}
								else
								{
									nullable_0 = iviewService_0.GetViewId(object_4);
									if (nullable_0.HasValue)
									{
										list_0.Add(nullable_0.Value);
										string_6 = aitoolContext_0.GetParameter<string>("exportDirectory", (string)null);
										if (!string.IsNullOrEmpty(string_6))
										{
											string_1 = string_6;
										}
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(36, 1);
										defaultInterpolatedStringHandler3.AppendLiteral("[ExportViewsAsDwgTool] 导出当前视图 (ID: ");
										defaultInterpolatedStringHandler3.AppendFormatted(nullable_0.Value);
										defaultInterpolatedStringHandler3.AppendLiteral(")");
										Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
										object_3 = null;
										iviewService_0 = null;
										object_4 = null;
										string_6 = null;
										goto IL_05db;
									}
									result = AIToolResult.Fail("无法获取视图 ID");
								}
							}
						}
					}
					else
					{
						result = AIToolResult.Fail("不支持的数据源类型: " + string_0 + "。支持的类型：'cache'、'ids'、'current'");
					}
				}
				goto end_IL_0067;
				IL_061c:
				dwgExportConfig_0 = new DwgExportConfig
				{
					ExportSettingName = string_2,
					ExportAs3D = bool_0,
					SharedLevels = bool_1,
					ExportAsDXF = bool_2
				};
				IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
				object_0 = ((revitAdapter3 != null) ? revitAdapter3.GetActiveDocument() : null);
				if (object_0 == null)
				{
					result = AIToolResult.Fail("无法获取活动文档");
				}
				else
				{
					dwgBatchExportResult_0 = idwgExportService_0.ExportViewsByIds(object_0, (IEnumerable<int>)list_0, string_1 ?? "", dwgExportConfig_0);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(12, 3);
					defaultInterpolatedStringHandler4.AppendLiteral("成功导出 ");
					defaultInterpolatedStringHandler4.AppendFormatted(dwgBatchExportResult_0.SuccessCount);
					defaultInterpolatedStringHandler4.AppendLiteral("/");
					defaultInterpolatedStringHandler4.AppendFormatted(dwgBatchExportResult_0.TotalViews);
					defaultInterpolatedStringHandler4.AppendLiteral(" 个视图到 ");
					defaultInterpolatedStringHandler4.AppendFormatted(string_1);
					result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class129<string, int, int, int, List<_003C_003Ef__AnonymousType133<bool, string, string, string>>>(dwgBatchExportResult_0.ExportDirectory, dwgBatchExportResult_0.TotalViews, dwgBatchExportResult_0.SuccessCount, dwgBatchExportResult_0.FailCount, dwgBatchExportResult_0.Results.Select((DwgExportResult dwgExportResult_0) => new _003C_003Ef__AnonymousType133<bool, string, string, string>(dwgExportResult_0.IsSuccess, dwgExportResult_0.FilePath, dwgExportResult_0.ViewName, dwgExportResult_0.Error)).ToList()));
				}
				goto end_IL_0067;
				IL_05db:
				if (!string.IsNullOrEmpty(string_1))
				{
					goto IL_061c;
				}
				string_1 = idwgExportService_0.SelectFolder((string)null);
				if (!string.IsNullOrEmpty(string_1))
				{
					goto IL_061c;
				}
				result = AIToolResult.Fail("用户取消了文件夹选择");
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[ExportViewsAsDwgTool] 导出视图为 DWG 时发生异常：" + exception_0.Message, exception_0);
				result = AIToolResult.Fail("导出视图为 DWG 时发生异常：" + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "export_views_as_dwg";

	public string Category => "视图导出";

	public string Description => "批量导出多个视图为 DWG/DXF 文件";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"dataSource\": {\n                \"type\": \"string\",\n                \"description\": \"数据源类型。可选值：'cache'（从缓存导出）、'ids'（指定视图 ID）或 'current'（导出当前视图）。默认为 'cache'。\",\n                \"enum\": [\"cache\", \"ids\", \"current\"]\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（当 dataSource 为 'cache' 时必需）。从之前查询返回的缓存 ID 中获取。\"\n            },\n            \"viewIds\": {\n                \"type\": \"array\",\n                \"description\": \"视图 ID 列表（当 dataSource 为 'ids' 时必需）。例如：[123, 456, 789]\",\n                \"items\": {\n                    \"type\": \"integer\"\n                }\n            },\n            \"exportDirectory\": {\n                \"type\": \"string\",\n                \"description\": \"导出目录路径（可选）。如果不提供，将弹出文件夹选择对话框让用户选择位置。\"\n            },\n            \"exportSettingName\": {\n                \"type\": \"string\",\n                \"description\": \"导出设置名称（可选）。如果不提供，使用默认设置。\"\n            },\n            \"exportAs3D\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否导出为 3D 模型（可选，默认为 false）。false 表示导出为 2D。\"\n            },\n            \"sharedLevels\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否共享标高（可选，默认为 true）。\"\n            },\n            \"exportAsDXF\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否导出为 DXF 格式（可选，默认为 false）。false 表示导出为 DWG 格式。\"\n            }\n        },\n        \"required\": [\"dataSource\"]\n    }";

	[AsyncStateMachine(typeof(Class468))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class468 stateMachine = new Class468();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.exportViewsAsDwgTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private List<int> method_0(object object_0)
	{
		List<int> list = new List<int>();
		try
		{
			if (object_0 is IEnumerable enumerable && !(object_0 is string))
			{
				foreach (object item3 in enumerable)
				{
					if (item3 == null)
					{
						continue;
					}
					Type type = item3.GetType();
					if (type == typeof(int) || type == typeof(int?))
					{
						int item = ((int?)item3) ?? Convert.ToInt32(item3);
						list.Add(item);
					}
					else
					{
						if (!type.IsClass || !(type != typeof(string)))
						{
							continue;
						}
						PropertyInfo property = type.GetProperty("Id");
						if (!(property != null))
						{
							continue;
						}
						object value = property.GetValue(item3);
						if (value != null)
						{
							int result;
							if (value is int item2)
							{
								list.Add(item2);
							}
							else if (int.TryParse(value.ToString(), out result))
							{
								list.Add(result);
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[ExportViewsAsDwgTool] 提取视图 ID 失败: " + ex.Message);
		}
		return list;
	}

	private List<int> method_1(object object_0)
	{
		List<int> list = new List<int>();
		try
		{
			if (object_0 is IEnumerable enumerable && !(object_0 is string))
			{
				foreach (object item in enumerable)
				{
					if (item != null && int.TryParse(item.ToString(), out var result))
					{
						list.Add(result);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[ExportViewsAsDwgTool] 转换视图 ID 列表失败: " + ex.Message);
		}
		return list;
	}
}
