namespace RevitAi.Abstractions.UI;

public interface IExternalCommandContext
{
	object? ActiveDocument { get; }

	object? UIApplication { get; }

	bool IsRevitInternalTrigger { get; }
}
