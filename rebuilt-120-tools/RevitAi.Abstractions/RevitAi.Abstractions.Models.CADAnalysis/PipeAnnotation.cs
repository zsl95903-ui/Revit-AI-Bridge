namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class PipeAnnotation
{
	public string Content { get; set; } = string.Empty;

	public string Type { get; set; } = string.Empty;

	public string? LineGroupId { get; set; }

	public string? TextId { get; set; }

	public double Distance { get; set; }

	public PipeAnnotation()
	{
	}

	public PipeAnnotation(string content, string type, double distance = 0.0)
	{
		Content = content;
		Type = type;
		Distance = distance;
	}

	public double? ExtractDiameter()
	{
		return PipeData.ExtractDiameterFromAnnotation(Content);
	}

	public PipeAnnotation Clone()
	{
		return new PipeAnnotation
		{
			Content = Content,
			Type = Type,
			LineGroupId = LineGroupId,
			TextId = TextId,
			Distance = Distance
		};
	}
}
