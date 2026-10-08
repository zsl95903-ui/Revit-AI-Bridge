namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 3 第四刀：共享参数文件 + 项目参数绑定。
// ---------------------------------------------------------------------------------------------

public sealed record SharedParameterFileStatus(bool Available, string Message);

public sealed record SharedParameterGroupInfo(string Name, int DefinitionCount, IReadOnlyList<string> ParameterNames);

public sealed record ProjectParameterInfo(
    string Name,
    string BindingType,
    bool IsShared,
    IReadOnlyList<string> CategoryNames);

public interface ISharedParameterService
{
    /// <summary>共享参数文件是否可用（Application.OpenSharedParameterFile 为 null 时不可用）。</summary>
    SharedParameterFileStatus GetFileStatus(object document);

    IReadOnlyList<SharedParameterGroupInfo> ListSharedParameterGroups(object document);

    /// <summary>在共享参数文件里新建参数定义（组不存在会自动创建）。</summary>
    string CreateSharedParameter(object document, string parameterName, string? parameterType, string groupName);

    IReadOnlyList<ProjectParameterInfo> ListProjectParameters(object document);

    /// <summary>把共享参数绑定到类别上（新建项目参数）。</summary>
    int CreateProjectParameter(
        object document,
        string parameterName,
        string? sharedParameterName,
        string? bindingType,
        IReadOnlyList<string> categoryNames,
        bool instanceParameter);

    bool DeleteProjectParameter(object document, string parameterName);

    /// <summary>给已有项目参数增删类别，返回变更后的类别数量。</summary>
    int UpdateProjectParameterCategories(object document, string parameterName, IReadOnlyList<string> categoryNames, bool add);
}
