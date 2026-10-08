using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;
using RevitAi.Engine.ToolContracts;

namespace RevitAi.Engine.SmokeTests;

/// <summary>假的适配器：服务都返回可预测的值，用来在无 Revit 环境下驱动引擎原语与工具实现。</summary>
internal sealed class FakeAdapter : IRevitAdapter
{
    public object Document { get; } = new();

    public ILevelService LevelService { get; } = new FakeLevelService();

    public IElementService ElementService { get; } = new FakeElementService();

    public IModificationService ModificationService { get; } = new FakeModificationService();

    public IGridService GridService { get; } = new FakeGridService();

    public IViewService ViewService { get; } = new FakeViewService();

    public ICategoryService CategoryService { get; } = new FakeCategoryService();

    public IWorksetService WorksetService { get; } = new FakeWorksetService();

    public IFamilyQueryService FamilyQueryService { get; } = new FakeFamilyQueryService();

    public IRoomService RoomService { get; } = new FakeRoomService();

    public ITagService TagService { get; } = new FakeTagService();

    public IElementQueryService ElementQueryService { get; } = new FakeElementQueryService();

    public IProjectUnitService ProjectUnitService { get; } = new FakeProjectUnitService();

    public IViewQueryService ViewQueryService { get; } = new FakeViewQueryService();

    public ITypeService TypeService { get; } = new FakeTypeService();

    public IPhaseService PhaseService { get; } = new FakePhaseService();

    public IElementWorksetService ElementWorksetService { get; } = new FakeElementWorksetService();

    public ILinkService LinkService { get; } = new FakeLinkService();

    public IRoofService RoofService { get; } = new FakeRoofService();

    public IFilterParameterService FilterParameterService { get; } = new FakeFilterParameterService();

    public IViewWriteService ViewWriteService { get; } = new FakeViewWriteService();

    public IViewCreationService ViewCreationService { get; } = new FakeViewCreationService();

    public IFilterService FilterService { get; } = new FakeFilterService();

    public IFilterCreationService FilterCreationService { get; } = new FakeFilterCreationService();

    public ITagCreationService TagCreationService { get; } = new FakeTagCreationService();

    public IScheduleService ScheduleService { get; } = new FakeScheduleService();

    public IExportService ExportService { get; } = new FakeExportService();

    public IDimensionCreationService DimensionCreationService { get; } = new FakeDimensionCreationService();

    public IScheduleEditService ScheduleEditService { get; } = new FakeScheduleEditService();

    public IParameterService ParameterService { get; } = new FakeParameterService();

    public IGlobalParameterService GlobalParameterService { get; } = new FakeGlobalParameterService();

    public IMaterialService MaterialService { get; } = new FakeMaterialService();

    public ICompoundStructureService CompoundStructureService { get; } = new FakeCompoundStructureService();

    public IFamilyTypeService FamilyTypeService { get; } = new FakeFamilyTypeService();

    public IElementAdminService ElementAdminService { get; } = new FakeElementAdminService();

    public IElementColorService ElementColorService { get; } = new FakeElementColorService();

    public ISharedParameterService SharedParameterService { get; } = new FakeSharedParameterService();

    public IModelingService ModelingService { get; } = new FakeModelingService();

    public IMepModelingService MepModelingService { get; } = new FakeMepModelingService();

    public IShapeModelingService ShapeModelingService { get; } = new FakeShapeModelingService();

    public IElementOpsService ElementOpsService { get; } = new FakeElementOpsService();

    public IAdvancedQueryService AdvancedQueryService { get; } = new FakeAdvancedQueryService();

    public IProjectSettingsService ProjectSettingsService { get; } = new FakeProjectSettingsService();

    public ITopographyService TopographyService { get; } = new FakeTopographyService();

    public IInsulationService InsulationService { get; } = new FakeInsulationService();

    public IMepSystemService MepSystemService { get; } = new FakeMepSystemService();

    public IDomainExtraService DomainExtraService { get; } = new FakeDomainExtraService();

    public IViewInteractionService ViewInteractionService { get; } = new FakeViewInteractionService();

    public IStairService StairService { get; } = new FakeStairService();

    public IRoofEditService RoofEditService { get; } = new FakeRoofEditService();

    public ISatelliteImportService SatelliteImportService { get; } = new FakeSatelliteImportService();

    public object? GetActiveDocument() => Document;

    /// <summary>UI 会话占位对象（真实环境里是 UIDocument；冒烟测试只验证服务契约）。</summary>
    public object? UiDocument { get; set; } = new object();

    public object? GetActiveUiDocument() => UiDocument;
}

internal sealed class FakeGridService : IGridService
{
    public IReadOnlyList<GridInfo> GetAllGrids(object document) => new[]
    {
        new GridInfo(1001, "1"),
        new GridInfo(1002, "A"),
    };
}

internal sealed class FakeViewService : IViewService
{
    private readonly List<ViewInfo> _views = new()
    {
        new ViewInfo(2001, "标高 1", "FloorPlan", false),
        new ViewInfo(2002, "三维视图", "ThreeD", false),
        new ViewInfo(2003, "视图样板", "FloorPlan", true),
    };

    public IReadOnlyList<ViewInfo> GetAllViews(object document) => _views;

    public ViewInfo? GetActiveView(object document) => _views[0];
}

internal sealed class FakeCategoryService : ICategoryService
{
    public IReadOnlyList<CategoryInfo> GetAllCategories(object document) => new[]
    {
        new CategoryInfo("墙", -2000011, "Model"),
        new CategoryInfo("标高", -2000240, "Model"),
    };

    public IReadOnlyList<string> GetAllCategoryNames(object document) => GetAllCategories(document).Select(c => c.Name).ToArray();
}

internal sealed class FakeWorksetService : IWorksetService
{
    public IReadOnlyList<WorksetInfo> GetAllWorksets(object document) => new[]
    {
        new WorksetInfo(1, "工作集 1", true, true, "UserWorkset"),
    };
}

// ---- 批次 1 第二批 ----

internal sealed class FakeFamilyQueryService : IFamilyQueryService
{
    public IReadOnlyList<FamilyInfo> GetAllFamilies(object document) => new[]
    {
        new FamilyInfo(3001, "M_单扇门", "门", 3),
        new FamilyInfo(3002, "M_矩形窗", "窗", 2),
    };

    public IReadOnlyList<FamilyTypeInfo> GetFamilyTypes(object document, string? familyName, int limit)
    {
        var all = new[]
        {
            new FamilyTypeInfo(4001, "M_单扇门", "900x2100mm", "门"),
            new FamilyTypeInfo(4002, "M_矩形窗", "1200x1500mm", "窗"),
        };

        return string.IsNullOrWhiteSpace(familyName)
            ? all.Take(limit <= 0 ? all.Length : limit).ToArray()
            : all.Where(type => type.FamilyName == familyName).ToArray();
    }
}

internal sealed class FakeRoomService : IRoomService
{
    public IReadOnlyList<RoomInfo> GetAllRooms(object document) => new[]
    {
        new RoomInfo(5001, "101", "办公室", 24.5, 101, "标高 1"),
    };
}

internal sealed class FakeTagService : ITagService
{
    public IReadOnlyList<TagInfo> GetAllTags(object document) => new[]
    {
        new TagInfo(6001, "C30", 7001, "结构框架标记"),
    };
}

