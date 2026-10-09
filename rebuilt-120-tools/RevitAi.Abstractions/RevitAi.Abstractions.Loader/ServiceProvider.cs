using System;
using System.Collections.Concurrent;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Logging;

namespace RevitAi.Abstractions.Loader;

public static class ServiceProvider
{
	private static ILogger? _logger;

	private static IModuleLoader? _moduleLoader;

	private static IAuthManager? _authManager;

	private static readonly ConcurrentDictionary<Type, object?> _services = new ConcurrentDictionary<Type, object>();

	public static IAuthManager? AuthManager => _authManager;

	public static void Initialize(ILogger logger, IModuleLoader moduleLoader)
	{
		_logger = logger ?? throw new ArgumentNullException("logger");
		_moduleLoader = moduleLoader ?? throw new ArgumentNullException("moduleLoader");
	}

	public static void SetAuthManager(IAuthManager authManager)
	{
		_authManager = authManager ?? throw new ArgumentNullException("authManager");
	}

	public static ILogger GetLogger()
	{
		if (_logger == null)
		{
			throw new InvalidOperationException("ServiceProvider 未初始化，请先调用 Initialize() 方法");
		}
		return _logger;
	}

	public static IModuleLoader GetModuleLoader()
	{
		if (_moduleLoader == null)
		{
			throw new InvalidOperationException("ServiceProvider 未初始化，请先调用 Initialize() 方法");
		}
		return _moduleLoader;
	}

	public static void RegisterService(Type serviceType, object? serviceInstance)
	{
		if (serviceType == null)
		{
			throw new ArgumentNullException("serviceType");
		}
		_services.AddOrUpdate(serviceType, serviceInstance, (Type _, object? _) => serviceInstance);
	}

	public static object? GetService(Type serviceType)
	{
		if (serviceType == null)
		{
			throw new ArgumentNullException("serviceType");
		}
		_services.TryGetValue(serviceType, out object value);
		return value;
	}

	public static T? GetService<T>() where T : class
	{
		return GetService(typeof(T)) as T;
	}
}
