using System;

namespace RevitAi.UI.Services;

public class CachedFamilyInfo
{
	public Guid FamilyId { get; set; }

	public string DisplayName { get; set; } = string.Empty;

	public DateTime CacheTime { get; set; }
}