internal sealed class FakeElementQueryService : IElementQueryService
{
    public IReadOnlyList<ElementSummaryInfo> GetAllElements(object document, string? categoryName, int limit) => new[]
    {
        new ElementSummaryInfo(8001, "基本墙: 常规 - 200mm", "墙", "常规 - 200mm"),
        new ElementSummaryInfo(8002, "楼板: 常规 - 150mm", "楼板", "常规 - 150mm"),
    };

    public ElementGeometryInfo? GetElementGeometry(object document, int elementId)
    {
        if (elementId != 8001)
        {
            return null;
        }

        return new ElementGeometryInfo(
            elementId, "墙", "常规 - 200mm", "Curve",
            100, 200, 300,
            true, 0, 0, 0, 5000, 200, 3600,
            2, 1, "millimeters");
    }
}

internal sealed class FakeProjectUnitService : IProjectUnitService
{
    public ProjectUnitsInfo GetProjectUnits(object document)
        => new("autodesk.spec.aec:length-2.0.0", "mm", "autodesk.spec.aec:area-2.0.0", "autodesk.spec.aec:volume-2.0.0", "autodesk.spec:angle-2.0.0", "decimal feet");
}

internal sealed class FakeViewQueryService : IViewQueryService
{
    public ViewSettingsInfo? GetViewSettings(object document, int? viewId)
        => new(viewId ?? 2001, "标高 1", "FloorPlan", 100, "Medium", "Wireframe", true, false);

    public IReadOnlyList<ViewFilterInfo> GetViewFilters(object document, int? viewId) => new[]
    {
        new ViewFilterInfo(9001, "可见性过滤器", true, true),
    };

    public CategoryVisibilityInfo? GetCategoryVisibility(object document, int? viewId, string categoryName)
        => new(viewId ?? 2001, categoryName, true);
}

internal sealed class FakeTypeService : ITypeService
{
    public IReadOnlyList<FamilyTypeInfo> GetTypes(object document, string categoryKey, int limit) => new[]
    {
        new FamilyTypeInfo(10001, "管道类型", $"标准 {categoryKey}", "管道"),
    };
}

internal sealed class FakeLevelService : ILevelService
{
    private readonly List<FakeLevel> _levels = new()
    {
        new FakeLevel(101, "标高 1", 0.0),
        new FakeLevel(102, "标高 2", 9.84251968503937),
    };

    public IReadOnlyList<object> GetAllLevels(object document) => _levels.Cast<object>().ToArray();

    public int GetLevelId(object level) => ((FakeLevel)level).Id;

    public string GetLevelName(object level) => ((FakeLevel)level).Name;

    public double GetLevelElevationFeet(object level) => ((FakeLevel)level).ElevationFeet;

    public object? FindLevel(object document, string name, double elevationFeet, double toleranceFeet)
        => _levels.FirstOrDefault(level => string.Equals(level.Name, name, StringComparison.OrdinalIgnoreCase))
           ?? _levels.FirstOrDefault(level => Math.Abs(level.ElevationFeet - elevationFeet) <= toleranceFeet);

    public object CreateLevel(object document, string name, double elevationFeet, bool buildingStory)
    {
        var level = new FakeLevel(200 + _levels.Count, name, elevationFeet);
        _levels.Add(level);
        return level;
    }

    public void SetLevelElevation(object level, double elevationFeet) => ((FakeLevel)level).ElevationFeet = elevationFeet;

    public void SetLevelName(object level, string name) => ((FakeLevel)level).Name = name;

    public void SetBuildingStory(object level, bool isBuildingStory)
        => ((FakeLevel)level).IsBuildingStory = isBuildingStory;

    private sealed class FakeLevel
    {
        public FakeLevel(int id, string name, double elevationFeet)
        {
            Id = id;
            Name = name;
            ElevationFeet = elevationFeet;
        }

        public int Id { get; }

        public string Name { get; set; }

        public double ElevationFeet { get; set; }

        public bool IsBuildingStory { get; set; } = true;
    }
}

internal sealed class FakeElementService : IElementService
{
    public HashSet<int> Existing { get; } = new() { 101, 102, 103 };

    public bool Exists(object document, int elementId) => Existing.Contains(elementId);
}

internal sealed class FakeModificationService : IModificationService
{
    public List<(IReadOnlyList<int> Ids, double X, double Y, double Z)> Calls { get; } = new();

    public int MoveElements(object document, IReadOnlyList<int> elementIds, double dxFeet, double dyFeet, double dzFeet)
    {
        Calls.Add((elementIds, dxFeet, dyFeet, dzFeet));
        return elementIds.Count;
    }
}

/// <summary>Fake 工具：分别覆盖"直接执行""只读但要文档""写入需要事务"三条路由分支。</summary>
[AITool(FakeDirectTool.ToolName, Category = "测试", Description = "纯只读，不回主线程。", RequiresTransaction = false, RequiresModification = false, RequiresActiveDocument = false)]
internal sealed class FakeDirectTool : IAITool
{
    public const string ToolName = "fake_direct";

    public string Name => FakeDirectTool.ToolName;

    public string Description => "纯只读，不回主线程。";

    public string Category => "测试";

    public string ParametersSchema => """{"type":"object","properties":{}}""";

    public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(AIToolResult.Ok("direct"));
}

[AITool(FakeDocumentTool.ToolName, Category = "测试", Description = "只读但需要活动文档，回主线程但不建事务。", RequiresTransaction = false, RequiresModification = false, RequiresActiveDocument = true)]
internal sealed class FakeDocumentTool : IAITool
{
    public const string ToolName = "fake_document";

    public string Name => FakeDocumentTool.ToolName;

    public string Description => "只读但需要活动文档，回主线程但不建事务。";

    public string Category => "测试";

    public string ParametersSchema => """{"type":"object","properties":{}}""";

    public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(AIToolResult.Ok("document"));
}

[AITool(FakeWriteTool.ToolName, Category = "测试", Description = "写入，需要事务 + 回主线程。", RequiresTransaction = true, RequiresModification = true, RequiresActiveDocument = true)]
internal sealed class FakeWriteTool : IAITool
{
    public const string ToolName = "fake_write";

    public string Name => FakeWriteTool.ToolName;

    public string Description => "写入，需要事务 + 回主线程。";

    public string Category => "测试";

    public string ParametersSchema => """{"type":"object","properties":{}}""";

    public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(AIToolResult.Ok("write"));
}

/// <summary>假的 Revit 宿主：记录是否被调用、是否需要事务，然后直接执行动作（不涉及真实 Revit）。</summary>
internal sealed class FakeToolHost : IRevitToolHost
{
    public bool IsAvailable { get; set; } = true;

    public bool WasUsed { get; private set; }

    public bool? LastRequiresTransaction { get; private set; }

    public Task<AIToolResult> ExecuteAsync(
        AIToolContext context,
        bool requiresTransaction,
        Func<AIToolContext, Task<AIToolResult>> action)
    {
        WasUsed = true;
        LastRequiresTransaction = requiresTransaction;
        return action(context);
    }
}

// ---- 批次 1 第三批 ----

internal sealed class FakePhaseService : IPhaseService
{
    public ElementPhaseInfo? GetElementPhase(object document, int elementId)
        => elementId == 8001 ? new ElementPhaseInfo(elementId, 101, "现有", 0, string.Empty) : null;
}

