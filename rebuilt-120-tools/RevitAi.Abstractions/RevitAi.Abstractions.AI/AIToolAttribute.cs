using System;

namespace RevitAi.Abstractions.AI;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AIToolAttribute : Attribute
{
	public string Name { get; }

	public string Category { get; set; } = "通用";

	public string Description { get; set; } = string.Empty;

	public bool RequiresTransaction { get; set; }

	public bool RequiresModification { get; set; }

	public bool RequiresActiveDocument { get; set; } = true;

	public AIToolAttribute(string name)
	{
		Name = name;
	}
}
