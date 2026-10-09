using System;
using RevitAi.Abstractions.Logging;

namespace RevitAi.Abstractions.Loader;

public sealed class RegisteredCommand
{
	public string Name { get; set; } = string.Empty;

	public Type CommandType { get; set; }

	public CommandAttribute Attribute { get; set; }

	public IDynamicCommand CreateInstance()
	{
		try
		{
			if (Activator.CreateInstance(CommandType) is IDynamicCommand result)
			{
				return result;
			}
			throw new InvalidOperationException("类型 " + CommandType.FullName + " 未实现 IDynamicCommand 接口");
		}
		catch (Exception ex)
		{
			Logger.Error("创建命令实例失败: " + Name, ex);
			throw;
		}
	}
}
