namespace RevitAi.Abstractions.Services;

public sealed class CodeSnippetDownloadResult
{
	public bool Owned { get; set; }

	public bool IsFree { get; set; }

	public string? TransactionId { get; set; }

	public decimal? PricePaid { get; set; }

	public CodeSnippetMarketItem? Snippet { get; set; }
}
