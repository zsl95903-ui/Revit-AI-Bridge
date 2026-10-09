using System.Collections.Generic;

namespace RevitAi.Abstractions.AI;

public interface IAIToolDataCache
{
	string Store<T>(string sessionId, string key, T data);

	T? Retrieve<T>(string cacheId);

	T? RetrieveByKey<T>(string sessionId, string key);

	bool Exists(string cacheId);

	bool Invalidate(string cacheId);

	void ClearSession(string sessionId);

	void ClearAll();

	CacheStatistics? GetStatistics(string cacheId);

	IEnumerable<string> GetSessionCacheIds(string sessionId);

	CacheValidationResult ValidateCache(string cacheId);
}
