using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

// RevitToolSet 工具集生成器
// ------------------------------------------------------------------
// 输入：厂商 AS.Tools R2027 的 TOOL-API.json（120 个工具的契约事实：name/category/description/
//      requiresTransaction/requiresModification/parameters JSON Schema）
// 输出：
//   1) GeneratedTools.g.cs        —— 120 个工具类（含 [AITool] 元数据），每个类只声明契约，
//                                    执行逻辑交给 ToolImplementationRegistry（自研实现）或回退引擎。
//   2) tool-index.json            —— 工具名索引（供无 Revit 的测试做全量契约对拍）。
//   3) 工具索引.md                —— 人读版索引。
// 用法：
//   dotnet run --project tools/ToolSetGenerator -- \
//     --catalog <TOOL-API.json> --out-tools <dir> --out-index <dir> --namespace RevitToolSet
// ------------------------------------------------------------------

var options = ParseArgs(args);

var catalogPath = options.GetValueOrDefault("catalog")
    ?? throw new InvalidOperationException("缺少 --catalog 参数（厂商 TOOL-API.json 路径）。");
var outToolsDirectory = options.GetValueOrDefault("out-tools")
    ?? throw new InvalidOperationException("缺少 --out-tools 参数（生成目录）。");
var namespaceName = options.GetValueOrDefault("namespace") ?? "RevitToolSet";

if (!File.Exists(catalogPath))
{
    Console.Error.WriteLine($"找不到契约文件：{catalogPath}");
    return 2;
}

var catalog = LoadCatalog(catalogPath);
Console.WriteLine($"读入厂商工具契约 {catalog.Count} 个。");

// 自研扩展工具（厂商 120 项里没有）：随生成一起产出，保证工具集是"厂商 120 + 自研扩展"。
catalog.AddRange(ExtraTools());

Directory.CreateDirectory(outToolsDirectory);

var generatedPath = Path.Combine(outToolsDirectory, "GeneratedTools.g.cs");
File.WriteAllText(generatedPath, EmitTools(catalog, namespaceName), new UTF8Encoding(false));

var indexPath = Path.Combine(outToolsDirectory, "tool-index.json");
File.WriteAllText(indexPath, EmitIndex(catalog), new UTF8Encoding(false));

var markdownPath = Path.Combine(outToolsDirectory, "工具索引.md");
File.WriteAllText(markdownPath, EmitMarkdown(catalog, namespaceName), new UTF8Encoding(false));

Console.WriteLine($"生成完成：");
Console.WriteLine($"  工具类      : {generatedPath}");
Console.WriteLine($"  工具名索引  : {indexPath}");
Console.WriteLine($"  人读索引    : {markdownPath}");
Console.WriteLine($"  工具总数    : {catalog.Count}");
return 0;

static Dictionary<string, string> ParseArgs(string[] arguments)
{
    var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    for (var i = 0; i < arguments.Length; i++)
    {
        if (!arguments[i].StartsWith("--", StringComparison.Ordinal))
        {
            continue;
        }

        var key = arguments[i][2..];
        var value = i + 1 < arguments.Length && !arguments[i + 1].StartsWith("--", StringComparison.Ordinal)
            ? arguments[++i]
            : "true";
        result[key] = value;
    }

    return result;
}

static List<ToolSeed> LoadCatalog(string path)
{
    using var document = JsonDocument.Parse(File.ReadAllText(path));
    var seeds = new List<ToolSeed>();

    foreach (var element in document.RootElement.EnumerateArray())
    {
        var name = element.GetProperty("name").GetString()!;
        var className = element.TryGetProperty("class", out var classElement)
            ? classElement.GetString()
            : null;

        seeds.Add(new ToolSeed(
            Name: name,
            Category: element.TryGetProperty("category", out var category) ? category.GetString() ?? "通用" : "通用",
            Description: element.TryGetProperty("description", out var description) ? description.GetString() ?? string.Empty : string.Empty,
            RequiresTransaction: element.TryGetProperty("requiresTransaction", out var tx) && tx.GetBoolean(),
            RequiresModification: element.TryGetProperty("requiresModification", out var mod) && mod.GetBoolean(),
            RequiresActiveDocument: true,
            Schema: element.TryGetProperty("parameters", out var parameters) ? Compact(parameters) : """{"type":"object","properties":{}}""",
            ClassName: ShortClassName(className, name),
            Source: "vendor"));
    }

    return seeds;
}

static IEnumerable<ToolSeed> ExtraTools() => new[]
{
    new ToolSeed(
        Name: "upsert_level",
        Category: "标高管理",
        Description: "按名称或容差创建或更新标高；支持 dry_run 只预演不落库。",
        RequiresTransaction: true,
        RequiresModification: true,
        RequiresActiveDocument: true,
        Schema: """
        {"type":"object","properties":{"name":{"type":"string","description":"标高名称。"},"elevation":{"type":"number","description":"标高，单位米。"},"updateExisting":{"type":"boolean","default":true},"buildingStory":{"type":"boolean","default":true},"toleranceMm":{"type":"number","default":5},"dry_run":{"type":"boolean","description":"仅预演，不修改模型。"}},"required":["name","elevation"],"additionalProperties":false}
        """.Trim(),
        ClassName: "UpsertLevelTool",
        Source: "self"),
};