internal sealed class FakeElementWorksetService : IElementWorksetService
{
    public ElementWorksetInfo? GetElementWorkset(object document, int elementId)
        => elementId == 8001 ? new ElementWorksetInfo(elementId, 1, "工作集 1") : null;
}

internal sealed class FakeLinkService : ILinkService
{
    public IReadOnlyList<LinkInfo> GetLinks(object document) => new[]
    {
        new LinkInfo(11001, "结构.rvt : 1", @"C:\Models\结构.rvt", true, "结构链接"),
    };

    public IReadOnlyList<ElementSummaryInfo> GetLinkElements(object document, int linkInstanceId, int limit)
        => linkInstanceId == 11001
            ? new[] { new ElementSummaryInfo(1, "柱", "结构柱", "混凝土 - 矩形") }
            : Array.Empty<ElementSummaryInfo>();
}

internal sealed class FakeRoofService : IRoofService
{
    public RoofInfo? GetRoofInfo(object document, int elementId)
        => elementId == 12001 ? new RoofInfo(elementId, "常规 - 125mm", "标高 3", 128.75, true, false) : null;
}

internal sealed class FakeFilterParameterService : IFilterParameterService
{
    public IReadOnlyList<FilterParameterInfo> GetAvailableFilterParameters(object document, string? categoryName) => new[]
    {
        new FilterParameterInfo(2001, "类型名称"),
        new FilterParameterInfo(2002, "标高"),
    };
}

// ---- 批次 2：视图与标注（写入） ----

internal sealed class FakeViewWriteService : IViewWriteService
{
    public List<string> Calls { get; } = new();

    public ViewSettingsInfo? SetDetailLevel(object document, int? viewId, string detailLevel)
    {
        Calls.Add($"detail:{detailLevel}");
        return new ViewSettingsInfo(viewId ?? 2001, "标高 1", "FloorPlan", 100, detailLevel, "Wireframe", true, false);
    }

    public ViewSettingsInfo? SetDisplayStyle(object document, int? viewId, string displayStyle)
    {
        Calls.Add($"style:{displayStyle}");
        return new ViewSettingsInfo(viewId ?? 2001, "标高 1", "FloorPlan", 100, "Medium", displayStyle, true, false);
    }

    public void SetBackgroundColor(object document, int red, int green, int blue) => Calls.Add($"bg:{red},{green},{blue}");

    public TemporaryViewControlResult ApplyTemporaryViewControl(object document, int? viewId, string action, IReadOnlyList<int> elementIds)
    {
        Calls.Add($"temp:{action}:{elementIds.Count}");
        return new TemporaryViewControlResult(
            viewId ?? 2001,
            action,
            elementIds.Count,
            !string.Equals(action, "RESET", StringComparison.OrdinalIgnoreCase));
    }

    public int DuplicateView(object document, int? viewId, string option, string? newName)
    {
        Calls.Add($"dup:{option}:{newName}");
        return 2100;
    }
}

internal sealed class FakeViewCreationService : IViewCreationService
{
    public int CreateFloorPlan(object document, string levelName, string? viewName) => 2201;

    public int CreateSection(object document, string? viewName, double minX, double minY, double maxX, double maxY, double minZ, double maxZ) => 2202;

    public int CreateSheet(object document, string? sheetNumber, string? sheetName) => 2203;

    public int CreateTextNote(object document, string text, double xMm, double yMm, int? viewId) => 2204;
}

internal sealed class FakeFilterService : IFilterService
{
    public List<string> Calls { get; } = new();

    public int ApplyFilterToView(object document, int? viewId, string filterName, string action, int red, int green, int blue, bool visible)
    {
        Calls.Add($"{action}:{filterName}");
        return 9001;
    }

    public bool DeleteFilter(object document, string filterName) => filterName != "不存在的过滤器";
}

// ---- 批次 2 剩余 ----

internal sealed class FakeFilterCreationService : IFilterCreationService
{
    public int CreateFilter(object document, string filterName, string categoryName, string parameterName, string? value, string? rule)
        => filterName == "已存在" ? throw new InvalidOperationException("同名过滤器已存在。") : 9500;
}

internal sealed class FakeTagCreationService : ITagCreationService
{
    public int CreateTag(object document, int elementId, int? tagTypeId, int? viewId, double xMm, double yMm, string? orientation, bool addLeader)
        => elementId == 8001 ? 9600 : throw new InvalidOperationException($"未找到元素 {elementId}。");
}

internal sealed class FakeScheduleService : IScheduleService
{
    public int CreateSchedule(object document, string categoryName, string? name, IReadOnlyList<string> fields) => 9700;

    public ExportResult ExportScheduleToCsv(object document, int? scheduleId, string? scheduleName, string? outputPath)
        => new(@"C:\Temp\exports\明细表.csv", 12, "rows");
}

internal sealed class FakeExportService : IExportService
{
    public ExportResult ExportViewImages(object document, IReadOnlyList<int> viewIds, string? outputDirectory, int pixelSize)
        => new(@"C:\Temp\exports", viewIds.Count == 0 ? 1 : viewIds.Count, "images");

    public ExportResult ExportViewsAsDwg(object document, IReadOnlyList<int> viewIds, string? outputDirectory, string? exportSettingName)
        => new(@"C:\Temp\exports", viewIds.Count == 0 ? 1 : viewIds.Count, "views");
}

// ---- 批次 2 收尾 ----

internal sealed class FakeDimensionCreationService : IDimensionCreationService
{
    public int CreateSpotDimension(object document, string kind, int elementId, int? viewId, double xMm, double yMm, int? spotDimensionTypeId, bool hasLeader)
        => elementId == 8001 ? 9800 : throw new InvalidOperationException($"未找到元素 {elementId}。");
}

internal sealed class FakeScheduleEditService : IScheduleEditService
{
    public int EditSchedule(
        object document,
        int? scheduleId,
        string? scheduleName,
        IReadOnlyList<int> addFieldParameterIds,
        IReadOnlyList<int> removeFieldParameterIds,
        bool replaceFields,
        int? sortByParameterId,
        string? sortOrder,
        int? groupByParameterId)
        => 9700;
}

// ---- 批次 3：参数 ----

internal sealed class FakeParameterService : IParameterService
{
    public List<ParameterAssignment> Applied { get; } = new();

    public IReadOnlyList<ParameterInfo> QueryParameters(object document, int elementId, IReadOnlyList<string> names, bool includeTypeParameters)
    {
        var all = new List<ParameterInfo>
        {
            new ParameterInfo(elementId, 1001, "类型名称", "String", "常规 - 200mm", true, false, true, "PG_TEXT"),
            new ParameterInfo(elementId, 1002, "标高", "ElementId", "101", false, false, false, "PG_IDENTITY_DATA"),
            new ParameterInfo(elementId, 1003, "体积", "Double", "1.5 m³", false, false, false, "PG_GEOMETRY"),
        };

        return names.Count == 0
            ? all
            : all.Where(item => names.Any(filter => string.Equals(filter, item.Name, StringComparison.OrdinalIgnoreCase))).ToArray();
    }

