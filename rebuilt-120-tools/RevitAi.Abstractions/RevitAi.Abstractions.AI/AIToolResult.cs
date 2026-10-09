using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.AI;

public sealed class AIToolResult
{
	public bool Success { get; set; }

	public string? Message { get; set; }

	public object? Data { get; set; }

	public string? Error { get; set; }

	public static AIToolResult Ok(string? message = null, object? data = null)
	{
		return new AIToolResult
		{
			Success = true,
			Message = message,
			Data = data
		};
	}

	public static AIToolResult Fail(string error)
	{
		return new AIToolResult
		{
			Success = false,
			Error = error
		};
	}

	public static AIToolResult OkWithCache(string message, string cacheId, object? summaryData = null, int totalCount = 0)
	{
		return new AIToolResult
		{
			Success = true,
			Message = message,
			Data = new
			{
				cache_id = cacheId,
				cache_reference = "使用缓存引用: " + cacheId,
				summary_data = summaryData,
				total_count = totalCount
			}
		};
	}

	public static AIToolResult OkWithSmartSummary<T>(IEnumerable<T> items, IAIToolDataCache cache, string sessionId, string cacheKey, string itemName = "项", int summaryLimit = 200)
	{
		List<T> list = items.ToList();
		int count = list.Count;
		string text = cache.Store(sessionId, cacheKey, list);
		CacheStatistics statistics = cache.GetStatistics(text);
		string message = ((count == 0) ? ("找到 0 个" + itemName) : ((count > summaryLimit) ? $"成功获取 {count} 个{itemName}（已返回前{summaryLimit}个摘要）" : $"成功获取 {count} 个{itemName}"));
		object obj = null;
		if (count > 0)
		{
			List<T> list2 = list.Take(summaryLimit).ToList();
			obj = ((count > 10) ? ((object)new
			{
				items = list2,
				count = list2.Count,
				total_count = count,
				has_more = (count > summaryLimit),
				cache_id = text,
				cache_info = new
				{
					cache_id = text,
					description = itemName + "查询结果",
					item_type = itemName,
					total_items = count,
					cached_at = (statistics?.CreatedAt.ToString("o") ?? DateTime.UtcNow.ToString("o")),
					is_summary = (count > summaryLimit),
					returned_items = list2.Count,
					remaining_items = Math.Max(0, count - list2.Count),
					estimated_size_kb = ((statistics != null) ? (statistics.EstimatedSizeBytes / 1024) : 0)
				}
			}) : ((object)new
			{
				items = list2,
				count = list2.Count,
				total_count = count,
				has_more = (count > summaryLimit)
			}));
		}
		else
		{
			obj = new
			{
				items = Array.Empty<T>(),
				count = 0,
				total_count = 0,
				has_more = false
			};
		}
		return new AIToolResult
		{
			Success = true,
			Message = message,
			Data = obj
		};
	}
}
