using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class FeedbackSubmitWindow : Window, IComponentConnector
{
	public FeedbackSubmitWindow()
	{
		InitializeComponent();
		base.AllowDrop = true;
		base.DragEnter += OnDragEnter;
		base.DragOver += OnDragEnter;
		base.DragLeave += OnDragLeave;
		base.Drop += OnDrop;
	}

	private void OnDragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			e.Effects = DragDropEffects.Copy;
		}
		else
		{
			e.Effects = DragDropEffects.None;
		}
		e.Handled = true;
	}

	private void OnDragLeave(object sender, DragEventArgs e)
	{
		e.Handled = true;
	}

	private void OnDrop(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Data.GetData(DataFormats.FileDrop) is string[] array && base.DataContext is FeedbackSubmitViewModel feedbackSubmitViewModel)
		{
			string[] array2 = array;
			foreach (string filePath in array2)
			{
				feedbackSubmitViewModel.AddAttachmentFromFile(filePath);
			}
		}
		e.Handled = true;
	}

	private void OnBorderClick(object sender, MouseButtonEventArgs e)
	{
		if (base.DataContext is FeedbackSubmitViewModel feedbackSubmitViewModel)
		{
			feedbackSubmitViewModel.BrowseFilesCommand.Execute(null);
		}
		e.Handled = true;
	}
}
