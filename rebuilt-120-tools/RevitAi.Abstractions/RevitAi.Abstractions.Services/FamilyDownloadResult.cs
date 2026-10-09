namespace RevitAi.Abstractions.Services;

public sealed class FamilyDownloadResult
{
	public bool IsSuccess { get; set; }

	public string? FilePath { get; set; }

	public string? Error { get; set; }

	public static FamilyDownloadResult Ok(string filePath)
	{
		return new FamilyDownloadResult
		{
			IsSuccess = true,
			FilePath = filePath
		};
	}

	public static FamilyDownloadResult Fail(string error)
	{
		return new FamilyDownloadResult
		{
			IsSuccess = false,
			Error = error
		};
	}
}
