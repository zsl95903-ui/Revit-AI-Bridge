namespace RevitAi.Engine.Abstractions.Services;

/// <summary>
/// 标高服务。方法粒度对齐厂商 ILevelService（GetAllLevels/GetLevelName/GetLevelElevation/GetLevelId/GetLevelByName），
/// 并按本 PoC 需要补齐创建与更新。实现见 RevitAi.Engine.Revit.Services.LevelService。
/// 说明：接口用 object 传递 Revit 对象，是为了让 Core / 工具的逻辑与 Revit 类型解耦（沿用厂商的分层方式）。
/// </summary>
public interface ILevelService
{
    IReadOnlyList<object> GetAllLevels(object document);

    int GetLevelId(object level);

    string GetLevelName(object level);

    /// <summary>返回英尺（Revit 内部单位）。</summary>
    double GetLevelElevationFeet(object level);

    object? FindLevel(object document, string name, double elevationFeet, double toleranceFeet);

    object CreateLevel(object document, string name, double elevationFeet, bool buildingStory);

    void SetLevelElevation(object level, double elevationFeet);

    void SetLevelName(object level, string name);

    void SetBuildingStory(object level, bool isBuildingStory);
}

/// <summary>元素服务（当前 PoC 只用到存在性校验）。</summary>
public interface IElementService
{
    bool Exists(object document, int elementId);
}

/// <summary>修改服务。位移单位为英尺。</summary>
public interface IModificationService
{
    int MoveElements(object document, IReadOnlyList<int> elementIds, double dxFeet, double dyFeet, double dzFeet);
}
