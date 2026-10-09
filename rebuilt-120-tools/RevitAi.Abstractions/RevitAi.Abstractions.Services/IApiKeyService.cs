using System.Threading.Tasks;

namespace RevitAi.Abstractions.Services;

public interface IApiKeyService
{
	Task<string> GetApiKeyAsync(string keyName);

	string GetApiKey(string keyName);

	Task UpdateLastUsedAsync(string keyName);

	Task SaveApiKeyAsync(string keyName, string keyValue, string provider, string? description = null);

	Task<string> GetTiandituTokenAsync();

	Task<string> GetGoogleMapsApiKeyAsync();
}
