using System;
using Newtonsoft.Json;

namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyLibraryItem
{
	[JsonProperty("id")]
	public Guid Id { get; set; }

	[JsonProperty("display_name")]
	public string DisplayName { get; set; } = string.Empty;

	[JsonProperty("category_id")]
	public Guid? CategoryId { get; set; }

	[JsonProperty("category_name")]
	public string? CategoryName { get; set; }

	[JsonProperty("revit_category_name")]
	public string? RevitCategoryName { get; set; }

	[JsonProperty("description")]
	public string? Description { get; set; }

	[JsonProperty("thumbnail_url")]
	public string? ThumbnailUrl { get; set; }

	[JsonProperty("file_size")]
	public long? FileSize { get; set; }

	[JsonProperty("revit_version")]
	public string? RevitVersion { get; set; }

	[JsonProperty("tags")]
	public string[]? Tags { get; set; }

	[JsonProperty("is_free")]
	public bool IsFree { get; set; }

	[JsonProperty("is_publicly_visible")]
	public bool IsPubliclyVisible { get; set; } = true;

	[JsonProperty("download_count")]
	public int DownloadCount { get; set; }

	[JsonProperty("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonProperty("key_params")]
	public FamilyKeyParams? KeyParams { get; set; }
}
