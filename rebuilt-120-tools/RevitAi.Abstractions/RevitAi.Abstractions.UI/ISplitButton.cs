namespace RevitAi.Abstractions.UI;

public interface ISplitButton
{
	IPushButton AddPushButton(string commandName, string displayName);
}
