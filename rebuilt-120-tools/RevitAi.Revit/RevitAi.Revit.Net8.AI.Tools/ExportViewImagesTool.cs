using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

[AITool("export_view_images", Category = "视图导出", Description = "批量导出多个视图为图片文件。用户只需选择一次文件夹位置，支持从缓存导出视图列表或直接指定视图 ID。", RequiresTransaction = false, RequiresModification = false)]
public sealed class ExportViewImagesTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class467 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ExportViewImagesTool exportViewImagesTool_0;

		private IViewExportService iviewExportService_0;

		private IViewService iviewService_0;

		private string string_0;

		private string string_1;

		private int int_1;

		private int int_2;

		private string string_2;

		private string string_3;

		private string string_4;

		private ViewExportConfig viewExportConfig_0;

		private BatchExportResult batchExportResult_0;

		private string string_5;

		private object object_0;

		private List<int> list_0;

		private List<int> list_1;

		private object object_1;

		private int? nullable_0;

		private string string_6;

		private string string_7;

		private ViewExportResult viewExportResult_0;

		private List<string> list_2;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Expected O, but got Unknown
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Expected O, but got Unknown
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class467 stateMachine = this;
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
				iviewExportService_0 = ((revitAdapter != null) ? revitAdapter.ViewExportService : null);
				if (iviewExportService_0 == null)
				{
					result = AIToolResult.Fail("无法获取视图导出服务");
				}
				else
				{
					IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
					iviewService_0 = ((revitAdapter2 != null) ? revitAdapter2.ViewService : null);
					if (iviewService_0 == null)
					{
						result = AIToolResult.Fail("无法获取视图服务");
					}
					else if (aitoolContext_0.Document == null)
					{
						result = AIToolResult.Fail("文档对象为空");
					}
					else
					{
						string_0 = aitoolContext_0.GetParameter<string>("dataSource", (string)null);
						if (string.IsNullOrEmpty(string_0))
						{
							string_0 = "cache";
						}
						string_1 = aitoolContext_0.GetParameter<string>("imageFormat", (string)null) ?? "png";
						int_1 = aitoolContext_0.GetParameter<int>("width", 1920);
						int_2 = aitoolContext_0.GetParameter<int>("quality", 100);
						string_2 = aitoolContext_0.GetParameter<string>("fileNamePattern", (string)null) ?? "viewName";
						string_3 = aitoolContext_0.GetParameter<string>("exportDirectory", (string)null);
						if (!string.IsNullOrEmpty(string_3))
						{
							string_4 = string_3;
							goto IL_0247;
						}
						string_4 = iviewExportService_0.SelectFolder((string)null) ?? string.Empty;
						if (!string.IsNullOrEmpty(string_4))
						{
							goto IL_0247;
						}
						result = AIToolResult.Fail("未选择导出目录，操作已取消");
					}
				}
				goto end_IL_0067;
				IL_0247:
				viewExportConfig_0 = new ViewExportConfig
				{
					Width = int_1,
					Height = (int)((double)int_1 * 0.75),
					Quality = int_2,
					ExportRange = "Viewport",
					ExportOnlyVisible = true,
					IncludeBackground = true
				};
				if (string_0 == "cache")
				{
					string_5 = aitoolContext_0.GetParameter<string>("cacheId", (string)null);
					if (string.IsNullOrEmpty(string_5))
					{
						result = AIToolResult.Fail("缓存 ID 不能为空（ dataSource 为 'cache' 时）");
					}
					else
					{
						object_0 = aitoolContext_0.GetCachedData<object>(string_5);
						if (object_0 == null)
						{
							result = AIToolResult.Fail("缓存 '" + string_5 + "' 不存在或已过期");
						}
						else
						{
							list_0 = exportViewImagesTool_0.method_0(object_0);
							if (list_0.Count != 0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
								defaultInterpolatedStringHandler.AppendLiteral("[ExportViewImagesTool] 从缓存导出 ");
								defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
								defaultInterpolatedStringHandler.AppendLiteral(" 个视图到 ");
								defaultInterpolatedStringHandler.AppendFormatted(string_4);
								Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
								batchExportResult_0 = iviewExportService_0.ExportViewsByIds(aitoolContext_0.Document, (IEnumerable<int>)list_0, string_4, viewExportConfig_0);
								string_5 = null;
								object_0 = null;
								list_0 = null;
								goto IL_06ff;
							}
							result = AIToolResult.Fail("缓存中没有有效的视图数据");
						}
					}
				}
				else if (string_0 == "ids")
				{
					list_1 = aitoolContext_0.GetParameter<List<int>>("viewIds", (List<int>)null);
					if (list_1 != null && list_1.Count != 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(35, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("[ExportViewImagesTool] 导出指定的 ");
						defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
						defaultInterpolatedStringHandler2.AppendLiteral(" 个视图到 ");
						defaultInterpolatedStringHandler2.AppendFormatted(string_4);
						Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
						batchExportResult_0 = iviewExportService_0.ExportViewsByIds(aitoolContext_0.Document, (IEnumerable<int>)list_1, string_4, viewExportConfig_0);
						list_1 = null;
						goto IL_06ff;
					}
					result = AIToolResult.Fail("视图 ID 列表不能为空（ dataSource 为 'ids' 时）");
				}
				else if (string_0 == "current")
				{
					object_1 = iviewService_0.GetActiveView(aitoolContext_0.Document);
					if (object_1 == null)
					{
						result = AIToolResult.Fail("无法获取当前活动视图");
					}
					else
					{
						nullable_0 = iviewService_0.GetViewId(object_1);
						if (nullable_0.HasValue)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(39, 2);
							defaultInterpolatedStringHandler3.AppendLiteral("[ExportViewImagesTool] 导出当前视图 (ID: ");
							defaultInterpolatedStringHandler3.AppendFormatted(nullable_0);
							defaultInterpolatedStringHandler3.AppendLiteral(") 到 ");
							defaultInterpolatedStringHandler3.AppendFormatted(string_4);
							Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
							string_6 = iviewExportService_0.GetDefaultFileName(object_1, string_1) ?? ("CurrentView." + string_1);
							string_7 = Path.Combine(string_4, string_6);
							viewExportConfig_0.FilePath = string_7;
							viewExportResult_0 = iviewExportService_0.ExportView(aitoolContext_0.Document, object_1, viewExportConfig_0);
							batchExportResult_0 = new BatchExportResult
							{
								TotalViews = 1,
								SuccessCount = (viewExportResult_0.IsSuccess ? 1 : 0),
								FailureCount = ((!viewExportResult_0.IsSuccess) ? 1 : 0),
								ExportDirectory = string_4,
								Results = new List<ViewExportResult> { viewExportResult_0 }
							};
							object_1 = null;
							string_6 = null;
							string_7 = null;
							viewExportResult_0 = null;
							goto IL_06ff;
						}
						result = AIToolResult.Fail("无法获取当前视图的 ID");
					}
				}
				else
				{
					result = AIToolResult.Fail("不支持的数据源类型: " + string_0 + "。支持的类型：'cache'、'ids'、'current'");
				}
				goto end_IL_0067;
				IL_06ff:
				if (batchExportResult_0.SuccessCount > 0)
				{
					list_2 = (from viewExportResult_0 in batchExportResult_0.Results
						where viewExportResult_0.IsSuccess
						select viewExportResult_0.FilePath).ToList();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(26, 4);
					defaultInterpolatedStringHandler4.AppendLiteral("✅ 成功导出 ");
					defaultInterpolatedStringHandler4.AppendFormatted(batchExportResult_0.SuccessCount);
					defaultInterpolatedStringHandler4.AppendLiteral("/");
					defaultInterpolatedStringHandler4.AppendFormatted(batchExportResult_0.TotalViews);
					defaultInterpolatedStringHandler4.AppendLiteral(" 个视图到目录：");
					defaultInterpolatedStringHandler4.AppendFormatted(string_4);
					defaultInterpolatedStringHandler4.AppendLiteral("\n");
					defaultInterpolatedStringHandler4.AppendLiteral("❌ 失败 ");
					defaultInterpolatedStringHandler4.AppendFormatted(batchExportResult_0.FailureCount);
					defaultInterpolatedStringHandler4.AppendLiteral(" 个视图");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class128<int, int, int, string, List<string>>(batchExportResult_0.SuccessCount, batchExportResult_0.FailureCount, batchExportResult_0.TotalViews, batchExportResult_0.ExportDirectory, list_2));
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(19, 1);
					defaultInterpolatedStringHandler5.AppendLiteral("批量导出失败：所有 ");
					defaultInterpolatedStringHandler5.AppendFormatted(batchExportResult_0.TotalViews);
					defaultInterpolatedStringHandler5.AppendLiteral(" 个视图导出均失败");
					result = AIToolResult.Fail(defaultInterpolatedStringHandler5.ToStringAndClear());
				}
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[ExportViewImagesTool] 导出视图图片时发生异常：" + exception_0.Message, exception_0);
				result = AIToolResult.Fail("导出视图图片时发生异常：" + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "export_view_images";

	public string Category => "视图导出";

	public string Description => "批量导出多个视图为图片文件";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"dataSource\": {\n                \"type\": \"string\",\n                \"description\": \"数据源类型。可选值：'cache'（从缓存导出视图）、'ids'（按视图 ID 导出）、'current'（导出当前视图）。默认为 'cache'。\",\n                \"enum\": [\"cache\", \"ids\", \"current\"]\n            },\n            \"cacheId\": {\n                \"type\": \"string\",\n                \"description\": \"缓存 ID（当 dataSource 为 'cache' 时必需）。从 get_all_views 工具返回的 cacheId。\"\n            },\n            \"viewIds\": {\n                \"type\": \"array\",\n                \"description\": \"视图 ID 列表（当 dataSource 为 'ids' 时必需）。例如：[321, 322, 323]。\",\n                \"items\": {\n                    \"type\": \"integer\"\n                }\n            },\n            \"exportDirectory\": {\n                \"type\": \"string\",\n                \"description\": \"导出目录路径。如果不提供，将弹出文件夹选择对话框让用户选择位置。\"\n            },\n            \"imageFormat\": {\n                \"type\": \"string\",\n                \"description\": \"图片格式。可选值：'png'、'jpg'。默认为 'png'。\",\n                \"enum\": [\"png\", \"jpg\"],\n                \"default\": \"png\"\n            },\n            \"width\": {\n                \"type\": \"integer\",\n                \"description\": \"图片宽度（像素）。默认为 1920。\",\n                \"default\": 1920,\n                \"minimum\": 100,\n                \"maximum\": 4096\n            },\n            \"quality\": {\n                \"type\": \"integer\",\n                \"description\": \"图片质量（1-100）。默认为 100。\",\n                \"default\": 100,\n                \"minimum\": 1,\n                \"maximum\": 100\n            },\n            \"fileNamePattern\": {\n                \"type\": \"string\",\n                \"description\": \"文件名模式。可选值：'viewName'（使用视图名称）、'viewId'（使用视图 ID）、'sequential'（使用序号）。默认为 'viewName'。\",\n                \"enum\": [\"viewName\", \"viewId\", \"sequential\"],\n                \"default\": \"viewName\"\n            }\n        },\n        \"required\": [\"dataSource\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class467))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class467 stateMachine = new Class467();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.exportViewImagesTool_0 = this;
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
				foreach (object item2 in enumerable)
				{
					if (item2 == null)
					{
						continue;
					}
					if (item2 is IDictionary dictionary)
					{
						if (dictionary.Contains("id"))
						{
							object obj = dictionary["id"];
							if (obj != null && int.TryParse(obj.ToString(), out var result))
							{
								list.Add(result);
							}
						}
					}
					else if (item2 is int item)
					{
						list.Add(item);
					}
					else if (item2 is long num)
					{
						list.Add((int)num);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[ExportViewImagesTool] 提取视图 ID 失败: " + ex.Message);
		}
		return list;
	}
}
