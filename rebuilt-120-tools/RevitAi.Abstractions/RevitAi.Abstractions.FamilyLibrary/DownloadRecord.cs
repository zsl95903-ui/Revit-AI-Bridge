using System.Collections.Generic;

namespace RevitAi.Abstractions.FamilyLibrary;

public class DownloadRecord
{
	public string WindowStartTime { get; set; } = string.Empty;

	public List<DownloadRecordItem> Downloads { get; set; } = new List<DownloadRecordItem>();
}
