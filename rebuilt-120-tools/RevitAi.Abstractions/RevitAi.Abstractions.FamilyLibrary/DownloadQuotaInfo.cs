namespace RevitAi.Abstractions.FamilyLibrary;

public class DownloadQuotaInfo
{
	public int DailyLimit { get; set; }

	public int DownloadedToday { get; set; }

	public int Remaining { get; set; }

	public int DaysRemaining { get; set; }

	public string UserType { get; set; } = "trial";
}
