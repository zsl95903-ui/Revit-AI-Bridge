using System;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class RoadProjectChangedEventArgs : EventArgs
{
	public RoadProjectChangeType ChangeType { get; set; }

	public RoadProject? Project { get; set; }

	public Guid? ProjectId { get; set; }
}
