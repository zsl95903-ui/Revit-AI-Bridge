using System.Runtime.Serialization;

namespace RevitAi.Abstractions.Services;

public enum CodeSnippetCategory
{
	[EnumMember(Value = "modeling")]
	Modeling,
	[EnumMember(Value = "annotation")]
	Annotation,
	[EnumMember(Value = "analysis")]
	Analysis,
	[EnumMember(Value = "view")]
	View,
	[EnumMember(Value = "documentation")]
	Documentation,
	[EnumMember(Value = "utility")]
	Utility,
	[EnumMember(Value = "other")]
	Other
}
