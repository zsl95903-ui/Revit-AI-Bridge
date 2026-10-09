using System;
using System.Net.Http;

namespace RevitAi.Abstractions.Network;

public interface IHttpClientFactory : IDisposable
{
	HttpClient CreateClient(string? baseAddress = null, int timeoutSeconds = 30);
}
