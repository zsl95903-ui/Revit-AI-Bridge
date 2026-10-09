using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RevitAi.Abstractions.Logging;

namespace RevitAi.Abstractions.Loader;

public static class CommandDiscovery
{
	public static int DiscoverAndRegisterFromAssembly(Assembly assembly)
	{
		if (assembly == null)
		{
			throw new ArgumentNullException("assembly");
		}
		int num = 0;
		try
		{
			Type[] array = (from t in assembly.GetTypes()
				where t.IsClass && !t.IsAbstract && t.IsPublic
				where typeof(IDynamicCommand).IsAssignableFrom(t)
				select t).ToArray();
			foreach (Type type in array)
			{
				try
				{
					CommandAttribute customAttribute = type.GetCustomAttribute<CommandAttribute>();
					if (customAttribute != null)
					{
						CommandRegistry.Register(customAttribute.Name, type, customAttribute);
						num++;
					}
				}
				catch (Exception ex)
				{
					Logger.Error("注册命令失败: " + type.FullName, ex);
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("扫描程序集失败: " + assembly.GetName().Name, ex2);
		}
		return num;
	}

	public static int DiscoverAndRegisterFromPluginDirectory()
	{
		int num = 0;
		try
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (string.IsNullOrEmpty(directoryName))
			{
				Logger.Error("无法获取插件目录");
				return 0;
			}
			Assembly[] array = Directory.GetFiles(directoryName, "RevitAi.Commands*.dll").SelectMany(delegate(string dll)
			{
				Assembly assembly2 = LoadAssemblySafely(dll);
				return (assembly2 != null) ? ((IEnumerable<Assembly>)new Assembly[1] { assembly2 }) : ((IEnumerable<Assembly>)Array.Empty<Assembly>());
			}).ToArray();
			foreach (Assembly assembly in array)
			{
				num += DiscoverAndRegisterFromAssembly(assembly);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("扫描插件目录失败", ex);
		}
		return num;
	}

	private static Assembly? LoadAssemblySafely(string assemblyPath)
	{
		try
		{
			if (!File.Exists(assemblyPath))
			{
				Logger.Debug("程序集不存在: " + assemblyPath);
				return null;
			}
			AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name == assemblyName.Name);
			if (assembly != null)
			{
				Logger.Debug("程序集已加载: " + assemblyName.Name);
				return assembly;
			}
			return Assembly.LoadFrom(assemblyPath);
		}
		catch (Exception ex)
		{
			Logger.Warning("加载程序集失败: " + assemblyPath + " - " + ex.Message);
			return null;
		}
	}
}
