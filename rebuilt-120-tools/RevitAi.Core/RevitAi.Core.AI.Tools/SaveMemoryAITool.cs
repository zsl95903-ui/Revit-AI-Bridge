using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using ns7;

namespace RevitAi.Core.AI.Tools;

[AITool("save_memory", Category = "会话记忆", Description = "将需要记住的关键信息保存到当前任务的记忆中，供后续步骤使用。典型场景：消息包含图片、PDF 等附件时，附件内容仅在当前轮次可见，应在调用其他任何工具之前，先用本工具把从附件中提取的关键信息（尺寸、文字标注、数量、结论等）保存下来，确保后续步骤仍可引用这些内容。也可用于记录任务执行过程中的中间结论。", RequiresTransaction = false, RequiresActiveDocument = false)]
public sealed class SaveMemoryAITool : IAITool
{
	public string Name => "save_memory";

	public string Category => "会话记忆";

	public string Description => "将关键信息保存到当前任务记忆中，供后续步骤引用。处理图片/PDF 等附件时，必须在调用其他工具前先用本工具保存从附件中提取的关键信息，因为附件在后续轮次不可见。";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"content\": {\n                \"type\": \"string\",\n                \"description\": \"要记住的关键信息，应完整、具体。例如：'图纸标注：洞口尺寸500x500mm，距左侧轴线2000mm，共3个'\"\n            },\n            \"topic\": {\n                \"type\": \"string\",\n                \"description\": \"可选，信息的主题分类，例如：'图纸尺寸'、'用户偏好'、'中间结论'\"\n            }\n        },\n        \"required\": [\"content\"]\n    }";

	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		try
		{
			string parameter = context.GetParameter<string>("content", (string)null);
			if (string.IsNullOrWhiteSpace(parameter))
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("记忆内容不能为空"));
			}
			string parameter2 = context.GetParameter<string>("topic", "通用");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
			defaultInterpolatedStringHandler.AppendLiteral("✅ 已记住（主题: ");
			defaultInterpolatedStringHandler.AppendFormatted(parameter2);
			defaultInterpolatedStringHandler.AppendLiteral("）：");
			defaultInterpolatedStringHandler.AppendFormatted(parameter);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			return Task.FromResult<AIToolResult>(AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear() + "以上信息已保存到任务记忆中，你可以在后续步骤中直接引用这些内容。", (object)null));
		}
		catch (Exception ex)
		{
			Logger.Error("[SaveMemory] 保存记忆失败: " + ex.Message, ex);
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("保存记忆失败: " + ex.Message));
		}
	}
}
