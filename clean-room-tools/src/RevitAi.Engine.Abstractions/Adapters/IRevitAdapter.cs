using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Abstractions.Adapters;

/// <summary>
/// Revit 适配器。厂商版暴露 21 个服务只读属性、由注册表按类型 switch 注入（AIToolRegistry.cs:311-413）；
/// 本引擎保持同样的注入机制，按批次逐步补齐服务属性。
/// </summary>
public interface IRevitAdapter
{
    object? GetActiveDocument();

    ILevelService? LevelService { get; }

    IElementService? ElementService { get; }

    IModificationService? ModificationService { get; }

    // ---- 批次 1：只读查询 ----
    IGridService? GridService { get; }

    IViewService? ViewService { get; }

    ICategoryService? CategoryService { get; }

    IWorksetService? WorksetService { get; }

    // ---- 批次 1 第二批：只读查询 ----
    IFamilyQueryService? FamilyQueryService { get; }

    IRoomService? RoomService { get; }

    ITagService? TagService { get; }

    IElementQueryService? ElementQueryService { get; }

    IProjectUnitService? ProjectUnitService { get; }

    IViewQueryService? ViewQueryService { get; }

    ITypeService? TypeService { get; }

    // ---- 批次 1 第三批：只读查询 ----
    IPhaseService? PhaseService { get; }

    IElementWorksetService? ElementWorksetService { get; }

    ILinkService? LinkService { get; }

    IRoofService? RoofService { get; }

    IFilterParameterService? FilterParameterService { get; }

    // ---- 批次 2：视图与标注（写入） ----
    IViewWriteService? ViewWriteService { get; }

    IViewCreationService? ViewCreationService { get; }

    IFilterService? FilterService { get; }

    // ---- 批次 2 剩余：过滤器创建 / 标记 / 明细表 / 导出 ----
    IFilterCreationService? FilterCreationService { get; }

    ITagCreationService? TagCreationService { get; }

    IScheduleService? ScheduleService { get; }

    IExportService? ExportService { get; }

    // ---- 批次 2 收尾：点标注 / 明细表编辑 ----
    IDimensionCreationService? DimensionCreationService { get; }

    IScheduleEditService? ScheduleEditService { get; }

    // ---- 批次 3：参数 / 全局参数 ----
    IParameterService? ParameterService { get; }

    IGlobalParameterService? GlobalParameterService { get; }

    // ---- 批次 3 第二刀：材料 / 复合层 ----
    IMaterialService? MaterialService { get; }

    ICompoundStructureService? CompoundStructureService { get; }

    // ---- 批次 3 第三刀：族类型 / 元素管理 / 元素着色 ----
    IFamilyTypeService? FamilyTypeService { get; }

    IElementAdminService? ElementAdminService { get; }

    IElementColorService? ElementColorService { get; }

    // ---- 批次 3 第四刀：共享参数 / 项目参数 ----
    ISharedParameterService? SharedParameterService { get; }

    // ---- 批次 4 第一刀：建模原语 ----
    IModelingService? ModelingService { get; }

    // ---- 批次 4 第二刀：MEP / 形体 ----
    IMepModelingService? MepModelingService { get; }

    IShapeModelingService? ShapeModelingService { get; }

    // ---- 批次 4 第三刀：元素操作 ----
    IElementOpsService? ElementOpsService { get; }

    // ---- 查询与交互 ----
    IAdvancedQueryService? AdvancedQueryService { get; }

    IProjectSettingsService? ProjectSettingsService { get; }

    // ---- 批次 5 第一批：地形 / 保温 / MEP 系统 ----
    ITopographyService? TopographyService { get; }

    IInsulationService? InsulationService { get; }

    IMepSystemService? MepSystemService { get; }

    // ---- 收尾批：道路 / CAD / 剪切顺序 + UI 交互 ----
    IDomainExtraService? DomainExtraService { get; }

    IViewInteractionService? ViewInteractionService { get; }

    // ---- 收尾批：楼梯 / 屋顶坡度 / 卫星图 ----
    IStairService? StairService { get; }

    IRoofEditService? RoofEditService { get; }

    ISatelliteImportService? SatelliteImportService { get; }

    /// <summary>UI 会话对象（UIDocument）；无界面会话时为 null。</summary>
    object? GetActiveUiDocument();
}
