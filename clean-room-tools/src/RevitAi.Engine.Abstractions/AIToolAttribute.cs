namespace RevitAi.Engine.Abstractions;

/// <summary>
/// 工具声明特性。字段与厂商 AS.Tools 的 AIToolAttribute 对齐（6 个字段，无工具集、无 dry_run），
/// 参考：反编译重建源码 Abstractions\AS\Tools\Abstractions\AI\AIToolAttribute.cs。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class AIToolAttribute : Attribute
{
    public AIToolAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("工具名称不能为空。", nameof(name));
        }

        Name = name;
    }

    /// <summary>唯一键，同时也是下发给模型的 function name。</summary>
    public string Name { get; }

    /// <summary>分类，仅用于分组/摘要。</summary>
    public string Category { get; set; } = "通用";

    /// <summary>给模型看的描述。</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>是否需要宿主开事务包裹执行。</summary>
    public bool RequiresTransaction { get; set; }

    /// <summary>是否必须回到 Revit 主线程执行。</summary>
    public bool RequiresModification { get; set; }

    /// <summary>是否需要活动文档（参与路由判定：为真时也要回主线程）。</summary>
    public bool RequiresActiveDocument { get; set; } = true;
}
