using System.Net.Http;

namespace RevitAi.UI.Services;

public interface ISupabaseClientProxy
{
	HttpClient HttpClient { get; }

	string ApiKey { get; }

	string BaseUrl { get; }
}
