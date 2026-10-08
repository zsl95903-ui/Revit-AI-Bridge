using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 标高服务实现（Revit API）。方法粒度对齐厂商 ILevelService：
/// GetAllLevels / GetLevelId / GetLevelName / GetLevelElevation，另补创建与更新。
/// 逻辑与自研外壳 RevitAiBatch\ToolDispatcher.cs（CreateLevel/UpsertLevel/TrySetBuildingStory）保持一致。
/// </summary>
internal sealed class LevelService : ILevelService
{
    public IReadOnlyList<object> GetAllLevels(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<object>();
        }

        return new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .Cast<object>()
            .ToArray();
    }

    public int GetLevelId(object level) => (int)((Level)level).Id.Value;

    public string GetLevelName(object level) => ((Level)level).Name;

    public double GetLevelElevationFeet(object level) => ((Level)level).Elevation;

    public object? FindLevel(object document, string name, double elevationFeet, double toleranceFeet)
    {
        if (document is not Document doc)
        {
            return null;
        }

        var levels = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .ToArray();

        return levels.FirstOrDefault(level => string.Equals(level.Name, name, StringComparison.OrdinalIgnoreCase))
            ?? levels.FirstOrDefault(level => Math.Abs(level.Elevation - elevationFeet) <= toleranceFeet);
    }

    public object CreateLevel(object document, string name, double elevationFeet, bool buildingStory)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var level = Level.Create(doc, elevationFeet);
        try
        {
            level.Name = name;
        }
        catch (Exception)
        {
            // 名称冲突等情况下保留已创建的标高。
        }

        SetBuildingStory(level, buildingStory);
        return level;
    }

    public void SetLevelElevation(object level, double elevationFeet) => ((Level)level).Elevation = elevationFeet;

    public void SetLevelName(object level, string name)
    {
        try
        {
            ((Level)level).Name = name;
        }
        catch (Exception)
        {
            // 名称非法/冲突时忽略，保持原名称。
        }
    }

    public void SetBuildingStory(object level, bool isBuildingStory)
    {
        var parameter = ((Level)level).get_Parameter(BuiltInParameter.LEVEL_IS_BUILDING_STORY);
        if (parameter is not null && !parameter.IsReadOnly)
        {
            parameter.Set(isBuildingStory ? 1 : 0);
        }
    }
}
