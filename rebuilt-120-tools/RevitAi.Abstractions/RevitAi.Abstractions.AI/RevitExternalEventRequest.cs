using System;
using System.Threading.Tasks;

namespace RevitAi.Abstractions.AI;

public sealed class RevitExternalEventRequest : IRevitExternalEventRequest
{
	public Func<AIToolContext, Task<AIToolResult>> Action { get; set; }

	public AIToolContext Context { get; set; }

	public TaskCompletionSource<AIToolResult> TaskSource { get; set; }

	public bool RequiresTransaction { get; set; } = true;
}
