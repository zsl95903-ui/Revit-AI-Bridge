using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.AI.Models;

public sealed class SkillsConfig
{
	public string Version { get; set; } = "1.0.0";

	public DateTime UpdatedAt { get; set; }

	public List<SkillInfo> Skills { get; set; } = new List<SkillInfo>();
}
