using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.Feedback;
using RevitAi.Core.Feedback.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace RevitAi.UI.ViewModels;

public class FeedbackSubmitViewModel : ObservableObject
{
	private readonly FeedbackService _feedbackService;

	private readonly Window? _window;

	[ObservableProperty]
	private string _title = string.Empty;

	[ObservableProperty]
	private string _content = string.Empty;

	[ObservableProperty]
	private string _email = string.Empty;

	[ObservableProperty]
	private ObservableCollection<AttachmentViewModel> _attachments = new ObservableCollection<AttachmentViewModel>();

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? browseFilesCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<AttachmentViewModel?>? removeAttachmentCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? submitCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelCommand;

	public bool HasNoAttachments => Attachments.Count == 0;

	public bool HasAttachments => Attachments.Count > 0;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Title
	{
		get
		{
			return _title;
		}
		[MemberNotNull("_title")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_title, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Title);
				_title = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Title);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Content
	{
		get
		{
			return _content;
		}
		[MemberNotNull("_content")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_content, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Content);
				_content = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Content);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Email
	{
		get
		{
			return _email;
		}
		[MemberNotNull("_email")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_email, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Email);
				_email = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Email);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<AttachmentViewModel> Attachments
	{
		get
		{
			return _attachments;
		}
		[MemberNotNull("_attachments")]
		set
		{
			if (!EqualityComparer<ObservableCollection<AttachmentViewModel>>.Default.Equals(_attachments, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Attachments);
				_attachments = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Attachments);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand BrowseFilesCommand => browseFilesCommand ?? (browseFilesCommand = new RelayCommand(BrowseFiles));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<AttachmentViewModel?> RemoveAttachmentCommand => removeAttachmentCommand ?? (removeAttachmentCommand = new RelayCommand<AttachmentViewModel>(RemoveAttachment));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SubmitCommand => submitCommand ?? (submitCommand = new AsyncRelayCommand(SubmitAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	public FeedbackSubmitViewModel(FeedbackService feedbackService, Window? window = null)
	{
		_feedbackService = feedbackService;
		_window = window;
		Attachments.CollectionChanged += delegate
		{
			OnPropertyChanged("HasNoAttachments");
			OnPropertyChanged("HasAttachments");
		};
	}

	[RelayCommand]
	private void BrowseFiles()
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Multiselect = true,
			Filter = "所有文件|*.*|图片|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp|文档|*.pdf;*.doc;*.docx;*.txt;*.zip|日志|*.log"
		};
		if (openFileDialog.ShowDialog() == true)
		{
			string[] fileNames = openFileDialog.FileNames;
			foreach (string filePath in fileNames)
			{
				AddAttachmentFromFile(filePath);
			}
		}
	}

	public void AddAttachmentFromFile(string filePath)
	{
		try
		{
			if (!File.Exists(filePath))
			{
				Logger.Warning("[FeedbackSubmitViewModel] 文件不存在: " + filePath);
				return;
			}
			FileInfo fileInfo = new FileInfo(filePath);
			if (fileInfo.Length > 10485760)
			{
				MessageBox.Show("文件 " + fileInfo.Name + " 超过 10MB 限制，已跳过。", "文件过大", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			Attachments.Add(new AttachmentViewModel
			{
				FileName = fileInfo.Name,
				FilePath = filePath,
				FileSize = fileInfo.Length
			});
		}
		catch (Exception ex)
		{
			Logger.Error("[FeedbackSubmitViewModel] 添加附件失败: " + filePath, ex);
		}
	}

	[RelayCommand]
	private void RemoveAttachment(AttachmentViewModel? attachment)
	{
		if (attachment != null)
		{
			Attachments.Remove(attachment);
		}
	}

	[RelayCommand]
	private async Task SubmitAsync()
	{
		if (string.IsNullOrWhiteSpace(Title))
		{
			MessageBox.Show("请输入反馈标题", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
		if (string.IsNullOrWhiteSpace(Content))
		{
			MessageBox.Show("请输入问题描述", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
		try
		{
			SubmitFeedbackRequest request = new SubmitFeedbackRequest
			{
				Title = Title.Trim(),
				Content = Content.Trim(),
				Email = (string.IsNullOrWhiteSpace(Email) ? null : Email.Trim()),
				AttachmentPaths = Attachments.Select((AttachmentViewModel a) => a.FilePath).ToList()
			};
			Result<Guid> result = await _feedbackService.SubmitFeedbackAsync(request);
			if (result.IsSuccess)
			{
				MessageBox.Show("感谢您的反馈！我们会尽快处理。", "提交成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				if (_window != null)
				{
					_window.DialogResult = true;
					_window.Close();
				}
			}
			else
			{
				MessageBox.Show("提交失败：" + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[FeedbackSubmitViewModel] 提交反馈异常", ex);
			MessageBox.Show("提交异常：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void Cancel()
	{
		if (_window != null)
		{
			_window.DialogResult = false;
			_window.Close();
		}
	}
}
