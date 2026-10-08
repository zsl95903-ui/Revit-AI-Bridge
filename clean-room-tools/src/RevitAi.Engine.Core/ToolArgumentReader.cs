using System.Globalization;

namespace RevitAi.Engine.Core;

/// <summary>
/// 工具入参读取辅助：把"单值或数组"两种写法统一成列表。
/// 厂商工具用 AIToolContext.GetParameter&lt;T&gt; 处理同样的兼容问题（见 AIToolContext.cs:35-155）。
/// </summary>
public static class ToolArgumentReader
{
    public static List<double> ReadNumbers(IDictionary<string, object?> parameters, string key)
    {
        if (!parameters.TryGetValue(key, out var value) || value is null)
        {
            return new List<double>();
        }

        return value switch
        {
            IReadOnlyList<object?> list => list.Select(ToDouble).ToList(),
            string text when double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                => new List<double> { parsed },
            _ => new List<double> { ToDouble(value) },
        };
    }

    public static List<string> ReadStrings(IDictionary<string, object?> parameters, string key)
    {
        if (!parameters.TryGetValue(key, out var value) || value is null)
        {
            return new List<string>();
        }

        return value switch
        {
            IReadOnlyList<object?> list => list.Select(item => item?.ToString() ?? string.Empty).ToList(),
            _ => new List<string> { value.ToString() ?? string.Empty },
        };
    }

    public static List<int> ReadIntegers(IDictionary<string, object?> parameters, params string[] keys)
    {
        var result = new List<int>();
        foreach (var key in keys)
        {
            if (!parameters.TryGetValue(key, out var value) || value is null)
            {
                continue;
            }

            if (value is IReadOnlyList<object?> list)
            {
                result.AddRange(list.Select(item => (int)ToDouble(item)));
            }
            else
            {
                result.Add((int)ToDouble(value));
            }
        }

        return result;
    }

    /// <summary>从缓存条目（字典数组，通常带 id 字段）里抽取元素 ID。</summary>
    public static List<int> ExtractIds(IEnumerable<Dictionary<string, object?>>? items, int limit = int.MaxValue)
    {
        var result = new List<int>();
        if (items is null)
        {
            return result;
        }

        foreach (var item in items)
        {
            if (result.Count >= limit)
            {
                break;
            }

            if (item.TryGetValue("id", out var raw) && raw is not null)
            {
                result.Add((int)ToDouble(raw));
            }
        }

        return result;
    }

    public static double ToDouble(object? value) => value switch
    {
        null => 0d,
        long l => l,
        int i => i,
        double d => d,
        decimal m => (decimal.ToDouble(m)),
        float f => f,
        bool b => b ? 1d : 0d,
        string s when double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) => parsed,
        _ => 0d,
    };
}
