using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public sealed class CodeMarketSearchResult
{
	public List<CodeSnippetMarketItem> Snippets { get; set; } = new List<CodeSnippetMarketItem>();

	public int Count { get; set; }

	public int Total { get; set; }

	public bool HasMore { get; set; }
}
