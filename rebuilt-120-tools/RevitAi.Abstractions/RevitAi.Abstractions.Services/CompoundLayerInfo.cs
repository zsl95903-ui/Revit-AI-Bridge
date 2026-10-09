namespace RevitAi.Abstractions.Services;

public class CompoundLayerInfo
{
	public int Index { get; set; }

	public double WidthMM { get; set; }

	public string Function { get; set; } = string.Empty;

	public string FunctionName { get; set; } = string.Empty;

	public int? MaterialId { get; set; }

	public string? MaterialName { get; set; }

	public bool IsCore { get; set; }

	public bool ParticipatesInWrapping { get; set; }
}
