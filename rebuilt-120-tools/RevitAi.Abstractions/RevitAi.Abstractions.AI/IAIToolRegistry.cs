using System.Collections.Generic;

namespace RevitAi.Abstractions.AI;

public interface IAIToolRegistry
{
	void RegisterTool(IAITool tool);

	void UnregisterTool(string toolName);

	IAITool? GetTool(string toolName);

	IEnumerable<IAITool> GetAllTools();

	string GetToolsDefinitionForAI();

	bool ContainsTool(string toolName);

	string GetToolsSummaryForAI();

	string? GetToolDetailForAI(string toolName);

	IEnumerable<string> GetToolCategories();

	IEnumerable<IAITool> GetToolsByCategory(string category);

	string GetCategorizedToolsSummary();
}
