using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitAi.Abstractions.AI;

public interface IAICodeToolInvoker
{
	ToolCallResult Call(string toolName, object? parameters = null);

	Task<ToolCallResult> CallAsync(string toolName, object? parameters = null);

	IEnumerable<string> ListTools();

	string? GetToolDescription(string toolName);
}
