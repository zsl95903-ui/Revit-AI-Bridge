using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RevitAi.UI.Behaviors;

public class ScrollPreviewEventArgs
{
	public ScrollViewer ScrollViewer { get; set; }

	public double VerticalOffset { get; set; }

	public double ViewportHeight { get; set; }

	public double ExtentHeight { get; set; }

	public IEnumerable<FrameworkElement> GetVisibleChildren()
	{
		if (ScrollViewer == null)
		{
			yield break;
		}
		double verticalOffset = VerticalOffset;
		double num = VerticalOffset + ViewportHeight;
		double preloadTop = Math.Max(0.0, verticalOffset - ViewportHeight);
		double preloadBottom = Math.Min(ExtentHeight, num + ViewportHeight);
		foreach (FrameworkElement item in FindVisualChildren<FrameworkElement>((DependencyObject)(object)ScrollViewer))
		{
			Point val = item.TransformToVisual(ScrollViewer).Transform(new Point(0.0, 0.0));
			if (((Point)val).Y + item.ActualHeight >= preloadTop && ((Point)val).Y <= preloadBottom)
			{
				yield return item;
			}
		}
	}

	private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
	{
		if (parent == null)
		{
			yield break;
		}
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, i);
			T val = (T)(object)((child is T) ? child : null);
			if (val != null)
			{
				yield return val;
			}
			foreach (T item in FindVisualChildren<T>(child))
			{
				yield return item;
			}
		}
	}
}
