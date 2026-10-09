using System;
using RevitAi.Abstractions.Commands;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Commands;

public abstract class RevitCommandWrapperBase : IExternalCommand
{
	protected abstract string CommandName { get; }

	public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ICommandExecutor executor = CommandExecutorRegistry.GetExecutor();
			if (executor == null)
			{
				Logger.Error("命令执行器未注册，请确保 MainApplication 已正确初始化");
				message = "命令执行器未注册";
				return (Result)(-1);
			}
			CommandExecutionResult commandExecutionResult_ = executor.ExecuteCommand(CommandName, (object)commandData);
			return method_0(commandExecutionResult_);
		}
		catch (Exception ex)
		{
			Logger.Error("命令执行异常: " + CommandName, ex);
			message = ex.Message;
			return (Result)(-1);
		}
	}

	private Result method_0(CommandExecutionResult commandExecutionResult_0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected I4, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		return (Result)(((int)commandExecutionResult_0 - 2) switch
		{
			0 => 1, 
			1 => -1, 
			2 => 0, 
			_ => -1, 
		});
	}
}
