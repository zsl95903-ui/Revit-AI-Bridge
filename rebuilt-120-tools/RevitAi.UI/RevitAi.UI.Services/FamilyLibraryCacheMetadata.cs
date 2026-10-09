using System;
using System.Collections.Generic;

namespace RevitAi.UI.Services;

public class FamilyLibraryCacheMetadata
{
	public DateTime? CategoriesCacheTime { get; set; }

	public DateTime? FamiliesCacheTime { get; set; }

	public Dictionary<string, CachedFamilyInfo> CachedFamilies { get; set; } = new Dictionary<string, CachedFamilyInfo>();
}
