using System;
using System.Windows;
using System.Windows.Threading;

namespace RevitAi.UI.Services;

public static class UserNotificationService
{
	private static ToastNotificationService? _toastService;

	public static ToastNotificationService Toast => _toastService ?? (_toastService = new ToastNotificationService());

	public static void ShowSuccess(string message)
	{
		ShowMessageBox(message, "成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	public static void ShowInfo(string message)
	{
		ShowMessageBox(message, "信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	public static void ShowWarning(string message)
	{
		ShowMessageBox(message, "警告", MessageBoxButton.OK, MessageBoxImage.Exclamation);
	}

	public static void ShowError(string message)
	{
		ShowMessageBox(message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
	}

	public static void ShowRibbonUpdateHint()
	{
		ShowMessageBox("✅ Ribbon 配置已更新！点击任意按钮或切换标签页以查看更改。", "更新完成", MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	public static void ShowFeatureEnabled(string featureName)
	{
		ShowMessageBox("✅ 已启用: " + featureName, "功能已启用", MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	public static void ShowFeatureDisabled(string featureName)
	{
		ShowMessageBox("⏸ 已禁用: " + featureName, "功能已禁用", MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	private static void ShowMessageBox(string message, string title, MessageBoxButton button, MessageBoxImage icon)
	{
		try
		{
			if (Application.Current != null && ((DispatcherObject)Application.Current).Dispatcher != null)
			{
				((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((DispatcherPriority)9, (Delegate)(Action)delegate
				{
					MessageBox.Show(message, title, button, icon);
				});
			}
		}
		catch (Exception)
		{
		}
	}

	public static bool ShowConfirm(string message, string title = "确认")
	{
		return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
	}
}
