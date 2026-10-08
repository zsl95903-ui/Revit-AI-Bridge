using System.Text.Json;

namespace RevitAi.Engine.Core;

/// <summary>
/// 把模型给出的 JSON 参数解析成 AIToolContext.Parameters 需要的 CLR 值。
/// 整数归一到 long、浮点归一到 double，数组归一到 List&lt;object?&gt;，
/// 与厂商 AIToolContext.GetParameter&lt;T&gt; 的兼容层配套使用。
/// </summary>
public static class ToolParameterParser
{
    public static IDictionary<string, object?> Parse(string? json)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(json))
        {
            return result;
        }

        using var document = JsonDocument.Parse(json);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("工具参数必须是 JSON 对象。", nameof(json));
        }

        foreach (var property in document.RootElement.EnumerateObject())
        {
            result[property.Name] = Convert(property.Value);
        }

        return result;
    }

    private static object? Convert(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Object => element.EnumerateObject()
            .ToDictionary(p => p.Name, p => Convert(p.Value), StringComparer.OrdinalIgnoreCase),
        JsonValueKind.Array => element.EnumerateArray().Select(Convert).ToList(),
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number => element.TryGetInt64(out var integer) ? integer : element.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        _ => null,
    };
}
