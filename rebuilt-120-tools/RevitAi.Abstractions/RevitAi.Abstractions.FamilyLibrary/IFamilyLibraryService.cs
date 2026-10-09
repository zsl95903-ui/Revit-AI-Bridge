using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;

namespace RevitAi.Abstractions.FamilyLibrary;

public interface IFamilyLibraryService
{
	Task<Result<List<FamilyLibraryItem>>> GetFamiliesAsync(Guid? categoryId = null, string? searchText = null, string[]? tags = null, string? revitVersion = null, string? revitCategoryName = null, bool? isFree = null, int page = 1, int pageSize = 50, bool includeNonPublic = false);

	Task<Result<List<FamilyCategory>>> GetCategoriesAsync(bool includeInactive = false);

	Task<Result<List<RevitCategoryItem>>> GetRevitCategoriesAsync();

	Task<Result<DownloadQuotaInfo>> GetDownloadQuotaAsync();

	Task<Result<FamilyDownloadInfo>> RequestDownloadAsync(Guid familyId);

	Task RecordLocalCacheUsageAsync(Guid familyId);

	Task<Result<List<FamilyDownloadHistoryItem>>> GetDownloadHistoryAsync(int page = 1, int pageSize = 20);

	Task<Result<List<FamilyLibraryItem>>> GetPopularFamiliesAsync(int limit = 20);

	Task<Result<FamilyLibraryDetail>> GetFamilyDetailsAsync(Guid familyId);

	Result<DownloadQuotaInfo> CalculateDownloadQuotaLocally(AuthorizationState authState);

	Task<Result<DownloadQuotaInfo>> CalculateDownloadQuotaLocallyAsync(AuthorizationState authState);

	Task RecordDownloadLocallyAsync(Guid familyId);

	void RecordDownloadLocally(Guid familyId);

	bool IsFamilyDownloadedToday(Guid familyId);
}