static string Compact(JsonElement element) => JsonSerializer.Serialize(element);

static string ShortClassName(string? fullyQualifiedName, string toolName)
{
    if (!string.IsNullOrWhiteSpace(fullyQualifiedName))
    {
        var last = fullyQualifiedName.Split('.').Last().Trim();
        if (last.Length > 0)
        {
            return last;
        }
    }

    var builder = new StringBuilder();
    foreach (var part in toolName.Split('_', StringSplitOptions.RemoveEmptyEntries))
    {
        builder.Append(char.ToUpperInvariant(part[0])).Append(part[1..]);
    }

    builder.Append("Tool");
    return builder.ToString();
}

static string EmitTools(List<ToolSeed> seeds, string namespaceName)
{
    var builder = new StringBuilder();
    builder.AppendLine("// <auto-generated>");
    builder.AppendLine("//   由 tools/ToolSetGenerator 依据厂商 TOOL-API.json 生成，请勿手工修改。");
    builder.AppendLine("//   每个类只声明契约（名称/分类/描述/Schema/事务标志），执行逻辑在 GeneratedToolBase 中");
    builder.AppendLine("//   分派给 ToolImplementationRegistry（自研实现）或回退引擎（厂商注册表）。");
    builder.AppendLine("// </auto-generated>");
    builder.AppendLine("#nullable enable");
    builder.AppendLine("using RevitAi.Engine.Abstractions;");
    builder.AppendLine("using RevitAi.Engine.Abstractions.Adapters;");
    builder.AppendLine();
    builder.AppendLine($"namespace {namespaceName}.Tools;");
    builder.AppendLine();

    var used = new HashSet<string>(StringComparer.Ordinal);
    foreach (var seed in seeds)
    {
        var className = seed.ClassName;
        var suffix = 2;
        while (!used.Add(className))
        {
            className = seed.ClassName + suffix++;
        }

        builder.AppendLine($"[AITool({Literal(seed.Name)}, Category = {Literal(seed.Category)}, Description = {Literal(seed.Description)}, RequiresTransaction = {Bool(seed.RequiresTransaction)}, RequiresModification = {Bool(seed.RequiresModification)}, RequiresActiveDocument = {Bool(seed.RequiresActiveDocument)})]");
        builder.AppendLine($"public sealed class {className} : GeneratedToolBase");
        builder.AppendLine("{");
        builder.AppendLine($"    public {className}(IRevitAdapter adapter) : base(adapter) {{ }}");
        builder.AppendLine();
        builder.AppendLine($"    public override string Name => {Literal(seed.Name)};");
        builder.AppendLine();
        builder.AppendLine($"    public override string Category => {Literal(seed.Category)};");
        builder.AppendLine();
        builder.AppendLine($"    public override string Description => {Literal(seed.Description)};");
        builder.AppendLine();
        builder.AppendLine($"    public override string ParametersSchema => {Literal(seed.Schema)};");
        builder.AppendLine("}");
        builder.AppendLine();
    }

    return builder.ToString();
}

static string EmitIndex(List<ToolSeed> seeds)
{
    var array = new JsonArray();
    foreach (var seed in seeds)
    {
        array.Add(new JsonObject
        {
            ["name"] = seed.Name,
            ["className"] = seed.ClassName,
            ["category"] = seed.Category,
            ["description"] = seed.Description,
            ["requiresTransaction"] = seed.RequiresTransaction,
            ["requiresModification"] = seed.RequiresModification,
            ["requiresActiveDocument"] = seed.RequiresActiveDocument,
            ["source"] = seed.Source,
            ["parameters"] = JsonNode.Parse(seed.Schema),
        });
    }

    return array.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
}

static string EmitMarkdown(List<ToolSeed> seeds, string namespaceName)
{
    var builder = new StringBuilder();
    builder.AppendLine("# RevitToolSet 工具索引");
    builder.AppendLine();
    builder.AppendLine($"生成自厂商 TOOL-API.json；工具类命名空间 `{namespaceName}.Tools`。");
    builder.AppendLine();
    builder.AppendLine("| # | 工具名 | 工具类 | 分类 | 事务 | 修改 | 来源 |");
    builder.AppendLine("|---|---|---|---|---|---|---|");

    var index = 1;
    foreach (var seed in seeds)
    {
        builder.AppendLine($"| {index++} | `{seed.Name}` | `{seed.ClassName}` | {seed.Category} | {Bool(seed.RequiresTransaction)} | {Bool(seed.RequiresModification)} | {seed.Source} |");
    }

    return builder.ToString();
}

static string Bool(bool value) => value ? "true" : "false";

static string Literal(string value)
    => "\"" + value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("\"", "\\\"", StringComparison.Ordinal)
        .Replace("\r", "\\r", StringComparison.Ordinal)
        .Replace("\n", "\\n", StringComparison.Ordinal)
        .Replace("\t", "\\t", StringComparison.Ordinal)
       + "\"";

internal sealed record ToolSeed(
    string Name,
    string Category,
    string Description,
    bool RequiresTransaction,
    bool RequiresModification,
    bool RequiresActiveDocument,
    string Schema,
    string ClassName,
    string Source);
