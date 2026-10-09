using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using RevitAi.Abstractions.Logging;

namespace RevitAi.UI.Views.Windows;

public partial class SignUpWindow : Window, IComponentConnector
{
	public SignUpWindow()
	{
		try
		{
			Logger.Info("[SignUpWindow] 构造函数开始");
			InitializeComponent();
			Logger.Info("[SignUpWindow] InitializeComponent 完成");
		}
		catch (Exception ex)
		{
			Logger.Error("[SignUpWindow] 构造函数异常", ex);
			throw;
		}
	}

	public string? GetPassword()
	{
		return PasswordBox?.Password;
	}

	public string? GetConfirmPassword()
	{
		return ConfirmPasswordBox?.Password;
	}

	public string GetEmail()
	{
		return EmailTextBox?.Text ?? string.Empty;
	}

	private void OnForgotPasswordClick(object sender, RequestNavigateEventArgs e)
	{
		try
		{
			Logger.Info("[SignUpWindow] 忘记密码链接被点击");
			if (MessageBox.Show("关闭注册窗口，打开重置密码窗口？", "切换窗口", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				string email = GetEmail();
				base.DialogResult = false;
				Close();
				ResetPasswordWindow resetPasswordWindow = new ResetPasswordWindow();
				if (!string.IsNullOrWhiteSpace(email) && resetPasswordWindow.EmailTextBox != null)
				{
					resetPasswordWindow.EmailTextBox.Text = email;
				}
				resetPasswordWindow.ShowDialog();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[SignUpWindow] 打开重置密码窗口失败", ex);
			MessageBox.Show("打开重置密码窗口失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}
}
