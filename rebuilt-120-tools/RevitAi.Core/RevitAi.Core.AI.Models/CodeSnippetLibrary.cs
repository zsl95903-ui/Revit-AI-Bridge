using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RevitAi.Core.AI.Models;

public sealed class CodeSnippetLibrary
{
	[CompilerGenerated]
	private List<CodeSnippet> list_0 = new List<CodeSnippet>();

	public List<CodeSnippet> Snippets
	{
		[CompilerGenerated]
		get
		{
			return list_0;
		}
		[CompilerGenerated]
		set
		{
			list_0 = value;
		}
	}
}
