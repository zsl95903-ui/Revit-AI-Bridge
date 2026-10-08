using System.Text.Json;
using System.Text.Json.Serialization;

namespace RevitAi.Engine.Core;

/// <summary>
/// 代码片段库（纯文本仓库，**不执行任何代码**）。
/// 存储位置：%LOCALAPPDATA%\RevitAiEngine\snippets.json，可用环境变量 REVITAI_SNIPPETS_PATH 覆盖（测试用）。
/// </summary>
public sealed class SnippetLibrary
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string _path;

    public SnippetLibrary(string? path = null)
    {
        _path = path ?? ResolveDefaultPath();
    }

    public string Path => _path;

    public static string ResolveDefaultPath()
    {
        var overridden = Environment.GetEnvironmentVariable("REVITAI_SNIPPETS_PATH");
        if (!string.IsNullOrWhiteSpace(overridden))
        {
            return overridden!;
        }

        return System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RevitAiEngine",
            "snippets.json");
    }

    public IReadOnlyList<Snippet> List(string? keyword, string? tag)
    {
        var items = Load();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            items = items
                .Where(item =>
                    item.Name.Contains(keyword!, StringComparison.OrdinalIgnoreCase) ||
                    item.Description.Contains(keyword!, StringComparison.OrdinalIgnoreCase) ||
                    item.Code.Contains(keyword!, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            items = items
                .Where(item => item.Tags.Any(item2 => string.Equals(item2, tag, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        return items.OrderBy(item => item.Id, StringComparer.Ordinal).ToList();
    }

    public Snippet? Get(string? id, string? name)
    {
        var items = Load();
        return items.FirstOrDefault(item =>
            (!string.IsNullOrWhiteSpace(id) && string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(name) && string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase)));
    }

    public Snippet Save(string? id, string name, string description, string code, IReadOnlyList<string> tags)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var items = Load();
        var existingIndex = -1;
        if (!string.IsNullOrWhiteSpace(id))
        {
            existingIndex = items.FindIndex(item => string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        if (existingIndex < 0)
        {
            existingIndex = items.FindIndex(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        var snippet = new Snippet
        {
            Id = existingIndex >= 0 ? items[existingIndex].Id : Guid.NewGuid().ToString("N")[..8],
            Name = name,
            Description = description,
            Code = code,
            Tags = tags.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            UpdatedAt = DateTimeOffset.Now,
        };

        if (existingIndex >= 0)
        {
            items[existingIndex] = snippet;
        }
        else
        {
            items.Add(snippet);
        }

        Save(items);
        return snippet;
    }

    public bool Delete(string? id, string? name)
    {
        var items = Load();
        var removed = items.RemoveAll(item =>
            (!string.IsNullOrWhiteSpace(id) && string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(name) && string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase)));

        if (removed > 0)
        {
            Save(items);
        }

        return removed > 0;
    }

    private List<Snippet> Load()
    {
        try
        {
            if (!File.Exists(_path))
            {
                return new List<Snippet>();
            }

            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Snippet>>(json, Options) ?? new List<Snippet>();
        }
        catch (Exception)
        {
            return new List<Snippet>();
        }
    }

    private void Save(List<Snippet> items)
    {
        var directory = System.IO.Path.GetDirectoryName(_path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(_path, JsonSerializer.Serialize(items, Options));
    }
}

public sealed class Snippet
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();

    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// execute_code 的"受限计划"解析器：只接受声明式的工具调用计划，**绝不执行任意代码**。
/// 支持两种写法：
///   1) JSON：{"steps":[{"tool":"create_grid","parameters":{...}}]}
///   2) 行式：tool(create_grid, {"grids":[...]})，每行一步
/// </summary>
public static class RestrictedCodePlan
{
    // 只拦截"代码"特征标记；注意不能拦 { } ; 等，因为受限计划本身就是 JSON 文本。
    private static readonly string[] RejectedMarkers =
    {
        "using ", "namespace ", "class ", "public ", "private ", "static void",
        "import ", "def ", "eval(", "exec(", "__", "System.", "Process.",
        "Reflection", "Assembly.Load", "dynamic ", "unsafe",
    };

    public sealed record Step(string Tool, IReadOnlyDictionary<string, object?> Parameters);

    public sealed record PlanResult(bool Accepted, string Message, int StepCount, IReadOnlyList<Step> Steps, IReadOnlyList<string> Violations);

    public static PlanResult Parse(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return new PlanResult(false, "必须提供 code（受限计划文本）。", 0, Array.Empty<Step>(), new[] { "code 为空" });
        }

        var violations = new List<string>();
        foreach (var marker in RejectedMarkers)
        {
            if (code.Contains(marker, StringComparison.OrdinalIgnoreCase))
            {
                violations.Add($"包含被禁止的标记：{marker.Trim()}");
            }
        }

        // 先尝试 JSON 计划。
        var steps = TryParseJson(code);
        if (steps.Count == 0 && violations.Count == 0)
        {
            steps = TryParseLines(code);
        }

        if (violations.Count > 0)
        {
            return new PlanResult(
                false,
                "本引擎不执行任意代码（C#/Python/表达式等一律拒绝）。如需批量操作，请改写成受限计划："
                + "JSON {\"steps\":[{\"tool\":\"<工具名>\",\"parameters\":{...}}]}，或每行 tool(<工具名>, {json})。",
                steps.Count,
                steps,
                violations);
        }

        if (steps.Count == 0)
        {
            return new PlanResult(false, "没有解析出任何步骤（请检查受限计划格式）。", 0, Array.Empty<Step>(), new[] { "空计划" });
        }

        return new PlanResult(true, $"已解析 {steps.Count} 步受限计划（需要宿主按步执行对应工具）。", steps.Count, steps, Array.Empty<string>());
    }

    private static List<Step> TryParseJson(string code)
    {
        try
        {
            using var document = JsonDocument.Parse(code);
            var root = document.RootElement;

            var stepsElement = root.ValueKind == JsonValueKind.Object && root.TryGetProperty("steps", out var stepsProperty)
                ? stepsProperty
                : root;

            if (stepsElement.ValueKind != JsonValueKind.Array)
            {
                return new List<Step>();
            }

            var steps = new List<Step>();
            foreach (var item in stepsElement.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object ||
                    !item.TryGetProperty("tool", out var toolProperty) ||
                    toolProperty.ValueKind != JsonValueKind.String)
                {
                    continue;
                }

                var parameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                if (item.TryGetProperty("parameters", out var parametersProperty) && parametersProperty.ValueKind == JsonValueKind.Object)
                {
                    foreach (var property in parametersProperty.EnumerateObject())
                    {
                        parameters[property.Name] = Convert(property.Value);
                    }
                }

                steps.Add(new Step(toolProperty.GetString() ?? string.Empty, parameters));
            }

            return steps;
        }
        catch (JsonException)
        {
            return new List<Step>();
        }
    }

    private static List<Step> TryParseLines(string code)
    {
        var steps = new List<Step>();
        foreach (var rawLine in code.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var line = rawLine;
            if (line.StartsWith("tool(", StringComparison.OrdinalIgnoreCase) && line.EndsWith(')'))
            {
                line = line[5..^1];
            }

            var parts = line.Split(',', 2);
            if (parts.Length == 0)
            {
                continue;
            }

            var tool = parts[0].Trim().Trim('"');
            var parameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[1]))
            {
                try
                {
                    using var document = JsonDocument.Parse(parts[1].Trim());
                    if (document.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var property in document.RootElement.EnumerateObject())
                        {
                            parameters[property.Name] = Convert(property.Value);
                        }
                    }
                }
                catch (JsonException)
                {
                    // 参数不是合法 JSON，保留空参数。
                }
            }

            if (!string.IsNullOrWhiteSpace(tool))
            {
                steps.Add(new Step(tool, parameters));
            }
        }

        return steps;
    }

    private static object? Convert(JsonElement element)
        => element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var integer) ? integer : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Array => element.EnumerateArray().Select(Convert).ToList(),
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(property => property.Name, property => Convert(property.Value), StringComparer.OrdinalIgnoreCase),
            _ => null,
        };
}
