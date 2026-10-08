using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Abstractions.Services;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 3 第一刀实现：参数查询、参数写入、全局参数管理。
/// </summary>
public static class ParameterImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("query_parameter_info", new QueryParameterInfoImplementation());
        ToolImplementationRegistry.Register("set_parameter_values", new SetParameterValuesImplementation());
        ToolImplementationRegistry.Register("manage_global_parameters", new ManageGlobalParametersImplementation());
    }
}

internal static class ParameterArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    /// <summary>从 elementId / elementIds / cacheId 解析元素 id 列表。</summary>
    public static List<int> ResolveElementIds(AIToolContext context)
    {
        var ids = WriteArgs.Ids(context, "elementIds", "element_ids", "elementId", "element_id");
        if (ids.Count > 0)
        {
            return ids;
        }

        var cacheId = WriteArgs.Text(context, "cacheId", "cache_id");
        if (string.IsNullOrWhiteSpace(cacheId))
        {
            return ids;
        }

        var cached = context.GetCachedData<List<Dictionary<string, object?>>>(cacheId);
        if (cached is not null)
        {
            ids.AddRange(ToolArgumentReader.ExtractIds(cached));
            return ids;
        }

        var cachedIds = context.GetCachedData<List<int>>(cacheId);
        if (cachedIds is not null)
        {
            ids.AddRange(cachedIds);
        }

        return ids;
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\QueryParameterInfoTool.cs（参数操作）
internal sealed class QueryParameterInfoImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ParameterService ?? throw new InvalidOperationException("无法获取 ParameterService");
        var document = ParameterArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "list").Trim().ToLowerInvariant();
        var ids = ParameterArgs.ResolveElementIds(context);
        if (ids.Count == 0)
        {
            return Task.FromResult(AIToolResult.Fail("必须提供 elementId、elementIds 或 cacheId。"));
        }

        var names = ToolArgumentReader.ReadStrings(
            new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase), "parameterNames");
        var singleName = WriteArgs.Text(context, "parameterName", "parameter_name");
        if (!string.IsNullOrWhiteSpace(singleName) && !names.Contains(singleName!, StringComparer.OrdinalIgnoreCase))
        {
            names.Add(singleName!);
        }

        var includeType = WriteArgs.Flag(context, false, "includeTypeParameters", "include_type_parameters")
                          || string.Equals(operation, "type", StringComparison.OrdinalIgnoreCase);

        var results = new List<object>();
        foreach (var id in ids)
        {
            var parameters = service.QueryParameters(document, id, names, includeType);
            results.Add(new
            {
                elementId = id,
                count = parameters.Count,
                parameters = parameters.Select(parameter => new
                {
                    parameterId = parameter.ParameterId,
                    name = parameter.Name,
                    storageType = parameter.StorageType,
                    value = parameter.Value,
                    isReadOnly = parameter.IsReadOnly,
                    isShared = parameter.IsShared,
                    isTypeParameter = parameter.IsTypeParameter,
                }).ToArray(),
            });
        }

        var total = results.Sum(_ => 0) + 0;
        var message = names.Count > 0
            ? $"✅ 已查询 {ids.Count} 个元素的参数（过滤：{string.Join('/', names)}）"
            : $"✅ 已查询 {ids.Count} 个元素的参数";

        return Task.FromResult(AIToolResult.Ok(message, new { operation, elements = results, elementCount = ids.Count, total }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\SetParameterValuesTool.cs（参数操作）
internal sealed class SetParameterValuesImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.ParameterService ?? throw new InvalidOperationException("无法获取 ParameterService");
        var document = ParameterArgs.RequireDocument(adapter, context);

        var assignments = new List<ParameterAssignment>();

        // 形式 1：elementParameterValues: [{ elementId, parameterName, value, unit }]
        if (context.Parameters.TryGetValue("elementParameterValues", out var bulk) && bulk is List<object?> items)
        {
            foreach (var item in items.OfType<Dictionary<string, object?>>())
            {
                var elementId = (int)ToolArgumentReader.ToDouble(item.TryGetValue("elementId", out var id) ? id : null);
                var parameterName = item.TryGetValue("parameterName", out var name) ? name?.ToString() : null;
                var value = item.TryGetValue("value", out var v) ? v?.ToString() : null;
                var unit = item.TryGetValue("unit", out var u) ? u?.ToString() : null;
                var isType = item.TryGetValue("isTypeParameter", out var t) && t is bool flag && flag;

                if (elementId > 0 && (!string.IsNullOrWhiteSpace(parameterName) || item.ContainsKey("parameterId")))
                {
                    assignments.Add(new ParameterAssignment(
                        elementId,
                        parameterName,
                        item.TryGetValue("parameterId", out var pid) && pid is not null ? (int)ToolArgumentReader.ToDouble(pid) : null,
                        value,
                        unit,
                        isType));
                }
            }
        }

        // 形式 2：elementId(s) + parameterName + value + unit
        if (assignments.Count == 0)
        {
            var ids = ParameterArgs.ResolveElementIds(context);
            var parameterName = WriteArgs.Text(context, "parameterName", "parameter_name");
            if (ids.Count == 0 || string.IsNullOrWhiteSpace(parameterName))
            {
                return Task.FromResult(AIToolResult.Fail(
                    "必须提供 elementParameterValues，或 elementId/elementIds/cacheId + parameterName + value。"));
            }

            var value = WriteArgs.Text(context, "value");
            var unit = WriteArgs.Text(context, "unit");
            var isType = WriteArgs.Flag(context, false, "isTypeParameter", "is_type_parameter");
            foreach (var id in ids)
            {
                assignments.Add(new ParameterAssignment(id, parameterName, null, value, unit, isType));
            }
        }

        var result = service.SetParameterValues(document, assignments);
        var message = result.Applied == result.Requested
            ? $"✅ 已设置 {result.Applied} 个参数值"
            : $"⚠️ 已设置 {result.Applied}/{result.Requested} 个参数值";

        return Task.FromResult(result.Applied > 0
            ? AIToolResult.Ok(message, new { requested = result.Requested, applied = result.Applied, failures = result.Failures })
            : AIToolResult.Fail($"{message}。失败原因：{string.Join("；", result.Failures)}", new { requested = result.Requested, applied = result.Applied, failures = result.Failures }));
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ManageGlobalParametersTool.cs（参数管理）
internal sealed class ManageGlobalParametersImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.GlobalParameterService ?? throw new InvalidOperationException("无法获取 GlobalParameterService");
        var document = ParameterArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "list").Trim().ToLowerInvariant();
        var parameterName = WriteArgs.Text(context, "parameterName", "parameter_name", "name");
        var parameterType = WriteArgs.Text(context, "parameterType", "parameter_type");
        var formula = WriteArgs.Text(context, "formula");
        var value = WriteArgs.Text(context, "value");

        switch (operation)
        {
            case "list":
            {
                var parameters = service.List(document);
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 全局参数共 {parameters.Count} 个",
                    new
                    {
                        parameters = parameters.Select(parameter => new
                        {
                            parameterId = parameter.ParameterId,
                            name = parameter.Name,
                            value = parameter.Value,
                            formula = parameter.Formula,
                            parameterType = parameter.ParameterType,
                        }).ToArray(),
                        count = parameters.Count,
                    }));
            }

            case "create":
            {
                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    return Task.FromResult(AIToolResult.Fail("create 需要 parameterName。"));
                }

                var created = service.Create(document, parameterName!, parameterType, formula, value);
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 已创建全局参数 {created.Name}",
                    new { parameterId = created.ParameterId, name = created.Name, value = created.Value, formula = created.Formula }));
            }

            case "set_value":
            case "set":
            {
                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    return Task.FromResult(AIToolResult.Fail("set_value 需要 parameterName。"));
                }

                var updated = service.SetValue(document, parameterName!, value, formula);
                return Task.FromResult(updated is null
                    ? AIToolResult.Fail($"未找到全局参数 {parameterName}。")
                    : AIToolResult.Ok($"✅ 已更新全局参数 {updated.Name} = {updated.Value}" + (string.IsNullOrWhiteSpace(updated.Formula) ? string.Empty : $"（公式 {updated.Formula}）"), new { parameterId = updated.ParameterId, name = updated.Name, value = updated.Value, formula = updated.Formula }));
            }

            case "delete":
            {
                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    return Task.FromResult(AIToolResult.Fail("delete 需要 parameterName。"));
                }

                var deleted = service.Delete(document, parameterName!);
                return Task.FromResult(deleted
                    ? AIToolResult.Ok($"✅ 已删除全局参数 {parameterName}", new { name = parameterName, deleted = true })
                    : AIToolResult.Fail($"未找到全局参数 {parameterName}。"));
            }

            default:
                return Task.FromResult(AIToolResult.Fail("operation 只支持 list / create / set_value / delete。"));
        }
    }
}
