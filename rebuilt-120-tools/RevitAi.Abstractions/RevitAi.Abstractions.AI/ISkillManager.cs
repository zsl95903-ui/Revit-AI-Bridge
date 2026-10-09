using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI.Models;
using RevitAi.Abstractions.Common;

namespace RevitAi.Abstractions.AI;

public interface ISkillManager
{
	Task InitializeAsync();

	Result<List<SkillInfo>> GetSkillList();

	Result<SkillInfo?> GetSkillDetail(string skillName);

	Task<Result<List<AIToolResult>>> ExecuteSkillAsync(string skillName, IDictionary<string, object> variables, AIToolContext context, CancellationToken cancellationToken = default(CancellationToken));

	Task<bool> CheckAndUpdateFromCOSAsync(CancellationToken cancellationToken = default(CancellationToken));

	string GenerateSkillToolDefinition();

	string GetCurrentVersion();

	int GetSkillCount();
}
