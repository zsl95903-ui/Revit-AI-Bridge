using Newtonsoft.Json;

namespace RevitAi.Abstractions.FamilyLibrary;

public class RevitCategoryItem
{
	[JsonProperty("revit_category_name")]
	public string RevitCategoryName { get; set; } = string.Empty;

	[JsonProperty("family_count")]
	public int FamilyCount { get; set; }
}
