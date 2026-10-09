using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Memory.Models;
using RevitAi.Core.AI;
using ns7;

namespace RevitAi.Core.Memory;

public class MemorySummarizer
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct41 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public List<ObservationRecord> list_0;

		public string string_0;

		public MemorySummarizer memorySummarizer_0;

		public string string_1;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			MemorySummarizer memorySummarizer = memorySummarizer_0;
			string result;
			string message = default(string);
			if (num != 0)
			{
				if (!list_0.Any())
				{
					result = "会话 \"" + string_0 + "\" 无重要活动记录。";
					goto IL_0191;
				}
				string value = memorySummarizer.method_0(list_0);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(270, 3);
				defaultInterpolatedStringHandler.AppendLiteral("请为以下 AI 助手会话生成简洁的摘要（中文）：\n\n会话标题：");
				defaultInterpolatedStringHandler.AppendFormatted(string_0);
				defaultInterpolatedStringHandler.AppendLiteral("\n会话 ID：");
				defaultInterpolatedStringHandler.AppendFormatted(string_1);
				defaultInterpolatedStringHandler.AppendLiteral("\n\n");
				defaultInterpolatedStringHandler.AppendFormatted(value);
				defaultInterpolatedStringHandler.AppendLiteral("\n\n请生成结构化摘要，包含以下部分：\n1. **主要工作**：本次会话完成的主要任务\n2. **关键决策**：重要的架构决策或技术选择\n3. **修改文件**：所有被修改的文件列表\n4. **问题解决**：遇到的 Bug 和解决方案\n5. **下次建议**：下次继续时的建议起始点\n\n摘要格式要求：\n- 使用 Markdown 格式\n- 保持简洁但信息完整\n- 文件路径使用项目相对路径（从 RevitAi/ 开始）\n- 代码引用使用格式：`文件路径:行号`");
				message = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			try
			{
				TaskAwaiter<string> awaiter;
				if (num != 0)
				{
					awaiter = memorySummarizer.claudeAIService_0.SendMessageAsync(message).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
				}
				string result2 = awaiter.GetResult();
				Logger.Info("[MemorySummarizer] 成功生成会话摘要: " + string_1);
				result = result2;
			}
			catch (Exception ex)
			{
				Logger.Error("[MemorySummarizer] 生成摘要失败: " + ex.Message);
				result = memorySummarizer.method_1(list_0, string_0);
			}
			goto IL_0191;
			IL_0191:
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly ClaudeAIService claudeAIService_0;

	public MemorySummarizer(ClaudeAIService aiService)
	{
		claudeAIService_0 = aiService ?? throw new ArgumentNullException("aiService");
	}

	[AsyncStateMachine(typeof(Struct41))]
	public Task<string> GenerateSessionSummaryAsync(string sessionId, List<ObservationRecord> observations, string sessionTitle)
	{
		Struct41 stateMachine = default(Struct41);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.memorySummarizer_0 = this;
		stateMachine.string_1 = sessionId;
		stateMachine.list_0 = observations;
		stateMachine.string_0 = sessionTitle;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public ObservationRecord? ExtractObservationFromToolCall(string sessionId, string toolName, string toolInput, string toolOutput, List<string> relatedFiles)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		ObservationType type = method_2(toolName, toolOutput);
		if (!method_3(toolName, toolOutput))
		{
			return null;
		}
		string content = method_4(toolName, toolInput, toolOutput);
		return new ObservationRecord
		{
			SessionId = sessionId,
			Type = type,
			Content = content,
			Timestamp = DateTime.Now,
			RelatedFiles = relatedFiles,
			Importance = method_5(toolName, toolOutput)
		};
	}

	private string method_0(List<ObservationRecord> list_0)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("## 重要活动记录");
		stringBuilder.AppendLine();
		foreach (IGrouping<ObservationType, ObservationRecord> item in list_0.GroupBy(delegate(ObservationRecord observationRecord_0)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return observationRecord_0.Type;
		}))
		{
			string value = method_6(item.Key);
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(9, 2, stringBuilder2);
			handler.AppendLiteral("### ");
			handler.AppendFormatted(value);
			handler.AppendLiteral(" (");
			handler.AppendFormatted(item.Count());
			handler.AppendLiteral(" 项)");
			stringBuilder3.AppendLine(ref handler);
			foreach (ObservationRecord item2 in item.OrderByDescending((ObservationRecord observationRecord_0) => observationRecord_0.Importance).Take(5))
			{
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(8, 2, stringBuilder2);
				handler.AppendLiteral("- [");
				handler.AppendFormatted(item2.Importance);
				handler.AppendLiteral("/10] ");
				handler.AppendFormatted(item2.Content);
				stringBuilder4.AppendLine(ref handler);
			}
			stringBuilder.AppendLine();
		}
		List<string> list = (from string_0 in list_0.SelectMany((ObservationRecord observationRecord_0) => observationRecord_0.RelatedFiles).Distinct()
			orderby string_0
			select string_0).ToList();
		if (list.Any())
		{
			stringBuilder.AppendLine("### 涉及的文件");
			foreach (string item3 in list)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
				handler.AppendLiteral("- ");
				handler.AppendFormatted(item3);
				stringBuilder5.AppendLine(ref handler);
			}
		}
		return stringBuilder.ToString();
	}

	private string method_1(List<ObservationRecord> list_0, string string_0)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
		handler.AppendLiteral("# ");
		handler.AppendFormatted(string_0);
		stringBuilder3.AppendLine(ref handler);
		stringBuilder.AppendLine();
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder4 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
		handler.AppendLiteral("*生成时间：");
		handler.AppendFormatted(DateTime.Now, "yyyy-MM-dd HH:mm");
		handler.AppendLiteral("*");
		stringBuilder4.AppendLine(ref handler);
		stringBuilder.AppendLine();
		foreach (IGrouping<ObservationType, ObservationRecord> item in list_0.GroupBy(delegate(ObservationRecord observationRecord_0)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return observationRecord_0.Type;
		}))
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(6, 2, stringBuilder2);
			handler.AppendLiteral("## ");
			handler.AppendFormatted(method_6(item.Key));
			handler.AppendLiteral(" (");
			handler.AppendFormatted(item.Count());
			handler.AppendLiteral(")");
			stringBuilder5.AppendLine(ref handler);
			foreach (ObservationRecord item2 in item.OrderByDescending((ObservationRecord observationRecord_0) => observationRecord_0.Timestamp))
			{
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder6 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
				handler.AppendLiteral("- ");
				handler.AppendFormatted(item2.Content);
				stringBuilder6.AppendLine(ref handler);
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	private ObservationType method_2(string string_0, string string_1)
	{
		if (!string_0.Contains("Create") && !string_0.Contains("Add"))
		{
			if (!string_0.Contains("Fix") && !string_0.Contains("Resolve"))
			{
				if (!string_0.Contains("Refactor") && !string_0.Contains("Rename"))
				{
					if (!string_0.Contains("Get") && !string_0.Contains("Find") && !string_0.Contains("Search"))
					{
						return (ObservationType)5;
					}
					return (ObservationType)3;
				}
				return (ObservationType)4;
			}
			return (ObservationType)1;
		}
		return (ObservationType)2;
	}

	private bool method_3(string string_0, string string_1)
	{
		if (string_0.Contains("Read") && string_1.Length < 100)
		{
			return false;
		}
		if (!string_0.Contains("Glob") && !string_0.Contains("Grep"))
		{
			return true;
		}
		return string_1.Length > 500;
	}

	private string method_4(string string_0, string string_1, string string_2)
	{
		string value = ((string_2.Length > 500) ? (string_2.Substring(0, 500) + "...") : string_2);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 3);
		defaultInterpolatedStringHandler.AppendLiteral("**");
		defaultInterpolatedStringHandler.AppendFormatted(string_0);
		defaultInterpolatedStringHandler.AppendLiteral("**\n输入: ");
		defaultInterpolatedStringHandler.AppendFormatted(string_1);
		defaultInterpolatedStringHandler.AppendLiteral("\n输出: ");
		defaultInterpolatedStringHandler.AppendFormatted(value);
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	private int method_5(string string_0, string string_1)
	{
		int num = 5;
		if (string_0.Contains("Create") || string_0.Contains("Write"))
		{
			num += 3;
		}
		if (string_0.Contains("Fix") || string_0.Contains("Error"))
		{
			num += 2;
		}
		if (string_1.Length > 1000)
		{
			num++;
		}
		return Math.Min(10, num);
	}

	private string method_6(ObservationType observationType_0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		return (int)observationType_0 switch
		{
			0 => "决策", 
			1 => "Bug 修复", 
			2 => "新功能", 
			3 => "发现", 
			4 => "重构", 
			5 => "工具调用", 
			6 => "用户反馈", 
			_ => "其他", 
		};
	}
}
