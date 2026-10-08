using Autodesk.Revit.DB;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 共享参数 + 项目参数服务。
/// API 事实（反射核对 27.3）：
///   * Application.OpenSharedParameterFile() → DefinitionFile（未配置时返回 null）；
///   * file.Groups.Create(name) / file.Groups[name]；group.Definitions.Create(ExternalDefinitionCreationOptions)；
///   * doc.ParameterBindings（BindingMap）：Insert/ReInsert/Remove/Contains/ForwardIterator；
///   * TypeBinding(CategorySet) / InstanceBinding(CategorySet)，类别集合是 CategorySet（Insert/Erase/Contains）。
/// 限制（如实告知，不静默失败）：Revit API 不支持从共享参数文件里删除定义，需要手工编辑该 txt 文件。
/// </summary>
internal sealed class SharedParameterService : ISharedParameterService
{
    public SharedParameterFileStatus GetFileStatus(object document)
    {
        if (document is not Document doc)
        {
            return new SharedParameterFileStatus(false, "文档对象无效。");
        }

        var file = doc.Application.OpenSharedParameterFile();
        return file is null
            ? new SharedParameterFileStatus(false, "当前没有配置共享参数文件：请在 Revit 里「管理 → 共享参数 → 创建」后重试。")
            : new SharedParameterFileStatus(true, "共享参数文件已加载。");
    }

    public IReadOnlyList<SharedParameterGroupInfo> ListSharedParameterGroups(object document)
    {
        var file = RequireFile(document);
        var result = new List<SharedParameterGroupInfo>();

        foreach (DefinitionGroup group in file.Groups)
        {
            var names = new List<string>();
            foreach (Definition definition in group.Definitions)
            {
                names.Add(definition.Name);
            }

            result.Add(new SharedParameterGroupInfo(group.Name, names.Count, names));
        }

        return result;
    }

    public string CreateSharedParameter(object document, string parameterName, string? parameterType, string groupName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new InvalidOperationException("必须提供 parameterName。");
        }

        var file = RequireFile(document);
        var safeGroupName = string.IsNullOrWhiteSpace(groupName) ? "自定义参数" : groupName!;

        // 注意：DefinitionGroups 没有 C# 可见的索引器（反射里是 Item/get_Item 但未标 DefaultMember），
        // 因此这里用遍历按名字找组，找不到再 Create。
        DefinitionGroup? group = null;
        foreach (DefinitionGroup candidate in file.Groups)
        {
            if (string.Equals(candidate.Name, safeGroupName, StringComparison.OrdinalIgnoreCase))
            {
                group = candidate;
                break;
            }
        }

        group ??= file.Groups.Create(safeGroupName);
        if (group.Definitions.get_Item(parameterName) is not null)
        {
            return parameterName;
        }

