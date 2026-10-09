namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class PipeNetworkModelingOptions
{
	public string ManholeFamilyFilePath { get; set; } = string.Empty;

	public string ManholeFamilyName { get; set; } = "AST_R_检查井";

	public string ManholeSymbolName { get; set; } = "标准";

	public string PipeSystemName { get; set; } = "排水管";

	public double DefaultGroundElevationM { get; set; }

	public double DefaultPipeBottomElevationM { get; set; } = -1.5;

	public double DefaultWellDepthM { get; set; } = 2.0;

	public double DefaultPipeDiameterMM { get; set; } = 300.0;

	public bool UseNeighborDataForMissing { get; set; } = true;

	public double MaxNeighborSearchDistanceMM { get; set; } = 50000.0;
}
