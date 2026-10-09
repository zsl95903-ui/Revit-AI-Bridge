using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RevitAi.Abstractions.AI;

public interface IFileStorageService
{
	Task<bool> SaveAsync<T>(string path, T data, CancellationToken cancellationToken = default(CancellationToken));

	Task<T?> LoadAsync<T>(string path, CancellationToken cancellationToken = default(CancellationToken));

	Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

	Task<List<string>> ListAsync(string directoryPath, string searchPattern = "*.json", CancellationToken cancellationToken = default(CancellationToken));

	void EnsureDirectoryExists(string path);

	bool FileExists(string path);
}
