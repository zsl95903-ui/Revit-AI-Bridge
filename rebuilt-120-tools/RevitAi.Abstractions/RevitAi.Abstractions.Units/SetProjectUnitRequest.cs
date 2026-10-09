using System;

namespace RevitAi.Abstractions.Units;

public sealed class SetProjectUnitRequest
{
	public object? Document { get; set; }

	public UnitType UnitType { get; set; }

	public object? DisplayUnitType { get; set; }

	public Action<bool>? OnCompleted { get; set; }
}
