namespace RevitAi.Abstractions.Services;

public sealed class CADFileInfo
{
	public string FilePath { get; set; } = string.Empty;

	public string ImportInstanceId { get; set; } = string.Empty;

	public string ImportUnit { get; set; } = string.Empty;

	public double ConversionFactorToMM { get; set; } = 1.0;

	public double ScaleFactor { get; set; } = 1.0;
}
