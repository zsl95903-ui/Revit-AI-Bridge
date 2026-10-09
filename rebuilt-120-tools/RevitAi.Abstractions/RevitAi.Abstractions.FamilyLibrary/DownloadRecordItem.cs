using System;

namespace RevitAi.Abstractions.FamilyLibrary;

public class DownloadRecordItem
{
	public Guid FamilyId { get; set; }

	public string DownloadTime { get; set; } = string.Empty;
}
