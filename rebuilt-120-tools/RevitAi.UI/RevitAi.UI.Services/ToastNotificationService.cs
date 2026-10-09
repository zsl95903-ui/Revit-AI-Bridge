using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using RevitAi.UI.Views.Windows;

namespace RevitAi.UI.Services;

public class ToastNotificationService
{
	private readonly List<ToastNotificationWindow> _activeToasts = new List<ToastNotificationWindow>();

	private const double ToastSpacing = 10.0;

	private const double MarginFromEdge = 20.0;

	public void ShowToast(string title, string message, int displayDuration = 5)
	{
		if (Application.Current == null)
		{
			return;
		}
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			try
			{
				ToastNotificationWindow toast = new ToastNotificationWindow(title, message, displayDuration);
				PositionToast(toast);
				toast.Closed += delegate
				{
					_activeToasts.Remove(toast);
				};
				toast.Show();
				_activeToasts.Add(toast);
			}
			catch (Exception)
			{
			}
		}, Array.Empty<object>());
	}

	public void ShowSuccess(string message, int displayDuration = 3)
	{
		ShowToast("成功", message, displayDuration);
	}

	public void ShowInfo(string message, int displayDuration = 3)
	{
		ShowToast("提示", message, displayDuration);
	}

	public void ShowWarning(string message, int displayDuration = 4)
	{
		ShowToast("警告", message, displayDuration);
	}

	public void ShowError(string message, int displayDuration = 5)
	{
		ShowToast("错误", message, displayDuration);
	}

	private void PositionToast(ToastNotificationWindow toast)
	{
		(double, double)? revitWindowBottomRight = RevitWindowHelper.GetRevitWindowBottomRight();
		if (revitWindowBottomRight.HasValue)
		{
			(double, double) value = revitWindowBottomRight.Value;
			double item = value.Item1;
			double item2 = value.Item2;
			double num = 20.0;
			foreach (ToastNotificationWindow activeToast in _activeToasts)
			{
				if (activeToast.IsLoaded)
				{
					num += activeToast.ActualHeight + 10.0;
				}
			}
			toast.PositionAtBottomRight(item, item2, 20.0, num);
			return;
		}
		Window referenceWindow = GetReferenceWindow();
		if (referenceWindow != null)
		{
			double num2 = 20.0;
			foreach (ToastNotificationWindow activeToast2 in _activeToasts)
			{
				if (activeToast2.IsLoaded)
				{
					num2 += activeToast2.ActualHeight + 10.0;
				}
			}
			double right = referenceWindow.Left + referenceWindow.ActualWidth;
			double bottom = referenceWindow.Top + referenceWindow.ActualHeight;
			toast.PositionAtBottomRight(right, bottom, 20.0, num2);
		}
		else
		{
			var (right2, bottom2) = GetPrimaryScreenWorkingAreaBottomRight();
			toast.PositionAtBottomRight(right2, bottom2);
		}
	}

	private (double Right, double Bottom) GetPrimaryScreenWorkingAreaBottomRight()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Rect workArea = SystemParameters.WorkArea;
			return (Right: ((Rect)workArea).Right, Bottom: ((Rect)workArea).Bottom);
		}
		catch
		{
			if (Application.Current?.MainWindow != null)
			{
				Window mainWindow = Application.Current.MainWindow;
				return (Right: mainWindow.Left + mainWindow.ActualWidth, Bottom: mainWindow.Top + mainWindow.ActualHeight);
			}
			return (Right: 1920.0, Bottom: 1080.0);
		}
	}

	private Window? GetReferenceWindow()
	{
		if (Application.Current.MainWindow != null)
		{
			return Application.Current.MainWindow;
		}
		foreach (Window window in Application.Current.Windows)
		{
			if (window.IsLoaded)
			{
				return window;
			}
		}
		return null;
	}

	public void CloseAll()
	{
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			ToastNotificationWindow[] array = _activeToasts.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Close();
			}
		}, Array.Empty<object>());
	}
}
