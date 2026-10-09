using System;
using System.Net.Http;

namespace RevitAi.UI.Services;

public class SupabaseHttpClientProxy : ISupabaseClientProxy
{
	public HttpClient HttpClient { get; }

	public string ApiKey { get; }

	public string BaseUrl { get; }

	public SupabaseHttpClientProxy(HttpClient httpClient, string apiKey = "", string baseUrl = "")
	{
		HttpClient = httpClient ?? throw new ArgumentNullException("httpClient");
		ApiKey = apiKey;
		BaseUrl = baseUrl;
	}

	public SupabaseHttpClientProxy(HttpClient httpClient)
		: this(httpClient, "", "")
	{
	}
}
