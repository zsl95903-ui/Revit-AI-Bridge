namespace RevitAi.Abstractions.Models;

public class SelectedElementInfo
{
	public int Id { get; set; }

	public string ElementType { get; set; } = "";

	public string SystemName { get; set; } = "";

	public string Size { get; set; } = "";

	public bool IsPipeRelated { get; set; }
}
