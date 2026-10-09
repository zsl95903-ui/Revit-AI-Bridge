namespace RevitAi.Abstractions.Models;

public class InsulationSystemInfo
{
	public int SystemTypeId { get; set; }

	public string SystemTypeName { get; set; } = "";

	public int ElementCount { get; set; }

	public int InsulatedCount { get; set; }

	public int UninsulatedCount => ElementCount - InsulatedCount;
}
