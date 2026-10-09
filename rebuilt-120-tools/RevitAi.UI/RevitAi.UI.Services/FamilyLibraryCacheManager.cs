using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Logging;

namespace RevitAi.UI.Services;

public class FamilyLibraryCacheManager
{
	private readonly string _cacheRoot;

	private readonly string _metadataCacheFile;

	private readonly string _thumbnailCacheDir;

	private readonly string _familyFileCacheDir;

	private FamilyLibraryCacheMetadata? _metadata;

	public FamilyLibraryCacheManager()
	{
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		_cacheRoot = Path.Combine(folderPath, "RevitAi", "FamilyLibrary");
		_metadataCacheFile = Path.Combine(_cacheRoot, "cache_metadata.json");
		_thumbnailCacheDir = Path.Combine(_cacheRoot, "Thumbnails");
		_familyFileCacheDir = Path.Combine(_cacheRoot, "FamilyFiles");
		Directory.CreateDirectory(_cacheRoot);
		Directory.CreateDirectory(_thumbnailCacheDir);
		Directory.CreateDirectory(_familyFileCacheDir);
		LoadMetadata();
	}

	private void LoadMetadata()
	{
		try
		{
			if (File.Exists(_metadataCacheFile))
			{
				string json = File.ReadAllText(_metadataCacheFile);
				_metadata = JsonSerializer.Deserialize<FamilyLibraryCacheMetadata>(json);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 加载缓存元数据失败: " + ex.Message);
		}
		if (_metadata == null)
		{
			_metadata = new FamilyLibraryCacheMetadata
			{
				CategoriesCacheTime = null,
				FamiliesCacheTime = null,
				CachedFamilies = new Dictionary<string, CachedFamilyInfo>()
			};
		}
	}

	private void SaveMetadata()
	{
		try
		{
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				WriteIndented = true
			};
			string contents = JsonSerializer.Serialize(_metadata, options);
			File.WriteAllText(_metadataCacheFile, contents);
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 保存缓存元数据失败: " + ex.Message);
		}
	}

	public bool IsCategoriesCacheValid()
	{
		return _metadata?.CategoriesCacheTime.HasValue ?? false;
	}

	public void SaveCategoriesCache(List<FamilyCategory> categories)
	{
		try
		{
			string path = Path.Combine(_cacheRoot, "categories.json");
			string contents = JsonSerializer.Serialize(categories, new JsonSerializerOptions
			{
				WriteIndented = true
			});
			File.WriteAllText(path, contents);
			_metadata.CategoriesCacheTime = DateTime.Now;
			SaveMetadata();
			Logger.Info($"[FamilyCache] 已缓存 {categories.Count} 个分类");
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 保存分类缓存失败: " + ex.Message);
		}
	}

