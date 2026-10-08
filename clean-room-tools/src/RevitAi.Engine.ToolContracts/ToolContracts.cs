namespace RevitAi.Engine.ToolContracts;

/// <summary>一个工具的对外契约（名称/分类/描述/三个布尔/JSON Schema）。</summary>
public sealed record ToolContract(
    string Name,
    string Category,
    string Description,
    bool RequiresTransaction,
    bool RequiresModification,
    bool RequiresActiveDocument,
    string ParametersSchema);

/// <summary>
/// 工具契约常量。单独放在一个不引用 Revit 的程序集里，好处有两个：
/// 1) 特性参数必须是编译期常量，工具类可以直接用这些 const；
/// 2) 无 Revit 的冒烟测试可以直接枚举契约，与厂商 TOOL-API.json 做逐字段对拍。
///
/// 三个厂商同名工具的契约（名称/分类/描述/schema/两个布尔）与 TOOL-API.json 完全一致；
/// upsert_level 是自研扩展（厂商 120 项里没有），schema 取自本机自研外壳的 LegacySchemaCatalog。
/// </summary>
public static class ToolContracts
{
    // ---------- get_all_levels（厂商同名，只读） ----------
    public const string GetAllLevels_Name = "get_all_levels";
    public const string GetAllLevels_Category = "标高查询";
    public const string GetAllLevels_Description = "获取文档中所有标高的列表，包括标高 ID、名称和高度值。返回单位：米(m)";
    public const string GetAllLevels_Schema = """{"type":"object","properties":{},"required":[]}""";

    // ---------- create_level（厂商同名，写 + 回主线程） ----------
    public const string CreateLevel_Name = "create_level";
    public const string CreateLevel_Category = "元素创建";
    public const string CreateLevel_Description = "创建新的标高";
    public const string CreateLevel_Schema = """
    {
      "type": "object",
      "properties": {
        "elevation": {
          "anyOf": [
            { "type": "number" },
            { "type": "array", "items": { "type": "number" } }
          ],
          "description": "标高高度（米）或标高高度数组。可以是单个值（如3.5表示3.5米）或数组（如[3.0, 4.5, 6.0]表示3米、4.5米、6米）。如果是数组，levelName也必须是数组且长度一致。",
          "examples": [3.5, [3.0, 4.5, 6.0]]
        },
        "levelName": {
          "anyOf": [
            { "type": "string" },
            { "type": "array", "items": { "type": "string" } }
          ],
          "description": "标高名称（可选）或标高名称数组。单个名称时用于单个标高，数组时对应每个标高（长度必须与elevation数组一致）。例如：标高 1、F1、Level 1，或数组形式：[标高 1, 标高 2, 标高 3]"
        }
      },
      "required": ["elevation"]
    }
    """;

    // ---------- move_elements（厂商同名，写 + 回主线程） ----------
    public const string MoveElements_Name = "move_elements";
    public const string MoveElements_Category = "元素修改";
    public const string MoveElements_Description = "移动单个或多个元素。参数单位：毫米。工具会自动将毫米转换为英尺后执行移动操作";
    public const string MoveElements_Schema = """
    {
      "type": "object",
      "properties": {
        "elementId": { "type": "integer", "description": "单个元素 ID（可选）。移动单个元素，例如：12345" },
        "elementIds": { "type": "array", "items": { "type": "integer" }, "description": "元素 ID 数组（可选）。批量移动多个元素，例如：[12345, 12346, 12347]" },
        "cacheId": { "type": "string", "description": "缓存 ID（可选）。从上一个查询工具（如 element_query）的返回结果中获取 cache_id 字段。使用缓存可以批量操作之前查询到的所有元素。注意：只需提供单个 cache_id 字符串，不需要数组。" },
        "x": { "type": "number", "description": "X 方向移动距离（毫米，正值为向右，负值为向左）" },
        "y": { "type": "number", "description": "Y 方向移动距离（毫米，正值为向上，负值为向下）" },
        "z": { "type": "number", "description": "Z 方向移动距离（毫米，正值为向上，负值为向下，默认 0）" }
      },
      "required": ["x", "y"]
    }
    """;

    // ---------- upsert_level（自研扩展：按名称/容差创建或更新标高） ----------
    public const string UpsertLevel_Name = "upsert_level";
    public const string UpsertLevel_Category = "标高管理";
    public const string UpsertLevel_Description = "按名称或容差创建或更新标高；支持 dry_run 只预演不落库。";
    public const string UpsertLevel_Schema = """
    {
      "type": "object",
      "properties": {
        "name": { "type": "string", "description": "标高名称。" },
        "elevation": { "type": "number", "description": "标高，单位米。" },
        "updateExisting": { "type": "boolean", "default": true },
        "buildingStory": { "type": "boolean", "default": true },
        "toleranceMm": { "type": "number", "default": 5 },
        "dry_run": { "type": "boolean", "description": "仅预演，不修改模型。" }
      },
      "required": ["name", "elevation"],
      "additionalProperties": false
    }
    """;

    public static readonly ToolContract GetAllLevels = new(
        GetAllLevels_Name, GetAllLevels_Category, GetAllLevels_Description,
        RequiresTransaction: false, RequiresModification: false, RequiresActiveDocument: true,
        GetAllLevels_Schema);

    public static readonly ToolContract CreateLevel = new(
        CreateLevel_Name, CreateLevel_Category, CreateLevel_Description,
        RequiresTransaction: true, RequiresModification: true, RequiresActiveDocument: true,
        CreateLevel_Schema);

    public static readonly ToolContract MoveElements = new(
        MoveElements_Name, MoveElements_Category, MoveElements_Description,
        RequiresTransaction: true, RequiresModification: true, RequiresActiveDocument: true,
        MoveElements_Schema);

    public static readonly ToolContract UpsertLevel = new(
        UpsertLevel_Name, UpsertLevel_Category, UpsertLevel_Description,
        RequiresTransaction: true, RequiresModification: true, RequiresActiveDocument: true,
        UpsertLevel_Schema);

    public static IReadOnlyList<ToolContract> All { get; } = new[] { GetAllLevels, CreateLevel, MoveElements, UpsertLevel };
}