    public ParameterSetResult SetParameterValues(object document, IReadOnlyList<ParameterAssignment> assignments)
    {
        Applied.AddRange(assignments);
        var failures = assignments
            .Where(item => string.Equals(item.ParameterName, "只读参数", StringComparison.OrdinalIgnoreCase))
            .Select(_ => "参数 只读参数 为只读")
            .ToList();

        return new ParameterSetResult(assignments.Count, assignments.Count - failures.Count, failures);
    }
}

internal sealed class FakeGlobalParameterService : IGlobalParameterService
{
    private readonly Dictionary<string, GlobalParameterInfo> _items = new(StringComparer.OrdinalIgnoreCase)
    {
        ["层高"] = new GlobalParameterInfo(3001, "层高", "3000", string.Empty, "autodesk.spec.aec:length-2.0.0"),
    };

    public IReadOnlyList<GlobalParameterInfo> List(object document) => _items.Values.ToArray();

    public GlobalParameterInfo Create(object document, string name, string? parameterType, string? formula, string? value)
    {
        var info = new GlobalParameterInfo(3002, name, value ?? "0", formula ?? string.Empty, parameterType ?? "Number");
        _items[name] = info;
        return info;
    }

    public GlobalParameterInfo? SetValue(object document, string name, string? value, string? formula)
    {
        if (!_items.TryGetValue(name, out var existing))
        {
            return null;
        }

        var updated = existing with { Value = value ?? existing.Value, Formula = formula ?? existing.Formula };
        _items[name] = updated;
        return updated;
    }

    public bool Delete(object document, string name) => _items.Remove(name);
}

// ---- 批次 3 第二刀：材料 / 复合层 ----

internal sealed class FakeMaterialService : IMaterialService
{
    private readonly List<MaterialInfo> _items = new()
    {
        new MaterialInfo(5001, "混凝土 - 现场浇筑", "混凝土", "#808080", 0, false),
        new MaterialInfo(5002, "玻璃", "玻璃", "#A0C8E0", 60, false),
    };

    public IReadOnlyList<MaterialInfo> Query(object document, string? nameFilter, int limit)
        => (string.IsNullOrWhiteSpace(nameFilter)
                ? _items
                : _items.Where(item => item.Name.Contains(nameFilter!, StringComparison.OrdinalIgnoreCase)).ToList())
            .Take(limit <= 0 ? 500 : limit)
            .ToArray();

    public MaterialInfo? Get(object document, int? materialId, string? name)
        => _items.FirstOrDefault(item =>
            (materialId is not null && item.MaterialId == materialId.Value) ||
            (!string.IsNullOrWhiteSpace(name) && string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase)));

    public MaterialInfo Create(object document, string name, string? colorHex, int? transparency, bool force)
    {
        var existing = Get(document, null, name);
        if (existing is not null && !force)
        {
            throw new InvalidOperationException($"材料 {name} 已存在（materialId={existing.MaterialId}）；force=true 可复用并更新。");
        }

        var info = existing is not null
            ? existing with { ColorHex = colorHex ?? existing.ColorHex, Transparency = transparency ?? existing.Transparency }
            : new MaterialInfo(5099, name, string.Empty, colorHex ?? "#FFFFFF", transparency ?? 0, false);

        _items.RemoveAll(item => item.MaterialId == info.MaterialId);
        _items.Add(info);
        return info;
    }

    public MaterialInfo? Update(object document, int? materialId, string? name, string? colorHex, int? transparency)
    {
        var existing = Get(document, materialId, name);
        if (existing is null)
        {
            return null;
        }

        var info = existing with
        {
            ColorHex = colorHex ?? existing.ColorHex,
            Transparency = transparency ?? existing.Transparency,
        };
        _items.RemoveAll(item => item.MaterialId == info.MaterialId);
        _items.Add(info);
        return info;
    }

    public bool Delete(object document, int? materialId, string? name)
    {
        var existing = Get(document, materialId, name);
        return existing is not null && _items.RemoveAll(item => item.MaterialId == existing.MaterialId) > 0;
    }
}

internal sealed class FakeCompoundStructureService : ICompoundStructureService
{
    private readonly List<CompoundLayerInfo> _layers = new()
    {
        new CompoundLayerInfo(0, "Finish1", 20, 5001, "混凝土 - 现场浇筑", false, false),
        new CompoundLayerInfo(1, "Structure", 200, 5001, "混凝土 - 现场浇筑", true, true),
    };

    public IReadOnlyList<CompoundLayerInfo> GetLayers(object document, int? typeId, string? typeName) => _layers.ToArray();

    public int AddLayer(
        object document,
        int? typeId,
        string? typeName,
        double thicknessMm,
        string function,
        int? materialId,
        string? materialName,
        int? insertIndex)
    {
        var index = Math.Clamp(insertIndex ?? _layers.Count, 0, _layers.Count);
        _layers.Insert(index, new CompoundLayerInfo(index, function, thicknessMm, materialId ?? 5001, materialName ?? "混凝土 - 现场浇筑", false, false));
        return _layers.Count;
    }

    public bool DeleteLayer(object document, int? typeId, string? typeName, int layerIndex)
    {
        if (layerIndex < 0 || layerIndex >= _layers.Count || _layers.Count <= 1)
        {
            throw new InvalidOperationException($"layerIndex {layerIndex} 越界（共 {_layers.Count} 层）。");
        }

        _layers.RemoveAt(layerIndex);
        return true;
    }

    public bool ModifyLayer(
        object document,
        int? typeId,
        string? typeName,
        int layerIndex,
        double? thicknessMm,
        string? function,
        int? materialId,
        string? materialName)
    {
        if (layerIndex < 0 || layerIndex >= _layers.Count)
        {
            throw new InvalidOperationException($"layerIndex {layerIndex} 越界（共 {_layers.Count} 层）。");
        }

        var layer = _layers[layerIndex];
        _layers[layerIndex] = layer with
        {
            WidthMm = thicknessMm ?? layer.WidthMm,
            Function = function ?? layer.Function,
            MaterialId = materialId ?? layer.MaterialId,
            MaterialName = materialName ?? layer.MaterialName,
        };

        return true;
    }
}

// ---- 批次 3 第三刀：族类型 / 元素管理 / 元素着色 ----

internal sealed class FakeFamilyTypeService : IFamilyTypeService
{
    public int DuplicateType(object document, int sourceTypeId, string newTypeName)
        => sourceTypeId == 300 ? 3100 : throw new InvalidOperationException($"元素 {sourceTypeId} 不是可复制的类型。");

    public int ChangeElementTypes(object document, IReadOnlyList<int> elementIds, int newTypeId)
        => elementIds.Count(id => id != 9999);
}

internal sealed class FakeElementAdminService : IElementAdminService
{
    private readonly Dictionary<int, bool> _pinned = new();
    private readonly Dictionary<int, GroupInfo> _groups = new();

    public IReadOnlyList<ElementLockInfo> GetLockState(object document, IReadOnlyList<int> elementIds)
        => elementIds
            .Select(id => new ElementLockInfo(id, $"元素{id}", "墙", _pinned.TryGetValue(id, out var pinned) && pinned))
            .ToArray();

    public int SetPinned(object document, IReadOnlyList<int> elementIds, bool pinned)
    {
        var changed = 0;
        foreach (var id in elementIds)
        {
            if (id == 9999)
            {
                continue;
            }

            _pinned[id] = pinned;
            changed++;
        }

        return changed;
    }

