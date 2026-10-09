using System;
using System.Threading.Tasks;

namespace RevitAi.Abstractions.AI;

public interface IRevitExternalEventRequest
{
	Func<AIToolContext, Task<AIToolResult>> Action { get; set; }

	AIToolContext Context { get; set; }

	TaskCompletionSource<AIToolResult> TaskSource { get; set; }

	bool RequiresTransaction { get; set; }
}
