namespace RevitAi.Engine.Abstractions;

/// <summary>缓存条目统计信息（对应厂商 CacheStatistics）。</summary>
public sealed record CacheStatistics(
    string CacheId,
    string SessionId,
    string Key,
    string DataType,
    long EstimatedSizeBytes,
    DateTimeOffset CreatedAt,
    int AccessCount);

/// <summary>缓存校验结果（对应厂商 CacheValidationResult）。</summary>
public sealed record CacheValidationResult(bool IsValid, string CacheId, string? Reason, CacheStatistics? Statistics)
{
    public static CacheValidationResult Valid(CacheStatistics statistics) => new(true, statistics.CacheId, null, statistics);

    public static CacheValidationResult Invalid(string cacheId, string reason) => new(false, cacheId, reason, null);

    public static CacheValidationResult SessionMismatch(string cacheId, string expected, string actual)
        => new(false, cacheId, $"缓存属于会话 {actual}，当前会话为 {expected}", null);
}

/// <summary>
/// 跨轮次数据缓存契约。作用：查询类工具把结果存进缓存并返回 cache_id，
/// 后续写入/导出类工具用 cache_id 取回元素集合，避免模型搬运大数组。
/// </summary>
public interface IAIToolDataCache
{
    /// <summary>存入数据并返回 cache_id（形如 cache_1、cache_2…）。</summary>
    string Store<T>(string sessionId, string key, T data);

    T? Retrieve<T>(string cacheId);

    bool Exists(string cacheId);

    CacheValidationResult ValidateCache(string cacheId);

    void ClearSession(string sessionId);

    void ClearAll();

    int Count { get; }
}
