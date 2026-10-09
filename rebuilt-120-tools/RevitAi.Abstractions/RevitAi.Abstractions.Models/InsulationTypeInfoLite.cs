namespace RevitAi.Abstractions.Models;

public class InsulationTypeInfoLite
{
	public int Id { get; set; }

	public string Name { get; set; } = "";

	public SystemElementType ElementType { get; set; }
}
