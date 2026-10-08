using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;

namespace RevitToolSet.Implementations;

/// <summary>
/// 批次 3 第四刀实现：共享参数文件管理、项目参数（绑定）管理。
/// </summary>
public static class ProjectParameterImplementations
{
    public static void RegisterAll()
    {
        ToolImplementationRegistry.Register("manage_shared_parameters", new ManageSharedParametersImplementation());
        ToolImplementationRegistry.Register("manage_project_parameters", new ManageProjectParametersImplementation());
    }
}

internal static class ProjectParameterArgs
{
    public static object RequireDocument(IRevitAdapter adapter, AIToolContext context)
        => context.Document ?? adapter.GetActiveDocument() ?? throw new InvalidOperationException("文档对象为空");

    public static List<string> Strings(AIToolContext context, params string[] keys)
    {
        var result = new List<string>();
        var parameters = new Dictionary<string, object?>(context.Parameters, StringComparer.OrdinalIgnoreCase);
        foreach (var key in keys)
        {
            result.AddRange(ToolArgumentReader.ReadStrings(parameters, key));
        }

        return result.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ManageSharedParametersTool.cs（参数管理）
internal sealed class ManageSharedParametersImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.SharedParameterService ?? throw new InvalidOperationException("无法获取 SharedParameterService");
        var document = ProjectParameterArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "list").Trim().ToLowerInvariant();
        var parameterName = WriteArgs.Text(context, "parameterName", "parameter_name", "name");
        var parameterType = WriteArgs.Text(context, "parameterType", "parameter_type");
        var groupName = WriteArgs.Text(context, "groupName", "group_name", "group");

        switch (operation)
        {
            case "status":
            {
                var status = service.GetFileStatus(document);
                return Task.FromResult(status.Available
                    ? AIToolResult.Ok($"✅ {status.Message}", new { available = true })
                    : AIToolResult.Fail(status.Message, new { available = false }));
            }

            case "list":
            {
                var groups = service.ListSharedParameterGroups(document);
                var total = groups.Sum(group => group.DefinitionCount);
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 共享参数共 {groups.Count} 组 / {total} 个定义",
                    new
                    {
                        groups = groups.Select(group => new
                        {
                            name = group.Name,
                            definitionCount = group.DefinitionCount,
                            parameterNames = group.ParameterNames,
                        }).ToArray(),
                        groupCount = groups.Count,
                        definitionCount = total,
                    }));
            }

            case "create":
            {
                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    return Task.FromResult(AIToolResult.Fail("create 需要 parameterName。"));
                }

                var created = service.CreateSharedParameter(document, parameterName!, parameterType, groupName ?? "自定义参数");
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 共享参数定义 {created} 已就绪（组 {groupName ?? "自定义参数"}）",
                    new
                    {
                        parameterName = created,
                        parameterType = parameterType ?? "Number",
                        groupName = groupName ?? "自定义参数",
                        note = "定义写入共享参数文件；还需用 manage_project_parameters create 绑定到类别后才生效。",
                    }));
            }

            case "delete":
            {
                // Revit API 没有"删除共享参数定义"的接口（DefinitionGroup/Definitions 只提供 Create）。
                return Task.FromResult(AIToolResult.Fail(
                    "Revit API 不支持从共享参数文件中删除定义（Definitions 只有 Create，没有 Remove）；"
                    + "请手工编辑共享参数 txt 文件，或改用 manage_project_parameters delete 解除绑定。"));
            }

            default:
                return Task.FromResult(AIToolResult.Fail("operation 只支持 status / list / create / delete。"));
        }
    }
}

// 反编译对照：Revit\AS\Tools\Revit\Net8\AI\Tools\ManageProjectParametersTool.cs（参数管理）
internal sealed class ManageProjectParametersImplementation : IToolImplementation
{
    public Task<AIToolResult> ExecuteAsync(IRevitAdapter adapter, AIToolContext context, CancellationToken cancellationToken = default)
    {
        var service = adapter.SharedParameterService ?? throw new InvalidOperationException("无法获取 SharedParameterService");
        var document = ProjectParameterArgs.RequireDocument(adapter, context);

        var operation = (WriteArgs.Text(context, "operation") ?? "list").Trim().ToLowerInvariant();
        var parameterName = WriteArgs.Text(context, "parameterName", "parameter_name", "name");
        var sharedParameterName = WriteArgs.Text(context, "sharedParameterName", "shared_parameter_name");
        var bindingType = WriteArgs.Text(context, "bindingType", "binding_type");
        var instanceParameter = WriteArgs.Flag(context, false, "instanceParameter", "instance_parameter");

        var categories = ProjectParameterArgs.Strings(context, "categoryNames", "category_names");
        var categoriesToAdd = ProjectParameterArgs.Strings(context, "categoriesToAdd", "categories_to_add");
        var categoriesToRemove = ProjectParameterArgs.Strings(context, "categoriesToRemove", "categories_to_remove");

        switch (operation)
        {
            case "list":
            {
                var parameters = service.ListProjectParameters(document);
                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 项目参数共 {parameters.Count} 个",
                    new
                    {
                        parameters = parameters.Select(parameter => new
                        {
                            name = parameter.Name,
                            bindingType = parameter.BindingType,
                            isShared = parameter.IsShared,
                            categoryNames = parameter.CategoryNames,
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

                var categoryCount = service.CreateProjectParameter(
                    document,
                    parameterName!,
                    sharedParameterName,
                    bindingType,
                    categories,
                    instanceParameter);

                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 项目参数 {parameterName} 已绑定到 {categoryCount} 个类别（{(instanceParameter || string.Equals(bindingType, "instance", StringComparison.OrdinalIgnoreCase) ? "实例" : "类型")}参数）",
                    new
                    {
                        parameterName,
                        sharedParameterName = sharedParameterName ?? parameterName,
                        bindingType = instanceParameter ? "instance" : bindingType ?? "type",
                        categoryCount,
                        parameterGroup = WriteArgs.Text(context, "parameterGroup", "parameter_group"),
                        instanceParameter,
                    }));
            }

            case "delete":
            {
                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    return Task.FromResult(AIToolResult.Fail("delete 需要 parameterName。"));
                }

                var deleted = service.DeleteProjectParameter(document, parameterName!);
                return Task.FromResult(deleted
                    ? AIToolResult.Ok($"✅ 已解除项目参数 {parameterName} 的绑定", new { parameterName, deleted = true })
                    : AIToolResult.Fail($"未找到项目参数 {parameterName}。"));
            }

            case "add_categories":
            case "remove_categories":
            {
                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    return Task.FromResult(AIToolResult.Fail($"{operation} 需要 parameterName。"));
                }

                var names = operation == "add_categories" ? categoriesToAdd : categoriesToRemove;
                if (names.Count == 0)
                {
                    return Task.FromResult(AIToolResult.Fail($"{operation} 需要 {(operation == "add_categories" ? "categoriesToAdd" : "categoriesToRemove")}。"));
                }

                var categoryCount = service.UpdateProjectParameterCategories(
                    document,
                    parameterName!,
                    names,
                    add: operation == "add_categories");

                return Task.FromResult(AIToolResult.Ok(
                    $"✅ 项目参数 {parameterName} 现在绑定 {categoryCount} 个类别（{(operation == "add_categories" ? "已添加" : "已移除")} {names.Count} 个）",
                    new { parameterName, categoryCount, changed = names, operation }));
            }

            default:
                return Task.FromResult(AIToolResult.Fail("operation 只支持 list / create / delete / add_categories / remove_categories。"));
        }
    }
}
