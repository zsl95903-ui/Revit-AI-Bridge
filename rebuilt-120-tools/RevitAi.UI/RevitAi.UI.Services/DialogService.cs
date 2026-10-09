using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RevitAi.UI.Services;

public class DialogService : IDialogService
{
	public void ShowInfo(string message, string title = "信息")
	{
		MessageBox.Show((Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.Topmost) ?? Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsActive)) ?? Application.Current?.MainWindow, message, title, MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	public Task ShowInfoAsync(string message, string title = "信息")
	{
		return Task.Run(delegate
		{
			Application current = Application.Current;
			if (current != null)
			{
				((DispatcherObject)current).Dispatcher.Invoke((Action)delegate
				{
					ShowInfo(message, title);
				});
			}
		});
	}

	public void ShowWarning(string message, string title = "警告")
	{
		MessageBox.Show((Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.Topmost) ?? Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsActive)) ?? Application.Current?.MainWindow, message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
	}

	public Task ShowWarningAsync(string message, string title = "警告")
	{
		return Task.Run(delegate
		{
			Application current = Application.Current;
			if (current != null)
			{
				((DispatcherObject)current).Dispatcher.Invoke((Action)delegate
				{
					ShowWarning(message, title);
				});
			}
		});
	}

	public void ShowError(string message, string title = "错误")
	{
		MessageBox.Show((Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.Topmost) ?? Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsActive)) ?? Application.Current?.MainWindow, message, title, MessageBoxButton.OK, MessageBoxImage.Hand);
	}

	public Task ShowErrorAsync(string message, string title = "错误")
	{
		return Task.Run(delegate
		{
			Application current = Application.Current;
			if (current != null)
			{
				((DispatcherObject)current).Dispatcher.Invoke((Action)delegate
				{
					ShowError(message, title);
				});
			}
		});
	}

	public bool ShowConfirm(string message, string title = "确认")
	{
		return MessageBox.Show((Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.Topmost) ?? Application.Current?.Windows.OfType<Window>().FirstOrDefault((Window w) => w.IsActive)) ?? Application.Current?.MainWindow, message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
	}

	public Task<bool> ShowConfirmAsync(string message, string title = "确认")
	{
		return Task.Run(delegate
		{
			Application current = Application.Current;
			return current != null && ((DispatcherObject)current).Dispatcher.Invoke<bool>((Func<bool>)(() => ShowConfirm(message, title)));
		});
	}
}
