namespace RevitAi.Abstractions.Commands;

public interface ICommandExecutor
{
	CommandExecutionResult ExecuteCommand(string commandName, object commandData);
}
