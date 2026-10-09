namespace RevitAi.Abstractions.Services;

public sealed class VerticalCurveReadConfig
{
	public int DataStartRow { get; set; } = 2;

	public int? PointNumberColumn { get; set; }

	public int StationColumn { get; set; } = 1;

	public int ElevationColumn { get; set; } = 2;

	public int RadiusColumn { get; set; } = 3;

	public int CurveTypeColumn { get; set; } = 4;

	public int? FrontGradientColumn { get; set; } = 5;

	public int? BackGradientColumn { get; set; } = 6;
}
