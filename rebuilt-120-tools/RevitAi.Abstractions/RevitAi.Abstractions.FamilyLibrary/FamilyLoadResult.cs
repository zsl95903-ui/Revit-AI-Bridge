namespace RevitAi.Abstractions.FamilyLibrary;

public class FamilyLoadResult
{
	public bool Success { get; set; }

	public string? FamilyId { get; set; }

	public string? FamilyName { get; set; }

	public string? Error { get; set; }

	public bool AlreadyExists { get; set; }
}
