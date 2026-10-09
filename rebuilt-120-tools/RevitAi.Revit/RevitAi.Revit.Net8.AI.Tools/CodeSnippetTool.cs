using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.AI;
using RevitAi.Core.AI.Models;
using RevitAi.Revit.Net8.AI.Execution;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("code_snippet", Category = "高级工具", Description = "管理本地代码片段库。支持五种操作：search（搜索返回列表）、view（查看详细内容）、execute（执行代码）、save（保存新片段）、update（更新现有片段）。", RequiresTransaction = false, RequiresModification = true, RequiresActiveDocument = true)]
public sealed class CodeSnippetTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class377 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public string string_0;

		public string string_1;

		public CancellationToken cancellationToken_0;

		public CodeSnippetTool codeSnippetTool_0;

		private CodeSnippet codeSnippet_0;

		private Document document_0;

		private UIDocument uidocument_0;

		private AIToolRegistry aitoolRegistry_0;

		private AICodeToolInvoker aicodeToolInvoker_0;

		private CodeExecutionGlobals codeExecutionGlobals_0;

		private AIToolResult aitoolResult_0;

		private string string_2;

		private List<CodeSnippet> list_0;

		private List<string> list_1;

		private string string_3;

		private AIToolResult aitoolResult_1;

		private TaskAwaiter<AIToolResult> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Expected O, but got Unknown
			AIToolResult result;
			if (int_0 != 0)
			{
				Logger.Info("[CodeSnippet] 执行执行操作");
				if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("[上下文错误] 无法获取 Revit Document");
					goto IL_0412;
				}
				codeSnippet_0 = null;
				if (!string.IsNullOrWhiteSpace(string_0))
				{
					codeSnippet_0 = codeSnippetTool_0.codeSnippetStorageService_0.GetSnippet(string_0);
				}
				else if (!string.IsNullOrWhiteSpace(string_1))
				{
					list_0 = codeSnippetTool_0.codeSnippetStorageService_0.SearchSnippets(string_1);
					if (list_0.Count == 0)
					{
						result = AIToolResult.Fail("未找到包含关键词 '" + string_1 + "' 的代码片段");
					}
					else
					{
						if (list_0.Count <= 1)
						{
							codeSnippet_0 = list_0[0];
							list_0 = null;
							goto IL_01ed;
						}
						list_1 = list_0.Select(delegate(CodeSnippet codeSnippet_0, int int_0)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(7, 4);
							defaultInterpolatedStringHandler2.AppendFormatted(int_0 + 1);
							defaultInterpolatedStringHandler2.AppendLiteral(". [");
							defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet_0.Id);
							defaultInterpolatedStringHandler2.AppendLiteral("] ");
							defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet_0.Name);
							defaultInterpolatedStringHandler2.AppendLiteral(": ");
							defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet_0.Description);
							return defaultInterpolatedStringHandler2.ToStringAndClear();
						}).ToList();
						string[] array = new string[7];
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找到 ");
						defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个匹配的代码片段：");
						array[0] = defaultInterpolatedStringHandler.ToStringAndClear();
						array[1] = Environment.NewLine;
						array[2] = Environment.NewLine;
						array[3] = string.Join(Environment.NewLine, list_1);
						array[4] = Environment.NewLine;
						array[5] = Environment.NewLine;
						array[6] = "💡 提示：请使用 snippetId 参数执行具体片段";
						string_3 = string.Concat(array);
						result = AIToolResult.Ok(string_3, (object)null);
					}
					goto IL_0412;
				}
				goto IL_01ed;
			}
			TaskAwaiter<AIToolResult> awaiter = taskAwaiter_0;
			taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
			int num = -1;
			int_0 = -1;
			goto IL_035a;
			IL_035a:
			aitoolResult_1 = awaiter.GetResult();
			aitoolResult_0 = aitoolResult_1;
			aitoolResult_1 = null;
			string_2 = "执行代码片段: " + codeSnippet_0.Name + Environment.NewLine + Environment.NewLine + codeSnippet_0.Description + Environment.NewLine + Environment.NewLine + "执行结果：" + Environment.NewLine + aitoolResult_0.Message;
			result = AIToolResult.Ok(string_2, aitoolResult_0.Data);
			goto IL_0412;
			IL_01ed:
			if (codeSnippet_0 == null)
			{
				result = AIToolResult.Fail("未找到指定的代码片段。请使用 snippetId 或 keyword 参数");
			}
			else
			{
				Logger.Info("[CodeSnippet] 执行片段: " + codeSnippet_0.Name);
				codeSnippetTool_0.codeSnippetStorageService_0.RecordUsage(codeSnippet_0.Id);
				object document = aitoolContext_0.Document;
				document_0 = (Document)((document is Document) ? document : null);
				if (document_0 != null)
				{
					uidocument_0 = null;
					try
					{
						uidocument_0 = new UIDocument(document_0);
					}
					catch
					{
						Logger.Warning("[CodeSnippet] 无法构造 UIDocument，仅注入 doc");
					}
					aitoolRegistry_0 = AIToolRegistry.Instance;
					aicodeToolInvoker_0 = new AICodeToolInvoker((IAIToolRegistry)(object)aitoolRegistry_0, aitoolContext_0);
					codeExecutionGlobals_0 = new CodeExecutionGlobals(document_0, uidocument_0, (IAICodeToolInvoker?)(object)aicodeToolInvoker_0);
					awaiter = CodeExecutor.ExecuteAsync(codeSnippet_0.Code, codeExecutionGlobals_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						Class377 stateMachine = this;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
						return;
					}
					goto IL_035a;
				}
				result = AIToolResult.Fail("[上下文错误] 无法获取 Revit Document");
			}
			goto IL_0412;
			IL_0412:
			int_0 = -2;
			codeSnippet_0 = null;
			document_0 = null;
			uidocument_0 = null;
			aitoolRegistry_0 = null;
			aicodeToolInvoker_0 = null;
			codeExecutionGlobals_0 = null;
			aitoolResult_0 = null;
			string_2 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	[CompilerGenerated]
	public sealed class Class378 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CodeSnippetTool codeSnippetTool_0;

		private string string_0;

		private string string_1;

		private string string_2;

		private string string_3;

		private AIToolResult aitoolResult_0;

		private AIToolResult aitoolResult_1;

		private AIToolResult aitoolResult_2;

		private AIToolResult aitoolResult_3;

		private AIToolResult aitoolResult_4;

		private Exception exception_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			if ((uint)num <= 4u)
			{
			}
			AIToolResult result;
			try
			{
				TaskAwaiter<AIToolResult> awaiter5;
				TaskAwaiter<AIToolResult> awaiter4;
				TaskAwaiter<AIToolResult> awaiter3;
				TaskAwaiter<AIToolResult> awaiter2;
				TaskAwaiter<AIToolResult> awaiter;
				switch (num)
				{
				default:
				{
					codeSnippetTool_0.codeSnippetStorageService_0.Reload();
					string_0 = aitoolContext_0.GetParameter<string>("action", string.Empty);
					string_1 = aitoolContext_0.GetParameter<string>("snippetId", string.Empty);
					string_2 = aitoolContext_0.GetParameter<string>("keyword", string.Empty);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
					defaultInterpolatedStringHandler.AppendLiteral("[CodeSnippet] 操作: ");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral(", snippetId='");
					defaultInterpolatedStringHandler.AppendFormatted(string_1);
					defaultInterpolatedStringHandler.AppendLiteral("', keyword='");
					defaultInterpolatedStringHandler.AppendFormatted(string_2);
					defaultInterpolatedStringHandler.AppendLiteral("'");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					if (string.IsNullOrWhiteSpace(string_0))
					{
						result = AIToolResult.Fail("必须指定 action 参数（search/view/execute/save/update）");
					}
					else
					{
						string text = string_0.ToLower();
						string_3 = text;
						string text2 = string_3;
						if (text2 == "search")
						{
							awaiter5 = codeSnippetTool_0.method_0(string_1, string_2).GetAwaiter();
							if (!awaiter5.IsCompleted)
							{
								num = 0;
								int_0 = 0;
								taskAwaiter_0 = awaiter5;
								Class378 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter5, ref stateMachine);
								return;
							}
							goto IL_039b;
						}
						if (text2 == "view")
						{
							awaiter4 = codeSnippetTool_0.method_1(string_1, string_2).GetAwaiter();
							if (!awaiter4.IsCompleted)
							{
								num = 1;
								int_0 = 1;
								taskAwaiter_0 = awaiter4;
								Class378 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter4, ref stateMachine);
								return;
							}
							goto IL_03d1;
						}
						if (text2 == "execute")
						{
							awaiter3 = codeSnippetTool_0.method_2(aitoolContext_0, string_1, string_2, cancellationToken_0).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = 2;
								int_0 = 2;
								taskAwaiter_0 = awaiter3;
								Class378 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
								return;
							}
							goto IL_0408;
						}
						if (text2 == "save")
						{
							awaiter2 = codeSnippetTool_0.method_3(aitoolContext_0).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = 3;
								int_0 = 3;
								taskAwaiter_0 = awaiter2;
								Class378 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
							goto IL_043f;
						}
						if (text2 == "update")
						{
							awaiter = codeSnippetTool_0.method_4(aitoolContext_0).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = 4;
								int_0 = 4;
								taskAwaiter_0 = awaiter;
								Class378 stateMachine = this;
								asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
								return;
							}
							break;
						}
						result = AIToolResult.Fail("未知的操作类型: " + string_0 + "。支持的类型：search、view、execute、save、update");
					}
					goto end_IL_000c;
				}
				case 0:
					awaiter5 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_039b;
				case 1:
					awaiter4 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_03d1;
				case 2:
					awaiter3 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_0408;
				case 3:
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
					goto IL_043f;
				case 4:
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
						break;
					}
					IL_03d1:
					aitoolResult_1 = awaiter4.GetResult();
					result = aitoolResult_1;
					goto end_IL_000c;
					IL_043f:
					aitoolResult_3 = awaiter2.GetResult();
					result = aitoolResult_3;
					goto end_IL_000c;
					IL_0408:
					aitoolResult_2 = awaiter3.GetResult();
					result = aitoolResult_2;
					goto end_IL_000c;
					IL_039b:
					aitoolResult_0 = awaiter5.GetResult();
					result = aitoolResult_0;
					goto end_IL_000c;
				}
				aitoolResult_4 = awaiter.GetResult();
				result = aitoolResult_4;
				end_IL_000c:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[CodeSnippet] 操作失败: " + exception_0.Message, exception_0);
				result = AIToolResult.Fail("代码片段操作失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private readonly CodeSnippetStorageService codeSnippetStorageService_0 = new CodeSnippetStorageService();

	public string Name => "code_snippet";

	public string Category => "高级工具";

	public string Description => "管理本地代码片段库（搜索/查看/执行/保存/更新）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"action\": {\n                \"type\": \"string\",\n                \"description\": \"操作类型：search（搜索返回列表）、view（查看详细内容不执行）、execute（执行代码）、save（保存新片段）、update（更新现有片段）\",\n                \"enum\": [\"search\", \"view\", \"execute\", \"save\", \"update\"]\n            },\n            \"snippetId\": {\n                \"type\": \"string\",\n                \"description\": \"代码片段的 ID（精确查找）。update 操作必需，save 操作可选。\"\n            },\n            \"keyword\": {\n                \"type\": \"string\",\n                \"description\": \"搜索关键词（模糊查找）。会在片段名称、描述、标签、代码内容中搜索。如果找到多个匹配片段，将返回列表供选择。\"\n            },\n            \"name\": {\n                \"type\": \"string\",\n                \"description\": \"代码片段名称。save/update 操作必需。\"\n            },\n            \"description\": {\n                \"type\": \"string\",\n                \"description\": \"代码片段描述。save/update 操作可选。\"\n            },\n            \"code\": {\n                \"type\": \"string\",\n                \"description\": \"代码内容。save/update 操作必需。\"\n            },\n            \"tags\": {\n                \"type\": \"string\",\n                \"description\": \"标签，逗号分隔的字符串（如：'墙,创建,批量'）。save/update 操作可选。\"\n            }\n        },\n        \"required\": [\"action\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class378))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class378 stateMachine = new Class378();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.codeSnippetTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private Task<AIToolResult> method_0(string string_0, string string_1)
	{
		Logger.Info("[CodeSnippet] 执行搜索操作");
		if (!string.IsNullOrWhiteSpace(string_0))
		{
			CodeSnippet snippet = codeSnippetStorageService_0.GetSnippet(string_0);
			if (snippet == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("未找到 ID 为 '" + string_0 + "' 的代码片段"));
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(96, 5);
			defaultInterpolatedStringHandler.AppendLiteral("找到代码片段：");
			defaultInterpolatedStringHandler.AppendFormatted(snippet.Name);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendLiteral("🆔 ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(snippet.Id);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendLiteral("📝 描述: ");
			defaultInterpolatedStringHandler.AppendFormatted(snippet.Description);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendLiteral("🏷️ 标签: ");
			defaultInterpolatedStringHandler.AppendFormatted(string.Join(", ", snippet.Tags));
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendLiteral("📊 使用次数: ");
			defaultInterpolatedStringHandler.AppendFormatted(snippet.UseCount);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendLiteral("💡 提示：使用 action='view' 查看完整代码，或 action='execute' 执行代码");
			string text = defaultInterpolatedStringHandler.ToStringAndClear();
			return Task.FromResult<AIToolResult>(AIToolResult.Ok(text, (object)null));
		}
		List<CodeSnippet> list = (string.IsNullOrWhiteSpace(string_1) ? codeSnippetStorageService_0.GetAllSnippets(sortByOrder: true) : codeSnippetStorageService_0.SearchSnippets(string_1));
		if (list.Count == 0)
		{
			List<CodeSnippet> allSnippets = codeSnippetStorageService_0.GetAllSnippets(sortByOrder: true);
			List<string> values = (from codeSnippet_0 in allSnippets.Take(10)
				select "- " + codeSnippet_0.Name + ": " + codeSnippet_0.Description).ToList();
			string text2 = (string.IsNullOrWhiteSpace(string_1) ? "代码片段库为空。" : ("未找到包含关键词 '" + string_1 + "' 的代码片段。"));
			text2 = text2 + Environment.NewLine + Environment.NewLine + "可用片段列表：" + Environment.NewLine + string.Join(Environment.NewLine, values);
			if (allSnippets.Count > 10)
			{
				string text3 = text2;
				string newLine = Environment.NewLine;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("... 共 ");
				defaultInterpolatedStringHandler2.AppendFormatted(allSnippets.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个片段");
				text2 = text3 + newLine + defaultInterpolatedStringHandler2.ToStringAndClear();
			}
			return Task.FromResult<AIToolResult>(AIToolResult.Ok(text2, (object)null));
		}
		List<string> values2 = list.Select(delegate(CodeSnippet codeSnippet_0, int int_0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(15, 5);
			defaultInterpolatedStringHandler4.AppendFormatted(int_0 + 1);
			defaultInterpolatedStringHandler4.AppendLiteral(". [");
			defaultInterpolatedStringHandler4.AppendFormatted(codeSnippet_0.Id);
			defaultInterpolatedStringHandler4.AppendLiteral("] ");
			defaultInterpolatedStringHandler4.AppendFormatted(codeSnippet_0.Name);
			defaultInterpolatedStringHandler4.AppendLiteral(": ");
			defaultInterpolatedStringHandler4.AppendFormatted(codeSnippet_0.Description);
			defaultInterpolatedStringHandler4.AppendLiteral(" (使用 ");
			defaultInterpolatedStringHandler4.AppendFormatted(codeSnippet_0.UseCount);
			defaultInterpolatedStringHandler4.AppendLiteral(" 次)");
			return defaultInterpolatedStringHandler4.ToStringAndClear();
		}).ToList();
		string[] array = new string[7];
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(10, 1);
		defaultInterpolatedStringHandler3.AppendLiteral("找到 ");
		defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler3.AppendLiteral(" 个代码片段：");
		array[0] = defaultInterpolatedStringHandler3.ToStringAndClear();
		array[1] = Environment.NewLine;
		array[2] = Environment.NewLine;
		array[3] = string.Join(Environment.NewLine, values2);
		array[4] = Environment.NewLine;
		array[5] = Environment.NewLine;
		array[6] = "💡 提示：使用 snippetId 查看详情或执行代码";
		string text4 = string.Concat(array);
		return Task.FromResult<AIToolResult>(AIToolResult.Ok(text4, (object)null));
	}

	private Task<AIToolResult> method_1(string string_0, string string_1)
	{
		Logger.Info("[CodeSnippet] 执行查看操作");
		CodeSnippet codeSnippet = null;
		if (!string.IsNullOrWhiteSpace(string_0))
		{
			codeSnippet = codeSnippetStorageService_0.GetSnippet(string_0);
		}
		else if (!string.IsNullOrWhiteSpace(string_1))
		{
			List<CodeSnippet> list = codeSnippetStorageService_0.SearchSnippets(string_1);
			if (list.Count == 0)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("未找到包含关键词 '" + string_1 + "' 的代码片段"));
			}
			if (list.Count > 1)
			{
				List<string> values = list.Select(delegate(CodeSnippet codeSnippet_0, int int_0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(7, 4);
					defaultInterpolatedStringHandler2.AppendFormatted(int_0 + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(". [");
					defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet_0.Id);
					defaultInterpolatedStringHandler2.AppendLiteral("] ");
					defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet_0.Name);
					defaultInterpolatedStringHandler2.AppendLiteral(": ");
					defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet_0.Description);
					return defaultInterpolatedStringHandler2.ToStringAndClear();
				}).ToList();
				string[] array = new string[7];
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找到 ");
				defaultInterpolatedStringHandler.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个匹配的代码片段：");
				array[0] = defaultInterpolatedStringHandler.ToStringAndClear();
				array[1] = Environment.NewLine;
				array[2] = Environment.NewLine;
				array[3] = string.Join(Environment.NewLine, values);
				array[4] = Environment.NewLine;
				array[5] = Environment.NewLine;
				array[6] = "💡 提示：请使用 snippetId 参数查看具体片段的详细内容";
				string text = string.Concat(array);
				return Task.FromResult<AIToolResult>(AIToolResult.Ok(text, (object)null));
			}
			codeSnippet = list[0];
		}
		if (codeSnippet == null)
		{
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("未找到指定的代码片段。请使用 snippetId 或 keyword 参数"));
		}
		string text2 = smethod_0(codeSnippet);
		return Task.FromResult<AIToolResult>(AIToolResult.Ok(text2, (object)null));
	}

	[AsyncStateMachine(typeof(Class377))]
	[DebuggerStepThrough]
	private Task<AIToolResult> method_2(AIToolContext aitoolContext_0, string string_0, string string_1, CancellationToken cancellationToken_0)
	{
		Class377 stateMachine = new Class377();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.codeSnippetTool_0 = this;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.string_0 = string_0;
		stateMachine.string_1 = string_1;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private static string smethod_0(CodeSnippet codeSnippet_0)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
		handler.AppendLiteral("📋 代码片段: ");
		handler.AppendFormatted(codeSnippet_0.Name);
		stringBuilder3.AppendLine(ref handler);
		stringBuilder.AppendLine();
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder4 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
		handler.AppendLiteral("📝 描述: ");
		handler.AppendFormatted(codeSnippet_0.Description);
		stringBuilder4.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder5 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
		handler.AppendLiteral("🆔 ID: ");
		handler.AppendFormatted(codeSnippet_0.Id);
		stringBuilder5.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder6 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
		handler.AppendLiteral("📊 使用次数: ");
		handler.AppendFormatted(codeSnippet_0.UseCount);
		stringBuilder6.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder7 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
		handler.AppendLiteral("🕐 最后使用: ");
		handler.AppendFormatted(codeSnippet_0.LastUsed, "yyyy-MM-dd HH:mm:ss");
		stringBuilder7.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder8 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
		handler.AppendLiteral("🏷️ 标签: ");
		handler.AppendFormatted(string.Join(", ", codeSnippet_0.Tags));
		stringBuilder8.AppendLine(ref handler);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("💻 代码内容:");
		stringBuilder.AppendLine("```");
		stringBuilder.AppendLine(codeSnippet_0.Code.Trim());
		stringBuilder.AppendLine("```");
		return stringBuilder.ToString();
	}

	private Task<AIToolResult> method_3(AIToolContext aitoolContext_0)
	{
		Logger.Info("[CodeSnippet] 执行保存操作");
		string parameter = aitoolContext_0.GetParameter<string>("name", string.Empty);
		string parameter2 = aitoolContext_0.GetParameter<string>("description", string.Empty);
		string parameter3 = aitoolContext_0.GetParameter<string>("code", string.Empty);
		string parameter4 = aitoolContext_0.GetParameter<string>("tags", string.Empty);
		if (string.IsNullOrWhiteSpace(parameter))
		{
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("保存代码片段失败：name 参数不能为空"));
		}
		if (string.IsNullOrWhiteSpace(parameter3))
		{
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("保存代码片段失败：code 参数不能为空"));
		}
		List<string> tags = new List<string>();
		if (!string.IsNullOrWhiteSpace(parameter4))
		{
			tags = (from string_0 in parameter4.Split(',', StringSplitOptions.RemoveEmptyEntries)
				select string_0.Trim() into string_0
				where !string.IsNullOrEmpty(string_0)
				select string_0).ToList();
		}
		CodeSnippet snippetByName = codeSnippetStorageService_0.GetSnippetByName(parameter);
		if (snippetByName != null)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
			defaultInterpolatedStringHandler.AppendLiteral("保存失败：已存在同名代码片段 '");
			defaultInterpolatedStringHandler.AppendFormatted(parameter);
			defaultInterpolatedStringHandler.AppendLiteral("'（ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(snippetByName.Id);
			defaultInterpolatedStringHandler.AppendLiteral("）。");
			return Task.FromResult<AIToolResult>(AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear() + "如需更新现有片段，请使用 action='update' 并提供 snippetId。"));
		}
		CodeSnippet codeSnippet = new CodeSnippet
		{
			Name = parameter,
			Description = parameter2,
			Code = parameter3,
			Tags = tags,
			Order = int.MaxValue
		};
		codeSnippetStorageService_0.AddSnippet(codeSnippet);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 2);
		defaultInterpolatedStringHandler2.AppendLiteral("[CodeSnippet] 保存成功: ");
		defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet.Name);
		defaultInterpolatedStringHandler2.AppendLiteral(" (ID: ");
		defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet.Id);
		defaultInterpolatedStringHandler2.AppendLiteral(")");
		Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(75, 5);
		defaultInterpolatedStringHandler3.AppendLiteral("✅ 代码片段已保存");
		defaultInterpolatedStringHandler3.AppendLiteral("\n📋 名称: ");
		defaultInterpolatedStringHandler3.AppendFormatted(codeSnippet.Name);
		defaultInterpolatedStringHandler3.AppendLiteral("\n🆔 ID: ");
		defaultInterpolatedStringHandler3.AppendFormatted(codeSnippet.Id);
		defaultInterpolatedStringHandler3.AppendLiteral("\n📝 描述: ");
		defaultInterpolatedStringHandler3.AppendFormatted(codeSnippet.Description);
		defaultInterpolatedStringHandler3.AppendLiteral("\n🏷️ 标签: ");
		defaultInterpolatedStringHandler3.AppendFormatted(string.Join(", ", codeSnippet.Tags));
		defaultInterpolatedStringHandler3.AppendLiteral("\n💡 提示：可以使用 snippetId='");
		defaultInterpolatedStringHandler3.AppendFormatted(codeSnippet.Id);
		defaultInterpolatedStringHandler3.AppendLiteral("' 执行或查看此片段");
		string text = defaultInterpolatedStringHandler3.ToStringAndClear();
		return Task.FromResult<AIToolResult>(AIToolResult.Ok(text, (object)new ns0.Class32<string>(codeSnippet.Id)));
	}

	private Task<AIToolResult> method_4(AIToolContext aitoolContext_0)
	{
		Logger.Info("[CodeSnippet] 执行更新操作");
		string parameter = aitoolContext_0.GetParameter<string>("snippetId", string.Empty);
		string parameter2 = aitoolContext_0.GetParameter<string>("name", string.Empty);
		string parameter3 = aitoolContext_0.GetParameter<string>("description", string.Empty);
		string parameter4 = aitoolContext_0.GetParameter<string>("code", string.Empty);
		string parameter5 = aitoolContext_0.GetParameter<string>("tags", string.Empty);
		if (string.IsNullOrWhiteSpace(parameter))
		{
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("更新代码片段失败：snippetId 参数不能为空"));
		}
		CodeSnippet snippet = codeSnippetStorageService_0.GetSnippet(parameter);
		if (snippet == null)
		{
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("更新失败：未找到 ID 为 '" + parameter + "' 的代码片段"));
		}
		List<string> tags = snippet.Tags;
		if (!string.IsNullOrWhiteSpace(parameter5))
		{
			tags = parameter5.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
		}
		CodeSnippet codeSnippet = new CodeSnippet
		{
			Id = snippet.Id,
			Name = (string.IsNullOrWhiteSpace(parameter2) ? snippet.Name : parameter2),
			Description = (string.IsNullOrWhiteSpace(parameter3) ? snippet.Description : parameter3),
			Code = (string.IsNullOrWhiteSpace(parameter4) ? snippet.Code : parameter4),
			Tags = tags,
			CreatedAt = snippet.CreatedAt,
			LastUsed = snippet.LastUsed,
			UseCount = snippet.UseCount,
			Order = snippet.Order
		};
		codeSnippetStorageService_0.UpdateSnippet(codeSnippet);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 2);
		defaultInterpolatedStringHandler.AppendLiteral("[CodeSnippet] 更新成功: ");
		defaultInterpolatedStringHandler.AppendFormatted(codeSnippet.Name);
		defaultInterpolatedStringHandler.AppendLiteral(" (ID: ");
		defaultInterpolatedStringHandler.AppendFormatted(codeSnippet.Id);
		defaultInterpolatedStringHandler.AppendLiteral(")");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(91, 5);
		defaultInterpolatedStringHandler2.AppendLiteral("✅ 代码片段已更新");
		defaultInterpolatedStringHandler2.AppendLiteral("\n📋 名称: ");
		defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet.Name);
		defaultInterpolatedStringHandler2.AppendLiteral("\n🆔 ID: ");
		defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet.Id);
		defaultInterpolatedStringHandler2.AppendLiteral("\n📝 描述: ");
		defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet.Description);
		defaultInterpolatedStringHandler2.AppendLiteral("\n🏷️ 标签: ");
		defaultInterpolatedStringHandler2.AppendFormatted(string.Join(", ", codeSnippet.Tags));
		defaultInterpolatedStringHandler2.AppendLiteral("\n💡 提示：可以使用 action='execute' 和 snippetId='");
		defaultInterpolatedStringHandler2.AppendFormatted(codeSnippet.Id);
		defaultInterpolatedStringHandler2.AppendLiteral("' 执行此片段");
		string text = defaultInterpolatedStringHandler2.ToStringAndClear();
		return Task.FromResult<AIToolResult>(AIToolResult.Ok(text, (object)new ns0.Class32<string>(codeSnippet.Id)));
	}
}
