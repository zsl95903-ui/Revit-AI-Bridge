using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.AI;
using RevitAi.Revit.Net8.AI.Execution;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("execute_code", Category = "高级工具", Description = "执行 C# 代码直接调用 Revit API。这是终极工具，可覆盖现有工具集未实现的任何功能。自动注入变量：doc（Document）、uidoc（UIDocument）、logger（日志）、tools（AI 工具调用器，可用 tools.Call(name, params) 复用其他工具）。距离单位：毫米（需在代码中除以 304.8 转英尺）。", RequiresTransaction = false, RequiresModification = true, RequiresActiveDocument = true)]
public sealed class ExecuteCodeTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class463 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public ExecuteCodeTool executeCodeTool_0;

		private string string_0;

		private int int_1;

		private Document document_0;

		private UIDocument uidocument_0;

		private AIToolRegistry aitoolRegistry_0;

		private AICodeToolInvoker aicodeToolInvoker_0;

		private CodeExecutionGlobals codeExecutionGlobals_0;

		private AIToolResult aitoolResult_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			AIToolResult result;
			TaskAwaiter<AIToolResult> awaiter;
			int num;
			if (int_0 != 0)
			{
				string_0 = aitoolContext_0.GetParameter<string>("code", string.Empty);
				int_1 = aitoolContext_0.GetParameter<int>("timeout", 60);
				if (string.IsNullOrWhiteSpace(string_0))
				{
					result = AIToolResult.Fail("[参数错误] code 不能为空");
				}
				else
				{
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
							Logger.Warning("[ExecuteCode] 无法构造 UIDocument，仅注入 doc");
						}
						aitoolRegistry_0 = AIToolRegistry.Instance;
						aicodeToolInvoker_0 = new AICodeToolInvoker((IAIToolRegistry)(object)aitoolRegistry_0, aitoolContext_0);
						codeExecutionGlobals_0 = new CodeExecutionGlobals(document_0, uidocument_0, (IAICodeToolInvoker?)(object)aicodeToolInvoker_0);
						awaiter = CodeExecutor.ExecuteAsync(string_0, codeExecutionGlobals_0, int_1).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							Class463 stateMachine = this;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
							return;
						}
						goto IL_0179;
					}
					result = AIToolResult.Fail("[上下文错误] 无法获取 Revit Document");
				}
				goto IL_018f;
			}
			awaiter = taskAwaiter_0;
			taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
			num = -1;
			int_0 = -1;
			goto IL_0179;
			IL_018f:
			int_0 = -2;
			string_0 = null;
			document_0 = null;
			uidocument_0 = null;
			aitoolRegistry_0 = null;
			aicodeToolInvoker_0 = null;
			codeExecutionGlobals_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_0179:
			aitoolResult_0 = awaiter.GetResult();
			result = aitoolResult_0;
			goto IL_018f;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "execute_code";

	public string Category => "高级工具";

	public string Description => "执行 C# 代码直接调用 Revit API（终极工具）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"code\": {\n                \"type\": \"string\",\n                \"description\": \"要执行的 C# 代码片段。可直接使用变量：doc（Document）、uidoc（UIDocument）、logger（Info/Warning/Error 方法）、tools（AI 工具调用器，可通过 tools.Call(工具名, new { 参数 }) 复用其他已注册工具）。事务规则：纯查询代码无需开事务；任何修改操作（Create/Set/Delete/Move 等）必须用 using (var t = new Transaction(doc, 名字)) { t.Start(); ...操作... t.Commit(); } 包裹。单位：用户侧毫米，代码内需 / 304.8 转英尺。\"\n            },\n            \"timeout\": {\n                \"type\": \"integer\",\n                \"description\": \"执行超时（秒），默认 60，最大 120\",\n                \"default\": 60\n            }\n        },\n        \"required\": [\"code\"]\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class463))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class463 stateMachine = new Class463();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.executeCodeTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
