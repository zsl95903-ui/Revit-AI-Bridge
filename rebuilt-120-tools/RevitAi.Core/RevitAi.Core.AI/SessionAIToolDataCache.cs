using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using RevitAi.Abstractions.AI;
using ns7;

namespace RevitAi.Core.AI;

public sealed class SessionAIToolDataCache : IDisposable, IAIToolDataCache
{
	public sealed class CacheEntry
	{
		[CompilerGenerated]
		private string string_0 = string.Empty;

		[CompilerGenerated]
		private string string_1 = string.Empty;

		[CompilerGenerated]
		private string string_2 = string.Empty;

		[CompilerGenerated]
		private object? object_0;

		[CompilerGenerated]
		private DateTime dateTime_0;

		[CompilerGenerated]
		private DateTime dateTime_1;

		[CompilerGenerated]
		private int int_0;

		[CompilerGenerated]
		private long long_0;

		[CompilerGenerated]
		private string string_3 = string.Empty;

		[CompilerGenerated]
		private int int_1;

		public string CacheId
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		public string SessionId
		{
			[CompilerGenerated]
			get
			{
				return string_1;
			}
			[CompilerGenerated]
			set
			{
				string_1 = value;
			}
		}

		public string Key
		{
			[CompilerGenerated]
			get
			{
				return string_2;
			}
			[CompilerGenerated]
			set
			{
				string_2 = value;
			}
		}

		public object? Data
		{
			[CompilerGenerated]
			get
			{
				return object_0;
			}
			[CompilerGenerated]
			set
			{
				object_0 = value;
			}
		}

		public DateTime CreatedAt
		{
			[CompilerGenerated]
			get
			{
				return dateTime_0;
			}
			[CompilerGenerated]
			set
			{
				dateTime_0 = value;
			}
		}

		public DateTime LastAccessedAt
		{
			[CompilerGenerated]
			get
			{
				return dateTime_1;
			}
			[CompilerGenerated]
			set
			{
				dateTime_1 = value;
			}
		}

		public int AccessCount
		{
			[CompilerGenerated]
			get
			{
				return int_0;
			}
			[CompilerGenerated]
			set
			{
				int_0 = value;
			}
		}

		public long EstimatedSizeBytes
		{
			[CompilerGenerated]
			get
			{
				return long_0;
			}
			[CompilerGenerated]
			set
			{
				long_0 = value;
			}
		}

		public string DataType
		{
			[CompilerGenerated]
			get
			{
				return string_3;
			}
			[CompilerGenerated]
			set
			{
				string_3 = value;
			}
		}

		public int SequenceNumber
		{
			[CompilerGenerated]
			get
			{
				return int_1;
			}
			[CompilerGenerated]
			set
			{
				int_1 = value;
			}
		}
	}

	[CompilerGenerated]
	public sealed class Class128
	{
		public string string_0;