    public GroupInfo? CreateGroup(object document, IReadOnlyList<int> elementIds, string? groupName)
    {
        if (elementIds.Count == 0)
        {
            return null;
        }

        var info = new GroupInfo(
            4000 + _groups.Count,
            string.IsNullOrWhiteSpace(groupName) ? $"组 {_groups.Count + 1}" : groupName!,
            elementIds.Count,
            elementIds.ToArray());
        _groups[info.GroupId] = info;
        return info;
    }

    public IReadOnlyList<GroupInfo> ListGroups(object document, int limit)
        => _groups.Values.Take(limit <= 0 ? 200 : limit).ToArray();

    public int Ungroup(object document, IReadOnlyList<int> groupIds) => groupIds.Count(id => _groups.Remove(id));

    public PurgeResult PurgeUnused(object document, IReadOnlyList<string> kinds)
        => new(
            3,
            7,
            kinds.Count > 0 ? kinds : new List<string> { "types", "materials" },
            new List<string> { "5001: 元素被使用" });
}

internal sealed class FakeElementColorService : IElementColorService
{
    public ElementColorResult SetElementColor(
        object document,
        int? viewId,
        IReadOnlyList<int> elementIds,
        string action,
        string? colorHex,
        int? transparency)
    {
        if (string.Equals(action, "set", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(colorHex))
        {
            throw new InvalidOperationException("action=set 时必须提供合法颜色（#RRGGBB 或 red/green/blue）。");
        }

        var failures = elementIds.Where(id => id == 9999).Select(id => $"元素 {id} 不存在").ToList();
        return new ElementColorResult(elementIds.Count, elementIds.Count - failures.Count, failures);
    }
}

// ---- 批次 3 第四刀：共享参数 / 项目参数 ----

internal sealed class FakeSharedParameterService : ISharedParameterService
{
    private readonly List<SharedParameterGroupInfo> _groups = new()
    {
        new SharedParameterGroupInfo("常用", 1, new List<string> { "自定义参数" }),
    };

    private readonly List<ProjectParameterInfo> _projectParameters = new()
    {
        new ProjectParameterInfo("自定义参数", "instance", true, new List<string> { "墙" }),
    };

    public SharedParameterFileStatus GetFileStatus(object document)
        => new(true, "共享参数文件已加载。");

    public IReadOnlyList<SharedParameterGroupInfo> ListSharedParameterGroups(object document) => _groups.ToArray();

