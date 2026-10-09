using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RevitAi.UI.Behaviors;

public static class ScrollPreviewBehavior
{
	public static readonly DependencyProperty PreviewLoadCommandProperty;

	public static ICommand GetPreviewLoadCommand(DependencyObject obj)
	{
		return (ICommand)obj.GetValue(PreviewLoadCommandProperty);
	}

	public static void SetPreviewLoadCommand(DependencyObject obj, ICommand value)
	{
		obj.SetValue(PreviewLoadCommandProperty, (object)value);
	}

	private static void OnPreviewLoadCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ScrollViewer scrollViewer)
		{
			scrollViewer.ScrollChanged -= OnScrollChanged;
			scrollViewer.ScrollChanged += OnScrollChanged;
		}
	}

	private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		if (sender is ScrollViewer scrollViewer && e.VerticalChange > 0.0)
		{
			GetPreviewLoadCommand((DependencyObject)(object)scrollViewer)?.Execute(new ScrollPreviewEventArgs
			{
				ScrollViewer = scrollViewer,
				VerticalOffset = e.VerticalOffset,
				ViewportHeight = e.ViewportHeight,
				ExtentHeight = e.ExtentHeight
			});
		}
	}

	static ScrollPreviewBehavior()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		PreviewLoadCommandProperty = DependencyProperty.RegisterAttached("PreviewLoadCommand", typeof(ICommand), typeof(ScrollPreviewBehavior), new PropertyMetadata((object)null, new PropertyChangedCallback(OnPreviewLoadCommandChanged)));
	}
}
