namespace RevitAi.Engine.Abstractions;

/// <summary>
/// 一次工具调用的执行上下文。字段对齐厂商 AIToolContext（Document/Parameters/RevitAdapter/
/// ExternalEvent/Transaction/SessionId/DataCache/UnitService），并保留其 GetParameter&lt;T&gt;
/// 的 JSON→CLR 兼容层语义（long→int、decimal→double、数组→int[]/string[]）。
/// 参考：反编译重建源码 Abstractions\AS\Tools\Abstractions\AI\AIToolContext.cs。
/// </summary>
public sealed class AIToolContext
{
    public object? Document { get; set; }

    public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Revit 适配器（Revit 侧为 IRevitAdapter 实现，Core 不引用 Revit 类型）。</summary>
    public object? RevitAdapter { get; set; }

    public object? ExternalEvent { get; set; }

    public object? Transaction { get; set; }

    public string? TransactionName { get; set; }

    public bool HasTransaction => Transaction is not null;

    public string? SessionId { get; set; }

    public IAIToolDataCache? DataCache { get; set; }

    public IUnitConverter? UnitService { get; set; }

    public bool HasParameter(string key) => Parameters.ContainsKey(key);

    /// <summary>
    /// 按目标类型取参数，容忍 JSON 解析后的实际类型差异。
    /// </summary>
    public T GetParameter<T>(string key, T defaultValue = default!)
    {
        if (!Parameters.TryGetValue(key, out var value) || value is null)
        {
            return defaultValue;
        }

        if (value is T typed)
        {
            return typed;
        }

        var target = typeof(T);

        // JSON 整数会解析成 long，这里补齐到 int / int?
        if (value is long longValue)
        {
            if (target == typeof(int) || target == typeof(int?))
            {
                return (T)(object)checked((int)longValue);
            }

            if (target == typeof(double) || target == typeof(double?))
            {
                return (T)(object)(double)longValue;
            }
        }

        if (value is decimal decimalValue && (target == typeof(double) || target == typeof(double?)))
        {
            return (T)(object)(double)decimalValue;
        }

        if (target == typeof(string))
        {
            return (T)(object)value.ToString()!;
        }

        if (target == typeof(int[]) && value is IReadOnlyList<object?> objectList)
        {
            return (T)(object)ToIntArray(objectList);
        }

        if (target == typeof(List<int>) && value is IReadOnlyList<object?> objectList2)
        {
            return (T)(object)ToIntArray(objectList2).ToList();
        }

        if (target == typeof(List<string>) && value is IReadOnlyList<object?> objectList3)
        {
            return (T)(object)objectList3.Select(item => item?.ToString() ?? string.Empty).ToList();
        }

        if (target == typeof(List<double>) && value is IReadOnlyList<object?> objectList4)
        {
            return (T)(object)objectList4.Select(ToDouble).ToList();
        }

        if (target == typeof(List<object?>))
        {
            return value is IReadOnlyList<object?> list ? (T)(object)list.ToList() : (T)(object)new List<object?> { value };
        }

        try
        {
            return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(target) ?? target);
        }
        catch
        {
            return defaultValue;
        }
    }

    public T? GetCachedData<T>(string? cacheId)
        => DataCache is null || string.IsNullOrEmpty(cacheId) ? default : DataCache.Retrieve<T>(cacheId);

    public bool HasCache(string? cacheId)
        => DataCache is not null && !string.IsNullOrEmpty(cacheId) && DataCache.Exists(cacheId);

    public CacheValidationResult ValidateCache(string? cacheId)
    {
        if (DataCache is null)
        {
            return CacheValidationResult.Invalid(cacheId ?? string.Empty, "数据缓存服务未初始化");
        }

        if (string.IsNullOrEmpty(cacheId))
        {
            return CacheValidationResult.Invalid(string.Empty, "缓存 ID 不能为空");
        }

        return DataCache.ValidateCache(cacheId);
    }

    private static int[] ToIntArray(IReadOnlyList<object?> items)
    {
        var result = new int[items.Count];
        for (var i = 0; i < items.Count; i++)
        {
            result[i] = (int)ToDouble(items[i]);
        }

        return result;
    }

    private static double ToDouble(object? value) => value switch
    {
        null => 0d,
        long l => l,
        int i => i,
        double d => d,
        decimal m => (double)m,
        string s when double.TryParse(s, out var parsed) => parsed,
        _ => Convert.ToDouble(value)
    };
}
