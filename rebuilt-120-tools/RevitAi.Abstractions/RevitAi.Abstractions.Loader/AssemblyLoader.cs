using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;

namespace RevitAi.Abstractions.Loader;

public static class AssemblyLoader
{
	private static readonly string[] BaseSearchPaths = new string[5] { ".", ".\\R_Legacy", ".\\R_Modern", ".\\R2024", ".\\R_Next" };

	public static Assembly LoadAdapterAssembly(RevitVersionInfo versionInfo)
	{
		string assemblyName = versionInfo.AssemblyName;
		string runtimeDirectoryName = versionInfo.RuntimeDirectoryName;
		string text = FindAssemblyInRuntime(assemblyName, runtimeDirectoryName);
		if (text == null)
		{
			throw new FileNotFoundException($"未找到适配层程序集: {assemblyName}.dll\n运行时环境: {runtimeDirectoryName}\n已搜索路径:\n{string.Join("\n", GetEffectiveSearchPaths(runtimeDirectoryName))}");
		}
		try
		{
			return Assembly.LoadFrom(text);
		}
		catch (Exception ex)
		{
			Logger.Error("加载适配层程序集失败: " + assemblyName, ex);
			throw new InvalidOperationException("加载适配层程序集失败: " + assemblyName, ex);
		}
	}

	private static string? FindAssemblyInRuntime(string assemblyName, string runtimeDir)
	{
		string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		if (directoryName == null)
		{
			Logger.Error("无法获取 Loader 所在目录");
			return null;
		}
		string text = Path.Combine(directoryName, runtimeDir, assemblyName + ".dll");
		if (File.Exists(text))
		{
			return text;
		}
		string text2 = Path.Combine(directoryName, assemblyName + ".dll");
		if (File.Exists(text2))
		{
			return text2;
		}
		string[] baseSearchPaths = BaseSearchPaths;
		foreach (string path in baseSearchPaths)
		{
			string text3 = Path.Combine(directoryName, path, assemblyName + ".dll");
			if (File.Exists(text3))
			{
				return text3;
			}
		}
		return null;
	}

	private static string[] GetEffectiveSearchPaths(string runtimeDir)
	{
		string text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
		List<string> list = new List<string>();
		list.Add(Path.Combine(text, runtimeDir));
		list.Add(text);
		string[] baseSearchPaths = BaseSearchPaths;
		foreach (string path in baseSearchPaths)
		{
			list.Add(Path.Combine(text, path));
		}
		return list.ToArray();
	}

	public static object? CreateAdapterInstance(Assembly adapterAssembly, string interfaceTypeName)
	{
		try
		{
			Type type = adapterAssembly.GetType(interfaceTypeName);
			if (type == null)
			{
				Logger.Error("未找到接口类型: " + interfaceTypeName);
				return null;
			}
			Type[] types = adapterAssembly.GetTypes();
			foreach (Type type2 in types)
			{
				if (type.IsAssignableFrom(type2) && !type2.IsInterface && !type2.IsAbstract)
				{
					object? result = Activator.CreateInstance(type2);
					Logger.Info("成功创建适配器实例: " + type2.FullName);
					return result;
				}
			}
			Logger.Error("未找到实现接口 " + interfaceTypeName + " 的类型");
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("创建适配器实例失败", ex);
			return null;
		}
	}

	public static IRevitAdapter? CreateRevitAdapter(Assembly adapterAssembly)
	{
		try
		{
			Type[] types;
			try
			{
				types = adapterAssembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				Logger.Error("GetTypes() 调用失败，部分类型无法加载:");
				if (ex.LoaderExceptions != null)
				{
					Exception[] loaderExceptions = ex.LoaderExceptions;
					foreach (Exception ex2 in loaderExceptions)
					{
						if (ex2 != null)
						{
							Logger.Error("  - " + ex2.Message);
						}
					}
				}
				throw;
			}
			Type type = types.FirstOrDefault((Type t) => typeof(IRevitAdapter).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
			if (type == null)
			{
				Logger.Error("未找到实现 " + typeof(IRevitAdapter).FullName + " 的类型");
				Logger.Error($"可用的类型数量: {types.Length}");
				foreach (Type item in types.Take(10))
				{
					Logger.Debug("  - " + item.FullName);
				}
				return null;
			}
			if (!(Activator.CreateInstance(type) is IRevitAdapter result))
			{
				Logger.Error("无法创建 " + type.FullName + " 实例");
				return null;
			}
			return result;
		}
		catch (Exception ex3)
		{
			Logger.Error("创建 Revit 适配器失败", ex3);
			return null;
		}
	}

	private static void LogInfo(string message)
	{
		Logger.Info("[Loader] " + message);
	}

	private static void LogError(string message)
	{
		Logger.Error("[Loader] " + message);
	}
}
