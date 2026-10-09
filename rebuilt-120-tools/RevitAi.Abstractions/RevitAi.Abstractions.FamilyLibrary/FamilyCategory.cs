using System;
using Newtonsoft.Json;

namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyCategory
{
	[JsonProperty("id")]
	public Guid Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; } = string.Empty;

	[JsonProperty("parent_id")]
	public Guid? ParentId { get; set; }

	[JsonProperty("display_order")]
	public int DisplayOrder { get; set; }

	[JsonProperty("icon_name")]
	public string? IconName { get; set; }

	[JsonProperty("description")]
	public string? Description { get; set; }
}