        var options = new ExternalDefinitionCreationOptions(parameterName, ResolveSpecTypeId(parameterType));
        group.Definitions.Create(options);
        return parameterName;
    }

    public IReadOnlyList<ProjectParameterInfo> ListProjectParameters(object document)
    {
        if (document is not Document doc)
        {
            return Array.Empty<ProjectParameterInfo>();
        }

        var result = new List<ProjectParameterInfo>();
        var iterator = doc.ParameterBindings.ForwardIterator();

        while (iterator.MoveNext())
        {
            if (iterator.Key is not { } definition)
            {
                continue;
            }

            var binding = doc.ParameterBindings.get_Item(definition);
            result.Add(Describe(definition, binding));
        }

        return result;
    }

    public int CreateProjectParameter(
        object document,
        string parameterName,
        string? sharedParameterName,
        string? bindingType,
        IReadOnlyList<string> categoryNames,
        bool instanceParameter)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new InvalidOperationException("必须提供 parameterName。");
        }

        if (categoryNames.Count == 0)
        {
            throw new InvalidOperationException("必须提供 categoryNames（至少一个类别）。");
        }

        var lookupName = string.IsNullOrWhiteSpace(sharedParameterName) ? parameterName : sharedParameterName!;
        var definition = FindSharedDefinition(doc, lookupName)
                         ?? throw new InvalidOperationException(
                             $"共享参数文件里找不到定义 {lookupName}。Revit 的项目参数必须来自共享参数文件，"
                             + "请先用 manage_shared_parameters create 创建定义。");

        if (doc.ParameterBindings.Contains(definition))
        {
            throw new InvalidOperationException($"项目参数 {lookupName} 已经绑定；如需改类别请用 add_categories/remove_categories。");
        }

        var categories = ResolveCategories(doc, categoryNames);
        var useInstance = instanceParameter ||
                          string.Equals(bindingType, "instance", StringComparison.OrdinalIgnoreCase);

        Binding binding = useInstance
            ? new InstanceBinding(categories)
            : new TypeBinding(categories);

        if (!doc.ParameterBindings.Insert(definition, binding))
        {
            throw new InvalidOperationException($"项目参数 {lookupName} 绑定失败（Revit 拒绝 Insert）。");
        }

        return categories.Size;
    }

    public bool DeleteProjectParameter(object document, string parameterName)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var definition = FindBoundDefinition(doc, parameterName);
        if (definition is null)
        {
            return false;
        }

        return doc.ParameterBindings.Remove(definition);
    }

    public int UpdateProjectParameterCategories(object document, string parameterName, IReadOnlyList<string> categoryNames, bool add)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        var definition = FindBoundDefinition(doc, parameterName)
                         ?? throw new InvalidOperationException($"未找到项目参数 {parameterName}。");

        var binding = doc.ParameterBindings.get_Item(definition) as ElementBinding
                      ?? throw new InvalidOperationException($"项目参数 {parameterName} 的绑定类型不支持改类别。");

        var current = binding.Categories;
        foreach (var categoryName in categoryNames)
        {
            var category = FindCategory(doc, categoryName)
                           ?? throw new InvalidOperationException($"未找到类别 {categoryName}。");

            if (add)
            {
                if (!current.Contains(category))
                {
                    current.Insert(category);
                }
            }
            else if (current.Contains(category))
            {
                current.Erase(category);
            }
        }

        binding.Categories = current;
        if (!doc.ParameterBindings.ReInsert(definition, binding))
        {
            throw new InvalidOperationException($"项目参数 {parameterName} 的类别更新被 Revit 拒绝。");
        }

        return current.Size;
    }

    private static DefinitionFile RequireFile(object document)
    {
        if (document is not Document doc)
        {
            throw new InvalidOperationException("文档对象无效。");
        }

        return doc.Application.OpenSharedParameterFile()
               ?? throw new InvalidOperationException(
                   "当前没有配置共享参数文件：请在 Revit 里「管理 → 共享参数 → 创建」后重试。");
    }

    private static ProjectParameterInfo Describe(Definition definition, Binding binding)
        => new(
            definition.Name,
            binding switch
            {
                InstanceBinding => "instance",
                TypeBinding => "type",
                _ => binding.GetType().Name,
            },
            definition is ExternalDefinition,
            (binding as ElementBinding) is { } elementBinding
                ? elementBinding.Categories.Cast<Category>().Select(category => category.Name).ToArray()
                : Array.Empty<string>());

    private static CategorySet ResolveCategories(Document document, IReadOnlyList<string> categoryNames)
    {
        var set = new CategorySet();
        foreach (var categoryName in categoryNames)
        {
            var category = FindCategory(document, categoryName)
                           ?? throw new InvalidOperationException($"未找到类别 {categoryName}。");
            set.Insert(category);
        }

        return set;
    }

    private static Category? FindCategory(Document document, string categoryName)
    {
        foreach (Category category in document.Settings.Categories)
        {
            if (string.Equals(category.Name, categoryName, StringComparison.OrdinalIgnoreCase))
            {
                return category;
            }
        }

        return null;
    }

    private static Definition? FindSharedDefinition(Document document, string parameterName)
    {
        var file = document.Application.OpenSharedParameterFile();
        if (file is null)
        {
            return null;
        }

        foreach (DefinitionGroup group in file.Groups)
        {
            if (group.Definitions.get_Item(parameterName) is { } definition)
            {
                return definition;
            }
        }

        return null;
    }

    private static Definition? FindBoundDefinition(Document document, string parameterName)
    {
        var iterator = document.ParameterBindings.ForwardIterator();
        while (iterator.MoveNext())
        {
            if (iterator.Key is { } definition &&
                string.Equals(definition.Name, parameterName, StringComparison.OrdinalIgnoreCase))
            {
                return definition;
            }
        }

        return null;
    }

    private static ForgeTypeId ResolveSpecTypeId(string? parameterType)
    {
        return (parameterType ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "length" or "长度" => SpecTypeId.Length,
            "angle" or "角度" => SpecTypeId.Angle,
            "integer" or "int" or "整数" => SpecTypeId.Int.Integer,
            "text" or "string" or "文本" => SpecTypeId.String.Text,
            "yesno" or "boolean" or "是/否" => SpecTypeId.Boolean.YesNo,
            "area" or "面积" => SpecTypeId.Area,
            "volume" or "体积" => SpecTypeId.Volume,
            _ => SpecTypeId.Number,
        };
    }
}
