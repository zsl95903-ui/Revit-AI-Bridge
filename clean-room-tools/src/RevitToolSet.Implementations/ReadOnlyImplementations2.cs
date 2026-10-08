using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 1 第二批：族/族类型、房间、标记、元素位置与几何、元素列表、项目单位、
/// 视图设置与过滤器、类别可见性、类型列表（管/风管/桥架/屋顶）。
/// 与第一批相同：只依赖抽象服务与 DTO，可在无 Revit 的冒烟测试里执行。
/// </summary>
public static class ReadOnlyImplementations2
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("get_all_families", new GetAllFamiliesImplementation());
        ToolImplementationRegistry.Register("get_family_types", new GetFamilyTypesImplementation());
        ToolImplementationRegistry.Register("get_all_rooms", new GetAllRoomsImplementation());
        ToolImplementationRegistry.Register("get_all_tags", new GetAllTagsImplementation());
        ToolImplementationRegistry.Register("get_element_location", new GetElementLocationImplementation());
        ToolImplementationRegistry.Register("get_element_geometry", new GetElementGeometryImplementation());
        ToolImplementationRegistry.Register("get_all_elements", new GetAllElementsImplementation());
        ToolImplementationRegistry.Register("get_project_units", new GetProjectUnitsImplementation());
        ToolImplementationRegistry.Register("get_view_settings", new GetViewSettingsImplementation());
        ToolImplementationRegistry.Register("get_view_filters", new GetViewFiltersImplementation());
        ToolImplementationRegistry.Register("get_category_visibility", new GetCategoryVisibilityImplementation());
        ToolImplementationRegistry.Register("get_pipe_types", new TypeListImplementation("get_pipe_types", "pipe", "管道"));
        ToolImplementationRegistry.Register("get_duct_types", new TypeListImplementation("get_duct_types", "duct", "风管"));
        ToolImplementationRegistry.Register("get_cable_tray_types", new TypeListImplementation("get_cable_tray_types", "cableTray", "桥架"));
        ToolImplementationRegistry.Register("get_roof_types", new TypeListImplementation("get_roof_types", "roof", "屋顶"));
    }
}

