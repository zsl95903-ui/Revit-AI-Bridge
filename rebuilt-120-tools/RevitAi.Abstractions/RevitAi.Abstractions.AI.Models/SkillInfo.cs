using System.Collections.Generic;

namespace RevitAi.Abstractions.AI.Models;

public sealed class SkillInfo
{
	public string SkillName { get; set; } = string.Empty;

	public string DisplayName { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string Category { get; set; } = "通用";

	public int Priority { get; set; }

	public List<SkillStep> Steps { get; set; } = new List<SkillStep>();

	public List<SkillVariable> Variables { get; set; } = new List<SkillVariable>();
}
