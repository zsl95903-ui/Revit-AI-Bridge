using System.Collections.Generic;
using Newtonsoft.Json;

namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyTypeParameterGroup
{
	[JsonProperty("typeName")]
	public string TypeName { get; set; } = string.Empty;

	[JsonProperty("parameters")]
	public List<FamilyParameter> Parameters { get; set; } = new List<FamilyParameter>();
}
