using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 4 第一刀实现：轴网 / 房间 / 墙 / 柱 / 梁 / 族实例 / 门窗。
/// 每个工具都支持"单条参数"与"数组批量"两种入参形式（厂商 schema 两者都有）。
/// </summary>
public static class ModelingImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("create_grid", new CreateGridImplementation());
        ToolImplementationRegistry.Register("create_room", new CreateRoomImplementation());
        ToolImplementationRegistry.Register("create_straight_wall", new CreateStraightWallImplementation());
        ToolImplementationRegistry.Register("create_column", new CreateColumnImplementation());
        ToolImplementationRegistry.Register("create_beam", new CreateBeamImplementation());
        ToolImplementationRegistry.Register("create_family_instance", new CreateFamilyInstanceImplementation());
        ToolImplementationRegistry.Register("create_door_in_wall", new CreateDoorInWallImplementation());
        ToolImplementationRegistry.Register("create_window_in_wall", new CreateWindowInWallImplementation());
    }
}

internal static class ModelingArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    public static List<Dictionary<string, object?>> ReadDicts(AIToolContext context, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (context.Parameters.TryGetValue(key, out var value) && value is List<object?> list)
            {
                return list.OfType<Dictionary<string, object?>>().ToList();
            }
        }

        return new List<Dictionary<string, object?>>();
    }

    public static double Number(Dictionary<string, object?> item, string key, double fallback = 0)
        => item.TryGetValue(key, out var value) && value is not null ? ToolArgumentReader.ToDouble(value) : fallback;

    public static int? OptionalInt(Dictionary<string, object?> item, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (item.TryGetValue(key, out var value) && value is not null)
            {
                return (int)ToolArgumentReader.ToDouble(value);
            }
        }

        return null;
    }

    public static string? Text(Dictionary<string, object?> item, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (item.TryGetValue(key, out var value) && value is not null && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                return value.ToString();
            }
        }

        return null;
    }

    public static bool Flag(Dictionary<string, object?> item, string key, bool fallback = false)
        => item.TryGetValue(key, out var value) && value is not null
            ? value is bool flag ? flag : string.Equals(value.ToString(), "true", StringComparison.OrdinalIgnoreCase)
            : fallback;

    /// <summary>把"单条 + 数组"统一成批量规格列表。</summary>
    public static List<Dictionary<string, object?>> Combine(AIToolContext context, params string[] arrayKeys)
    {
        var items = ReadDicts(context, arrayKeys);
        if (items.Count > 0)
        {
            return items;
        }

        // 单条形式：把顶层参数当成一个待处理项。
        var single = new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase);
        single.Remove("cacheId");
        return new List<Dictionary<string, object?>> { single };
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateGridTool.cs（元素创建）
internal sealed class CreateGridImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ModelingService ?? throw new InvalidOperationException("无法获取 ModelingService");
        var document = ModelingArgs.RequireDocument(adapter, context);

        var items = ModelingArgs.ReadDicts(context, "grids");
        if (items.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 grids（数组，元素含 start_x/start_y/end_x/end_y，可带 name）。"));
        }

        var specs = items.Select(item => new GridSpec(
            ModelingArgs.Text(item, "name"),
            ModelingArgs.Number(item, "start_x", ModelingArgs.Number(item, "startX")),
            ModelingArgs.Number(item, "start_y", ModelingArgs.Number(item, "startY")),
            ModelingArgs.Number(item, "end_x", ModelingArgs.Number(item, "endX")),
            ModelingArgs.Number(item, "end_y", ModelingArgs.Number(item, "endY")))).ToList();

        var result = service.CreateGrids(document, specs);
        return Task.FromResult(Result("轴网", result));
    }

    internal static AIToolResult Result(string what, CreateManyResult result)
        => result.Created == result.Requested
            ? AIToolResult.Ok($"✅ 已创建 {result.Created} 个{what}", new { requested = result.Requested, created = result.Created, createdIds = result.CreatedIds })
            : AIToolResult.Fail(
                $"⚠️ 已创建 {result.Created}/{result.Requested} 个{what}；失败原因：{string.Join("；", result.Failures)}",
                new { requested = result.Requested, created = result.Created, createdIds = result.CreatedIds, failures = result.Failures });
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateRoomTool.cs（元素创建）
internal sealed class CreateRoomImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ModelingService ?? throw new InvalidOperationException("无法获取 ModelingService");
        var document = ModelingArgs.RequireDocument(adapter, context);

        var levelId = WriteArgs.OptionalInt(context, "levelId", "level_id");
        if (levelId is null)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 levelId。"));
        }

        var autoMode = WriteArgs.Flag(context, false, "autoMode", "auto_mode");
        var rooms = ModelingArgs.ReadDicts(context, "rooms");
        var createdIds = new List<int>();
        var failures = new List<string>();

        if (rooms.Count > 0)
        {
            foreach (var room in rooms)
            {
                try
                {
                    createdIds.Add(service.CreateRoom(
                        document,
                        ModelingArgs.OptionalInt(room, "levelId", "level_id") ?? levelId.Value,
                        ModelingArgs.Number(room, "x"),
                        ModelingArgs.Number(room, "y"),
                        ModelingArgs.Text(room, "roomName", "name"),
                        ModelingArgs.Text(room, "roomNumber", "number")));
                }
                catch (Exception ex)
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
        }
        else
        {
            var x = WriteArgs.Number(context, 0, "x");
            var y = WriteArgs.Number(context, 0, "y");
            var roomId = service.CreateRoom(document, levelId.Value, x, y, WriteArgs.Text(context, "roomName", "name"), WriteArgs.Text(context, "roomNumber", "number"));
            createdIds.Add(roomId);
        }

        return Task.FromResult(failures.Count == 0
            ? AIToolResult.Ok(
                $"✅ 已创建 {createdIds.Count} 个房间" + (autoMode ? "（autoMode 生效：仅创建未放置边界，房间面积需后续\"房间边界\"自动检测）" : string.Empty),
                new { created = createdIds.Count, createdIds, roomNumber = WriteArgs.Text(context, "roomNumber"), roomName = WriteArgs.Text(context, "roomName"), autoMode })
            : AIToolResult.Fail(
                $"⚠️ 已创建 {createdIds.Count}/{createdIds.Count + failures.Count} 个房间；失败原因：{string.Join("；", failures)}",
                new { created = createdIds.Count, createdIds, failures }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateStraightWallTool.cs（元素创建）
internal sealed class CreateStraightWallImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ModelingService ?? throw new InvalidOperationException("无法获取 ModelingService");
        var document = ModelingArgs.RequireDocument(adapter, context);

        var items = ModelingArgs.Combine(context, "walls");
        var specs = items.Select(item => new WallSpec(
            ModelingArgs.Number(item, "start_x", ModelingArgs.Number(item, "startX")),
            ModelingArgs.Number(item, "start_y", ModelingArgs.Number(item, "startY")),
            ModelingArgs.Number(item, "end_x", ModelingArgs.Number(item, "endX")),
            ModelingArgs.Number(item, "end_y", ModelingArgs.Number(item, "endY")),
            ModelingArgs.OptionalInt(item, "level_id", "levelId"),
            ModelingArgs.Number(item, "height", 3000),
            ModelingArgs.OptionalInt(item, "wall_type_id", "wallTypeId"))).ToList();

        var result = service.CreateWalls(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("墙", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateColumnTool.cs（元素创建）
internal sealed class CreateColumnImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ModelingService ?? throw new InvalidOperationException("无法获取 ModelingService");
        var document = ModelingArgs.RequireDocument(adapter, context);

        var items = ModelingArgs.Combine(context, "columns");
        var specs = items.Select(item => new ColumnSpec(
            ModelingArgs.Number(item, "position_x", ModelingArgs.Number(item, "x")),
            ModelingArgs.Number(item, "position_y", ModelingArgs.Number(item, "y")),
            ModelingArgs.Number(item, "position_z", ModelingArgs.Number(item, "z")),
            ModelingArgs.OptionalInt(item, "level_id", "levelId"),
            ModelingArgs.OptionalInt(item, "column_type_id", "columnTypeId"))).ToList();

        var result = service.CreateColumns(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("结构柱", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateBeamTool.cs（元素创建）
internal sealed class CreateBeamImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ModelingService ?? throw new InvalidOperationException("无法获取 ModelingService");
        var document = ModelingArgs.RequireDocument(adapter, context);

        var items = ModelingArgs.Combine(context, "beams");
        var specs = items.Select(item => new BeamSpec(
            ModelingArgs.Number(item, "start_x", ModelingArgs.Number(item, "startX")),
            ModelingArgs.Number(item, "start_y", ModelingArgs.Number(item, "startY")),
            ModelingArgs.Number(item, "end_x", ModelingArgs.Number(item, "endX")),
            ModelingArgs.Number(item, "end_y", ModelingArgs.Number(item, "endY")),
            ModelingArgs.OptionalInt(item, "level_id", "levelId"),
            ModelingArgs.OptionalInt(item, "beam_type_id", "beamTypeId"))).ToList();

        var result = service.CreateBeams(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("梁", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateFamilyInstanceTool.cs（元素创建）
internal sealed class CreateFamilyInstanceImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ModelingService ?? throw new InvalidOperationException("无法获取 ModelingService");
        var document = ModelingArgs.RequireDocument(adapter, context);

        var items = ModelingArgs.Combine(context, "instances");
        var specs = new List<InstanceSpec>();
        foreach (var item in items)
        {
            var typeId = ModelingArgs.OptionalInt(item, "type_id", "typeId");
            if (typeId is null)
            {
                return Task.FromResult(AIToolResult.Fail("必须提供 type_id（族类型 id）。"));
            }

            specs.Add(new InstanceSpec(
                typeId.Value,
                ModelingArgs.Number(item, "x"),
                ModelingArgs.Number(item, "y"),
                ModelingArgs.Number(item, "z"),
                ModelingArgs.OptionalInt(item, "level_id", "levelId")));
        }

        var result = service.CreateFamilyInstances(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("族实例", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateDoorInWallTool.cs（元素创建）
internal sealed class CreateDoorInWallImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ModelingService ?? throw new InvalidOperationException("无法获取 ModelingService");
        var document = ModelingArgs.RequireDocument(adapter, context);

        var items = ModelingArgs.Combine(context, "doors");
        var specs = new List<HostedSpec>();

        foreach (var item in items)
        {
            var wallId = ModelingArgs.OptionalInt(item, "wall_id", "wallId");
            var typeId = ModelingArgs.OptionalInt(item, "door_type_id", "doorTypeId", "type_id");
            if (wallId is null || typeId is null)
            {
                return Task.FromResult(AIToolResult.Fail("必须提供 wall_id 与 door_type_id。"));
            }

            specs.Add(new HostedSpec(
                wallId.Value,
                typeId.Value,
                ModelingArgs.Number(item, "position_x", ModelingArgs.Number(item, "x")),
                ModelingArgs.Number(item, "position_y", ModelingArgs.Number(item, "y")),
                0,
                ModelingArgs.OptionalInt(item, "level_id", "levelId"),
                ModelingArgs.Flag(item, "flip")));
        }

        var result = service.CreateHostedInstances(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("门", result));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\CreateWindowInWallTool.cs（元素创建）
internal sealed class CreateWindowInWallImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ModelingService ?? throw new InvalidOperationException("无法获取 ModelingService");
        var document = ModelingArgs.RequireDocument(adapter, context);

        var items = ModelingArgs.Combine(context, "windows");
        var specs = new List<HostedSpec>();

        foreach (var item in items)
        {
            var wallId = ModelingArgs.OptionalInt(item, "wall_id", "wallId");
            var typeId = ModelingArgs.OptionalInt(item, "window_type_id", "windowTypeId", "type_id");
            if (wallId is null || typeId is null)
            {
                return Task.FromResult(AIToolResult.Fail("必须提供 wall_id 与 window_type_id。"));
            }

            specs.Add(new HostedSpec(
                wallId.Value,
                typeId.Value,
                ModelingArgs.Number(item, "position_x", ModelingArgs.Number(item, "x")),
                ModelingArgs.Number(item, "position_y", ModelingArgs.Number(item, "y")),
                ModelingArgs.Number(item, "height_offset", 1000),
                ModelingArgs.OptionalInt(item, "level_id", "levelId"),
                ModelingArgs.Flag(item, "flip")));
        }

        var result = service.CreateHostedInstances(document, specs);
        return Task.FromResult(CreateGridImplementation.Result("窗", result));
    }
}
