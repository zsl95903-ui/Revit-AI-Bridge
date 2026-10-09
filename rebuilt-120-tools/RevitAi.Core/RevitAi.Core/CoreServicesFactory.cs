using System;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Network;
using RevitAi.Abstractions.Services;
using RevitAi.Core.AI;
using RevitAi.Core.Authentication;
using RevitAi.Core.CAD;
using RevitAi.Core.Configuration;
using RevitAi.Core.Network;
using RevitAi.Core.Security;
using ns7;

namespace RevitAi.Core;

public static class CoreServicesFactory
{
	public static IHttpClientFactory CreateHttpClientFactory()
	{
		try
		{
			return (IHttpClientFactory)(object)new HttpClientFactory();
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 HttpClientFactory: " + ex.Message, ex);
		}
	}

	public static INativeCryptoService CreateCryptoService()
	{
		try
		{
			return new NativeCryptoService();
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 NativeCryptoService: " + ex.Message, ex);
		}
	}

	public static IConfigManager CreateConfigManager(INativeCryptoService cryptoService)
	{
		if (cryptoService == null)
		{
			throw new ArgumentNullException("cryptoService");
		}
		try
		{
			return new ConfigManager(cryptoService);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 ConfigManager: " + ex.Message, ex);
		}
	}

	public static ISupabaseClient CreateSupabaseClient(INativeCryptoService cryptoService, IHttpClientFactory? httpClientFactory = null)
	{
		if (cryptoService == null)
		{
			throw new ArgumentNullException("cryptoService");
		}
		try
		{
			return new SupabaseClient(cryptoService, httpClientFactory);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 SupabaseClient: " + ex.Message, ex);
		}
	}

	public static IAuthManager CreateAuthManager(ISupabaseClient supabaseClient, INativeCryptoService cryptoService, IConfigManager configManager, IHttpClientFactory? httpClientFactory = null)
	{
		if (supabaseClient == null)
		{
			throw new ArgumentNullException("supabaseClient");
		}
		if (cryptoService == null)
		{
			throw new ArgumentNullException("cryptoService");
		}
		if (configManager == null)
		{
			throw new ArgumentNullException("configManager");
		}
		try
		{
			return (IAuthManager)(object)new LocalAuthManager();
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 AuthManager: " + ex.Message, ex);
		}
	}

	public static ApiKeyManager CreateApiKeyManager(INativeCryptoService cryptoService, ISupabaseClient supabaseClient)
	{
		if (cryptoService == null)
		{
			throw new ArgumentNullException("cryptoService");
		}
		if (supabaseClient == null)
		{
			throw new ArgumentNullException("supabaseClient");
		}
		try
		{
			return new ApiKeyManager(cryptoService, supabaseClient);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 ApiKeyManager: " + ex.Message, ex);
		}
	}

	public static IAIService CreateAIService(ApiKeyManager apiKeyManager, ISupabaseClient supabaseClient, IHttpClientFactory? httpClientFactory = null)
	{
		if (apiKeyManager == null)
		{
			throw new ArgumentNullException("apiKeyManager");
		}
		if (supabaseClient == null)
		{
			throw new ArgumentNullException("supabaseClient");
		}
		try
		{
			return new ClaudeAIService(apiKeyManager, supabaseClient, httpClientFactory);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 ClaudeAIService: " + ex.Message, ex);
		}
	}

	public static void CreateCoreServices(out IAuthManager authManager, out INativeCryptoService cryptoService, out ISupabaseClient supabaseClient, out IConfigManager configManager, out IHttpClientFactory httpClientFactory)
	{
		try
		{
			httpClientFactory = CreateHttpClientFactory();
			cryptoService = CreateCryptoService();
			configManager = CreateConfigManager(cryptoService);
			supabaseClient = CreateSupabaseClient(cryptoService, httpClientFactory);
			authManager = CreateAuthManager(supabaseClient, cryptoService, configManager, httpClientFactory);
		}
		catch (Exception ex)
		{
			cryptoService = null;
			supabaseClient = null;
			configManager = null;
			authManager = null;
			httpClientFactory = null;
			throw new InvalidOperationException("创建核心服务堆栈失败: " + ex.Message, ex);
		}
	}

	public static void CreateCoreServices(out IAuthManager authManager, out INativeCryptoService cryptoService, out ISupabaseClient supabaseClient, out IConfigManager configManager)
	{
		CreateCoreServices(out authManager, out cryptoService, out supabaseClient, out configManager, out IHttpClientFactory _);
	}

	public static ICADFileService CreateCADFileService()
	{
		try
		{
			return (ICADFileService)(object)new CADFileService();
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 CADFileService: " + ex.Message, ex);
		}
	}

	public static ICADAnalyzerService CreateCADAnalyzerService(ICADFileService cadFileService)
	{
		if (cadFileService == null)
		{
			throw new ArgumentNullException("cadFileService");
		}
		try
		{
			return (ICADAnalyzerService)(object)new CADAnalyzerService(cadFileService);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 CADAnalyzerService: " + ex.Message, ex);
		}
	}

	public static IPipeNetworkModelingService CreatePipeNetworkModelingService(IExcelDataService excelDataService)
	{
		if (excelDataService == null)
		{
			throw new ArgumentNullException("excelDataService");
		}
		try
		{
			return (IPipeNetworkModelingService)(object)new PipeNetworkModelingService(excelDataService);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("无法创建 PipeNetworkModelingService: " + ex.Message, ex);
		}
	}
}
