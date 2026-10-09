using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Memory.Models;
using RevitAi.Core.AI;
using ns7;

namespace RevitAi.Core.Memory;

public class RevitMemorySummarizer
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct42 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public List<ObservationRecord> list_0;

		public string string_0;

		public RevitMemorySummarizer revitMemorySummarizer_0;

		public string string_1;

		private TaskAwaiter<string> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			RevitMemorySummarizer revitMemorySummarizer = revitMemorySummarizer_0;
			string result;
			string message = default(string);
			if (num != 0)
			{
				if (!list_0.Any())
				{
					result = "会话 \"" + string_0 + "\" 无重要活动记录。";
					goto IL_0191;
				}
				string value = revitMemorySummarizer.method_0(list_0);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(304, 3);
				defaultInterpolatedStringHandler.AppendLiteral("请为以下 Revit AI 助手会话生成简洁的摘要（中文）：\n\n会话标题：");
				defaultInterpolatedStringHandler.AppendFormatted(string_0);
				defaultInterpolatedStringHandler.AppendLiteral("\n会话 ID：");
				defaultInterpolatedStringHandler.AppendFormatted(string_1);
				defaultInterpolatedStringHandler.AppendLiteral("\n\n");
				defaultInterpolatedStringHandler.AppendFormatted(value);
				defaultInterpolatedStringHandler.AppendLiteral("\n\n请生成结构化摘要，包含以下部分：\n1. **模型操作**：对 Revit 模型执行的操作（创建元素、修改参数、视图操作等）\n2. **族库操作**：族搜索和加载记录\n3. **遇到的问题**：操作中遇到的错误或问题\n4. **解决方案**：问题的解决方法\n5. **项目信息**：涉及的项目、文档、族类型等\n\n摘要格式要求：\n- 使用 Markdown 格式\n- 保持简洁但信息完整\n- 族名称和类别使用中文\n- 元素类型使用中文（如：柱、梁、墙、楼板等）\n- 参数名称保持原始名称\n- 重点关注用户的目标和结果");
				message = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			try
			{
				TaskAwaiter<string> awaiter;
				if (num != 0)
				{
					awaiter = revitMemorySummarizer.claudeAIService_0.SendMessageAsync(message).GetAwaiter();
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
				Logger.Info("[RevitMemorySummarizer] 成功生成会话摘要: " + string_1);
				result = result2;
			}
			catch (Exception ex)
			{
				Logger.Error("[RevitMemorySummarizer] 生成摘要失败: " + ex.Message);
				result = revitMemorySummarizer.method_1(list_0, string_0);
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

	public RevitMemorySummarizer(ClaudeAIService aiService)
	{
		claudeAIService_0 = aiService ?? throw new ArgumentNullException("aiService");
	}

	[AsyncStateMachine(typeof(Struct42))]
	public Task<string> GenerateSessionSummaryAsync(string sessionId, List<ObservationRecord> observations, string sessionTitle)
	{
		Struct42 stateMachine = default(Struct42);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.revitMemorySummarizer_0 = this;
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
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("## 重要活动记录");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, ObservationRecord> item in from observationRecord_0 in list_0
			group observationRecord_0 by method_6(method_9(observationRecord_0.Content)) into igrouping_0
			orderby igrouping_0.Key
			select igrouping_0)
		{
			string key = item.Key;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(9, 2, stringBuilder2);
			handler.AppendLiteral("### ");
			handler.AppendFormatted(key);
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
		List<string> list = method_10(list_0);
		if (list.Any())
		{
			stringBuilder.AppendLine("### 涉及的族和元素类型");
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
			handler.AppendFormatted(method_11(item.Key));
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
		if (!string_0.Contains("Create") && !string_0.Contains("load_family"))
		{
			if (!string_0.Contains("Modify") && !string_0.Contains("Update") && !string_0.Contains("Set"))
			{
				if (!string_0.Contains("Fix") && !string_0.Contains("Error") && !string_1.Contains("失败") && !string_1.Contains("错误"))
				{
					if (!string_0.Contains("Get") && !string_0.Contains("Search") && !string_0.Contains("Query"))
					{
						if (!string_0.Contains("Zoom") && !string_0.Contains("View"))
						{
							return (ObservationType)5;
						}
						return (ObservationType)3;
					}
					return (ObservationType)3;
				}
				return (ObservationType)1;
			}
			return (ObservationType)4;
		}
		return (ObservationType)2;
	}

	private bool method_3(string string_0, string string_1)
	{
		if (string_0.Contains("Get") && string_1.Length < 100)
		{
			return false;
		}
		if (string_0.Contains("Zoom"))
		{
			return false;
		}
		return true;
	}

	private string method_4(string string_0, string string_1, string string_2)
	{
		string text = ((string_2.Length > 500) ? (string_2.Substring(0, 500) + "...") : string_2);
		if (string_0.Contains("family"))
		{
			return "**" + string_0 + "**\n" + text;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 3);
		defaultInterpolatedStringHandler.AppendLiteral("**");
		defaultInterpolatedStringHandler.AppendFormatted(string_0);
		defaultInterpolatedStringHandler.AppendLiteral("**\n输入: ");
		defaultInterpolatedStringHandler.AppendFormatted(string_1);
		defaultInterpolatedStringHandler.AppendLiteral("\n输出: ");
		defaultInterpolatedStringHandler.AppendFormatted(text);
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	private int method_5(string string_0, string string_1)
	{
		int num = 5;
		if (string_0.Contains("family"))
		{
			num += 2;
		}
		if (string_0.Contains("Create") || string_0.Contains("load"))
		{
			num += 2;
		}
		if (string_1.Contains("失败") || string_1.Contains("错误"))
		{
			num += 2;
		}
		if (string_1.Length > 1000)
		{
			num++;
		}
		return Math.Min(10, num);
	}

	private string method_6(string string_0)
	{
		try
		{
			IAITool tool = AIToolRegistry.Instance.GetTool(string_0);
			if (tool != null)
			{
				return method_7(tool.Category);
			}
		}
		catch
		{
		}
		return method_8(string_0);
	}

	private string method_7(string string_0)
	{
		if (new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["元素创建"] = "模型创建",
			["元素修改"] = "模型修改",
			["元素删除"] = "模型删除",
			["元素操作"] = "模型修改",
			["元素查询"] = "信息查询",
			["视图查询"] = "信息查询",
			["查询"] = "信息查询",
			["查询工具"] = "信息查询",
			["机电查询"] = "信息查询",
			["轴网查询"] = "信息查询",
			["标高查询"] = "信息查询",
			["几何分析"] = "信息查询",
			["族管理"] = "族操作",
			["视图管理"] = "视图操作",
			["视图创建"] = "视图操作",
			["视图高级操作"] = "视图操作",
			["参数操作"] = "参数设置",
			["参数管理"] = "参数设置",
			["复合层结构"] = "参数设置",
			["材料管理"] = "材料操作",
			["工作集管理"] = "协同操作",
			["链接管理"] = "协同操作",
			["文档操作"] = "图纸操作",
			["数据导出"] = "数据操作",
			["注释与标记"] = "标注操作",
			["房间与分区"] = "模型创建",
			["元素选择"] = "信息查询",
			["在线族库"] = "族库操作",
			["CAD 图纸识别"] = "CAD 操作"
		}.TryGetValue(string_0, out var value))
		{
			return value;
		}
		return string_0;
	}

	private string method_8(string string_0)
	{
		if (!string_0.Contains("Family") && !string_0.Contains("family"))
		{
			if (!string_0.Contains("Create") && !string_0.Contains("Add"))
			{
				if (!string_0.Contains("Delete") && !string_0.Contains("Remove"))
				{
					if (!string_0.Contains("Modify") && !string_0.Contains("Set") && !string_0.Contains("Move") && !string_0.Contains("Copy") && !string_0.Contains("Mirror") && !string_0.Contains("Array") && !string_0.Contains("Rotate"))
					{
						if (!string_0.Contains("Get") && !string_0.Contains("Query") && !string_0.Contains("Search") && !string_0.Contains("Find"))
						{
							if (!string_0.Contains("View") && !string_0.Contains("Zoom") && !string_0.Contains("Activate"))
							{
								if (string_0.Contains("Parameter"))
								{
									return "参数设置";
								}
								if (string_0.Contains("Material"))
								{
									return "材料操作";
								}
								if (!string_0.Contains("Cad") && !string_0.Contains("CAD"))
								{
									if (string_0.Contains("Workset"))
									{
										return "协同操作";
									}
									if (string_0.Contains("Link"))
									{
										return "协同操作";
									}
									return "其他操作";
								}
								return "CAD 操作";
							}
							return "视图操作";
						}
						if (string_0.Contains("Geometry"))
						{
							return "几何分析";
						}
						if (string_0.Contains("Material"))
						{
							return "材料查询";
						}
						if (string_0.Contains("Parameter"))
						{
							return "参数查询";
						}
						if (string_0.Contains("View"))
						{
							return "视图查询";
						}
						if (string_0.Contains("Family"))
						{
							return "族查询";
						}
						if (string_0.Contains("Level"))
						{
							return "标高查询";
						}
						if (string_0.Contains("Grid"))
						{
							return "轴网查询";
						}
						return "信息查询";
					}
					return "模型修改";
				}
				return "模型删除";
			}
			if (string_0.Contains("View"))
			{
				return "视图操作";
			}
			if (string_0.Contains("Sheet"))
			{
				return "图纸操作";
			}
			if (!string_0.Contains("Level") && !string_0.Contains("Grid"))
			{
				return "模型创建";
			}
			return "基准构件";
		}
		if (!string_0.Contains("Library") && !string_0.Contains("Search"))
		{
			return "族操作";
		}
		return "族库操作";
	}

	private string method_9(string string_0)
	{
		if (string_0.StartsWith("**") && string_0.Contains("**"))
		{
			int num = string_0.IndexOf("**", 2);
			if (num > 0)
			{
				return string_0.Substring(2, num - 2);
			}
		}
		return "未知工具";
	}

	private List<string> method_10(List<ObservationRecord> list_0)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (ObservationRecord item in list_0)
		{
			string content = item.Content;
			string[] array = new string[9]
			{
				"柱",
				"梁",
				"墙",
				"楼板",
				"门",
				"窗",
				"族",
				"家具",
				"卫浴"
			};
			foreach (string text in array)
			{
				if (content.Contains(text))
				{
					hashSet.Add(text);
				}
			}
			if (content.Contains("Element") || content.Contains("元素"))
			{
				hashSet.Add("模型元素");
			}
		}
		return hashSet.ToList();
	}

	private string method_11(ObservationType observationType_0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		return (int)observationType_0 switch
		{
			0 => "决策", 
			1 => "问题解决", 
			2 => "模型操作", 
			3 => "信息查询", 
			4 => "模型修改", 
			5 => "工具调用", 
			6 => "用户反馈", 
			_ => "其他", 
		};
	}

	[CompilerGenerated]
	private string method_12(ObservationRecord observationRecord_0)
	{
		return method_6(method_9(observationRecord_0.Content));
	}
}
