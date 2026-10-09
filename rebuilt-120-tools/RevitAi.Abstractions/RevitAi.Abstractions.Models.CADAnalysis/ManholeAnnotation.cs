namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class ManholeAnnotation
{
	public string Content { get; set; } = string.Empty;

	public string Type { get; set; } = string.Empty;

	public string? LineGroupId { get; set; }

	public string? TextId { get; set; }

	public double Distance { get; set; }

	public ManholeAnnotation()
	{
	}

	public ManholeAnnotation(string content, string type, double distance = 0.0)
	{
		Content = content;
		Type = type;
		Distance = distance;
	}

	public ManholeAnnotation Clone()
	{
		return new ManholeAnnotation
		{
			Content = Content,
			Type = Type,
			LineGroupId = LineGroupId,
			TextId = TextId,
			Distance = Distance
		};
	}
}