internal static class ReadOnlySupport2
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    public static int? OptionalInt(AIToolContext context, string key)
    {
        if (!context.Parameters.ContainsKey(key) || context.Parameters[key] is null)
        {
            return null;
        }

        var value = ToolArgumentReader.ToDouble(context.Parameters[key]);
        return value > 0 ? (int)value : null;
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAllFamiliesTool.cs（族管理）
internal sealed class GetAllFamiliesImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.FamilyQueryService ?? throw new InvalidOperationException("无法获取 FamilyQueryService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);
        var families = service.GetAllFamilies(document);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {families.Count} 个族",
            new
            {
                families = families.Select(family => new
                {
                    id = family.FamilyId,
                    name = family.FamilyName,
                    category = family.Category,
                    typeCount = family.TypeCount,
                }).ToArray(),
                count = families.Count,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetFamilyTypesTool.cs
internal sealed class GetFamilyTypesImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.FamilyQueryService ?? throw new InvalidOperationException("无法获取 FamilyQueryService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);

        var familyName = context.GetParameter<string?>("familyName", null);
        var limit = context.GetParameter("limit", 200);
        var types = service.GetFamilyTypes(document, familyName, limit);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {types.Count} 个族类型",
            new
            {
                familyName,
                types = types.Select(type => new
                {
                    typeId = type.TypeId,
                    familyName = type.FamilyName,
                    typeName = type.TypeName,
                    category = type.Category,
                }).ToArray(),
                count = types.Count,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAllRoomsTool.cs（房间与分区）
internal sealed class GetAllRoomsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.RoomService ?? throw new InvalidOperationException("无法获取 RoomService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);
        var rooms = service.GetAllRooms(document);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {rooms.Count} 个房间",
            new
            {
                rooms = rooms.Select(room => new
                {
                    id = room.RoomId,
                    number = room.Number,
                    name = room.Name,
                    area = room.AreaSquareMeters,
                    unit = "square_meters",
                    levelId = room.LevelId,
                    levelName = room.LevelName,
                }).ToArray(),
                count = rooms.Count,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAllTagsTool.cs（注释与标记）
internal sealed class GetAllTagsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.TagService ?? throw new InvalidOperationException("无法获取 TagService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);
        var tags = service.GetAllTags(document);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {tags.Count} 个标记",
            new
            {
                tags = tags.Select(tag => new
                {
                    id = tag.TagId,
                    text = tag.Text,
                    taggedElementId = tag.TaggedElementId,
                    category = tag.Category,
                }).ToArray(),
                count = tags.Count,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetElementLocationTool.cs（元素查询）
internal sealed class GetElementLocationImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementQueryService ?? throw new InvalidOperationException("无法获取 ElementQueryService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);
        var elementId = context.GetParameter("elementId", 0);
        if (elementId <= 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId。"));
        }

        var geometry = service.GetElementGeometry(document, elementId);
        if (geometry is null)
        {
            return Task.FromResult(AIToolResult.Fail($"未找到元素 {elementId}。"));
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 元素 {elementId} 位置：({geometry.LocationX:F1}, {geometry.LocationY:F1}, {geometry.LocationZ:F1}) mm",
            new
            {
                elementId,
                locationKind = geometry.LocationKind,
                x = geometry.LocationX,
                y = geometry.LocationY,
                z = geometry.LocationZ,
                unit = geometry.Unit,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetElementGeometryTool.cs（混淆密度最高的工具之一）
internal sealed class GetElementGeometryImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementQueryService ?? throw new InvalidOperationException("无法获取 ElementQueryService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);
        var elementId = context.GetParameter("elementId", 0);
        if (elementId <= 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId。"));
        }

        var geometry = service.GetElementGeometry(document, elementId);
        if (geometry is null)
        {
            return Task.FromResult(AIToolResult.Fail($"未找到元素 {elementId}。"));
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 元素 {elementId}：{geometry.Category} / {geometry.TypeName}，实体 {geometry.SolidCount} 个，曲线 {geometry.CurveCount} 条",
            new
            {
                elementId,
                category = geometry.Category,
                typeName = geometry.TypeName,
                locationKind = geometry.LocationKind,
                location = new { x = geometry.LocationX, y = geometry.LocationY, z = geometry.LocationZ, unit = geometry.Unit },
                boundingBox = geometry.HasBoundingBox
                    ? new
                    {
                        min = new { x = geometry.MinX, y = geometry.MinY, z = geometry.MinZ },
                        max = new { x = geometry.MaxX, y = geometry.MaxY, z = geometry.MaxZ },
                        unit = geometry.Unit,
                    }
                    : null,
                solidCount = geometry.SolidCount,
                curveCount = geometry.CurveCount,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetAllElementsTool.cs
internal sealed class GetAllElementsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ElementQueryService ?? throw new InvalidOperationException("无法获取 ElementQueryService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);

        var category = context.GetParameter<string?>("category", null)
                       ?? context.GetParameter<string?>("categoryName", null);
        var limit = context.GetParameter("limit", 100);
        var elements = service.GetAllElements(document, category, limit);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {elements.Count} 个元素" + (string.IsNullOrWhiteSpace(category) ? string.Empty : $"（类别：{category}）"),
            new
            {
                category,
                elements = elements.Select(element => new
                {
                    id = element.Id,
                    name = element.Name,
                    category = element.Category,
                    typeName = element.TypeName,
                }).ToArray(),
                count = elements.Count,
                limit,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\AI\Tools\GetProjectUnitsTool.cs（项目设置）
internal sealed class GetProjectUnitsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ProjectUnitService ?? throw new InvalidOperationException("无法获取 ProjectUnitService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);
        var units = service.GetProjectUnits(document);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 项目单位：长度 {units.LengthUnit}（{units.LengthSymbol}），内部单位 {units.InternalLengthUnit}",
            new
            {
                length = units.LengthUnit,
                lengthSymbol = units.LengthSymbol,
                area = units.AreaUnit,
                volume = units.VolumeUnit,
                angle = units.AngleUnit,
                internalLengthUnit = units.InternalLengthUnit,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetViewSettingsTool.cs（视图查询）
internal sealed class GetViewSettingsImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewQueryService ?? throw new InvalidOperationException("无法获取 ViewQueryService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);
        var settings = service.GetViewSettings(document, ReadOnlySupport2.OptionalInt(context, "viewId"));

        if (settings is null)
        {
            return Task.FromResult(AIToolResult.Fail("未找到视图（请检查 viewId，或确保存在活动视图）。"));
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 视图 {settings.Name}：比例 1:{settings.Scale}，详细程度 {settings.DetailLevel}",
            new
            {
                viewId = settings.ViewId,
                name = settings.Name,
                viewType = settings.ViewType,
                scale = settings.Scale,
                detailLevel = settings.DetailLevel,
                displayStyle = settings.DisplayStyle,
                cropActive = settings.CropActive,
                cropBoxVisible = settings.CropBoxVisible,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetViewFiltersTool.cs（视图高级操作）
internal sealed class GetViewFiltersImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewQueryService ?? throw new InvalidOperationException("无法获取 ViewQueryService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);
        var filters = service.GetViewFilters(document, ReadOnlySupport2.OptionalInt(context, "viewId"));

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 当前视图共有 {filters.Count} 个过滤器",
            new
            {
                filters = filters.Select(filter => new
                {
                    filterId = filter.FilterId,
                    name = filter.Name,
                    enabled = filter.Enabled,
                    visible = filter.Visible,
                }).ToArray(),
                count = filters.Count,
            }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetCategoryVisibilityTool.cs
internal sealed class GetCategoryVisibilityImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ViewQueryService ?? throw new InvalidOperationException("无法获取 ViewQueryService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);

        var categoryName = context.GetParameter<string?>("categoryName", null)
                           ?? context.GetParameter<string?>("category_name", null);
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 categoryName（类别名称，如：墙、门、窗）。"));
        }

        var visibility = service.GetCategoryVisibility(document, ReadOnlySupport2.OptionalInt(context, "viewId"), categoryName);
        if (visibility is null)
        {
            return Task.FromResult(AIToolResult.Fail($"未找到类别 {categoryName} 或视图无效。"));
        }

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 类别 {visibility.Category} 在视图 {(visibility.Visible ? "可见" : "已隐藏")}",
            new { viewId = visibility.ViewId, category = visibility.Category, visible = visibility.Visible }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\GetPipeTypesTool.cs / GetDuctTypesTool.cs / GetCableTrayTypesTool.cs / GetRoofTypesTool.cs
internal sealed class TypeListImplementation : IToolImplementation
{
    private readonly string _categoryKey;
    private readonly string _displayName;

    public TypeListImplementation(string toolName, string categoryKey, string displayName)
    {
        ToolName = toolName;
        _categoryKey = categoryKey;
        _displayName = displayName;
    }

    public string ToolName { get; }

    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.TypeService ?? throw new InvalidOperationException("无法获取 TypeService");
        var document = ReadOnlySupport2.RequireDocument(adapter, context);
        var limit = context.GetParameter("limit", 200);
        var types = service.GetTypes(document, _categoryKey, limit);

        return Task.FromResult(AIToolResult.Ok(
            $"✅ 成功获取 {types.Count} 个{_displayName}类型",
            new
            {
                types = types.Select(type => new
                {
                    typeId = type.TypeId,
                    familyName = type.FamilyName,
                    typeName = type.TypeName,
                    category = type.Category,
                }).ToArray(),
                count = types.Count,
            }));
    }
}
