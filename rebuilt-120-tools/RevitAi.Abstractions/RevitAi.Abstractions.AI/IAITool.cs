using System.Threading;
using System.Threading.Tasks;

namespace RevitAi.Abstractions.AI;

public interface IAITool
{
	string Name { get; }

	string Description { get; }

	string Category { get; }

	string ParametersSchema { get; }

	Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken));
}
