using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Product;
using RevitAi.Core.Feedback;
using RevitAi.Core.Feedback.Models;
using ns0;
using ns7;

namespace RevitAi.Core.AI.Tools;

public sealed class SubmitDeveloperIssueTool : IAITool
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct222 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public SubmitDeveloperIssueTool submitDeveloperIssueTool_0;

		private string string_0;

		private string string_1;

		private TaskAwaiter<Result<Guid>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SubmitDeveloperIssueTool submitDeveloperIssueTool = submitDeveloperIssueTool_0;
			AIToolResult result;
			try
			{
				TaskAwaiter<Result<Guid>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<Result<Guid>>);
					num = -1;
					int_0 = -1;
					goto IL_01d1;
				}
				object value2;
				object obj;
				if (!aitoolContext_0.Parameters.TryGetValue("title", out var value))
				{
					result = AIToolResult.Fail("缺少必需参数：title");
				}
				else
				{
					if (aitoolContext_0.Parameters.TryGetValue("description", out value2))
					{
						if (value == null)
						{
							obj = null;
						}
						else
						{
							obj = value.ToString();
							if (obj != null)
							{
								goto IL_0091;
							}
						}
						obj = string.Empty;
						goto IL_0091;
					}
					result = AIToolResult.Fail("缺少必需参数：description");
				}
				goto end_IL_000f;
				IL_00ad:
				object obj2;
				string text = (string)obj2;
				aitoolContext_0.Parameters.TryGetValue("category", out var value3);
				aitoolContext_0.Parameters.TryGetValue("email", out var value4);
				object obj3;
				if (value3 == null)
				{
					obj3 = null;
				}
				else
				{
					obj3 = value3.ToString();
					if (obj3 != null)
					{
						goto IL_0106;
					}
				}
				obj3 = "其他";
				goto IL_0106;
				IL_01d1:
				Result<Guid> result2 = awaiter.GetResult();
				if (!result2.IsSuccess)
				{
					result = AIToolResult.Fail("提交失败：" + result2.Error);
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(78, 3);
					defaultInterpolatedStringHandler.AppendLiteral("✅ 问题已成功提交给开发者！\n\n");
					defaultInterpolatedStringHandler.AppendLiteral("问题 ID：");
					defaultInterpolatedStringHandler.AppendFormatted(result2.Value);
					defaultInterpolatedStringHandler.AppendLiteral("\n");
					defaultInterpolatedStringHandler.AppendLiteral("标题：");
					defaultInterpolatedStringHandler.AppendFormatted(string_0);
					defaultInterpolatedStringHandler.AppendLiteral("\n");
					defaultInterpolatedStringHandler.AppendLiteral("类型：");
					defaultInterpolatedStringHandler.AppendFormatted(string_1);
					defaultInterpolatedStringHandler.AppendLiteral("\n\n");
					defaultInterpolatedStringHandler.AppendLiteral("开发者会尽快处理此问题，并通过邮箱（如果提供）跟进。\n");
					defaultInterpolatedStringHandler.AppendLiteral("您也可以在「反馈管理」中查看问题进度。");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class23<string, string, string>(result2.Value.ToString(), string_0, string_1));
				}
				goto end_IL_000f;
				IL_0091:
				string_0 = (string)obj;
				if (value2 == null)
				{
					obj2 = null;
				}
				else
				{
					obj2 = value2.ToString();
					if (obj2 != null)
					{
						goto IL_00ad;
					}
				}
				obj2 = string.Empty;
				goto IL_00ad;
				IL_0106:
				string_1 = (string)obj3;
				string email = value4?.ToString();
				string content = submitDeveloperIssueTool.method_0(aitoolContext_0, text, string_1);
				SubmitFeedbackRequest request = new SubmitFeedbackRequest
				{
					Title = "[AI] " + string_1 + ": " + string_0,
					Content = content,
					Email = email
				};
				awaiter = submitDeveloperIssueTool.feedbackService_0.SubmitFeedbackAsync(request).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
				goto IL_01d1;
				end_IL_000f:;
			}
			catch (Exception ex)
			{
				Logger.Error("[SubmitDeveloperIssue] 执行失败: " + ex.Message, ex);
				result = AIToolResult.Fail("提交问题时发生异常：" + ex.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly FeedbackService feedbackService_0;

	public string Name => "submit_developer_issue";

	public string Category => "问题反馈";

	public string Description => "当 AI 无法自动解决问题时，将问题自动提交给开发者处理。此工具仅用于需要人工干预的复杂问题。";

	public string ParametersSchema => "\n        {\n            \"type\": \"object\",\n            \"properties\": {\n                \"title\": {\n                    \"type\": \"string\",\n                    \"description\": \"问题标题（简短描述问题）\"\n                },\n                \"description\": {\n                    \"type\": \"string\",\n                    \"description\": \"详细描述问题，包括：1. 用户想要做什么 2. AI 尝试了什么 3. 遇到了什么错误 4. 预期结果是什么\"\n                },\n                \"category\": {\n                    \"type\": \"string\",\n                    \"description\": \"问题类型（可选）\",\n                    \"enum\": [\"功能请求\", \"Bug报告\", \"性能问题\", \"使用问题\", \"其他\"]\n                },\n                \"email\": {\n                    \"type\": \"string\",\n                    \"description\": \"联系邮箱（可选），用于开发者跟进问题\"\n                }\n            },\n            \"required\": [\"title\", \"description\"]\n        }";

	public SubmitDeveloperIssueTool(FeedbackService feedbackService)
	{
		feedbackService_0 = feedbackService ?? throw new ArgumentNullException("feedbackService");
	}

	[AsyncStateMachine(typeof(Struct222))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct222 stateMachine = default(Struct222);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.submitDeveloperIssueTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private string method_0(AIToolContext aitoolContext_0, string string_0, string string_1)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("## 问题描述");
		stringBuilder.AppendLine(string_0);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("## 应用信息");
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
		handler.AppendLiteral("- 版本：v");
		handler.AppendFormatted(ProductInfo.Version);
		stringBuilder3.AppendLine(ref handler);
		stringBuilder.AppendLine();
		if (aitoolContext_0.Document != null || aitoolContext_0.RevitAdapter != null)
		{
			stringBuilder.AppendLine("## Revit 环境信息");
			if (aitoolContext_0.RevitAdapter != null)
			{
				try
				{
					RevitVersion version = aitoolContext_0.RevitAdapter.Version;
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder4 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(14, 2, stringBuilder2);
					handler.AppendLiteral("- Revit 版本：");
					handler.AppendFormatted(version.DisplayVersion);
					handler.AppendLiteral(" (");
					handler.AppendFormatted(version.VersionString);
					handler.AppendLiteral(")");
					stringBuilder4.AppendLine(ref handler);
				}
				catch (Exception ex)
				{
					Logger.Debug("[SubmitDeveloperIssue] 获取 Revit 版本失败: " + ex.Message);
				}
			}
			if (aitoolContext_0.Document != null)
			{
				try
				{
					Type type = aitoolContext_0.Document.GetType();
					PropertyInfo property = type.GetProperty("Title");
					if (property != null)
					{
						string value = property.GetValue(aitoolContext_0.Document)?.ToString();
						if (!string.IsNullOrEmpty(value))
						{
							stringBuilder2 = stringBuilder;
							StringBuilder stringBuilder5 = stringBuilder2;
							handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
							handler.AppendLiteral("- 文档名称：");
							handler.AppendFormatted(value);
							stringBuilder5.AppendLine(ref handler);
						}
					}
					PropertyInfo property2 = type.GetProperty("PathName");
					if (property2 != null)
					{
						string value2 = property2.GetValue(aitoolContext_0.Document)?.ToString();
						if (!string.IsNullOrEmpty(value2))
						{
							stringBuilder2 = stringBuilder;
							StringBuilder stringBuilder6 = stringBuilder2;
							handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
							handler.AppendLiteral("- 文档路径：");
							handler.AppendFormatted(value2);
							stringBuilder6.AppendLine(ref handler);
						}
					}
				}
				catch (Exception ex2)
				{
					Logger.Debug("[SubmitDeveloperIssue] 获取文档信息失败: " + ex2.Message);
				}
			}
			stringBuilder.AppendLine();
		}
		if (!string.IsNullOrEmpty(aitoolContext_0.SessionId))
		{
			stringBuilder.AppendLine("## 会话信息");
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
			handler.AppendLiteral("- 会话 ID：");
			handler.AppendFormatted(aitoolContext_0.SessionId);
			stringBuilder7.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder8 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("- 提交时间：");
			handler.AppendFormatted(DateTime.Now, "yyyy-MM-dd HH:mm:ss");
			stringBuilder8.AppendLine(ref handler);
			stringBuilder.AppendLine();
		}
		stringBuilder.AppendLine("## 问题分类");
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder9 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
		handler.AppendLiteral("分类：");
		handler.AppendFormatted(string_1);
		stringBuilder9.AppendLine(ref handler);
		stringBuilder.AppendLine("来源：AI 对话自动提交");
		stringBuilder.AppendLine();
		return stringBuilder.ToString();
	}
}
