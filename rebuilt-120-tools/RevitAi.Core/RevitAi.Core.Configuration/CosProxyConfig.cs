using System;
using System.Runtime.CompilerServices;
using ns7;

namespace RevitAi.Core.Configuration;

public static class CosProxyConfig
{
	[CompilerGenerated]
	private static string string_0 = "https://astools.tech";

	public const string PresignedUrlPath = "/api/cos/presigned-url";

	public const string UploadPath = "/api/cos/upload";

	public const string VerifyPath = "/api/verify-cos";

	public static string ProxyBaseUrl
	{
		[CompilerGenerated]
		get
		{
			return string_0;
		}
		[CompilerGenerated]
		set
		{
			string_0 = value;
		}
	}

	public static string GetPresignedUrlUrl()
	{
		return ProxyBaseUrl.TrimEnd('/') + "/api/cos/presigned-url";
	}

	public static string GetUploadUrl()
	{
		return ProxyBaseUrl.TrimEnd('/') + "/api/cos/upload";
	}

	public static string GetVerifyUrl()
	{
		return ProxyBaseUrl.TrimEnd('/') + "/api/verify-cos";
	}

	public static void LoadConfig()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("COS_PROXY_URL");
		if (!string.IsNullOrEmpty(environmentVariable))
		{
			ProxyBaseUrl = environmentVariable;
		}
	}
}
