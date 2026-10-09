namespace RevitAi.Abstractions.Adapters;

public sealed class CADInstanceInfo
{
	public string ElementId { get; set; } = string.Empty;

	public string FilePath { get; set; } = string.Empty;

	public string FileName { get; set; } = string.Empty;

	public object? ImportInstance { get; set; }
}
