using System;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class EnrichedPipeData
{
	public PipeData CADData { get; set; }

	public double StartX { get; set; }

	public double StartY { get; set; }

	public double StartZ { get; set; }

	public double EndX { get; set; }

	public double EndY { get; set; }

	public double EndZ { get; set; }

	public double DiameterMM { get; set; }

	public string? StartManholeId { get; set; }

	public string? EndManholeId { get; set; }

	public EnrichedManholeData? StartManhole { get; set; }

	public EnrichedManholeData? EndManhole { get; set; }

	public bool HasCompleteConnections
	{
		get
		{
			if (StartManhole != null)
			{
				return EndManhole != null;
			}
			return false;
		}
	}

	public double LengthMM => Math.Sqrt(Math.Pow(EndX - StartX, 2.0) + Math.Pow(EndY - StartY, 2.0) + Math.Pow(EndZ - StartZ, 2.0));
}
