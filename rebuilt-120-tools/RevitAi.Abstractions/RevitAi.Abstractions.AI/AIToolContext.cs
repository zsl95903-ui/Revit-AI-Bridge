using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Units;
using Newtonsoft.Json.Linq;

namespace RevitAi.Abstractions.AI;

public sealed class AIToolContext
{
	public object? Document { get; set; }

	public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

	public IRevitAdapter? RevitAdapter { get; set; }

	public object? ExternalEvent { get; set; }

	public object? UserData { get; set; }

	public object? Transaction { get; set; }

	public string? TransactionName { get; set; }

	public bool HasTransaction => Transaction != null;

	public string? SessionId { get; set; }

	public IAIToolDataCache? DataCache { get; set; }

	public IUnitService? UnitService { get; set; }

	public IProjectUnitService? ProjectUnitService { get; set; }

	public T GetParameter<T>(string key, T defaultValue = default(T))
	{
		if (Parameters.TryGetValue(key, out object value))
		{
			if (value is T val)
			{
				if (val is List<object> list && NeedConvertJToken(list))
				{
					List<object> list2 = new List<object>();
					foreach (object item in list)
					{
						object obj = ConvertJTokenToObjectRecursive(item);
						if (obj != null)
						{
							list2.Add(obj);
						}
					}
					return (T)(object)list2;
				}
				return val;
			}
			if (value is long num && (typeof(T) == typeof(int) || typeof(T) == typeof(int?)))
			{
				return (T)(object)checked((int)num);
			}
			if (typeof(T) == typeof(double) && value is decimal num2)
			{
				return (T)(object)(double)num2;
			}
			if (typeof(T) == typeof(List<string>) && value is List<object> list3)
			{
				List<string> list4 = new List<string>();
				foreach (object item2 in list3)
				{
					if (item2 != null)
					{
						list4.Add(item2.ToString() ?? string.Empty);
					}
				}
				return (T)(object)list4;
			}
			if (typeof(T) == typeof(IList<string>) && value is List<object> list5)
			{
				List<string> list6 = new List<string>();
				foreach (object item3 in list5)
				{
					if (item3 != null)
					{
						list6.Add(item3.ToString() ?? string.Empty);
					}
				}
				return (T)(object)list6;
			}
			if (typeof(T) == typeof(int[]) && value is List<object> list7)
			{
				int[] array = new int[list7.Count];
				for (int i = 0; i < list7.Count; i++)
				{
					if (list7[i] != null)
					{
						if (list7[i] is long num3)
						{
							array[i] = (int)num3;
						}
						else
						{
							array[i] = Convert.ToInt32(list7[i]);
						}
					}
				}
				return (T)(object)array;
			}
			if (typeof(T) == typeof(string[]) && value is List<object> list8)
			{
				string[] array2 = new string[list8.Count];
				for (int j = 0; j < list8.Count; j++)
				{
					array2[j] = list8[j]?.ToString() ?? string.Empty;
				}
				return (T)(object)array2;
			}
			if (typeof(T) == typeof(List<object>) && value is JArray jArray)
			{
				List<object> list9 = new List<object>();
				foreach (JToken item4 in jArray)
				{
					if (item4 is JObject jObject)
					{
						Dictionary<string, object> dictionary = new Dictionary<string, object>();
						foreach (JProperty item5 in jObject.Properties())
						{
							object obj2 = ConvertJTokenToObject(item5.Value);
							dictionary[item5.Name] = obj2 ?? new object();
						}
						list9.Add(dictionary);
					}
					else if (item4 != null)
					{
						object obj3 = ConvertJTokenToObject(item4);
						if (obj3 != null)
						{
							list9.Add(obj3);
						}
					}
				}
				return (T)(object)list9;
			}
			if (typeof(T) == typeof(List<object>) && value is List<object> list10)
			{
				return (T)(object)list10;
			}
			try
			{
				return (T)Convert.ChangeType(value, typeof(T));
			}
			catch
			{
			}
		}
		return defaultValue;
	}

	public bool HasParameter(string key)
	{
		return Parameters.ContainsKey(key);
	}