	public List<FamilyCategory>? LoadCategoriesCache()
	{
		try
		{
			string path = Path.Combine(_cacheRoot, "categories.json");
			if (File.Exists(path))
			{
				List<FamilyCategory> list = JsonSerializer.Deserialize<List<FamilyCategory>>(File.ReadAllText(path));
				Logger.Info($"[FamilyCache] 从缓存加载了 {list?.Count ?? 0} 个分类");
				return list;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 加载分类缓存失败: " + ex.Message);
		}
		return null;
	}

	public void SaveFamiliesCache(List<FamilyLibraryItem> families, Guid? categoryId, string? searchText)
	{
		try
		{
			string familiesCacheKey = GetFamiliesCacheKey(categoryId, searchText);
			string path = Path.Combine(_cacheRoot, "families_" + familiesCacheKey + ".json");
			string contents = JsonSerializer.Serialize(families, new JsonSerializerOptions
			{
				WriteIndented = true
			});
			File.WriteAllText(path, contents);
			foreach (FamilyLibraryItem family in families)
			{
				string key = family.Id.ToString();
				if (!_metadata.CachedFamilies.ContainsKey(key))
				{
					_metadata.CachedFamilies[key] = new CachedFamilyInfo
					{
						FamilyId = family.Id,
						DisplayName = family.DisplayName,
						CacheTime = DateTime.Now
					};
				}
				else
				{
					_metadata.CachedFamilies[key].CacheTime = DateTime.Now;
				}
			}
			_metadata.FamiliesCacheTime = DateTime.Now;
			SaveMetadata();
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 保存族列表缓存失败: " + ex.Message);
		}
	}

	public List<FamilyLibraryItem>? LoadFamiliesCache(Guid? categoryId, string? searchText)
	{
		try
		{
			string familiesCacheKey = GetFamiliesCacheKey(categoryId, searchText);
			string path = Path.Combine(_cacheRoot, "families_" + familiesCacheKey + ".json");
			if (File.Exists(path))
			{
				List<FamilyLibraryItem> list = JsonSerializer.Deserialize<List<FamilyLibraryItem>>(File.ReadAllText(path));
				Logger.Info($"[FamilyCache] 从缓存加载了 {list?.Count ?? 0} 个族 (key: {familiesCacheKey})");
				return list;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 加载族列表缓存失败: " + ex.Message);
		}
		return null;
	}

	private string GetFamiliesCacheKey(Guid? categoryId, string? searchText)
	{
		List<string> list = new List<string>();
		if (categoryId.HasValue)
		{
			list.Add($"cat_{categoryId.Value}");
		}
		else
		{
			list.Add("cat_all");
		}
		if (!string.IsNullOrEmpty(searchText))
		{
			list.Add($"search_{searchText.GetHashCode()}");
		}
		else
		{
			list.Add("search_none");
		}
		return string.Join("_", list);
	}

	public void SaveFamilyDetailCache(FamilyLibraryDetail detail)
	{
		try
		{
			string path = Path.Combine(_cacheRoot, $"detail_{detail.Family.Id}.json");
			string contents = JsonSerializer.Serialize(detail, new JsonSerializerOptions
			{
				WriteIndented = true
			});
			File.WriteAllText(path, contents);
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 保存族详情缓存失败: " + ex.Message);
		}
	}

	public FamilyLibraryDetail? LoadFamilyDetailCache(Guid familyId)
	{
		try
		{
			string path = Path.Combine(_cacheRoot, $"detail_{familyId}.json");
			if (File.Exists(path))
			{
				FamilyLibraryDetail familyLibraryDetail = JsonSerializer.Deserialize<FamilyLibraryDetail>(File.ReadAllText(path));
				if (familyLibraryDetail != null && (familyLibraryDetail.TypeGroups == null || familyLibraryDetail.TypeGroups.Count == 0))
				{
					Logger.Info($"[FamilyCache] 检测到旧版本缓存（缺少 TypeGroups），删除: {familyId}");
					File.Delete(path);
					return null;
				}
				return familyLibraryDetail;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 加载族详情缓存失败: " + ex.Message);
		}
		return null;
	}

	public string GetThumbnailCachePath(Guid familyId)
	{
		return Path.Combine(_thumbnailCacheDir, $"{familyId}.jpg");
	}

	public bool IsThumbnailCached(Guid familyId)
	{
		return File.Exists(GetThumbnailCachePath(familyId));
	}

	public void SaveThumbnailToCache(Guid familyId, byte[] imageData)
	{
		try
		{
			File.WriteAllBytes(GetThumbnailCachePath(familyId), imageData);
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 保存缩略图失败: " + ex.Message);
		}
	}

	public async Task LoadThumbnailAsync(Guid familyId, string thumbnailUrl)
	{
		if (IsThumbnailCached(familyId))
		{
			return;
		}
		try
		{
			using HttpClient client = new HttpClient();
			client.Timeout = TimeSpan.FromSeconds(10L);
			HttpResponseMessage httpResponseMessage = await client.GetAsync(thumbnailUrl);
			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				Logger.Debug($"[FamilyCache] 下载缩略图失败: {familyId}, 状态码: {httpResponseMessage.StatusCode}");
				return;
			}
			SaveThumbnailToCache(familyId, await httpResponseMessage.Content.ReadAsByteArrayAsync());
		}
		catch (Exception ex)
		{
			Logger.Warning($"[FamilyCache] 预加载缩略图失败: {familyId}, {ex.Message}");
		}
	}

	public string GetFamilyFileCachePath(Guid familyId)
	{
		return Path.Combine(_familyFileCacheDir, $"{familyId}.rfa");
	}

	public bool IsFamilyFileCached(Guid familyId)
	{
		return File.Exists(GetFamilyFileCachePath(familyId));
	}

	public void CleanExpiredCache()
	{
	}

	public FamilyCacheStats GetCacheStats()
	{
		FamilyCacheStats familyCacheStats = new FamilyCacheStats();
		try
		{
			if (Directory.Exists(_thumbnailCacheDir))
			{
				familyCacheStats.ThumbnailCount = Directory.GetFiles(_thumbnailCacheDir).Length;
				familyCacheStats.ThumbnailSize = (from f in Directory.GetFiles(_thumbnailCacheDir)
					select new FileInfo(f).Length).Sum();
			}
			if (Directory.Exists(_familyFileCacheDir))
			{
				familyCacheStats.FamilyFileCount = Directory.GetFiles(_familyFileCacheDir).Length;
				familyCacheStats.FamilyFileSize = (from f in Directory.GetFiles(_familyFileCacheDir)
					select new FileInfo(f).Length).Sum();
			}
			if (Directory.Exists(_cacheRoot))
			{
				familyCacheStats.MetadataCount = Directory.GetFiles(_cacheRoot, "*.json").Length;
			}
			familyCacheStats.LastCleanup = DateTime.Now;
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 获取缓存统计失败: " + ex.Message);
		}
		return familyCacheStats;
	}

	public void ClearAllCache()
	{
		try
		{
			if (Directory.Exists(_thumbnailCacheDir))
			{
				Directory.Delete(_thumbnailCacheDir, recursive: true);
				Directory.CreateDirectory(_thumbnailCacheDir);
			}
			if (Directory.Exists(_cacheRoot))
			{
				string[] files = Directory.GetFiles(_cacheRoot, "*.json");
				foreach (string path in files)
				{
					try
					{
						File.Delete(path);
					}
					catch
					{
					}
				}
			}
			_metadata = new FamilyLibraryCacheMetadata
			{
				CategoriesCacheTime = null,
				FamiliesCacheTime = null,
				CachedFamilies = new Dictionary<string, CachedFamilyInfo>()
			};
			SaveMetadata();
			Logger.Info("[FamilyCache] 已清空所有缓存");
		}
		catch (Exception ex)
		{
			Logger.Warning("[FamilyCache] 清空缓存失败: " + ex.Message);
		}
	}
}
