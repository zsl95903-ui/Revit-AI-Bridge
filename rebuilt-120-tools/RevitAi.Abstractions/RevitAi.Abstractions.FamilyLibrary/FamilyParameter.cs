using System;
using Newtonsoft.Json;

namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyParameter
{
	[JsonProperty("id")]
	public Guid Id { get; set; }

	[JsonProperty("familyId")]
	public Guid FamilyId { get; set; }

	[JsonProperty("parameterName")]
	public string ParameterName { get; set; } = string.Empty;

	[JsonProperty("parameterType")]
	public string ParameterType { get; set; } = string.Empty;

	[JsonProperty("parameterValueType")]
	public string ParameterValueType { get; set; } = string.Empty;

	[JsonProperty("defaultValue")]
	public string? DefaultValue { get; set; }

	[JsonProperty("isShared")]
	public bool IsShared { get; set; }

	[JsonProperty("isInstance")]
	public bool IsInstance { get; set; }

	[JsonProperty("isType")]
	public bool IsType { get; set; } = true;

	[JsonProperty("typeName")]
	public string TypeName { get; set; } = string.Empty;

	[JsonProperty("description")]
	public string? Description { get; set; }

	[JsonProperty("displayOrder")]
	public int DisplayOrder { get; set; }
}
