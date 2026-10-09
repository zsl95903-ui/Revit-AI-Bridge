using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using RevitAi.Core.Feedback.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAi.UI.ViewModels;

public class FeedbackItemViewModel : ObservableObject
{
	private readonly FeedbackInfo _feedback;

	public Guid Id => _feedback.Id;

	public string Title => _feedback.Title;

	public string ContentPreview
	{
		get
		{
			if (_feedback.Content.Length <= 150)
			{
				return _feedback.Content;
			}
			return _feedback.Content.Substring(0, 150) + "...";
		}
	}

	public string StatusText
	{
		get
		{
			if (!(_feedback.Status == "replied"))
			{
				return "待处理";
			}
			return "已回复";
		}
	}

	public string StatusIcon
	{
		get
		{
			if (!(_feedback.Status == "replied"))
			{
				return "⏱";
			}
			return "✓";
		}
	}

	public Brush StatusBrush
	{
		get
		{
			if (!(_feedback.Status == "replied"))
			{
				return new SolidColorBrush(Color.FromRgb(byte.MaxValue, 152, 0));
			}
			return new SolidColorBrush(Color.FromRgb(76, 175, 80));
		}
	}

	public string CreatedAtText => $"提交时间：{_feedback.CreatedAt:yyyy-MM-dd HH:mm}";

	public bool HasReply
	{
		get
		{
			if (_feedback.Status == "replied")
			{
				return !string.IsNullOrEmpty(_feedback.AdminReply);
			}
			return false;
		}
	}

	public string? AdminReply => _feedback.AdminReply;

	public string? AdminReplyAtText
	{
		get
		{
			if (!_feedback.AdminReplyAt.HasValue)
			{
				return null;
			}
			return $"回复时间：{_feedback.AdminReplyAt:yyyy-MM-dd HH:mm}";
		}
	}

	public bool HasAttachments
	{
		get
		{
			if (Attachments != null)
			{
				return Attachments.Count > 0;
			}
			return false;
		}
	}

	public ObservableCollection<AttachmentDisplayViewModel>? Attachments { get; private set; }

	public FeedbackItemViewModel(FeedbackInfo feedback)
	{
		_feedback = feedback;
		if (feedback.Attachments == null || feedback.Attachments.Count <= 0)
		{
			return;
		}
		List<AttachmentDisplayViewModel> list = new List<AttachmentDisplayViewModel>();
		foreach (FeedbackAttachment attachment in feedback.Attachments)
		{
			AttachmentDisplayViewModel attachmentDisplayViewModel = new AttachmentDisplayViewModel(attachment);
			attachmentDisplayViewModel.InitializeProperties();
			list.Add(attachmentDisplayViewModel);
		}
		Attachments = new ObservableCollection<AttachmentDisplayViewModel>(list);
	}
}
