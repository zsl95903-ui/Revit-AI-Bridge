using System.Collections.Concurrent;
using System.Text.Json;
using RevitAi.Engine.Abstractions;

namespace RevitAi.Engine.Core;

/// <summary>
/// 会话级工具数据缓存。规则对齐厂商 SessionAIToolDataCache：
///   * cache_id 由全局单调计数器生成：cache_1、cache_2…
///   * 单条 &gt; 50MB 直接抛错；会话内条目 ≥ 100 时先淘汰最久未访问的 25 条；
///   * 会话总字节 &gt; 200MB 时按最后访问时间继续淘汰。
/// 参考：反编译重建源码 Core\AS\Tools\Core\AI\SessionAIToolDataCache.cs。
/// </summary>
public sealed class SessionAIToolDataCache : IAIToolDataCache
{
    private const long MaxItemBytes = 50L * 1024 * 1024;
    private const int MaxItemsPerSession = 100;
    private const int EvictBatchSize = 25;
    private const long MaxSessionBytes = 200L * 1024 * 1024;

    private sealed class Entry
    {
        public required string CacheId { get; init; }

        public required string SessionId { get; init; }

        public required string Key { get; init; }

        public object? Data { get; init; }

        public required string DataType { get; init; }

        public required long EstimatedSizeBytes { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset LastAccessedAt { get; set; }

        public int AccessCount { get; set; }
    }

    private readonly ConcurrentDictionary<string, Entry> _entries = new(StringComparer.Ordinal);
    private int _counter;

    public int Count => _entries.Count;

    public string Store<T>(string sessionId, string key, T data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var size = EstimateSize(data);
        if (size > MaxItemBytes)
        {
            throw new InvalidOperationException(
                $"缓存条目过大（{size / 1024 / 1024} MB），超过单条上限 {MaxItemBytes / 1024 / 1024} MB。");
        }

        EvictIfNeeded(sessionId);

        var cacheId = "cache_" + Interlocked.Increment(ref _counter);
        var entry = new Entry
        {
            CacheId = cacheId,
            SessionId = sessionId,
            Key = key,
            Data = data,
            DataType = typeof(T).FullName ?? typeof(T).Name,
            EstimatedSizeBytes = size,
            CreatedAt = DateTimeOffset.Now,
            LastAccessedAt = DateTimeOffset.Now,
        };

        _entries[cacheId] = entry;
        return cacheId;
    }

    public T? Retrieve<T>(string cacheId)
    {
        if (string.IsNullOrWhiteSpace(cacheId) || !_entries.TryGetValue(cacheId, out var entry))
        {
            return default;
        }

        entry.LastAccessedAt = DateTimeOffset.Now;
        entry.AccessCount++;

        if (entry.Data is T typed)
        {
            return typed;
        }

        if (entry.Data is JsonElement element)
        {
            try
            {
                return element.Deserialize<T>();
            }
            catch
            {
                return default;
            }
        }

        if (entry.Data is null)
        {
            return default;
        }

        try
        {
            return (T)entry.Data;
        }
        catch
        {
            return default;
        }
    }

    public bool Exists(string cacheId) => !string.IsNullOrWhiteSpace(cacheId) && _entries.ContainsKey(cacheId);

    public CacheValidationResult ValidateCache(string cacheId)
    {
        if (string.IsNullOrWhiteSpace(cacheId))
        {
            return CacheValidationResult.Invalid(string.Empty, "缓存 ID 不能为空");
        }

        return _entries.TryGetValue(cacheId, out var entry)
            ? CacheValidationResult.Valid(ToStatistics(entry))
            : CacheValidationResult.Invalid(cacheId, "缓存不存在或已过期");
    }

    public CacheValidationResult ValidateCacheWithSession(string cacheId, string expectedSessionId)
    {
        var result = ValidateCache(cacheId);
        if (!result.IsValid || result.Statistics is null)
        {
            return result;
        }

        return string.Equals(result.Statistics.SessionId, expectedSessionId, StringComparison.Ordinal)
            ? result
            : CacheValidationResult.SessionMismatch(cacheId, expectedSessionId, result.Statistics.SessionId);
    }

    public void ClearSession(string sessionId)
    {
        foreach (var pair in _entries.Where(p => string.Equals(p.Value.SessionId, sessionId, StringComparison.Ordinal)).ToArray())
        {
            _entries.TryRemove(pair.Key, out _);
        }
    }

    public void ClearAll() => _entries.Clear();

    private void EvictIfNeeded(string sessionId)
    {
        var sessionEntries = _entries.Values
            .Where(entry => string.Equals(entry.SessionId, sessionId, StringComparison.Ordinal))
            .OrderBy(entry => entry.LastAccessedAt)
            .ToArray();

        if (sessionEntries.Length >= MaxItemsPerSession)
        {
            foreach (var entry in sessionEntries.Take(EvictBatchSize))
            {
                _entries.TryRemove(entry.CacheId, out _);
            }

            sessionEntries = _entries.Values
                .Where(entry => string.Equals(entry.SessionId, sessionId, StringComparison.Ordinal))
                .OrderBy(entry => entry.LastAccessedAt)
                .ToArray();
        }

        var total = sessionEntries.Sum(entry => entry.EstimatedSizeBytes);
        foreach (var entry in sessionEntries)
        {
            if (total <= MaxSessionBytes)
            {
                break;
            }

            if (_entries.TryRemove(entry.CacheId, out _))
            {
                total -= entry.EstimatedSizeBytes;
            }
        }
    }

    private static long EstimateSize<T>(T data)
    {
        if (data is null)
        {
            return 0;
        }

        try
        {
            return JsonSerializer.SerializeToUtf8Bytes(data).LongLength;
        }
        catch
        {
            return 0;
        }
    }

    private static CacheStatistics ToStatistics(Entry entry) => new(
        entry.CacheId,
        entry.SessionId,
        entry.Key,
        entry.DataType,
        entry.EstimatedSizeBytes,
        entry.CreatedAt,
        entry.AccessCount);
}
