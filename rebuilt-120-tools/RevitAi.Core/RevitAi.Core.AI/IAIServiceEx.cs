using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;

namespace RevitAi.Core.AI;

public interface IAIServiceEx
{
	Task<string> SendMessageAsync(string message, AIConfig config, CancellationToken cancellationToken = default(CancellationToken));

	IAsyncEnumerable<string> SendMessageStreamAsync(string message, AIConfig config, CancellationToken cancellationToken = default(CancellationToken));

	IAsyncEnumerable<string> SendMessageStreamAsync(string conversationHistory, AIConfig config, string? toolsDefinition = null, CancellationToken cancellationToken = default(CancellationToken));

	Task<string> SendMessageAsync(string message, AIConfig config, string? toolsDefinition = null, CancellationToken cancellationToken = default(CancellationToken));

	Task<AIResponseWithThinking> SendMessageWithUsageAsync(string message, AIConfig config, string? toolsDefinition = null, List<FileAttachment>? attachments = null, CancellationToken cancellationToken = default(CancellationToken));

	Task<string> SendMessageAsync(List<AIMessage> messages, AIConfig config, CancellationToken cancellationToken = default(CancellationToken));

	Task<string> SendMessageAsync(List<AIMessage> messages, AIConfig config, string? toolsDefinition, CancellationToken cancellationToken = default(CancellationToken));

	Task<AIResponseWithThinking> SendMessageWithUsageAsync(List<AIMessage> messages, AIConfig config, string? toolsDefinition = null, CancellationToken cancellationToken = default(CancellationToken));
}
