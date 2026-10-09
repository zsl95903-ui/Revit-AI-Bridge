using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.Feedback;
using RevitAi.Core.Feedback.Models;
using RevitAi.UI.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class FeedbackListViewModel : ObservableObject
{
	private readonly FeedbackService _feedbackService;

	private readonly Window? _window;

	[ObservableProperty]
	private ObservableCollection<FeedbackItemViewModel> _feedbacks = new ObservableCollection<FeedbackItemViewModel>();

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? newFeedbackCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand<Guid>? deleteFeedbackCommand;

	public bool HasNoFeedbacks => Feedbacks.Count == 0;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<FeedbackItemViewModel> Feedbacks
	{
		get
		{
			return _feedbacks;
		}
		[MemberNotNull("_feedbacks")]
		set
		{
			if (!EqualityComparer<ObservableCollection<FeedbackItemViewModel>>.Default.Equals(_feedbacks, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Feedbacks);
				_feedbacks = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Feedbacks);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand NewFeedbackCommand => newFeedbackCommand ?? (newFeedbackCommand = new RelayCommand(NewFeedback));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand<Guid> DeleteFeedbackCommand => deleteFeedbackCommand ?? (deleteFeedbackCommand = new AsyncRelayCommand<Guid>(DeleteFeedbackAsync));

	public FeedbackListViewModel(FeedbackService feedbackService, Window? window = null)
	{
		_feedbackService = feedbackService;
		_window = window;
		LoadFeedbacksAsync();
		MarkAllAsReadAsync();
	}

	private async Task LoadFeedbacksAsync()
	{
		try
		{
			Result<List<FeedbackInfo>> result = await _feedbackService.GetFeedbackListAsync();
			if (!result.IsSuccess || result.Value == null)
			{
				return;
			}
			Feedbacks.Clear();
			foreach (FeedbackInfo item in result.Value)
			{
				Feedbacks.Add(new FeedbackItemViewModel(item));
			}
			OnPropertyChanged("HasNoFeedbacks");
		}
		catch (Exception ex)
		{
			Logger.Error("[FeedbackListViewModel] 加载反馈列表异常", ex);
		}
	}

	private async Task MarkAllAsReadAsync()
	{
		try
		{
			await _feedbackService.MarkAllRepliedAsReadAsync();
		}
		catch (Exception ex)
		{
			Logger.Error("[FeedbackListViewModel] 标记已读异常", ex);
		}
	}

	[RelayCommand]
	private void NewFeedback()
	{
		FeedbackSubmitWindow feedbackSubmitWindow = new FeedbackSubmitWindow();
		FeedbackSubmitViewModel dataContext = new FeedbackSubmitViewModel(_feedbackService, feedbackSubmitWindow);
		feedbackSubmitWindow.DataContext = dataContext;
		if (feedbackSubmitWindow.ShowDialog() == true)
		{
			LoadFeedbacksAsync();
		}
	}

	[RelayCommand]
	private async Task DeleteFeedbackAsync(Guid feedbackId)
	{
		if (MessageBox.Show("确定要删除这条反馈吗？此操作无法撤销。", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
		{
			return;
		}
		try
		{
			Result result = await _feedbackService.DeleteFeedbackAsync(feedbackId);
			if (result.IsSuccess)
			{
				FeedbackItemViewModel feedbackItemViewModel = Feedbacks.FirstOrDefault((FeedbackItemViewModel f) => f.Id == feedbackId);
				if (feedbackItemViewModel != null)
				{
					Feedbacks.Remove(feedbackItemViewModel);
					OnPropertyChanged("HasNoFeedbacks");
				}
				MessageBox.Show("删除成功", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			else
			{
				MessageBox.Show("删除失败：" + result.Error, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[FeedbackListViewModel] 删除反馈异常", ex);
			MessageBox.Show("删除异常：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}
}
