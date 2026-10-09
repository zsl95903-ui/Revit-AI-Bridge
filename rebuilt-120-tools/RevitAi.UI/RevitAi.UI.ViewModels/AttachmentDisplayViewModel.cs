using RevitAi.Core.Feedback.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAi.UI.ViewModels;

public class AttachmentDisplayViewModel : ObservableObject
{
	private readonly FeedbackAttachment _attachment;

	private string _fileName = string.Empty;

	private string _publicUrl = string.Empty;

	private string _fileSizeText = string.Empty;

	public string FileName
	{
		get
		{
			return _fileName;
		}
		private set
		{
			SetProperty(ref _fileName, value, "FileName");
		}
	}

	public string PublicUrl
	{
		get
		{
			return _publicUrl;
		}
		private set
		{
			SetProperty(ref _publicUrl, value, "PublicUrl");
		}
	}

	public string FileSizeText
	{
		get
		{
			return _fileSizeText;
		}
		private set
		{
			SetProperty(ref _fileSizeText, value, "FileSizeText");
		}
	}

	public AttachmentDisplayViewModel(FeedbackAttachment attachment)
	{
		_attachment = attachment;
	}

	public void InitializeProperties()
	{
		FileName = _attachment.FileName;
		PublicUrl = _attachment.PublicUrl;
		FileSizeText = FormatFileSize(_attachment.FileSize);
	}

	private string FormatFileSize(long bytes)
	{
		if (bytes >= 1024)
		{
			if (bytes < 1048576)
			{
				return $"{(double)bytes / 1024.0:F1} KB";
			}
			return $"{(double)bytes / 1048576.0:F1} MB";
		}
		return $"{bytes} B";
	}
}
