using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ReVitAI.Bridge;

internal sealed class BridgeEnvelope
{
    public string? Type { get; set; }
    public string? RequestId { get; set; }
    public string? Tool { get; set; }
    public JsonElement Arguments { get; set; }
    public string? Message { get; set; }
}

internal sealed class ToolDefinition
{
    public required string Name { get; init; }
    public string Category { get; set; } = "ReVitAI";
    public required string Description { get; init; }
    public required bool Mutating { get; init; }
    public bool RequiresTransaction { get; set; }
    public required bool RequiresActiveDocument { get; init; }
    public required Func<InvocationContext, object?> Handler { get; init; }
    public object? InputSchema { get; set; }
}

internal sealed record InvocationContext
{
    public required UIApplication UiApplication { get; init; }
    public required JsonElement Arguments { get; init; }
    public bool DryRun { get; init; }

    public Autodesk.Revit.DB.Document Document =>
        UiApplication.ActiveUIDocument?.Document
        ?? throw new InvalidOperationException("No active Revit document.");

    public Autodesk.Revit.UI.UIDocument UiDocument =>
        UiApplication.ActiveUIDocument
        ?? throw new InvalidOperationException("No active Revit UI document.");
}

internal sealed class PendingCommand
{
    public required string Tool { get; init; }
    public required JsonElement Arguments { get; init; }
    public required TaskCompletionSource<object?> Completion { get; init; }
}

internal static class Json
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
    };

    public static JsonElement EmptyObject()
    {
        using var document = JsonDocument.Parse("{}");
        return document.RootElement.Clone();
    }

    public static string? String(JsonElement element, string name)
    {
        return element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(name, out var value)
            && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    public static string? StringAny(JsonElement element, params string[] names)
    {
        foreach (var name in names)
        {
            var value = String(element, name);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    public static double Double(JsonElement element, string name, double fallback = 0)
    {
        if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(name, out var value))
        {
            return fallback;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var number))
        {
            return number;
        }

        if (value.ValueKind == JsonValueKind.String
            && double.TryParse(value.GetString(), out number))
        {
            return number;
        }

        return fallback;
    }

    public static int Int(JsonElement element, string name, int fallback = 0)
    {
        return (int)Math.Round(Double(element, name, fallback));
    }

    public static long Long(JsonElement element, string name, long fallback = 0)
    {
        return (long)Math.Round(Double(element, name, fallback));
    }

    public static bool Bool(JsonElement element, string name, bool fallback = false)
    {
        if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(name, out var value))
        {
            return fallback;
        }

        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number => value.GetDouble() != 0,
            JsonValueKind.String => bool.TryParse(value.GetString(), out var parsed) && parsed,
            _ => fallback,
        };
    }

    public static IEnumerable<JsonElement> Array(JsonElement element, string name)
    {
        if (element.ValueKind != JsonValueKind.Object
            || !element.TryGetProperty(name, out var value)
            || value.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return value.EnumerateArray();
    }

    public static XYZ Point(JsonElement element, string name, double zMillimeters = 0)
    {
        if (element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(name, out var point)
            && point.ValueKind == JsonValueKind.Object)
        {
            return Units.Point(
                Double(point, "x", Double(point, "X")),
                Double(point, "y", Double(point, "Y")),
                Double(point, "z", zMillimeters));
        }

        return Units.Point(
            Double(element, $"{name}_x", Double(element, $"{name}X")),
            Double(element, $"{name}_y", Double(element, $"{name}Y")),
            Double(element, $"{name}_z", zMillimeters));
    }
}