		internal bool method_0(KeyValuePair<string, CacheEntry> keyValuePair_0)
		{
			return keyValuePair_0.Value.SessionId == string_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class129
	{
		public string string_0;

		internal bool method_0(KeyValuePair<string, CacheEntry> keyValuePair_0)
		{
			return keyValuePair_0.Value.SessionId == string_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class130
	{
		public string string_0;

		internal bool method_0(KeyValuePair<string, CacheEntry> keyValuePair_0)
		{
			return keyValuePair_0.Value.SessionId == string_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class131
	{
		public string string_0;

		internal bool method_0(KeyValuePair<string, CacheEntry> keyValuePair_0)
		{
			return keyValuePair_0.Value.SessionId == string_0;
		}
	}

	private const long long_0 = 52428800L;

	private const long long_1 = 209715200L;

	private const int int_0 = 100;

	private readonly ConcurrentDictionary<string, CacheEntry> concurrentDictionary_0 = new ConcurrentDictionary<string, CacheEntry>();

	private readonly ReaderWriterLockSlim readerWriterLockSlim_0 = new ReaderWriterLockSlim();

	private readonly ConcurrentDictionary<string, int> concurrentDictionary_1 = new ConcurrentDictionary<string, int>();

	private bool bool_0;

	public string Store<T>(string sessionId, string key, T data)
	{
		method_1();
		if (string.IsNullOrEmpty(sessionId))
		{
			throw new ArgumentException("会话 ID 不能为空", "sessionId");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException("缓存键不能为空", "key");
		}
		long num = method_0(data);
		if (num > 52428800L)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
			defaultInterpolatedStringHandler.AppendLiteral("数据大小超过限制 (");
			defaultInterpolatedStringHandler.AppendFormatted((double)num / 1024.0 / 1024.0, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(" MB > ");
			defaultInterpolatedStringHandler.AppendFormatted(50.0, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(" MB)");
			throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		IEnumerable<string> sessionCacheIds = GetSessionCacheIds(sessionId);
		if (sessionCacheIds.Count() >= 100)
		{
			method_2(sessionId, 25);
		}
		long num2 = sessionCacheIds.Select((string string_0) => (!concurrentDictionary_0.TryGetValue(string_0, out CacheEntry value2)) ? 0L : value2.EstimatedSizeBytes).Sum();
		if (num2 + num > 209715200L)
		{
			long long_ = num2 + num - 209715200L;
			method_3(sessionId, long_);
		}
		int num3 = concurrentDictionary_1.AddOrUpdate(sessionId, 1, (string string_0, int int_0) => int_0 + 1);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 1);
		defaultInterpolatedStringHandler2.AppendLiteral("cache_");
		defaultInterpolatedStringHandler2.AppendFormatted(num3);
		string text = defaultInterpolatedStringHandler2.ToStringAndClear();
		CacheEntry value = new CacheEntry
		{
			CacheId = text,
			SessionId = sessionId,
			Key = key,
			Data = data,
			CreatedAt = DateTime.UtcNow,
			LastAccessedAt = DateTime.UtcNow,
			AccessCount = 0,
			EstimatedSizeBytes = num,
			DataType = typeof(T).Name,
			SequenceNumber = num3
		};
		concurrentDictionary_0[text] = value;
		return text;
	}

	public T? Retrieve<T>(string cacheId)
	{
		method_1();
		if (string.IsNullOrEmpty(cacheId))
		{
			return default(T);
		}
		if (concurrentDictionary_0.TryGetValue(cacheId, out CacheEntry value))
		{
			readerWriterLockSlim_0.EnterWriteLock();
			try
			{
				value.LastAccessedAt = DateTime.UtcNow;
				value.AccessCount++;
			}
			finally
			{
				readerWriterLockSlim_0.ExitWriteLock();
			}
			object data = value.Data;
			if (data is T)
			{
				return (T)data;
			}
		}
		return default(T);
	}

	public T? RetrieveByKey<T>(string sessionId, string key)
	{
		method_1();
		if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(key))
		{
			foreach (KeyValuePair<string, CacheEntry> item in concurrentDictionary_0)
			{
				if (item.Value.SessionId == sessionId && item.Value.Key == key)
				{
					readerWriterLockSlim_0.EnterWriteLock();
					try
					{
						item.Value.LastAccessedAt = DateTime.UtcNow;
						item.Value.AccessCount++;
					}
					finally
					{
						readerWriterLockSlim_0.ExitWriteLock();
					}
					object data = item.Value.Data;
					if (data is T)
					{
						return (T)data;
					}
				}
			}
			return default(T);
		}
		return default(T);
	}

	public bool Exists(string cacheId)
	{
		method_1();
		if (!string.IsNullOrEmpty(cacheId))
		{
			return concurrentDictionary_0.ContainsKey(cacheId);
		}
		return false;
	}

	public bool Invalidate(string cacheId)
	{
		method_1();
		if (string.IsNullOrEmpty(cacheId))
		{
			return false;
		}
		CacheEntry value;
		return concurrentDictionary_0.TryRemove(cacheId, out value);
	}

	public void ClearSession(string sessionId)
	{
		method_1();
		if (string.IsNullOrEmpty(sessionId))
		{
			return;
		}
		foreach (string item in (from keyValuePair_0 in concurrentDictionary_0
			where keyValuePair_0.Value.SessionId == sessionId
			select keyValuePair_0.Key).ToList())
		{
			concurrentDictionary_0.TryRemove(item, out CacheEntry _);
		}
	}

	public void ClearAll()
	{
		method_1();
		concurrentDictionary_0.Clear();
	}

	public CacheStatistics? GetStatistics(string cacheId)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		method_1();
		if (string.IsNullOrEmpty(cacheId))
		{
			return null;
		}
		if (concurrentDictionary_0.TryGetValue(cacheId, out CacheEntry value))
		{
			readerWriterLockSlim_0.EnterReadLock();
			try
			{
				return new CacheStatistics
				{
					CacheId = value.CacheId,
					SessionId = value.SessionId,
					Key = value.Key,
					CreatedAt = value.CreatedAt,
					LastAccessedAt = value.LastAccessedAt,
					AccessCount = value.AccessCount,
					EstimatedSizeBytes = value.EstimatedSizeBytes,
					DataType = value.DataType
				};
			}
			finally
			{
				readerWriterLockSlim_0.ExitReadLock();
			}
		}
		return null;
	}

	public IEnumerable<string> GetSessionCacheIds(string sessionId)
	{
		method_1();
		if (string.IsNullOrEmpty(sessionId))
		{
			return Enumerable.Empty<string>();
		}
		return (from keyValuePair_0 in concurrentDictionary_0
			where keyValuePair_0.Value.SessionId == sessionId
			select keyValuePair_0.Key).ToList();
	}

	public CacheValidationResult ValidateCache(string cacheId)
	{
		method_1();
		if (string.IsNullOrEmpty(cacheId))
		{
			return CacheValidationResult.Invalid(cacheId ?? string.Empty, "缓存 ID 不能为空");
		}
		if (!concurrentDictionary_0.TryGetValue(cacheId, out CacheEntry _))
		{
			return CacheValidationResult.Invalid(cacheId, "缓存不存在或已过期");
		}
		CacheStatistics statistics = GetStatistics(cacheId);
		return CacheValidationResult.Valid(cacheId, statistics);
	}

	public CacheValidationResult ValidateCacheWithSession(string cacheId, string expectedSessionId)
	{
		method_1();
		if (string.IsNullOrEmpty(cacheId))
		{
			return CacheValidationResult.Invalid(cacheId ?? string.Empty, "缓存 ID 不能为空");
		}
		if (!concurrentDictionary_0.TryGetValue(cacheId, out CacheEntry value))
		{
			return CacheValidationResult.Invalid(cacheId, "缓存不存在或已过期");
		}
		if (value.SessionId != expectedSessionId)
		{
			return CacheValidationResult.SessionMismatch(cacheId, expectedSessionId, value.SessionId);
		}
		CacheStatistics statistics = GetStatistics(cacheId);
		return CacheValidationResult.Valid(cacheId, statistics);
	}

	private long method_0(object? object_0)
	{
		if (object_0 == null)
		{
			return 0L;
		}
		if (object_0 is string text)
		{
			return text.Length * 2;
		}
		if (object_0 is ICollection collection)
		{
			return collection.Count * 64;
		}
		return 128L;
	}

	private void method_1()
	{
		if (bool_0)
		{
			throw new ObjectDisposedException("SessionAIToolDataCache");
		}
	}

	private void method_2(string string_0, int int_1)
	{
		foreach (string item in (from keyValuePair_0 in (from keyValuePair_0 in concurrentDictionary_0
				where keyValuePair_0.Value.SessionId == string_0
				orderby keyValuePair_0.Value.LastAccessedAt
				select keyValuePair_0).Take(int_1)
			select keyValuePair_0.Key).ToList())
		{
			concurrentDictionary_0.TryRemove(item, out CacheEntry _);
		}
	}

	private void method_3(string string_0, long long_2)
	{
		long num = 0L;
		foreach (KeyValuePair<string, CacheEntry> item in (from keyValuePair_0 in concurrentDictionary_0
			where keyValuePair_0.Value.SessionId == string_0
			orderby keyValuePair_0.Value.LastAccessedAt
			select keyValuePair_0).ToList())
		{
			if (num < long_2)
			{
				string key = item.Key;
				CacheEntry value = item.Value;
				if (concurrentDictionary_0.TryRemove(key, out CacheEntry _))
				{
					num += value.EstimatedSizeBytes;
				}
				continue;
			}
			break;
		}
	}

	public void Dispose()
	{
		if (!bool_0)
		{
			concurrentDictionary_0.Clear();
			readerWriterLockSlim_0.Dispose();
			bool_0 = true;
		}
	}

	[CompilerGenerated]
	private long method_4<T>(string string_0)
	{
		if (!concurrentDictionary_0.TryGetValue(string_0, out CacheEntry value))
		{
			return 0L;
		}
		return value.EstimatedSizeBytes;
	}
}
