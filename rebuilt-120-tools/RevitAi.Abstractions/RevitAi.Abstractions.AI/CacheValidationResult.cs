namespace RevitAi.Abstractions.AI;

public class CacheValidationResult
{
	public string CacheId { get; set; } = string.Empty;

	public bool IsValid { get; set; }

	public bool Exists { get; set; }

	public string? SessionId { get; set; }

	public string? ErrorMessage { get; set; }

	public CacheStatistics? Statistics { get; set; }

	public static CacheValidationResult Valid(string cacheId, CacheStatistics? statistics = null)
	{
		return new CacheValidationResult
		{
			CacheId = cacheId,
			IsValid = true,
			Exists = true,
			Statistics = statistics
		};
	}

	public static CacheValidationResult Invalid(string cacheId, string errorMessage)
	{
		return new CacheValidationResult
		{
			CacheId = cacheId,
			IsValid = false,
			Exists = false,
			ErrorMessage = errorMessage
		};
	}

	public static CacheValidationResult SessionMismatch(string cacheId, string expectedSessionId, string actualSessionId)
	{
		return new CacheValidationResult
		{
			CacheId = cacheId,
			IsValid = false,
			Exists = true,
			SessionId = actualSessionId,
			ErrorMessage = "缓存属于会话 " + actualSessionId + "，但当前会话是 " + expectedSessionId
		};
	}
}
