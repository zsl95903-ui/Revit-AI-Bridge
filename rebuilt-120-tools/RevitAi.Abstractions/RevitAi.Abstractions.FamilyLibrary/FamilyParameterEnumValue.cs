using System;

namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyParameterEnumValue
{
	public Guid Id { get; set; }

	public Guid ParameterId { get; set; }

	public string Value { get; set; } = string.Empty;

	public int DisplayOrder { get; set; }
}
