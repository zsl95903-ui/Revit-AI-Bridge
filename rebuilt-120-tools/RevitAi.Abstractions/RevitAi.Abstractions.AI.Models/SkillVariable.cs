namespace RevitAi.Abstractions.AI.Models;

public sealed class SkillVariable
{
	public string Name { get; set; } = string.Empty;

	public string Type { get; set; } = "string";

	public string? Description { get; set; }

	public bool Required { get; set; }

	public object? DefaultValue { get; set; }
}
