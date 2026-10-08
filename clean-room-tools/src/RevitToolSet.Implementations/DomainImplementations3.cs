using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 收尾批实现（Revit 侧 4 个）：楼梯、楼梯类型、屋顶坡度、卫星图导入。
/// </summary>
public static class DomainImplementations3
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("create_stair", new CreateStairImplementation());
        ToolImplementationRegistry.Register("create_stair_type", new CreateStairTypeImplementation());
        ToolImplementationRegistry.Register("modify_roof", new ModifyRoofImplementation());
        ToolImplementationRegistry.Register("import_satellite_map", new ImportSatelliteMapImplementation());
    }
}

internal static class StairArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateStairTool.cs（元素创建）
internal sealed class CreateStairImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.StairService ?? throw new InvalidOperationException("无法获取 StairService");
        var document = StairArgs.RequireDocument(adapter, context);

        var height = WriteArgs.Number(context, 0, "totalHeightMm", "total_height_mm");
        var runCount = (int)WriteArgs.Number(context, 0, "runCount", "run_count");
        if (height <= 0 || runCount <= 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 totalHeightMm（>0）与 runCount（>0）。"));
        }

        var result = service.CreateStair(
            document,
            height,
            runCount,
            NumberOrNull(context, "treadDepthMm", "tread_depth_mm"),
            NumberOrNull(context, "runWidthMm", "run_width_mm"),
            NumberOrNull(context, "wellWidthMm", "well_width_mm"),
            NumberOrNull(context, "landingWidthMm", "landing_width_mm"),
            WriteArgs.Number(context, 0, "insertPointX", "insert_point_x"),
            WriteArgs.Number(context, 0, "insertPointY", "insert_point_y"));

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建楼梯 {result.StairId}（{runCount} 跑，总高 {height} mm，梯段 {result.RunId}）",
            new
            {
                stairId = result.StairId,
                runId = result.RunId,
                runCount = result.RunCount,
                totalHeightMM = result.TotalHeightMm,
                apiPath = result.ApiPath,
                direction = WriteArgs.Text(context, "direction"),
                startDirection = WriteArgs.Text(context, "startDirection", "start_direction"),
                baseElevationM = context.Parameters.ContainsKey("baseElevationM") ? WriteArgs.Number(context, 0, "baseElevationM") : (double?)null,
                note = "wellWidthMm / landingWidthMm / 方向参数已记录；本实现创建直跑梯段（StairsRun.CreateStraightRun）。",
            }));
    }

    private static double? NumberOrNull(AIToolContext context, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (context.Parameters.ContainsKey(key))
            {
                return WriteArgs.Number(context, 0, key);
            }
        }

        return null;
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateStairTypeTool.cs（族管理）
internal sealed class CreateStairTypeImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.StairService ?? throw new InvalidOperationException("无法获取 StairService");
        var document = StairArgs.RequireDocument(adapter, context);

        var newName = WriteArgs.Text(context, "newStairTypeName", "new_stair_type_name");
        if (string.IsNullOrWhiteSpace(newName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 newStairTypeName。"));
        }

        var result = service.CreateStairType(
            document,
            newName!,
            WriteArgs.Text(context, "baseStairTypeName", "base_stair_type_name"),
            context.Parameters.ContainsKey("runThicknessMm") ? WriteArgs.Number(context, 0, "runThicknessMm") : (double?)null,
            context.Parameters.ContainsKey("landingThicknessMm") ? WriteArgs.Number(context, 0, "landingThicknessMm") : (double?)null);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 已创建楼梯类型 {result.TypeId}（{result.TypeName}）",
            new
            {
                typeId = result.TypeId,
                typeName = result.TypeName,
                runThicknessMM = result.RunThicknessMm,
                landingThicknessMM = result.LandingThicknessMm,
                notes = result.Notes,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ModifyRoofTool.cs（屋顶）
internal sealed class ModifyRoofImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.RoofEditService ?? throw new InvalidOperationException("无法获取 RoofEditService");
        var document = StairArgs.RequireDocument(adapter, context);

        var roofId = WriteArgs.OptionalInt(context, "roof_id", "roofId");
        if (roofId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 roof_id。"));
        }

        var definesSlope = WriteArgs.Flag(context, true, "defines_slope", "definesSlope");
        var applyToAll = WriteArgs.Flag(context, false, "apply_to_all_edges", "applyToAllEdges");
        var edgeIndex = WriteArgs.OptionalInt(context, "edge_index", "edgeIndex");
        var angle = context.Parameters.ContainsKey("slope_angle")
            ? (double?)WriteArgs.Number(context, 0, "slope_angle")
            : null;

        var result = service.ModifyRoofSlope(document, roofId.Value, edgeIndex, definesSlope, angle, applyToAll);

        return Task.FromResult(result.ChangedEdges > 0
            ? AIToolResult.Ok(
                $"✅ 屋顶 {result.RoofId} 已更新 {result.ChangedEdges} 条边的坡度定义（应用全部边={result.ApplyToAllEdges}）",
                new
                {
                    roofId = result.RoofId,
                    changedEdges = result.ChangedEdges,
                    definesSlope,
                    slopeAngle = result.SlopeAngle,
                    applyToAllEdges = result.ApplyToAllEdges,
                    notes = result.Notes,
                })
            : AIToolResult.Fail(
                $"没有边被更新：{string.Join("；", result.Notes)}",
                new { roofId = result.RoofId, notes = result.Notes }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ImportSatelliteMapTool.cs（卫星图）
internal sealed class ImportSatelliteMapImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.SatelliteImportService ?? throw new InvalidOperationException("无法获取 SatelliteImportService");
        var document = StairArgs.RequireDocument(adapter, context);

        var imagePath = WriteArgs.Text(context, "image_path", "imagePath", "file_path", "filePath");
        var result = service.Import(
            document,
            imagePath,
            WriteArgs.Text(context, "location_name", "locationName"),
            context.Parameters.ContainsKey("latitude") ? WriteArgs.Number(context, 0, "latitude") : (double?)null,
            context.Parameters.ContainsKey("longitude") ? WriteArgs.Number(context, 0, "longitude") : (double?)null,
            WriteArgs.Text(context, "map_source", "mapSource"),
            WriteArgs.OptionalInt(context, "zoom_level", "zoomLevel"),
            context.Parameters.ContainsKey("radius_km") ? WriteArgs.Number(context, 0, "radius_km") : (double?)null);

        var payload = new
        {
            status = result.Status,
            elementId = result.ElementId,
            geoReference = result.Notes,
            risk = "本工具只做本地栅格导入；不会发起任何网络请求。",
        };

        return Task.FromResult(result.ElementId is not null
            ? AIToolResult.Ok($"✅ {result.Message}", payload)
            : AIToolResult.Fail(result.Message, payload));
    }
}

/// <summary>
/// 收尾批实现（Revit-free 2 个）：代码片段库 + 受限计划解析。
/// 两个工具都**不执行任何代码**：code_snippet 是文本库，execute_code 只做静态解析与风险拦截。
/// </summary>
public static class CodeToolImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("code_snippet", new CodeSnippetImplementation());
        ToolImplementationRegistry.Register("execute_code", new ExecuteCodeImplementation());
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CodeSnippetTool.cs（代码片段）
internal sealed class CodeSnippetImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var library = new SnippetLibrary();
        var action = (WriteArgs.Text(context, "action") ?? "list").Trim().ToLowerInvariant();
        var id = WriteArgs.Text(context, "snippetId", "snippet_id", "id");
        var name = WriteArgs.Text(context, "name");
        var description = WriteArgs.Text(context, "description") ?? string.Empty;
        var code = WriteArgs.Text(context, "code");
        var keyword = WriteArgs.Text(context, "keyword");
        var tags = ToolArgumentReader.ReadStrings(
            new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase), "tags");

        switch (action)
        {
            case "list":
            case "search":
            {
                var items = library.List(keyword, tags.FirstOrDefault());
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 代码片段共 {items.Count} 条",
                    new
                    {
                        count = items.Count,
                        storePath = library.Path,
                        snippets = items.Select(item => new
                        {
                            snippetId = item.Id,
                            name = item.Name,
                            description = item.Description,
                            tags = item.Tags,
                            codeLength = item.Code.Length,
                            updatedAt = item.UpdatedAt,
                        }).ToArray(),
                    }));
            }

            case "get":
            {
                var snippet = library.Get(id, name);
                return Task.FromResult(snippet is null
                    ? AIToolResult.Fail("未找到该代码片段（请提供 snippetId 或 name）。")
                    : AIToolResult.Ok(
                        $"✅ 代码片段 {snippet.Name}",
                        new { snippetId = snippet.Id, name = snippet.Name, description = snippet.Description, tags = snippet.Tags, code = snippet.Code }));
            }

            case "save":
            case "add":
            case "update":
            {
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code))
                {
                    return Task.FromResult(AIToolResult.Fail("save 需要 name 与 code。"));
                }

                var snippet = library.Save(id, name!, description, code!, tags);
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 代码片段 {snippet.Name} 已保存（id={snippet.Id}）",
                    new { snippetId = snippet.Id, name = snippet.Name, tags = snippet.Tags, storePath = library.Path }));
            }

            case "delete":
            {
                var deleted = library.Delete(id, name);
                return Task.FromResult(deleted
                    ? AIToolResult.Ok("✅ 已删除代码片段", new { deleted = true })
                    : AIToolResult.Fail("未找到要删除的代码片段。"));
            }

            default:
                return Task.FromResult(AIToolResult.Fail("action 只支持 list / get / save / delete（search 与 list 同义）。"));
        }
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ExecuteCodeTool.cs（代码执行）
internal sealed class ExecuteCodeImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var code = WriteArgs.Text(context, "code");
        var timeout = WriteArgs.OptionalInt(context, "timeout");

        var plan = RestrictedCodePlan.Parse(code);
        var payload = new
        {
            accepted = plan.Accepted,
            stepCount = plan.StepCount,
            steps = plan.Steps.Select(step => new { tool = step.Tool, parameters = step.Parameters }).ToArray(),
            violations = plan.Violations,
            timeoutSeconds = timeout,
            policy = "本引擎不执行任意代码（C#/Python/表达式/反射/文件系统一律拒绝）。"
                     + "execute_code 只把传入文本解析成受限的工具调用计划；实际执行请由宿主逐步调用对应工具。",
        };

        return Task.FromResult(plan.Accepted
            ? AIToolResult.Ok($"✅ {plan.Message}", payload)
            : AIToolResult.Fail(plan.Message, payload));
    }
}
