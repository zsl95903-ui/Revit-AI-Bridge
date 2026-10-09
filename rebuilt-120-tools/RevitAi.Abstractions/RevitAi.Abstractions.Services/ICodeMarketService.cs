using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RevitAi.Abstractions.Services;

public interface ICodeMarketService
{
	Task<CodeMarketResult<CodeSnippetUploadResult>> UploadSnippetAsync(string name, string description, string codeContent, List<string> tags, string category, decimal price, CancellationToken cancellationToken = default(CancellationToken));

	Task<CodeMarketResult<CodeMarketSearchResult>> BrowseMarketAsync(CodeMarketSearchOptions options, CancellationToken cancellationToken = default(CancellationToken));

	Task<CodeMarketResult<CodeSnippetDownloadResult>> DownloadSnippetAsync(string snippetId, CancellationToken cancellationToken = default(CancellationToken));

	Task<CodeMarketResult<bool>> ToggleFavoriteAsync(string snippetId, CancellationToken cancellationToken = default(CancellationToken));

	Task<CodeMarketResult<List<CodeSnippetMarketItem>>> GetMyFavoritesAsync(int limit = 20, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));

	Task<CodeMarketResult<HashSet<string>>> CheckFavoritedStatusAsync(List<string> snippetIds, CancellationToken cancellationToken = default(CancellationToken));

	Task<CodeMarketResult<bool>> RateSnippetAsync(string snippetId, int rating, string? comment = null, CancellationToken cancellationToken = default(CancellationToken));

	Task<CodeMarketResult<List<CodeSnippetMarketItem>>> GetMyPublishedSnippetsAsync(bool includeDeleted = false, CancellationToken cancellationToken = default(CancellationToken));

	Task<CodeMarketResult<bool>> DeleteSnippetAsync(string snippetId, CancellationToken cancellationToken = default(CancellationToken));

	Task<CodeMarketResult<bool>> UpdateSnippetAsync(string snippetId, string? name = null, string? description = null, List<string>? tags = null, string? category = null, int? price = null, CancellationToken cancellationToken = default(CancellationToken));
}
