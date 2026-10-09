namespace RevitAi.Abstractions.Loader;

public interface IDynamicCommand
{
	CommandResult Execute(CommandContext context);
}
