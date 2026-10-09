using System.Threading.Tasks;

namespace RevitAi.UI.Services;

public interface IDialogService
{
	void ShowInfo(string message, string title = "信息");

	Task ShowInfoAsync(string message, string title = "信息");

	void ShowWarning(string message, string title = "警告");

	Task ShowWarningAsync(string message, string title = "警告");

	void ShowError(string message, string title = "错误");

	Task ShowErrorAsync(string message, string title = "错误");

	bool ShowConfirm(string message, string title = "确认");

	Task<bool> ShowConfirmAsync(string message, string title = "确认");
}
