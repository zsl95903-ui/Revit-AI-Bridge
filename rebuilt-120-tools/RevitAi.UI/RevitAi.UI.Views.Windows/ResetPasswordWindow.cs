using System;
using System.Windows;
using System.Windows.Markup;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Services;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class ResetPasswordWindow : Window, IComponentConnector
{
	public ResetPasswordWindow()
	{
		try
		{
			Logger.Info("[ResetPasswordWindow] 构造函数开始");
			InitializeComponent();
			Logger.Info("[ResetPasswordWindow] InitializeComponent 完成");
			try
			{
				ResetPasswordViewModel dataContext = new ResetPasswordViewModel(UIBootstrapper.GetService<IAuthManager>(), UIBootstrapper.GetService<IDialogService>(), this);
				base.DataContext = dataContext;
				Logger.Info("[ResetPasswordWindow] DataContext 设置完成");
			}
			catch (Exception ex)
			{
				Logger.Error("[ResetPasswordWindow] 设置 DataContext 失败", ex);
				throw;
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("[ResetPasswordWindow] 构造函数异常", ex2);
			throw;
		}
	}

	public string GetEmail()
	{
		return EmailTextBox?.Text ?? string.Empty;
	}
}
