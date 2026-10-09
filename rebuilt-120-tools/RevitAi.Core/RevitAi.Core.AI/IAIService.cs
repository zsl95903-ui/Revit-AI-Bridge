using System.Threading;
using System.Threading.Tasks;

namespace RevitAi.Core.AI;

public interface IAIService
{
	Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default(CancellationToken));
}
