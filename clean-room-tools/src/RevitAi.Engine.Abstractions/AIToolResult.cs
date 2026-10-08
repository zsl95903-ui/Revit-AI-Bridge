namespace RevitAi.Engine.Abstractions;

/// <summary>
/// 工具执行结果。成功判定决定宿主是否提交事务（对齐厂商：AIToolResult.Success == true 才 Commit）。
/// </summary>
public sealed class AIToolResult
{
    private AIToolResult(bool success, string message, object? data, string? cacheId)
    {
        Success = success;
        Message = message ?? string.Empty;
        Data = data;
        CacheId = cacheId;
    }

    public bool Success { get; }

    public string Message { get; }

    public object? Data { get; }

    public string? CacheId { get; }

    public static AIToolResult Ok(string message, object? data = null) => new(true, message, data, null);

    public static AIToolResult Fail(string message, object? data = null) => new(false, message, data, null);

    public static AIToolResult OkWithCache(string message, string cacheId, object? data = null)
        => new(true, message, data, cacheId);
}
