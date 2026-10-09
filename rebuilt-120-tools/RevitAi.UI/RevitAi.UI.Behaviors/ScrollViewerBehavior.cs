using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RevitAi.UI.Behaviors;

public static class ScrollViewerBehavior
{
	public static readonly DependencyProperty PropagateMouseWheelProperty;

	public static bool GetPropagateMouseWheel(DependencyObject obj)
	{
		return (bool)obj.GetValue(PropagateMouseWheelProperty);
	}

	public static void SetPropagateMouseWheel(DependencyObject obj, bool value)
	{
		obj.SetValue(PropagateMouseWheelProperty, (object)value);
	}

	private static void OnPropagateMouseWheelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextBox textBox)
		{
			if ((bool)((DependencyPropertyChangedEventArgs)e).NewValue)
			{
				textBox.PreviewMouseWheel += OnTextBoxPreviewMouseWheel;
			}
			else
			{
				textBox.PreviewMouseWheel -= OnTextBoxPreviewMouseWheel;
			}
		}
	}

	private static void OnTextBoxPreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		if (sender is TextBox obj)
		{
			ScrollViewer scrollViewer = FindParentScrollViewer((DependencyObject?)(object)obj);
			if (scrollViewer != null && scrollViewer.ExtentHeight > scrollViewer.ViewportHeight)
			{
				e.Handled = true;
				MouseWheelEventArgs e2 = new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, e.Delta)
				{
					RoutedEvent = UIElement.MouseWheelEvent,
					Source = sender
				};
				scrollViewer.RaiseEvent(e2);
			}
		}
	}

	private static ScrollViewer? FindParentScrollViewer(DependencyObject? obj)
	{
		if (obj == null)
		{
			return null;
		}
		DependencyObject parent = VisualTreeHelper.GetParent(obj);
		if (parent == null)
		{
			return null;
		}
		if (parent is ScrollViewer result)
		{
			return result;
		}
		return FindParentScrollViewer(parent);
	}

	static ScrollViewerBehavior()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		PropagateMouseWheelProperty = DependencyProperty.RegisterAttached("PropagateMouseWheel", typeof(bool), typeof(ScrollViewerBehavior), new PropertyMetadata((object)false, new PropertyChangedCallback(OnPropagateMouseWheelChanged)));
	}
}
