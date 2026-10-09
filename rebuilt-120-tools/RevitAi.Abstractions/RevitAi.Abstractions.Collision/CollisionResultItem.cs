using System;

namespace RevitAi.Abstractions.Collision;

public class CollisionResultItem
{
	public int SourceElementId { get; set; }

	public string? SourceCategoryName { get; set; }

	public string? SourceTypeName { get; set; }

	public string? SourceFamilyName { get; set; }

	public int TargetElementId { get; set; }

	public string? TargetCategoryName { get; set; }

	public string? TargetTypeName { get; set; }

	public string? TargetFamilyName { get; set; }

	public bool HasCollision { get; set; }

	public DateTime CheckTime { get; set; } = DateTime.UtcNow;
}
