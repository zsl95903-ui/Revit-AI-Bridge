using System;

namespace RevitAi.UI.ViewModels;

public class ExecuteSnippetEventArgs : EventArgs
{
	public string Code { get; }

	public ExecuteSnippetEventArgs(string code)
	{
		Code = code;
	}
}
