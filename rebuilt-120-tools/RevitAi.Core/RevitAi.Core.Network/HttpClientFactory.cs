using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Network;
using ns7;

namespace RevitAi.Core.Network;

public sealed class HttpClientFactory : IHttpClientFactory, IDisposable
{
	private readonly IWebProxy? iwebProxy_0;

	private bool bool_0;

	public HttpClientFactory()
	{
		try
		{
			iwebProxy_0 = WebRequest.GetSystemWebProxy();
			if (iwebProxy_0 != null)
			{
				try
				{
					Uri destination = new Uri("https://www.example.com");
					smethod_0(iwebProxy_0.GetProxy(destination)?.ToString());
					return;
				}
				catch
				{
					Logger.Info("[HttpClientFactory] 检测到系统代理（地址未知）");
					return;
				}
			}
			Logger.Info("[HttpClientFactory] 未检测到系统代理");
		}
		catch (Exception ex)
		{
			Logger.Warning("[HttpClientFactory] 获取系统代理失败: " + ex.Message);
		}
	}

	public HttpClient CreateClient(string? baseAddress = null, int timeoutSeconds = 30)
	{
		if (bool_0)
		{
			throw new ObjectDisposedException("HttpClientFactory");
		}
		HttpClient httpClient = new HttpClient(method_0())
		{
			Timeout = TimeSpan.FromSeconds(timeoutSeconds)
		};
		if (!string.IsNullOrEmpty(baseAddress))
		{
			httpClient.BaseAddress = new Uri(baseAddress);
		}
		return httpClient;
	}

	private HttpClientHandler method_0()
	{
		HttpClientHandler httpClientHandler = new HttpClientHandler
		{
			UseProxy = true,
			SslProtocols = (SslProtocols.Tls12 | SslProtocols.Tls13),
			AllowAutoRedirect = true,
			MaxAutomaticRedirections = 5,
			UseCookies = false
		};
		if (iwebProxy_0 != null)
		{
			httpClientHandler.Proxy = iwebProxy_0;
		}
		return httpClientHandler;
	}

	private static string smethod_0(string? string_0)
	{
		if (string.IsNullOrEmpty(string_0))
		{
			return "<unknown>";
		}
		try
		{
			Uri uri = new Uri(string_0);
			return uri.Scheme + "://" + uri.Host + "/<masked>";
		}
		catch
		{
			return "<invalid>";
		}
	}

	public void Dispose()
	{
		if (!bool_0)
		{
			bool_0 = true;
		}
	}
}
