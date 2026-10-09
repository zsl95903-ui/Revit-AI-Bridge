using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public sealed class CodeSnippetMarketItem
{
	public string Id { get; set; } = string.Empty;

	public string AuthorId { get; set; } = string.Empty;

	public string AuthorName { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string? CodeContent { get; set; }

	public List<string> Tags { get; set; } = new List<string>();

	public string Category { get; set; } = string.Empty;

	public decimal Price { get; set; }

	public bool IsFree { get; set; }

	public int DownloadCount { get; set; }

	public int ViewCount { get; set; }

	public int FavoriteCount { get; set; }

	public decimal AverageRating { get; set; }

	public int RatingCount { get; set; }

	public decimal HotScore { get; set; }

	public bool IsNew { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime? UpdatedAt { get; set; }

	public string? Status { get; set; }

	public int? Earnings { get; set; }

	public DateTime? FavoritedAt { get; set; }
}
