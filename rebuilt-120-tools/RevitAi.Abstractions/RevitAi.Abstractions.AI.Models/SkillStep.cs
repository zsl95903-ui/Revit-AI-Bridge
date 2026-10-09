namespace RevitAi.Abstractions.AI.Models;

public sealed class SkillStep
{
	public int Order { get; set; }

	public string Tool { get; set; } = string.Empty;

	public object? Parameters { get; set; }

	public string? Description { get; set; }
}
