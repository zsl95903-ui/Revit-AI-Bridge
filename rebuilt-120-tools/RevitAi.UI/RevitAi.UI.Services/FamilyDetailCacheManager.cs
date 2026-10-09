using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Logging;

namespace RevitAi.UI.Services;

public class FamilyDetailCacheManager
{
	private readonly IFamilyLibraryService _familyLibraryService;

	private readonly FamilyLibraryCacheManager _cacheManager;

	private readonly ConcurrentDictionary<Guid, FamilyLibraryDetail> _detailCache = new ConcurrentDictionary<Guid, FamilyLibraryDetail>();

	private readonly ConcurrentDictionary<Guid, Task<FamilyLibraryDetail?>> _loadingTasks = new ConcurrentDictionary<Guid, Task<FamilyLibraryDetail>>();

	public int CacheSize => _detailCache.Count;

	public FamilyDetailCacheManager(IFamilyLibraryService familyLibraryService)
	{
		_familyLibraryService = familyLibraryService ?? throw new ArgumentNullException("familyLibraryService");
		_cacheManager = new FamilyLibraryCacheManager();
	}

	public async Task<FamilyLibraryDetail?> GetDetailsAsync(Guid familyId)
	{
		if (_detailCache.TryGetValue(familyId, out FamilyLibraryDetail value))
		{
			return value;
		}
		FamilyLibraryDetail familyLibraryDetail = _cacheManager.LoadFamilyDetailCache(familyId);
		if (familyLibraryDetail != null)
		{
			_detailCache[familyId] = familyLibraryDetail;
			return familyLibraryDetail;
		}
		if (_loadingTasks.TryGetValue(familyId, out Task<FamilyLibraryDetail> value2))
		{
			return await value2;
		}
		Task<FamilyLibraryDetail> task = LoadDetailsAsync(familyId);
		_loadingTasks[familyId] = task;
		try
		{
			FamilyLibraryDetail familyLibraryDetail2 = await task;
			if (familyLibraryDetail2 != null)
			{
				_detailCache[familyId] = familyLibraryDetail2;
				_cacheManager.SaveFamilyDetailCache(familyLibraryDetail2);
			}
			return familyLibraryDetail2;
		}
		finally
		{
			_loadingTasks.TryRemove(familyId, out Task<FamilyLibraryDetail> _);
		}
	}

	public void PrefetchDetails(Guid familyId)
	{
		if (_detailCache.ContainsKey(familyId) || _loadingTasks.ContainsKey(familyId) || _cacheManager.LoadFamilyDetailCache(familyId) != null)
		{
			return;
		}
		Task.Run(async delegate
		{
			try
			{
				await GetDetailsAsync(familyId);
				Logger.Debug($"[FamilyDetailCache] 预加载成功: {familyId}");
			}
			catch (Exception ex)
			{
				Logger.Debug($"[FamilyDetailCache] 预加载失败: {familyId}, {ex.Message}");
			}
		});
	}

	public void PrefetchDetailsBatch(Guid[] familyIds)
	{
		foreach (Guid familyId in familyIds)
		{
			PrefetchDetails(familyId);
		}
	}

	public bool IsCached(Guid familyId)
	{
		if (!_detailCache.ContainsKey(familyId))
		{
			return _cacheManager.LoadFamilyDetailCache(familyId) != null;
		}
		return true;
	}

	public bool IsLoading(Guid familyId)
	{
		return _loadingTasks.ContainsKey(familyId);
	}

	public void ClearCache()
	{
		_detailCache.Clear();
	}

	public void ClearCache(Guid familyId)
	{
		_detailCache.TryRemove(familyId, out FamilyLibraryDetail _);
	}

	private async Task<FamilyLibraryDetail?> LoadDetailsAsync(Guid familyId)
	{
		try
		{
			Result<FamilyLibraryDetail> result = await _familyLibraryService.GetFamilyDetailsAsync(familyId);
			if (result.IsSuccess && result.Value != null)
			{
				return result.Value;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Warning($"[FamilyDetailCache] 加载详情失败: {familyId}", ex);
			return null;
		}
	}
}