	private static object? ConvertJTokenToObject(JToken token)
	{
		if (token == null)
		{
			return null;
		}
		switch (token.Type)
		{
		case JTokenType.Object:
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			{
				foreach (JProperty item in ((JObject)token).Properties())
				{
					object obj2 = ConvertJTokenToObject(item.Value);
					dictionary[item.Name] = obj2 ?? new object();
				}
				return dictionary;
			}
		}
		case JTokenType.Array:
		{
			List<object> list = new List<object>();
			{
				foreach (JToken item2 in (JArray)token)
				{
					object obj = ConvertJTokenToObject(item2);
					if (obj != null)
					{
						list.Add(obj);
					}
				}
				return list;
			}
		}
		case JTokenType.Integer:
			if (!(((JValue)token).Value is long num2))
			{
				return ((JValue)token).Value;
			}
			return num2;
		case JTokenType.Float:
			if (!(((JValue)token).Value is double num))
			{
				return ((JValue)token).Value;
			}
			return num;
		case JTokenType.String:
			return ((JValue)token).ToString();
		case JTokenType.Boolean:
			return (bool)(JToken)(JValue)token;
		case JTokenType.Null:
		case JTokenType.Undefined:
			return null;
		default:
			return ((JValue)token).Value;
		}
	}

	private static object? ConvertJTokenToObjectRecursive(object obj)
	{
		if (obj is JToken token)
		{
			return ConvertJTokenToObject(token);
		}
		return obj;
	}

	private static bool NeedConvertJToken(List<object> list)
	{
		foreach (object item in list)
		{
			if (item is JToken)
			{
				return true;
			}
		}
		return false;
	}

	public T? GetCachedData<T>(string cacheId)
	{
		if (DataCache == null || string.IsNullOrEmpty(cacheId))
		{
			return default(T);
		}
		return DataCache.Retrieve<T>(cacheId);
	}

	public T? GetCachedDataByKey<T>(string key)
	{
		if (DataCache == null || string.IsNullOrEmpty(SessionId) || string.IsNullOrEmpty(key))
		{
			return default(T);
		}
		return DataCache.RetrieveByKey<T>(SessionId, key);
	}

	public IEnumerable<T> GetCachedDataMultiple<T>(IEnumerable<string> cacheIds)
	{
		if (DataCache == null || cacheIds == null)
		{
			yield break;
		}
		foreach (string cacheId in cacheIds)
		{
			T val = DataCache.Retrieve<T>(cacheId);
			if (val != null)
			{
				yield return val;
			}
		}
	}

	public bool HasCache(string cacheId)
	{
		if (DataCache != null && !string.IsNullOrEmpty(cacheId))
		{
			return DataCache.Exists(cacheId);
		}
		return false;
	}

	public CacheValidationResult ValidateCache(string cacheId)
	{
		if (DataCache == null)
		{
			return CacheValidationResult.Invalid(cacheId, "数据缓存服务未初始化");
		}
		if (string.IsNullOrEmpty(cacheId))
		{
			return CacheValidationResult.Invalid(cacheId ?? string.Empty, "缓存 ID 不能为空");
		}
		return DataCache.ValidateCache(cacheId);
	}

	public CacheValidationResult ValidateCacheWithSession(string cacheId, string expectedSessionId)
	{
		if (DataCache == null)
		{
			return CacheValidationResult.Invalid(cacheId, "数据缓存服务未初始化");
		}
		if (string.IsNullOrEmpty(cacheId))
		{
			return CacheValidationResult.Invalid(cacheId ?? string.Empty, "缓存 ID 不能为空");
		}
		CacheValidationResult cacheValidationResult = DataCache.ValidateCache(cacheId);
		if (!cacheValidationResult.IsValid)
		{
			return cacheValidationResult;
		}
		CacheStatistics statistics = cacheValidationResult.Statistics;
		if (statistics != null && statistics.SessionId != expectedSessionId)
		{
			return CacheValidationResult.SessionMismatch(cacheId, expectedSessionId, statistics.SessionId);
		}
		return cacheValidationResult;
	}
}
