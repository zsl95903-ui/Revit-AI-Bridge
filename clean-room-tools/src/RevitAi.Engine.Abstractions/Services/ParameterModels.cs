namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 3 第一刀：参数查询与写入、全局参数管理。
// ---------------------------------------------------------------------------------------------

public sealed record ParameterInfo(
    int ElementId,
    int ParameterId,
    string Name,
    string StorageType,
    string Value,
    bool IsReadOnly,
    bool IsShared,
    bool IsTypeParameter,
    string Group);

/// <summary>一次参数赋值：按名称或 id 指定参数。</summary>
public sealed record ParameterAssignment(
    int ElementId,
    string? ParameterName,
    int? ParameterId,
    string? Value,
    string? Unit,
    bool IsTypeParameter = false);

public sealed record ParameterSetResult(int Requested, int Applied, IReadOnlyList<string> Failures);

public sealed record GlobalParameterInfo(
    int ParameterId,
    string Name,
    string Value,
    string Formula,
    string ParameterType);

public interface IParameterService
{
    IReadOnlyList<ParameterInfo> QueryParameters(
        object document,
        int elementId,
        IReadOnlyList<string> names,
        bool includeTypeParameters);

    ParameterSetResult SetParameterValues(object document, IReadOnlyList<ParameterAssignment> assignments);
}

public interface IGlobalParameterService
{
    IReadOnlyList<GlobalParameterInfo> List(object document);

    GlobalParameterInfo Create(object document, string name, string? parameterType, string? formula, string? value);

    GlobalParameterInfo? SetValue(object document, string name, string? value, string? formula);

    bool Delete(object document, string name);
}
