using Autodesk.Revit.UI;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;
using RevitAi.Engine.Revit.Services;

namespace RevitAi.Engine.Revit;

/// <summary>
/// Revit 适配器：把当前文档与各领域服务暴露给注册表/工具。
/// 厂商版暴露 21 个服务属性、由注册表按类型 switch 注入（AIToolRegistry.cs:311-413）；本 PoC 保留同机制、只挂 4 个。
/// </summary>
public sealed class RevitAdapter : IRevitAdapter
{
    private readonly UIApplication _uiApplication;

    public RevitAdapter(UIApplication uiApplication)
    {
        _uiApplication = uiApplication ?? throw new ArgumentNullException(nameof(uiApplication));
        LevelService = new LevelService();
        ElementService = new ElementService();
        ModificationService = new ModificationService();
        GridService = new GridService();
        ViewService = new RevitViewService();
        CategoryService = new CategoryService();
        WorksetService = new WorksetService();
        FamilyQueryService = new FamilyQueryService();
        RoomService = new RoomService();
        TagService = new TagService();
        ElementQueryService = new ElementQueryService();
        ProjectUnitService = new ProjectUnitService();
        ViewQueryService = new ViewQueryService();
        TypeService = new TypeService();
        PhaseService = new PhaseService();
        ElementWorksetService = new ElementWorksetService();
        LinkService = new LinkService();
        RoofService = new RoofService();
        FilterParameterService = new FilterParameterService();
        ViewWriteService = new ViewWriteService();
        ViewCreationService = new ViewCreationService();
        FilterService = new FilterService();
        FilterCreationService = new FilterCreationService();
        TagCreationService = new TagCreationService();
        ScheduleService = new ScheduleService();
        ExportService = new ExportService();
        DimensionCreationService = new DimensionCreationService();
        ScheduleEditService = new ScheduleEditService();
        ParameterService = new ParameterService();
        GlobalParameterService = new GlobalParameterService();
        MaterialService = new MaterialService();
        CompoundStructureService = new CompoundStructureService();
        FamilyTypeService = new FamilyTypeService();
        ElementAdminService = new ElementAdminService();
        ElementColorService = new ElementColorService();
        SharedParameterService = new SharedParameterService();
        ModelingService = new ModelingService();
        MepModelingService = new MepModelingService();
        ShapeModelingService = new ShapeModelingService();
        ElementOpsService = new ElementOpsService();
        AdvancedQueryService = new AdvancedQueryService();
        ProjectSettingsService = new ProjectSettingsService();
        TopographyService = new TopographyService();
        InsulationService = new InsulationService();
        MepSystemService = new MepSystemService();
        DomainExtraService = new DomainExtraService();
        ViewInteractionService = new ViewInteractionService();
        StairService = new StairService();
        RoofEditService = new RoofEditService();
        SatelliteImportService = new SatelliteImportService();
    }

    public ILevelService LevelService { get; }

    public IElementService ElementService { get; }

    public IModificationService ModificationService { get; }

    public IGridService GridService { get; }

    public IViewService ViewService { get; }

    public ICategoryService CategoryService { get; }

    public IWorksetService WorksetService { get; }

    public IFamilyQueryService FamilyQueryService { get; }

    public IRoomService RoomService { get; }

    public ITagService TagService { get; }

    public IElementQueryService ElementQueryService { get; }

    public IProjectUnitService ProjectUnitService { get; }

    public IPhaseService PhaseService { get; }

    public IElementWorksetService ElementWorksetService { get; }

    public ILinkService LinkService { get; }

    public IRoofService RoofService { get; }

    public IFilterParameterService FilterParameterService { get; }

    public IViewQueryService ViewQueryService { get; }

    public ITypeService TypeService { get; }

    public IViewWriteService ViewWriteService { get; }

    public IViewCreationService ViewCreationService { get; }

    public IFilterService FilterService { get; }

    public IFilterCreationService FilterCreationService { get; }

    public ITagCreationService TagCreationService { get; }

    public IScheduleService ScheduleService { get; }

    public IExportService ExportService { get; }

    public IDimensionCreationService DimensionCreationService { get; }

    public IScheduleEditService ScheduleEditService { get; }

    public IParameterService ParameterService { get; }

    public IGlobalParameterService GlobalParameterService { get; }

    public IMaterialService MaterialService { get; }

    public ICompoundStructureService CompoundStructureService { get; }

    public IFamilyTypeService FamilyTypeService { get; }

    public IElementAdminService ElementAdminService { get; }

    public IElementColorService ElementColorService { get; }

    public ISharedParameterService SharedParameterService { get; }

    public IModelingService ModelingService { get; }

    public IMepModelingService MepModelingService { get; }

    public IShapeModelingService ShapeModelingService { get; }

    public IElementOpsService ElementOpsService { get; }

    public IAdvancedQueryService AdvancedQueryService { get; }

    public IProjectSettingsService ProjectSettingsService { get; }

    public ITopographyService TopographyService { get; }

    public IInsulationService InsulationService { get; }

    public IMepSystemService MepSystemService { get; }

    public IDomainExtraService DomainExtraService { get; }

    public IViewInteractionService ViewInteractionService { get; }

    public IStairService StairService { get; }

    public IRoofEditService RoofEditService { get; }

    public ISatelliteImportService SatelliteImportService { get; }

    public object? GetActiveDocument() => _uiApplication.ActiveUIDocument?.Document;

    public object? GetActiveUiDocument() => _uiApplication.ActiveUIDocument;

}
