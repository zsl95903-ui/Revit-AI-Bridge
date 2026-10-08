namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 2 收尾契约：高程点/坐标点标注创建、明细表编辑。
// export_excel 不需要服务——它由工具直接调用 Core 里的 XlsxWriterLite 产出 xlsx。
// ---------------------------------------------------------------------------------------------

/// <summary>点标注类型：elevation（高程点）/ coordinate（坐标点）/ slope（坡度）。</summary>
public interface IDimensionCreationService
{
    int CreateSpotDimension(
        object document,
        string kind,
        int elementId,
        int? viewId,
        double xMm,
        double yMm,
        int? spotDimensionTypeId,
        bool hasLeader);
}

public interface IScheduleEditService
{
    /// <summary>按参数 id 增删字段，并按名称设置排序/分组（可选）。</summary>
    int EditSchedule(
        object document,
        int? scheduleId,
        string? scheduleName,
        IReadOnlyList<int> addFieldParameterIds,
        IReadOnlyList<int> removeFieldParameterIds,
        bool replaceFields,
        int? sortByParameterId,
        string? sortOrder,
        int? groupByParameterId);
}
