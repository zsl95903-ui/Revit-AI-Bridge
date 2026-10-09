using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public sealed class CodeMarketSearchOptions
{
	public string? SearchKeyword { get; set; }

	public List<string>? Tags { get; set; }

	public List<string>? Categories { get; set; }

	public int? MinPrice { get; set; }

	public int? MaxPrice { get; set; }

	public decimal? MinRating { get; set; }

	public CodeMarketSortOrder SortOrder { get; set; }

	public CodeMarketSortDirection SortDirection { get; set; } = CodeMarketSortDirection.Desc;

	public int Limit { get; set; } = 20;

	public int Offset { get; set; }
}
