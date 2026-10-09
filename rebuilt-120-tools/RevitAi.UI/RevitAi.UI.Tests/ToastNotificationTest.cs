using System.Threading;
using RevitAi.UI.Services;

namespace RevitAi.UI.Tests;

public static class ToastNotificationTest
{
	public static void TestBasicToasts()
	{
		UserNotificationService.Toast.ShowSuccess("操作成功完成");
		Thread.Sleep(1000);
		UserNotificationService.Toast.ShowInfo("这是一条信息提示");
		Thread.Sleep(1000);
		UserNotificationService.Toast.ShowWarning("请注意检查参数设置");
		Thread.Sleep(1000);
		UserNotificationService.Toast.ShowError("操作失败，请重试");
	}

	public static void TestCustomToasts()
	{
		UserNotificationService.Toast.ShowToast("AI 执行反馈", "成功识别并处理了 50 个管道构件", 3);
		UserNotificationService.Toast.ShowToast("长时间任务", "正在导出大量数据，请稍候...", 10);
		UserNotificationService.Toast.ShowToast("快速提示", "配置已保存", 2);
	}

	public static void TestStackedToasts()
	{
		for (int i = 1; i <= 5; i++)
		{
			UserNotificationService.Toast.ShowInfo($"通知 #{i}");
			Thread.Sleep(500);
		}
	}

	public static void TestBatchProgress()
	{
		int num = 100;
		for (int i = 1; i <= num; i++)
		{
			Thread.Sleep(50);
			if (i % 20 == 0)
			{
				UserNotificationService.Toast.ShowInfo($"已处理 {i}/{num} 个元素");
			}
		}
		UserNotificationService.Toast.ShowSuccess($"批量处理完成，共 {num} 个元素");
	}

	public static void TestCloseAll()
	{
		for (int i = 1; i <= 5; i++)
		{
			UserNotificationService.Toast.ShowInfo($"通知 #{i}");
			Thread.Sleep(200);
		}
		Thread.Sleep(2000);
		UserNotificationService.Toast.CloseAll();
	}

	public static void TestDurations()
	{
		UserNotificationService.Toast.ShowToast("1 秒", "这条消息显示 1 秒", 1);
		Thread.Sleep(1200);
		UserNotificationService.Toast.ShowToast("3 秒", "这条消息显示 3 秒（默认）", 3);
		Thread.Sleep(3200);
		UserNotificationService.Toast.ShowToast("5 秒", "这条消息显示 5 秒");
		Thread.Sleep(5200);
		UserNotificationService.Toast.ShowToast("10 秒", "这条消息显示 10 秒", 10);
	}
}
