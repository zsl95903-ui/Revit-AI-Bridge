using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAi.UI.ViewModels;

public class AttachmentViewModel : ObservableObject
{
	public string FileName { get; set; } = string.Empty;

	public string FilePath { get; set; } = string.Empty;

	public long FileSize { get; set; }

	public string FileSizeText
	{
		get
		{
			long fileSize = FileSize;
			if (fileSize >= 1024)
			{
				if (fileSize < 1048576)
				{
					return $"{(double)FileSize / 1024.0:F1} KB";
				}
				return $"{(double)FileSize / 1048576.0:F1} MB";
			}
			return $"{FileSize} B";
		}
	}
}
