using System;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.Feedback;
using ns7;

namespace RevitAi.Core.AI.Tools;

public static class FeedbackAITools
{
	public static void RegisterTools(IAIToolRegistry toolRegistry, FeedbackService feedbackService)
	{
		if (toolRegistry == null)
		{
			Logger.Warning("[FeedbackAI] 工具注册表为 null，无法注册反馈 AI 工具");
			return;
		}
		if (feedbackService == null)
		{
			Logger.Warning("[FeedbackAI] 反馈服务为 null，无法注册反馈 AI 工具");
			return;
		}
		try
		{
			SubmitDeveloperIssueTool submitDeveloperIssueTool = new SubmitDeveloperIssueTool(feedbackService);
			toolRegistry.RegisterTool((IAITool)(object)submitDeveloperIssueTool);
		}
		catch (Exception ex)
		{
			Logger.Error("[FeedbackAI] 注册反馈 AI 工具失败: " + ex.Message, ex);
		}
	}
}
