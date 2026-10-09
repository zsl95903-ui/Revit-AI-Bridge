using System;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class AncillaryStructureLayoutSegmentData
{
	public Guid SegmentId { get; set; } = Guid.NewGuid();

	public double StartStationKm { get; set; }

	public double EndStationKm { get; set; }

	public double ModelLength { get; set; } = 5.0;
}
