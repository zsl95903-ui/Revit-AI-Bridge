using System.Collections.Generic;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class CADAnalysisOptions
{
	public List<string> ManholeLayerNames { get; set; } = new List<string>();

	public List<string> PipeLayerNames { get; set; } = new List<string>();

	public List<string> ManholeAnnotationLayerNames { get; set; } = new List<string>();

	public List<string> PipeAnnotationLayerNames { get; set; } = new List<string>();

	public double LineConnectionToleranceMM { get; set; } = 10.0;

	public double TextToLineMaxDistanceMM { get; set; } = 500.0;

	public double TextToManholeMaxDistanceMM { get; set; } = 1000.0;

	public double LineEndToManholeMaxDistanceMM { get; set; } = 500.0;

	public double DirectionAngleToleranceDegrees { get; set; } = 15.0;

	public double PipeToManholeConnectionToleranceMM { get; set; } = 100.0;
}
