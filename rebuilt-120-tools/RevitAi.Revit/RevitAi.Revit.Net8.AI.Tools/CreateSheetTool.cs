using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("create_sheet", Category = "文档操作", Description = "创建新的图纸（Sheet），可选择向图纸添加视图", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateSheetTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class417 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateSheetTool createSheetTool_0;

		private string string_0;

		private string string_1;

		private int? nullable_0;

		private double? nullable_1;

		private double? nullable_2;

		private IDocumentService idocumentService_0;

		private IElementService ielementService_0;

		private object object_0;

		private int? nullable_3;

		private string string_2;

		private object object_1;

		private object object_2;

		private object object_3;

		private double double_0;

		private double double_1;

		private Exception exception_0;

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
					Class417 stateMachine = this;
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
				string_0 = aitoolContext_0.GetParameter<string>("title", (string)null);
				string_1 = aitoolContext_0.GetParameter<string>("number", (string)null);
				nullable_0 = aitoolContext_0.GetParameter<int?>("add_view", (int?)null);
				nullable_1 = aitoolContext_0.GetParameter<double?>("view_position_x", (double?)null);
				nullable_2 = aitoolContext_0.GetParameter<double?>("view_position_y", (double?)null);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				idocumentService_0 = ((revitAdapter != null) ? revitAdapter.DocumentService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				if (idocumentService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 DocumentService");
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
					object_0 = idocumentService_0.CreateSheet(aitoolContext_0.Document, (int?)null, string_0 ?? "图纸", string_1 ?? "");
					if (object_0 == null)
					{
						result = AIToolResult.Fail("创建图纸失败");
					}
					else
					{
						nullable_3 = ielementService_0.GetElementId(object_0);
						string_2 = ielementService_0.GetElementName(object_0);
						if (!nullable_0.HasValue || nullable_0.Value <= 0)
						{
							goto IL_03cf;
						}
						object_1 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_0.Value);
						if (object_1 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
							defaultInterpolatedStringHandler.AppendFormatted(nullable_0.Value);
							defaultInterpolatedStringHandler.AppendLiteral(" 的视图");
							result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							object_2 = null;
							if (nullable_1.HasValue && nullable_2.HasValue)
							{
								double_0 = nullable_1.Value / 304.8;
								double_1 = nullable_2.Value / 304.8;
								object_2 = new
								{
									x = double_0,
									y = double_1,
									z = 0.0
								};
							}
							object_3 = idocumentService_0.AddViewToSheet(aitoolContext_0.Document, object_0, object_1, object_2);
							if (object_3 != null)
							{
								object_1 = null;
								object_2 = null;
								object_3 = null;
								goto IL_03cf;
							}
							result = AIToolResult.Fail("图纸创建成功，但添加视图失败");
						}
					}
				}
				goto end_IL_0067;
				IL_03cf:
				string text = "成功创建图纸: ";
				string obj = string_2 ?? "未命名";
				string text2;
				if (!nullable_0.HasValue)
				{
					text2 = "";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(8, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("（已添加视图 ");
					defaultInterpolatedStringHandler2.AppendFormatted(nullable_0.Value);
					defaultInterpolatedStringHandler2.AppendLiteral("）");
					text2 = defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				result = AIToolResult.Ok(text + obj + text2, (object)new Class79<int?, string, string, string, int?>(nullable_3, string_2, string_0, string_1, nullable_0));
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("创建图纸失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "create_sheet";

	public string Category => "文档操作";

	public string Description => "创建新的图纸（可选择添加视图）";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"title\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"图纸标题（可选，默认为 '图纸'）\"\r\n            },\r\n            \"number\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"图纸编号（可选）\"\r\n            },\r\n            \"add_view\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"要添加到图纸的视图 ID（可选）\"\r\n            },\r\n            \"view_position_x\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"视图在图纸上的 X 位置（毫米，可选，默认为中心位置）\"\r\n            },\r\n            \"view_position_y\": {\r\n                \"type\": \"number\",\r\n                \"description\": \"视图在图纸上的 Y 位置（毫米，可选，默认为中心位置）\"\r\n            }\r\n        },\r\n        \"required\": []\r\n    }";

	[AsyncStateMachine(typeof(Class417))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class417 stateMachine = new Class417();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createSheetTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