    public string CreateSharedParameter(object document, string parameterName, string? parameterType, string groupName)
    {
        var index = _groups.FindIndex(group => string.Equals(group.Name, groupName, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            _groups.Add(new SharedParameterGroupInfo(groupName, 1, new List<string> { parameterName }));
            return parameterName;
        }

        var existing = _groups[index];
        if (!existing.ParameterNames.Contains(parameterName, StringComparer.OrdinalIgnoreCase))
        {
            _groups[index] = existing with
            {
                DefinitionCount = existing.DefinitionCount + 1,
                ParameterNames = existing.ParameterNames.Append(parameterName).ToList(),
            };
        }

        return parameterName;
    }

    public IReadOnlyList<ProjectParameterInfo> ListProjectParameters(object document) => _projectParameters.ToArray();

    public int CreateProjectParameter(
        object document,
        string parameterName,
        string? sharedParameterName,
        string? bindingType,
        IReadOnlyList<string> categoryNames,
        bool instanceParameter)
    {
        var lookupName = string.IsNullOrWhiteSpace(sharedParameterName) ? parameterName : sharedParameterName!;
        var exists = _groups.SelectMany(group => group.ParameterNames)
            .Any(name => string.Equals(name, lookupName, StringComparison.OrdinalIgnoreCase));

        if (!exists)
        {
            throw new InvalidOperationException($"共享参数文件里找不到定义 {lookupName}。");
        }

        if (categoryNames.Count == 0)
        {
            throw new InvalidOperationException("必须提供 categoryNames（至少一个类别）。");
        }

        if (_projectParameters.Any(parameter => string.Equals(parameter.Name, parameterName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"项目参数 {parameterName} 已经绑定。");
        }

        _projectParameters.Add(new ProjectParameterInfo(
            parameterName,
            instanceParameter ? "instance" : bindingType ?? "type",
            true,
            categoryNames.ToArray()));
        return categoryNames.Count;
    }

    public bool DeleteProjectParameter(object document, string parameterName)
        => _projectParameters.RemoveAll(parameter => string.Equals(parameter.Name, parameterName, StringComparison.OrdinalIgnoreCase)) > 0;

    public int UpdateProjectParameterCategories(object document, string parameterName, IReadOnlyList<string> categoryNames, bool add)
    {
        var index = _projectParameters.FindIndex(parameter => string.Equals(parameter.Name, parameterName, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            throw new InvalidOperationException($"未找到项目参数 {parameterName}。");
        }

        var current = _projectParameters[index];
        var set = current.CategoryNames.ToList();
        foreach (var name in categoryNames)
        {
            if (add)
            {
                if (!set.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    set.Add(name);
                }
            }
            else
            {
                set.RemoveAll(item => string.Equals(item, name, StringComparison.OrdinalIgnoreCase));
            }
        }

        _projectParameters[index] = current with { CategoryNames = set };
        return set.Count;
    }
}

// ---- 批次 4 第一刀：建模原语 ----

internal sealed class FakeModelingService : IModelingService
{
    private int _nextId = 6000;

    public List<string> Calls { get; } = new();

    public CreateManyResult CreateGrids(object document, IReadOnlyList<GridSpec> grids)
    {
        Calls.Add($"grids:{grids.Count}");
        return Many(grids.Count, grids.Any(grid => Math.Abs(grid.StartX - grid.EndX) < 0.001 && Math.Abs(grid.StartY - grid.EndY) < 0.001));
    }

    public int CreateRoom(object document, int levelId, double xMm, double yMm, string? roomName, string? roomNumber)
    {
        Calls.Add($"room:{levelId}:{roomName}");
        return _nextId++;
    }

    public CreateManyResult CreateWalls(object document, IReadOnlyList<WallSpec> walls)
    {
        Calls.Add($"walls:{walls.Count}");
        return Many(walls.Count, walls.Any(wall => Math.Abs(wall.StartX - wall.EndX) < 0.001 && Math.Abs(wall.StartY - wall.EndY) < 0.001));
    }

    public CreateManyResult CreateColumns(object document, IReadOnlyList<ColumnSpec> columns)
    {
        Calls.Add($"columns:{columns.Count}");
        return Many(columns.Count, false);
    }

    public CreateManyResult CreateBeams(object document, IReadOnlyList<BeamSpec> beams)
    {
        Calls.Add($"beams:{beams.Count}");
        return Many(beams.Count, false);
    }

    public CreateManyResult CreateFamilyInstances(object document, IReadOnlyList<InstanceSpec> instances)
    {
        Calls.Add($"instances:{instances.Count}");
        return Many(instances.Count, instances.Any(instance => instance.TypeId <= 0));
    }

    public CreateManyResult CreateHostedInstances(object document, IReadOnlyList<HostedSpec> hosted)
    {
        Calls.Add($"hosted:{hosted.Count}");
        return Many(hosted.Count, hosted.Any(item => item.HostId <= 0 || item.TypeId <= 0));
    }

    private CreateManyResult Many(int requested, bool anyInvalid)
    {
        if (requested == 0)
        {
            return new CreateManyResult(0, 0, Array.Empty<int>(), new[] { "请求为空" });
        }

        if (anyInvalid)
        {
            return new CreateManyResult(requested, requested - 1, Enumerable.Range(0, requested - 1).Select(_ => _nextId++).ToArray(), new[] { "第 1 条参数无效（起终点重合或 id 非法）" });
        }

        return new CreateManyResult(requested, requested, Enumerable.Range(0, requested).Select(_ => _nextId++).ToArray(), Array.Empty<string>());
    }
}

// ---- 批次 4 第二刀：MEP / 形体 ----

internal sealed class FakeMepModelingService : IMepModelingService
{
    private int _nextId = 7000;

    public CreateManyResult CreatePipes(object document, IReadOnlyList<MePSpec> items)
        => Many(items.Count, items.Any(item => IsZeroLength(item)));

    public CreateManyResult CreateDucts(object document, IReadOnlyList<MePSpec> items)
        => Many(items.Count, items.Any(item => IsZeroLength(item)));

    public CreateManyResult CreateCableTrays(object document, IReadOnlyList<MePSpec> items)
        => Many(items.Count, items.Any(item => IsZeroLength(item)));

    private static bool IsZeroLength(MePSpec item)
        => Math.Abs(item.StartX - item.EndX) < 0.001 &&
           Math.Abs(item.StartY - item.EndY) < 0.001 &&
           Math.Abs(item.StartZ - item.EndZ) < 0.001;

    private CreateManyResult Many(int requested, bool anyInvalid)
    {
        if (requested == 0)
        {
            return new CreateManyResult(0, 0, Array.Empty<int>(), new[] { "请求为空" });
        }

        if (anyInvalid)
        {
            return new CreateManyResult(requested, requested - 1, Enumerable.Range(0, requested - 1).Select(_ => _nextId++).ToArray(), new[] { "存在零长度直段" });
        }

        return new CreateManyResult(requested, requested, Enumerable.Range(0, requested).Select(_ => _nextId++).ToArray(), Array.Empty<string>());
    }
}

internal sealed class FakeShapeModelingService : IShapeModelingService
{
    private int _nextId = 7500;

    public CreateManyResult CreateFloors(object document, IReadOnlyList<ProfileSpec> floors)
        => Many(floors.Count, floors.Any(floor => floor.Points.Count < 3));

    public CreateManyResult CreateFootprintRoofs(object document, IReadOnlyList<ProfileSpec> roofs)
        => Many(roofs.Count, roofs.Any(roof => roof.Points.Count < 3));

    public int CreateExtrusionRoof(
        object document,
        IReadOnlyList<Point2> profile,
        int levelId,
        int? roofTypeId,
        double extrusionStartMm,
        double extrusionEndMm)
    {
        if (profile.Count < 3)
        {
            throw new InvalidOperationException("拉伸屋顶的轮廓至少需要 3 个点。");
        }

        return _nextId++;
    }

    public int CreateDimensionByElements(object document, int viewId, IReadOnlyList<int> elementIds, double offsetXMm, double offsetYMm)
    {
        if (elementIds.Count < 2)
        {
            throw new InvalidOperationException("按元素标注尺寸至少需要 2 个元素。");
        }

        return _nextId++;
    }

    public int Create3DView(object document, string? viewName, IReadOnlyList<int> elementIds) => _nextId++;

    private CreateManyResult Many(int requested, bool anyInvalid)
    {
        if (requested == 0)
        {
            return new CreateManyResult(0, 0, Array.Empty<int>(), new[] { "请求为空" });
        }

        if (anyInvalid)
        {
            return new CreateManyResult(requested, requested - 1, Enumerable.Range(0, requested - 1).Select(_ => _nextId++).ToArray(), new[] { "轮廓点数不足 3" });
        }

        return new CreateManyResult(requested, requested, Enumerable.Range(0, requested).Select(_ => _nextId++).ToArray(), Array.Empty<string>());
    }
}

// ---- 批次 4 第三刀：元素操作 ----

internal sealed class FakeElementOpsService : IElementOpsService
{
    private int _nextId = 8000;

    public TransformResult CopyElements(object document, IReadOnlyList<int> elementIds, double dxMm, double dyMm, double dzMm)
        => Many(elementIds.Count, false);

    public TransformResult RotateElements(
        object document,
        IReadOnlyList<int> elementIds,
        double axisX,
        double axisY,
        double axisZ,
        double angleDegrees,
        double originX,
        double originY,
        double originZ)
        => axisX == 0 && axisY == 0 && axisZ == 0
            ? new TransformResult(elementIds.Count, 0, Array.Empty<int>(), new[] { "旋转轴向量不能为零向量" })
            : Many(elementIds.Count, false);

    public TransformResult MirrorElements(
        object document,
        IReadOnlyList<int> elementIds,
        string mirrorType,
        double lineStartX,
        double lineStartY,
        double lineEndX,
        double lineEndY,
        double pointX,
        double pointY)
        => string.Equals(mirrorType, "line", StringComparison.OrdinalIgnoreCase) &&
           Math.Abs(lineStartX - lineEndX) < 0.001 && Math.Abs(lineStartY - lineEndY) < 0.001
            ? new TransformResult(elementIds.Count, 0, Array.Empty<int>(), new[] { "镜像线的起终点不能重合" })
            : Many(elementIds.Count, false);

    public TransformResult ArrayElements(
        object document,
        int elementId,
        string arrayType,
        int count,
        double moveX,
        double moveY,
        double moveZ,
        double originX,
        double originY,
        double originZ)
        => count < 2
            ? new TransformResult(count, 0, Array.Empty<int>(), new[] { "count 至少为 2" })
            : Many(count - 1, false);

    public int SplitElement(
        object document,
        int elementId,
        string splitMode,
        double? lengthMm,
        IReadOnlyList<Point2> points,
        IReadOnlyList<double> parameters)
    {
        if (elementId == 9999)
        {
            throw new InvalidOperationException($"元素 {elementId} 不是曲线类 MEP 构件。");
        }

        if (string.Equals(splitMode, "parameter", StringComparison.OrdinalIgnoreCase) && parameters.Count == 0)
        {
            throw new InvalidOperationException("splitMode=parameter 需要提供 parameters。");
        }

        return points.Count > 0 ? points.Count : 1;
    }

    public int DeleteElements(object document, IReadOnlyList<int> elementIds)
        => elementIds.Count(id => id != 9999);

    public int JoinGeometry(object document, IReadOnlyList<int> firstIds, IReadOnlyList<int> secondIds, string operation)
    {
        if (firstIds.Count == 0 || secondIds.Count == 0)
        {
            throw new InvalidOperationException("需要提供两组元素。");
        }

        return Math.Max(firstIds.Count, secondIds.Count);
    }

    public int CutGeometry(object document, IReadOnlyList<int> toCutIds, IReadOnlyList<int> cuttingIds, string operation)
    {
        if (toCutIds.Count == 0 || cuttingIds.Count == 0)
        {
            throw new InvalidOperationException("需要提供被剪切元素与剪切元素。");
        }

        return Math.Max(toCutIds.Count, cuttingIds.Count);
    }

    private TransformResult Many(int requested, bool anyInvalid)
        => requested == 0
            ? new TransformResult(0, 0, Array.Empty<int>(), new[] { "请求为空" })
            : anyInvalid
                ? new TransformResult(requested, requested - 1, Enumerable.Range(0, requested - 1).Select(_ => _nextId++).ToArray(), new[] { "存在无效项" })
                : new TransformResult(requested, requested, Enumerable.Range(0, requested).Select(_ => _nextId++).ToArray(), Array.Empty<string>());
}

// ---- 查询与交互 ----

internal sealed class FakeAdvancedQueryService : IAdvancedQueryService
{
    private static ElementSummary Sample(int id, string category, string typeName) =>
        new(id, $"元素{id}", category, 300, typeName, 101, "标高 1");

    public IReadOnlyList<ElementSummary> QueryElements(
        object document,
        string operation,
        IReadOnlyList<int> elementIds,
        string? categoryName,
        int? typeId,
        string? typeName,
        int limit)
    {
        if (string.Equals(operation, "byType", StringComparison.OrdinalIgnoreCase) && typeId is null && string.IsNullOrWhiteSpace(typeName))
        {
            throw new InvalidOperationException("operation=byType 需要提供 typeId 或 typeName。");
        }

        return new[]
        {
            Sample(8001, "墙", "常规 - 200mm"),
            Sample(8002, "楼板", "常规 - 100mm"),
        };
    }

    public IReadOnlyList<ElementSummary> FilterElements(
        object document,
        IReadOnlyList<int> elementIds,
        string? categoryName,
        string? typeName,
        string? levelName,
        int limit)
        => string.Equals(categoryName, "不存在的类别", StringComparison.OrdinalIgnoreCase)
            ? Array.Empty<ElementSummary>()
            : new[] { Sample(8001, "墙", "常规 - 200mm") };

    public IReadOnlyList<BoundaryLoopInfo> GetRoomBoundaries(object document, int roomId)
    {
        if (roomId == 9999)
        {
            throw new InvalidOperationException($"元素 {roomId} 不是房间（Room）。");
        }

        return new[]
        {
            new BoundaryLoopInfo(0, true, 20000, new List<Point2> { new(0, 0), new(6000, 0), new(6000, 4000), new(0, 4000), new(0, 0) }),
        };
    }

    public GridIntersectionInfo GetGridIntersection(object document, int grid1Id, int grid2Id)
        => grid1Id == grid2Id
            ? new GridIntersectionInfo(false, 0, 0, 0, "两条轴网平行或重合，没有唯一交点。")
            : new GridIntersectionInfo(true, 0, 0, 0, "轴网 1 × 轴网 2");

    public IReadOnlyList<ScheduleFieldInfo> GetScheduleFields(object document, int? scheduleId, string? scheduleName)
        => new[]
        {
            new ScheduleFieldInfo(0, "类型", "Instance", false, "类型"),
            new ScheduleFieldInfo(1, "体积", "Instance", false, "体积"),
        };

    public ScheduleDataInfo ReadScheduleData(
        object document,
        int? scheduleId,
        string? scheduleName,
        int startRow,
        int maxRows,
        bool includeHeader)
    {
        var headers = includeHeader ? new List<string> { "类型", "体积" } : new List<string>();
        var rows = new List<IReadOnlyList<string>>
        {
            new List<string> { "常规 - 200mm", "1.50" },
            new List<string> { "常规 - 100mm", "0.75" },
        };

        return new ScheduleDataInfo(scheduleName ?? "墙明细表", headers, rows, 2, rows.Count);
    }

    public IReadOnlyList<CollisionPairInfo> CheckCollision(object document, IReadOnlyList<int> sourceIds, IReadOnlyList<int> targetIds)
        => sourceIds.Contains(8001) && (targetIds.Count == 0 || targetIds.Contains(8002))
            ? new[] { new CollisionPairInfo(8001, 8002, true) }
            : new[] { new CollisionPairInfo(sourceIds.FirstOrDefault(), -1, false) };

    public bool DeleteLink(object document, int linkId)
        => linkId == 11001
            ? true
            : throw new InvalidOperationException($"元素 {linkId} 不是 Revit 链接。");
}

internal sealed class FakeProjectSettingsService : IProjectSettingsService
{
    public int SetProjectUnits(object document, string? length, string? area, string? volume, string? angle, string? slope)
    {
        var changed = new[] { length, area, volume, angle, slope }.Count(value => !string.IsNullOrWhiteSpace(value));
        if (changed == 0)
        {
            throw new InvalidOperationException("没有可应用的单位设置。");
        }

        return changed;
    }

    public int SetCategoryVisibility(object document, int viewId, string categoryName, bool visible)
        => categoryName == "不存在的类别"
            ? throw new InvalidOperationException($"未找到类别 {categoryName}。")
            : 1;
}

// ---- 批次 5 第一批：地形 / 保温 / MEP 系统 ----

internal sealed class FakeTopographyService : ITopographyService
{
    public int CreateTopography(object document, IReadOnlyList<Point3> points)
        => points.Distinct().Count() < 3
            ? throw new InvalidOperationException("地形点必须互不重合（至少 3 个不同点）。")
            : 9001;

    public TopographyInfo AddPointsFromFloors(object document, int topographyId, IReadOnlyList<int> floorIds)
        => floorIds.Count == 0
            ? throw new InvalidOperationException("必须提供 floor ids。")
            : new TopographyInfo(topographyId, 12, floorIds.Count * 4, 0, 3000);
}

internal sealed class FakeInsulationService : IInsulationService
{
    public IReadOnlyList<InsulationInfo> QueryInsulation(object document, IReadOnlyList<int> elementIds, bool onlyUninsulated, int limit)
        => onlyUninsulated
            ? new[] { new InsulationInfo(300, "常规 - 200mm", "常规 - 200mm", false, 0, string.Empty, 0) }
            : new[]
            {
                new InsulationInfo(300, "常规 - 200mm", "常规 - 200mm", true, 25, "岩棉", 0),
                new InsulationInfo(300, "常规 - 200mm", "常规 - 200mm", false, 200, "混凝土", 1),
            };

    public int ManageInsulation(
        object document,
        string operation,
        IReadOnlyList<int> elementIds,
        double? thicknessMm,
        string? materialName,
        bool overrideExisting)
        => elementIds.Contains(9999) ? 0 : elementIds.Count;
}

internal sealed class FakeMepSystemService : IMepSystemService
{
    private readonly Dictionary<int, MepSystemInfo> _systems = new()
    {
        [9500] = new MepSystemInfo(
            9500,
            "家用冷水 1",
            "家用冷水",
            "piping",
            3,
            new List<int> { 8001, 8002, 8003 },
            1,
            new Dictionary<string, int> { ["100"] = 2, ["50"] = 1 }),
    };

    public IReadOnlyList<MepSystemInfo> QuerySystems(object document, string operation, int? systemTypeId, bool onlyUninsulated, int limit)
        => _systems.Values
            .Where(system => !onlyUninsulated || system.UninsulatedMemberCount > 0)
            .Take(limit <= 0 ? 200 : limit)
            .ToArray();

    public MepSystemInfo? CreateSystem(object document, int systemTypeId, string? newName)
        => systemTypeId == 9999
            ? throw new InvalidOperationException($"元素 {systemTypeId} 不是管道/风管系统类型。")
            : new MepSystemInfo(9600, newName ?? "新建系统", "家用冷水", "piping", 0, Array.Empty<int>(), 0, new Dictionary<string, int>());

    public int RenameSystem(object document, int systemId, string newName)
        => _systems.ContainsKey(systemId)
            ? 1
            : throw new InvalidOperationException($"元素 {systemId} 不是 MEP 系统。");

    public bool DeleteSystem(object document, int systemId, bool force)
    {
        if (!_systems.TryGetValue(systemId, out var system))
        {
            throw new InvalidOperationException($"元素 {systemId} 不是 MEP 系统。");
        }

        if (system.MemberCount > 0 && !force)
        {
            throw new InvalidOperationException($"系统 {system.Name} 仍有 {system.MemberCount} 个成员，删除前请先清空成员或用 force=true。");
        }

        return _systems.Remove(systemId);
    }
}

// ---- 收尾批：道路 / CAD / 剪切顺序 + UI 交互 ----

internal sealed class FakeDomainExtraService : IDomainExtraService
{
    public RoadResult CreateRoadFromWalls(
        object document,
        IReadOnlyList<int> wallIds,
        double? laneWidthMeters,
        double? sidewalkWidthMeters,
        double? defaultRoadWidthMeters)
    {
        if (wallIds.Count == 0)
        {
            throw new InvalidOperationException("没有可用的墙。");
        }

        return new RoadResult(
            9800,
            wallIds.Count,
            (defaultRoadWidthMeters ?? 6.0) * 1000,
            true,
            new List<Point2> { new(0, 0), new(12000, 0), new(12000, 6000), new(0, 6000) },
            new List<string> { "轮廓来自墙中心线首尾相接的闭合环" });
    }

    public IReadOnlyList<CadLayerInfo> ManageCadLayers(object document, int viewId, int linkId, string operation, string? layerName)
    {
        if (linkId == 9999)
        {
            throw new InvalidOperationException($"未找到 CAD 链接/导入元素 {linkId}。");
        }

        return (operation ?? "list").Trim().ToLowerInvariant() switch
        {
            "list" => new List<CadLayerInfo>
            {
                new("A-WALL", 5001, true),
                new("A-DOOR", 5002, false),
            },
            "show" or "hide" => new List<CadLayerInfo> { new(layerName ?? "A-WALL", 5001, operation == "show") },
            _ => throw new InvalidOperationException("operation 只支持 list / show / hide。"),
        };
    }

    public CutOrderResult FixCuttingOrder(object document, IReadOnlyList<int> cuttingIds, IReadOnlyList<int> toCutIds, bool dryRun)
    {
        if (cuttingIds.Count == 0 || toCutIds.Count == 0)
        {
            throw new InvalidOperationException("需要提供两组元素。");
        }

        var pairs = Math.Max(cuttingIds.Count, toCutIds.Count);
        return dryRun
            ? new CutOrderResult(pairs, 0, true, new List<string> { "8001 应成为剪切方（当前相反）" })
            : new CutOrderResult(pairs, pairs, false, new List<string>());
    }
}

internal sealed class FakeViewInteractionService : IViewInteractionService
{
    public SelectionResult SelectElements(object uiDocument, IReadOnlyList<int> elementIds, bool append, bool zoomToFit)
    {
        if (uiDocument is null)
        {
            throw new InvalidOperationException("当前没有可用的 UIDocument。");
        }

        return new SelectionResult(elementIds.Count, 0, zoomToFit, $"已选择 {elementIds.Count} 个元素" + (zoomToFit ? "，并缩放到该选择集" : string.Empty));
    }

    public SelectionResult ClearSelection(object uiDocument)
        => new(0, 3, false, "已清空选择（原有 3 个）");

    public SelectionResult ZoomToElements(object uiDocument, IReadOnlyList<int> elementIds)
    {
        if (elementIds.Count == 0)
        {
            throw new InvalidOperationException("必须提供 elementIds。");
        }

        return new SelectionResult(elementIds.Count, 0, true, $"已缩放到 {elementIds.Count} 个元素");
    }

    public SelectionResult ActivateView(object uiDocument, int viewId)
        => viewId == 9999
            ? throw new InvalidOperationException($"未找到视图 {viewId}。")
            : new SelectionResult(0, 0, false, $"已激活视图 标高 1（{viewId}）");
}

// ---- 收尾批：楼梯 / 屋顶 / 卫星图 ----

internal sealed class FakeStairService : IStairService
{
    public StairResult CreateStair(
        object document,
        double totalHeightMm,
        int runCount,
        double? treadDepthMm,
        double? runWidthMm,
        double? wellWidthMm,
        double? landingWidthMm,
        double insertPointX,
        double insertPointY)
        => totalHeightMm <= 0 || runCount <= 0
            ? throw new InvalidOperationException("totalHeightMm 与 runCount 必须为正数。")
            : new StairResult(9900, 9901, runCount, totalHeightMm, "Fake.StairsEditScope.StartNewStairs + StairsRun.CreateStraightRun");

    public StairTypeResult CreateStairType(
        object document,
        string newStairTypeName,
        string? baseStairTypeName,
        double? runThicknessMm,
        double? landingThicknessMm)
        => string.IsNullOrWhiteSpace(newStairTypeName)
            ? throw new InvalidOperationException("必须提供 newStairTypeName。")
            : new StairTypeResult(9902, newStairTypeName, runThicknessMm, landingThicknessMm, new List<string> { "梯段厚度已设为 200 mm" });
}

internal sealed class FakeRoofEditService : IRoofEditService
{
    public RoofSlopeResult ModifyRoofSlope(
        object document,
        int roofId,
        int? edgeIndex,
        bool definesSlope,
        double? slopeAngleDegrees,
        bool applyToAllEdges)
        => roofId == 9999
            ? throw new InvalidOperationException($"元素 {roofId} 不是足迹屋顶（FootPrintRoof）。")
            : new RoofSlopeResult(
                roofId,
                applyToAllEdges || edgeIndex is null ? 4 : 1,
                applyToAllEdges || edgeIndex is null,
                slopeAngleDegrees,
                new List<string>());
}

internal sealed class FakeSatelliteImportService : ISatelliteImportService
{
    public SatelliteImportResult Import(
        object document,
        string? imagePath,
        string? locationName,
        double? latitude,
        double? longitude,
        string? mapSource,
        int? zoomLevel,
        double? radiusKm)
        => string.IsNullOrWhiteSpace(imagePath)
            ? new SatelliteImportResult(
                null,
                "needs-image",
                "Revit API 不提供网络能力，无法直接下载卫星图瓦片；请先准备本地栅格图片。",
                new List<string> { $"location={locationName ?? "(未提供)"}" })
            : new SatelliteImportResult(
                9950,
                "imported",
                "已把本地栅格图导入为图像实例 9950",
                new List<string> { $"location={locationName ?? "(未提供)"}", "lat=31.2", "lon=121.5" });
}

/// <summary>工具契约的冒烟断言入口（供 Program 调用）。</summary>
internal static class ContractFacts
{
    public static IReadOnlyList<ToolContract> ExpectedCatalog => ToolContracts.ToolContracts.All;
}
