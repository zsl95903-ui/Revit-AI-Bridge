using System.Collections.Generic;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;

namespace RevitAi.Core.Configuration;

public interface IConfigManager
{
	Task<Result<T>> GetAsync<T>(string key, T? defaultValue = default(T?));

	Task<Result> SetAsync<T>(string key, T value);

	Task<Result<bool>> ExistsAsync(string key);

	Task<Result> RemoveAsync(string key);

	Task<Result> SaveAsync();

	Task<Result> ReloadAsync();

	Task<Result> ClearAsync();

	Task<Result<string[]>> GetKeysInSectionAsync(string section);

	Task<Result<Dictionary<string, object>>> GetSectionAsync(string section);
}
